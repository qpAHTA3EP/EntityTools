using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EntityTools.Forms
{
    public partial class ObjectInfoForm : XtraForm
    {
        readonly List<ObjectInfoForm> childForms = new List<ObjectInfoForm>();
        private int monitoringRefreshTime = 0;

        public ObjectInfoForm(object obj = null)
        {
            InitializeComponent();
            if (obj != null)
            {
                pgObjectInfos.SelectedObject = obj;
                Text = obj.ToString();
            }
        }

        public void Show(object obj, int monitoringRefreshTime = 0)
        {
            pgObjectInfos.SelectedObject = obj;
            if (monitoringRefreshTime > 0)
            {
                this.monitoringRefreshTime = monitoringRefreshTime;
                backgroundWorker.RunWorkerAsync();
            }

            Text = obj?.ToString() ?? string.Empty;
            Show();
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void handler_ItemDetail(object sender, EventArgs e)
        {
            var obj = pgObjectInfos.SelectedGridItem.Value;
            if (obj != null && obj.GetType().IsClass)
            {
                var form = new ObjectInfoForm();
                childForms.Add(form);
                form.Show(obj, monitoringRefreshTime);
            }
        }

        private void handler_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            miDetail.Enabled = e.NewSelection.Value?.GetType().IsClass == true;
        }

        private void handler_FormClosed(object sender, FormClosedEventArgs e)
        {
            childForms.ForEach(f => f.Close());
            backgroundWorker.CancelAsync();
        }

        private void Refresh(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!IsDisposed && !backgroundWorker.CancellationPending)
            {
                pgObjectInfos.Refresh(); 
                Thread.Sleep(monitoringRefreshTime);
            }
        }
    }
}
