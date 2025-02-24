using FactorSADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.ViewModel
{
    public class IndividualTargetModel : INotifyPropertyChanged
    {
        private ProductionTarget _productionTarget;
        public ProductionTarget TargetItem { get => _productionTarget;
            set { 
                    _productionTarget = value;
                    OnPropertyChanged(nameof(TargetItem));
                } 
        }
        private ObservableCollection<ProductionTarget> _targetoverdays;
        public ObservableCollection<ProductionTarget> TargetOverDays {
            get => _targetoverdays; 
            set {
                    _targetoverdays = value;
                    OnPropertyChanged(nameof(TargetOverDays));
                }
        }
        private double[] _daysrun;

        public event PropertyChangedEventHandler? PropertyChanged;

        public double[] DaysRun {
            get => _daysrun;
            set {
                    _daysrun = value;
                    OnPropertyChanged(nameof(DaysRun));
                }
        }

        private ObservableCollection<string> _issues;
        // Convert issues to string messages, and then dump in here for a message box.
        public ObservableCollection<string> Issues
        {
            get => _issues;
            set
            {
                _issues = value;
                OnPropertyChanged(nameof(Issues));
            }
        }

        private double _DayCompleted;
        public double DayCompleted
        {
            get => _DayCompleted;
            set
            {
                _DayCompleted = value;
                OnPropertyChanged(nameof(DayCompleted));
            }
        }

        public IndividualTargetModel()
        {
            TargetItem = new ProductionTarget();
            TargetOverDays = new ObservableCollection<ProductionTarget>();
            _daysrun = Array.Empty<double>();
            _DayCompleted = -1;
            _issues = new ObservableCollection<string>();
            _productionTarget = new ProductionTarget();
            _targetoverdays = new ObservableCollection<ProductionTarget>();
        }

        protected void OnPropertyChanged(string? name = null)
        {
            if (name is not null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
