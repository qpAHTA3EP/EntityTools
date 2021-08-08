using AcTp0Tools.Reflection;
using Astral.Controllers;
using Astral.Logic.UCC.Classes;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using HarmonyLib;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
// ReSharper disable RedundantNameQualifier
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedParameter.Local

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
        public static void Apply()
        {
            if (PatchesWasApplied)
                return;
            
            tAddUCCActions = typeof(Astral.Quester.Classes.Actions.AddUCCActions);
            tPatch = typeof(ComplexPatch_Quester_UccEditing);

            original_AddUCCAction_InternalValidity = AccessTools.Property(tAddUCCActions, "InternalValidity" /*nameof(Astral.Quester.Classes.Actions.AddUCCActions.InternalValidity)*/)
                                                                ?.GetGetMethod(true);
            if (original_AddUCCAction_InternalValidity is null)
            {
                ETLogger.WriteLine($@"{nameof(original_AddUCCAction_InternalValidity)} not found");
                return;
            }
            original_AddUCCAction_Run = AccessTools.Method(tAddUCCActions, nameof(Astral.Quester.Classes.Actions.AddUCCActions.Run));
            if(original_AddUCCAction_Run is null)
            {
                ETLogger.WriteLine($@"{nameof(original_AddUCCAction_Run)} not found");
                return;
            }            
            original_SpecialAction_Run_RemoveAllTempUCCActions = AccessTools.Method(typeof(Astral.Quester.Classes.Actions.SpecialAction),
                                                                                nameof(Astral.Quester.Classes.Actions.SpecialAction.Run));
            if (original_SpecialAction_Run_RemoveAllTempUCCActions is null)
            {
                ETLogger.WriteLine($@"{nameof(original_SpecialAction_Run_RemoveAllTempUCCActions)} not found");
                return;
            }
            original_UccProfile_Save = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                      nameof(Astral.Logic.UCC.Classes.Profil.Save));
            if (original_UccProfile_Save is null)
            {
                ETLogger.WriteLine($@"{nameof(original_UccProfile_Save)} not found");
                return;
            }
            original_UccProfile_ToString = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                          nameof(Astral.Logic.UCC.Classes.Profil.ToString));
            if (original_UccProfile_ToString is null)
            {
                ETLogger.WriteLine($@"{nameof(original_UccProfile_ToString)} not found");
                return;
            }
            original_UCCProfileEditor_EditValue = AccessTools.Method(typeof(Astral.Quester.UIEditors.UCCProfileEditor),
                                                                     nameof(Astral.Quester.UIEditors.UCCProfileEditor.EditValue),
                                                                     new[] { typeof(ITypeDescriptorContext), typeof(IServiceProvider), typeof(object) });
            if (original_UCCProfileEditor_EditValue is null)
            {
                ETLogger.WriteLine($@"{nameof(original_UCCProfileEditor_EditValue)} not found");
                return;
            }

            prefix_AddUCCAction_InternalValidity = AccessTools.Method(tPatch, nameof(AddUCCAction_InternalValidity));
            if (prefix_AddUCCAction_InternalValidity is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_AddUCCAction_InternalValidity)} not found");
                return;
            }
            prefix_AddUCCAction_Run = AccessTools.Method(tPatch, nameof(AddUCCAction_Run));
            if (prefix_AddUCCAction_Run is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_AddUCCAction_Run)} not found");
                return;
            }
            prefix_SpecialAction_Run_RemoveAllTempUCCActions = AccessTools.Method(tPatch, nameof(SpecialAction_Run_RemoveAllTempUCCActions));
            if (prefix_SpecialAction_Run_RemoveAllTempUCCActions is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_SpecialAction_Run_RemoveAllTempUCCActions)} not found");
                return;
            }            
            prefix_UccProfile_Save = AccessTools.Method(tPatch, nameof(UccProfile_Save));
            if (prefix_UccProfile_Save is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_UccProfile_Save)} not found");
                return;
            }
            prefix_UccProfile_ToString = AccessTools.Method(tPatch, nameof(UccProfile_ToString));
            if (prefix_UccProfile_ToString is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_UccProfile_ToString)} not found");
                return;
            }
            prefix_UCCProfileEditor_EditValue = AccessTools.Method(tPatch, nameof(UCCProfileEditor_EditValue));
            if (prefix_UCCProfileEditor_EditValue is null)
            {
                ETLogger.WriteLine($@"{nameof(prefix_UCCProfileEditor_EditValue)} not found");
                return;
            }

            Action unpatch = null;

            try
            {
                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_AddUCCAction_InternalValidity, new HarmonyMethod(prefix_AddUCCAction_InternalValidity));
                unpatch = () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_AddUCCAction_InternalValidity, prefix_AddUCCAction_InternalValidity);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_AddUCCAction_Run, new HarmonyMethod(prefix_AddUCCAction_Run));
                unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_AddUCCAction_Run, prefix_AddUCCAction_Run);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_SpecialAction_Run_RemoveAllTempUCCActions, new HarmonyMethod(prefix_SpecialAction_Run_RemoveAllTempUCCActions));
                unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_SpecialAction_Run_RemoveAllTempUCCActions, prefix_SpecialAction_Run_RemoveAllTempUCCActions);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_UccProfile_Save, new HarmonyMethod(prefix_UccProfile_Save));
                unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_UccProfile_Save, prefix_UccProfile_Save);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_UccProfile_ToString, new HarmonyMethod(prefix_UccProfile_ToString));
                unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_UccProfile_ToString, prefix_UccProfile_ToString);

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_UCCProfileEditor_EditValue, new HarmonyMethod(prefix_UCCProfileEditor_EditValue));
                    
                PatchesWasApplied = true;
            }
            catch (Exception e)
            {
                unpatch?.Invoke();
                ETLogger.WriteLine($@"{nameof(ComplexPatch_Quester_UccEditing)} failed");
                ETLogger.WriteLine(LogType.Error, e.ToString());
                throw;
            }
            PatchesWasApplied = true;
            ETLogger.WriteLine($@"{nameof(ComplexPatch_Quester_UccEditing)} succeeded");
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Quester.Classes.Actions.AddUCCActions.Run()"/>
        /// добавляющий временные <seealso cref="UCCAction"/> в <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static bool AddUCCAction_Run(Astral.Quester.Classes.Actions.AddUCCActions __instance)
        {
            Action<UCCAction> setTempActionFlag = a => a.TempAction = true;
            __instance.Actions.ActionsCombat.ForEach(setTempActionFlag);
            __instance.Actions.ActionsPatrol.ForEach(setTempActionFlag);

            var uccProfile = Astral.Logic.UCC.Core.Get.mProfil;
            uccProfile.ActionsCombat.InsertRange(0, __instance.Actions.ActionsCombat);
            uccProfile.ActionsPatrol.InsertRange(0, __instance.Actions.ActionsPatrol);

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
            if (uccProfile.ActionsCombat.Count == 0
                && uccProfile.ActionsPatrol.Count == 0)
                __result = new Astral.Quester.Classes.Action.ActionValidity("Does not contain any actions");
            else __result = AcTp0Tools.Reflection.Empty.ActionValidity;
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
                int i = 0;
                while (i < uccProfile.ActionsCombat.Count)
                {
                    var uccAction = uccProfile.ActionsCombat[i];
                    if (uccAction.TempAction)
                        uccProfile.ActionsCombat.RemoveAt(i);
                    else i++;
                }
                i = 0;
                while (i < uccProfile.ActionsPatrol.Count)
                {
                    var uccAction = uccProfile.ActionsPatrol[i];
                    if (uccAction.TempAction)
                        uccProfile.ActionsPatrol.RemoveAt(i);
                    else i++;
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
        private static void UccProfile_Save(Astral.Logic.UCC.Classes.Profil __instance)
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
                while (i < __instance.ActionsPatrol.Count)
                {
                    var uccAction = __instance.ActionsPatrol[i];
                    if (uccAction.TempAction)
                        __instance.ActionsPatrol.RemoveAt(i);
                    else i++;
                }
            }
#endif
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Logic.UCC.Classes.Profil.ToString()"/>
        /// предназначенный для корректного отображения
        /// </summary>
        private static bool UccProfile_ToString(Astral.Logic.UCC.Classes.Profil __instance, ref string __result)
        {
            __result = $"Combat({__instance.ActionsCombat.Count}) and Patrol({__instance.ActionsPatrol.Count})";

            return false;
        }

        /// <summary>
        /// Патч метода <seealso cref="Astral.Quester.UIEditors.UCCProfileEditor.EditValue(object,ITypeDescriptorContext, IServiceProvider)" />,
        /// Модифицирующий окно редактора ucc-профиля <seealso cref="Astral.Logic.UCC.Forms.Editor"/>
        /// </summary>
        private static bool UCCProfileEditor_EditValue(ref object __result, ITypeDescriptorContext /*__context*/ __0, IServiceProvider /*__provider*/__1, object /*__value*/__2)
        {
            if (__2 is Profil uccProfile)
            {
                try
                {
                    var uccEditor = new Astral.Logic.UCC.Forms.Editor(uccProfile);
#if false
                    uccEditor.GetField<SimpleButton>("btn_newProfile").Value.Visible = false;
                    uccEditor.GetField<SimpleButton>("btn_saveProfile").Value.Visible = true;
                    uccEditor.GetField<SimpleButton>("bGenerate").Value.Visible = false;
                    uccEditor.GetField<RadioGroup>("momentChoose").Value.Visible = true;
                    uccEditor.GetField<XtraTabPage>("tacticTab").Value.PageEnabled = false; 
#else
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

                    var btnSaveProfileVisiblity = editorTrvs.Field("btn_saveProfile").Property("Visible");
                    if (btnSaveProfileVisiblity.PropertyExists())
                        btnSaveProfileVisiblity.SetValue(true);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.btn_saveProfile'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var btnGenerateVisible = editorTrvs.Field("bGenerate").Property("Visible");
                    if (btnGenerateVisible.PropertyExists())
                        btnGenerateVisible.SetValue(false);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.bGenerate'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var momentChooseVisible = editorTrvs.Field("momentChoose").Property("Visible");
                    if (momentChooseVisible.PropertyExists())
                        momentChooseVisible.SetValue(true);
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.momentChoose'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }

                    var tabTactic = editorTrvs.Field("tacticTab");
                    var tabTacticPageEnabled = tabTactic.Property("PageEnabled");
                    var tabTacticPageVisible = tabTactic.Property("PageVisible");
                    if (tabTactic.FieldExists()
                        && tabTacticPageEnabled.PropertyExists()
                        && tabTacticPageVisible.PropertyExists())
                    {
                        tabTacticPageEnabled.SetValue(false);
                        tabTacticPageVisible.SetValue(false);
                    }
                    else
                    {
                        var msg = "Fail to access to 'Astral.Logic.UCC.Forms.Editor.tacticTab'";
                        ETLogger.WriteLine(LogType.Error, msg);
                        XtraMessageBox.Show(msg);
                        return true;
                    }
#endif

                    uccEditor.ShowDialog();
                    __result = uccProfile;
                    return false;
                }
                catch { }
            }
            return true;
        }
    }
}
