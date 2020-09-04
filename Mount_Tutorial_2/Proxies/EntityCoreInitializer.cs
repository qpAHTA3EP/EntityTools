using EntityTools.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mount_Tutorial.Proxies
{
    internal static class EntityCoreInitiazlizer
    {
        internal static Return Initialize<Return>(ref Func<Return> method)
        {
            if (MountTutorialLoaderPlugin.Core.Initialize(method.Target))
                return method();
            else return default(Return);
        }
        internal static void Initialize(ref System.Action method)
        {
            if (MountTutorialLoaderPlugin.Core.Initialize(method.Target))
                method();
        }
        internal static void Initialize<ArgumentType>(ref System.Action<ArgumentType> method, ArgumentType argument)
        {
            if (MountTutorialLoaderPlugin.Core.Initialize(method.Target))
                method(argument);
        }
    }
}
