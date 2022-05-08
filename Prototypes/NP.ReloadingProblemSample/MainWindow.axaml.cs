using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDockService;
using NP.Utilities;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.ReloadingProblemSample
{
    public partial class MainWindow : Window
    {
        private DockManager _dockManager;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _dockManager = (DockManager) this.FindResource("TheDockManager")!;
            _dockManager.DockItemsViewModels =
                (
                    new DockItemViewModelBase[]
                    {
                        new DockItemViewModelBase
                        {
                            DockId = "1",
                            DefaultDockGroupId = "DocumentGroup",
                            Header = "Tab 1",
                            Content = "Hello World",
                            ContentTemplateResourceKey = "ReloadingDataTemplate",
                        },
                        new DockItemViewModelBase
                        {
                            DockId = "2",
                            DefaultDockGroupId = "DocumentGroup",
                            Header = "Tab 2",
                            Content = "Hello World",
                            ContentTemplateResourceKey = "ReloadingDataTemplate",
                        }
                    }
                ).ToObservableCollection();


        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
