using System;
using System.Collections.ObjectModel;

namespace NP.Avalonia.UniDockService
{
    public interface IUniDockService
    {
        ObservableCollection<DockItemViewModelBase> DockItemsViewModels { get; set; }

        void SaveToFile(string filePath);

        void RestoreFromFile(string filePath,
                             bool restorePredefinedWindowsPositionParams = false);

        void SaveViewModelsToFile(string filePath);

        void RestoreViewModelsFromFile(string filePath, params Type[] extraTypes);

        DockObjectInfo? GetParentGroupInfo(string? dockId);

        DockObjectInfo? GetGroupByDockId(string? dockId);
    }
}
