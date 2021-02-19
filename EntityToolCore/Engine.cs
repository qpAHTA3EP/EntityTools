﻿using Astral.Logic.UCC.Classes;
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

namespace EntityCore
{
    public class Engine : IEntityToolsCore
    {
        //TODO: Исправить ошибку восстановления статуса после PullProfileFromStack, которая проявилась в Lliiras_Night_DartKotik.amp.zip

        //internal static Dictionary<object, object> dictionary = new Dictionary<object, object>();

        internal static Dictionary<QuesterAction, IQuesterActionEngine> dictQuesterAction = new Dictionary<QuesterAction, IQuesterActionEngine>();
        internal static Dictionary<QuesterCondition, IQuesterConditionEngine> dictQuesterCondition = new Dictionary<QuesterCondition, IQuesterConditionEngine>();
        internal static Dictionary<UCCAction, IUCCActionEngine> dictUccAction = new Dictionary<UCCAction, IUCCActionEngine>();
        internal static Dictionary<UCCCondition, IUCCConditionEngine> dictUccCondition = new Dictionary<UCCCondition, IUCCConditionEngine>();

        public Engine()
        {
            ETLogger.WriteLine("EntityToolsCore loaded");
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
                if (action is MoveToEntity m2e)
                {
#if false
                    if (!dictionary.ContainsKey(m2e))
                        dictionary.Add(m2e, new MoveToEntityEngine(m2e));
                    else if (dictionary[m2e] is MoveToEntityEngine m2ee)
                    {
                        m2e.Engine = m2ee;
                        return true;
                    } 
#else
                    if (dictQuesterAction.TryGetValue(m2e, out IQuesterActionEngine engine))
                    {
                        if (engine is MoveToEntityEngine m2ee)
                            m2e.Engine = m2ee;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(MoveToEntityEngine) + '\''));
                            dictQuesterAction[m2e] = new MoveToEntityEngine(m2e);
                        }
                    }
                    else dictQuesterAction.Add(m2e, new MoveToEntityEngine(m2e));
                    return true;
#endif
                }

                if (action is InteractEntities ie)
                {
#if false
                    if (!dictionary.ContainsKey(ie))
                        dictionary.Add(ie, new InteractEntitiesEngine(ie));
                    else if (dictionary[ie] is InteractEntitiesEngine iee)
                    {
                        ie.ActionEngine = iee;
                        return true;
                    } 
#else
                    if (dictQuesterAction.TryGetValue(ie, out IQuesterActionEngine engine))
                    {
                        if (engine is InteractEntitiesEngine iee)
                            ie.Engine = iee;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(InteractEntitiesEngine) + '\''));
                            dictQuesterAction[ie] = new InteractEntitiesEngine(ie);
                        }
                    }
                    else dictQuesterAction.Add(ie, new InteractEntitiesEngine(ie));
                    return true;
