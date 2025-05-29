using System;
using Avalonia;
using System.Diagnostics;

namespace RKISApp
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.SetOut(new DebugWriter());
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }

    public class DebugWriter : System.IO.TextWriter
    {
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

        public override void WriteLine(string? value)
        {
            Debug.WriteLine(value);
        }

        public override void Write(string? value)
        {
            Debug.Write(value);
        }
    }
}