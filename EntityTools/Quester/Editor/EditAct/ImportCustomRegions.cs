using Astral.Quester.Classes;
using DevExpress.XtraEditors;
using EntityTools.Forms;
using Infrastructure.Quester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class ImportCustomRegions : IEditAct
        {
            private List<CustomRegion> customRegion2import = new List<CustomRegion>();

            public bool Prepare(QuesterEditor editorForm)
            {
                customRegion2import.Clear();
                var editedProfile = editorForm?.Profile;
                if (editedProfile is null)
                    throw new ArgumentException("Quester Profile not defined");

                string profilePath = default;
                var profile = QuesterHelper.Load(ref profilePath);
                if (profile is null)
                    return false;

                var customRegions = profile.CustomRegions;
                if (customRegions.Count == 0)
                {
                    XtraMessageBox.Show(
                        $"There is no CustomRegions into the profile '{profilePath}'",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                var selectedCR = new List<CustomRegion>(customRegions.Count);
                if (MultipleItemSelectForm.GUIRequest(
                        source: () => customRegions,
                        selectedValues: ref selectedCR,
                        "Select CustomRegions to import.")
                    )
                {
                    var editedCustomRegionList = editedProfile.CustomRegions;

                    foreach (var cr in selectedCR)
                    {
                        bool needInsert = false;
                        var crName = cr.Name;
                        while (true)
                        {
                            var existsCR = editedCustomRegionList.FirstOrDefault(c => c.Name == crName);
                            needInsert = existsCR is null;

                            if (needInsert)
                                break;
                            
                            var choose = XtraMessageBox.Show(
                                                    $"The current profile already contains CustomRegion with the name  '{crName}'.\n" +
                                                    $"\tPress YES to rename and import it.\n" +
                                                    $"\tPress NO to import CustomRegion without renaming.\n" +
                                                    $"\tPress CANCEL to skip CustomRegion.",
                                                    "CustomRegion name conflict",
                                                    MessageBoxButtons.YesNoCancel
                                                );

                            if (choose == DialogResult.Yes)
                            {
                                if (InputBox.EditValue(
                                    value: ref crName,
                                    message: "Enter unique CustomRegion name",
                                    caption: "CustomRegion Name"))
                                {
                                    cr.Name = crName;
                                }
                                else break;
                            }
                            else if (choose == DialogResult.No)
                            {
                                needInsert = true;
                                break;
                            }
                            else break;
                        }

                        if (needInsert)
                        {
                            customRegion2import.Add(cr);
                        }
                    }
                }

                return customRegion2import.Count > 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                var editedCustomRegionList = editorForm.Profile?.CustomRegions;
                if (editedCustomRegionList is null
                    || customRegion2import.Count == 0)
                    return;

                foreach (var cr in customRegion2import)
                {
                    editedCustomRegionList.Add(cr);
                }

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                var editedCustomRegionList = editorForm.Profile?.CustomRegions;
                if (editedCustomRegionList is null
                    || customRegion2import.Count == 0)
                    return;

                int removedNum = 0;
                foreach (var cr in customRegion2import)
                {
                    if (editedCustomRegionList.Remove(cr))
                        removedNum++;
                }

                Applied = removedNum == 0;
            }

            public bool IsReady => customRegion2import.Count > 0;

            public bool Applied { get; private set; }
            public string Label => $"Import {customRegion2import.Count} CustomRegions from another Profile";

            public string UndoLabel => $"Remove {customRegion2import.Count} imported CustomRegions";
        }
    }
}
