using Avalonia.Layout;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NP.AvaloniaDock.Serialization
{
    public class DockGroupParams
    {
        [XmlAttribute]
        public string? GroupFullTypeName { get; set; }

        [XmlAttribute]
        public string? DockId { get; set; }

        [XmlAttribute]
        public string? ParentDockId { get; set; }

        [XmlAttribute]
        public List<string>? ChildrenDockIds { get; set; }

        [XmlAttribute]
        public bool AutoDestroy { get; set; }

        #region Width and Height Coefficients
        /// <summary>
        /// these coefficients are only applicable to children of DockStackGroups and
        /// they signify the proportion of the horizontal or vertical space that the
        /// child gets.
        /// </summary>
        [XmlAttribute]
        public double SizeCoefficient { get; set; }

        #endregion Width or Height Coefficients

        // only applicable to DockStackGroup, otherwise - null
        [XmlAttribute]
        public Orientation? TheOrientation { get; set; }

        #region DockItem parameters
        [XmlElement]
        public string? HeaderRestorationInfo { get; set; }

        [XmlElement]
        public string? ContentRestorationInfo { get; set; }
        #endregion DockItem Parameters
    }
}
