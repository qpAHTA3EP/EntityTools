using Astral.Logic.UCC.Classes;
//using Astral.Quester.Classes;
//using EntityTools.Core;
//using EntityTools.Quester.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Action;
//using EntityTools.Quester.Conditions;
//using UCCConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
//using EntityTools.Tools;
//using Astral.Classes.ItemFilter;
//using EntityTools.Enums;
//using System.Threading.Tasks;
//using DevExpress.XtraEditors;
//using MyNW.Classes;
//using EntityCore.Entities;
//using EntityTools.Extentions;
//using EntityCore.Forms;
//using System.Windows.Forms;
//using MyNW.Internals;
//using EntityTools.Reflection;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;
using EntityTools.Core.Interfaces;
using Mount_Tutorial;

namespace Mount_Tutorial_Core
{
    public class Engine : IEntityToolsCore
    {
        internal static LinkedList<object> list = new LinkedList<object>();
        internal static Dictionary<object, object> dictionary = new Dictionary<object, object>();

        public bool Initialize(object obj)
        {
            try
            {
                if (obj is QuesterAction qAction)
                    return Initialize(qAction);
                else if (obj is QuesterCondition qCondition)
                    return Initialize(qCondition);
                if (obj is UCCAction uccAction)
                    return Initialize(uccAction);
                else if (obj is UCCCondition uccCondition)
                    return Initialize(uccCondition);
            }
            catch { }
            return false;
        }
        public bool Initialize(QuesterAction action)
        {
            try
            {
                //if (action is MoveToEntity m2e)
                //{
                //    //list.AddLast(new MoveToEntityEngine(m2e));
                //    dictionary.Add(m2e, new MoveToEntityEngine(m2e));
                //    return true;
                //}
                //else if (action is InteractEntities ie)
                //{
                //    //list.AddLast(new InteractEntitiesEngine(ie));
                //    dictionary.Add(ie, new InteractEntitiesEngine(ie));
                //    return true;
                //}
                //else if (action is PickUpMissionExt pum)
                //{
                //    //list.AddLast(new PickUpMissionEngine(pum));
                //    dictionary.Add(pum, new PickUpMissionEngine(pum));
                //    return true;
                //}
                //else 
                /*if (action is InsertInsignia ii)
                {
                    //list.AddLast(new InsertInsigniaEngine(ii));
                    dictionary.Add(ii, new InsertInsigniaEngine(ii));
                    return true;
                }
                else */if (action is HorseEquipTutorialAction horseEquip)
                {
                    dictionary.Add(horseEquip, new HorseEquipTutorialEngine(horseEquip));
                    return true;
                }
            }
            catch { }
            return false;
        }
        public bool Initialize(QuesterCondition condition)
        {
            //try
            //{
            //    if (condition is EntityCount ettCount)
            //    {
            //        //list.AddLast(new EntityCountEngine(ettCount));
            //        dictionary.Add(ettCount, new EntityCountEngine(ettCount));
            //        return true;
            //    }
            //}
            //catch { }
            return false;
        }
        public bool Initialize(UCCAction action)
        {
            throw new NotImplementedException();
        }
        public bool Initialize(UCCCondition condition)
        {
            throw new NotImplementedException();
        }

        //public bool GUIRequest_AuraId(ref string id)
        //{
        //    string newId = Forms.AuraSelectForm.Processing();
        //    if(!string.IsNullOrEmpty(newId))
        //    {
        //        id = newId;
        //        return true;
        //    }
        //    return false;
        //}

        //public bool GUIRequest_UIGenId(ref string id)
        //{
        //    string newId = Forms.UIViewer.Procesing(id);
        //    if (!string.IsNullOrEmpty(newId))
        //    {
        //        id = newId;
        //        return true;
        //    }
        //    return false;
        //}

        //public UCCConditionList GUIRequest_UCCConditions(UCCConditionList list)
        //{
        //    return Forms.ConditionListForm.Processing(list);
        //}

        //public EntityDef GUIRequest_EntityId(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated)
        //{
        //    return Forms.EntitySelectForm.Processing(entPattern, strMatchType, nameType);
        //}

        //public bool GUIRequest_CustomRegions(ref List<string> crList)
        //{
        //    List<string> list = crList ?? new List<string>();

        //    if (MultiSelectForm.Processing("Select CustomRegions:", 
        //                                    (DataGridView dgv) => CustomRegionExtentions.CustomRegionList2DataGridView(list, dgv), 
        //                                    (DataGridView dgv) => CustomRegionExtentions.DataGridView2CustomRegionList(dgv, ref list)))
        //    {
        //        crList = list;
        //        return true;
        //    }
        //    return false;
        //}

        //public bool GUIRequest_NodeLocation(ref Vector3 pos, string caption)
        //{
        //    while (TargetSelectForm.TargetGuiRequest(caption) == DialogResult.OK)
        //    {
        //        if (EntityManager.LocalPlayer.Player.InteractStatus.pMouseOverNode != IntPtr.Zero)
        //        {
        //            var node = EntityManager.LocalPlayer.Player.InteractStatus.PreferredTargetNode;
        //            if(node != null && node.IsMouseOver)
        //            {
        //                pos = node.Location.Clone();
        //                return true;
        //            }
        //            //foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
        //            //{
        //            //    if (targetableNode.IsValid && targetableNode.IsMouseOver)
        //            //    {
        //            //        pos = targetableNode.WorldInteractionNode.Location.Clone();
        //            //        return true;
        //            //    }
        //            //}
        //        }
        //    }
        //    return false;
        //}

