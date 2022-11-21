using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools.Extensions;
using MyNW.Classes;

namespace EntityTools.Tools.CustomRegions
{
    /// <summary>
    /// Коллекция из не повторяющихся <seealso cref="CustomRegion"/>, сочетание которые опередяет область на карте
    /// </summary>
    public class CustomRegionCollection : KeyedCollection<string, CustomRegionEntry>, IXmlSerializable
    {
        // TODO Добавить реализацию IPropertyChange
        public CustomRegionCollection()
        {
            within = initialize_withing;
        }
        public CustomRegionCollection(IEnumerable<CustomRegionEntry> collection, bool clone = true)
        {
            var predicate = construct_withing_predicate(collection, true, clone);
            within = predicate ?? initialize_withing;
        }
        public CustomRegionCollection(IEnumerable<CustomRegion> collection, InclusionType inclusion = InclusionType.Union)
        {
            var internInclusion = inclusion;
            var predicate = construct_withing_predicate(collection.Select(cr => new CustomRegionEntry(cr, internInclusion)), true);
            within = predicate ?? initialize_withing;
        }
        private long version;

        #region KeyedCollection
        protected override string GetKeyForItem(CustomRegionEntry customRegionEntry)
        {
            return customRegionEntry.Name;
        }
        protected override void ClearItems()
        {
            version++;

            if (Count > 0)
            {
                CustomRegionEntry[] crEntryArray = new CustomRegionEntry[Count];
                CopyTo(crEntryArray, 0);
                crEntryArray.ForEach(crEntry => crEntry.Unbind());
                base.ClearItems();
            }
            label = string.Empty;
            within = initialize_withing;
        }
        protected override void InsertItem(int index, CustomRegionEntry customRegionEntry)
        {
            if (customRegionEntry is null)
                return;

            version++;
            if (!Contains(customRegionEntry))
            {
                base.InsertItem(index, customRegionEntry);
                customRegionEntry.Bind(this);
            }
            label = string.Empty;
            within = initialize_withing;
        }
        protected override void SetItem(int index, CustomRegionEntry customRegionEntry)
        {
            if (customRegionEntry is null)
                return;

            var crEntry = Items[index];

            version++;
            if (crEntry.Collection != null)
                crEntry.Unbind();

            base.SetItem(index, customRegionEntry);
            customRegionEntry.Bind(this);
            within = initialize_withing;
        }
        protected override void RemoveItem(int index)
        {
            var crEntry = Items[index];

            version++;
            if (crEntry.Collection != null)
                crEntry.Unbind();

            base.RemoveItem(index);
            label = string.Empty;
            within = initialize_withing;
        }
        #endregion

        [Browsable(false), XmlIgnore]
        public QuesterProfileProxy DesignContext
        {
            get => _context ?? AstralAccessors.Quester.Core.ActiveProfileProxy;
            set => _context = value;
        }
        private QuesterProfileProxy _context;

