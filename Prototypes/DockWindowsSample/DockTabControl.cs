using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace DockWindowsSample
{
    public class DockTabControl : ItemsControl
    {
        #region SelectedItem Styled Avalonia Property
        public object? SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<DockTabControl, object?>
            (
                nameof(SelectedItem)
            );
        #endregion SelectedItem Styled Avalonia Property


        #region TabStripPlacement Styled Avalonia Property
        public Dock TabStripPlacement
        {
            get { return GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        public static readonly StyledProperty<Dock> TabStripPlacementProperty =
            AvaloniaProperty.Register<DockTabControl, Dock>
            (
                nameof(TabStripPlacement),
                Dock.Top
            );
        #endregion TabStripPlacement Styled Avalonia Property


        #region HorizontalContentAlignment Styled Avalonia Property
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            AvaloniaProperty.Register<DockTabControl, HorizontalAlignment>
            (
                nameof(HorizontalContentAlignment)
            );
        #endregion HorizontalContentAlignment Styled Avalonia Property


        #region VerticalContentAlignment Styled Avalonia Property
        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            AvaloniaProperty.Register<DockTabControl, VerticalAlignment>
            (
                nameof(VerticalContentAlignment)
            );
        #endregion VerticalContentAlignment Styled Avalonia Property


        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new DockTabItemContainerGenerator(this);
        }

        private static readonly FuncTemplate<IPanel> DefaultPanel =
            new FuncTemplate<IPanel>(() => new WrapPanel() { Orientation = Orientation.Horizontal });

        static DockTabControl()
        {
            ItemsPanelProperty.OverrideDefaultValue<DockTabControl>(DefaultPanel);
            AffectsMeasure<DockTabControl>(TabStripPlacementProperty);
        }
    }
}
