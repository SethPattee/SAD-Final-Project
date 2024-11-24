﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models
{
    public class ComponentToProductTransformer
    {
        private ObservableCollection<Product> _components;
        private Product _product;
        public ObservableCollection<Product> Components
        {
            get => _components ?? (new ObservableCollection<Product>());
            set => _components = value;
        }

        public Product ResultingProduct
        {
            get => _product ?? (new Product()
            {
                Quantity = 1,
                Price = 0,
                ProductName = "Duck"
            });
            set => _product = value;
        }
    }
}
