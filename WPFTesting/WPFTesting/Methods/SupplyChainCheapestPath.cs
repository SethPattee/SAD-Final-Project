using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.Methods
{
    public static class SupplyChainCheapestPath
    {
        public static List<string> CalculateSupplyChainOutputAsync(List<Shipment> s, List<Supplier> p)
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

        public static List<Supplier> FindRootSuppliers(List<Shipment> s, List<Supplier> p)
        {
            List<Supplier> SupplierRoots = new List<Supplier>();
            foreach (Supplier testsupp in p) // Node to test against
            {
                bool supplierisasourceflag = false;
                bool supplierisdestflag = false;
                foreach (Shipment testnode in s) // connection being checked. 
                {
                    // If a node is ever a recipient, it is not a root.
                    if (testsupp == testnode.Sender)
                        supplierisasourceflag = true;
                    if (testsupp == testnode.Receiver)
                        supplierisdestflag = true;
                }

                if (supplierisasourceflag && !supplierisdestflag)
                {
                    SupplierRoots.Add(testsupp);
                }
            }

            return SupplierRoots;
        }

        public static List<Supplier> FindLeafSuppliers(List<Shipment> s, List<Supplier> p)
        {
            List<Supplier> SupplierRoots = new List<Supplier>();
            foreach (Supplier testsupp in p) // Node to test against
            {
                bool supplierisasourceflag = false;
                bool supplierisdestflag = false;
                foreach (Shipment testnode in s) // connection being checked. 
                {
                    // If a node is ever a recipient, it is not a root.
                    if (testsupp.Id == testnode.Sender.Id)
                        supplierisasourceflag = true;
                    if (testsupp.Id == testnode.Receiver.Id)
                        supplierisdestflag = true;
                }

                if (!supplierisasourceflag && supplierisdestflag)
                {
                    SupplierRoots.Add(testsupp);
                }
            }

            return SupplierRoots;
        }

        public static string DijkstraSupplyChain(List<Shipment> s, Supplier Source, Supplier Destination)
        {
            return "";
        }
    }
}
