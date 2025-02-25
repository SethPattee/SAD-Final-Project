using System;
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

namespace FactorSADEfficiencyOptimizer.ViewModel;
public class AnalizorModel : CopyMaker, INotifyPropertyChanged
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
		set { _snapshot = value; OnPropertyChanged(nameof(Snapshot));}
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
			SupplierUIValues sup = makeShallowCopySupplier(supplier);
			AddSupplier(sup);
		}
		foreach (EndpointUIValues endpoint in model.EndpointList)
		{
			EndpointUIValues end = makeShallowCopyEndpoint(endpoint);
			foreach (ProductLine pl in ((EndpointNode)end.supplier).ProductionList)
			{
				if (pl.IsEnabled) { 
					ProductionTargets.Add(
						new ProductionTarget()
						{
							DueDate = 1,
							IsTargetEnabled = true,
							TargetQuantity = pl.ResultingProduct.Quantity,
							ProductTarget = makeShallowCopyProduct(pl.ResultingProduct),
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
        ship.Products = makeShallowCopyOfProductColection(shipment.Products);
        ship.ToJoiningBoxCorner = "";
        ship.FromJoiningBoxCorner = "";
        // TODO: Add Dallins changes to Shipments HERE!!! It is missing some stuff
        return ship;
    }
    public Snapshot MakeCurrentSnapShot()
	{
		Snapshot snapshot = new Snapshot();
		foreach (var supplier in SupplierList)
		{
			SupplierUIValues sup = makeShallowCopySupplier(supplier);
			snapshot.Suppliers.Add(supplier);
		}
		foreach (EndpointUIValues endpoint in EndpointList)
		{
			EndpointUIValues end = makeShallowCopyEndpoint(endpoint);
			snapshot.Endpoints.Add(end);
		}
		foreach (Shipment shipment in ShipmentList)
		{
			Shipment ship = GetShipmentWithSenderReciver(shipment);
			ship.Products = makeShallowCopyOfProductColection(shipment.Products);
			ship.ToJoiningBoxCorner = "";
			ship.FromJoiningBoxCorner = "";
			// TODO: Add Dallins changes to Shipments HERE!!! It is missing some stuff
			snapshot.Shipments.Add(ship);
		}
		snapshot.ComponentsUsed = makeShallowCopyOfProductColection(_daysUsedComponents).ToList();
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
            newtarg.ProductTarget = makeShallowCopyProduct(target.ProductTarget ?? new Product());
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
            EndpointList.Add(makeShallowCopyEndpoint(end));
        }
        SupplierList.Clear();
        foreach (var sup in initValues.Suppliers)
        {
            SupplierList.Add(makeShallowCopySupplier(sup));
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
		List<Product> ToAddComponents = new List<Product>();
		//get old products
		List<Product> startProducts = new List<Product>();
        foreach (EndpointUIValues end in Snapshots.Last().Endpoints)
		{
			startProducts.AddRange(end.supplier.ProductInventory);
		}
		foreach (EndpointUIValues end in EndpointList)
		{
			foreach ( Product prod in end.supplier.ProductInventory )
			{
				var BiginDayProd = startProducts.FirstOrDefault(p => p.ProductName == prod.ProductName);
				var productionrequirments = ((EndpointNode)end.supplier).ProductionList.FirstOrDefault(p => p.ResultingProduct.ProductName == prod.ProductName) ?? new ProductLine();
				foreach (Product component in productionrequirments.Components)
				{
					var addForQntProd = makeShallowCopyProduct(component);
					if (BiginDayProd == null)
					{
						addForQntProd.Quantity = component.Quantity;
					}
					else
					{

						addForQntProd.Quantity = component.Quantity * ((prod.Quantity - BiginDayProd.Quantity) / productionrequirments.ResultingProduct.Quantity);
					}
                    ToAddComponents.Add(addForQntProd);
				}
			}
		}
		foreach (Product prod in ToAddComponents)
		{
			var existingProd = _daysUsedComponents.FirstOrDefault(p => p.ProductName == prod.ProductName);

            if (existingProd != null)
			{
				existingProd.Quantity += prod.Quantity;
			}
			else
			{
				_daysUsedComponents.Add(prod);
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
				if (target.CurrentAmount == target.TargetQuantity)
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

		if (supplier != null) {
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
		return Snapshots.Select<Snapshot,double>(x =>
		{
			return (double)x.Endpoints.Sum(item => ((EndpointNode)item.supplier).Balance) / 1000;
		}).ToArray();
	}

	public List<ProductionTarget?> ExtractProductionTargetChanges(string name)
	{
		return Snapshots.Select(x => x.Targets.Find(y => (y.ProductTarget ?? throw new ArgumentNullException("Product target list is null!")).ProductName == name)).ToList();
	}
}
