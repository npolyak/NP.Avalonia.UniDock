namespace NP.Avalonia.Visuals.Converters
{
    public static class IntConverters
    {
        public static ToBoolConverter<int> IsPositiveConverter { get; } =
            new ToBoolConverter<int>
            {
                IsTrue = val => val > 0
            };
    }
}
