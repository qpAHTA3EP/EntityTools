using Astral.Logic.UCC.Classes;
using EntityTools.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Patches.UCC
{
#if DEVELOPER
    internal class Patch_AddClass_Show : Patch                
    {
        internal Patch_AddClass_Show() : base(typeof(Astral.Logic.UCC.Forms.AddClass).GetMethod(nameof(Astral.Logic.UCC.Forms.AddClass.Show), ReflectionHelper.DefaultFlags, null, new Type[] { typeof(Type) }, null),
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
