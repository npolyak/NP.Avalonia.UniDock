// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.Collections.Generic;

namespace NP.Avalonia.UniDock
{
    public class SetDockGroupBehavior<T> : IDisposable
        where T : IDockGroup
    {
        private RemoveDockGroupBehavior<T>? _removeItemBehavior;
        private SetParentBehavior<T>? _setParentBehavior;
        private SetAttachedPropertyFromParentGroupBehavior<T, DockManager>? _setDockManagerBehavior;
        public SetDockGroupBehavior(IDockGroup parent, IList<T> items)
        {
            _removeItemBehavior = new RemoveDockGroupBehavior<T>(items);
            _setParentBehavior = new SetParentBehavior<T>(parent, items);
            _setDockManagerBehavior = new SetAttachedPropertyFromParentGroupBehavior<T, DockManager>(parent, items, DockAttachedProperties.TheDockManagerProperty);
        }

        public void Dispose()
        {
            _setDockManagerBehavior?.Dispose();
            _setDockManagerBehavior = null;
            _setParentBehavior?.Dispose();
            _setParentBehavior = null;
            _removeItemBehavior?.Dispose();
            _removeItemBehavior = null;
        }
    }

    public class SetDockGroupBehavior : SetDockGroupBehavior<IDockGroup>
    {
        public SetDockGroupBehavior(IDockGroup parent, IList<IDockGroup> items) : 
            base(parent, items)
        {

        }
    }
}
