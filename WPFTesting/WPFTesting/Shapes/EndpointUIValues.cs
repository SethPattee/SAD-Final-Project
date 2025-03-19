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
        get => ((EndpointNode)supplier).Balance;
        set
        {
            ((EndpointNode)supplier).Balance = value;
            OnPropertyChanged(nameof(supplier));
        }
    }
    public EndpointUIValues()
    {
        supplier = new EndpointNode();
        SetDefaultValues();
    }
    public void SetDefaultValues()
    {
        Position = new System.Drawing.Point(120, 120);
        supplier = new EndpointNode()
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
                        //Units = "Boards"
                    },
                    new Product()
                    {
                        ProductName = "Glue",
                        Price = (decimal)0.01,
                        Quantity = (float)3000.0,
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
            DeliveryRequirementsList = new ObservableCollection<Product>()
            {
                new Product() { ProductName = "Door", Quantity = 1, Price = (decimal)300 }
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
        end.supplier = new EndpointNode();
        end.supplier.Name = supplier.Name;
        end.supplier.ComponentInventory = CopyMaker.makeShallowCopyOfProductColection(supplier.ComponentInventory);
        end.supplier.ProductInventory = CopyMaker.makeShallowCopyOfProductColection(supplier.ProductInventory);
        end.supplier.Id = supplier.Id;
        end.Position = new System.Drawing.Point();
        ((EndpointNode)end.supplier).DeliveryRequirementsList = CopyMaker.makeShallowCopyOfProductColection(((EndpointNode)supplier).DeliveryRequirementsList);
        ((EndpointNode)end.supplier).ProductionList = CopyMaker.makeShalowCopyColectionOfProductionLine(((EndpointNode)supplier).ProductionList);
        ((EndpointNode)end.supplier).Balance = ((EndpointNode)supplier).Balance;
        return end;
    }
}
