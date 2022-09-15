using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using NP.Avalonia.UniDock;
using NP.Avalonia.UniDockService;
using NP.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using NP.Concepts.Behaviors;

namespace NP.ViewModelSaveRestoreSample
{
    public partial class MainWindow : Window
    {
        private DockManager _dockManager;
        public IUniDockService _uniDockService;

        IDisposable _behavior;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _dockManager = (DockManager) this.FindResource("TheDockManager")!;

            _uniDockService = (IUniDockService)_dockManager!;

            _uniDockService.DockItemsViewModels =
                new ObservableCollection<DockItemViewModelBase>();

            Button addTabButton = this.FindControl<Button>("AddTabButton");

            addTabButton.Click += AddTabButton_Click;

            Button saveButton = this.FindControl<Button>("SaveButton");
            saveButton.Click += SaveButton_Click;

            Button restoreButton = this.FindControl<Button>("RestoreButton");
            restoreButton.Click += RestoreButton_Click;

            _behavior = _uniDockService.DockItemsViewModels.AddBehavior(OnItemAdded, OnItemRemoved);

            _uniDockService.DockItemRemovedEvent += _uniDockService_DockItemRemovedEvent;
        }

        private void _uniDockService_DockItemRemovedEvent(DockItemViewModelBase obj)
        {

        }

        void OnItemAdded(DockItemViewModelBase item)
        {
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = (DockItemViewModelBase)sender;
            if (e.PropertyName == nameof(DockItemViewModelBase.IsSelected))
            {
                string str = $"{item.DockId}:{item.IsSelected}";
                Console.WriteLine(str);
            }
        }

        void OnItemRemoved(DockItemViewModelBase item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        private int _tabNumber = 1;
        private void AddTabButton_Click(object? sender, RoutedEventArgs e)
        {
            string tabStr = $"Tab_{_tabNumber}";
            _uniDockService.DockItemsViewModels.Add
            (
                new DockItemViewModelBase
                {
                    DockId = tabStr,
                    Header = tabStr,
                    Content = $"This is tab {_tabNumber}",
                    DefaultDockGroupId = "Tabs",
                    DefaultDockOrderInGroup = _tabNumber,
                    IsSelected = true,
                    IsActive = true
                });

            _tabNumber++;
        }


        private const string DockSerializationFileName = "DockSerialization.xml";
        private const string VMSerializationFileName = "DockVMSerialization.xml";

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            // save the layout
            _uniDockService.SaveToFile(DockSerializationFileName);

            // save the view models
            _uniDockService.SaveViewModelsToFile(VMSerializationFileName);
        }

        private void RestoreButton_Click(object? sender, RoutedEventArgs e)
        {
            _behavior?.Dispose();
            _behavior = null;
            // clear the view models
            _uniDockService.DockItemsViewModels = null;

            // restore the layout
            _uniDockService.RestoreFromFile(DockSerializationFileName);

            // restore the view models
            _uniDockService.RestoreViewModelsFromFile(VMSerializationFileName);
            _behavior = _uniDockService.DockItemsViewModels.AddBehavior(OnItemAdded, OnItemRemoved);
            // select the first tab.
            _uniDockService.DockItemsViewModels?.FirstOrDefault()?.Select();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
