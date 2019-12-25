using Astral.Classes.ItemFilter;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EntityTools.Tools.Entities
{
    interface IEntitySearchOptions
    {
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        string EntityID
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
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        ItemFilterStringType EntityIdType
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

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        EntityNameType EntityNameType
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

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Optional")]
        bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        bool HealthCheck { get; set; } = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within 'Distance' of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Optional")]
        bool HoldTargetEntity { get; set; } = true;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        float ReactionRange { get; set; } = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    customRegions = CustomRegionTools.GetCustomRegions(value);
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;
                }
            }
        }

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        int SearchTimeInterval { get; set; } = 100;

        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Interruptions")]
        float Distance { get; set; } = 30;

        [Description("Enable 'IgnoreCombat' profile value while playing action")]
        [Category("Interruptions")]
        bool IgnoreCombat { get; set; } = true;

        [XmlIgnore]
        EntityComparerToPattern Comparer { get; private set; } = null;

        bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
        }

        [NonSerialized]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;
        [NonSerialized]
        internal Entity target = new Entity(IntPtr.Zero);

        [NonSerialized]
        private List<string> customRegionNames = new List<string>();
        [NonSerialized]
        private List<CustomRegion> customRegions = new List<CustomRegion>();
    }
}
