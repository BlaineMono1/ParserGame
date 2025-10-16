namespace ParserService.Interfaces
{
    public interface IParser<T>
        where T : class
    {
        Task<IEnumerable<T>> ParseAsync(string url, HttpClient httpClient);

        Task<T> ParseAsyncJson(
            string url,
            string conceptId,
            HttpClient httpClient,
            HttpClient httpClientTr
        );

        Task<T> ParseProductAsyncJson(
            string url,
            string urlTr,
            string conceptId,
            string productIdTr,
            HttpClient httpClient,
            HttpClient httpClientTr
        );
    }
}
