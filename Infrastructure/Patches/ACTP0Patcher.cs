using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using DiffMatchPatch;
using HarmonyLib;
using Infrastructure.Reflection;
using Mono.Cecil;

namespace Infrastructure.Patches
{
    public static class ACTP0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(Infrastructure));

        private static bool Patched = false;

        public static void Apply()
        {
            if (!Patched)
            {
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

                Harmony.PatchAll();

                // Вызвает исключение
                // Не удается загрузить один или более запрошенных типов. Обратитесь к свойству LoaderExceptions для получения дополнительных сведений.
                //  в System.Reflection.RuntimeModule.GetTypes(RuntimeModule module)
                //  в System.Reflection.RuntimeModule.GetTypes()
                //  в System.Reflection.Assembly.GetTypes()
                //  в Infrastructure.Patches.ACTP0Patcher.PatchAll() в D:\Source\EntityTools\Infrastructure\Patches\ACTP0Patcher.cs:строка 52
                //  в Infrastructure.Patches.ACTP0Patcher.Apply() в D:\Source\EntityTools\Infrastructure\Patches\ACTP0Patcher.cs:строка 23
                //  в EntityTools.Patches.ETPatcher.Apply() в D:\Source\EntityTools\EntityTools\Patches\Patcher.cs:строка 35
                //  в EntityTools.EntityTools.OnLoad() в D:\Source\EntityTools\EntityTools\Core\EntityPlugin.cs:строка 79
                //  в Astral.Controllers.Plugins.Initialize()
                //PatchAll();

                AstralAccessors.Quester.Core.Apply(Harmony);
                ACTP0Serializer.ApplyPatches();
                Astral_Core_Before3DDraw.ApplyPatches(); 

                Patched = true;
            }
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
            try
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
            catch (ReflectionTypeLoadException e)
            {
                var strBldr = new StringBuilder("Catch 'TypeLoadException'.\n");
                strBldr.AppendLine(e.ToString());
                if (e.LoaderExceptions?.Length > 0)
                {
                    strBldr.AppendLine("LoaderExceptions:");
                    foreach(var exc in e.LoaderExceptions)
                    {
                        strBldr.AppendLine(exc.ToString());
                    }
                }
                ETLogger.WriteLine(strBldr.ToString(), true);
            }
        }
    }
}
