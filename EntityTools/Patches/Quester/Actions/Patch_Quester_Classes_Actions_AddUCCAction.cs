using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcTp0Tools.Reflection;
using Astral.Controllers;
using Astral.Logic.UCC.Classes;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using HarmonyLib;

namespace EntityTools.Patches.Quester.Actions
{
    /// <summary>
    /// Патч, расширяющий добавляющий возможность редактирования <seealso cref="Astral.Logic.UCC.Classes.Profil.ActionsPatrol"/>
    /// </summary>
    public static class Patch_Quester_UccEditing
    {
        public static void Patch()
        {
            var tAddUCCActions = typeof(Astral.Quester.Classes.Actions.AddUCCActions);
            var orig_AddUCCAction_InternalValidity = AccessTools.Property(tAddUCCActions, "InternalValidity" /*nameof(Astral.Quester.Classes.Actions.AddUCCActions.InternalValidity)*/)
                                                                    ?.GetGetMethod(true);
            var orig_AddUCCAction_Run = AccessTools.Method(tAddUCCActions, nameof(Astral.Quester.Classes.Actions.AddUCCActions.Run));
            var orig_SpecialAction_Run_RemoveAllTempUCCActions = AccessTools.Method(typeof(Astral.Quester.Classes.Actions.SpecialAction),
                                                                                    nameof(Astral.Quester.Classes.Actions.SpecialAction.Run));
            var orig_UccProfile_Save = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                          nameof(Astral.Logic.UCC.Classes.Profil.Save));
            var orig_UccProfile_ToString = AccessTools.Method(typeof(Astral.Logic.UCC.Classes.Profil),
                                                              nameof(Astral.Logic.UCC.Classes.Profil.ToString));
            var orig_UCCProfileEditor_EditValue = AccessTools.Method(typeof(Astral.Quester.UIEditors.UCCProfileEditor),
                                                                     nameof(Astral.Quester.UIEditors.UCCProfileEditor.EditValue),
                                                                     new Type[] { typeof(ITypeDescriptorContext), typeof(IServiceProvider), typeof(object) });

            var tPatch = typeof(Patch_Quester_UccEditing);
            var prepatch_AddUCCAction_InternalValidity = new HarmonyMethod(AccessTools.Method(tPatch, nameof(AddUCCAction_InternalValidity)));
            var prepatch_AddUCCAction_Run = new HarmonyMethod(AccessTools.Method(tPatch, nameof(AddUCCAction_Run)));
            var prepatch_SpecialAction_Run_RemoveAllTempUCCActions = new HarmonyMethod(AccessTools.Method(tPatch, nameof(SpecialAction_Run_RemoveAllTempUCCActions)));
            var prepatch_UccProfile_Save = new HarmonyMethod(AccessTools.Method(tPatch, nameof(UccProfile_Save)));
            var prepatch_UccProfile_ToString = new HarmonyMethod(AccessTools.Method(tPatch, nameof(UccProfile_ToString)));
            var prepatch_UCCProfileEditor_EditValue = new HarmonyMethod(AccessTools.Method(tPatch, nameof(UCCProfileEditor_EditValue)));

            if (orig_AddUCCAction_Run != null && prepatch_AddUCCAction_Run != null
                && orig_AddUCCAction_InternalValidity != null
                && prepatch_AddUCCAction_InternalValidity != null
                && orig_SpecialAction_Run_RemoveAllTempUCCActions != null
                && prepatch_SpecialAction_Run_RemoveAllTempUCCActions != null
                && orig_UccProfile_Save != null && prepatch_UccProfile_Save != null
                && orig_UccProfile_ToString != null && prepatch_UccProfile_ToString != null
                && orig_UCCProfileEditor_EditValue != null
                && prepatch_UCCProfileEditor_EditValue != null)
            {
                Action unpatch = null;

                try
                {
                    var patch_AddUCCAction_InternalValidity = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_AddUCCAction_InternalValidity, prepatch_AddUCCAction_InternalValidity);
                    unpatch = () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(orig_AddUCCAction_InternalValidity, prepatch_AddUCCAction_InternalValidity.method);

                    var patch_AddUCCAction_Run = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_AddUCCAction_Run, prepatch_AddUCCAction_Run);
                    unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(orig_AddUCCAction_Run, prepatch_AddUCCAction_Run.method);

