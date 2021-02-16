using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EntityTools.Editors.Forms
{
    public partial class ObjectInfoForm : XtraForm
    {
        readonly List<ObjectInfoForm> childForms = new List<ObjectInfoForm>();

        public ObjectInfoForm(object obj = null)
        {
            InitializeComponent();
            if (obj != null)
            {
                pgObjectInfos.SelectedObject = obj;
                Text = obj.ToString();
            }
        }

        public void Show(object obj)
        {
            pgObjectInfos.SelectedObject = obj;

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
                var form = new ObjectInfoForm(obj);
                childForms.Add(form);
                form.Show();
            }
        }

        private void handler_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            miDetail.Enabled = e.NewSelection.Value?.GetType().IsClass == true;
        }

        private void handler_FormClosed(object sender, FormClosedEventArgs e)
        {
            childForms.ForEach(f => f.Close());
        }
    }
}
