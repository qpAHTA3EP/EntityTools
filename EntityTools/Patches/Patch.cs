using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Infrastructure;

namespace EntityTools.Patches
{
    internal abstract class Patch
    {
        protected MethodInfo methodToReplace;
        protected MethodInfo methodToInject;

        public Patch()
        {
            methodToReplace = null;
            methodToInject = null;
        }
        public Patch(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            this.methodToReplace = methodToReplace;
            this.methodToInject = methodToInject;
        }

        public abstract bool NeedInjection { get; }

        public void Inject()
        {
            if (NeedInjection)
            {
                if (methodToReplace != null
                  && methodToInject != null)
                {
                    RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
                    RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

                    unsafe
                    {
                        long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                        long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;

                        *tar = *inj;
                    }
                    ETLogger.WriteLine($"Patch of the '{methodToReplace.ReflectedType.Name}.{methodToReplace.Name}' succeeded!", true);
                }
                else
                {
                    string msg = string.Concat("Fail to inject:", Environment.NewLine,
                                                (methodToReplace == null) ? "MethodToReplace: NULL" : $"MethodToReplace: {methodToReplace.Name}", Environment.NewLine,
                                                (methodToInject == null) ? "MethodToInject: NULL" : $"MethodToInject: {methodToInject.Name}");
                    ETLogger.WriteLine(LogType.Error, msg, true);
                }
            }
        }
    }
}
