using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public class Shipment : INotifyPropertyChanged
{
    private List<Product> _products = new List<Product>();
    private IVendor _sender;
    private IVendor _reciever;
	public string FromJoiningBoxCorner { get; set; }
	public string ToJoiningBoxCorner { get; set; }
	public Guid Id { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public Shipment()
    {
        Id = Guid.NewGuid();
        _sender = new Supplier();
        _reciever = new Supplier();
        FromJoiningBoxCorner = string.Empty;
        ToJoiningBoxCorner = string.Empty;
    }

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

    public IVendor Sender
    {
        get => _sender;
        set
        {
            _sender = value;
            OnPropertyChanged(nameof(Sender));
        }
    }
    public IVendor Receiver
    {
        get => _reciever;
        set
        {
            _reciever = value;
            OnPropertyChanged(nameof(Receiver));
        }
    }

}
