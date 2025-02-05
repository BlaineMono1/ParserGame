using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Business.Data.Models;
using HtmlAgilityPack;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Models.GameModel;
using ParserService.Utils;

namespace ParserService.Parsers
{
    public class GameParser : IParser<ConceptRetrieveResponse>
    {
        public Task<IEnumerable<ConceptRetrieveResponse>> ParseAsync(string url, HttpClient httpClient)
        {
            throw new NotImplementedException();
        }

        public async  Task<ConceptRetrieveResponse> ParseAsyncJson(string url,string conceptId, HttpClient httpClient)
        {
            try
            {
                Logger.Log($"parsing {url}");
                var requestBody = new
                {
                    operationName = "conceptRetrieveForUpsellWithCtas",
                    variables = new { conceptId = conceptId},
                    extensions = new
                    {
                        persistedQuery = new
                        {
                            version = 1,
                            sha256Hash = "278822e6c6b9f304e4c788867b3e8a448c67847ac932d09213d5085811be3a18"
                        }
                    }
                };
                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(url,content).Result;
                response.EnsureSuccessStatusCode();
                // Читаем JSON-ответ
                var json = await response.Content.ReadAsStringAsync();
                var result  =  JsonSerializer.Deserialize<ConceptRetrieveResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,// Игнорировать регистр свойств
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                    return result;
              
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }




        }

    }
}
