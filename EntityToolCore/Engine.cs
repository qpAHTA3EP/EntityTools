using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using EntityTools.Quester.Actions;
using EntityCore.Quester.Action;
using EntityTools.Quester.Conditions;
using EntityTools.Tools;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using MyNW.Classes;
using EntityCore.Entities;
using EntityCore.Extensions;
using EntityCore.Forms;
using System.Windows.Forms;
using MyNW.Internals;
using EntityTools.Reflection;
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

namespace EntityCore
{
    public class Engine : IEntityToolsCore
    {
        //TODO: Исправить ошибку восстановления статуса после PullProfileFromStack, которая проявилась в Lliiras_Night_DartKotik.amp.zip
        //TODO: Очищать словари при загрузке нового Quester- и UCC-профилей

        internal static Dictionary<QuesterAction, IQuesterActionEngine> dictQuesterAction = new Dictionary<QuesterAction, IQuesterActionEngine>();
        internal static Dictionary<QuesterCondition, IQuesterConditionEngine> dictQuesterCondition = new Dictionary<QuesterCondition, IQuesterConditionEngine>();
        internal static Dictionary<UCCAction, IUccActionEngine> dictUccAction = new Dictionary<UCCAction, IUccActionEngine>();
        internal static Dictionary<UCCCondition, IUccConditionEngine> dictUccCondition = new Dictionary<UCCCondition, IUccConditionEngine>();

#if false
        public Engine()
        {
            ETLogger.WriteLine("EntityToolsCore loaded");
        }
#else
        public Engine()
        {
            AstralAccessors.Quester.Core.AfterLoad += ResetQuesterCache;
            AstralAccessors.Quester.Core.AfterNew += ResetQuesterCache;
            ETLogger.WriteLine("EntityToolsCore loaded");
        }
        ~Engine()
        {
            AstralAccessors.Quester.Core.AfterLoad -= ResetQuesterCache;
            AstralAccessors.Quester.Core.AfterNew -= ResetQuesterCache;
        }

        private void ResetQuesterCache()
        {
            dictQuesterAction.ForEach(a => a.Value.Dispose());
            dictQuesterAction.Clear();
            dictQuesterCondition.ForEach(a => a.Value.Dispose());
            dictQuesterCondition.Clear();
        }

        private void ResetQuesterCache(string path)
        {
            ResetQuesterCache();
        } 
#endif

        public bool CheckCore()
        {
            return true;
        }

        #region Инициализация элементов
        public bool Initialize(object obj)
        {
            try
            {
                if (obj is QuesterAction qAction)
                    return Initialize(qAction);
                if (obj is QuesterCondition qCondition)
                    return Initialize(qCondition);
                if (obj is UCCAction uccAction)
                    return Initialize(uccAction);
                if (obj is UCCCondition uccCondition)
                    return Initialize(uccCondition);
            }
            catch { }
            return false;
        }
        public bool Initialize(QuesterAction action)
        {
            try
            {
#if false
                if (action is MoveToEntity m2e)
                {
                    if (dictQuesterAction.TryGetValue(m2e, out IQuesterActionEngine engine))
                    {
                        if (engine is MoveToEntityEngine m2eEndg)
                            m2e.Engine = m2eEndg;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(MoveToEntityEngine) + '\''));
                            dictQuesterAction[m2e] = new MoveToEntityEngine(m2e);
                        }
                    }
                    else dictQuesterAction.Add(m2e, new MoveToEntityEngine(m2e));
                    return true;
                }

