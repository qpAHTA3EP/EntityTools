using Astral.Classes.ItemFilter;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;
using System.ComponentModel;
using System.Drawing.Design;
using MyNW.Classes;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EntityTools.Tools.Entities
{
    /// <summary>
    /// Класс, задающий комплексный идентификатор <seealso cref="Entity"/>
    /// </summary>
    public class EntityDef
    {
#if DEVELOPER
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
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
                    checkEntity = initialize_checkEntity;
                }
            }
        }
        internal string _entityId = string.Empty;

#if DEVELOPER
        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
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
                    checkEntity = initialize_checkEntity;
                }
            }
        }
        internal ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("The switcher of the Entity filed which compared to the property EntityID")]
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
                    checkEntity = initialize_checkEntity;
                }
            }
        }
        internal EntityNameType _entityNameType = EntityNameType.InternalName;

        #region IsMatch
        /// <summary>
        /// Проверка 
        /// </summary>
        public bool IsMatch(Entity e)
        {
            if (e is null) return false;
            return checkEntity(e); 
        }

        Predicate<Entity> checkEntity = null;
        string pattern;

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="checkEntity"/>,
        /// использующийся для проверки сущности <paramref name="e"/> на соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        private bool initialize_checkEntity(Entity e)//string _entityId, ItemFilterStringType _entityIdType = ItemFilterStringType.Simple, EntityNameType _entityNameType = EntityNameType.NameUntranslated)
        {
            bool extendedDebugInfo = EntityTools.Config.Logger.DebugEntityTools;
            string currentMethodName = extendedDebugInfo ? string.Concat(GetType().Name, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            Predicate<Entity> predicate;
            pattern = string.Empty;

            if (_entityIdType == ItemFilterStringType.Simple)
            {
                if (_entityId == "*" || _entityId == "**")
                    predicate = CompareSimpleAny;
                else
                {
                    SimplePatternPos pos = _entityId.GetSimplePatternPosition(out string pattern);

                    if (_entityNameType == EntityNameType.InternalName)
                        switch (pos)
                        {
                            case SimplePatternPos.Full:
                                predicate = CompareInternal2SimpleFull;
                                break;
                            case SimplePatternPos.Start:
                                predicate = CompareInternal2SimpleStart;
                                break;
                            case SimplePatternPos.Middle:
                                predicate = CompareInternal2SimpleMiddle;
                                break;
                            case SimplePatternPos.End:
                                predicate = CompareInternal2SimpleEnd;
                                break;
                            default:
                                predicate = CompareFalse;
                                break;
                        }
                    else if (_entityNameType == EntityNameType.NameUntranslated)
                        switch (pos)
                        {
                            case SimplePatternPos.Full:
                                predicate = CompareUntranslated2SimpleFull;
                                break;
                            case SimplePatternPos.Start:
                                predicate = CompareUntranslated2SimpleStart;
                                break;
                            case SimplePatternPos.Middle:
                                predicate = CompareUntranslated2SimpleMiddle;
                                break;
                            case SimplePatternPos.End:
                                predicate = CompareUntranslated2SimpleEnd;
                                break;
                            default:
                                predicate = CompareFalse;
                                break;
                        }
                    else predicate = CompareEmpty;
                }
            }
            else
            {
                if (_entityNameType == EntityNameType.InternalName)
                    predicate = CompareInternal2Regex;
                else if (_entityNameType == EntityNameType.NameUntranslated)
                    predicate = CompareUntranslated2Regex;
                else predicate = CompareEmpty;
            }

            if (predicate != null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Initialize '" + nameof(checkEntity) + '\''));
                checkEntity = predicate;
                return checkEntity(e);
            }
            else if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Error, string.Concat(currentMethodName, ": Fail to initialize '" + nameof(checkEntity) + '\''));
            return false;
        }

        private bool CompareInternal2SimpleFull(Entity e) => e.InternalName.Equals(pattern);        
        private bool CompareUntranslated2SimpleFull(Entity e) => e.NameUntranslated.Equals(pattern);        

        private bool CompareInternal2SimpleStart(Entity e) => e.InternalName.StartsWith(pattern);        
        private bool CompareUntranslated2SimpleStart(Entity e) => e.NameUntranslated.StartsWith(pattern);        

        private bool CompareInternal2SimpleMiddle(Entity e) => e.InternalName.Contains(pattern);        
        private bool CompareUntranslated2SimpleMiddle(Entity e) => e.NameUntranslated.Contains(pattern);

        private bool CompareInternal2SimpleEnd(Entity e) => e.InternalName.EndsWith(pattern);        
        private bool CompareUntranslated2SimpleEnd(Entity e) => e.NameUntranslated.EndsWith(pattern);
        
        private bool CompareSimpleAny(Entity e) => true;

        private bool CompareInternal2Regex(Entity e) => Regex.IsMatch(e.InternalName, pattern);
        private bool CompareUntranslated2Regex(Entity e) => Regex.IsMatch(e.NameUntranslated, pattern);

        private bool CompareEmpty(Entity e) => string.IsNullOrEmpty(e.InternalName) && string.IsNullOrEmpty(e.NameUntranslated);
        
        private bool CompareFalse(Entity e) => false;
        #endregion
    }
}
