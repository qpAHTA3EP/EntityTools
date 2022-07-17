using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class UpgradeItem : Astral.Quester.Classes.Action
    {

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

#if false
        [Category("Irregular")]
        public bool Irregular { get; set; } = false; 
#endif
#if false
        /// <summary>
        /// Количество очков обработки, которыми нужно заполнить предмет, у которого ProgressionLogic не валидно.
        /// К таким предметам относятся обучающие предметы T1_Enchantment_Tutorial и Insignia_Barbed_Tutorial_R1
        /// </summary>
        [DisplayName("ForcedFeedingRPNumber")]
        [Description("Количество очков обработки, для принудительного заполнения обрабатываемого предмета. \n" +
                     "Используется если количество очков обработки не удалось вычислить, т.к. у предмета не валидно свойство ProgressionLogic.\n" +
                     "Значение -1 отключает принудительное заполнение предмета очками обработки.")]
        public int Feed { get; set; } = -1; 

        [Description("Переключатель типа катализатора, который должен быть использован при обработке.")]
        public WardType Ward { get; set; } = WardType.None;
#endif
        [Description("Переключатель типа пылинки, которую нужно использовать при обработке.")]
        public MoteType Mote { get; set; } = MoteType.None;


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

        //public override string Category => Core.Category;
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
            var searchResult = FindSlot(ItemIdPredicate, Mote, WardType.None, out InventorySlot itemSlot, out InventorySlot moteSlot, out InventorySlot wardSlot);
            
            if(searchResult == SearchResult.Absent)
                return ActionResult.Skip;

            if (!Forced && !HaveRequiredCatalysts(itemSlot))
                return ActionResult.Skip;

            if (Mote != MoteType.None)
            { 
                if(moteSlot is null)
                    return ActionResult.Skip;
                
                itemSlot.Evolve(moteSlot);
            }
            else itemSlot.Evolve();

            return ActionResult.Completed;
        }

        /// <summary>
        /// Поиск слота инвентаря, содержащего предмет, соответствующие <param name="predicate"/>
        /// </summary>
        /// <param name="predicate">Критерий отбора предметов. Как правило - идентификатор</param>
        /// <param name="itemSlot">Найденный слот инвентаря</param>
        /// <returns>Перечисление, описывающее результаты поиска</returns>
        private static SearchResult FindSlot(Predicate<InventorySlot> predicate, out InventorySlot itemSlot)
        {
            itemSlot = null;

            InventorySlot unboundedItem = null;
            InventorySlot bounded2CharacterItem = null;
            InventorySlot bounded2AccountItem = null;

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
        /// <param name="wardType">Тип катализатора</param>
        /// <param name="itemSlot">Слот инвентаря, содержащего искомый предмет</param>
        /// <param name="moteSlot">Слот инвентаря, содержащий найденную частицу</param>
        /// <param name="wardSlot">Слот инвентаря, содержащий найденный катализатор</param>
        /// <returns>Перечисление, описывающее результаты поиска</returns>
        private static SearchResult FindSlot(Predicate<InventorySlot> predicate, MoteType moteType, WardType wardType, out InventorySlot itemSlot, out InventorySlot moteSlot, out InventorySlot wardSlot)
        {
            itemSlot = null;
            moteSlot = null;
            wardSlot = null;

            if (moteType == MoteType.None 
                && wardType == WardType.None)
                return FindSlot(predicate, out itemSlot);

            InventorySlot unboundedItem = null;
            InventorySlot bounded2CharacterItem = null;
            InventorySlot bounded2AccountItem = null;

            InventorySlot unboundedWard = null;
            InventorySlot bounded2CharacterWard = null;
            InventorySlot bounded2AccountWard = null;

            InventorySlot unboundedMote = null;
            InventorySlot bounded2CharacterMote = null;
            InventorySlot bounded2AccountMote = null;

            var player = EntityManager.LocalPlayer;

            Predicate<InventorySlot> wardPredicate;
            switch (wardType)
            {
                case WardType.Preservation:
                    wardPredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Preservation", StringComparison.Ordinal);
                    break;
#if false
                case WardType.Coalescent:
                    wardPredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Coalescent", StringComparison.Ordinal);
                    break; 
#endif
                default:
                    wardPredicate = slot => false;
                    break;
            }

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
#if false
                case WardType.Coalescent:
                    wardPredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Coalescent", StringComparison.Ordinal);
                    break; 
#endif
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
                else if (bounded2CharacterWard is null
                        && wardPredicate(s))
                {
                    var flag = (ItemFlags) s.Item.Flags;

                    if ((flag & ItemFlags.Bound) > 0)
                        bounded2CharacterWard = s;
                    else if ((flag & ItemFlags.BoundToAccount) > 0)
                        bounded2AccountWard = s;
                    else unboundedWard = s;
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
            else if(moteType != MoteType.None)
            {
                Logger.WriteLine(Logger.LogType.Log, $"No required mote found.");
                return SearchResult.Absent;
            }

            if (bounded2CharacterWard != null)
            {
                wardSlot = bounded2CharacterWard;
                Logger.WriteLine(Logger.LogType.Log, $"Found ward '{wardSlot.Item.ItemDef.InternalName}' bounded to character.");
            }
            else if (bounded2AccountWard != null)
            {
                wardSlot = bounded2AccountWard;
                Logger.WriteLine(Logger.LogType.Log, $"Found ward '{wardSlot.Item.ItemDef.InternalName}' bounded to account.");
            }
            else if(unboundedWard != null)
            {
                wardSlot = unboundedWard;
                Logger.WriteLine(Logger.LogType.Log, $"Found unbounded ward '{wardSlot.Item.ItemDef.InternalName}'.");
            }
            else if(wardType != WardType.None)
            {
                Logger.WriteLine(Logger.LogType.Log, $"No required ward found.");
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
#if false

        /// <summary>
        /// Заполнение предмета очками обработки
        /// </summary>
        /// <param name="s">Слот инвентаря, в котором находится обрабатываемый предмет</param>
        /// <returns>Успешность заполнения полной шкалы очков обработки</returns>
        private bool FeedSlot(InventorySlot s)
        {
            var progress = s.Item.ProgressionLogic;

            int points;
            if (progress.IsValid)
                points = (int)(progress.CurrentRankTotalRequiredXP - progress.CurrentRankXP);
            else
            {
                var defaultFeed = Feed;
                if (defaultFeed < 0)
                {
                    Logger.WriteLine(Logger.LogType.Log, $"Can not calculate required RP. Feed Skip feeding.");
                    return true;
                }

                if (defaultFeed == 0)
                {
                    Logger.WriteLine(Logger.LogType.Log, $"Can not calculate required RP and 'ForcedFeedingRPNumber' does not set.");
                    return false;
                }
                points = defaultFeed;
            }

            if (points == 0)
                return true;

            int refinementCurrency = EntityManager.LocalPlayer.Inventory.RefinementCurrency;
            if (refinementCurrency >= points)
            {
                Logger.WriteLine(Logger.LogType.Log, $"Feeding item {s.Item.ItemDef.InternalName}[{s.Item.Id:X}] with {points} RP.");
                s.Feed(points);
                Thread.Sleep(500);
                return true;
            }

            Logger.WriteLine(Logger.LogType.Log, $"Not enough {refinementCurrency - points} RP to feed item {s.Item.ItemDef.InternalName}[{s.Item.Id:X}].");
            return false;
        }

#endif
        /// <summary>
        /// Проверяем наличие всех компонентов, необходимых для обработки предмета <param name="itemSlot" />
        /// </summary>
        private bool HaveRequiredCatalysts(InventorySlot itemSlot)
        {
            if (itemSlot == null) return false;

            var item = itemSlot.Item;
            var progressionLogic = item.ProgressionLogic;

            if (!progressionLogic.IsValid)
                return false;

            var catalystItems = progressionLogic.CurrentTier.CatalystItems;
            
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
    }
}
