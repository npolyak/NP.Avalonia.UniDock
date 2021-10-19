namespace NP.Avalonia.UniDockService
{
    public class DockObjectInfo
    {
        public string DockId { get; }

        public GroupKind TheGroupKind { get; }

        public DockObjectInfo(string dockId, GroupKind groupKind)
        {
            DockId = dockId;
            TheGroupKind = groupKind;
        }
    }
}
