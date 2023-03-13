using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Infrastructure.Patches;

// ReSharper disable RedundantAssignment

namespace Infrastructure.Reflection.Tests
{
    public class StaticMethodPatchTester
    {
        private readonly Stopwatch sw = new Stopwatch();

        public string Test()
        {
            sw.Start();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = TestTarget.Method1(i);
            }
            sw.Stop();

            var traverseMs = sw.ElapsedMilliseconds;
            var traverseTicks = sw.ElapsedTicks;

            sw.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = TestTarget.Method2(i);
            }
            sw.Stop();

            var accessorMs = sw.ElapsedMilliseconds;
            var accessorTicks = sw.ElapsedTicks;

            sw.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = TestTarget.Method3(i);
            }
            sw.Stop();

            var directMs = sw.ElapsedMilliseconds;
            var directTicks = sw.ElapsedTicks;

            return string.Format("{0,-8} : {1,10:0} {2,10:0}\n" +
                                 "{3,-8} : {4,10:0} {5,10:0}\n" +
                                 "{6,-8} : {7,10:0} {8,10:0}",
                                 "Traverse", traverseMs, traverseTicks,
                                 "Accessor", accessorMs, accessorTicks,
                                 "Direct", directMs, directTicks);
        }
    }

    public static class TestTarget
    {
        private static readonly string str = nameof(TestTarget);
        private static int number;

        public static string Method1(int i)
        {
            number += i;
            return str + number;
        }
        public static string Method2(int i)
        {
            number += i;
            return str + number;
        }
        public static string Method3(int i)
        {
            number = i;
            return str + number;
        }
        public static string Method4(int i)
        {
            number += i;
            return str + number;
        }
        public static string Method5(int i)
        {
            number += i;
            return str + number;
        }
    }

    public static class TestPatch
    {
        private static readonly Type target;
        private static readonly MethodInfo originalMethod;
        private static readonly MethodInfo patchMethod;

        private static string str = nameof(TestPatch);
        private static int number;

        static TestPatch()
        {
            target = typeof(TestTarget);

            originalMethod = AccessTools.Method(target, nameof(TestTarget.Method2));
            patchMethod = AccessTools.Method(typeof(TestPatch), nameof(Patch_Method));
        }

        static void Inject()
        {
            if (originalMethod != null
                && patchMethod != null)
            {
                RuntimeHelpers.PrepareMethod(originalMethod.MethodHandle);
                RuntimeHelpers.PrepareMethod(patchMethod.MethodHandle);

                unsafe
                {
                    long* inj = (long*)patchMethod.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)originalMethod.MethodHandle.Value.ToPointer() + 1;

                    *tar = *inj;
                }
            }
        }

        public static string Patch_Method(int i)
        {
            number = i;
            return str + number;
        }
    }

    public static class HarmonyTestPatch
    {
        private static readonly Type target;
        private static readonly MethodInfo originalMethod;
        private static readonly MethodInfo patchMethod;

        private static string str = nameof(HarmonyTestPatch);
        private static int number;

        static HarmonyTestPatch()
        {
            target = typeof(TestTarget);

            originalMethod = AccessTools.Method(target, nameof(TestTarget.Method1));
            patchMethod = AccessTools.Method(typeof(HarmonyTestPatch), nameof(HarmonyPatch_Method));

            ACTP0Patcher.Harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));
        }

        public static bool HarmonyPatch_Method(ref string __result, int i)
        {
            number = i;
            __result = str + number;
            return false;
        }
    }
}
