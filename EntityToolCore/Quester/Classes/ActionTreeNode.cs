using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Quester.Classes;
using EntityCore.Tools;
using QuesterAction = Astral.Quester.Classes.Action;


namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterAction"/>
    /// </summary>
    public class ActionTreeNode : TreeNode, IActionTreeNode
    {
        public bool AllowChildren => false;

        public ActionTreeNode(QuesterAction action, bool clone = false)
        {
            var act = clone ? CopyHelper.CreateDeepCopy(action) : action;
            Tag = act;
            SelectIcon(act);
            var txt = action.ToString();
            var type = action.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            Checked = !act.Disabled;

            _conditionTreeNodes = act.Conditions.ToTreeNodes().ToArray();
        }

        private void SelectIcon(QuesterAction action)
        {
            var actionIsValid = action.IsValid;
            if (actionIsValid.IsValid)
            {
                ImageKey = "Cube";
                SelectedImageKey = "Cube";
                ToolTipText = string.Empty;
#if false
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
#endif
            }
            else
            {
                ImageKey = "CubeRed";
                SelectedImageKey = "CubeRed";
                ToolTipText = actionIsValid.Message;
            }
        }
        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is QuesterAction action)
            {
                var txt = action.ToString();
                var type = action.GetType();
                if (string.IsNullOrEmpty(txt)
                    || txt == type.FullName)
                    txt = type.Name;
                Text = txt;
                Checked = !action.Disabled;
                SelectIcon(action);
            }
            else throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(QuesterAction).FullName}");
        }

        public QuesterAction ReconstructInternal()
        {
            if (Tag is QuesterAction action)
            {
                if(_conditionTreeNodes != null)
                    action.Conditions = _conditionTreeNodes.ToQuesterConditionList();
                return action;
            }
            throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(QuesterAction).FullName}");
        }

        public override object Clone()
        {
            var newAction = CopyHelper.CreateDeepCopy(ReconstructInternal());
            newAction.ActionID = Guid.NewGuid();
            return new ActionTreeNode(newAction);
        }

        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="QuesterAction"/>
        /// </summary>
        public TreeNode[] ConditionTreeNodes
        {
            get => _conditionTreeNodes ?? (_conditionTreeNodes = ReconstructInternal().Conditions
                                                                                      .ToTreeNodes()
                                                                                      .ToArray()); 
            set => _conditionTreeNodes = value;
        }
        private TreeNode[] _conditionTreeNodes;
    }
}
