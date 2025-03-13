using FactorySADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FactorySADEfficiencyOptimizer.UIComponent.EventArguments
{
    public class CloseScheduledShipmentsEventArgs : RoutedEventArgs
    {
        public bool UserSaveChoice;
    }
}
