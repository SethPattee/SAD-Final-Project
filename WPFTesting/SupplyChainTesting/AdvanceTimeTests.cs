﻿using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Data;
using WPFTesting.Models;
using WPFTesting.ViewModel;

namespace SupplyChainTesting;

internal class AdvanceTimeTests
{
    SupplyChainViewModel setupTest()
    {
        IInitializedDataProvider data = new DataProvider_FAKE();
        SupplyChainViewModel model = new SupplyChainViewModel(data);
        model.Load();
        Shipment s = new Shipment();
        s.Sender = model.SupplierList[0].supplier;
        s.Receiver = model.SupplierList[1].supplier;
        s.Products = new List<Product>() {
            new Product()
                        {
                            Quantity=10,
                            ProductName="Drill Bit"
                        },
                        new Product()
                        {
                            Quantity=5,
                            ProductName="Saw Blade 2-pack",
                            Units="kg"
                        }
        };
        model.ShipmentList.Add(s);
        return model;
    }
    [Test]
    public void Test_Setup_Creates_valid_data()
    {
        SupplyChainViewModel model = setupTest();
        //Model has data properly
        Assert.AreEqual(3, model.SupplierList.Count);
        Assert.AreEqual(20, model.SupplierList[0].supplier.ProductInventory[0].Quantity);
        Assert.AreEqual(20, model.SupplierList[1].supplier.ProductInventory[0].Quantity);
        Assert.AreEqual(10, model.SupplierList[0].supplier.ProductInventory[1].Quantity);
        Assert.AreEqual(10, model.SupplierList[1].supplier.ProductInventory[1].Quantity);
    }
    [Test]
    public void AdvanceTime_with_one_shippment()
    {
        SupplyChainViewModel model = setupTest();

        model.AdvanceTime();
        var giver_first_product = model.SupplierList[0].supplier.ProductInventory[0];
        var reciver_first_product = model.SupplierList[1].supplier.ProductInventory[0];
        var giver_second_product = model.SupplierList[0].supplier.ProductInventory[1];
        var reciver_second_product = model.SupplierList[1].supplier.ProductInventory[1];
        // supplier loses 10 
        Assert.AreEqual(10, giver_first_product.Quantity);
        Assert.AreEqual(5, giver_second_product.Quantity);
        // recever gains 10
        Assert.AreEqual(30, reciver_first_product.Quantity);
        Assert.AreEqual(15, reciver_second_product.Quantity);
    }

    [Test]
    public void AdvanceTime_Ships_even_when_reciver_does_not_have_product()
    {
        SupplyChainViewModel model = setupTest();
        //swap reciver to supplier with no drill bits (sender is sending drill bits)
        model.ShipmentList.First().Receiver = model.SupplierList[2].supplier;
        Assert.AreEqual(4, model.ShipmentList[0].Receiver.ProductInventory.Count);

        model.AdvanceTime();
        var first_product = model.ShipmentList[0].Products[0];
        var giver_first_product = model.SupplierList[0].supplier.ProductInventory
                .FirstOrDefault(p => p.ProductName == first_product.ProductName);
        var reciver_first_product = model.SupplierList[2].supplier.ProductInventory
                .FirstOrDefault(p => p.ProductName == first_product.ProductName);
        var second_product = model.ShipmentList[0].Products[1];
        var giver_second_product = model.SupplierList[0].supplier.ProductInventory
                .FirstOrDefault(p => p.ProductName == second_product.ProductName);
        var reciver_second_product = model.SupplierList[2].supplier.ProductInventory
                .FirstOrDefault(p => p.ProductName == second_product.ProductName);
        // supplier loses 10 
        Assert.AreEqual(10, giver_first_product.Quantity);
        Assert.AreEqual(5, giver_second_product.Quantity);
        // recever gains 10
        Assert.AreEqual(10, reciver_first_product.Quantity);
        Assert.AreEqual(5, reciver_second_product.Quantity);
        // recever has more products than before
        Assert.AreEqual(6, model.ShipmentList[0].Receiver.ProductInventory.Count);
    }

