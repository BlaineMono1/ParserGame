using System.Text;
using System.Text.Json;
using HtmlAgilityPack;
using ParserService.Interfaces;
using ParserService.Service;
using ParserService.SetUrl;
using ParserService.Utils;
using static ParserService.Models.GameModel.ModelAddon;
using static ParserService.Models.GameModel.ModelGame;
using static ParserService.Models.GameModel.StarProductModel;

namespace ParserService.Parsers;

public class AddonParser : IParser<DataAddon>
{
    public Task<IEnumerable<DataGame>> ParseAsync(string url, HttpClient httpClient)
    {
        throw new NotImplementedException();
    }

    public async Task<DataAddon> ParseAsyncJson(
        string url,
        string productId,
        HttpClient httpClientUa,
        HttpClient httpClientTr
    )
    {
        try
        {
            Logger.Log($"parsing {productId}");
            var requestBody = new
            {
                operationName = "productRetrieveForCtasWithPrice",
                variables = new { productId = productId },
                extensions = new
                {
                    persistedQuery = new
                    {
                        version = 1,
                        sha256Hash = "8872b0419dcab2fea5916ef698544c237b1096f9e76acc6aacf629551adee8cd",
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
                    var result = JsonSerializer.Deserialize<RootobjectAddon>(
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
                    var resultTr = JsonSerializer.Deserialize<RootobjectTrAddon>(
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
                        operationName = "wcaProductStarRatingRetrive",
                        variables = new { productId = productId },
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
                        .PostAsync(generateUrl.GenerateRequest(productId), contentStar)
                        .Result;
                    response.EnsureSuccessStatusCode();
                    var jsonStar = await responseStar.Content.ReadAsStringAsync();

                    var resultStar = JsonSerializer.Deserialize<RootobjectStarProduct>(
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

                    // if (result.data.productRetrieve != null)
                    // {
                    //     foreach (var item in result.data.productRetrieve.products)
                    //     {
                    //         item.release = await Release(item.id, httpClientUa) ?? string.Empty;
                    //     }
                    // }

                    var data = new DataAddon()
                    {
                        dataUa = result,
                        dataTr = resultTr,
                        dataStar = resultStar,
                    };

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

    Task<IEnumerable<DataAddon>> IParser<DataAddon>.ParseAsync(string url, HttpClient httpClient)
    {
        throw new NotImplementedException();
    }
}
