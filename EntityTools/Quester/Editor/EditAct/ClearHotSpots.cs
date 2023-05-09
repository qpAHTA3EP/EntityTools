using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntityTools.Quester.Editor.TreeViewCustomization;
using MyNW.Classes;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteHotSpots : IEditAct
        {
            private ActionBaseTreeNode actionNode;
            private List<Vector3> hotSpots = null;
            private int[] deletedHotSpots = null;

            private EditActionEvent onApply = null;
            private EditActionEvent onRedo = null;
            public DeleteHotSpots(EditActionEvent onApply = null, EditActionEvent onRedo = null)
            {
                this.onApply = onApply;
                this.onRedo = onRedo;
            }

            public bool Prepare(QuesterEditor editorForm)
            {
                if (editorForm is null)
                    return false;

                actionNode = editorForm.treeActions.SelectedNode as ActionBaseTreeNode;
                if(actionNode is null || !actionNode.UseHotSpots)
                    return false;

                hotSpots = null;
                Label = string.Empty;
                UndoLabel = string.Empty;

                deletedHotSpots = editorForm.gridViewHotSpots.GetSelectedRows();
                
                return IsReady;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady) 
                    return;

                var gridHotSpots = editorForm.gridViewHotSpots;

                var msgBuilder = new StringBuilder();

                gridHotSpots.BeginUpdate();
                var actionHotSpots = actionNode.HotSpots;
                hotSpots = actionHotSpots.ToList();

                for (int i = 0; i < deletedHotSpots.Length; i++)
                {
                    int ind = deletedHotSpots[i] - i;
                    if (i > 0)
                        msgBuilder.Append("; ");
                    var pos = actionHotSpots[ind];
                    msgBuilder.Append($"<{pos.X:N2}, {pos.Y:N2}, {pos.Z:N2}>");
                    actionHotSpots.RemoveAt(ind);
                }
                gridHotSpots.RefreshData();
                gridHotSpots.EndUpdate();

                Label = $"Deleted from action '{actionNode}' following HotSpots: {msgBuilder}";
                UndoLabel = $"Restore for action '{actionNode}' following HotSpots: {msgBuilder}";

                onApply?.Invoke(this);
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (Applied)
                {
                    var actionHotSpots = actionNode.HotSpots;

                    actionHotSpots.Clear();
                    actionHotSpots.AddRange(hotSpots);
                    
                    editorForm.SetSelectedActionTo(actionNode);

                    editorForm.gridViewHotSpots.RefreshData();
                }
            }

            public bool IsReady => actionNode!= null && deletedHotSpots?.Length > 0;

            public bool Applied => actionNode != null && hotSpots?.Count > 0;

            public string Label { get; private set; }

            public string UndoLabel { get; private set; }
        }
    }
}
