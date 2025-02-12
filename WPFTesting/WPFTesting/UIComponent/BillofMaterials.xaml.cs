﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FactorySADEfficiencyOptimizer.Models;
namespace FactorSADEfficiencyOptimizer.UIComponent
{
    public partial class BillofMaterials : Window, INotifyPropertyChanged
    {
        public BillofMaterials()
        {
            InitializeComponent();
            DataContext = this;
            SampleData();
        }

        private List<Product> _list;
        public List<Product> List
        {
            get { return _list; }
            set
            {
                _list = value;
                OnPropertyChanged(nameof(List));
            }
        }

        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        private void SampleData()
        {
            List = new List<Product>
            {
                new Product { ProductName = "Component A", Quantity = 5, Units = "pcs", Price = 15.50M },
                new Product { ProductName = "Component B", Quantity = 3, Units = "pcs", Price = 22.75M },
                new Product { ProductName = "Component C", Quantity = 10, Units = "pcs", Price = 8.30M },
                new Product { ProductName = "Component D", Quantity = 2, Units = "pcs", Price = 50.00M }
            };

            Total = List.Sum(p => (float)p.Quantity * (float)p.Price);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
