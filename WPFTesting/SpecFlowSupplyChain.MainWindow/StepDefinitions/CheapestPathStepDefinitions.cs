//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WPFTesting.Methods;
//using WPFTesting.Models;

//namespace SpecFlowSupplyChain.CheapestPath.StepDefinitions
//{
//    [Binding]
//    public class CheapestPathStepDefinitions
//    {
//        private readonly ScenarioContext _scenariocontext;
//        private static string SupplierList = "CompanyList";
//        private static string ShipmentList = "ShipmentList";
//        public CheapestPathStepDefinitions(ScenarioContext sc)
//        {
//            _scenariocontext = sc;
//            _scenariocontext.Add("pathfinderobject", new SupplyChainCheapestPath());
//        }

//        [Given("A (.*) company")]
//        public void GivenACompany(string companyname)
//        {
//            List<IVendor> companies = new List<IVendor>();
//            if (!_scenariocontext.ContainsKey(SupplierList))
//            {
//                companies.Add(new Supplier()
//                {
//                    Name = companyname,
//                    Id = Guid.NewGuid()
//                });
//            }
//            else
//            {
//                companies = _scenariocontext.Get<List<IVendor>>(SupplierList);
//                companies.Add(new Supplier() { 
//                    Name = companyname,
//                    Id = Guid.NewGuid()
//                });
//                _scenariocontext.Remove(SupplierList);
//            }
            
//            _scenariocontext.Add(SupplierList, companies);
//        }

//        [Given("The (.*) company sells to the (.*) company")]
//        public void GivenACompanySellsToAnotherCompany(string companyname1, string companyname2)
//        {
//            List<IVendor> companies = (List<IVendor>)_scenariocontext[SupplierList];
//            if (!_scenariocontext.ContainsKey(ShipmentList))
//            {
//                _scenariocontext.Add(ShipmentList, new List<Shipment>()
//                {
//                    new Shipment()
//                    {
//                        Sender = companies.First(p => p.Name == companyname1),
//                        Receiver = companies.First(p => p.Name == companyname2)
//                    }
//                });
//            }

//            else
//            {
//                List<Shipment> shippments = _scenariocontext.Get<List<Shipment>>(ShipmentList);
//                _scenariocontext.Remove(ShipmentList);
//                shippments.Add(new Shipment()
//                {
//                    Sender = companies.First(p => p.Name == companyname1),
//                    Receiver = companies.First(p => p.Name == companyname2)
//                });
//                _scenariocontext.Add(ShipmentList, shippments);
//            }
//        }

//        [Given("The (.*) company sells to the (.*) company for (.*)")]
//        public void GivenACompanySellsWithPrice(string companyname1, string companyname2, Decimal price)
//        {
//            List<IVendor> companies = (List<IVendor>)_scenariocontext[SupplierList];
//            if (!_scenariocontext.ContainsKey(ShipmentList))
//            {
//                _scenariocontext.Add(ShipmentList, new List<Shipment>()
//                {
//                    new Shipment()
//                    {
//                        Sender = companies.First(p => p.Name == companyname1),
//                        Receiver = companies.First(p => p.Name == companyname2),
//                        Products = new List<Product>()
//                        {
//                            new Product()
//                            {
//                                Price = price
//                            }
//                        }
//                    }
//                });
//            }
//            else
//            {
//                List<Shipment> shippments = _scenariocontext.Get<List<Shipment>>(ShipmentList);
//                _scenariocontext.Remove(ShipmentList);
//                shippments.Add(new Shipment()
//                {
//                    Sender = companies.First(p => p.Name == companyname1),
//                    Receiver = companies.First(p => p.Name == companyname2),
//                    Products = new List<Product>()
//                    {
//                        new Product()
//                        {
//                            Price = price
//                        }
//                    }
//                });
//                _scenariocontext.Add(ShipmentList, shippments);
//            }
//        }

//        [When("We want leaves")]
//        public void WhenTheCheapestPathHasLeaves()
//        {
//            SupplyChainCheapestPath sp = _scenariocontext.Get<SupplyChainCheapestPath>("pathfinderobject");
//            List<Shipment> shipments = _scenariocontext.Get<List<Shipment>>(ShipmentList);
//            List<IVendor> companies = _scenariocontext.Get<List<IVendor>>(SupplierList);
//            List<IVendor> leaves = sp.FindLeafSuppliers(shipments, companies);
//            _scenariocontext.Add("leaves", leaves);
//        }

//        [When("We want roots")]
//        public void WhenTheCheapestPathHasRoots()
//        {
//            SupplyChainCheapestPath sp = _scenariocontext.Get<SupplyChainCheapestPath>("pathfinderobject");
//            List<Shipment> shipments = _scenariocontext.Get<List<Shipment>>(ShipmentList);
//            List<IVendor> companies = _scenariocontext.Get<List<IVendor>>(SupplierList);
//            List<IVendor> roots = sp.FindRootSuppliers(shipments, companies);
//            _scenariocontext.Add("roots", roots);
//        }

//        [When("We want the cheapest path")]
//        public void WhenWeWantTheCheapestPath()
//        {
//            SupplyChainCheapestPath sp = _scenariocontext.Get<SupplyChainCheapestPath>("pathfinderobject");
//            List<Shipment> shipments = _scenariocontext.Get<List<Shipment>>(ShipmentList);
//            List<IVendor> companies = _scenariocontext.Get<List<IVendor>>(SupplierList);
//            List<IVendor> roots = sp.FindRootSuppliers(shipments, companies);
//            List<IVendor> leaves = sp.FindRootSuppliers(shipments, companies);
//            List<(string, decimal)> paths = new List<(string, decimal)>();

//            foreach (IVendor r in roots)
//            {
//                foreach (IVendor l in leaves)
//                {
//                    sp.FindSupplyChainPaths(shipments, r, l, true);
//                }
//            }

//            _scenariocontext.Add("paths", paths);
//        }


//        [Then("The cheapest path list has (.*) leaves")]
//        public void ThenTheCheapestPathHasXLeaves(int leafcount)
//        {
//            List<IVendor> leaves = _scenariocontext.Get<List<IVendor>>("leaves");
//            leaves.Count().Should().Be(leafcount);
//        }

//        [Then("The cheapest path list has (.*) roots")]
//        public void ThenTheCheapestPathHasXRoots(int rootcount)
//        {
//            List<IVendor> roots = _scenariocontext.Get<List<IVendor>>("roots");
//            roots.Count().Should().Be(rootcount);
//        }

//        [Then("The cheapest path is \"(.*)\" for (.*)")]
//        public void ThenTheCheapestPathIs(string path, decimal cost)
//        {
//            List<(string,decimal)> paths = _scenariocontext.Get<List<(string,decimal)>>("paths");

//            Assert.IsTrue(paths.Contains((path, cost)));
//        }
//    }
//}
