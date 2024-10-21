using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.ViewModel
{
    public class SupplyChainCheapestPath
    {
        public List<string> CalculateSupplyChainOutputAsync(List<Shippment> s, List<Supplier> p)
        {
            List<string> CheapestPaths = new List<string>();
            List<Supplier> SupplierRoots;
            List<Supplier> SupplierLeaves;

            if (s.Count > 0)
            {
                SupplierRoots = FindRootSuppliers(s, p);

                foreach (Supplier supplierRoot in SupplierRoots)
                {
                    // CheapestPaths.Add(DijkstraSupplyChain(s, supplierRoot));
                }
            }
            return CheapestPaths;
        }

        public List<Supplier> FindRootSuppliers(List<Shippment> s, List<Supplier> p)
        {
            List<Supplier> SupplierRoots = new List<Supplier>();
            foreach (Supplier testsupp in p) // Node to test against
            {
                bool supplierisasourceflag = false;
                bool supplierisdestflag = false;
                foreach (Shippment testnode in s) // connection being checked. 
                {
                    // If a node is ever a recipient, it is not a root.
                    if (testsupp == testnode.Supplier)
                        supplierisasourceflag = true;
                    if (testsupp == testnode.Reciever)
                        supplierisdestflag = true;
                }

                if (supplierisasourceflag && !supplierisdestflag)
                {
                    SupplierRoots.Add(testsupp);
                }
            }

            return SupplierRoots;
        }

        public List<Supplier> FindLeafSuppliers(List<Shippment> s, List<Supplier> p)
        {
            List<Supplier> SupplierRoots = new List<Supplier>();
            foreach (Supplier testsupp in p) // Node to test against
            {
                bool supplierisasourceflag = false;
                bool supplierisdestflag = false;
                foreach (Shippment testnode in s) // connection being checked. 
                {
                    // If a node is ever a recipient, it is not a root.
                    if (testsupp == testnode.Supplier)
                        supplierisasourceflag = true;
                    if (testsupp == testnode.Reciever)
                        supplierisdestflag = true;
                }

                if (!supplierisasourceflag && supplierisdestflag)
                {
                    SupplierRoots.Add(testsupp);
                }
            }

            return SupplierRoots;
        }

        public string DijkstraSupplyChain(List<Shippment> s, Supplier Source, Supplier Destination)
        {
            return "";
        }
    }
}
