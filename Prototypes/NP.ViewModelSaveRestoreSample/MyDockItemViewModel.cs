using NP.Avalonia.UniDockService;
using System.Xml.Serialization;

namespace NP.ViewModelSaveRestoreSample
{
    public class MyDockItemViewModel : DockItemViewModel
    {
        [XmlIgnore]
        public override object? Header
        { 
            get => base.Header; 
            set => base.Header = value; 
        }
    }
}
