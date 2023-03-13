using DevExpress.XtraEditors;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    internal class CurrentShardEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var characterChoices = Game.CharacterSelectionData.CharacterChoices;
            if (characterChoices.IsValid 
                && characterChoices.LastPlayedCharacter.IsValid)
            {
                DialogResult result = XtraMessageBox.Show("Set to current Shard ?");

                if (result == DialogResult.OK)
                {
                    return characterChoices.LastPlayedCharacter.ShardName;
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
