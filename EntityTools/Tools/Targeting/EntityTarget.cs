using Astral.Classes.ItemFilter;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Tools.Targeting
{
    /// <summary>
    /// Целеуказатель на <seealso cref="Entity"/>
    /// </summary>
    [Serializable]
    public class EntityTarget : TargetSelector, IEntityIdentifier
    {
        #region Опции команды
        [Description("The text identifier or part of it used to search for an Entity")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        [NotifyParentProperty(true)]
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    _label = string.Empty;
                    OnPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

        [Description("Type of and EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        [NotifyParentProperty(true)]
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    OnPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity property which compared to 'EntityID'")]
        [Category("Entity")]
        [NotifyParentProperty(true)]
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    OnPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName; 
        #endregion

        public override TargetSelector Clone()
        {
            return new EntityTarget
            {
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType
            };
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = string.IsNullOrEmpty(_entityId) 
                       ? "Target Entity" 
                       : $"Target Entity [{_entityId}]";
            }

            return _label;
        }
        private string _label;
    }
}
