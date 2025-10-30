namespace ParserGame.Settings
{
    public static class BaseUrl
    {
        public static readonly string UrlConcept =
            "https://store.playstation.com/ru-ua/pages/browse";
        public static readonly string UrlConceptPreOrderAndNow =
            "https://store.playstation.com/ru-ua/pages/browse/fake?next_thirty_days=conceptReleaseDate&last_thirty_days=conceptReleaseDate";
        public static readonly string AddOneId =
            "https://store.playstation.com/ru-ua/category/51c9aa7a-c0c7-4b68-90b4-328ad11bf42e";
        public static readonly string RequestJson =
            "https://web.np.playstation.com/api/graphql/v1/op?operationName=conceptRetrieveForUpsellWithCtas&variables=%7B%22conceptId%22%3A%22fakeId%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%22278822e6c6b9f304e4c788867b3e8a448c67847ac932d09213d5085811be3a18%22%7D%7D";

        public static readonly string RequestJsonAddon =
            "https://web.np.playstation.com/api/graphql/v1/op?operationName=productRetrieveForCtasWithPrice&variables=%7B%22productId%22%3A%22fakeId%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%228872b0419dcab2fea5916ef698544c237b1096f9e76acc6aacf629551adee8cd%22%7D%7D";
    }
}
