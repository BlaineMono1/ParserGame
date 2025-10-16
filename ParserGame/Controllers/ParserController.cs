using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ParserGame.Models;
using ParserGame.Settings;
using ParserService.Models;
using ParserService.Models.GameModel;
using ParserService.Service;
using ParserService.Utils;
using ParserService.Utils.Helper;
using ParserService.Utils.Model;
using static ParserService.Models.GameModel.ModelAddon;
using static ParserService.Models.GameModel.ModelGame;

namespace ParserGame.Controllers
{
    public class ParserController : ControllerBase
    {
        private readonly ParserAdapter _adapter;

        public ParserController(ParserAdapter adapter)
        {
            _adapter = adapter;
        }

        [HttpPost("proxy-set")]
        public IActionResult ProxySet(ProxyModel proxyModel)
        {
            ProxyHelper.Proxies[0].Url = proxyModel.Url;
            ProxyHelper.Proxies[0].Login = proxyModel.Login;
            ProxyHelper.Proxies[0].Password = proxyModel.Password;
            return Ok(ProxyHelper.Proxies[0]);
        }

        [HttpPost("current-price")]
        public async Task<IActionResult> ParseSubscipes([FromBody] List<ProductCode> cusaList)
        {
            try
            {
                var proxy = ProxyHelper.Proxies[0];
                HttpClient httpClient = HttpClientFactory.CreateClientUa(proxy);

                Dictionary<string, string> list = new Dictionary<string, string>();
                if (cusaList == null)
                    return Ok(new { Mesage = "Пустой спискок" });

                foreach (var item in cusaList)
                {
                    list.Add(item.CusaCodeUa, item.CusaCodeTr);
                }

                if (list != null)
                {
                    //Генерация для UA
                    var requestService = new UrlGeneratorService(BaseUrl.RequestJsonAddon);
                    var requestList = requestService.GenerateRequests(list);

                    HttpClient httpClientTr = HttpClientFactory.CreateClientTr(proxy);
                    var result = await _adapter.ParseMultipleAddonAsync<DataAddon>(
                        "addon",
                        requestList,
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

        [HttpGet("game")]
        public async Task<IActionResult> ParseEntity(string concept)
        {
            try
            {
                var proxy = ProxyHelper.Proxies[0];
                HttpClient httpClient = HttpClientFactory.CreateClientUa(proxy);

                HashSet<string> list = new HashSet<string>();

                list.Add(concept);

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

                return null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
