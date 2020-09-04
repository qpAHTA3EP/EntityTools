using System;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Forms
{
    public partial class PositionEditorForm : XtraForm
    {
        private Vector3 usedPos;
        private Vector3 originalPos;
        private bool valid;
        private Func<Vector3> acquisition;

        private PositionEditorForm()
        {
            InitializeComponent();
        }

        public static void Show(Vector3 position, Func<Vector3> acquisition)
        {
            var positionEdit = new PositionEditorForm
                {usedPos = position, originalPos = position.Clone(), acquisition = acquisition};
            positionEdit.RefreshPos();
            positionEdit.ShowDialog();
            if (positionEdit.valid) return;
            positionEdit.usedPos.X = positionEdit.originalPos.X;
            positionEdit.usedPos.Y = positionEdit.originalPos.Y;
            positionEdit.usedPos.Z = positionEdit.originalPos.Z;
        }

        private void RefreshPos()
        {
            position.Text = usedPos.ToString();
        }

        private void b_PlayerPos_Click(object sender, EventArgs e)
        {
            var location = EntityManager.LocalPlayer.Location;
            if (location.IsValid)
            {
                usedPos.X = location.X;
                usedPos.Y = location.Y;
                usedPos.Z = location.Z;
                RefreshPos();
                return;
            }
            XtraMessageBox.Show("No valid player position.");
        }

        private void b_TargetPos_Click(object sender, EventArgs e)
        {
            var location = acquisition();
            if (location.IsValid)
            {
                usedPos.X = location.X;
                usedPos.Y = location.Y;
                usedPos.Z = location.Z;
                RefreshPos();
                return;
            }
            XtraMessageBox.Show("No valid target position.");
        }

        private void b_Valid_Click(object sender, EventArgs e)
        {
            valid = true;
            Close();
        }
    }
}
