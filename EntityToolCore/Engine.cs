using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using System;
using System.Collections.Generic;
using EntityTools.Quester.Actions;
using EntityCore.Quester.Action;
using EntityTools.Quester.Conditions;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using DevExpress.XtraEditors;
using MyNW.Classes;
using EntityCore.Entities;
using EntityCore.Forms;
using System.Windows.Forms;
using MyNW.Internals;
using EntityCore.Quester.Conditions;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Conditions;
using EntityTools.UCC.Actions;
using EntityCore.UCC.Actions;
using EntityCore.UCC.Conditions;
using UCCConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;
using EntityTools;
using Astral.Logic.NW;
using System.Linq;
using EntityTools.Tools.Extensions;
using AcTp0Tools;
using DevExpress.Utils;
using EntityCore.Tools;
using EntityTools.Forms;
using EntityTools.Quester;
using EntityTools.Tools.Targeting;

namespace EntityCore
{
    public class Engine : IEntityToolsCore
    {
        private static readonly Dictionary<QuesterAction, IQuesterActionEngine> DictQuesterAction = new Dictionary<QuesterAction, IQuesterActionEngine>();
        private static readonly Dictionary<QuesterCondition, IQuesterConditionEngine> DictQuesterCondition = new Dictionary<QuesterCondition, IQuesterConditionEngine>();
        
        //TODO: реализовать сброс dictUccAction и dictUccCondition при загрузке нового UCC-профиля
        private static readonly Dictionary<UCCAction, IUccActionEngine> DictUccAction = new Dictionary<UCCAction, IUccActionEngine>();
        private static readonly Dictionary<UCCCondition, IUccConditionEngine> DictUccCondition = new Dictionary<UCCCondition, IUccConditionEngine>();


        public Engine()
        {
            AstralAccessors.Quester.Core.OnProfileChanged += ResetQuesterCache;
            ETLogger.WriteLine("EntityToolsCore loaded");
        }
        ~Engine()
        {
            AstralAccessors.Quester.Core.OnProfileChanged -= ResetQuesterCache;
        }

        //TODO: ResetQuesterCache не учитывает PullProfileFromStackAndLoad
        private void ResetQuesterCache()
        {
            DictQuesterAction.ForEach(a => a.Value.Dispose());
            DictQuesterAction.Clear();
            DictQuesterCondition.ForEach(a => a.Value.Dispose());
            DictQuesterCondition.Clear();
        }

        public bool CheckCore()
        {
            return true;
        }

