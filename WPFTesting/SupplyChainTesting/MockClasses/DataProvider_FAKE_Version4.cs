﻿using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChainTesting.MockClasses;

public class DataProvider_FAKE_Version4 : IInitializedDataProvider
{
	public DataProvider_FAKE_Version4()
	{
		
	}
	public IEnumerable<SupplierUIValues> GetBoxValues()
	{

		IEnumerable<SupplierUIValues> suppliers = new List<SupplierUIValues>
		{
			new SupplierUIValues {
				Supplier = new Supplier()
				{ ProductInventory =
					{
						new Product()
						{
							Quantity=20,
							ProductName="Drill Bit"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Wood"
						}
					},

					Name="Wood Vendor",
					Id = Guid.NewGuid()
				},

				Position = new System.Drawing.Point(140,140)

			},
			new SupplierUIValues
			{
				Supplier = new Supplier()
				{ ProductInventory =
					{
						new Product()
						{
							Quantity=20,
							ProductName="Drill Bit"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Screws"
						}
					},

					Name="Screws Vendor",
					Id = Guid.NewGuid()
				},
				Position= new System.Drawing.Point(280,280)
			},
			new SupplierUIValues
			{
				Supplier = new Supplier()
				{
					ProductInventory =
					{
						new Product()
						{
							Quantity = 1,
							ProductName = "Hammer"
						},
						new Product()
						{
							Quantity=1,
							ProductName="10mm socket"
						},
						new Product()
						{
							Quantity=20,
							ProductName="Screws"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Glue"
						}
					},
					Name="Glue Vendor",
					Id=Guid.NewGuid()
				},
				Position= new System.Drawing.Point(50,320)
			},
			new EndpointUIValues
			{
				Supplier = new EndpointNode()
				{
					ProductInventory =
					{
						new Product()
						{
							Quantity = 0,
							ProductName = "Box"
						}
					},
					Name = "Enpoint Name",
					Id=Guid.NewGuid(),
					ComponentInventory =
					{
						new Product()
						{
							Quantity=200,
							ProductName="Screws"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Sawdust"
						},
						new Product()
						{
							Quantity=10,
							ProductName= "Wood"
						}
					},
					ProductionList =
					{
						   new ProductLine()
							{
								ResultingProduct = new Product()
								{
									Quantity = 2,
									ProductName = "Box"
								},
								Components = new ObservableCollection<Product>()
								{
									new Product()
									{
										Quantity=12,
										ProductName="Screws"
									},
									new Product()
									{
										Quantity=10,
										ProductName= "Wood"
									}
								},
								IsEnabled = true,
							},
						   new ProductLine()
							{
								ResultingProduct = new Product()
								{
									Quantity = 1,
									ProductName = "Door"
								},
								Components = new ObservableCollection<Product>()
								{
									new Product()
									{
										Quantity=10,
										ProductName="Glue"
									},
									new Product()
									{
										Quantity=10,
										ProductName= "Wood"
									}
								},
								IsEnabled = true,
							}
					}
				}
			}
		};

		return suppliers;
	}

	public IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{
		var toreturn = new List<Shipment>() {
			new Shipment()
			{
				Products = new ObservableCollection<Product>()
				{
					new Product()
									{
										Quantity=10,
										ProductName= "Glue"
									},
					new Product()
									{
										Quantity=2,
										ProductName= "Screws"
									}

				},
				Sender = new Supplier()
				{
					ProductInventory =
					{
					},
					Name="Glue Vendor",
					Id=Guid.NewGuid()
				},
				Receiver =
					new Supplier()
				{
					ProductInventory =
					{
					},
					Name="Enpoint Name",
					Id=Guid.NewGuid()

				},
				Id=Guid.NewGuid(),
				FromJoiningBoxCorner = "",
				ToJoiningBoxCorner = "",


			}
	   };

		return toreturn;
	}

	public void SaveShipmentInfo(IEnumerable<Shipment> shipments)
	{
		throw new NotImplementedException();
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath)
    {
        throw new NotImplementedException();
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues)
	{
		throw new NotImplementedException();
	}

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath)
    {
        throw new NotImplementedException();
    }
}


public class DataProvider_FAKE_Version5 : IInitializedDataProvider
{
	public DataProvider_FAKE_Version5()
	{

	}
	public IEnumerable<SupplierUIValues> GetBoxValues()
	{

		IEnumerable<SupplierUIValues> suppliers = new List<SupplierUIValues>
		{
			new SupplierUIValues {
				Supplier = new Supplier()
				{ ProductInventory =
					{
						new Product()
						{
							Quantity=20,
							ProductName="Drill Bit"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Wood"
						}
					},

					Name="Wood Vendor",
					Id = Guid.NewGuid()
				},

				Position = new System.Drawing.Point(140,140)

			},
			new SupplierUIValues
			{
				Supplier = new Supplier()
				{ ProductInventory =
					{
						new Product()
						{
							Quantity=20,
							ProductName="Drill Bit"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Screws"
						}
					},

					Name="Screws Vendor",
					Id = Guid.NewGuid()
				},
				Position= new System.Drawing.Point(280,280)
			},
			new SupplierUIValues
			{
				Supplier = new Supplier()
				{
					ProductInventory =
					{
						new Product()
						{
							Quantity = 1,
							ProductName = "Hammer"
						},
						new Product()
						{
							Quantity=1,
							ProductName="10mm socket"
						},
						new Product()
						{
							Quantity=20,
							ProductName="Screws"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Glue"
						}
					},
					Name="Glue Vendor",
					Id=Guid.NewGuid()
				},
				Position= new System.Drawing.Point(50,320)
			},
			new EndpointUIValues
			{
				Supplier = new EndpointNode()
				{
					ProductInventory =
					{
						new Product()
						{
							Quantity = 0,
							ProductName = "Box"
						}
					},
					Name = "Enpoint Name",
					Id=Guid.NewGuid(),
					ComponentInventory =
					{
						new Product()
						{
							Quantity=1000,
							ProductName="Screws"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="Sawdust"
						},
						new Product()
						{
							Quantity=1000,
							ProductName= "Wood"
						},
						new Product()
						{
							Quantity=1000,
							ProductName = "Glue"
						}
					},
					ProductionList =
					{
						   new ProductLine()
							{
								ResultingProduct = new Product()
								{
									Quantity = 2,
									ProductName = "Box"
								},
								Components = new ObservableCollection<Product>()
								{
									new Product()
									{
										Quantity=12,
										ProductName="Screws"
									},
									new Product()
									{
										Quantity=10,
										ProductName= "Wood"
									}
								},
								IsEnabled = true,
							},
						   new ProductLine()
							{
								ResultingProduct = new Product()
								{
									Quantity = 1,
									ProductName = "Door"
								},
								Components = new ObservableCollection<Product>()
								{
									new Product()
									{
										Quantity=10,
										ProductName="Glue"
									},
									new Product()
									{
										Quantity=10,
										ProductName= "Wood"
									}
								},
								IsEnabled = true,
							}
					}
				}
			}
		};

		return suppliers;
	}

	public IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{
		var toreturn = new List<Shipment>() {
			
	   };

		return toreturn;
	}

	public void SaveShipmentInfo(IEnumerable<Shipment> shipments)
	{
		throw new NotImplementedException();
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath)
    {
        throw new NotImplementedException();
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues)
	{
		throw new NotImplementedException();
	}

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath)
    {
        throw new NotImplementedException();
    }
}