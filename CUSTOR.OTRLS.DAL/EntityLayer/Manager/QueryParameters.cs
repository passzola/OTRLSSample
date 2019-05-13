using System;
using System.Collections.Generic;
using System.Text;

namespace CUSTOR.OTRLS.Core 
{
    public class QueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int CustomerId { get; set; } = 1;
        public int PageCount  { get; set; } = 10;
        public string Lang { get; set; }
    }
}
