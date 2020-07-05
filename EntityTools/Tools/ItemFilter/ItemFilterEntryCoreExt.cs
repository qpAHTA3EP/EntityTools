using Astral.Classes.ItemFilter;
using EntityTools.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MyNW.Classes;

namespace EntityTools.Tools.ItemFilter
{
    /// <summary>
    /// Класс, реализующий набор фильтров и логику проверки соответствия
    /// </summary>
    /// <typeparam name="TFilterEntry"></typeparam>
    [Serializable]
    public partial class ItemFilterCoreExt<TFilterEntry> where TFilterEntry : IFilterEntry
    {
        public ItemFilterCoreExt()
        {
            _fullFilters.ListChanged += ResetPredicates;
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="source"></param>
        public ItemFilterCoreExt(ItemFilterCoreExt<TFilterEntry> source)
        {
            if (source != null && source._fullFilters != null && source._fullFilters.Count > 0)
            {
                foreach (var f in source._fullFilters)
                {
                    TFilterEntry clone = (TFilterEntry)f.Clone();
                    _fullFilters.Add(clone);
                    clone.PropertyChanged += ResetPredicates;
                }
                isMatchPredicate = IsMatch_Selector;
#if disabled_20200624_2153
                excludePredicate = IsForbidden_Selector; 
#endif
            }
            _fullFilters.ListChanged += ResetPredicates;
        }
        /// <summary>
        /// Полный список фильтров без разделения на Include/Exclude
        /// </summary>
        public BindingList<TFilterEntry> Filters
        {
            get => _fullFilters;
            set
            {
                if (value != null && value.Count > 0)
                {
                    ResetPredicates();
                    _fullFilters.ListChanged -= ResetPredicates;
                    _fullFilters = value;
                    _fullFilters.ListChanged += ResetPredicates;
                }
                else
                {
                    _fullFilters.ListChanged -= ResetPredicates;
                    _fullFilters.Clear();
                    ResetPredicates();
                }
            } 
        }
        [XmlIgnore]
        BindingList<TFilterEntry> _fullFilters = new BindingList<TFilterEntry>();

        /// <summary>
        /// Список фильтров, сгруппированных и обработанных для использования
        /// </summary>
        public IEnumerable<ItemFilterGroup<TFilterEntry>> Groups
        {
            get
            {
                if(_groups.Count == 0)
                {
                    if (_fullFilters != null && _fullFilters.Count > 0)
                    {
                        if (!AnalizeFilters(_fullFilters, ref _groups, ref generalExclude))
                        {
                            _groups.Clear();
                            generalExclude.Clear();
                        }
                    }
                }
#if false
                foreach (var g in _groups)
                    yield return g; 
#else
                return _groups;
#endif
            }
        }
        SortedSet<ItemFilterGroup<TFilterEntry>> _groups = new SortedSet<ItemFilterGroup<TFilterEntry>>();
        ItemFilterGroup<TFilterEntry> generalExclude = null;

        public bool IsValid
        {
            get => _fullFilters != null && _fullFilters.Count > 0 && isMatchPredicate != null;
        }

        /// <summary>
        /// Анализ <paramref name="inFilters"/> Формирование групп фильтров <paramref name="outFilterGroup"/>
        /// </summary>
        /// <param name="inFilters"></param>
        /// <param name="outFilterGroup"></param>
        /// <returns></returns>
        private bool AnalizeFilters(IEnumerable<TFilterEntry> inFilters, ref SortedSet<ItemFilterGroup<TFilterEntry>> outFilterGroup, ref ItemFilterGroup<TFilterEntry> outGeneralExcude)
        {
            if (outFilterGroup is null)
                outFilterGroup = new SortedSet<ItemFilterGroup<TFilterEntry>>();
            else outFilterGroup.Clear();
            if (outGeneralExcude is null)
                outGeneralExcude = new ItemFilterGroup<TFilterEntry>();
            else outGeneralExcude.Clear();

            if(inFilters != null)
            {
                IOrderedEnumerable<IGrouping<uint, TFilterEntry>> filterGroups = inFilters.GroupBy((TFilterEntry f) => f.Group).OrderBy((group) => group.Key);

#if SortedSet
                outFilterGroup = new SortedSet<ItemFilterGroup<TFilterEntry>>(); 
#else
                outFilterGroup = new SortedSet<ItemFilterGroup<TFilterEntry>>();
#endif

                foreach (var grp in filterGroups)
                {
                    if (grp.Key == uint.MaxValue)
                    {
                        if (outGeneralExcude is null)
                            outGeneralExcude = new ItemFilterGroup<TFilterEntry>(grp);
                        else outGeneralExcude.AddFilter(grp);
                    }
                    else outFilterGroup.Add(new ItemFilterGroup<TFilterEntry>(grp));
                }

                return outFilterGroup.Count > 0;
            }
            return false;
        }


        /// <summary>
        /// Обработчик, реализующий обнуление предикатов <seealso cref="isMatchPredicate"/> и <seealso cref="excludePredicate"/> в случае изменения свойств элементов списка фильтров <seealso cref="Filters"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPredicates(object sender, string propertyName)
        {
            if (propertyName == nameof(IFilterEntry.EntryType)
                || propertyName == nameof(IFilterEntry.Mode)
                || propertyName == nameof(IFilterEntry.Pattern)
                || propertyName == nameof(BuyFilterEntry.Priority))
            {
                _groups.Clear();
#if disabled_20200624_2153
                excludePredicate = IsForbidden_Selector; 
#endif
                isMatchPredicate = IsMatch_Selector;
            }
        }

        /// <summary>
        /// Обработчик, реализующий обнуление предикатов <seealso cref="isMatchPredicate"/> и <seealso cref="excludePredicate"/> сопоставления в случае изменения списка <seealso cref="Filters"/> (добавление, удаление, изменение)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetPredicates(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded
               || e.ListChangedType == ListChangedType.ItemChanged
               || e.ListChangedType == ListChangedType.ItemDeleted)
            {
                _groups.Clear();
#if disabled_20200624_2153
                excludePredicate = IsForbidden_Selector; 
#endif
                isMatchPredicate = IsMatch_Selector;
            }
        }

