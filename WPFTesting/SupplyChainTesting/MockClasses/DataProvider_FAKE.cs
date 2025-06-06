﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace SupplyChainTesting.MockClasses;

internal class DataProvider_FAKE : IInitializedDataProvider
{
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 1",
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 2",
                    Id = Guid.NewGuid()
                },
                Position= new Point(280,280)
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
                            ProductName="screws"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName="sawdust"
                        }
                    },
                    Name="Vendor 3",
                    Id=Guid.NewGuid()
                },
                Position= new Point(50,320)
            }
        };

        return suppliers;
    }

	public IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{
		return new List<Shipment>();
	}

	public void SaveShipmentInfo(IEnumerable<Shipment> shipments)
	{
        return;
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath)
    {
        throw new NotImplementedException();
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        return;
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath)
    {
        throw new NotImplementedException();
    }
}

internal class DataProvider_FAKE_Version2 : IInitializedDataProvider
{
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 1",
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 2",
                    Id = Guid.NewGuid()
                },
                Position= new Point(280,280)
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
                            ProductName="screws"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName="sawdust"
                        }
                    },
                    Name="Vendor 3",
                    Id=Guid.NewGuid()
                },
                Position= new Point(50,320)
            }
        };

        return suppliers;
    }

	public IEnumerable<Shipment> GetShipments(IEnumerable<EndpointUIValues> endpoints, IEnumerable<SupplierUIValues> suppliers)
	{
		return new List<Shipment>();
	}

	public void SaveShipmentInfo(IEnumerable<Shipment> shipments)
	{
        return;
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath)
    {
        throw new NotImplementedException();
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        return;
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath)
    {
        throw new NotImplementedException();
    }
}

internal class DataProvider_FAKE_Version3 : IInitializedDataProvider
{ // This version is same as version1, but has a EndpointNode
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 1",
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
                            Quantity=10,
                            ProductName="Saw Blade 2-pack"
                        }
                    },

                    Name="Vendor 2",
                    Id = Guid.NewGuid()
                },
                Position= new Point(280,280)
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
                            ProductName="screws"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName="sawdust"
                        }
                    },
                    Name="Vendor 3",
                    Id=Guid.NewGuid()
                },
                Position= new Point(50,320)
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
                            ProductName = "box"
                        }
                    },
                    Name = "Enpoint Name",
                    Id=Guid.NewGuid(),
                    ComponentInventory =
                    {
                        new Product()
                        {
                            Quantity=20,
                            ProductName="screws"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName="sawdust"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName= "wood"
                        }
                    },
                    ProductionList =
                    {
                           new ProductLine()
                            {
                                ResultingProduct = new Product()
                                {
                                    Quantity = 1,
                                    ProductName = "box"
                                },
                                Components = new ObservableCollection<Product>()
                                {
                                    new Product()
                                    {
                                        Quantity=12,
                                        ProductName="screws"
                                    },
                                    new Product()
                                    {
                                        Quantity=10,
                                        ProductName= "wood"
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
       var toreturn =  new List<Shipment>() { 
            new Shipment()
            {
                Products = new ObservableCollection<Product>()
                {
					new Product()
									{
										Quantity=10,
										ProductName= "wood"
									},
                    new Product()
                                    {
                                        Quantity=10,
                                        ProductName= "nails"
                                    }

                },
                Sender = new Supplier()
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
							ProductName="screws"
						},
						new Product()
						{
							Quantity=1000,
							ProductName="sawdust"
						}
					},
					Name="Vendor 3",
					Id=Guid.NewGuid()
				},
                Receiver =
					new Supplier()
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
					},
					Name="Vendor 2",
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
        return;
	}

    public void SaveShipmentInfo(IEnumerable<Shipment> shipments, string? filepath)
    {
        throw new NotImplementedException();
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        return;
    }

    public void SaveSupplierInfo(IEnumerable<SupplierUIValues> supplierUIValues, string? filepath)
    {
        throw new NotImplementedException();
    }
}