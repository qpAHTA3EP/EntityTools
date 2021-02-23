using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using Action = Astral.Quester.Classes.Action;
using UCCConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.Core.Interfaces
{
    public interface IEntityToolsCore
    {
        bool CheckCore();

        bool Initialize(object obj);
        bool Initialize(Action action);
        bool Initialize(Condition condition);
        bool Initialize(UCCAction action);
        bool Initialize(UCCCondition condition);
#if DEVELOPER
        bool GUIRequest_Item<T>(Func<IEnumerable<T>> source, ref T selectedValue);
        bool GUIRequest_AuraId(ref string id);
        bool GUIRequest_UIGenId(ref string id);
        bool GUIRequest_EntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType);
        bool GUIRequest_UCCConditions(ref UCCConditionList list);

        bool GUIRequest_CustomRegions(ref List<string> crList);
        bool GUIRequest_NodeLocation(ref Vector3 pos, string caption);
        bool GUIRequest_EntityToInteract(ref Entity entity);

        bool GUIRequest_UCCAction(out UCCAction action);

        string EntityDiagnosticInfos(object obj/*, bool isTarget = false*/);
#endif
#if DEBUG
        LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null);
#endif
    }
}
