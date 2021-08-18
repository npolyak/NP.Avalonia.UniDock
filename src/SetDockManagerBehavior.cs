using NP.Concepts.Behaviors;

namespace NP.AvaloniaDock
{
    public class SetDockManagerBehavior : SetItemsBehavior<DockManagerContainer, IDockGroup, DockManager>
    {
        public SetDockManagerBehavior(IDockGroupDockManagerContainer dockGroupDockManagerContainer) : base
        (
            container => container?.TheDockManager!,
            (item, dockManager) => item.TheDockManager = dockManager
        )
        {
            this.GetterObj = dockGroupDockManagerContainer.TheDockManagerContainer;
            this.ChildItems = dockGroupDockManagerContainer.DockChildren;
        }
    }
}
