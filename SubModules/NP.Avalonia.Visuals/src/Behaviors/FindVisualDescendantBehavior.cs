using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using NP.Concepts.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class FindVisualDescendantBehavior 
    {
        IControl? _foundDescendant;
        public IControl? FoundDescendant 
        {
            get => _foundDescendant;
            private set
            {
                if (_foundDescendant == value)
                    return;

                _foundDescendant = value;

                SetResult(AttachedToControl!, _foundDescendant);
            }
        }

        public string? DescendantName { get; set; }


        private IDisposable? _visualDescendantsBehavior;
        private ReactiveVisualDesendantsBehavior? _reactiveVisualDescendants;

        IControl? _attachedToControl;
        private IControl? AttachedToControl 
        { 
            get => _attachedToControl; 
            set
            {
                if (_attachedToControl == value)
                {
                    return;
                }

                StopBehavior();

                _attachedToControl = value;

                if (_attachedToControl != null)
                {
                    _reactiveVisualDescendants = 
                        new ReactiveVisualDesendantsBehavior(_attachedToControl);

                    _visualDescendantsBehavior = 
                        _reactiveVisualDescendants.Result.AddBehavior(OnVisualDescendantAdded);
                }
            }
        }

        private void StopBehavior()
        {
            if (_visualDescendantsBehavior != null)
            {
                _visualDescendantsBehavior.Dispose();
                _visualDescendantsBehavior = null;
            }

            if (_reactiveVisualDescendants != null)
            {
                _reactiveVisualDescendants.DetachCollections();
                _reactiveVisualDescendants = null;
            }
        }

        private void OnVisualDescendantAdded(IVisual childVisual)
        {
            IControl childControl = (IControl)childVisual;

            if (childControl.Name == DescendantName)
            {
                this.FoundDescendant = childControl;

                StopBehavior();
            }
        }

        #region Result Attached Avalonia Property
        public static IControl? GetResult(IControl obj)
        {
            return obj.GetValue(ResultProperty);
        }

        public static void SetResult(IControl obj, IControl? value)
        {
            obj.SetValue(ResultProperty, value);
        }

        public static readonly AttachedProperty<IControl?> ResultProperty =
            AvaloniaProperty.RegisterAttached<FindVisualDescendantBehavior, IControl, IControl?>
            (
                "Result"
            );
        #endregion Result Attached Avalonia Property


        #region BehaviorInstance Attached Avalonia Property
        public static FindVisualDescendantBehavior GetBehaviorInstance(IControl obj)
        {
            return obj.GetValue(BehaviorInstanceProperty);
        }

        public static void SetBehaviorInstance(IControl obj, FindVisualDescendantBehavior value)
        {
            obj.SetValue(BehaviorInstanceProperty, value);
        }

        public static readonly AttachedProperty<FindVisualDescendantBehavior> BehaviorInstanceProperty =
            AvaloniaProperty.RegisterAttached<FindVisualDescendantBehavior, IControl, FindVisualDescendantBehavior>
            (
                "BehaviorInstance"
            );
        #endregion BehaviorInstance Attached Avalonia Property

        static FindVisualDescendantBehavior()
        {
            BehaviorInstanceProperty.Changed.Subscribe(OnBehaviorChanged);
        }

        private static void OnBehaviorChanged(AvaloniaPropertyChangedEventArgs<FindVisualDescendantBehavior> obj)
        {
            FindVisualDescendantBehavior? oldBehavior = obj.OldValue.Value;

            if (oldBehavior != null)
            {
                oldBehavior.AttachedToControl = null;
            }

            FindVisualDescendantBehavior? newBehavior = obj.NewValue.Value;

            if (newBehavior != null)
            {
                newBehavior.AttachedToControl = (IControl)obj.Sender; 
            }
        }
    }
}
