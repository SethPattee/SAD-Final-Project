using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Shapes;

namespace WPFTesting.Data;

public interface IInitializedDataProvider
{
    Task<IEnumerable<SupplierUIValues>?> GetBoxValuesAsync();
}
