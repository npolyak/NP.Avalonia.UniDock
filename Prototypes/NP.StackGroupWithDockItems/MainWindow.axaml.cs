using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Controls;
using NP.AvaloniaDock;
using NP.Utilities;

namespace NP.GroupPanelSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void OnCurrentScreenPointChanged(Point2D screenPoint)
        {
            AttachedProperties.SetCurrentScreenPoint(this, screenPoint);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
