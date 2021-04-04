using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Reflection;
using EntityTools.Tools;
using EntityTools.Tools.CustomRegions;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Entities
{
    public static class EntityDiagnosticTools
    {
#if false
        public static string DiagnosticInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
                for (int i = 1; i < @this._customRegionNames.Count; i++)
                    sb.Append(", ").Append(@this._customRegionNames[i]);
                sb.AppendLine("}");
            }
            sb.AppendLine();

            var entityKey = EntityKey;
            var entityCheck = SpecialCheckSelector;
            // список всех Entity, удовлетворяющих условиям
#if false
            LinkedList<Entity> entities = SearchCached.FindAllEntity(entityKey,
                                                                     @this._healthCheck,
                                                                     @this._reactionRange, @this._reactionZRange,
                                                                     @this._regionCheck,
                                                                     SpecialCheckSelector); 
#else
            var entities = SearchCached.FindAllEntity(entityKey, entityCheck);
#endif

            // Количество Entity, удовлетворяющих условиям
            if (entities?.Count > 0)
                sb.Append("Found Entities which matched to ID '" + nameof(@this.EntityID) + '\'').AppendLine(entities.Count.ToString());
            else sb.Append("Found Entities which matched to ID '" + nameof(@this.EntityID) + "': 0");
            sb.AppendLine();

#if false
            target = SearchCached.FindClosestEntity(entityKey,
                                                        false,
                                                        0, 0,
                                                        @this._regionCheck,
                                                        SpecialCheckSelector); 
#else
            target = SearchCached.FindClosestEntity(entityKey, entityCheck);
#endif

            closestEntity = null;

            if (entityKey.Validate(target))
            {
                bool distOk = @this._reactionRange <= 0 || target.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(target.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !target.IsDead;
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
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                sb.Append("\tDistance: ").Append(target.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
                if (zOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;

        } 
#endif

#if false
        public static string EntityDiagnosticInfos(object obj)
        {
            if (obj != null)
            {
                StringBuilder sb = new StringBuilder();

                var @object = Traverse.Create(obj);
                var entityId = @object.Property("EntityID");
                var entityNameType = @object.Property("EntityNameType");
                var entityIdType = @object.Property("EntityIdType");


                if (entityId.PropertyExists() && entityNameType.PropertyExists() && entityIdType.PropertyExists())
                {
                    sb.Append("EntityID: ").AppendLine(entityId.GetValue<string>());
                    sb.Append("EntityIdType: ").AppendLine(entityNameType.GetValue().ToString());
                    sb.Append("EntityNameType: ").AppendLine(entityIdType.GetValue().ToString());

                    var entitySet = @object.Property("EntitySetType");
                    //EntitySetType entitySet = entitySet.PropertyExists() ? entitySet.GetValue<EntitySetType>() : EntitySetType.Complete;
                    var regionCheck = @object.Property("RegionCheck");
                    var healthCheck = @object.Property("HealthCheck");
                    var reactionRange = @object.Property("ReactionRange");
                    var reactionZRange = @object.Property("ReactionZRange");
                    var customRegions = @object.Property("CustomRegionNames");
                    var auraOption = @object.Property("Aura");

                    EntitySetType entitySetValue = EntitySetType.Complete;
                    if (entitySet.PropertyExists())
                    {
                        entitySetValue = entitySet.GetValue<EntitySetType>();
                        sb.Append("EntitySetType: ").AppendLine(entitySetValue.ToString());
                    }
                    if (healthCheck.PropertyExists())
                        sb.Append("HealthCheck: ").AppendLine(healthCheck.GetValue().ToString());
                    if (reactionRange.PropertyExists())
                        sb.Append("ReactionRange: ").AppendLine(reactionRange.GetValue().ToString());
                    if (reactionZRange.PropertyExists())
                        sb.Append("ReactionZRange: ").AppendLine(reactionZRange.GetValue().ToString());
                    if (regionCheck.PropertyExists())
                        sb.Append("RegionCheck: ").AppendLine(regionCheck.GetValue().ToString());

                    Predicate<Entity> crCheck = null;
                    Predicate<Entity> auraCheck = null;
                    if (customRegions.PropertyExists())
                    {
                        var customRegionObj = customRegions.GetValue();
                        if (customRegionObj != null)
                        {
                            if (customRegionObj is CustomRegionCollection crCollection)
                            {
                                crCheck = crCollection.Within;
                                sb.Append("CustomRegionCheck: ").Append(crCollection.ToString());
                            }
                            sb.Append("CustomRegionCheck: {").Append(customRegions[0].Name);
                            for (int i = 1; i < customRegions.Count; i++)
                                sb.Append(", ").Append(customRegions[i].Name);
                            sb.AppendLine("}");
                        }
                    }
                    if (auraOption.PropertyExists())
                    {
                        var auraOptionValue = auraOption.GetValue<AuraOption>();
                        if (auraOptionValue != null)
                        {
                            auraCheck = auraOptionValue.Checker;
                            sb.Append("Aura: ").AppendLine(auraOptionValue.ToString());
                        }
                    }

                    sb.AppendLine();
                    var entityKey = new EntityCacheRecordKey(entityId.GetValue<string>(),
                                                              entityIdType.GetValue<ItemFilterStringType>(),
                                                              entityNameType.GetValue<EntityNameType>(),
                                                              entitySetValue);
                    var entityCheck = SearchCached.Construct_SearchPredicate(healthCheck.GetValue<bool>(),
                                                              reactionRange.GetValue<float>(),
                                                              reactionZRange.GetValue<float>(),
                                                              regionCheck.GetValue<bool>(),
                                                              crCheck,
                                                              auraCheck);

                    var entities = SearchCached.FindAllEntity(entityKey, entityCheck);

                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    // Ближайшее Entity
                    Entity target = SearchCached.FindClosestEntity(entityId.GetValue<string>(),
                                                                   entityIdType.GetValue<ItemFilterStringType>(),
                                                                   entityNameType.GetValue<EntityNameType>(),
                                                                   entitySetValue,
                                                                   healthCheck.GetValue<bool>(),
                                                                   reactionRange.GetValue<float>(),
                                                                   reactionZRange.GetValue<float>(),
                                                                   regionCheck.GetValue<bool>(),
                                                                   customRegions
                                                                   auraOption.GetValue<AuraOption>()?.Checker);
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

                //Task.Factory.StartNew(() => XtraMessageBox.Show(/*Form.ActiveForm, */sb.ToString(), "Test of '" + obj.ToString() + '\''));
                return sb.ToString();
            }
            return string.Empty;
        } 
#endif

#if true
        public static string Construct(IEntityDescriptor entityDescriptor)
        {
            if (entityDescriptor != null)
            {
                StringBuilder sb = new StringBuilder();

                var @object = Traverse.Create(entityDescriptor);

                if (entityDescriptor.EntityNameType == EntityNameType.Empty || !string.IsNullOrEmpty(entityDescriptor.EntityID))
                {
                    sb.Append("EntityID: ").AppendLine(entityDescriptor.EntityID);
                    sb.Append("EntityIdType: ").AppendLine(entityDescriptor.EntityIdType.ToString());
                    sb.Append("EntityNameType: ").AppendLine(entityDescriptor.EntityNameType.ToString());

                    var entitySet = entityDescriptor.GetProperty<EntitySetType>(nameof(EntitySetType));
                    EntitySetType entitySetValue = EntitySetType.Complete;
                    if (entitySet.IsValid)
                    {
                        entitySetValue = entitySet.Value;
                        sb.Append("EntitySetType: ").AppendLine(entitySetValue.ToString());
                    }

                    sb.Append("HealthCheck: ").AppendLine(entityDescriptor.HealthCheck.ToString());
                    sb.Append("ReactionRange: ").AppendLine(entityDescriptor.ReactionRange.ToString());
                    sb.Append("ReactionZRange: ").AppendLine(entityDescriptor.ReactionZRange.ToString());
                    sb.Append("Region: ").AppendLine(entityDescriptor.RegionCheck.ToString());

                    Predicate<Entity> crCheck = null;
                    var customRegions = entityDescriptor.GetProperty<CustomRegionCollection>("CustomRegionNames");
                    var customRegionCheck = entityDescriptor.GetProperty<Astral.Quester.Classes.Condition.Presence>("CustomRegionCheck");
                    if (customRegions.IsValid)
                    {
                        var crCollection = customRegions.Value;

                        if(customRegionCheck.IsValid && customRegionCheck.Value == Condition.Presence.NotEquel)
                            crCheck = crCollection.Outside;
                        else crCheck = crCollection.Within;
                        sb.Append("CustomRegions: ").Append(crCollection.ToString());
                        if (crCollection.Count > 0)
                        {
                            sb.AppendLine(" {");

                            using (var enumerator = crCollection.GetEnumerator())
                            {
                                if (enumerator.MoveNext())
                                {
                                    sb.Append('\t').Append(enumerator.Current);

                                    while (enumerator.MoveNext())
                                        sb.AppendLine(", ").Append('\t').Append(enumerator.Current);
                                }
                            }
                            sb.AppendLine("\n}");
                        }
                        else sb.AppendLine();
                    }
                    var auraOption = entityDescriptor.GetProperty<AuraOption>("Aura");
                    Predicate<Entity> auraCheck = null;
                    if (auraOption.IsValid)
                    {
                        var auraOptionValue = auraOption.Value;
                        if (auraOptionValue != null)
                        {
                            auraCheck = auraOptionValue.IsMatch;
                            sb.Append("Aura: ").AppendLine(auraOptionValue.ToString());
                        }
                    }

                    sb.AppendLine();
                    var entityKey = new EntityCacheRecordKey(entityDescriptor.EntityID,
                                                             entityDescriptor.EntityIdType,
                                                             entityDescriptor.EntityNameType,
                                                             entitySetValue);
                    var entityCheck = SearchHelper.Construct_EntityAttributePredicate(entityDescriptor.HealthCheck,
                                                              entityDescriptor.ReactionRange,
                                                              entityDescriptor.ReactionZRange,
                                                              entityDescriptor.RegionCheck,
                                                              customRegions.Value,
                                                              customRegionCheck.IsValid && customRegionCheck.Value == Condition.Presence.NotEquel,
                                                              auraCheck);

                    var entities = SearchCached.FindAllEntity(entityKey);

                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    // Ближайшее Entity
                    Entity target = SearchCached.FindClosestEntity(entityKey, entityCheck);

                    if (target != null && target.IsValid)
                    {
                        bool distOk = entityDescriptor.ReactionRange <= 0 || target.Location.Distance3DFromPlayer < entityDescriptor.ReactionRange;
                        bool zOk = entityDescriptor.ReactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(target.Location) < entityDescriptor.ReactionZRange;
                        bool alive = !entityDescriptor.HealthCheck || !target.IsDead;
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
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else sb.Append("Unable recognize test context!");

                return sb.ToString();
            }
            return string.Empty;
        }
        public static string Construct(IEntityIdentifier entityIdentifier)
        {
            if (entityIdentifier != null)
            {
                StringBuilder sb = new StringBuilder();

                var @object = Traverse.Create(entityIdentifier);

                if (entityIdentifier.EntityNameType == EntityNameType.Empty || !string.IsNullOrEmpty(entityIdentifier.EntityID))
                {
                    sb.Append("EntityID: ").AppendLine(entityIdentifier.EntityID);
                    sb.Append("EntityIdType: ").AppendLine(entityIdentifier.EntityIdType.ToString());
                    sb.Append("EntityNameType: ").AppendLine(entityIdentifier.EntityNameType.ToString());

                    var entitySet = entityIdentifier.GetProperty<EntitySetType>(nameof(EntitySetType));
                    EntitySetType entitySetValue = EntitySetType.Complete;
                    if (entitySet.IsValid)
                    {
                        entitySetValue = entitySet.Value;
                        sb.Append("EntitySetType: ").AppendLine(entitySetValue.ToString());
                    }

                    sb.AppendLine();
                    var entityKey = new EntityCacheRecordKey(entityIdentifier.EntityID,
                                                             entityIdentifier.EntityIdType,
                                                             entityIdentifier.EntityNameType,
                                                             entitySetValue);

                    var entities = SearchCached.FindAllEntity(entityKey);

                    // Количество Entity, удовлетворяющих условиям
                    if (entities != null)
                        sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                    else sb.Append("Founded Entities: 0");
                    sb.AppendLine();

                    // Ближайшее Entity
                    Entity target = SearchCached.FindClosestEntity(entityKey);

                    if (target != null && target.IsValid)
                    {
                        sb.Append("ClosestEntity: ").Append(target.ToString());
                        sb.Append("\tName: ").AppendLine(target.Name);
                        sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                        sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                        sb.Append("\tIsDead: ").Append(target.IsDead.ToString());
                        sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                        sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                        sb.Append("\tDistance: ").Append(target.Location.Distance3DFromPlayer.ToString());
                        sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
                    }
                    else sb.AppendLine("Closest Entity not found!");
                }
                else sb.Append("Unable recognize test context!");

                return sb.ToString();
            }
            return string.Empty;
        }
#endif
    }
}
