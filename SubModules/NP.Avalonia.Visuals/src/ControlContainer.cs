using Avalonia.Controls;

namespace NP.Avalonia.Visuals
{
    internal class ControlContainer
    {
        public IControl Control { get; }

        internal ControlContainer(IControl control)
        {
            Control = control;
        }
    }
}
