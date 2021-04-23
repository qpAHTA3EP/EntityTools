namespace EntityTools.Forms
{
    partial class ObjectInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pgObjectInfos = new System.Windows.Forms.PropertyGrid();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miDetail = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgObjectInfos
            // 
            this.pgObjectInfos.ContextMenuStrip = this.contextMenu;
            this.pgObjectInfos.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            this.pgObjectInfos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgObjectInfos.HelpVisible = false;
            this.pgObjectInfos.Location = new System.Drawing.Point(1, 1);
            this.pgObjectInfos.Name = "pgObjectInfos";
            this.pgObjectInfos.Size = new System.Drawing.Size(332, 310);
            this.pgObjectInfos.TabIndex = 0;
            this.pgObjectInfos.ToolbarVisible = false;
            this.pgObjectInfos.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.handler_SelectedGridItemChanged);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDetail});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(105, 26);
            // 
            // miDetail
            // 
            this.miDetail.Name = "miDetail";
            this.miDetail.Size = new System.Drawing.Size(104, 22);
            this.miDetail.Text = "Detail";
            this.miDetail.Click += new System.EventHandler(this.handler_ItemDetail);
            // 
            // ObjectInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 312);
            this.Controls.Add(this.pgObjectInfos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "ObjectInfoForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ObjectInfoForm";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.handler_FormClosed);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgObjectInfos;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem miDetail;
    }
}