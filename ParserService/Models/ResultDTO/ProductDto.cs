using System;

namespace ParserService.Models.ResultDTO;

public class ProductDto
{
    public string Type { get; set; }
    public decimal? PriceUa { get; set; }
    public decimal? PriceTr { get; set; }
    public string DiscountPercent { get; set; }
    public DateTime? DiscountDate { get; set; }
}
