using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using UCCConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.Core.Interfaces
{
    public interface IEntityToolsCore
    {
        bool Initialize(object obj);
        bool Initialize(Astral.Quester.Classes.Action action);
        bool Initialize(Astral.Quester.Classes.Condition condition);
        bool Initialize(Astral.Logic.UCC.Classes.UCCAction action);
        bool Initialize(Astral.Logic.UCC.Classes.UCCCondition condition);
#if DEVELOPER
        bool GUIRequest_AuraId(ref string id);
        bool GUIRequest_UIGenId(ref string id);
        bool GUIRequest_EntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType);
        bool GUIRequest_UCCConditions(ref UCCConditionList list);

        bool GUIRequest_CustomRegions(ref List<string> crList);
        bool GUIRequest_NodeLocation(ref Vector3 pos, string caption);

        string EntityDiagnosticInfos(object obj);
#endif
#if DEBUG
        LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null);
#endif
    }
}
