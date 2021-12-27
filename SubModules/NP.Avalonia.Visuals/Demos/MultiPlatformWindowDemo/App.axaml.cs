using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NP.Utilities;

namespace NP.Demos.MultiPlatformWindowDemo
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new StartupTestWindow()
                {
                    Classes = new Classes("PlainCustomWindow")
                };
            }

            NP.NLogAdapter.NLog.SetLog();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
