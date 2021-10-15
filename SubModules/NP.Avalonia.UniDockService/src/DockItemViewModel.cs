using NP.Utilities;

namespace NP.Avalonia.UniDockService
{
    public class DockItemViewModel : VMBase
    {
        #region IsDockVisible Property
        private bool _isDockVisible = true;
        public bool IsDockVisible
        {
            get
            {
                return this._isDockVisible;
            }
            set
            {
                if (this._isDockVisible == value)
                {
                    return;
                }

                this._isDockVisible = value;
                this.OnPropertyChanged(nameof(IsDockVisible));
            }
        }
        #endregion IsDockVisible Property

        public string? DockId { get; set; }

        public string? DefaultDockGroupId { get; set; }

        public int DefaultDockOrderInGroup { get; set; } = default;


        #region IsSelected Property
        private bool _isSelected = default;
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (this._isSelected == value)
                {
                    return;
                }

                this._isSelected = value;
                this.OnPropertyChanged(nameof(IsSelected));
            }
        }
        #endregion IsSelected Property

        #region IsActive Property
        private bool _isActive = default;
        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                if (this._isActive == value)
                {
                    return;
                }

                this._isActive = value;
                this.OnPropertyChanged(nameof(IsActive));
            }
        }
        #endregion IsActive Property

        public bool CanFloat { get; set; } = true;

        public bool CanClose { get; set; } = true;

        public bool IsPredefined { get; set; } = true;

        public bool IsConstructed { get; set; }

        #region HeaderContent Property
        private object? _headerContent;
        public object? HeaderContent
        {
            get
            {
                return this._headerContent;
            }
            set
            {
                if (this._headerContent == value)
                {
                    return;
                }

                this._headerContent = value;
                this.OnPropertyChanged(nameof(HeaderContent));
            }
        }
        #endregion HeaderContent Property

        #region HeaderContentTemplateResourceKey Property
        private string? _headerContentTemplateResourceKey;
        public string? HeaderContentTemplateResourceKey
        {
            get
            {
                return this._headerContentTemplateResourceKey;
            }
            set
            {
                if (this._headerContentTemplateResourceKey == value)
                {
                    return;
                }

                this._headerContentTemplateResourceKey = value;
                this.OnPropertyChanged(nameof(HeaderContentTemplateResourceKey));
            }
        }
        #endregion HeaderContentTemplateResourceKey Property


        #region Content Property
        private object? _content;
        public object? Content
        {
            get
            {
                return this._content;
            }
            set
            {
                if (this._content == value)
                {
                    return;
                }

                this._content = value;
                this.OnPropertyChanged(nameof(Content));
            }
        }
        #endregion Content Property

        #region ContentTemplateResourceKey Property
        private string? _contentTemplateResourceKey;
        public string? ContentTemplateResourceKey
        {
            get
            {
                return this._contentTemplateResourceKey;
            }
            set
            {
                if (this._contentTemplateResourceKey == value)
                {
                    return;
                }

                this._contentTemplateResourceKey = value;
                this.OnPropertyChanged(nameof(ContentTemplateResourceKey));
            }
        }
        #endregion ContentTemplateResourceKey Property

    }
}
