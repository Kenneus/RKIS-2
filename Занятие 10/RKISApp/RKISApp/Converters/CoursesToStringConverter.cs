using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using RKISApp.Models;

namespace RKISApp.Converters
{
    public class CoursesToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Course> courses && courses.Any())
            {
                return string.Join(", ", courses.Select(c => c.Name));
            }
            return "Нет курсов";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}