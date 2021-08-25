using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.AvaloniaDock;
using NP.AvaloniaDock.Serialization;
using NP.Utilities;
using System;

namespace DockWindowsSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            CurrentScreenPointBehavior.CurrentScreenPoint.Subscribe(OnCurrentScreenPointChanged);

            //CurrentScreenPointBehavior.Capture(this);
        }

        private void OnCurrentScreenPointChanged(Point2D screenPoint)
        {
            AttachedProperties.SetCurrentScreenPoint(this, screenPoint);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SetButtonBounds()
        {
            Button button = this.FindControl<Button>("TheButton");

            ButtonBounds = button.GetScreenBounds();
        }


        #region ButtonBounds Styled Avalonia Property
        public Rect2D ButtonBounds
        {
            get { return GetValue(ButtonBoundsProperty); }
            private set { SetValue(ButtonBoundsProperty, value); }
        }

        public static readonly StyledProperty<Rect2D> ButtonBoundsProperty =
            AvaloniaProperty.Register<MainWindow, Rect2D>
            (
                nameof(ButtonBounds)
            );
        #endregion ButtonBounds Styled Avalonia Property

        public void PrepareForSerialization()
        {
            DockManager dockManager = DockAttachedProperties.GetTheDockManager(this);

            var dockManagerParams = dockManager.ToParams();

            string serialization = 
                XmlSerializationUtils.Serialize(dockManagerParams);
        }
    }
}
