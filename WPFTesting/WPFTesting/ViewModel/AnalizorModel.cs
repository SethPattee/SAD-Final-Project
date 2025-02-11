using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FactorSADEfficiencyOptimizer.Models;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;

namespace FactorSADEfficiencyOptimizer.ViewModel;
public class AnalizorModel
{
	public event PropertyChangedEventHandler? PropertyChanged;
	private ObservableCollection<EndpointUIValues> _endpointList = new ObservableCollection<EndpointUIValues>();
	public ObservableCollection<EndpointUIValues> EndpointList
	{
		get => _endpointList;
		set
		{
			_endpointList = value;
			OnPropertyChanged(nameof(EndpointList));
		}
	}

	private ObservableCollection<Shipment> _shipmentList = new ObservableCollection<Shipment>();
	public ObservableCollection<Shipment> ShipmentList
	{
		get { return _shipmentList; }
		set
		{
			_shipmentList = value;
			OnPropertyChanged(nameof(ShipmentList));
		}
	}

	public string ShortestPath;


	private ObservableCollection<SupplierUIValues> _supplierList = new ObservableCollection<SupplierUIValues>();
	public ObservableCollection<SupplierUIValues> SupplierList
	{
		get => _supplierList;
		set
		{
			_supplierList = value;
			OnPropertyChanged(nameof(SupplierList));
		}
	}
    private ObservableCollection<ProductionTarget> _productionTargets = new ObservableCollection<ProductionTarget>();
	public ObservableCollection<ProductionTarget> ProductionTargets
	{
		get => _productionTargets;
		set
		{
			_productionTargets = value;
			OnPropertyChanged(nameof(ProductionTargets));
		}
	}
	private int _currentDay = 1;
	public int CurrentDay
	{
		get => _currentDay;
		set {
			_currentDay = value;
			OnPropertyChanged(nameof(CurrentDay));
		}
	}
    public AnalizorModel(SupplyChainViewModel model)
	{
		ShortestPath = "";
		foreach (var supplier in model.SupplierList)
		{
			SupplierUIValues sup = new SupplierUIValues();
			sup.supplier = new Supplier();
			sup.supplier.Name = supplier.supplier.Name;
			sup.supplier.ComponentInventory = supplier.supplier.ComponentInventory;
			sup.supplier.ProductInventory = supplier.supplier.ProductInventory;
			sup.supplier.Id = supplier.supplier.Id;
			sup.Position = new System.Drawing.Point();
			AddSupplier(sup);
		}
		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			EndpointUIValues end = new EndpointUIValues();
			end.supplier = new EndpointNode();
			end.supplier.Name = endpoint.supplier.Name;
			end.supplier.ComponentInventory = endpoint.supplier.ComponentInventory;
			end.supplier.ProductInventory = endpoint.supplier.ProductInventory;
			end.supplier.Id = endpoint.supplier.Id;
			end.Position = new System.Drawing.Point();
			((EndpointNode)end.supplier).DeliveryRequirementsList = ((EndpointNode)endpoint.supplier).DeliveryRequirementsList;
			((EndpointNode)end.supplier).ProductionList = ((EndpointNode)endpoint.supplier).ProductionList;
			((EndpointNode)end.supplier).Profit = ((EndpointNode)endpoint.supplier).Profit;
			AddEndpoint(end);
		}
		foreach (Shipment shipment in model.ShipmentList) {
			Shipment ship = new Shipment();
			ship.Sender = SupplierList.FirstOrDefault(s => s.supplier.Id == shipment.Sender.Id)?.supplier ?? new Supplier();
			if (shipment.Sender is EndpointNode endpoint)
			{
				ship.Receiver = EndpointList.FirstOrDefault(e => e.supplier.Id == shipment.Receiver.Id)?.supplier ?? new EndpointNode();
			}
			else
			{
				ship.Receiver = SupplierList.FirstOrDefault(s => s.supplier.Id == shipment.Receiver.Id)?.supplier ?? new Supplier();
			}
			ship.Products = shipment.Products;
			ship.ToJoiningBoxCorner = "";
			ship.FromJoiningBoxCorner = "";
			// TODO: Add Dallins changes to Shipments HERE!!! It is missing some stuff
			AddShipment(ship);
		}
	}

	protected void OnPropertyChanged(string? name = null)
	{
		if (name is not null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public void AddEndpoint(EndpointUIValues endpoint)
	{
		EndpointList.Add(endpoint);
		OnPropertyChanged(nameof(EndpointList));
	}

	public void AddSupplier(SupplierUIValues supplier)
	{
		SupplierList.Add(supplier);
		OnPropertyChanged(nameof(SupplierList));
	}
	public void AddShipment(Shipment shipment)
	{
		ShipmentList.Add(shipment);
		OnPropertyChanged(nameof(ShipmentList));
	}
	public void AdvanceTime()
	{
		ToolsForViewModel.AdvanceTimeforShipmentList(ShipmentList);
		ToolsForViewModel.AdvancetimeForSupplierList(SupplierList);
		ToolsForViewModel.AdvanceTimeForEndpointList(EndpointList);
	}

	public void PassTimeUntilDuration(int duration)
	{
		for (int i = 0; i < duration; i++)
		{
			AdvanceTime();
			UpdateProducitonTargets();
		}
	}
	private void UpdateProducitonTargets()
	{
		foreach (var target in ProductionTargets)
		{
			
		}
	}
	public int GetProductsNeededPerDay(ProductionTarget newtarg, Product product)
	{
		double neededQuant = newtarg.TargetQuantity - product.Quantity;
		if (neededQuant > 0)
		{
			double perDayQuant = neededQuant / (newtarg.DueDate - CurrentDay);
			return (int)Math.Ceiling(perDayQuant);
		}
		else
			return 0;
	}
	public List<Product> GetNeededComponentQuantitiesForTarget(ProductionTarget newtarg, ProductLine productLine)
	{
		List<Product> products = new List<Product>();
		foreach (Product p in productLine.Components)
		{
			Product qp = new Product();
			qp.ProductName = p.ProductName;
			qp.Price = p.Price;
			qp.Quantity = p.Quantity * newtarg.TargetQuantity;
			products.Add(qp);
		}
		return products;
	}
	public void OrderMissingComponents()
	{
		foreach (ProductionTarget target in ProductionTargets)
		{
			EndpointNode endpointNode = new EndpointNode();
			ProductLine productLine = new ProductLine();
			foreach (var endpoint in EndpointList)
			{
				var posibleLine = ((EndpointNode)endpoint.supplier).ProductionList
					.FirstOrDefault(pl => pl.ResultingProduct.ProductName == target.ProductTarget?.ProductName);
				if (posibleLine is not null)
				{
					endpointNode = (EndpointNode)endpoint.supplier;
					productLine = (ProductLine)posibleLine;
					List<Product> products = GetNeededComponentQuantitiesForTarget(target, productLine).ToList();
					foreach (Product product in products)
					{
						float Quant = endpoint.supplier.ComponentInventory.FirstOrDefault(p => p.ProductName == product.ProductName)?.Quantity ?? 0;
						if (!(Quant >= product.Quantity))
						{
							Product toOrder = new Product();
							toOrder.ProductName = product.ProductName;
							toOrder.Price = product.Price;
							toOrder.Quantity = product.Quantity - Quant;
							PlaceOrderFor(product, endpoint);
						}
					}
					break;
				}
			}

		}
	}
	public void PlaceOrderFor(Product product, EndpointUIValues endpoint)
	{
		//find a supplier with the product (enough of the product)
		Supplier supplier = 
			(Supplier)(SupplierList.FirstOrDefault(s => s.supplier.ProductInventory
				.FirstOrDefault(p => 
					p.ProductName == product.ProductName 
					&& p.Quantity >= product.Quantity) 
			!= null)?.supplier ?? new Supplier());

		//create shippingline that has some of the product 
		Shipment shipment = new Shipment()
		{
			Sender = supplier,
			Receiver = endpoint.supplier,
			Products = new ObservableCollection<Product>() { product }
		};
		ShipmentList.Add(shipment);
	}
}
