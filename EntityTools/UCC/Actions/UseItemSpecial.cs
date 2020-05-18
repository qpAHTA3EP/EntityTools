using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Extensions;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using EntityTools.Core.Proxies;
using EntityTools.Core.Interfaces;
using EntityTools.Tools.BuySellItems;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class UseItemSpecial : UCCAction
    {
#if DEVELOPER && CheckedListBoxCommonSelector_InvBagIDs
        internal class InventoryBagsCheckedListBoxEditor : CheckedListBoxCommonEditor<InvBagIDs> { }
#endif

        #region Взаимодействие с EntityToolsCore
#if CORE_INTERFACES
        [NonSerialized]
        internal IUCCActionEngine Engine;
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        public UseItemSpecial()
        {
#if CORE_INTERFACES
            Engine = new UCCActionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 18000;
        }
        #endregion


        #region Опции команды

#if DEVELOPER
        [Editor(typeof(ItemIdEditor), typeof(UITypeEditor))]
        [Category("Item")]
#else
        [Browsable(false)]
#endif
        public string ItemId
        {
            get => _itemId;
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemId)));
                }
            }
        }
        internal string _itemId = string.Empty;

#if DEVELOPER
        [Description("Type of and ItemId:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Item")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType ItemIdType
        {
            get => _itemIdType; set
            {
                if (_itemIdType != value)
                {
                    _itemIdType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemIdType)));
                }
            }
        }
        internal ItemFilterStringType _itemIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Category("Item")]
#else
        [Browsable(false)]
#endif
        public bool CheckItemCooldown
        {
            get => _checkItemCooldown; set
            {
                if (_checkItemCooldown != value)
                {
                    _checkItemCooldown = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckItemCooldown)));
                }
            }
        }
        internal bool _checkItemCooldown = false;

#if DEVELOPER
        [Category("Item")]
        [Description("Identificator of the bag where Item would be searched\n" +
            "When selected value is 'None' then item is searched in the all inventories")]
        [Browsable(false)]
#else
        [Browsable(false)]
#endif
        public InvBagIDs BagId
        {
#if CheckedListBoxCommonSelector_InvBagIDs
            get
            {
                if (Bags != null && Bags.Items.Count > 0)
                    return Bags.Items[0];
                else return InvBagIDs.None;
            }
            set
            {
                if (Bags == null)
                {
                    Bags = new CheckedListBoxCommonSelector<InvBagIDs>();
                    Bags.Items.Add(value);
                }
                else
                {
                    if (!Bags.Items.Contains(value))
                        Bags.Items.Add(value);
                }
            } 
#else
            get
            {
                if (Bags != null)
                    return Bags.FirstOrDefault();
                return InvBagIDs.None;
            }
            set
            {
                if (Bags == null)
                {
                    Bags = new BagsList();
                    Bags.Add(value);
                }
                else
                {
                    Bags.Add(value);
                }
            }
#endif
        }

#if CheckedListBoxCommonSelector_InvBagIDs
#if DEVELOPER
        [Category("Item")]
        [Description("Identificator of the bags where Item would be searched\n")]
        [Editor(typeof(InventoryBagsCheckedListBoxEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public CheckedListBoxCommonSelector<InvBagIDs> Bags
        {
            get => _bags; set
            {
                if (_bags != value)
                {
                    _bags = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bags)));
                }
            }
        }
        internal CheckedListBoxCommonSelector<InvBagIDs> _bags = new CheckedListBoxCommonSelector<InvBagIDs>();
#else 
#if DEVELOPER
        [Category("Item")]
        [Description("Identificator of the bags where Item would be searched\n")]
        [Editor(typeof(BagsListEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public BagsList Bags
        {
            get => _bags; set
            {
                if (_bags != value)
                {
                    _bags = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bags)));
                }
            }
        }
        internal BagsList _bags = new BagsList();
#endif

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }
        #endregion
        #endregion


        public override bool NeedToRun => Engine.NeedToRun;

        public override bool Run() => Engine.Run();

        public override string ToString() => Engine.Label();

        public override UCCAction Clone()
        {
            return base.BaseClone(new UseItemSpecial
            {
                _itemId = this._itemId,
                _itemIdType = this._itemIdType,
                _checkItemCooldown = this._checkItemCooldown,
                _bags = this._bags.Clone()
            });
        }
    }
}
