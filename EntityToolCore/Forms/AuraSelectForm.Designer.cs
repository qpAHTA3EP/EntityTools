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
            this.unitRefName = new DevExpress.XtraEditors.LabelControl();
            this.Auras = new DevExpress.XtraEditors.ListBoxControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnReload = new DevExpress.XtraEditors.SimpleButton();
            this.Selector = new System.Windows.Forms.ComboBox();
            this.NameFilter = new System.Windows.Forms.TextBox();
            this.InternalNameFilter = new System.Windows.Forms.TextBox();
            this.lblFilters = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.Description = new System.Windows.Forms.TextBox();
            this.ckbNewAuras = new System.Windows.Forms.CheckBox();
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
            // unitRefName
            // 
            this.unitRefName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.unitRefName.Appearance.Options.UseTextOptions = true;
            this.unitRefName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.unitRefName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.unitRefName.Location = new System.Drawing.Point(12, 36);
            this.unitRefName.Name = "unitRefName";
            this.unitRefName.Size = new System.Drawing.Size(360, 21);
            this.unitRefName.TabIndex = 2;
            this.unitRefName.Text = "-";
            // 
            // Auras
            // 
            this.Auras.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Auras.Location = new System.Drawing.Point(0, 0);
            this.Auras.Name = "Auras";
            this.Auras.Size = new System.Drawing.Size(360, 190);
            this.Auras.TabIndex = 3;
            this.Auras.SelectedIndexChanged += new System.EventHandler(this.lbAuras_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ImageOptions.Image = global::EntityCore.Properties.Resources.miniValid;
            this.btnSelect.Location = new System.Drawing.Point(298, 391);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.ImageOptions.Image = global::EntityCore.Properties.Resources.miniRefresh;
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
            this.Selector.Location = new System.Drawing.Point(214, 9);
            this.Selector.Name = "Selector";
            this.Selector.Size = new System.Drawing.Size(158, 21);
            this.Selector.TabIndex = 7;
            this.Selector.SelectedIndexChanged += new System.EventHandler(this.Selector_SelectedIndexChanged);
            // 
            // NameFilter
            // 
            this.NameFilter.Location = new System.Drawing.Point(12, 84);
            this.NameFilter.Name = "NameFilter";
            this.NameFilter.Size = new System.Drawing.Size(172, 21);
            this.NameFilter.TabIndex = 8;
            // 
            // InternalNameFilter
            // 
            this.InternalNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InternalNameFilter.Location = new System.Drawing.Point(190, 84);
            this.InternalNameFilter.Name = "InternalNameFilter";
            this.InternalNameFilter.Size = new System.Drawing.Size(182, 21);
            this.InternalNameFilter.TabIndex = 8;
            // 
            // lblFilters
            // 
            this.lblFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFilters.Location = new System.Drawing.Point(12, 65);
            this.lblFilters.Name = "lblFilters";
            this.lblFilters.Size = new System.Drawing.Size(193, 13);
            this.lblFilters.TabIndex = 0;
            this.lblFilters.Text = "2. Filters (DisplayName | InternalName):";
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
            this.splitContainer.Size = new System.Drawing.Size(360, 267);
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
            this.Description.Size = new System.Drawing.Size(360, 73);
            this.Description.TabIndex = 9;
            // 
            // ckbNewAuras
            // 
            this.ckbNewAuras.AutoSize = true;
            this.ckbNewAuras.Location = new System.Drawing.Point(236, 65);
            this.ckbNewAuras.Name = "ckbNewAuras";
            this.ckbNewAuras.Size = new System.Drawing.Size(136, 17);
            this.ckbNewAuras.TabIndex = 11;
            this.ckbNewAuras.Text = "Display new auras only";
            this.ckbNewAuras.UseVisualStyleBackColor = true;
            this.ckbNewAuras.CheckedChanged += new System.EventHandler(this.ckbNewAuras_CheckedChanged);
            // 
            // AuraSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 426);
            this.Controls.Add(this.ckbNewAuras);
            this.Controls.Add(this.unitRefName);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.InternalNameFilter);
            this.Controls.Add(this.NameFilter);
            this.Controls.Add(this.Selector);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
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

        private LabelControl lblEntitySelector;
        private LabelControl unitRefName;
        private ListBoxControl Auras;
        private SimpleButton btnSelect;
        private SimpleButton btnReload;
        private System.Windows.Forms.ComboBox Selector;
        private TextBox NameFilter;
        private TextBox InternalNameFilter;
        private LabelControl lblFilters;
        private SplitContainer splitContainer;
        private TextBox Description;
        private CheckBox ckbNewAuras;
    }
}