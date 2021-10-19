using System.Xml.Serialization;

namespace NP.ViewModelSaveRestoreSample
{
    public class TabViewModel
    {
        [XmlAttribute]
        public string? Header { get; set; }

        [XmlAttribute]
        public string? Content { get; set; }
    }
}
