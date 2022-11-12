using System;

using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;

using DevExpress.XtraEditors;

using HarmonyLib;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Patches.Quester
{
    public static class ComplexPatch_Quester_Editor
    {
        private static readonly PropertyAccessor<Astral.Quester.Classes.Action> selectedActionAccessor;

        public static bool PatchesWasApplied { get; private set; }

        private static bool PrepatchEditProfile(object sender, EventArgs e)
        {
            if (EntityTools.Config.Patches.QuesterPatches.ReplaceQuesterEditor)
            {
                var profile = Astral.Quester.API.CurrentProfile;
                var profileName = Astral.API.CurrentSettings.LastQuesterProfile;

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
                /*
                if (this.SelectedAction == null)
	            {
		            XtraMessageBox.Show("No selected action !");
		            return;
	            }
	            if (Main.editorForm != null && !Main.editorForm.IsDisposed)
	            {
		            Main.editorForm.SelectedAction = this.SelectedAction;
		            Main.editorForm.Focus();
		            return;
	            }
	            Main.editorForm = new Editor();
	            Main.editorForm.HaveToSelect = this.SelectedAction;
	            Main.editorForm.Show();
                */
                var selectedAction = selectedActionAccessor[__instance];
                if (selectedAction is null)
                {
                    XtraMessageBox.Show("No action selected!");
                    return false;
                }

                var profile = Astral.Quester.API.CurrentProfile;
                var profileName = Astral.API.CurrentSettings.LastQuesterProfile;

                if (profile.Saved && !string.IsNullOrEmpty(profileName))
                    QuesterEditor.Edit(profile, profileName, selectedAction.ActionID);
                else QuesterEditor.Edit(profile, selectedAction.ActionID);
                return false;
            }
            return true;
        }

        public static void Apply(){}

        static ComplexPatch_Quester_Editor()
        {
            if (!EntityTools.Config.Patches.QuesterPatches.ReplaceQuesterEditor)
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
