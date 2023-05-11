using Astral.Quester.Classes;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteVendor : IEditAct
        {
            private NPCInfos deletedVendor = null;
            private int deleteFromPosition = -1;

            public bool Prepare(QuesterEditor editorForm)
            {
                deletedVendor = editorForm.listVendor.SelectedItem as NPCInfos;
                deleteFromPosition = editorForm.listVendor.SelectedIndex;
                return deletedVendor != null && deleteFromPosition >= 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                editorForm.listVendor.Items.RemoveAt(deleteFromPosition);

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (Applied && IsReady)
                {
                    editorForm.listVendor.Items.Insert(deleteFromPosition, deletedVendor);
                    deletedVendor = default;
                    deleteFromPosition = -1;
                    Applied = false;
                }
            }

            public bool IsReady => deletedVendor != null && deleteFromPosition >= 0;

            public bool Applied { get; private set; }
            public string Label => $"Delete Vendor '{deletedVendor?.ToString() ?? string.Empty}'";

            public string UndoLabel => $"Restore Vendor '{deletedVendor?.ToString() ?? string.Empty}'";
        }
    }
}
