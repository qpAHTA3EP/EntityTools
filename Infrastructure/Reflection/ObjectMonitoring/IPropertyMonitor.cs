using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcTp0Tools.Reflection.ObjectMonitoring
{
    public interface IPropertyMonitor
    {
        string Name { get; }
        string DisplayedValue { get; }
        object Value { get; }
        Type Type { get; }
        int TimeStamp { get; set; }
        void Reread(int timeStamp);
        ICollection<IPropertyMonitor> Members { get; }
    }
}
