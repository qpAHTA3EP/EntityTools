using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyNW.Classes;

namespace EntityTools.Tools.BuySellItems
{
    /// <summary>
    /// Список слотов сумки, содержащие предметы, удовлетворяющих категории/фильтру
    /// а также статистические данные по этим предметам
    /// </summary>
    public class SlotCache : IList<InventorySlot>
    {
        public SlotCache() { }
        public SlotCache(bool readOnly = false)
        {
            _readOnly = readOnly;
        }
        public SlotCache(SlotCache source, bool readOnly = false)
        {
            MaxItemLevel = source.MaxItemLevel;
            TotalItemsCount = source.TotalItemsCount;
            _slots = source._slots;
            _readOnly = readOnly;
        }
        public SlotCache(InventorySlot slot)
        {
            MaxItemLevel = slot.Item.ItemDef.Level;
            TotalItemsCount = slot.Item.Count;
            _slots = new List<InventorySlot> { slot };
            _readOnly = false;
        }

        public uint MaxItemLevel { get; private set; }

        public uint TotalItemsCount { get; private set; }

        public ReadOnlyCollection<InventorySlot> Slots { get => _slots.AsReadOnly(); }
        private List<InventorySlot> _slots = new List<InventorySlot>();

        public bool IsReadOnly => _readOnly;
        private bool _readOnly;

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

        public SlotCache Clone()
        {
            return new SlotCache(this);
        }

        public SlotCache AsReadOnly()
        {
            return new SlotCache(this, true);
        }

        public void Add(InventorySlot slot)
        {
            if (!_readOnly)
            {
                _slots.Add(slot);

                if (MaxItemLevel < slot.Item.ItemDef.Level)
                    MaxItemLevel = slot.Item.ItemDef.Level;

                TotalItemsCount += slot.Item.Count;
            }
        }

        public bool Contains(InventorySlot value) => _slots.Contains(value);

        public int IndexOf(InventorySlot value) => _slots.IndexOf(value);

        public void Insert(int index, InventorySlot slot)
        {
            if (!_readOnly)
            {
                _slots.Insert(index, slot);

                if (MaxItemLevel < slot.Item.ItemDef.Level)
                    MaxItemLevel = slot.Item.ItemDef.Level;

                TotalItemsCount += slot.Item.Count;
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
    }

}
