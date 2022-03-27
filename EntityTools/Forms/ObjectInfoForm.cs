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
        private int monitoringRefreshTime;

        protected ObjectInfoForm(object obj = null)
        {
            InitializeComponent();
            if (obj != null)
            {
                pgObjectInfos.SelectedObject = obj;
                Text = obj.ToString();
            }
        }

        protected new void Show(IWin32Window owner)
        {
            base.Show(owner);
            backgroundWorker.RunWorkerAsync();
        }

        public static ObjectInfoForm Show(object obj, int refreshTime = 500, IWin32Window owner = null)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (refreshTime <= 0)
                refreshTime = 500;

            var @this = new ObjectInfoForm
            {
                pgObjectInfos = {SelectedObject = obj},
                monitoringRefreshTime = refreshTime,

                Text = obj.ToString()
            };

            //if(owner is null)
            //    base.Show();
            //else base.
            @this.Show(owner);

            return @this;
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
                childForms.Add(Show(obj, monitoringRefreshTime));
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
