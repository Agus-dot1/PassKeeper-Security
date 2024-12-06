﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PassKeeper.Helpers;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? Visibility.Collapsed : Visibility.Visible;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null; // not needed
    }
}