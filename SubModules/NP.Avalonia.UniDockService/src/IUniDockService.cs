using System.Collections.Generic;

namespace NP.Avalonia.UniDockService
{
    public interface IUniDockService
    {
        IEnumerable<DockItemViewModel> DockItemsViewModels { get; set; }

        void SaveToFile(string filePath);

        void RestoreFromFile(string filePath);

        public (string? dockId, GroupKind? groupKind) GetContainingGroupInfo(string dockId);
    }
}
