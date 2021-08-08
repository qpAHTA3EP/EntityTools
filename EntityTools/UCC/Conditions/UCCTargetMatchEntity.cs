using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;

namespace EntityTools.UCC.Conditions
{
    public class UCCTargetMatchEntity : UCCCondition, ICustomUCCCondition
    {
        #region Опции команды
#if DEVELOPER
        [Description("The ID of the entity that the Target of the ucc-action should match.\n" +
                     "Идентификатор Entity, с которой сопоставляется цель ucc-команды (Target).")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityID)));
                }
            }
        }
        internal string _entityId = string.Empty;

#if DEVELOPER
        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityIdType)));
                }
            }
        }
        internal ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("The switcher of the Entity filed which compared to the EntityID")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        internal EntityNameType _entityNameType = EntityNameType.NameUntranslated;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
#else
        [Browsable(false)]
#endif
        public string TestInfo { get; } = "Нажми '...' =>";

#if DEVELOPER
        [Description("The expected result of the comparison of the Target and EntityID.\n" +
            "Ожидаемый результат сопоставления цели ucc-команды (Target) и заданного EntityID.")]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public MatchType Match
        {
            get => _match; set
            {
                if (_match != value)
                {
                    _match = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Match)));
                }
            }
        }
        internal MatchType _match = MatchType.Match;

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Sign Sign { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
        #endregion
        #endregion

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccConditionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        public UCCTargetMatchEntity()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;

            Engine = new UccConditionProxy(this);
        }
        private IUccConditionEngine MakeProxy()
        {
            return new UccConditionProxy(this);
        }
        #endregion

        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).IsOK(refAction);

        bool ICustomUCCCondition.Loсked { get => Locked; set => Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).TestInfos(refAction);
        #endregion

        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();

    }
}
