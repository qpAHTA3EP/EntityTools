using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper
{
    public partial class ObjectInfoForm : XtraForm
    {
        //MenuItem miDetail;

        public ObjectInfoForm(object obj = null)
        {
            InitializeComponent();
            if (obj != null)
            {
                pgObjectInfos.SelectedObject = obj;
                Text = obj.ToString();
            }
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void Show(object obj)
        {
            pgObjectInfos.SelectedObject = obj;

            Text = obj?.ToString() ?? string.Empty;
            Show();
        }

        private void handler_ItemDetail(object sender, EventArgs e)
        {
            var obj = pgObjectInfos.SelectedGridItem.Value;
            if (obj != null && obj.GetType().IsClass)
            {
                var form = new ObjectInfoForm(obj);
                form.Show();
            }
        }

        private void pgObjectInfos_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            miDetail.Enabled = e.NewSelection.Value?.GetType().IsClass == true;
        }
    }
}
