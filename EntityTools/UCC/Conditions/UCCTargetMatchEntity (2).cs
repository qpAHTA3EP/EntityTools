using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using static Astral.Quester.Classes.Condition;
using Astral.Logic.UCC.Ressources;
using EntityTools.Enums;
using System;
using EntityCore.Entities;
using EntityTools.UCC.Extensions;
using System.Text;
using EntityCore.Enums;

namespace EntityTools.UCC.Conditions
{
    public class UCCTargetMatchEntity : UCCCondition, ICustomUCCCondition
    {
        [Description("The ID of the entity that the Target of the ucc-action should match.\n" +
            "Идентификатор Entity, с которой сопоставляется цель ucc-команды (Target).")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => entityId;
            set
            {
                if (entityId != value)
                {
                    entityId = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => entityIdType;
            set
            {
                if (entityIdType != value)
                {
                    entityIdType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("The switcher of the Entity filed which compared to the EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => entityNameType;
            set
            {
                if (entityNameType != value)
                {
                    entityNameType = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
        public string TestInfo { get; } = "Нажми '...' =>";

        [Description("The expected result of the comparison of the Target and EntityID.\n" +
            "Ожидаемый результат сопоставления цели ucc-команды (Target) и заданного EntityID.")]
        [Category("Required")]
        public MatchType Match { get; set; } = MatchType.Match;

        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction = null)
        {
            Entity target = refAction?.GetTarget();

            if (Comparer == null && !string.IsNullOrEmpty(entityId))
                Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);

            switch(Match)
            {
                case MatchType.Match:
                    return Validate(target);
                case MatchType.Mismatch:
                    return !Validate(target);
            }
            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            if (Comparer == null && !string.IsNullOrEmpty(entityId))
                Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);

            if (Comparer != null)//(!string.IsNullOrEmpty(EntityID))
            {
                StringBuilder sb = new StringBuilder("Target ");
                if (target != null)
                {
                    if (EntityNameType == EntityNameType.InternalName)
                        sb.Append('[').Append(target.InternalName).Append(']');
                    else sb.Append('[').Append(target.NameUntranslated).Append(']');
                    if (Validate(target))
                        sb.Append(" match");
                    else sb.Append(" does not match");
                    sb.Append("EntityID [").Append(entityId).Append(']');
                }
                else sb.Append("is NULL");

                return sb.ToString();
            }
            return "Condition options is invalid!";
        }
        #endregion

        [XmlIgnore]
        [Browsable(false)]
        internal EntityComparerToPattern Comparer { get; private set; } = null;

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
        }

        public override string ToString()
        {
            return $"TargetMatchEntity [{EntityID}]";
        }

        public UCCTargetMatchEntity()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
        }

        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;


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
    }
}
