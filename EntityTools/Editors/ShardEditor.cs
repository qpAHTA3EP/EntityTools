using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    public class CurrentShardEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (Game.CharacterSelectionData.CharacterChoices.IsValid && Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.IsValid)
            {
                DialogResult result = MessageBox.Show("Set to current Shard ?");
                if (result == DialogResult.OK)
                {
                    return Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName;
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
