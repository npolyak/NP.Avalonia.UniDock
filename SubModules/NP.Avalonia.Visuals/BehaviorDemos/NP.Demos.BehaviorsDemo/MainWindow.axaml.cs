using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.ThemingAndL10N;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System.Linq;

namespace NP.ThemingPrototype
{
    public partial class MainWindow : CustomWindow
    {
        ThemeLoader _lightDarkThemeLoader;
        ThemeLoader _accentThemeLoader;

        ReactiveVisualDesendantsBehavior _flattenVisualTreeBehavior;


        #region ThePoint Styled Avalonia Property
        public Point2D ThePoint
        {
            get { return GetValue(ThePointProperty); }
            set { SetValue(ThePointProperty, value); }
        }

        public static readonly StyledProperty<Point2D> ThePointProperty =
            AvaloniaProperty.Register<MainWindow, Point2D>
            (
                nameof(ThePoint)
            );
        #endregion ThePoint Styled Avalonia Property


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _lightDarkThemeLoader = 
                Application.Current.Resources.GetThemeLoader("LightDarkThemeLoader")!;

            _accentThemeLoader =
                Application.Current.Resources.GetThemeLoader("AccentThemeLoader")!;

            Button button = this.FindControl<Button>("ChangeThemeButton");

            _flattenVisualTreeBehavior = 
                new ReactiveVisualDesendantsBehavior(button);
            button.Click += Button_Click;
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            //Popup p = comboBox.TheNameScope.Get<Popup>("PART_Popup");

            _flattenVisualTreeBehavior.DetachCollections();

            _flattenVisualTreeBehavior.AttachCollections();

            _lightDarkThemeLoader.SwitchTheme();

            if (_lightDarkThemeLoader.SelectedThemeId == "Light")
            {
                _accentThemeLoader.SelectedThemeId = "DarkBlue";
            }
            else
            {
                _accentThemeLoader.SelectedThemeId = "LightBlue";
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
