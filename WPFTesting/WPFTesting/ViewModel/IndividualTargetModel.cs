using FactorSADEfficiencyOptimizer.Models;
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
        public ProductionTarget ProductionTarget { get => _productionTarget;
            set { 
                    _productionTarget = value;
                    OnPropertyChanged(nameof(ProductionTarget));
                } 
        }
        private ObservableCollection<ProductionTarget> _TargetOverDays;
        public ObservableCollection<ProductionTarget> TargetOverDays {
            get => _TargetOverDays; 
            set {
                    TargetOverDays = value;
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



        public IndividualTargetModel()
        {
            ProductionTarget = new ProductionTarget();
            TargetOverDays = new ObservableCollection<ProductionTarget>();
            DaysRun = Array.Empty<double>();
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
