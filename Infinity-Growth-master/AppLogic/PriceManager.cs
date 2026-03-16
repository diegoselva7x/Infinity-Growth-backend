using AppLogic.Services;
using DTO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AppLogic
{
    public interface IPriceManager  
    {
        Task<string> ObtenerPrecioAsync(string symbol); 
    }

    public class PriceManager : IPriceManager
    {
        private readonly Price_Service _priceService;

        public PriceManager(IConfiguration configuration)
        {
            _priceService = new Price_Service(configuration);
        }

        // Cambié la firma del método para devolver un 'string' en lugar de 'TwelveDataResponse'
        public async Task<string> ObtenerPrecioAsync(string symbol)
        {
            return await _priceService.GetLastPriceInLastMinuteAsync(symbol);
        }
    }
}
