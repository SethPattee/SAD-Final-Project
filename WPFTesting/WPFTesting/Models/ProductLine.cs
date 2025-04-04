﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class ProductLine
    {
        private ObservableCollection<Product> _components;
        private Product _product;
        private Guid _id;
        //private bool _isSelected;
        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        //public bool IsSelected
        //{
        //    get => _isSelected;
        //    set
        //    {
        //        _isEnabled = value;
        //        OnPropertyChanged(nameof(IsSelected));
        //    }
        //}
        public ObservableCollection<Product> Components
        {
            get => _components ?? (new ObservableCollection<Product>());

            set {
                _components = value;
                OnPropertyChanged(nameof(Components));
            }
        }

        public Product ResultingProduct
        {
            get => _product ?? (new Product()
            {
                Quantity = 1,
                Price = 0,
                ProductName = "Duck"
            });
            set {
                _product = value;
                OnPropertyChanged(nameof(ResultingProduct));
            }
        }
        public Guid ProductLineId
        {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductLineId));
            }
        }   

        public ProductLine()
        {
            ProductLineId = Guid.NewGuid();
            _components = new ObservableCollection<Product>();
            _product = new Product();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProductLine ShallowCopy()
        {
            var nl = new ProductLine();
            nl.ResultingProduct = this.ResultingProduct.ShallowCopy();
            nl.ProductLineId = this.ProductLineId;
            nl.IsEnabled = this.IsEnabled;
            nl.Components = FactorSADEfficiencyOptimizer.ViewModel.CopyMaker.makeShallowCopyOfProductColection(this.Components);
            return nl;
        }
    }
}
