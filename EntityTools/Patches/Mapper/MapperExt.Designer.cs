using Astral.Logic.Classes.Map;
using DevExpress.XtraEditors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper
{
    partial class MapperExt
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MapPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // MapPicture
            // 
            this.MapPicture.Cursor = System.Windows.Forms.Cursors.Cross;
            this.MapPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPicture.Location = new System.Drawing.Point(0, 0);
            this.MapPicture.Name = "MapPicture";
            this.MapPicture.Size = new System.Drawing.Size(360, 360);
            this.MapPicture.TabIndex = 0;
            this.MapPicture.TabStop = false;
            this.MapPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.handler_MouseClick);
            this.MapPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.handler_MouseMove);
            this.MapPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.handler_MouseUp);
            // 
            // MapperExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MapPicture);
            this.Name = "MapperExt";
            this.Size = new System.Drawing.Size(360, 360);
            this.Load += new System.EventHandler(this.handler_Load);
            this.VisibleChanged += new System.EventHandler(this.handler_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.handler_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.handler_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.handler_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.handler_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.handler_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private Vector3 objectPosition;
        private Point mousePointClickPosition = Point.Empty;
        private PictureBox MapPicture;

#if false
        [CompilerGenerated]
        private Action<GraphicsNW> action_0;
        [CompilerGenerated]
        private Action<MouseEventArgs, GraphicsNW> action_1; 
#endif
    }
}
