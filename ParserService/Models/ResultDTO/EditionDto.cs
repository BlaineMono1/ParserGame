using System;

namespace ParserService.Models.ResultDTO;

public class EditionDto
{
    public string? CusaCodeUA { get; set; }
    public string? CusaCodeTR { get; set; }
    public string Type { get; set; }
    public string EditionType { get; set; }
    public string EditionName { get; set; }
    public string Geners { get; set; }
    public string Image { get; set; }
    public string Platform { get; set; }
    public string? Subscription { get; set; }
    public string? Features { get; set; }
    public string CodeRegion { get; set; }
    public string OrderType { get; set; }
    public DateTime? Release { get; set; }
    public ProductDto Product { get; set; }
}
