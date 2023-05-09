using DevExpress.XtraBars.Docking2010.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntityTools.Quester.Editor.Classes
{
    /// <summary>
    /// Описание пути к узлу дерева <see cref="TreeView"/>
    /// </summary>
    public class TreeNodePosition
    {
        public TreeNodePosition(int index, string text, TreeNodePosition next = default)
        {
            Index = index;
            Text = text;
            Next = next;
        }

        public int Index { get; set; }
        public string Text { get; set; }
        public TreeNodePosition Next { get; set; }


        public bool IsMatch(TreeNode node)
        {
            return node != null
                && node.Index == Index
                && node.Text == Text;
        }
    }
}