        public string EntityDiagnosticInfos(object obj)
        {
            //    if (obj != null)
            //    {
            //        StringBuilder sb = new StringBuilder();

            //        if (obj is MoveToEntity mte)
            //        {


            //            //XtraMessageBox.Show(sb.ToString(), "Test of '" + mte.ToString() + '\'');
            //        }
            //        else if (obj is InteractEntities ie)
            //        {

            //        }
            //        else if (ReflectionHelper.GetPropertyValue(obj, "EntityID", out object entityId)
            //                && ReflectionHelper.GetPropertyValue(obj, "EntityNameType", out object entityNameType)
            //                && ReflectionHelper.GetPropertyValue(obj, "EntityIdType", out object entityIdType))
            //        {
            //            sb.Append("EntityID: ").AppendLine(entityId.ToString());

            //            EntitySetType entitySet = ReflectionHelper.GetPropertyValue(obj, "EntitySetType", out object entitySetObj) ? (EntitySetType)entitySetObj : EntitySetType.Complete;
            //            bool regionCheck = ReflectionHelper.GetPropertyValue(obj, "RegionCheck", out object regionCheckObj) ? (bool)regionCheckObj : false;
            //            bool healthCheck = ReflectionHelper.GetPropertyValue(obj, "HealthCheck", out object healthCheckObj) ? (bool)healthCheckObj : false;
            //            float reactionRange = ReflectionHelper.GetPropertyValue(obj, "ReactionRange", out object reactionRangeObj) ? (float)reactionRangeObj : 0;
            //            float reactionZRange = ReflectionHelper.GetPropertyValue(obj, "ReactionZRange", out object reactionZRangeObj) ? (float)reactionZRangeObj : 0;
            //            List<CustomRegion> customRegions = ReflectionHelper.GetFieldValue(obj, "customRegions", out object customRegionsObj) ? customRegionsObj as List<CustomRegion> : null;
            //            AuraOption auraOption = ReflectionHelper.GetPropertyValue(obj, "Aura", out object auraOptionObj) ? auraOptionObj as AuraOption : new AuraOption();

            //            sb.Append("EntityIdType: ").AppendLine(entityNameType.ToString());
            //            sb.Append("EntityNameType: ").AppendLine(entityIdType.ToString());
            //            sb.Append("EntitySetType: ").AppendLine(entitySetObj?.ToString() ?? "null");
            //            sb.Append("HealthCheck: ").AppendLine(healthCheckObj?.ToString() ?? "null");
            //            sb.Append("ReactionRange: ").AppendLine(reactionRangeObj?.ToString() ?? "null");
            //            sb.Append("ReactionZRange: ").AppendLine(reactionZRangeObj?.ToString() ?? "null");
            //            sb.Append("RegionCheck: ").AppendLine(regionCheckObj?.ToString() ?? "null");
            //            if (customRegions != null && customRegions.Count > 0)
            //            {
            //                sb.Append("RegionCheck: {").Append(customRegions[0].Name);
            //                for (int i = 1; i < customRegions.Count; i++)
            //                    sb.Append(", ").Append(customRegions[i].Name);
            //                sb.AppendLine("}");
            //            }
            //            sb.Append("Aura: ").AppendLine(auraOptionObj?.ToString() ?? "null");

            //            sb.AppendLine();
            //            LinkedList<Entity> entities = SearchCached.FindAllEntity(entityId.ToString(), entityIdType, entityNameType, entitySet,
            //                healthCheck, reactionRange, reactionZRange, regionCheck, customRegions, auraOption.Checker);

            //            // Количество Entity, удовлетворяющих условиям
            //            if (entities != null)
            //                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            //            else sb.Append("Founded Entities: 0");
            //            sb.AppendLine();

            //            // Ближайшее Entity
            //            Entity target = SearchCached.FindClosestEntity(entityId.ToString(), entityIdType,
            //                                    entityNameType, entitySet, healthCheck, reactionRange, reactionZRange, regionCheck, customRegions);
            //            if (target != null && target.IsValid)
            //            {
            //                sb.Append("ClosectEntity: ").AppendLine(target.ToString());
            //                sb.Append("\tName: ").AppendLine(target.Name);
            //                sb.Append("\tInternalName: ").AppendLine(target.InternalName);
            //                sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
            //                sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
            //                sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
            //                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
            //                sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
            //            }
            //            else sb.AppendLine("Closest Entity not found!");
            //        }
            //        else sb.Append("Unable recognize test context!");

            //        Task.Factory.StartNew(() => XtraMessageBox.Show(/*Form.ActiveForm, */sb.ToString(), "Test of '" + obj.ToString() + '\''));
            //    }

            return string.Empty;
        }
    }
}
