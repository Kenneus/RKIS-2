using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using RKISApp.ViewModels;
using RKISApp.Views;

namespace RKISApp
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param is null)
            {
                Console.WriteLine("ViewLocator.Build: Parameter is null");
                return null;
            }

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            Console.WriteLine($"ViewLocator.Build: Attempting to create view for {name}");

            if (param is RKISViewModel)
            {
                Console.WriteLine("ViewLocator.Build: Creating RKISView");
                return new RKISView();
            }
            if (param is CoursesViewModel)
            {
                Console.WriteLine("ViewLocator.Build: Creating CoursesView");
                return new CoursesView();
            }

            Console.WriteLine($"ViewLocator.Build: Type {name} not found");
            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            var result = data is ViewModelBase;
            Console.WriteLine($"ViewLocator.Match: Data type = {data?.GetType().FullName}, Match = {result}");
            return result;
        }
    }
}