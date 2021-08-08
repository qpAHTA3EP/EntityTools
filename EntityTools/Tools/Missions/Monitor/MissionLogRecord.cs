using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MissionLogRecord
    {
        public readonly object Sender;
        public readonly long   TimeStamp;
        public readonly string MissionName;
        public readonly string PropertyName;
        public readonly string SubPropertyName;
        public readonly object OldValue;
        public readonly object NewValue;

        public MissionLogRecord(object sender, long timeStamp, string missionName, string propertyName, string subPropertyName,
            object oldValue, object newValue)
        {
            Sender = sender;
            TimeStamp = timeStamp;
            MissionName = missionName;
            PropertyName = propertyName;
            SubPropertyName = subPropertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
