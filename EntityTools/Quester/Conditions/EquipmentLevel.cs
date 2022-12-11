using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EntityTools.Quester.Conditions
{
    /// <summary>
    /// Проверка уровня заданного предмета(ов)
    /// </summary>
    [Serializable]
    public class EquipmentLevel : Condition, INotifyPropertyChanged
    {
        public EquipmentLevel()
        {
            itemLevelEvaluator = evaluator_initializer;
            conditionValidator = conditionValidator_initializer;
        }

        #region Опции команды
#if DEVELOPER
        [Description("ID of the bag containing the item(s)")]
        [Category("ItemSlot")]
#else
        [Browsable(false)]
#endif
        public SpecificBags Bag
        {
            get => _bag;
            set
            {
                if (value != _bag)
                {
                    _bag = value;
                    _label = string.Empty;
                    itemLevelEvaluator = evaluator_initializer;
                    OnPropertyChanged();
                }
            }
        }
        private SpecificBags _bag = SpecificBags.WeaponMain;

        [Description("Specify a method to select an item from many which level will be compared to the '" + nameof(Value) + "':\n" +
                     nameof(ItemSelectorType.Any) + ": select a first available item;\n" +
                     nameof(ItemSelectorType.Min) + ": select an item having minimum level;\n" +
                     nameof(ItemSelectorType.Max) + ": select an item having maximum level;\n" +
                     nameof(ItemSelectorType.All) + ": sum all item level.\n" +
                     "The option is ignored if '" + nameof(Bag) + "' differ from '" + nameof(SpecificBags.Inventory) + "' or '" + nameof(SpecificBags.Inventory) + "'")]
        [Category("ItemSlot")]
        public ItemSelectorType ItemSelector
        {
            get => _itemSelectorType;
            set
            {
                if (value != _itemSelectorType)
                {
                    _itemSelectorType = value;
                    _label = string.Empty;
                    itemLevelEvaluator = evaluator_initializer;
                    OnPropertyChanged();
                }
            }
        }
        private ItemSelectorType _itemSelectorType = ItemSelectorType.Min;

#if DEVELOPER
        [Description("Filter of the item Id.\n" +
                     "Ignored if empty than means 'All items'")]
        [Category("ItemFilter")]
#else
        [Browsable(false)]
#endif
        public string ItemFilter
        {
            get => _itemFiler;
            set
            {
                if (_itemFiler != value)
                {
                    _itemFiler = value;
                    itemLevelEvaluator = evaluator_initializer;
                    OnPropertyChanged();
                }
            }
        }
        private string _itemFiler = string.Empty;

#if DEVELOPER
        [Description("Type of the '" + nameof(ItemFilter) + "' property:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("ItemFilter")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType ItemFilterType
        {
            get => _itemFilerType;
            set
            {
                if (_itemFilerType != value)
                {
                    _itemFilerType = value;
                    OnPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _itemFilerType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("Threshold value of the specified item(s) level")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public uint Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    _label = string.Empty;
                    OnPropertyChanged();
                }
            }
        }
        private uint _value;

#if DEVELOPER
        [Description("Ratio of the '" + nameof(Value) + "' and the level of the specified item(s)")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign;
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    _label = string.Empty;
                    conditionValidator = conditionValidator_initializer;
                    OnPropertyChanged();
                }
            }
        }
        internal Relation _sign = Relation.Inferior;
        
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if false
            _label = string.Empty;
            itemLevelEvaluator = evaluator_initializer;
            conditionValidator = conditionValidator_initializer; 
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool IsValid => conditionValidator(itemLevelEvaluator());

        public override void Reset() {}

        public override string TestInfos
        {
            get
            {
                var sb = new StringBuilder();
                int itemLvl;
                switch (_bag)
                {
                    case SpecificBags.Inventory:
                    case SpecificBags.Bank:
                        itemLvl = itemLevelEvaluator();
                        switch (_itemSelectorType)
                        {
                            case ItemSelectorType.Any:
                                sb.AppendFormat("First item from the {0} has the level {1}", _bag, itemLvl);
                                break;
                            case ItemSelectorType.Min:
                            case ItemSelectorType.Max:
                                sb.AppendFormat("The {0} level of items from the {1} is {2}", _itemSelectorType, _bag, itemLvl);
                                break;
                            case ItemSelectorType.All:
                                sb.AppendFormat("Total items level into the {0} is {1}", _bag, itemLvl);
                                break;
                        }
                        break;
                    case SpecificBags.Head:
                    case SpecificBags.Body:
                    case SpecificBags.Arms:
                    case SpecificBags.Waist:
                    case SpecificBags.Feet:
                    case SpecificBags.WeaponMain:
                    case SpecificBags.WeaponSecondary:
                    case SpecificBags.Shirt:
                    case SpecificBags.Paints:
                    case SpecificBags.RingLeft:
                    //case SpecificBags.RingRight:
                    //case SpecificBags.ArtifactPrimary:
                    //case SpecificBags.ArtifactSecondary1:
                    //case SpecificBags.ArtifactSecondary2:
                    //case SpecificBags.ArtifactSecondary3:
                        var slot = InventoryHelper.GetEquipmentSlot(_bag);
                        var item = slot?.Item;
                        if (item is null || !item.IsValid)
                        {
                            sb.AppendFormat("Slot {0} does not contain an item", _bag);
                            return sb.ToString();
                        }

                        var itemDef = item.ItemDef;
                        sb.AppendFormat("Slot {0} contains an item '{1}'[{2}]", _bag, itemDef.DisplayName, itemDef.InternalName).AppendLine();
                        if (!string.IsNullOrEmpty(_itemFiler))
                        {
                            var isMatch = _itemFiler.GetComparer(_itemFilerType).Invoke(itemDef.InternalName);
                            sb.AppendFormat("Item {0} the pattern {1}", isMatch? "matches" : "does not match", _itemFiler).AppendLine();
                        }

                        var itemTierDef = item.ItemProgression_GetItemTierDef();
                        if (!itemTierDef.IsValid)
                        {
                            sb.AppendLine("Item is Artifact which level can't be evaluated");
                        }
                        else
                        {
                            itemLvl = GetLevel(slot);
                            sb.AppendFormat("Item level of the {0} is {1}", _bag, itemLvl);
                        }
                        break;
                }

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (_bag == SpecificBags.Inventory
                    || _bag == SpecificBags.Bank)
                {
                    switch (_itemSelectorType)
                    {
                        case ItemSelectorType.Any:
                            _label = $"Check the level for the first item into the {{ {_bag} }} {_sign} {_value}";
                            break;
                        case ItemSelectorType.Min:
                        case ItemSelectorType.Max:
                            _label = $"Check the {_itemSelectorType} level of items into the {{ {_bag} }} {_sign} {_value}";
                            break;
                        case ItemSelectorType.All:
                            _label = $"Check total items level into the {{ {_bag} }} {_sign} {_value}";
                            break;
                    }
                }
                else _label = $"Check item level on the '{_bag}' {_sign} {_value}";
            }

            return _label;
        }
        private string _label;

        #region Вспомогательные методы
        /// <summary>
        /// Функтор вычисляющий уровень предмета
        /// </summary>
        private Func<int> itemLevelEvaluator;

        /// <summary>
        /// Метод авто-инициализации функтора <see cref="itemLevelEvaluator"/>
        /// Производит предварительный анализ опций и конструирует функтор <seealso cref="itemLevelEvaluator"/>
        /// </summary>
        /// <returns></returns>
        private int evaluator_initializer()
        {
            Func<int> func = null;

            if (string.IsNullOrEmpty(_itemFiler))
            {
                switch (_bag)
                {
                    case SpecificBags.Inventory:
                        //var inventory = InventoryHelper.GetInventorySlots();
                        switch (_itemSelectorType)
                        {
                            case ItemSelectorType.Any:
                                func = () => GetLevel(InventoryHelper.EnumerateInventorySlots().FirstOrDefault());
                                //result = GetLevel(inventory.FirstOrDefault());
                                break;
                            case ItemSelectorType.Min:
                                func = () => GetMinimum(InventoryHelper.EnumerateInventorySlots(), out _);
                                //result = inventory.Min(GetLevel);
                                break;
                            case ItemSelectorType.Max:
                                func = () => GetMaximum(InventoryHelper.EnumerateInventorySlots(), out _);
                                //result = inventory.Max(GetLevel);
                                break;
                            case ItemSelectorType.All:
                                func = () => GetSum(InventoryHelper.EnumerateInventorySlots());
                                //result = inventory.Sum(GetLevel);
                                break;
                        }
                        break;
                    case SpecificBags.Bank:
                        switch (_itemSelectorType)
                        {
                            case ItemSelectorType.Any:
                                func = () => GetLevel(InventoryHelper.EnumerateBankSlots().FirstOrDefault());
                                break;
                            case ItemSelectorType.Min:
                                func = () => InventoryHelper.EnumerateBankSlots().Min(GetLevel);
                                break;
                            case ItemSelectorType.Max:
                                func = () => InventoryHelper.EnumerateBankSlots().Max(GetLevel);
                                break;
                            case ItemSelectorType.All:
                                func = () => GetSum(InventoryHelper.EnumerateBankSlots());
                                break;
                        }
                        break;
                    case SpecificBags.RingLeft:
                        func = () => GetLevel(InventoryHelper.GetRingLeft());
                        break;
                    case SpecificBags.RingRight:
                        func = () => GetLevel(InventoryHelper.GetRingRight());
                        break;
                    default:
                        func = () => GetLevel(InventoryHelper.GetEquipmentSlot(_bag));
                        break;
                }
            }
            else
            {
                var itemNameFilter = _itemFiler.GetCompareFunc(_itemFilerType, (InventorySlot s) => s.Item.ItemDef.InternalName);

                switch (_bag)
                {
                    case SpecificBags.Inventory:
                        //var inventory = InventoryHelper.GetInventorySlots();
                        switch (_itemSelectorType)
                        {
                            case ItemSelectorType.Any:
                                func = () => GetLevel(InventoryHelper.EnumerateInventorySlots().FirstOrDefault(itemNameFilter));
                                //result = GetLevel(inventory.FirstOrDefault());
                                break;
                            case ItemSelectorType.Min:
                                func = () => GetMinimum(InventoryHelper.EnumerateInventorySlots().Where(itemNameFilter), out _);
                                //result = inventory.Min(GetLevel);
                                break;
                            case ItemSelectorType.Max:
                                func = () => GetMaximum(InventoryHelper.EnumerateInventorySlots().Where(itemNameFilter), out _);
                                //result = inventory.Max(GetLevel);
                                break;
                            case ItemSelectorType.All:
                                func = () => GetSum(InventoryHelper.EnumerateInventorySlots().Where(itemNameFilter));
                                //result = inventory.Sum(GetLevel);
                                break;
                        }
                        break;
                    case SpecificBags.Bank:
                        switch (_itemSelectorType)
                        {
                            case ItemSelectorType.Any:
                                func = () => GetLevel(InventoryHelper.EnumerateBankSlots().FirstOrDefault(itemNameFilter));
                                break;
                            case ItemSelectorType.Min:
                                func = () => GetMinimum(InventoryHelper.EnumerateBankSlots().Where(itemNameFilter), out _);
                                break;
                            case ItemSelectorType.Max:
                                func = () => GetMaximum(InventoryHelper.EnumerateBankSlots().Where(itemNameFilter), out _);
                                break;
                            case ItemSelectorType.All:
                                func = () => GetSum(InventoryHelper.EnumerateBankSlots().Where(itemNameFilter));
                                break;
                        }
                        break;
                    default:
                        func = () =>
                        {
                            var s = InventoryHelper.GetEquipmentSlot(_bag);
                            return itemNameFilter(s) ? GetLevel(s) : -1;
                        };
                        break;
                }
            }

            if (func is null)
            {
                itemLevelEvaluator = () => -1;
                return -1;
            }

            itemLevelEvaluator = func;

            return itemLevelEvaluator();
        }

        /// <summary>
        /// Вычисление уровня предмета в слоте <param name="slot"/>
        /// </summary>
        /// <returns>-1 для артефактных предметов</returns>
        private int GetLevel(InventorySlot slot)
        {
            if (slot is null
                || !slot.IsValid
                || !slot.Filled)
                return -1;

            return GetLevel(slot.Item);
        }
        /// <summary>
        /// Вычисление уровня предмета <param name="item"/>
        /// </summary>
        /// <returns></returns>
        private int GetLevel(Item item)
        {
            var itemDef = item?.ItemDef;
            if (itemDef?.IsValid == true)
                return (int)itemDef.Level;
            return -1;
        }

        /// <summary>
        /// Поиск предмета, имеющего минимальный уровень
        /// Артефактные (улучшаемые) предметы игнорируются
        /// </summary>
        /// <param name="slots">перебираемые предметы</param>
        /// <param name="minItemLevelSlot">слот с предметом, имеющим минимальный уровень</param>
        /// <returns>Уровень найденного предмета.
        /// -1, если предмет не найден</returns>
        private int GetMinimum(IEnumerable<InventorySlot> slots, out InventorySlot minItemLevelSlot)
        {
            int result = int.MaxValue;
            minItemLevelSlot = null;

            foreach (var slot in slots)
            {
                var item = slot.Item;

                if (!item.IsValid 
                    || !item.IsEquippable())
                    continue;

                var lvl = GetLevel(item);
                if (lvl >= 0 && lvl < result)
                {
                    result = lvl;
                    minItemLevelSlot = slot;
                }
            }

            if (minItemLevelSlot == null)
            {
                result = -1;
            }

            return result;
        }


        /// <summary>
        /// Поиск предмета, имеющего максимальный уровень
        /// Артефактные (улучшаемые) предметы игнорируются
        /// </summary>
        /// <param name="slots">перебираемые предметы</param>
        /// <param name="maxItemLevelSlot">слот с предметом, имеющим минимальный уровень</param>
        /// <returns>Уровень найденного предмета.
        /// -1, если предмет не найден</returns>
        private int GetMaximum(IEnumerable<InventorySlot> slots, out InventorySlot maxItemLevelSlot)
        {
            int result = -1;
            maxItemLevelSlot = null;

            foreach (var slot in slots)
            {
                var item = slot.Item;

                if (!item.IsValid
                    || !item.IsEquippable())
                    continue;

                var lvl = GetLevel(item);
                if (lvl >= 0 && lvl > result)
                {
                    result = lvl;
                    maxItemLevelSlot = slot;
                }
            }

            return result;
        }

        /// <summary>
        /// Суммирование уровня предметов в <param name="slots"/>
        /// Артефактные (улучшаемые) предметы игнорируются
        /// </summary>
        /// <param name="slots">перебираемые предметы</param>
        /// <returns>Суммарный уровень предметов.
        /// -1, если предметов не найдено</returns>
        private int GetSum(IEnumerable<InventorySlot> slots)
        {
            int result = 0;
            int count = 0;
            foreach (var slot in slots)
            {
                var item = slot.Item;

                if (!item.IsValid
                    || !item.IsEquippable())
                    continue;

                var lvl = GetLevel(item);
                if (lvl >= 0)
                {
                    result += lvl;
                    count++;
                }
            }


            return (count > 0) ? result : -1;
        }

        /// <summary>
        /// Предикат, проверяющий соотношение <see cref="Sign"/>> уровня предмета с референтным значением <see cref="Value"/>
        /// </summary>
        private Predicate<int> conditionValidator;

        /// <summary>
        /// Метод авто-инициализации предиката <see cref="conditionValidator"/>
        /// Производит предварительный анализ опций и конструирует предикат <seealso cref="conditionValidator"/>
        /// </summary>
        /// <returns></returns>
        private bool conditionValidator_initializer(int itemLvl)
        {
            Predicate<int> pred = null;

            switch (_sign)
            {
                case Relation.Equal:
                    pred = iLvl =>
                    {
                        if (iLvl < 0)
                            return false;

                        return iLvl == _value;
                    };
                    break;
                case Relation.NotEqual:
                    pred = (iLvl) =>
                    {
                        if (iLvl < 0)
                            return false;

                        return iLvl != _value;
                    };

                    break;
                case Relation.Inferior:
                    pred = (iLvl) =>
                    {
                        if (iLvl < 0)
                            return false;

                        return iLvl < _value;
                    };

                    break;
                case Relation.Superior:
                    pred = (iLvl) =>
                    {
                        if (iLvl < 0)
                            return false;

                        return iLvl > _value;
                    };
                    break;
            }

            if (pred is null)
            {
                conditionValidator = (_) => false;
                return false;
            }

            conditionValidator = pred;

            return conditionValidator(itemLvl);
        }
        #endregion
    }
}