using System;
using System.Reflection;
using Astral;
using HarmonyLib;

namespace AcTp0Tools.Patches
{
    // Патч часто вызывает ошибку чтения памяти игры, по следующей причине:
    // При вызове патча игровой клиент еще не загружен.
    // Однако при "подготовке" метода "MyNW.Internals.WorldDrawing.\u0001"
    // вызывается статический конструктор класса "MyNW.Internals.WorldDrawing"
    // который запускает цикл внутриигровой отрисовки объектов
    public static class MyNW_Internals_WorldDrawing
    {
        private static MethodInfo original_Drawing;
        private static MethodInfo finalizer;


        // ReSharper disable once InconsistentNaming
        static Exception Finalizer(Exception __exception)
        {
            Logger.WriteLine(Logger.LogType.Debug, $@"{nameof(MyNW.Internals.WorldDrawing)} exception.");
            Logger.WriteLine(Logger.LogType.Debug, __exception.ToString());
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
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch of the '{tWorldDrawing}' failed. Method '{finalizer}' not found");
                return;
            }

            original_Drawing = AccessTools.Method(tWorldDrawing, "\u0001");
            if (original_Drawing is null)
            {
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch of the '{tWorldDrawing}' failed. Method '{original_Drawing}' method not found.");
                return;
            }

            Action unPatch = null;
            try
            {
                var harPatch = new HarmonyMethod(finalizer);
                AcTp0Patcher.Harmony.Patch(original_Drawing, null, null, null, harPatch);
                unPatch = () =>
                {
                    Logger.WriteLine(Logger.LogType.Debug, $@"Unpatch method '{original_Drawing}'");
                    AcTp0Patcher.Harmony.Unpatch(original_Drawing, finalizer);
                };

                Logger.WriteLine($@"Patch of the '{tWorldDrawing}' succeeded.");
            }
            catch (Exception e)
            {
                unPatch?.Invoke();
                Logger.WriteLine(Logger.LogType.Debug, $@"Patch of the '{tWorldDrawing}' failed.");
                Logger.WriteLine(Logger.LogType.Debug, e.ToString());
                throw;
            }
        }
    }
}
