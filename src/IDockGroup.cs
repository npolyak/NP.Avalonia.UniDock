using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace NP.AvaloniaDock
{
    public interface IDockGroup : IControl
    {
        DockManager? TheDockManager { get; set; }

        IDockGroup? DockParent { get; }

        IList<IDockGroup>? DockChildren { get; }
    }

    public interface IDockGroupDockManagerContainer : IDockGroup
    {
        DockManagerContainer TheDockManagerContainer { get; }

        DockManager? IDockGroup.TheDockManager
        {
            get => TheDockManagerContainer.TheDockManager;

            set => TheDockManagerContainer.TheDockManager = value;
        }
    }
}
