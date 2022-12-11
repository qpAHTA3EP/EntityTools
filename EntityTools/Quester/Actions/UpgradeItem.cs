using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using MyNW.Classes.ItemProgression;
using MyNW.Internals;
using MyNW.Patchables.Enums;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class UpgradeItem : Astral.Quester.Classes.Action
    {

        #region Опции команды
        /// <summary>
        /// Идентификатор предмета, который нужно "обработать"
        /// </summary>
        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        [Category("Item")]
        [Description("Идентификатор обрабатываемого предмета.")]
        public string ItemId
        {
            get => _itemId;
            set
            {
                _itemIdPredicate = null;
                _itemId = value;
            }
        }
        private string _itemId;

        /// <summary>
        /// Переключатель типа предмета
        /// </summary>
        [Category("Item")]
        [Description("Переключатель типа идентификатор предмета:\n" +
                     "Shimple : текстовая строка, допускающая символ подстановки '*' в начале или в конце.\n" +
                     "Regex : регулярное выражение.")]
        public ItemFilterStringType ItemIdType
        {
            get => _itemIdType;
            set
            {
                _itemIdPredicate = null;
                _itemIdType = value;
            }
        }
        private ItemFilterStringType _itemIdType = ItemFilterStringType.Simple;

        /// <summary>
        /// Флаг, позволяющий обрабатывать предметы, привязанные к персонажу
        /// </summary>
        [Category("Item")]
        [DisplayName("AllowBoundToCharacter")]
        [Description("Флаг, позволяющий обрабатывать предметы, привязанные к персонажу.")]
        public bool B2C
        {
            get => _b2c;
            set
            {
                if (_b2c != value)
                {
                    _b2c = value;
                    _itemIdPredicate = null;
                }
            }
        }

        private bool _b2c;


        /// <summary>
        /// Флаг, позволяющий обрабатывать предметы, привязанные к аккаунту
        /// </summary>
        [Category("Item")]
        [DisplayName("AllowBoundToAccount")]
        [Description("Флаг, позволяющий обрабатывать предметы, привязанные к аккаунту.")]
        public bool B2A
        {
            get => _b2a;
            set
            {
                if (_b2a != value)
                {
                    _b2a = value;
                    _itemIdPredicate = null;
                }
            }
        }

        private bool _b2a;

        /// <summary>
        /// Принудительное улочшение без учета ProgressionLogic
        /// </summary>
        [Category("Options")]
        [Description("Флаг, принудительного улучшения предмета, отключающий проверки возможности улучешния и наличия необходимых компонентов.\n" +
                     "Требуется для улучшения квестовых (обучающих) предметов, и первого улучшения некоторых предметов.\n" +
                     "Применять следует с осторожностью, так как может спровоцировать отключение игрового клиента от сервера или аварийное завершение программы.")]
        public bool Forced
        {
            get => _forced;
            set
            {
                if (_forced != value)
                {
                    _forced = value;
                    _itemIdPredicate = null;
                }
            }
        }
        private bool _forced;

        [Category("Options")]
        [Description("Mote type required in evolution.")]
        public MoteType Mote { get; set; } = MoteType.None;

        [Category("Options")]
        [Description("The list of the Catalysts with have to be protected by [Preservation Ward].")]
        [Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        public ItemFilterCore ProtectedCatalysts { get; set; } = new ItemFilterCore();
        public bool ShouldSerializeProtectedCatalysts() => ProtectedCatalysts.Entries.Count > 0; 
        #endregion





        /// <summary>
        /// Функтор, определяющий соответствие <see cref="InventorySlot"/> группе идентификаторов:
        /// <see cref="ItemId"/>, <see cref="ItemIdType"/>, <see cref="B2C"/>  <see cref="B2A"/>
        /// </summary>
        [Browsable(false)]
        private Predicate<InventorySlot> ItemIdPredicate
        {
            get
            {
                if (_itemIdPredicate is null)
                {
                    var internalNamePredicate = _itemId.GetComparer(_itemIdType, (InventorySlot s) => s.Item.ItemDef.InternalName);
                    if (_b2a)
                    {
                        if (_b2c)
                        {
                            // Разрешено обрабатывать предметы с любой привязкой
                            _itemIdPredicate = internalNamePredicate;
                        }
                        else
                        {
                            // Разрешено использовать свободные и предметы с привязкой к аккаунту
                            // т.е. запрещено использовать предметы с привязкой к персонажу ItemFlags.Bound
                            _itemIdPredicate = s =>
                            {
                                if (s is null || !s.IsValid)
                                    return false;

                                var item = s.Item;
                                var flag = (ItemFlags)item.Flags;

                                if ((flag & ItemFlags.Bound) > 0)
                                    return false;

                                return internalNamePredicate(s);
                            };
                        }
                    }
                    else
                    {
                        if (_b2c)
                        {
                            // Разрешено использовать свободные и предметы с привязкой к персонажу
                            // т.е. запрещено использовать предметы с привязкой к аккаунту ItemFlags.BoundToAccount
                            _itemIdPredicate = s =>
                            {
                                if (s is null || !s.IsValid)
                                    return false;

                                var item = s.Item;
                                var flag = (ItemFlags)item.Flags;

                                if ((flag & ItemFlags.BoundToAccount) > 0)
                                    return false;

                                return internalNamePredicate(s);
                            };
                        }
                        else
                        {
                            // Разрешено использовать только свободные предметы
                            // т.е. запрещено использовать предметы с привязкой к аккаунту или персонажу
                            var anyBoundFlags = ItemFlags.Bound | ItemFlags.BoundToAccount;

                            _itemIdPredicate = s =>
                            {
                                if (s is null || !s.IsValid)
                                    return false;

                                var item = s.Item;
                                var flag = (ItemFlags)item.Flags;

                                if ((flag & anyBoundFlags) > 0)
                                    return false;

                                return internalNamePredicate(s);
                            };
                        }
                    }
                }

                return _itemIdPredicate;
            }
        }
        private Predicate<InventorySlot> _itemIdPredicate;

        public override string ActionLabel
        {
            get
            {
                var id = ItemId;
                if (string.IsNullOrEmpty(id))
                    return nameof(UpgradeItem);
                return nameof(UpgradeItem) + " [" + ItemId + "]";
            }
        }


        public override bool NeedToRun => true;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => !string.IsNullOrEmpty(ItemId);

        protected override Vector3 InternalDestination => Vector3.Empty;
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(ItemId))
                    return new ActionValidity(nameof(ItemId) + " is empty");
                return new ActionValidity();
            }
        }

        public override void GatherInfos() {}
        public override void InternalReset() {}
        public override void OnMapDraw(GraphicsNW graph) {}

        public override ActionResult Run()
        {
            var searchResult = FindSlot(ItemIdPredicate, Mote, out InventorySlot itemSlot, out InventorySlot moteSlot, out List<InventorySlot> wards);
            
            if(searchResult == SearchResult.Absent)
                return ActionResult.Skip;

            if (!Forced && !HaveRequiredCatalysts(itemSlot))
                return ActionResult.Skip;

            var wardSlots = MakeWardListForCatalysts(itemSlot.Item, ProtectedCatalysts, wards);
            if (wardSlots is null)
            {
                if (!Forced)
                    return ActionResult.Skip;
                wardSlots = new List<InventorySlot>{new InventorySlot(IntPtr.Zero)};
            }

            if (Mote != MoteType.None)
            { 
                if(moteSlot is null)
                    return ActionResult.Skip;
                itemSlot.Evolve(moteSlot, wardSlots);
            }
            else itemSlot.Evolve(wardSlots);

            return ActionResult.Completed;
        }

        /// <summary>
        /// Поиск слота инвентаря, содержащего предмет, соответствующие <param name="predicate"/>
        /// </summary>
        /// <param name="predicate">Критерий отбора предметов. Как правило - идентификатор</param>
        /// <param name="itemSlot">Найденный слот инвентаря</param>
        /// <returns>Перечисление, описывающее результаты поиска</returns>
        private static SearchResult FindSlot(Predicate<InventorySlot> predicate, out InventorySlot itemSlot, out List<InventorySlot> wardSlots)
        {
            itemSlot = null;

            InventorySlot unboundedItem = null;
            InventorySlot bounded2CharacterItem = null;
            InventorySlot bounded2AccountItem = null;
            wardSlots = new List<InventorySlot>(5);

            var player = EntityManager.LocalPlayer;
            
            foreach (var s in player.BagsItems)
            {
                if (predicate(s))
                {
                    var flag = (ItemFlags)s.Item.Flags;

                    if ((flag & ItemFlags.Bound) > 0)
                        bounded2CharacterItem = s;
                    else if ((flag & ItemFlags.BoundToAccount) > 0)
                        bounded2AccountItem = s;
                    else unboundedItem = s;
                }
                else if(IsFuseWardPreservation(s))
                {
                    wardSlots.Add(s);
                }
            }

            if (unboundedItem != null && unboundedItem.IsValid && unboundedItem.Filled)
            {
                itemSlot = unboundedItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found unbounded item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Unbounded;
            }

            if (bounded2AccountItem != null && bounded2AccountItem.IsValid && bounded2AccountItem.Filled)
            {
                itemSlot = bounded2AccountItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found bounded to account item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Bounded2Account;
            }

            if (bounded2CharacterItem != null && bounded2CharacterItem.IsValid && bounded2CharacterItem.Filled)
            {
                itemSlot = bounded2CharacterItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found bounded to character item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Bounded2Character;
            }

            Logger.WriteLine(Logger.LogType.Log, $"No item found.");
            return SearchResult.Absent;
        }

        /// <summary>
        /// Поиск слотов инвентаря, содержащих предмет, соответствующий <param name="predicate"/>, и катализатор
        /// </summary>
        /// <param name="predicate">Критерий отбора предметов. Как правило - идентификатор</param>
        /// <param name="moteType">Тип частицы</param>
        /// <param name="itemSlot">Слот инвентаря, содержащего искомый предмет</param>
        /// <param name="moteSlot">Слот инвентаря, содержащий найденную частицу</param>
        /// <param name="wardSlots">Слоты инвентаря, содержащие найденные защитные катализаторы</param>
        /// <returns>Перечисление, описывающее результаты поиска</returns>
        private static SearchResult FindSlot(Predicate<InventorySlot> predicate, MoteType moteType, out InventorySlot itemSlot, out InventorySlot moteSlot, out List<InventorySlot> wardSlots)
        {
            itemSlot = null;
            moteSlot = null;
            wardSlots = new List<InventorySlot>(5);

            if (moteType == MoteType.None)
                return FindSlot(predicate, out itemSlot, out wardSlots);

            InventorySlot unboundedItem = null;
            InventorySlot bounded2CharacterItem = null;
            InventorySlot bounded2AccountItem = null;

            InventorySlot unboundedMote = null;
            InventorySlot bounded2CharacterMote = null;
            InventorySlot bounded2AccountMote = null;

            var player = EntityManager.LocalPlayer;

            Predicate<InventorySlot> motePredicate;
            switch (moteType)
            {
                case MoteType.Mote_1:
                    motePredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_1", StringComparison.Ordinal);
                    break;
                case MoteType.Mote_2:
                    motePredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_2", StringComparison.Ordinal);
                    break;
                case MoteType.Mote_5:
                    motePredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_5", StringComparison.Ordinal);
                    break;
                case MoteType.Mote_10:
                    motePredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_10", StringComparison.Ordinal);
                    break;
                case MoteType.Mote_100:
                    motePredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Coalescent", StringComparison.Ordinal);
                    break;
                case MoteType.AnyProbableMote:
                    motePredicate = slot =>
                    {
                        var internalName = slot.Item.ItemDef.InternalName;
                        return internalName.StartsWith("Fuse_Ward_", StringComparison.Ordinal)
                            && internalName.Length >=10 && !char.IsDigit(internalName[10]);
                    };
                    break;
                default:
                    motePredicate = slot => false;
                    break;
            }

            foreach (var s in player.BagsItems)
            {
                if (predicate(s))
                {
                    var flag = (ItemFlags)s.Item.Flags;

                    if ((flag & ItemFlags.Bound) > 0)
                        bounded2CharacterItem = s;
                    else if ((flag & ItemFlags.BoundToAccount) > 0)
                        bounded2AccountItem = s;
                    else unboundedItem = s;
                }
                else if (bounded2CharacterMote is null
                         && motePredicate(s))
                {
                    var flag = (ItemFlags)s.Item.Flags;

                    if ((flag & ItemFlags.Bound) > 0)
                        bounded2CharacterMote = s;
                    else if ((flag & ItemFlags.BoundToAccount) > 0)
                        bounded2AccountMote = s;
                    else unboundedMote = s;
                }
                else if (IsFuseWardPreservation(s))
                {
                    wardSlots.Add(s);
                }
            }

            if (bounded2CharacterMote != null)
            {
                moteSlot = bounded2CharacterMote;
                Logger.WriteLine(Logger.LogType.Log, $"Found mote '{moteSlot.Item.ItemDef.InternalName}' bounded to character.");
            }
            else if (bounded2AccountMote != null)
            {
                moteSlot = bounded2AccountMote;
                Logger.WriteLine(Logger.LogType.Log, $"Found mote '{moteSlot.Item.ItemDef.InternalName}' bounded to account.");
            }
            else if (unboundedMote != null)
            {
                moteSlot = unboundedMote;
                Logger.WriteLine(Logger.LogType.Log, $"Found unbounded mote '{moteSlot.Item.ItemDef.InternalName}'.");
            }
            else
            {
                Logger.WriteLine(Logger.LogType.Log, $"No required mote found.");
                return SearchResult.Absent;
            }

            if (unboundedItem != null && unboundedItem.IsValid && unboundedItem.Filled)
            {
                itemSlot = unboundedItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found unbounded item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Unbounded;
            }

            if (bounded2AccountItem != null && bounded2AccountItem.IsValid && bounded2AccountItem.Filled)
            {
                itemSlot = bounded2AccountItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found bounded to account item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Bounded2Account;
            }

            if (bounded2CharacterItem != null && bounded2CharacterItem.IsValid && bounded2CharacterItem.Filled)
            {
                itemSlot = bounded2CharacterItem;
                Logger.WriteLine(Logger.LogType.Log, $"Found bounded to character item '{itemSlot.Item.ItemDef.InternalName}'[{itemSlot.Item.Id:X}].");
                return SearchResult.Bounded2Character;
            }

            Logger.WriteLine(Logger.LogType.Log, $"No item found.");
            return SearchResult.Absent;
        }

        /// <summary>
        /// Проверяем наличие всех компонентов, необходимых для обработки предмета <param name="itemSlot" />
        /// </summary>
        private bool HaveRequiredCatalysts(InventorySlot itemSlot)
        {
            if (itemSlot == null) return false;

            var item = itemSlot.Item;
            var itemTierDef = item.ItemProgression_GetItemTierDef();

            if (!itemTierDef.IsValid)
                return false;

            var catalystItems = itemTierDef.CatalystItems;
            
            if (!catalystItems.Any()) return true;

            var itemDef = item.ItemDef;
            var itemInternalName = itemDef.InternalName;
            var player = EntityManager.LocalPlayer;

            //TODO заменить итерацию catalystItems на итерацию инвентаря 
            foreach (var catalyst in catalystItems)
            {
                var catalystNumRequired = catalyst.NumRequired;

                // catalyst.Type == 2 у осколков волшебных камней брони и оружия
                // при этом у них невалидно свойство catalyst.ItemDef,
                // поэтому невозможно определить его InternalName
                
                if (catalyst.Type == 2u)
                {
                    var itemCount = player.GetItemCountByInternalNameInBags(itemInternalName);
                    if (itemCount < catalystNumRequired + 1u)
                    {
                        Logger.WriteLine(
                            $"Not enough {catalystNumRequired - itemCount + 1} catalysts '{itemInternalName}'");
                        return false;
                    }
                    continue;
                }


                var catalystInternalName = catalyst.ItemDef.InternalName;

                // Ищем реагент среди предметов в инвентаре
                var count = player.GetItemCountByInternalNameInBags(catalystInternalName);

                if (catalystInternalName == itemInternalName)
                    count--;
                if (count < catalystNumRequired)
                {
                    // Поскольку реагент не найден среди предметов
                    // Ищем реагент в разделе ценностей (Numeric)
                    count = (uint) player.Inventory.GetNumericCount(catalystInternalName);
                    if (count < catalystNumRequired)
                    {
                        Logger.WriteLine($"Not enough {catalystNumRequired - count} catalysts '{itemInternalName}'");
                        return false;
                    }
                }
            }

            Logger.WriteLine($"All catalysts are present");
            return true;
        }

        /// <summary>
        /// Проверка наличия в слоте инвентаря страхующего катализатор <seealso href="https://neverwinter.fandom.com/wiki/Preservation_Ward">Fuse_Ward_Preservation</seealso>
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        private static bool IsFuseWardPreservation(InventorySlot itemSlot)
        {
            return itemSlot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Preservation", StringComparison.Ordinal);
        }

        /// <summary>
        /// Формирование списка страхующих катализаторов для защиты компонентов (Catalysts), перечисленных в <paramref name="protectedCatalysts"/>
        /// </summary>
        /// <param name="evolvingItem">Улучшаемый предмет.</param>
        /// <param name="protectedCatalysts"></param>
        /// <param name="wardsInBag">Слоты инвентаря, содержащие страхующие катализаторы</param>
        /// <returns>Список слотов с катализаторами, в том же порядке, в котором следуют защищаемые компоненты в <see cref="evolvingInventorySlot.Item.ProgressionLogic.CurrentTier.CatalystItems"/>.<br/>
        /// Если компонент не подлежит защите (отсутствует в <paramref name="protectedCatalysts"/>), в соответствующей позиции списка страхующищ катализаторов указывается пустой слот <see cref="InventorySlot(IntPtr.Zero)"/>.<br/>
        /// 
        /// </returns>
        private static List<InventorySlot> MakeWardListForCatalysts(
            Item evolvingItem,
            ItemFilterCore protectedCatalysts, 
            List<InventorySlot> wardsInBag)
        {
            var itemTierDef = evolvingItem.ItemProgression_GetItemTierDef();
            List<ItemProgressionCatalyst> catalystItems;
            if (itemTierDef.IsValid)
                catalystItems = itemTierDef.CatalystItems;
            else
            {
                // Посколько progressionLogic.CurrentTier не валиден, исходим из того, что предмет еще не обрабатывался.
                // Поэтому используем первый набор компонентов 
                catalystItems = evolvingItem.ItemDef.ProgressionDef.Tiers.FirstOrDefault()?.CatalystItems;
            }

            var resultWardList = new List<InventorySlot>();

            if (!(catalystItems?.Count > 0))
            {
                // Нет компонентов, которые требовалось бы защищать
                return resultWardList;
            }

            if (!(protectedCatalysts?.Entries.Count > 0))
            {
                // Список защищаемых компонентов пуст, поэтому список катализаторов содержит "пустые слоты"
                return Enumerable.Repeat(new InventorySlot(IntPtr.Zero), catalystItems.Count).ToList();
            }

            int wardSlotsNum = wardsInBag?.Count ?? 0;
            if (wardSlotsNum > 1)
            {
                // В сумке несколько слотов с катализаторами
                // В первую очерещь необходимо использовать привязанные к персонажу катализаторы, поэтому сортируем их с учетом привязки
                wardsInBag.Sort(new BoundingComparerOfInventorySlot());
            }

            var itemDef = evolvingItem.ItemDef;
            var itemInternalName = itemDef.InternalName;

            int currentWardSlotInd = 0;
            uint usedWardNumInCurrentSlot = 0;

            foreach (var catalyst in catalystItems)
            {
                // catalyst.Type == 2 у осколков волшебных камней брони и оружия
                // при этом у них невалидно свойство catalyst.ItemDef,
                // поэтому невозможно определить его InternalName

                var catalystInternalName = (catalyst.Type == 2u) 
                                              ? itemInternalName 
                                              : catalyst.ItemDef.InternalName;
                if (protectedCatalysts.IsMatch(catalystInternalName))
                {
                    InventorySlot ward;
                    bool wardAbsent = true;
                    while (currentWardSlotInd < wardSlotsNum)
                    {
                        ward = wardsInBag[currentWardSlotInd];
                        uint wardNumInSlot = ward.Item.Count;
                        if (wardNumInSlot - usedWardNumInCurrentSlot > 1)
                        {
                            resultWardList.Add(ward);
                            usedWardNumInCurrentSlot++;
                            wardAbsent = false;
                            break;
                        }
                        currentWardSlotInd++;
                        usedWardNumInCurrentSlot = 0;
                    }

                    if (wardAbsent)
                    {
                        // Для защиты компонента недостаточно катализаторов
                        return null;
                    }
                }
                else resultWardList.Add(new InventorySlot(IntPtr.Zero));
            }

            if (catalystItems.Count == resultWardList.Count)
                return resultWardList;

            return null;
        }
    }
}
