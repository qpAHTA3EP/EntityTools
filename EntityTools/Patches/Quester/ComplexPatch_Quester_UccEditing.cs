using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using DevExpress.XtraEditors;
using EntityTools.UCC.Editor;
using HarmonyLib;

namespace EntityTools.Patches.Quester
{
    /// <summary>
    /// Патч, расширяющий добавляющий возможность редактирования <seealso cref="Astral.Logic.UCC.Classes.Profil.ActionsPatrol"/>
    /// </summary>
    public static class ComplexPatch_Quester_UccEditing
    {
        public static bool PatchesWasApplied { get; private set; }

        private static Type tAddUCCActions;
        private static MethodInfo original_AddUCCAction_InternalValidity;
        private static MethodInfo prefix_AddUCCAction_InternalValidity;

        private static MethodInfo original_AddUCCAction_Run;
        private static MethodInfo prefix_AddUCCAction_Run;

        private static MethodInfo original_SpecialAction_Run_RemoveAllTempUCCActions;
        private static MethodInfo prefix_SpecialAction_Run_RemoveAllTempUCCActions;

        private static MethodInfo original_UccProfile_Save;
        private static MethodInfo prefix_UccProfile_Save;

        private static MethodInfo original_UCCProfileEditor_EditValue;
        private static MethodInfo prefix_UCCProfileEditor_EditValue;


        private static Type tPatch;
        private static MethodInfo original_UccProfile_ToString;
        private static MethodInfo prefix_UccProfile_ToString;

