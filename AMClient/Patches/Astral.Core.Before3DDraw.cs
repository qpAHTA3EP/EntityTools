using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using AcTp0Tools.Reflection;
using Astral;
using HarmonyLib;

namespace AcTp0Tools.Patches
{
    /// <summary>
    /// Попытка перехватить исключение <see cref="NullReferenceException "/> в 
    /// MyNW.Internals.WorldDrawings
    /// </summary>
    public static class Astral_Core_Before3DDraw
    {
        private const string TypeWrapperName = "\u0001.\u0002";
        private static Type tBefore3DDrawWrapper;

        private static MethodInfo original_Before3DDraw;
        private static MethodInfo finalizer;


        // ReSharper disable once InconsistentNaming
        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
#if false
                Logger.WriteLine(Logger.LogType.Debug, $"Patch '{nameof(Astra_Core_Before3DDraw)}' catch an exception:\n{__exception}\n{__exception.StackTrace}");
#else
            {
                StringBuilder sb =
                    new StringBuilder($"Patch '{nameof(Astral_Core_Before3DDraw)}' catch an exception:\n");
                foreach (var c in __exception.Message)
                {
                    if (c == '\n' 
                        || c == '\r'
                        || c == '\t'
                        || c == ' '
                        || char.GetUnicodeCategory(c) == UnicodeCategory.LineSeparator
                        || !char.IsControl(c))
                        sb.Append(c);
                    else sb.AppendFormat(@"\u{0:X4}", c);
                }
            }
#endif
            return null;
        }

        internal static void ApplyPatches()
        {
            tBefore3DDrawWrapper = ReflectionHelper.GetTypeByName(TypeWrapperName, true);

            if (tBefore3DDrawWrapper is null)
            {
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch of the '{nameof(Astral_Core_Before3DDraw)}' failed. Type '{TypeWrapperName}' not found");
                return;
            }

            var tPatch = typeof(Astral_Core_Before3DDraw);

            finalizer = AccessTools.Method(tPatch, nameof(Finalizer));
            if (finalizer is null)
            {
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch '{nameof(Astral_Core_Before3DDraw)}' failed. Method '{nameof(Finalizer)}' not found");
                return;
            }

            original_Before3DDraw = AccessTools.Method(tBefore3DDrawWrapper, "\u0001");
            if (original_Before3DDraw is null)
            {
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch '{nameof(Astral_Core_Before3DDraw)}' failed. Method method not found.");
                return;
            }

            Action unPatch = null;
            try
            {
                var harPatch = new HarmonyMethod(finalizer);
                unPatch = () =>
                {
                    Logger.WriteLine(Logger.LogType.Debug, $@"Unpatch '{nameof(Astral_Core_Before3DDraw)}'");
                    AcTp0Patcher.Harmony.Unpatch(original_Before3DDraw, finalizer);
                };
                AcTp0Patcher.Harmony.Patch(original_Before3DDraw, null, null, null, harPatch);
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch '{nameof(Astral_Core_Before3DDraw)}' succeeded.");
            }
            catch (Exception e)
            {
                unPatch?.Invoke();
                Logger.WriteLine(Logger.LogType.Debug, $"Patch '{nameof(Astral_Core_Before3DDraw)}' failed.\n{e}");
                throw;
            }
        }
    }
}
