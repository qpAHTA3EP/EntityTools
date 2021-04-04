using System;
using System.Collections.Generic;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using MyNW.Classes;
using Action = System.Action;

namespace EntityTools.Core
{
    internal class EntityCoreProxy : IEntityToolsCore
    {
        internal static Return Initialize<Return>(ref Func<Return> method)
        {
            throw new NotImplementedException();
        }
        internal static void Initialize(ref Action method)
        {
            throw new NotImplementedException(); 
        }

        public bool Initialize(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(Astral.Quester.Classes.Action action)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(Condition condition)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(UCCAction action)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(UCCCondition condition)
        {
            throw new NotImplementedException();
        }

#if DEVELOPER
        public bool GUIRequest_Item<T>(Func<IEnumerable<T>> source, ref T selectedValue)
        {
            throw new NotImplementedException();
        }

        public string EntityDiagnosticInfos(object obj)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_AuraId(ref string id)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_CustomRegions(ref List<string> crList)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_EntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_NodeLocation(ref Vector3 pos, string caption)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_UCCConditions(ref List<UCCCondition> list)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_UIGenId(ref string id)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_EntityToInteract(ref Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool GUIRequest_UCCAction(out UCCAction action)
        {
            throw new NotImplementedException();
        }
#if DEBUG
        public LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete, bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null, Predicate<Entity> specialCheck = null)
        {
            throw new NotImplementedException();
        } 
#endif
#endif

        public bool CheckCore()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
