using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Avalonia.UniDock;
using NP.Avalonia.Visuals.Behaviors;
using System.Collections.ObjectModel;

namespace NP.TestBindingBehavior
{
    public partial class MainWindow : Window
    {
        ObservableCollection<DockManagerContainer> _collection = new ObservableCollection<DockManagerContainer>();
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            SimpleDockGroup dockGroup = this.FindControl<SimpleDockGroup>("TheSimpleDockGroup");

            _collection.Add(new DockManagerContainer());
            _collection.Add(new DockManagerContainer());

            AttachedToPlainBindingBehavior<IControl, DockManager, DockManagerContainer> _behavior =
                new AttachedToPlainBindingBehavior<IControl, DockManager, DockManagerContainer>
                (
                    dockGroup,
                    DockAttachedProperties.TheDockManagerProperty, 
                    _collection,
                    (container, dm) => container.TheDockManager = dm);

            _collection.Add(new DockManagerContainer());

            dockGroup.TheDockManager = null;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
