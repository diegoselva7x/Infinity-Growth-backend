using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class API_Response
    {
        public string Result { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
