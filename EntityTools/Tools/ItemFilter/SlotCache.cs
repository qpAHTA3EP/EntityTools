using MyNW.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EntityTools.Tools.ItemFilter
{
    /// <summary>
    /// Список слотов сумки, содержащие предметы, удовлетворяющих категории/фильтру
    /// а также статистические данные по этим предметам
    /// </summary>
    public partial class SlotCache<TFilterEntry> where  TFilterEntry : IFilterEntry { }
    public partial class SlotCache<TFilterEntry> : IList<InventorySlot>, IComparable<SlotCache<TFilterEntry>>
    {
        public SlotCache(ItemFilterGroup<TFilterEntry> filterGrp)
        {
            _filterGroup = filterGrp;
        }
        public SlotCache(ItemFilterGroup<TFilterEntry> filterGrp, bool readOnly = false)
        {
            _filterGroup = filterGrp;
            _readOnly = readOnly;
        }
        public SlotCache(SlotCache<TFilterEntry> source, bool readOnly = false)
        {
            _filterGroup = source._filterGroup;
            MaxItemLevel = source.MaxItemLevel;
            TotalItemsCount = source.TotalItemsCount;
            _slots = source._slots;
            _readOnly = readOnly;
        }
        public SlotCache(ItemFilterGroup<TFilterEntry> filterGrp, InventorySlot slot)
        {
            _filterGroup = filterGrp;
            MaxItemLevel = slot.Item.ItemDef.Level;
            TotalItemsCount = slot.Item.Count;
            _slots = new List<InventorySlot>() { slot };
            _readOnly = false;
        }

        public ItemFilterGroup<TFilterEntry> FilterGroup { get => _filterGroup; }
        ItemFilterGroup<TFilterEntry> _filterGroup = null;

        public uint MaxItemLevel { get; private set; }

        public uint TotalItemsCount { get; private set; }

        //TODO: Реализовать обновление TotalMaxLevelItemCount при добавлении, удалении и т.д.
        public uint TotalMaxLevelItemCount { get; private set; }

        public IList<InventorySlot> Slots { get => _slots.AsReadOnly(); }
        private List<InventorySlot> _slots = new List<InventorySlot>();


        //TODO: Обновление списка обновляемых слотов при добавлении, удалении и т.д. а также общей статистики
        public IList<InventorySlot> UpgradableSlots { get => _upgradableSlots.AsReadOnly(); }
        private List<InventorySlot> _upgradableSlots = new List<InventorySlot>();

        public bool IsReadOnly => _readOnly;
        private bool _readOnly = false;

        public bool IsFixedSize => throw new NotImplementedException();

        public int Count => _slots.Count;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        public InventorySlot this[int index]
        {
            get => _slots[index];
            set
            {
                _slots[index] = value;
            }
        }

        public void Clear()
        {
            if (!_readOnly)
            {
                MaxItemLevel = 0;
                TotalItemsCount = 0;
                _slots.Clear();
            }
        }

        public SlotCache<TFilterEntry> Clone()
        {
            return new SlotCache<TFilterEntry>(this);
        }

        public SlotCache<TFilterEntry> AsReadOnly()
        {
            if (_readOnly)
                return this;
            else return new SlotCache<TFilterEntry>(this, true);
        }

        public void Add(InventorySlot slot)
        {
            if (!_readOnly)
            {
                if (_filterGroup.IsMatch(slot.Item))
                {
                    _slots.Add(slot);

                    if (MaxItemLevel < slot.Item.ItemDef.Level)
                        MaxItemLevel = slot.Item.ItemDef.Level;

                    TotalItemsCount += slot.Item.Count;
                }
            }
        }

        public bool Contains(InventorySlot value) => _slots.Contains(value);

        public int IndexOf(InventorySlot value) => _slots.IndexOf(value);

        public void Insert(int index, InventorySlot slot)
        {
            if (!_readOnly)
            {
                if (_filterGroup.IsMatch(slot.Item))
                {
                    _slots.Insert(index, slot);

                    if (MaxItemLevel < slot.Item.ItemDef.Level)
                    {
                        MaxItemLevel = slot.Item.ItemDef.Level;
                        TotalMaxLevelItemCount = slot.Item.Count;
                    }
                    else if (MaxItemLevel == slot.Item.ItemDef.Level)
                        TotalMaxLevelItemCount += slot.Item.Count;

                    TotalItemsCount += slot.Item.Count;
                }
            }
        }

        public bool Remove(InventorySlot slot)
        {
            if (!_readOnly)
            {
                if (_slots.Remove(slot))
                {
                    if (MaxItemLevel >= slot.Item.ItemDef.Level)
                        MaxItemLevel = _slots.Max(s => s.Item.ItemDef.Level);

                    _upgradableSlots.Remove(slot);

                    TotalItemsCount -= slot.Item.Count;

                    return true;
                }
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (!_readOnly)
            {
                InventorySlot slot = _slots[index];
                _slots.RemoveAt(index);
                _upgradableSlots.Remove(slot);

                if (MaxItemLevel >= slot.Item.ItemDef.Level)
                    MaxItemLevel = _slots.Max(s => s.Item.ItemDef.Level);


                TotalItemsCount -= slot.Item.Count;
            }
        }

        public void CopyTo(InventorySlot[] array, int arrayIndex)
        {
            _slots.CopyTo(array, arrayIndex);
        }

        public IEnumerator<InventorySlot> GetEnumerator()
        {
            return _slots.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int CompareTo(SlotCache<TFilterEntry> other)
        {
            if (other is null)
                return 1;
            uint result = _filterGroup.GroupIndex - other._filterGroup.GroupIndex;
            if (result > 0)
                return 1;
            else if (result < 0)
                return -1;
            return 0;
        }
    }

}