                    var patch_SpecialAction_Run_RemoveAllTempUCCActions = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_SpecialAction_Run_RemoveAllTempUCCActions, prepatch_SpecialAction_Run_RemoveAllTempUCCActions);
                    unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(orig_SpecialAction_Run_RemoveAllTempUCCActions, prepatch_SpecialAction_Run_RemoveAllTempUCCActions.method);

                    var patch_UccProfile_Save = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_UccProfile_Save, prepatch_UccProfile_Save);
                    unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(orig_UccProfile_Save, prepatch_UccProfile_Save.method);

                    var patch_UccProfile_ToString = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_UccProfile_ToString, prepatch_UccProfile_ToString);
                    unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(orig_UccProfile_ToString, prepatch_UccProfile_ToString.method);

                    var patch_UCCProfileEditor_EditValue = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(orig_UCCProfileEditor_EditValue, prepatch_UCCProfileEditor_EditValue);
                }
                catch (Exception)
                {
                    unpatch?.Invoke();
                    throw;
                }
            }
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Quester.Classes.Actions.AddUCCActions.Run()"/>
        /// добавляющий временные <seealso cref="UCCAction"/> в <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static bool AddUCCAction_Run(Astral.Quester.Classes.Actions.AddUCCActions __instance,
                                             Astral.Quester.Classes.Action.ActionResult __result)
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
#if false
                bool flag = false;
                do
                {
                    flag = false;
                    foreach (UCCAction uccaction in Core.Get.mProfil.ActionsCombat)
                    {
                        if (uccaction.TempAction)
                        {
                            Core.Get.mProfil.ActionsCombat.Remove(uccaction);
                            flag = true;
                            break;
                        }
                    }
                }
                while (flag); 
#else
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
#endif
                __result = Astral.Quester.Classes.Action.ActionResult.Completed;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Препатч метода <seealso cref="Astral.Logic.UCC.Classes.Profil.Save()"/>
        /// предназначенный для корректного удаления временных <seealso cref="UCCAction"/> из <seealso cref="Profil.ActionsPatrol"/>
        /// </summary>
        private static bool UccProfile_Save(Astral.Logic.UCC.Classes.Profil __instance)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.SettingsPath + "\\CC";
            saveFileDialog.DefaultExt = "XML";
            saveFileDialog.Filter = "UCC Profiles (*.xml)|*.xml";
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
        /// Препатч метода <seealso cref="Astral.Quester.UIEditors.UCCProfileEditor.EditValue()"/>,
        /// Модифицирующий окно редактора ucc-профиля <seealso cref="Astral.Logic.UCC.Forms.Editor"/>
        /// </summary>
        private static bool UCCProfileEditor_EditValue(ref object __result, ITypeDescriptorContext /*__context*/ __0, IServiceProvider /*__provider*/__1, object /*__value*/__2)
        {
            if (__2 is Profil uccProfile)
            {
                var uccEditor = new Astral.Logic.UCC.Forms.Editor(uccProfile, false);

                uccEditor.GetField<SimpleButton>("btn_newProfile").Value.Visible = false;
                uccEditor.GetField<SimpleButton>("btn_saveProfile").Value.Visible = true;
                uccEditor.GetField<SimpleButton>("bGenerate").Value.Visible = false;
                uccEditor.GetField<RadioGroup>("momentChoose").Value.Visible = true;
                uccEditor.GetField<XtraTabPage>("tacticTab").Value.PageEnabled = false;

                uccEditor.ShowDialog();
                __result = uccProfile;
                return false; 
            }
            return true;
        }
    }
}
