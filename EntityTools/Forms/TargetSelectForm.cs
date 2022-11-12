using System.Windows.Forms;
using Astral.Controllers;
using DevExpress.XtraEditors;

namespace EntityTools.Forms
{
    public partial class TargetSelectForm : XtraForm
    {
        static TargetSelectForm @this = null;
        private SimpleButton btnOK;
        private LabelControl lblMessage;

        public TargetSelectForm()
        {
            InitializeComponent();
        }

        public static DialogResult GUIRequest(string caption, string message = "")
        {
            if (@this == null)
                @this = new TargetSelectForm();

            try
            {
                @this.Text = caption;
                @this.lblMessage.Text = string.IsNullOrEmpty(message) ? caption : message;

                Binds.AddAction(Keys.F12, @this.btnOK.PerformClick);
                return @this.ShowDialog();
            }
            finally
            {
                Binds.RemoveAction(Keys.F12);
            }
        }
    }
}
