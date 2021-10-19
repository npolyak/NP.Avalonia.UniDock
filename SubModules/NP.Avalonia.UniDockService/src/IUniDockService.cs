using System.Collections.Generic;

namespace NP.Avalonia.UniDockService
{
    public interface IUniDockService
    {
        IEnumerable<IDockItemViewModel> DockItemsViewModels { get; set; }

        void SaveToFile(string filePath);

        void RestoreFromFile(string filePath);

        DockObjectInfo? GetParentGroupInfo(string? dockId);

        DockObjectInfo? GetGroupByDockId(string? dockId);
    }
}
