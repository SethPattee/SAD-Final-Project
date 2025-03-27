using FactorySADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChainTesting
{
    public class ModelTests
    {
        [Test]
        public void DeliveryLineAccumulatesTotalCostCorrectly_Test()
        {
            DeliveryLine testClass = new()
            {
                DeliveryItem =
                new Product()
                {
                    ProductName = "Door",
                    Quantity = 6,
                    Price = 149.70M
                }
                
            };
        }
    }
}
