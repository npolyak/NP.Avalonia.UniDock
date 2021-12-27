using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using NP.Avalonia.Visuals.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public class NpComboBox : ComboBox
    {        /// <summary>
             /// The default value for the <see cref="ItemsControl.ItemsPanel"/> property.
             /// </summary>
        private static readonly FuncTemplate<IPanel> DefaultPanel =
            new FuncTemplate<IPanel>(() => new VirtualizingStackPanel());

        FindVisualDescendantBehavior _findVisualDescendantBehavior = 
            new FindVisualDescendantBehavior { DescendantName = "PART_Popup" };


        public NpComboBox()
        {
            FindVisualDescendantBehavior.SetBehaviorInstance(this, _findVisualDescendantBehavior);


            this.GetObservable(FindVisualDescendantBehavior.ResultProperty).Subscribe(OnPopupChanged!);
        }

        private void OnPopupChanged(IControl obj)
        {
            if (obj == null)
            {
                return;
            }

            Popup popup = (Popup)obj;

            NameScope nameScope = new NameScope();
            nameScope.Register("PART_Popup", popup);

            base.OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
        }
    }
}
