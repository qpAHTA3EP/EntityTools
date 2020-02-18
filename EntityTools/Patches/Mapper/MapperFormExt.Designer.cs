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
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.toolbarMainMapper = new DevExpress.XtraBars.Bar();
            this.toolGroupMapping = new DevExpress.XtraBars.BarButtonGroup();
            this.menuCheckStopMapping = new DevExpress.XtraBars.BarCheckItem();
            this.menuBidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.menuUnidirectional = new DevExpress.XtraBars.BarCheckItem();
            this.menuLinearPath = new DevExpress.XtraBars.BarCheckItem();
            this.menuForceLinkLast = new DevExpress.XtraBars.BarCheckItem();
            this.toolGroupCR = new DevExpress.XtraBars.BarButtonGroup();
            this.menuRectangularCR = new DevExpress.XtraBars.BarButtonItem();
            this.menuEllipticalCR = new DevExpress.XtraBars.BarButtonItem();
            this.toolGroupNodes = new DevExpress.XtraBars.BarButtonGroup();
            this.menuImportGame = new DevExpress.XtraBars.BarButtonItem();
            this.menuImportProfile = new DevExpress.XtraBars.BarButtonItem();
            this.menuExportMesh = new DevExpress.XtraBars.BarButtonItem();
            this.menuClear = new DevExpress.XtraBars.BarButtonItem();
            this.menuOptions = new DevExpress.XtraBars.BarButtonItem();
            this.popMenuOptions = new DevExpress.XtraBars.PopupMenu(this.components);
            this.menuWaypointDistance = new DevExpress.XtraBars.BarEditItem();
            this.seWaypointDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuMaxZDifference = new DevExpress.XtraBars.BarEditItem();
            this.seMaxZDifference = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuEquivalenceDistance = new DevExpress.XtraBars.BarEditItem();
            this.seEquivalenceDistance = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuDeleteRadius = new DevExpress.XtraBars.BarEditItem();
            this.seDeleteRadius = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.menuCacheActive = new DevExpress.XtraBars.BarCheckItem();
            this.toolbarCustomRegion = new DevExpress.XtraBars.Bar();
            this.menuLableCR = new DevExpress.XtraBars.BarStaticItem();
            this.menuCRName = new DevExpress.XtraBars.BarEditItem();
            this.teCRName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.menuCRAccept = new DevExpress.XtraBars.BarButtonItem();
            this.menuCRCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barMapper = new DevExpress.XtraBars.BarSubItem();
            this.barCustomRegion = new DevExpress.XtraBars.BarSubItem();
            this.barNodes = new DevExpress.XtraBars.BarSubItem();
            this.menuImportMesh = new DevExpress.XtraBars.BarButtonItem();
            this.barOptions = new DevExpress.XtraBars.BarSubItem();
            this.menuBtnStopMapping = new DevExpress.XtraBars.BarButtonItem();
            this.bsrcAstralSettings = new System.Windows.Forms.BindingSource(this.components);
            this.statusBar = new DevExpress.XtraBars.Bar();
            this.statMousePos = new DevExpress.XtraBars.BarStaticItem();
            this.statZoom = new DevExpress.XtraBars.BarEditItem();
            this.zoomer = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            this.statCenterPlayerText = new DevExpress.XtraBars.BarStaticItem();
            this.statCenterPlayer = new DevExpress.XtraBars.BarCheckItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCRName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomer)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkUpdateFormCaption);
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.toolbarMainMapper,
            this.toolbarCustomRegion,
            this.statusBar});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barMapper,
            this.menuUnidirectional,
            this.barCustomRegion,
            this.menuRectangularCR,
            this.menuEllipticalCR,
            this.barNodes,
            this.menuImportGame,
            this.menuImportProfile,
            this.menuImportMesh,
            this.menuExportMesh,
            this.barOptions,
            this.menuWaypointDistance,
            this.menuMaxZDifference,
            this.menuEquivalenceDistance,
            this.menuDeleteRadius,
            this.menuClear,
            this.menuBidirectional,
            this.menuForceLinkLast,
            this.menuLinearPath,
            this.menuCheckStopMapping,
            this.menuBtnStopMapping,
            this.toolGroupMapping,
            this.toolGroupCR,
            this.toolGroupNodes,
            this.menuOptions,
            this.menuLableCR,
            this.menuCRName,
            this.menuCRAccept,
            this.menuCRCancel,
            this.menuCacheActive,
            this.statMousePos,
            this.statZoom,
            this.statCenterPlayerText,
            this.statCenterPlayer});
            this.barManager.MaxItemId = 90;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.seDeleteRadius,
            this.seWaypointDistance,
            this.seMaxZDifference,
            this.seEquivalenceDistance,
            this.teCRName,
            this.zoomer});
            this.barManager.StatusBar = this.statusBar;
            // 
            // toolbarMainMapper
            // 
            this.toolbarMainMapper.BarName = "MapperTools";
            this.toolbarMainMapper.DockCol = 0;
            this.toolbarMainMapper.DockRow = 0;
            this.toolbarMainMapper.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.toolbarMainMapper.FloatLocation = new System.Drawing.Point(43, 217);
            this.toolbarMainMapper.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.toolGroupMapping, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.toolGroupCR, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.toolGroupNodes, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuOptions, true)});
            this.toolbarMainMapper.Text = "MapperTools";
            // 
            // toolGroupMapping
            // 
            this.toolGroupMapping.Caption = "Mapping";
            this.toolGroupMapping.Id = 70;
            this.toolGroupMapping.ItemLinks.Add(this.menuCheckStopMapping);
            this.toolGroupMapping.ItemLinks.Add(this.menuBidirectional);
            this.toolGroupMapping.ItemLinks.Add(this.menuUnidirectional);
            this.toolGroupMapping.ItemLinks.Add(this.menuLinearPath);
            this.toolGroupMapping.ItemLinks.Add(this.menuForceLinkLast);
            this.toolGroupMapping.Name = "toolGroupMapping";
            // 
            // menuCheckStopMapping
            // 
            this.menuCheckStopMapping.BindableChecked = true;
            this.menuCheckStopMapping.Caption = "Stop Mapping";
            this.menuCheckStopMapping.Checked = true;
            this.menuCheckStopMapping.GroupIndex = 1;
            this.menuCheckStopMapping.Id = 67;
            this.menuCheckStopMapping.ImageOptions.Image = global::EntityTools.Properties.Resources.miniStopSimple;
            this.menuCheckStopMapping.Name = "menuCheckStopMapping";
            this.menuCheckStopMapping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventStopMapping);
            // 
            // menuBidirectional
            // 
            this.menuBidirectional.Caption = "Bidirectional Mapping";
            this.menuBidirectional.GroupIndex = 1;
            this.menuBidirectional.Id = 64;
            this.menuBidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniBiPath;
            this.menuBidirectional.Name = "menuBidirectional";
            this.menuBidirectional.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.eventStartMapping);
            // 
            // menuUnidirectional
            // 
            this.menuUnidirectional.Caption = "Unidirectional Mapping";
            this.menuUnidirectional.GroupIndex = 1;
            this.menuUnidirectional.Id = 44;
            this.menuUnidirectional.ImageOptions.Image = global::EntityTools.Properties.Resources.miniUniPath;
            this.menuUnidirectional.Name = "menuUnidirectional";
            this.menuUnidirectional.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.eventStartMapping);
            // 
            // menuLinearPath
            // 
            this.menuLinearPath.Caption = "Linear Path";
            this.menuLinearPath.Id = 66;
            this.menuLinearPath.ImageOptions.Image = global::EntityTools.Properties.Resources.miniLinePath;
            this.menuLinearPath.Name = "menuLinearPath";
            // 
            // menuForceLinkLast
            // 
            this.menuForceLinkLast.Caption = "Force Linking to Last Node";
            this.menuForceLinkLast.Id = 65;
            this.menuForceLinkLast.ImageOptions.Image = global::EntityTools.Properties.Resources.miniHurdLink;
            this.menuForceLinkLast.Name = "menuForceLinkLast";
            // 
            // toolGroupCR
            // 
            this.toolGroupCR.Caption = "CustomRegion";
            this.toolGroupCR.Id = 71;
            this.toolGroupCR.ItemLinks.Add(this.menuRectangularCR);
            this.toolGroupCR.ItemLinks.Add(this.menuEllipticalCR);
            this.toolGroupCR.Name = "toolGroupCR";
            // 
            // menuRectangularCR
            // 
            this.menuRectangularCR.Caption = "Add Rectangular";
            this.menuRectangularCR.Id = 48;
            this.menuRectangularCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            this.menuRectangularCR.Name = "menuRectangularCR";
            this.menuRectangularCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventAddRectangularCR);
            // 
            // menuEllipticalCR
            // 
            this.menuEllipticalCR.Caption = "Add Elliptical";
            this.menuEllipticalCR.Id = 49;
            this.menuEllipticalCR.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCREllipce;
            this.menuEllipticalCR.Name = "menuEllipticalCR";
            this.menuEllipticalCR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventAddElipticalCR);
            // 
            // toolGroupNodes
            // 
            this.toolGroupNodes.Caption = "Nodes";
            this.toolGroupNodes.Id = 72;
            this.toolGroupNodes.ItemLinks.Add(this.menuImportGame);
            this.toolGroupNodes.ItemLinks.Add(this.menuImportProfile);
            this.toolGroupNodes.ItemLinks.Add(this.menuExportMesh);
            this.toolGroupNodes.ItemLinks.Add(this.menuClear);
            this.toolGroupNodes.Name = "toolGroupNodes";
            // 
            // menuImportGame
            // 
            this.menuImportGame.Caption = "Import from Game";
            this.menuImportGame.Id = 51;
            this.menuImportGame.ImageOptions.Image = global::EntityTools.Properties.Resources.miniClone;
            this.menuImportGame.Name = "menuImportGame";
            this.menuImportGame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventImportNodesFromGame);
            // 
            // menuImportProfile
            // 
            this.menuImportProfile.Caption = "Import from Profile";
            this.menuImportProfile.Id = 52;
            this.menuImportProfile.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.menuImportProfile.Name = "menuImportProfile";
            this.menuImportProfile.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventImportNodesFromFile);
            // 
            // menuExportMesh
            // 
            this.menuExportMesh.Caption = "Export to Mesh";
            this.menuExportMesh.Id = 55;
            this.menuExportMesh.ImageOptions.Image = global::EntityTools.Properties.Resources.miniExport;
            this.menuExportMesh.Name = "menuExportMesh";
            this.menuExportMesh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventExportNodes2Mesh);
            // 
            // menuClear
            // 
            this.menuClear.Caption = "Clear";
            this.menuClear.Id = 63;
            this.menuClear.ImageOptions.Image = global::EntityTools.Properties.Resources.Deletemini;
            this.menuClear.Name = "menuClear";
            this.menuClear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventClearNodes);
            // 
            // menuOptions
            // 
            this.menuOptions.ActAsDropDown = true;
            this.menuOptions.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.menuOptions.Caption = "Options";
            this.menuOptions.DropDownControl = this.popMenuOptions;
            this.menuOptions.Hint = "Options";
            this.menuOptions.Id = 74;
            this.menuOptions.ImageOptions.Image = global::EntityTools.Properties.Resources.miniGear;
            this.menuOptions.Name = "menuOptions";
            // 
            // popMenuOptions
            // 
            this.popMenuOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuEquivalenceDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuDeleteRadius),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuCacheActive)});
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
            // menuCacheActive
            // 
            this.menuCacheActive.Caption = "Use Cache";
            this.menuCacheActive.Id = 81;
            this.menuCacheActive.Name = "menuCacheActive";
            this.menuCacheActive.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // toolbarCustomRegion
            // 
            this.toolbarCustomRegion.BarName = "CustomRegion";
            this.toolbarCustomRegion.DockCol = 0;
            this.toolbarCustomRegion.DockRow = 0;
            this.toolbarCustomRegion.FloatLocation = new System.Drawing.Point(66, 200);
            this.toolbarCustomRegion.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuLableCR, true),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.menuCRName, "", false, true, true, 152),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuCRAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuCRCancel)});
            this.toolbarCustomRegion.Offset = 10;
            this.toolbarCustomRegion.OptionsBar.DisableClose = true;
            this.toolbarCustomRegion.Text = "CustomRegion";
            this.toolbarCustomRegion.Visible = false;
            // 
            // menuLableCR
            // 
            this.menuLableCR.Caption = "CustomRegion";
            this.menuLableCR.Id = 77;
            this.menuLableCR.Name = "menuLableCR";
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
            // menuCRAccept
            // 
            this.menuCRAccept.Caption = "Accept";
            this.menuCRAccept.DropDownEnabled = false;
            this.menuCRAccept.Id = 79;
            this.menuCRAccept.ImageOptions.Image = global::EntityTools.Properties.Resources.miniAdd;
            this.menuCRAccept.Name = "menuCRAccept";
            this.menuCRAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventMenuCRAcceptClick);
            // 
            // menuCRCancel
            // 
            this.menuCRCancel.Caption = "Cancel";
            this.menuCRCancel.Id = 80;
            this.menuCRCancel.ImageOptions.Image = global::EntityTools.Properties.Resources.miniRemove;
            this.menuCRCancel.Name = "menuCRCancel";
            this.menuCRCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventMenuCRCancelClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(389, 31);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 335);
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlBottom.Size = new System.Drawing.Size(389, 27);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 304);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(389, 31);
            this.barDockControlRight.Manager = this.barManager;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 304);
            // 
            // barMapper
            // 
            this.barMapper.Caption = "Mapping";
            this.barMapper.Hint = "Make the Path";
            this.barMapper.Id = 36;
            this.barMapper.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuCheckStopMapping),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuBidirectional),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuUnidirectional),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuLinearPath, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuForceLinkLast, true)});
            this.barMapper.Name = "barMapper";
            // 
            // barCustomRegion
            // 
            this.barCustomRegion.Caption = "CustomRegion";
            this.barCustomRegion.Id = 47;
            this.barCustomRegion.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuRectangularCR),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuEllipticalCR)});
            this.barCustomRegion.Name = "barCustomRegion";
            // 
            // barNodes
            // 
            this.barNodes.Caption = "Nodes";
            this.barNodes.Id = 50;
            this.barNodes.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuImportGame),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuImportProfile),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuImportMesh),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuExportMesh, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuClear, true)});
            this.barNodes.Name = "barNodes";
            // 
            // menuImportMesh
            // 
            this.menuImportMesh.Caption = "Import from Mesh";
            this.menuImportMesh.Id = 53;
            this.menuImportMesh.ImageOptions.Image = global::EntityTools.Properties.Resources.miniImport;
            this.menuImportMesh.Name = "menuImportMesh";
            // 
            // barOptions
            // 
            this.barOptions.Caption = "Option";
            this.barOptions.Hint = "Option";
            this.barOptions.Id = 57;
            this.barOptions.ImageOptions.Image = global::EntityTools.Properties.Resources.miniGear;
            this.barOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.menuWaypointDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuMaxZDifference),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuEquivalenceDistance),
            new DevExpress.XtraBars.LinkPersistInfo(this.menuDeleteRadius)});
            this.barOptions.MenuAppearance.MenuBar.Image = global::EntityTools.Properties.Resources.miniGear;
            this.barOptions.MenuAppearance.MenuBar.Options.UseImage = true;
            this.barOptions.MenuAppearance.MenuCaption.Image = global::EntityTools.Properties.Resources.miniGear;
            this.barOptions.MenuAppearance.MenuCaption.Options.UseImage = true;
            this.barOptions.Name = "barOptions";
            // 
            // menuBtnStopMapping
            // 
            this.menuBtnStopMapping.Caption = "Stop Mapping";
            this.menuBtnStopMapping.GroupIndex = 1;
            this.menuBtnStopMapping.Id = 68;
            this.menuBtnStopMapping.ImageOptions.Image = global::EntityTools.Properties.Resources.miniStopSimple;
            this.menuBtnStopMapping.Name = "menuBtnStopMapping";
            this.menuBtnStopMapping.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.eventStopMapping);
            // 
            // statusBar
            // 
            this.statusBar.BarName = "statusBar";
            this.statusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.statusBar.DockCol = 0;
            this.statusBar.DockRow = 0;
            this.statusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.statusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.statCenterPlayer),
            new DevExpress.XtraBars.LinkPersistInfo(this.statMousePos, true),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.statZoom, "", false, true, true, 196)});
            this.statusBar.OptionsBar.AllowQuickCustomization = false;
            this.statusBar.OptionsBar.DrawDragBorder = false;
            this.statusBar.OptionsBar.UseWholeRow = true;
            this.statusBar.Visible = false;
            // 
            // statMousePos
            // 
            this.statMousePos.Caption = "mousePos";
            this.statMousePos.Id = 82;
            this.statMousePos.Name = "statMousePos";
            // 
            // statZoom
            // 
            this.statZoom.Caption = "statZoom";
            this.statZoom.Edit = this.zoomer;
            this.statZoom.Id = 83;
            this.statZoom.Name = "statZoom";
            // 
            // zoomer
            // 
            this.zoomer.Name = "zoomer";
            // 
            // statCenterPlayerText
            // 
            this.statCenterPlayerText.Caption = "Center Player";
            this.statCenterPlayerText.Id = 88;
            this.statCenterPlayerText.Name = "statCenterPlayerText";
            // 
            // statCenterPlayer
            // 
            this.statCenterPlayer.Id = 89;
            this.statCenterPlayer.ImageOptions.Image = global::EntityTools.Properties.Resources.miniTarget;
            this.statCenterPlayer.Name = "statCenterPlayer";
            // 
            // MapperFormExt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 362);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.Name = "MapperFormExt";
            this.ShowIcon = false;
            this.Text = "Mapper(Extended)";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.eventClosingMapperForm);
            this.Load += new System.EventHandler(this.eventLoadMapperForm);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popMenuOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seWaypointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seMaxZDifference)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEquivalenceDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seDeleteRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCRName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsrcAstralSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private static MapperFormExt @this;

        public static Thread MapperThread;

        private Astral.Forms.UserControls.Mapper mapper = new Astral.Forms.UserControls.Mapper();

        private CustomRegion newRegion = new CustomRegion();
        private BackgroundWorker backgroundWorker;
        private BarManager barManager;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarSubItem barMapper;
        private BarCheckItem menuUnidirectional;
        private RepositoryItemSpinEdit seDeleteRadius;
        private BarSubItem barCustomRegion;
        private BarButtonItem menuRectangularCR;
        private BarButtonItem menuEllipticalCR;
        private BarSubItem barNodes;
        private BarButtonItem menuImportGame;
        private BarButtonItem menuImportProfile;
        private BarButtonItem menuImportMesh;
        private BarButtonItem menuExportMesh;
        private BarSubItem barOptions;
        private BarEditItem menuWaypointDistance;
        private RepositoryItemSpinEdit seWaypointDistance;
        private BarEditItem menuMaxZDifference;
        private RepositoryItemSpinEdit seMaxZDifference;
        private BarEditItem menuEquivalenceDistance;
        private RepositoryItemSpinEdit seEquivalenceDistance;
        private BarEditItem menuDeleteRadius;
        private BarButtonItem menuClear;
        private BarCheckItem menuBidirectional;
        private BarCheckItem menuForceLinkLast;
        private BarCheckItem menuLinearPath;
        private BarCheckItem menuCheckStopMapping;
        private Bar toolbarMainMapper;
        private BarButtonItem menuBtnStopMapping;
        private BarButtonGroup toolGroupMapping;
        private BarButtonGroup toolGroupCR;
        private BarButtonGroup toolGroupNodes;
        private BarButtonItem menuOptions;
        private PopupMenu popMenuOptions;
        private BindingSource bsrcAstralSettings;
        private Bar toolbarCustomRegion;
        private BarStaticItem menuLableCR;
        private BarEditItem menuCRName;
        private RepositoryItemTextEdit teCRName;
        private BarButtonItem menuCRAccept;
        private BarButtonItem menuCRCancel;
        private BarCheckItem menuCacheActive;
        private Bar statusBar;
        private BarStaticItem statMousePos;
        private BarEditItem statZoom;
        private RepositoryItemZoomTrackBar zoomer;
        private BarCheckItem statCenterPlayer;
        private BarStaticItem statCenterPlayerText;
    }
}