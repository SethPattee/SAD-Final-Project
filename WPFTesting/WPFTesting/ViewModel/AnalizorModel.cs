﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FactorSADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using Microsoft.Extensions.Logging;

namespace FactorSADEfficiencyOptimizer.ViewModel;
public class AnalizorModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	private ObservableCollection<Product> _daysUsedComponents = new ObservableCollection<Product>();
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
	private ObservableCollection<Change> _changeLog = new ObservableCollection<Change>();
	public ObservableCollection<Change> ChangeLog
	{
		get { return _changeLog; }
		set
		{
			_changeLog = value;
			OnPropertyChanged(nameof(ChangeLog));
		}
	}
	private ObservableCollection<Issue> _issueLog = new ObservableCollection<Issue>();
	public ObservableCollection<Issue> IssueLog
	{
		get { return _issueLog; }
		set
		{
			_issueLog = value;
			OnPropertyChanged(nameof(IssueLog));
		}
	}
	private ObservableCollection<Snapshot> _snapshot = new ObservableCollection<Snapshot>();
	public ObservableCollection<Snapshot> Snapshots
	{
		get { return _snapshot; }
		set { _snapshot = value; OnPropertyChanged(nameof(Snapshot)); }
	}
	private int _currentDay = 1;
	public int CurrentDay
	{
		get => _currentDay;
		set
		{
			_currentDay = value;
			OnPropertyChanged(nameof(CurrentDay));
		}
	}
	private double _daysToRun = 1;
	public double DaysToRun
	{
		get => _daysToRun;
		set
		{
			_daysToRun = value;
			OnPropertyChanged(nameof(DaysToRun));
		}
	}

	public AnalizorModel(SupplyChainViewModel model)
	{
		ShortestPath = "";
		foreach (var supplier in model.SupplierList)
		{
			SupplierUIValues sup = CopyMaker.makeShallowCopySupplier(supplier);
			AddSupplier(sup);
		}
		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			EndpointUIValues end = CopyMaker.makeShallowCopyEndpoint(endpoint);
			foreach (ProductLine pl in ((EndpointNode)end.supplier).ProductionList)
			{
				if (pl.IsEnabled)
				{
					ProductionTargets.Add(
						new ProductionTarget()
						{
							DueDate = 1,
							IsTargetEnabled = true,
							TargetQuantity = pl.ResultingProduct.Quantity,
							ProductTarget = CopyMaker.makeShallowCopyProduct(pl.ResultingProduct),
							CurrentAmount = (end.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == pl.ResultingProduct.ProductName) ?? new Product()).Quantity,
							Status = StatusEnum.NotDone
						}
					);
				}
			}
			AddEndpoint(end);
		}
		foreach (Shipment shipment in model.ShipmentList) {
            Shipment ship = makeShallowCopyShipment(shipment);
            AddShipment(ship);
		}
		Snapshots.Add(MakeCurrentSnapShot());
	}
    private Shipment makeShallowCopyShipment(Shipment shipment)
    {
        Shipment ship = GetShipmentWithSenderReciver(shipment);
        ship.Products = CopyMaker.makeShallowCopyOfProductColection(shipment.Products);
        ship.ToJoiningBoxCorner = "";
        ship.FromJoiningBoxCorner = "";
        return ship;
    }
    public Snapshot MakeCurrentSnapShot()
	{
		Snapshot snapshot = new Snapshot();
		foreach (var supplier in SupplierList)
		{
			SupplierUIValues sup = CopyMaker.makeShallowCopySupplier(supplier);
			snapshot.Suppliers.Add(supplier);
		}
		foreach (EndpointUIValues endpoint in EndpointList)
		{
			EndpointUIValues end = CopyMaker.makeShallowCopyEndpoint(endpoint);
			snapshot.Endpoints.Add(end);
		}
		foreach (Shipment shipment in ShipmentList)
		{
			Shipment ship = GetShipmentWithSenderReciver(shipment);
			ship.Products = CopyMaker.makeShallowCopyOfProductColection(shipment.Products);
			ship.ToJoiningBoxCorner = "";
			ship.FromJoiningBoxCorner = "";
			snapshot.Shipments.Add(ship);
		}
		foreach (ProductionTarget target in ProductionTargets)
        {
            ProductionTarget targCopy = CopyMaker.makeShallowCopyProductionTarget(target);
            snapshot.Targets.Add(targCopy);
        }
        snapshot.ComponentsUsed = CopyMaker.makeShallowCopyOfProductColection(_daysUsedComponents).ToList();
		_daysUsedComponents.Clear();
		return snapshot;
	}

    

    private Shipment GetShipmentWithSenderReciver(Shipment shipment)
	{
		Shipment ship = new Shipment();
		ship.Sender = SupplierList.FirstOrDefault(s => s.supplier.Id == shipment.Sender.Id)?.supplier ?? new Supplier();
		if (shipment.Receiver is EndpointNode endpoint)
		{
			ship.Receiver = EndpointList.FirstOrDefault(e => e.supplier.Id == shipment.Receiver.Id)?.supplier ?? new EndpointNode();
		}
		else
		{
			ship.Receiver = SupplierList.FirstOrDefault(s => s.supplier.Id == shipment.Receiver.Id)?.supplier ?? new Supplier();
		}
		return ship;
	}

	public static ObservableCollection<ProductionTarget> makeShallowCopyColectionOfProductionTargets(ObservableCollection<ProductionTarget> targets)
    {
        var newTargets = new ObservableCollection<ProductionTarget>();
        foreach (var target in targets)
        {
            var newtarg = new ProductionTarget();
            newtarg.ProductTarget = CopyMaker.makeShallowCopyProduct(target.ProductTarget ?? new Product());
            newtarg.CurrentAmount = target.CurrentAmount;
            newtarg.DueDate = target.DueDate;
            StatusEnum temStatus;
            switch (target.Status)
            {
                case (StatusEnum.Success):
                    temStatus = StatusEnum.Success;
                    break;
                case (StatusEnum.Failure):
                    temStatus = StatusEnum.Failure;
                    break;
                case (StatusEnum.Warning):
                    temStatus = StatusEnum.Warning;
                    break;
                default:
                    temStatus = StatusEnum.NotDone;
                    break;
            }
            newtarg.Status = temStatus;
            newtarg.IsTargetEnabled = target.IsTargetEnabled;
        }
        return newTargets;
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

	public void PassTimeUntilDuration(double duration)
	{
		ResetStatetoFirstSnapShot();
		OrderMissingComponents();
		for (double i = 0; i < duration; i++)
		{
			AdvanceTime();
			RecordUsedComponents();
			UpdateProducitonTargets();
			Snapshots.Add(MakeCurrentSnapShot());
			CurrentDay++;
		}
		UpdateProducitonTargets(isLastGo: true);
	}
    private void ResetStatetoFirstSnapShot()
    {
        Snapshot initValues = Snapshots[0];
        EndpointList.Clear();
        foreach (var end in initValues.Endpoints)
        {
            EndpointList.Add(CopyMaker.makeShallowCopyEndpoint(end));
        }
        SupplierList.Clear();
        foreach (var sup in initValues.Suppliers)
        {
            SupplierList.Add(CopyMaker.makeShallowCopySupplier(sup));
        }
        ShipmentList.Clear();
        foreach (var shipment in initValues.Shipments)
        {
            ShipmentList.Add(makeShallowCopyShipment(shipment));
        }
        // we want to keep produciton targets as is
        CurrentDay = 1;
        ChangeLog.Clear();
        IssueLog.Clear();
        Snapshots.Clear();
        Snapshots.Add(initValues);
        foreach (ProductionTarget targ in ProductionTargets)
        {
            targ.Status = StatusEnum.NotDone;
        }
    }
    private void RecordUsedComponents()
	{
		foreach (EndpointUIValues end in EndpointList)
		{
			ObservableCollection<Product> toUse = ((EndpointNode)end.supplier).DaysUsedComponents;
			foreach (var component in toUse)
			{
                var existingProd = _daysUsedComponents.FirstOrDefault(p => p.ProductName == component.ProductName);
				if (existingProd != null)
				{
					existingProd.Quantity += component.Quantity;
				}
				else
				{
					_daysUsedComponents.Add(component);
				}
            }
		}
	}
	private void UpdateProducitonTargets(bool isLastGo = false)
	{
		foreach (var target in ProductionTargets)
		{
			EndpointUIValues? endpoint = EndpointList.FirstOrDefault(e => e.supplier.ProductInventory.Where(p => p.ProductName == target?.ProductTarget?.ProductName) is not null);
			if (endpoint != null)
			{
				Product prod = endpoint.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == target?.ProductTarget?.ProductName) ?? new Product();
				target.CurrentAmount = prod.Quantity;
				if (target.CurrentAmount >= target.TargetQuantity)
				{
					target.Status = StatusEnum.Success;
					OnPropertyChanged(nameof(target.Status));
					var pl = (((EndpointNode)endpoint.supplier).ProductionList.FirstOrDefault(pl => pl.ResultingProduct.ProductName == prod.ProductName) ?? new ProductLine());
					pl.IsEnabled = false;
					OnPropertyChanged(nameof(pl));
					//TODO: add the profit made from the target here
				}
				else if (isLastGo)
				{
					target.Status = StatusEnum.Failure;
					OnPropertyChanged(nameof(target.Status));
				}
			}
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
	public ObservableCollection<Product> GetNeededComponentQuantitiesForTarget(ProductionTarget newtarg, ProductLine productLine)
	{
		ObservableCollection<Product> products = new ObservableCollection<Product>();
		foreach (Product p in productLine.Components)
		{
			Product qp = new Product();
			qp.ProductName = p.ProductName;
			qp.Price = p.Price;
			qp.Quantity = (float)(p.Quantity * newtarg.TargetQuantity);
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
							Issue issue = new Issue()
							{
								DayFound = 1,
								Severity = StatusEnum.Warning,
								Solution = new Change()
								{
									Action = ActionEnum.addedShipment,
									neededProduct = toOrder,
								}
							};
							IssueLog.Add(issue);
							ChangeLog.Add(new Change() { Action = ActionEnum.addedShipment, neededProduct = toOrder, shipmentReceiver = endpoint });
							PlaceOrderFor(toOrder, endpoint);
						}
					}
					break;
				}
			}

		}
	}
	public void PlaceOrderFor(Product product, EndpointUIValues endpoint)
	{
		//find a supplier with enough of the product
		Supplier? supplier =
			(Supplier?)(SupplierList.FirstOrDefault(s => s.supplier.ProductInventory
				.FirstOrDefault(p =>
					p.ProductName == product.ProductName
					&& p.Quantity >= product.Quantity)
			!= null)?.supplier);

		if (supplier != null)
		{
			//create shippingline that has all of the product 
			Shipment shipment = new Shipment()
			{
				Sender = supplier,
				Receiver = endpoint.supplier,
				Products = new ObservableCollection<Product>() { product }
			};
			ShipmentList.Add(shipment);
		}
		else
		{
			float TotalRemamingNeeded = product.Quantity;
			//order as much of the product as we can
			foreach (SupplierUIValues sup in SupplierList)
			{
				Product? productWithSmallQuantity = sup.supplier.ProductInventory.FirstOrDefault(p => p.ProductName == product.ProductName && p.Quantity > 0);
				if (productWithSmallQuantity != null)
				{
					Shipment shipment = new Shipment()
					{
						Sender = sup.supplier,
						Receiver = endpoint.supplier,
						Products = new ObservableCollection<Product>() { productWithSmallQuantity }
					};
					ShipmentList.Add(shipment);
					TotalRemamingNeeded -= productWithSmallQuantity.Quantity;
				}
			}
		}
	}

	public double[] GetQuantityPerDayForGraph(string ItemName)
	{
		return Snapshots.Where(x =>
		{
			return x.Targets.Exists(x =>
			{
				if (x.ProductTarget is not null)
					return x.ProductTarget.ProductName == ItemName;
				else return false;
			});
		})
		.Select(x => (double)x.Targets.Sum(y => y.TargetQuantity)).ToArray();
	}

	public double[] GetBalancePerDayForGraph()
	{
		return Snapshots.Select<Snapshot, double>(x =>
		{
			return (double)x.Endpoints.Sum(item => ((EndpointNode)item.supplier).Balance) / 1000;
		}).ToArray();
	}

	public List<ProductionTarget?> ExtractProductionTargetChanges(string name)
	{
		var targetlist = new List<ProductionTarget?>();
		foreach(var snapshot in Snapshots)
		{
			foreach(var target in snapshot.Targets)
			{
				if(target.ProductTarget!.ProductName == name)
					targetlist.Add(target);
			}
		}
		
		return targetlist;

		//return Snapshots.ToList((x => x.Targets.Find(y => (y.ProductTarget ?? throw new ArgumentNullException("Product target list is null!")).ProductName == name)).ToList();
	}

	public double GetDayCompletedFor(string product_name)
	{
		try
		{
			var item = Snapshots?.Where(x => x.Targets.Exists(x => x.ProductTarget?.ProductName == product_name)).LastOrDefault()?.Targets.FirstOrDefault()?.DayCompleted;
			return (double)item!;
		}
		catch (Exception ex)
		{
			{
				Console.WriteLine("Finding day completed ran aground on a null reference.\n", ex.Message);
			}
			return -1;
		}
	}
}
