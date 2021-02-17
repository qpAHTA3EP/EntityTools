using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EntityTools.Tools.Extensions
{
    public static class Vector3Extentions
    {
        public static void ReadXml(this Vector3 vector, XmlReader reader)
        {
            if (reader.ReadState == ReadState.Initial)
                reader.Read();
            string startElemName = reader.Name;

            while (reader.ReadState == ReadState.Interactive)
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.IsStartElement())
                {
                    string coordName = reader.Name;
                    if (coordName == nameof(vector.X))
                        vector.X = reader.ReadElementContentAsFloat(coordName, "");
                    else if (coordName == nameof(vector.Y))
                        vector.Y = reader.ReadElementContentAsFloat(coordName, "");
                    else if (coordName == nameof(vector.Z))
                        vector.Z = reader.ReadElementContentAsFloat(coordName, "");
                    else reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement
                    && reader.Name == startElemName)
                {
                    reader.ReadEndElement();
                    break;
                }
                else reader.Read();
            }
        }

        public static void WriteXml(this Vector3 vector, XmlWriter writer)
        {
            writer.WriteElementString(nameof(vector.X), vector.X.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString(nameof(vector.Y), vector.Y.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString(nameof(vector.Z), vector.Z.ToString(CultureInfo.InvariantCulture));
        }
    }
}
