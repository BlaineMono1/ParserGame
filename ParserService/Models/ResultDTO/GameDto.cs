namespace ParserService.Models.ResultDTO;

public class GameDto
{
    public string ConceptId { get; set; }
    public string Name { get; set; }
    public string LanguagesVoice { get; set; }
    public string LanguagesInterface { get; set; }
    public int StarCount { get; set; }
    public List<EditionDto>? Editions { get; set; }
}
