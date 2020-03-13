using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
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

        bool GUIRequest_AuraId(ref string id);
        bool GUIRequest_UIGenId(ref string id);
        EntityDef GUIRequest_EntityId(string entPattern, ItemFilterStringType strMatchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated);
        UCCConditionList GUIRequest_UCCConditions(UCCConditionList list);

        bool GUIRequest_CustomRegions(ref List<string> crList);
        bool GUIRequest_NodeLocation(ref Vector3 pos, string caption);

        string EntityDiagnosticInfos(object obj);
    }
}
