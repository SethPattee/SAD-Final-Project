using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.ViewModel;
using FactorySADEfficiencyOptimizer.Models;
using System.Windows;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class ProductionTarget : INotifyPropertyChanged
    {
        public bool PalacedAutoOrderForComponent = false;
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
		private ObservableCollection<string> _posibleNames = new ObservableCollection<string>();
		public ObservableCollection<string> PosibleTargetNames
		{
			set
			{
				_posibleNames = value;
				OnPropertyChanged(nameof(PosibleTargetNames));
			}
			get
			{
				return _posibleNames;
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
        private double _daycompleted;
        public double DayCompleted
        {
            get => _daycompleted;
            set
            {
                _daycompleted = value;
                OnPropertyChanged(nameof(DayCompleted));
            }
        }
        private double _countProduced = 0;
        public double ProducedSoFar
        {
            get
            {
                return _countProduced;
            }
        }

        private bool _cannotBeFulfilled;
        public bool CannotBeFulfilled
        {
            get => _cannotBeFulfilled;
            set
            {
                _cannotBeFulfilled = value;
                OnPropertyChanged(nameof(CannotBeFulfilled));
            }
        }
        private string _productsNeeded = "";
        public string ProductsNeeded
        {
            get => _productsNeeded;
            set
            {
                _productsNeeded = value;
                OnPropertyChanged(nameof(ProductsNeeded));
            }
        }

        public void AddToProducedQuantity(double addiitonalCompletedCount)
        {
            _countProduced = _countProduced + addiitonalCompletedCount;
            OnPropertyChanged(nameof(ProducedSoFar));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProductionTarget ShallowCopy()
        {
            ProductionTarget targCopy = new ProductionTarget();
            targCopy.ProductTarget = this.ProductTarget?.ShallowCopy() ?? new Product();
            targCopy.CurrentAmount = this.CurrentAmount;
            targCopy.DueDate = this.DueDate;
            targCopy.TargetQuantity = this.TargetQuantity;
            targCopy.CurrentAmount = this.CurrentAmount;
            targCopy.CannotBeFulfilled = this.CannotBeFulfilled;
            StatusEnum temStatus;
            switch (this.Status)
            {
                case (StatusEnum.Success):
                    temStatus = StatusEnum.Success;
                    break;
                case (StatusEnum.Failure):
                    temStatus = StatusEnum.Failure;
                    break;
                case (StatusEnum.Warning):
                    temStatus = StatusEnum.Warning;
                    break;
                default:
                    temStatus = StatusEnum.NotDone;
                    break;
            }
            targCopy.Status = temStatus;
            targCopy.DayCompleted = this.DayCompleted;
            targCopy.IsTargetEnabled = this.IsTargetEnabled;
            targCopy.AddToProducedQuantity(this._countProduced);
            return targCopy;
        }
    }
}
