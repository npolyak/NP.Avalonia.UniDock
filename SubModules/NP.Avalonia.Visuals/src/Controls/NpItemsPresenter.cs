using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Presenters;
using Avalonia.LogicalTree;
using NP.Avalonia.Visuals.Behaviors;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace NP.Avalonia.Visuals.Controls
{
    public class NpItemsPresenter : ItemsPresenter, IItemsPresenter, IChildIndexProvider
    {
        private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;
        private IItemContainerGenerator? _itemContainerGenerator;
        
        event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
        {
            add => _childIndexChanged += value;
            remove => _childIndexChanged -= value;
        }

        public NpItemsPresenter()
        {
            FindLogicalAncestorBehavior.SetAncestorType(this, typeof(NpComboBox));

            this.GetObservable(FindLogicalAncestorBehavior.AncestorProperty).Subscribe(OnAncestorChanged!);
        }

        private void OnAncestorChanged(IControl ancestor)
        {
            (ancestor as IItemsPresenterHost)?.RegisterItemsPresenter(this);
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            if (_itemContainerGenerator == null)
            {
                ItemsControl parent =
                    this.GetLogicalAncestors().OfType<ItemsControl>().First();

                _itemContainerGenerator = parent.ItemContainerGenerator;

                _itemContainerGenerator.Materialized += ContainerActionHandlerImpl;
                _itemContainerGenerator.Dematerialized += ContainerActionHandlerImpl;
                _itemContainerGenerator.Recycled += ContainerActionHandlerImpl;
            }

            return _itemContainerGenerator;
        }

        void IItemsPresenter.ItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Panel != null)
            {
                ItemsChanged(e);

                _childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs());
            }
        }

        private void ContainerActionHandlerImpl(object? sender, ItemContainerEventArgs e)
        {
            for (var i = 0; i < e.Containers.Count; i++)
            {
                _childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs(e.Containers[i].ContainerControl));
            }
        }
    }
}
