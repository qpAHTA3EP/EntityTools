using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using AStar;
using Astral;
using Astral.Quester.Classes;
using HarmonyLib;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment
// ReSharper disable RedundantNameQualifier

namespace ACTP0Tools
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static partial class Quester
        {
            public static class Core
            {
                #region Meshes
                private static bool PrefixGetMeshes(ref Graph __result)
                {
                    __result = CurrentProfile.CurrentMesh;

                    return false;
                }

                // TODO Использовать RWLocker для синхронизации доступа к _mapsMeshes
                //private static readonly AStar.Tools.RWLocker mapsMeshesLocker = new RWLocker();

                private static bool PrefixGetMapsMeshes(ref Dictionary<string, Graph> __result)
                {
                    __result = CurrentProfile.MapsMeshes;
                    return false;
                }
                private static bool PrefixSetMapsMeshes(Dictionary<string, Graph> value)
                {
                    CurrentProfile.MapsMeshes = value;
                    return false;
                }

                internal static string CurrentProfileZipMeshFile
                {
                    get
                    {
                        var lastQuesterProfileFileName = Astral.API.CurrentSettings.LastQuesterProfile;
                        var currentProfile = Astral.Quester.API.CurrentProfile;
                        if (lastQuesterProfileFileName.Length > 0
                            && currentProfile.Saved)
                        {
                            string meshesFile;
                            if (currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= 10)
                                meshesFile = Path.Combine(Path.GetDirectoryName(lastQuesterProfileFileName) ?? string.Empty,
                                                          currentProfile.ExternalMeshFileName);
                            else meshesFile = lastQuesterProfileFileName;
                            if (File.Exists(meshesFile))
                            {
                                return meshesFile;
                            }
                        }
                        return string.Empty;
                    }
                }

                private static bool PrefixLoadAllMeshes(out int __result)
                {
                    __result = QuesterHelper.LoadAllMeshes(CurrentProfile.MapsMeshes, CurrentProfileZipMeshFile);
                    return false;
                }
                #endregion

                #region Profile
                private static readonly MethodInfo engineSetProfile;

                /// <summary>
                /// Прокси-объект, опосредующий доступ к активному quester-профилю, выполняемому в роли Quester
                /// </summary>
                public static readonly BaseQuesterProfileProxy CurrentProfile;

                /// <summary>
                /// препатч <seealso cref="Astral.Quester.Core.Load(string, bool)"/>
                /// </summary>
                internal static bool PrefixLoad(string Path, bool savePath = true)
                {
                    CurrentProfile.LoadFromFile(Path);

                    return false;
                }

                internal static bool PrefixSave(bool saveas = false)
                {
                    if (saveas)
                        CurrentProfile.SaveAs();
                    else CurrentProfile.Save();
                    return false;
                }
                #endregion

                #region Events
                public delegate void ProfileChangedEvent();
                /// <summary>
                /// Событие, происходящее после изменение <seealso cref="Astral.Quester.Core.Profile"/>
                /// </summary>
                public static event ProfileChangedEvent OnProfileChanged;

                private static void PostfixSetProfile(Profile value)
                {
                    OnProfileChanged?.Invoke();
                }
                #endregion

                internal static void ApplyPatches() { }

                static Core()
                {
                    // TODO Пропатчить Core.LoadAllMeshes

                    var tCore = typeof(Astral.Quester.Core);
                    var tPatch = typeof(Core);
                    var propProfile = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Profile));
                    engineSetProfile = propProfile.GetSetMethod(true);
                    var postfixSetProfile = AccessTools.Method(tPatch, nameof(PostfixSetProfile));

                    if (engineSetProfile != null
                        && postfixSetProfile != null)
                    {
                        ACTP0Patcher.Harmony.Patch(engineSetProfile, null, new HarmonyMethod(postfixSetProfile));
                    }

                    var propMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Meshes));
                    var originalGetMeshes = propMeshes.GetGetMethod(true);
                    var prefixGetMeshes = AccessTools.Method(tPatch, nameof(Core.PrefixGetMeshes));

                    if (originalGetMeshes != null
                        && prefixGetMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalGetMeshes, new HarmonyMethod(prefixGetMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' failed");

                    var propMapsMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.MapsMeshes));
                    var originalGetMapsMeshes = propMapsMeshes.GetGetMethod(true);
                    var originalSetMapsMeshes = propMapsMeshes.GetSetMethod(true);
                    var prefixGetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixGetMapsMeshes));
                    var prefixSetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixSetMapsMeshes));
                    if (originalGetMapsMeshes != null
                        && prefixGetMapsMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalGetMapsMeshes, new HarmonyMethod(prefixGetMapsMeshes));
                        Logger.WriteLine(Logger.LogType.Debug,
                            $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug,
                        $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    if (originalSetMapsMeshes != null
                        && prefixSetMapsMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalSetMapsMeshes, new HarmonyMethod(prefixSetMapsMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    var originalLoad = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(tPatch, nameof(PrefixLoad));
                    if (originalLoad != null &&
                        prefixLoad != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' failed");

                    var originalSave = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Save));
                    var prefixSave = AccessTools.Method(tPatch, nameof(PrefixSave));
                    if (originalSave != null &&
                        prefixSave != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalSave, new HarmonyMethod(prefixSave));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' failed");

                    var originalLoadAllMeshes = AccessTools.Method(tCore, "LoadAllMeshes");
                    var prefixLoadAllMeshes = AccessTools.Method(tPatch, nameof(PrefixLoadAllMeshes));
                    if (originalLoadAllMeshes != null &&
                        prefixLoadAllMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoadAllMeshes, new HarmonyMethod(prefixLoadAllMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.LoadAllMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.LoadAllMeshes' failed");

                    CurrentProfile = new ActiveProfileProxy(engineSetProfile);
                }
            }
        }
    }
}