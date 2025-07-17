using System;
using static ParserService.Models.GameModel.StarProductModel;

namespace ParserService.Models.GameModel;

public class ModelAddon
{
    public class DataAddon
    {
        public RootobjectAddon dataUa { get; set; }

        public RootobjectTrAddon dataTr { get; set; }

        public RootobjectStarProduct dataStar { get; set; }
    }

    public class RootobjectTrAddon
    {
        public Data data { get; set; }
    }

    public class RootobjectAddon
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public Productretrieve productRetrieve { get; set; }
    }

    public class Productretrieve
    {
        public string __typename { get; set; }
        public Concept concept { get; set; }
        public string id { get; set; }
        public string invariantName { get; set; }
        public bool isInWishlist { get; set; }
        public bool isWishlistable { get; set; }
        public string name { get; set; }
        public string npTitleId { get; set; }
        public Sku[] skus { get; set; }
        public Webcta[] webctas { get; set; }
    }

    public class Concept
    {
        public string __typename { get; set; }
        public string id { get; set; }
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
        public int basePriceValue { get; set; }
        public object campaignId { get; set; }
        public string currencyCode { get; set; }
        public string discountText { get; set; }
        public string discountedPrice { get; set; }
        public decimal? discountedValue { get; set; }
        public object endTime { get; set; }
        public bool isExclusive { get; set; }
        public bool isFree { get; set; }
        public bool isTiedToSubscription { get; set; }
        public object[] qualifications { get; set; }
        public string rewardId { get; set; }
        public string[] serviceBranding { get; set; }
        public object upsellText { get; set; }
    }
}
