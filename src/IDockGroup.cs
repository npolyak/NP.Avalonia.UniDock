using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public interface IDockGroup : IControl, IRemovable
    {
        DockManager? TheDockManager { get; }

        IDockGroup? DockParent { get; set; }

        IList<IDockGroup>? DockChildren { get; }
    }
}
