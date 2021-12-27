using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.ThemingAndL10N;

namespace NP.AutoGridTest
{
    public partial class MainWindow : Window
    {
        private ThemeLoader _themeLoader;

        AutoGrid _autoGrid;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _autoGrid = this.FindControl<AutoGrid>("MyAutoGrid");

            _themeLoader = 
                Application.Current.Resources.GetThemeLoader("ColorThemeLoader")!;

            Button changeLocationButton = this.FindControl<Button>("ChangeLocationButton");

            changeLocationButton.Click += ChangeLocationButton_Click;

            Button changeThemeButton = this.FindControl<Button>("ChangeThemeButton");

            changeThemeButton.Click += ChangeThemeButton_Click;
        }

        private void ChangeLocationButton_Click(object? sender, RoutedEventArgs e)
        {
            Button button3 = this.FindControl<Button>("Button3");

            if (AutoGrid.GetRow(button3) == 2)
            {
                AutoGrid.SetRow(button3, -2);
                AutoGrid.SetColumn(button3, -2);
            }
            else
            {
                AutoGrid.SetRow(button3, 2);
                AutoGrid.SetColumn(button3, 1);
            }
        }

        private void ChangeThemeButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_themeLoader.SelectedThemeId == "Dark")
            {
                _themeLoader.SelectedThemeId = "Light";
            }
            else
            {
                _themeLoader.SelectedThemeId = "Dark";
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
