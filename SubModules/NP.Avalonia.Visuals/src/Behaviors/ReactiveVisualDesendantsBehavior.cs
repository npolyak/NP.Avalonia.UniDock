using Avalonia.VisualTree;
using NP.Concepts.Behaviors;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class ReactiveVisualDesendantsBehavior : FlattenReactiveTreeBehavior<IVisual>
    {
        public ReactiveVisualDesendantsBehavior(IVisual root) 
            : 
            base(root, visual => visual.VisualChildren)
        {
        }
    }
}
