using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FactorySADEfficiencyOptimizer.UIComponent.EventArguments
{
    class RadialNameRoutedEventArgs(string radialname) : RoutedEventArgs
    {
        public string Radial_Name { get; set; } = radialname;
    }
}
