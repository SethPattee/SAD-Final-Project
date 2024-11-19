using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Shapes;

namespace WPFTesting.Components
{
    public interface INodeElement
    {
        SupplierUIValues NodeUIValues { get; set; }
    }
}
