using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class DeliveryLine: INotifyPropertyChanged
    {
        public DeliveryLine()
        {
            _deliveryItem = new();
            IsRecurring = false;
            IsFulfilled = false;
            TotalPrice = 0;
        }

        public DeliveryLine(Product p, bool recurring, bool fulfilled)
        {
            DeliveryItem = p;
            IsRecurring = recurring;
            IsFulfilled = fulfilled;

            TotalPrice = DeliveryItem.Price * (decimal)DeliveryItem.Quantity;
        }


        private Product _deliveryItem;
        public Product DeliveryItem
        {
            get => _deliveryItem;
            set
            {
                _deliveryItem = value;
                TotalPrice = (decimal)_deliveryItem.Quantity * _deliveryItem.Price;

                OnPropertyChanged(nameof(DeliveryItem));
            }
        }

        private decimal _totalPrice;
        public decimal TotalPrice {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private bool _isRecurring;
        public bool IsRecurring
        {
            get => _isRecurring;
            set
            {
                _isRecurring = value;
                OnPropertyChanged(nameof(IsRecurring));
            }
        }

        private bool _isFulfilled;
        public bool IsFulfilled
        {
            get => _isFulfilled;
            set
            {
                _isFulfilled = value;
                OnPropertyChanged(nameof(IsFulfilled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