        #region Within
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в области, заданной, <seealso cref="CustomRegionCollection"/>
        /// </summary>
        public bool Within(Entity entity)
        {
            if (entity is null || !entity.IsValid)
                return false;
            if (version != versionWithin)
                within = initialize_withing;
            return within(entity.Location);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> за пределами области, заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        public bool Within(Vector3 position)
        {
            if (version != versionWithin)
                within = initialize_withing;
            return within(position);
        }
        public bool Outside(Entity entity)
        {
            if (entity is null || !entity.IsValid)
                return false;
            if (version != versionWithin)
                within = initialize_withing;
            return !within(entity.Location);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> за пределами области, заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        public bool Outside(Vector3 position)
        {
            if (version != versionWithin)
                within = initialize_withing;
            return !within(position);
        }

        /// <summary>
        /// Инициализация функтора проверки <paramref name="position"/> на предмет находения в области, заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        protected bool initialize_withing(Vector3 position)
        {
            if (version != versionWithin)
            {
                var predicate = construct_withing_predicate(this);
                if (predicate != null)
                {
                    within = predicate;
                    versionWithin = version;
                    return within(position);
                }
            }
            return false;
        }

        /// <summary>
        /// Анализ коллекции <paramref name="collection"/> и конструирование предиката, 
        /// определяющего нахождение точки внутри области, заданной этой коллекцией
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="reinitialize">Указывает на неоходимость заменить содержимое <seealso cref="CustomRegionCollection"/> на элементы коллекции <paramref name="collection"/></param>
        /// <param name="clone">Указывает на необходимость клонирования <seealso cref="CustomRegionEntry"/> при добавлении в коллекцию (если задан параметр <paramref name="reinitialize"/>)</param>
        protected Predicate<Vector3> construct_withing_predicate(IEnumerable<CustomRegionEntry> collection, bool reinitialize = false, bool clone = false)
        {
            if (reinitialize)
            {
                if (!ReferenceEquals(collection, this))
                    ClearItems();
                else reinitialize = false;
            }
            Predicate<Vector3> predicate;
            if (collection != null)
            {
                _union.Clear();
                _exclusion.Clear();
                _intersection.Clear();

                var customRegions = DesignContext.CustomRegions;

                CustomRegion FindCustomRegion(CustomRegionEntry customRegionEntry)
                {
                    return customRegions.FirstOrDefault(cr => cr.Name == customRegionEntry.Name);
                }
                foreach (var crEntry in collection)
                {
                    if (reinitialize && !TryAddValue(clone ? crEntry.Clone() : crEntry))
                        continue;

                    CustomRegion cr;
                    switch (crEntry.Inclusion)
                    {
                        case InclusionType.Union:
                            // Отсутствие cr, соответствующего crEntry,
                            // не является препятствием для обработки InclusionType.Union
                            cr = FindCustomRegion(crEntry);
                            if (cr != null)
                                _union.Add(cr);
                            break;
                        case InclusionType.Exclusion:
                            // Отсутствие cr, соответствующего crEntry,
                            // не является препятствием для обработки InclusionType.Exclusion
                            cr = FindCustomRegion(crEntry);
                            if (cr != null)
                                _exclusion.Add(cr);
                            break;
                        case InclusionType.Intersection:
                            // Отсутствие cr, соответствующего crEntry,
                            // означает, что пересечение является вырожденным множеством и ни одна точка в него не входит
                            cr = FindCustomRegion(crEntry);
                            if (cr != null)
                                _intersection.Add(cr);
                            break;
                    }

                    versionUnion = version;
                    versionIntersection = version;
                    versionExclusion = version;
                }

                // Выбираем предикат
                if (_exclusion.Count > 0)
                {
                    if (_intersection.Count > 0)
                    {
                        if (_union.Count > 0)
                            predicate = check_union_intersect_exclude;
                        else predicate = check_intersect_exclude;
                    }
                    else
                    {
                        if (_union.Count > 0)
                            predicate = check_union_exclude;
                        else predicate = check_exclude;
                    }
                }
                else
                {
                    if (_intersection.Count > 0)
                    {
                        if (_union.Count > 0)
                            predicate = check_union_intersect;
                        else predicate = check_intersect;
                    }
                    else
                    {
                        if (_union.Count > 0)
                            predicate = check_union;
                        else predicate = check_false;
                    }
                }
            }
            else predicate = check_true;
            return predicate;
        }

        /// <summary>
        /// Функторы сопоставления 
        /// </summary>
        protected bool check_union_intersect_exclude(Vector3 position)
        {
            return !_exclusion.Any(position.Within)
                   && _intersection.TrueForAll(position.Within)
                   && _union.Any(position.Within);
        }
        protected bool check_union_intersect(Vector3 position)
        {
            return _intersection.TrueForAll(position.Within)
                   && _union.Any(position.Within);
        }
        protected bool check_union_exclude(Vector3 position)
        {
            return !_exclusion.Any(position.Within)
                   && _union.Any(position.Within);
        }
        protected bool check_union(Vector3 position)
        {
            return _union.Any(position.Within);
        }
        protected bool check_intersect(Vector3 position)
        {
            return _intersection.TrueForAll(position.Within);
        }
        protected bool check_intersect_exclude(Vector3 position)
        {
            return !_exclusion.Any(position.Within)
                   && _intersection.TrueForAll(position.Within);
        }
        protected bool check_exclude(Vector3 position)
        {
            return !_exclusion.Any(position.Within);
        }
        protected bool check_false(Vector3 position) => false;
        protected bool check_true(Vector3 position) => true;
        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(label))
            {
                if (Count > 0)
                {
                    int unionCount = _union.Count,
                        intersectionCount = _intersection.Count,
                        exclusionCount = _exclusion.Count;
                    label = string.Concat(GetType().Name, '[',
                                          unionCount > 0 ? $" \x22c3 ({unionCount})" : string.Empty,
                                          intersectionCount > 0 ? $" \x22c2 ({intersectionCount})" : string.Empty,
                                          //exclusionCount > 0 ? $" \x00ac ({exclusionCount})" : string.Empty,
                                          exclusionCount > 0 ? $" \\ ({exclusionCount})" : string.Empty,
                                          ']');
                }
                else label = "Empty";
            }
            return label;
        }
        string label;

        public bool TryGetValue(string customRegionName, out CustomRegionEntry customRegionEntry)
        {
            if (Contains(customRegionName))
            {
                customRegionEntry = base[customRegionName];
                return true;
            }
            customRegionEntry = null;
            return false;
        }

        public bool TryAddValue(CustomRegionEntry customRegionEntry)
        {
            if (Contains(customRegionEntry.Name))
                return false;
#if INotifyPropertyChanged
            customRegionEntry.PropertyChanged += EntryPropertyChanged; 
#endif
            Add(customRegionEntry);
            customRegionEntry.Bind(this);
            return true;
        }

        public void EntryChanged(CustomRegionEntry customRegionEntry, string oldName, InclusionType oldInclusion)
        {
            if (ReferenceEquals(this, customRegionEntry.Collection))
            {
                version++;
                within = initialize_withing;
            }
        }

