using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using static ParserService.Models.GameModel.ModelGame;

namespace ParserService.Parsers
{
    public class GameParser : IParser<DataGame>
    {
        public Task<IEnumerable<DataGame>> ParseAsync(string url, HttpClient httpClient)
        {
            throw new NotImplementedException();
        }

        public async  Task<DataGame> ParseAsyncJson(string url,string conceptId, HttpClient httpClientUa, HttpClient httpClientTr)
        {
            try
            {
                Logger.Log($"parsing {conceptId}");
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
                int retryCount = 3; // Количество повторных попыток
                int delayMilliseconds = 500; // Задержка между попытками

                for (int attempt = 0; attempt < retryCount; attempt++)
                {
                    try
                    {

                        var response = httpClientUa.PostAsync(url, content).Result;
                        response.EnsureSuccessStatusCode();
                        var json = await response.Content.ReadAsStringAsync();

                             // Читаем JSON-ответ
                        var result = JsonSerializer.Deserialize<Rootobject>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,// Игнорировать регистр свойств
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });
                        var responseTr = httpClientTr.PostAsync(url, content).Result;
                        responseTr.EnsureSuccessStatusCode();
                        var jsonTr = await responseTr.Content.ReadAsStringAsync();
                        // Читаем JSON-ответ
                        var resultTr = JsonSerializer.Deserialize<RootobjectTr>(jsonTr, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,// Игнорировать регистр свойств
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });

                        var data = new DataGame()
                        {
                            dataUa = result,
                            dataTr = resultTr
                        };
                        return data;
                    }
                    catch (Exception ex) 
                    {
                        Logger.Log($"Attempt {attempt + 1} failed: {ex.Message}");
                        if (attempt == retryCount - 1) throw; // Если последняя попытка, выбрасываем исключение
                        await Task.Delay(delayMilliseconds); // Ждём перед следующей попыткой
                    }

                }
                return null;
              
              
            }
            catch (HttpRequestException ex)
            {
                Logger.Log($"Error: {ex.Message}");
                return null;
            }




        }

       
    }
}
