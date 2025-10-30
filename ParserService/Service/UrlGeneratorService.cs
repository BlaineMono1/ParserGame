using ParserService.Models;

namespace ParserService.Service
{
    public class UrlGeneratorService
    {
        private readonly string _baseUrl;

        public UrlGeneratorService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Генерирует URL для указанной страницы.
        /// </summary>
        /// <param name="pageNumber">Номер страницы.</param>
        /// <returns>Полный URL с номером страницы.</returns>
        public string GenerateUrl(int pageNumber)
        {
            return $"{_baseUrl}/{pageNumber}";
        }
        public string GenerateUrlPreOrderAndNow(int pageNumber)
        {
            return _baseUrl.Replace("fake", pageNumber.ToString());
        }
        public string GenerateUrlCusaCode(string cusacode)
        {
            return $"{_baseUrl}/{cusacode}";
        }

        /// <summary>
        /// Генерирует список URL для диапазона страниц.
        /// </summary>
        /// <param name="startPage">Начальная страница.</param>
        /// <param name="endPage">Конечная страница.</param>
        /// <returns>Список URL для указанных страниц.</returns>
        public IEnumerable<string> GenerateUrls(int startPage, int endPage)
        {
            for (int i = startPage; i <= endPage; i++)
            {
                yield return GenerateUrl(i);
            }
        }
        /// <summary>
        /// Генерирует страницы для преордера и новых игр 
        /// </summary>
        /// <param name="startPage"></param>
        /// <param name="endPage"></param>
        /// <returns></returns>
        public IEnumerable<string> GenerateUrlsPreOrderAndNow(int startPage, int endPage)
        {
            for (int i = startPage; i <= endPage; i++)
            {
                yield return GenerateUrlPreOrderAndNow(i);
            }
        }
        /// <summary>
        /// Генерирует URL с новым conceptId.
        /// </summary>
        /// <param name="conceptId">Новый conceptId.</param>
        /// <returns>URL с обновленным conceptId.</returns>
        public string GenerateRequest(string conceptId)
        {
            // Заменяем старый conceptId на новый
            var updatedRequest = _baseUrl.Replace("fakeId", Uri.EscapeDataString(conceptId));
            return updatedRequest;
        }

        /// <summary>
        /// Генерирует список URL для диапазона conceptId.
        /// </summary>
        /// <param name="startId">Начальный conceptId.</param>
        /// <param name="endId">Конечный conceptId.</param>
        /// <returns>Список URL для указанных conceptId.</returns>
        public Dictionary<string, string> GenerateRequests(List<string> concepIds)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (var concepId in concepIds)
            {
                results.Add(concepId, GenerateRequest(concepId));
            }
            return results;
        }

        /// <summary>
        /// Генерирует список URL для диапазона Product.
        /// </summary>
        /// <param name="startId">Начальный conceptId.</param>
        /// <param name="endId">Конечный conceptId.</param>
        /// <returns>Список URL для указанных conceptId.</returns>
        public List<PrtoductIdRequest> GenerateRequests(Dictionary<string, string> productId)
        {
            var result = new List<PrtoductIdRequest>();
            foreach (var item in productId)
            {
                result.Add(
                    new PrtoductIdRequest
                    {
                        Url = GenerateRequest(item.Key),
                        UrlTr = GenerateRequest(item.Value),
                        ProductIdUa = item.Key,
                        ProductIdTr = item.Value,
                    }
                );
            }
            return result;
        }
    }
}
