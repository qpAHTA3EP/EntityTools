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

namespace EntityTools.Editors
{
    public class EntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //string EntityId = string.Empty;
            //EntityNameType NameType = EntityNameType.NameUntranslated;
            //ItemFilterStringType EntityIdType = ItemFilterStringType.Simple;
            //bool RegionCheck = false;
            //bool HealthCheck = false;
            //float ReactionRange = 0;
            //List<string> CustomRegionNames = null;
            //int EntitiesNumber = 0;

            //Entity target = null;
            StringBuilder sb = new StringBuilder();
            //sb.Append("Test '").Append(context.Instance.GetType().Name).AppendLine("':");

            if (context.Instance != null)
            {
                if (context.Instance is MoveToEntity mte)
                {
                    mte.InternalReset();
                    sb.Append("EntityID: ").AppendLine(mte.EntityID);
                    sb.AppendLine();
                    sb.Append("NeedToRun: ").AppendLine(mte.NeedToRun.ToString());
                    sb.AppendLine();

                    //EntityId = mte.EntityID;
                    //NameType = mte.EntityNameType;
                    //EntityIdType = mte.EntityIdType;
                    //RegionCheck = mte.RegionCheck;
                    //HealthCheck = mte.HealthCheck;
                    //CustomRegionNames = mte.CustomRegionNames;

                    // список всех Entity, удовлетворяющих условиям
                    List<Entity> entities = EntitySelectionTools.FindAllEntities(EntityManager.GetEntities(), mte.EntityID, mte.EntityIdType, mte.EntityNameType,
                        mte.HealthCheck, mte.RegionCheck, mte.CustomRegionNames,
                        (Entity e) => mte.ReactionRange == 0 || e.Location.Distance3DFromPlayer < mte.ReactionRange);
                    // Количество Entity, удовлетворяющих условиям
                    sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    sb.AppendLine();

                    /// Ближайшее Entity (найдено при вызове mte.NeedToRun, поэтому строка ниже закомментирована)
                    //target = EntitySelectionTools.FindClosestEntity(entities, EntityId, EntityIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);
                    if (mte.target != null && mte.target.IsValid)
                    {
                        sb.Append("ClosectEntity: ").AppendLine(mte.target.ToString());
                        sb.Append("\tName: ").AppendLine(mte.target.Name);
                        sb.Append("\tInternalName: ").AppendLine(mte.target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(mte.target.NameUntranslated);
                        sb.Append("\t[").Append(!(mte.HealthCheck && mte.target.IsDead) ? "+" : "-")
                            .Append("]IsDead: ").AppendLine(mte.target.IsDead.ToString());
                        sb.Append("\t[").Append((!mte.RegionCheck || mte.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                            .Append("]Region: '").Append(mte.target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(mte.target.Location.ToString());
                        sb.Append("\t[").Append((mte.ReactionRange == 0 || mte.target.Location.Distance3DFromPlayer < mte.ReactionRange) ? "+" : "-")
                            .Append("]Distance: ").AppendLine(mte.target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");

                    XtraMessageBox.Show(sb.ToString(), "Test of '" + mte.ToString() + '\'');
                }
                else if (context.Instance is InteractEntities ie)
                {
                    ie.InternalReset();
                    sb.Append("EntityID: ").AppendLine(ie.EntityID);
                    sb.AppendLine();
                    sb.Append("NeedToRun: ").AppendLine(ie.NeedToRun.ToString());
                    sb.AppendLine();
                    //EntityId = ie.EntityID;
                    //NameType = ie.EntityNameType;
                    //EntityIdType = ie.EntityIdType;
                    //RegionCheck = ie.RegionCheck;
                    //HealthCheck = ie.HealthCheck;
                    //CustomRegionNames = ie.CustomRegionNames;

                    // список всех Entity, удовлетворяющих условиям
                    List<Entity> entities = EntitySelectionTools.FindAllEntities(EntityManager.GetEntities(), ie.EntityID, ie.EntityIdType, ie.EntityNameType,
                        ie.HealthCheck, ie.RegionCheck, ie.CustomRegionNames,
                        (Entity e) => ie.ReactionRange == 0 || e.Location.Distance3DFromPlayer < ie.ReactionRange);
                    // Количество Entity, удовлетворяющих условиям
                    sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    sb.AppendLine();

                    // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
                    //target = EntitySelectionTools.FindClosestEntity(entities, EntityId, EntityIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames);
                    if (ie.target != null && ie.target.IsValid)
                    {
                        sb.Append("ClosectEntity: ").AppendLine(ie.target.ToString());
                        sb.Append("\tName: ").AppendLine(ie.target.Name);
                        sb.Append("\tInternalName: ").AppendLine(ie.target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(ie.target.NameUntranslated);
                        sb.Append("\t[").Append(!(ie.HealthCheck && ie.target.IsDead) ? "+" : "-")
                            .Append("]IsDead: ").AppendLine(ie.target.IsDead.ToString());
                        sb.Append("\t[").Append((!ie.RegionCheck || ie.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                            .Append("]Region: '").Append(ie.target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(ie.target.Location.ToString());
                        sb.Append("\t[").Append((ie.ReactionRange == 0 || ie.target.Location.Distance3DFromPlayer < ie.ReactionRange) ? "+" : "-")
                            .Append("]Distance: ").AppendLine(ie.target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else if (ReflectionHelper.GetPropertyValue(context.Instance, "EntityID", out object entityId)
                        && ReflectionHelper.GetPropertyValue(context.Instance, "EntityNameType", out object entityNameType)
                        && ReflectionHelper.GetPropertyValue(context.Instance, "EntityIdType", out object entityIdType))
                {
                    sb.Append("EntityID: ").AppendLine(entityId.ToString());
                    sb.AppendLine();

                    bool regionCheck = ReflectionHelper.GetPropertyValue(context.Instance, "RegionCheck", out object regionCheckObj) ? (bool)regionCheckObj : false;
                    bool healthCheck = ReflectionHelper.GetPropertyValue(context.Instance, "HealthCheck", out object healthCheckObj) ? (bool)healthCheckObj : false;
                    float reactionRange = ReflectionHelper.GetPropertyValue(context.Instance, "ReactionRange", out object reactionRangeObj) ? (float)reactionRangeObj : 0;

                    List <Entity> entities = EntitySelectionTools.FindAllEntities(EntityManager.GetEntities(), entityId.ToString(), (ItemFilterStringType)entityIdType,
                                            (EntityNameType)entityNameType, healthCheck, regionCheck, null,
                                            (Entity e) => reactionRange == 0f || e.Location.Distance3DFromPlayer < reactionRange);

                    // Количество Entity, удовлетворяющих условиям
                    sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    sb.AppendLine();

                    // Ближайшее Entity
                    Entity target = EntitySelectionTools.FindClosestEntity(entities, entityId.ToString(), (ItemFilterStringType)entityIdType,
                                            (EntityNameType)entityNameType, healthCheck, reactionRange, regionCheck, null);
                    if (target != null && target.IsValid)
                    {
                        sb.Append("ClosectEntity: ").AppendLine(target.ToString());
                        sb.Append("\tName: ").AppendLine(target.Name);
                        sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                        sb.Append("\t[").Append(!(healthCheck && target.IsDead) ? "+" : "-")
                            .Append("]IsDead: ").AppendLine(target.IsDead.ToString());
                        sb.Append("\t[").Append((!regionCheck || target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                            .Append("]Region: '").Append(target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                        sb.Append("\t[").Append((reactionRange == 0 || target.Location.Distance3DFromPlayer < reactionRange) ? "+" : "-")
                            .Append("]Distance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else sb.Append("Unable recognize test context!");
                XtraMessageBox.Show(sb.ToString(), "Test of '" + context.Instance.ToString() + '\'');
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