        private static KeyValuePair<Profil, List<TargetPriorityEntry>> tempPriorities;
        public static void Apply()
        {
            if (EntityTools.Config.Patches.QuesterPatches.ReplaceEditorForAddUccActions || PatchesWasApplied)
                return;
            
            tAddUCCActions = typeof(Astral.Quester.Classes.Actions.AddUCCActions);
            tPatch = typeof(ComplexPatch_Quester_UccEditing);

            original_AddUCCAction_InternalValidity = AccessTools.Property(tAddUCCActions, "InternalValidity" /*nameof(Astral.Quester.Classes.Actions.AddUCCActions.InternalValidity)*/)
                                                                ?.GetGetMethod(true);
            if (original_AddUCCAction_InternalValidity is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_AddUCCAction_InternalValidity}' not found", true);
                return;
            }
            original_AddUCCAction_Run = AccessTools.Method(tAddUCCActions, nameof(Astral.Quester.Classes.Actions.AddUCCActions.Run));
            if(original_AddUCCAction_Run is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_AddUCCAction_Run}' not found", true);
                return;
            }            
            original_SpecialAction_Run_RemoveAllTempUCCActions = AccessTools.Method(typeof(Astral.Quester.Classes.Actions.SpecialAction),
                                                                                nameof(Astral.Quester.Classes.Actions.SpecialAction.Run));
            if (original_SpecialAction_Run_RemoveAllTempUCCActions is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_SpecialAction_Run_RemoveAllTempUCCActions}' not found", true);
                return;
            }
            original_UccProfile_Save = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                      nameof(Astral.Logic.UCC.Classes.Profil.Save));
            if (original_UccProfile_Save is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_UccProfile_Save}' not found", true);
                return;
            }
            original_UccProfile_ToString = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                          nameof(Astral.Logic.UCC.Classes.Profil.ToString));
            if (original_UccProfile_ToString is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_UccProfile_ToString}' not found", true);
                return;
            }
            original_UCCProfileEditor_EditValue = AccessTools.Method(typeof(Astral.Quester.UIEditors.UCCProfileEditor),
                                                                     nameof(Astral.Quester.UIEditors.UCCProfileEditor.EditValue),
                                                                     new[] { typeof(ITypeDescriptorContext), typeof(IServiceProvider), typeof(object) });
            if (original_UCCProfileEditor_EditValue is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{original_UCCProfileEditor_EditValue}' not found", true);
                return;
            }

            prefix_AddUCCAction_InternalValidity = AccessTools.Method(tPatch, nameof(AddUCCAction_InternalValidity));
            if (prefix_AddUCCAction_InternalValidity is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{{nameof(ComplexPatch_Quester_UccEditing)}}' failed. Method '{prefix_AddUCCAction_InternalValidity}' not found", true);
                return;
            }
            prefix_AddUCCAction_Run = AccessTools.Method(tPatch, nameof(AddUCCAction_Run));
            if (prefix_AddUCCAction_Run is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{{nameof(ComplexPatch_Quester_UccEditing)}}' failed. Method '{prefix_AddUCCAction_Run}' not found", true);
                return;
            }
            prefix_SpecialAction_Run_RemoveAllTempUCCActions = AccessTools.Method(tPatch, nameof(SpecialAction_Run_RemoveAllTempUCCActions));
            if (prefix_SpecialAction_Run_RemoveAllTempUCCActions is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{prefix_SpecialAction_Run_RemoveAllTempUCCActions}' not found", true);
                return;
            }            
            prefix_UccProfile_Save = AccessTools.Method(tPatch, nameof(UccProfile_Save));
            if (prefix_UccProfile_Save is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{prefix_UccProfile_Save}' not found", true);
                return;
            }
            prefix_UccProfile_ToString = AccessTools.Method(tPatch, nameof(UccProfile_ToString));
            if (prefix_UccProfile_ToString is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{prefix_UccProfile_ToString}' not found", true);
                return;
            }
            prefix_UCCProfileEditor_EditValue = AccessTools.Method(tPatch, nameof(UCCProfileEditor_EditValue));
            if (prefix_UCCProfileEditor_EditValue is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed. Method '{prefix_UCCProfileEditor_EditValue}' not found", true);
                return;
            }

            Action unpatch = null;

            try
            {
                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_AddUCCAction_InternalValidity, new HarmonyMethod(prefix_AddUCCAction_InternalValidity));
                unpatch = () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_AddUCCAction_InternalValidity}'", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_AddUCCAction_InternalValidity,
                            prefix_AddUCCAction_InternalValidity);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_AddUCCAction_Run, new HarmonyMethod(prefix_AddUCCAction_Run));
                unpatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{prefix_AddUCCAction_Run}'", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_AddUCCAction_Run,
                            prefix_AddUCCAction_Run);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_SpecialAction_Run_RemoveAllTempUCCActions, new HarmonyMethod(prefix_SpecialAction_Run_RemoveAllTempUCCActions));
                unpatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_SpecialAction_Run_RemoveAllTempUCCActions}'", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(
                            original_SpecialAction_Run_RemoveAllTempUCCActions,
                            prefix_SpecialAction_Run_RemoveAllTempUCCActions);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_UccProfile_Save, new HarmonyMethod(prefix_UccProfile_Save));
                unpatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_UccProfile_Save}'", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_UccProfile_Save,
                            prefix_UccProfile_Save);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_UccProfile_ToString, new HarmonyMethod(prefix_UccProfile_ToString));
                //unpatch += () =>
                //{
                //    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_UccProfile_ToString}'", true);
                //    AcTp0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_UccProfile_ToString,
                //            prefix_UccProfile_ToString);
                //};

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_UCCProfileEditor_EditValue, new HarmonyMethod(prefix_UCCProfileEditor_EditValue));
                    
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester_UccEditing)}' failed", true);
                unpatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
            PatchesWasApplied = true;
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Quester.Classes.Actions.AddUCCActions.Run()"/>
        /// добавляющий временные <seealso cref="UCCAction"/> в <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static bool AddUCCAction_Run(Astral.Quester.Classes.Actions.AddUCCActions __instance)
        {
            var tempUccProfile = __instance.Actions;
            var currentUccProfile = Astral.Logic.UCC.Core.Get.mProfil;
            if (tempUccProfile.ActionsCombat?.Count > 0)
            {
                currentUccProfile.ActionsCombat.InsertRange(0, tempUccProfile.ActionsCombat.Select(a => {
                    var copy = a.Clone();
                    copy.TempAction = true;
                    return copy;
                }));
            }
            if (tempUccProfile.ActionsPatrol?.Count > 0)
            {
                currentUccProfile.ActionsPatrol.InsertRange(0, tempUccProfile.ActionsPatrol.Select(a => {
                    var copy = a.Clone();
                    copy.TempAction = true;
                    return copy;
                }));
            }

            if (tempUccProfile.TargetPriorities?.Count > 0)
            {
                var priorities = tempUccProfile.TargetPriorities.CreateDeepCopy();
                if (tempPriorities.Key != currentUccProfile)
                    tempPriorities = new KeyValuePair<Profil, List<TargetPriorityEntry>>(currentUccProfile, priorities);
                else tempPriorities.Value.AddRange(priorities);
                currentUccProfile.TargetPriorities.InsertRange(0, priorities);
            }
#if false
            var enableTargetPriorities = uccProfile.EnableTargetPriorities;
            var potionHealth = uccProfile.PotionHealth;
            var smartPotionUse = uccProfile.SmartPotionUse;
            var targetChangeCD = uccProfile.TargetChangeCD;
            var targetPriorityRange = uccProfile.TargetPriorityRange;
            var _ =
            Action action2 = () =>
            {

            }
            uccProfile.
            __result = Astral.Quester.Classes.Action.ActionResult.Completed; 
#endif

            return false;
        }

        private static bool AddUCCAction_InternalValidity(Astral.Quester.Classes.Actions.AddUCCActions __instance,
                                             ref Astral.Quester.Classes.Action.ActionValidity __result)
        {
            var uccProfile = __instance.Actions;
            if (uccProfile is null 
                || uccProfile.ActionsCombat?.Count == 0
                && uccProfile.ActionsPatrol?.Count == 0
                && uccProfile.TargetPriorities?.Count == 0)
                __result = new Astral.Quester.Classes.Action.ActionValidity("Does not contain any actions or priorities");
            else __result = Empty.ActionValidity;
            return false;
        }

        /// <summary>
        /// Препатч метода <see cref="Astral.Quester.Classes.Actions.SpecialAction.Run()"/>
        /// предназначенный для корректной обработки RemoveAllTempUCCActions (удаление временных <seealso cref="UCCAction"/> из <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static bool SpecialAction_Run_RemoveAllTempUCCActions(Astral.Quester.Classes.Actions.SpecialAction __instance,
                                   ref Astral.Quester.Classes.Action.ActionResult __result)
        {
            if (__instance.Action == Astral.Quester.Classes.Actions.SpecialAction.SAction.RemoveAllTempUCCActions)
            {
                var uccProfile = Astral.Logic.UCC.Core.Get.mProfil;
                if (uccProfile is null)
                    return false;
                int i = 0;
                var actions = uccProfile.ActionsCombat;
                if (actions != null)
                {
                    while (i < actions.Count)
                    {
                        var uccAction = actions[i];
                        if (uccAction.TempAction)
                            actions.RemoveAt(i);
                        else i++;
                    } 
                }
                i = 0;
                actions = uccProfile.ActionsPatrol;
                if (actions != null)
                {
                    while (i < actions.Count)
                    {
                        var uccAction = actions[i];
                        if (uccAction.TempAction)
                            actions.RemoveAt(i);
                        else i++;
                    } 
                }
                if (tempPriorities.Key == uccProfile)
                {
                    i = 0;
                    var priorities = uccProfile.TargetPriorities;
                    if (priorities != null)
                    {
                        while (i < priorities.Count)
                        {
                            var tempPriority = priorities[i];
                            if (tempPriorities.Value.Contains(tempPriority))
                                priorities.RemoveAt(i);
                            else i++;
                        } 
                    }
                }

                __result = Astral.Quester.Classes.Action.ActionResult.Completed;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Патч метода <seealso cref="Astral.Logic.UCC.Classes.Profil.Save()"/>
        /// предназначенный для корректного удаления временных <seealso cref="UCCAction"/> из <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static void UccProfile_Save(Profil __instance)
        {
#if false
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.SettingsPath + "\\CC";
            saveFileDialog.DefaultExt = "XML";
            saveFileDialog.Filter = @"UCC Profiles (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                int i = 0;
                while (i < __instance.ActionsCombat.Count)
                {
                    var uccAction = __instance.ActionsCombat[i];
                    if (uccAction.TempAction)
                        __instance.ActionsCombat.RemoveAt(i);
                    else i++;
                }
                i = 0;
                while (i < __instance.ActionsPatrol.Count)
                {
                    var uccAction = __instance.ActionsPatrol[i];
                    if (uccAction.TempAction)
                        __instance.ActionsPatrol.RemoveAt(i);
                    else i++;
                }

                Astral.Functions.XmlSerializer.Serialize(saveFileDialog.FileName, __instance, 1);
                Astral.API.CurrentSettings.LastUCCProfile = saveFileDialog.FileName;
            }

            return false; 
#else
            if (__instance != null)
            {
                int i = 0;
#if false
                while (i < __instance.ActionsCombat.Count)
                {
                    var uccAction = __instance.ActionsCombat[i];
                    if (uccAction.TempAction)
                        __instance.ActionsCombat.RemoveAt(i);
                    else i++;
                }
                i = 0; 
#endif
                var actions = __instance.ActionsPatrol;
                if (actions != null)
                {
                    while (i < actions.Count)
                    {
                        var uccAction = actions[i];
                        if (uccAction.TempAction)
                            actions.RemoveAt(i);
                        else i++;
                    } 
                }
                if (tempPriorities.Key == __instance)
                {
                    i = 0;
                    var priorities = __instance.TargetPriorities;
                    if (priorities != null)
                    {
                        while (i < priorities.Count)
                        {
                            var tempPriority = priorities[i];
                            if (tempPriorities.Value.Contains(tempPriority))
                                priorities.RemoveAt(i);
                            else i++;
                        } 
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Logic.UCC.Classes.Profil.ToString()"/>
        /// предназначенный для корректного отображения
        /// </summary>
        private static bool UccProfile_ToString(Profil __instance, ref string __result)
        {
            if(__instance is null)
                __result =  "Combat | Patrol | Priorities";
            else __result = string.Concat("Combat(", __instance.ActionsCombat?.Count ?? 0, 
                                      ") | Patrol(", __instance.ActionsPatrol?.Count ?? 0, 
                                      ") | Priorities (", __instance.TargetPriorities?.Count ?? 0, ')');

            return false;
        }

        /// <summary>
        /// Патч метода <seealso cref="Astral.Quester.UIEditors.UCCProfileEditor.EditValue(object,ITypeDescriptorContext, IServiceProvider)" />,
        /// модифицирующий окно редактора ucc-профиля <seealso cref="Astral.Logic.UCC.Forms.Editor"/>
        /// </summary>
        private static bool UCCProfileEditor_EditValue(ref object __result, ITypeDescriptorContext /*__context*/ __0, IServiceProvider /*__provider*/__1, object /*__value*/__2)
        {
            if (__2 is Profil uccProfile)
            {
                try
                {
                    if (EntityTools.Config.Patches.UccComplexPatch)
                    {
                        // Вызов собственного ucc-редактора
                        __result = UccEditor.Edit(uccProfile, "", true) 
                                 ? uccProfile 
                                 : __2;
                        return false;
                    }

                    // Модификация и вызов штатного ucc-редактора 
                    var uccEditor = new Astral.Logic.UCC.Forms.Editor(uccProfile);
                    var editorTrvs = Traverse.Create(uccEditor);

                    var btnNewProfileVisible = editorTrvs.Field("btn_newProfile").Property("Visible");
                    if (btnNewProfileVisible.PropertyExists())
                        btnNewProfileVisible.SetValue(false);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.btn_newProfile'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var btnSaveProfileVisibility = editorTrvs.Field("btn_saveProfile").Property("Visible");
                    if (btnSaveProfileVisibility.PropertyExists())
                        btnSaveProfileVisibility.SetValue(true);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.btn_saveProfile'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var btnGenerateVisibility = editorTrvs.Field("bGenerate").Property("Visible");
                    if (btnGenerateVisibility.PropertyExists())
                        btnGenerateVisibility.SetValue(false);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.bGenerate'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var momentChooseVisibility = editorTrvs.Field("momentChoose").Property("Visible");
                    if (momentChooseVisibility.PropertyExists())
                        momentChooseVisibility.SetValue(true);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.momentChoose'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var tabTactic = editorTrvs.Field("tacticTab");
                    var tabTacticPageEnabled = tabTactic.Property("PageEnabled");
                    var tabTacticPageVisibility = tabTactic.Property("PageVisible");
                    if (tabTactic.FieldExists()
                        && tabTacticPageEnabled.PropertyExists()
                        && tabTacticPageVisibility.PropertyExists())
                    {
                        tabTacticPageEnabled.SetValue(false);
                        tabTacticPageVisibility.SetValue(false);
                    }
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.tacticTab'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    uccEditor.ShowDialog();
                    __result = uccProfile;

                    return false;
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine(LogType.Error, e.ToString(), true);
                }
            }
            else __result = __2;
            return true;
        }
    }
}
