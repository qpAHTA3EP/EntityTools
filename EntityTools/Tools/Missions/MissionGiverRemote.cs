using EntityTools.Enums;
using EntityTools.Tools.Missions;
using MyNW.Classes;

namespace EntityTools.Tools.Missions
{
    public class MissionGiverRemote : MissionGiverBase
    {
        public override MissionGiverType GiverType => MissionGiverType.Remote; 

        public override bool IsValid => !string.IsNullOrEmpty(_id);

        public override bool IsAccessible() => true;

        public override bool IsMatching(Entity entity) => false;

        public override string ToString()
        {
            return string.IsNullOrEmpty(_id) ? "Not set" : _id;
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


        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(nameof(GiverType), MissionGiverType.Remote.ToString());
            writer.WriteElementString(nameof(Id), _id);
        } 
#endif
        #endregion
    }
}
