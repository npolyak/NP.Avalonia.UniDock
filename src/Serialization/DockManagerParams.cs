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
using Avalonia.VisualTree;
using NP.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NP.Avalonia.UniDock.Serialization
{
    public class DockManagerParams
    {
        [XmlArray]
        public List<WindowParams>? WindowsSerializationParams { get; set; }

        [XmlArray]
        public List<DockGroupParams>? DockGroupParams { get; set; }
    }

    public static class DockManagerParamsHelper
    {
        public static DockManagerParams ToParams(this DockManager dockManager)
        {
            DockManagerParams dmp = new DockManagerParams
            {
                WindowsSerializationParams = new List<WindowParams>(),

                DockGroupParams = new List<DockGroupParams>()
            };

            foreach(var window in dockManager.Windows)
            {
                dmp.WindowsSerializationParams.Add(window.ToWindowParams());
            }

            foreach(var group in dockManager.AllGroups)
            {
                dmp.DockGroupParams.Add(group.ToGroupParams());
            }    

            return dmp;
        }

        private static Window? GetWindowById(this DockManager dm, string? windowId)
        {
            Window? w = dm.Windows.FirstOrDefault(win => DockAttachedProperties.GetWindowId(win) == windowId);

            return w;
        }

        private static WindowParams? GetWindowParamsById(this DockManagerParams dmp, string? windowId)
        {
            WindowParams? wp = dmp.WindowsSerializationParams.NullToEmpty()?.FirstOrDefault(winParams => winParams.WindowId == windowId);

            return wp;
        }

        private static Window? ShowOwnersAndWindow(this DockManager dm, DockManagerParams dmp, string windowId)
        {
            WindowParams? wp = dmp.GetWindowParamsById(windowId);

            if (wp == null)
                return null;

            Window? w = dm.GetWindowById(windowId);

            if (w == null || w.IsVisible)
            {
                return w;
            }

            if (wp.DockChildWindowOwnerId != null)
            {
                Window? childWindowOwner = dm.GetWindowById(wp.DockChildWindowOwnerId);

                if (childWindowOwner != null)
                {
                    DockAttachedProperties.SetDockChildWindowOwner(w, childWindowOwner);
                }
            }

            if (wp.OwnerWindowId != null)
            {
                Window? owner = dm.ShowOwnersAndWindow(dmp, wp.OwnerWindowId);

                if (owner != null)
                {
                    w.Show(owner);
                    return w;
                }
            }

            w.Show();

            return w;
        }

        public static IDockGroup? FindGroupById(this DockManager dm, string? dockId)
        {
            var result = dm.AllGroups.FirstOrDefault(g => g.DockId == dockId);

            return result;
        }

        public static DockGroupParams? FindGroupParamsById(this DockManagerParams dmp, string? dockId)
        {
            var result = dmp.DockGroupParams.NullToEmpty().FirstOrDefault(g => g.DockId == dockId);

            return result;
        }

        public static IDockGroup? BuildGroup(this DockManager dm, DockManagerParams dmp, string? dockId, IEnumerable<DockItem> dockItems)
        {
            DockGroupParams? p = dmp.FindGroupParamsById(dockId);
            IDockGroup? dg = dm.FindGroupById(dockId);

            if (p == null)
                return null;

            if (dg == null)
            {
                if (p.IsDockItem())
                {
                    dg = dockItems.FirstOrDefault(di => di.DockId == dockId);
                }
            }

            if (dg == null)
            {
                dg = p.ToGroup();
            }
            else
            {
                dg.SetGroupFromParams(p);
            }

            dg.TheDockManager = dm;

            foreach (var childDockId in p.ChildrenDockIds.NullToEmpty())
            {
                IDockGroup? childGroup = dm.BuildGroup(dmp, childDockId, dockItems);

                if (childGroup != null)
                {
                    dg.DockChildren.Add(childGroup);
                }
            }

            return dg;
        }

        public static void SetDockManagerFromParams(this DockManager dm, DockManagerParams dmp)
        {
            List<DockItem> dockItems = dm.AllGroups.OfType<DockItem>().ToList();

            dm.ClearGroups();

            var newWindowIds = dmp.WindowsSerializationParams.NullToEmpty().Select(wp => wp.WindowId).ToList();

            // clear the windows that do not exist in the serialization.
            var oldWindowsToRemove = 
                dm.Windows.Where(w => !DockAttachedProperties.GetWindowId(w).IsInValCollection(newWindowIds)).ToList();
            foreach(var w in oldWindowsToRemove)
            {
                w.Close();

                dm.RemoveWindow(w);

                DockAttachedProperties.SetTheDockManager(w, null!);
            }

            // create windows
            foreach(WindowParams wp in dmp.WindowsSerializationParams.NullToEmpty())
            {
                Window? w = 
                    dm.Windows.FirstOrDefault(win => DockAttachedProperties.GetWindowId(win) == wp.WindowId);

                if (w != null)
                {
                    w.SetWindowFromParams(wp);
                }
                else
                {
                    w = wp.RestoreWindow();

                    if (w != null)
                    {
                        DockAttachedProperties.SetTheDockManager(w, dm);
                    }
                }
            }

            foreach(WindowParams wp in dmp.WindowsSerializationParams.NullToEmpty())
            {
                // show windows - owners first
                dm.ShowOwnersAndWindow(dmp, wp.WindowId!);
            }

            // set the groups
            foreach(Window w in dm.Windows)
            {
                if (w is DockWindow dw)
                {
                    string dockId = dw.TheDockGroup.DockId;

                    dm.BuildGroup(dmp, dockId, dockItems);
                }
                else
                {
                    var winGroups = 
                        w.GetVisualDescendants().OfType<IDockGroup>().ToList();

                    winGroups.DoForEach(g => dm.BuildGroup(dmp, g.DockId, dockItems));
                }
            }
        }
    }
}
