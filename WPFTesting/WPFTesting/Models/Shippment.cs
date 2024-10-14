using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public class Shippment : INotifyPropertyChanged
{
    private List<Product> _products = new List<Product>();
    private Supplier _supplier = new Supplier();
    private Supplier _reciever = new Supplier();
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string PropertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    public List<Product> Products
    {
        get => _products;
        set
        {
            _products = value;
            OnPropertyChanged(nameof(Products));
        }
    }

    public Supplier Supplier
    {
        get => _supplier;
        set
        {
            _supplier = value;
            OnPropertyChanged(nameof(Supplier));
        }
    }
    public Supplier Reciever
    {
        get => _reciever;
        set
        {
            _reciever = value;
            OnPropertyChanged(nameof(Reciever));
        }
    }

}
