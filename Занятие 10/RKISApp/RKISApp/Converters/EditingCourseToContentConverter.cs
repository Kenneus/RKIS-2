using Avalonia.Data.Converters;
using System;
using System.Globalization;
using RKISApp.Models;

namespace RKISApp.Converters
{
    public class EditingCourseToContentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is Course ? "Сохранить" : "Добавить";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}