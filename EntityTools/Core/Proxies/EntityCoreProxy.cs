using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using DevExpress.Utils;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
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
        public bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayName = "")
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, ListControlConvertEventHandler itemFormatter)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_SelectItemList<T>(Func<IEnumerable<T>> source, ref IList<T> selectedValue, string caption = "")
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_EditValue(ref string value, string message = "", string caption = "", FormatInfo formatInfo = null)
        {
            throw new NotImplementedException();
        }

        public string EntityDiagnosticInfos(object obj)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_SelectAuraId(ref string id)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_EditCustomRegionList(ref List<string> crList)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_EditEntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_GetNodeLocation(ref Vector3 pos, string caption, string message = "")
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_EditUccConditions(ref List<UCCCondition> list)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_EditUccConditions(ref List<UCCCondition> list, ref LogicRule logic, ref bool negation)
        {
            throw new NotImplementedException();
        }
        
        public bool UserRequest_SelectUIGenId(ref string id)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_GetPosition(ref Vector3 pos, string caption, string message = "")
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_GetEntityToInteract(ref Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool UserRequest_GetUccAction(out UCCAction action)
        {
            throw new NotImplementedException();
        }

        public void Monitor(object monitor)
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
