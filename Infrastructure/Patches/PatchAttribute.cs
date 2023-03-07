using System;
using HarmonyLib;

namespace Infrastructure.Patches
{
    /// <summary>
    /// Атрибут, помечающий статический класс-патч,
    /// обязательным членом которого должен быть метод <see cref="Apply(Harmony)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PatchAttribute : Attribute
    {
        public PatchAttribute()
        {
        }
    }
}
