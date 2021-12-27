using Avalonia.Layout;
using Avalonia.Media;
using NP.Utilities;
using System;

namespace NP.Avalonia.Visuals.ColorUtils
{
    public static class ColorHelper
    {
        /// <summary>
        /// this is agorithm from https://stackoverflow.com/questions/39118528/rgb-to-hsl-conversion/39147465#39147465
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static HslColor ToHSL(this Color color)
        {
            byte a = color.A;

            double r = color.R / 255d;
            double g = color.G / 255d;
            double b = color.B / 255d;

            double max = MathUtils.Max(r, g, b);
            double min = MathUtils.Min(r, g, b);

            double delta = max - min;

            double hue;
            if (delta.AlmostEquals(0d)) // some very small number since c is double
            {
                hue = 0;
            }
            else
            {
                double segment;
                double shift;
                if (max.AlmostEquals(r))
                {
                    segment = (g - b) / delta;
                    shift = segment < 0 ? 6 : 0;
                }
                else if (max.AlmostEquals(g))
                {
                    segment = (b - r) / delta;
                    shift = 2;
                }
                else if (max.AlmostEquals(b))
                {
                    segment = (r - g) / delta;
                    shift = 4;
                }
                else
                {
                    throw new ProgrammingError("Should never get here.");
                }
                hue = segment + shift;

                hue *= 60;
            }

            double sum = max + min;
            double lightness = sum / 2d;
            double saturation = delta / (1 - Math.Abs(1 - sum));

            return new HslColor(a, (float) hue, (float) saturation, (float) lightness);
        }

        private static double HueToRGB(this double v1, double v2, double hue)
        {
            if (hue < 0)
                hue += 1;

            if (hue > 1)
                hue -= 1;

            if ((6 * hue) < 1)
                return (v1 + (v2 - v1) * 6 * hue);

            if ((2 * hue) < 1)
                return v2;

            if ((3 * hue) < 2)
                return (v1 + (v2 - v1) * ((2.0f / 3) - hue) * 6);

            return v1;
        }

        // borrowed from https://www.programmingalgorithms.com/algorithm/hsl-to-rgb/
        public static Color ToColor(this HslColor hslColor)
        {
            (byte a, double h, double s, double l) = hslColor;


            if (s.AlmostEquals(0))
            {
                byte result = (byte)(l * 255d);

                return new Color(a, result, result, result);
            }

            double hue = h / 360d;

            double v2 = (l < 0.5) ? (l * (1 + s)) : ((l + s) - (l * s));
            double v1 = 2 * l - v2;

            byte r = (byte)(v1.HueToRGB(v2, hue + (1d / 3d)) * 255d);
            byte g = (byte)(v1.HueToRGB(v2, hue) * 255d);
            byte b = (byte)(v1.HueToRGB(v2, hue - (1d / 3d)) * 255d);

            return new Color(a, r, g, b);
        }

        public static Color ToColor(int r, int g, int b, int a = 255)
        {
            return new Color((byte)a, (byte)r, (byte)g, (byte)b);
        }

        public static string ToStr(this Color c)
        {
            return c.ToString().ToUpper();
        }

        public static string ToIntStr(this Color c)
        {
            (int a, int r, int g, int b) = (c.A, c.R, c.G, c.B);
            return $"{a}, {r}, {g}, {b}";
        }

        // inverts light to dark and vice versa
        public static HslColor Invert(this HslColor hslColor)
        {
            (byte a, float h, float s, float l) = hslColor;

            return new HslColor(a, h, s, 1 - l);
        }

        // inverts light to dark and vice versa
        public static Color Invert(this Color color)
        {
            HslColor invertedHslColor = color.ToHSL().Invert();

            return invertedHslColor.ToColor();
        }

        public static Func<Color, Color> InvertColor { get; } = Invert;

        public static SolidColorBrush ToBrush(this Color color)
        {
            return new SolidColorBrush(color);
        }
    }
}
