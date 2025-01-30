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
    IEnumerable<SupplierUIValues> GetBoxValues();
    IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers);
    void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues);
    void SaveShipmentInfo(IEnumerable<Shipment> shipments);

}
