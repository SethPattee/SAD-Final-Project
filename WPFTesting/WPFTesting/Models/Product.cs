﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class Product : INotifyPropertyChanged
    {
        private float _quantity;

        private Decimal _price;
        private Guid _catalogueKey = Guid.NewGuid();
        private string _units = "";
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public float Quantity
        {
            get => _quantity; 
            set
            {
                _quantity = value; OnPropertyChanged(nameof(Quantity));
            }
        }
        public string ProductName
        {
            get {
				if (ProductCatalog.Products.TryGetValue(_catalogueKey, out GeneralProduct? genProd))
                {
                    return genProd.ProductName;
                }
                else
                {
                    return "No Name";
                }
            } 
            set
            {
				if (ProductCatalog.Products.TryGetValue(_catalogueKey, out GeneralProduct? genProd))
				{
					genProd.ProductName = value;
				}
				else
				{
                    string v = string.Empty;
                    foreach (Guid Key in ProductCatalog.Products.Keys)
                    {
                        if (ProductCatalog.Products.TryGetValue(Key, out GeneralProduct? Prod))
                        {
                            if (value == Prod.ProductName)
                            {
                                _catalogueKey = Key;
                                break;
                            }
                        }               
                    }
				    ProductCatalog.Products[_catalogueKey] = new GeneralProduct() { ProductName = value};
				}
                OnPropertyChanged(nameof(ProductName));
            }
        }
        public string Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(nameof(Units)); }
        }
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        public Product ShallowCopy()
        {
            Product p = new Product();
            p.ProductName = this.ProductName;
            p.Price = this.Price;
            p.Quantity = this.Quantity;
            p.Units = this.Units;
            return p;
        }
    }
}