                if (action is InteractEntities ie)
                {
                    if (dictQuesterAction.TryGetValue(ie, out IQuesterActionEngine engine))
                    {
                        if (engine is InteractEntitiesEngine ieEndg)
                            ie.Engine = ieEndg;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(InteractEntitiesEngine) + '\''));
                            dictQuesterAction[ie] = new InteractEntitiesEngine(ie);
                        }
                    }
                    else dictQuesterAction.Add(ie, new InteractEntitiesEngine(ie));
                    return true;
                }

                if (action is PickUpMissionExt pum)
                {
                    if (dictQuesterAction.TryGetValue(pum, out IQuesterActionEngine engine))
                    {
                        if (engine is PickUpMissionEngine pumEndg)
                            pum.Engine = pumEndg;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(PickUpMissionEngine) + '\''));
                            dictQuesterAction[pum] = new PickUpMissionEngine(pum);
                        }
                    }
                    else dictQuesterAction.Add(pum, new PickUpMissionEngine(pum));
                    return true;
                }

                if (action is TurnInMissionExt tim)
                {
                    if (dictQuesterAction.TryGetValue(tim, out IQuesterActionEngine engine))
                    {
                        if (engine is TurnInMissionEngine timEndg)
                            tim.Engine = timEndg;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(TurnInMissionEngine) + '\''));
                            dictQuesterAction[tim] = new TurnInMissionEngine(tim);
                        }
                    }
                    else dictQuesterAction.Add(tim, new TurnInMissionEngine(tim));
                    return true;
                }

                if (action is InsertInsignia ii)
                {
                    if (dictQuesterAction.TryGetValue(ii, out IQuesterActionEngine engine))
                    {
                        if (engine is InsertInsigniaEngine iie)
                            ii.Engine = iie;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(InsertInsigniaEngine) + '\''));
                            dictQuesterAction[ii] = new InsertInsigniaEngine(ii);
                        }
                    }
                    else dictQuesterAction.Add(ii, new InsertInsigniaEngine(ii));
                    return true;
                } 
#else
                if (dictQuesterAction.TryGetValue(action, out IQuesterActionEngine engine))
                    return engine.Rebase(action);
                else
                {
                    if (action is MoveToEntity m2e)
                    {
                        dictQuesterAction.Add(m2e, new MoveToEntityEngine(m2e));
                        return true;
                    }
                    else if (action is InteractEntities ie)
                    {
                        dictQuesterAction.Add(ie, new InteractEntitiesEngine(ie));
                        return true;
                    }
                    else if (action is PickUpMissionExt pum)
                    {
                        dictQuesterAction.Add(pum, new PickUpMissionEngine(pum));
                        return true;
                    }
                    else if (action is TurnInMissionExt tim)
                    {
                        dictQuesterAction.Add(tim, new TurnInMissionEngine(tim));
                        return true;
                    }
                    else if (action is InsertInsignia ii)
                    {
                        dictQuesterAction.Add(ii, new InsertInsigniaEngine(ii));
                        return true;
                    }
                }
#endif
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
#if false
                if (condition is EntityCount ettCount)
                {
                    if (dictQuesterCondition.TryGetValue(ettCount, out IQuesterConditionEngine engine))
                    {
                        if (engine is EntityCountEngine ettCountEngine)
                            ettCount.Engine = ettCountEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(EntityCountEngine) + '\''));
                            dictQuesterCondition[ettCount] = new EntityCountEngine(ettCount);
                        }
                    }
                    else dictQuesterCondition.Add(ettCount, new EntityCountEngine(ettCount));
                    return true;
                }
                if (condition is EntityProperty ettProperty)
                {
                    if (dictQuesterCondition.TryGetValue(ettProperty, out IQuesterConditionEngine engine))
                    {
                        if (engine is EntityPropertyEngine ettPropertyEngine)
                            ettProperty.Engine = ettPropertyEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(EntityPropertyEngine) + '\''));
                            dictQuesterCondition[ettProperty] = new EntityPropertyEngine(ettProperty);
                        }
                    }
                    else dictQuesterCondition.Add(ettProperty, new EntityPropertyEngine(ettProperty));
                    return true;
                }
                if (condition is TeamMembersCount teamCount)
                {
                    if (dictQuesterCondition.TryGetValue(teamCount, out IQuesterConditionEngine engine))
                    {
                        if (engine is EntityPropertyEngine teamCountEngine)
                            teamCount.Engine = teamCountEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(TeamMembersCountEngine) + '\''));
                            dictQuesterCondition[teamCount] = new TeamMembersCountEngine(teamCount);
                        }
                    }
                    else dictQuesterCondition.Add(teamCount, new TeamMembersCountEngine(teamCount));
                    return true;
                }
                if (condition is CheckGameGUI guiCheck)
                {
                    if (dictQuesterCondition.TryGetValue(guiCheck, out IQuesterConditionEngine engine))
                    {
                        if (engine is CheckGameGUIEngine guiCheckEngine)
                            guiCheck.Engine = guiCheckEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(CheckGameGUIEngine) + '\''));
                            dictQuesterCondition[guiCheck] = new CheckGameGUIEngine(guiCheck);
                        }
                    }
                    else dictQuesterCondition.Add(guiCheck, new CheckGameGUIEngine(guiCheck));
                    return true;
                }
                if (condition is EntityDistance ettDist)
                {
                    if (dictQuesterCondition.TryGetValue(ettDist, out IQuesterConditionEngine engine))
                    {
                        if (engine is EntityDistanceEngine ettDistEngine)
                            ettDist.Engine = ettDistEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(EntityDistanceEngine) + '\''));
                            dictQuesterCondition[ettDist] = new EntityDistanceEngine(ettDist);
                        }
                    }
                    else dictQuesterCondition.Add(ettDist, new EntityDistanceEngine(ettDist));
                    return true;
                } 
