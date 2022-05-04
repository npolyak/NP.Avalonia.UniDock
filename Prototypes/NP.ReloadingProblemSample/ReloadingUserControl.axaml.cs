using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NP.ReloadingProblemSample
{
    public partial class ReloadingUserControl : UserControl
    {
        public ReloadingUserControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