    [Test]
    public void Advancetime_increments_by_constant_amount_products_in_Shipment()
    {
        // there has been a bug where calling advance time will increase the quantity of the products in the shipment. 
        // we want the quantity to remain the same for repeated shipments
        SupplyChainViewModel model = setupTest();
        var first_product = model.ShipmentList.First().Products.First();
        var reciver_first_product = model.ShipmentList.First().Receiver.ProductInventory
                .FirstOrDefault(p => p.ProductName == first_product.ProductName);
        model.AdvanceTime();
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(30, reciver_first_product.Quantity);
        model.AdvanceTime();
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(40, reciver_first_product.Quantity);
        model.AdvanceTime();
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(50, reciver_first_product.Quantity);
        for (int i = 0; i < 10; i++) {
            model.AdvanceTime();
        }
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(150, reciver_first_product.Quantity);
        // keep the test, it showed the bug was somwhere in the display, because the viewmodel didn't have the changing value issue.
    }
    [Test]
    public void Advancetime_increments_by_constant_amount_products_in_Shipment_when_frist_added_product()
    {
        // there has been a bug where calling advance time will increase the quantity of the products in the shipment. 
        // we want the quantity to remain the same for repeated shipments
        SupplyChainViewModel model = setupTest();
        model.ShipmentList.First().Receiver = model.SupplierList[2].supplier;
        var first_product = model.ShipmentList.First().Products.First();
        model.AdvanceTime();
        var reciver_first_product = model.SupplierList[2].supplier.ProductInventory
                .FirstOrDefault(p => p.ProductName == first_product.ProductName);
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(10, reciver_first_product.Quantity);
        model.AdvanceTime();
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(20, reciver_first_product.Quantity);
        model.AdvanceTime();
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(30, reciver_first_product.Quantity);
        for (int i = 0; i < 10; i++)
        {
            model.AdvanceTime();
        }
        Assert.AreEqual(10, first_product.Quantity);
        Assert.AreEqual(130, reciver_first_product.Quantity);
        // keep the test, it showed the bug was somwhere in the display, because the viewmodel didn't have the changing value issue.
    }

    [Test]
    public void EndpointNodeProcessDoesConsumeProperlyAndProduce()
    {
        // Beeg arrange
        EndpointNode endpointTest = new EndpointNode()
        {
            Id = Guid.NewGuid(),
            Name = "Factory",
            ComponentInventory = new List<Product>()
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
                    Units = "ml"
                },
                new Product()
                {
                    Price = 10,
                    ProductName = "nails",
                    Quantity = 400
                }
            },
            ProductInventory = new List<Product>()
            {
                new Product()
                {
                    Price = 100,
                    ProductName = "box",
                    Quantity = 0
                }
            },
            ProductionList = new List<ComponentToProductTransformer>()
            {
                new ComponentToProductTransformer()
                {
                    Components = new List<Product>()
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
                                       }
                }
            }

        };

        endpointTest.Process();
        Assert.That(endpointTest.ProductInventory.First(x => x.ProductName == "box").Quantity, Is.EqualTo(1));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "glue").Quantity, Is.EqualTo(8));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "wood").Quantity, Is.EqualTo(24));
    }

    [Test]
    public void EndpointNodeDoesReceiveProperly()
    {
        EndpointNode endpointTest = new EndpointNode()
        {
            Name = "Factory",
            Profit = (decimal)10000
        };
        Shipment testShipment = new Shipment()
        {
            Products = new List<Product>()
            {
                new Product()
                {
                    Price = (decimal)5.99,
                    ProductName = "Swedish fish",
                    Quantity = 100
                },
                new Product()
                {
                    Price = (decimal)100,
                    ProductName = "wood",
                    Quantity = 100
                }
            },
            Receiver = endpointTest
        };

        endpointTest.Receive(testShipment.Products);

        Assert.That(endpointTest.ComponentInventory.Count, Is.GreaterThan(0));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "Swedish fish").Quantity, Is.EqualTo(100));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "wood").Quantity, Is.EqualTo(100));
        Assert.That(endpointTest.Profit, Is.EqualTo(9894.01));
    }

    [Test]
    public void EndpointNodeShipOrderSendsProductsProperly()
    {
        EndpointNode endpointTest = new EndpointNode()
        {
            Name = "Swedish fish factory",
            ProductInventory = new List<Product>()
            {
                new Product()
                {
                    ProductName = "Swedish fish",
                    Price = (decimal)0.0599,
                    Quantity = 10000
                }
            },
            Profit = 10000
        };
        List<Product> testOrder = new List<Product>()
        {
            new Product() {
                ProductName = "Swedish fish",
                Price = (decimal)5.99,
                Quantity = 100
            }
        };

        endpointTest.ShipOrder(testOrder);

        Assert.That(endpointTest.Profit, Is.EqualTo(10005.99));
        Assert.That(endpointTest.ProductInventory.Find(x => x.ProductName == "Swedish fish").Quantity, Is.EqualTo(9900));
        }
}
