using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NP.ReloadingProblemSample
{
    public partial class ReloadingUserControl : UserControl
    {
        private static int _staticCount = 0;


        #region Count Styled Avalonia Property
        public int Count
        {
            get { return GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<ReloadingUserControl, int>
            (
                nameof(Count)
            );
        #endregion Count Styled Avalonia Property


        public ReloadingUserControl()
        {
            _staticCount++;

            Count = _staticCount;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
