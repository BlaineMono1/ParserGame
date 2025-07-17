namespace ParserService.Models.GameModel
{
    public class ModelGame
    {
        public class DataGame
        {
            public Rootobject dataUa { get; set; }

            public RootobjectTr dataTr { get; set; }

            public string Voice { get; set; }
            public string SubtitlesLanguages { get; set; }
            public List<AddOneModel> addonList { get; set; } = new List<AddOneModel>();

            public RootobjectStar dataStar { get; set; }
        }

        public class AddOneModel
        {
            public string CusaCode { get; set; }

            public string Name { get; set; }

            public string Type { get; set; }

            public string Image { get; set; }

            public string Price { get; set; }
        }

        public class RootobjectTr
        {
            public Data data { get; set; }
        }

        public class Rootobject
        {
            public Data data { get; set; }
        }

        public class Data
        {
            public Conceptretrieve conceptRetrieve { get; set; }
        }

        public class Conceptretrieve
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public Medium[] media { get; set; }
            public string name { get; set; }
            public Product[] products { get; set; }
        }

        public class Medium
        {
            public string __typename { get; set; }
            public string role { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Product
        {
            public string __typename { get; set; }
            public Contentrating contentRating { get; set; }
            public Edition? edition { get; set; }
            public string id { get; set; }
            public Localizedgenre[] localizedGenres { get; set; }
            public Medium1[] media { get; set; }
            public string[] platforms { get; set; }
            public string topCategory { get; set; }
            public Concept concept { get; set; }
            public string invariantName { get; set; }
            public bool isInWishlist { get; set; }
            public bool isWishlistable { get; set; }
            public string name { get; set; }
            public string npTitleId { get; set; }
            public Sku[] skus { get; set; }
            public Webcta[] webctas { get; set; }

            public string release { get; set; }
        }

        public class Contentrating
        {
            public string __typename { get; set; }
            public string name { get; set; }
        }

        public class Edition
        {
            public string __typename { get; set; }
            public object[] features { get; set; }
            public string name { get; set; }
            public object ordering { get; set; }
            public string type { get; set; }
        }

        public class Concept
        {
            public string __typename { get; set; }
            public string id { get; set; }
        }

        public class Localizedgenre
        {
            public string __typename { get; set; }
            public string value { get; set; }
        }

        public class Medium1
        {
            public string __typename { get; set; }
            public string role { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Sku
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Webcta
        {
            public string __typename { get; set; }
            public Action action { get; set; }
            public bool hasLinkedConsole { get; set; }
            public Meta meta { get; set; }
            public string type { get; set; }
            public Price price { get; set; }
        }

        public class Action
        {
            public string __typename { get; set; }
            public Param[] param { get; set; }
            public string type { get; set; }
        }

        public class Param
        {
            public string __typename { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Meta
        {
            public string __typename { get; set; }
            public bool exclusive { get; set; }
            public Ineligibilityreason[] ineligibilityReasons { get; set; }
            public object playabilityDate { get; set; }
            public string upSellService { get; set; }
        }

        public class Ineligibilityreason
        {
            public string __typename { get; set; }
            public object[] names { get; set; }
            public string type { get; set; }
        }

        public class Price
        {
            public string __typename { get; set; }
            public string applicability { get; set; }
            public string basePrice { get; set; }
            public decimal? basePriceValue { get; set; } = default(decimal?);
            public object campaignId { get; set; }
            public string currencyCode { get; set; }
            public string discountText { get; set; }
            public string discountedPrice { get; set; }
            public decimal? discountedValue { get; set; } = default(decimal?);
            public object endTime { get; set; }
            public bool? isExclusive { get; set; }
            public bool? isFree { get; set; }
            public bool? isTiedToSubscription { get; set; }
            public Qualification[] qualifications { get; set; }
            public string rewardId { get; set; }
            public string[] serviceBranding { get; set; }
            public string upsellText { get; set; }
        }

        public class Qualification
        {
            public string __typename { get; set; }
            public string type { get; set; }
            public string value { get; set; }
        }
    }
}
