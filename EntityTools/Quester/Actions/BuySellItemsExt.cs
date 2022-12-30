using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools.Inventory;
using Infrastructure;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = Astral.Quester.Classes.Action;
using Inventory = Astral.Logic.NW.Inventory;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class BuySellItemsExt : Action
    {
        string _idStr;

        #region Опции команды
#if DEVELOPER
        [Editor(typeof(VendorInfoEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
        [Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        [Category("Setting of selling")]
#else
        [Browsable(false)]
#endif
        public ItemFilterCore SellOptions
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
        internal ItemFilterCore _sellOptions = new ItemFilterCore();

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
                    _sellBags = value;
                    slots2sellCache = null;
                }
            }
        }
        internal BagsList _sellBags = BagsList.GetPlayerBags();

#if DEVELOPER
        [Editor(typeof(ItemFilterEntryListEditor), typeof(UITypeEditor))]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public List<ItemFilterEntryExt> BuyOptions { get => _buyOptions; set => _buyOptions = value; }
        private List<ItemFilterEntryExt> _buyOptions = new List<ItemFilterEntryExt>();

#if DEVELOPER
        [Description("Use options set in general settings to buy")]
        [Category("Setting of buying")]
#else
        [Browsable(false)]
#endif
        public bool UseGeneralSettingsToBuy { get => _useGeneralSettingsToBuy; set => _useGeneralSettingsToBuy = value; }
        internal bool _useGeneralSettingsToBuy;

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
        internal bool _closeContactDialog;

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
                if (!Equals(_buyBags, value))
                {
                    _buyBags = value;
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
            _idStr = $"{GetType().Name}[{ActionID}]";
        }

        /// <summary>
        /// Кэшированный список слотов, которые подлежат продаже
        /// </summary>
        private List<InventorySlot> slots2sellCache;

        #region Интерфейс Quester.Action
        public override string ActionLabel => GetType().Name;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions
        {
            get
            {
                if (!_vendor.IsAvailable)
                    return false;

                if (_useGeneralSettingsToSell)
                {
                    if (_sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> slots, true))
                        slots2sellCache = slots;
                }

                if (_sellOptions != null && _sellOptions.Entries.Count > 0)
                {
                    if (_sellBags.GetItems(_sellOptions, out List<InventorySlot> slots, true))
                        if (slots2sellCache != null)
                            slots2sellCache.AddRange(slots);
                        else slots2sellCache = slots;
                }

                return slots2sellCache?.Count > 0
                    || _useGeneralSettingsToBuy && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0
                    || _buyOptions.Count > 0;
            }
        }
        protected override Vector3 InternalDestination => _vendor.Position;
        public override void GatherInfos()
        {
#if DEVELOPER
            if (VendorInfoEditor.SetInfos(out VendorInfo vndInfo))
            {
                _vendor = vndInfo;
                if (XtraMessageBox.Show("Add a dialog ? (open the dialog window before)", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var player = EntityManager.LocalPlayer.Player;
                    switch (_vendor.VendorType)
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
                            RemoteContact remoteContact = player.InteractInfo.RemoteContacts.Find(ct => ct.ContactDef == _vendor.CostumeName);
                            if (remoteContact != null)
                            {
                                remoteContact.Start();
                                Interact.WaitForInteraction();
                            }
                            break;
                        case VendorType.Node:
                            foreach (TargetableNode targetableNode in player.InteractStatus.TargetableNodes)
                            {
                                if (_vendor.IsMatch(targetableNode.WorldInteractionNode))
                                {
                                    var currentNode = new Interact.DynaNode(targetableNode.WorldInteractionNode.Key);

                                    //Approach.NodeForInteraction(currentNode);
                                    //Interact.WaitForInteraction();
                                    //AstralAccessors.Controllers.Engine.MainEngine.Navigation.Stop();
                                    //Thread.Sleep(500);
                                    if (!currentNode.Node.WorldInteractionNode.Interact())
                                    {
                                        break;
                                    }
                                    Interact.WaitForInteraction();
                                }
                            }
                            break;
                        default:
                            ContactInfo contactInfo = player.InteractInfo.NearbyContacts.Find(ct => ct.Entity.IsValid && Vendor.IsMatch(ct.Entity));
                            if (contactInfo != null)
                            {
                                Interact.Vendor(contactInfo.Entity);
                                Interact.WaitForInteraction();
                            }
                            break;
                    }

                    var screenType = player.InteractInfo.ContactDialog.ScreenType;
                    if (screenType == ScreenType.List
                        || screenType == ScreenType.Buttons
                        || screenType == ScreenType.Store
                        || screenType == ScreenType.StoreCollection)
                    {
                        DialogEdit.Show(VendorMenus);
                    }
                }
            }
#endif
        }
        public override void InternalReset()
        {
            tryNum = _tries;
            slots2sellCache = null;
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
            //TODO: Отрисовывать вендора-NPC/Node
        }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (!_vendor.IsValid)
                    return new ActionValidity("Vendor does not set");

                if (!(_useGeneralSettingsToBuy || (_buyOptions != null && _buyOptions.Count > 0)
                    || _useGeneralSettingsToSell || (_sellOptions != null && _sellOptions.Entries.Count > 0)))
                    return new ActionValidity("The items to buy or sell are not specified!");

                return new ActionValidity();
            }
        }
        #endregion

        public override bool NeedToRun => _vendor.Distance < 25 && _vendor.IsAvailable;

        public override ActionResult Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            Entity theVendor;
            string methodName = extendedDebugInfo ? _idStr + "." + MethodBase.GetCurrentMethod().Name : string.Empty;
            if (tryNum > 0)
            {
                var player = EntityManager.LocalPlayer.Player;

                tryNum--;
                switch (_vendor.VendorType)
                {
                    case VendorType.None:
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Vendor not specified. Fail");
                        return ActionResult.Fail;
                    case VendorType.ArtifactVendor:
                        theVendor = SpecialVendor.VendorEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (SpecialVendor.IsAvailable())
                            {
                                if (extendedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summon 'ArtifactVendor'");
                                SpecialVendor.UseItem();
                                Thread.Sleep(3000);
                                theVendor = SpecialVendor.VendorEntity;
                                if (extendedDebugInfo)
                                {
                                    if (theVendor != null)
                                        ETLogger.WriteLine(LogType.Debug,
                                            $"{methodName}: Vendor '{theVendor.InternalName}' was successfully summoned");
                                    else ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summoning failed");
                                }
                            }
                            else
                            {
                                if (extendedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{methodName}: 'ArtifactVendor' does not available. Failed");
                                return ActionResult.Fail;
                            }
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{methodName}: Vendor '{theVendor.InternalName}' already summoned");
                        return Traiding(theVendor);
                    case VendorType.VIPSealTrader:
                        theVendor = VIP.SealTraderEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summon 'VIPSealTrader'");
                            VIP.SummonSealTrader();
                            Thread.Sleep(3000);
                            theVendor = VIP.SealTraderEntity;
                            if (extendedDebugInfo)
                            {
                                if (theVendor != null)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{methodName}: Vendor '{theVendor.InternalName}' was successfully summoned");
                                else ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summoning failed");
                            }
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{methodName}: Vendor '{theVendor.InternalName}' already summoned");
                        return Traiding(theVendor);
                    case VendorType.VIPProfessionVendor:
                        theVendor = VIP.ProfessionVendorEntity;
                        if (theVendor is null || !theVendor.IsValid)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summon 'VIPProfessionVendor'");
                            VIP.SummonProfessionVendor();
                            Thread.Sleep(3000);
                            theVendor = VIP.ProfessionVendorEntity;
                            if (extendedDebugInfo)
                            {
                                if (theVendor != null)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{methodName}: Vendor '{theVendor.InternalName}' was successfully summoned");
                                else ETLogger.WriteLine(LogType.Debug, $"{methodName}: Summoning failed");
                            }
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{methodName}: Vendor '{theVendor.InternalName}' already summoned");
                        return Traiding(theVendor);
                    case VendorType.RemoteVendor:
                        // TODO для вызова RemoteVendor, которые не видные в окне выбора, нужно реализовать торговлю как в QuesterAssistant через Injection.cmdwrapper_contact_StartRemoteContact(this.RemoteContact);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Call 'RemoteVendor'");
                        RemoteContact remoteContact = player.InteractInfo.RemoteContacts.Find(ct => ct.ContactDef == _vendor.CostumeName);
                        if (extendedDebugInfo)
                        {
                            if (remoteContact != null && remoteContact.IsValid)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{methodName}: Vendor '{remoteContact.ContactDef}' was called successfully");
                            else ETLogger.WriteLine(LogType.Debug, $"{methodName}: Calling failed");
                        }
                        return RemoteTraiding(remoteContact);
                    case VendorType.Node:
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Search '{_vendor}'");

                        WorldInteractionNode targetNode = null;
                        foreach (TargetableNode targetableNode in player.InteractStatus.TargetableNodes)
                        {
                            var worldIntNode = targetableNode.WorldInteractionNode;
                            if (_vendor.IsMatch(worldIntNode))
                            {
                                targetNode = worldIntNode;
                                break;
                            }
                        }

                        if (targetNode != null)
                        {
                            if (extendedDebugInfo)
                            {
                                var pos = targetNode.Location;
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{methodName}: Vendor-Node <{pos.X:N1}, {pos.Y:N1}, {pos.Z:N1}>  was successfully found at the Distance = {pos.Distance3DFromPlayer:N1}");
                            }
                            return Traiding(targetNode);
                        }
                        else
                        {
                            if (tryNum < _tries)
                            {
                                if (extendedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{methodName}: Vendor-Node not found. Approach Position to retry");

                                Approach.PostionByDistance(_vendor.Position, 5);

                                Thread.Sleep(1000);
                                tryNum++;
                                return ActionResult.Running;
                            }
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{methodName}: Vendor-Node not found. Tries are exhausted.");
                        }
                        break;
                    default:
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Search '{_vendor.CostumeName}'");
                        // ищем ближайший "контакт" совпадающий с Vendor
                        double dist = double.MaxValue;
                        ContactInfo contactInfo = null;
                        foreach (var cntInfo in player.InteractInfo.NearbyContacts)
                        {
                            double curDist = cntInfo.Entity.Location.Distance3DFromPlayer;
                            if (curDist < dist && _vendor.IsMatch(cntInfo.Entity))
                            {
                                contactInfo = cntInfo;
                                dist = curDist;
                            }
                        }
                        if (contactInfo != null && contactInfo.IsValid)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{methodName}: Vendor '{contactInfo.Entity.Name}[{contactInfo.Entity.InternalName}]' was successfully found at the Distance = {dist}");
                            return Traiding(contactInfo.Entity);
                        }

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Search failed");

                        break;
                }
            }

            return ActionResult.Fail;
        }

        /// <summary>
        /// Торговая сесссия с удаленным вендором
        /// </summary>
        /// <param name="remoteContact"></param>
        /// <returns></returns>
        private ActionResult RemoteTraiding(RemoteContact remoteContact)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;
            if (remoteContact != null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

                remoteContact.Start();
                Interact.WaitForInteraction();

                ScreenType screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                if (contactDialog.IsValid && (screenType == ScreenType.List || screenType == ScreenType.Store || screenType == ScreenType.StoreCollection))
                {
                    // взаимодействие с вендором для открытия окна магазина
                    if (_vendorMenus.Count > 0)
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
                        {   // Открыто диалоговое окно продавца
                            Interact.DoDialog(_vendorMenus);
                        }
                        else if (Check_ReadyToTraid(screenType))
                        {   // Открыто витрина магазина (список товаров)
                            // необходимо переключиться на нужную вкладку
                            string key = _vendorMenus.Last();
                            if (contactDialog.HasOptionByKey(key))
                                contactDialog.SelectOptionByKey(key);
                        }
                        if (_closeContactDialog)
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
                        if (_closeContactDialog)
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();

                        return result;
                    }
                }
            }
            return ActionResult.Running;
        }

        /// <summary>
        /// Торговая сессия с вендором-NPC <param name="vendorEntity"/>
        /// </summary>
        private ActionResult Traiding(Entity vendorEntity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo 
                              ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}" 
                              : string.Empty;
            if (vendorEntity != null && vendorEntity.IsValid)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

                // взаимодействие с вендором для открытия окна магазина
                if (ApproachAndInteractVendor(vendorEntity))
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
                        if (_closeContactDialog)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: CloseContactDialog");

                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                        }
                        return result;
                    }
                }
            }
            return ActionResult.Running;
        }

        /// <summary>
        /// Торговая сессия с вендором <param name="vendorNode"/>
        /// </summary>
        private ActionResult Traiding(WorldInteractionNode vendorNode)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

            if (vendorNode != null && vendorNode.IsValid)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

                // взаимодействие с вендором для открытия окна магазина
                if (ApproachAndInteractNode(vendorNode))
                {
#if false
                    // ApproachAndInteractNode гарантирует, что торговец может совершать сделки
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
                        if (_closeContactDialog)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: CloseContactDialog");

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
        private bool ApproachAndInteractVendor(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

            // Проверяем соответствие 
            if (_vendor.IsMatch(entity))
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: VendorEntity is ok");

                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                bool ready;
                if (contactDialog.IsValid)
                {
                    ScreenType screenType = contactDialog.ScreenType;

                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: ContactDialog is valid. ScreenType = {screenType}");

                    // Открыто диалоговое окно
                    if (screenType == ScreenType.List || screenType == ScreenType.Buttons)
                    {
                        // Открыто диалоговое окно продавца
                        if (_vendorMenus.Count > 0)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Call Interact.DoDialog(..)");
                            Interact.DoDialog(_vendorMenus);
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                        return ready;
                    }

                    if (screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)
                    // Открыто витрина магазина (список товаров)
                    // необходимо переключиться на нужную вкладку
                    {
                        if (_vendorMenus.Count > 0)
                        {
                            string key = _vendorMenus.Last();
                            if (contactDialog.HasOptionByKey(key))
                            {
                                if (extendedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Select Shop-Tab");

                                contactDialog.SelectOptionByKey(key);
                            }
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                        return ready;
                    }
                    if (_closeContactDialog)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: CloseContactDialog");
                        contactDialog.Close();
                    }
                }
                else if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: ContactDialog is invalid");

                double dist = entity.Location.Distance3DFromPlayer;
                if (!contactDialog.IsValid || dist > 10)
                {
                    // Расстояние до продавца больше 10
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: Distance to the Vendor = {dist}. Call Interact.VendorWithDialogs(..)");

                    bool interactResult = Interact.VendorWithDialogs(entity, _vendorMenus);

                    ready = Check_ReadyToTraid(EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType);
                    if (extendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: InteractionResult = {interactResult}, ReadyToTraid = {ready}");
                    }
                    return interactResult && ready;
                }
                ready = Check_ReadyToTraid(contactDialog.ScreenType);
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                return ready;
            }

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: VendorEntity checks failed");

            return false;
        }

        private bool ApproachAndInteractNode(WorldInteractionNode node)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

            if (_vendor.IsMatch(node))
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Vendor-Node is ok");

                ContactDialog contactDialog = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog;
                bool ready;
                if (contactDialog.IsValid)
                {
                    ScreenType screenType = contactDialog.ScreenType;

                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: ContactDialog is valid. ScreenType = {screenType}");

                    // Открыто диалоговое окно
                    if (screenType == ScreenType.List || screenType == ScreenType.Buttons)
                    {
                        // Открыто диалоговое окно продавца
                        if (_vendorMenus.Count > 0)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Call Interact.DoDialog(..)");
                            Interact.DoDialog(_vendorMenus);
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                        return ready;
                    }

                    if (screenType == ScreenType.Store || screenType == ScreenType.StoreCollection)
                    // Открыто витрина магазина (список товаров)
                    // необходимо переключиться на нужную вкладку
                    {
                        if (_vendorMenus.Count > 0)
                        {
                            string key = _vendorMenus.Last();
                            if (contactDialog.HasOptionByKey(key))
                            {
                                if (extendedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Select Shop-Tab");

                                contactDialog.SelectOptionByKey(key);
                            }
                        }
                        ready = Check_ReadyToTraid(contactDialog.ScreenType);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                        return ready;
                    }
                    if (_closeContactDialog)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: CloseContactDialog");
                        contactDialog.Close();
                    }
                }
                else if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: ContactDialog is invalid");

                double dist = node.Location.Distance3DFromPlayer;
                if (!contactDialog.IsValid || dist > 10)
                {
                    // Расстояние до продавца больше 10
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: Distance to the Vendor-Node = {dist}. Interact with Node");

                    var dynaNode = new Interact.DynaNode(node.Key);
                    Approach.NodeForInteraction(dynaNode);
                    bool interactResult = dynaNode.Node.WorldInteractionNode.Interact();
                    Interact.WaitForInteraction();

                    if (_vendorMenus.Count > 0)
                        Interact.DoDialog(_vendorMenus);

                    Thread.Sleep(500);

                    var screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                    ready = Check_ReadyToTraid(screenType);
                    if (extendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: InteractionResult = {interactResult}, ReadyToTraid = {ready}ScreenType = {screenType}");
                    }
                    return interactResult && ready;
                }
                ready = Check_ReadyToTraid(contactDialog.ScreenType);
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: ReadyToTraid = {ready}");

                return ready;
            }

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Vendor-Node checks failed");

            return false;
        }

        /// <summary>
        /// Покупка предметов
        /// </summary>
        /// <returns></returns>
        private ActionResult BuyItems()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");

            if (!_checkFreeBags || Check_FreeSlots(_buyBags))
            {
                if (_useGeneralSettingsToBuy)
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Processing GeneralSettingsToBuy");

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
                    foreach (ItemFilterEntryExt item2buy in _buyOptions)
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
                if (_buyOptions.Count > 0)
                {
                    if (extendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug,
                            $"{methodName}: Begins the processing of {nameof(BuyOptions)}");
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Indexing selected Bags");
                    }
                    // Анализируем содержимое сумок
                    IndexedBags indexedBags = new IndexedBags(_buyOptions, _buyBags);

                    foreach (var filterEntry in indexedBags.Filters)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Processing FilterEntry: {filterEntry}");

                        var slotCache = indexedBags[filterEntry];
                        List<ItemDef> boughtItems = null;
                        BuyItemResult buyItemResult = slotCache is null
                            ? BuyAnItem(filterEntry, ref boughtItems)
                            : BuyAnItem(filterEntry, slotCache, ref boughtItems);

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: Result: {buyItemResult}");
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
                                            // Здесь не учитывается возможность приобретения и экипировки нескольких вещей (например, колец)

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
                }
