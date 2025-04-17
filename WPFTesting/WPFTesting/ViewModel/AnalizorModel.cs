using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;
using Microsoft.Extensions.Logging;

namespace FactorySADEfficiencyOptimizer.ViewModel;
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
		ObservableCollection<string> _PosibletargetNames = new();
		foreach (var supplier in model.SupplierList)
		{
			SupplierUIValues sup = supplier.ShallowCopy();
			AddSupplier(sup);
		}
		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			EndpointUIValues end = endpoint.ShallowCopy();
			foreach (ProductLine pl in ((EndpointNode)end.Supplier).ProductionList)
			{
				if (pl.IsEnabled)
				{
					ProductionTarget toAdd = new ProductionTarget()
					{
						DueDate = 1,
						IsTargetEnabled = true,
						TargetQuantity = pl.ResultingProduct.Quantity,
						ProductTarget = pl.ResultingProduct.ShallowCopy(),
						CurrentAmount = (end.Supplier.ProductInventory.FirstOrDefault(p => p.ProductName == pl.ResultingProduct.ProductName) ?? new Product()).Quantity,
						Status = StatusEnum.NotDone
					};
					ProductionTargets.Add(toAdd);
					_PosibletargetNames.Add(pl.ResultingProduct.ProductName);
				}
			}
				AddEndpoint(end);
		}
		foreach (Shipment shipment in model.ShipmentList) {
            Shipment ship = makeShallowCopyShipment(shipment);
            AddShipment(ship);
		}
		foreach (ProductionTarget targ in ProductionTargets)
		{
			targ.PosibleTargetNames = _PosibletargetNames;
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
			SupplierUIValues sup = supplier.ShallowCopy();
			snapshot.Suppliers.Add(supplier);
		}
		foreach (EndpointUIValues endpoint in EndpointList)
		{
			EndpointUIValues end = endpoint.ShallowCopy();
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
			ProductionTarget targCopy = target.ShallowCopy();
            snapshot.Targets.Add(targCopy);
        }
        snapshot.ComponentsUsed = CopyMaker.makeShallowCopyOfProductColection(_daysUsedComponents).ToList();
		_daysUsedComponents.Clear();
		return snapshot;
	}

    

    private Shipment GetShipmentWithSenderReciver(Shipment shipment)
	{
		Shipment ship = new Shipment();
		ship.Sender = SupplierList.FirstOrDefault(s => s.Supplier.Id == shipment.Sender.Id)?.Supplier ?? new Supplier();
		if (shipment.Receiver is EndpointNode endpoint)
		{
			ship.Receiver = EndpointList.FirstOrDefault(e => e.Supplier.Id == shipment.Receiver.Id)?.Supplier ?? new EndpointNode();
		}
		else
		{
			ship.Receiver = SupplierList.FirstOrDefault(s => s.Supplier.Id == shipment.Receiver.Id)?.Supplier ?? new Supplier();
		}
		return ship;
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
		ToolsForViewModel.FillCurrentPriceInShipments(SupplierList, ShipmentList);
		ToolsForViewModel.AdvanceTimeforShipmentList(ShipmentList);
		ToolsForViewModel.AdvancetimeForSupplierList(SupplierList);
		ToolsForViewModel.AdvanceTimeForEndpointList(EndpointList);
	}

	public void PassTimeUntilDuration(double duration)
	{
		ResetStatetoFirstSnapShot();
		OnlyEnableProductlinesWithProductTargets();
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
	private void OnlyEnableProductlinesWithProductTargets()
	{
		foreach (var endpoint in EndpointList)
		{
			foreach (var productline in ((EndpointNode)endpoint.Supplier).ProductionList)
			{
				
				string Pname = productline.ResultingProduct.ProductName;
				ProductionTarget? prodT = ProductionTargets.FirstOrDefault(pt => pt.ProductTarget?.ProductName == Pname);
				if (prodT != null) {
					productline.IsEnabled = true;
				}
				else
				{
					productline.IsEnabled = false;
				}
			}
		}
	}
	private void ResetStatetoFirstSnapShot()
    {
        Snapshot initValues = Snapshots[0];
        EndpointList.Clear();
        foreach (var end in initValues.Endpoints)
        {
            EndpointList.Add(end.ShallowCopy());
        }
        SupplierList.Clear();
        foreach (var sup in initValues.Suppliers)
        {
            SupplierList.Add(sup.ShallowCopy());
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
			targ.PalacedAutoOrderForComponent = false;
			targ.AddToProducedQuantity(-targ.ProducedSoFar);
        }
    }
    private void RecordUsedComponents()
	{
		foreach (EndpointUIValues end in EndpointList)
		{
			ObservableCollection<Product> toUse = ((EndpointNode)end.Supplier).DaysUsedComponents;
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
			EndpointUIValues? endpoint = EndpointList.FirstOrDefault(e => e.Supplier.ProductInventory.Where(p => p.ProductName == target?.ProductTarget?.ProductName) is not null);
			if (endpoint != null)
			{
				Product prod = endpoint.Supplier.ProductInventory.FirstOrDefault(p => p.ProductName == target?.ProductTarget?.ProductName) ?? new Product();
				target.AddToProducedQuantity(prod.RecentlyAddedQuantity);
				target.CurrentAmount = prod.Quantity;
				if (target.ProducedSoFar >= target.TargetQuantity)
				{
                    if (target.Status == StatusEnum.NotDone || target.Status == StatusEnum.Failure)
                    {
                        target.DayCompleted = CurrentDay;
                    }
                    if (!target.PalacedAutoOrderForComponent)
					{
						target.Status = StatusEnum.Success;
					}
					else
					{
                        target.Status = StatusEnum.Warning;
                    }
					
                    OnPropertyChanged(nameof(target.Status));
					var pl = (((EndpointNode)endpoint.Supplier).ProductionList.FirstOrDefault(pl => pl.ResultingProduct.ProductName == prod.ProductName) ?? new ProductLine());
					pl.IsEnabled = false;
					OnPropertyChanged(nameof(pl));
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
		List<KeyValuePair<EndpointUIValues, Product>> _orderedProducts = new List<KeyValuePair<EndpointUIValues, Product>>();
		foreach (ProductionTarget target in ProductionTargets)
		{
			EndpointNode endpointNode = new EndpointNode();
			ProductLine productLine = new ProductLine();
			foreach (var endpoint in EndpointList)
			{
				var posibleLine = ((EndpointNode)endpoint.Supplier).ProductionList
					.FirstOrDefault(pl => pl.ResultingProduct.ProductName == target.ProductTarget?.ProductName);
				if (posibleLine is not null)
				{
					endpointNode = (EndpointNode)endpoint.Supplier;
					productLine = (ProductLine)posibleLine;
					List<Product> products = GetNeededComponentQuantitiesForTarget(target, productLine).ToList();
					foreach (Product product in products)
					{
						float Quant = endpoint.Supplier.ComponentInventory.FirstOrDefault(p => p.ProductName == product.ProductName)?.Quantity ?? 0;
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
								},
								ProductionTarget = target.ShallowCopy()
							};
							IssueLog.Add(issue);
							ChangeLog.Add(new Change() { Action = ActionEnum.addedShipment, neededProduct = toOrder, shipmentReceiver = endpoint });
							if (_orderedProducts.Where(op => op.Key.Supplier.Name == endpoint.Supplier.Name && op.Value.ProductName == toOrder.ProductName).ToList().Count() >= 1)
							{
								_orderedProducts.Where(op => op.Key.Supplier.Name == endpoint.Supplier.Name && op.Value.ProductName == toOrder.ProductName).First().Value.Quantity += toOrder.Quantity += Quant;
							}
							else
							{
								_orderedProducts.Add(new KeyValuePair<EndpointUIValues, Product>(endpoint, toOrder) );
							}
							target.PalacedAutoOrderForComponent = true;
						}
					}
					break;
				}
			}

		}
		foreach (var prodToOrder in _orderedProducts)
		{
			PlaceOrderFor(prodToOrder.Value, prodToOrder.Key);
		}
	}
	public void PlaceOrderFor(Product product, EndpointUIValues endpoint)
	{
		//find a supplier with enough of the product
		Supplier? supplier =
			(Supplier?)(SupplierList.FirstOrDefault(s => s.Supplier.ProductInventory
				.FirstOrDefault(p =>
					p.ProductName == product.ProductName
					&& p.Quantity >= product.Quantity)
			!= null)?.Supplier);

		if (supplier != null)
		{
			//create shippingline that has all of the product 
			Shipment shipment = new Shipment()
			{
				Sender = supplier,
				Receiver = endpoint.Supplier,
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
				Product? productWithSmallQuantity = sup.Supplier.ProductInventory.FirstOrDefault(p => p.ProductName == product.ProductName && p.Quantity > 0);
				if (productWithSmallQuantity != null)
				{
					Shipment shipment = new Shipment()
					{
						Sender = sup.Supplier,
						Receiver = endpoint.Supplier,
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
			return (double)x.Endpoints.Sum(item => ((EndpointNode)item.Supplier).Balance) / 1000;
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
			var item = Snapshots?.Where(x => x.Targets.Exists(x => x.ProductTarget?.ProductName == product_name));

            var item2 = item.LastOrDefault()?.Targets.FirstOrDefault()?.DayCompleted;

			return (double)item2!;
		}
		catch (Exception ex)
		{
			{
				Console.WriteLine("Finding day completed ran aground on a null reference.\n", ex.Message);
			}
			return -1;
		}
	}

	public void CheckProductLinesMissingSuppliers()
	{
		foreach(var product in ProductionTargets)
		{
			if (!product.IsTargetEnabled)
				continue;

			//filter for productionlists that make this product.
			var productionListComponents = new List<Product>();
			foreach(var endpoint in _endpointList)
			{
				productionListComponents.AddRange(((EndpointNode)endpoint.Supplier).ProductionList.Where(x =>
                x.ResultingProduct.ProductName == product.ProductTarget?.ProductName).SelectMany(x => x.Components).ToList());
			}

			//filter for all products covered
			var availableproducts = new List<Product>();
			availableproducts = SupplierList.SelectMany(x => x.Supplier.ProductInventory).ToList();
			availableproducts = availableproducts.DistinctBy(x => x.ProductName).ToList();

			var Missing = productionListComponents.Where(c => !availableproducts.Exists(x => c.ProductName == x.ProductName)).ToList();

            // if any component in the product lines does not appear in the whole list of supplier products
            if (Missing.Count() > 0)
			{
				product.CannotBeFulfilled = true;
				product.ProductsNeeded = product.ProductsNeeded + ReduceProductsNeededToString(Missing);
			}
			else
			{
				product.CannotBeFulfilled = false;
			}
		}
	}
		
	public string ReduceProductsNeededToString(List<Product> list)
	{
		if(list.Count() == 0)
		{
			return "";
		}
		if(list.Count() == 1)
		{
			return list.First().ProductName;
		}

		return list.First().ProductName + ", " + ReduceProductsNeededToString(list.Slice(1, list.Count() - 1));
	}
}
