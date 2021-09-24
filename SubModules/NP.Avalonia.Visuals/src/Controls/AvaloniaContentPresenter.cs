using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using NP.Avalonia.Visuals.Converters;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public class AvaloniaContentPresenter : ContentPresenter
    {
        readonly IDisposable? _bindingSubscription;
        public AvaloniaContentPresenter()
        {
            _bindingSubscription = 
                this.Bind(ContentProperty, new Binding { Source = this, Path = "RealContent", Converter = ToControlContainerConverter.Instance });
        }


        #region RealContent Styled Avalonia Property
        public object RealContent
        {
            get { return GetValue(RealContentProperty); }
            set { SetValue(RealContentProperty, value); }
        }

        public static readonly StyledProperty<object> RealContentProperty =
            AvaloniaProperty.Register<AvaloniaContentPresenter, object>
            (
                nameof(RealContent)
            );
        #endregion RealContent Styled Avalonia Property

    }
}
