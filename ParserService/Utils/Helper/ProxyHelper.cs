using ParserService.Utils.Model;

namespace ParserService.Utils.Helper
{
    public static class ProxyHelper
    {
        public static readonly List<ProxyModel> Proxies = new List<ProxyModel>
        {
            new ProxyModel
            {
                Url = "http://46.37.109.110:5519",
                Login = "user303788",
                Password = "1k6bro",
            },
            new ProxyModel
            {
                Url =
                    "http://ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c52472:mRvujB1Cc3DS2lGs@93.190.141.57:9999",
                Login =
                    "ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c52472",
                Password = "mRvujB1Cc3DS2lGs",
            },
            new ProxyModel()
            {
                Url =
                    "http://ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c4ff18:mRvujB1Cc3DS2lGs@88.99.166.254:9999",
                Login =
                    "ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c4ff18",
                Password = "mRvujB1Cc3DS2lGs",
            },
            new ProxyModel()
            {
                Url =
                    "http://ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c4d6fa:mRvujB1Cc3DS2lGs@88.99.102.207:9999",
                Login =
                    "ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c4d6fa",
                Password = "mRvujB1Cc3DS2lGs",
            },
            new ProxyModel()
            {
                Url =
                    "http://ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c49664:mRvujB1Cc3DS2lGs@138.201.49.224:9999",
                Login =
                    "ui5jpeyern-mobile.res-country-TR-state-745042-city-745044-hold-session-session-679e7f8c49664",
                Password = "mRvujB1Cc3DS2lGs",
            },
        };

        public static ProxyModel GetRandomProxy()
        {
            var random = new Random();
            return Proxies[random.Next(Proxies.Count)];
        }
    }
}
