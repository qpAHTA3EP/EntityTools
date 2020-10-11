﻿using System;
using MyNW.Classes;

namespace EntityTools.Tools
{
    /// <summary>
    /// краткое описание объекта Entity
    /// </summary>
    public class EntityDef
    {
        public Entity entity = new Entity(IntPtr.Zero);
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
