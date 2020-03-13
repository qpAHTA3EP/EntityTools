using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EntityTools.Forms
{
    partial class PositionEditorForm
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
            this.position = new DevExpress.XtraEditors.TextEdit();
            this.b_PlayerPos = new DevExpress.XtraEditors.SimpleButton();
            this.b_TargetPos = new DevExpress.XtraEditors.SimpleButton();
            this.b_Valid = new DevExpress.XtraEditors.SimpleButton();
            this.defaultLookAndFeel = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.position.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // position
            // 
            this.position.Location = new System.Drawing.Point(17, 12);
            this.position.Name = "position";
            this.position.Properties.ReadOnly = true;
            this.position.Size = new System.Drawing.Size(209, 20);
            this.position.TabIndex = 0;
            // 
            // b_PlayerPos
            // 
            this.b_PlayerPos.Location = new System.Drawing.Point(17, 38);
            this.b_PlayerPos.Name = "b_PlayerPos";
            this.b_PlayerPos.Size = new System.Drawing.Size(209, 23);
            this.b_PlayerPos.TabIndex = 1;
            this.b_PlayerPos.Text = "Use player position";
            this.b_PlayerPos.Click += new System.EventHandler(this.b_PlayerPos_Click);
            // 
            // b_TargetPos
            // 
            this.b_TargetPos.Location = new System.Drawing.Point(17, 68);
            this.b_TargetPos.Name = "b_TargetPos";
            this.b_TargetPos.Size = new System.Drawing.Size(209, 23);
            this.b_TargetPos.TabIndex = 2;
            this.b_TargetPos.Text = "Use target position";
            this.b_TargetPos.Click += new System.EventHandler(this.b_TargetPos_Click);
            // 
            // b_Valid
            // 
            this.b_Valid.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_Valid.Appearance.Options.UseFont = true;
            this.b_Valid.Location = new System.Drawing.Point(17, 97);
            this.b_Valid.Name = "b_Valid";
            this.b_Valid.Size = new System.Drawing.Size(209, 23);
            this.b_Valid.TabIndex = 3;
            this.b_Valid.Text = "Valid";
            this.b_Valid.Click += new System.EventHandler(this.b_Valid_Click);
            // 
            // defaultLookAndFeel
            // 
            this.defaultLookAndFeel.LookAndFeel.SkinName = "Office 2013 Light Gray";
            // 
            // PositionEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 132);
            this.Controls.Add(this.b_Valid);
            this.Controls.Add(this.b_TargetPos);
            this.Controls.Add(this.b_PlayerPos);
            this.Controls.Add(this.position);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(257, 171);
            this.MinimumSize = new System.Drawing.Size(257, 142);
            this.Name = "PositionEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit position";
            ((System.ComponentModel.ISupportInitialize)(this.position.Properties)).EndInit();
            this.ResumeLayout(false);

        }
        private TextEdit position;
        private SimpleButton b_PlayerPos;
        private SimpleButton b_TargetPos;
        private SimpleButton b_Valid;
        #endregion

        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel;
    }
}