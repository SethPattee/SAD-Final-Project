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
using Newtonsoft.Json;

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
            if (jString == null || jString == "")
            { _Suppliers = GenneratedFirstSuppliers.InitialSuppliers(); }
            else
            {
                _Suppliers = JsonConvert.DeserializeObject<IEnumerable<SupplierUIValues>>(jString);
            }
        }
        catch (Exception E) { Console.WriteLine(E); }
    }
    public IEnumerable<SupplierUIValues> GetBoxValuesAsync()
    {
        return _Suppliers;
    }

    public void SaveSupplierInfoAsync(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        var jString = System.Text.Json.JsonSerializer.Serialize(supplierUIValues);
        try
        {
            //TODO: NEED an environment variable that has the path, or be happy diving into the bin/debug/.... stuff every time. 
            // for now, it is saving down SAD-Final-Project\WPFTesting\WPFTesting\bin\Debug\net8.0-windows7.0\Suppliers.json
            File.WriteAllText("Suppliers.json", jString);
        }
        catch (Exception ex) { 
            Console.WriteLine(ex.Message);
    }

    }
}
