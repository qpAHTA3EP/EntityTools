using System;
using System.Xml.Serialization;
using AcTp0Tools.Reflection;
using MyNW.Classes;

namespace EntityTools.Tools
{
    /// <summary>
    /// краткое описание объекта Entity
    /// </summary>
    public class EntityInfo
    {
        public Entity entity = Empty.Entity;
        public string Name = string.Empty;
        public string NameUntranslated = string.Empty;
        public string InternalName = string.Empty;
        public double Distance = 0;

        public bool IsValid
        {
            get
            {
                return entity.Pointer != IntPtr.Zero;
            }
        }
    }
}
