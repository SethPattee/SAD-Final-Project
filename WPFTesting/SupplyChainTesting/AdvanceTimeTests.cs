using SupplyChainTesting.MockClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Data;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using FactorySADEfficiencyOptimizer.ViewModel;

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
        s.Products = new ObservableCollection<Product>() {
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
    public void AdvanceTime_Moves_only_products_in_sender()
    {
        SupplyChainViewModel model = setupTest();
        model.ShipmentList.First().Sender = model.SupplierList[2].supplier;
        //Sender doesn't have Drill Bit or Saw Blade 2-pack
        model.AdvanceTime();
        var reciverInventory = model.SupplierList[1].supplier.ProductInventory;
        var reciver_first_product = reciverInventory[0];
        var reciver_second_product = reciverInventory[1];

        // recever gains nothing
        Assert.AreEqual(20, reciver_first_product.Quantity);
        Assert.AreEqual(10, reciver_second_product.Quantity);
        Assert.AreEqual(2, reciverInventory.Count);

        model.ShipmentList.First().Products.Add(new Product() {
            Quantity = 5,
            ProductName = "screws"
        });

        model.AdvanceTime();
        reciverInventory = model.SupplierList[1].supplier.ProductInventory;
        reciver_first_product = reciverInventory[0];
        reciver_second_product = reciverInventory[1];
        var reciver_third_product = reciverInventory[2];

        // recever gains just the screws
        Assert.AreEqual(20, reciver_first_product.Quantity);
        Assert.AreEqual(10, reciver_second_product.Quantity);
        Assert.AreEqual(5, reciver_third_product.Quantity);
        Assert.AreEqual(3, reciverInventory.Count);
    }
    [Test]
    public void AdvanceTime_Moves_up_to_number_products_in_sender()
    {
        SupplyChainViewModel model = setupTest();
        //Sender only has 20 Drill Bit or 10 Saw Blade 2-pack
        model.ShipmentList.First().Products.First().Quantity = 200;
        var reciverInventory = model.SupplierList[1].supplier.ProductInventory ?? new();
        var suplierInventory = model.SupplierList[0].supplier.ProductInventory;
        var reciver_first_product = reciverInventory[0];
        var suplier_first_product = suplierInventory[0];
        var suplier_second_product = suplierInventory[1];
        var reciver_second_product = reciverInventory[1];

        Assert.AreEqual(20, suplier_first_product.Quantity);
        Assert.AreEqual(10, suplier_second_product.Quantity);
        Assert.AreEqual(2, suplierInventory.Count);
        Assert.AreEqual(20, reciver_first_product.Quantity);
        Assert.AreEqual(10, reciver_second_product.Quantity);
        Assert.AreEqual(2, reciverInventory.Count);

        model.AdvanceTime();
        reciverInventory = model.SupplierList[1].supplier.ProductInventory ?? new();
        suplierInventory = model.SupplierList[0].supplier.ProductInventory;
        reciver_first_product = reciverInventory[0];
        suplier_first_product = suplierInventory[0];
        reciver_second_product = reciverInventory[1];
        suplier_second_product = suplierInventory[1];

        Assert.AreEqual(0, suplier_first_product.Quantity);
        Assert.AreEqual(5, suplier_second_product.Quantity);
        Assert.AreEqual(2, suplierInventory.Count);
        Assert.AreEqual(40, reciver_first_product.Quantity);
        Assert.AreEqual(15, reciver_second_product.Quantity);
        Assert.AreEqual(2, reciverInventory.Count);
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
        model.ShipmentList.First().Sender.ProductInventory.First(p => p.ProductName == first_product.ProductName).Quantity = 1000;
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
    }
    [Test]
    public void Advancetime_increments_by_constant_amount_products_in_Shipment_when_frist_added_product()
    {
        // there has been a bug where calling advance time will increase the quantity of the products in the shipment. 
        // we want the quantity to remain the same for repeated shipments
        SupplyChainViewModel model = setupTest();
        model.ShipmentList.First().Receiver = model.SupplierList[2].supplier;
        var first_product = model.ShipmentList.First().Products.First();
        model.ShipmentList.First().Sender.ProductInventory.First(p => p.ProductName == first_product.ProductName).Quantity = 1000;
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
    }

    [Test]
    public void EndpointNodeProcessDoesConsumeProperlyAndProduce()
    {
        // Beeg arrange
        EndpointNode endpointTest = RealEnpointForTest.makeAnEnpointForTest();
        endpointTest.Process();
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "glue").Quantity, Is.EqualTo(8));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "wood").Quantity, Is.EqualTo(24));
        Assert.That(endpointTest.ProductInventory.First(x => x.ProductName == "box").Quantity, Is.EqualTo(1));
    }

    

    [Test]
    public void EndpointNodeDoesReceiveProperly()
    {
        EndpointNode endpointTest = new EndpointNode()
        {
            Name = "Factory",
            Balance = (decimal)10000
        };
        Shipment testShipment = new Shipment()
        {
            Products = new ObservableCollection<Product>()
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

        endpointTest.Receive(testShipment.Products.ToList<Product>());

        Assert.That(endpointTest.ComponentInventory.Count, Is.GreaterThan(0));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "Swedish fish").Quantity, Is.EqualTo(100));
        Assert.That(endpointTest.ComponentInventory.First(x => x.ProductName == "wood").Quantity, Is.EqualTo(100));
        Assert.That(endpointTest.Balance, Is.EqualTo(9894.01));
    }

    [Test]
    public void EndpointNodeShipOrderSendsProductsProperly()
    {
        EndpointNode endpointTest = new EndpointNode()
        {
            Name = "Swedish fish factory",
            ProductInventory = new ObservableCollection<Product>()
            {
                new Product()
                {
                    ProductName = "Swedish fish",
                    Price = (decimal)0.0599,
                    Quantity = 10000
                }
            },
            Balance = 10000
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

        Assert.That(endpointTest.Balance, Is.EqualTo(10005.99));
        Assert.That(endpointTest.ProductInventory.FirstOrDefault(x => x.ProductName == "Swedish fish").Quantity, Is.EqualTo(9900));
        }
    [Test]
    public void EndpointWillProccessProductionLineWhenViewModelCallAddvanceTime()
    {
        SupplyChainViewModel model = setupTest();
        EndpointNode endpointTest = RealEnpointForTest.makeAnEnpointForTest();
        EndpointUIValues endpointUIValues = new EndpointUIValues();
        endpointUIValues.supplier = endpointTest;
        model.EndpointList.Clear();
        model.EndpointList.Add(endpointUIValues);
        model.AdvanceTime();
        Assert.That(endpointTest.ProductInventory.First(x => x.ProductName == "box").Quantity, Is.EqualTo(1));
    }

    [Test]
    public void ShipmentProcessTime_DecrementsCorrectly_NumberGoDown()
    {
        Shipment shipment = new();
        shipment.TimeUntilNextDelivery = 3;
        shipment.ProcessTime();
        Assert.That(shipment.TimeUntilNextDelivery == 2);
    }

    [Test]
    public void ShipmentProcessTime_DecrementsCorrectly_NumberResets()
    {
        Shipment shipment = new();
        shipment.TimeUntilNextDelivery = 0;
        shipment.TimeToDeliver = 5;
        shipment.ProcessTime();
        Assert.That(shipment.TimeUntilNextDelivery, Is.EqualTo(4));
    }
}
