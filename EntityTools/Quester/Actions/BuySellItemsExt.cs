//#define grouping_filterEntry_by_priority
#define ItemFilterGroup_for_buying

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
using EntityTools.Tools.ItemFilter;
using static EntityTools.Tools.ItemFilter.BuyFilterEntry;
using EntityTools.Extensions;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using NPCInfos = Astral.Quester.Classes.NPCInfos;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class BuySellItemsExt : Astral.Quester.Classes.Action
    {
        internal readonly InstancePropertyAccessor<BuySellItemsExt, ActionDebug> debug = null;

        BuySellItemsExt @this => this;

        #region Опции команды
#if DEVELOPER
        [Editor(typeof(VendorInfoEditor), typeof(UITypeEditor))]
        [Category("Vendor")]
#else
        [Browsable(false)]
#endif
        public VendorInfo Vendor { get => _vendor; set => _vendor = value; }
        internal VendorInfo _vendor = new VendorInfo();

#if DEVELOPER
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Description("Specific dialogs before reaching item list.")]
        [Category("Vendor")]
#else
        [Browsable(false)]
#endif
        [XmlElement(typeof(List<string>), ElementName = "BuyMenus")]
        public List<string> VendorMenus { get => _vendorMenus; set => _vendorMenus = value; }
        internal List<string> _vendorMenus = new List<string>();

#if DEVELOPER
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        [Editor(typeof(ItemFilterCoreExtEditor<CommonFilterEntry>), typeof(UITypeEditor))]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public ItemFilterCoreExt<CommonFilterEntry> SellOptions
        {
            get => _sellOptions;
            set
            {
                if (_sellOptions != value)
                {
                    _sellOptions = value;
                    slots2sellCache = null;
                }
            }
        }
        internal ItemFilterCoreExt<CommonFilterEntry> _sellOptions = new ItemFilterCoreExt<CommonFilterEntry>();

#if DEVELOPER
        [Description("Use options set in general settings to sell")]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToSell
        {
            get => _useGeneralSettingsToSell; set
            {
                if (_useGeneralSettingsToSell != value)
                {
                    _useGeneralSettingsToSell = value;
                    slots2sellCache = null; 
                }
            }
        }
        internal bool _useGeneralSettingsToSell = true;

