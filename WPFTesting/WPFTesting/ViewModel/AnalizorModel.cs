using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;
using WPFTesting.ViewModel;

namespace FactorSADEfficiencyOptimizer.ViewModel;
public class AnalizorModel
{
	public int Duration { get; set; }
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
			// TODO: Add Dalins changes to Shipments HERE!!! It is missing some stuff
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

	public void PassTimeUntilDuration()
	{
		for (int i = 0; i < Duration; i++)
		{
			AdvanceTime();
		}
	}
}
