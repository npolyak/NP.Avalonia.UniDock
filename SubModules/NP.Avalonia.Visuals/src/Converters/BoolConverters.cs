using Avalonia.Data.Converters;

namespace NP.Avalonia.Visuals.Converters
{
    public static class BoolConverters
    {
        public static IMultiValueConverter And { get; } = new AndConverter();

        public static IMultiValueConverter Or { get; } = new OrConverter();

        public static IValueConverter Not { get; } =
            new GenericBoolConverter<bool> { FalseValue = true, TrueValue = false };
    }
}
