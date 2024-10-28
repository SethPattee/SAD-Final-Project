using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.Methods
{
    public class SupplyChainCheapestPath
    {
        private List<List<Shipment>> ShipmentPaths = new List<List<Shipment>>();

        public List<(string, decimal)> CalculateSupplyChainOutputAsync(List<Shipment> s, List<Supplier> p)
        {
            List<(string,decimal)> CheapestPaths = new List<(string,decimal)>();
            List<List<Shipment>> ListOfPaths = new List<List<Shipment>>();
            List<Supplier> SupplierRoots;
            List<Supplier> SupplierLeaves;

            if (s.Count > 0)
            {
                SupplierRoots = FindRootSuppliers(s, p);
                SupplierLeaves = FindLeafSuppliers(s, p);
                ShipmentPaths = new List<List<Shipment>>(); // reset the paths. Can't trust any of them now.

                foreach (Supplier supplierRoot in SupplierRoots)
                {
                    foreach (Supplier supplierLeaf in SupplierLeaves)
                    {
                        FindSupplyChainPaths(s, supplierRoot, supplierLeaf, true);
                    }
                }

                foreach (List<Shipment> shipments in ListOfPaths)
                {

                }
            }
            return CheapestPaths;
        }

        public List<Supplier> FindRootSuppliers(List<Shipment> s, List<Supplier> p)
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

        public List<Supplier> FindLeafSuppliers(List<Shipment> s, List<Supplier> p)
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

        public List<Shipment> FindSupplyChainPaths(List<Shipment> shipments, Supplier current, Supplier destination, bool isStart)
        {
            List<Shipment> returnpath = new List<Shipment>();
            foreach (Shipment s in shipments)
            {
                if(s.Sender == current)
                {
                    if(s.Receiver == destination)
                    {
                        return new List<Shipment> { s };
                    }
                    else
                    {
                       returnpath = FindSupplyChainPaths(shipments, s.Sender, destination, false);
                    }

                    if (returnpath.Exists(x => x.Receiver == destination))
                    {
                        returnpath.Add(s);
                        if (isStart)
                            ShipmentPaths.Add(returnpath);
                        
                        return returnpath;
                    }


                }
            }

            // Algorithm: Find every path between source and destination.
            // Once they're found, return a list of the paths traversed.
            // If path not traversed, throw an exception to say that it wasn't found.

            throw new ArgumentException("No path between nodes found.");
        }
    }
}
