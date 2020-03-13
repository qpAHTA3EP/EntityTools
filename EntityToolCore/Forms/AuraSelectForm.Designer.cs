using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Astral.Controllers;
using DevExpress.XtraEditors;
using MyNW.Classes;

namespace EntityCore.Forms
{
    partial class AuraSelectForm
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
            this.lblEntitySelector = new DevExpress.XtraEditors.LabelControl();
            this.btnTarget = new DevExpress.XtraEditors.SimpleButton();
            this.unitRefName = new DevExpress.XtraEditors.LabelControl();
            this.Auras = new DevExpress.XtraEditors.ListBoxControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnPlayer = new DevExpress.XtraEditors.SimpleButton();
            this.btnEntity = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.Selector = new System.Windows.Forms.ComboBox();
            this.NameFilter = new System.Windows.Forms.TextBox();
            this.InternalNameFilter = new System.Windows.Forms.TextBox();
            this.lblFilters = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.Description = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Auras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEntitySelector
            // 
            this.lblEntitySelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEntitySelector.Location = new System.Drawing.Point(12, 12);
            this.lblEntitySelector.Name = "lblEntitySelector";
            this.lblEntitySelector.Size = new System.Drawing.Size(196, 13);
            this.lblEntitySelector.TabIndex = 0;
            this.lblEntitySelector.Text = "1. Select entity which to search an Aura:";
            // 
            // btnTarget
            // 
            this.btnTarget.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.btnTarget.Location = new System.Drawing.Point(115, 31);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(89, 23);
            this.btnTarget.TabIndex = 1;
            this.btnTarget.Text = "Use target";
            this.btnTarget.Visible = false;
            this.btnTarget.Click += new System.EventHandler(this.btnTarget_Click);
            // 
            // unitRefName
            // 
            this.unitRefName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.unitRefName.Appearance.Options.UseTextOptions = true;
            this.unitRefName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.unitRefName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.unitRefName.Location = new System.Drawing.Point(12, 36);
            this.unitRefName.Name = "unitRefName";
            this.unitRefName.Size = new System.Drawing.Size(368, 21);
            this.unitRefName.TabIndex = 2;
            this.unitRefName.Text = "-";
            // 
            // Auras
            // 
            this.Auras.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Auras.Location = new System.Drawing.Point(0, 0);
            this.Auras.Name = "Auras";
            this.Auras.Size = new System.Drawing.Size(368, 190);
            this.Auras.TabIndex = 3;
            this.Auras.SelectedIndexChanged += new System.EventHandler(this.lbAuras_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ImageOptions.Image = global::EntityTools.Properties.Resources.miniValid;
            this.btnSelect.Location = new System.Drawing.Point(306, 391);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnPlayer
            // 
            this.btnPlayer.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.btnPlayer.Location = new System.Drawing.Point(20, 31);
            this.btnPlayer.Name = "btnPlayer";
            this.btnPlayer.Size = new System.Drawing.Size(89, 23);
            this.btnPlayer.TabIndex = 6;
            this.btnPlayer.Text = "Use player";
            this.btnPlayer.Visible = false;
            this.btnPlayer.Click += new System.EventHandler(this.btnPlayer_Click);
            // 
            // btnEntity
            // 
            this.btnEntity.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.btnEntity.Location = new System.Drawing.Point(210, 31);
            this.btnEntity.Name = "btnEntity";
            this.btnEntity.Size = new System.Drawing.Size(89, 23);
            this.btnEntity.TabIndex = 1;
            this.btnEntity.Text = "Use Entity";
            this.btnEntity.Visible = false;
            this.btnEntity.Click += new System.EventHandler(this.btnEntity_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRefresh;
            this.btnReload.Location = new System.Drawing.Point(12, 391);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(74, 23);
            this.btnReload.TabIndex = 4;
            this.btnReload.Text = "Reload";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // Selector
            // 
            this.Selector.FormattingEnabled = true;
            this.Selector.Location = new System.Drawing.Point(227, 9);
            this.Selector.Name = "Selector";
            this.Selector.Size = new System.Drawing.Size(153, 21);
            this.Selector.TabIndex = 7;
            this.Selector.SelectedIndexChanged += new System.EventHandler(this.Selector_SelectedIndexChanged);
            // 
            // NameFilter
            // 
            this.NameFilter.Location = new System.Drawing.Point(12, 84);
            this.NameFilter.Name = "NameFilter";
            this.NameFilter.Size = new System.Drawing.Size(181, 21);
            this.NameFilter.TabIndex = 8;
            // 
            // InternalNameFilter
            // 
            this.InternalNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InternalNameFilter.Location = new System.Drawing.Point(199, 84);
            this.InternalNameFilter.Name = "InternalNameFilter";
            this.InternalNameFilter.Size = new System.Drawing.Size(181, 21);
            this.InternalNameFilter.TabIndex = 8;
            // 
            // lblFilters
            // 
            this.lblFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilters.Location = new System.Drawing.Point(12, 65);
            this.lblFilters.Name = "lblFilters";
            this.lblFilters.Size = new System.Drawing.Size(159, 13);
            this.lblFilters.TabIndex = 0;
            this.lblFilters.Text = "2. Filters (Name | InternalName):";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 111);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.Auras);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.Description);
            this.splitContainer.Size = new System.Drawing.Size(368, 267);
            this.splitContainer.SplitterDistance = 190;
            this.splitContainer.TabIndex = 10;
            // 
            // Description
            // 
            this.Description.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Description.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Description.Location = new System.Drawing.Point(0, 0);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new System.Drawing.Size(368, 73);
            this.Description.TabIndex = 9;
            // 
            // AuraSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 426);
            this.Controls.Add(this.unitRefName);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.InternalNameFilter);
            this.Controls.Add(this.NameFilter);
            this.Controls.Add(this.Selector);
            this.Controls.Add(this.btnPlayer);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnEntity);
            this.Controls.Add(this.btnTarget);
            this.Controls.Add(this.lblFilters);
            this.Controls.Add(this.lblEntitySelector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximumSize = new System.Drawing.Size(400, 700);
            this.Name = "AuraSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Aura";
            this.Load += new System.EventHandler(this.AuraSelectForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Auras)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Thread thread_0;
        private Entity entity_0 = new Entity(IntPtr.Zero);
        private bool bool_0;
        private LabelControl lblEntitySelector;
        private SimpleButton btnTarget;
        private LabelControl unitRefName;
        private ListBoxControl Auras;
        private SimpleButton btnSelect;
        private SimpleButton btnPlayer;
        private SimpleButton btnEntity;
        private SimpleButton btnReload;
        private System.Windows.Forms.ComboBox Selector;
        private TextBox NameFilter;
        private TextBox InternalNameFilter;
        private LabelControl lblFilters;
        private SplitContainer splitContainer;
        private TextBox Description;
    }
}