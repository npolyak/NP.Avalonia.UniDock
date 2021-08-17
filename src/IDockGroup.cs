using Avalonia.Controls;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public interface IDockGroup : IControl
    {
        IDockGroup? DockParent { get; }

        IList<IDockGroup>? DockChildren { get; }
    }
}
