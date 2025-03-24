using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorSADEfficiencyOptimizer.Data;

public class ForJsonShipment
{
	public List<Product> _products { get; set; }
	public Guid _senderId {  get; set; }
	public Guid _recieverId { get; set; }
	public string FromJoiningBoxCorner { get; set; }
	public string ToJoiningBoxCorner { get; set; }
	public Guid Id { get; set; }
    public ForJsonShipment()
    {
        _products = new List<Product>();
		_senderId = Guid.NewGuid();
		_recieverId = Guid.NewGuid();
		FromJoiningBoxCorner = String.Empty;
		ToJoiningBoxCorner = String.Empty;
		Id = Guid.NewGuid();
    }
    public ForJsonShipment(Shipment shipment) {
		_products = shipment.Products.ToList<Product>();
		_senderId = shipment.Sender.Id;
		_recieverId = shipment.Receiver.Id;
		FromJoiningBoxCorner = shipment.FromJoiningBoxCorner;
		ToJoiningBoxCorner = shipment.ToJoiningBoxCorner;
		Id = shipment.Id;
	}

}
public class FromJsonShipment
{
	public ForJsonShipment _shipment { get; set; }
	public FromJsonShipment(ForJsonShipment shipment)
    {
        _shipment = shipment;
    }
	public Shipment GetShipment(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{
		Shipment shipment = new Shipment();
		shipment.Id = _shipment.Id;
		shipment.Products = new ObservableCollection<Product>(_shipment._products);
		shipment.FromJoiningBoxCorner = _shipment.FromJoiningBoxCorner;
		shipment.ToJoiningBoxCorner = _shipment.ToJoiningBoxCorner;
		foreach (var endpoint in endpoints)
		{
			if (endpoint.Supplier.Id == _shipment._recieverId)
				shipment.Receiver = endpoint.Supplier;
			else if (endpoint.Supplier.Id == _shipment._senderId)
				shipment.Sender = endpoint.Supplier;
		}
		foreach (var supplier in suppliers)
		{
			if (supplier.Supplier.Id == _shipment._senderId)
				shipment.Sender = supplier.Supplier;
			else if (supplier.Supplier.Id == _shipment._recieverId)
				shipment.Receiver = supplier.Supplier;
		}
		return shipment;
	}
}
