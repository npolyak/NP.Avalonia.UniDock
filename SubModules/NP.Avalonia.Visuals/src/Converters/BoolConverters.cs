namespace NP.Avalonia.Visuals.Converters
{
    public static class BoolConverters
    {
        public static GenericBoolConverter<bool> Not { get; } =
            new GenericBoolConverter<bool> { FalseValue = true, TrueValue = false };
    }
}
