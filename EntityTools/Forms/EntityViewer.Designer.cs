﻿namespace EntityTools.Forms
{
    partial class EntityViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dgvEntities = new System.Windows.Forms.DataGridView();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnInternalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnNameUntranslated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReload = new System.Windows.Forms.Button();
            this.cbNameType = new System.Windows.Forms.ComboBox();
            this.cbStrMatch = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.tbPattern = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntities)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSelect.FlatAppearance.BorderSize = 0;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Image = global::EntityTools.Properties.Resources.Valid;
            this.btnSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelect.Location = new System.Drawing.Point(643, 433);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(71, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select";
            this.btnSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Image = global::EntityTools.Properties.Resources.Cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(720, 433);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvEntities
            // 
            this.dgvEntities.AllowDrop = true;
            this.dgvEntities.AllowUserToAddRows = false;
            this.dgvEntities.AllowUserToDeleteRows = false;
            this.dgvEntities.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.dgvEntities.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEntities.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvEntities.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvEntities.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvEntities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvEntities.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnName,
            this.clmnInternalName,
            this.clmnNameUntranslated,
            this.clmnDistance});
            this.dgvEntities.Location = new System.Drawing.Point(13, 13);
            this.dgvEntities.MultiSelect = false;
            this.dgvEntities.Name = "dgvEntities";
            this.dgvEntities.ReadOnly = true;
            this.dgvEntities.RowHeadersVisible = false;
            this.dgvEntities.RowHeadersWidth = 20;
            this.dgvEntities.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvEntities.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvEntities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEntities.ShowEditingIcon = false;
            this.dgvEntities.ShowRowErrors = false;
            this.dgvEntities.Size = new System.Drawing.Size(772, 382);
            this.dgvEntities.TabIndex = 1;
            this.dgvEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvEntities_MouseDown);
            // 
            // clmnName
            // 
            this.clmnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnName.HeaderText = "Name";
            this.clmnName.Name = "clmnName";
            this.clmnName.ReadOnly = true;
            this.clmnName.Width = 200;
            // 
            // clmnInternalName
            // 
            this.clmnInternalName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnInternalName.HeaderText = "InternalName";
            this.clmnInternalName.Name = "clmnInternalName";
            this.clmnInternalName.ReadOnly = true;
            this.clmnInternalName.Width = 200;
            // 
            // clmnNameUntranslated
            // 
            this.clmnNameUntranslated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clmnNameUntranslated.HeaderText = "NameUntranslated";
            this.clmnNameUntranslated.Name = "clmnNameUntranslated";
            this.clmnNameUntranslated.ReadOnly = true;
            // 
            // clmnDistance
            // 
            this.clmnDistance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.clmnDistance.HeaderText = "Distance";
            this.clmnDistance.Name = "clmnDistance";
            this.clmnDistance.ReadOnly = true;
            this.clmnDistance.Width = 74;
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnReload.FlatAppearance.BorderSize = 0;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Image = global::EntityTools.Properties.Resources.Refresh;
            this.btnReload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReload.Location = new System.Drawing.Point(643, 404);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(71, 23);
            this.btnReload.TabIndex = 0;
            this.btnReload.Text = "Reload";
            this.btnReload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // cbNameType
            // 
            this.cbNameType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbNameType.BackColor = System.Drawing.SystemColors.Window;
            this.cbNameType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNameType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbNameType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbNameType.Items.AddRange(new object[] {
            "NameUntranslated"});
            this.cbNameType.Location = new System.Drawing.Point(13, 406);
            this.cbNameType.Name = "cbNameType";
            this.cbNameType.Size = new System.Drawing.Size(130, 21);
            this.cbNameType.TabIndex = 4;
            // 
            // cbStrMatch
            // 
            this.cbStrMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbStrMatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStrMatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbStrMatch.Location = new System.Drawing.Point(13, 435);
            this.cbStrMatch.Name = "cbStrMatch";
            this.cbStrMatch.Size = new System.Drawing.Size(130, 21);
            this.cbStrMatch.TabIndex = 4;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnTest.FlatAppearance.BorderSize = 0;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Image = global::EntityTools.Properties.Resources.Play;
            this.btnTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTest.Location = new System.Drawing.Point(720, 404);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(65, 23);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Test   ";
            this.btnTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tbPattern
            // 
            this.tbPattern.AllowDrop = true;
            this.tbPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPattern.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPattern.Location = new System.Drawing.Point(149, 406);
            this.tbPattern.Multiline = true;
            this.tbPattern.Name = "tbPattern";
            this.tbPattern.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPattern.Size = new System.Drawing.Size(488, 50);
            this.tbPattern.TabIndex = 3;
            this.tbPattern.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbPattern_DragDrop);
            this.tbPattern.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbPattern_DragEnter);
            // 
            // EntitySelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 469);
            this.ControlBox = false;
            this.Controls.Add(this.cbStrMatch);
            this.Controls.Add(this.cbNameType);
            this.Controls.Add(this.tbPattern);
            this.Controls.Add(this.dgvEntities);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(486, 293);
            this.Name = "EntitySelectForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EntitySelectForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntities)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvEntities;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.ComboBox cbNameType;
        private System.Windows.Forms.ComboBox cbStrMatch;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnInternalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnNameUntranslated;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnDistance;
        private System.Windows.Forms.TextBox tbPattern;
    }
}