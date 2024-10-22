using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Shapes;
using WPFTesting.Models;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace WPFTesting.Data;


public class InitializedDataProvider : IInitializedDataProvider
{
    private IEnumerable<SupplierUIValues> _Suppliers;
    public InitializedDataProvider()
    {
        _Suppliers = GenneratedFirstSuppliers.InitialSuppliers();
        try
        {
            var jString = File.ReadAllText("Suppliers.json");
            //_Suppliers = JsonConvert.DeserializeObject<IEnumerable<SupplierUIValues>>(jString);
        }
        catch (Exception E) { Console.WriteLine(E); }
    }
    public async Task<IEnumerable<SupplierUIValues>?> GetBoxValuesAsync()
    {
        await Task.Delay(0);
        return _Suppliers;
    }

    public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
    {
        await Task.Delay(100);

        return new List<Supplier> { };
    }

    public void SaveSupplierInfoAsync(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        var jString = JsonSerializer.Serialize(supplierUIValues);
        try
        {
            //TODO: NEED an environment variable that has the path, or be happy diving into the bin/debug/.... stuff every time. 
            //string relativePath = "Data//Suppliers.json";

            //string s = AppDomain.CurrentDomain.BaseDirectory;
            //string path = Environment.CurrentDirectory;
            //string q = Path.GetFullPath(relativePath);
            //string p = Path.GetDirectoryName("");
            File.WriteAllText("Suppliers.json", jString);
        }
        catch (Exception ex) { 
            Console.WriteLine(ex.Message);
    }

    }
}
