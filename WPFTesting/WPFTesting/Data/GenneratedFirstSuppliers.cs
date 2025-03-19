using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorySADEfficiencyOptimizer.Data;

internal static class GenneratedFirstSuppliers
{
    public static IEnumerable<SupplierUIValues> InitialSuppliers()
    {
        return new List<SupplierUIValues>
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
                            //Units="kg"
                        }
                    },

                    Name="Supplier Bob",
                    Id = Guid.NewGuid()
                },
                Position= new System.Drawing.Point(140,140),
            },
            new SupplierUIValues
            {
                supplier = new Supplier()
                { ProductInventory =
                    {
                        new Product()
                        {
                            Quantity = 1,
                            ProductName = "computer"
                        },
                        new Product()
                        {
                            Quantity=1,
                            ProductName="AV Cable"
                        },
                    },

                    Name="my company",
                    Id = Guid.NewGuid()
                },
                Position= new System.Drawing.Point(280,280),
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
                            //Units="kg"
                        }
                    },
                    Name="Lost Hardware",
                    Id=Guid.NewGuid()
                },
                Position= new System.Drawing.Point(50,320)
            }
        };
    }
}
