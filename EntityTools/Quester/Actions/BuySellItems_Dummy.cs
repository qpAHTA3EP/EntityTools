using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.Classes.Map;
using Astral.Quester.Forms;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Tools.ItemFilter;
using EntityTools.Reflection;
using MyNW.Classes;
using System.Xml.Serialization;

namespace EntityTools.Quester.Actions
{
    /// <summary>
    /// Заглушка для <seealso cref="BuySellItemsExt"/>
    /// </summary>
    public class BuySellItemsExt : Astral.Quester.Classes.Action
    {
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
                _sellOptions = value;
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
                _useGeneralSettingsToSell = value;
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
                _sellBags = CopyHelper.CreateDeepCopy(value);
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
                if (!Equals(_buyBags, value))
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

        public override string ActionLabel => GetType().Name;

        public override bool NeedToRun => true;

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => false;

        protected override bool IntenalConditions => false;

        protected override Vector3 InternalDestination => new Vector3();

        protected override ActionValidity InternalValidity => new ActionValidity("BuySellItemsExt on maintenance");

        public override void GatherInfos() { }

        public override void InternalReset() { }

        public override void OnMapDraw(GraphicsNW graph) { }

        public override ActionResult Run() { return ActionResult.Completed; }
    }
}
