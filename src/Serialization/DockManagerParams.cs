using System.Collections.Generic;
using System.Xml.Serialization;

namespace NP.AvaloniaDock.Serialization
{
    public class DockManagerParams
    {
        [XmlArray]
        public List<WindowParams>? WindowsSerializationParams { get; set; }
    }

    public static class DockManagerParamsHelper
    {
        public static DockManagerParams ToParams(this DockManager dockManager)
        {
            DockManagerParams dmp = new DockManagerParams
            {
                WindowsSerializationParams = new List<WindowParams>()
            };

            foreach(var window in dockManager.Windows)
            {
                dmp.WindowsSerializationParams.Add(window.ToWindowParams());
            }

            return dmp;
        }
    }
}
