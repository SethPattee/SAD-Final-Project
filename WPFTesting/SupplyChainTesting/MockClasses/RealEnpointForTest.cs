using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;

namespace SupplyChainTesting.MockClasses;

public static class RealEnpointForTest
{
public static EndpointNode makeAnEnpointForTest()
    {
        return new EndpointNode()
        {
            Id = Guid.NewGuid(),
            Name = "Factory",
            ComponentInventory = new ObservableCollection<Product>()
            {
                new Product()
                {
                    Price = 12,
                    ProductName = "wood",
                    Quantity = 30
                },
                new Product()
                {
                    Price = (decimal)5.99,
                    ProductName = "glue",
                    Quantity = 10,
                    //Units = "ml"
                },
                new Product()
                {
                    Price = 10,
                    ProductName = "nails",
                    Quantity = 400
                }
            },
            ProductInventory = new ObservableCollection<Product>()
            {
                new Product()
                {
                    Price = 100,
                    ProductName = "box",
                    Quantity = 0
                }
            },
            ProductionList = new ObservableCollection<ProductLine>()
            {
                new ProductLine()
                {
                    Components = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "wood",
                            Quantity = 6
                        },
                        new Product()
                        {
                            ProductName = "glue",
                            Quantity = 2
                        }
                    },
                    ResultingProduct = new Product()
                                       {
                                           ProductName = "box",
                                           Quantity = 1
                                       },
                    IsEnabled = true,
                }
            }

        };
    }

}