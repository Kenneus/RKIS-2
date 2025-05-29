using Avalonia.Data.Converters;
using System;
using System.Globalization;
using RKISApp.Models;

namespace RKISApp.Converters
{
    public class EditingStudentToContentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is Student ? "Сохранить" : "Добавить";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}