        /// <summary>
        /// Обработчик, реализующий обнуление предикатов <seealso cref="isMatchPredicate"/> и <seealso cref="excludePredicate"/> сопоставления в случае изменения списка <seealso cref="Filters"/> (добавление, удаление, изменение)
        /// </summary>
        private void ResetPredicates()
        {
            _groups.Clear();
#if disabled_20200624_2153
            excludePredicate = IsForbidden_Selector; 
#endif
            isMatchPredicate = IsMatch_Selector;
        }

        #region Сопоставление
        /// <summary>
        /// Функтор сопоставления, выполняющий проверку include и exclude фильтров
        /// </summary>
        Predicate<Item> isMatchPredicate = null;

#if disabled_20200624_2153
        /// <summary>
        /// Запрещающий предикат, исключающий Item из обработки
        /// </summary>
        [XmlIgnore]
        Predicate<Item> excludePredicate = null;

        /// <summary>
        /// Проверка слота на соответствие запрещающему Exclude-фильтру
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool IsForbidden(InventorySlot slot)
        {
            return excludePredicate?.Invoke(slot.Item) == true;
        }
        public bool IsForbidden(Item item)
        {
            return excludePredicate?.Invoke(item) == true;
        }
        /// <summary>
        /// Конструирование предиката, 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsForbidden_Selector(Item item)
        {
            if (generalExclude is null)
            {
                if (_fullFilters != null && _fullFilters.Count > 0)
                {
                    if (_groups is null || _groups.Count == 0)
                    {
                        if (AnalizeFilters(_fullFilters, out _groups, out generalExclude))
                        {
                            if (generalExclude is null)
                            {
                                excludePredicate = IsForbidden_False;
                                return false;
                            }
                            else
                            {
                                excludePredicate = generalExclude.IsForbidden;
                                return generalExclude.IsForbidden(item);
                            }
                        }
                    }
                    else excludePredicate = IsForbidden_False;
                }
                excludePredicate = IsForbidden_True;
                return true;
            }
            else
            {
                excludePredicate = generalExclude.IsForbidden;
                return generalExclude.IsForbidden(item);
            }
        }
        bool IsForbidden_True(Item item)
        {
            return true;
        }
        bool IsForbidden_False(Item item)
        {
            return false;
        } 
#endif

