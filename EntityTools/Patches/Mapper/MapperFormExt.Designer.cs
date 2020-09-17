using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    partial class MapperFormExt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapperFormExt));
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barMainTools = new DevExpress.XtraBars.Bar();
            this.btnSaveMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.groupMapping = new DevExpress.XtraBars.BarButtonGroup();
            this.btnBidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnUnidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnLinearPath = new DevExpress.XtraBars.BarCheckItem();
            this.btnForceLinkLast = new DevExpress.XtraBars.BarCheckItem();
            this.btnStopMapping = new DevExpress.XtraBars.BarCheckItem();
            this.groupCR = new DevExpress.XtraBars.BarButtonGroup();
            this.btnRectangularCR = new DevExpress.XtraBars.BarButtonItem();
            this.btnEllipticalCR = new DevExpress.XtraBars.BarButtonItem();
            this.btnEditCR = new DevExpress.XtraBars.BarCheckItem();
            this.btnOptions = new DevExpress.XtraBars.BarButtonItem();
            this.popMenuOptions = new DevExpress.XtraBars.PopupMenu(this.components);
            this.menuWaypointDistance = new DevExpress.XtraBars.BarEditItem();
            this.seWaypointDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuMaxZDifference = new DevExpress.XtraBars.BarEditItem();
            this.seMaxZDifference = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuEquivalenceDistance = new DevExpress.XtraBars.BarEditItem();
            this.seEquivalenceDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuDeleteRadius = new DevExpress.XtraBars.BarEditItem();
            this.seDeleteRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barCustomRegion = new DevExpress.XtraBars.Bar();
            this.lblCR = new DevExpress.XtraBars.BarStaticItem();
            this.menuCRName = new DevExpress.XtraBars.BarEditItem();
            this.teCRName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnCRAccept = new DevExpress.XtraBars.BarButtonItem();
            this.btnCRCancel = new DevExpress.XtraBars.BarButtonItem();
            this.statusBar = new DevExpress.XtraBars.Bar();
            this.btnLockMapOnPlayer = new DevExpress.XtraBars.BarCheckItem();
            this.trbarZoom = new DevExpress.XtraBars.BarEditItem();
            this.zoomer = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            this.lblMousePos = new DevExpress.XtraBars.BarStaticItem();
            this.barEditMeshes = new DevExpress.XtraBars.Bar();
            this.groupImportExportNodes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnImportMeshesFromGame = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportMeshesFromProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.groupEditMeshes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnUndo = new DevExpress.XtraBars.BarButtonItem();
            this.btnMoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnRemoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnEditEdges = new DevExpress.XtraBars.BarCheckItem();
            this.btnMeshesInfo = new DevExpress.XtraBars.BarButtonItem();
            this.btnCompression = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.groupNodes = new DevExpress.XtraBars.BarSubItem();
            this.menuImportMesh = new DevExpress.XtraBars.BarButtonItem();
            this.bsrcAstralSettings = new System.Windows.Forms.BindingSource(this.components);
            this.btnShowStatBar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCRName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.work_UpdateFormStatus);
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMainTools,
            this.barCustomRegion,
            this.statusBar,
            this.barEditMeshes});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnUnidirectional,
            this.btnRectangularCR,
            this.btnEllipticalCR,
            this.groupNodes,
            this.btnImportMeshesFromGame,
            this.btnImportMeshesFromProfile,
            this.menuImportMesh,
            this.btnExportMeshes,
            this.menuWaypointDistance,
            this.menuMaxZDifference,
            this.menuEquivalenceDistance,
            this.menuDeleteRadius,
            this.btnClearMeshes,
            this.btnBidirectional,
            this.btnForceLinkLast,
            this.btnLinearPath,
            this.btnStopMapping,
            this.groupMapping,
            this.groupCR,
            this.groupImportExportNodes,
            this.btnOptions,
            this.lblCR,
            this.menuCRName,
            this.btnCRAccept,
            this.btnCRCancel,
            this.lblMousePos,
            this.trbarZoom,
            this.btnLockMapOnPlayer,
            this.btnSaveMeshes,
            this.btnMoveNodes,
            this.btnRemoveNodes,
            this.btnEditEdges,
            this.groupEditMeshes,
            this.btnUndo,
            this.btnEditCR,
            this.btnMeshesInfo,
            this.btnCompression});
            this.barManager.MaxItemId = 105;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.seDeleteRadius,
            this.seWaypointDistance,
            this.seMaxZDifference,
            this.seEquivalenceDistance,
            this.teCRName,
            this.zoomer});
            this.barManager.StatusBar = this.statusBar;
            // 
            // barMainTools
            // 
            this.barMainTools.BarName = "MapperTools";
            this.barMainTools.DockCol = 0;
            this.barMainTools.DockRow = 0;
            this.barMainTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMainTools.FloatLocation = new System.Drawing.Point(325, 181);
            this.barMainTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveMeshes),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupMapping, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupCR, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOptions, true)});
            this.barMainTools.OptionsBar.AllowQuickCustomization = false;
            this.barMainTools.Text = "MapperTools";
            this.barMainTools.Visible = false;
            this.barMainTools.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnSaveMeshes
            // 
            this.btnSaveMeshes.Caption = "Save";
            this.btnSaveMeshes.Id = 91;
            this.btnSaveMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDiskette;
            this.btnSaveMeshes.ImageOptions.LargeImage = global::EntityTools.Properties.Resources.miniDiskette;
            this.btnSaveMeshes.Name = "btnSaveMeshes";
            this.btnSaveMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_SaveCurrentMapMeshes2QuesterProfile);
            // 
            // groupMapping
            // 
            this.groupMapping.Caption = "Mapping";
            this.groupMapping.Id = 70;
            this.groupMapping.ItemLinks.Add(this.btnBidirectional);
            this.groupMapping.ItemLinks.Add(this.btnUnidirectional);
            this.groupMapping.ItemLinks.Add(this.btnLinearPath);
            this.groupMapping.ItemLinks.Add(this.btnForceLinkLast);
            this.groupMapping.ItemLinks.Add(this.btnStopMapping);
            this.groupMapping.Name = "groupMapping";
            // 
            // btnBidirectional
            // 
            this.btnBidirectional.Caption = "Bidirectional Mapping";
            this.btnBidirectional.GroupIndex = 1;
            this.btnBidirectional.Id = 64;
            this.btnBidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniBiPath;
            this.btnBidirectional.Name = "btnBidirectional";
            this.btnBidirectional.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartMapping);
            // 
            // btnUnidirectional
            // 
            this.btnUnidirectional.Caption = "Unidirectional Mapping";
            this.btnUnidirectional.GroupIndex = 1;
            this.btnUnidirectional.Id = 44;
            this.btnUnidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniUniPath;
            this.btnUnidirectional.Name = "btnUnidirectional";
            this.btnUnidirectional.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartMapping);
            // 
            // btnLinearPath
            // 
            this.btnLinearPath.Caption = "Linear Path";
            this.btnLinearPath.Id = 66;
            this.btnLinearPath.ImageOptions.Image = global::EntityTools.Properties.Resources.miniLinePath;
            this.btnLinearPath.Name = "btnLinearPath";
            // 
            // btnForceLinkLast
            // 
            this.btnForceLinkLast.Caption = "Force Linking to Last Node";
            this.btnForceLinkLast.Id = 65;
            this.btnForceLinkLast.ImageOptions.Image = global::EntityTools.Properties.Resources.miniHurdLink;
            this.btnForceLinkLast.Name = "btnForceLinkLast";
            // 
            // btnStopMapping
            // 
            this.btnStopMapping.BindableChecked = true;
            this.btnStopMapping.Caption = "Stop Mapping";
            this.btnStopMapping.Checked = true;
            this.btnStopMapping.GroupIndex = 1;
            this.btnStopMapping.Id = 67;
            this.btnStopMapping.ImageOptions.Image = global::EntityTools.Properties.Resources.miniStop;
            this.btnStopMapping.Name = "btnStopMapping";
            this.btnStopMapping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StopMapping);
            // 
            // groupCR
            // 
            this.groupCR.Caption = "CustomRegion";
            this.groupCR.Id = 71;
            this.groupCR.ItemLinks.Add(this.btnRectangularCR);
            this.groupCR.ItemLinks.Add(this.btnEllipticalCR);
            this.groupCR.ItemLinks.Add(this.btnEditCR);
            this.groupCR.Name = "groupCR";
            // 
            // btnRectangularCR
            // 
            this.btnRectangularCR.Caption = "Add Rectangular";
            this.btnRectangularCR.Id = 48;
            this.btnRectangularCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            this.btnRectangularCR.Name = "btnRectangularCR";
            this.btnRectangularCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AddRectangularCR);
            // 
            // btnEllipticalCR
            // 
            this.btnEllipticalCR.Caption = "Add Elliptical";
            this.btnEllipticalCR.Id = 49;
            this.btnEllipticalCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCREllipce;
            this.btnEllipticalCR.Name = "btnEllipticalCR";
            this.btnEllipticalCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AddElipticalCR);
            // 
            // btnEditCR
            // 
            this.btnEditCR.Caption = "EditCR";
            this.btnEditCR.Id = 102;
            this.btnEditCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniEditCR;
            this.btnEditCR.Name = "btnEditCR";
            this.btnEditCR.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // btnOptions
            // 
            this.btnOptions.ActAsDropDown = true;
            this.btnOptions.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.btnOptions.Caption = "Options";
            this.btnOptions.DropDownControl = this.popMenuOptions;
            this.btnOptions.Hint = "Options";
            this.btnOptions.Id = 74;
            this.btnOptions.ImageOptions.Image = global::EntityTools.Properties.Resources.miniGear;
            this.btnOptions.Name = "btnOptions";
            // 
            // popMenuOptions
            // 
            this.popMenuOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuEquivalenceDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuDeleteRadius)});
            this.popMenuOptions.Manager = this.barManager;
            this.popMenuOptions.Name = "popMenuOptions";
            // 
            // menuWaypointDistance
            // 
            this.menuWaypointDistance.Caption = "Waypoint Distance";
            this.menuWaypointDistance.Edit = this.seWaypointDistance;
            this.menuWaypointDistance.Id = 58;
            this.menuWaypointDistance.ImageOptions.Image = global::EntityTools.Properties.Resources.miniNodeDistance;
            this.menuWaypointDistance.Name = "menuWaypointDistance";
            // 
            // seWaypointDistance
            // 
            this.seWaypointDistance.AutoHeight = false;
            this.seWaypointDistance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seWaypointDistance.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seWaypointDistance.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seWaypointDistance.IsFloatValue = false;
            this.seWaypointDistance.Mask.EditMask = "N00";
            this.seWaypointDistance.Name = "seWaypointDistance";
            // 
            // menuMaxZDifference
            // 
            this.menuMaxZDifference.Caption = "MaxElevationDifference";
            this.menuMaxZDifference.Edit = this.seMaxZDifference;
            this.menuMaxZDifference.Id = 59;
            this.menuMaxZDifference.ImageOptions.Image = global::EntityTools.Properties.Resources.miniZdiff;
            this.menuMaxZDifference.Name = "menuMaxZDifference";
            // 
            // seMaxZDifference
            // 
            this.seMaxZDifference.AutoHeight = false;
            this.seMaxZDifference.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seMaxZDifference.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMaxZDifference.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seMaxZDifference.IsFloatValue = false;
            this.seMaxZDifference.Mask.EditMask = "N00";
            this.seMaxZDifference.Name = "seMaxZDifference";
            // 
            // menuEquivalenceDistance
            // 
            this.menuEquivalenceDistance.Caption = "Node Equivalence Distance";
            this.menuEquivalenceDistance.Edit = this.seEquivalenceDistance;
            this.menuEquivalenceDistance.Id = 60;
            this.menuEquivalenceDistance.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDistance;
            this.menuEquivalenceDistance.Name = "menuEquivalenceDistance";
            // 
            // seEquivalenceDistance
            // 
            this.seEquivalenceDistance.AutoHeight = false;
            this.seEquivalenceDistance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seEquivalenceDistance.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seEquivalenceDistance.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.seEquivalenceDistance.IsFloatValue = false;
            this.seEquivalenceDistance.Mask.EditMask = "N00";
            this.seEquivalenceDistance.Name = "seEquivalenceDistance";
            // 
            // menuDeleteRadius
            // 
            this.menuDeleteRadius.Caption = "Node Delete Radius";
            this.menuDeleteRadius.Edit = this.seDeleteRadius;
            this.menuDeleteRadius.Id = 61;
            this.menuDeleteRadius.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.menuDeleteRadius.Name = "menuDeleteRadius";
            this.menuDeleteRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // seDeleteRadius
            // 
            this.seDeleteRadius.AutoHeight = false;
            this.seDeleteRadius.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.seDeleteRadius.IsFloatValue = false;
            this.seDeleteRadius.Mask.EditMask = "N00";
            this.seDeleteRadius.MaxLength = 4;
            this.seDeleteRadius.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.seDeleteRadius.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.seDeleteRadius.Name = "seDeleteRadius";
            // 
            // barCustomRegion
            // 
            this.barCustomRegion.BarName = "CustomRegion";
            this.barCustomRegion.DockCol = 0;
            this.barCustomRegion.DockRow = 0;
            this.barCustomRegion.FloatLocation = new System.Drawing.Point(30, 568);
            this.barCustomRegion.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.lblCR, true),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.menuCRName, "", false, true, true, 152),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRCancel)});
            this.barCustomRegion.OptionsBar.DisableClose = true;
            this.barCustomRegion.OptionsBar.UseWholeRow = true;
            this.barCustomRegion.Text = "CustomRegion";
            this.barCustomRegion.Visible = false;
            // 
            // lblCR
            // 
            this.lblCR.Caption = "CustomRegion";
            this.lblCR.Id = 77;
            this.lblCR.Name = "lblCR";
            // 
            // menuCRName
            // 
            this.menuCRName.Edit = this.teCRName;
            this.menuCRName.Id = 78;
            this.menuCRName.Name = "menuCRName";
            // 
            // teCRName
            // 
            this.teCRName.AutoHeight = false;
            this.teCRName.Name = "teCRName";
            // 
            // btnCRAccept
            // 
            this.btnCRAccept.Caption = "Accept";
            this.btnCRAccept.DropDownEnabled = false;
            this.btnCRAccept.Id = 79;
            this.btnCRAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAdd;
            this.btnCRAccept.Name = "btnCRAccept";
            this.btnCRAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MenuCRAcceptClick);
            // 
            // btnCRCancel
            // 
            this.btnCRCancel.Caption = "Cancel";
            this.btnCRCancel.Id = 80;
            this.btnCRCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnCRCancel.Name = "btnCRCancel";
            this.btnCRCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MenuCRCancelClick);
            // 
            // statusBar
            // 
            this.statusBar.BarName = "statusBar";
            this.statusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.statusBar.DockCol = 0;
            this.statusBar.DockRow = 0;
            this.statusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.statusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLockMapOnPlayer),
            new DevExpress.XtraBars.LinkPersistInfo(this.trbarZoom),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblMousePos, true)});
            this.statusBar.OptionsBar.AllowQuickCustomization = false;
            this.statusBar.OptionsBar.DrawDragBorder = false;
            this.statusBar.OptionsBar.UseWholeRow = true;
            this.statusBar.Text = "StatusBar";
            this.statusBar.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnLockMapOnPlayer
            // 
            this.btnLockMapOnPlayer.BindableChecked = true;
            this.btnLockMapOnPlayer.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.AfterText;
            this.btnLockMapOnPlayer.Checked = true;
            this.btnLockMapOnPlayer.Description = "Lock Map on the Player";
            this.btnLockMapOnPlayer.Hint = "Check to hold Player in the center of the Map";
            this.btnLockMapOnPlayer.Id = 89;
            this.btnLockMapOnPlayer.Name = "btnLockMapOnPlayer";
            this.btnLockMapOnPlayer.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MapLockOnOnPlayerChanged);
            // 
            // trbarZoom
            // 
            this.trbarZoom.Caption = "statZoom";
            this.trbarZoom.Description = "Zoom of the Map";
            this.trbarZoom.Edit = this.zoomer;
            this.trbarZoom.EditValue = 5D;
            this.trbarZoom.EditWidth = 200;
            this.trbarZoom.Hint = "Zoom of the Map";
            this.trbarZoom.Id = 83;
            this.trbarZoom.Name = "trbarZoom";
            this.trbarZoom.EditValueChanged += new System.EventHandler(this.handler_ZoomChanged);
            // 
            // zoomer
            // 
            this.zoomer.LargeChange = 1;
            this.zoomer.Maximum = 20;
            this.zoomer.Name = "zoomer";
            this.zoomer.ShowValueToolTip = true;
            this.zoomer.ValueChanged += new System.EventHandler(this.handler_ZoomChanged);
            // 
            // lblMousePos
            // 
            this.lblMousePos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblMousePos.Caption = "mousePos";
            this.lblMousePos.Description = "Player position";
            this.lblMousePos.Hint = "Player position";
            this.lblMousePos.Id = 82;
            this.lblMousePos.Name = "lblMousePos";
            // 
            // barEditMeshes
            // 
            this.barEditMeshes.BarName = "EditMeshes";
            this.barEditMeshes.DockCol = 0;
            this.barEditMeshes.DockRow = 1;
            this.barEditMeshes.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barEditMeshes.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.groupImportExportNodes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupEditMeshes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnMeshesInfo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCompression)});
            this.barEditMeshes.OptionsBar.AllowQuickCustomization = false;
            this.barEditMeshes.Text = "EditMeshes";
            this.barEditMeshes.Visible = false;
            this.barEditMeshes.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // groupImportExportNodes
            // 
            this.groupImportExportNodes.Caption = "ImportExportNodes";
            this.groupImportExportNodes.Id = 72;
            this.groupImportExportNodes.ItemLinks.Add(this.btnImportMeshesFromGame, true);
            this.groupImportExportNodes.ItemLinks.Add(this.btnImportMeshesFromProfile);
            this.groupImportExportNodes.ItemLinks.Add(this.btnExportMeshes);
            this.groupImportExportNodes.ItemLinks.Add(this.btnClearMeshes, true);
            this.groupImportExportNodes.Name = "groupImportExportNodes";
            // 
            // btnImportMeshesFromGame
            // 
            this.btnImportMeshesFromGame.Caption = "Import from Game";
            this.btnImportMeshesFromGame.Id = 51;
            this.btnImportMeshesFromGame.ImageOptions.Image = global::EntityTools.Properties.Resources.miniClone;
            this.btnImportMeshesFromGame.Name = "btnImportMeshesFromGame";
            this.btnImportMeshesFromGame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportCurrentMapMeshesFromGame);
            // 
            // btnImportMeshesFromProfile
            // 
            this.btnImportMeshesFromProfile.Caption = "Import from Profile";
            this.btnImportMeshesFromProfile.Id = 52;
            this.btnImportMeshesFromProfile.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.btnImportMeshesFromProfile.Name = "btnImportMeshesFromProfile";
            this.btnImportMeshesFromProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportMapMeshesFromFile);
            // 
            // btnExportMeshes
            // 
            this.btnExportMeshes.Caption = "Export to Mesh";
            this.btnExportMeshes.Enabled = false;
            this.btnExportMeshes.Id = 55;
            this.btnExportMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniExport;
            this.btnExportMeshes.Name = "btnExportMeshes";
            this.btnExportMeshes.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnExportMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ExportCurrentMapMeshes2File);
            // 
            // btnClearMeshes
            // 
            this.btnClearMeshes.Caption = "Clear";
            this.btnClearMeshes.Id = 63;
            this.btnClearMeshes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniDelete;
            this.btnClearMeshes.Name = "btnClearMeshes";
            this.btnClearMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ClearCurrentMapMeshes);
            // 
            // groupEditMeshes
            // 
            this.groupEditMeshes.Caption = "EditMeshes";
            this.groupEditMeshes.Id = 96;
            this.groupEditMeshes.ItemLinks.Add(this.btnUndo);
            this.groupEditMeshes.ItemLinks.Add(this.btnMoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnRemoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnEditEdges);
            this.groupEditMeshes.Name = "groupEditMeshes";
            // 
            // btnUndo
            // 
            this.btnUndo.Caption = "Undo";
            this.btnUndo.Id = 101;
            this.btnUndo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.Image")));
            this.btnUndo.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.LargeImage")));
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnUndo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Undo);
            // 
            // btnMoveNodes
            // 
            this.btnMoveNodes.Caption = "MoveNodes";
            this.btnMoveNodes.Id = 92;
            this.btnMoveNodes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveNodes.ImageOptions.Image")));
            this.btnMoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.M));
            this.btnMoveNodes.Name = "btnMoveNodes";
            this.btnMoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MoveNodes_ModeChanged);
            // 
            // btnRemoveNodes
            // 
            this.btnRemoveNodes.Caption = "RemoveNodes";
            this.btnRemoveNodes.Id = 93;
            this.btnRemoveNodes.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCancel;
            this.btnRemoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete));
            this.btnRemoveNodes.Name = "btnRemoveNodes";
            this.btnRemoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_RemoveNodes_ModeChanged);
            // 
            // btnEditEdges
            // 
            this.btnEditEdges.Caption = "EditEdges";
            this.btnEditEdges.Id = 94;
            this.btnEditEdges.ImageOptions.Image = global::EntityTools.Properties.Resources.miniEditEdge_2;
            this.btnEditEdges.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.E));
            this.btnEditEdges.Name = "btnEditEdges";
            this.btnEditEdges.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_EditEdges_ModeChanged);
            // 
            // btnMeshesInfo
            // 
            this.btnMeshesInfo.Caption = "MeshesInfo";
            this.btnMeshesInfo.Hint = "Meshes information";
            this.btnMeshesInfo.Id = 103;
            this.btnMeshesInfo.ImageOptions.Image = global::EntityTools.Properties.Resources.previewchart_16x16;
            this.btnMeshesInfo.ImageOptions.LargeImage = global::EntityTools.Properties.Resources.previewchart_32x32;
            this.btnMeshesInfo.Name = "btnMeshesInfo";
            this.btnMeshesInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnMeshesInfo_ItemClick);
            // 
            // btnCompression
            // 
            this.btnCompression.Caption = "Compression";
            this.btnCompression.Hint = "Удаление из графа непроходимых(невидимых) вершин и ребер";
            this.btnCompression.Id = 104;
            this.btnCompression.ImageOptions.Image = global::EntityTools.Properties.Resources.newwizard_16x16;
            this.btnCompression.Name = "btnCompression";
            this.btnCompression.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnCompression.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCompression_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(388, 56);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 340);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(388, 26);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 56);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 284);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(388, 56);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 284);
            // 
            // groupNodes
            // 
            this.groupNodes.Caption = "Nodes";
            this.groupNodes.Id = 50;
            this.groupNodes.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnImportMeshesFromGame),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnImportMeshesFromProfile),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuImportMesh),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExportMeshes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnClearMeshes, true)});
            this.groupNodes.Name = "groupNodes";
            // 
            // menuImportMesh
            // 
            this.menuImportMesh.Caption = "Import from Mesh";
            this.menuImportMesh.Id = 53;
            this.menuImportMesh.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.menuImportMesh.Name = "menuImportMesh";
            // 
            // btnShowStatBar
            // 
            this.btnShowStatBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowStatBar.BackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnShowStatBar.BackgroundImage")));
            this.btnShowStatBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnShowStatBar.FlatAppearance.BorderSize = 0;
            this.btnShowStatBar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnShowStatBar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowStatBar.Location = new System.Drawing.Point(372, 350);
            this.btnShowStatBar.Name = "btnShowStatBar";
            this.btnShowStatBar.Size = new System.Drawing.Size(16, 16);
            this.btnShowStatBar.TabIndex = 4;
            this.btnShowStatBar.UseVisualStyleBackColor = false;
            this.btnShowStatBar.Visible = false;
            this.btnShowStatBar.Click += new System.EventHandler(this.handler_ShowStatusBar);
            // 
            // MapperFormExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 366);
            this.Controls.Add(this.btnShowStatBar);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.LookAndFeel.SkinName = "Office 2013 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MapperFormExt";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mapper(Extended)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handler_ClosingMapperForm);
            this.Load += new System.EventHandler(this.handler_LoadMapperForm);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCRName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

        private static MapperFormExt @this;

        private BackgroundWorker backgroundWorker;
        private BarManager barManager;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarCheckItem btnUnidirectional;
        private RepositoryItemSpinEdit seDeleteRadius;
        private BarButtonItem btnRectangularCR;
        private BarButtonItem btnEllipticalCR;
        private BarSubItem groupNodes;
        private BarButtonItem btnImportMeshesFromGame;
        private BarButtonItem btnImportMeshesFromProfile;
        private BarButtonItem menuImportMesh;
        private BarButtonItem btnExportMeshes;
        private BarEditItem menuWaypointDistance;
        private RepositoryItemSpinEdit seWaypointDistance;
        private BarEditItem menuMaxZDifference;
        private RepositoryItemSpinEdit seMaxZDifference;
        private BarEditItem menuEquivalenceDistance;
        private RepositoryItemSpinEdit seEquivalenceDistance;
        private BarEditItem menuDeleteRadius;
        private BarButtonItem btnClearMeshes;
        private BarCheckItem btnBidirectional;
        private BarCheckItem btnForceLinkLast;
        private BarCheckItem btnLinearPath;
        private BarCheckItem btnStopMapping;
        private Bar barMainTools;
        private BarButtonGroup groupMapping;
        private BarButtonGroup groupCR;
        private BarButtonGroup groupImportExportNodes;
        private BarButtonItem btnOptions;
        private PopupMenu popMenuOptions;
        private Bar barCustomRegion;
        private BarStaticItem lblCR;
        private BarEditItem menuCRName;
        private RepositoryItemTextEdit teCRName;
        private BarButtonItem btnCRAccept;
        private BarButtonItem btnCRCancel;
        private Bar statusBar;
        private BarStaticItem lblMousePos;
        private BarEditItem trbarZoom;
        private RepositoryItemZoomTrackBar zoomer;
        private BarCheckItem btnLockMapOnPlayer;
        private BindingSource bsrcAstralSettings;
        private Button btnShowStatBar;
        private BarButtonItem btnSaveMeshes;
        private Bar barEditMeshes;
        private BarCheckItem btnMoveNodes;
        private BarCheckItem btnRemoveNodes;
        private BarCheckItem btnEditEdges;
        private BarButtonGroup groupEditMeshes;
        private BarButtonItem btnUndo;
        private BarCheckItem btnEditCR;
        private BarButtonItem btnMeshesInfo;
        private BarButtonItem btnCompression;
    } 
#endif
}