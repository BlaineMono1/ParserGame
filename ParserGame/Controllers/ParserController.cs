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
using static ParserService.Models.GameModel.ModelAddon;
using static ParserService.Models.GameModel.ModelGame;

namespace ParserGame.Controllers
{
    public class ParserController : ControllerBase
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

                var urlService = new UrlGeneratorService(BaseUrl.AddOneId);
                var urls = urlService.GenerateUrls(startPage, endPage);

                var conceptList = await _adapter.ParseMultiplePagesAsync<ConceptDto>(
                    "concept",
                    urls,
                    httpClient
                );

                HashSet<string> list = new HashSet<string>();
                var c = conceptList.Select(c => c.Id).ToList();
                foreach (string item in c)
                {
                    list.Add(item);
                }

                if (conceptList != null)
                {
                    //Генерация для UA
                    var requestService = new UrlGeneratorService(BaseUrl.RequestJsonAddon);
                    var requestList = requestService.GenerateRequests(list.ToList());
                    var fileName = "cusacode.json";
                    var filePathWrite = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Output",
                        fileName
                    );
                    HttpClient httpClientTr = HttpClientFactory.CreateClientTr(proxy);
                    var result = await _adapter.ParseMultipleAddonAsync<DataAddon>(
                        "addon",
                        requestList,
                        filePathWrite,
                        httpClient,
                        httpClientTr
                    );
                    return Ok(result);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("game-full")]
        public async Task<IActionResult> ParseEntityFull(int startPage, int endPage)
        {
            try
            {
                var proxy = ProxyHelper.Proxies[0];
                HttpClient httpClient = HttpClientFactory.CreateClientUa(proxy);

                var urlService = new UrlGeneratorService(BaseUrl.UrlConcept);
                var urls = urlService.GenerateUrls(startPage, endPage);

                var conceptList = await _adapter.ParseMultiplePagesAsync<ConceptDto>(
                    "concept",
                    urls,
                    httpClient
                );

                HashSet<string> list = new HashSet<string>();
                var c = conceptList.Select(c => c.Id).ToList();
                foreach (string item in c)
                {
                    list.Add(item);
                }

                if (conceptList != null)
                {
                    //Генерация для UA
                    var requestService = new UrlGeneratorService(BaseUrl.RequestJson);
                    var requestList = requestService.GenerateRequests(list.ToList());
                    // var fileName = "cusacode.json";
                    // var filePathWrite = Path.Combine(
                    //     Directory.GetCurrentDirectory(),
                    //     "Output",
                    //     fileName
                    // );
                    HttpClient httpClientTr = HttpClientFactory.CreateClientTr(proxy);
                    var result = await _adapter.ParseMultipleJsonAsync<DataGame>(
                        "cusacode",
                        requestList,
                        httpClient,
                        httpClientTr
                    );
                    return Ok(result);
                }
                return null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
