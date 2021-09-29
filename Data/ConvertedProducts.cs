using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCurrency.Data
{
    public class ConvertedProducts
    {
        public int ProductsId { get; set; }
        public string Title { get; set; }
        public string OldCurrency { get; set; }
        public float Cost { get; set; }
        public string Currency { get; set; }
        public float ConvertedCost { get; set; }
    }
}
