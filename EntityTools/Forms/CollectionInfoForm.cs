using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    public partial class CollectionInfoForm : DevExpress.XtraEditors.XtraForm
    {
        protected CollectionInfoForm(Func<object> source, int refreshTime = 500)
        {
            InitializeComponent();
            this.source = source;
            this.refreshTime = refreshTime;
            safeRefreshGrid = SafeRefreshGrid;
        }

        //private static CancellationTokenSource tokenSource;
        private readonly Func<object> source;
        private readonly int refreshTime;

        public static void Show<T>(Func<IEnumerable<T>> source, int refreshTime = 500, IWin32Window owner = null)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (refreshTime <= 0)
                refreshTime = 500;

            var @this = new CollectionInfoForm(source, refreshTime);
            @this.Show(owner);
        }

        private void handler_FormClosed(object sender, FormClosedEventArgs e)
        {
            //tokenSource?.Cancel();
            backgroundWorker.CancelAsync();
        }

        public delegate void SafeRefreshGridDelegate(object items);
        private readonly SafeRefreshGridDelegate safeRefreshGrid;
        private void SafeRefreshGrid(object items)
        {
            gridControl.BeginUpdate();
            gridControl.DataSource = items;
            gridControl.EndUpdate();
            gridControl.Refresh();
        }


        private void RefreshGrid(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (!backgroundWorker.CancellationPending)
            {
                Invoke(safeRefreshGrid, source());

                Thread.Sleep(refreshTime);
            }
        }

        private void handler_LoadForm(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }
    }
}