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
            this.NameFilter = new System.Windows.Forms.TextBox();
            this.InternalNameFilter = new System.Windows.Forms.TextBox();
            this.lblFilters = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.Description = new System.Windows.Forms.TextBox();
            this.ckbAuraInspectionMode = new System.Windows.Forms.CheckBox();
            this.Selector = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.Auras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Selector.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEntitySelector
            // 
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
            this.Auras.Size = new System.Drawing.Size(360, 189);
            this.Auras.TabIndex = 3;
            this.Auras.SelectedIndexChanged += new System.EventHandler(this.handler_SelectedAuraChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Appearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnSelect.Appearance.Options.UseBorderColor = true;
            this.btnSelect.ImageOptions.Image = global::EntityCore.Properties.Resources.miniValid;
            this.btnSelect.Location = new System.Drawing.Point(298, 383);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.handler_SelectAura);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.Appearance.BorderColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnReload.Appearance.Options.UseBorderColor = true;
            this.btnReload.ImageOptions.Image = global::EntityCore.Properties.Resources.miniRefresh;
            this.btnReload.Location = new System.Drawing.Point(12, 383);
            this.btnReload.Name = "btnReload";
            this.btnReload.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnReload.Size = new System.Drawing.Size(74, 23);
            this.btnReload.TabIndex = 4;
            this.btnReload.Text = "Reload";
            this.btnReload.Click += new System.EventHandler(this.handler_Reload);
            // 
            // NameFilter
            // 
            this.NameFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameFilter.Location = new System.Drawing.Point(12, 84);
            this.NameFilter.Name = "NameFilter";
            this.NameFilter.Size = new System.Drawing.Size(180, 21);
            this.NameFilter.TabIndex = 8;
            // 
            // InternalNameFilter
            // 
            this.InternalNameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InternalNameFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InternalNameFilter.Location = new System.Drawing.Point(198, 84);
            this.InternalNameFilter.Name = "InternalNameFilter";
            this.InternalNameFilter.Size = new System.Drawing.Size(174, 21);
            this.InternalNameFilter.TabIndex = 8;
            // 
            // lblFilters
            // 
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
            this.splitContainer.SplitterDistance = 189;
            this.splitContainer.TabIndex = 10;
            // 
            // Description
            // 
            this.Description.BackColor = System.Drawing.SystemColors.Window;
            this.Description.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Description.Location = new System.Drawing.Point(0, 0);
            this.Description.Multiline = true;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Size = new System.Drawing.Size(360, 74);
            this.Description.TabIndex = 9;
            // 
            // ckbAuraInspectionMode
            // 
            this.ckbAuraInspectionMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ckbAuraInspectionMode.AutoSize = true;
            this.ckbAuraInspectionMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ckbAuraInspectionMode.Location = new System.Drawing.Point(239, 63);
            this.ckbAuraInspectionMode.Name = "ckbAuraInspectionMode";
            this.ckbAuraInspectionMode.Size = new System.Drawing.Size(133, 17);
            this.ckbAuraInspectionMode.TabIndex = 11;
            this.ckbAuraInspectionMode.Text = "Display new auras only";
            this.ckbAuraInspectionMode.UseVisualStyleBackColor = true;
            this.ckbAuraInspectionMode.CheckedChanged += new System.EventHandler(this.handler_AuraInspectionModeChanged);
            // 
            // Selector
            // 
            this.Selector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Selector.Location = new System.Drawing.Point(214, 9);
            this.Selector.Name = "Selector";
            this.Selector.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.Selector.Size = new System.Drawing.Size(158, 20);
            this.Selector.TabIndex = 12;
            this.Selector.SelectedValueChanged += new System.EventHandler(this.Handler_TargetSelect);
            // 
            // AuraSelectForm
            // 
            this.AcceptButton = this.btnSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 418);
            this.Controls.Add(this.Selector);
            this.Controls.Add(this.ckbAuraInspectionMode);
            this.Controls.Add(this.unitRefName);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.InternalNameFilter);
            this.Controls.Add(this.NameFilter);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblFilters);
            this.Controls.Add(this.lblEntitySelector);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.MinimumSize = new System.Drawing.Size(386, 450);
            this.Name = "AuraSelectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aura Viewer";
            this.Load += new System.EventHandler(this.handler_LoadForm);
            ((System.ComponentModel.ISupportInitialize)(this.Auras)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Selector.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private LabelControl lblEntitySelector;
        private LabelControl unitRefName;
        private ListBoxControl Auras;
        private SimpleButton btnSelect;
        private SimpleButton btnReload;
        private TextBox NameFilter;
        private TextBox InternalNameFilter;
        private LabelControl lblFilters;
        private SplitContainer splitContainer;
        private TextBox Description;
        private CheckBox ckbAuraInspectionMode;
        private ComboBoxEdit Selector;
    }
}