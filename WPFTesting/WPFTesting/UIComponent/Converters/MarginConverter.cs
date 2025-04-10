using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace FactorySADEfficiencyOptimizer.UIComponent.Converters;

[ValueConversion(typeof(ListView), typeof(double))]
public class MarginConverter : IValueConverter
{
    public MarginConverter()
    {
        
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double width = (double)value;
        return width - 16;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
