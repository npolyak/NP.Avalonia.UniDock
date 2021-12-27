namespace NP.Avalonia.Visuals.Behaviors
{
    public enum VisualFlow
    {
        Normal,
        Reverse
    }

    public static class VisualFlowHelper
    {
        public static double ToScale(this VisualFlow visualFlow)
        {
            return visualFlow switch
            {
                VisualFlow.Normal => 1,
                _ => -1
            };
        }
    }
}
