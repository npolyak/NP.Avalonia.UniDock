using Avalonia.Controls;
using Avalonia.Layout;

namespace NP.Avalonia.UniDock
{
    public interface IControlWithOrientation : IControl
    {
        public Orientation TheOrientation { get; set; }
    }
}
