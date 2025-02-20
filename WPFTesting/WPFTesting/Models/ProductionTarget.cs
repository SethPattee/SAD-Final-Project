using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorSADEfficiencyOptimizer.Models
{
    public class ProductionTarget : INotifyPropertyChanged
    {
        private Product? _productiontarget;
        public Product? ProductTarget
        {
            get => _productiontarget;
            set
            {
                _productiontarget = value;
                OnPropertyChanged(nameof(ProductionTarget));
            }
        }
        private float _targetQuantity;
        public float TargetQuantity { 
            get => _targetQuantity; 
            set
            {
                _targetQuantity = value;
                OnPropertyChanged(nameof(TargetQuantity));
            }
        }
        private int _dueDate;
        public int DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }
        private float _currentAmount;
        public float CurrentAmount
        {
            get => _currentAmount;
            set
            {
                _currentAmount = value;
                OnPropertyChanged(nameof(CurrentAmount));
            }
        }
        private StatusEnum _status;
        public StatusEnum Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        private bool _isTargetEnabled;
        public bool IsTargetEnabled
        {
            get => _isTargetEnabled;
            set
            {
                _isTargetEnabled = value;
                OnPropertyChanged(nameof(IsTargetEnabled));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
