using Astral;
using EntityTools.Logger;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EntityTools.Patches
{
    internal class Patch
    {
        private readonly MethodInfo methodToReplace;
        private readonly MethodInfo methodToInject;

        public Patch(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            this.methodToReplace = methodToReplace;
            this.methodToInject = methodToInject;
        }

        public void Inject()
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
            }
            else
            {

                EntityToolsLogger.WriteLine("Fail to inject:");
                if(methodToReplace == null)
                    EntityToolsLogger.WriteLine($"MethodToReplace: NULL");
                else EntityToolsLogger.WriteLine($"MethodToReplace: {methodToReplace.Name}");
                if (methodToInject== null)
                    EntityToolsLogger.WriteLine($"MethodToInject: NULL");
                else EntityToolsLogger.WriteLine($"MethodToInject: {methodToInject.Name}");
            }
        }
    }
}
