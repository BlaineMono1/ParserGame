using Business.Data.Models;
using ParserService.Interfaces;
using HtmlAgilityPack;
using System.Text.Json;
using ParserService.Utils;
using ParserService.Models;
namespace ParserService.Parsers
{
    public class ConceptParser : IParser<ConceptDto>
    {
  

        public async Task<IEnumerable<ConceptDto>> ParseAsync(string url, HttpClient httpClient)
        {
            try
            {
                Logger.Log($"parsing {url}");
                var html = await httpClient.GetStringAsync(url);
                var htmlDoument = new HtmlDocument();
                htmlDoument.LoadHtml(html);
                var gameList = new List<ConceptDto>();

                var gameNodes = htmlDoument.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']//li");

                if (gameNodes != null)
                {
                    foreach (var gameNode in gameNodes)
                    {
                        var aNode = gameNode.SelectSingleNode(".//a");
                        if (aNode != null)
                        {
                            var telemetryMeta = aNode.GetAttributeValue("data-telemetry-meta", null);
                            if (!string.IsNullOrEmpty(telemetryMeta))
                            {
                                try
                                {
                                    // Декодирование JSON-строки
                                    telemetryMeta = telemetryMeta.Replace("&quot;", "\"");

                                    // Парсинг JSON с использованием JsonDocument
                                    using var jsonDoc = JsonDocument.Parse(telemetryMeta);
                                    var root = jsonDoc.RootElement;

                                    // Извлечение id и name
                                    var id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
                                    var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

                                    // Добавляем данные в список
                                    gameList.Add(new ConceptDto
                                    {
                                        Id = id,
                                        //Name = name
                                    });
                                }
                                catch (JsonException ex)
                                {
                                   Logger.Log($"Error parsing JSON: {ex.Message}");
                                }
                            }
                        }



                    }
                }
                return gameList;
            }
            catch(HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        


           
        }

        public Task<ConceptDto> ParseAsyncJson(string url,string conceptId, HttpClient httpClient)
        {
            throw new NotImplementedException();
        }
    }
}
