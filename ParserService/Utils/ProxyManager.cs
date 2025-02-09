using ParserService.Utils.Model;
using System;

namespace ParserService.Utils
{


    public class ProxyManager
    {
        private readonly SemaphoreSlim _semaphore;

        private readonly List<ProxyModel> _proxies;

        private readonly Random _random;
        public ProxyManager(IEnumerable<ProxyModel> proxies)
        {
            _proxies = new List<ProxyModel>(proxies);
            _semaphore = new SemaphoreSlim(_proxies.Count); // Ограничиваем количество одновременных запросов
            _random = new Random();
        }

        public async Task<ProxyModel> GetProxyAsync()
        {
            await _semaphore.WaitAsync(); // Ждем, пока освободится слот
            return _proxies[_random.Next(_proxies.Count)];
        }

        public void ReleaseProxy(ProxyModel proxy)
        {
            _semaphore.Release(); // Освобождаем слот
        }
    }
}
