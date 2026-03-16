using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


public class TwelveData_Service
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TwelveData_Service(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["TwelveData:ApiKey"];
    }

    public async Task<T> GetStockDataAsync<T>(string symbol, string range)
    {
        var url = $"https://api.twelvedata.com/time_series?symbol={symbol}&interval={range}&apikey={_apiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }

}