#endif
                }

                if (action is PickUpMissionExt pum)
                {
#if false
                    if (!dictionary.ContainsKey(pum))
                        dictionary.Add(pum, new PickUpMissionEngine(pum));
                    else if (dictionary[pum] is PickUpMissionEngine pume)
                    {
                        pum.ActionEngine = pume;
                        return true;
                    } 
#else
                    if (dictQuesterAction.TryGetValue(pum, out IQuesterActionEngine engine))
                    {
                        if (engine is PickUpMissionEngine pume)
                            pum.Engine = pume;
                        else
                        {
                            ETLogger.WriteLine(string.Concat("Invalid cast type '", engine.GetType().Name, "' to type '" + nameof(PickUpMissionEngine) + '\''));
                            dictQuesterAction[pum] = new PickUpMissionEngine(pum);
                        }
                    }
                    else dictQuesterAction.Add(pum, new PickUpMissionEngine(pum));
                        return true;
#endif
                }

                if (action is InsertInsignia ii)
                {
#if false
                    if (!dictionary.ContainsKey(ii))
                        dictionary.Add(ii, new InsertInsigniaEngine(ii));
                    else if (dictionary[ii] is InsertInsigniaEngine iie)
                    {
                        ii.ActionEngine = iie;
                        return true;
                    } 
#else
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
#endif
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
                if (condition is EntityCount ettCount)
                {
#if false
                    if (!dictionary.ContainsKey(ettCount))
                        dictionary.Add(ettCount, new EntityCountEngine(ettCount));
                    else if (dictionary[ettCount] is EntityCountEngine ettCountEngine)
                    {
                        ettCount.Engine = ettCountEngine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is EntityProperty ettProperty)
                {
#if false
                    if (!dictionary.ContainsKey(ettProperty))
                        dictionary.Add(ettProperty, new EntityPropertyEngine(ettProperty));
                    else if (dictionary[ettProperty] is EntityPropertyEngine ettPropertyEngine)
                    {
                        ettProperty.Engine = ettPropertyEngine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is TeamMembersCount teamCount)
                {
#if false
                    if (!dictionary.ContainsKey(teamCount))
                        dictionary.Add(teamCount, new TeamMembersCountEngine(teamCount));
                    else if (dictionary[teamCount] is TeamMembersCountEngine teamCountEngine)
                    {
                        teamCount.ConditionEngine = teamCountEngine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is CheckGameGUI guiCheck)
                {
#if false
                    if (!dictionary.ContainsKey(guiCheck))
                        dictionary.Add(guiCheck, new CheckGameGUIEngine(guiCheck));
                    else if (dictionary[guiCheck] is CheckGameGUIEngine guiCheckEngine)
                    {
                        guiCheck.ConditionEngine = guiCheckEngine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is EntityDistance ettDist)
                {
#if false
                    if (!dictionary.ContainsKey(ettDist))
                        dictionary.Add(ettDist, new EntityDistanceEngine(ettDist));
                    else if (dictionary[ettDist] is EntityDistanceEngine ettDistEngine)
                    {
                        ettDist.Engine = ettDistEngine;
                        return true;
                    } 
#else
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
#endif
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
                if (action is ApproachEntity ettApproach)
                {
#if false
                    if (!dictionary.ContainsKey(ettApproach))
                        dictionary.Add(ettApproach, new ApproachEntityEngine(ettApproach));
                    else if (dictionary[ettApproach] is ApproachEntityEngine ettApproachEngine)
                    {
                        ettApproach.Engine = ettApproachEngine;
                        return true;
                    } 
#else
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
#endif
                }
                if (action is DodgeFromEntity ettDodge)
                {
#if false
                    if (!dictionary.ContainsKey(ettDodge))
                        dictionary.Add(ettDodge, new DodgeFromEntityEngine(ettDodge));
                    else if (dictionary[ettDodge] is DodgeFromEntityEngine engine)
                    {
                        ettDodge.Engine = engine;
                        return true;
                    } 
#else
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
#endif
                }
                if (action is ExecuteSpecificPower execPower)
                {
#if false
                    if (!dictionary.ContainsKey(execPower))
                        dictionary.Add(execPower, new ExecuteSpecificPowerEngine(execPower));
                    else if (dictionary[execPower] is ExecuteSpecificPowerEngine engine)
                    {
                        execPower.Engine = engine;
                        return true;
                    } 
#else
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
#endif
                }
                if (action is UseItemSpecial useItem)
                {
#if false
                    if (!dictionary.ContainsKey(useItem))
                        dictionary.Add(useItem, new UseItemSpecialEngine(useItem));
                    else if (dictionary[useItem] is UseItemSpecialEngine engine)
                    {
                        useItem.Engine = engine;
                        return true;
                    } 
#else
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
#endif
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
                if (condition is UCCEntityCheck ettCheck)
                {
#if false
                    if (!dictionary.ContainsKey(ettCheck))
                        dictionary.Add(ettCheck, new UCCEntityCheckEngine(ettCheck));
                    else if (dictionary[ettCheck] is UCCEntityCheckEngine engine)
                    {
                        ettCheck.Engine = engine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is UCCTargetMatchEntity targMatch)
                {
#if false
                    if (!dictionary.ContainsKey(targMatch))
                        dictionary.Add(targMatch, new UCCTargetMatchEntityEngine(targMatch));
                    else if (dictionary[targMatch] is UCCTargetMatchEntityEngine engine)
                    {
                        targMatch.Engine = engine;
                        return true;
                    } 
#else
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
#endif
                }
                if (condition is UCCGameUICheck uiCheck)
                {
#if false
                    if (!dictionary.ContainsKey(uiCheck))
                        dictionary.Add(uiCheck, new UCCGameUICheckEngine(uiCheck));
                    else if (dictionary[uiCheck] is UCCGameUICheckEngine engine)
                    {
                        uiCheck.Engine = engine;
                        return true;
                    } 
#else
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
#endif
                }
            }
            catch { }
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

        public bool GUIRequest_NPCInfos(ref NPCInfos npc)
        {
            npc = null;
            while (TargetSelectForm.GUIRequest("Target the Traider and press ok.") == DialogResult.OK)
            {
                Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                if (betterEntityToInteract.IsValid)
                {
                    npc = new NPCInfos()
                    {
                        CostumeName = betterEntityToInteract.CostumeRef.CostumeName,
                        DisplayName = betterEntityToInteract.Name,
                        Position = betterEntityToInteract.Location.Clone(),
                        MapName = EntityManager.LocalPlayer.MapState.MapName,
                        RegionName = EntityManager.LocalPlayer.RegionInternalName
                    };
                    return true;
                }
            }
            return false;
        }

        
        public bool GUIRequest_UCCAction(out UCCAction action)
        {
            return Forms.AddUccActionForm.GUIRequest(out action);
        }
        #endregion

        public string EntityDiagnosticInfos(object obj)
        {
            if (obj != null)
            {
                StringBuilder sb = new StringBuilder();

                IEntityInfos ettInfos = null;

                if (obj is QuesterAction qa
                    && dictQuesterAction.TryGetValue(qa, out IQuesterActionEngine qaEngine))
                        ettInfos = qaEngine as IEntityInfos;
                else if(obj is QuesterCondition qc
                    && dictQuesterCondition.TryGetValue(qc, out IQuesterConditionEngine qcEngine))
                        ettInfos = qcEngine as IEntityInfos;
                else if (obj is UCCCondition uccCond
                    && dictUccCondition.TryGetValue(uccCond, out IUCCConditionEngine uccCondEngine))
                        ettInfos = uccCondEngine as IEntityInfos;
                else if (obj is UCCAction uccAct
                    && dictUccAction.TryGetValue(uccAct, out IUCCActionEngine uccActEngine))
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
                        healthCheck, reactionRange, reactionZRange, regionCheck, customRegions, auraOption.Checker);

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
#endif
#if DEBUG
        public LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null)
        {
            return Entities.SearchCached.FindAllEntity(pattern, matchType, nameType, setType, healthCheck, range, zRange, regionCheck, customRegions, specialCheck);
        }
#endif
    }
}
