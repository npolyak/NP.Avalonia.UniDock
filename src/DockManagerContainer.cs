using NP.Utilities;
using System.ComponentModel;

namespace NP.AvaloniaDock
{
    public class DockManagerContainer : VMBase, INotifyPropertyChanging
    {
        private DockManager? _dockManager;
        public DockManager? TheDockManager
        {
            get => _dockManager;
            internal set
            {
                if (ReferenceEquals(_dockManager, value))
                    return;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(TheDockManager)));

                _dockManager = value;

                OnPropertyChanged(nameof(TheDockManager));
            }
        }

        public event PropertyChangingEventHandler? PropertyChanging;
    }
}
