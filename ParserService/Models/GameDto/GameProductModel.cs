namespace ParserService.Models.GameDto
{
    public class GameProductModel
    {

        public EditionGameModel Edition { get; set; }
        public string Id {  get; set; }
        public string Name { get; set; }

        public List<PlatformModel> Platforms { get; set; }
    }
}
