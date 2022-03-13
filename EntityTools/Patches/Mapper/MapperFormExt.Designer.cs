using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    partial class MapperFormExt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.barMapping = new DevExpress.XtraBars.Bar();
            this.groupMapping = new DevExpress.XtraBars.BarButtonGroup();
            this.btnMappingBidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingUnidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingStop = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingLinearPath = new DevExpress.XtraBars.BarCheckItem();
            this.btnMappingForceLink = new DevExpress.XtraBars.BarCheckItem();
            this.btnOptions = new DevExpress.XtraBars.BarButtonItem();
            this.popMenuOptions = new DevExpress.XtraBars.PopupMenu(this.components);
            this.editWaypointDistance = new DevExpress.XtraBars.BarEditItem();
            this.seWaypointDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editMaxZDifference = new DevExpress.XtraBars.BarEditItem();
            this.seMaxZDifference = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.editEquivalenceDistance = new DevExpress.XtraBars.BarEditItem();
            this.seEquivalenceDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.barGraphEditTools = new DevExpress.XtraBars.Bar();
            this.btnUndo = new DevExpress.XtraBars.BarButtonItem();
            this.groupEditMeshes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnMoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnRemoveNodes = new DevExpress.XtraBars.BarCheckItem();
            this.btnEditEdges = new DevExpress.XtraBars.BarCheckItem();
            this.barGraphTools = new DevExpress.XtraBars.Bar();
            this.btnSaveMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.groupImportExportNodes = new DevExpress.XtraBars.BarButtonGroup();
            this.btnImportMeshesFromGame = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportMeshesFromProfile = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnClearMeshes = new DevExpress.XtraBars.BarButtonItem();
            this.btnMeshesInfo = new DevExpress.XtraBars.BarButtonItem();
            this.btnCompression = new DevExpress.XtraBars.BarButtonItem();
            this.btnDistanceMeasurement = new DevExpress.XtraBars.BarCheckItem();
            this.btnObjectInfo = new DevExpress.XtraBars.BarCheckItem();
            this.barCustomRegions = new DevExpress.XtraBars.Bar();
            this.btnAddCR = new DevExpress.XtraBars.BarButtonItem();
            this.btnEditCR = new DevExpress.XtraBars.BarButtonItem();
            this.barEditCustomRegion = new DevExpress.XtraBars.Bar();
            this.btnCRTypeSelector = new DevExpress.XtraBars.BarCheckItem();
            this.btnCRRename = new DevExpress.XtraBars.BarCheckItem();
            this.editCRSelector = new DevExpress.XtraBars.BarEditItem();
            this.itemEditCRList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.editCRName = new DevExpress.XtraBars.BarEditItem();
            this.itemEditCRName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.btnCRAdditionAccept = new DevExpress.XtraBars.BarButtonItem();
            this.btnCREditionAccept = new DevExpress.XtraBars.BarButtonItem();
            this.btnCRCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.btnLockMapOnPlayer = new DevExpress.XtraBars.BarCheckItem();
            this.btnSettings = new DevExpress.XtraBars.BarCheckItem();
            this.groupSaveUndo = new DevExpress.XtraBars.BarButtonGroup();
            this.groupZoom = new DevExpress.XtraBars.BarButtonGroup();
            this.btnZoomIn = new DevExpress.XtraBars.BarButtonItem();
            this.lblZoom = new DevExpress.XtraBars.BarStaticItem();
            this.btnZoomOut = new DevExpress.XtraBars.BarButtonItem();
            this.btnPanelVisibility = new DevExpress.XtraBars.BarButtonItem();
            this.lblPlayerPos = new DevExpress.XtraBars.BarStaticItem();
            this.lblMousePos = new DevExpress.XtraBars.BarStaticItem();
            this.lblDrawInfo = new DevExpress.XtraBars.BarStaticItem();
            this.btnDebugTool = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.menuImportMesh = new DevExpress.XtraBars.BarButtonItem();
            this.editDeleteRadius = new DevExpress.XtraBars.BarEditItem();
            this.seDeleteRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.groupCR = new DevExpress.XtraBars.BarButtonGroup();
            this.editBidirPathColor = new DevExpress.XtraBars.BarEditItem();
            this.editItemColor = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            this.editUnidirPathColor = new DevExpress.XtraBars.BarEditItem();
            this.btnLockOnSpecialObject = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddRoad = new DevExpress.XtraBars.BarCheckItem();
            this.bsrcAstralSettings = new System.Windows.Forms.BindingSource(this.components);
            this.btnShowStatBar = new System.Windows.Forms.Button();
            this.MapPicture = new System.Windows.Forms.PictureBox();
            this.panelSettings = new DevExpress.XtraEditors.PanelControl();
            this.navBarCustomization = new DevExpress.XtraNavBar.NavBarControl();
            this.navGroupGeneral = new DevExpress.XtraNavBar.NavBarGroup();
            this.containerGeneral = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lblLayerDepth = new System.Windows.Forms.Label();
            this.editLayerDepth = new DevExpress.XtraEditors.SpinEdit();
            this.ckbChacheEnable = new DevExpress.XtraEditors.CheckEdit();
            this.containerMeshes = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblUnidirPath = new System.Windows.Forms.Label();
            this.lblBidirPath = new System.Windows.Forms.Label();
            this.colorEditBidirPath = new DevExpress.XtraEditors.ColorEdit();
            this.colorBackground = new DevExpress.XtraEditors.ColorEdit();
            this.colorEditUnidirPath = new DevExpress.XtraEditors.ColorEdit();
            this.containerObjects = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.grpNodeCustomization = new DevExpress.XtraEditors.GroupControl();
            this.ckbNodes = new DevExpress.XtraEditors.CheckEdit();
            this.colorNodes = new DevExpress.XtraEditors.ColorEdit();
            this.ckbSkillnodes = new DevExpress.XtraEditors.CheckEdit();
            this.colorSkillnodes = new DevExpress.XtraEditors.ColorEdit();
            this.grpEntityCustomization = new DevExpress.XtraEditors.GroupControl();
            this.ckbEnemies = new DevExpress.XtraEditors.CheckEdit();
            this.ckbOtherNPC = new DevExpress.XtraEditors.CheckEdit();
            this.colorEnemies = new DevExpress.XtraEditors.ColorEdit();
            this.ckbPlayers = new DevExpress.XtraEditors.CheckEdit();
            this.colorFriends = new DevExpress.XtraEditors.ColorEdit();
            this.colorPlayers = new DevExpress.XtraEditors.ColorEdit();
            this.colorOtherNPC = new DevExpress.XtraEditors.ColorEdit();
            this.ckbFriends = new DevExpress.XtraEditors.CheckEdit();
            this.navGroupMeshes = new DevExpress.XtraNavBar.NavBarGroup();
            this.navGroupObjects = new DevExpress.XtraNavBar.NavBarGroup();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editItemColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelSettings)).BeginInit();
            this.panelSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarCustomization)).BeginInit();
            this.navBarCustomization.SuspendLayout();
            this.containerGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editLayerDepth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbChacheEnable.Properties)).BeginInit();
            this.containerMeshes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditBidirPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBackground.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditUnidirPath.Properties)).BeginInit();
            this.containerObjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpNodeCustomization)).BeginInit();
            this.grpNodeCustomization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckbNodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorNodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbSkillnodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorSkillnodes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEntityCustomization)).BeginInit();
            this.grpEntityCustomization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ckbEnemies.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbOtherNPC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEnemies.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbPlayers.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorFriends.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPlayers.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorOtherNPC.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbFriends.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.work_MapperFormUpdate);
            // 
            // barManager
            // 
            this.barManager.AllowCustomization = false;
            this.barManager.AllowQuickCustomization = false;
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMapping,
            this.barGraphEditTools,
            this.barGraphTools,
            this.barCustomRegions,
            this.barEditCustomRegion,
            this.barStatus});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnMappingUnidirectional,
            this.btnAddCR,
            this.btnImportMeshesFromGame,
            this.btnImportMeshesFromProfile,
            this.menuImportMesh,
            this.btnExportMeshes,
            this.editWaypointDistance,
            this.editMaxZDifference,
            this.editEquivalenceDistance,
            this.editDeleteRadius,
            this.btnClearMeshes,
            this.btnMappingBidirectional,
            this.btnMappingForceLink,
            this.btnMappingLinearPath,
            this.btnMappingStop,
            this.groupMapping,
            this.groupCR,
            this.groupImportExportNodes,
            this.btnOptions,
            this.editCRName,
            this.btnCRAdditionAccept,
            this.btnCRCancel,
            this.lblMousePos,
            this.btnSaveMeshes,
            this.btnMoveNodes,
            this.btnRemoveNodes,
            this.btnEditEdges,
            this.groupEditMeshes,
            this.btnUndo,
            this.btnEditCR,
            this.btnMeshesInfo,
            this.btnCompression,
            this.btnCRTypeSelector,
            this.btnCREditionAccept,
            this.editCRSelector,
            this.btnLockMapOnPlayer,
            this.btnCRRename,
            this.groupZoom,
            this.btnZoomIn,
            this.btnZoomOut,
            this.lblZoom,
            this.groupSaveUndo,
            this.editBidirPathColor,
            this.editUnidirPathColor,
            this.btnLockOnSpecialObject,
            this.btnSettings,
            this.lblPlayerPos,
            this.lblDrawInfo,
            this.btnDistanceMeasurement,
            this.btnAddRoad,
            this.btnObjectInfo,
            this.btnPanelVisibility,
            this.btnDebugTool});
            this.barManager.MaxItemId = 138;
            this.barManager.OptionsLayout.AllowAddNewItems = false;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.seDeleteRadius,
            this.seWaypointDistance,
            this.seMaxZDifference,
            this.seEquivalenceDistance,
            this.itemEditCRName,
            this.itemEditCRList,
            this.editItemColor});
            this.barManager.StatusBar = this.barStatus;
            // 
            // barMapping
            // 
            this.barMapping.BarName = "MappingTools";
            this.barMapping.DockCol = 0;
            this.barMapping.DockRow = 0;
            this.barMapping.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMapping.FloatLocation = new System.Drawing.Point(325, 181);
            this.barMapping.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.groupMapping, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnOptions, true)});
            this.barMapping.OptionsBar.AllowQuickCustomization = false;
            this.barMapping.Text = "Mapping Tools";
            this.barMapping.Visible = false;
            this.barMapping.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // groupMapping
            // 
            this.groupMapping.Caption = "Mapping";
            this.groupMapping.Id = 70;
            this.groupMapping.ItemLinks.Add(this.btnMappingBidirectional);
            this.groupMapping.ItemLinks.Add(this.btnMappingUnidirectional);
            this.groupMapping.ItemLinks.Add(this.btnMappingStop);
            this.groupMapping.ItemLinks.Add(this.btnMappingLinearPath, true);
            this.groupMapping.ItemLinks.Add(this.btnMappingForceLink);
            this.groupMapping.Name = "groupMapping";
            // 
            // btnMappingBidirectional
            // 
            this.btnMappingBidirectional.Caption = "Bidirectional Mapping";
            this.btnMappingBidirectional.GroupIndex = 1;
            this.btnMappingBidirectional.Id = 64;
            this.btnMappingBidirectional.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingBidirectional.ImageOptions.Image")));
            this.btnMappingBidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.B));
            this.btnMappingBidirectional.Name = "btnMappingBidirectional";
            this.btnMappingBidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_BidirectionalPath);
            // 
            // btnMappingUnidirectional
            // 
            this.btnMappingUnidirectional.Caption = "Unidirectional Mapping";
            this.btnMappingUnidirectional.GroupIndex = 1;
            this.btnMappingUnidirectional.Id = 44;
            this.btnMappingUnidirectional.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingUnidirectional.ImageOptions.Image")));
            this.btnMappingUnidirectional.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.U));
            this.btnMappingUnidirectional.Name = "btnMappingUnidirectional";
            this.btnMappingUnidirectional.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_UnidirectionalPath);
            // 
            // btnMappingStop
            // 
            this.btnMappingStop.BindableChecked = true;
            this.btnMappingStop.Caption = "Stop Mapping";
            this.btnMappingStop.Checked = true;
            this.btnMappingStop.GroupIndex = 1;
            this.btnMappingStop.Id = 67;
            this.btnMappingStop.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingStop.ImageOptions.Image")));
            this.btnMappingStop.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.End));
            this.btnMappingStop.Name = "btnMappingStop";
            this.btnMappingStop.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_Stop);
            // 
            // btnMappingLinearPath
            // 
            this.btnMappingLinearPath.Caption = "Linear Path";
            this.btnMappingLinearPath.Id = 66;
            this.btnMappingLinearPath.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingLinearPath.ImageOptions.Image")));
            this.btnMappingLinearPath.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.L));
            this.btnMappingLinearPath.Name = "btnMappingLinearPath";
            this.btnMappingLinearPath.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_LinearPath);
            // 
            // btnMappingForceLink
            // 
            this.btnMappingForceLink.BindableChecked = true;
            this.btnMappingForceLink.Caption = "Force Linking to Last Node";
            this.btnMappingForceLink.Checked = true;
            this.btnMappingForceLink.Id = 65;
            this.btnMappingForceLink.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMappingForceLink.ImageOptions.Image")));
            this.btnMappingForceLink.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
                | System.Windows.Forms.Keys.L));
            this.btnMappingForceLink.Name = "btnMappingForceLink";
            this.btnMappingForceLink.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Mapping_ForceLink);
            // 
            // btnOptions
            // 
            this.btnOptions.ActAsDropDown = true;
            this.btnOptions.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.btnOptions.Caption = "Options";
            this.btnOptions.DropDownControl = this.popMenuOptions;
            this.btnOptions.Hint = "Options";
            this.btnOptions.Id = 74;
            this.btnOptions.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOptions.ImageOptions.Image")));
            this.btnOptions.Name = "btnOptions";
            // 
            // popMenuOptions
            // 
            this.popMenuOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.editWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.editMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.editEquivalenceDistance)});
            this.popMenuOptions.Manager = this.barManager;
            this.popMenuOptions.Name = "popMenuOptions";
            // 
            // editWaypointDistance
            // 
            this.editWaypointDistance.Caption = "Waypoint Distance";
            this.editWaypointDistance.Edit = this.seWaypointDistance;
            this.editWaypointDistance.Id = 58;
            this.editWaypointDistance.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editWaypointDistance.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editWaypointDistance.ImageOptions.Image")));
            this.editWaypointDistance.Name = "editWaypointDistance";
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
            // editMaxZDifference
            // 
            this.editMaxZDifference.Caption = "MaxElevationDifference";
            this.editMaxZDifference.Edit = this.seMaxZDifference;
            this.editMaxZDifference.Id = 59;
            this.editMaxZDifference.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editMaxZDifference.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editMaxZDifference.ImageOptions.Image")));
            this.editMaxZDifference.Name = "editMaxZDifference";
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
            // editEquivalenceDistance
            // 
            this.editEquivalenceDistance.Caption = "Node Equivalence Distance";
            this.editEquivalenceDistance.Edit = this.seEquivalenceDistance;
            this.editEquivalenceDistance.Id = 60;
            this.editEquivalenceDistance.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editEquivalenceDistance.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editEquivalenceDistance.ImageOptions.Image")));
            this.editEquivalenceDistance.Name = "editEquivalenceDistance";
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
            // barGraphEditTools
            // 
            this.barGraphEditTools.BarName = "GraphEditTools";
            this.barGraphEditTools.DockCol = 1;
            this.barGraphEditTools.DockRow = 0;
            this.barGraphEditTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barGraphEditTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupEditMeshes, true)});
            this.barGraphEditTools.Offset = 207;
            this.barGraphEditTools.Text = "Graph Edit Tools";
            this.barGraphEditTools.Visible = false;
            this.barGraphEditTools.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnUndo
            // 
            this.btnUndo.Caption = "Undo";
            this.btnUndo.Id = 101;
            this.btnUndo.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnUndo.ImageOptions.AllowStubGlyph = DevExpress.Utils.DefaultBoolean.False;
            this.btnUndo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.Image")));
            this.btnUndo.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnUndo.ImageOptions.LargeImage")));
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Undo);
            // 
            // groupEditMeshes
            // 
            this.groupEditMeshes.Caption = "EditMeshes";
            this.groupEditMeshes.Id = 96;
            this.groupEditMeshes.ItemLinks.Add(this.btnMoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnRemoveNodes);
            this.groupEditMeshes.ItemLinks.Add(this.btnEditEdges);
            this.groupEditMeshes.Name = "groupEditMeshes";
            // 
            // btnMoveNodes
            // 
            this.btnMoveNodes.Caption = "Relocate Nodes";
            this.btnMoveNodes.Id = 92;
            this.btnMoveNodes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMoveNodes.ImageOptions.Image")));
            this.btnMoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.R));
            this.btnMoveNodes.Name = "btnMoveNodes";
            this.btnMoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_RelocateNodes_ModeChanged);
            // 
            // btnRemoveNodes
            // 
            this.btnRemoveNodes.Caption = "Delete Nodes";
            this.btnRemoveNodes.Id = 93;
            this.btnRemoveNodes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveNodes.ImageOptions.Image")));
            this.btnRemoveNodes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.D));
            this.btnRemoveNodes.Name = "btnRemoveNodes";
            this.btnRemoveNodes.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DeleteNodes_ModeChanged);
            // 
            // btnEditEdges
            // 
            this.btnEditEdges.Caption = "Edit Edges";
            this.btnEditEdges.Id = 94;
            this.btnEditEdges.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditEdges.ImageOptions.Image")));
            this.btnEditEdges.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.E));
            this.btnEditEdges.Name = "btnEditEdges";
            this.btnEditEdges.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_EditEdges_ModeChanged);
            // 
            // barGraphTools
            // 
            this.barGraphTools.BarName = "GraphTools";
            this.barGraphTools.DockCol = 0;
            this.barGraphTools.DockRow = 1;
            this.barGraphTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barGraphTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveMeshes),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupImportExportNodes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnMeshesInfo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCompression),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDistanceMeasurement, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnObjectInfo)});
            this.barGraphTools.OptionsBar.AllowQuickCustomization = false;
            this.barGraphTools.Text = "Graph Tools";
            this.barGraphTools.Visible = false;
            this.barGraphTools.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnSaveMeshes
            // 
            this.btnSaveMeshes.Caption = "Save";
            this.btnSaveMeshes.Id = 91;
            this.btnSaveMeshes.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSaveMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveMeshes.ImageOptions.Image")));
            this.btnSaveMeshes.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSaveMeshes.ImageOptions.LargeImage")));
            this.btnSaveMeshes.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.S));
            this.btnSaveMeshes.Name = "btnSaveMeshes";
            this.btnSaveMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_SaveChanges2QuesterProfile);
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
            this.btnImportMeshesFromGame.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportMeshesFromGame.ImageOptions.Image")));
            this.btnImportMeshesFromGame.Name = "btnImportMeshesFromGame";
            this.btnImportMeshesFromGame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportCurrentMapMeshesFromGame);
            // 
            // btnImportMeshesFromProfile
            // 
            this.btnImportMeshesFromProfile.Caption = "Import from Profile";
            this.btnImportMeshesFromProfile.Id = 52;
            this.btnImportMeshesFromProfile.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportMeshesFromProfile.ImageOptions.Image")));
            this.btnImportMeshesFromProfile.Name = "btnImportMeshesFromProfile";
            this.btnImportMeshesFromProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ImportMapMeshesFromFile);
            // 
            // btnExportMeshes
            // 
            this.btnExportMeshes.Caption = "Export to Mesh";
            this.btnExportMeshes.Enabled = false;
            this.btnExportMeshes.Id = 55;
            this.btnExportMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportMeshes.ImageOptions.Image")));
            this.btnExportMeshes.Name = "btnExportMeshes";
            this.btnExportMeshes.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnExportMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ExportCurrentMapMeshes2File);
            // 
            // btnClearMeshes
            // 
            this.btnClearMeshes.Caption = "Clear";
            this.btnClearMeshes.Id = 63;
            this.btnClearMeshes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClearMeshes.ImageOptions.Image")));
            this.btnClearMeshes.Name = "btnClearMeshes";
            this.btnClearMeshes.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ClearCurrentMapMeshes);
            // 
            // btnMeshesInfo
            // 
            this.btnMeshesInfo.Caption = "MeshesInfo";
            this.btnMeshesInfo.Hint = "Meshes information";
            this.btnMeshesInfo.Id = 103;
            this.btnMeshesInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMeshesInfo.ImageOptions.Image")));
            this.btnMeshesInfo.Name = "btnMeshesInfo";
            this.btnMeshesInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesInfo);
            // 
            // btnCompression
            // 
            this.btnCompression.Caption = "Compression";
            this.btnCompression.Hint = "Удаление из графа непроходимых(невидимых) вершин и ребер";
            this.btnCompression.Id = 104;
            this.btnCompression.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCompression.ImageOptions.Image")));
            this.btnCompression.Name = "btnCompression";
            this.btnCompression.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_MeshesCompression);
            // 
            // btnDistanceMeasurement
            // 
            this.btnDistanceMeasurement.Caption = "DistanceMeasurement";
            this.btnDistanceMeasurement.Id = 132;
            this.btnDistanceMeasurement.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDistanceMeasurement.ImageOptions.Image")));
            this.btnDistanceMeasurement.Name = "btnDistanceMeasurement";
            this.btnDistanceMeasurement.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DistanceMeasurement_ModeChanged);
            // 
            // btnObjectInfo
            // 
            this.btnObjectInfo.Caption = "ObjectInfo";
            this.btnObjectInfo.Id = 134;
            this.btnObjectInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnObjectInfo.ImageOptions.Image")));
            this.btnObjectInfo.Name = "btnObjectInfo";
            this.btnObjectInfo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ObjectInfo);
            // 
            // barCustomRegions
            // 
            this.barCustomRegions.BarName = "CustomRegionTools";
            this.barCustomRegions.DockCol = 1;
            this.barCustomRegions.DockRow = 1;
            this.barCustomRegions.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barCustomRegions.FloatLocation = new System.Drawing.Point(66, 208);
            this.barCustomRegions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnUndo),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddCR),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnEditCR)});
            this.barCustomRegions.Offset = 266;
            this.barCustomRegions.OptionsBar.AllowQuickCustomization = false;
            this.barCustomRegions.Text = "CustomRegion Tools";
            this.barCustomRegions.Visible = false;
            this.barCustomRegions.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnAddCR
            // 
            this.btnAddCR.Caption = "Add CustomRegion";
            this.btnAddCR.Id = 48;
            this.btnAddCR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCR.ImageOptions.Image")));
            this.btnAddCR.Name = "btnAddCR";
            this.btnAddCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartAddingCustomRegion);
            // 
            // btnEditCR
            // 
            this.btnEditCR.Caption = "Edit CustomRegion";
            this.btnEditCR.Id = 102;
            this.btnEditCR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditCR.ImageOptions.Image")));
            this.btnEditCR.Name = "btnEditCR";
            this.btnEditCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_StartEditingCustomRegion);
            // 
            // barEditCustomRegion
            // 
            this.barEditCustomRegion.BarName = "barEditCustomRegion";
            this.barEditCustomRegion.CanDockStyle = ((DevExpress.XtraBars.BarCanDockStyle)((((DevExpress.XtraBars.BarCanDockStyle.Floating | DevExpress.XtraBars.BarCanDockStyle.Top) 
            | DevExpress.XtraBars.BarCanDockStyle.Bottom) 
            | DevExpress.XtraBars.BarCanDockStyle.Standalone)));
            this.barEditCustomRegion.DockCol = 0;
            this.barEditCustomRegion.DockRow = 0;
            this.barEditCustomRegion.FloatLocation = new System.Drawing.Point(46, 560);
            this.barEditCustomRegion.FloatSize = new System.Drawing.Size(397, 88);
            this.barEditCustomRegion.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRTypeSelector),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRRename),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.editCRSelector, "", false, true, true, 231),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.editCRName, "", false, true, true, 216),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRAdditionAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCREditionAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnCRCancel)});
            this.barEditCustomRegion.OptionsBar.AllowQuickCustomization = false;
            this.barEditCustomRegion.OptionsBar.DisableClose = true;
            this.barEditCustomRegion.Text = "Edit CustomRegion";
            this.barEditCustomRegion.Visible = false;
            // 
            // btnCRTypeSelector
            // 
            this.btnCRTypeSelector.Caption = "CustomRegion Form";
            this.btnCRTypeSelector.Hint = "Change type of the CustomRegion (Rectangular to Elliptical)";
            this.btnCRTypeSelector.Id = 106;
            this.btnCRTypeSelector.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCRTypeSelector.ImageOptions.Image")));
            this.btnCRTypeSelector.Name = "btnCRTypeSelector";
            this.btnCRTypeSelector.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ChangeCustomRegionType);
            // 
            // btnCRRename
            // 
            this.btnCRRename.Caption = "Edit the Name of the selected CustomRegion";
            this.btnCRRename.Id = 115;
            this.btnCRRename.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCRRename.ImageOptions.Image")));
            this.btnCRRename.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnCRRename.ImageOptions.LargeImage")));
            this.btnCRRename.Name = "btnCRRename";
            this.btnCRRename.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ChangedRenameCRMode);
            // 
            // editCRSelector
            // 
            this.editCRSelector.Edit = this.itemEditCRList;
            this.editCRSelector.Id = 112;
            this.editCRSelector.Name = "editCRSelector";
            this.editCRSelector.EditValueChanged += new System.EventHandler(this.handler_ChangeSelectedCustomRegion);
            // 
            // itemEditCRList
            // 
            this.itemEditCRList.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.False;
            this.itemEditCRList.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.True;
            this.itemEditCRList.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.itemEditCRList.AutoHeight = false;
            this.itemEditCRList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.itemEditCRList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.Ascending, DevExpress.Utils.DefaultBoolean.Default)});
            this.itemEditCRList.Name = "itemEditCRList";
            this.itemEditCRList.NullText = "";
            this.itemEditCRList.ShowFooter = false;
            // 
            // editCRName
            // 
            this.editCRName.Edit = this.itemEditCRName;
            this.editCRName.Hint = "Enter name of the CustomRegion";
            this.editCRName.Id = 78;
            this.editCRName.Name = "editCRName";
            // 
            // itemEditCRName
            // 
            this.itemEditCRName.AutoHeight = false;
            this.itemEditCRName.Name = "itemEditCRName";
            // 
            // btnCRAdditionAccept
            // 
            this.btnCRAdditionAccept.Caption = "Add";
            this.btnCRAdditionAccept.DropDownEnabled = false;
            this.btnCRAdditionAccept.Id = 79;
            this.btnCRAdditionAccept.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCRAdditionAccept.ImageOptions.Image")));
            this.btnCRAdditionAccept.Name = "btnCRAdditionAccept";
            this.btnCRAdditionAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AcceptCRAddition);
            // 
            // btnCREditionAccept
            // 
            this.btnCREditionAccept.Caption = "Accept";
            this.btnCREditionAccept.Id = 107;
            this.btnCREditionAccept.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCREditionAccept.ImageOptions.Image")));
            this.btnCREditionAccept.Name = "btnCREditionAccept";
            this.btnCREditionAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_AcceptCREdition);
            // 
            // btnCRCancel
            // 
            this.btnCRCancel.Caption = "Cancel";
            this.btnCRCancel.Id = 80;
            this.btnCRCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCRCancel.ImageOptions.Image")));
            this.btnCRCancel.Name = "btnCRCancel";
            this.btnCRCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_CancelCRManipulation);
            // 
            // barStatus
            // 
            this.barStatus.BarName = "statusBar";
            this.barStatus.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.barStatus.DockCol = 0;
            this.barStatus.DockRow = 0;
            this.barStatus.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.barStatus.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLockMapOnPlayer),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSettings, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupSaveUndo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.groupZoom, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPanelVisibility, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblPlayerPos, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblMousePos, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.lblDrawInfo, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDebugTool)});
            this.barStatus.OptionsBar.AllowQuickCustomization = false;
            this.barStatus.OptionsBar.DrawDragBorder = false;
            this.barStatus.OptionsBar.UseWholeRow = true;
            this.barStatus.Text = "StatusBar";
            this.barStatus.VisibleChanged += new System.EventHandler(this.handler_BarVisibleChanged);
            // 
            // btnLockMapOnPlayer
            // 
            this.btnLockMapOnPlayer.BindableChecked = true;
            this.btnLockMapOnPlayer.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.AfterText;
            this.btnLockMapOnPlayer.Checked = true;
            this.btnLockMapOnPlayer.Hint = "Check to hold Player in the center of the Map";
            this.btnLockMapOnPlayer.Id = 114;
            this.btnLockMapOnPlayer.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnLockMapOnPlayer.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.P));
            this.btnLockMapOnPlayer.Name = "btnLockMapOnPlayer";
            // 
            // btnSettings
            // 
            this.btnSettings.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.btnSettings.Caption = "Settings";
            this.btnSettings.Hint = "Open Mapper settings panel";
            this.btnSettings.Id = 129;
            this.btnSettings.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnSettings.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSettings.ImageOptions.Image")));
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ShowSettingsTab);
            // 
            // groupSaveUndo
            // 
            this.groupSaveUndo.Caption = "SaveUndo";
            this.groupSaveUndo.Id = 125;
            this.groupSaveUndo.ItemLinks.Add(this.btnUndo);
            this.groupSaveUndo.ItemLinks.Add(this.btnSaveMeshes);
            this.groupSaveUndo.Name = "groupSaveUndo";
            // 
            // groupZoom
            // 
            this.groupZoom.Caption = "Zoom";
            this.groupZoom.Id = 118;
            this.groupZoom.ItemLinks.Add(this.btnZoomIn);
            this.groupZoom.ItemLinks.Add(this.lblZoom);
            this.groupZoom.ItemLinks.Add(this.btnZoomOut);
            this.groupZoom.Name = "groupZoom";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Caption = "ZoomIn";
            this.btnZoomIn.Id = 119;
            this.btnZoomIn.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnZoomIn.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.ImageOptions.Image")));
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomIn);
            // 
            // lblZoom
            // 
            this.lblZoom.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblZoom.Caption = "zoom";
            this.lblZoom.Id = 122;
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_DoubleClickZoom);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Caption = "ZoomOut";
            this.btnZoomOut.Id = 120;
            this.btnZoomOut.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnZoomOut.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.ImageOptions.Image")));
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_ZoomOut);
            // 
            // btnPanelVisibility
            // 
            this.btnPanelVisibility.Caption = "Panel Visibility";
            this.btnPanelVisibility.Hint = "Show (hide) tools panel";
            this.btnPanelVisibility.Id = 135;
            this.btnPanelVisibility.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPanelVisibility.ImageOptions.Image")));
            this.btnPanelVisibility.Name = "btnPanelVisibility";
            this.btnPanelVisibility.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_Change_PanelVisibility);
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblPlayerPos.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblPlayerPos.Caption = "player";
            this.lblPlayerPos.Hint = "Player coordinates";
            this.lblPlayerPos.Id = 130;
            this.lblPlayerPos.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblPlayerPos.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblPlayerPos.ImageOptions.Image")));
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // lblMousePos
            // 
            this.lblMousePos.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblMousePos.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblMousePos.Caption = "mouse";
            this.lblMousePos.Description = "Mouse position";
            this.lblMousePos.Hint = "The coordinates of the mouse pointer";
            this.lblMousePos.Id = 82;
            this.lblMousePos.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblMousePos.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblMousePos.ImageOptions.Image")));
            this.lblMousePos.Name = "lblMousePos";
            this.lblMousePos.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // lblDrawInfo
            // 
            this.lblDrawInfo.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.lblDrawInfo.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.lblDrawInfo.Caption = "form";
            this.lblDrawInfo.Hint = "Windows redraw information";
            this.lblDrawInfo.Id = 131;
            this.lblDrawInfo.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.lblDrawInfo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("lblDrawInfo.ImageOptions.Image")));
            this.lblDrawInfo.Name = "lblDrawInfo";
            this.lblDrawInfo.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnDebugTool
            // 
            this.btnDebugTool.Id = 137;
            this.btnDebugTool.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDebugTool.ImageOptions.Image")));
            this.btnDebugTool.Name = "btnDebugTool";
            this.btnDebugTool.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_OpenDebugTool);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(403, 49);
            this.barDockControlTop.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 373);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(403, 26);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 324);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(403, 49);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 324);
            // 
            // menuImportMesh
            // 
            this.menuImportMesh.Caption = "Import from Mesh";
            this.menuImportMesh.Id = 53;
            this.menuImportMesh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("menuImportMesh.ImageOptions.Image")));
            this.menuImportMesh.Name = "menuImportMesh";
            // 
            // editDeleteRadius
            // 
            this.editDeleteRadius.Caption = "Node Delete Radius";
            this.editDeleteRadius.Edit = this.seDeleteRadius;
            this.editDeleteRadius.Id = 61;
            this.editDeleteRadius.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editDeleteRadius.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editDeleteRadius.ImageOptions.Image")));
            this.editDeleteRadius.Name = "editDeleteRadius";
            this.editDeleteRadius.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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
            // groupCR
            // 
            this.groupCR.Caption = "CustomRegion";
            this.groupCR.Id = 71;
            this.groupCR.ItemLinks.Add(this.btnAddCR);
            this.groupCR.ItemLinks.Add(this.btnEditCR);
            this.groupCR.Name = "groupCR";
            // 
            // editBidirPathColor
            // 
            this.editBidirPathColor.Caption = "Bidir Path";
            this.editBidirPathColor.Edit = this.editItemColor;
            this.editBidirPathColor.Id = 126;
            this.editBidirPathColor.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editBidirPathColor.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editBidirPathColor.ImageOptions.Image")));
            this.editBidirPathColor.Name = "editBidirPathColor";
            // 
            // editItemColor
            // 
            this.editItemColor.AutoHeight = false;
            this.editItemColor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editItemColor.Name = "editItemColor";
            // 
            // editUnidirPathColor
            // 
            this.editUnidirPathColor.Caption = "Unid Path";
            this.editUnidirPathColor.Edit = this.editItemColor;
            this.editUnidirPathColor.Id = 127;
            this.editUnidirPathColor.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.editUnidirPathColor.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("editUnidirPathColor.ImageOptions.Image")));
            this.editUnidirPathColor.Name = "editUnidirPathColor";
            // 
            // btnLockOnSpecialObject
            // 
            this.btnLockOnSpecialObject.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnLockOnSpecialObject.Caption = "Lock On SpecialObject";
            this.btnLockOnSpecialObject.Id = 128;
            this.btnLockOnSpecialObject.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.btnLockOnSpecialObject.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLockOnSpecialObject.ImageOptions.Image")));
            this.btnLockOnSpecialObject.Name = "btnLockOnSpecialObject";
            this.btnLockOnSpecialObject.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.handler_LockOnSpecialObject);
            // 
            // btnAddRoad
            // 
            this.btnAddRoad.Caption = "Add Road";
            this.btnAddRoad.Description = "Добавить путь между двумя точками";
            this.btnAddRoad.Id = 133;
            this.btnAddRoad.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRoad.ImageOptions.Image")));
            this.btnAddRoad.Name = "btnAddRoad";
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
            this.btnShowStatBar.Location = new System.Drawing.Point(386, 382);
            this.btnShowStatBar.Name = "btnShowStatBar";
            this.btnShowStatBar.Size = new System.Drawing.Size(16, 16);
            this.btnShowStatBar.TabIndex = 4;
            this.btnShowStatBar.UseVisualStyleBackColor = false;
            this.btnShowStatBar.Visible = false;
            this.btnShowStatBar.Click += new System.EventHandler(this.handler_ShowStatusBar);
            // 
            // MapPicture
            // 
            this.MapPicture.BackColor = System.Drawing.Color.Black;
            this.MapPicture.Cursor = System.Windows.Forms.Cursors.Cross;
            this.MapPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapPicture.Location = new System.Drawing.Point(0, 49);
            this.MapPicture.Name = "MapPicture";
            this.MapPicture.Size = new System.Drawing.Size(403, 324);
            this.MapPicture.TabIndex = 9;
            this.MapPicture.TabStop = false;
            this.MapPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.handler_MouseClick);
            this.MapPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.handler_MouseMove);
            this.MapPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.handler_MouseUp);
            this.MapPicture.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // panelSettings
            // 
            this.panelSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSettings.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelSettings.Controls.Add(this.navBarCustomization);
            this.panelSettings.Location = new System.Drawing.Point(12, 66);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(379, 285);
            this.panelSettings.TabIndex = 14;
            this.panelSettings.Visible = false;
            // 
            // navBarCustomization
            // 
            this.navBarCustomization.ActiveGroup = this.navGroupGeneral;
            this.navBarCustomization.Controls.Add(this.containerMeshes);
            this.navBarCustomization.Controls.Add(this.containerObjects);
            this.navBarCustomization.Controls.Add(this.containerGeneral);
            this.navBarCustomization.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarCustomization.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navGroupGeneral,
            this.navGroupMeshes,
            this.navGroupObjects});
            this.navBarCustomization.Location = new System.Drawing.Point(0, 0);
            this.navBarCustomization.Name = "navBarCustomization";
            this.navBarCustomization.OptionsNavPane.ExpandedWidth = 379;
            this.navBarCustomization.Size = new System.Drawing.Size(379, 285);
            this.navBarCustomization.TabIndex = 2;
            this.navBarCustomization.Text = "Customization";
            this.navBarCustomization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // navGroupGeneral
            // 
            this.navGroupGeneral.Caption = "General";
            this.navGroupGeneral.ControlContainer = this.containerGeneral;
            this.navGroupGeneral.GroupClientHeight = 70;
            this.navGroupGeneral.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupGeneral.Name = "navGroupGeneral";
            // 
            // containerGeneral
            // 
            this.containerGeneral.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerGeneral.Appearance.Options.UseBackColor = true;
            this.containerGeneral.Controls.Add(this.lblLayerDepth);
            this.containerGeneral.Controls.Add(this.editLayerDepth);
            this.containerGeneral.Controls.Add(this.ckbChacheEnable);
            this.containerGeneral.Name = "containerGeneral";
            this.containerGeneral.Size = new System.Drawing.Size(415, 70);
            this.containerGeneral.TabIndex = 2;
            // 
            // lblLayerDepth
            // 
            this.lblLayerDepth.AutoSize = true;
            this.lblLayerDepth.Location = new System.Drawing.Point(12, 37);
            this.lblLayerDepth.Name = "lblLayerDepth";
            this.lblLayerDepth.Size = new System.Drawing.Size(238, 13);
            this.lblLayerDepth.TabIndex = 3;
            this.lblLayerDepth.Text = "The limit of the layer\'s depth drawing on Mapper";
            this.lblLayerDepth.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // editLayerDepth
            // 
            this.editLayerDepth.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.editLayerDepth.Location = new System.Drawing.Point(256, 34);
            this.editLayerDepth.MenuManager = this.barManager;
            this.editLayerDepth.Name = "editLayerDepth";
            this.editLayerDepth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editLayerDepth.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editLayerDepth.Properties.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.editLayerDepth.Properties.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.editLayerDepth.Size = new System.Drawing.Size(120, 20);
            this.editLayerDepth.TabIndex = 2;
            this.editLayerDepth.EditValueChanged += new System.EventHandler(this.handler_LayerDepth_Changed);
            this.editLayerDepth.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbChacheEnable
            // 
            this.ckbChacheEnable.Location = new System.Drawing.Point(12, 10);
            this.ckbChacheEnable.MenuManager = this.barManager;
            this.ckbChacheEnable.Name = "ckbChacheEnable";
            this.ckbChacheEnable.Properties.Caption = "Caching of the visible meshes (will take effect in a new Mapper window)";
            this.ckbChacheEnable.Size = new System.Drawing.Size(374, 20);
            this.ckbChacheEnable.TabIndex = 0;
            this.ckbChacheEnable.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // containerMeshes
            // 
            this.containerMeshes.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerMeshes.Appearance.Options.UseBackColor = true;
            this.containerMeshes.Controls.Add(this.lblBackground);
            this.containerMeshes.Controls.Add(this.lblUnidirPath);
            this.containerMeshes.Controls.Add(this.lblBidirPath);
            this.containerMeshes.Controls.Add(this.colorEditBidirPath);
            this.containerMeshes.Controls.Add(this.colorBackground);
            this.containerMeshes.Controls.Add(this.colorEditUnidirPath);
            this.containerMeshes.Name = "containerMeshes";
            this.containerMeshes.Size = new System.Drawing.Size(393, 88);
            this.containerMeshes.TabIndex = 0;
            this.containerMeshes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // lblBackground
            // 
            this.lblBackground.Image = ((System.Drawing.Image)(resources.GetObject("lblBackground.Image")));
            this.lblBackground.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBackground.Location = new System.Drawing.Point(5, 56);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(139, 16);
            this.lblBackground.TabIndex = 2;
            this.lblBackground.Text = "        Background";
            // 
            // lblUnidirPath
            // 
            this.lblUnidirPath.Image = ((System.Drawing.Image)(resources.GetObject("lblUnidirPath.Image")));
            this.lblUnidirPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUnidirPath.Location = new System.Drawing.Point(5, 30);
            this.lblUnidirPath.Name = "lblUnidirPath";
            this.lblUnidirPath.Size = new System.Drawing.Size(139, 16);
            this.lblUnidirPath.TabIndex = 2;
            this.lblUnidirPath.Text = "        Unidirectional path color";
            // 
            // lblBidirPath
            // 
            this.lblBidirPath.Image = ((System.Drawing.Image)(resources.GetObject("lblBidirPath.Image")));
            this.lblBidirPath.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBidirPath.Location = new System.Drawing.Point(5, 4);
            this.lblBidirPath.Name = "lblBidirPath";
            this.lblBidirPath.Size = new System.Drawing.Size(139, 16);
            this.lblBidirPath.TabIndex = 2;
            this.lblBidirPath.Text = "        Bidirectional path color";
            // 
            // colorEditBidirPath
            // 
            this.colorEditBidirPath.EditValue = System.Drawing.Color.Empty;
            this.colorEditBidirPath.Location = new System.Drawing.Point(150, 1);
            this.colorEditBidirPath.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEditBidirPath.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEditBidirPath.Name = "colorEditBidirPath";
            this.colorEditBidirPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEditBidirPath.Size = new System.Drawing.Size(150, 20);
            this.colorEditBidirPath.TabIndex = 0;
            this.colorEditBidirPath.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorBackground
            // 
            this.colorBackground.EditValue = System.Drawing.Color.Empty;
            this.colorBackground.Location = new System.Drawing.Point(150, 53);
            this.colorBackground.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorBackground.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorBackground.Name = "colorBackground";
            this.colorBackground.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorBackground.Size = new System.Drawing.Size(150, 20);
            this.colorBackground.TabIndex = 0;
            this.colorBackground.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorEditUnidirPath
            // 
            this.colorEditUnidirPath.EditValue = System.Drawing.Color.Empty;
            this.colorEditUnidirPath.Location = new System.Drawing.Point(150, 27);
            this.colorEditUnidirPath.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEditUnidirPath.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEditUnidirPath.Name = "colorEditUnidirPath";
            this.colorEditUnidirPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEditUnidirPath.Size = new System.Drawing.Size(150, 20);
            this.colorEditUnidirPath.TabIndex = 0;
            this.colorEditUnidirPath.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // containerObjects
            // 
            this.containerObjects.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.containerObjects.Appearance.Options.UseBackColor = true;
            this.containerObjects.Controls.Add(this.grpNodeCustomization);
            this.containerObjects.Controls.Add(this.grpEntityCustomization);
            this.containerObjects.Name = "containerObjects";
            this.containerObjects.Size = new System.Drawing.Size(393, 175);
            this.containerObjects.TabIndex = 1;
            this.containerObjects.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // grpNodeCustomization
            // 
            this.grpNodeCustomization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNodeCustomization.CaptionLocation = DevExpress.Utils.Locations.Left;
            this.grpNodeCustomization.Controls.Add(this.ckbNodes);
            this.grpNodeCustomization.Controls.Add(this.colorNodes);
            this.grpNodeCustomization.Controls.Add(this.ckbSkillnodes);
            this.grpNodeCustomization.Controls.Add(this.colorSkillnodes);
            this.grpNodeCustomization.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.grpNodeCustomization.Location = new System.Drawing.Point(3, 116);
            this.grpNodeCustomization.Name = "grpNodeCustomization";
            this.grpNodeCustomization.Size = new System.Drawing.Size(387, 55);
            this.grpNodeCustomization.TabIndex = 2;
            this.grpNodeCustomization.Text = "Nodes";
            this.grpNodeCustomization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbNodes
            // 
            this.ckbNodes.Location = new System.Drawing.Point(25, 4);
            this.ckbNodes.Name = "ckbNodes";
            this.ckbNodes.Properties.Caption = "Draw Nodes";
            this.ckbNodes.Size = new System.Drawing.Size(96, 20);
            this.ckbNodes.TabIndex = 0;
            this.ckbNodes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorNodes
            // 
            this.colorNodes.EditValue = System.Drawing.Color.Empty;
            this.colorNodes.Location = new System.Drawing.Point(127, 4);
            this.colorNodes.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorNodes.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorNodes.Name = "colorNodes";
            this.colorNodes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorNodes.Size = new System.Drawing.Size(150, 20);
            this.colorNodes.TabIndex = 0;
            this.colorNodes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbSkillnodes
            // 
            this.ckbSkillnodes.Location = new System.Drawing.Point(25, 30);
            this.ckbSkillnodes.Name = "ckbSkillnodes";
            this.ckbSkillnodes.Properties.Caption = "Draw Skillnodes";
            this.ckbSkillnodes.Size = new System.Drawing.Size(96, 20);
            this.ckbSkillnodes.TabIndex = 0;
            this.ckbSkillnodes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorSkillnodes
            // 
            this.colorSkillnodes.EditValue = System.Drawing.Color.Empty;
            this.colorSkillnodes.Location = new System.Drawing.Point(127, 30);
            this.colorSkillnodes.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorSkillnodes.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorSkillnodes.Name = "colorSkillnodes";
            this.colorSkillnodes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorSkillnodes.Size = new System.Drawing.Size(150, 20);
            this.colorSkillnodes.TabIndex = 0;
            this.colorSkillnodes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // grpEntityCustomization
            // 
            this.grpEntityCustomization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEntityCustomization.CaptionLocation = DevExpress.Utils.Locations.Left;
            this.grpEntityCustomization.Controls.Add(this.ckbEnemies);
            this.grpEntityCustomization.Controls.Add(this.ckbOtherNPC);
            this.grpEntityCustomization.Controls.Add(this.colorEnemies);
            this.grpEntityCustomization.Controls.Add(this.ckbPlayers);
            this.grpEntityCustomization.Controls.Add(this.colorFriends);
            this.grpEntityCustomization.Controls.Add(this.colorPlayers);
            this.grpEntityCustomization.Controls.Add(this.colorOtherNPC);
            this.grpEntityCustomization.Controls.Add(this.ckbFriends);
            this.grpEntityCustomization.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.grpEntityCustomization.Location = new System.Drawing.Point(3, 3);
            this.grpEntityCustomization.Name = "grpEntityCustomization";
            this.grpEntityCustomization.Size = new System.Drawing.Size(387, 107);
            this.grpEntityCustomization.TabIndex = 1;
            this.grpEntityCustomization.Text = "Draw Entities";
            this.grpEntityCustomization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbEnemies
            // 
            this.ckbEnemies.Location = new System.Drawing.Point(25, 4);
            this.ckbEnemies.MenuManager = this.barManager;
            this.ckbEnemies.Name = "ckbEnemies";
            this.ckbEnemies.Properties.Caption = "Enemies";
            this.ckbEnemies.Size = new System.Drawing.Size(62, 20);
            this.ckbEnemies.TabIndex = 0;
            this.ckbEnemies.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbOtherNPC
            // 
            this.ckbOtherNPC.Location = new System.Drawing.Point(25, 82);
            this.ckbOtherNPC.Name = "ckbOtherNPC";
            this.ckbOtherNPC.Properties.Caption = "Other";
            this.ckbOtherNPC.Size = new System.Drawing.Size(62, 20);
            this.ckbOtherNPC.TabIndex = 0;
            this.ckbOtherNPC.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorEnemies
            // 
            this.colorEnemies.EditValue = System.Drawing.Color.Empty;
            this.colorEnemies.Location = new System.Drawing.Point(93, 4);
            this.colorEnemies.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorEnemies.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorEnemies.Name = "colorEnemies";
            this.colorEnemies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorEnemies.Size = new System.Drawing.Size(150, 20);
            this.colorEnemies.TabIndex = 0;
            this.colorEnemies.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbPlayers
            // 
            this.ckbPlayers.Location = new System.Drawing.Point(25, 56);
            this.ckbPlayers.Name = "ckbPlayers";
            this.ckbPlayers.Properties.Caption = "Players";
            this.ckbPlayers.Size = new System.Drawing.Size(62, 20);
            this.ckbPlayers.TabIndex = 0;
            this.ckbPlayers.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorFriends
            // 
            this.colorFriends.EditValue = System.Drawing.Color.Empty;
            this.colorFriends.Location = new System.Drawing.Point(93, 30);
            this.colorFriends.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorFriends.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorFriends.Name = "colorFriends";
            this.colorFriends.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorFriends.Size = new System.Drawing.Size(150, 20);
            this.colorFriends.TabIndex = 0;
            this.colorFriends.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorPlayers
            // 
            this.colorPlayers.EditValue = System.Drawing.Color.Empty;
            this.colorPlayers.Location = new System.Drawing.Point(93, 56);
            this.colorPlayers.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorPlayers.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorPlayers.Name = "colorPlayers";
            this.colorPlayers.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorPlayers.Size = new System.Drawing.Size(150, 20);
            this.colorPlayers.TabIndex = 0;
            this.colorPlayers.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // colorOtherNPC
            // 
            this.colorOtherNPC.EditValue = System.Drawing.Color.Empty;
            this.colorOtherNPC.Location = new System.Drawing.Point(93, 82);
            this.colorOtherNPC.MaximumSize = new System.Drawing.Size(150, 20);
            this.colorOtherNPC.MinimumSize = new System.Drawing.Size(45, 20);
            this.colorOtherNPC.Name = "colorOtherNPC";
            this.colorOtherNPC.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorOtherNPC.Size = new System.Drawing.Size(150, 20);
            this.colorOtherNPC.TabIndex = 0;
            this.colorOtherNPC.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // ckbFriends
            // 
            this.ckbFriends.Location = new System.Drawing.Point(25, 30);
            this.ckbFriends.Name = "ckbFriends";
            this.ckbFriends.Properties.Caption = "Friends";
            this.ckbFriends.Size = new System.Drawing.Size(62, 20);
            this.ckbFriends.TabIndex = 0;
            this.ckbFriends.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            // 
            // navGroupMeshes
            // 
            this.navGroupMeshes.Caption = "Meshes";
            this.navGroupMeshes.ControlContainer = this.containerMeshes;
            this.navGroupMeshes.GroupClientHeight = 95;
            this.navGroupMeshes.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupMeshes.Name = "navGroupMeshes";
            // 
            // navGroupObjects
            // 
            this.navGroupObjects.Caption = "Objects";
            this.navGroupObjects.ControlContainer = this.containerObjects;
            this.navGroupObjects.GroupClientHeight = 182;
            this.navGroupObjects.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navGroupObjects.Name = "navGroupObjects";
            // 
            // MapperFormExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 399);
            this.Controls.Add(this.btnShowStatBar);
            this.Controls.Add(this.panelSettings);
            this.Controls.Add(this.MapPicture);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.HelpButton = true;
            this.IconOptions.ShowIcon = false;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "MapperFormExt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mapper(Extended)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.handler_FormClose);
            this.Load += new System.EventHandler(this.handler_FormLoad);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.handler_HelpRequested);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.handler_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.handler_PreviewKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemEditCRName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editItemColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapPicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelSettings)).EndInit();
            this.panelSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarCustomization)).EndInit();
            this.navBarCustomization.ResumeLayout(false);
            this.containerGeneral.ResumeLayout(false);
            this.containerGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editLayerDepth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbChacheEnable.Properties)).EndInit();
            this.containerMeshes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.colorEditBidirPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorBackground.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEditUnidirPath.Properties)).EndInit();
            this.containerObjects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpNodeCustomization)).EndInit();
            this.grpNodeCustomization.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ckbNodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorNodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbSkillnodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorSkillnodes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpEntityCustomization)).EndInit();
            this.grpEntityCustomization.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ckbEnemies.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbOtherNPC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorEnemies.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbPlayers.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorFriends.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPlayers.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorOtherNPC.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ckbFriends.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

        private BackgroundWorker backgroundWorker;
        private BarManager barManager;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarCheckItem btnMappingUnidirectional;
        private RepositoryItemSpinEdit seDeleteRadius;
        private BarButtonItem btnAddCR;
        private BarButtonItem btnImportMeshesFromGame;
        private BarButtonItem btnImportMeshesFromProfile;
        private BarButtonItem menuImportMesh;
        private BarButtonItem btnExportMeshes;
        private BarEditItem editWaypointDistance;
        private RepositoryItemSpinEdit seWaypointDistance;
        private BarEditItem editMaxZDifference;
        private RepositoryItemSpinEdit seMaxZDifference;
        private BarEditItem editEquivalenceDistance;
        private RepositoryItemSpinEdit seEquivalenceDistance;
        private BarEditItem editDeleteRadius;
        private BarButtonItem btnClearMeshes;
        private BarCheckItem btnMappingBidirectional;
        private BarCheckItem btnMappingForceLink;
        private BarCheckItem btnMappingLinearPath;
        private BarCheckItem btnMappingStop;
        private Bar barMapping;
        private BarButtonGroup groupMapping;
        private BarButtonGroup groupCR;
        private BarButtonGroup groupImportExportNodes;
        private BarButtonItem btnOptions;
        private PopupMenu popMenuOptions;
        private BarEditItem editCRName;
        private RepositoryItemTextEdit itemEditCRName;
        private BarButtonItem btnCRAdditionAccept;
        private BarButtonItem btnCRCancel;
        private Bar barStatus;
        private BarStaticItem lblMousePos;
        private BindingSource bsrcAstralSettings;
        private Button btnShowStatBar;
        private BarButtonItem btnSaveMeshes;
        private Bar barGraphTools;
        private BarCheckItem btnMoveNodes;
        private BarCheckItem btnRemoveNodes;
        private BarCheckItem btnEditEdges;
        private BarButtonGroup groupEditMeshes;
        private BarButtonItem btnUndo;
        private BarButtonItem btnEditCR;
        private BarButtonItem btnMeshesInfo;
        private BarButtonItem btnCompression;
        private Bar barEditCustomRegion;
        private BarCheckItem btnCRTypeSelector;
        private BarButtonItem btnCREditionAccept;
        private BarEditItem editCRSelector;
        private RepositoryItemLookUpEdit itemEditCRList;
        private BarCheckItem btnLockMapOnPlayer;
        private BarCheckItem btnCRRename;
        private PictureBox MapPicture;
        private BarButtonGroup groupZoom;
        private BarButtonItem btnZoomIn;
        private BarButtonItem btnZoomOut;
        private BarStaticItem lblZoom;
        private Bar barCustomRegions;
        private Bar barGraphEditTools;
        private BarButtonGroup groupSaveUndo;
        private BarEditItem editBidirPathColor;
        private RepositoryItemColorEdit editItemColor;
        private BarEditItem editUnidirPathColor;
        private BarButtonItem btnLockOnSpecialObject;
        private DevExpress.XtraEditors.PanelControl panelSettings;
        private DevExpress.XtraEditors.ColorEdit colorEditUnidirPath;
        private DevExpress.XtraEditors.ColorEdit colorEditBidirPath;
        private BarCheckItem btnSettings;
        private BarStaticItem lblPlayerPos;
        private BarStaticItem lblDrawInfo;
        private BarCheckItem btnDistanceMeasurement;
        private DevExpress.XtraNavBar.NavBarControl navBarCustomization;
        private DevExpress.XtraNavBar.NavBarGroup navGroupMeshes;
        private DevExpress.XtraNavBar.NavBarGroup navGroupGeneral;
        private DevExpress.XtraNavBar.NavBarGroup navGroupObjects;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerMeshes;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerObjects;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer containerGeneral;
        private DevExpress.XtraEditors.CheckEdit ckbNodes;
        private DevExpress.XtraEditors.CheckEdit ckbFriends;
        private DevExpress.XtraEditors.CheckEdit ckbEnemies;
        private DevExpress.XtraEditors.ColorEdit colorNodes;
        private DevExpress.XtraEditors.ColorEdit colorFriends;
        private DevExpress.XtraEditors.ColorEdit colorEnemies;
        private DevExpress.XtraEditors.CheckEdit ckbPlayers;
        private DevExpress.XtraEditors.ColorEdit colorPlayers;
        private DevExpress.XtraEditors.ColorEdit colorBackground;
        private DevExpress.XtraEditors.CheckEdit ckbSkillnodes;
        private DevExpress.XtraEditors.ColorEdit colorSkillnodes;
        private DevExpress.XtraEditors.CheckEdit ckbOtherNPC;
        private DevExpress.XtraEditors.ColorEdit colorOtherNPC;
        private DevExpress.XtraEditors.GroupControl grpNodeCustomization;
        private DevExpress.XtraEditors.GroupControl grpEntityCustomization;
        private BarCheckItem btnAddRoad;
        private Label lblBackground;
        private Label lblUnidirPath;
        private Label lblBidirPath;
        private DevExpress.XtraEditors.CheckEdit ckbChacheEnable;
        private DevExpress.XtraEditors.SpinEdit editLayerDepth;
        private Label lblLayerDepth;
        private BarCheckItem btnObjectInfo;
        private BarButtonItem btnPanelVisibility;
        private BarButtonItem btnDebugTool;
    } 
#endif
}