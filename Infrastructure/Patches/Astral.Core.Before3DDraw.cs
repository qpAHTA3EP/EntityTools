using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using Astral;
using Astral.Classes;
using HarmonyLib;
using Infrastructure.Reflection;

namespace Infrastructure.Patches
{
    /// <summary>
    /// Попытка перехватить исключение <see cref="NullReferenceException "/> в 
    /// MyNW.Internals.WorldDrawings
    /// </summary>
    public static class Astral_Core_Before3DDraw
    {
        private static Timeout timeout = new Timeout(0);
        private static StringBuilder errorBuffer = new StringBuilder();
        private static int skippedErrorCount;

        private const string TypeWrapperName = "\u0001.\u0002";
        private static Type tBefore3DDrawWrapper;

        private static MethodInfo original_Before3DDraw;
        private static MethodInfo finalizer;

        private static bool Patched = false;

        // ReSharper disable once InconsistentNaming
        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
#if false
                Logger.WriteLine(Logger.LogType.Debug, $"Patch '{nameof(Astra_Core_Before3DDraw)}' catch an exception:\n{__exception}\n{__exception.StackTrace}");
#else
            {
                if (timeout.IsTimedOut)
                {
                    errorBuffer.AppendLine($"Patch '{nameof(Astral_Core_Before3DDraw)}' catch an exception:\n");
                    foreach (var c in __exception.Message)
                    {
                        if (c == '\n'
                            || c == '\r'
                            || c == '\t'
                            || c == ' '
                            || char.GetUnicodeCategory(c) == UnicodeCategory.LineSeparator
                            || !char.IsControl(c))
                            errorBuffer.Append(c);
                        else errorBuffer.AppendFormat(@"\u{0:X4}", c);
                    }

                    errorBuffer.AppendLine($"Another {skippedErrorCount} exceptions were suppressed");

                    Logger.WriteLine(Logger.LogType.Debug, errorBuffer.ToString());
                    errorBuffer.Clear();
                    skippedErrorCount = 0;
                    timeout.ChangeTime(2000);
                }
                else skippedErrorCount++;
            }
#endif
            return null;
        }

        internal static void ApplyPatches()
        {
            if (!Patched)
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
                    //unPatch = () =>
                    //{
                    //    Logger.WriteLine(Logger.LogType.Debug, $@"Unpatch '{nameof(Astral_Core_Before3DDraw)}'");
                    //    ACTP0Patcher.Harmony.Unpatch(original_Before3DDraw, finalizer);
                    //};
                    ACTP0Patcher.Harmony.Patch(original_Before3DDraw, null, null, null, harPatch);
                    Logger.WriteLine(Logger.LogType.Debug, $@"Patch '{nameof(Astral_Core_Before3DDraw)}' succeeded.");

                    Patched = true;
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
}