        /// <summary>
        /// Проверка слота на соответствие набору фильтров, включая проверку include и exclude фильтров
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool IsMatch(InventorySlot slot)
        {
            return IsMatch(slot.Item);
        }
        public bool IsMatch(Item item)
        {
            return isMatchPredicate.Invoke(item) == true;
        }

        /// <summary>
        /// Конструирование функтора сопоставления
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsMatch_Selector(Item item)
        {
            if (_groups.Count == 0)
            {
                if (_fullFilters == null || _fullFilters.Count > 0 || !AnalizeFilters(_fullFilters, ref _groups, ref generalExclude))
                {
                    isMatchPredicate = IsMatch_False;
#if disabled_20200624_2153
                    excludePredicate = IsForbidden_True; 
#endif
                    return false;
                }
                else
                {
                    _groups.Clear();
                    generalExclude.Clear();
                }
            }

            if (generalExclude is null)
                isMatchPredicate = IsMatch_Groups;
            else isMatchPredicate = IsMatch_GroupsAndGeneralExclude;

            return isMatchPredicate(item); 
        }

        /// <summary>
        /// Сопоставление предмета <paramref name="item"/> с набором групп <seealso cref="Groups"/> и нааборм запрещающий фильтров <seealso cref="generalExclude"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsMatch_GroupsAndGeneralExclude(Item item)
        {
            if(!generalExclude.IsMatch(item))
            {
                foreach(var filterGroup in _groups)
                    if (filterGroup.IsMatch(item))
                        return true;
            }
            return false;
        }
        /// <summary>
        /// Сопоставление предмета <paramref name="item"/> с набором групп <seealso cref="Groups"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsMatch_Groups(Item item)
        {
            foreach (var filterGroup in _groups)
                if (filterGroup.IsMatch(item))
                    return true;
            return false;
        }
        /// <summary>
        /// Заглушка при отсутствии фильтров
        /// </summary>
        bool IsMatch_False(Item item)
        {
            return false;
        }
        #endregion

