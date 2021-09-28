using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestCurrency.Data
{
    public class Categories
    {
        [Key]
        public int CategoriesId { get; set; }
        public String Title { get; set; }

        public ICollection<Products> Products { get; set; }
        public Categories()
        {
            Products = new List<Products>();
        }
    }
}
