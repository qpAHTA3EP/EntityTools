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
    public partial class MissionMonitorForm : DevExpress.XtraEditors.XtraForm
    {
        MissionMonitor missionMonitor;

        public MissionMonitorForm()
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
                    missionMonitor = new MissionMonitor(mission, -1, true);
                    treeViewMissions.Nodes.Clear();
                    var children = new TreeNode[2];
                    children[0] = new TreeNode(nameof(missionMonitor.MissionDef) + ": " + missionMonitor.MissionDef) { Tag = missionMonitor.MissionDef };
                    children[1] = new TreeNode(nameof(missionMonitor.Childrens) + " [" + missionMonitor.Childrens.Count + "]") { Tag = missionMonitor.MissionDef };
                    TreeNode tn = new TreeNode(missionMonitor.ToString(), children) { Tag = missionMonitor };
                    treeViewMissions.Nodes.Add(tn);
                }
            }
            else
            {
                missionMonitor = null;
                treeViewMissions.Nodes.Clear();
            }
        }

        private void handler_ExpandNode(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void handler_SelectNode(object sender, TreeViewEventArgs e)
        {
            var selection = e.Node.Tag;
            pgDetail.SelectedObject = selection;
            pgDetail2.SelectedObject = selection;
        }
    }
}