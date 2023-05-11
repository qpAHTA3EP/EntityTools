using Astral.Quester.Classes;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteCustomRegion : IEditAct
        {
            private CustomRegion deletedCR = null;
            private int deleteFromPosition = -1;

            public bool Prepare(QuesterEditor editorForm)
            {
                deletedCR = editorForm.listCustomRegions.SelectedItem as CustomRegion;
                deleteFromPosition = editorForm.listCustomRegions.SelectedIndex;
                return deletedCR != null && deleteFromPosition >= 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                editorForm.listCustomRegions.Items.RemoveAt(deleteFromPosition);

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (Applied && IsReady)
                {
                    editorForm.listCustomRegions.Items.Insert(deleteFromPosition, deletedCR);
                    deletedCR = default;
                    deleteFromPosition = -1;
                    Applied = false;
                }
            }

            public bool IsReady => deletedCR != null && deleteFromPosition >= 0;

            public bool Applied { get; private set; }
            public string Label => $"Delete CustomRegion '{deletedCR?.ToString() ?? string.Empty}'";

            public string UndoLabel => $"Restore CustomRegion '{deletedCR?.ToString() ?? string.Empty}'";
        }
    }
}
