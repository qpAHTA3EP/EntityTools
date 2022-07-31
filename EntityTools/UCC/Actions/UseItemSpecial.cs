using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Tools.Inventory;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;


namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class UseItemSpecial : UCCAction
    {
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
                    NotifyPropertyChanged();
                }
            }
        }
        private string _itemId = string.Empty;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _itemIdType = ItemFilterStringType.Simple;


#if DEVELOPER
        [Category("Item")]
        [Description("Identificator of the bags where Item would be searched")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private BagsList _bags = BagsList.GetPlayerBagsAndPotions();

#if DEVELOPER
        [Category("Optional")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _checkItemCooldown;

#if DEVELOPER
        [Category("Optional")]
        [Description("Equip an item into the needed slot automatically if it can't be used from the bag")]
#else
        [Browsable(false)]
#endif
        public bool AutoEquip
        {
            get => _autoEquip; set
            {
                if (_autoEquip != value)
                {
                    _autoEquip = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _autoEquip;

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
        [XmlIgnore]
        [NonSerialized]
        internal IUccActionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                var arg = new PropertyChangedEventArgs(propertyName);
                PropertyChanged?.Invoke(this, arg);
            }
        }

        public UseItemSpecial()
        {
            Engine = new UccActionProxy(this);

            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Player;
            CoolDown = 18000;
        }
        private IUccActionEngine MakeProxy()
        {
            return new UccActionProxy(this);
        }
        #endregion

        #region Интерфейс команды
        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).NeedToRun;
        public override bool Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Run();
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).UnitRef;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();
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
