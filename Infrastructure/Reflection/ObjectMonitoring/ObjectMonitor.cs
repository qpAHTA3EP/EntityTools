using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcTp0Tools.Reflection.ObjectMonitoring
{
    public class ObjectMonitor : IPropertyMonitor
    {
        public static readonly ICollection<IPropertyMonitor> EmptyMonitorCollection = new List<IPropertyMonitor>().AsReadOnly();

        private object @object;
        public ObjectMonitor(ObjectMonitor obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            @object = obj;
        }

        readonly PropertyMonitorCollection properties = new PropertyMonitorCollection();

        public string Name => throw new NotImplementedException();

        public string DisplayedValue => throw new NotImplementedException();

        public object Value => throw new NotImplementedException();

        public Type Type => throw new NotImplementedException();

        public int TimeStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<IPropertyMonitor> Members => throw new NotImplementedException();



        public void Reread(int timeStamp)
        {
            throw new NotImplementedException();
        }
    }
}
