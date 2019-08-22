using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    public partial class UIViewer : Form
    {
        private static UIViewer uiViewer;

        public UIViewer()
        {
            InitializeComponent();
        }

        public static void GetUiGen(UIGen parentUiGen = null)
        {
            if (uiViewer == null)
                uiViewer = new UIViewer();
            uiViewer.ShowDialog();
        }

        private void FillTreeView(TreeNode root = null)
        {
            if (root == null)
            {
                root = new TreeNode();
                root.Text = "Game Interfaces";
            }
            UIGen parentUiGen = root.Tag as UIGen;

            List<UIGen> uiChilds;

            if(parentUiGen == null || !parentUiGen.IsValid)
                uiChilds = MyNW.Internals.UIManager.AllUIGen.FindAll(ui =>
                                (ui.IsValid && (!filterVisibleOnly.Checked || ui.IsVisible)
                                 && !string.IsNullOrEmpty(ui.Name) && (!string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >=0)
                                 && (ui.Parent == null || !ui.Parent.IsValid)));
            else uiChilds = MyNW.Internals.UIManager.AllUIGen.FindAll(ui =>
                                (ui.IsValid && (!filterVisibleOnly.Checked || ui.IsVisible) 
                                 //&& !string.IsNullOrEmpty(ui.Name) && (!string.IsNullOrEmpty(filterName.Text) || ui.Name.Contains(filterName.Text))
                                 && (ui.Parent != null && ui.Parent.Pointer == parentUiGen.Pointer)));

            foreach (UIGen uiGen in uiChilds)
            {
                TreeNode child = new TreeNode();
                child.Text = uiGen.Name;
                child.Tag = uiGen;
                root.Nodes.Add(child);
                FillTreeView(child);
            }

            tvInterfaces.Nodes.Add(root);
        }

        private void Refresh(object sender, EventArgs e)
        {
            tvInterfaces.Nodes.Clear();
            FillTreeView();
        }
    }
}
