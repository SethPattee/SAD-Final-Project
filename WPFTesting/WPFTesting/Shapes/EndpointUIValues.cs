using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FactorSADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorySADEfficiencyOptimizer.Shapes;

public class EndpointUIValues : SupplierUIValues, INotifyPropertyChanged
{
    public decimal Profit
    {
        get => ((EndpointNode)Supplier).Balance;
        set
        {
            ((EndpointNode)Supplier).Balance = value;
            OnPropertyChanged(nameof(Supplier));
        }
    }
    public EndpointUIValues()
    {
        Supplier = new EndpointNode();
        SetDefaultValues();
    }
    public void SetDefaultValues()
    {
        Position = new System.Drawing.Point(120, 120);
        Supplier = new EndpointNode()
        {
            Id = Guid.NewGuid(),
            Balance = 1000,
            Name = "Factory",
            ComponentInventory = new ObservableCollection<Product>()
                {
                    new Product()
                    {
                        ProductName = "Nails",
                        Price = (decimal)0.1,
                        Quantity = 2000
                    },
                    new Product()
                    {
                        ProductName = "Wood",
                        Price = (decimal)3.00,
                        Quantity = 40,
                        Units = "Boards"
                    },
                    new Product()
                    {
                        ProductName = "Glue",
                        Price = (decimal)0.01,
                        Quantity = (float)3000.0,
                        Units = "mL"
                    }
                },

            ProductInventory = new ObservableCollection<Product>()
                {
                    new Product()
                    {
                        ProductName = "Door",
                        Price = (decimal)300,
                        Quantity = 2
                    }
                },
            ActiveDeliveryLines = new ObservableCollection<DeliveryLine>()
            {
                new(new Product() { ProductName = "Door", Quantity = 1, Price = (decimal)300 }, true, true)
            },

            ProductionList = new ObservableCollection<ProductLine>()
            {
                new ProductLine()
                {
                    ResultingProduct = new Product()
                    {
                        ProductName = "red herring",
                        Quantity = 1
                    },
                    Components = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "seemingly important clue",
                            Quantity = 1
                        }
                    }
                },
                new ProductLine()
                {
                    ResultingProduct = new Product()
                    {
                        ProductName = "Marriage",
                        Quantity = 1
                    },
                    Components = new ObservableCollection<Product>()
                    {
                        new Product()
                        {
                            ProductName = "commitment",
                            Quantity = 20000
                        }
                    }
                }
            }
        };
    }
    public new EndpointUIValues ShallowCopy()
    {
        EndpointUIValues end = new EndpointUIValues();
        end.Supplier = new EndpointNode();
        end.Supplier.Name = Supplier.Name;
        end.Supplier.ComponentInventory = CopyMaker.makeShallowCopyOfProductColection(Supplier.ComponentInventory);
        end.Supplier.ProductInventory = CopyMaker.makeShallowCopyOfProductColection(Supplier.ProductInventory);
        end.Supplier.Id = Supplier.Id;
        end.Position = new System.Drawing.Point();
        ((EndpointNode)end.Supplier).ActiveDeliveryLines = CopyMaker.makeShallowCopyOfDLCollection(((EndpointNode)Supplier).ActiveDeliveryLines);
        ((EndpointNode)end.Supplier).ProductionList = CopyMaker.makeShalowCopyColectionOfProductionLine(((EndpointNode)Supplier).ProductionList);
        ((EndpointNode)end.Supplier).Balance = ((EndpointNode)Supplier).Balance;
        return end;
    }
}
