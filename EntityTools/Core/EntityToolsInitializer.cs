using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Core
{
    internal static class Initializer
    {
        internal static Return Initialize<Return>(ref Func<Return> method)
        {
            if (EntityTools.Core.Initialize(method.Target))
                return method();
            else return default(Return);
        }
        internal static void Initialize(ref System.Action method)
        {
            if (EntityTools.Core.Initialize(method.Target))
                    method();
        }

    }
}
