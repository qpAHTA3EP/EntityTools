using EntityTools.Tools.Missions.Monitor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class MissionMonitorForm : DevExpress.XtraEditors.XtraForm
    {
        public MissionMonitorForm()
        {
            InitializeComponent();
            pgSettings.SelectedObject = EntityTools.Config.MissionMonitor;
        }

#if false
        private CancellationTokenSource tokenSource;

        private void Update(CancellationToken token)
        {
            while (!token.IsCancellationRequested && treeMissions.Nodes.Count > 0)
            {
                var timeStamp = DateTime.Now.Ticks;
                treeMissions.BeginUpdate();
                for (int i = 0; i < treeMissions.Nodes.Count; i++)
                {
                    if (treeMissions.Nodes[i] is MissionTreeNode missNode)
                        missNode.Update(timeStamp);
                }
                treeMissions.EndUpdate();
                Thread.Sleep(1000);
            }
        } 
#endif

        private void handler_SelectMission(object sender, EventArgs e)
        {
            var missId = Astral.Quester.Forms.GetMissionId.Show();
            
            if (string.IsNullOrEmpty(missId)) return;

            var mission = Astral.Logic.NW.Missions.GetMissionByPath(missId);

            if (mission is null) return;

            error = null;
            treeMissions.Nodes.Clear();
            TimeStamps.Clear();

            var now = DateTime.Now;
            var currentTicks = now.Ticks;
            var missNode = new MissionTreeNode(mission, currentTicks, LogMissionChanges);
            treeMissions.Nodes.Add(missNode);

            txtMissionLog.AppendText("<================== MONITORING START ==================>");
            txtMissionLog.AppendText(Environment.NewLine);
            txtMissionLog.AppendText(string.Concat('[', now.ToLongTimeString(), "] ", missId, Environment.NewLine));
            txtMissionLog.AppendText(Environment.NewLine);

            TimeStamps.Add(currentTicks);
            trackMonitoring.Properties.Maximum = TimeStamps.Count;
            trackMonitoring.Value = TimeStamps.Count;
#if false
            tokenSource = new CancellationTokenSource();
            Task.Run(() => Update(tokenSource.Token)); 
#else
            backgroundWorker.RunWorkerAsync();
#endif
        }

#if false
        private void handler_Update(object sender, EventArgs e)
        {
            TreeMissionsUpdate();
        }

        void TreeMissionsUpdate()
        {
            var timeStamp = DateTime.Now.Ticks;
            treeMissions.BeginUpdate();
            for (int i = 0; i < treeMissions.Nodes.Count; i++)
            {
                if (treeMissions.Nodes[i] is MissionTreeNode missNode)
                    missNode.Update(timeStamp);
            }
            treeMissions.EndUpdate();
        } 
#endif

        void LogMissionChanges(MissionTreeNode sender, long timeStamp, string missionName, string propertyName, string subPropertyName,
            object oldValue, object newValue)
        {
            var dateTime = new DateTime(timeStamp);

            var msg = string.Concat('[', dateTime.ToLongTimeString(), "] ", missionName, 
                string.IsNullOrEmpty(propertyName) ? string.Empty : string.Concat('.', propertyName),
                string.IsNullOrEmpty(subPropertyName) ? string.Empty : string.Concat('.', subPropertyName),
                " : ",
                oldValue is null ? newValue : oldValue + " => " + newValue,
                Environment.NewLine);
            txtMissionLog.AppendText(msg);
        }


        private int processingInd;
        private static readonly char[] processingChar = { '-', '\\', '|', '/' };

        private void ChangeCaption()
        {
            string caption;
            var count = TimeStamps.Count;
            var trackValue = trackMonitoring.Value;
            if (error is null)
            {
                if (count > 0 && trackValue <= count)
                {
                    DateTime dateTime = new DateTime(TimeStamps[trackValue - 1]);

                    caption = backgroundWorker.IsBusy
                        ? string.Concat("Mission Monitor [", dateTime.ToLongTimeString(), "] ( ", processingChar[++processingInd % 4], " )")
                        : string.Concat("Mission Monitor [", dateTime.ToLongTimeString(), "]");
                }
                else
                {
                    caption = backgroundWorker.IsBusy
                        ? string.Concat("Mission Monitor ( ", processingChar[++processingInd % 4], " )")
                        : string.Concat("Mission Monitor");
                } 
            }
            else
            {
                if (count > 0 && trackValue <= count)
                {
                    DateTime dateTime = new DateTime(TimeStamps[trackValue - 1]);
                    caption = string.Concat("Mission Monitor [", dateTime.ToLongTimeString(), "] : Error");
                }
                else caption = string.Concat("Mission Monitor : Error");
            }

            Text = caption;
        }

        private readonly List<long> TimeStamps = new List<long>(32);
        private Exception error;

        private void Monitoring(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Cursor = Cursors.AppStarting;
            try
            {
                bool stopMonitoring = false;
                while (!stopMonitoring && !backgroundWorker.CancellationPending && treeMissions.Nodes.Count > 0)
                {
                    Thread.Sleep(EntityTools.Config.MissionMonitor.UpdateTimeout);
#if false
                    treeMissions.Invoke(treeMissionsUpdate);
#elif true
                    Action updater = null;
                    var timeStamp = DateTime.Now.Ticks;
                    treeMissions.BeginUpdate();
                    for (int i = 0; i < treeMissions.Nodes.Count; i++)
                    {
                        if (treeMissions.Nodes[i] is MissionTreeNode missNode)
                        {
                            if (missNode.IsValid)
                                missNode.DelayedUpdate(timeStamp, ref updater);
                            else
                            {
                                txtMissionLog.AppendText(string.Concat('[', DateTime.Now.ToLongTimeString(), "] ", missNode.Name, '.' + nameof(MissionTreeNode.IsValid)+ " : False", Environment.NewLine));
                                updater = null;
                                stopMonitoring = true;
                                break;
                            }
                        }
                    }

                    if (updater != null)
                    {
                        treeMissions.Invoke(updater);
                        TimeStamps.Add(timeStamp);
                        var isLast = trackMonitoring.Value == trackMonitoring.Properties.Maximum;
                        trackMonitoring.Properties.Maximum = TimeStamps.Count;
                        //trackMonitoring.Properties.Labels.Add(new TrackBarLabel(, ));
                        if (isLast)
                            trackMonitoring.Value = TimeStamps.Count;

                        //txtMissionLog.AppendText(Environment.NewLine);
                        txtMissionLog.Select(txtMissionLog.TextLength, 0);
                        txtMissionLog.ScrollToCaret();
                    }
                    treeMissions.EndUpdate();

#else
                    var timeStamp = DateTime.Now.Ticks;
                    treeMissions.BeginUpdate();
                    for (int i = 0; i < treeMissions.Nodes.Count; i++)
                    {
                        if (treeMissions.Nodes[i] is MissionTreeNode missNode)
                            missNode.Update(timeStamp);
                    }
                    treeMissions.EndUpdate(); 
#endif
                    ChangeCaption();
                }
                
                txtMissionLog.AppendText(string.Concat('[', DateTime.Now.ToLongTimeString(), "] Stopped", Environment.NewLine)); 
                
                ChangeCaption();
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                txtMissionLog.AppendText(string.Concat('[', DateTime.Now.ToLongTimeString(), "] ", ex.ToString()));
                error = ex;
                ChangeCaption();
                Cursor = Cursors.Default;
                throw;
            }
        }

        private void handler_Stop(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void handler_Save(object sender, EventArgs e)
        {
            if (txtMissionLog.Text.Length <= 0) return;

            saveFileDialog.InitialDirectory = Astral.Controllers.Directories.LogsPath;
            string fileName;
            var nodes = treeMissions.Nodes;
            if (nodes.Count > 0 && nodes[0] is MissionTreeNode missNode)
                fileName = string.Concat(missNode.MissionName, '_', DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"),".txt");
            else fileName = string.Concat("MissionLog_", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), ".txt");
            saveFileDialog.FileName = fileName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.AppendAllLines(saveFileDialog.FileName, txtMissionLog.Lines);
            }
        }
    }
}