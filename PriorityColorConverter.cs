using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoList;

public class PriorityColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int priority)
        {
            string hex = priority switch
            {
                1 => AppSettings.Instance.HighColor,
                2 => AppSettings.Instance.MedColor,
                3 => AppSettings.Instance.LowColor,
                _ => AppSettings.Instance.MedColor
            };
            
            try 
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                return new SolidColorBrush(color);
            }
            catch 
            {
                return new SolidColorBrush(Colors.LightGray);
            }
        }
        return new SolidColorBrush(Colors.LightGray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
