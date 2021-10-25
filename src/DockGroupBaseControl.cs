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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;

namespace NP.Avalonia.UniDock
{
    public class DockGroupBaseControl : TemplatedControl
    {
        public event Action<IDockGroup>? DockIdChanged;

        #region DockId Styled Avalonia Property
        public string DockId
        {
            get { return GetValue(DockIdProperty); }
            set { SetValue(DockIdProperty, value); }
        }

        public static readonly StyledProperty<string> DockIdProperty =
            AvaloniaProperty.Register<TabbedDockGroup, string>
            (
                nameof(DockId)
            );
        #endregion Id Styled Avalonia Property

        private void FireDockIdChanged()
        {
            DockIdChanged?.Invoke((IDockGroup) this);
        }

        protected void OnDockIdChanged(AvaloniaPropertyChangedEventArgs e)
        {
            FireDockIdChanged();
        }

        public override string ToString() => DockId;

        #region ShowCompass Styled Avalonia Property
        public bool ShowCompass
        {
            get { return GetValue(ShowCompassProperty); }
            set { SetValue(ShowCompassProperty, value); }
        }

        public static readonly StyledProperty<bool> ShowCompassProperty =
            DockAttachedProperties.ShowCompassProperty.AddOwner<TabbedDockGroup>();
        #endregion ShowCompass Styled Avalonia Property

        public IDictionary<IDockGroup, IDisposable> ChildSubscriptions { get; } =
            new Dictionary<IDockGroup, IDisposable>();

        public Subject<Unit> DockChangedWithin { get; } = new Subject<Unit>();


        #region HasStableChild Direct Avalonia Property
        private bool _HasStableChild = default;

        public static readonly DirectProperty<DockGroupBaseControl, bool> HasStableChildProperty =
            AvaloniaProperty.RegisterDirect<DockGroupBaseControl, bool>
            (
                nameof(HasStableDescendant),
                o => o.HasStableDescendant,
                (o, v) => o.HasStableDescendant = v
            );

        public bool HasStableDescendant
        {
            get => _HasStableChild;
            protected set
            {
                SetAndRaise(HasStableChildProperty, ref _HasStableChild, value);
            }
        }

        #endregion HasStableChild Direct Avalonia Property

        private IDisposable _subscription;
        public DockGroupBaseControl()
        {
            _subscription = DockChangedWithin.Subscribe(OnDockChangedWithin);
        }

        private void OnDockChangedWithin(Unit _)
        {
            HasStableDescendant = ((IDockGroup)this).HasStableGroup();
        }
    }
}
