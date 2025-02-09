using ParserService.Models;
namespace ParserService.Interfaces
{
    public interface IParser<T> where T : class
    {
        Task<IEnumerable<T>> ParseAsync(string url, HttpClient httpClient);

        Task<T> ParseAsyncJson(string url, string conceptId, HttpClient httpClient, HttpClient httpClientTr);


    }
}
