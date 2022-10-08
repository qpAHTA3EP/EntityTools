using ACTP0Tools.Reflection;
using MyNW.Classes;
using System;

namespace EntityTools.Tools
{
    /// <summary>
    /// краткое описание объекта Entity
    /// </summary>
    public class EntityExportInfo
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
