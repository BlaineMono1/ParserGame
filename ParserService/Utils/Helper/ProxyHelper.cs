using ParserService.Utils.Model;

namespace ParserService.Utils.Helper
{
    public static class ProxyHelper
    {
        public static readonly List<ProxyModel> Proxies = new List<ProxyModel>
        {
            new ProxyModel
            {
                Url = "",
                Login = "",
                Password = "",
            },
        };

        public static ProxyModel GetRandomProxy()
        {
            var random = new Random();
            return Proxies[random.Next(Proxies.Count)];
        }
    }
}
