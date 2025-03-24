using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorySADEfficiencyOptimizer.Data;

public interface IInitializedDataProvider
{
    IEnumerable<SupplierUIValues> GetBoxValues();
    IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers);
    void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath);
    void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath);

}