        public override string ToString()
        {
            if (_fullFilters != null && _fullFilters.Count > 0)
                return $"{_fullFilters.Count} entries in filter";
            else return "Filter is empty";
        }
    }

    // Часть определения класса, реализующего интерфейсы
    public partial class ItemFilterCoreExt<TFilterEntry> : IXmlSerializable, IEnumerable<TFilterEntry>
    {
        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read(); 
            while (reader.ReadState == ReadState.Interactive)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elemName = reader.Name;
                    if (reader.IsEmptyElement)
                        throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: does not expect empty element <{elemName} />");
                    else
                    {
                        List<TFilterEntry> deprecatedFilters = new List<TFilterEntry>();
                        using (XmlReader subtreeReader = reader.ReadSubtree())
                        {
                            ReadInnerXml(subtreeReader, elemName, deprecatedFilters);
                        }
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elemName)
                            reader.ReadEndElement();
                        else throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected {reader.NodeType} named '{reader.Name}' instead of <\\{elemName}>");

                        // Если в Xml не заданы индексы группы для TFilterEntry, тогда им присваивается временный индекс uint.MaxValue
                        // Разделение группы фильтров с индексом uint.MaxValue на запрещающие и разрешающие элементы
                        if (deprecatedFilters.Count > 0)
                        {
                            uint groupInd = 0;
                            if (_fullFilters.Count > 0)
                            {
                                foreach (var deprFilter in deprecatedFilters)
                                {
                                    if (deprFilter.Mode == ItemFilterMode.Include)
                                    {
                                        // Ищем "свободный" инденкc
                                        while (_fullFilters.First((TFilterEntry f) => f.Group == groupInd) != null && groupInd < uint.MaxValue)
                                            groupInd++;

                                        deprFilter.Group = groupInd;
                                        _fullFilters.Add(deprFilter);
                                    }
                                    else
                                    {
                                        deprFilter.Group = uint.MaxValue;
                                        _fullFilters.Add(deprFilter);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var f in deprecatedFilters)
                                {
                                    if (f.Mode == ItemFilterMode.Include)
                                    {
                                        f.Group = groupInd;
                                        _fullFilters.Add(f);
                                        groupInd++;
                                    }
                                    else
                                    {
                                        f.Group = uint.MaxValue;
                                        _fullFilters.Add(f);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                        reader.ReadEndElement();
                else reader.Skip();
            }
#if false
            if (reader.IsStartElement())
            {
                string elemName = reader.Name;
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement(elemName);
                    }
                    else
                    {
                        List<TFilterEntry> deprecatedFilters = new List<TFilterEntry>();

                        reader.ReadStartElement(elemName);
                        ReadInnerXml(reader, elemName, deprecatedFilters);
                        reader.ReadEndElement();

                        //TODO: Разделить группу фильтров с индексом uint.MaxValue на запрещающие и разрешающие элементы
                        if (deprecatedFilters.Count > 0)
                        {
                            uint groupInd = 0;
                            if (_fullFilters.Count > 0)
                            {
                                foreach (var defF in deprecatedFilters)
                                {
                                    if (defF.Mode == ItemFilterMode.Include)
                                    {
                                        // Ищем "свободный" инденк 
                                        while (_fullFilters.First((TFilterEntry f) => f.Group == groupInd) != null && groupInd < uint.MaxValue)
                                            groupInd++;

                                        defF.Group = groupInd;
                                        _fullFilters.Add(defF);
                                    }
                                    else
                                    {
                                        defF.Group = uint.MaxValue;
                                        _fullFilters.Add(defF);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var f in deprecatedFilters)
                                {
                                    if (f.Mode == ItemFilterMode.Include)
                                    {
                                        f.Group = groupInd;
                                        _fullFilters.Add(f);
                                        groupInd++;
                                    }
                                    else
                                    {
                                        f.Group = uint.MaxValue;
                                        _fullFilters.Add(f);
                                    }
                                }
                            }
                        }
                    }
                }
            } 
#endif
        }

        /// <summary>
        ///  Рекурсивное считывание поддерева xml до обнаружения xmlEndElement
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="xmlEndElement"></param>
        private void ReadInnerXml(XmlReader reader, string xmlEndElement, List<TFilterEntry> deprecatedFilters)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            while (reader.ReadState == ReadState.Interactive)
            {
                string elemName = reader.Name;
                if (elemName == nameof(ItemFilterEntry)
                                  || elemName == nameof(CommonFilterEntry)
                                  || elemName == nameof(BuyFilterEntry)
                                  || elemName == "ItemFilterEntryExt")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.IsEmptyElement)
                        {
                            reader.ReadStartElement(elemName);
                            continue;
                        }
                        else
                        {
                            try
                            {
                                using (XmlReader subtreeReader = reader.ReadSubtree())
                                {
                                    TFilterEntry filterEntry = (TFilterEntry)Activator.CreateInstance(typeof(TFilterEntry));
                                    filterEntry.ReadXml(subtreeReader);
                                    if (filterEntry.Group != uint.MaxValue)
                                        _fullFilters.Add(filterEntry);
                                    else deprecatedFilters.Add(filterEntry);
                                }
                            }
                            catch(XmlException except)
                            {
                                ETLogger.WriteLine(LogType.Error, except.Message, true);
                                continue;
                            }
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement)
                        reader.ReadEndElement();
                }
                else if (reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement)
                {
                    reader.ReadStartElement(elemName);
                    ReadInnerXml(reader, elemName, deprecatedFilters);
                }
                else reader.Skip();

#if false
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string elemName = reader.Name;
                        if (elemName == nameof(ItemFilterCore.Entries))
                        {
                            if (reader.IsEmptyElement)
                            {
                                reader.ReadStartElement(elemName);
                                return;
                            }
                            else if (reader.IsStartElement())
                            {
                                using (XmlReader subtreeReader = reader.ReadSubtree())
                                {
                                    ReadInnerXml(subtreeReader, elemName, deprecatedFilters);
                                }
                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elemName)
                                    reader.ReadEndElement();
                                else throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected {reader.NodeType} named '{elemName}' instead of <\\{xmlEndElement}>");
                            }
                            else return;
                        }
                        else if (elemName == nameof(ItemFilterEntry)
                                  || elemName == nameof(CommonFilterEntry)
                                  || elemName == nameof(BuyFilterEntry)
                                  || elemName == "ItemFilterEntryExt")
                        {
                            using (XmlReader subtreeReader = reader.ReadSubtree())
                            {
                                TFilterEntry filterEntry = (TFilterEntry)Activator.CreateInstance(typeof(TFilterEntry));
                                filterEntry.ReadXml(subtreeReader);
                                if (filterEntry.Group != uint.MaxValue)
                                    _fullFilters.Add(filterEntry);
                                else deprecatedFilters.Add(filterEntry);
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        elemName = reader.Name;
                        if (elemName == xmlEndElement)
                            return;
                        else throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected {XmlNodeType.EndElement} '{elemName}'  instead of <\\{xmlEndElement}>");
                    default:
                        reader.Skip();
                        break;
                } 
#endif
            }
        }
