using System.Net;
using ParserService.Utils.Helper;

namespace ParserService.Utils
{
    public class HttpClientFactory
    {

        public static HttpClient CreateClient()
        {
            var proxyUrl = ProxyHelper.GetRandomProxy();

            var proxy = new WebProxy(proxyUrl.Url)
            {
                Credentials = new NetworkCredential(proxyUrl.Login, proxyUrl.Password)
            };
            var handler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = true,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentHelper.GetRandomUserAgent());
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/json,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", GetRandomLanguage());
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }

        private static string GetRandomLanguage()
        {
            var languages = new[] { "en-US", "en-GB", "fr-FR", "de-DE", "es-ES" };
            var random = new Random();
            return languages[random.Next(languages.Length)];
        }
    }
}
