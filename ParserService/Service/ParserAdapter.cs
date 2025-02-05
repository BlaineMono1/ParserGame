using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                HttpClient httpClient = HttpClientFactory.CreateClient();
               
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
        public async Task<IEnumerable<T>> ParseMultiplePagesAsync<T>(string parserKey, IEnumerable<string>urls) where T : class
        {
            
            var results = new ConcurrentBag<T>();
            await Parallel.ForEachAsync(urls, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async (url, cancellationToken) =>
            {
                var pageResults = await ParseAsync<T>(parserKey, url);
                foreach (var pageResult in pageResults)
                {
                    results.Add(pageResult);
                }
            });


            return results.AsEnumerable();
        }


        public async Task<T> ParseJsonAsync<T>(string parserKey, string url, string conceptId) where T:class
        {

            if (_parsers.ContainsKey(parserKey))
            {
                HttpClient httpClient = HttpClientFactory.CreateClient();

                var parser = _parsers[parserKey] as IParser<T>;
                if (parser != null)
                {
                    return await parser.ParseAsyncJson(url, conceptId, httpClient);
                }
            }

            throw new KeyNotFoundException($"Parser with key '{parserKey}' not found.");
        }


        /// <summary>
        /// Парсит данные с нескольких страниц.
        /// </summary>
        public async Task<IEnumerable<T>> ParseMultipleJsonAsync<T>(string parserKey,Dictionary<string,string> urls) where T : class
        {

            var results = new ConcurrentBag<T>();
            await Parallel.ForEachAsync(urls, new ParallelOptions { MaxDegreeOfParallelism = 1 }, async (url, cancellationToken) =>
            {
                var pageResults = await ParseJsonAsync<T>(parserKey, url.Value,url.Key);
               
                    results.Add(pageResults);
                
            });


            return results.AsEnumerable();
        }

    }
}
