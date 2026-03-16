using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TwelveDataResponse
    {
        public string Status { get; set; }
        public MetaData Meta { get; set; }
        public List<TimeSeriesData> Values { get; set; }
        public string LastClosePrice { get; set; }
    }


}