#else
                if (dictQuesterCondition.TryGetValue(condition, out IQuesterConditionEngine engine))
                    return engine.Rebase(condition);
                else
                {
                    if (condition is EntityCount ettCount)
                    {
                        dictQuesterCondition.Add(ettCount, new EntityCountEngine(ettCount));
                        return true;
                    }
                    if (condition is EntityProperty ettProperty)
                    {
                        dictQuesterCondition.Add(ettProperty, new EntityPropertyEngine(ettProperty));
                        return true;
                    }
                    if (condition is TeamMembersCount teamCount)
                    {
                        dictQuesterCondition.Add(teamCount, new TeamMembersCountEngine(teamCount));
                        return true;
                    }
                    if (condition is CheckGameGUI guiCheck)
                    {
                        dictQuesterCondition.Add(guiCheck, new CheckGameGuiEngine(guiCheck));
                        return true;
                    }
                    if (condition is EntityDistance ettDist)
                    {
                        dictQuesterCondition.Add(ettDist, new EntityDistanceEngine(ettDist));
                        return true;
                    }
                }
#endif
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
#if false
                if (action is ApproachEntity ettApproach)
                {
                    if (dictUccAction.TryGetValue(ettApproach, out IUCCActionEngine engine))
                    {
                        if (engine is ApproachEntityEngine ettApproachEngine)
                            ettApproach.Engine = ettApproachEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(ApproachEntityEngine) + '\''));
                            dictUccAction[ettApproach] = new ApproachEntityEngine(ettApproach);
                        }
                    }
                    else dictUccAction.Add(ettApproach, new ApproachEntityEngine(ettApproach));
                    return true;
                }
                if (action is DodgeFromEntity ettDodge)
                {
                    if (dictUccAction.TryGetValue(ettDodge, out IUCCActionEngine engine))
                    {
                        if (engine is DodgeFromEntityEngine ettDodgeEngine)
                            ettDodge.Engine = ettDodgeEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(DodgeFromEntityEngine) + '\''));
                            dictUccAction[ettDodge] = new DodgeFromEntityEngine(ettDodge);
                        }
                    }
                    else dictUccAction.Add(ettDodge, new DodgeFromEntityEngine(ettDodge));
                    return true;
                }
                if (action is ExecuteSpecificPower execPower)
                {
                    if (dictUccAction.TryGetValue(execPower, out IUCCActionEngine engine))
                    {
                        if (engine is ExecuteSpecificPowerEngine execPowerEngine)
                            execPower.Engine = execPowerEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(ExecuteSpecificPowerEngine) + '\''));
                            dictUccAction[execPower] = new ExecuteSpecificPowerEngine(execPower);
                        }
                    }
                    else dictUccAction.Add(execPower, new ExecuteSpecificPowerEngine(execPower));
                    return true;
                }
                if (action is UseItemSpecial useItem)
                {
                    if (dictUccAction.TryGetValue(useItem, out IUCCActionEngine engine))
                    {
                        if (engine is UseItemSpecialEngine useItemEngine)
                            useItem.Engine = useItemEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(UseItemSpecialEngine) + '\''));
                            dictUccAction[useItem] = new UseItemSpecialEngine(useItem);
                        }
                    }
                    else dictUccAction.Add(useItem, new UseItemSpecialEngine(useItem));
                    return true;
                } 
#else
                if (dictUccAction.TryGetValue(action, out IUccActionEngine engine))
                    return engine.Rebase(action);
                else
                {
                    if (action is ApproachEntity ettApproach)
                    {
                        dictUccAction.Add(ettApproach, new ApproachEntityEngine(ettApproach));
                        return true;
                    }
                    if (action is DodgeFromEntity ettDodge)
                    {
                        dictUccAction.Add(ettDodge, new DodgeFromEntityEngine(ettDodge));
                        return true;
                    }
                    if (action is ExecuteSpecificPower execPower)
                    {
                        dictUccAction.Add(execPower, new ExecuteSpecificPowerEngine(execPower));
                        return true;
                    }
                    if (action is UseItemSpecial useItem)
                    {
                        dictUccAction.Add(useItem, new UseItemSpecialEngine(useItem));
                        return true;
                    }
                }
