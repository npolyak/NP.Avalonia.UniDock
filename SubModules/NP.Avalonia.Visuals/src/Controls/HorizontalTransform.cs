using Avalonia;
using Avalonia.Media;
using NP.Avalonia.Visuals.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public class HorizontalTransform : ScaleTransform
    {
        #region TheVisualFlow Styled Avalonia Property
        public VisualFlow TheVisualFlow
        {
            get { return GetValue(TheVisualFlowProperty); }
            set { SetValue(TheVisualFlowProperty, value); }
        }

        public static readonly StyledProperty<VisualFlow> TheVisualFlowProperty =
            AvaloniaProperty.Register<HorizontalTransform, VisualFlow>
            (
                nameof(TheVisualFlow)
            );
        #endregion TheVisualFlow Styled Avalonia Property

        public HorizontalTransform()
        {
            this.GetObservable(TheVisualFlowProperty).Subscribe(OnVisualFlowChanged);
        }

        private void OnVisualFlowChanged(VisualFlow visualFlow)
        {
            this.ScaleX = visualFlow.ToScale();
        }
    }
}
