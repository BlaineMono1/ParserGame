using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Models.ResultDTO;
using ParserService.Models.ResultDTO.Addon;
using ParserService.Utils;
using ParserService.Utils.Helper;
using Polly;
using static ParserService.Models.GameModel.ModelAddon;
using static ParserService.Models.GameModel.ModelGame;

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
        public async Task<IEnumerable<T>> ParseAsync<T>(
            string parserKey,
            string url,
            HttpClient httpClient
        )
            where T : class
        {
            if (_parsers.ContainsKey(parserKey))
            {
                var parser = _parsers[parserKey] as IParser<T>;
                if (parser != null)
                {
                    return await parser.ParseAsync(url, httpClient);
                }
            }

            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }

        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task<IEnumerable<T>> ParseMultiplePagesAsync<T>(
            string parserKey,
            IEnumerable<string> urls,
            HttpClient httpClient
        )
            where T : class
        {
            var results = new ConcurrentBag<T>();
            var processedUrls = new HashSet<string>(); // Для отслеживания уникальных URL
            await Parallel.ForEachAsync(
                urls,
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                async (url, cancellationToken) =>
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
                        var pageResults = await ParseAsync<T>(parserKey, url, httpClient);
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
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message);
                    }
                }
            );

            return results.AsEnumerable();
        }

        public async Task<T> ParseJsonAsync<T>(
            string parserKey,
            string url,
            string conceptId,
            HttpClient httpClient,
            HttpClient httpClientTr
        )
            where T : class
        {
            int retryCount = 3; // Количество повторных попыток
            int delayMilliseconds = 500; // Задержка между попытками
            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                try
                {
                    if (_parsers.ContainsKey(parserKey))
                    {
                        //httpClient.Timeout = TimeSpan.FromSeconds(30); // Увеличиваем таймаут
                        var parser = _parsers[parserKey] as IParser<T>;
                        if (parser != null)
                        {
                            return await parser.ParseAsyncJson(
                                url,
                                conceptId,
                                httpClient,
                                httpClientTr
                            );
                        }
                    }
                }
                catch (Exception ex) when (ex is SocketException || ex is HttpRequestException)
                {
                    Logger.Log($"Attempt {attempt + 1} failed: {ex.Message}");
                    if (attempt == retryCount - 1)
                        throw; // Если последняя попытка, выбрасываем исключение
                    await Task.Delay(delayMilliseconds); // Ждём перед следующей попыткой
                }
            }
            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }

        public async Task<T> ProductParse<T>(
            string parserKey,
            string url,
            string urlTr,
            string productId,
            string productIdTr,
            HttpClient httpClient,
            HttpClient httpClientTr
        )
            where T : class
        {
            int retryCount = 3; // Количество повторных попыток
            int delayMilliseconds = 500; // Задержка между попытками
            for (int attempt = 0; attempt < retryCount; attempt++)
            {
                try
                {
                    if (_parsers.ContainsKey(parserKey))
                    {
                        //httpClient.Timeout = TimeSpan.FromSeconds(30); // Увеличиваем таймаут
                        var parser = _parsers[parserKey] as IParser<T>;
                        if (parser != null)
                        {
                            return await parser.ParseProductAsyncJson(
                                url,
                                urlTr,
                                productId,
                                productIdTr,
                                httpClient,
                                httpClientTr
                            );
                        }
                    }
                }
                catch (Exception ex) when (ex is SocketException || ex is HttpRequestException)
                {
                    Logger.Log($"Attempt {attempt + 1} failed: {ex.Message}");
                    if (attempt == retryCount - 1)
                        throw; // Если последняя попытка, выбрасываем исключение
                    await Task.Delay(delayMilliseconds); // Ждём перед следующей попыткой
                }
            }
            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }

        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task<List<GameDto>> ParseMultipleJsonAsync<T>(
            string parserKey,
            Dictionary<string, string> urls,
            HttpClient httpClient,
            HttpClient httpClientTr,
            int batchSize = 10
        )
            where T : class
        {
            var resultGame = new List<GameDto>();
            int a = 0;
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    Environment.ProcessorCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                ); // Экспоненциальная задержка
            Logger.Log(Convert.ToString("Кол-во потоков " + Environment.ProcessorCount));

            var processedUrls = new ConcurrentDictionary<string, byte>(); // Потокобезопасный словарь
            var dataBuffer = new BlockingCollection<DataGame>();
            var writeSemaphore = new SemaphoreSlim(1, 1);
            var batchCounter = 0;
            // Задача для фоновой записи
            var writeTask = Task.Run(async () =>
            {
                var batch = new List<DataGame>();

                while (!dataBuffer.IsCompleted || batch.Count > 0)
                {
                    while (batch.Count < batchSize && !dataBuffer.IsCompleted)
                    {
                        try
                        {
                            batch.Add(dataBuffer.Take());
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex.Message);
                        }
                    }

                    if (batch.Count > 0)
                    {
                        await writeSemaphore.WaitAsync();
                        try
                        {
                            resultGame.AddRange(GetDtoGames(batch));
                            batchCounter += batch.Count;
                            Logger.Log($"Written {batchCounter} items total");
                            batch.Clear();
                        }
                        finally
                        {
                            writeSemaphore.Release();
                        }
                    }
                }
            });
            var tasks = new List<Task>();
            await Parallel.ForEachAsync(
                urls,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    CancellationToken = CancellationToken.None,
                },
                async (url, cancellationToken) =>
                {
                    if (processedUrls.TryAdd(url.Key, 0))
                    {
                        //var randomDelay = TimeSpan.FromMilliseconds(new Random().Next(500, 2000));

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
                                    url.Key,
                                    httpClient,
                                    httpClientTr
                                );
                                //await Task.Delay(randomDelay);
                                if (result.dataUa.data != null)
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
                }
            );

            dataBuffer.CompleteAdding();
            await writeTask;
            Logger.Log("All data processed");
            return resultGame;
        }

        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task<List<AddonDto>> ParseMultipleAddonAsync<T>(
            string parserKey,
            List<PrtoductIdRequest> requests,
            HttpClient httpClient,
            HttpClient httpClientTr,
            int batchSize = 10
        )
            where T : class
        {
            int a = 0;
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    Environment.ProcessorCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt))
                ); // Экспоненциальная задержка
            Logger.Log(Convert.ToString("Кол-во потоков " + Environment.ProcessorCount));
            var resultList = new List<AddonDto>();
            var processedUrls = new ConcurrentDictionary<string, byte>(); // Потокобезопасный словарь
            var dataBuffer = new BlockingCollection<DataAddon>();
            var writeSemaphore = new SemaphoreSlim(1, 1);
            var batchCounter = 0;
            // Задача для фоновой записи
            var writeTask = Task.Run(async () =>
            {
                var batch = new List<DataAddon>();

                while (!dataBuffer.IsCompleted || batch.Count > 0)
                {
                    while (batch.Count < batchSize && !dataBuffer.IsCompleted)
                    {
                        try
                        {
                            batch.Add(dataBuffer.Take());
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex.Message);
                        }
                    }

                    if (batch.Count > 0)
                    {
                        await writeSemaphore.WaitAsync();
                        try
                        {
                            resultList.AddRange(GetAddonDtos(batch));
                            batchCounter += batch.Count;
                            Logger.Log($"Written {batchCounter} items total");
                            batch.Clear();
                        }
                        finally
                        {
                            writeSemaphore.Release();
                        }
                    }
                }
            });
            var tasks = new List<Task>();
            await Parallel.ForEachAsync(
                requests,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    CancellationToken = CancellationToken.None,
                },
                async (request, cancellationToken) =>
                {
                    if (processedUrls.TryAdd(request.ProductIdUa, 0))
                    {
                        //var randomDelay = TimeSpan.FromMilliseconds(new Random().Next(500, 2000));

                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            var proxyUrl = ProxyHelper.Proxies;
                            var proxyManager = new ProxyManager(proxyUrl);
                            var proxy = await proxyManager.GetProxyAsync();
                            try
                            {
                                var result = await ProductParse<DataAddon>(
                                    parserKey,
                                    request.Url,
                                    request.UrlTr,
                                    request.ProductIdUa,
                                    request.ProductIdTr,
                                    httpClient,
                                    httpClientTr
                                );
                                //await Task.Delay(randomDelay);
                                if (result.dataUa.data != null)
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
                        Logger.Log($"Duplicate URL detected: {request.ProductIdUa}");
                    }
                }
            );

            dataBuffer.CompleteAdding();
            await writeTask;
            Logger.Log("All data processed");

            return resultList;
        }

        private List<GameDto> GetDtoGames(List<DataGame> batch)
        {
            string conceptError = "";

            try
            {
                var games = new List<GameDto>();

                foreach (var p in batch)
                {
                    // Проверка на null для основных объектов
                    if (
                        p?.dataUa?.data.conceptRetrieve == null
                        || p?.dataUa?.data.conceptRetrieve.products == null
                    )
                        continue;
                    if (
                        p?.dataTr?.data.conceptRetrieve == null
                        || p?.dataTr?.data?.conceptRetrieve.products == null
                    )
                        continue;

                    conceptError = p.dataUa.data.conceptRetrieve.id;
                    var conceptId = p.dataUa.data.conceptRetrieve.id ?? string.Empty;
                    var name = p.dataUa.data.conceptRetrieve.name ?? string.Empty;
                    var editions = new List<EditionDto>();
                    var voice = p.Voice ?? string.Empty;
                    var lang = p.SubtitlesLanguages ?? string.Empty;
                    int starCount = default;
                    if (
                        p.dataStar != null
                        && p.dataStar.data != null // ← Добавлено!
                        && p.dataStar.data.conceptRetrieve != null
                        && p.dataStar.data.conceptRetrieve.defaultProduct != null
                    )
                    {
                        if (
                            p.dataStar
                                .data
                                .conceptRetrieve
                                .defaultProduct
                                .starRating
                                .totalRatingsCount != null
                        )
                            starCount = p.dataStar
                                .data
                                .conceptRetrieve
                                .defaultProduct
                                .starRating
                                .totalRatingsCount;
                    }

                    foreach (var p1 in p.dataUa.data.conceptRetrieve.products)
                    {
                        var webcast = p1.webctas;
                        var matchedProduct = p.dataTr.data.conceptRetrieve.products.FirstOrDefault(
                            x =>
                                x.invariantName == p1.invariantName
                                && x.platforms != null
                                && p1.platforms != null
                                && x.platforms.SequenceEqual(p1.platforms)
                        );

                        var webcastTr = matchedProduct?.webctas; // может быть null

                        // Проверка на null для webctas и edition
                        if (webcast.FirstOrDefault() == null)
                            continue;

                        bool hasValidWebcta = false;
                        // Проверка на null для price
                        int indexWebcast = 0;
                        int indexWebcastTr = 0;
                        string? sub = null;

                        if (webcast.Length > 0)
                        {
                            for (int i = 0; i < webcast.Length; i++)
                            {
                                var item = webcast[i];
                                if (
                                    item.price != null
                                    && item.price.basePriceValue != 0
                                    && item.price.basePriceValue != null
                                )
                                {
                                    if (
                                        item.type == "UPSELL_PS_PLUS_GAME_CATALOG"
                                        || item.type == "UPSELL_EA_ACCESS_FREE"
                                    )
                                    {
                                        // Это upsell — запоминаем тип, но не считаем валидным webcta
                                        switch (item.type)
                                        {
                                            case "UPSELL_PS_PLUS_GAME_CATALOG":
                                                sub = "UPSELL_PS_PLUS_GAME_CATALOG";
                                                break;
                                            case "UPSELL_EA_ACCESS_FREE":
                                                sub = "UPSELL_EA_ACCESS_FREE";
                                                break;
                                        }
                                        // Продолжаем поиск валидного webcta в следующих элементах
                                        continue;
                                    }

                                    // Нашли валидный webcta без type → фиксируем и выходим
                                    hasValidWebcta = true;
                                    indexWebcast = i;
                                    break; // больше не нужно искать
                                }
                            }
                        }

                        #region Турция
                        if (webcastTr != null)
                        {
                            if (webcastTr.Length > 0)
                            {
                                for (int i = 0; i < webcastTr.Length; i++)
                                {
                                    var item = webcastTr[i];
                                    if (
                                        item.price != null
                                        && item.price.basePriceValue != 0
                                        && item.price.basePriceValue != null
                                    )
                                    {
                                        if (
                                            item.type == "UPSELL_PS_PLUS_GAME_CATALOG"
                                            || item.type == "UPSELL_EA_ACCESS_FREE"
                                        )
                                        {
                                            // Это upsell — запоминаем тип, но не считаем валидным webcta
                                            switch (item.type)
                                            {
                                                case "UPSELL_PS_PLUS_GAME_CATALOG":
                                                    sub = "UPSELL_PS_PLUS_GAME_CATALOG";
                                                    break;
                                                case "UPSELL_EA_ACCESS_FREE":
                                                    sub = "UPSELL_EA_ACCESS_FREE";
                                                    break;
                                            }
                                            // Продолжаем поиск валидного webcta в следующих элементах
                                            continue;
                                        }

                                        // Нашли валидный webcta без type → фиксируем и выходим
                                        hasValidWebcta = true;
                                        indexWebcastTr = i;
                                        break; // больше не нужно искать
                                    }
                                }
                            }
                        }

                        #endregion
                        if (!hasValidWebcta)
                            continue;

                        // Создание Edition
                        var edition = new EditionDto
                        {
                            CusaCodeUA = p1.id ?? string.Empty,
                            Type = "Game",
                            EditionName = p1.invariantName ?? string.Empty,
                            EditionType =
                                p1.edition != null ? p1.edition.name ?? string.Empty : string.Empty,
                            Geners =
                                p1.localizedGenres != null
                                    ? string.Join("|", p1.localizedGenres.Select(l => l.value))
                                    : string.Empty,
                            Image = GetImageUrl(p1.media),
                            Features =
                                p1.edition != null
                                    ? p1.edition.features != null
                                        ? string.Join("|", p1.edition.features)
                                        : string.Empty
                                    : string.Empty,
                            Platform =
                                p1.platforms != null
                                    ? string.Join("|", p1.platforms)
                                    : string.Empty,
                            CodeRegion = GetCurrencyCode(webcast[indexWebcast]) + "|",
                            OrderType = webcast[indexWebcast].type,
                        };
                        if (webcastTr != null)
                        {
                            if (webcastTr.Length > 0)
                            {
                                edition.CodeRegion += GetCurrencyCode(webcastTr[indexWebcastTr]);
                            }
                        }
                        if (p1.release != null)
                        {
                            edition.Release = p1.release;
                        }
                        if (sub != null)
                        {
                            edition.Subscription = sub;
                        }
                        var product = new ProductDto()
                        {
                            Type = "Игра",
                            PriceUa = webcast[indexWebcast].price.discountedValue / 100m ?? 0,
                            DiscountPercent =
                                webcast[indexWebcast].price.discountText ?? string.Empty,
                        };
                        if (webcastTr != null)
                        {
                            if (webcastTr.Length > 0)
                            {
                                if (webcastTr[indexWebcastTr] != null)
                                {
                                    edition.CusaCodeTR = p1.id ?? string.Empty;
                                    product.PriceTr =
                                        webcastTr[indexWebcastTr].price.discountedValue / 100m ?? 0;
                                    product.DiscountPercentTr =
                                        webcastTr[indexWebcastTr].price.discountText
                                        ?? string.Empty;
                                }
                            }
                        }
                        if (webcast[indexWebcast].price.endTime != null)
                        {
                            if (
                                long.TryParse(
                                    webcast[indexWebcast].price.endTime.ToString(),
                                    out long unixTimestampMs
                                )
                            )
                            {
                                // Преобразуем Unix-время в DateTime
                                DateTime utcTime = DateTimeOffset
                                    .FromUnixTimeMilliseconds(unixTimestampMs)
                                    .UtcDateTime;
                                product.DiscountDate = utcTime.AddHours(3);
                            }
                        }
                        edition.Product = product;
                        editions.Add(edition);
                    }
                    if (editions.Count > 0)
                        games.Add(
                            new GameDto
                            {
                                ConceptId = conceptId,
                                Name = name,
                                Editions = editions,
                                StarCount = starCount,
                                LanguagesVoice = voice,
                                LanguagesInterface = lang,
                            }
                        );
                }
                return games;
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message} - {conceptError}");
                return null;
            }
        }

        private List<AddonDto> GetAddonDtos(List<DataAddon> batch)
        {
            try
            {
                var addons = new List<AddonDto>();

                foreach (var p in batch)
                {
                    // Проверка на null для основных объектов
                    if (
                        p?.dataUa?.data.productRetrieve == null
                        || p?.dataUa?.data.productRetrieve == null
                    )
                        continue;
                    if (
                        p?.dataTr?.data.productRetrieve == null
                        || p?.dataTr?.data?.productRetrieve == null
                    )
                        continue;
                    var productId = p.dataUa.data.productRetrieve.id ?? string.Empty;
                    var name = p.dataUa.data.productRetrieve.name ?? string.Empty;
                    var cusaCodeUa = p.dataUa.data.productRetrieve.id;
                    var cusaCodeTr = p.dataTr.data.productRetrieve.id;

                    var webcast = p.dataUa.data.productRetrieve.webctas;

                    var webcastTr = p.dataTr.data.productRetrieve.webctas;
                    // Проверка на null для webctas и edition
                    if (webcast == null)
                        continue;

                    bool hasValidWebcta = false;
                    // Проверка на null для price
                    int indexWebcast = 0;
                    int indexWebcastTr = 0;
                    string? sub = null;
                    if (webcast.Length > 0)
                    {
                        for (int i = 0; i < webcast.Length; i++)
                        {
                            var item = webcast[i];
                            if (
                                item.price != null
                                && item.price.basePriceValue != 0
                                && item.price.basePriceValue != null
                            )
                            {
                                if (
                                    item.type == "UPSELL_PS_PLUS_GAME_CATALOG"
                                    || item.type == "UPSELL_EA_ACCESS_FREE"
                                )
                                {
                                    // Это upsell — запоминаем тип, но не считаем валидным webcta
                                    switch (item.type)
                                    {
                                        case "UPSELL_PS_PLUS_GAME_CATALOG":
                                            sub = "UPSELL_PS_PLUS_GAME_CATALOG";
                                            break;
                                        case "UPSELL_EA_ACCESS_FREE":
                                            sub = "UPSELL_EA_ACCESS_FREE";
                                            break;
                                    }
                                    // Продолжаем поиск валидного webcta в следующих элементах
                                    continue;
                                }

                                // Нашли валидный webcta без type → фиксируем и выходим
                                hasValidWebcta = true;
                                indexWebcast = i;
                                break; // больше не нужно искать
                            }
                        }
                    }

                    #region Турция
                    if (webcastTr != null)
                    {
                        if (webcastTr.Length > 0)
                        {
                            for (int i = 0; i < webcastTr.Length; i++)
                            {
                                var item = webcastTr[i];
                                if (
                                    item.price != null
                                    && item.price.basePriceValue != 0
                                    && item.price.basePriceValue != null
                                )
                                {
                                    if (
                                        item.type == "UPSELL_PS_PLUS_GAME_CATALOG"
                                        || item.type == "UPSELL_EA_ACCESS_FREE"
                                    )
                                    {
                                        // Это upsell — запоминаем тип, но не считаем валидным webcta
                                        switch (item.type)
                                        {
                                            case "UPSELL_PS_PLUS_GAME_CATALOG":
                                                sub = "UPSELL_PS_PLUS_GAME_CATALOG";
                                                break;
                                            case "UPSELL_EA_ACCESS_FREE":
                                                sub = "UPSELL_EA_ACCESS_FREE";
                                                break;
                                        }
                                        // Продолжаем поиск валидного webcta в следующих элементах
                                        continue;
                                    }

                                    // Нашли валидный webcta без type → фиксируем и выходим
                                    hasValidWebcta = true;
                                    indexWebcastTr = i;
                                    break; // больше не нужно искать
                                }
                            }
                        }
                    }

                    #endregion
                    if (!hasValidWebcta)
                        continue;
                    var addon = new AddonDto()
                    {
                        ConceptId = p.dataUa.data.productRetrieve.concept.id,
                        Name = name,
                        CusaCodeUA = cusaCodeUa,
                        CusaCodeTR = cusaCodeTr,
                    };
                    if (sub != null)
                    {
                        addon.Subscription = sub;
                    }

                    var product = new ProductDto()
                    {
                        Type = "Доп пакет",
                        PriceUa = webcast[indexWebcast].price.discountedValue / 100m ?? 0,
                        DiscountPercent = webcast[indexWebcast].price.discountText ?? string.Empty,
                    };
                    if (webcastTr != null)
                    {
                        product.PriceTr =
                            webcastTr[indexWebcastTr].price.discountedValue / 100m ?? 0;
                        product.DiscountPercentTr =
                            webcastTr[indexWebcastTr].price.discountText ?? string.Empty;
                    }

                    if (webcast[indexWebcast].price.endTime != null)
                    {
                        if (
                            long.TryParse(
                                webcast[indexWebcast].price.endTime.ToString(),
                                out long unixTimestampMs
                            )
                        )
                        {
                            // Преобразуем Unix-время в DateTime
                            DateTime utcTime = DateTimeOffset
                                .FromUnixTimeMilliseconds(unixTimestampMs)
                                .UtcDateTime;
                            product.DiscountDate = utcTime.AddHours(3);
                            product.DiscountDateTr = utcTime.AddHours(3);
                        }
                    }

                    addon.productDto = product;
                    addons.Add(addon);
                }
                return addons;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;
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
        string GetCurrencyCode(Models.GameModel.ModelGame.Webcta webctas)
        {
            if (webctas == null)
                return string.Empty;

            if (webctas.price != null)
                return webctas.price.currencyCode ?? string.Empty;

            return string.Empty;
        }
    }
}
