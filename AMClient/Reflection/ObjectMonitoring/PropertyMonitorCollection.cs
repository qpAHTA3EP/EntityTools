using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcTp0Tools.Reflection.ObjectMonitoring
{
    public class PropertyMonitorCollection : KeyedCollection<string, IPropertyMonitor>
    {
        protected override string GetKeyForItem(IPropertyMonitor item)
        {
            return item.Name;
        }
    }
}
