using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.GroupPanelSample;
using NP.Utilities;
using System;

namespace NP.GroupPanelSample
{
    public partial class MainWindow : Window
    {
        StackGroup _stackGroup;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _stackGroup = this.FindControl<StackGroup>("TheStackGroup");

            //_stackGroup.PanelChildren.Add(new Grid { Background = new SolidColorBrush(Colors.Red) });
            //_stackGroup.PanelChildren.Add(new Grid { Background = new SolidColorBrush(Colors.Green) });

            //_stackGroup.PanelChildren.Insert(0, new Grid { Background = new SolidColorBrush(Colors.Yellow) });

            //_stackGroup.PanelChildren.Insert(2, new Grid { Background = new SolidColorBrush(Colors.Blue) });

            //_stackGroup.PanelChildren.Insert(3, new Grid { Background = new SolidColorBrush(Colors.Purple) });
        }

        private void OnCurrentScreenPointChanged(Point2D screenPoint)
        {
            AttachedProperties.SetCurrentScreenPoint(this, screenPoint);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void RemoveFirst()
        {
            _stackGroup.PanelChildren.RemoveAt(0);
        }

        public void RemoveLast()
        {
            _stackGroup.PanelChildren.RemoveAt(_stackGroup.PanelChildren.Count() - 1);
        }

        public void RemoveSecond()
        {
            _stackGroup.PanelChildren.RemoveAt(1);
        }
    }
}
