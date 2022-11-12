namespace EntityCore.Forms
{
    partial class AddUccActionForm
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listOfActionTypes = new System.Windows.Forms.ListBox();
            this.actionProperties = new System.Windows.Forms.PropertyGrid();
            this.bntCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listOfActionTypes);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.actionProperties);
            this.splitContainer.Size = new System.Drawing.Size(594, 313);
            this.splitContainer.SplitterDistance = 204;
            this.splitContainer.TabIndex = 3;
            // 
            // listOfActionTypes
            // 
            this.listOfActionTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listOfActionTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listOfActionTypes.Location = new System.Drawing.Point(0, 0);
            this.listOfActionTypes.Name = "listOfActionTypes";
            this.listOfActionTypes.Size = new System.Drawing.Size(204, 313);
            this.listOfActionTypes.Sorted = true;
            this.listOfActionTypes.TabIndex = 0;
            this.listOfActionTypes.SelectedIndexChanged += new System.EventHandler(this.ActionList_SelectedIndexChanged);
            // 
            // actionProperties
            // 
            this.actionProperties.CommandsVisibleIfAvailable = false;
            this.actionProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionProperties.Location = new System.Drawing.Point(0, 0);
            this.actionProperties.Name = "actionProperties";
            this.actionProperties.Size = new System.Drawing.Size(386, 313);
            this.actionProperties.TabIndex = 1;
            // 
            // bntCancel
            // 
            this.bntCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bntCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.bntCancel.FlatAppearance.BorderSize = 0;
            this.bntCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntCancel.Image = global::EntityTools.Properties.Resources.Cancel;
            this.bntCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bntCancel.Location = new System.Drawing.Point(538, 338);
            this.bntCancel.Name = "bntCancel";
            this.bntCancel.Size = new System.Drawing.Size(68, 23);
            this.bntCancel.TabIndex = 4;
            this.bntCancel.Text = "Cancel";
            this.bntCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntCancel.UseVisualStyleBackColor = true;
            this.bntCancel.Click += new System.EventHandler(this.bntCancel_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSelect.FlatAppearance.BorderSize = 0;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Image = global::EntityTools.Properties.Resources.Valid;
            this.btnSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelect.Location = new System.Drawing.Point(464, 338);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(68, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // AddUccActionForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bntCancel;
            this.ClientSize = new System.Drawing.Size(618, 373);
            this.ControlBox = false;
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.bntCancel);
            this.Controls.Add(this.splitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(598, 383);
            this.Name = "AddUccActionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ucc Actions";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.PropertyGrid actionProperties;
        private System.Windows.Forms.ListBox listOfActionTypes;
        private System.Windows.Forms.Button bntCancel;
        private System.Windows.Forms.Button btnSelect;
    }
}