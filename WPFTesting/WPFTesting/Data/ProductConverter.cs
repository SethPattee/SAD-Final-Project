﻿using System.Globalization;
using System.Windows.Data;
using WPFTesting.Models;

namespace WPFTesting.Data;

public class ProductConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Product productData = (Product)value;
        return productData.ProductName + ", "
            + productData.Quantity.ToString() + " "
            + productData.Units + " "
            + productData.Price.ToString();
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}