#endif

                return ActionResult.Completed;
            }

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Bags is full. Skip");

            return ActionResult.Skip;
        }

        /// <summary>
        /// Обработка одной позиции списка покупок
        /// </summary>
        /// <param name="item2buy"></param>
        /// <param name="boughtItems"></param>
        /// <returns></returns>
        private BuyItemResult BuyAnItem(ItemFilterEntryExt item2buy, ref List<ItemDef> boughtItems)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

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
                                Timeout timeout = new Timeout(_timer > 0 ? (int)_timer : int.MaxValue);

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

                                        if (mayBuyInBulk > toBuyNum)
                                        {
                                            // Единовременная покупка нужного количества предметов
                                            string str = $"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                            if (extendedDebugInfo)
                                                ETLogger.WriteLine(LogType.Debug, string.Concat(methodName, str));
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
                                            if (extendedDebugInfo)
                                                ETLogger.WriteLine(LogType.Debug, str);

                                            uint totalPurchasedNum;
                                            for (totalPurchasedNum = 0; totalPurchasedNum <= toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (timeout.IsTimedOut)
                                                {
                                                    str = $"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...";
                                                    Logger.WriteLine(str);
                                                    if (extendedDebugInfo)
                                                        ETLogger.WriteLine(LogType.Debug, string.Concat(methodName, str));
                                                    break;
                                                }
                                                if (_checkFreeBags && !Check_FreeSlots(_buyBags))
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    Logger.WriteLine(str);
                                                    if (extendedDebugInfo)
                                                        ETLogger.WriteLine(LogType.Debug, string.Concat(methodName, str));
                                                    return BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                    }
                                }
                                else
                                {
                                    if (extendedDebugInfo)
                                        ETLogger.WriteLine(LogType.Debug, "Bags are full, skip ...");

                                    Logger.WriteLine("Bags are full, skip ...");
                                    return BuyItemResult.FullBag;
                                }

                                if (succeeded)
                                {
                                    result = BuyItemResult.Succeeded;
                                    boughtItems.Add(storeItemInfo.Item.ItemDef);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private BuyItemResult BuyAnItem(ItemFilterEntryExt filterEntry, SlotCache slotCache, ref List<ItemDef> boughtItems)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string methodName = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod().Name}"
                : string.Empty;

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
                                Timeout timeout = new Timeout(_timer > 0 ? (int)_timer : int.MaxValue);

                                if (!_checkFreeBags || Check_FreeSlots(_buyBags))
                                {
                                    //Вычисляем количество предметов, которые необходимо докупить
                                    uint toBuyNum = storeItemInfo.NumberOfItem2Buy(slotCache, filterEntry);
                                    if (toBuyNum > 0)
                                    {
                                        uint mayBuyInBulk = storeItemInfo.Item.ItemDef.MayBuyInBulk;

                                        if (mayBuyInBulk > toBuyNum)
                                        {
                                            // Покупка предмета в необходимом количество за один раз
                                            string str = $"Buy {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                            if (extendedDebugInfo)
                                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: {str}");
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
                                            if (extendedDebugInfo)
                                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: {str}");
                                            Logger.WriteLine(str);

                                            uint totalPurchasedNum = 0;
                                            for (totalPurchasedNum = 0; totalPurchasedNum <= toBuyNum; totalPurchasedNum++)
                                            {
                                                storeItemInfo.BuyItem(1);
                                                Thread.Sleep(250);
                                                if (timeout.IsTimedOut)
                                                {
                                                    str = $"Buying time is out. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] was bought ...";
                                                    if (extendedDebugInfo)
                                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: {str}");
                                                    Logger.WriteLine(str);
                                                    break;
                                                }
                                                if (storeItemInfo.CanBuyError != 0u)
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Buying is impossible now. Error code '{storeItemInfo.CanBuyError}'. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    if (extendedDebugInfo)
                                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: {str}");
                                                    Logger.WriteLine(str);

                                                    return BuyItemResult.NotEnoughCurrency;
                                                }
                                                if (_checkFreeBags && !Check_FreeSlots(_buyBags))
                                                {
                                                    boughtItems.Add(storeItemInfo.Item.ItemDef);

                                                    str = $"Bags are full. Bought only {totalPurchasedNum} of {toBuyNum} {storeItemInfo.Item.DisplayName}[{storeItemInfo.Item.ItemDef.InternalName}] ...";
                                                    if (extendedDebugInfo)
                                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: {str}");
                                                    Logger.WriteLine(str);
                                                    return BuyItemResult.FullBag;
                                                }
                                            }

                                            succeeded = totalPurchasedNum >= toBuyNum;
                                        }
                                    }
                                }
                                else
                                {
                                    if (extendedDebugInfo)
                                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Bags are full, skip ...");
                                    Logger.WriteLine("Bags are full, skip ...");
                                    return BuyItemResult.FullBag;
                                }

                                if (succeeded)
                                {
                                    result = BuyItemResult.Succeeded;
                                    boughtItems.Add(storeItemInfo.Item.ItemDef);
                                }
                            }
                        }
                    }
                }
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
                if (_useGeneralSettingsToSell && slots2sellCache == null && Astral.Controllers.Settings.Get.SellFilter.Entries.Count > 0)
                {
                    // необходимо использовать глобальный список продажи, а slots2sellCache не сформирован
#if false
                    Interact.SellItems(); 
#else
                    // Производим поиск и продажу предметов, подходящих под глобальный фильтр продажи
                    if (_sellBags.GetItems(Astral.Controllers.Settings.Get.SellFilter, out List<InventorySlot> generalSellSlotsCache, true))
                        foreach (InventorySlot slot in generalSellSlotsCache)
                            if (Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(
                                    $"Sell : {slot.Item.DisplayName}[{slot.Item.ItemDef.InternalName}] x {slot.Item.Count}");
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
#endif
                }
                if (_sellOptions.Entries.Count > 0)
                {
                    // Задан локальный фильтр продажи
                    if (slots2sellCache?.Count > 0)
                    {
                        // lots2sellCache сформирован, дополнительный происк производить не нужно
                        foreach (InventorySlot slot in slots2sellCache)
                            if (Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(
                                    $"Sell : {slot.Item.DisplayName}[{slot.Item.ItemDef.InternalName}] x {slot.Item.Count}");
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
                    }
                    else if (_sellBags.GetItems(_sellOptions, out List<InventorySlot> slots2sell, true))
                        foreach (InventorySlot slot in slots2sell)
                            if (Inventory.CanSell(slot.Item))
                            {
                                Logger.WriteLine(
                                    $"Sell : {slot.Item.DisplayName}[{slot.Item.ItemDef.InternalName}] x {slot.Item.Count}");
                                slot.StoreSellItem();
                                Thread.Sleep(250);
                            }
                            else Logger.WriteLine("Nothing to sell !");
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

        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugBuySellItems && logConf.Active;
            }
        }
    }
}
