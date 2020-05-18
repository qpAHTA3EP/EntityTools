using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;
using System;
using QuesterCondition = Astral.Quester.Classes.Condition;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using EntityTools.Reflection;

namespace EntityTools.UCC.Conditions
{
    public class UCCQuesterCheck : UCCCondition, ICustomUCCCondition
    {
        static readonly Func<int, List<Type>> GetExtraTypes = typeof(Astral.Functions.XmlSerializer).GetStaticFunction<int, List<Type>>("GetExtraTypes", ReflectionHelper.DefaultFlags);

#if DEVELOPER
        internal class QuesterConditionEditor : AddTypeCommonEditor<QuesterCondition>
        { }

        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        [XmlIgnore]
        public QuesterCondition Condition
        {
            get
            {
                if (_condition != null)
                    return _condition;
                else if (string.IsNullOrEmpty(ConditionXml))
                    return null;
                else
                {
                    // десериализация 
                    string xml = ConditionXml;
                    XmlSerializer serializer = new XmlSerializer(typeof(QuesterCondition), GetExtraTypes(2).ToArray());
                    using (StringReader reader = new StringReader(xml))
                    {
                        return _condition = serializer.Deserialize(reader) as QuesterCondition;
                    }
                }
            }
            set => _condition = value;
        }
        private QuesterCondition _condition;

        [Browsable(false)]
        public string ConditionXml
        {
            get
            {
                if (_condition != null)
                {
                    XmlSerializer serializer = new XmlSerializer(_condition.GetType(), GetExtraTypes(2).ToArray());
                    using (StringWriter writer = new StringWriter())
                    {
                        serializer.Serialize(writer, _condition);
                        return writer.ToString();
                    }
                }
                else return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _condition = null;
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(QuesterCondition), GetExtraTypes(2).ToArray());
                    using (StringReader reader = new StringReader(value))
                    {
                        _condition = serializer.Deserialize(reader) as QuesterCondition;
                    }
                }
            }
        }

        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            return Condition?.IsValid == true;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (Condition != null)
                return Condition.TestInfos;
            return $"{nameof(Condition)} does not set.";
        }
#endregion

        public override string ToString()
        {
            return GetType().Name;
        }

#region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Sign Sign { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; } = string.Empty;

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
#endregion
    }
}
