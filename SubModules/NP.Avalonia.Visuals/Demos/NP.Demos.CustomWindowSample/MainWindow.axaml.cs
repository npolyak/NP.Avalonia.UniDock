using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Controls;

namespace NP.Demos.CustomWindowSample
{
    public partial class MainWindow : CustomWindow
    {

        #region WindowPosition Styled Avalonia Property
        public PixelPoint WindowPosition
        {
            get { return GetValue(WindowPositionProperty); }
            set { SetValue(WindowPositionProperty, value); }
        }

        public static readonly StyledProperty<PixelPoint> WindowPositionProperty =
            AvaloniaProperty.Register<MainWindow, PixelPoint>
            (
                nameof(WindowPosition)
            );
        #endregion WindowPosition Styled Avalonia Property


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.PositionChanged += MainWindow_PositionChanged;
        }

        private void MainWindow_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            WindowPosition = Position;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
