using NP.Utilities;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NP.Avalonia.UniDockService
{
    public interface IDockItemViewModel
    {
        bool IsDockVisible { get; set; }

        string? DockId { get; }

        string? DefaultDockGroupId { get; }

        string? GroupOnlyById { get; }

        double DefaultDockOrderInGroup { get; }

        bool IsSelected { get; set; }

        bool IsActive { get; set; }

        bool CanFloat { get; }

        bool CanClose { get; }

        bool IsPredefined { get; }

        object? Header { get; }

        string? HeaderContentTemplateResourceKey { get; }

        object Content { get; }

        string? ContentTemplateResourceKey { get; }
    }

    public class DockItemViewModelBase : VMBase, IDockItemViewModel
    {
        #region IsDockVisible Property
        private bool _isDockVisible = true;
        [XmlAttribute]
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

        [XmlAttribute]
        public string? DockId { get; set; }
        
        [XmlAttribute]
        public string? DefaultDockGroupId { get; set; }

        [XmlAttribute]
        public string? GroupOnlyById { get; set; }

        [XmlAttribute]
        public double DefaultDockOrderInGroup { get; set; } = default;

        #region IsSelected Property
        private bool _isSelected = default;
        [XmlIgnore]
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
        [XmlIgnore]
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

        [XmlAttribute]
        public bool CanFloat { get; set; } = true;

        [XmlAttribute]
        public bool CanClose { get; set; } = true;

        [XmlAttribute]
        public bool IsPredefined { get; set; } = false;

        #region HeaderContent Property
        [XmlIgnore]
        public virtual object? Header
        {
            get;
            set;
        }
        #endregion HeaderContent Property

        #region HeaderContentTemplateResourceKey Property
        [XmlAttribute]
        public string? HeaderContentTemplateResourceKey
        {
            get;
            set;
        }
        #endregion HeaderContentTemplateResourceKey Property


        #region Content Property
        [XmlIgnore]
        public virtual object? Content
        {
            get;
            set;
        }
        #endregion Content Property

        #region ContentTemplateResourceKey Property
        [XmlAttribute]
        public string? ContentTemplateResourceKey
        {
            get;
            set;
        }
        #endregion ContentTemplateResourceKey Property

        public void MakeActive()
        {
            IsActive = true;
        }

        public void Select()
        {
            IsSelected = true;
        }
    }

    public class DockItemViewModel<TViewModel> : DockItemViewModelBase
        where TViewModel : class
    {
        #region TheVM Property
        private TViewModel? _vm;
        [XmlElement]
        public TViewModel? TheVM
        {
            get
            {
                return this._vm;
            }
            set
            {
                if (this._vm == value)
                {
                    return;
                }

                this._vm = value;
                this.OnPropertyChanged(nameof(TheVM));
            }
        }
        #endregion TheVM Property

        [XmlIgnore]
        public override object? Header
        {
            get => TheVM;
            set
            {

            }
        }

        [XmlIgnore]
        public override object? Content
        {
            get => TheVM;
            set
            {

            }
        }
    }
}