        public ReadOnlyCollection<CustomRegion> Union
        {
            get
            {
                if (versionUnion != version)
                    construct_withing_predicate(this);
                return _union.AsReadOnly();
            }
        }

        readonly List<CustomRegion> _union = new List<CustomRegion>();
        private long versionUnion = -1;
        public ReadOnlyCollection<CustomRegion> Exclusion
        {
            get
            {
                if (versionExclusion != version)
                    construct_withing_predicate(this);
                return _exclusion.AsReadOnly();
            }
        }

        readonly List<CustomRegion> _exclusion = new List<CustomRegion>();
        private long versionExclusion = -1;
        public ReadOnlyCollection<CustomRegion> Intersection
        {
            get
            {
                if (versionIntersection != version)
                    construct_withing_predicate(this);
                return _intersection.AsReadOnly();
            }
        }

        public void ResetCache()
        {
            version++;
            versionUnion = -1;
            versionIntersection = -1;
            versionExclusion = -1;
            versionWithin = -1;
            within = initialize_withing;
            _context = null;
        }

        readonly List<CustomRegion> _intersection = new List<CustomRegion>();
        private long versionIntersection = -1;

        Predicate<Vector3> within;
        private long versionWithin = -1;
        
        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            string startElemName = reader.Name;
            if (reader.ReadState == ReadState.Interactive && reader.IsEmptyElement)
            {
                reader.ReadStartElement(startElemName);
                return;
            }

            while (reader.ReadState == ReadState.Interactive)
            {
                string elemName = reader.Name;
                if (reader.IsStartElement())
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement(elemName);
                    }
                    else if (elemName == nameof(CustomRegion)
                        || elemName == nameof(CustomRegionEntry)
                        || elemName == "string")
                    {
                        var crEntry = reader.ReadContentAsCustomRegionEntry();
                        if (crEntry != null && !Contains(crEntry.Name))
                        {
                            Add(crEntry);
                            version++;
                        }
                        continue;
                    }
                    else if (elemName == nameof(InclusionType.Union))
                    {
                        ReadXmlAsList(reader, InclusionType.Union);
                        continue;
                    }
                    else if (elemName == nameof(InclusionType.Intersection))
                    {
                        ReadXmlAsList(reader, InclusionType.Intersection);
                        continue;
                    }
                    else if (elemName == nameof(InclusionType.Exclusion))
                    {
                        ReadXmlAsList(reader, InclusionType.Exclusion);
                        continue;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement
                && elemName == startElemName)
                {
                    reader.ReadEndElement();
                    break;
                }
                reader.Read();
            }
        }
        /// <summary>
        ///  Cчитывание поддерева xml, содержащего список названий, и добавление их в коллекцию с признаком <paramref name="inclusion"/>
        /// </summary>
        private void ReadXmlAsList(XmlReader reader, InclusionType inclusion)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            string startElemName = reader.Name;

            while (reader.ReadState == ReadState.Interactive)
            {
                string elemName = reader.Name;
                if (reader.IsStartElement())
                {
                    if (reader.IsEmptyElement)
                        reader.ReadStartElement(elemName);

                    else if (elemName == nameof(CustomRegion.Name)
                             || elemName == "string")
                    {
                        string crName = reader.ReadElementContentAsString(elemName, "");
                        if (!Contains(crName))
                        {
                            if (TryAddValue(new CustomRegionEntry(crName, inclusion)))
                                version++;
                        }
                    }
                    else reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement
                         && elemName == startElemName)
                {
                    reader.ReadEndElement();
                    break;
                }
                else reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
#if true
            if (Count > 0)
            {
                var groups = this.GroupBy(cr => cr.Inclusion);

                foreach (var group in groups)
                {
                    writer.WriteStartElement(group.Key.ToString(), "");
                    foreach (var item in group)
                        writer.WriteElementString(nameof(CustomRegion.Name), item.Name);
                    writer.WriteEndElement();
                } 
            }
#else
            // Проверяем актуальность списков union, intersection, exclusion
            if (versionUnion != version
                || versionIntersection != version
                || versionExclusion != version)
                construct_withing_predicate(this);

            // Сохраняем списки регионов
            if (_union?.Count > 0)
            {
                writer.WriteStartElement(nameof(InclusionType.Union), "");
                foreach (var cr in _union)
                    writer.WriteElementString(nameof(CustomRegion.Name), cr.Name);
                writer.WriteEndElement();
            }
            if (_intersection?.Count > 0)
            {
                writer.WriteStartElement(nameof(InclusionType.Intersection), "");
                foreach (var cr in _intersection)
                    writer.WriteElementString(nameof(CustomRegion.Name), cr.Name);
                writer.WriteEndElement();
            }
            if (_exclusion?.Count > 0)
            {
                writer.WriteStartElement(nameof(InclusionType.Exclusion), "");
                foreach (var cr in _exclusion)
                    writer.WriteElementString(nameof(CustomRegion.Name), cr.Name);
                writer.WriteEndElement();
            }
#endif
        }
        #endregion
    }
}
