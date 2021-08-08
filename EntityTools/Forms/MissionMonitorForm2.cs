using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EntityTools.Tools.Missions.Monitor;

namespace EntityTools.Forms
{
    public partial class MissionMonitorForm2 : DevExpress.XtraEditors.XtraForm
    {
        //MissionMonitor2 missionMonitor;

        public MissionMonitorForm2()
        {
            InitializeComponent();
        }

        private void handler_SelectMission(object sender, EventArgs e)
        {
            var missId = Astral.Quester.Forms.GetMissionId.Show();
            if (!string.IsNullOrEmpty(missId))
            {
                var mission = Astral.Logic.NW.Missions.GetMissionByPath(missId);

                if (mission != null)
                {
                    var missionMonitor = new MissionMonitor2(mission, -1, true);
                    listMissions.Nodes.Clear();
                    listMissionDef.Nodes.Clear();
                    listMissions.DataSource = new List<MissionMonitor2>() { missionMonitor };
#if false
                    var children = new TreeNode[2];
                    children[0] = new TreeNode(nameof(missionMonitor.MissionDef) + ": " + missionMonitor.MissionDef) { Tag = missionMonitor.MissionDef };
                    children[1] = new TreeNode(nameof(missionMonitor.Childrens) + " [" + missionMonitor.Childrens.Count + "]") { Tag = missionMonitor.MissionDef };
                    TreeNode tn = new TreeNode(missionMonitor.ToString(), children) { Tag = missionMonitor }; 
#endif

                }
            }
            else
            {
                //missionMonitor = null;
                listMissions.Nodes.Clear();
                listMissionDef.Nodes.Clear();
            }
        }

        private void handler_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            var selected = e.Node.GetValue(clmnMissionDef);
            listMissionDef.DataSource = new List<object>() { selected };
        }
    }
}