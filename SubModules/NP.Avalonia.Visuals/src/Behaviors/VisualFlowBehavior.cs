using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class VisualFlowBehavior
    {

        #region ReadVisualFlow Attached Avalonia Property
        public static VisualFlow GetReadVisualFlow(IControl obj)
        {
            return obj.GetValue(ReadVisualFlowProperty);
        }

        internal static void SetReadVisualFlow(IControl obj, VisualFlow value)
        {
            obj.SetValue(ReadVisualFlowProperty, value);
        }

        public static readonly AttachedProperty<VisualFlow> ReadVisualFlowProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, VisualFlow>
            (
                "ReadVisualFlow",
                VisualFlow.Normal, 
                true
            );
        #endregion ReadVisualFlow Attached Avalonia Property


        #region TheVisualFlow Attached Avalonia Property
        public static VisualFlow GetTheVisualFlow(IControl obj)
        {
            return obj.GetValue(TheVisualFlowProperty);
        }

        public static void SetTheVisualFlow(IControl obj, VisualFlow value)
        {
            obj.SetValue(TheVisualFlowProperty, value);
        }

        public static readonly AttachedProperty<VisualFlow> TheVisualFlowProperty =
            AvaloniaProperty.RegisterAttached<object, IControl, VisualFlow>
            (
                "TheVisualFlow",
                VisualFlow.Normal
            );
        #endregion TheVisualFlow Attached Avalonia Property

        static VisualFlowBehavior()
        {
            TheVisualFlowProperty.Changed.Subscribe(OnVisualFlowChanged);
        }

        private static void CheckTransform(this IControl control, bool isNormalFlow)
        {
            if (!isNormalFlow && control.RenderTransform != null && !control.RenderTransform.Value.IsIdentity)
            {
                throw new Exception("Error - VisualFlowBehavior might be ruining the current render transform");
            }
        }

        private static void SetTransform(this IControl control, bool isNormalFlow)
        {
            control.CheckTransform(isNormalFlow);

            RelativePoint renderTransformOrigin =
                new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            if (isNormalFlow)
            {
                control.ClearValue(Visual.RenderTransformOriginProperty);
                control.ClearValue(Visual.RenderTransformProperty);
            }
            else
            {
                ScaleTransform flowTransform = isNormalFlow ? null : new ScaleTransform(-1, 1);

                control.RenderTransformOrigin = renderTransformOrigin;

                control.RenderTransform = flowTransform;
            }
        }

        private static void OnVisualFlowChanged(AvaloniaPropertyChangedEventArgs<VisualFlow> args)
        {
            IControl control = (IControl) args.Sender;

            VisualFlow visualFlow = args.NewValue.Value;
            bool isNormalFlow = (visualFlow == VisualFlow.Normal);

            control.SetTransform(isNormalFlow);

            SetReadVisualFlow(control, visualFlow);
        }
    }
}
