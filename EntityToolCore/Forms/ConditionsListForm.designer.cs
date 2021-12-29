namespace EntityCore.Forms
{
    partial class ConditionListForm
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
            this.cheNegation = new DevExpress.XtraEditors.CheckEdit();
            this.cbLogic = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lsbxConditions = new System.Windows.Forms.CheckedListBox();
            this.Properties = new System.Windows.Forms.PropertyGrid();
            this.bntAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.bntCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnTestAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cheNegation.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogic.Properties)).BeginInit();
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
            this.splitContainer.Panel1.Controls.Add(this.cheNegation);
            this.splitContainer.Panel1.Controls.Add(this.cbLogic);
            this.splitContainer.Panel1.Controls.Add(this.lsbxConditions);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.Properties);
            this.splitContainer.Size = new System.Drawing.Size(594, 305);
            this.splitContainer.SplitterDistance = 251;
            this.splitContainer.TabIndex = 3;
            // 
            // cheNegation
            // 
            this.cheNegation.Location = new System.Drawing.Point(159, 0);
            this.cheNegation.Name = "cheNegation";
            this.cheNegation.Properties.Caption = "Negation";
            this.cheNegation.Size = new System.Drawing.Size(135, 20);
            this.cheNegation.TabIndex = 2;
            // 
            // cbLogic
            // 
            this.cbLogic.Location = new System.Drawing.Point(0, 0);
            this.cbLogic.Name = "cbLogic";
            this.cbLogic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbLogic.Size = new System.Drawing.Size(153, 20);
            this.cbLogic.TabIndex = 1;
            // 
            // lsbxConditions
            // 
            this.lsbxConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsbxConditions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lsbxConditions.Location = new System.Drawing.Point(0, 30);
            this.lsbxConditions.Name = "lsbxConditions";
            this.lsbxConditions.Size = new System.Drawing.Size(250, 274);
            this.lsbxConditions.TabIndex = 0;
            this.lsbxConditions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.handler_LockCondition);
            this.lsbxConditions.SelectedIndexChanged += new System.EventHandler(this.handler_SelectedConditionChanged);
            // 
            // Properties
            // 
            this.Properties.CommandsVisibleIfAvailable = false;
            this.Properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Properties.Location = new System.Drawing.Point(0, 0);
            this.Properties.Name = "Properties";
            this.Properties.Size = new System.Drawing.Size(339, 305);
            this.Properties.TabIndex = 1;
            this.Properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.handler_ConditionPropertyValueChanged);
            // 
            // bntAdd
            // 
            this.bntAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bntAdd.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.bntAdd.FlatAppearance.BorderSize = 0;
            this.bntAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntAdd.Image = global::EntityCore.Properties.Resources.miniAdd;
            this.bntAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bntAdd.Location = new System.Drawing.Point(13, 330);
            this.bntAdd.Name = "bntAdd";
            this.bntAdd.Size = new System.Drawing.Size(58, 23);
            this.bntAdd.TabIndex = 4;
            this.bntAdd.Text = "Add";
            this.bntAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntAdd.UseVisualStyleBackColor = true;
            this.bntAdd.Click += new System.EventHandler(this.handler_Add);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnRemove.FlatAppearance.BorderSize = 0;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Image = global::EntityCore.Properties.Resources.miniDelete;
            this.btnRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemove.Location = new System.Drawing.Point(77, 330);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(74, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.handler_Remove);
            // 
            // bntCancel
            // 
            this.bntCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bntCancel.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.bntCancel.FlatAppearance.BorderSize = 0;
            this.bntCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntCancel.Image = global::EntityCore.Properties.Resources.miniCancel;
            this.bntCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bntCancel.Location = new System.Drawing.Point(538, 330);
            this.bntCancel.Name = "bntCancel";
            this.bntCancel.Size = new System.Drawing.Size(68, 23);
            this.bntCancel.TabIndex = 4;
            this.bntCancel.Text = "Cancel";
            this.bntCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bntCancel.UseVisualStyleBackColor = true;
            this.bntCancel.Click += new System.EventHandler(this.handler_Cancel);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Image = global::EntityCore.Properties.Resources.miniValid;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(472, 330);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.handler_Save);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTest.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnTest.FlatAppearance.BorderSize = 0;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Image = global::EntityCore.Properties.Resources.miniPlay;
            this.btnTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTest.Location = new System.Drawing.Point(295, 330);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(58, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.handler_Test);
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaste.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnPaste.FlatAppearance.BorderSize = 0;
            this.btnPaste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPaste.Image = global::EntityCore.Properties.Resources.miniPaste;
            this.btnPaste.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPaste.Location = new System.Drawing.Point(225, 330);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(64, 23);
            this.btnPaste.TabIndex = 4;
            this.btnPaste.Text = "Paste";
            this.btnPaste.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.handler_Paste);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Image = global::EntityCore.Properties.Resources.miniCopy;
            this.btnCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopy.Location = new System.Drawing.Point(157, 330);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(62, 23);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "Copy";
            this.btnCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.handler_Copy);
            // 
            // btnTestAll
            // 
            this.btnTestAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTestAll.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnTestAll.FlatAppearance.BorderSize = 0;
            this.btnTestAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestAll.Image = global::EntityCore.Properties.Resources.miniPlayAll;
            this.btnTestAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTestAll.Location = new System.Drawing.Point(359, 330);
            this.btnTestAll.Name = "btnTestAll";
            this.btnTestAll.Size = new System.Drawing.Size(68, 23);
            this.btnTestAll.TabIndex = 4;
            this.btnTestAll.Text = "TestAll";
            this.btnTestAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTestAll.UseVisualStyleBackColor = true;
            this.btnTestAll.Click += new System.EventHandler(this.handler_TestAll);
            // 
            // ConditionListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 365);
            this.ControlBox = false;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.bntCancel);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnTestAll);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.bntAdd);
            this.Controls.Add(this.splitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(598, 383);
            this.Name = "ConditionListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConditionList";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cheNegation.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbLogic.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.PropertyGrid Properties;
        private System.Windows.Forms.CheckedListBox lsbxConditions;
        private System.Windows.Forms.Button bntAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button bntCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnTestAll;
        private DevExpress.XtraEditors.CheckEdit cheNegation;
        private DevExpress.XtraEditors.ComboBoxEdit cbLogic;
    }
}