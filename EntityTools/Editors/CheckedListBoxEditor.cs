using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using EntityTools.Tools.Inventory;
using MyNW.Patchables.Enums;

namespace EntityTools.Editors
{
    internal class BagsListEditor : UITypeEditor, IDisposable
    {
        public override bool IsDropDownResizable => true;

        internal BagsListEditor()
        {
            cbx.KeyDown += cbx_KeyDown;
            cbx.MouseHover += cbx_ShowTooltip;
        }

        private void cbx_ShowTooltip(object sender, EventArgs e)
        {
            int num = cbx.IndexFromPoint(cbx.PointToClient(Control.MousePosition));
            bool flag = num > -1;
            if (flag)
            {
                toolTip.SetToolTip(cbx, cbx.Items[num].ToString());
            }
        }

        private void cbx_KeyDown(object sender, KeyEventArgs e)
        {
            bool flag = e.Control && e.KeyCode == Keys.A;
            if (flag)
            {
                e.SuppressKeyPress = true;
                for (int i = 0; i < cbx.Items.Count; i++)
                {
                    cbx.SetItemChecked(i, true);
                }
            }
            bool flag2 = e.Control && e.KeyCode == Keys.D;
            if (flag2)
            {
                e.SuppressKeyPress = true;
                for (int j = 0; j < cbx.Items.Count; j++)
                {
                    cbx.SetItemChecked(j, false);
                }
            }
            bool flag3 = e.Control && e.KeyCode == Keys.I;
            if (flag3)
            {
                e.SuppressKeyPress = true;
                for (int k = 0; k < cbx.Items.Count; k++)
                {
                    cbx.SetItemChecked(k, !cbx.GetItemChecked(k));
                }
            }
            bool flag4 = e.Control && e.KeyCode == Keys.S;
            if (flag4)
            {
                e.SuppressKeyPress = true;
                for (int l = 0; l < cbx.Items.Count; l++)
                {
                    cbx.Sorted = true;
                }
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService windowsFormsEditorService;
            BagsList bagsList = value as BagsList;
            bool flag = (windowsFormsEditorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService) != null && bagsList != null;
            object result;
            if (flag)
            {
                foreach (object obj in Enum.GetValues(typeof(InvBagIDs)))
                {
                    InvBagIDs invBagIDs = (InvBagIDs)obj;
                    int index = cbx.Items.Add(invBagIDs);
                    cbx.SetItemChecked(index, bagsList[invBagIDs]);
                }
                windowsFormsEditorService.DropDownControl(cbx);
                bagsList = new BagsList(null);
                foreach (object obj2 in cbx.CheckedItems)
                {
                    InvBagIDs bagId = (InvBagIDs)obj2;
                    bagsList[bagId] = true;
                }
                result = bagsList;
            }
            else
            {
                result = value;
            }
            return result;
        }

        public void Dispose()
        {
            cbx.Dispose();
        }

        private readonly CheckedListBox cbx = new CheckedListBox();

        private ToolTip toolTip = new ToolTip
        {
            ToolTipTitle = "Ctrl+A to select all, Ctrl+D to deselect, Ctrl+I to inverse, Ctrl+S to sort"
        };
    }
}
