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
using EntityTools.Enums;
using EntityTools.Tools.BuySellItems;
using EntityTools.UIEditors;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using NPCInfos = Astral.Quester.Classes.NPCInfos;

namespace EntityTools.Quester.Actions
{
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
        public NPCInfos Vendor { get => _vendor; set => _vendor = value; }
        private NPCInfos _vendor = new NPCInfos();

#if DEVELOPER
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Description("Specific dialogs before reaching item list.")]
        [Category("Vendor")]
#else
        [Browsable(false)]
#endif
        public List<string> BuyMenus { get => _buyMenus; set => _buyMenus = value; }
        private List<string> _buyMenus = new List<string>();

#if DEVELOPER
        //[Editor(typeof(BuyOptionsEditor), typeof(UITypeEditor))]
        [Category("Purchase")]
#else
        [Browsable(false)]
#endif
        public List<ItemFilterEntryExt> BuyOptions { get => _buyOptions; set => _buyOptions = value; }
        private List<ItemFilterEntryExt> _buyOptions = new List<ItemFilterEntryExt>();

#if DEVELOPER
        [Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public ItemFilterCore SellOptions { get => _sellOptions; set => _sellOptions = value; }
        private ItemFilterCore _sellOptions;

#if DEVELOPER
        [Description("Use options set in general settings to sell")]
        [Category("Purchase")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToSell { get => _useGeneralSettingsToSell; set => _useGeneralSettingsToSell = value; }
        private bool _useGeneralSettingsToSell = true;

#if DEVELOPER
        [Description("Use options set in general settings to buy")]
        [Category("Purchase")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToBuy { get => _useGeneralSettingsToBuy; set => _useGeneralSettingsToBuy = value; }
        private bool _useGeneralSettingsToBuy = false;

#if DEVELOPER
        [Category("Restriction")]
        [Description("Проверять наличие свободных слотов сумки")]
#else
        [Browsable(false)]
#endif
        public bool CheckFreeBags { get => _checkFreeBags; set => _checkFreeBags = value; }
        private bool _checkFreeBags = true;

#if DEVELOPER
        [Description("True: Закрывать диалоговое окно после покупки\n\r" +
            "False: Оставлять диалоговое окно открытым (значение по умолчанию)")]
#else
        [Browsable(false)]
#endif
        public bool CloseContactDialog { get => _closeContactDialog; set => _closeContactDialog = value; }
        private bool _closeContactDialog = false;

#if DEVELOPER
        [Description("Продолжительно торговой сессии")]
#else
        [Browsable(false)]
#endif
        public uint Timer { get => _timer; set => _timer = value; }
        private uint _timer = 10000;
        #endregion

        public BuySellItemsExt() { }

        #region Интерфейс Quester.Action
        public override string ActionLabel => GetType().Name;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => Validate(Vendor) && (UseGeneralSettingsToBuy || BuyOptions.Count > 0);
        protected override Vector3 InternalDestination => Vendor.Position;
        public override void GatherInfos()
        {
#if DEVELOPER
            if (NPCVendorInfosExtEditor.SetInfos(out NPCInfos npcInfos))
            {
                Vendor = npcInfos;
                if (XtraMessageBox.Show("Add a dialog ? (open the dialog window before)", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DialogEdit.Show(BuyMenus);
                }
            } 
#endif
        }
        public override void InternalReset() { tries = 3; }
        public override void OnMapDraw(GraphicsNW graph) { }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (!Validate(Vendor))
                    return new ActionValidity("Vendor does not set");

                if (!UseGeneralSettingsToBuy && (BuyOptions == null || BuyOptions.Count == 0))
                    return new ActionValidity("Items to buy are not specified");

                return new ActionValidity();
            }
        }
        #endregion

        public override bool NeedToRun => tries > 0 && Validate(@this._vendor) && (!@this._vendor.Position.IsValid || @this._vendor.Position.Distance3DFromPlayer < 25);

        private uint tries = 3;

        public override ActionResult Run()
        {
            tries--;
            switch (@this._vendor.CostumeName)
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
                    if (remoteContact != null)
                    {
                        remoteContact.Start();
                        Interact.WaitForInteraction();
                        return RemouteTraiding(remoteContact);
                    }
                    else return ActionResult.Fail;
                default:
                    ContactInfo contactInfo = EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts.Find((ct) => ct.Entity.IsValid && Vendor.IsMatching(ct.Entity));
                    if (contactInfo != null)
                        return Traiding(contactInfo.Entity);
                    else return ActionResult.Running;
            }
        }

        private ActionResult RemouteTraiding(RemoteContact remoteContact)
        {
            remoteContact.Start();
            Interact.WaitForInteraction();

            ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
            if (screenType == ScreenType.List || screenType == ScreenType.StoreCollection)
            {
                if (BuyMenus.Count > 0)
                {
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
                }

                foreach (ItemFilterEntryExt item2buy in BuyOptions)
                {
                    foreach (StoreItemInfo storeItemInfo in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems)
                    {
                        if (storeItemInfo.CanBuyError == 0u && item2buy.IsMatch(storeItemInfo.Item))
                        {
                            Logger.WriteLine($"Buy {item2buy.Count} {storeItemInfo.Item.ItemDef.DisplayName} ...");
                            storeItemInfo.BuyItem(item2buy.Count);
                            Thread.Sleep(250);
                            break;
                        }
                    }
                }
                if (CloseContactDialog)
                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();

                return ActionResult.Completed;
            }
            return ActionResult.Running;
        }

        private ActionResult Traiding(Entity vendorEntity)
        {
            ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
            if (screenType == ScreenType.List || screenType == ScreenType.StoreCollection)
            {
                if (vendorEntity is null)
                    return ActionResult.Running;

                if (Interact.VendorWithDialogs(vendorEntity, @this._buyMenus))
                {
                    SellItems();

                    BuyItems();

                    if (@this._closeContactDialog)
                        EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();

                    return ActionResult.Completed;
                }
                return ActionResult.Fail;
            }
            return ActionResult.Running;
        }

        private BuyItemResult BuyItems()
        {
            if (CheckFreeSlots())
            {
                if (UseGeneralSettingsToBuy)
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
                foreach (ItemFilterEntryExt item2Buy in @this._buyOptions)
                {
                    BuyAnItem(item2Buy);

                    if (CheckFreeSlots())
                        return BuyItemResult.FullBag;
                }

                return BuyItemResult.Completed;
            }
            else return BuyItemResult.FullBag;
        }

        /// <summary>
        /// Обработка одной позиции списка покупок
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        private BuyItemResult BuyAnItem(ItemFilterEntryExt item2buy)
        {
            foreach (StoreItemInfo storeItemInfo in EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems)
            {
                if (storeItemInfo.CanBuyError == 0u)
                {
                    if (item2buy.IsMatch(storeItemInfo.Item))
                    {
                        Astral.Classes.Timeout timeout = new Astral.Classes.Timeout((int)@this._timer);

                        //Вычисляем количество предметов, которые необходимо докупить
                        uint toBuyNum = 0;
                        while (!timeout.IsTimedOut)
                        {
                            if (CheckFreeSlots())
                            {
                                if ((toBuyNum = NumberOfItem2By(item2buy)) > 0)
                                {
                                    // Здесь нужно добавить проверку стоимости

                                    // Покупка предмета
                                    Logger.WriteLine(string.Concat("Buy ", item2buy.Count, ' ', storeItemInfo.Item.ItemDef.DisplayName, "..."));
                                    storeItemInfo.BuyItem(toBuyNum);
                                    Thread.Sleep(250);
                                }
                                else return BuyItemResult.Succeeded;
                            }
                            else return BuyItemResult.FullBag;
                        };
                        return BuyItemResult.Completed;
                    }
                    else return BuyItemResult.Skiped;
                }
                else return BuyItemResult.Error;
            }
            return BuyItemResult.Completed;
        }

        /// <summary>
        /// Продажа всех предметов
        /// </summary>
        private void SellItems()
        {
#if false
            if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SellEnabled)
            {
                if (@this._useGeneralSettingsToSell)
                    Interact.SellItems();
                if (@this._sellOptions.Entries.Count > 0)
                {
                    EntityManager.LocalPlayer.BagsItems.AddRange(Professions2.CraftingBags);

                    foreach (InventorySlot inventorySlot in EntityManager.LocalPlayer.BagsItems)
                        if (Astral.Logic.NW.Inventory.CanSell(inventorySlot.Item) && SellOptions.method_0(inventorySlot.Item))
                        {
                            Logger.WriteLine("Sell : '" + inventorySlot.Item.DisplayName + "'");
                            inventorySlot.StoreSellItem();
                            Thread.Sleep(250);
                        }
                }
            }
            else Logger.WriteLine("Can't sell to this vendor !");
#else
            throw new NotImplementedException(System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
        }

        /// <summary>
        /// Подсчет количества предметов, заданных данной позицией списка покупок, которые необходимо (до)купить
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        public uint NumberOfItem2By(ItemFilterEntryExt item2buy)
        {
            uint toBuyNum = item2buy.Count;
            if (item2buy.OverallNumber)
            {
                uint haveItemNum = item2buy.CountItemInBag();
                if (haveItemNum < toBuyNum)
                    toBuyNum -= haveItemNum;
                else toBuyNum = 0;
            }
            return toBuyNum;
        }

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
        private static bool CheckFreeSlots()
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
    }
}
