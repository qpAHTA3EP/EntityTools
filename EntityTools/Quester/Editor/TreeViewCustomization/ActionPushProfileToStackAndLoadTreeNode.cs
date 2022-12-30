using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infrastructure.Quester;
using Infrastructure.Patches;
using Infrastructure.Reflection;
using EntityTools.Tools;
using MyNW.Classes;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;


namespace EntityTools.Quester.Editor.TreeViewCustomization
{

    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterAction"/>
    /// </summary>
    public sealed class ActionPushProfileToStackAndLoadTreeNode : ActionBaseTreeNode
    {
        private readonly QuesterAction action;

        private static readonly PropertyAccessor<string> DesignProfilePathAccessor =
            ACTP0Serializer.PushProfileToStackAndLoad.GetProperty<string>("DesignProfilePath");
        public ActionPushProfileToStackAndLoadTreeNode(BaseQuesterProfileProxy profile, QuesterAction action, bool clone = false) : base(profile)
        {
            if (!action.IsPushProfileToStackAndLoad())
                throw new ArgumentException($"Action has type '{action.GetType().Name}' instead of the expected type 'PushProfileToStackAndLoad'");
            if (clone)
                action = action.CreateXmlCopy();
            Tag = action;
            this.action = action;
            UpdateView();
        }

        public override QuesterAction Content => action;

        public override bool UseHotSpots => action.UseHotSpots;

        public override List<Vector3> HotSpots => action.HotSpots;

        public override QuesterAction.ActionValidity IsValid => action.IsValid;

        public override bool AllowChildren => false;

        public override void RegenActionID() => action.RegenActionID(); 

        public override void UpdateView()
        {
            DesignProfilePathAccessor[action] = Owner.ProfilePath;

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
            action.InternalReset();
            if (conditionTreeNodes != null)
                action.Conditions = conditionTreeNodes.ToQuesterConditionList();
            return action;
        }

        public override object Clone()
        {
            var newAction = ReconstructInternal().CreateXmlCopy();
            newAction.RegenActionID();
            return new ActionTreeNode(Owner, newAction);
        }

        public override TreeNode[] GetConditionTreeNodes()
        {
            return conditionTreeNodes 
                ?? (conditionTreeNodes = ReconstructInternal().Conditions
                                                              .ToTreeNodes(Owner)
                                                              .ToArray());
        }
    }
}
