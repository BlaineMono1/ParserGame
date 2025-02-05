
namespace ParserService.Models.GameModel
{
    // Основная модель для ответа
    public class ConceptRetrieveResponse
    {
        public ConceptRetrieve Data { get; set; }
    }
    public class ConceptRetrieve
    {
        public string __typename { get; set; } // "Concept"
        public string Id { get; set; }
        public List<Media> Media { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }

    public class Media
    {
        public string __typename { get; set; } // "Media"
        public string Role { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }

    public class Product
    {
        public string __typename { get; set; } // "Product"
        public ContentRating ContentRating { get; set; }
        public ProductEdition Edition { get; set; }
        public string Id { get; set; }
        public List<LocalizedGenre> LocalizedGenres { get; set; }
        public List<Media> Media { get; set; }
        public List<string> Platforms { get; set; }
        public string TopCategory { get; set; }
        public Concept Concept { get; set; }
        public string InvariantName { get; set; }
        public bool IsInWishlist { get; set; }
        public bool IsWishlistable { get; set; }
        public string Name { get; set; }
        public string NpTitleId { get; set; }
        public List<Sku> Skus { get; set; }
        public List<GameCTA> WebCtas { get; set; }
    }

    public class ContentRating
    {
        public string __typename { get; set; } // "ProductContentRating"
        public string Name { get; set; }
    }

    public class ProductEdition
    {
        public string __typename { get; set; } // "ProductEdition"
        public List<string> Features { get; set; }
        public string Name { get; set; }
        public object Ordering { get; set; } // Может быть null
        public string Type { get; set; }
    }

    public class LocalizedGenre
    {
        public string __typename { get; set; } // "LocalizedGenre"
        public string Value { get; set; }
    }

    public class Sku
    {
        public string __typename { get; set; } // "Sku"
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class GameCTA
    {
        public string __typename { get; set; } // "GameCTA"
        public Action Action { get; set; }
        public bool HasLinkedConsole { get; set; }
        public CTAMeta Meta { get; set; }
        public string Type { get; set; }
        public Price Price { get; set; }
    }

    public class Action
    {
        public string __typename { get; set; } // "Action"
        public List<ActionParam> Param { get; set; }
        public string Type { get; set; }
    }

    public class ActionParam
    {
        public string __typename { get; set; } // "ActionParam"
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class CTAMeta
    {
        public string __typename { get; set; } // "CTAMeta"
        public bool Exclusive { get; set; }
        public List<IneligibilityReason> IneligibilityReasons { get; set; }
        public object PlayabilityDate { get; set; } // Может быть null
        public string UpSellService { get; set; }
    }
    public class IneligibilityReason
    {
        public string __typename { get; set; } // "IneligibilityReason"
        public List<string> Names { get; set; }
        public string Type { get; set; }
    }

    public class Price
    {
        public string __typename { get; set; } // "Price"
        public string Applicability { get; set; }
        public string BasePrice { get; set; }
        public int BasePriceValue { get; set; }
        public object CampaignId { get; set; } // Может быть null
        public string CurrencyCode { get; set; }
        public string DiscountText { get; set; }
        public string DiscountedPrice { get; set; }
        public int DiscountedValue { get; set; }
        public object EndTime { get; set; } // Может быть null
        public bool IsExclusive { get; set; }
        public bool IsFree { get; set; }
        public bool IsTiedToSubscription { get; set; }
        public List<object> Qualifications { get; set; } // Может быть пустым
        public string RewardId { get; set; }
        public List<string> ServiceBranding { get; set; }
        public string UpsellText { get; set; }
    }

    public class Concept
    {
        public string __typename { get; set; } // "Concept"
        public string Id { get; set; }
    }

}