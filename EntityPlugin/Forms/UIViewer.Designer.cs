namespace EntityPlugin.Forms
{
    partial class UIViewer
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
            this.tvInterfaces = new System.Windows.Forms.TreeView();
            this.pgProperties = new System.Windows.Forms.PropertyGrid();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.filterVisibleOnly = new System.Windows.Forms.CheckBox();
            this.filterName = new System.Windows.Forms.TextBox();
            this.btnFill = new System.Windows.Forms.Button();
            this.lblFilterName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvInterfaces
            // 
            this.tvInterfaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvInterfaces.Location = new System.Drawing.Point(0, 0);
            this.tvInterfaces.Name = "tvInterfaces";
            this.tvInterfaces.Size = new System.Drawing.Size(400, 340);
            this.tvInterfaces.TabIndex = 0;
            // 
            // pgProperties
            // 
            this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProperties.Location = new System.Drawing.Point(0, 0);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(372, 340);
            this.pgProperties.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(13, 51);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tvInterfaces);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pgProperties);
            this.splitContainer.Size = new System.Drawing.Size(776, 340);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.TabIndex = 2;
            // 
            // filterVisibleOnly
            // 
            this.filterVisibleOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterVisibleOnly.AutoSize = true;
            this.filterVisibleOnly.Checked = true;
            this.filterVisibleOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterVisibleOnly.Location = new System.Drawing.Point(432, 15);
            this.filterVisibleOnly.Name = "filterVisibleOnly";
            this.filterVisibleOnly.Size = new System.Drawing.Size(156, 17);
            this.filterVisibleOnly.TabIndex = 3;
            this.filterVisibleOnly.Text = "Show only visible interfaces";
            this.filterVisibleOnly.UseVisualStyleBackColor = true;
            // 
            // filterName
            // 
            this.filterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterName.Location = new System.Drawing.Point(114, 13);
            this.filterName.Name = "filterName";
            this.filterName.Size = new System.Drawing.Size(298, 20);
            this.filterName.TabIndex = 4;
            // 
            // btnFill
            // 
            this.btnFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFill.Location = new System.Drawing.Point(713, 11);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(75, 23);
            this.btnFill.TabIndex = 5;
            this.btnFill.Text = "Refresh";
            this.btnFill.UseVisualStyleBackColor = true;
            this.btnFill.Click += new System.EventHandler(this.Refresh);
            // 
            // lblFilterName
            // 
            this.lblFilterName.AutoSize = true;
            this.lblFilterName.Location = new System.Drawing.Point(12, 16);
            this.lblFilterName.Name = "lblFilterName";
            this.lblFilterName.Size = new System.Drawing.Size(96, 13);
            this.lblFilterName.TabIndex = 6;
            this.lblFilterName.Text = "Filter by name part:";
            // 
            // UIViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblFilterName);
            this.Controls.Add(this.btnFill);
            this.Controls.Add(this.filterName);
            this.Controls.Add(this.filterVisibleOnly);
            this.Controls.Add(this.splitContainer);
            this.Name = "UIViewer";
            this.Text = "UIViewer";
            this.Load += new System.EventHandler(this.Refresh);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvInterfaces;
        private System.Windows.Forms.PropertyGrid pgProperties;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.CheckBox filterVisibleOnly;
        private System.Windows.Forms.TextBox filterName;
        private System.Windows.Forms.Button btnFill;
        private System.Windows.Forms.Label lblFilterName;
    }
}