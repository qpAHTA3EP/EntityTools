using System;
using System.ComponentModel;
using System.Xml.Serialization;
using EntityTools.Enums;
using MyNW.Classes;

namespace EntityTools.Tools.Missions
{
    [Serializable]
    [XmlInclude(typeof(MissionGiverNPC))]
    [XmlInclude(typeof(MissionGiverRemote))]
    [XmlType()]
    public abstract class MissionGiverBase // : IXmlSerializable
    {
        [Browsable(false)]
        public abstract MissionGiverType GiverType { get; }
#if false
        [Browsable(false)]
        public string CostumeName
        {
            get => string.Empty;
            set => _id = value;
        } 
#endif
        public virtual string Id
        {
            get => _id;
            set => _id = value;
        }
        protected string _id = string.Empty;

        public abstract Vector3 Position { get; set; }

        public abstract double Distance { get; }

        public abstract bool IsAccessible { get; }

        public abstract bool IsMatching(Entity entity);

        public abstract bool IsValid { get; }

        #region IXmlSerializable
#if false
        public XmlSchema GetSchema() => null;

        public abstract void ReadXml(XmlReader reader);

        public abstract void WriteXml(XmlWriter writer); 
#endif
        #endregion
    }
}
