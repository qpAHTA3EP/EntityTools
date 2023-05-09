using DevExpress.XtraEditors;
using EntityTools.Forms;
using Infrastructure.Quester;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class ImportEnemies : IEditAct
        {
            private List<string> enemies2import = new List<string>();

            public bool Prepare(QuesterEditor editorForm)
            {
                enemies2import.Clear();
                var editedProfile = editorForm?.Profile;
                if (editedProfile is null)
                    throw new ArgumentException("Quester Profile not defined");

                string profilePath = default;
                var profile = QuesterHelper.Load(ref profilePath);
                if (profile is null)
                    return false;

                var vendors = profile.BlackList;
                if (vendors.Count == 0)
                {
                    XtraMessageBox.Show(
                        $"There is empty Blacklist into the profile '{profilePath}'",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                var selectedEnemies = new List<string>(vendors.Count);
                if (MultipleItemSelectForm.GUIRequest(
                        source: () => vendors,
                        selectedValues: ref selectedEnemies,
                        "Select Enemies to import.")
                    )
                {
                    var editedBlackList = editedProfile.BlackList;

                    foreach (var enemy in selectedEnemies)
                    {
                        if (!editedBlackList.Contains(enemy))
                            enemies2import.Add(enemy);
                    }
                }

                return enemies2import.Count > 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                var profileBlackList = editorForm.Profile?.BlackList;
                if (profileBlackList is null
                    || enemies2import.Count == 0)
                    return;

                foreach (var cr in enemies2import)
                    profileBlackList.Add(cr);

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                var profileBlackList = editorForm.Profile?.BlackList;
                if (profileBlackList is null
                    || enemies2import.Count == 0)
                    return;

                foreach (var enemy in enemies2import)
                    profileBlackList.Remove(enemy);

                Applied = false;
            }

            public bool IsReady => enemies2import.Count > 0;

            public bool Applied { get; private set; }
            public string Label => $"Import {enemies2import.Count} Enemies from another Profile to BlackList ";

            public string UndoLabel => $"Remove {enemies2import.Count} imported Enemies from BlackList";
        }
    }
}