#endif
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
#if false
                if (condition is UCCEntityCheck ettCheck)
                {
                    if (dictUccCondition.TryGetValue(ettCheck, out IUCCConditionEngine engine))
                    {
                        if (engine is UCCEntityCheckEngine ettCheckEngine)
                            ettCheck.Engine = ettCheckEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(UCCEntityCheckEngine) + '\''));
                            dictUccCondition[ettCheck] = new UCCEntityCheckEngine(ettCheck);
                        }
                    }
                    else dictUccCondition.Add(ettCheck, new UCCEntityCheckEngine(ettCheck));
                    return true;
                }
                if (condition is UCCTargetMatchEntity targMatch)
                {
                    if (dictUccCondition.TryGetValue(targMatch, out IUCCConditionEngine engine))
                    {
                        if (engine is UCCTargetMatchEntityEngine targMatchEngine)
                            targMatch.Engine = targMatchEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(UCCTargetMatchEntityEngine) + '\''));
                            dictUccCondition[targMatch] = new UCCTargetMatchEntityEngine(targMatch);
                        }
                    }
                    else dictUccCondition.Add(targMatch, new UCCTargetMatchEntityEngine(targMatch));
                    return true;
                }
                if (condition is UCCGameUICheck uiCheck)
                {
                    if (dictUccCondition.TryGetValue(uiCheck, out IUCCConditionEngine engine))
                    {
                        if (engine is UCCGameUICheckEngine uiCheckEngine)
                            uiCheck.Engine = uiCheckEngine;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(UCCGameUICheckEngine) + '\''));
                            dictUccCondition[uiCheck] = new UCCGameUICheckEngine(uiCheck);
                        }
                    }
                    else dictUccCondition.Add(uiCheck, new UCCGameUICheckEngine(uiCheck));
                    return true;
                } 
#else
                if (dictUccCondition.TryGetValue(condition, out IUccConditionEngine engine))
                    return engine.Rebase(condition);
                {
                    if (condition is UCCEntityCheck ettCheck)
                    {
                        dictUccCondition.Add(ettCheck, new UccEntityCheckEngine(ettCheck));
                        return true;
                    }
                    if (condition is UCCTargetMatchEntity targMatch)
                    {
                        dictUccCondition.Add(targMatch, new UccTargetMatchEntityEngine(targMatch));
                        return true;
                    }
                    if (condition is UCCGameUICheck uiCheck)
                    {
                        dictUccCondition.Add(uiCheck, new UccGameUiCheckEngine(uiCheck));
                        return true;
                    }
                }
