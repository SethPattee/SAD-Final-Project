using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting.Data;

public interface IInitializedDataProvider
{
    IEnumerable<SupplierUIValues> GetBoxValuesAsync();
    void SaveSupplierInfoAsync(IEnumerable<SupplierUIValues> supplierUIValues);
    //Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
}
