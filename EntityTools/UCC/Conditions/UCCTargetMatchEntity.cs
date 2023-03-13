using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.Entities;
using EntityTools.UCC.Extensions;
using MyNW.Classes;
// ReSharper disable InconsistentNaming

namespace EntityTools.UCC.Conditions
{
    public class UCCTargetMatchEntity : UCCCondition, ICustomUCCCondition, INotifyPropertyChanged
    {
        #region Опции команды
        [Description("The ID of the entity that the Target of the ucc-action should match.\n" +
                     "Идентификатор Entity, с которой сопоставляется цель ucc-команды (Target).")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    _key = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    _key = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity filed which compared to the EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    _key = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.NameUntranslated;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";

        [Description("The expected result of the comparison of the Target and EntityID.\n" +
            "Ожидаемый результат сопоставления цели ucc-команды (Target) и заданного EntityID.")]
        [Category("Required")]
        public MatchType Match
        {
            get => _match; set
            {
                if (_match != value)
                {
                    _match = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private MatchType _match = MatchType.Match;

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




        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        public new ICustomUCCCondition Clone()
        {
            return new UCCTargetMatchEntity
            {
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType,
                _match = _match,
                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Tested = Tested,
                Value = Value
            };
        }

        


        #region Вспомогательные методы
        public new bool IsOK(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();
            bool match = EntityKey.IsMatch(target);
            if (Match == MatchType.Match)
                return match;
            return !match;
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            StringBuilder sb = new StringBuilder("Target ");
            if (target != null)
            {
                if (EntityNameType == EntityNameType.InternalName)
                    sb.Append('[').Append(target.InternalName).Append(']');
                else sb.Append('[').Append(target.NameUntranslated).Append(']');
                if (EntityKey.IsMatch(target))
                    sb.Append(" match");
                else sb.Append(" does not match");
                sb.Append(" EntityID [").Append(EntityID).Append(']');
            }
            else sb.Append("is NULL");

            return sb.ToString();
        }

        public override string ToString()
        {
            _label = string.IsNullOrEmpty(_label)
                   ? $"{GetType().Name} [{EntityID}]"
                   : GetType().Name;

            return _label;
        }
        private string _label = string.Empty;
        #endregion

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey => _key ?? (_key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType));
        private EntityCacheRecordKey _key;
        #endregion
    }
}
