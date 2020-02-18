using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Actions;
using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Conditions;
using static EntityTools.Forms.EntitySelectForm;
using EntityTools.UCC;
using EntityTools.Actions.Deprecated;
using Astral.Classes.ItemFilter;
using MyNW.Classes;
using MyNW.Internals;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using System.Threading.Tasks;
using EntityTools.Tools.Entities;
using Astral.Quester.Classes;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    public class EntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            StringBuilder sb = new StringBuilder();

            if (context.Instance != null)
            {
                if (context.Instance is MoveToEntity mte)
                {
                    sb.Append("EntityID: ").AppendLine(mte.EntityID);
                    sb.Append("EntityIdType: ").AppendLine(mte.EntityIdType.ToString());
                    sb.Append("EntityNameType: ").AppendLine(mte.EntityNameType.ToString());
                    sb.Append("HealthCheck: ").AppendLine(mte.HealthCheck.ToString());
                    sb.Append("ReactionRange: ").AppendLine(mte.ReactionRange.ToString());
                    sb.Append("ReactionZRange: ").AppendLine(mte.ReactionZRange.ToString());
                    sb.Append("RegionCheck: ").AppendLine(mte.RegionCheck.ToString());
                    if(mte.CustomRegionNames != null && mte.CustomRegionNames.Count > 0)
                    {
                        sb.Append("RegionCheck: {").Append(mte.CustomRegionNames[0]);
                        for (int i = 1; i < mte.CustomRegionNames.Count; i++)
                            sb.Append(", ").Append(mte.CustomRegionNames[i]);
                        sb.AppendLine("}");
                    }
                    sb.AppendLine();
                    sb.Append("NeedToRun: ").AppendLine(mte.NeedToRun.ToString());
                    sb.AppendLine();
                    // список всех Entity, удовлетворяющих условиям
                    LinkedList<Entity> entities = SearchCached.FindAllEntity(mte.EntityID, mte.EntityIdType, mte.EntityNameType, EntitySetType.Complete,
                        mte.HealthCheck, mte.ReactionRange, mte.ReactionZRange, mte.RegionCheck, CustomRegionTools.GetCustomRegions(mte.CustomRegionNames));

                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    /// Ближайшее Entity (найдено при вызове mte.NeedToRun, поэтому строка ниже закомментирована)
                    //target = EntitySelectionTools.FindClosestEntity(entities, EntityId, EntityIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);
                    if (mte.target != null && mte.target.IsValid)
                    {
                        //sb.Append("ClosectEntity: ").AppendLine(mte.target.ToString());
                        //sb.Append("\tName: ").AppendLine(mte.target.Name);
                        //sb.Append("\tInternalName: ").AppendLine(mte.target.InternalName);
                        //sb.Append("\tNameUntranslated: ").AppendLine(mte.target.NameUntranslated);
                        //sb.Append("\t[").Append(!(mte.HealthCheck && mte.target.IsDead) ? "+" : "-")
                        //    .Append("]IsDead: ").AppendLine(mte.target.IsDead.ToString());
                        //sb.Append("\t[").Append((!mte.RegionCheck || mte.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                        //    .Append("]Region: '").Append(mte.target.RegionInternalName).AppendLine("'");
                        //sb.Append("\tLocation: ").AppendLine(mte.target.Location.ToString());
                        //sb.Append("\t[").Append((mte.ReactionRange == 0 || mte.target.Location.Distance3DFromPlayer < mte.ReactionRange) ? "+" : "-")
                        //    .Append("]Distance: ").AppendLine(mte.target.Location.Distance3DFromPlayer.ToString());
                        sb.Append("Target: ").AppendLine(mte.target.ToString());
                        sb.Append("\tName: ").AppendLine(mte.target.Name);
                        sb.Append("\tInternalName: ").AppendLine(mte.target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(mte.target.NameUntranslated);
                        sb.Append("\tIsDead: ").AppendLine(mte.target.IsDead.ToString());
                        sb.Append("\tRegion: '").Append(mte.target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(mte.target.Location.ToString());
                        sb.Append("\tDistance: ").AppendLine(mte.target.Location.Distance3DFromPlayer.ToString());
                        sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(mte.target.Location).ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");

                    //XtraMessageBox.Show(sb.ToString(), "Test of '" + mte.ToString() + '\'');
                }
                else if (context.Instance is InteractEntities ie)
                {
                    ie.InternalReset();
                    sb.Append("EntityID: ").AppendLine(ie.EntityID);
                    sb.Append("EntityIdType: ").AppendLine(ie.EntityIdType.ToString());
                    sb.Append("EntityNameType: ").AppendLine(ie.EntityNameType.ToString());
                    sb.Append("EntitySetType: ").AppendLine(ie.EntitySetType.ToString());
                    sb.Append("HealthCheck: ").AppendLine(ie.HealthCheck.ToString());
                    sb.Append("ReactionRange: ").AppendLine(ie.ReactionRange.ToString());
                    sb.Append("ReactionZRange: ").AppendLine(ie.ReactionZRange.ToString());
                    sb.Append("RegionCheck: ").AppendLine(ie.RegionCheck.ToString());
                    if (ie.CustomRegionNames != null && ie.CustomRegionNames.Count > 0)
                    {
                        sb.Append("RegionCheck: {").Append(ie.CustomRegionNames[0]);
                        for (int i = 1; i < ie.CustomRegionNames.Count; i++)
                            sb.Append(", ").Append(ie.CustomRegionNames[i]);
                        sb.AppendLine("}");
                    }
                    sb.AppendLine();
                    sb.Append("NeedToRun: ").AppendLine(ie.NeedToRun.ToString());
                    sb.AppendLine();

                    // список всех Entity, удовлетворяющих условиям
                    LinkedList<Entity> entities = SearchCached.FindAllEntity(ie.EntityID, ie.EntityIdType, ie.EntityNameType, ie.EntitySetType,
                                                                        ie.HealthCheck, ie.ReactionRange, ie.ReactionZRange, ie.RegionCheck, CustomRegionTools.GetCustomRegions(ie.CustomRegionNames));


                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
                    //target = EntitySelectionTools.FindClosestEntity(entities, EntityId, EntityIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);
                    if (ie.target != null && ie.target.IsValid)
                    {
                        sb.Append("Target: ").AppendLine(ie.target.ToString());
                        sb.Append("\tName: ").AppendLine(ie.target.Name);
                        sb.Append("\tInternalName: ").AppendLine(ie.target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(ie.target.NameUntranslated);
                        sb.Append("\tIsDead: ").AppendLine(ie.target.IsDead.ToString());
                        sb.Append("\tRegion: '").Append(ie.target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(ie.target.Location.ToString());
                        sb.Append("\tDistance: ").AppendLine(ie.target.Location.Distance3DFromPlayer.ToString());
                        sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(ie.target.Location).ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else if (ReflectionHelper.GetPropertyValue(context.Instance, "EntityID", out object entityId)
                        && ReflectionHelper.GetPropertyValue(context.Instance, "EntityNameType", out object entityNameType)
                        && ReflectionHelper.GetPropertyValue(context.Instance, "EntityIdType", out object entityIdType))
                {
                    sb.Append("EntityID: ").AppendLine(entityId.ToString());

                    EntitySetType entitySet = ReflectionHelper.GetPropertyValue(context.Instance, "EntitySetType", out object entitySetObj) ? (EntitySetType)entitySetObj: EntitySetType.Complete;
                    bool regionCheck = ReflectionHelper.GetPropertyValue(context.Instance, "RegionCheck", out object regionCheckObj) ? (bool)regionCheckObj : false;
                    bool healthCheck = ReflectionHelper.GetPropertyValue(context.Instance, "HealthCheck", out object healthCheckObj) ? (bool)healthCheckObj : false;
                    float reactionRange = ReflectionHelper.GetPropertyValue(context.Instance, "ReactionRange", out object reactionRangeObj) ? (float)reactionRangeObj : 0;
                    float reactionZRange = ReflectionHelper.GetPropertyValue(context.Instance, "ReactionZRange", out object reactionZRangeObj) ? (float)reactionZRangeObj : 0;
                    List<CustomRegion> customRegions = ReflectionHelper.GetFieldValue(context.Instance, "customRegions", out object customRegionsObj) ? customRegionsObj as List<CustomRegion> : null;
                    AuraOption auraOption = ReflectionHelper.GetPropertyValue(context.Instance, "Aura", out object auraOptionObj) ? auraOptionObj as AuraOption : new AuraOption();

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
                                            (EntityNameType)entityNameType, entitySet, healthCheck, reactionRange, reactionZRange, regionCheck, customRegions);
                    if (target != null && target.IsValid)
                    {
                        sb.Append("ClosectEntity: ").AppendLine(target.ToString());
                        sb.Append("\tName: ").AppendLine(target.Name);
                        sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                        sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
                        sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                        sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else sb.Append("Unable recognize test context!");

                Task.Factory.StartNew(() => XtraMessageBox.Show(/*Form.ActiveForm, */sb.ToString(), "Test of '" + context.Instance.ToString() + '\''));                
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
