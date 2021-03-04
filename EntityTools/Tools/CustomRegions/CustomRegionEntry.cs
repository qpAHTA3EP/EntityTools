using Astral.Quester.Classes;
using EntityTools.Enums;
using System;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace EntityTools.Tools.CustomRegions
{
    public class CustomRegionEntry : IXmlSerializable
    {
        public CustomRegionEntry() { }
        public CustomRegionEntry(string name, InclusionType inclusion)
        {
            _name = name;
            _inclusion = inclusion;
        }
        public CustomRegionEntry(CustomRegion cr, InclusionType inclusion)
        {
            _name = cr.Name;
            _inclusion = inclusion;
            _customRegion = cr;
        }


        #region Данные
        public string Name
        {
            get => _name;
            protected set
            {
                if (_name != value)
                {
                    var oldName = _name;
                    _customRegion = null;
                    _label = string.Empty;
                    _name = value;
                    _collection?.EntryChanged(this, oldName, _inclusion);
                }
            }
        }
        private string _name;

        /// <summary>
        /// Тип включения в набор CustomRegion'ов
        /// </summary>
        public InclusionType Inclusion
        {
            get => _inclusion;
            set
            {
                var oldInclusion = _inclusion;
                _label = string.Empty;
                _inclusion = value;
                _collection?.EntryChanged(this, _name, oldInclusion);
            }
        }
        private InclusionType _inclusion;

        /// <summary>
        /// CustomRegion, соответствующий идентификатору <see cref="Name"/>
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public CustomRegion CustomRegion
        {
            get
            {
                if (_customRegion is null || _customRegion.Name != _name)
                    _customRegion = Astral.Quester.API.CurrentProfile.CustomRegions.FirstOrDefault(cr => cr.Name == _name);

                return _customRegion;
            }
            set
            {
                if(_name == value?.Name)
                {
                    _customRegion = value;
                }
            }
        }
        private CustomRegion _customRegion;

        /// <summary>
        /// Коллекция, к которой привязан объект
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public CustomRegionCollection Collection
        {
            get => _collection;
        }
        CustomRegionCollection _collection; 
        #endregion

        /// <summary>
        /// Привязка элемента к коллекции
        /// </summary>
        /// <param name="collection"></param>
        public void Bind(CustomRegionCollection collection)
        {
            if (collection != null && !ReferenceEquals(_collection, collection))
            {
                if (_collection != null)
                {
                    var oldCollection = _collection;
                    _collection = null;
                    oldCollection.Remove(this);
                }
                _collection = collection;
                _collection.EntryChanged(this, string.Empty, InclusionType.Ignore);
            }
        }
        /// <summary>
        /// Отвезка элемента от коллекции
        /// </summary>
        public void Unbind()
        {
            if (_collection != null)
            {
                var oldCollection = _collection;
                _collection = null;
                oldCollection.Remove(this);
            }
        }

        public CustomRegionEntry Clone()
        {
            if(_customRegion is null)
                return new CustomRegionEntry(_name, _inclusion);
            else return new CustomRegionEntry(_customRegion, _inclusion);
        }

#if false
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
                _label = $"[{_inclusion}] {_name}";
            return _label;
        } 
#else
        public override string ToString() => _name;
#endif
        string _label;

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

            string crName = string.Empty;
            InclusionType crInclusion = InclusionType.Ignore;
            bool initializedInclusion = false;
            while (reader.ReadState == ReadState.Interactive)
            {
                string elemName = reader.Name;
                if (reader.IsStartElement())
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement(elemName);
                        continue;
                    }

                    switch (elemName)
                    {
                        case nameof(Inclusion):
                            var strGiverType = reader.ReadElementContentAsString(elemName, "");
                            initializedInclusion = Enum.TryParse(strGiverType, out crInclusion);
                            break;
                        case nameof(Name):
                            crName = reader.ReadElementContentAsString(elemName, "");
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement
                    && elemName == startElemName)
                {
                    reader.ReadEndElement();
                    break;
                }
                else reader.Read();
            }
            _name = crName;
            _inclusion = initializedInclusion ? crInclusion : InclusionType.Union;
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(Inclusion), _inclusion.ToString());
            writer.WriteElementString(nameof(Name), _name);
        }
        #endregion
    }

    public static class CustomRegionXmlReaderExtention
    {
        public static CustomRegionEntry ReadContentAsCustomRegionEntry(this XmlReader reader, bool skipIgnored = true)
        {
            CustomRegionEntry crEntry = null;

            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            string startElemName = reader.Name;

            var crName = string.Empty;
            if (startElemName == "string")
            {
                // Проверка старого представления списка CustomRegion'ов как списка строк
                crName = reader.ReadElementContentAsString(startElemName, "");
                crEntry = new CustomRegionEntry(crName, InclusionType.Union);
            }
            else
            { 
                var crInclusion = InclusionType.Ignore;
                bool initializedInclusion = false;
                while (reader.ReadState == ReadState.Interactive)
                {
                    string elemName = reader.Name;
                    if (reader.IsStartElement())
                    {
                        if (reader.IsEmptyElement)
                        {
                            reader.ReadStartElement(elemName);
                            continue;
                        }

                        switch (elemName)
                        {
                            case nameof(CustomRegionEntry.Inclusion):
                                var strInclusionType = reader.ReadElementContentAsString(elemName, "");
                                initializedInclusion = Enum.TryParse(strInclusionType, out crInclusion);
                                if(!initializedInclusion)
                                {
                                    if(strInclusionType == "Merge")
                                    {
                                        crInclusion = InclusionType.Union;
                                        initializedInclusion = true;
                                    }
                                    else if(strInclusionType == "Intersect")
                                    {
                                        crInclusion = InclusionType.Intersection;
                                        initializedInclusion = true;
                                    }
                                    if (strInclusionType == "Exclude")
                                    {
                                        crInclusion = InclusionType.Exclusion;
                                        initializedInclusion = true;
                                    }
                                }
                                break;
                            case nameof(CustomRegionEntry.Name):
                                crName = reader.ReadElementContentAsString(elemName, "");
                                break;
                            default:
                                reader.Read();
                                break;
                        }
                        continue;
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement
                        && elemName == startElemName)
                    {
                        reader.ReadEndElement();
                        break;
                    }
                    reader.Read();
                } 
                crInclusion = initializedInclusion ? crInclusion : InclusionType.Union;
                if (!string.IsNullOrEmpty(crName) && !(skipIgnored && crInclusion == InclusionType.Ignore))
                    crEntry = new CustomRegionEntry(crName, crInclusion);
            }

            return crEntry;
        }
    }
}
