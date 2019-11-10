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
                        sb.Append("\t\tName: ").AppendLine(mte.target.Name);
                        sb.Append("\t\tInternalName: ").AppendLine(mte.target.InternalName);
                        sb.Append("\t\tNameUntranslated: ").AppendLine(mte.target.NameUntranslated);
                        sb.Append("[").Append(!(mte.HealthCheck && mte.target.IsDead) ? "+" : "-")
                            .Append("]\tIsDead: ").AppendLine(mte.target.IsDead.ToString());
                        sb.Append("[").Append((!mte.RegionCheck || mte.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                            .Append("]\tRegion: '").Append(mte.target.RegionInternalName).AppendLine("'");
                        sb.Append("\t\tLocation: ").AppendLine(mte.target.Location.ToString());
                        sb.Append("[").Append((mte.ReactionRange == 0 || mte.target.Location.Distance3DFromPlayer < mte.ReactionRange) ? "+" : "-")
                            .Append("]\tDistance: ").AppendLine(mte.target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("No Entity found");

                    XtraMessageBox.Show(sb.ToString(), "Test of '"+ mte.ToString() + '\'');                    
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
                        sb.Append("\t\tName: ").AppendLine(ie.target.Name);
                        sb.Append("\t\tInternalName: ").AppendLine(ie.target.InternalName);
                        sb.Append("\t\tNameUntranslated: ").AppendLine(ie.target.NameUntranslated);
                        sb.Append("[").Append(!(ie.HealthCheck && ie.target.IsDead) ? "+" : "-")
                            .Append("]\tIsDead: ").AppendLine(ie.target.IsDead.ToString());
                        sb.Append("[").Append((!ie.RegionCheck || ie.target.RegionInternalName == EntityManager.LocalPlayer.RegionInternalName) ? "+" : "-")
                            .Append("]\tRegion: '").Append(ie.target.RegionInternalName).AppendLine("'");
                        sb.Append("\t\tLocation: ").AppendLine(ie.target.Location.ToString());
                        sb.Append("[").Append((ie.ReactionRange == 0 || ie.target.Location.Distance3DFromPlayer < ie.ReactionRange) ? "+" : "-")
                            .Append("]\tDistance: ").AppendLine(ie.target.Location.Distance3DFromPlayer.ToString());
                    }
                    else sb.AppendLine("No Entity found");

                    XtraMessageBox.Show(sb.ToString(), "Test of '" + ie.ToString() + '\'');
                }
                //else if (context.Instance is EntityProperty)
                //    NameType = ((EntityProperty)context.Instance).EntityNameType;
                //else if (context.Instance is EntityProperty)
                //    NameType = ((EntityProperty)context.Instance).EntityNameType;
                //else if (context.Instance is EntityCountInCustomRegions)
                //    NameType = ((EntityCountInCustomRegions)context.Instance).EntityNameType;
                //else if (context.Instance is AbortCombatEntity)
                //    NameType = ((AbortCombatEntity)context.Instance).EntityNameType;
                //else if (context.Instance is InteractNPCext)
                //    nameType = ((InteractNPCext)context.Instance).NameType;                
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
