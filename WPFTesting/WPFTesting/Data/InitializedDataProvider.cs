using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.Models;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using YourNamespace;
using FactorySADEfficiencyOptimizer.Data;
using Microsoft.Win32;

namespace FactorySADEfficiencyOptimizer.Data;


public class InitializedDataProvider : IInitializedDataProvider
{
    private IEnumerable<SupplierUIValues> _Suppliers;
    public InitializedDataProvider()
    {
        _Suppliers = GenneratedFirstSuppliers.InitialSuppliers();
        try
        {
            var jString = File.ReadAllText("Suppliers.json");
            var jString_endpoint = File.ReadAllText("Endpoints.json");
            if (jString == null || jString == "")
            {
                _Suppliers = GenneratedFirstSuppliers.InitialSuppliers();
            }
            else
            {
                IEnumerable<ForJsonSupplier> forSuppliers = JsonConvert.DeserializeObject<IEnumerable<ForJsonSupplier>>(jString);
                IEnumerable<ForJsonEndpoint> forEndpoints = JsonConvert.DeserializeObject<IEnumerable<ForJsonEndpoint>>(jString_endpoint);
                List<SupplierUIValues> toUiEndpointsAndSuppliers = new List<SupplierUIValues>();
                foreach (var s in forSuppliers)
                {
                    FromJsonSuplier fs = new FromJsonSuplier(s);
                    toUiEndpointsAndSuppliers.Add((SupplierUIValues)fs.Supplier);
                }
                foreach (var item in forEndpoints)
                {
                    FromJsonEndpoint fe = new FromJsonEndpoint(item);
                    toUiEndpointsAndSuppliers.Add((EndpointUIValues)fe.Supplier);
                }
                _Suppliers = (IEnumerable<SupplierUIValues>)toUiEndpointsAndSuppliers;
            }
        }
        catch (Exception E) { Console.WriteLine(E); }
    }
    public IEnumerable<SupplierUIValues> GetBoxValues()
    {
        return _Suppliers;
    }

	public IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{

        List<Shipment> shipments = new List<Shipment>();
        try
        {

            var jString = File.ReadAllText("Shipments.json");
            IEnumerable<ForJsonShipment>? forShipments = JsonConvert.DeserializeObject<IEnumerable<ForJsonShipment>>(jString);
            foreach (ForJsonShipment ship in forShipments ?? new List<ForJsonShipment>())
            {
                shipments.Add(new FromJsonShipment(ship).GetShipment(endpoints, suppliers));
            }
        }
        catch (Exception E) { Console.WriteLine("couldn't read shipments from file"); }
		return shipments;
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string filePath)
    {
        List<ForJsonShipment> shipments1 = new List<ForJsonShipment>();
        foreach (var shipment in shipments)
        {
            shipments1.Add(new ForJsonShipment(shipment));
        }

        var jString = System.Text.Json.JsonSerializer.Serialize(shipments1);

        try
        {
            // Save to the provided file path
            File.WriteAllText(filePath, jString);
			// Save to the path read from on start up
			File.WriteAllText("Shipments.json", jString);

		}
		catch (Exception e)
        {
            Console.WriteLine(e.Message); // Handle the error, could also show a message to the user
        }
    }


    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string filePath)
    {
        List<ForJsonSupplier> forJsonSupliers = new List<ForJsonSupplier>();
        List<ForJsonEndpoint> forJsonEndpoints = new List<ForJsonEndpoint>();

        foreach (var value in supplierUIValues)
        {
            if (!(value.GetType() == typeof(EndpointUIValues)))
            {
                forJsonSupliers.Add(new ForJsonSupplier(value));
            }
            else if (value is EndpointUIValues endpoint)
            {
                forJsonEndpoints.Add(new ForJsonEndpoint(endpoint));
            }
        }

        var jString = System.Text.Json.JsonSerializer.Serialize(forJsonSupliers);
        var jString_endpoint = System.Text.Json.JsonSerializer.Serialize(forJsonEndpoints);

        try
        {
            // Save to the provided file path for suppliers and endpoints
            string path = Path.GetDirectoryName(filePath) ?? "";
            string fileName = Path.GetFileNameWithoutExtension(filePath);

			File.WriteAllText(Path.Combine(path, fileName + "_Suppliers.json"), jString);
			File.WriteAllText(Path.Combine(path, fileName + "_Endpoints.json"), jString_endpoint);
			// Save to the path read from on Start up. ( could be the same path)
			File.WriteAllText("Suppliers.json", jString);
			File.WriteAllText("Endpoints.json", jString_endpoint);
		}
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); 
        }
    }



}

