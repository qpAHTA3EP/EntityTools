using System;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;
using EntityTools.Reflection;

namespace EntityTools.Patches.UCC
{
#if PATCH_ASTRAL
    internal class Patch_AddClass_Show : Patch                
    {
        internal Patch_AddClass_Show() : base(typeof(AddClass).GetMethod(nameof(AddClass.Show), ReflectionHelper.DefaultFlags, null, new[] { typeof(Type) }, null),
                                              typeof(Patch_AddClass_Show).GetMethod(nameof(Show), ReflectionHelper.DefaultFlags))
        { }

        internal static object Show(Type type)
        {
            EntityTools.Core.GUIRequest_UCCAction(out UCCAction action);

            return action;
        }
    }
#endif
}
