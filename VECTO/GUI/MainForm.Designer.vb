' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class MainForm
	Inherits Form

	'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
	<DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Wird vom Windows Form-Designer benÃ¶tigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StatusBAR = New System.Windows.Forms.StatusStrip()
        Me.ToolStripLbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgBarJob = New System.Windows.Forms.ToolStripProgressBar()
        Me.ToolStripProgBarOverall = New System.Windows.Forms.ToolStripProgressBar()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPageGEN = New System.Windows.Forms.TabPage()
        Me.btnImportXML = New System.Windows.Forms.Button()
        Me.btnExportXML = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btStartV3 = New System.Windows.Forms.Button()
        Me.LbDecl = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.BtGENdown = New System.Windows.Forms.Button()
        Me.BtGENup = New System.Windows.Forms.Button()
        Me.ChBoxAllGEN = New System.Windows.Forms.CheckBox()
        Me.LvGEN = New System.Windows.Forms.ListView()
        Me.ColGENpath = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColGENstatus = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ButtonGENremove = New System.Windows.Forms.Button()
        Me.ButtonGENadd = New System.Windows.Forms.Button()
        Me.TabPgOptions = New System.Windows.Forms.TabPage()
        Me.PanelOptAllg = New System.Windows.Forms.Panel()
        Me.gbXMLHashValidation = New System.Windows.Forms.GroupBox()
        Me.cbValidateXMLDeclarationJobs = New System.Windows.Forms.CheckBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.BtTCfileBrowse = New System.Windows.Forms.Button()
        Me.tbOutputFolder = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cbActVmod = New System.Windows.Forms.CheckBox()
        Me.cbValidateRunData = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ChBoxModOut = New System.Windows.Forms.CheckBox()
        Me.ChBoxMod1Hz = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RbDev = New System.Windows.Forms.RadioButton()
        Me.RbDecl = New System.Windows.Forms.RadioButton()
        Me.TabPageDEV = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LvDEVoptions = New System.Windows.Forms.ListView()
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ConMenFilelist = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowInFolderMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadDefaultListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.LvMsg = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripDrDnBtTools = New System.Windows.Forms.ToolStripDropDownButton()
        Me.GENEditorToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.EPTPJobEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VEHEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EngineEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GearboxEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GraphToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDrDnBtInfo = New System.Windows.Forms.ToolStripDropDownButton()
        Me.UserManualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateNotesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReportBugViaCITnetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutVECTOToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.CmDEV = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TmProgSec = New System.Windows.Forms.Timer(Me.components)
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenInGraphWindowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Label2 = New System.Windows.Forms.Label()
        Me.StatusBAR.SuspendLayout
        Me.TabControl1.SuspendLayout
        Me.TabPageGEN.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.TabPgOptions.SuspendLayout
        Me.PanelOptAllg.SuspendLayout
        Me.gbXMLHashValidation.SuspendLayout
        Me.GroupBox4.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.GroupBox2.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.TabPageDEV.SuspendLayout
        Me.ConMenFilelist.SuspendLayout
        CType(Me.SplitContainer1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SplitContainer1.Panel1.SuspendLayout
        Me.SplitContainer1.Panel2.SuspendLayout
        Me.SplitContainer1.SuspendLayout
        Me.ToolStrip1.SuspendLayout
        Me.CmOpenFile.SuspendLayout
        Me.SuspendLayout
        '
        'StatusBAR
        '
        Me.StatusBAR.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusBAR.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLbStatus, Me.ToolStripProgBarJob, Me.ToolStripProgBarOverall})
        Me.StatusBAR.Location = New System.Drawing.Point(0, 648)
        Me.StatusBAR.Name = "StatusBAR"
        Me.StatusBAR.Size = New System.Drawing.Size(1045, 22)
        Me.StatusBAR.TabIndex = 7
        Me.StatusBAR.Text = "StatusBAR"
        '
        'ToolStripLbStatus
        '
        Me.ToolStripLbStatus.Name = "ToolStripLbStatus"
        Me.ToolStripLbStatus.Size = New System.Drawing.Size(1030, 17)
        Me.ToolStripLbStatus.Spring = true
        Me.ToolStripLbStatus.Text = "Status Text"
        Me.ToolStripLbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripProgBarJob
        '
        Me.ToolStripProgBarJob.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgBarJob.AutoSize = false
        Me.ToolStripProgBarJob.Name = "ToolStripProgBarJob"
        Me.ToolStripProgBarJob.Size = New System.Drawing.Size(100, 16)
        Me.ToolStripProgBarJob.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ToolStripProgBarJob.ToolTipText = "overall progress"
        Me.ToolStripProgBarJob.Visible = false
        '
        'ToolStripProgBarOverall
        '
        Me.ToolStripProgBarOverall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgBarOverall.AutoSize = false
        Me.ToolStripProgBarOverall.Name = "ToolStripProgBarOverall"
        Me.ToolStripProgBarOverall.Size = New System.Drawing.Size(100, 16)
        Me.ToolStripProgBarOverall.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ToolStripProgBarOverall.ToolTipText = "job progress"
        Me.ToolStripProgBarOverall.Visible = false
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPageGEN)
        Me.TabControl1.Controls.Add(Me.TabPgOptions)
        Me.TabControl1.Controls.Add(Me.TabPageDEV)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(3, 3)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(0, 0)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1042, 328)
        Me.TabControl1.TabIndex = 10
        '
        'TabPageGEN
        '
        Me.TabPageGEN.Controls.Add(Me.btnImportXML)
        Me.TabPageGEN.Controls.Add(Me.btnExportXML)
        Me.TabPageGEN.Controls.Add(Me.Label6)
        Me.TabPageGEN.Controls.Add(Me.btStartV3)
        Me.TabPageGEN.Controls.Add(Me.LbDecl)
        Me.TabPageGEN.Controls.Add(Me.PictureBox1)
        Me.TabPageGEN.Controls.Add(Me.BtGENdown)
        Me.TabPageGEN.Controls.Add(Me.BtGENup)
        Me.TabPageGEN.Controls.Add(Me.ChBoxAllGEN)
        Me.TabPageGEN.Controls.Add(Me.LvGEN)
        Me.TabPageGEN.Controls.Add(Me.ButtonGENremove)
        Me.TabPageGEN.Controls.Add(Me.ButtonGENadd)
        Me.TabPageGEN.Location = New System.Drawing.Point(4, 22)
        Me.TabPageGEN.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPageGEN.Name = "TabPageGEN"
        Me.TabPageGEN.Size = New System.Drawing.Size(1034, 302)
        Me.TabPageGEN.TabIndex = 0
        Me.TabPageGEN.Text = "Job Files"
        Me.TabPageGEN.UseVisualStyleBackColor = true
        '
        'btnImportXML
        '
        Me.btnImportXML.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnImportXML.Location = New System.Drawing.Point(460, 267)
        Me.btnImportXML.Name = "btnImportXML"
        Me.btnImportXML.Size = New System.Drawing.Size(115, 30)
        Me.btnImportXML.TabIndex = 23
        Me.btnImportXML.Text = "Import from XML"
        Me.btnImportXML.UseVisualStyleBackColor = true
        Me.btnImportXML.Visible = false
        '
        'btnExportXML
        '
        Me.btnExportXML.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnExportXML.Enabled = false
        Me.btnExportXML.Location = New System.Drawing.Point(344, 267)
        Me.btnExportXML.Name = "btnExportXML"
        Me.btnExportXML.Size = New System.Drawing.Size(115, 30)
        Me.btnExportXML.TabIndex = 22
        Me.btnExportXML.Text = "Export as XML"
        Me.btnExportXML.UseVisualStyleBackColor = true
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(814, 268)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(217, 13)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "(Double-Click to Edit, Right-Click for Options)"
        '
        'btStartV3
        '
        Me.btStartV3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.btStartV3.Image = Global.TUGraz.VECTO.My.Resources.Resources.Play_icon
        Me.btStartV3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btStartV3.Location = New System.Drawing.Point(3, 56)
        Me.btStartV3.Name = "btStartV3"
        Me.btStartV3.Size = New System.Drawing.Size(108, 50)
        Me.btStartV3.TabIndex = 20
        Me.btStartV3.Text = "START"
        Me.btStartV3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ToolTip1.SetToolTip(Me.btStartV3, "Start Simulation")
        Me.btStartV3.UseVisualStyleBackColor = true
        '
        'LbDecl
        '
        Me.LbDecl.AutoSize = true
        Me.LbDecl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LbDecl.Location = New System.Drawing.Point(5, 109)
        Me.LbDecl.Name = "LbDecl"
        Me.LbDecl.Size = New System.Drawing.Size(107, 13)
        Me.LbDecl.TabIndex = 19
        Me.LbDecl.Text = "Declaration Mode"
        Me.LbDecl.Visible = false
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"),System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(3, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(108, 47)
        Me.PictureBox1.TabIndex = 18
        Me.PictureBox1.TabStop = false
        '
        'BtGENdown
        '
        Me.BtGENdown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.BtGENdown.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_arrow_down_icon
        Me.BtGENdown.Location = New System.Drawing.Point(307, 267)
        Me.BtGENdown.Name = "BtGENdown"
        Me.BtGENdown.Size = New System.Drawing.Size(30, 30)
        Me.BtGENdown.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.BtGENdown, "Move job down one row")
        Me.BtGENdown.UseVisualStyleBackColor = true
        '
        'BtGENup
        '
        Me.BtGENup.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.BtGENup.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_arrow_up_icon
        Me.BtGENup.Location = New System.Drawing.Point(276, 267)
        Me.BtGENup.Name = "BtGENup"
        Me.BtGENup.Size = New System.Drawing.Size(30, 30)
        Me.BtGENup.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.BtGENup, "Move job up one row")
        Me.BtGENup.UseVisualStyleBackColor = true
        '
        'ChBoxAllGEN
        '
        Me.ChBoxAllGEN.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ChBoxAllGEN.AutoSize = true
        Me.ChBoxAllGEN.Location = New System.Drawing.Point(195, 274)
        Me.ChBoxAllGEN.Name = "ChBoxAllGEN"
        Me.ChBoxAllGEN.Size = New System.Drawing.Size(70, 17)
        Me.ChBoxAllGEN.TabIndex = 16
        Me.ChBoxAllGEN.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.ChBoxAllGEN, "Select All / None")
        Me.ChBoxAllGEN.UseVisualStyleBackColor = true
        '
        'LvGEN
        '
        Me.LvGEN.AllowDrop = true
        Me.LvGEN.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvGEN.CheckBoxes = true
        Me.LvGEN.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColGENpath, Me.ColGENstatus})
        Me.LvGEN.FullRowSelect = true
        Me.LvGEN.GridLines = true
        Me.LvGEN.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LvGEN.HideSelection = false
        Me.LvGEN.LabelEdit = true
        Me.LvGEN.Location = New System.Drawing.Point(114, 3)
        Me.LvGEN.Name = "LvGEN"
        Me.LvGEN.Size = New System.Drawing.Size(917, 263)
        Me.LvGEN.TabIndex = 14
        Me.LvGEN.UseCompatibleStateImageBehavior = false
        Me.LvGEN.View = System.Windows.Forms.View.Details
        '
        'ColGENpath
        '
        Me.ColGENpath.Text = "Filepath"
        Me.ColGENpath.Width = 797
        '
        'ColGENstatus
        '
        Me.ColGENstatus.Text = ""
        Me.ColGENstatus.Width = 175
        '
        'ButtonGENremove
        '
        Me.ButtonGENremove.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ButtonGENremove.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ButtonGENremove.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.ButtonGENremove.Location = New System.Drawing.Point(147, 267)
        Me.ButtonGENremove.Name = "ButtonGENremove"
        Me.ButtonGENremove.Size = New System.Drawing.Size(33, 30)
        Me.ButtonGENremove.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.ButtonGENremove, "Remove selected entries")
        Me.ButtonGENremove.UseVisualStyleBackColor = true
        '
        'ButtonGENadd
        '
        Me.ButtonGENadd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ButtonGENadd.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ButtonGENadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.ButtonGENadd.Location = New System.Drawing.Point(113, 267)
        Me.ButtonGENadd.Name = "ButtonGENadd"
        Me.ButtonGENadd.Size = New System.Drawing.Size(33, 30)
        Me.ButtonGENadd.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.ButtonGENadd, "Add Job File")
        Me.ButtonGENadd.UseVisualStyleBackColor = true
        '
        'TabPgOptions
        '
        Me.TabPgOptions.Controls.Add(Me.PanelOptAllg)
        Me.TabPgOptions.Location = New System.Drawing.Point(4, 22)
        Me.TabPgOptions.Name = "TabPgOptions"
        Me.TabPgOptions.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPgOptions.Size = New System.Drawing.Size(1034, 302)
        Me.TabPgOptions.TabIndex = 2
        Me.TabPgOptions.Text = "Options"
        Me.TabPgOptions.UseVisualStyleBackColor = true
        '
        'PanelOptAllg
        '
        Me.PanelOptAllg.Controls.Add(Me.gbXMLHashValidation)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox4)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox3)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox2)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox1)
        Me.PanelOptAllg.Location = New System.Drawing.Point(6, 6)
        Me.PanelOptAllg.Name = "PanelOptAllg"
        Me.PanelOptAllg.Size = New System.Drawing.Size(1022, 290)
        Me.PanelOptAllg.TabIndex = 0
        '
        'gbXMLHashValidation
        '
        Me.gbXMLHashValidation.Controls.Add(Me.cbValidateXMLDeclarationJobs)
        Me.gbXMLHashValidation.Location = New System.Drawing.Point(183, 56)
        Me.gbXMLHashValidation.Name = "gbXMLHashValidation"
        Me.gbXMLHashValidation.Size = New System.Drawing.Size(260, 72)
        Me.gbXMLHashValidation.TabIndex = 20
        Me.gbXMLHashValidation.TabStop = false
        Me.gbXMLHashValidation.Text = "XML Declaration Jobs"
        '
        'cbValidateXMLDeclarationJobs
        '
        Me.cbValidateXMLDeclarationJobs.AutoSize = true
        Me.cbValidateXMLDeclarationJobs.Checked = true
        Me.cbValidateXMLDeclarationJobs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbValidateXMLDeclarationJobs.Location = New System.Drawing.Point(6, 19)
        Me.cbValidateXMLDeclarationJobs.Name = "cbValidateXMLDeclarationJobs"
        Me.cbValidateXMLDeclarationJobs.Size = New System.Drawing.Size(222, 17)
        Me.cbValidateXMLDeclarationJobs.TabIndex = 0
        Me.cbValidateXMLDeclarationJobs.Text = "Validate Hashes of XML Declaration Jobs"
        Me.cbValidateXMLDeclarationJobs.UseVisualStyleBackColor = true
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.BtTCfileBrowse)
        Me.GroupBox4.Controls.Add(Me.tbOutputFolder)
        Me.GroupBox4.Location = New System.Drawing.Point(183, 4)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(260, 46)
        Me.GroupBox4.TabIndex = 19
        Me.GroupBox4.TabStop = false
        Me.GroupBox4.Text = "Output Directory"
        '
        'BtTCfileBrowse
        '
        Me.BtTCfileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtTCfileBrowse.Location = New System.Drawing.Point(227, 14)
        Me.BtTCfileBrowse.Name = "BtTCfileBrowse"
        Me.BtTCfileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtTCfileBrowse.TabIndex = 27
        Me.BtTCfileBrowse.TabStop = false
        Me.BtTCfileBrowse.UseVisualStyleBackColor = true
        '
        'tbOutputFolder
        '
        Me.tbOutputFolder.Location = New System.Drawing.Point(6, 17)
        Me.tbOutputFolder.Name = "tbOutputFolder"
        Me.tbOutputFolder.Size = New System.Drawing.Size(215, 20)
        Me.tbOutputFolder.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cbActVmod)
        Me.GroupBox3.Controls.Add(Me.cbValidateRunData)
        Me.GroupBox3.Location = New System.Drawing.Point(3, 177)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(173, 110)
        Me.GroupBox3.TabIndex = 18
        Me.GroupBox3.TabStop = false
        Me.GroupBox3.Text = "Misc"
        '
        'cbActVmod
        '
        Me.cbActVmod.Location = New System.Drawing.Point(6, 41)
        Me.cbActVmod.Name = "cbActVmod"
        Me.cbActVmod.Size = New System.Drawing.Size(161, 63)
        Me.cbActVmod.TabIndex = 18
        Me.cbActVmod.Text = "Output values in vmod at beginning and end of simulation interval (EXPERT!)"
        Me.cbActVmod.UseVisualStyleBackColor = true
        '
        'cbValidateRunData
        '
        Me.cbValidateRunData.AutoSize = true
        Me.cbValidateRunData.Checked = true
        Me.cbValidateRunData.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbValidateRunData.Location = New System.Drawing.Point(6, 19)
        Me.cbValidateRunData.Name = "cbValidateRunData"
        Me.cbValidateRunData.Size = New System.Drawing.Size(90, 17)
        Me.cbValidateRunData.TabIndex = 17
        Me.cbValidateRunData.Text = "Validate Data"
        Me.cbValidateRunData.UseVisualStyleBackColor = true
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ChBoxModOut)
        Me.GroupBox2.Controls.Add(Me.ChBoxMod1Hz)
        Me.GroupBox2.Location = New System.Drawing.Point(3, 82)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(173, 89)
        Me.GroupBox2.TabIndex = 16
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Output"
        '
        'ChBoxModOut
        '
        Me.ChBoxModOut.AutoSize = true
        Me.ChBoxModOut.Checked = true
        Me.ChBoxModOut.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBoxModOut.Location = New System.Drawing.Point(6, 19)
        Me.ChBoxModOut.Name = "ChBoxModOut"
        Me.ChBoxModOut.Size = New System.Drawing.Size(115, 17)
        Me.ChBoxModOut.TabIndex = 0
        Me.ChBoxModOut.Text = "Write modal results"
        Me.ChBoxModOut.UseVisualStyleBackColor = true
        '
        'ChBoxMod1Hz
        '
        Me.ChBoxMod1Hz.AutoSize = true
        Me.ChBoxMod1Hz.Location = New System.Drawing.Point(6, 42)
        Me.ChBoxMod1Hz.Name = "ChBoxMod1Hz"
        Me.ChBoxMod1Hz.Size = New System.Drawing.Size(121, 17)
        Me.ChBoxMod1Hz.TabIndex = 16
        Me.ChBoxMod1Hz.Text = "Modal results in 1Hz"
        Me.ChBoxMod1Hz.UseVisualStyleBackColor = true
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.RbDev)
        Me.GroupBox1.Controls.Add(Me.RbDecl)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(173, 72)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Mode"
        '
        'RbDev
        '
        Me.RbDev.AutoSize = true
        Me.RbDev.Checked = true
        Me.RbDev.Location = New System.Drawing.Point(6, 42)
        Me.RbDev.Name = "RbDev"
        Me.RbDev.Size = New System.Drawing.Size(111, 17)
        Me.RbDev.TabIndex = 1
        Me.RbDev.TabStop = true
        Me.RbDev.Text = "Engineering Mode"
        Me.RbDev.UseVisualStyleBackColor = true
        '
        'RbDecl
        '
        Me.RbDecl.AutoSize = true
        Me.RbDecl.Location = New System.Drawing.Point(6, 19)
        Me.RbDecl.Name = "RbDecl"
        Me.RbDecl.Size = New System.Drawing.Size(109, 17)
        Me.RbDecl.TabIndex = 0
        Me.RbDecl.TabStop = true
        Me.RbDecl.Text = "Declaration Mode"
        Me.RbDecl.UseVisualStyleBackColor = true
        '
        'TabPageDEV
        '
        Me.TabPageDEV.Controls.Add(Me.Label1)
        Me.TabPageDEV.Controls.Add(Me.LvDEVoptions)
        Me.TabPageDEV.Location = New System.Drawing.Point(4, 22)
        Me.TabPageDEV.Name = "TabPageDEV"
        Me.TabPageDEV.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageDEV.Size = New System.Drawing.Size(1034, 302)
        Me.TabPageDEV.TabIndex = 3
        Me.TabPageDEV.Text = "Test"
        Me.TabPageDEV.UseVisualStyleBackColor = true
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(1012, 283)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(106, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "(Double-Click to Edit)"
        '
        'LvDEVoptions
        '
        Me.LvDEVoptions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvDEVoptions.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader7, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader8, Me.ColumnHeader9})
        Me.LvDEVoptions.FullRowSelect = true
        Me.LvDEVoptions.GridLines = true
        Me.LvDEVoptions.HideSelection = false
        Me.LvDEVoptions.Location = New System.Drawing.Point(6, 6)
        Me.LvDEVoptions.MultiSelect = false
        Me.LvDEVoptions.Name = "LvDEVoptions"
        Me.LvDEVoptions.Size = New System.Drawing.Size(1022, 277)
        Me.LvDEVoptions.TabIndex = 0
        Me.LvDEVoptions.UseCompatibleStateImageBehavior = false
        Me.LvDEVoptions.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Property"
        Me.ColumnHeader4.Width = 89
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Description"
        Me.ColumnHeader7.Width = 527
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Type"
        Me.ColumnHeader5.Width = 82
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Value"
        Me.ColumnHeader6.Width = 134
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Default"
        Me.ColumnHeader8.Width = 120
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Saved In DEVconfig.txt"
        Me.ColumnHeader9.Width = 129
        '
        'ConMenFilelist
        '
        Me.ConMenFilelist.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ConMenFilelist.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowInFolderMenuItem, Me.SaveListToolStripMenuItem, Me.LoadListToolStripMenuItem, Me.LoadDefaultListToolStripMenuItem, Me.ClearListToolStripMenuItem})
        Me.ConMenFilelist.Name = "ConMenFilelist"
        Me.ConMenFilelist.ShowImageMargin = false
        Me.ConMenFilelist.Size = New System.Drawing.Size(151, 114)
        '
        'ShowInFolderMenuItem
        '
        Me.ShowInFolderMenuItem.Name = "ShowInFolderMenuItem"
        Me.ShowInFolderMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.ShowInFolderMenuItem.Text = "Show in Folder"
        '
        'SaveListToolStripMenuItem
        '
        Me.SaveListToolStripMenuItem.Name = "SaveListToolStripMenuItem"
        Me.SaveListToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.SaveListToolStripMenuItem.Text = "Save List..."
        '
        'LoadListToolStripMenuItem
        '
        Me.LoadListToolStripMenuItem.Name = "LoadListToolStripMenuItem"
        Me.LoadListToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.LoadListToolStripMenuItem.Text = "Load List..."
        '
        'LoadDefaultListToolStripMenuItem
        '
        Me.LoadDefaultListToolStripMenuItem.Name = "LoadDefaultListToolStripMenuItem"
        Me.LoadDefaultListToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.LoadDefaultListToolStripMenuItem.Text = "Load Autosave-List"
        '
        'ClearListToolStripMenuItem
        '
        Me.ClearListToolStripMenuItem.Name = "ClearListToolStripMenuItem"
        Me.ClearListToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
        Me.ClearListToolStripMenuItem.Text = "Clear List"
        '
        'LvMsg
        '
        Me.LvMsg.AllowColumnReorder = true
        Me.LvMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LvMsg.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.LvMsg.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LvMsg.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LvMsg.FullRowSelect = true
        Me.LvMsg.GridLines = true
        Me.LvMsg.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LvMsg.HideSelection = false
        Me.LvMsg.LabelWrap = false
        Me.LvMsg.Location = New System.Drawing.Point(0, 0)
        Me.LvMsg.Margin = New System.Windows.Forms.Padding(0)
        Me.LvMsg.Name = "LvMsg"
        Me.LvMsg.Size = New System.Drawing.Size(1045, 281)
        Me.LvMsg.TabIndex = 0
        Me.LvMsg.UseCompatibleStateImageBehavior = false
        Me.LvMsg.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Message"
        Me.ColumnHeader1.Width = 779
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Time"
        Me.ColumnHeader2.Width = 151
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Source"
        Me.ColumnHeader3.Width = 138
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 27)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabControl1)
        Me.SplitContainer1.Panel1.Padding = New System.Windows.Forms.Padding(3, 3, 0, 2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.LvMsg)
        Me.SplitContainer1.Size = New System.Drawing.Size(1045, 618)
        Me.SplitContainer1.SplitterDistance = 333
        Me.SplitContainer1.TabIndex = 12
        '
        'ToolStrip1
        '
        Me.ToolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripSeparator2, Me.ToolStripDrDnBtTools, Me.ToolStripDrDnBtInfo})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1045, 31)
        Me.ToolStrip1.TabIndex = 11
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtNew.Text = "ToolStripBtNew"
        Me.ToolStripBtNew.ToolTipText = "New Job File"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open File..."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripDrDnBtTools
        '
        Me.ToolStripDrDnBtTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GENEditorToolStripMenuItem1, Me.EPTPJobEditorToolStripMenuItem, Me.VEHEditorToolStripMenuItem, Me.EngineEditorToolStripMenuItem, Me.GearboxEditorToolStripMenuItem, Me.GraphToolStripMenuItem, Me.ToolStripSeparator6, Me.OpenLogToolStripMenuItem, Me.SettingsToolStripMenuItem})
        Me.ToolStripDrDnBtTools.Image = Global.TUGraz.VECTO.My.Resources.Resources.Misc_Tools_icon
        Me.ToolStripDrDnBtTools.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDrDnBtTools.Name = "ToolStripDrDnBtTools"
        Me.ToolStripDrDnBtTools.Size = New System.Drawing.Size(71, 28)
        Me.ToolStripDrDnBtTools.Text = "Tools"
        '
        'GENEditorToolStripMenuItem1
        '
        Me.GENEditorToolStripMenuItem1.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.GENEditorToolStripMenuItem1.Name = "GENEditorToolStripMenuItem1"
        Me.GENEditorToolStripMenuItem1.Size = New System.Drawing.Size(160, 30)
        Me.GENEditorToolStripMenuItem1.Text = "Job Editor"
        '
        'EPTPJobEditorToolStripMenuItem
        '
        Me.EPTPJobEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.EPTPJobEditorToolStripMenuItem.Name = "EPTPJobEditorToolStripMenuItem"
        Me.EPTPJobEditorToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.EPTPJobEditorToolStripMenuItem.Text = "VTP Job Editor"
        '
        'VEHEditorToolStripMenuItem
        '
        Me.VEHEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VEH
        Me.VEHEditorToolStripMenuItem.Name = "VEHEditorToolStripMenuItem"
        Me.VEHEditorToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.VEHEditorToolStripMenuItem.Text = "Vehicle Editor"
        '
        'EngineEditorToolStripMenuItem
        '
        Me.EngineEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_ENG
        Me.EngineEditorToolStripMenuItem.Name = "EngineEditorToolStripMenuItem"
        Me.EngineEditorToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.EngineEditorToolStripMenuItem.Text = "Engine Editor"
        '
        'GearboxEditorToolStripMenuItem
        '
        Me.GearboxEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_GBX
        Me.GearboxEditorToolStripMenuItem.Name = "GearboxEditorToolStripMenuItem"
        Me.GearboxEditorToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.GearboxEditorToolStripMenuItem.Text = "Gearbox Editor"
        '
        'GraphToolStripMenuItem
        '
        Me.GraphToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_Graph
        Me.GraphToolStripMenuItem.Name = "GraphToolStripMenuItem"
        Me.GraphToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.GraphToolStripMenuItem.Text = "Graph"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(157, 6)
        '
        'OpenLogToolStripMenuItem
        '
        Me.OpenLogToolStripMenuItem.Name = "OpenLogToolStripMenuItem"
        Me.OpenLogToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.OpenLogToolStripMenuItem.Text = "Open Log"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(160, 30)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'ToolStripDrDnBtInfo
        '
        Me.ToolStripDrDnBtInfo.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UserManualToolStripMenuItem, Me.UpdateNotesToolStripMenuItem, Me.ReportBugViaCITnetToolStripMenuItem, Me.ToolStripSeparator3, Me.AboutVECTOToolStripMenuItem1})
        Me.ToolStripDrDnBtInfo.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripDrDnBtInfo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDrDnBtInfo.Name = "ToolStripDrDnBtInfo"
        Me.ToolStripDrDnBtInfo.Size = New System.Drawing.Size(69, 28)
        Me.ToolStripDrDnBtInfo.Text = "Help"
        '
        'UserManualToolStripMenuItem
        '
        Me.UserManualToolStripMenuItem.Name = "UserManualToolStripMenuItem"
        Me.UserManualToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.UserManualToolStripMenuItem.Text = "User Manual"
        '
        'UpdateNotesToolStripMenuItem
        '
        Me.UpdateNotesToolStripMenuItem.Name = "UpdateNotesToolStripMenuItem"
        Me.UpdateNotesToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.UpdateNotesToolStripMenuItem.Text = "Release Notes"
        '
        'ReportBugViaCITnetToolStripMenuItem
        '
        Me.ReportBugViaCITnetToolStripMenuItem.Name = "ReportBugViaCITnetToolStripMenuItem"
        Me.ReportBugViaCITnetToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.ReportBugViaCITnetToolStripMenuItem.Text = "Report Bug via CITnet / JIRA"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(217, 6)
        '
        'AboutVECTOToolStripMenuItem1
        '
        Me.AboutVECTOToolStripMenuItem1.Name = "AboutVECTOToolStripMenuItem1"
        Me.AboutVECTOToolStripMenuItem1.Size = New System.Drawing.Size(220, 22)
        Me.AboutVECTOToolStripMenuItem1.Text = "About VECTO"
        '
        'CmDEV
        '
        Me.CmDEV.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmDEV.Name = "CmDEV"
        Me.CmDEV.ShowImageMargin = false
        Me.CmDEV.Size = New System.Drawing.Size(36, 4)
        '
        'TmProgSec
        '
        Me.TmProgSec.Interval = 1000
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.OpenInGraphWindowToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = false
        Me.CmOpenFile.Size = New System.Drawing.Size(174, 70)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'OpenInGraphWindowToolStripMenuItem
        '
        Me.OpenInGraphWindowToolStripMenuItem.Name = "OpenInGraphWindowToolStripMenuItem"
        Me.OpenInGraphWindowToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.OpenInGraphWindowToolStripMenuItem.Text = "Open in Graph Window"
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(173, 22)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(7, 19)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(84, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Output Directory"
        '
        'MainForm
        '
        Me.AcceptButton = Me.btStartV3
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1045, 670)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusBAR)
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(785, 485)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "VECTO"
        Me.StatusBAR.ResumeLayout(false)
        Me.StatusBAR.PerformLayout
        Me.TabControl1.ResumeLayout(false)
        Me.TabPageGEN.ResumeLayout(false)
        Me.TabPageGEN.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.TabPgOptions.ResumeLayout(false)
        Me.PanelOptAllg.ResumeLayout(false)
        Me.gbXMLHashValidation.ResumeLayout(false)
        Me.gbXMLHashValidation.PerformLayout
        Me.GroupBox4.ResumeLayout(false)
        Me.GroupBox4.PerformLayout
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox3.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.TabPageDEV.ResumeLayout(false)
        Me.TabPageDEV.PerformLayout
        Me.ConMenFilelist.ResumeLayout(false)
        Me.SplitContainer1.Panel1.ResumeLayout(false)
        Me.SplitContainer1.Panel2.ResumeLayout(false)
        CType(Me.SplitContainer1,System.ComponentModel.ISupportInitialize).EndInit
        Me.SplitContainer1.ResumeLayout(false)
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.CmOpenFile.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents StatusBAR As StatusStrip
	Friend WithEvents ToolStripLbStatus As ToolStripStatusLabel
	Friend WithEvents TabControl1 As TabControl
	Friend WithEvents TabPageGEN As TabPage
	Friend WithEvents ButtonGENadd As Button
	Friend WithEvents ButtonGENremove As Button
	Friend WithEvents ConMenFilelist As ContextMenuStrip
	Friend WithEvents SaveListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LoadListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LoadDefaultListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ClearListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BackgroundWorker1 As BackgroundWorker
	Friend WithEvents ToolStripProgBarOverall As ToolStripProgressBar
	Friend WithEvents LvGEN As ListView
	Friend WithEvents ColGENpath As ColumnHeader
	Friend WithEvents ColGENstatus As ColumnHeader
	Friend WithEvents ChBoxAllGEN As CheckBox
	Friend WithEvents TabPgOptions As TabPage
	Friend WithEvents ChBoxModOut As CheckBox
	Friend WithEvents PanelOptAllg As Panel
	Friend WithEvents LvMsg As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents TabPageDEV As TabPage
	Friend WithEvents LvDEVoptions As ListView
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents CmDEV As ContextMenuStrip
	Friend WithEvents ColumnHeader7 As ColumnHeader
	Friend WithEvents BtGENup As Button
	Friend WithEvents BtGENdown As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripDrDnBtTools As ToolStripDropDownButton
	Friend WithEvents GENEditorToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents VEHEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenLogToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripDrDnBtInfo As ToolStripDropDownButton
	Friend WithEvents AboutVECTOToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
	Friend WithEvents ToolStripProgBarJob As ToolStripProgressBar
	Friend WithEvents TmProgSec As Timer
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents EngineEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents GearboxEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UserManualToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ColumnHeader8 As ColumnHeader
	Friend WithEvents ColumnHeader9 As ColumnHeader
	Friend WithEvents UpdateNotesToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
	Friend WithEvents LbDecl As Label
	Friend WithEvents GraphToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenInGraphWindowToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents RbDev As RadioButton
	Friend WithEvents RbDecl As RadioButton
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents Label1 As Label
	Friend WithEvents ReportBugViaCITnetToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents btStartV3 As Button
	Friend WithEvents ChBoxMod1Hz As CheckBox
	Friend WithEvents Label6 As Label
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents NewToolStripButton As ToolStripButton
	Friend WithEvents OpenToolStripButton As ToolStripButton
	Friend WithEvents SaveToolStripButton As ToolStripButton
	Friend WithEvents PrintToolStripButton As ToolStripButton
	Friend WithEvents toolStripSeparator As ToolStripSeparator
	Friend WithEvents CutToolStripButton As ToolStripButton
	Friend WithEvents CopyToolStripButton As ToolStripButton
	Friend WithEvents PasteToolStripButton As ToolStripButton
	Friend WithEvents toolStripSeparator1 As ToolStripSeparator
	Friend WithEvents HelpToolStripButton As ToolStripButton
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents btnExportXML As System.Windows.Forms.Button
	Friend WithEvents btnImportXML As System.Windows.Forms.Button
	Friend WithEvents ShowInFolderMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents cbValidateRunData As System.Windows.Forms.CheckBox
	Friend WithEvents cbActVmod As System.Windows.Forms.CheckBox
	Friend WithEvents EPTPJobEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents tbOutputFolder As TextBox
    Friend WithEvents BtTCfileBrowse As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents gbXMLHashValidation As GroupBox
    Friend WithEvents cbValidateXMLDeclarationJobs As CheckBox
End Class
