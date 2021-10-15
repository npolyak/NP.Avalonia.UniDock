using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDockService;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.ViewModelSample
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
            _dockManager = MyContainer.TheDockManager;

            Opened += MainWindow_Opened;

            Button saveButton = this.FindControl<Button>("SaveButton");
            saveButton.Click += SaveButton_Click;

            Button restoreButton = this.FindControl<Button>("RestoreButton");
            restoreButton.Click += RestoreButton_Click;

            ObservableCollection<DockItemViewModel> vms = 
                new ObservableCollection<DockItemViewModel>();

            DockItemViewModel vm1 = new DockItemViewModel
            {
                DockId = "Tab1",
                HeaderContent = "Tab1",
                DefaultDockGroupId = "Group1",
                DefaultDockOrderInGroup = 1,
                Content = "Hello World!"
            };

            vms.Add(vm1);


            DockItemViewModel vm2 = new DockItemViewModel
            {
                DockId = "Tab2",
                HeaderContent = "Tab2",
                DefaultDockGroupId = "Group1",
                DefaultDockOrderInGroup = 2,
                Content = "Hi World!",
                ContentTemplateResourceKey = "TheDataTemplate"
            };

            vms.Add(vm2);

            vm1.IsActive = true;

            _dockManager.DockItemsViewModels = vms;

            DockItemViewModel vm3 = new DockItemViewModel
            {
                DockId = "Tab3",
                HeaderContent = "Tab3",
                DefaultDockGroupId = "Group2",
                DefaultDockOrderInGroup = 0,
                Content = "3333"
            };

            vms.Add(vm3);

            DockItemViewModel vm4 = new DockItemViewModel
            {
                DockId = "Tab4",
                HeaderContent = "Tab4",
                DefaultDockGroupId = "Group2",
                DefaultDockOrderInGroup = 1,
                Content = "4444"
            };

            vms.Add(vm4);

            DockItemViewModel floatingVm1 = new DockItemViewModel
            {
                DockId = "FloatingDockItem1",
                HeaderContent = "FloatingWindowPanel1",
                DefaultDockGroupId = "FloatingGroup1",
                DefaultDockOrderInGroup = 1,
                Content = "Floating Panel"
            };

            vms.Add(floatingVm1);

            DockItemViewModel floatingVm2 = new DockItemViewModel
            {
                DockId = "FloatingDockItem2",
                HeaderContent = "Floating Tab 1",
                DefaultDockGroupId = "FloatingGroup2",
                DefaultDockOrderInGroup = 1,
                Content = "Floating Tab 1"
            };

            vms.Add(floatingVm2);

            DockItemViewModel floatingVm3 = new DockItemViewModel
            {
                DockId = "FloatingDockItem3",
                HeaderContent = "Floating Tab 2",
                DefaultDockGroupId = "FloatingGroup2",
                DefaultDockOrderInGroup = 2,
                Content = "Floating Tab 2"
            };

            vms.Add(floatingVm3);

            // object? result = this.FindResource("DefaultWindowTitleAreaDataTemplate");
        }


        private const string SerializationFile = "Serialization.xml";
        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            _dockManager.SaveToFile(SerializationFile);
        }

        private void RestoreButton_Click(object? sender, RoutedEventArgs e)
        {
            _dockManager.RestoreFromFile(SerializationFile);
        }

        private void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            var w = (FloatingWindow)(App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).Windows[1];

            var visualDescendants = 
                w.GetVisualDescendants().ToList();
        }

        private void MainWindow_Initialized(object? sender, System.EventArgs e)
        {
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
