using Avalonia;
using Avalonia.Media;
using NP.Avalonia.Visuals.ColorUtils;
using System;

namespace NP.ThemingPrototype
{
    class Program
    {
        public static void Main(string[] args)
        {
            Color color = Color.Parse(args[0]);

            Console.WriteLine(color.Invert().ToStr());
        }
    }
}
