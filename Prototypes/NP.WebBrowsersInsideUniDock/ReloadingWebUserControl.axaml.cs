using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Xilium.CefGlue.Avalonia;

namespace NP.WebBrowsersInsideUniDock
{
    public partial class ReloadingWebUserControl : UserControl, IDisposable
    {
        private static int _staticCount = 0;

        int _count;
        #region Count Styled Avalonia Property
        public int Count
        {
            get { return GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<ReloadingWebUserControl, int>
            (
                nameof(Count)
            );
        #endregion Count Styled Avalonia Property


        private AvaloniaCefBrowser browser = new();
        public ReloadingWebUserControl()
        {
            _staticCount++;

            _count = _staticCount;
            Count = _staticCount;

            InitializeComponent();
        }

        ~ReloadingWebUserControl()
        {

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var browserWrapper = this.FindControl<Decorator>("browserWrapper");
            browserWrapper.Child = browser;
            // NICK everytime you redock or move dock item ... you will see a new DILL roll ...i.e. control has been destroyed/created
            browser.Address = "https://www.google.com/search?q=Roll%20a%20die&stick=H4sIAAAAAAAAAOOwfcRoxC3w8sc9YSnNSWtOXmNU5uILyM-pzEhNKUrMcclMThUS5OLMK82NL85MSS0WYpFiEmDjAQAnwJaNNwAAAA";
        }

        public void Dispose()
        {
            
        }
    }
}
