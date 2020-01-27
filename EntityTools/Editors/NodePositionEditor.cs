using Astral.Controllers;
using EntityTools.Forms;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    class NodePositionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            //while (MessageBox.Show("Target the node and press ok.", "Select node Posiotion", MessageBoxButtons.OKCancel) == DialogResult.OK)
            while (TargetSelectForm.TargetGuiRequest("Target the node and press ok.") == DialogResult.OK)
            {
                if (EntityManager.LocalPlayer.Player.InteractStatus.pMouseOverNode != IntPtr.Zero)
                {
                    using (List<TargetableNode>.Enumerator enumerator = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            TargetableNode targetableNode = enumerator.Current;
                            if (targetableNode.IsValid && targetableNode.IsMouseOver)
                                return targetableNode.WorldInteractionNode.Location.Clone();
                        }
                    }
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