#endif
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
        public bool GUIRequest_Item<T>(Func<IEnumerable<T>> source, ref T selectedValue)
        {
            T value = selectedValue;
            if(ItemSelectForm.GetAnItem(source, ref value))
            {
                selectedValue = value;
                return true;
            }
            return false;
        }

        public bool GUIRequest_AuraId(ref string id)
        {
            string newId = Forms.AuraSelectForm.GUIRequest();
            if (!string.IsNullOrEmpty(newId))
            {
                id = newId;
                return true;
            }
            return false;
        }

        public bool GUIRequest_UIGenId(ref string id)
        {
            string newId = Forms.UIViewer.GUIRequest(id);
            if (!string.IsNullOrEmpty(newId))
            {
                id = newId;
                return true;
            }
            return false;
        }

        public bool GUIRequest_UCCConditions(ref UCCConditionList list)
        {
            UCCConditionList newList = Forms.ConditionListForm.GUIRequest(list);
            if (newList != null)
            {
                if (!ReferenceEquals(list, newList))
                    list = newList;
                return true;
            }
            return false;
        }

        public bool GUIRequest_EntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
        {
            return Forms.EntitySelectForm.GUIRequest(ref entPattern, ref strMatchType, ref nameType) != null;
        }

        public bool GUIRequest_CustomRegions(ref List<string> crList)
        {
            //TODO: Исправить ошибку отображения списка CustomRegion (отображается предыдущий список), а также ошибку приведения ListBoxItem к System.String
            if(crList is null)
                crList = new List<string>();

#if disabled_20200510_0025
            if (MultiItemSelectForm.GUIRequest("Select CustomRegions:",
            (DataGridView dgv) => CustomRegionExtentions.CustomRegionList2DataGridView(list, dgv),
            (DataGridView dgv) => CustomRegionExtentions.DataGridView2CustomRegionList(dgv, ref list)))
            {
                crList = list;
                return true;
            }
            return false; 
#else
            //Нужно разобраться с селектором.
            //Не работает когда list не пустой.

            if (Astral.Quester.API.CurrentProfile.CustomRegions.Count > 0)
            {
                IEnumerable<string> allCRNames = Astral.Quester.API.CurrentProfile.CustomRegions.Select(cr => cr.Name);

                return MultiItemSelectForm.GUIRequest("Select CustomRegions:", () => allCRNames, ref crList);
            }
            else XtraMessageBox.Show($"List of the {nameof(Astral.Quester.API.CurrentProfile.CustomRegions)} is empty", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
#endif
        }

        public bool GUIRequest_NodeLocation(ref Vector3 pos, string caption)
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

        public bool GUIRequest_EntityToInteract(ref Entity entity)
        {
            while (TargetSelectForm.GUIRequest("Target the Traider and press ok.") == DialogResult.OK)
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

        
        public bool GUIRequest_UCCAction(out UCCAction action)
        {
            return Forms.AddUccActionForm.GUIRequest(out action);
        }
        #endregion

#if false
        public string EntityDiagnosticInfos(object obj)
        {
            if (obj != null)
            {
                StringBuilder sb = new StringBuilder();

                IEntityInfos ettInfos = null;

                if (obj is QuesterAction qa
                    && dictQuesterAction.TryGetValue(qa, out IQuesterActionEngine qaEngine))
                    ettInfos = qaEngine as IEntityInfos;
                else if (obj is QuesterCondition qc
                    && dictQuesterCondition.TryGetValue(qc, out IQuesterConditionEngine qcEngine))
                    ettInfos = qcEngine as IEntityInfos;
                else if (obj is UCCCondition uccCond
                    && dictUccCondition.TryGetValue(uccCond, out IUccConditionEngine uccCondEngine))
                    ettInfos = uccCondEngine as IEntityInfos;
                else if (obj is UCCAction uccAct
                    && dictUccAction.TryGetValue(uccAct, out IUccActionEngine uccActEngine))
                    ettInfos = uccActEngine as IEntityInfos;

                if (ettInfos != null)
                {
                    sb.Append(ettInfos.EntityDiagnosticString(out string infos)
                        ? infos
                        : "DiagnosticString formatting error");
                }
                else if (ReflectionHelper.GetPropertyValue(obj, "EntityID", out object entityId)
                        && ReflectionHelper.GetPropertyValue(obj, "EntityNameType", out object entityNameType)
                        && ReflectionHelper.GetPropertyValue(obj, "EntityIdType", out object entityIdType))
                {
                    sb.Append("EntityID: ").AppendLine(entityId.ToString());

                    EntitySetType entitySet = ReflectionHelper.GetPropertyValue(obj, "EntitySetType", out object entitySetObj) ? (EntitySetType)entitySetObj : EntitySetType.Complete;
                    bool regionCheck = ReflectionHelper.GetPropertyValue(obj, "RegionCheck", out object regionCheckObj) ? (bool)regionCheckObj : false;
                    bool healthCheck = ReflectionHelper.GetPropertyValue(obj, "HealthCheck", out object healthCheckObj) ? (bool)healthCheckObj : false;
                    float reactionRange = ReflectionHelper.GetPropertyValue(obj, "ReactionRange", out object reactionRangeObj) ? (float)reactionRangeObj : 0;
                    float reactionZRange = ReflectionHelper.GetPropertyValue(obj, "ReactionZRange", out object reactionZRangeObj) ? (float)reactionZRangeObj : 0;
                    List<CustomRegion> customRegions = ReflectionHelper.GetFieldValue(obj, "customRegions", out object customRegionsObj) ? customRegionsObj as List<CustomRegion> : null;
                    AuraOption auraOption = ReflectionHelper.GetPropertyValue(obj, "Aura", out object auraOptionObj) ? auraOptionObj as AuraOption : new AuraOption();

                    sb.Append("EntityIdType: ").AppendLine(entityNameType.ToString());
                    sb.Append("EntityNameType: ").AppendLine(entityIdType.ToString());
                    sb.Append("EntitySetType: ").AppendLine(entitySetObj?.ToString() ?? "null");
                    sb.Append("HealthCheck: ").AppendLine(healthCheckObj?.ToString() ?? "null");
                    sb.Append("ReactionRange: ").AppendLine(reactionRangeObj?.ToString() ?? "null");
                    sb.Append("ReactionZRange: ").AppendLine(reactionZRangeObj?.ToString() ?? "null");
                    sb.Append("RegionCheck: ").AppendLine(regionCheckObj?.ToString() ?? "null");
                    if (customRegions != null && customRegions.Count > 0)
                    {
                        sb.Append("RegionCheck: {").Append(customRegions[0].Name);
                        for (int i = 1; i < customRegions.Count; i++)
                            sb.Append(", ").Append(customRegions[i].Name);
                        sb.AppendLine("}");
                    }
                    sb.Append("Aura: ").AppendLine(auraOptionObj?.ToString() ?? "null");

                    sb.AppendLine();
                    LinkedList<Entity> entities = SearchCached.FindAllEntity(entityId.ToString(), (ItemFilterStringType)entityIdType, (EntityNameType)entityNameType, entitySet,
                        healthCheck, reactionRange, reactionZRange, regionCheck, customRegions, auraOption.IsMatch);

                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    // Ближайшее Entity
                    Entity target = SearchCached.FindClosestEntity(entityId.ToString(), (ItemFilterStringType)entityIdType,
                                            (EntityNameType)entityNameType, entitySet, false /*healthCheck*/, 0 /*reactionRange*/, 0/*reactionZRange*/, regionCheck, customRegions);
                    if (target != null && target.IsValid)
                    {
#if false
                        sb.Append("ClosectEntity: ").AppendLine(target.ToString());
                        sb.Append("\tName: ").AppendLine(target.Name);
                        sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                        sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
                        sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                        sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString()); 
#else
                        bool distOk = reactionRange <= 0 || target.Location.Distance3DFromPlayer < reactionRange;
                        bool zOk = reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(target.Location) < reactionZRange;
                        bool alive = !healthCheck || !target.IsDead;
                        sb.Append("ClosestEntity: ").Append(target.ToString());
                        if (distOk && zOk && alive)
                            sb.AppendLine(" [MATCH]");
                        else sb.AppendLine(" [MISMATCH]");
                        sb.Append("\tName: ").AppendLine(target.Name);
                        sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                        sb.Append("\tIsDead: ").Append(target.IsDead.ToString());
                        if (alive)
                            sb.AppendLine(" [OK]");
                        else sb.AppendLine(" [FAIL]");
                        sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                        sb.Append("\tDistance: ").Append(target.Location.Distance3DFromPlayer.ToString());
                        if (distOk)
                            sb.AppendLine(" [OK]");
                        else sb.AppendLine(" [FAIL]");
                        sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
                        if (zOk)
                            sb.AppendLine(" [OK]");
                        else sb.AppendLine(" [FAIL]");
#endif
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else sb.Append("Unable recognize test context!");

                Task.Factory.StartNew(() => XtraMessageBox.Show(/*Form.ActiveForm, */sb.ToString(), "Test of '" + obj.ToString() + '\''));
            }

            return string.Empty;
        } 
#else
        public string EntityDiagnosticInfos(object obj)
        {
            if (obj != null)
            {
                if (obj is IEntityDescriptor entityDescriptor)
                    return EntityDiagnosticTools.Construct(entityDescriptor);
                else if(obj is IEntityIdentifier entityIdentifier)
                    return EntityDiagnosticTools.Construct(entityIdentifier);
            }

            return "Unable recognize test context!";
        }
#endif
#endif
#if DEBUG
        public LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null)
        {
            return Entities.SearchCached.FindAllEntity(pattern, matchType, nameType, setType, healthCheck, range, zRange, regionCheck, customRegions, specialCheck);
        }

#endif
        public void Dispose()
        {
            dictQuesterAction.ForEach(a => a.Value.Dispose());
            dictQuesterCondition.ForEach(a => a.Value.Dispose());
            dictUccAction.ForEach(a => a.Value.Dispose());
            dictUccCondition.ForEach(a => a.Value.Dispose());
        }
    }
}
