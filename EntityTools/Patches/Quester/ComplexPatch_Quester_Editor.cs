using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Infrastructure;
using Infrastructure.Patches;
using Infrastructure.Reflection;
using Astral.Quester.Classes.Actions;
using Astral.Quester.Classes.Conditions;
using DevExpress.XtraEditors;
using EntityTools.Quester.Editor.Classes;
using HarmonyLib;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Patches.Quester
{
    public static class ComplexPatch_Quester_Editor
    {
        private static PropertyAccessor<Astral.Quester.Classes.Action> selectedActionAccessor;

        public static bool PatchesWasApplied { get; private set; }

        private static bool PrepatchEditProfile(object sender, EventArgs e)
        {
            if (EntityTools.Config.Patches.QuesterPatches.ReplaceQuesterEditor)
            {
                var activeProfile = AstralAccessors.Quester.Core.CurrentProfile;
                var profile = activeProfile.GetProfile();
                var profileName = activeProfile.ProfilePath;

                if (profile.Saved && !string.IsNullOrEmpty(profileName))
                    QuesterEditor.Edit(profile, profileName);
                else QuesterEditor.Edit(profile);
                return false;
            }
            return true;
        }

        private static bool PrepatchEditSelectedAction(Astral.Quester.Forms.Main __instance, object sender, EventArgs e)
        {
            if (EntityTools.Config.Patches.QuesterPatches.ReplaceQuesterEditor)
            {
                var selectedAction = selectedActionAccessor[__instance];
                if (selectedAction is null)
                {
                    XtraMessageBox.Show("No action selected!");
                    return false;
                }

                var activeProfile = AstralAccessors.Quester.Core.CurrentProfile;
                var profile = activeProfile.GetProfile();
                var profileName = activeProfile.ProfilePath;

                if (profile.Saved && !string.IsNullOrEmpty(profileName))
                    QuesterEditor.Edit(profile, profileName, selectedAction.ActionID);
                else QuesterEditor.Edit(profile, selectedAction.ActionID);
                return false;
            }
            return true;
        }

#if false
        private static void SetTypeAssociations()
        {
            // Подмена метаданных для IsInCustomRegion
            // https://stackoverflow.com/questions/46099675/can-you-how-to-specify-editor-at-runtime-for-propertygrid-for-pcl
            var provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(IsInCustomRegion),
                typeof(IsInCustomRegionMetadataType));
            TypeDescriptor.AddProvider(provider, typeof(IsInCustomRegion));

            provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(LoadProfile),
                typeof(LoadProfileMetadataType));
            TypeDescriptor.AddProvider(provider, typeof(LoadProfile));

            var tPushProfileToStackAndLoad = ACTP0Serializer.PushProfileToStackAndLoad;
            if (tPushProfileToStackAndLoad != null)
            {
                provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                    tPushProfileToStackAndLoad,
                    typeof(PushProfileToStackAndLoadMetadataType));
                TypeDescriptor.AddProvider(provider, tPushProfileToStackAndLoad);
            }
            var tConditionPack = ACTP0Serializer.QuesterConditionPack;
            if (tConditionPack != null)
            {
                provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                    tConditionPack,
                    typeof(ConditionPackMetadataType));
                TypeDescriptor.AddProvider(provider, tConditionPack);
            }
        } 
#endif

        public static void Apply()
        {
            if (PatchesWasApplied || !EntityTools.Config.Patches.QuesterPatches.ReplaceQuesterEditor)
                return;

            var tPatch = typeof(ComplexPatch_Quester_Editor);
            var tMain = typeof(Astral.Quester.Forms.Main);


            var miEditorClick = AccessTools.Method(tMain, "b_editor_Click");
            if (miEditorClick is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_Editor)}' failed. Method 'b_editor_Click' not found", true);
                return;
            }
            var prepatchEditProfile = AccessTools.Method(tPatch, nameof(PrepatchEditProfile));

            var miEditActionClick = AccessTools.Method(tMain, "editToolStripMenuItem_Click");
            if (miEditActionClick is null)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Quester_Editor)}' failed. Method 'editToolStripMenuItem_Click' not found", true);
                return;
            }
            var prepatchEditAction = AccessTools.Method(tPatch, nameof(PrepatchEditSelectedAction));

            Action unpatch = null;
            try
            {
                ACTP0Patcher.Harmony.Patch(miEditorClick, new HarmonyMethod(prepatchEditProfile));
                unpatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{miEditorClick}'", true);
                    ACTP0Patcher.Harmony.Unpatch(miEditorClick, prepatchEditProfile);
                };

                ACTP0Patcher.Harmony.Patch(miEditActionClick, new HarmonyMethod(prepatchEditAction));
                unpatch += () =>
                {
                    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{miEditActionClick}'", true);
                    ACTP0Patcher.Harmony.Unpatch(miEditActionClick, prepatchEditAction);
                };
                selectedActionAccessor = tMain.GetProperty<Astral.Quester.Classes.Action>("SelectedAction");

                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester_Editor)}' succeeded", true);

#if false
                SetTypeAssociations(); 
#endif
                PatchesWasApplied = true;
            }
            catch (Exception e)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester_Editor)}' failed", true);
                unpatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString());
            }
        }
    }
}
