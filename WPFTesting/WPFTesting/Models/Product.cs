using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models
{
    public class Product
    {
        public float Quantity { get; set; }
        public string ProductName { get; set; } = "";
        public string Units { get; set; } = "";
        public decimal Price { get; set; }
    }
}
