using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace SupplyChainTesting.MockClasses;

internal class DataProvider_FAKE : IInitializedDataProvider
{
    public IEnumerable<SupplierUIValues> GetBoxValuesAsync()
    { 
        
        IEnumerable<SupplierUIValues> suppliers = new List<SupplierUIValues>
        {
            new SupplierUIValues {
                supplier = new Supplier()
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
                            ProductName="Saw Blade 2-pack",
                            Units="kg"
                        }
                    },

                    Name="Vendor 1",
                    Id = Guid.NewGuid()
                },
                Position = new System.Drawing.Point(140,140)

            },
            new SupplierUIValues
            {
                supplier = new Supplier()
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
                            ProductName="Saw Blade 2-pack",
                            Units="kg"
                        }
                    },

                    Name="Vendor 2",
                    Id = Guid.NewGuid()
                },
            },
            new SupplierUIValues
            {
                supplier = new Supplier()
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
                            ProductName="sawdust",
                            Units="kg"
                        }
                    },
                    Name="Vendor 3",
                    Id=Guid.NewGuid()
                },
            }
        };

        return suppliers;
    }

    public void SaveSupplierInfoAsync(IEnumerable<SupplierUIValues> supplierUIValues)
    {
        throw new NotImplementedException();
    }
}
