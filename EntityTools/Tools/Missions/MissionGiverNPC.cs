using Astral.Quester.UIEditors;
using EntityTools.Enums;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Tools.Missions
{
    public class MissionGiverNPC : MissionGiverBase
    {
        public override MissionGiverType GiverType => MissionGiverType.NPC;

        public override Vector3 Position { get => _position; set => _position = value; }
        private Vector3 _position = Vector3.Empty;

        public override double Distance => _position.IsValid ? _position.Distance3DFromPlayer : 0;

        public override string Id
        {
            get => _id;
            set
            {
                _id = value;
                _label = string.Empty;
            }
        }

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
            get => _regionName;
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
        public uint GiverTolerance
        {
            get => _giverTolerance;
            set
            {
                value = Math.Max(value, 1);
                _giverTolerance = value;
            }
        }

        private uint _giverTolerance = 1;

        public override bool IsValid
        {
            get
            {
                if (!_position.IsValid
                    || string.IsNullOrEmpty(Id))
                    return false;

                var player = EntityManager.LocalPlayer;

                return string.IsNullOrEmpty(MapName)
                    || MapName.Equals(player.MapState.MapName) && RegionName.Equals(player.RegionInternalName);
            }
        }

        public override bool IsAccessible
        {
            get
            {
                var player = EntityManager.LocalPlayer;
                return _position.IsValid
                       && !string.IsNullOrEmpty(_id)
                       && (string.IsNullOrEmpty(_mapName) || _mapName.Equals(player.MapState.MapName) && _regionName.Equals(player.RegionInternalName));
            }
        }

        public override bool IsMatching(Entity entity)
        {
            if (string.IsNullOrEmpty(Id)
                || entity is null
                || !entity.IsValid 
                || entity.CostumeRef.CostumeName != _id)
                return false;
            return entity.Location.Distance3D(_position) <= _giverTolerance;
        }

        private string _label = string.Empty;
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (string.IsNullOrEmpty(_id))
                    _label = "Not set";

                if (_position.IsValid)
                {
                    if (string.IsNullOrEmpty(_mapName))
                        _label = _id;
                    else if (string.IsNullOrEmpty(_regionName))
                        _label = string.Concat(_id, " (", _mapName, ")");
                    else _label = string.Concat(_id, " (", _mapName, "/", _regionName, ")");
                } 
            }

            return _label;
        }
        
        #region IXmlSerializable
#if false
        public override void ReadXml(XmlReader reader)
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
                    if (elemName == nameof(Id)
                        || elemName == nameof(NPCInfos.CostumeName))
                        _id = reader.ReadElementContentAsString(elemName, "");
                    else if (elemName == nameof(MapName))
                        _mapName = reader.ReadElementContentAsString(elemName, "");
                    else if (elemName == nameof(RegionName))
                        _regionName = reader.ReadElementContentAsString(elemName, "");
#if false
                    else if (elemName == nameof(GiverType))
                    {
                        string giverStr = reader.ReadElementContentAsString(elemName, "");
                        if (Enum.TryParse(giverStr, out MissionGiverType gType))
                            _giverType = gType;
                        else
                        {
                            _giverType = MissionGiverType.None;
                            ETLogger.WriteLine(LogType.Error, $"{MethodBase.GetCurrentMethod().Name} failed to parse '{elemName}'", true);
                        }
                    } 
#endif
                    else if (elemName == nameof(Position))
                    {
                        try
                        {
                            if (_position is null)
                                _position = Vector3.Empty;
                            using (XmlReader subtreeReader = reader.ReadSubtree())
                            {
                                _position.ReadXml(subtreeReader);
                            }
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

#if false
            if (_giverType == MissionGiverType.None
        && _position.IsValid
        && !string.IsNullOrEmpty(_id))
                _giverType = MissionGiverType.NPC; 
#endif
        }


        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(Id), _id);
            writer.WriteElementString(nameof(GiverType), MissionGiverType.NPC.ToString());
            writer.WriteElementString(nameof(MapName), _mapName);
            writer.WriteElementString(nameof(RegionName), _regionName);
            writer.WriteStartElement(nameof(Position), "");
            _position.WriteXml(writer);
            writer.WriteEndElement();
        } 
#endif
        #endregion
    }
}
