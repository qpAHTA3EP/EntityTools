using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.BuySellItems;
using EntityTools.Extensions;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using NPCInfos = Astral.Quester.Classes.NPCInfos;
using static EntityTools.Tools.BuySellItems.ItemFilterEntryExt;
using System.Collections.ObjectModel;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class BuySellItemsExt : Astral.Quester.Classes.Action
    {
        BuySellItemsExt @this => this;

        #region Опции команды
#if DEVELOPER
        [Editor(typeof(NPCVendorInfosExtEditor), typeof(UITypeEditor))]
        [Category("Vendor")]
#else
        [Browsable(false)]
#endif
        public NPCInfos Vendor { get => vendor; set => vendor = value; }
        internal NPCInfos vendor = new NPCInfos();

#if DEVELOPER
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Description("Specific dialogs before reaching item list.")]
        [Category("Vendor")]
#else
        [Browsable(false)]
#endif
        public List<string> VendorMenus { get => vendorMenus; set => vendorMenus = value; }
        internal List<string> vendorMenus = new List<string>();

#if DEVELOPER
        [Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public ItemFilterCore SellOptions
        {
            get => sellOptions;
            set
            {
                if (sellOptions != value)
                {
                    sellOptions = value;
                    slots2sellCache = null;
                }
            }
        }
        internal ItemFilterCore sellOptions = new ItemFilterCore();

#if DEVELOPER
        [Description("Use options set in general settings to sell")]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToSell
        {
            get => useGeneralSettingsToSell; set
            {
                if (useGeneralSettingsToSell != value)
                {
                    useGeneralSettingsToSell = value;
                    slots2sellCache = null; 
                }
            }
        }
        internal bool useGeneralSettingsToSell = true;

#if DEVELOPER
        [Description("Список сумок, предметы из которых могут быть проданы")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public BagsList SellBags
        {
            get => sellBags;
            set
            {
                if (!Equals(sellBags, value))
                {
                    sellBags = CopyHelper.CreateDeepCopy(value);
                    slots2sellCache = null;
                }
            }
        }
        internal BagsList sellBags = BagsList.GetPlayerBags();

#if DEVELOPER
        [Editor(typeof(ItemFilterEntryListEditor), typeof(UITypeEditor))]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public List<ItemFilterEntryExt> BuyOptions { get => buyOptions; set => buyOptions = value; }
        private List<ItemFilterEntryExt> buyOptions = new List<ItemFilterEntryExt>();

#if DEVELOPER
        [Description("Use options set in general settings to buy")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToBuy { get => useGeneralSettingsToBuy; set => useGeneralSettingsToBuy = value; }
        internal bool useGeneralSettingsToBuy = false;

#if DEVELOPER
        [Description("Проверять наличие свободных слотов сумки")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public bool CheckFreeBags { get => checkFreeBags; set => checkFreeBags = value; }
        internal bool checkFreeBags = true;

#if DEVELOPER
        [Description("True: Закрывать диалоговое окно после покупки\n\r" +
            "False: Оставлять диалоговое окно открытым (значение по умолчанию)")]
#else
        [Browsable(false)]
#endif
        public bool CloseContactDialog { get => closeContactDialog; set => closeContactDialog = value; }
        internal bool closeContactDialog = false;

#if DEVELOPER
        [Description("Список сумок, в которых производится поиск предметов для сравнения с предлагаемыми в магазине")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public BagsList Bags
        {
            get => bags;
            set
            {
                if(!Equals(bags, value))
                {
                    bags = CopyHelper.CreateDeepCopy(value);
                }
            }
        }
        internal BagsList bags = BagsList.GetFullPlayerInventory();

#if DEVELOPER
        [Description("Продолжительно торговой операции (покупки 1 предмета)")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public uint Timer { get => timer; set => timer = value; }
        internal uint timer = 10000;

#if DEVELOPER
        [Description("Количество попыток инициировать торговую сессию")]
#else
        [Browsable(false)]
#endif
        public uint Tries
        {
            get => tries;
            set
            {
                tries = value;
                tryNum = value;
            }
        }
        internal uint tries = 3;
        private uint tryNum;
        #endregion

        public BuySellItemsExt()
        {
            tryNum = tries;
        }

        /// <summary>
        /// Кэшированный список слотов, которые подлежат продаже
        /// </summary>
        private List<InventorySlot> slots2sellCache = null;

        /// <summary>
        /// Функтор доступа к методу сопоставления предмета с ItemFilterCore
        /// </summary>
        private Func<object, Func<Item, bool>> ItemFilterCore_SellItems = typeof(ItemFilterCore).GetFunction<Item, bool>("\u0001");

        #region Интерфейс Quester.Action
        public override string ActionLabel => GetType().Name;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions
        {
            get
            {
                if (!Validate(@this.vendor))
                    return false;

                if (@this.useGeneralSettingsToSell)
                {
                    if (sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> slots))
                        slots2sellCache = slots;
                }

                if (@this.sellOptions != null && @this.sellOptions.Entries.Count > 0)
                {
                    if (bags.GetItems(@this.sellOptions, out List<InventorySlot> slots))
                        if (slots2sellCache != null)
                            slots2sellCache.AddRange(slots);
                        else slots2sellCache = slots;
                }

                return slots2sellCache?.Count > 0 
                    || @this.useGeneralSettingsToBuy && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0 
                    || @this.buyOptions.Count > 0;
            }
        }
        protected override Vector3 InternalDestination => Vendor.Position;
        public override void GatherInfos()
        {
#if DEVELOPER
            if (NPCVendorInfosExtEditor.SetInfos(out NPCInfos npcInfos))
            {
                Vendor = npcInfos;
                if (XtraMessageBox.Show("Add a dialog ? (open the dialog window before)", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    switch (@this.vendor.CostumeName)
                    {
                        case "ArtifactVendor":
                            SpecialVendor.UseItem();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case "VIPSummonSealTrader":
                            VIP.SummonSealTrader();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case "VIPProfessionVendor":
                            VIP.SummonProfessionVendor();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case "RemouteVendor":
                            RemoteContact remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.Find((ct) => ct.ContactDef == Vendor.CostumeName);
                            if (remoteContact != null)
                            {
                                remoteContact.Start();
                                Interact.WaitForInteraction();
                            }
                            break;
                        default:
                            ContactInfo contactInfo = EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts.Find((ct) => ct.Entity.IsValid && Vendor.IsMatching(ct.Entity));
                            if (contactInfo != null)
                            {
                                Interact.Vendor(contactInfo.Entity);
                                Interact.WaitForInteraction();
                            }
                            break;
                    }
                    DialogEdit.Show(VendorMenus);
                }
            }
#endif
        }
        public override void InternalReset()
        {
            tryNum = tries;
            slots2sellCache = null;
        }
        public override void OnMapDraw(GraphicsNW graph) { }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (!Validate(@this.vendor))
                    return new ActionValidity("Vendor does not set");

                if (!@this.useGeneralSettingsToBuy && (@this.buyOptions == null || @this.buyOptions.Count == 0)
                    || !@this.useGeneralSettingsToSell && (@this.sellOptions == null || @this.sellOptions.Entries.Count == 0))
                    return new ActionValidity("The items to buy of sell are not specified!");

                return new ActionValidity();
            }
        }
        #endregion

        public override bool NeedToRun => @this.vendor.Position.Distance3DFromPlayer < 25 || (!@this.vendor.Position.IsValid && @this.vendor.MapName == "All");

        public override ActionResult Run()
        {
            if (tryNum > 0)
            {
                tryNum--;
                switch (@this.vendor.CostumeName)
                {
                    case "None":
                        return ActionResult.Fail;
                    case "ArtifactVendor":
                        SpecialVendor.UseItem();
                        Thread.Sleep(3000);
                        return Traiding(SpecialVendor.VendorEntity);
                    case "VIPSummonSealTrader":
                        VIP.SummonSealTrader();
                        Thread.Sleep(3000);
                        return Traiding(VIP.SealTraderEntity);
                    case "VIPProfessionVendor":
                        VIP.SummonProfessionVendor();
                        Thread.Sleep(3000);
                        return Traiding(VIP.ProfessionVendorEntity);
                    case "RemouteVendor":
                        RemoteContact remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.Find((ct) => ct.ContactDef == Vendor.CostumeName);
                        return RemouteTraiding(remoteContact);
                    default:
                        ContactInfo contactInfo = EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts.Find((ct) => ct.Entity.IsValid && Vendor.IsMatching(ct.Entity));
                        return Traiding(contactInfo.Entity);
                }
            }
            else return ActionResult.Fail;
        }

        /// <summary>
        /// Торговая сесссия с удаленным вендором
        /// </summary>
        /// <param name="remoteContact"></param>
        /// <returns></returns>
        private ActionResult RemouteTraiding(RemoteContact remoteContact)
        {
            if (remoteContact != null)
            {
                remoteContact.Start();
                Interact.WaitForInteraction();

                ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                if (contactDialog.IsValid && (screenType == ScreenType.List || screenType == ScreenType.Store || screenType == ScreenType.StoreCollection))
                {
                    // взаимодействие с вендором для открытия окна магазина
                    if (@this.vendorMenus.Count > 0)
                    {
#if false
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timeout.IsTimedOut)
                                return ActionResult.Fail;
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        foreach (string dialogItem in BuyMenus)
                        {
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(dialogItem, "");
                            Thread.Sleep(1000);
                        } 
                        canTraid = true;
#else 
                        // Открыто диалоговое окно
                        screenType = contactDialog.ScreenType;
                        if (screenType == ScreenType.List || screenType == ScreenType.Buttons)
                            // Открыто диалоговое окно продавца
                            if (@this.vendorMenus.Count > 0)
                                Interact.DoDialog(vendorMenus);
                        else if (Check_ReadyToTraid(screenType))
                        // Открыто витрина магазина (список товаров)
                        // необходимо переключиться на нужную вкладку
                        {
                            if (@this.vendorMenus.Count > 0)
                            {
                                string key = @this.vendorMenus.Last();
                                if (contactDialog.HasOptionByKey(key))
                                    contactDialog.SelectOptionByKey(key, "");
                            }
                        }
                        if (@this.closeContactDialog)
                            contactDialog.Close();
#endif
                    }

                    if (Check_ReadyToTraid(screenType))
                    {
                        // Унифицированная версия, совмещенная с продажей вендору-NPC
                        // Продажа предметов
                        SellItems();

                        // Покупка предметов
                        ActionResult result = BuyItems();

                        // Закрытие диалогового окна
                        if (@this.closeContactDialog)
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();

                        return result;
                    }
                }
            }
            return ActionResult.Running;
        }

        /// <summary>
        /// Торговая сессия с вендором-NPC, включая призываемых
        /// </summary>
        /// <param name="vendorEntity"></param>
        /// <returns></returns>
        private ActionResult Traiding(Entity vendorEntity)
        {
            if (vendorEntity != null)
            {
                // взаимодействие с вендором для открытия окна магазина
                if (ApproachAndInteractToVendor(vendorEntity))
                {
                    ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                    if (/*screenType == ScreenType.List || */screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)
                    {
                        // Продажа предметов
                        SellItems();

                        // Покупка предметов
                        ActionResult result = BuyItems();

                        // Закрытие диалогового окна
                        if (@this.closeContactDialog)
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();

                        return result;
                    }
                }
            }
            return ActionResult.Running;
        }

        /// <summary>
        /// Функция взаимодействия с торговцем-NPC
        /// в случа необходимости задействуется навигационная система 
        /// для перемещения персонажа к месту нахождения NPC
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool ApproachAndInteractToVendor(Entity entity)
        {
            if (@this.vendor.MapName == EntityManager.LocalPlayer.MapState.MapName
                && @this.vendor.RegionName == EntityManager.LocalPlayer.RegionInternalName)
            {
                // Проверяем соответствие 
                if (entity != null && entity.IsValid && @this.vendor.IsMatching(entity))
                {
                    ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                    if (contactDialog.IsValid)
                    {
                        // Открыто диалоговое окно
                        ScreenType screenType = contactDialog.ScreenType;
                        if (screenType == ScreenType.List || screenType == ScreenType.Buttons)
                        {
                            // Открыто диалоговое окно продавца
                            if (@this.vendorMenus.Count > 0)
                                Interact.DoDialog(vendorMenus);
                            return Check_ReadyToTraid(contactDialog.ScreenType);
                        }
                        else if (screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)
                        // Открыто витрина магазина (список товаров)
                        // необходимо переключиться на нужную вкладку
                        {
                            if (@this.vendorMenus.Count > 0)
                            {
                                string key = @this.vendorMenus.Last();
                                if (contactDialog.HasOptionByKey(key))
                                    contactDialog.SelectOptionByKey(key, "");
                            }
                            return Check_ReadyToTraid(contactDialog.ScreenType);
                        }
                        if (@this.closeContactDialog)
                            contactDialog.Close();
                    }
                    if (!contactDialog.IsValid || entity.Location.Distance3DFromPlayer > 10)
                    {
                        // Расстояние до продавца больше 10
                        return Interact.VendorWithDialogs(entity, vendorMenus);
                    }
                    return Check_ReadyToTraid(contactDialog.ScreenType);
                }
            }
            return false;
        }

        /// <summary>
        /// Покупка предметов
        /// </summary>
        /// <returns></returns>
        private ActionResult BuyItems()
        {
            if (!checkFreeBags || Check_FreeSlots())
            {
                if (useGeneralSettingsToBuy)
                {
                    Interact.BuyItems();
                    if (Astral.Controllers.Settings.Get.QuesterOptions.RestockPotions)
                    {
                        Interact.RestockBetterPotion(Astral.Controllers.Settings.Get.QuesterOptions.RestockPotionsCount);
                    }
                    if (Astral.Controllers.Settings.Get.QuesterOptions.RestockInjuryKits)
                    {
                        Interact.RestockBetterInjuryKit(Astral.Controllers.Settings.Get.QuesterOptions.RestockInjuryKitsCount);
                    }
                }

#if direct_item_search_in_bags
                if (_buyOptions.Count > 0)
                {
                    foreach (ItemFilterEntryExt item2buy in @this._buyOptions)
                    {
#if false
                        // Подсчет общего количества предметов
                        int totalNum = playerBag[item2buy].Sum((slot) => (int)slot.Item.Count);
                        // Вычисляем количество предметов, которые нужно купить
                        int buyNum = 0;
                        if (item2buy.KeepNumber)
                        {
                            if (item2buy.Count > totalNum)
                                buyNum = (int)item2buy.Count - totalNum;
                        }
                        else buyNum = (int)item2buy.Count;

                        if (buyNum > 0) 
#endif
                        {
                            List<ItemDef> boughtItems = null;
                            BuyItemResult buyItemResult = BuyAnItem(item2buy, ref boughtItems);
                            // Обработка результата покупки
                            // Если результат не позволяет продолжать покупку - прерываем выполнение команды
                            switch (buyItemResult)
                            {
                                case BuyItemResult.Succeeded:
                                    {
                                        // Предмет необходимо экипировать после покупки
                                        if (item2buy.PutOnItem && boughtItems != null && boughtItems.Count > 0)
                                        {
                                            foreach (ItemDef item in boughtItems)
                                            {
                                                // Здесь не учитывается возможно приобретения и экипировки нескольких вещений (колец)

                                                InventorySlot slot = _bags.Find(item);
                                                if (slot != null)
                                                    slot.Equip();
                                            }
                                        }
                                        break;
                                    }
                                case BuyItemResult.FullBag:
                                    return ActionResult.Skip;
                                case BuyItemResult.Error:
                                    return ActionResult.Fail;
                            }
                        }
                    } 
                }
#else
                if (buyOptions.Count > 0)
                {
                    // Анализируем содержимое сумок
                    IndexedBags indexedBags = new IndexedBags(@this.buyOptions, @this.bags);

                    foreach (var filterEntry in indexedBags.Filters)
                    {
                        var slotCache = indexedBags[filterEntry];
                        List<ItemDef> boughtItems = null;
                        BuyItemResult buyItemResult = (slotCache is null) ? BuyAnItem(filterEntry, ref boughtItems) : BuyAnItem(filterEntry, slotCache, ref boughtItems);
                        // Обработка результата покупки
                        // Если результат не позволяет продолжать покупку - прерываем выполнение команды
                        switch (buyItemResult)
                        {
                            case BuyItemResult.Succeeded:
                                {
                                    // Предмет необходимо экипировать после покупки
                                    if (filterEntry.PutOnItem && boughtItems != null && boughtItems.Count > 0)
                                    {
                                        foreach (ItemDef item in boughtItems)
                                        {
                                            // Здесь не учитывается возможно приобретения и экипировки нескольких вещений (например, колец)

                                            InventorySlot slot = bags.Find(item);
                                            if (slot != null)
                                                slot.Equip();
                                        }
                                    }
                                    break;
                                }
                            case BuyItemResult.FullBag:
                                return ActionResult.Skip;
                            case BuyItemResult.Error:
                                return ActionResult.Fail;
                        }
                    }
                }
#endif

                return ActionResult.Completed;
            }
            else return ActionResult.Skip;
        }

        /// <summary>
        /// Обработка одной позиции списка покупок
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        private BuyItemResult BuyAnItem(ItemFilterEntryExt item2buy, ref List<ItemDef> boughtItems)
        {
            BuyItemResult result = BuyItemResult.Completed;
            if (boughtItems == null)
                boughtItems = new List<ItemDef>();
            else boughtItems.Clear();
            foreach (StoreItemInfo storeItemInfo in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems)
            {
                if (storeItemInfo.CanBuyError == 0u)
                {
                    if (item2buy.IsMatch(storeItemInfo.Item))
                    {
                        // Проверка соответствия уровню персонажа
                        if (!item2buy.CheckPlayerLevel || storeItemInfo.FitsPlayerLevel())
                        {
                            // Проверка уровня предмета
                            if (!item2buy.CheckEquipmentLevel || bags.ContainsBetterItemEquipmentLevel(storeItemInfo))
                            {
                                bool succeeded = false;
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(@this.timer > 0 ? (int)@this.timer : int.MaxValue);

                                if (!checkFreeBags || Check_FreeSlots())
                                {
                                    //Вычисляем количество предметов, которые необходимо докупить
                                    uint toBuyNum = storeItemInfo.NumberOfItem2Buy(bags, item2buy);
                                    if (toBuyNum > 0)
                                    {
                                        // Проверка наличия валюты заложена в storeItemInfo.CanBuyError
                                        // Код ошибки (нет валюты) = 6

                                        if (item2buy.BuyByOne)
                                        {
                                            Logger.WriteLine($"Buy total {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] by one item...");
                                            uint totalPurchasedNum = 0;
                                            for (totalPurchasedNum = 0; totalPurchasedNum < toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (timeout.IsTimedOut)
                                                {
                                                    Logger.WriteLine($"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...");
                                                    break;
                                                }
                                                if (checkFreeBags && !Check_FreeSlots())
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    Logger.WriteLine($"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...");
                                                    return result = BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                        else
                                        {
                                            // Покупка предмета
                                            Logger.WriteLine($"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...");
                                            storeItemInfo.BuyItem(toBuyNum);
                                            Thread.Sleep(250);

                                            succeeded = true;
                                            result = BuyItemResult.Succeeded;
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.WriteLine("Bags are full, skip ...");
                                    return result = BuyItemResult.FullBag;
                                }

                                if (succeeded)
                                {
                                    result = BuyItemResult.Succeeded;
                                    boughtItems.Add(storeItemInfo.Item.ItemDef);
                                }
                            }
                        }
                    }
                    //else return BuyItemResult.Skiped;
                }
                //else return BuyItemResult.Error;
            }
            return result;
        }
#if ReadOnlyItemFilterEntryExt
        private BuyItemResult BuyAnItem(KeyValuePair<ReadOnlyItemFilterEntryExt, SlotCache> filterEntryCache, ref List<ItemDef> boughtItems)
#else
        private BuyItemResult BuyAnItem(ItemFilterEntryExt filterEntry, SlotCache slotCache, ref List<ItemDef> boughtItems)
#endif
        {
            BuyItemResult result = BuyItemResult.Completed;
            if (boughtItems == null)
                boughtItems = new List<ItemDef>();
            else boughtItems.Clear();

            foreach (StoreItemInfo storeItemInfo in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems)
            {
                if (storeItemInfo.CanBuyError == 0u)
                {
                    // Проверка наличия валюты заложена в storeItemInfo.CanBuyError
                    // Код ошибки (нет валюты) = 6

                    if (filterEntry.IsMatch(storeItemInfo.Item))
                    {
                        // Проверка соответствия уровню персонажа
                        if (!filterEntry.CheckPlayerLevel || storeItemInfo.FitsPlayerLevel())
                        {
                            // Проверка уровня предмета
                            if (!filterEntry.CheckEquipmentLevel 
                                //|| filterEntryCache.Value.MaxItemLevel <= storeItemInfo.Item.ItemDef.Level)
                                || slotCache.HasWorseThen(storeItemInfo))
                            {
                                bool succeeded = false;
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(@this.timer > 0 ? (int)@this.timer : int.MaxValue);

                                if (!checkFreeBags || Check_FreeSlots())
                                {
                                    //Вычисляем количество предметов, которые необходимо докупить
                                    uint toBuyNum = storeItemInfo.NumberOfItem2Buy(slotCache, filterEntry);
                                    if (toBuyNum > 0)
                                    {
                                        uint mayBuyInBulk = storeItemInfo.Item.ItemDef.MayBuyInBulk;

                                        //if (!filterEntryCache.Key.BuyByOne)
                                        if (mayBuyInBulk > toBuyNum)
                                        {
                                            // Покупка предмета в необходимом количество за один раз
                                            Logger.WriteLine($"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...");
                                            storeItemInfo.BuyItem(toBuyNum);
                                            Thread.Sleep(250);

                                            succeeded = true;
                                            result = BuyItemResult.Succeeded;
                                        }
                                        else
                                        {
                                            // Невозможна единовременная покупка необходимого количества предметов, 
                                            // производим покупку в цикле
                                            Logger.WriteLine($"Buy total {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] by one item...");
                                            uint totalPurchasedNum = 0;
                                            for (totalPurchasedNum = 0; totalPurchasedNum < toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (timeout.IsTimedOut)
                                                {
                                                    Logger.WriteLine($"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...");
                                                    break;
                                                }
                                                if(storeItemInfo.CanBuyError != 0u)
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    Logger.WriteLine($"Buying is impossible now. Error code '{storeItemInfo.CanBuyError}'. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...");
                                                    return result = BuyItemResult.NotEnoughCurrency;
                                                }
                                                if (checkFreeBags && !Check_FreeSlots())
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    Logger.WriteLine($"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...");
                                                    return result = BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.WriteLine("Bags are full, skip ...");
                                    return result = BuyItemResult.FullBag;
                                }

                                if (succeeded)
                                {
                                    result = BuyItemResult.Succeeded;
                                    boughtItems.Add(storeItemInfo.Item.ItemDef);
                                }
                            }
                        }
                    }
                    //else return BuyItemResult.Skiped;
                }
                //else return BuyItemResult.Error;
            }
            return result;
        }

        /// <summary>
        /// Продажа всех предметов
        /// </summary>
        private void SellItems()
        {
            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SellEnabled)
            {
                if (@this.useGeneralSettingsToSell && slots2sellCache == null && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0)
                {
                    // необходимо использовать глобальный список продажи, а slots2sellCache не сформирован
#if false
                    Interact.SellItems(); 
#else
                    // Производим поиск и продажу предметов, подходящих под глобальный фильтр продажи
                    if (sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> generalSellSlotsCache))
                        foreach(InventorySlot slot in generalSellSlotsCache)
                            if (Astral.Logic.NW.Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
#endif
                }
                if (ItemFilterCore_SellItems != null && @this.sellOptions.Entries.Count > 0)
                {
                    // Задан локальный фильтр продажи
#if false
                    EntityManager.LocalPlayer.BagsItems.AddRange(Professions2.CraftingBags);

                    foreach (InventorySlot inventorySlot in EntityManager.LocalPlayer.BagsItems)
                        if (Astral.Logic.NW.Inventory.CanSell(inventorySlot.Item) && sellItems(@this._sellOptions)(inventorySlot.Item))
                        {
                            Logger.WriteLine("Sell : '" + inventorySlot.Item.DisplayName + "'");
                            inventorySlot.StoreSellItem();
                            Thread.Sleep(250);
                        } 
#else
                    if(slots2sellCache?.Count > 0)
                    {
                        // lots2sellCache сформирован, дополнительный происк производить не нужно
                        foreach (InventorySlot slot in slots2sellCache)
                            if (Astral.Logic.NW.Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
                    }
                    else if(sellBags.GetItems(@this.sellOptions, out List<InventorySlot> slots2sell))
                        foreach(InventorySlot slot in slots2sell)
                            if (Astral.Logic.NW.Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
                    else Logger.WriteLine("Nothing to sell !");
#endif
                }
            }
            else Logger.WriteLine("Can't sell to this vendor !");
        }

#if false
        /// <summary>
        /// Подсчет количества предметов, заданных данной позицией списка покупок, которые необходимо (до)купить
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public uint NumberOfItem2Buy(ItemFilterEntryExt item2buy)
        {
            uint toBuyNum = item2buy.Count;
            if (item2buy.KeepNumber)
            {
                uint haveItemNum = item2buy.CountItemInBag();
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0;
            }
            return toBuyNum;
        }
#endif
        /// <summary>
        /// Проверка корректности вендора и возможности взаимодействия с ним в текущей локации
        /// </summary>
        /// <param name="npcInfos"></param>
        /// <returns></returns>
        private bool Validate(NPCInfos npcInfos)
        {
            return npcInfos != null && !string.IsNullOrEmpty(npcInfos.CostumeName)
                && (npcInfos.MapName == "All" || (npcInfos.MapName == EntityManager.LocalPlayer.MapState.MapName && npcInfos.RegionName == EntityManager.LocalPlayer.RegionInternalName));
        }

        /// <summary>
        /// Проверка наличия в сумке свободных слотов.
        /// </summary>
        /// <returns></returns>
        private static bool Check_FreeSlots()
        {
            return EntityManager.LocalPlayer.BagsFreeSlots > 0
                && EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).FilledSlots == 0;
        }

        private static bool Check_PlayerLevel(Item item)
        {
#if false
            item.ItemDef.UsageRestriction.MinLevel; // Минимальный требуемый уровень персонажа
            item.ItemDef.UsageRestriction.AllowedClasses; // Список допустимых классов
            item.ItemDef.Level; // Уроверь предмета  
#endif

            return false;
        }

        private static bool Check_ReadyToTraid(ScreenType screenType)
        {
            return screenType == ScreenType.Store || screenType == ScreenType.StoreCollection;
        }
    }
}
