using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models;

public class Shipment : INotifyPropertyChanged
{
    private ObservableCollection<Product> _products = new ObservableCollection<Product>();
    private IVendor _sender;
    private IVendor _reciever;
    private bool _isRecurring = true;
    public bool IsRecurring {
        get { return _isRecurring; }
        set
        {   _isRecurring = value; 
            OnPropertyChanged(nameof(IsRecurring));
        }
    }
    private int _timeToDeliver;
    public int TimeToDeliver
    {
        get { return _timeToDeliver; }
        set
        {
            _timeToDeliver = value;
            OnPropertyChanged(nameof(TimeToDeliver));
        }
    }
    public int TimeUntilNextDelivery;

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
        TimeToDeliver = 1;
        TimeUntilNextDelivery = TimeToDeliver;
    }

    protected void OnPropertyChanged(string PropertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    public ObservableCollection<Product> Products
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

    public void ProcessTime()
    {
        
        if (TimeUntilNextDelivery  <= 0)
        {
            if (IsRecurring)
                TimeUntilNextDelivery = TimeToDeliver - 1;
        }
        else 
            TimeUntilNextDelivery--;
    }
}
