using System.Text.Json;
using Business.Data.Models;
using DataBaseToAccess;
using Microsoft.AspNetCore.Mvc;
using ParserGame.Settings;
using ParserService.Models;
using ParserService.Models.GameModel;
using ParserService.Service;
using ParserService.Utils;
using ParserService.Utils.Helper;
using static ParserService.Models.GameModel.ModelGame;

namespace ParserGame.Controllers
{
    public class ParserController : Controller
    {
        private readonly ParserAdapter _adapter;
        public readonly BaseDbContext _context;
        public ParserController(ParserAdapter adapter, BaseDbContext context)
        {
            _adapter = adapter;
            _context = context;
        }

        [HttpGet("concept/{startPage}/{endPage}")]
        public async Task<IActionResult> ParsePlayStationStore(int startPage, int endPage)
        {
            try
            {
                var proxy = ProxyHelper.Proxies[0];
                HttpClient httpClient = HttpClientFactory.CreateClientUa(proxy);


                var urlService = new UrlGeneratorService(BaseUrl.UrlConcept);
                var urls = urlService.GenerateUrls(startPage, endPage);

                var products = await _adapter.ParseMultiplePagesAsync<ConceptDto>("concept", urls,httpClient);
           
                Logger.Log("Stream file");
                // Генерируем имя файла
                var fileName = "concept.json";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Output", fileName);
                // Создаем директорию, если она не существует
                var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                // Записываем данные в файл в формате JSON
                var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(filePath, json);

                Logger.Log("Stream OK");

                return Ok(new { Message = "Data parsed and saved successfully.", FileName = fileName });

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }




        [HttpGet("cusacode")]
        public async Task<IActionResult> ParseEntityFull()
        {
            try
            {
                string filePath = "Output/concept.json";
                string json;
                using (var reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }
                var conceptList = JsonSerializer.Deserialize<List<ConceptDto>>(json);
                List<string> list = conceptList.Select(c=>c.Id).ToList();
                if (conceptList != null)
                {
                    //Генерация для UA
                    var requestService = new UrlGeneratorService(BaseUrl.RequestJson);
                    var requestList = requestService.GenerateRequests(list);
                    var fileName = "cusacode.json";
                    var filePathWrite = Path.Combine(Directory.GetCurrentDirectory(), "Output", fileName);
                    var proxy = ProxyHelper.Proxies[0];
                    HttpClient httpClient = HttpClientFactory.CreateClientUa(proxy);
                    HttpClient httpClientTr = HttpClientFactory.CreateClientTr(proxy);
                    await _adapter.ParseMultipleJsonAsync<DataGame>("cusacode", requestList, filePathWrite,httpClient, httpClientTr);


                   
                }

                return Ok(new { Message = "Data parsed and saved successfully." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
    }
}
