using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using EntityTools.Tools.Missions;
using EntityTools.Tools.Missions.Monitor;

namespace EntityTools.Forms
{
    public partial class MissionMonitorForm2 : DevExpress.XtraEditors.XtraForm
    {
        MissionMonitor2 missionMonitor;

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
                    missionMonitor = new MissionMonitor2(mission, "", -1, true);
                    listMissions.Nodes.Clear();
                    listMissionDef.Nodes.Clear();
                    listMissions.DataSource = missionMonitor;// new List<>(missionMonitor);
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
                missionMonitor = null;
                listMissions.Nodes.Clear();
                listMissionDef.Nodes.Clear();
            }
        }

        private void handler_ExpandNode(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void handler_SelectNode(object sender, TreeViewEventArgs e)
        {
            var selection = e.Node.Tag;
            //pgDetail.SelectedObject = selection;
        }
    }
}