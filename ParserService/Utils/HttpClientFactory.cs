using System.Net;
using ParserService.Utils.Helper;
using ParserService.Utils.Model;

namespace ParserService.Utils
{
    public class HttpClientFactory
    {
        public static HttpClient CreateClientTr(ProxyModel proxy)
        {
            var proxyweb = new WebProxy(proxy.Url)
            {
                Credentials = new NetworkCredential(proxy.Login, proxy.Password),
            };
            var handler = new HttpClientHandler()
            {
                Proxy = proxyweb,
                UseProxy = true,
                ServerCertificateCustomValidationCallback = (
                    sender,
                    cert,
                    chain,
                    sslPolicyErrors
                ) =>
                {
                    return true;
                },
            };

            var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(5) };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                UserAgentHelper.GetRandomUserAgent()
            );
            httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "text/html,application/xhtml+xml,application/json,application/xml;q=0.9,image/webp,*/*;q=0.8"
            );
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-TR");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }

        public static HttpClient CreateClientUa(ProxyModel proxy)
        {
            var proxyweb = new WebProxy(proxy.Url)
            {
                Credentials = new NetworkCredential(proxy.Login, proxy.Password),
            };
            var handler = new HttpClientHandler()
            {
                Proxy = proxyweb,
                UseProxy = true,
                ServerCertificateCustomValidationCallback = (
                    sender,
                    cert,
                    chain,
                    sslPolicyErrors
                ) =>
                {
                    return true;
                },
            };

            var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(5) };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                UserAgentHelper.GetRandomUserAgent()
            );
            httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "text/html,application/xhtml+xml,application/json,application/xml;q=0.9,image/webp,*/*;q=0.8"
            );
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-UA");
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
