using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Tools.Extensions;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools.Missions
{
    /* Реализация с несколькими вариантами квестодателя 
     * Item, RemoteContact, NPC
     */
    [Serializable]
    public class MissionGiverInfo : IXmlSerializable
    {
        public Vector3 Position { get => _position; set => _position = value; }
        private Vector3 _position = new Vector3();

        public double Distance =>  _position.IsValid ? _position.Distance3DFromPlayer : 0;

        public string Id
        {
            get => _id; set
            {
                _id = value;
                _label = string.Empty;
            }
        }
        private string _id = string.Empty;

        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        public string MapName
        {
            get => _mapName;
            set
            {
                _mapName = value;
                _label = string.Empty;
            }
        }
        private string _mapName = string.Empty;

        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
        public string RegionName
        {
            get => _regionName; set
            {
                _regionName = value;
                _label = string.Empty;
            }
        }
        private string _regionName = string.Empty;

        /// <summary>
        /// <paramref name="entity"/> соответствует патаметрам квестодателя
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsMatching(Entity entity)
        {
            if (entity is null
                || !entity.IsValid
                || string.IsNullOrEmpty(_id))
                return false;

            if (entity.Location.Distance3D(_position) > 1.0
                || !(entity.CostumeRef.CostumeName.Equals(_id) || entity.InternalName.Equals(_id)))
                return false;

            if (string.IsNullOrEmpty(_mapName))
                return true;

            var player = EntityManager.LocalPlayer;
            return _mapName.Equals(player.MapState.MapName) && _regionName.Equals(player.RegionInternalName);
        }
        /// <summary>
        /// Квестодатель задан корректно
        /// </summary>
        public bool IsValid
        {
            get
            {
#if false
                var player = EntityManager.LocalPlayer;
                return _position.IsValid
                       && !string.IsNullOrEmpty(_id)
                       && (string.IsNullOrEmpty(_mapName)
                           || _mapName.Equals(player.MapState.MapName) && _regionName.Equals(player.RegionInternalName)); 
#else
                return _position.IsValid
                       && !string.IsNullOrEmpty(_id);
#endif
            }
        }
        /// <summary>
        /// Квестодатель зада корректно и к нему можно переместиться для взаимодействия,
        /// то есть персонаж находится на нужной карте и в том же регионе, что и квестодатель
        /// </summary>
        public bool IsAccessible
        {
            get
            {
                var player = EntityManager.LocalPlayer;
                return _position!= null && _position.IsValid
                       && !string.IsNullOrEmpty(_id)
                       && (string.IsNullOrEmpty(_mapName) || _mapName.Equals(player.MapState.MapName) && _regionName.Equals(player.RegionInternalName));
            }
        }

        string _label = string.Empty;
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_id)
                || !Position.IsValid)
                _label = "Not set";
            if (string.IsNullOrEmpty(_mapName))
                _label = _id;
            else if (string.IsNullOrEmpty(_regionName))
                _label = string.Concat(_id, " (", _mapName, ")");
            else _label = string.Concat(_id, " (", _mapName, "/", _regionName, ")");

            return _label;
        }

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
                        case nameof(Id):
                        case nameof(Astral.Quester.Classes.NPCInfos.CostumeName):
                            _id = reader.ReadElementContentAsString(elemName, "");
                            break;
                        case nameof(MapName):
                            _mapName = reader.ReadElementContentAsString(elemName, "");
                            break;
                        case nameof(RegionName):
                            _regionName = reader.ReadElementContentAsString(elemName, "");
                            break;
                        case nameof(Position):
                            try
                            {
                                if (_position is null)
                                    _position = new Vector3();
                                using (XmlReader subtreeReader = reader.ReadSubtree())
                                    _position.ReadXml(subtreeReader);

                                if (reader.NodeType == XmlNodeType.EndElement
                                    && reader.Name == nameof(Position))
                                    reader.ReadEndElement();
                            }
                            catch (XmlException except)
                            {
                                _position.X = 0;
                                _position.Y = 0;
                                _position.Z = 0;
                                ETLogger.WriteLine(LogType.Error, except.Message, true);
                            }
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
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(Id), _id);
            if(!string.IsNullOrEmpty(_mapName))
                writer.WriteElementString(nameof(MapName), _mapName);
            if (!string.IsNullOrEmpty(_regionName))
                writer.WriteElementString(nameof(RegionName), _regionName);
            if (!_position.IsValid) return;

            writer.WriteStartElement(nameof(Position), "");
            _position.WriteXml(writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
