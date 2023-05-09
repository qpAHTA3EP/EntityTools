using EntityTools.Quester.Editor.Classes;
using EntityTools.Quester.Editor.TreeViewCustomization;
using EntityTools.Tools;
using System;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class InsertQuesterCondition : IEditAct
        {
            private readonly QuesterEditor editor;

            private TreeNodePosition anchorPath;
            private readonly bool altHeld;

            private readonly ActionBaseTreeNode actionNode;
            private ConditionBaseTreeNode insertingNode;

            private readonly EditActionEvent onApply;
            private readonly EditActionEvent onRedo;

            public InsertQuesterCondition(
                QuesterEditor editor, 
                ConditionBaseTreeNode anchorNode, 
                bool altHeld,
                EditActionEvent onApply = null, 
                EditActionEvent onRedo = null)
            {
                this.editor = editor 
                           ?? throw new ArgumentException(nameof(editor));
                this.actionNode = editor.treeActions.SelectedNode as ActionBaseTreeNode 
                               ?? throw new Exception("No action selected");
                this.anchorPath = anchorNode?.MakePath() 
                               ?? throw new ArgumentException(nameof(anchorNode));
                this.altHeld = altHeld;
                this.onApply = onApply;
                this.onRedo = onRedo;
            }

            public bool Prepare(QuesterEditor editorForm)
            {
                if (Astral.Quester.Forms.AddAction.Show(typeof(QuesterCondition)) is QuesterCondition newCondition)
                {
                    insertingNode = newCondition.MakeTreeNode(editor.Profile);
                }
                else insertingNode = null;

                return insertingNode != null;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                var treeConditions = editor.treeConditions;

                treeConditions.BeginUpdate();
                treeConditions.InsertSmart(
                    insertingNode, 
                    anchorPath, 
                    altHeld
                );
                treeConditions.EndUpdate();

                editor.SetSelectedConditionTo(insertingNode);
                
                onApply?.Invoke(this);                
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (!Applied)
                    return;

                if (actionNode is null)
                    throw new InvalidOperationException($"Not found an Action for which condition should be restored.");

                editor.SetSelectedActionTo(actionNode);

                var treeConditions = editor.treeConditions;

                if (treeConditions is null)
                    throw new Exception("Not found node to remove.");

                treeConditions.BeginUpdate();
                insertingNode.Remove();
                treeConditions.EndUpdate();

                anchorPath = null;
                onRedo?.Invoke(this);
            }

            public bool IsReady => insertingNode != null;

            public bool Applied => insertingNode != null;

            public string Label
            {
                get
                {
                    return insertingNode != null
                        ? $"Insert condition '{insertingNode?.Text}'"
                        : "NotInitialized";
                }
            }

            public string UndoLabel
            {
                get
                {
                    return insertingNode != null
                        ? $"Remove condition '{insertingNode.Text}'"
                        : "NotInitialized";
                }
            }
        }
    }
}
