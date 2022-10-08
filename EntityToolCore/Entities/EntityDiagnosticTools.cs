using ACTP0Tools.Reflection;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.CustomRegions;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Text;

namespace EntityCore.Entities
{
    public static class EntityDiagnosticTools
    {
        public static string Construct(IEntityDescriptor entityDescriptor)
        {
            if (entityDescriptor != null)
            {
                StringBuilder sb = new StringBuilder();

                //var @object = Traverse.Create(entityDescriptor);

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

                    //Predicate<Entity> crCheck = null;
                    var customRegions = entityDescriptor.GetProperty<CustomRegionCollection>("CustomRegionNames");
                    var customRegionCheck = entityDescriptor.GetProperty<Astral.Quester.Classes.Condition.Presence>("CustomRegionCheck");
                    if (customRegions.IsValid)
                    {
                        var crCollection = customRegions.Value;

                        sb.Append("CustomRegions: ").Append(crCollection);
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
                                                              customRegions.IsValid ? customRegions.Value : null,
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
    }
}
