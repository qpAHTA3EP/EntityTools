using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
#if true
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
#else
using EntityTools.Tools.ItemFilter;
#endif

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class UseItemSpecial : UCCAction
    {
#if DEVELOPER && CheckedListBoxCommonSelector_InvBagIDs
        internal class InventoryBagsCheckedListBoxEditor : CheckedListBoxCommonEditor<InvBagIDs> { }
#endif


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
        internal bool _checkItemCooldown;


#if disabled_20200527_1854
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
#if disabled_20200527_1847
                if (_bags != null)
                    return _bags.FirstOrDefault();

#endif
                return InvBagIDs.None;
            }
            set
            {
                if (_bags == null)
                {
                    _bags = new BagsList();
                    _bags.Add(value);
                }
                else
                {
                    _bags.Add(value);
                }
            }
#endif
        } 
#endif

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
        internal BagsList _bags = BagsList.GetPlayerBagsAndPotions();
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

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccActionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        public UseItemSpecial()
        {
            Engine = new UccActionProxy(this);

            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 18000;
        }
        private IUccActionEngine MakeProxie()
        {
            return new UccActionProxy(this);
        }
        #endregion

        #region Интерфейс команды
        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).NeedToRun;
        public override bool Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Run();
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).UnitRef;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Label();
        #endregion

        public override UCCAction Clone()
        {
            return BaseClone(new UseItemSpecial
            {
                _itemId = _itemId,
                _itemIdType = _itemIdType,
                _checkItemCooldown = _checkItemCooldown,
                _bags = _bags.Clone()
            });
        }
    }
}
