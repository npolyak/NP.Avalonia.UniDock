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
using System.Xml.Serialization;

namespace NP.ViewModelSaveRestoreSample
{
    public partial class MainWindow : Window
    {
        private DockManager _dockManager;

        DataItemsViewModelBehavior<DockItemViewModel> TheDataItemsViewModelBehavior { get; }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _dockManager = MyContainer.TheDockManager;

            Button saveButton = this.FindControl<Button>("SaveButton");
            saveButton.Click += SaveButton_Click;

            Button restoreButton = this.FindControl<Button>("RestoreButton");
            restoreButton.Click += RestoreButton_Click;

            ObservableCollection<DockItemViewModel> vms = 
                new ObservableCollection<DockItemViewModel>();

            TheDataItemsViewModelBehavior =
                new DataItemsViewModelBehavior<DockItemViewModel>(_dockManager);

            TheDataItemsViewModelBehavior.DockItemsViewModels = new ObservableCollection<DockItemViewModel>();

            Button mainTabsButton = this.FindControl<Button>("AddMainTabButton");

            mainTabsButton.Click += MainTabsButton_Click;


            Button floatingTabsButton = this.FindControl<Button>("AddFloatingTabButton");

            floatingTabsButton.Click += FloatingTabsButton_Click;
        }

        int _tabNumber = 5;
        private void MainTabsButton_Click(object? sender, RoutedEventArgs e)
        {
            string tabStr = $"Tab{_tabNumber}";

            var newTabVm = new DockItemViewModel
            {
                DockId = tabStr,
                Header = tabStr,
                DefaultDockGroupId = "Group2",
                DefaultDockOrderInGroup = _tabNumber,
                Content = tabStr,
                IsPredefined = false
            };

            TheDataItemsViewModelBehavior.DockItemsViewModels!.Add(newTabVm);

            newTabVm.IsSelected = true;

            _tabNumber++;
        }

        int _floatingTabNumber = 3;
        private void FloatingTabsButton_Click(object? sender, RoutedEventArgs e)
        {
            string floatingTabStr = $"FloatingTab{_floatingTabNumber}";

            var newTabVm = new DockItemViewModel
            {
                DockId = floatingTabStr,
                Header = floatingTabStr,
                DefaultDockGroupId = "FloatingGroup2",
                DefaultDockOrderInGroup = _floatingTabNumber,
                Content = floatingTabStr,
                IsPredefined = false
            };

            TheDataItemsViewModelBehavior.DockItemsViewModels!.Add(newTabVm);

            _floatingTabNumber++;
        }


        private const string SerializationFile = "Serialization.xml";
        private const string VMSerializationFile = "VMSerialization.xml";
        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            _dockManager.SaveToFile(SerializationFile);

            (TheDataItemsViewModelBehavior.DockItemsViewModels! as ObservableCollection<DockItemViewModel>)
                .SerializeToFile(VMSerializationFile, typeof(MyDockItemViewModel), typeof(TabViewModel));

        }

        private void RestoreButton_Click(object? sender, RoutedEventArgs e)
        {
            _dockManager.RestoreFromFile(SerializationFile);

            var restoredVms = XmlSerializationUtils.DeserializeFromFile<ObservableCollection<DockItemViewModel>>(VMSerializationFile);

            TheDataItemsViewModelBehavior.DockItemsViewModels = restoredVms;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
