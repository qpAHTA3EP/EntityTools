using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcTp0Tools.Reflection.ObjectMonitoring
{
    public static class ObjectMonitorHelper
    {
        public static bool ConstructPropertyMonitorCollection(Func<object> getOwner, out ICollection<IPropertyMonitor> properties, int timeStamp = -1, bool inDepth = false)
        {
            var owner = getOwner();
            var o_type = owner.GetType();
            var collections = new LinkedList<IPropertyMonitor>();
            if (timeStamp <= 0)
                timeStamp = Environment.TickCount;
            foreach (var prInfo in o_type.GetProperties())
            {
                var propertyType = prInfo.PropertyType;
                if (propertyType.IsPrimitive)
                {
                    var prMon = new PrimitivePropertyMonitor(owner, prInfo, timeStamp);
                    collections.AddLast(prMon);
                }
                else if (propertyType.IsArray)
                else if (propertyType.IsClass && inDepth)
                {
                    var complexMon = new ComplexPropertyMonitor(() => prInfo.GetValue(getOwner()), prInfo, timeStamp);
                    collections.AddLast(complexMon);
                }
            }
        }

    }
}
