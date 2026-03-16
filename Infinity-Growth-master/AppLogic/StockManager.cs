using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using Microsoft.Extensions.Configuration;

namespace AppLogic
{
    public class StockManager
    {
        private readonly TwelveData_Service _twelveDataService;

        public StockManager(IConfiguration configuration)
        {
            _twelveDataService = new TwelveData_Service(configuration);
        }

        public async Task<TwelveDataResponse> ObtenerStockAsync(string simbolo, string range)
        {
            return await _twelveDataService.GetStockDataAsync<TwelveDataResponse>(simbolo, range);
        }
    }
}