        #region Инициализация элементов
        public bool Initialize(object obj)
        {
            try
            {
                switch (obj)
                {
                    case QuesterAction qAction:
                        return Initialize(qAction);
                    case QuesterCondition qCondition:
                        return Initialize(qCondition);
                    case UCCAction uccAction:
                        return Initialize(uccAction);
                    case UCCCondition uccCondition:
                        return Initialize(uccCondition);
                }
            }
            catch { }
            return false;
        }
        public bool Initialize(QuesterAction action)
        {
            try
            {
                if (DictQuesterAction.TryGetValue(action, out IQuesterActionEngine engine))
                    return engine.Rebase(action);

                switch (action)
                {
                    case MoveToEntity m2e:
                        DictQuesterAction.Add(m2e, new MoveToEntityEngine(m2e));
                        return true;
                    case InteractEntities ie:
                        DictQuesterAction.Add(ie, new InteractEntitiesEngine(ie));
                        return true;
                    case PickUpMissionExt pum:
                        DictQuesterAction.Add(pum, new PickUpMissionEngine(pum));
                        return true;
                    case TurnInMissionExt tim:
                        DictQuesterAction.Add(tim, new TurnInMissionEngine(tim));
                        return true;
                    case InsertInsignia ii:
                        DictQuesterAction.Add(ii, new InsertInsigniaEngine(ii));
                        return true;
                    case MoveToTeammate m2t:
                        DictQuesterAction.Add(m2t, new MoveToTeammateEngine(m2t));
                        return true;
                }
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
            return false;
        }
        public bool Initialize(QuesterCondition condition)
        {
            try
            {
                if (DictQuesterCondition.TryGetValue(condition, out IQuesterConditionEngine engine))
                    return engine.Rebase(condition);
                switch (condition)
                {
                    case EntityCount ettCount:
                        DictQuesterCondition.Add(ettCount, new EntityCountEngine(ettCount));
                        return true;
                    case EntityProperty ettProperty:
                        DictQuesterCondition.Add(ettProperty, new EntityPropertyEngine(ettProperty));
                        return true;
                    case TeamMembersCount teamCount:
                        DictQuesterCondition.Add(teamCount, new TeamMembersCountEngine(teamCount));
                        return true;
                    case CheckGameGUI guiCheck:
                        DictQuesterCondition.Add(guiCheck, new CheckGameGuiEngine(guiCheck));
                        return true;
                    case EntityDistance ettDist:
                        DictQuesterCondition.Add(ettDist, new EntityDistanceEngine(ettDist));
                        return true;
                }
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
            return false;
        }

        public bool Initialize(UCCAction action)
        {
            try
            {
                if (DictUccAction.TryGetValue(action, out IUccActionEngine engine))
                    return engine.Rebase(action);
                switch (action)
                {
                    case ApproachEntity ettApproach:
                        DictUccAction.Add(ettApproach, new ApproachEntityEngine(ettApproach));
                        return true;
                    case DodgeFromEntity ettDodge:
                        DictUccAction.Add(ettDodge, new DodgeFromEntityEngine(ettDodge));
                        return true;
                    case ExecuteSpecificPower execPower:
                        DictUccAction.Add(execPower, new ExecuteSpecificPowerEngine(execPower));
                        return true;
                    //case ExecutePowerSmart execPowerSmart:
                    //    DictUccAction.Add(execPowerSmart, new ExecutePowerSmartEngine(execPowerSmart));
                    //    return true;
                    case PluggedSkill artPower:
                        DictUccAction.Add(artPower, new PluggedSkillEngine(artPower));
                        return true;
                    case UseItemSpecial useItem:
                        DictUccAction.Add(useItem, new UseItemSpecialEngine(useItem));
                        return true;
                    case ChangeTarget cht:
                        DictUccAction.Add(cht, new ChangeTargetEngine(cht));
                        return true;
                }
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
            return false;
        }
        public bool Initialize(UCCCondition condition)
        {
            try
            {
                if (DictUccCondition.TryGetValue(condition, out IUccConditionEngine engine))
                    return engine.Rebase(condition);

                switch (condition)
                {
                    case UCCEntityCheck ettCheck:
                        DictUccCondition.Add(ettCheck, new UccEntityCheckEngine(ettCheck));
                        return true;
                    case UCCTargetMatchEntity targMatch:
                        DictUccCondition.Add(targMatch, new UccTargetMatchEntityEngine(targMatch));
                        return true;
                    case UCCGameUICheck uiCheck:
                        DictUccCondition.Add(uiCheck, new UccGameUiCheckEngine(uiCheck));
                        return true;
                }
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
            return false;
        }
        #endregion

#if DEVELOPER
        #region Запрос к пользователю
        public bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayName = "")
        {
            T value = selectedValue;
            if(ItemSelectForm.GetAnItem(source, ref value, displayName))
            {
                selectedValue = value;
                return true;
            }
            return false;
        }

        public bool UserRequest_SelectItemList<T>(Func<IEnumerable<T>> source, ref IList<T> selectedValues, string caption = "")
        {
            var value = selectedValues;
            if (MultipleItemSelectForm.GUIRequest(source, ref value, caption))
            {
                selectedValues = value;
                return true;
            }
            return false;
        }

        public bool UserRequest_EditValue(ref string value, string message = "", string caption = "", FormatInfo formatInfo = null)
        {
            return InputBox.EditValue(ref value, message, caption, formatInfo);
        }

        public bool UserRequest_SelectAuraId(ref string id)
        {
            string newId = AuraSelectForm.GUIRequest();
            if (!string.IsNullOrEmpty(newId))
            {
                id = newId;
                return true;
            }
            return false;
        }

        public bool UserRequest_SelectUIGenId(ref string id)
        {
            string newId = Forms.UIViewer.GUIRequest(id);
            if (!string.IsNullOrEmpty(newId))
            {
                id = newId;
                return true;
            }
            return false;
        }

        public bool UserRequest_EditUccConditions(ref UCCConditionList list)
        {
            UCCConditionList newList = Forms.ConditionListForm.UserRequest(list);
            if (newList != null)
            {
                if (!ReferenceEquals(list, newList))
                    list = newList;
                return true;
            }
            return false;
        }

        public bool UserRequest_EditUccConditions(ref UCCConditionList list, ref LogicRule logic, ref bool negation)
        {
            return Forms.ConditionListForm.UserRequest(ref list, ref logic, ref negation);
        }

        public bool UserRequest_EditEntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
        {
            return EntitySelectForm.GUIRequest(ref entPattern, ref strMatchType, ref nameType) != null;
        }

        public bool UserRequest_EditCustomRegionList(ref List<string> crList)
        {
            //TODO: Исправить ошибку отображения списка CustomRegion (отображается предыдущий список), а также ошибку приведения ListBoxItem к System.String
            if(crList is null)
                crList = new List<string>();

            //Нужно разобраться с селектором.
            //Не работает когда list не пустой.

            if (Astral.Quester.API.CurrentProfile.CustomRegions.Count > 0)
            {
                IEnumerable<string> allCRNames = Astral.Quester.API.CurrentProfile.CustomRegions.Select(cr => cr.Name);

                return MultipleItemSelectForm.GUIRequest(() => allCRNames, ref crList, "Select CustomRegions:");
            }
            XtraMessageBox.Show($"List of the {nameof(Astral.Quester.API.CurrentProfile.CustomRegions)} is empty", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        public bool UserRequest_GetNodeLocation(ref Vector3 pos, string caption)
        {
            while (TargetSelectForm.GUIRequest(caption) == DialogResult.OK)
            {
                if (EntityManager.LocalPlayer.Player.InteractStatus.pMouseOverNode != IntPtr.Zero)
                {
                    var node = EntityManager.LocalPlayer.Player.InteractStatus.PreferredTargetNode;
                    if (node != null && node.IsMouseOver)
                    {
                        pos = node.Location.Clone();
                        return true;
                    }
                    //foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                    //{
                    //    if (targetableNode.IsValid && targetableNode.IsMouseOver)
                    //    {
                    //        pos = targetableNode.WorldInteractionNode.Location.Clone();
                    //        return true;
                    //    }
                    //}
                }
            }
            return false;
        }

        public bool UserRequest_GetEntityToInteract(ref Entity entity)
        {
            while (TargetSelectForm.GUIRequest("Target the NPC and press ok.") == DialogResult.OK)
            {
                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                if (betterEntityToInteract.IsValid)
                {
#if false
                    entity = new NPCInfos()
                    {
                        CostumeName = betterEntityToInteract.CostumeRef.CostumeName,
                        DisplayName = betterEntityToInteract.Name,
                        Position = betterEntityToInteract.Location.Clone(),
                        MapName = EntityManager.LocalPlayer.MapState.MapName,
                        RegionName = EntityManager.LocalPlayer.RegionInternalName
                    }; 
#endif
                    entity = betterEntityToInteract;
                    return true;
                }
            }
            entity = null;
            return false;
        }

        
        public bool UserRequest_GetUccAction(out UCCAction action)
        {
            return Forms.AddUccActionForm.GUIRequest(out action);
        }
        #endregion

        public string EntityDiagnosticInfos(object obj)
        {
            if (obj == null)
                return "Object is empty";
            
            switch (obj)
            {
                case IEntityDescriptor entityDescriptor:
                    return EntityDiagnosticTools.Construct(entityDescriptor);
                case IEntityIdentifier entityIdentifier:
                    return EntityDiagnosticTools.Construct(entityIdentifier);
            }

            return "Unable recognize test context!";
        }

        public void Monitor(object monitor)
        {
            switch (monitor)
            {
                case PlayerTeamMonitor team:
                    new ObjectInfoForm().Show(new PlayerTeamHelper.Monitor(), 500);
                    break;
                default:
                    new ObjectInfoForm().Show(monitor, 500);
                    break;
            }
            
        }
#endif
#if DEBUG
        public LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null)
        {
            return SearchCached.FindAllEntity(pattern, matchType, nameType, setType, healthCheck, range, zRange, regionCheck, customRegions, specialCheck);
        }

#endif
        public void Dispose()
        {
            DictQuesterAction.ForEach(a => a.Value.Dispose());
            DictQuesterAction.Clear();
            DictQuesterCondition.ForEach(a => a.Value.Dispose());
            DictQuesterCondition.Clear();
            DictUccAction.ForEach(a => a.Value.Dispose());
            DictUccAction.Clear();
            DictUccCondition.ForEach(a => a.Value.Dispose());
            DictUccCondition.Clear();
        }
    }
}
