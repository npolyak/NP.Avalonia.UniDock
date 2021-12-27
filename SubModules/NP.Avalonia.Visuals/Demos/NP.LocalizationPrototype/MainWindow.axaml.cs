using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Layout;
using System;
using System.Linq;
using NP.Avalonia.Visuals.ThemingAndL10N;
using NP.Avalonia.Visuals.Behaviors;

namespace NP.LocalizationPrototype
{
    public partial class MainWindow : Window
    {
        private ThemeLoader? _languageThemeLoader;
        private ThemeLoader? _colorThemeLoader;

        private Data _data = new Data("FirstDataTemplateText");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _languageThemeLoader = 
                Application.Current.Resources.GetThemeLoader("LanguageLoader");

            _colorThemeLoader =
                Application.Current.Resources.GetThemeLoader("ColorLoader");

            this.GetObservable(SelectedLanguageProperty).Subscribe(OnSelectedLanguageChanged);
            this.GetObservable(SelectedColorThemeProperty).Subscribe(OnSelectedColorThemeChanged);

            ContentControl el1 = this.FindControl<ContentControl>("_elementI");
            ContentControl el2 = this.FindControl<ContentControl>("_elementII");

            el1.Content = _data;
            el2.Content = new Data("SecondDataTemplateText");

            Button errorButton = this.FindControl<Button>("ErrorButton");
            errorButton.Click += ButtonError_Click!;

            Button closeButton = this.FindControl<Button>("CloseButton");
            closeButton.Click += ButtonClose_Click!;

            //Button changeUidButton = this.FindControl<Button>("ChangeUidButton");
            //changeUidButton.Click += ChangeUidButton_Click;


        }

        private void OnSelectedLanguageChanged(Language language)
        {
            _languageThemeLoader!.SelectedThemeId = language.ToString();
        }

        private void OnSelectedColorThemeChanged(ColorTheme colorTheme)
        {
            _colorThemeLoader!.SelectedThemeId = colorTheme.ToString();
        }

        private void ChangeUidButton_Click(object? sender, RoutedEventArgs e)
        {
            _data.Uid = "SecondDataTemplateText";
        }

        #region SelectedLanguage Styled Avalonia Property
        public Language SelectedLanguage
        {
            get { return GetValue(SelectedLanguageProperty); }
            set { SetValue(SelectedLanguageProperty, value); }
        }

        public static readonly StyledProperty<Language> SelectedLanguageProperty =
            AvaloniaProperty.Register<MainWindow, Language>
            (
                nameof(SelectedLanguage),
                Language.English
            );
        #endregion SelectedLanguage Styled Avalonia Property


        #region SelectedColorTheme Styled Avalonia Property
        public ColorTheme SelectedColorTheme
        {
            get { return GetValue(SelectedColorThemeProperty); }
            set { SetValue(SelectedColorThemeProperty, value); }
        }

        public static readonly StyledProperty<ColorTheme> SelectedColorThemeProperty =
            AvaloniaProperty.Register<MainWindow, ColorTheme>
            (
                nameof(SelectedColorTheme)
            );
        #endregion SelectedColorTheme Styled Avalonia Property


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public int Uid
        {
            get { return 4; }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            BeginMoveDrag(e);
        }

        private void ButtonError_Click(object sender, RoutedEventArgs e)
        {
            string? message = _languageThemeLoader?.GetResource<string>("NotEnoughMemory");
            string? closeWindowStr = _languageThemeLoader?.GetResource<string>("CloseWindowMessage"); ;

            var window = CreateSampleWindow(message, closeWindowStr);

            window.ShowDialog(this);
        }

        private Window CreateSampleWindow(string? msg, string? closeWindowStr)
        {
            Button button;

            var window = new Window
            {
                Height = 100,
                Width = 300,
                Content = new StackPanel
                {
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock { Text = msg, HorizontalAlignment = HorizontalAlignment.Center},
                        (button = new Button
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Content = closeWindowStr
                        })
                    }
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            button.Click += (_, __) => window.Close();

            return window;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
