using Newtonsoft.Json;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static ParserService.Models.GameModel.ModelGame;
using Newtonsoft.Json;
using Polly;
using Business.Data.Models;
using ParserService.Utils.Helper;
using Product = Business.Data.Models.Product;
namespace ParserService.Service
{
    public class ParserAdapter
    {

        private readonly Dictionary<string, dynamic> _parsers;

     
        public ParserAdapter(Dictionary<string, dynamic> parsers)
        {
            _parsers = parsers;
        }
        /// <summary>
        /// Парсит одну страницу 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parserKey"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<IEnumerable<T>> ParseAsync<T>(string parserKey, string url) where T:class
        {

            if (_parsers.ContainsKey(parserKey))
            {
                //HttpClient httpClient = HttpClientFactory.CreateClientUa();
               
                //var parser = _parsers[parserKey] as IParser<T>;
                //if (parser != null)
                //{
                //    return await parser.ParseAsync(url, httpClient);
                //}
            }

            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }

        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task<IEnumerable<T>> ParseMultiplePagesAsync<T>(string parserKey, IEnumerable<string>urls) where T : class
        {
            
            var results = new ConcurrentBag<T>();
            var processedUrls = new HashSet<string>(); // Для отслеживания уникальных URL
            await Parallel.ForEachAsync(urls, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async (url, cancellationToken) =>
            {


                // Проверяем, был ли этот URL уже обработан
                if (processedUrls.Contains(url))
                {
                    Logger.Log($"Duplicate URL detected: {url}");
                    return;
                }
                // Добавляем URL в множество обработанных
                if (!processedUrls.Add(url))
                {
                    Logger.Log($"Failed to add URL to processed list: {url}");
                    return;
                }

                try
                {
                    var pageResults = await ParseAsync<T>(parserKey, url);
                    if (pageResults != null)
                    {
                        foreach (var pageResult in pageResults)
                        {
                            results.Add(pageResult);
                        }
                    }
                    else
                    {
                        Logger.Log("Eerror null" + pageResults);

                    }
                }
                catch(Exception ex)
                {
                    Logger.Log(ex.Message );
                }
              



            });


            return results.AsEnumerable();
        }


