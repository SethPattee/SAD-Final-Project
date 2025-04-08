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
        private ProductionTarget _target;
        public ProductionTarget TargetItem { get => _target;
            set { 
                    _target = value;
                    OnPropertyChanged(nameof(TargetItem));
                } 
        }

        private double[] _targetoverdays;
        public  double[] TargetOverDays {
            get => _targetoverdays; 
            set {
                    _targetoverdays = value;
                    OnPropertyChanged(nameof(TargetOverDays));
                }
        }
		private double[] _targetProducedoverdays;
		public double[] TargetProducedOverDays
		{
			get => _targetProducedoverdays;
			set
			{
				_targetProducedoverdays = value;
				OnPropertyChanged(nameof(TargetProducedOverDays));
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
            TargetOverDays = Array.Empty<double>();
            TargetProducedOverDays = Array.Empty<double>();
            DaysRun = Array.Empty<double>();
            DayCompleted = -1;
            Issues = new ObservableCollection<string>();
            TargetItem = new ProductionTarget();
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
