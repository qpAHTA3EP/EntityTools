using System;

using ACTP0Tools.Reflection;

using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;

using EntityTools.Forms;

namespace EntityTools.Patches.UCC
{
#if PATCH_ASTRAL
    internal class Patch_Logic_UCC_Forms_AddClass_Show : Patch                
    {
        internal Patch_Logic_UCC_Forms_AddClass_Show()
        {
            if (NeedInjection)
            {
                methodToReplace = typeof(AddClass).GetMethod(nameof(AddClass.Show), ReflectionHelper.DefaultFlags, null,
                    new[] {typeof(Type)},
                    null);

                methodToInject = GetType().GetMethod(nameof(Show), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjection => true;

        internal static object Show(Type type)
        {
            return AddUccActionForm.GUIRequest(out UCCAction action)
                 ? action
                 : null;
        }
    }
#endif
}
