using DevExpress.XtraEditors;
using EntityTools.Quester.Actions;
using AcTp0Tools.Reflection;
using EntityTools.Tools.Combats.IgnoredFoes;
using EntityTools.Tools.Extensions;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using AcTp0Tools;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class IgnoredFoesTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var sb = new StringBuilder();
            if(context.Instance is AddIgnoredFoes addIgnFoes)
            {
                var blackList = Astral.Quester.API.CurrentProfile.BlackList;
                sb.Append(blackList, "Profile BlackList :", "Profile BlackList is empty");

                IgnoredFoesCore.Remove();// addIgnFoes.Foes);
                addIgnFoes.Run();

                var tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList();//IgnoredFoesCore.ActualIgnoredFoes;
#if true
                sb.Append(tempBlackList, "Actual ignored foe list :", "Actual ignored foe list is empty");
                sb.AppendLine();
#else
                if (tempBlackList != null)
                {
                    using (var blEnumerator = tempBlackList.GetEnumerator())
                    {
                        sb.AppendLine("Actual ignored foe list :");
                        if (blEnumerator.MoveNext())
                        {
                            sb.Append('\t').Append(blEnumerator.Current);
                            while (blEnumerator.MoveNext())
                            {
                                sb.Append(" ,\n\t").Append(blEnumerator.Current);
                            }
                            sb.AppendLine();
                        }
                        else sb.AppendLine("Actual ignored foe list is empty");
                    }
                }
                else sb.AppendLine("Actual ignored foe list is empty"); 
#endif
            }
            else if(context.Instance is RemoveIgnoredFoes removeIgnFoes)
            {
                var tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList(); //IgnoredFoesCore.ActualIgnoredFoes;
                sb.Append(tempBlackList, "Ignored foe list before removing :", "Actual ignored foe list is empty");

                removeIgnFoes.Run();
                tempBlackList = AstralAccessors.Logic.NW.Combats.BLAttackersList(); //IgnoredFoesCore.ActualIgnoredFoes;
                sb.Append(tempBlackList, "Actual ignored foe list (after removing):", "Actual ignored foe list is empty");
            }

            if (sb.Length > 0)
                XtraMessageBox.Show(sb.ToString());

            return value;
        }

#if false
        private static void Append(StringBuilder sb, System.Collections.Generic.List<string> collection, string collectionCaption = "", string emptyMessage = "")
        {
            if (collection != null)
            {
                using (var blEnumerator = collection.GetEnumerator())
                {
                    if (blEnumerator.MoveNext())
                    {
                        if (!string.IsNullOrEmpty(collectionCaption))
                            sb.AppendLine(collectionCaption);
                        sb.Append('\t').Append(blEnumerator.Current);
                        while (blEnumerator.MoveNext())
                        {
                            sb.Append(" ,\n\t").Append(blEnumerator.Current);
                        }
                        sb.AppendLine();
                    }
                    else if (string.IsNullOrEmpty(emptyMessage))
                        sb.AppendLine(emptyMessage);
                }
            }
        } 
#endif

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
