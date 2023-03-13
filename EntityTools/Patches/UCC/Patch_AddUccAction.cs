using System;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;
using EntityTools.Forms;
using Infrastructure.Reflection;

namespace EntityTools.Patches.UCC
{
    internal class Patch_AddUccAction : Patch                
    {
        internal Patch_AddUccAction()
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
}
