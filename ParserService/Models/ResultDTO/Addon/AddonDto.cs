namespace ParserService.Models.ResultDTO.Addon;

public class AddonDto
{
    public string ConceptId { get; set; }
    public string CusaCodeUA { get; set; }
    public string CusaCodeTR { get; set; }
    public string Name { get; set; }
    public string Subscription { get; set; }

    public ProductDto productDto { get; set; }
}