#if DEVELOPER
        [Description("Список сумок, предметы из которых могут быть проданы")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public BagsList SellBags
        {
            get => _sellBags;
            set
            {
                if (!Equals(_sellBags, value))
                {
                    _sellBags = CopyHelper.CreateDeepCopy(value);
                    slots2sellCache = null;
                }
            }
        }
        internal BagsList _sellBags = BagsList.GetPlayerBags();

#if DEVELOPER
        [Editor(typeof(ItemFilterCoreExtEditor<BuyFilterEntry>), typeof(UITypeEditor))]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public ItemFilterCoreExt<BuyFilterEntry> BuyOptions { get => _buyOptions; set => _buyOptions = value; }
        internal ItemFilterCoreExt<BuyFilterEntry> _buyOptions = new ItemFilterCoreExt<BuyFilterEntry>();

#if BoundingRestriction
#if DEVELOPER
        [Description("")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public BoundingRestrictionType BoundingRestriction
        {
            get => _boundingRestriction;
            set
            {
                if (_boundingRestriction != value)
                {
                    _boundingRestriction = value;
                }
            }

        }
        internal BoundingRestrictionType _boundingRestriction = BoundingRestrictionType.None; 
#endif

#if DEVELOPER
        [Description("Use options set in general settings to buy")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToBuy { get => _useGeneralSettingsToBuy; set => _useGeneralSettingsToBuy = value; }
        internal bool _useGeneralSettingsToBuy = false;

#if DEVELOPER
        [Description("Проверять наличие свободных слотов сумки")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public bool CheckFreeBags { get => _checkFreeBags; set => _checkFreeBags = value; }
        internal bool _checkFreeBags = true;

#if DEVELOPER
        [Description("True: Закрывать диалоговое окно после покупки\n\r" +
            "False: Оставлять диалоговое окно открытым (значение по умолчанию)")]
#else
        [Browsable(false)]
#endif
        public bool CloseContactDialog { get => _closeContactDialog; set => _closeContactDialog = value; }
        internal bool _closeContactDialog = false;

#if DEVELOPER
        [Description("Список сумок, в которых производится поиск предметов для сравнения с предлагаемыми в магазине")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public BagsList Bags
        {
            get => _buyBags;
            set
            {
                if(!Equals(_buyBags, value))
                {
                    _buyBags = CopyHelper.CreateDeepCopy(value);
                }
            }
        }
        internal BagsList _buyBags = BagsList.GetFullPlayerInventory();

#if DEVELOPER
        [Description("Продолжительно торговой операции (покупки 1 предмета)")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public uint Timer { get => _timer; set => _timer = value; }
        internal uint _timer = 10000;

#if DEVELOPER
        [Description("Количество попыток инициировать торговую сессию")]
#else
        [Browsable(false)]
#endif
        public uint Tries
        {
            get => _tries;
            set
            {
                _tries = value;
                tryNum = value;
            }
        }
        internal uint _tries = 3;
        private uint tryNum;
        #endregion

        public BuySellItemsExt()
        {
            tryNum = _tries;
            debug = this.GetInstanceProperty<BuySellItemsExt, ActionDebug>("Debug");
        }

        /// <summary>
        /// Кэшированный список слотов, которые подлежат продаже
        /// </summary>
        private List<InventorySlot> slots2sellCache = null;

        #region Интерфейс Quester.Action
        public override string ActionLabel => GetType().Name;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions
        {
            get
            {
#if change_on_20200603_0958
                if (!Validate(@this._vendor))
                    return false;

                // Функционал перенесен в VendorInfo.IsAvailable
                switch (@this._vendor.VendorType)
                {
                    case VendorType.None:
                        return false;
                    case VendorType.Auto:
                        return false;
                    case VendorType.ArtifactVendor:
                        if (!SpecialVendor.IsAvailable())
                            return false;
                        break;
                    case VendorType.VIPSummonSealTrader:
                        if (!VIP.CanSummonSealTrader)
                            return false;
                        break;
                    case VendorType.VIPProfessionVendor:
                        if (!VIP.CanSummonProfessionVendor)
                            return false;
                        break;
                } 
#else
                if (!@this._vendor.IsAvailable)
                    return false;
#endif

                if (@this._useGeneralSettingsToSell)
                {
                    if (_sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> slots, true))
                        slots2sellCache = slots;
                }

                if (@this._sellOptions != null && @this._sellOptions.IsValid)
                {
                    if (_sellBags.GetItems(@this._sellOptions, out List<InventorySlot> slots, true))
                        if (slots2sellCache != null)
                            slots2sellCache.AddRange(slots);
                        else slots2sellCache = slots;
                }

                return slots2sellCache?.Count > 0 
                    || @this._useGeneralSettingsToBuy && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0 
                    || @this._buyOptions.IsValid;
            }
        }
        protected override Vector3 InternalDestination => Vendor.Position;
        public override void GatherInfos()
        {
#if DEVELOPER
            if (VendorInfoEditor.SetInfos(out VendorInfo vndInfo))
            {
                Vendor = vndInfo;
                if (XtraMessageBox.Show("Add a dialog ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    switch (@this._vendor.VendorType)
                    {
                        case VendorType.ArtifactVendor:
                            SpecialVendor.UseItem();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case VendorType.VIPSealTrader:
                            VIP.SummonSealTrader();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case VendorType.VIPProfessionVendor:
                            VIP.SummonProfessionVendor();
                            //Thread.Sleep(3000);
                            Interact.WaitForInteraction();
                            break;
                        case VendorType.RemoteVendor:
                            RemoteContact remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.Find((ct) => ct.ContactDef == Vendor.CostumeName);
                            if (remoteContact != null)
                            {
                                remoteContact.Start();
                                Interact.WaitForInteraction();
                            }
                            break;
                        default:
                            ContactInfo contactInfo = EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts.Find((ct) => ct.Entity.IsValid && Vendor.IsMatch(ct.Entity));
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
            tryNum = _tries;
            slots2sellCache = null;
        }
        public override void OnMapDraw(GraphicsNW graph) { }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (!@this._vendor.IsValid)
                    return new ActionValidity("Vendor does not set");

                if (!(@this._useGeneralSettingsToBuy || (@this._buyOptions != null && @this._buyOptions.IsValid)
                    || @this._useGeneralSettingsToSell || (@this._sellOptions != null && @this._sellOptions.IsValid)))
                    return new ActionValidity("The items to buy or sell are not specified!");

                return new ActionValidity();
            }
        }
        #endregion

        public override bool NeedToRun => (!@this._vendor.Position.IsValid || @this._vendor.Position.Distance3DFromPlayer < 25) && @this._vendor.IsAvailable;

        public override ActionResult Run()
        {
            bool extendedActionDebugInfo  = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            Entity theVendor = null;
            string methodName = MethodBase.GetCurrentMethod().Name;
            if (tryNum > 0)
            {
                tryNum--;
                switch (@this._vendor.VendorType)
                {
                    case VendorType.None:
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Vendor not specified. Fail"));
                        return ActionResult.Fail;
                    case VendorType.ArtifactVendor:
                        theVendor = SpecialVendor.VendorEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (SpecialVendor.IsAvailable())
                            {
                                if (extendedActionDebugInfo)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Summon 'ArtifactVendor'"));
                                SpecialVendor.UseItem();
                                Thread.Sleep(3000);
                                theVendor = SpecialVendor.VendorEntity;
                                if (extendedActionDebugInfo)
                                {
                                    if (theVendor != null)
                                        debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' was successfully summoned"));
                                    else debug.Value.AddInfo(string.Concat(methodName, ": Summoning failed"));
                                }
                            }
                            else
                            {
                                if (extendedActionDebugInfo)
                                    debug.Value.AddInfo(string.Concat(methodName, ": 'ArtifactVendor' does not available. Failed"));
                                return ActionResult.Fail;
                            }
                        }
                        else if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' already summoned"));
                        return Traiding(theVendor);
                    case VendorType.VIPSealTrader:
                        theVendor = VIP.SealTraderEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (extendedActionDebugInfo)
                                debug.Value.AddInfo(string.Concat(methodName, ": Summon 'VIPSealTrader'"));
                            VIP.SummonSealTrader();
                            Thread.Sleep(3000);
                            theVendor = VIP.SealTraderEntity;
                            if (extendedActionDebugInfo)
                            {
                                if (theVendor != null)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' was successfully summoned"));
                                else debug.Value.AddInfo(string.Concat(methodName, ": Summoning failed"));
                            }
                        }
                        else if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' already summoned"));
                        return Traiding(theVendor);
                    case VendorType.VIPProfessionVendor:
                        theVendor = VIP.ProfessionVendorEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Summon 'VIPProfessionVendor'"));
                            VIP.SummonProfessionVendor();
                            Thread.Sleep(3000);
                            theVendor = VIP.ProfessionVendorEntity;
                            if (extendedActionDebugInfo)
                            {
                                if (theVendor != null)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' was successfully summoned"));
                                else debug.Value.AddInfo(string.Concat(methodName, ": Summoning failed"));
                            }
                        }
                        else if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", theVendor.InternalName, "' already summoned"));
#if true
                        return Traiding(theVendor); 
#else
                        Entity professionsVendor = VIP_GetNearestEntityByCostume?.Invoke("Vip_Professions_Vendor");
                        if (professionsVendor != null)
                            return Traiding(professionsVendor);
                        else return ActionResult.Running;
#endif
                    case VendorType.RemoteVendor:
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Call 'RemoteVendor'"));
                        RemoteContact remoteContact = EntityManager.LocalPlayer.Player.InteractInfo.RemoteContacts.Find((ct) => ct.ContactDef == _vendor.CostumeName);
                        if (extendedActionDebugInfo)
                        {
                            if (remoteContact != null && remoteContact.IsValid)
                                debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", remoteContact.ContactDef, "' was called successfully"));
                            else debug.Value.AddInfo(string.Concat(methodName, ": Calling failed"));
                        }
                        return RemouteTraiding(remoteContact);
                    default:
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Search '", _vendor.CostumeName, '\''));
                        // ищем ближайший "контакт" совпадающий с Vendor
                        double dist = double.MaxValue;
                        ContactInfo contactInfo = null;
                        foreach (var cntInfo in EntityManager.LocalPlayer.Player.InteractInfo.NearbyContacts)
                        {
                            double curDist = cntInfo.Entity.Location.Distance3DFromPlayer;
                            if (curDist < dist && _vendor.IsMatch(cntInfo.Entity))
                            {
                                contactInfo = cntInfo;
                                dist = curDist;
                            }
                        }
                        if (extendedActionDebugInfo)
                        {
                            if (contactInfo != null && contactInfo.IsValid)
                                debug.Value.AddInfo(string.Concat(methodName, ": Vendor '", contactInfo.Entity.Name, '[', contactInfo.Entity.InternalName, "]' was successfully found at the Distance = ", dist));
                            else debug.Value.AddInfo(string.Concat(methodName, ": Search failed"));
                        }
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
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;
            if (remoteContact != null)
            {
                if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": Begins"));

                remoteContact.Start();
                Interact.WaitForInteraction();

                ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                if (contactDialog.IsValid && (screenType == ScreenType.List || screenType == ScreenType.Store || screenType == ScreenType.StoreCollection))
                {
                    // взаимодействие с вендором для открытия окна магазина
                    if (@this._vendorMenus.Count > 0)
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
                            if (@this._vendorMenus.Count > 0)
                                Interact.DoDialog(_vendorMenus);
                        else if (Check_ReadyToTraid(screenType))
                        // Открыто витрина магазина (список товаров)
                        // необходимо переключиться на нужную вкладку
                        {
                            if (@this._vendorMenus.Count > 0)
                            {
                                string key = @this._vendorMenus.Last();
                                if (contactDialog.HasOptionByKey(key))
                                    contactDialog.SelectOptionByKey(key, "");
                            }
                        }
                        if (@this._closeContactDialog)
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
                        if (@this._closeContactDialog)
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
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;
            if (vendorEntity != null)
            {
                if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": Begins"));

                // взаимодействие с вендором для открытия окна магазина
                if (ApproachAndInteractToVendor(vendorEntity))
                {
#if false
                    // ApproachAndInteractToVendor гарантирует, что торговец может совершать сделки
                    // дополнительная проверка не нужна
                    ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                    if (/*screenType == ScreenType.List || */screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)

#endif
                    {
                        // Продажа предметов
                        SellItems();

                        // Покупка предметов
                        ActionResult result = BuyItems();

                        // Закрытие диалогового окна
                        if (@this._closeContactDialog)
                        {
                            if (extendedActionDebugInfo)
                                debug.Value.AddInfo(string.Concat(methodName, ": CloseContactDialog"));

                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                        }
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
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;

            if (extendedActionDebugInfo)
                debug.Value.AddInfo(string.Concat(methodName, ": Begins"));
            bool ready = false;

            // Проверяем соответствие 
            if (@this._vendor.IsMatch(entity)) 
            {
                if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": VendorEntity is ok"));

                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                if (contactDialog.IsValid)
                {
                    ScreenType screenType = contactDialog.ScreenType;

                    if (extendedActionDebugInfo)
                        debug.Value.AddInfo(string.Concat(methodName, ": ContactDialog is valid. ScreenType = ", screenType));
 
                    // Открыто диалоговое окно
                    if (screenType == ScreenType.List || screenType == ScreenType.Buttons)
                    {
                        // Открыто диалоговое окно продавца
                        if (@this._vendorMenus.Count > 0)
                        {
                            if (extendedActionDebugInfo)
                                debug.Value.AddInfo(string.Concat(methodName, ": Call Interact.DoDialog(..)"));
                            Interact.DoDialog(_vendorMenus);
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": ReadyToTraid = ", ready));

                        return ready;
                    }
                    else if (screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)
                    // Открыто витрина магазина (список товаров)
                    // необходимо переключиться на нужную вкладку
                    {
                        if (@this._vendorMenus.Count > 0)
                        {
                            string key = @this._vendorMenus.Last();
                            if (contactDialog.HasOptionByKey(key))
                            {
                                if (extendedActionDebugInfo)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Select Shop-Tab"));

                                contactDialog.SelectOptionByKey(key, "");
                            }
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": ReadyToTraid = ", ready));

                        return ready;
                    }
                    if (@this._closeContactDialog)
                    {
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": CloseContactDialog"));
                        contactDialog.Close();
                    }
                }
                else if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": ContactDialog is invalid"));

                double dist = entity.Location.Distance3DFromPlayer;
                if (!contactDialog.IsValid || dist > 10)
                {
                    // Расстояние до продавца больше 10
                    if (extendedActionDebugInfo)
                        debug.Value.AddInfo(string.Concat(methodName, ": Distance to the Vendor = ", dist, ". Call Interact.VendorWithDialogs(..)"));
                        
                    bool interactResult = Interact.VendorWithDialogs(entity, _vendorMenus);

                    ready = Check_ReadyToTraid(EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType);
                    if (extendedActionDebugInfo)
                    {
                        debug.Value.AddInfo(string.Concat(methodName, ": InteractionResult = ", interactResult, ", ReadyToTraid = ", ready));
                    }
                    return interactResult && ready;
                }
                ready = Check_ReadyToTraid(contactDialog.ScreenType);
                if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": ReadyToTraid = ", ready));

                return ready;
            }
		    else if (extendedActionDebugInfo)
                debug.Value.AddInfo(string.Concat(methodName, ": VendorEntity checks failed"));

            return false;
        }

        /// <summary>
        /// Покупка предметов
        /// </summary>
        /// <returns></returns>
        private ActionResult BuyItems()
        {
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;
            if (extendedActionDebugInfo)
                debug.Value.AddInfo(string.Concat(methodName, ": Begins"));

            if (!_checkFreeBags || Check_FreeSlots(_buyBags))
            {
                if (_useGeneralSettingsToBuy)
                {
                    if (extendedActionDebugInfo)
                        debug.Value.AddInfo(string.Concat(methodName, ": Processing GeneralSettingsToBuy"));

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
                if (_buyOptions.IsValid)
                {
                    if (extendedActionDebugInfo)
                    {
                        debug.Value.AddInfo(string.Concat(methodName, ": Begins the processing of ", nameof(@this.BuyOptions)));
                        debug.Value.AddInfo(string.Concat(methodName, ": Indexing selected Bags"));
                    }
                    // Анализируем содержимое сумок
                    IndexedBags<BuyFilterEntry> indexedBags = new IndexedBags<BuyFilterEntry>(@this._buyOptions, @this._buyBags);

#if grouping_filterEntry_by_priority
                    uint allowBuyPriority = uint.MaxValue;
                    foreach(var group in indexedBags.Filters.GroupBy((f) => f.Priority))
                    {
                        if (allowBuyPriority >= group.Key)
                        {
                            foreach (var filterEntry in indexedBags.Filters)
                            {
                                if (extendedActionDebugInfo)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Processing FilterEntry: ", filterEntry.ToString()));

                                var slotCache = indexedBags[filterEntry];
                                List<ItemDef> boughtItems = null;
                                BuyItemResult buyItemResult = (slotCache is null) ? BuyAnItem(filterEntry, ref boughtItems) : BuyAnItem(slotCache, ref boughtItems);

                                if (extendedActionDebugInfo)
                                    debug.Value.AddInfo(string.Concat(methodName, ": Result: ", buyItemResult));
                                
                                // Обработка результата покупки
                                // Если результат не позволяет продолжать покупку - прерываем выполнение команды
                                switch (buyItemResult)
                                {
                                    case BuyItemResult.Succeeded:
                                        {
                                            // Предмет необходимо экипировать после покупки
                                            if (filterEntry.Wear && boughtItems != null && boughtItems.Count > 0)
                                            {
                                                foreach (ItemDef item in boughtItems)
                                                {
                                                    Thread.Sleep(1000);
                                                    
                                                    // Здесь не учитывается возможно приобретения и экипировки нескольких вещений (например, колец)
                                                    InventorySlot slot = _buyBags.Find(item);
                                                    if (slot != null)
                                                        slot.Equip();
                                                }
                                            }
                                            // можно продолжать покупки
                                            break;
                                        }
                                    case BuyItemResult.Exists:
                                        // можно продолжать покупки
                                        break;
                                    case BuyItemResult.Completed:
                                        // можно продолжать покупки
                                        break;
                                    case BuyItemResult.FullBag:
                                        return ActionResult.Skip;
                                    case BuyItemResult.Error:
                                        return ActionResult.Fail;
                                    case BuyItemResult.NotEnoughCurrency:
                                        // можно продолжать покупки с приоритетом не выше allowBuyPriority
                                        allowBuyPriority = group.Key;
                                        break;
                                    default:
                                        allowBuyPriority = group.Key;
                                        break;
                                }
                            }
                        }
                    }
#elif ItemFilterGroup_for_buying
                    foreach(var filterGroup in indexedBags.)
#else
                    foreach (var filterEntry in indexedBags.Filters)
                    {
                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Processing FilterEntry: ", filterEntry.ToString()));

                        var slotCache = indexedBags[filterEntry];
                        List<ItemDef> boughtItems = null;
                        BuyItemResult buyItemResult = (slotCache is null) ? BuyAnItem(filterEntry, ref boughtItems) : BuyAnItem(filterEntry, slotCache, ref boughtItems);

                        if (extendedActionDebugInfo)
                            debug.Value.AddInfo(string.Concat(methodName, ": Result: ", buyItemResult));
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

                                            InventorySlot slot = _buyBags.Find(item);
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
#endif
                }
#endif

                return ActionResult.Completed;
            }
            else
            {
                if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, ": Bags is full. Skip"));

                return ActionResult.Skip;
            }
        }

        /// <summary>
        /// Обработка одной позиции списка покупок
        /// </summary>
        /// <param name="item2buy"></param>
        /// <returns></returns>
        private BuyItemResult BuyAnItem(BuyFilterEntry item2buy, ref List<ItemDef> boughtItems)
        {
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;

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
                            if (!item2buy.CheckEquipmentLevel || _buyBags.ContainsBetterItemEquipmentLevel(storeItemInfo))
                            {
                                bool succeeded = false;
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(@this._timer > 0 ? (int)@this._timer : int.MaxValue);

                                if (!_checkFreeBags || Check_FreeSlots(_buyBags))
                                {
                                    //Вычисляем количество предметов, которые необходимо докупить
                                    uint toBuyNum = storeItemInfo.NumberOfItem2Buy(_buyBags, item2buy);
                                    if (toBuyNum > 0)
                                    {
                                        // Проверка наличия валюты заложена в storeItemInfo.CanBuyError
                                        // Код ошибки (нет валюты) = 6

                                        // Максимальное количество предметов в стаке, которые можно купить за одну транзакцию
                                        // 0 - одна штука
                                        uint mayBuyInBulk = storeItemInfo.Item.ItemDef.MayBuyInBulk;

                                        //if (!item2buy.BuyByOne)
                                        if (mayBuyInBulk > toBuyNum)
                                        {
                                            // Единовременная покупка нужного количества предметов
                                            string str = $"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                            if (extendedActionDebugInfo)
                                                debug.Value.AddInfo(string.Concat(methodName, str));
                                            Logger.WriteLine(str);

                                            storeItemInfo.BuyItem(toBuyNum);
                                            Thread.Sleep(250);

                                            succeeded = true;
                                            result = BuyItemResult.Succeeded;
                                        }
                                        else
                                        {
                                            // Покупка предмета в цикле
                                            string str = $"Buy total {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] by one item...";
                                            Logger.WriteLine(str);
                                            if (extendedActionDebugInfo)
                                                debug.Value.AddInfo(str);

                                            uint totalPurchasedNum = 0;
                                            for (totalPurchasedNum = 0; totalPurchasedNum < toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (timeout.IsTimedOut)
                                                {
                                                    str = $"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...";
                                                    Logger.WriteLine(str);
                                                    if (extendedActionDebugInfo)
                                                        debug.Value.AddInfo(string.Concat(methodName, str));
                                                    break;
                                                }
                                                if (_checkFreeBags && !Check_FreeSlots(_buyBags))
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    Logger.WriteLine(str);
                                                    if (extendedActionDebugInfo)
                                                        debug.Value.AddInfo(string.Concat(methodName, str));
                                                    return result = BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                    }
                                }
                                else
                                {
                                    if (extendedActionDebugInfo)
                                        debug.Value.AddInfo("Bags are full, skip ...");

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
        private BuyItemResult BuyAnItem(SlotCache<BuyFilterEntry> slotCache, ref List<ItemDef> boughtItems)
#endif
        {
            bool extendedActionDebugInfo = EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo;
            string methodName = MethodBase.GetCurrentMethod().Name;

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

                    if (slotCache.Filter.IsMatch(storeItemInfo.Item))
                    {
                        // Проверка соответствия уровню персонажа
                        if (!slotCache.Filter.CheckPlayerLevel || storeItemInfo.FitsPlayerLevel())
                        {
                            // Проверка уровня предмета
                            if (!slotCache.Filter.CheckEquipmentLevel 
                                //|| filterEntryCache.Value.MaxItemLevel <= storeItemInfo.Item.ItemDef.Level)
                                || slotCache.HasWorseThen(storeItemInfo))
                            {
                                bool succeeded = false;
                                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout((int)@this._timer);

                                if (!_checkFreeBags || Check_FreeSlots(_buyBags))
                                {
                                    //Вычисляем количество предметов, которые необходимо докупить
                                    uint toBuyNum = storeItemInfo.NumberOfItem2Buy(slotCache);
                                    if (toBuyNum > 0)
                                    {
                                        uint mayBuyInBulk = storeItemInfo.Item.ItemDef.MayBuyInBulk;

                                        if (mayBuyInBulk > toBuyNum)
                                        {
                                            // Покупка предмета в необходимом количество за один раз
                                            string str = $"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                            if (extendedActionDebugInfo)
                                                debug.Value.AddInfo(string.Concat(methodName, str));
                                            Logger.WriteLine(str);


                                            storeItemInfo.BuyItem(toBuyNum);
                                            Thread.Sleep(250);

                                            succeeded = true;
                                            result = BuyItemResult.Succeeded;
                                        }
                                        else
                                        {
                                            // Невозможна единовременная покупка необходимого количества предметов, 
                                            // производим покупку в цикле
                                            string str = $"Buy total {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] by one item...";
                                            if (extendedActionDebugInfo)
                                                debug.Value.AddInfo(string.Concat(methodName, str));
                                            Logger.WriteLine(str);

                                            uint totalPurchasedNum = 0;
                                            for (totalPurchasedNum = 0; totalPurchasedNum < toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (@this._timer > 0 && timeout.IsTimedOut)
                                                {
                                                    str = $"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...";
                                                    if (extendedActionDebugInfo)
                                                        debug.Value.AddInfo(string.Concat(methodName, str));
                                                    Logger.WriteLine(str);
                                                    break;
                                                }
                                                if(storeItemInfo.CanBuyError != 0u)
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Buying is impossible now. Error code '{storeItemInfo.CanBuyError}'. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    if (extendedActionDebugInfo)
                                                        debug.Value.AddInfo(string.Concat(methodName, str));
                                                    Logger.WriteLine(str);

                                                    return result = BuyItemResult.NotEnoughCurrency;
                                                }
                                                if (_checkFreeBags && !Check_FreeSlots(_buyBags))
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    if (extendedActionDebugInfo)
                                                        debug.Value.AddInfo(string.Concat(methodName, str));
                                                    Logger.WriteLine(str);
                                                    return result = BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                    }
                                }
                                else
                                {
                                    if (extendedActionDebugInfo)
                                        debug.Value.AddInfo(string.Concat(methodName, "Bags are full, skip ..."));
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
#if false
                    else if (extendedActionDebugInfo)
                        debug.Value.AddInfo(string.Concat(methodName, ); 
#endif
                }
#if false
                else if (extendedActionDebugInfo)
                    debug.Value.AddInfo(string.Concat(methodName, "Buying failed. ErrorCode = ", storeItemInfo.CanBuyError)); 
#endif
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
                if (@this._useGeneralSettingsToSell && slots2sellCache == null && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0)
                {
                    // необходимо использовать глобальный список продажи, а slots2sellCache не сформирован
#if false
                    Interact.SellItems(); 
#else
                    // Производим поиск и продажу предметов, подходящих под глобальный фильтр продажи
                    if (_sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> generalSellSlotsCache, true))
                        foreach(InventorySlot slot in generalSellSlotsCache)
                            if (Astral.Logic.NW.Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
#endif
                }
                if (@this._sellOptions.IncludeFilters.Count > 0)
                {
                    // Задан локальный фильтр продажи
                    if (slots2sellCache?.Count > 0)
                    {
                        // lots2sellCache сформирован, дополнительный происк производить не нужно
                        foreach (InventorySlot slot in slots2sellCache)
                            TrySellAnItem(slot);
                    }
                    else if (_sellBags.GetItems(@this._sellOptions, out List<InventorySlot> slots2sell, true))
                    {
                        foreach (InventorySlot slot in slots2sell)
                            TrySellAnItem(slot);
                    }
                    else Logger.WriteLine("Nothing to sell !");
                }
            }
            else Logger.WriteLine("Can't sell to this vendor !");
        }

        private void TrySellAnItem(InventorySlot slot)
        {
            if (Astral.Logic.NW.Inventory.CanSell(slot.Item))
#if BoundingRestriction
                switch (_boundingRestriction)
                {
                    case BoundingRestrictionType.None:
                        Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                        slot.StoreSellItem();
                        Thread.Sleep(250);
                        break;
                    case BoundingRestrictionType.Unbounded:
                        if ((slot.Item.Flags & (uint)BoundingRestrictionType.Bounded) == 0u)
                        {
                            Logger.WriteLine(string.Concat("Sell unbounded items : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                            slot.StoreSellItem();
                            Thread.Sleep(250);
                        }
                        break;
                    case BoundingRestrictionType.Bounded:
                        if ((slot.Item.Flags & (uint)BoundingRestrictionType.Bounded) > 0u)
                        {
                            Logger.WriteLine(string.Concat("Sell bounded items : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                            slot.StoreSellItem();
                            Thread.Sleep(250);
                        }
                        break;
                    case BoundingRestrictionType.CharacterBounded:
                        if ((slot.Item.Flags & (uint)BoundingRestrictionType.CharacterBounded) > 0u)
                        {
                            Logger.WriteLine(string.Concat("Sell bounded to character items : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                            slot.StoreSellItem();
                            Thread.Sleep(250);
                        }
                        break;
                    case BoundingRestrictionType.AccountBounded:
                        if ((slot.Item.Flags & (uint)BoundingRestrictionType.AccountBounded) > 0u)
                        {
                            Logger.WriteLine(string.Concat("Sell bounded to account items : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                            slot.StoreSellItem();
                            Thread.Sleep(250);
                        }
                        break;
                } 
#else
            {
                Logger.WriteLine(string.Concat("Sell : ", slot.Item.DisplayName, '[', slot.Item.ItemDef.InternalName, "] x ", slot.Item.Count));
                slot.StoreSellItem();
                Thread.Sleep(250);
            }
#endif
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
#if disabled_20200603_1002
        // Функционал перенесен в VendofInfo.IsAvailable

        /// <summary>
        /// Проверка корректности вендора и возможности взаимодействия с ним в текущей локации
        /// </summary>
        /// <param name="vndInfo"></param>
        /// <returns></returns>
        private bool Validate(VendorInfo vndInfo)
        {
            return vndInfo != null && !string.IsNullOrEmpty(vndInfo.CostumeName)
                && (vndInfo.MapName == "All" || (vndInfo.MapName == EntityManager.LocalPlayer.MapState.MapName && vndInfo.RegionName == EntityManager.LocalPlayer.RegionInternalName));
        } 
#endif

        /// <summary>
        /// Проверка наличия в сумке свободных слотов.
        /// </summary>
        /// <returns></returns>
        private static bool Check_FreeSlots(BagsList bags)
        {
#if changed_20200603_0919
            return EntityManager.LocalPlayer.BagsFreeSlots > 0
        && EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).FilledSlots == 0;
#else
            bool result = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Overflow).FilledSlots == 0;
            if (result)
            {
                // Запасная сумка пуста, поэтому проверяем наличие свободных слотов в основной
                uint freeSlotNum = 0;
                foreach (var bag in bags.GetSelectedBags())
                {
                    if (bag.BagId != InvBagIDs.None)
                        freeSlotNum += bag.MaxSlots - bag.FilledSlots;
                }
                result = freeSlotNum > 0;
            }
            return result;
#endif
        }

        private static bool Check_ReadyToTraid(ScreenType screenType)
        {
            return screenType == ScreenType.Store || screenType == ScreenType.StoreCollection;
        }
    }
}
