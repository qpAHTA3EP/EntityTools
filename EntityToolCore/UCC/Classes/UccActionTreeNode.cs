using AcTp0Tools.Reflection;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityTools.UCC.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using EntityCore.Tools;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="UCCAction"/>
    /// </summary>
    public class UccActionTreeNode : TreeNode, IUccActionTreeNode
    {
        //public UCCAction Data => (UCCAction)Tag;

        public bool AllowChildren => false;

        public UccActionTreeNode(UCCAction action, bool clone = false)
        {
            var act = clone ? CopyHelper.CreateDeepCopy(action) : action;
            Tag = act;
            SelectIcon(act);
            var txt = act.Label;
            if (txt == "(Unknown Spell)")
            {
                txt = act.GetType().Name;
            }
            Text = txt;
            Checked = act.Enabled;

            _conditionTreeNodes = act.Conditions.ToTreeNodes().ToArray();
        }

        private void SelectIcon(UCCAction action)
        {
            switch (action)
            {
                case Spell _:
                case ExecuteSpecificPower _:
                case Special spec when spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.Artifact:
                    ImageKey = "Power";
                    SelectedImageKey = "Power";
                    break;
                case Consumables _:
                    ImageKey = "Flask";
                    SelectedImageKey = "Flask";
                    break;
                case UseItemSpecial _:
                    ImageKey = "Gem";
                    SelectedImageKey = "Gem";
                    break;
                case PluggedSkill _:
                //ImageKey = "Pazl";
                //SelectedImageKey = "Pazl";
                //break;
                case Special spec when spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.Artifact:
                    ImageKey = "Art";
                    SelectedImageKey = "Art";
                    break;
                case ApproachEntity _:
                case Special spec when spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.ApproachMelee
                                    || spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.GoToMelee
                                    || spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.AvoidMelee:
                    ImageKey = "Track";
                    SelectedImageKey = "Track";
                    break;
                case AbordCombat _:
                case Special spec when spec.Action == Astral.Logic.UCC.Ressources.Enums.SpecialUCCAction.ResetCombat:
                    ImageKey = "Cancel";
                    SelectedImageKey = "Cancel";
                    break;
                //case Special _:
                //    ImageKey = "List";
                //    SelectedImageKey = "List";
                //    break;
                case Dodge _:
                case DodgeFromEntity _:
                    ImageKey = "Dodge";
                    SelectedImageKey = "Dodge";
                    break;
                case ChangeTarget _:
                    ImageKey = "Target";
                    SelectedImageKey = "Target";
                    break;
                case SpecializedUCCAction _:
                    ImageKey = "Cube";
                    SelectedImageKey = "Cube";
                    break;
                default:
                    ImageKey = "Gear";
                    SelectedImageKey = "Gear";
                    break;
            }
        }
        public void UpdateView()
        {
            if (Tag is UCCAction action)
            {
                var txt = action.Label;
                if (txt == "(Unknown Spell)")
                {
                    txt = action.GetType().Name;
                }
                Text = txt;
                Checked = action.Enabled;
                SelectIcon(action);
                TreeView.Refresh();
            }
            else throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCAction)}");
        }

        public UCCAction ReconstructInternal()
        {
            if (Tag is UCCAction action)
            {
                if(_conditionTreeNodes?.Length > 0)
                    action.Conditions = _conditionTreeNodes.ToUccConditionList();
                return action;
            }
            return Tag as UCCAction;
        }

        public override object Clone()
        {
            //TODO: Добави реконструкцию списка условий
            return new UccActionTreeNode(CopyHelper.CreateDeepCopy(ReconstructInternal()));
        }


        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="UCCAction"/>
        /// </summary>
        public TreeNode[] ConditionTreeNodes
        {
            get => _conditionTreeNodes ?? (_conditionTreeNodes = ReconstructInternal().Conditions.ToTreeNodes().ToArray()); set => _conditionTreeNodes = value; }
        private TreeNode[] _conditionTreeNodes;
    }
}
