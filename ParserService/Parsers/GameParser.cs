using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
using ParserService.Service;
using ParserService.SetUrl;
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

        public async Task<DataGame> ParseAsyncJson(
            string url,
            string conceptId,
            HttpClient httpClientUa,
            HttpClient httpClientTr
        )
        {
            try
            {
                Logger.Log($"parsing {conceptId}");
                var requestBody = new
                {
                    operationName = "conceptRetrieveForUpsellWithCtas",
                    variables = new { conceptId = conceptId },
                    extensions = new
                    {
                        persistedQuery = new
                        {
                            version = 1,
                            sha256Hash = "278822e6c6b9f304e4c788867b3e8a448c67847ac932d09213d5085811be3a18",
                        },
                    },
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
                        var result = JsonSerializer.Deserialize<Rootobject>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true, // Игнорировать регистр свойств
                                DefaultIgnoreCondition = System
                                    .Text
                                    .Json
                                    .Serialization
                                    .JsonIgnoreCondition
                                    .WhenWritingNull,
                            }
                        );
                        var responseTr = httpClientTr.PostAsync(url, content).Result;
                        responseTr.EnsureSuccessStatusCode();
                        var jsonTr = await responseTr.Content.ReadAsStringAsync();
                        // Читаем JSON-ответ
                        var resultTr = JsonSerializer.Deserialize<RootobjectTr>(
                            jsonTr,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true, // Игнорировать регистр свойств
                                DefaultIgnoreCondition = System
                                    .Text
                                    .Json
                                    .Serialization
                                    .JsonIgnoreCondition
                                    .WhenWritingNull,
                            }
                        );

                        var requestBodyStar = new
                        {
                            operationName = "wcaConceptStarRatingRetrive",
                            variables = new { conceptId = conceptId },
                            extensions = new
                            {
                                persistedQuery = new
                                {
                                    version = 1,
                                    sha256Hash = "8c3dea41cf2f56baf3e0e0bfdf5e7298fa2941ab7488b8d7859bb0200dfb99b9",
                                },
                            },
                        };

                        var jsonBodyStar = JsonSerializer.Serialize(requestBodyStar);
                        var contentStar = new StringContent(
                            jsonBodyStar,
                            Encoding.UTF8,
                            "application/json"
                        );
                        var generateUrl = new UrlGeneratorService(UrlStorage.GetStars);
                        var responseStar = httpClientUa
                            .PostAsync(generateUrl.GenerateRequest(conceptId), contentStar)
                            .Result;
                        response.EnsureSuccessStatusCode();
                        var jsonStar = await responseStar.Content.ReadAsStringAsync();

                        var resultStar = JsonSerializer.Deserialize<RootobjectStar>(
                            jsonStar,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true, // Игнорировать регистр свойств
                                DefaultIgnoreCondition = System
                                    .Text
                                    .Json
                                    .Serialization
                                    .JsonIgnoreCondition
                                    .WhenWritingNull,
                            }
                        );

                        if (result.data.conceptRetrieve != null)
                        {
                            foreach (var item in result.data.conceptRetrieve.products)
                            {
                                item.release = await Release(item.id, httpClientUa) ?? string.Empty;
                            }
                        }

                        //Читаем язык и аддоны игры
                        var generate = new UrlGeneratorService(UrlStorage.UrlConceptUa);
                        var urlLang = generate.GenerateUrlCusaCode(conceptId);
                        var html = await httpClientUa.GetStringAsync(urlLang);
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);

                        var platform = htmlDocument.DocumentNode.SelectSingleNode(
                            "//dd[@data-qa='gameInfo#releaseInformation#platform-value']"
                        );
                        string voice = "";
                        string subtitlesLanguages = "";
                        if (platform != null)
                        {
                            if (platform.InnerText.Trim() == "PS4, PS5")
                            {
                                var voiceNodePs5 = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#ps5Voice-value']"
                                );
                                var subtitlesNodePs5 = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#ps5Subtitles-value']"
                                );

                                var voiceNodePs4 = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#ps4Voice-value']"
                                );
                                var subtitlesNodePs4 = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#ps4Subtitles-value']"
                                );
                                voice =
                                    voiceNodePs5?.InnerText.Trim()
                                    ?? string.Empty + "," + voiceNodePs4?.InnerText.Trim()
                                    ?? string.Empty;
                                subtitlesLanguages =
                                    subtitlesNodePs5?.InnerText.Trim()
                                    ?? string.Empty + "," + subtitlesNodePs4?.InnerText.Trim()
                                    ?? string.Empty;
                                if (voice == null || subtitlesLanguages == null)
                                {
                                    var voiceNode = htmlDocument.DocumentNode.SelectSingleNode(
                                        "//dd[@data-qa='gameInfo#releaseInformation#voice-value']"
                                    );
                                    var subtitlesNode = htmlDocument.DocumentNode.SelectSingleNode(
                                        "//dd[@data-qa='gameInfo#releaseInformation#subtitles-value']"
                                    );
                                    voice = voiceNode?.InnerText.Trim() ?? string.Empty;
                                    subtitlesLanguages =
                                        subtitlesNode?.InnerText.Trim() ?? string.Empty;
                                }
                            }
                            else
                            {
                                var voiceNode = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#voice-value']"
                                );
                                var subtitlesNode = htmlDocument.DocumentNode.SelectSingleNode(
                                    "//dd[@data-qa='gameInfo#releaseInformation#subtitles-value']"
                                );
                                voice = voiceNode?.InnerText.Trim() ?? string.Empty;
                                subtitlesLanguages =
                                    subtitlesNode?.InnerText.Trim() ?? string.Empty;
                            }
                        }

                        //var AddOns = htmlDocument.DocumentNode.SelectNodes("//ul[@class='psw-l-gap-y-6 psw-grid-list psw-l-grid']//li");

                        //var addOnList = new List<AddOneModel>();
                        //if (AddOns != null)
                        //{
                        //    Logger.Log("Parsing Add_ons");

                        //    foreach (var addOn in AddOns)
                        //    {
                        //        var aNode = addOn.SelectSingleNode(".//a");
                        //        if (aNode != null)
                        //        {
                        //            var telemetryMeta = aNode.GetAttributeValue("data-telemetry-meta", null);
                        //            if (!string.IsNullOrEmpty(telemetryMeta))
                        //            {
                        //                try
                        //                {
                        //                    // Декодирование JSON-строки
                        //                    telemetryMeta = telemetryMeta.Replace("&quot;", "\"");

                        //                    // Парсинг JSON с использованием JsonDocument
                        //                    using var jsonDoc = JsonDocument.Parse(telemetryMeta);
                        //                    var root = jsonDoc.RootElement;

                        //                    // Извлечение id и name
                        //                    var id = root.TryGetProperty("productId", out var idProp) ? idProp.GetString() : null;

                        //                    var productNameNode = addOn.SelectSingleNode(".//span[contains(@data-qa, 'product-name')]");
                        //                    var priceNode = addOn.SelectSingleNode(".//span[contains(@data-qa, 'price#display-price')]");
                        //                    var productTypeNode = addOn.SelectSingleNode(".//span[contains(@data-qa, 'product-type')]");
                        //                    var imageNode = addOn.SelectSingleNode(".//img[contains(@data-qa, 'game-art#image#image')]");

                        //                    addOnList.Add(new AddOneModel
                        //                    {
                        //                        CusaCode = id,
                        //                        Name = productNameNode.InnerText.Trim() ?? string.Empty,
                        //                        Price = priceNode.InnerText.Trim() ?? string.Empty,
                        //                        Type = productTypeNode.InnerText.Trim() ?? string.Empty,
                        //                        Image = imageNode.InnerText.Trim() ?? string.Empty,
                        //                    });

                        //                }
                        //                catch (JsonException ex)
                        //                {
                        //                    Logger.Log($"Error parsing JSON: {ex.Message}");
                        //                }
                        //            }
                        //        }

                        //    }
                        //}

                        var data = new DataGame()
                        {
                            dataUa = result,
                            dataTr = resultTr,
                            dataStar = resultStar,
                        };

                        data.Voice = voice;
                        data.SubtitlesLanguages = subtitlesLanguages;
                        //if (addOnList != null)
                        //    data.addonList.AddRange(addOnList);

                        return data;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Attempt {attempt + 1} failed: {ex.Message}");
                        if (attempt == retryCount - 1)
                            throw; // Если последняя попытка, выбрасываем исключение
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

        public static async Task<string>? Release(string cusacode, HttpClient httpClientUa)
        {
            var generate = new UrlGeneratorService(UrlStorage.UrlProductUA);
            var url = generate.GenerateUrlCusaCode(cusacode);
            //Читаем язык и аддоны игры
            var html = await httpClientUa.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var date = htmlDocument.DocumentNode.SelectSingleNode(
                "//dd[@data-qa='gameInfo#releaseInformation#releaseDate-value']"
            );
            if (date == null)
            {
                return null;
            }
            return date.InnerText.Trim();
        }
    }
}
