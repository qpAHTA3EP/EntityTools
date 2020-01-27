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
            throw new NotImplementedException();
            //RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            //RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            //unsafe
            //{
            //    long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
            //    long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;

            //    *tar = *inj;
            //}
        }
    }
}