        public async Task<T> ParseJsonAsync<T>(string parserKey, string url, string conceptId,HttpClient httpClient, HttpClient httpClientTr) where T:class
        {

            int retryCount = 3; // Количество повторных попыток
            int delayMilliseconds = 500; // Задержка между попытками
            for(int attempt =0; attempt < retryCount; attempt++)
            {
                try
                {
                    if (_parsers.ContainsKey(parserKey))
                    {
                        
                       
                        //httpClient.Timeout = TimeSpan.FromSeconds(30); // Увеличиваем таймаут
                        var parser = _parsers[parserKey] as IParser<T>;
                        if (parser != null)
                        {
                            return await parser.ParseAsyncJson(url, conceptId, httpClient, httpClientTr);
                        }
                    }
                }
                catch (Exception ex) when (ex is SocketException || ex is HttpRequestException)
                {
                    Logger.Log($"Attempt {attempt + 1} failed: {ex.Message}");
                    if (attempt == retryCount - 1) throw; // Если последняя попытка, выбрасываем исключение
                    await Task.Delay(delayMilliseconds); // Ждём перед следующей попыткой
                }
                
            }
            //if (_parsers.ContainsKey(parserKey))
            //{
            //    HttpClient httpClient = HttpClientFactory.CreateClientTR();

            //    var parser = _parsers[parserKey] as IParser<T>;
            //    if (parser != null)
            //    {
            //        return await parser.ParseAsyncJson(url, conceptId, httpClient);
            //    }
            //}

            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }


        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task ParseMultipleJsonAsync<T>(string parserKey,Dictionary<string,string> urls, 
            string outputPath, HttpClient httpClient, HttpClient httpClientTr, int batchSize = 100) where T : class
        {

                var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Экспоненциальная задержка
            Logger.Log(Convert.ToString(Environment.ProcessorCount));

            var processedUrls = new ConcurrentDictionary<string, byte>(); // Потокобезопасный словарь
            var dataBuffer = new BlockingCollection<DataGame>();
            var writeSemaphore = new SemaphoreSlim(1, 1);
            var batchCounter = 0;
            // Задача для фоновой записи
            var writeTask = Task.Run(async () =>
            {
                while (!dataBuffer.IsCompleted)
                {
                    var batch = new List<DataGame>();
                    while (batch.Count < batchSize && !dataBuffer.IsCompleted)
                    {
                        try
                        {
                            batch.Add(dataBuffer.Take());
                        }
                        catch (Exception ex) { Logger.Log(ex.Message); }
                    }

                    if (batch.Count > 0)
                    {
                        await writeSemaphore.WaitAsync();
                        try
                        {
                            await AppendToJsonFile(outputPath, batch);
                            batchCounter += batch.Count;
                            Logger.Log($"Written {batchCounter} items total");
                        }
                        finally
                        {
                            writeSemaphore.Release();
                        }
                    }
                }
            });
            await Parallel.ForEachAsync(urls, new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = CancellationToken.None
            }, async (url, cancellationToken) =>
            {
                if (processedUrls.TryAdd(url.Key, 0))
                {
                    var randomDelay = TimeSpan.FromMilliseconds(new Random().Next(500, 2000));
                   
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var proxyUrl = ProxyHelper.Proxies;
                        var proxyManager = new ProxyManager(proxyUrl);
                        var proxy = await proxyManager.GetProxyAsync();
                        try
                        {
                          
                          var result = await ParseJsonAsync<DataGame>(
                          parserKey,
                          url.Value,
                          url.Key, httpClient, httpClientTr);
                            await Task.Delay(randomDelay);
                            if (result != null)
                            {
                                dataBuffer.Add(result);
                            }
                        }
                        finally
                        {
                            proxyManager.ReleaseProxy(proxy);

                        }

                    });
                }
                else
                {
                    Logger.Log($"Duplicate URL detected: {url.Key}");
                }
            });

            await writeTask;

            Logger.Log("All data processed and written");
            //await Parallel.ForEachAsync(urls, new ParallelOptions { MaxDegreeOfParallelism = 6 }, async (url, cancellationToken) =>
            //{
            //    var pageResults = await ParseJsonAsync<Rootobject>(parserKey, url.Value,url.Key);

            //    // Проверяем, был ли этот URL уже обработан
            //    if (processedUrls.Contains(url.Key))
            //    {
            //        Logger.Log($"Duplicate URL detected: {url}");
            //        return;
            //    }
            //    // Добавляем URL в множество обработанных
            //    if (!processedUrls.Add(url.Key))
            //    {
            //        Logger.Log($"Failed to add URL to processed list: {url}");
            //        return;
            //    }
            //    if (pageResults != null) results.Add(pageResults);



            //});


        }

        private async Task AppendToJsonFile(string path, List<DataGame> batch)
        {
            try
            {
                var games = new List<Game>();

                foreach (var p in batch)
                {
                    // Проверка на null для основных объектов
                    if (p?.dataUa?.data.conceptRetrieve == null || p?.dataUa?.data.conceptRetrieve.products == null)
                        continue;
                    if (p?.dataTr?.data.conceptRetrieve == null || p?.dataTr?.data?.conceptRetrieve.products == null)
                        continue;
                    var conceptId = p.dataUa.data.conceptRetrieve.id ?? string.Empty;
                    var name = p.dataUa.data.conceptRetrieve.name ?? string.Empty;
                    var editions = new List<Business.Data.Models.Edition>();


                    foreach (var p1 in p.dataUa.data.conceptRetrieve.products)
                    {
                        var webcast = p1.webctas.FirstOrDefault();
                        var webcastTr = p.dataTr.data.conceptRetrieve.products.Where(p => p.id == p1.id).Select(p => p.webctas.FirstOrDefault()).FirstOrDefault();
                        // Проверка на null для webctas и edition
                        if (webcast == null)
                            continue;
                        if (webcastTr == null)
                            continue;


                        bool hasValidWebcta = false;
                        // Проверка на null для price

                        if (webcast.price != null && webcast.price.isFree != true && webcast.price.basePrice != null)
                        {
                            hasValidWebcta = true;

                        }

                        if (webcastTr.price != null && webcastTr.price.isFree != true && webcastTr.price.basePrice != null)
                        {
                            hasValidWebcta = true;

                        }

                        if (!hasValidWebcta)
                            continue;

                        // Создание Edition
                        var edition = new Business.Data.Models.Edition
                        {
                            Guid = Guid.NewGuid(),
                            CusaCode = p1.id ?? string.Empty,
                            Type = "Game",
                            EditionName = p1.name ?? string.Empty,
                            EditionType = p1.edition != null ? p1.edition.name ?? string.Empty : string.Empty,
                            Geners = p1.localizedGenres != null ? string.Join("|", p1.localizedGenres.Select(l => l.value)) : string.Empty,
                            Image = GetImageUrl(p1.media),
                            Features = p1.edition != null ? p1.edition.features != null ? string.Join("|", p1.edition.features) : string.Empty: string.Empty,
                            Platform = p1.platforms != null ? string.Join("|", p1.platforms) : string.Empty,
                            CodeRegion = GetCurrencyCode(webcast) + "|" + GetCurrencyCode(webcastTr),
                            OrderType = webcast.type,

                        };

                        var product = new Product()
                        {
                            TypeId = edition.Guid,
                            Type = "Game",
                            PriceUa = webcast.price.basePriceValue / 100m ?? 0,
                            DiscountUa = webcast.price.discountedValue / 100m ?? 0,
                            PriceTr = webcastTr.price.basePriceValue / 100m ?? 0,
                            DiscountTr = webcastTr.price.discountedValue / 100m ?? 0,
                            DiscountPercent = webcast.price.discountText ?? string.Empty,

                        };

                        if (webcast.price.endTime != null)
                        {
                            if (long.TryParse(webcast.price.endTime.ToString(), out long unixTimestampMs))
                            {
                                // Преобразуем Unix-время в DateTime
                                DateTime utcTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestampMs).UtcDateTime;
                               product.DiscountDate = utcTime.AddHours(3);
                            }
                            
                        }
                        edition.Product = product;
                        edition.ProductId = product.Guid;
                        editions.Add(edition);
                    }

                    // Добавление Game в список
                    if (editions.Count > 0)
                    {
                        games.Add(new Game
                        {
                            ConceptId = conceptId,
                            Name = name,
                            Editions = editions
                        });
                    }
                }



                var jsonData = JsonConvert.SerializeObject(games, Newtonsoft.Json.Formatting.Indented);

                // Если файл новый, начинаем массив
                if (!File.Exists(path))
                {
                    await File.WriteAllTextAsync(path, "[\n" + jsonData.TrimStart('[', '\n').TrimEnd(']', '\n'));
                    return;
                }

                // Дозапись в существующий файл
                var temp = JsonConvert.SerializeObject(games, Newtonsoft.Json.Formatting.Indented)
                    .TrimStart('[')
                    .TrimEnd(']');

                await using var stream = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None);

                stream.Seek(-2, SeekOrigin.End); // Перемещаемся перед закрывающей ]
                stream.Write(Encoding.UTF8.GetBytes(",\n" + temp + "\n]"));
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }

           
        }
        // Вспомогательный метод для получения URL изображения
        string GetImageUrl(IEnumerable<Medium1> media)
        {
            if (media == null)
                return string.Empty;

            foreach (var m in media)
            {
                if (m?.role == "MASTER")
                    return m.url ?? string.Empty;
            }

            return string.Empty;
        }
        // Вспомогательный метод для получения currencyCode
        string GetCurrencyCode(Webcta webctas)
        {
         
            if (webctas == null)
                return string.Empty;

          
                if (webctas.price != null)
                    return webctas.price.currencyCode ?? string.Empty;
           
            return string.Empty;
        }
    }
}
