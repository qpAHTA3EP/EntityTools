using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Entities;
using MyNW.Classes;

namespace EntityTools.Quester.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>
    [Serializable]
    public class EntityCount : Condition, INotifyPropertyChanged, IEntityDescriptor
    {
        #region Опции команды
#if DEVELOPER
        [Description("ID of the Entity for the search")]
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
                    _key = null;
                    _label = string.Empty;
                    entities?.Clear();
                    NotifyPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

#if DEVELOPER
        [Description("The switcher of the Entity filed which compared to the property EntityID")]
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
                    _key = null;
                    _label = string.Empty;
                    entities?.Clear();
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

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
                    _key = null;
                    _label = string.Empty;
                    entities?.Clear();
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public EntitySetType EntitySetType
        {
            get => _entitySetType;
            set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntitySetType _entitySetType = EntitySetType.Complete;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";
#endif

#if DEVELOPER
        [Description("Check Entity's Region:\n" +
            "True: Count Entity if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting Entities")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck;
            set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    _specialCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _regionCheck;

#if DEVELOPER
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool HealthCheck
        {
            get => _healthCheck; 
            set
            {
                if (_healthCheck == value)
                {
                    _healthCheck = value;
                    _specialCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

#if DEVELOPER
        [Description("Threshold value to compare by 'Sign' with the number of the Entities")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public uint Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _value;

#if DEVELOPER
        [Description("The comparison type for the number of the Entities with 'Value'")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign;
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    countChecker = Initialize_CountChecker;
                    NotifyPropertyChanged();
                }
            }
        }
        private Relation _sign = Relation.Superior;

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionRange
        {
            get => _reactionRange;
            set
            {
                if (Math.Abs(_reactionRange - value) > 0.1)
                {
                    _reactionRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionRange;

#if DEVELOPER
        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionZRange
        {
            get => _reactionZRange; set
            {
                if (Math.Abs(_reactionZRange - value) > 0.1)
                {
                    _reactionZRange = value;
                    _specialCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

#if DEVELOPER
        [Description("The list of the CustomRegions where Entities is counted")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    _specialCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();


#if DEVELOPER
        [Category("Location")]
        [Description("The position of the counted entities relative to CustomRegions")]
#else
        [Browsable(false)]
#endif
        [XmlElement("Tested")]
        public Presence CustomRegionCheck
        {
            get => _customRegionCheck;
            set
            {
                if (_customRegionCheck != value)
                {
                    _customRegionCheck = value;
                    _specialCheck = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private Presence _customRegionCheck = Presence.Equal;
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        public EntityCount()
        {
            _idStr = string.Concat(GetType().Name, '[', GetHashCode().ToString("X2"), ']');
            countChecker = Initialize_CountChecker;
        }




        private LinkedList<Entity> entities;
        private string _label = string.Empty;

        /// <summary>
        /// Префикс, идентифицирующий принадлежность отладочной информации, выводимой в Лог
        /// </summary>
        private string _idStr = string.Empty;
        


        
        public override bool IsValid
        {
            get
            {
                bool result = false;
                bool debugInfoEnabled = ExtendedDebugInfo;
                string currentMethodName = debugInfoEnabled ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(IsValid)) : string.Empty;

                if (debugInfoEnabled)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Begins");
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (!string.IsNullOrEmpty(EntityID) || EntityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    int entCount = entities?.Count ?? 0;

                    if (debugInfoEnabled)
                    {
                        string debugMsg = entCount > 0 
                                        ? $"{currentMethodName}: Search Entities (irrespectively CustomRegion). Total found: {entCount}" 
                                        : $"{currentMethodName}: Search Entities (irrespectively CustomRegion). Nothing found";

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }

                    result = countChecker(entCount);

                    if (debugInfoEnabled)
                    {
                        string debugMsg = $"{currentMethodName}: Result={result} ({entCount} entities matched)";

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                }
                return result;
            }
        }

        public override void Reset() => entities?.Clear();

        public override string TestInfos
        {
            get
            {
                var enttId = EntityID;
                if (!string.IsNullOrEmpty(enttId) || EntityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null && entities.Count > 0)
                    {
                        var crList = CustomRegionNames;
                        if (crList.Count > 0)
                        {
                            strBldr.AppendLine();
                            foreach (Entity entity in entities)
                            {
                                StringBuilder strBldr2 = new StringBuilder();
                                bool match = false;

                                foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
                                {
                                    if (entity.Within(customRegion))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{customRegion.Name}]");
                                    }
                                }

                                if (CustomRegionCheck == Presence.Equal && match)
                                    entCount++;
                                if (CustomRegionCheck == Presence.NotEquel && !match)
                                    entCount++;

                                switch (EntityNameType)
                                {
                                    case EntityNameType.InternalName:
                                        strBldr.Append($"\t[{entity.InternalName}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.NameUntranslated:
                                        strBldr.Append($"\t[{entity.NameUntranslated}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.Empty:
                                        strBldr.Append($"\t[{entity.Name}] is in CustomRegions: ");
                                        break;
                                }
                                strBldr.Append(strBldr2).AppendLine();
                            }

                            if (CustomRegionCheck == Presence.Equal)
                                strBldr.Insert(0, $"Total {entCount} Entities [{enttId}] are detected in CustomRegions({crList}):");
                            if (CustomRegionCheck == Presence.NotEquel)
                                strBldr.Insert(0, $"Total {entCount} Entities [{enttId}] are detected out of CustomRegions({crList}):");
                        }
                        else strBldr.AppendLine($"Total {entities.Count} Entities [{enttId}] are detected");
                    }
                    else strBldr.AppendLine($"No Entity [{enttId}] was found.");

                    if (ExtendedDebugInfo)
                    {
                        string debugMsg = string.Concat(_idStr, '.', nameof(TestInfos), ':', strBldr.ToString());

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    return strBldr.ToString();
                }
                return $"Property '{nameof(enttId)}' does not set !";
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var entId = EntityID;
                _label = string.IsNullOrEmpty(entId)
                       ? $"{GetType().Name} {Sign} {Value}"
                       : $"{GetType().Name}({entId}) {Sign} {Value}";
            }
            return _label;
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="EntityCount.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                {
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                            ReactionZRange, 
                                                            ReactionZRange,
                                                            RegionCheck,
                                                            CustomRegionNames,
                                                            CustomRegionCheck == Presence.NotEquel);

                }
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;

        /// <summary>
        /// Предикат, сравнивающий количество <seealso cref="Entity"/>, удовлетворяющих уcловиям <seealso cref="EntityCount"/>
        /// c референтным значеним <seealso cref="Value"/>
        /// </summary>
        Predicate<int> countChecker;

        private bool Initialize_CountChecker(int count)
        {
            switch (Sign)
            {
                case Relation.Inferior:
                    countChecker = Inferior_Than_Value;
                    break;
                case Relation.Superior:
                    countChecker = Superior_Than_Value;
                    break;
                case Relation.Equal:
                    countChecker = Equal_To_Value;
                    break;
                case Relation.NotEqual:
                    countChecker = NotEqual_To_Value;
                    break;
            }
            return countChecker(count);
        }
        private bool Inferior_Than_Value(int count) => count < Value;

        private bool Superior_Than_Value(int count) => count > Value;

        private bool Equal_To_Value(int count) => count == Value;

        private bool NotEqual_To_Value(int count) => count != Value;
        #endregion

        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.QuesterConditions.DebugEntityCount && logConf.Active;
            }
        }
    }
}
