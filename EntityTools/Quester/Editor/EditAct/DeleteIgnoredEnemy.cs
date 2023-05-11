using Astral.Quester.Classes;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteIgnoredEnemy : IEditAct
        {
            private string deletedEnemy = null;
            private int deleteFromPosition = -1;

            public bool Prepare(QuesterEditor editorForm)
            {
                deletedEnemy = editorForm.listBlackList.SelectedItem as string;
                deleteFromPosition = editorForm.listBlackList.SelectedIndex;
                return deletedEnemy != null && deleteFromPosition >= 0;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                editorForm.listBlackList.Items.RemoveAt(deleteFromPosition);

                Applied = true;
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (Applied && IsReady)
                {
                    editorForm.listBlackList.Items.Insert(deleteFromPosition, deletedEnemy);
                    deletedEnemy = default;
                    deleteFromPosition = -1;
                    Applied = false;
                }
            }

            public bool IsReady => deletedEnemy != null && deleteFromPosition >= 0;

            public bool Applied { get; private set; }
            public string Label => $"Delete Enemy '{deletedEnemy ?? string.Empty}' from BlackList";

            public string UndoLabel => $"Restore Vendor '{deletedEnemy ?? string.Empty}' into BlackList";
        }
    }
}
