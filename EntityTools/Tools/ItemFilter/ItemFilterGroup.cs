using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using MyNW.Classes;

namespace EntityTools.Tools.ItemFilter
{
    public partial class ItemFilterGroup<TFilterEntry> where TFilterEntry : IFilterEntry
    {
        Predicate<Item> predicate = null;

        public ItemFilterGroup(IGrouping<uint, TFilterEntry> fgroup = null)
        {
            if (fgroup != null)
            {
                foreach (TFilterEntry f in fgroup)
                {
                    TFilterEntry readOnlyFilter = (TFilterEntry)f.AsReadOnly();
                    _filters.Add(readOnlyFilter);
#if disabled_20200624_2357
                    if (readOnlyFilter.Mode == ItemFilterMode.Include)
                        predicate += readOnlyFilter.IsMatch;
                    else predicate += readOnlyFilter.IsForbidden; 
#endif
                }
                _groupInd = fgroup.Key;
            }
            if (_filters.Count == 0)
                predicate = IsMatch_False;
            else predicate = IsMatch_Selector;
        }

        public uint GroupIndex { get; }
        uint _groupInd = uint.MaxValue;

        public IList<TFilterEntry> Filters { get => _filters.AsReadOnly(); }
        private List<TFilterEntry> _filters = new List<TFilterEntry>();

        /// <summary>
        /// Добавление элементов фильтра <paramref name="inFilters"/> в список фильтров группы
        /// </summary>
        /// <param name="inFilters"></param>
        public void AddFilter(IEnumerable<TFilterEntry> inFilters)
        {
            uint num = 0;
            foreach(var f in inFilters)
            {
                if (f.Group == _groupInd)
                {
                    TFilterEntry readOnlyFilter = (TFilterEntry)f.AsReadOnly();
                    _filters.Add(readOnlyFilter);
#if disabled_20200624_2143
                    if (readOnlyFilter.Mode == ItemFilterMode.Include)
                        predicate += readOnlyFilter.IsMatch;
                    else predicate += readOnlyFilter.IsForbidden; 
#endif
                    num++;
                }
            }
            if (num > 0)
                predicate = IsMatch_Selector;
        }

        public void Clear()
        {
            _filters?.Clear();
            predicate = IsMatch_False;
        }

        public bool IsMatch(Item item)
        {
            return predicate(item);
        }
        public bool IsMatch(InventorySlot slot)
        {
            return predicate(slot.Item);
        }
        bool IsMatch_Selector(Item item)
        {
            predicate = null;
            if (_filters != null && _filters.Count > 0)
                predicate = IsMatch_CheckAll;
            else predicate = IsMatch_False;
            return predicate(item);
        }
        bool IsMatch_CheckAll(Item item)
        {
            foreach(TFilterEntry f in _filters)
            {
                if (f.Mode == ItemFilterMode.Include)
                {
                    if (!f.IsMatch(item))
                        return false;
                }
                else if (f.IsForbidden(item))
                    return false;

            }
            return true;
        }
        bool IsMatch_False(Item item)
        {
            return false;
        }

#if disabled_20200621_2146
        public virtual bool IsForbidden(InventorySlot slot)
        {
            return !IsMatch(slot);
        }
        public virtual bool IsForbidden(Item item)
        {
            return !IsMatch(item);
        } 
#endif

        public override string ToString()
        {
            return $"{nameof(ItemFilterGroup<TFilterEntry>)} [{_groupInd}] with {_filters.Count} entires";
        }
    }
    public partial class ItemFilterGroup<TFilterEntry> : IComparable<ItemFilterGroup<TFilterEntry>>
    {
        public int CompareTo(ItemFilterGroup<TFilterEntry> other)
        {
            if (other is null)
                return 1;
            long result = _groupInd - other._groupInd;
            if (result > 0)
                return 1;
            else if (result < 0) return -1;
            return 0;
        }
    }
}
