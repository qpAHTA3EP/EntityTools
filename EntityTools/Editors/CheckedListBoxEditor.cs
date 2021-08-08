using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using EntityTools.Tools.BuySellItems;
using MyNW.Patchables.Enums;

namespace EntityTools.Editors
{
    // Token: 0x02000091 RID: 145
    internal class BagsListEditor : UITypeEditor, IDisposable
    {
        // Token: 0x170001E4 RID: 484
        // (get) Token: 0x060005C1 RID: 1473 RVA: 0x00027159 File Offset: 0x00025359
        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }

        // Token: 0x060005C2 RID: 1474 RVA: 0x0002715C File Offset: 0x0002535C
        internal BagsListEditor()
        {
            cbx.KeyDown += cbx_KeyDown;
            cbx.MouseHover += cbx_ShowTooltip;
        }

        // Token: 0x060005C3 RID: 1475 RVA: 0x000271C4 File Offset: 0x000253C4
        private void cbx_ShowTooltip(object sender, EventArgs e)
        {
            int num = cbx.IndexFromPoint(cbx.PointToClient(Control.MousePosition));
            bool flag = num > -1;
            if (flag)
            {
                toolTip.SetToolTip(cbx, cbx.Items[num].ToString());
            }
        }

        // Token: 0x060005C4 RID: 1476 RVA: 0x00027220 File Offset: 0x00025420
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

        // Token: 0x060005C5 RID: 1477 RVA: 0x0002739C File Offset: 0x0002559C
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        // Token: 0x060005C6 RID: 1478 RVA: 0x000273B0 File Offset: 0x000255B0
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService windowsFormsEditorService;
            BagsList bagsList = value as BagsList;
            bool flag = (windowsFormsEditorService = (provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService)) != null && bagsList != null;
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

        // Token: 0x060005C7 RID: 1479 RVA: 0x000274EC File Offset: 0x000256EC
        public void Dispose()
        {
            cbx.Dispose();
        }

        // Token: 0x0400037E RID: 894
        private readonly CheckedListBox cbx = new CheckedListBox();

        // Token: 0x0400037F RID: 895
        private ToolTip toolTip = new ToolTip
        {
            ToolTipTitle = "Ctrl+A to select all, Ctrl+D to deselect, Ctrl+I to inverse, Ctrl+S to sort"
        };
    }
}
