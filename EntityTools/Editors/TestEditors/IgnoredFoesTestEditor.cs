using ACTP0Tools;
using DevExpress.XtraEditors;
using EntityTools.Quester.Actions;
using EntityTools.Tools.Combats.IgnoredFoes;
using EntityTools.Tools.Extensions;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class IgnoredFoesTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context is null)
                return value;

            var sb = new StringBuilder();
            switch (context.Instance)
            {
                case AddIgnoredFoes addIgnFoes:
                {
                    var blackList = Astral.Quester.API.CurrentProfile.BlackList;
                    sb.Append(blackList, "Profile BlackList :", "Profile BlackList is empty");

                    IgnoredFoesCore.Remove();
                    addIgnFoes.Run();

                    var tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList();
                    sb.Append(tempBlackList, "Actual ignored foe list :", "Actual ignored foe list is empty");
                    sb.AppendLine();
                    break;
                }
                case RemoveIgnoredFoes removeIgnFoes:
                {
                    var tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList(); 
                    sb.Append(tempBlackList, "Ignored foe list before removing :", "Actual ignored foe list is empty");

                    removeIgnFoes.Run();
                    tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList(); 
                    sb.Append(tempBlackList, "Actual ignored foe list (after removing):", "Actual ignored foe list is empty");
                    break;
                }
            }

            if (sb.Length > 0)
                XtraMessageBox.Show(sb.ToString());

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
