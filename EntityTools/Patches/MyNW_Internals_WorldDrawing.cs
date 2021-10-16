using HarmonyLib;
using System;
using System.Reflection;
using AcTp0Tools.Patches;

namespace EntityTools.Patches
{
    public static class MyNW_Internals_WorldDrawing
    {
        private static MethodInfo original_Drawing;
        private static MethodInfo finalizer;


        // ReSharper disable once InconsistentNaming
        static Exception Finalizer(Exception __exception)
        {
            ETLogger.WriteLine(LogType.Error, $@"{nameof(MyNW.Internals.WorldDrawing)} exception.", true);
            ETLogger.WriteLine(LogType.Error, __exception.ToString(), true);
            return null;
        }

        internal static void ApplyPatches()
        {
            var tWorldDrawing = typeof(MyNW.Internals.WorldDrawing);
            var tPatch = typeof(MyNW_Internals_WorldDrawing);

            // Патчить статический конструктор нельзя
            // https://harmony.pardeike.net/articles/patching-edgecases.html#static-constructors

            finalizer = AccessTools.Method(tPatch, nameof(Finalizer));
            if (finalizer is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch of the '{tWorldDrawing}' failed. Method '{finalizer}' not found", true);
                return;
            }

            original_Drawing = AccessTools.Method(tWorldDrawing, "\u0001");
            if (original_Drawing is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch of the '{tWorldDrawing}' failed. Method '{original_Drawing}' method not found.", true);
                return;
            }

            Action unPatch = null;
            try
            {
                var harPatch = new HarmonyMethod(finalizer);
                AcTp0Patcher.Harmony.Patch(original_Drawing, null, null, null, harPatch);
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_Drawing}'", true);
                    AcTp0Patcher.Harmony.Unpatch(original_Drawing, finalizer);
                };

                ETLogger.WriteLine($@"Patch of the '{tWorldDrawing}' succeeded.", true);
            }
            catch (Exception e)
            {
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, $@"Patch of the '{tWorldDrawing}' failed.", true);
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
                throw;
            }
        }
    }
}
