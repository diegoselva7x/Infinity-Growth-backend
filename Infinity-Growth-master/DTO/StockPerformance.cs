using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class StockPerformance
    {
        public string Symbol { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal GetPercentageChange()
        {
            if (OpenPrice == 0)
            {
                return 0;
            }

            return ((CurrentPrice - OpenPrice) / OpenPrice) * 100;
        }
    }
}
