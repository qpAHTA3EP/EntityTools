namespace EntityTools.Forms
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
            this.cbSort = new System.Windows.Forms.CheckBox();
            this.lblGenTime = new System.Windows.Forms.Label();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblCommand = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvInterfaces
            // 
            this.tvInterfaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvInterfaces.FullRowSelect = true;
            this.tvInterfaces.Location = new System.Drawing.Point(0, 0);
            this.tvInterfaces.Name = "tvInterfaces";
            this.tvInterfaces.Size = new System.Drawing.Size(310, 301);
            this.tvInterfaces.TabIndex = 0;
            this.tvInterfaces.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvInterfaces_AfterSelect);
            // 
            // pgProperties
            // 
            this.pgProperties.CommandsVisibleIfAvailable = false;
            this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProperties.HelpVisible = false;
            this.pgProperties.Location = new System.Drawing.Point(0, 0);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(307, 301);
            this.pgProperties.TabIndex = 1;
            this.pgProperties.ToolbarVisible = false;
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
            this.splitContainer.Size = new System.Drawing.Size(621, 301);
            this.splitContainer.SplitterDistance = 310;
            this.splitContainer.TabIndex = 2;
            // 
            // filterVisibleOnly
            // 
            this.filterVisibleOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterVisibleOnly.AutoSize = true;
            this.filterVisibleOnly.Checked = true;
            this.filterVisibleOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterVisibleOnly.Location = new System.Drawing.Point(329, 15);
            this.filterVisibleOnly.Name = "filterVisibleOnly";
            this.filterVisibleOnly.Size = new System.Drawing.Size(78, 17);
            this.filterVisibleOnly.TabIndex = 3;
            this.filterVisibleOnly.Text = "Visible only";
            this.filterVisibleOnly.UseVisualStyleBackColor = true;
            // 
            // filterName
            // 
            this.filterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterName.Location = new System.Drawing.Point(114, 13);
            this.filterName.Name = "filterName";
            this.filterName.Size = new System.Drawing.Size(209, 21);
            this.filterName.TabIndex = 4;
            this.filterName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filterName_KeyPress);
            // 
            // btnFill
            // 
            this.btnFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFill.Image = global::EntityTools.Properties.Resources.Refresh;
            this.btnFill.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFill.Location = new System.Drawing.Point(558, 11);
            this.btnFill.Name = "btnFill";
            this.btnFill.Size = new System.Drawing.Size(75, 23);
            this.btnFill.TabIndex = 5;
            this.btnFill.Text = "Refresh";
            this.btnFill.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFill.UseVisualStyleBackColor = true;
            this.btnFill.Click += new System.EventHandler(this.Refresh);
            // 
            // lblFilterName
            // 
            this.lblFilterName.AutoSize = true;
            this.lblFilterName.Location = new System.Drawing.Point(12, 16);
            this.lblFilterName.Name = "lblFilterName";
            this.lblFilterName.Size = new System.Drawing.Size(102, 13);
            this.lblFilterName.TabIndex = 6;
            this.lblFilterName.Text = "Filter by name part:";
            // 
            // cbSort
            // 
            this.cbSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSort.AutoSize = true;
            this.cbSort.Checked = true;
            this.cbSort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSort.Location = new System.Drawing.Point(412, 17);
            this.cbSort.Name = "cbSort";
            this.cbSort.Size = new System.Drawing.Size(46, 17);
            this.cbSort.TabIndex = 3;
            this.cbSort.Text = "Sort";
            this.cbSort.UseVisualStyleBackColor = true;
            // 
            // lblGenTime
            // 
            this.lblGenTime.AutoSize = true;
            this.lblGenTime.Location = new System.Drawing.Point(464, 18);
            this.lblGenTime.Name = "lblGenTime";
            this.lblGenTime.Size = new System.Drawing.Size(0, 13);
            this.lblGenTime.TabIndex = 7;
            // 
            // tbCommand
            // 
            this.tbCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCommand.Location = new System.Drawing.Point(114, 367);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(438, 21);
            this.tbCommand.TabIndex = 8;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.Image = global::EntityTools.Properties.Resources.Play;
            this.btnExecute.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExecute.Location = new System.Drawing.Point(558, 365);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 5;
            this.btnExecute.Text = "Execute";
            this.btnExecute.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblCommand
            // 
            this.lblCommand.AutoSize = true;
            this.lblCommand.Location = new System.Drawing.Point(11, 370);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(97, 13);
            this.lblCommand.TabIndex = 6;
            this.lblCommand.Text = "Console command:";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Image = global::EntityTools.Properties.Resources.Valid;
            this.btnSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelect.Location = new System.Drawing.Point(491, 404);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(68, 23);
            this.btnSelect.TabIndex = 9;
            this.btnSelect.Text = "Select";
            this.btnSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Image = global::EntityTools.Properties.Resources.Cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(565, 404);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // UIViewer
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(645, 439);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.tbCommand);
            this.Controls.Add(this.lblGenTime);
            this.Controls.Add(this.lblCommand);
            this.Controls.Add(this.lblFilterName);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnFill);
            this.Controls.Add(this.filterName);
            this.Controls.Add(this.cbSort);
            this.Controls.Add(this.filterVisibleOnly);
            this.Controls.Add(this.splitContainer);
            this.Name = "UIViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.CheckBox cbSort;
        private System.Windows.Forms.Label lblGenTime;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
    }
}