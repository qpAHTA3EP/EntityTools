using Astral;
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

                Logger.WriteLine("Fail to inject:");
                if(methodToReplace == null)
                    Logger.WriteLine($"MethodToReplace: NULL");
                else Logger.WriteLine($"MethodToReplace: {methodToReplace.Name}");
                if (methodToInject== null)
                    Logger.WriteLine($"MethodToInject: NULL");
                else Logger.WriteLine($"MethodToInject: {methodToInject.Name}");
            }
        }
    }
}
