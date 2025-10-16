using System.Text.Json;
using HtmlAgilityPack;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Utils;

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
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                var gameList = new List<ConceptDto>();

                var gameNodes = htmlDocument.DocumentNode.SelectNodes(
                    "//ul[@class='psw-grid-list psw-l-grid']//li"
                );

                if (gameNodes != null)
                {
                    foreach (var gameNode in gameNodes)
                    {
                        var aNode = gameNode.SelectSingleNode(".//a");
                        if (aNode != null)
                        {
                            var telemetryMeta = aNode.GetAttributeValue(
                                "data-telemetry-meta",
                                null
                            );
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
                                    var id = root.TryGetProperty("id", out var idProp)
                                        ? idProp.GetString()
                                        : null;
                                    var name = root.TryGetProperty("name", out var nameProp)
                                        ? nameProp.GetString()
                                        : null;

                                    // Добавляем данные в список
                                    gameList.Add(
                                        new ConceptDto
                                        {
                                            Id = id,
                                            //Name = name
                                        }
                                    );
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
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public Task<ConceptDto> ParseAsyncJson(
            string url,
            string urlTr,
            string conceptId,
            HttpClient httpClient,
            HttpClient httpClientTr
        )
        {
            throw new NotImplementedException();
        }

        public Task<ConceptDto> ParseAsyncJson(
            string url,
            string conceptId,
            HttpClient httpClient,
            HttpClient httpClientTr
        )
        {
            throw new NotImplementedException();
        }

        public Task<ConceptDto> ParseProductAsyncJson(
            string url,
            string urlTr,
            string conceptId,
            string productIdTr,
            HttpClient httpClient,
            HttpClient httpClientTr
        )
        {
            throw new NotImplementedException();
        }
    }
}
