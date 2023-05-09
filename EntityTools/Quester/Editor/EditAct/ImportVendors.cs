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
        private class ImportVendors : IEditAct
        {
            private List<NPCInfos> vendor2import = new List<NPCInfos>();

            public bool Prepare(QuesterEditor editorForm)
            {
                vendor2import.Clear();
                var editedProfile = editorForm?.Profile;
                if (editedProfile is null)
                    throw new ArgumentException("Quester Profile not defined");

                string profilePath = default;
                var profile = QuesterHelper.Load(ref profilePath);
                if (profile is null)
                    return false;

                var vendors = profile.Vendors;
                if (vendors.Count == 0)
                {
                    XtraMessageBox.Show(
                        $"There is no Vendors into the profile '{profilePath}'",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                var selectedVendors = new List<NPCInfos>(vendors.Count);
                if (MultipleItemSelectForm.GUIRequest(
                        source: () => vendors,
                        selectedValues: ref selectedVendors,
                        "Select Vendors to import.")
                    )
                {
                    var editedVendors = editedProfile.Vendors;

                    foreach (var v in selectedVendors)
                    {
                        var costumeName = v.CostumeName;
                        var existsVendor = editedVendors.FirstOrDefault(
                            c => c.CostumeName == v.CostumeName
                                && c.MapName == v.MapName
                                && c.RegionName == v.RegionName
                                && c.Position == v.Position
                        );
                        
                        if (existsVendor is null)
                            vendor2import.Add(v);
                    }
                }

                return vendor2import.Count > 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                var editedCustomRegionList = editorForm.Profile?.Vendors;
                if (editedCustomRegionList is null
                    || vendor2import.Count == 0)
                    return;

                foreach (var cr in vendor2import)
                    editedCustomRegionList.Add(cr);

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                var vendors = editorForm.Profile?.Vendors;
                if (vendors is null
                    || vendor2import.Count == 0)
                    return;

                foreach (var v in vendor2import)
                    vendors.Remove(v);

                Applied = false;
            }

            public bool IsReady => vendor2import.Count > 0;

            public bool Applied { get; private set; }
            public string Label => $"Import {vendor2import.Count} Vendors from another Profile";

            public string UndoLabel => $"Remove {vendor2import.Count} imported Vendors";
        }
    }
}
