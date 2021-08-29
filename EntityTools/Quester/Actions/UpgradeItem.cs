using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
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
        [Description("Идентификатор обрабатываемого предмета")]
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
                     "Regex : регулярное выражение")]
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
        [Description("Флаг, позволяющий обрабатывать предметы, привязанные к персонажу")]
        public bool AllowBounded 
        { 
            get => _allowBounded;
            set
            {
                if (_allowBounded != value)
                {
                    _allowBounded = value;
                    _itemIdPredicate = null;
                }
            }
        }

        private bool _allowBounded;


        /// <summary>
        /// Флаг, позволяющий обрабатывать предметы, привязанные к аккаунту
        /// </summary>
        [Category("Item")]
        [Description("Флаг, позволяющий обрабатывать предметы, привязанные к аккаунту")]
        public bool AllowAccountBounded
        {
            get => _allowAccountBounded;
            set
            {
                if (_allowAccountBounded != value)
                {
                    _allowAccountBounded = value;
                    _itemIdPredicate = null;
                }
            }
        }

        private bool _allowAccountBounded;


        /// <summary>
        /// Функтор, определяющий соответствие <see cref="InventorySlot"/> группе идентификаторов:
        /// <see cref="ItemId"/>, <see cref="ItemIdType"/>, <see cref="AllowBounded"/>  <see cref="AllowAccountBounded"/>
        /// </summary>
        private Predicate<InventorySlot> ItemIdPredicate
        {
            get
            {
                if (_itemIdPredicate is null)
                {
                    var internalNamePredicate = _itemId.GetComparer(_itemIdType, (InventorySlot s) => s.Item.ItemDef.InternalName);
                    if (_allowAccountBounded)
                    {
                        if (_allowBounded)
                        {
                            // Разрешено обрабатывать предметы с любой привязкой
                            _itemIdPredicate = internalNamePredicate;
                        }
                        else
                        {
                            // Разрешено использовать свободные и предметы с привязкой к аккаунту
                            // т.е. запредено использовать предметы с привязкой к персонажу ItemFlags.Bound
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
                        if (_allowBounded)
                        {
                            // Разрешено использовать свободные и предметы с привязкой к персонажу
                            // т.е. запредено использовать предметы с привязкой к аккаунту ItemFlags.BoundToAccount
                            _itemIdPredicate = s =>
                            {
                                if (s is null || !s.IsValid)
                                    return false;

                                var item = s.Item;
                                var flag = (ItemFlags)item.Flags;

                                if ((flag & ItemFlags.BoundToAccount) == 0)
                                    return false;

                                return internalNamePredicate(s);
                            };
                        }
                        else
                        {
                            // Разрешено использовать только свободные предметы
                            // т.е. запредено использовать предметы с привязкой к аккаунту или персонажу
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
        /// <summary>
        /// Количество очков обработки, которыми нужно заполнить предмет, у которого ProgressionLogic не валидно.
        /// К таким предметам относятся обучающие предметы T1_Enchantment_Tutorial и Insignia_Barbed_Tutorial_R1
        /// </summary>
        [DisplayName("ForcedFeedingRPNumber")]
        [Description("Количество очков обработки, для принудительного заполнения обрабатываемого предмета. \n" +
                     "Используется если количество очков обработни не удалось вычислить, т.к. свойство ProgressionLogic не валидно.")]
        public int Feed { get; set; } = 0;

        [Description("Переключатель типа катализатора, который должен быть использован при обработке")]
        public WardType Ward { get; set; } = WardType.None;

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

            bool filled = false;
            var searchResult = FindSlot(ItemIdPredicate, Ward, out InventorySlot itemSlot, out InventorySlot wardSlot);

            switch (searchResult)
            {
                case SearchResult.Unfilled:
                case SearchResult.PartialFilled:
                case SearchResult.Indefinite:
                    filled = FeedSlot(itemSlot);
                    break;
                case SearchResult.FullFilled:
                    filled = true;
                    break;
                case SearchResult.Absent:
                    return ActionResult.Skip;
            }

            if (!filled)
                return ActionResult.Skip;

            if (searchResult != SearchResult.Indefinite
                && !HaveRequiredItems(itemSlot))
                return ActionResult.Skip;

            if (Ward != WardType.None)
            { 
                if(wardSlot is null)
                    return ActionResult.Skip;
                
                itemSlot.Evolve(wardSlot);
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

            InventorySlot fullFilled = null;
            InventorySlot partialFilled = null;
            InventorySlot unfilled = null;

            var player = EntityManager.LocalPlayer;
            
            foreach (var s in player.BagsItems)
            {
                if (predicate(s))
                {
                    itemSlot = s;

                    var progression = s.Item.ProgressionLogic;

                    if (progression.CurrentRankXP > 0 
                        && progression.CurrentRankTotalRequiredXP > progression.CurrentRankXP)
                    {
                        if (partialFilled is null)
                            partialFilled = s;
                        else if (progression.CurrentRankXP > partialFilled.Item.ProgressionLogic.CurrentRankXP)
                            partialFilled = s;
                    }
                    else if (progression.CurrentTier.Index > 0
                             && Math.Abs(progression.CurrentRankTotalRequiredXP - progression.CurrentRankXP) < 1.0)
                    {
                        fullFilled = s;
                        break;
                    }
                    else if (progression.CurrentRankXP == 0)
                    {
                        unfilled = s;
                    }
                }
            }

            if (fullFilled != null && fullFilled.IsValid && fullFilled.Filled)
            {
                itemSlot = fullFilled;
                return SearchResult.FullFilled;
            }

            if (partialFilled !=null && partialFilled.IsValid && partialFilled.Filled)
            {
                itemSlot = partialFilled;
                return SearchResult.PartialFilled;
            }

            if (unfilled != null && unfilled.IsValid)
            {
                itemSlot = unfilled;
                return SearchResult.Unfilled;
            }

            return SearchResult.Absent;
        }

        /// <summary>
        /// Поиск слотов инвентаря, содержащих предмет, соответствующий <param name="predicate"/>, и катализатор
        /// </summary>
        /// <param name="predicate">Критерий отбора предметов. Как правило - идентификатор</param>
        /// <param name="wardType">Тип катализатора</param>
        /// <param name="itemSlot">Слот инвентаря, содержащего искомый предмет</param>
        /// <param name="wardSlot">Слот инвентаря, содержащий найденный катализацтор</param>
        /// <returns>Перечисление, описывающее результаты поиска</returns>
        private static SearchResult FindSlot(Predicate<InventorySlot> predicate, WardType wardType, out InventorySlot itemSlot, out InventorySlot wardSlot)
        {
            itemSlot = null;
            wardSlot = null;

            if (wardType == WardType.None)
                return FindSlot(predicate, out itemSlot);

            InventorySlot fullFilled = null;
            InventorySlot partialFilled = null;
            InventorySlot unfilled = null;

            InventorySlot unboundedWard = null;
            InventorySlot bounded2CharacterWard = null;
            InventorySlot bounded2AccountWard = null;

            var player = EntityManager.LocalPlayer;

            Predicate<InventorySlot> wardPredicate;
            switch (wardType)
            {
                case WardType.Preservation:
                    wardPredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Preservation", StringComparison.Ordinal);
                    break;
                case WardType.Coalescent:
                    wardPredicate = slot => slot.Item.ItemDef.InternalName.StartsWith("Fuse_Ward_Coalescent", StringComparison.Ordinal);
                    break;
                default:
                    wardPredicate = slot => false;
                    break;
            }

            foreach (var s in player.BagsItems)
            {
                if (predicate(s))
                {
                    itemSlot = s;

                    var progression = s.Item.ProgressionLogic;
                    if (progression.CurrentRankXP > 0
                        && progression.CurrentRankTotalRequiredXP > progression.CurrentRankXP)
                    {
                        if (partialFilled is null)
                            partialFilled = s;
                        else if (progression.CurrentRankXP > partialFilled.Item.ProgressionLogic.CurrentRankXP)
                            partialFilled = s;
                    }
                    else if (progression.CurrentTier.Index > 0
                             && Math.Abs(progression.CurrentRankTotalRequiredXP - progression.CurrentRankXP) < 1.0)
                    {
                        fullFilled = s;
                    }
                    else if (progression.CurrentRankXP == 0)
                        unfilled = s;
                }
                else 
                {
                    if (bounded2AccountWard is null
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
            }

            if (bounded2CharacterWard != null)
                wardSlot = bounded2CharacterWard;
            else if (bounded2AccountWard != null)
                wardSlot = bounded2AccountWard;
            else wardSlot = unboundedWard;

            if (fullFilled != null && fullFilled.IsValid && fullFilled.Filled)
            {
                itemSlot = fullFilled;
                return SearchResult.FullFilled;
            }

            if (partialFilled != null && partialFilled.IsValid && partialFilled.Filled)
            {
                itemSlot = partialFilled;
                return SearchResult.PartialFilled;
            }

            if (unfilled != null && unfilled.IsValid && unfilled.Filled)
            {
                itemSlot = unfilled;
                return SearchResult.Unfilled;
            }

            if (itemSlot != null && itemSlot.IsValid && itemSlot.Filled)
            {
                return SearchResult.Indefinite;
            }

            return SearchResult.Absent;
        }

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
                points = (int) (progress.CurrentRankTotalRequiredXP - progress.CurrentRankXP);
            else
            {
                var defaultFeed = Feed;
                if (defaultFeed > 0)
                    points = defaultFeed;
                else return false;
            }

            if (points == 0)
                return true;

            if (EntityManager.LocalPlayer.Inventory.RefinementCurrency >= points)
            {
                s.Feed(points);
                Thread.Sleep(500);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяем наличие всех предметов, необходимых для обработки
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        private bool HaveRequiredItems(InventorySlot itemSlot)
        {
            if (itemSlot == null) return false;

            var progressionLogic = itemSlot.Item.ProgressionLogic;

            if (!progressionLogic.IsValid)
                return false;

            var catalystItems = progressionLogic.CurrentTier.CatalystItems;
            
            if (!catalystItems.Any()) return true;

            var player = EntityManager.LocalPlayer;

            //TODO заменить итерацию catalystItems на итерацию инвентаря 
            foreach (var catalyst in catalystItems)
            {
                // Ищем реагент среди предметов в инвентаре
                var count = player.GetItemCountByInternalNameInBags(catalyst.ItemDef.InternalName);
                if (catalyst.ItemDef.InternalName == itemSlot.Item.ItemDef.InternalName)
                    count--;
                if (count < catalyst.NumRequired)
                {
                    // Поскольку реагент не найден среди предметов
                    // Ищем реагент в разделе ценностей (Numeric)
                    count = (uint) player.Inventory.GetNumericCount(catalyst.ItemDef.InternalName);
                    if (count < catalyst.NumRequired)
                        return false;
                }
            }

            return true;
        }
    }
}
