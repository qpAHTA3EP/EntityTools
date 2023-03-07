using System;
using System.ComponentModel;
using System.Reflection;
using HarmonyLib;
using Infrastructure.Reflection;

namespace Infrastructure.Patches
{
    public static class ACTP0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(Infrastructure));

        public static void Apply()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            Harmony.PatchAll();

            PatchAll();

            //TODO заменить на алгоритм перебора вложенных типов
            //AstralAccessors.Quester.Core.ApplyPatches();
            ACTP0Serializer.ApplyPatches();
            Astral_Core_Before3DDraw.ApplyPatches();
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;
            if (name.StartsWith(nameof(Infrastructure))
                || name.StartsWith("0Harmony"))
                return typeof(ACTP0Patcher).Assembly;
            return null;
        }

        // <summary>
        /// Сканирование всех загруженных сборок и активация патчей, помеченных атрибутом <see cref="PatchAttribute"/>.
        /// </summary>
        public static void PatchAll()
        {
            var patchAttribute = new PatchAttribute();
            var harmonyParam = new object[] { Harmony };
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!TypeDescriptor.GetAttributes(type).Contains(patchAttribute))
                        continue;

                    try
                    {
                        ETLogger.WriteLine($"Applying patch '{type.Name}'.", true);

                        ReflectionHelper.ExecStaticMethod(
                            type,
                            "Apply",
                            harmonyParam,
                            out object result
                        );

                        if (Equals(result, true))
                            ETLogger.WriteLine($"Patch '{type.Name}' applied successfully.", true);
                        else ETLogger.WriteLine($"Patch '{type.Name}' failed.", true);
                    }
                    catch (Exception e)
                    {
                        ETLogger.WriteLine($"Catch an exception doing patch '{type.Name}'.\n" +
                                           e, true);

                        throw;
                    }
                }
            }
        }
    }
}
