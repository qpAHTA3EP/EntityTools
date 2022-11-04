using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using EntityCore.Tools;
using MyNW.Classes;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;


namespace EntityCore.Quester.Editor.TreeViewExtension
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterAction"/>
    /// </summary>
    public sealed class ActionTreeNode : ActionBaseTreeNode
    {
        private readonly QuesterAction action;
        public ActionTreeNode(QuesterProfileProxy profile, QuesterAction action, bool clone = false) : base(profile)
        {
            var act = clone ? CopyHelper.CreateDeepCopy(action) : action;
            Tag = act;
            this.action = act;
            UpdateView();
        }

        public override QuesterAction Content => action;

        public override bool UseHotSpots => action.UseHotSpots;

        public override List<Vector3> HotSpots => action.HotSpots;

        public override QuesterAction.ActionValidity IsValid => action.IsValid;

        public override bool AllowChildren => false;

        public override void NewID() => action.ActionID = Guid.NewGuid();

        public override void UpdateView()
        {
            if (Parent is ActionBaseTreeNode parentPackNode)
                parentPackNode.UpdateView();

            var txt = action.ToString();
            var type = action.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            if (Checked == action.Disabled)
                Checked = !action.Disabled;
            SelectIcon();
        }
        private void SelectIcon()
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

        public override QuesterAction ReconstructInternal()
        {
            action.Disabled = !Checked;
            if (conditionTreeNodes != null)
                action.Conditions = conditionTreeNodes.ToListOf<QuesterCondition>();
            return action;
        }

        public override object Clone()
        {
            var newAction = CopyHelper.CreateDeepCopy(ReconstructInternal());
            newAction.ActionID = Guid.NewGuid();
            return new ActionTreeNode(owner, newAction);
        }

#if false
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
#else
        public override TreeNode[] GetConditionTreeNodes()
        {
            return conditionTreeNodes ?? (conditionTreeNodes = ReconstructInternal().Conditions
                                                                                    .ToTreeNodes()
                                                                                    .ToArray());
        }
#endif
    }
}