#if ReadInnerXml_old
        private void ReadInnerXml_old(XmlReader reader, string xmlEndElement, List<TFilterEntry> deprecatedFilters)
        {
            while (reader.ReadState == ReadState.Interactive)
            {
                string elemName = reader.Name;
                if (elemName == nameof(ItemFilterCore.Entries))
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement(elemName);
                        return;
                    }
                    else if (reader.IsStartElement())
                    {
                        reader.ReadStartElement(elemName);
                        ReadInnerXml(reader, elemName, deprecatedFilters);
                        reader.ReadEndElement();
                    }
                    else return;
                }
                else if (reader.IsStartElement()
                          && (elemName == nameof(ItemFilterEntry)
                              || elemName == nameof(CommonFilterEntry)
                              || elemName == nameof(BuyFilterEntry)
                              || elemName == "ItemFilterEntryExt"))
                {
                    TFilterEntry filterEntry = (TFilterEntry)Activator.CreateInstance(typeof(TFilterEntry));
                    filterEntry.ReadXml(reader);
                    if (filterEntry.Group != uint.MaxValue)
                        _fullFilters.Add(filterEntry);
                    else
                    {
                        deprecatedFilters.Add(filterEntry);
                    }
                }
                else if (elemName == xmlEndElement)
                {
                    if (!reader.IsStartElement())
                        return;
                    else throw new XmlException($"{MethodBase.GetCurrentMethod().Name}: Unexpected XmlStartElement '{elemName}' while there are should be the XmlEndElement '{xmlEndElement}'");
                }
                else reader.Skip();
            }
        } 
#endif

        public void WriteXml(XmlWriter writer)
        {
            foreach(TFilterEntry fEntry in _fullFilters)
            {
                writer.WriteStartElement(fEntry.GetType().Name, "");
                fEntry.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion

        #region IEnumerable
        public IEnumerator<TFilterEntry> GetEnumerator()
        {
            if (_fullFilters != null && _fullFilters.Count > 0)
                return _fullFilters.GetEnumerator();
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_fullFilters != null && _fullFilters.Count > 0)
                return _fullFilters.GetEnumerator();
            return null;
        } 
        #endregion
    }
}
