
namespace AssemblyComparer
{
    partial class AssemblyComparerForm
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblFile1 = new System.Windows.Forms.Label();
            this.lblFile2 = new System.Windows.Forms.Label();
            this.btnFile2 = new System.Windows.Forms.Button();
            this.btnFile1 = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.tbFile2 = new System.Windows.Forms.TextBox();
            this.tbFile1 = new System.Windows.Forms.TextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "exe|*.exe|dll|*.dll";
            // 
            // lblFile1
            // 
            this.lblFile1.AutoSize = true;
            this.lblFile1.Location = new System.Drawing.Point(3, 0);
            this.lblFile1.Name = "lblFile1";
            this.lblFile1.Size = new System.Drawing.Size(45, 13);
            this.lblFile1.TabIndex = 1;
            this.lblFile1.Text = "First File";
            // 
            // lblFile2
            // 
            this.lblFile2.AutoSize = true;
            this.lblFile2.Location = new System.Drawing.Point(3, 0);
            this.lblFile2.Name = "lblFile2";
            this.lblFile2.Size = new System.Drawing.Size(63, 13);
            this.lblFile2.TabIndex = 2;
            this.lblFile2.Text = "Second File";
            // 
            // btnFile2
            // 
            this.btnFile2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFile2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFile2.Location = new System.Drawing.Point(128, 127);
            this.btnFile2.Name = "btnFile2";
            this.btnFile2.Size = new System.Drawing.Size(110, 23);
            this.btnFile2.TabIndex = 3;
            this.btnFile2.Text = "Select Second File";
            this.btnFile2.UseVisualStyleBackColor = true;
            this.btnFile2.Click += new System.EventHandler(this.btnFile2_Click);
            // 
            // btnFile1
            // 
            this.btnFile1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFile1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFile1.Location = new System.Drawing.Point(12, 127);
            this.btnFile1.Name = "btnFile1";
            this.btnFile1.Size = new System.Drawing.Size(110, 23);
            this.btnFile1.TabIndex = 4;
            this.btnFile1.Text = "Select First File";
            this.btnFile1.UseVisualStyleBackColor = true;
            this.btnFile1.Click += new System.EventHandler(this.btnFile1_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Enabled = false;
            this.btnCompare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompare.Location = new System.Drawing.Point(287, 127);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(75, 23);
            this.btnCompare.TabIndex = 3;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // tbFile2
            // 
            this.tbFile2.AllowDrop = true;
            this.tbFile2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFile2.Location = new System.Drawing.Point(3, 16);
            this.tbFile2.Multiline = true;
            this.tbFile2.Name = "tbFile2";
            this.tbFile2.ReadOnly = true;
            this.tbFile2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFile2.Size = new System.Drawing.Size(344, 34);
            this.tbFile2.TabIndex = 0;
            // 
            // tbFile1
            // 
            this.tbFile1.AllowDrop = true;
            this.tbFile1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFile1.Location = new System.Drawing.Point(3, 16);
            this.tbFile1.Multiline = true;
            this.tbFile1.Name = "tbFile1";
            this.tbFile1.ReadOnly = true;
            this.tbFile1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFile1.Size = new System.Drawing.Size(344, 33);
            this.tbFile1.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.lblFile1);
            this.splitContainer.Panel1.Controls.Add(this.tbFile1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.lblFile2);
            this.splitContainer.Panel2.Controls.Add(this.tbFile2);
            this.splitContainer.Size = new System.Drawing.Size(350, 109);
            this.splitContainer.SplitterDistance = 52;
            this.splitContainer.TabIndex = 5;
            // 
            // AssemblyComparerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 162);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.btnFile1);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnFile2);
            this.MinimumSize = new System.Drawing.Size(390, 200);
            this.Name = "AssemblyComparerForm";
            this.Text = "AssemplyComparer";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lblFile1;
        private System.Windows.Forms.Label lblFile2;
        private System.Windows.Forms.Button btnFile2;
        private System.Windows.Forms.Button btnFile1;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.TextBox tbFile2;
        private System.Windows.Forms.TextBox tbFile1;
        private System.Windows.Forms.SplitContainer splitContainer;
    }
}

