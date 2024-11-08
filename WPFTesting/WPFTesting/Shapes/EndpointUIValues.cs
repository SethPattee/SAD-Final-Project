﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;

namespace WPFTesting.Shapes
{
    public class EndpointUIValues : SupplierUIValues
    {
        private decimal? _profit;
        public decimal? Profit
        {
            get => _profit;
            set {
                _profit = value;
                OnPropertyChanged(nameof(Profit));
            }
        }
        public EndpointUIValues()
        {
            supplier = new EndpointNode();
        }
        public void SetDefaultValues()
        {
            xPosition = 120;
            yPosition = 120;
            Profit = 10;
            supplier = new EndpointNode()
            {
                Id = new Guid(),
                Name = "Factory",
                ComponentInventory = new List<Product>()
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

                ProductInventory = new List<Product>()
                    {
                        new Product()
                        {
                            ProductName = "Door",
                            Price = (decimal)300,
                            Quantity = 2
                        }
                    }
            };
            }

    }
}
