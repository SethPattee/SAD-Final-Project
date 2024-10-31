using WPFTesting.Models;

namespace SupplyChainTesting;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_Endpoint_ProduceProduct_Increments_by_1()
    {
        var cut = new EndpointNode();

        Product p = new Product();
        p.ProductName = "Test prod prod";
        p.Quantity = 1;
        Product c = new Product();
        c.ProductName = "Test component prod";
        c.Quantity = 1;
        cut.ComponentInventory = new List<Product> { c };
        cut.ProductInventory = new List<Product> { p };

        cut.ProduceProduct();

        Assert.AreEqual(0, cut.ComponentInventory[0].Quantity);
        Assert.AreEqual(2, cut.ProductInventory[0].Quantity);

    }
}