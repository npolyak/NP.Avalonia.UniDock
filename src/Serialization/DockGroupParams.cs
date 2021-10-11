// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using Avalonia.Controls;
using Avalonia.Layout;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NP.Avalonia.UniDock.Serialization
{
    public class DockGroupParams
    {
        [XmlAttribute]
        public bool CanFloat { get; set; }

        [XmlAttribute]
        public bool CanRemove { get; set; }

        [XmlAttribute]
        public string? GroupFullTypeName { get; set; }

        public Type GetGroupType()
        {
            Type groupType = ReflectionUtils.RestoreType(GroupFullTypeName);

            return groupType;
        }

        public bool IsDockItem()
        {
            return typeof(DockItem).IsAssignableFrom(GetGroupType());
        }

        [XmlAttribute]
        public string? DockId { get; set; }

        [XmlAttribute]
        public string? ParentDockId { get; set; }

        [XmlAttribute]
        public List<string>? ChildrenDockIds { get; set; }

        [XmlAttribute]
        public bool AutoDestroy { get; set; }

        #region Width or Height Coefficients
        /// <summary>
        /// these coefficients are only applicable to children of DockStackGroups and
        /// they signify the proportion of the horizontal or vertical space that the
        /// child gets.
        /// </summary>
        [XmlArray]
        public double[]? SizeCoefficients { get; set; }

        #endregion Width or Height Coefficients

        // only applicable to DockStackGroup, otherwise - null
        [XmlAttribute]
        public Orientation TheOrientation { get; set; }

        [XmlAttribute]
        public bool IsPredefined { get; set; }

        #region DockItem parameters
        [XmlElement]
        public string? HeaderRestorationInfo { get; set; }

        [XmlElement]
        public string? ContentRestorationInfo { get; set; }
        #endregion DockItem Parameters
    }

    public static class GroupParamsHelper
    {
        public static DockGroupParams ToGroupParams(this IDockGroup dg)
        {
            DockGroupParams p = new DockGroupParams();

            p.GroupFullTypeName = dg.GetType().FullName;
            p.DockId = dg.DockId;
            p.ParentDockId = dg.DockParent?.DockId;
            p.IsPredefined = dg.IsPredefined;
            p.CanFloat = dg.CanFloat;
            p.CanRemove = dg.CanClose;
            
            if (dg.GetNumberChildren() > 0)
            {
                p.ChildrenDockIds = new List<string>();

                foreach(var child in dg.DockChildren)
                {
                    p.ChildrenDockIds.Add(child.DockId);
                }
            }

            if (dg is StackDockGroup dockStackGroup)
            {
                p.SizeCoefficients = dockStackGroup.GetSizeCoefficients();
            }

            if (dg is StackDockGroup stackGroup)
            {
                p.TheOrientation = stackGroup.TheOrientation;
            }

            if (dg is DockItem dockItem)
            {
                p.HeaderRestorationInfo = dockItem.Header?.ToString();
            }

            return p;
        }

        public static void SetGroupFromParams(this IDockGroup dg, DockGroupParams p)
        {
            dg.DockId = p.DockId!;
            dg.IsPredefined = p.IsPredefined;
            dg.CanFloat = p.CanFloat;
            dg.CanClose = p.CanRemove;

            if (dg is StackDockGroup stackGroup)
            {
                stackGroup.TheOrientation = (Orientation)p.TheOrientation!;
            }

            if (dg is DockItem dockItem)
            {
                dockItem.Header = p.HeaderRestorationInfo;
            }
        }

        public static IDockGroup ToGroup(this DockGroupParams p)
        {
            Type groupType = p.GetGroupType();

            IDockGroup dg = (IDockGroup) Activator.CreateInstance(groupType)!;

            dg.SetGroupFromParams(p);

            return dg;
        }
    }
}
