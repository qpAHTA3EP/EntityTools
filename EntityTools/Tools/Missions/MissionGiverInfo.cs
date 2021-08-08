using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Astral.Quester.UIEditors;
using EntityTools.Enums;
using EntityTools.Tools.Extensions;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Tools.Missions
{
    [Serializable]
    public class MissionGiverInfo : IXmlSerializable
    {
        public MissionGiverInfo() { }
        public MissionGiverInfo(string id, Vector3 position, string map = "", string region = "")
        {
            _id = id;
            _position = position.Clone();
            _mapName = map;
            _regionName = region;
            _type = MissionGiverType.NPC;
        }
        public MissionGiverInfo(string id)
        {
            _id = id;
            _type = MissionGiverType.Remote;
        }

        [NotifyParentProperty(true)]
        public MissionGiverType Type
        {
            get
            {
                return _type;
            }
            private set
            {
                _type = value;
            }
        }

        private MissionGiverType _type = MissionGiverType.NPC;

        [NotifyParentProperty(true)]
        public Vector3 Position
        {
            get
            {
                return _type == MissionGiverType.NPC
                    ? _position 
                    : Vector3.Empty;
            }
            set
            {
                if(Type == MissionGiverType.NPC)
                    _position = value;
            }
        }

        private Vector3 _position = Vector3.Empty;

        [NotifyParentProperty(true)]
        public double Distance
        {
            get
            {
                if (_type == MissionGiverType.NPC)
                    return _position.IsValid ? _position.Distance3DFromPlayer : 0;
                return double.MaxValue;
            }
        }

        [NotifyParentProperty(true)]
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
        [NotifyParentProperty(true)]
        public string MapName
        {
            get
            {
                return _type == MissionGiverType.NPC
                    ? _mapName : string.Empty;
            }
            set
            {
                _mapName = value;
                _label = string.Empty;
            }
        }
        private string _mapName = string.Empty;

        [Editor(typeof(CurrentRegionEdit), typeof(UITypeEditor))]
        [NotifyParentProperty(true)]
        public string RegionName
        {
            get
            {
                return _type == MissionGiverType.NPC
                    ? _regionName : string.Empty;
            }
            set
            {
                _regionName = value;
                _label = string.Empty;
            }
        }
        private string _regionName = string.Empty;

#if DEVELOPER
        [Description("The allowed deviation of the NPC from the specified Position. The minimum value is 1.")]
#else
        [Browsable(false)]
#endif
        [NotifyParentProperty(true)]
        public uint Tolerance
        {
            get
            {
                
                return _type == MissionGiverType.NPC 
                    ? _tolerance : uint.MaxValue;
            }

            set
            {
                value = Math.Max(value, 1);
                if (_type != MissionGiverType.NPC || _tolerance == value) return;
                _tolerance = value;
            }
        }

        private uint _tolerance = 1;

        /// <summary>
        /// <paramref name="entity"/> соответствует патаметрам квестодателя
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsMatching(Entity entity)
        {
            if (_type != MissionGiverType.NPC)
                return false;

            if (entity is null
                || !entity.IsValid
                || string.IsNullOrEmpty(_id))
                return false;

            if (entity.Location.Distance3D(_position) > _tolerance
                || !(entity.CostumeRef.CostumeName.Equals(_id, StringComparison.Ordinal) || entity.InternalName.Equals(_id, StringComparison.Ordinal)))
                return false;

            if (string.IsNullOrEmpty(_mapName))
                return true;

            var player = EntityManager.LocalPlayer;
            return _mapName.Equals(player.MapState.MapName, StringComparison.Ordinal) && _regionName.Equals(player.RegionInternalName, StringComparison.Ordinal);
        }
        /// <summary>
        /// Квестодатель задан корректно
        /// </summary>
        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                return (_type == MissionGiverType.Remote 
                            || (_type == MissionGiverType.NPC && _position.IsValid))
                       && !string.IsNullOrEmpty(_id);
            }
        }
        /// <summary>
        /// Квестодатель зада корректно и к нему можно переместиться для взаимодействия,
        /// то есть персонаж находится на нужной карте и в том же регионе, что и квестодатель
        /// </summary>
        [Browsable(false)]
        public bool IsAccessible
        {
            get
            {
                if (_type == MissionGiverType.Remote)
                    return !string.IsNullOrEmpty(_id);

                var player = EntityManager.LocalPlayer;
                return _position != null && _position.IsValid
                       && !string.IsNullOrEmpty(_id)
                       && (string.IsNullOrEmpty(_mapName) 
                           || _mapName.Equals(player.MapState.MapName, StringComparison.Ordinal) 
                               && _regionName.Equals(player.RegionInternalName, StringComparison.Ordinal));
            }
        }

        string _label = string.Empty;
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_id))
            {
                if (_type == MissionGiverType.Remote)
                    _label = _id;
                else if(!Position.IsValid)
                     _label = "Not set";
                else if (string.IsNullOrEmpty(_mapName))
                    _label = _id;
                else if (string.IsNullOrEmpty(_regionName))
                    _label = string.Concat(_id, " (", _mapName, ')');
                else _label = string.Concat(_id, " (", _mapName, '/', _regionName, ')');
            }
            else _label = "Not set";
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
                        case nameof(Type):
                            var strGiverType = reader.ReadElementContentAsString(elemName, "");
                            _type = Enum.TryParse(strGiverType, out MissionGiverType giverType) 
                                ? giverType : MissionGiverType.NPC;
                            break;
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
                                    _position = Vector3.Empty;
                                using (XmlReader subtreeReader = reader.ReadSubtree())
                                    _position.ReadXml(subtreeReader);

                                if (reader.NodeType == XmlNodeType.EndElement
                                    && reader.Name == nameof(Position))
                                    reader.ReadEndElement();
                            }
                            catch (XmlException except)
                            {
                                _position = Vector3.Empty;
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
            writer.WriteElementString(nameof(Type), _type.ToString());
            writer.WriteElementString(nameof(Id), _id);
            if (_type == MissionGiverType.NPC)
            {
                if (!string.IsNullOrEmpty(_mapName))
                    writer.WriteElementString(nameof(MapName), _mapName);
                if (!string.IsNullOrEmpty(_regionName))
                    writer.WriteElementString(nameof(RegionName), _regionName);
                if (_position.IsValid)
                {
                    writer.WriteStartElement(nameof(Position), "");
                    _position.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
        }
        #endregion
    }
}
