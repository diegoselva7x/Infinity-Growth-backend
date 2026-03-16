using System.Net.Http;
using System.Threading.Tasks;
using DTO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace AppLogic.Services
{
    public class Price_Service
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public Price_Service(IConfiguration configuration)
        {
            _httpClient = new HttpClient();//Singleton
            _apiKey = configuration["TwelveData:ApiKey"];
        }

        public async Task<string> GetLastPriceInLastMinuteAsync(string symbol)
        {
            var url = $"https://api.twelvedata.com/quote?symbol={symbol}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();

            var jsonObj = JsonNode.Parse(content);
            var price = jsonObj?["price"]?.ToString();
            var close = jsonObj?["close"]?.ToString();

            // 👇 Si el precio en vivo no está disponible, usamos el de cierre
            return !string.IsNullOrWhiteSpace(price) ? price : close;
        }

        private async Task<string> GetLastClosePrice(string symbol)
        {
            var url = $"https://api.twelvedata.com/quote?symbol={symbol}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();

            var jsonObj = JsonNode.Parse(content);
            var close = jsonObj?["close"]?.ToString();

            return close;
        }
    }
}