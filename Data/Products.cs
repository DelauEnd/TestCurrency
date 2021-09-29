using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace TestCurrency.Data
{
    public class Products
    {
        [Key]
        public int ProductsId { get; set; }
        public String Title { get; set; }
        public float Cost { get; set; }

        public int? CategoryId { get; set; }
        public Categories Category { get; set; }
    }
} 
