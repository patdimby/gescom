﻿namespace gescom.create.Views
{
    partial class XtraVente
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraVente));
            this.telerikMetroTheme1 = new Telerik.WinControls.Themes.TelerikMetroTheme();
            this.bigSplitter = new DevExpress.XtraEditors.SplitContainerControl();
            this.divisConteneur = new System.Windows.Forms.SplitContainer();
            this.gridActions = new DevExpress.XtraGrid.GridControl();
            this.gridVue = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQte = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPrix = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnite = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMont = new DevExpress.XtraGrid.Columns.GridColumn();
            this.annuler = new System.Windows.Forms.Button();
            this.txtNum = new System.Windows.Forms.Label();
            this.creer = new System.Windows.Forms.Button();
            this.ultraPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.txtUnite = new Telerik.WinControls.UI.RadPanel();
            this.txtCode = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.txtStock = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
            this.txtPrix = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
            this.txtQte = new Syncfusion.Windows.Forms.Tools.DoubleTextBox();
            this.txtNom = new DevExpress.XtraEditors.LabelControl();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.radPanel6 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel5 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel4 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel3 = new Telerik.WinControls.UI.RadPanel();
            ((System.ComponentModel.ISupportInitialize)(this.bigSplitter)).BeginInit();
            this.bigSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.divisConteneur)).BeginInit();
            this.divisConteneur.Panel1.SuspendLayout();
            this.divisConteneur.Panel2.SuspendLayout();
            this.divisConteneur.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridVue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUnite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQte)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel3)).BeginInit();
            this.SuspendLayout();
            // 
            // bigSplitter
            // 
            this.bigSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bigSplitter.IsSplitterFixed = true;
            this.bigSplitter.Location = new System.Drawing.Point(0, 0);
            this.bigSplitter.Name = "bigSplitter";
            this.bigSplitter.Panel1.Controls.Add(this.divisConteneur);
            this.bigSplitter.Panel1.Controls.Add(this.ultraPanel1);
            this.bigSplitter.Panel1.Controls.Add(this.panelHeader);
            this.bigSplitter.Panel1.Text = "Panel1";
            this.bigSplitter.Panel2.Text = "Panel2";
            this.bigSplitter.Size = new System.Drawing.Size(847, 634);
            this.bigSplitter.SplitterPosition = 826;
            this.bigSplitter.TabIndex = 0;
            this.bigSplitter.Text = "splitContainerControl1";
            // 
            // divisConteneur
            // 
            this.divisConteneur.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.divisConteneur.IsSplitterFixed = true;
            this.divisConteneur.Location = new System.Drawing.Point(0, 59);
            this.divisConteneur.Name = "divisConteneur";
            this.divisConteneur.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // divisConteneur.Panel1
            // 
            this.divisConteneur.Panel1.Controls.Add(this.gridActions);
            // 
            // divisConteneur.Panel2
            // 
            this.divisConteneur.Panel2.Controls.Add(this.annuler);
            this.divisConteneur.Panel2.Controls.Add(this.txtNum);
            this.divisConteneur.Panel2.Controls.Add(this.creer);
            this.divisConteneur.Size = new System.Drawing.Size(826, 575);
            this.divisConteneur.SplitterDistance = 540;
            this.divisConteneur.TabIndex = 10;
            // 
            // gridActions
            // 
            this.gridActions.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridActions.Location = new System.Drawing.Point(0, 0);
            this.gridActions.LookAndFeel.SkinName = "Office 2013";
            this.gridActions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridActions.MainView = this.gridVue;
            this.gridActions.Name = "gridActions";
            this.gridActions.Size = new System.Drawing.Size(826, 540);
            this.gridActions.TabIndex = 7;
            this.gridActions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridVue});
            // 
            // gridVue
            // 
            this.gridVue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCode,
            this.colNom,
            this.colQte,
            this.colPrix,
            this.colUnite,
            this.colMont});
            this.gridVue.GridControl = this.gridActions;
            this.gridVue.Name = "gridVue";
            this.gridVue.OptionsView.ColumnAutoWidth = false;
            this.gridVue.OptionsView.ShowFooter = true;
            this.gridVue.OptionsView.ShowGroupPanel = false;
            // 
            // colCode
            // 
            this.colCode.AppearanceHeader.Options.UseTextOptions = true;
            this.colCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCode.Caption = "CODE";
            this.colCode.FieldName = "Fcode";
            this.colCode.Name = "colCode";
            this.colCode.OptionsColumn.AllowFocus = false;
            this.colCode.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "Code", "{0:n0}")});
            this.colCode.Visible = true;
            this.colCode.VisibleIndex = 0;
            this.colCode.Width = 78;
            // 
            // colNom
            // 
            this.colNom.AppearanceHeader.Options.UseTextOptions = true;
            this.colNom.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNom.Caption = "DESIGNATION";
            this.colNom.FieldName = "Nom";
            this.colNom.Name = "colNom";
            this.colNom.OptionsColumn.AllowFocus = false;
            this.colNom.Visible = true;
            this.colNom.VisibleIndex = 1;
            this.colNom.Width = 331;
            // 
            // colQte
            // 
            this.colQte.AppearanceHeader.Options.UseTextOptions = true;
            this.colQte.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colQte.Caption = "QUANTITE";
            this.colQte.DisplayFormat.FormatString = "n0";
            this.colQte.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colQte.FieldName = "Quantite";
            this.colQte.Name = "colQte";
            this.colQte.Visible = true;
            this.colQte.VisibleIndex = 2;
            this.colQte.Width = 89;
            // 
            // colPrix
            // 
            this.colPrix.AppearanceHeader.Options.UseTextOptions = true;
            this.colPrix.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPrix.Caption = "PRIX";
            this.colPrix.DisplayFormat.FormatString = "n0";
            this.colPrix.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colPrix.FieldName = "Prix";
            this.colPrix.Name = "colPrix";
            this.colPrix.OptionsColumn.AllowFocus = false;
            this.colPrix.Visible = true;
            this.colPrix.VisibleIndex = 3;
            this.colPrix.Width = 88;
            // 
            // colUnite
            // 
            this.colUnite.AppearanceHeader.Options.UseTextOptions = true;
            this.colUnite.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colUnite.Caption = "UNITE";
            this.colUnite.FieldName = "Unite";
            this.colUnite.Name = "colUnite";
            this.colUnite.OptionsColumn.AllowFocus = false;
            this.colUnite.Visible = true;
            this.colUnite.VisibleIndex = 4;
            this.colUnite.Width = 89;
            // 
            // colMont
            // 
            this.colMont.AppearanceHeader.Options.UseTextOptions = true;
            this.colMont.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMont.Caption = "MONTANT";
            this.colMont.DisplayFormat.FormatString = "n0";
            this.colMont.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colMont.FieldName = "colMont";
            this.colMont.Name = "colMont";
            this.colMont.OptionsColumn.AllowFocus = false;
            this.colMont.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "colMont", "{0:n0}")});
            this.colMont.UnboundExpression = "[Prix] * [Quantite]";
            this.colMont.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.colMont.Visible = true;
            this.colMont.VisibleIndex = 5;
            this.colMont.Width = 113;
            // 
            // annuler
            // 
            this.annuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.annuler.Location = new System.Drawing.Point(651, 4);
            this.annuler.Name = "annuler";
            this.annuler.Size = new System.Drawing.Size(75, 23);
            this.annuler.TabIndex = 2;
            this.annuler.Text = "&Annuler";
            this.annuler.UseVisualStyleBackColor = true;
            this.annuler.Click += new System.EventHandler(this.annuler_Click);
            // 
            // txtNum
            // 
            this.txtNum.AutoSize = true;
            this.txtNum.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNum.ForeColor = System.Drawing.SystemColors.Control;
            this.txtNum.Location = new System.Drawing.Point(24, 6);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(13, 13);
            this.txtNum.TabIndex = 1;
            this.txtNum.Text = "0";
            // 
            // creer
            // 
            this.creer.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.creer.Location = new System.Drawing.Point(731, 4);
            this.creer.Name = "creer";
            this.creer.Size = new System.Drawing.Size(75, 23);
            this.creer.TabIndex = 0;
            this.creer.Text = "&OK";
            this.creer.UseVisualStyleBackColor = true;
            this.creer.Click += new System.EventHandler(this.creer_Click);
            // 
            // ultraPanel1
            // 
            this.ultraPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ultraPanel1.Controls.Add(this.txtUnite);
            this.ultraPanel1.Controls.Add(this.txtCode);
            this.ultraPanel1.Controls.Add(this.txtStock);
            this.ultraPanel1.Controls.Add(this.txtPrix);
            this.ultraPanel1.Controls.Add(this.txtQte);
            this.ultraPanel1.Controls.Add(this.txtNom);
            this.ultraPanel1.Location = new System.Drawing.Point(0, 29);
            this.ultraPanel1.Name = "ultraPanel1";
            // 
            // 
            // 
            this.ultraPanel1.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.ultraPanel1.Size = new System.Drawing.Size(840, 28);
            this.ultraPanel1.TabIndex = 9;
            this.ultraPanel1.ThemeName = "TelerikMetro";
            // 
            // txtUnite
            // 
            this.txtUnite.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtUnite.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUnite.Location = new System.Drawing.Point(608, 3);
            this.txtUnite.Name = "txtUnite";
            // 
            // 
            // 
            this.txtUnite.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.txtUnite.Size = new System.Drawing.Size(85, 22);
            this.txtUnite.TabIndex = 7;
            this.txtUnite.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtUnite.ThemeName = "ControlDefault";
            this.txtUnite.Visible = false;
            // 
            // txtCode
            // 
            this.txtCode.BeforeTouchSize = new System.Drawing.Size(86, 22);
            this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCode.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.Location = new System.Drawing.Point(23, 2);
            this.txtCode.Metrocolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212)))));
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(77, 22);
            this.txtCode.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Default;
            this.txtCode.TabIndex = 0;
            this.txtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCode.TextChanged += new System.EventHandler(this.txtCode_ValueChanged);
            // 
            // txtStock
            // 
            this.txtStock.BackGroundColor = System.Drawing.SystemColors.Window;
            this.txtStock.BeforeTouchSize = new System.Drawing.Size(86, 22);
            this.txtStock.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStock.DoubleValue = 0D;
            this.txtStock.Enabled = false;
            this.txtStock.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStock.Location = new System.Drawing.Point(697, 3);
            this.txtStock.Metrocolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212)))));
            this.txtStock.Name = "txtStock";
            this.txtStock.NullString = "";
            this.txtStock.ReadOnly = true;
            this.txtStock.SelectAllOnFocus = false;
            this.txtStock.Size = new System.Drawing.Size(113, 22);
            this.txtStock.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Default;
            this.txtStock.TabIndex = 5;
            this.txtStock.Text = "0,00";
            this.txtStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStock.Visible = false;
            // 
            // txtPrix
            // 
            this.txtPrix.BackGroundColor = System.Drawing.SystemColors.Window;
            this.txtPrix.BeforeTouchSize = new System.Drawing.Size(86, 22);
            this.txtPrix.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPrix.DoubleValue = 0D;
            this.txtPrix.Enabled = false;
            this.txtPrix.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrix.Location = new System.Drawing.Point(521, 3);
            this.txtPrix.Metrocolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212)))));
            this.txtPrix.Name = "txtPrix";
            this.txtPrix.NullString = "";
            this.txtPrix.ReadOnly = true;
            this.txtPrix.SelectAllOnFocus = false;
            this.txtPrix.Size = new System.Drawing.Size(82, 22);
            this.txtPrix.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Default;
            this.txtPrix.TabIndex = 4;
            this.txtPrix.Text = "0,00";
            this.txtPrix.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPrix.Visible = false;
            // 
            // txtQte
            // 
            this.txtQte.BackGroundColor = System.Drawing.SystemColors.Window;
            this.txtQte.BeforeTouchSize = new System.Drawing.Size(86, 22);
            this.txtQte.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtQte.DoubleValue = 0D;
            this.txtQte.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQte.Location = new System.Drawing.Point(430, 3);
            this.txtQte.Metrocolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(211)))), ((int)(((byte)(212)))));
            this.txtQte.Name = "txtQte";
            this.txtQte.NullString = "";
            this.txtQte.Size = new System.Drawing.Size(86, 22);
            this.txtQte.Style = Syncfusion.Windows.Forms.Tools.TextBoxExt.theme.Default;
            this.txtQte.TabIndex = 3;
            this.txtQte.Text = "0,00";
            this.txtQte.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtQte.Visible = false;
            // 
            // txtNom
            // 
            this.txtNom.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNom.Appearance.Options.UseFont = true;
            this.txtNom.Location = new System.Drawing.Point(117, 5);
            this.txtNom.Name = "txtNom";
            this.txtNom.Size = new System.Drawing.Size(0, 15);
            this.txtNom.TabIndex = 1;
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.radPanel6);
            this.panelHeader.Controls.Add(this.radPanel5);
            this.panelHeader.Controls.Add(this.radPanel4);
            this.panelHeader.Controls.Add(this.radPanel1);
            this.panelHeader.Controls.Add(this.radPanel2);
            this.panelHeader.Controls.Add(this.radPanel3);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.ForeColor = System.Drawing.Color.DarkMagenta;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(826, 30);
            this.panelHeader.TabIndex = 8;
            // 
            // radPanel6
            // 
            this.radPanel6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel6.Location = new System.Drawing.Point(23, 4);
            this.radPanel6.Name = "radPanel6";
            // 
            // 
            // 
            this.radPanel6.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.Auto;
            this.radPanel6.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel6.Size = new System.Drawing.Size(77, 23);
            this.radPanel6.TabIndex = 10;
            this.radPanel6.Text = "CODE";
            this.radPanel6.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel6.ThemeName = "TelerikMetro";
            // 
            // radPanel5
            // 
            this.radPanel5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel5.Location = new System.Drawing.Point(103, 4);
            this.radPanel5.Name = "radPanel5";
            // 
            // 
            // 
            this.radPanel5.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel5.Size = new System.Drawing.Size(321, 23);
            this.radPanel5.TabIndex = 9;
            this.radPanel5.Text = "DESIGNATION";
            this.radPanel5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel5.ThemeName = "TelerikMetro";
            // 
            // radPanel4
            // 
            this.radPanel4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel4.Location = new System.Drawing.Point(430, 4);
            this.radPanel4.Name = "radPanel4";
            // 
            // 
            // 
            this.radPanel4.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel4.Size = new System.Drawing.Size(86, 23);
            this.radPanel4.TabIndex = 8;
            this.radPanel4.Text = "QUANTITE";
            this.radPanel4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel4.ThemeName = "TelerikMetro";
            // 
            // radPanel1
            // 
            this.radPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel1.Location = new System.Drawing.Point(520, 4);
            this.radPanel1.Name = "radPanel1";
            // 
            // 
            // 
            this.radPanel1.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel1.Size = new System.Drawing.Size(82, 23);
            this.radPanel1.TabIndex = 5;
            this.radPanel1.Text = "PRIX";
            this.radPanel1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel1.ThemeName = "TelerikMetro";
            // 
            // radPanel2
            // 
            this.radPanel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel2.Location = new System.Drawing.Point(607, 4);
            this.radPanel2.Name = "radPanel2";
            // 
            // 
            // 
            this.radPanel2.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel2.Size = new System.Drawing.Size(85, 23);
            this.radPanel2.TabIndex = 6;
            this.radPanel2.Text = "UNITE";
            this.radPanel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel2.ThemeName = "TelerikMetro";
            // 
            // radPanel3
            // 
            this.radPanel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.radPanel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radPanel3.Location = new System.Drawing.Point(697, 4);
            this.radPanel3.Name = "radPanel3";
            // 
            // 
            // 
            this.radPanel3.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 200, 100);
            this.radPanel3.Size = new System.Drawing.Size(113, 23);
            this.radPanel3.TabIndex = 7;
            this.radPanel3.Text = "DISPONIBLE";
            this.radPanel3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.radPanel3.ThemeName = "TelerikMetro";
            // 
            // XtraVente
            // 
            this.AcceptButton = this.creer;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.annuler;
            this.ClientSize = new System.Drawing.Size(847, 634);
            this.Controls.Add(this.bigSplitter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XtraVente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sell";
            this.Load += new System.EventHandler(this.XtraVente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bigSplitter)).EndInit();
            this.bigSplitter.ResumeLayout(false);
            this.divisConteneur.Panel1.ResumeLayout(false);
            this.divisConteneur.Panel2.ResumeLayout(false);
            this.divisConteneur.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.divisConteneur)).EndInit();
            this.divisConteneur.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridVue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUnite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQte)).EndInit();
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

       // private Infragistics.Win.Misc.UltraFlowLayoutManager ultraPageHeader;
        private Telerik.WinControls.Themes.TelerikMetroTheme telerikMetroTheme1;
        private DevExpress.XtraEditors.SplitContainerControl bigSplitter;
        private System.Windows.Forms.SplitContainer divisConteneur;
        private DevExpress.XtraGrid.GridControl gridActions;
        private DevExpress.XtraGrid.Views.Grid.GridView gridVue;
        private DevExpress.XtraGrid.Columns.GridColumn colCode;
        private DevExpress.XtraGrid.Columns.GridColumn colNom;
        private DevExpress.XtraGrid.Columns.GridColumn colQte;
        private DevExpress.XtraGrid.Columns.GridColumn colPrix;
        private DevExpress.XtraGrid.Columns.GridColumn colUnite;
        private DevExpress.XtraGrid.Columns.GridColumn colMont;
        private System.Windows.Forms.Button annuler;
        private System.Windows.Forms.Label txtNum;
        private System.Windows.Forms.Button creer;
        private Telerik.WinControls.UI.RadPanel ultraPanel1;
        private Telerik.WinControls.UI.RadPanel txtUnite;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt txtCode;
        private Syncfusion.Windows.Forms.Tools.DoubleTextBox txtStock;
        private Syncfusion.Windows.Forms.Tools.DoubleTextBox txtPrix;
        private Syncfusion.Windows.Forms.Tools.DoubleTextBox txtQte;
        private DevExpress.XtraEditors.LabelControl txtNom;
        private System.Windows.Forms.Panel panelHeader;
        private Telerik.WinControls.UI.RadPanel radPanel6;
        private Telerik.WinControls.UI.RadPanel radPanel5;
        private Telerik.WinControls.UI.RadPanel radPanel4;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadPanel radPanel3;
    }
}

