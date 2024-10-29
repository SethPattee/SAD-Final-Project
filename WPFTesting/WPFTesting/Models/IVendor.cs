using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public interface IVendor
{
    string Name { get; set; }
    Guid Id { get; set; }
    List<Product> Products { get; set; }
}
