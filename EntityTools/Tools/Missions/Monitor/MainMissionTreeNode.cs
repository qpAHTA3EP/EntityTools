using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNW.Classes;

namespace EntityTools.Tools.Missions.Monitor
{
    public class MainMissionTreeNode : MissionTreeNode
    {
        public MainMissionTreeNode(Mission mission, long timeStamp = -1, NotifyMissionChanged notifier = null) : base(mission, timeStamp, notifier){}

        protected override void ChildMissionChanged(MissionTreeNode sender, long timeStamp, string missionName, string propertyName,
            string subPropertyName, object oldValue, object newValue)
        {
            base.ChildMissionChanged(sender, timeStamp, missionName, propertyName, subPropertyName, oldValue, newValue);
        }

        protected override void MissionDefChanged(MissionDefTreeNode sender, long timeStamp, string missionDefName, string propertyName,
            object oldValue, object newValue)
        {
            base.MissionDefChanged(sender, timeStamp, missionDefName, propertyName, oldValue, newValue);
        }
    }
}
