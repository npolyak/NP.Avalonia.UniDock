using Avalonia;
using Avalonia.Metadata;
using NP.Utilities;
using System.ComponentModel;

namespace NP.Avalonia.UniDock
{
    public class FloatingWindowContainer : IDockManagerContainer
    {
        private void TrySetWindow()
        {
            if ( (_dockManager == null) ||
                 (_windowSize == null) ||
                 (_windowPosition == null) ||
                 (_windowId == null) )
            {
                return;
            }

            _floatingWindow = _dockManager.FloatingWindowFactory.CreateFloatingWindow();
            _floatingWindow.Position = WindowPosition!.Value;
            _floatingWindow.Width = _windowSize.Value.X;
            _floatingWindow.Height = _windowSize.Value.Y;
            DockAttachedProperties.SetWindowId(_floatingWindow, WindowId);
            _floatingWindow.IsStable = true;
            _floatingWindow.TheDockManager = _dockManager;
            SetExtras();

            _floatingWindow.Show();
        }

        DockManager? _dockManager;
        public DockManager? TheDockManager 
        {
            get => _dockManager;
            set
            {
                if (_dockManager == value)
                    return;

                _dockManager = value;
                TrySetWindow();
            }
        }

        private FloatingWindow? _floatingWindow;

        private PixelPoint? _windowPosition;
        public PixelPoint? WindowPosition 
        { 
            get => _windowPosition;
            set
            {
                if (_windowPosition.ObjEquals(value))
                {
                    return;
                }

                _windowPosition = value;
                TrySetWindow();
            }
        }

        private PixelPoint? _windowSize;
        public PixelPoint? WindowSize 
        {
            get => _windowSize; 
            set
            {
                if (_windowSize.ObjEquals(value))
                {
                    return;
                }

                _windowSize = value;
                TrySetWindow();
            }
        }

        private string? _windowId;
        public string? WindowId
        {
            get => _windowId;
            set
            {
                if (_windowId == value)
                {
                    return;
                }

                _windowId = value;
                TrySetWindow();
            }
        }

        private IDockGroup? _dockContent;
        [Content]
        public IDockGroup? DockContent
        {
            get => _dockContent;

            set
            {
                if (_dockContent == value)
                    return;

                _dockContent = value;

                SetDockContent();
            }
        }

        private void SetDockContent()
        {
            if (_floatingWindow != null && _dockContent != null)
            {
                _floatingWindow.TheDockGroup.TheChild = _dockContent;
            }
        }


        #region Title Property
        private string? _title;
        public string? Title
        {
            get
            {
                return this._title;
            }
            set
            {
                if (this._title == value)
                {
                    return;
                }

                this._title = value;

                if (_floatingWindow != null)
                {
                    _floatingWindow.Title = Title;
                }
            }
        }
        #endregion Title Property


        private void SetExtras()
        {
            if (_floatingWindow == null)
                return;

            _floatingWindow.Title = Title;
            SetDockContent();
        }
    }
}
