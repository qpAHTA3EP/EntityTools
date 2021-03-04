using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools.Extensions;
using MyNW.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EntityTools.Tools.CustomRegions
{
    /// <summary>
    /// Коллекция из не повторяющихся <seealso cref="CustomRegion"/>, сочетание которые опередяет область на карте
    /// </summary>
    public class CustomRegionCollection : KeyedCollection<string, CustomRegionEntry>, IXmlSerializable
    {
        public CustomRegionCollection()
        {
            within = initialize_withing;
        }
        public CustomRegionCollection(IEnumerable<CustomRegionEntry> collection, bool clone = true)
        {
            var predicate = consctuct_withing_predicate(collection, true, clone);
            if (predicate != null)
                within = predicate;
            else within = initialize_withing;
        }
        public CustomRegionCollection(IEnumerable<CustomRegion> collection, InclusionType inclusion = InclusionType.Union)
        {
            var internInclusion = inclusion;
            var predicate = consctuct_withing_predicate(collection.Select(cr => new CustomRegionEntry(cr, internInclusion)), true);
            if (predicate != null)
                within = predicate;
            else within = initialize_withing;
        }
        private long version = 0;

        #region KeyedCollection
        protected override string GetKeyForItem(CustomRegionEntry customRegionEntry)
        {
            return customRegionEntry.Name;
        }
        protected override void ClearItems()
        {
            version++;
#if false   // Вызов Unbind() внутри перечисления недопустим
            using (var enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var crEntry = enumerator.Current;
                    if (crEntry.Collection != null)
                        enumerator.Current.Unbind();
                }
            } 
#else
            if (Count > 0)
            {
                CustomRegionEntry[] crEntryArray = new CustomRegionEntry[Count];
                CopyTo(crEntryArray, 0);
                crEntryArray.ForEach(crEntry => crEntry.Unbind());
                base.ClearItems();
            }
#endif
            label = string.Empty;
            within = initialize_withing;
        }
        protected override void InsertItem(int index, CustomRegionEntry customRegionEntry)
        {
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

        #region Within
        /// <summary>
        /// Проверка нахождения <paramref name="entity"/> в области заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        public bool Within(Entity entity)
        {
            if (entity is null)
                return false;
            return within(entity.Location);
        }
        /// <summary>
        /// Проверка нахождения <paramref name="position"/> в области заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        public bool Within(Vector3 position)
        {
            if (version != version_within)
                within = initialize_withing;
            return within(position);
        }

        /// <summary>
        /// Инициализация функтора проверки <paramref name="position"/> на предмет находения в области, заданной <seealso cref="CustomRegionCollection"/>
        /// </summary>
        protected bool initialize_withing(Vector3 position)
        {
            if (version != version_within)
            {
                var predicate = consctuct_withing_predicate(this);
                if (predicate != null)
                {
                    within = predicate;
                    version_within = version;
                    return within(position);
                }
            }
            return false; 
        }

        /// <summary>
        /// Анализ коллекции <paramref name="collection"/> и конструирование предиката, 
        /// определяющего нахождение точки внутри области, заданной этой коллекцией
        /// </summary>
        /// <param name="reinitialize">Указывает на неоходимость заменить содержимое <seealso cref="CustomRegionCollection"/> на элементы коллекции <paramref name="collection"/></param>
        /// <param name="clone">Указывает на необходимость клонирования <seealso cref="CustomRegionEntry"/> при добавлении в коллекцию (если задан параметр <paramref name="reinitialize"/>)</param>
        protected Predicate<Vector3> consctuct_withing_predicate(IEnumerable<CustomRegionEntry> collection, bool reinitialize = false, bool clone = false)
        {
            if (reinitialize)
            {
                if (!ReferenceEquals(collection, this))
                    ClearItems();
                else reinitialize = false;
            }
            Predicate<Vector3> predicate = null;
            if (collection != null)
            {
                _union.Clear();
                _exclusion.Clear();
                _intersection.Clear();

                int count = 0;
                int positiveCount = 0;
                CustomRegion cr;
                bool invalid = false;
                foreach (var crEntry in collection)
                {
                    if (reinitialize && !TryAddValue(clone ? crEntry.Clone() : crEntry))
                            continue;

                    count++;
                    switch (crEntry.Inclusion)
                    {
                        case InclusionType.Union:
                            // Отсутствие cr, соответствующего crEntry,
                            // не является препятствием для обработки InclusionType.Union
                            cr = crEntry.CustomRegion;
                            if (cr != null)
                            {
                                _union.Add(cr);
                                positiveCount++;
                            }
                            break;
                        case InclusionType.Exclusion:
                            // Отсутствие cr, соответствующего crEntry,
                            // не является препятствием для обработки InclusionType.Exclusion
                            cr = crEntry.CustomRegion;
                            if (cr != null)
                                _exclusion.Add(cr);
                            break;
                        case InclusionType.Intersection:
                            // Отсутствие cr, соответствующего crEntry,
                            // означает, что пересечение является вырожденным множеством и ни одна точка в него не входит
                            cr = crEntry.CustomRegion;
                            if (cr != null)
                            {
                                _intersection.Add(cr);
                                positiveCount++;
                            }
#if false
                            else
                            {
                                within = within_false;
                                union_set.Clear();
                                exclude_set.Clear();
                                intersect_set.Clear();
                                return false;
                            } 
#else
                            else invalid = true;
#endif
                            break;
                    }

                    version_union = version;
                    version_intersection = version;
                    version_intersection = version;
                }
                // Выбираем предикат
                if (!invalid && positiveCount > 0)
                {
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
                            else predicate = check_false;
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
                else predicate = check_false;
            }
            return predicate;
        }

        /// <summary>
        /// Функторы сопоставления 
        /// </summary>
        protected bool check_union_intersect_exclude(Vector3 position)
        {
            return (_exclusion.Count == 0 || _exclusion.TrueForAll(cr => !position.Within(cr)))
                && (_intersection.Count == 0 || _intersection.TrueForAll(cr => position.Within(cr)))
                && _union.Any(cr => position.Within(cr));
        }
        protected bool check_union_intersect(Vector3 position)
        {
            return (_intersection.Count == 0 || _intersection.TrueForAll(cr => position.Within(cr)))
                && _union.Any(cr => position.Within(cr));
        }
        protected bool check_union_exclude(Vector3 position)
        {
            return (_exclusion.Count == 0 || _exclusion.TrueForAll(cr => !position.Within(cr)))
                && _union.Any(cr => position.Within(cr));
        }
        protected bool check_union(Vector3 position)
        {
            return _union.Any(cr => position.Within(cr));
        }
        protected bool check_intersect(Vector3 position)
        {
            return _intersection.Count == 0 || _intersection.TrueForAll(cr => position.Within(cr));
        }
        protected bool check_intersect_exclude(Vector3 position)
        {
            return (_exclusion.Count == 0 || _exclusion.TrueForAll(cr => !position.Within(cr)))
                && (_intersection.Count == 0 || _intersection.TrueForAll(cr => position.Within(cr)));
        }
        protected bool check_false(Vector3 position) => false;
        protected bool check_true(Vector3 position) => true; 
        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(label))
            {
                if (base.Count > 0)
                    label = string.Concat(GetType().Name, '[', base.Count, ']');
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
                if (version_union != version)
                    consctuct_withing_predicate(this);
                return _union.AsReadOnly();
            }
        }
        List<CustomRegion> _union = new List<CustomRegion>();
        long version_union = -1;
        public ReadOnlyCollection<CustomRegion> Exclusion
        {
            get
            {
                if (version_exclusion != version)
                    consctuct_withing_predicate(this);
                return _exclusion.AsReadOnly();
            }
        }
        List<CustomRegion> _exclusion = new List<CustomRegion>();
        long version_exclusion = -1;
        public ReadOnlyCollection<CustomRegion> Intersection
        {
            get
            {
                if (version_intersection != version)
                    consctuct_withing_predicate(this);
                return _intersection.AsReadOnly();
            }
        }
        List<CustomRegion> _intersection = new List<CustomRegion>();
        long version_intersection = -1;

        Predicate<Vector3> within;
        long version_within = -1;

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

#if true
        public void ReadXml(XmlReader reader)
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
                    else if(elemName == nameof(InclusionType.Union))
                        ReadXmlAsList(reader, InclusionType.Union);
                    else if (elemName == nameof(InclusionType.Intersection))
                        ReadXmlAsList(reader, InclusionType.Intersection);
                    else if (elemName == nameof(InclusionType.Exclusion))
                        ReadXmlAsList(reader, InclusionType.Exclusion);
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
        /// <param name="reader"></param>
        /// <param name="xmlEndElement"></param>
        private void ReadXmlAsList(XmlReader reader, InclusionType inclusion)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            string startElemName = reader.Name;

            string crName = string.Empty;
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
                        crName = reader.ReadElementContentAsString(elemName, "");
                        if (!Contains(crName))
                        {
                            if(TryAddValue(new CustomRegionEntry(crName, inclusion)))
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
#else
        public void ReadXml(XmlReader reader)
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
                    {
                        reader.ReadStartElement(elemName);
                    }
#if ReadInnerXml
                    else
                    {
                        reader.ReadStartElement(elemName);
                        ReadInnerXml(reader, elemName)
                        reader.ReadEndElement(); 
                    }
#endif
                    else if (elemName == nameof(CustomRegion)
                        || elemName == nameof(CustomRegionEntry)
                        || elemName == "string")
                    {
                        var crEntry = reader.ReadContentAsCustomRegionEntry();
                        if (crEntry != null && !Contains(crEntry.Name))
                            Add(crEntry);
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
#endif

#if true // Запись в Xml списков CustomRegion.Name, сгруппированных по InclusionType
        public void WriteXml(XmlWriter writer)
        {
            // Проверяем актуальность списков union, intersection, exclusion
            if (version_union != version
                || version_intersection != version
                || version_exclusion != version)
                consctuct_withing_predicate(this);

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
        }
#else   // Запись в Xml каждого CustomRegionEntry отдельно
        public void WriteXml(XmlWriter writer)
        {
            using (var itemEnumerator = base.GetEnumerator())
            {
                while (itemEnumerator.MoveNext())
                {
                    var item = itemEnumerator.Current;
                    if (item.Inclusion != InclusionType.Ignore)
                    {
                        writer.WriteStartElement(nameof(CustomRegion), "");
                        item.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
            }
        }
#endif
        #endregion
    }
}
