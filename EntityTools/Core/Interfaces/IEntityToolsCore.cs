using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using DevExpress.Utils;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuesterAction = Astral.Quester.Classes.Action;
using UCCConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.Core.Interfaces
{
    public interface IEntityToolsCore : IDisposable
    {
        bool CheckCore();

        bool Initialize(object obj);
        bool Initialize(QuesterAction action);
        bool Initialize(Condition condition);
        bool Initialize(UCCAction action);
        bool Initialize(UCCCondition condition);
#if DEVELOPER
        bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayName = "");

        bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue,
            ListControlConvertEventHandler itemFormatter);

        bool UserRequest_SelectItemList<T>(Func<IEnumerable<T>> source, ref IList<T> selectedValues, string caption = "");

        bool UserRequest_EditValue(ref string value, string message = "", string caption = "",
            FormatInfo formatInfo = null);

        bool UserRequest_SelectAuraId(ref string id);

        bool UserRequest_SelectUIGenId(ref string id);

        bool UserRequest_EditEntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType);

        bool UserRequest_EditUccConditions(ref UCCConditionList list);

        bool UserRequest_EditUccConditions(ref UCCConditionList list, ref LogicRule logic, ref bool negation);
        
        bool UserRequest_EditCustomRegionList(ref List<string> crList);
        bool UserRequest_GetNodeLocation(ref Vector3 pos, string caption);
        bool UserRequest_GetEntityToInteract(ref Entity entity);

        bool UserRequest_GetUccAction(out UCCAction action);

        string EntityDiagnosticInfos(object obj);

        void Monitor(object monitor);
#endif
#if DEBUG
        LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, 
                                         EntityNameType nameType = EntityNameType.NameUntranslated, 
                                         EntitySetType setType =  EntitySetType.Complete,
                                         bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, 
                                         List<CustomRegion> customRegions = null,
                                         Predicate<Entity> specialCheck = null);
#endif
    }
}
