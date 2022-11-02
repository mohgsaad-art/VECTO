Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VectoJobForm
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VectoJobForm))
		Me.TabPgGen = New System.Windows.Forms.TabPage()
		Me.pnHybridStrategy = New System.Windows.Forms.Panel()
		Me.btnOpenHybridStrategyParameters = New System.Windows.Forms.Button()
		Me.tbHybridStrategyParams = New System.Windows.Forms.TextBox()
		Me.btnBrowseHybridStrategyParams = New System.Windows.Forms.Button()
		Me.pnVehicle = New System.Windows.Forms.Panel()
		Me.ButOpenVEH = New System.Windows.Forms.Button()
		Me.ButtonVEH = New System.Windows.Forms.Button()
		Me.TbVEH = New System.Windows.Forms.TextBox()
		Me.pnEngine = New System.Windows.Forms.Panel()
		Me.ButOpenENG = New System.Windows.Forms.Button()
		Me.ButtonMAP = New System.Windows.Forms.Button()
		Me.TbENG = New System.Windows.Forms.TextBox()
		Me.pnGearbox = New System.Windows.Forms.Panel()
		Me.ButOpenGBX = New System.Windows.Forms.Button()
		Me.ButtonGBX = New System.Windows.Forms.Button()
		Me.TbGBX = New System.Windows.Forms.TextBox()
		Me.pnShiftParams = New System.Windows.Forms.Panel()
		Me.BtnShiftParamsForm = New System.Windows.Forms.Button()
		Me.TbShiftStrategyParams = New System.Windows.Forms.TextBox()
		Me.BtnShiftStrategyParams = New System.Windows.Forms.Button()
		Me.GrCycles = New System.Windows.Forms.GroupBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.LvCycles = New System.Windows.Forms.ListView()
		Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader()
		Me.BtDRIrem = New System.Windows.Forms.Button()
		Me.BtDRIadd = New System.Windows.Forms.Button()
		Me.GrAuxMech = New System.Windows.Forms.GroupBox()
		Me.pnAuxDeclarationMode = New System.Windows.Forms.Panel()
		Me.LvAux = New System.Windows.Forms.ListView()
		Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader()
		Me.ColumnHeader5 = New System.Windows.Forms.ColumnHeader()
		Me.ColumnHeader6 = New System.Windows.Forms.ColumnHeader()
		Me.Label32 = New System.Windows.Forms.Label()
		Me.pnAuxEngineering = New System.Windows.Forms.Panel()
		Me.tbPAuxStandstillICEOff = New System.Windows.Forms.TextBox()
		Me.lblAuxStandstillICEOffUnit = New System.Windows.Forms.Label()
		Me.lblAuxStandstillICEOff = New System.Windows.Forms.Label()
		Me.tbPAuxDrivingICEOff = New System.Windows.Forms.TextBox()
		Me.lblAuxDrivingICEOffUnit = New System.Windows.Forms.Label()
		Me.lblAuxDrivingICEOff = New System.Windows.Forms.Label()
		Me.TbAuxPAuxICEOn = New System.Windows.Forms.TextBox()
		Me.lblAuxICEOnUnit = New System.Windows.Forms.Label()
		Me.lblAuxICEOn = New System.Windows.Forms.Label()
		Me.GrAux = New System.Windows.Forms.GroupBox()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.TbAuxPAdd = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.btnAAUXOpen = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.btnBrowseAAUXFile = New System.Windows.Forms.Button()
		Me.txtAdvancedAuxiliaryFile = New System.Windows.Forms.TextBox()
		Me.picAuxInfo = New System.Windows.Forms.PictureBox()
		Me.cboAdvancedAuxiliaries = New System.Windows.Forms.ComboBox()
		Me.lbAdvancedAuxiliaries = New System.Windows.Forms.Label()
		Me.tcJob = New System.Windows.Forms.TabControl()
		Me.tpAuxiliaries = New System.Windows.Forms.TabPage()
		Me.gbBusAux = New System.Windows.Forms.GroupBox()
		Me.cbEnableBusAux = New System.Windows.Forms.CheckBox()
		Me.pnBusAux = New System.Windows.Forms.Panel()
		Me.btnBusAuxP = New System.Windows.Forms.Button()
		Me.tbBusAuxParams = New System.Windows.Forms.TextBox()
		Me.btnBrowsBusAuxParams = New System.Windows.Forms.Button()
		Me.gbElectricAux = New System.Windows.Forms.GroupBox()
		Me.Label46 = New System.Windows.Forms.Label()
		Me.lblElAuxConstUnit = New System.Windows.Forms.Label()
		Me.tbElectricAuxConstant = New System.Windows.Forms.TextBox()
		Me.lblElAuxConst = New System.Windows.Forms.Label()
		Me.tpCycles = New System.Windows.Forms.TabPage()
		Me.TabPgDriver = New System.Windows.Forms.TabPage()
		Me.GrVACC = New System.Windows.Forms.GroupBox()
		Me.Label15 = New System.Windows.Forms.Label()
		Me.TbDesMaxFile = New System.Windows.Forms.TextBox()
		Me.BtDesMaxBr = New System.Windows.Forms.Button()
		Me.BtAccOpen = New System.Windows.Forms.Button()
		Me.GrLAC = New System.Windows.Forms.GroupBox()
		Me.pnLookAheadCoasting = New System.Windows.Forms.Panel()
		Me.Label49 = New System.Windows.Forms.Label()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.tbLacMinSpeed = New System.Windows.Forms.TextBox()
		Me.btnDfVelocityDrop = New System.Windows.Forms.Button()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.tbDfCoastingScale = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.tbDfCoastingOffset = New System.Windows.Forms.TextBox()
		Me.tbLacDfTargetSpeedFile = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.btnDfTargetSpeed = New System.Windows.Forms.Button()
		Me.tbLacPreviewFactor = New System.Windows.Forms.TextBox()
		Me.tbLacDfVelocityDropFile = New System.Windows.Forms.TextBox()
		Me.CbLookAhead = New System.Windows.Forms.CheckBox()
		Me.gbOverspeed = New System.Windows.Forms.GroupBox()
		Me.PnEcoRoll = New System.Windows.Forms.Panel()
		Me.Label21 = New System.Windows.Forms.Label()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.TbVmin = New System.Windows.Forms.TextBox()
		Me.TbOverspeed = New System.Windows.Forms.TextBox()
		Me.Label23 = New System.Windows.Forms.Label()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.RdOverspeed = New System.Windows.Forms.RadioButton()
		Me.RdOff = New System.Windows.Forms.RadioButton()
		Me.TabPgADAS = New System.Windows.Forms.TabPage()
		Me.gbPCC = New System.Windows.Forms.GroupBox()
		Me.Label43 = New System.Windows.Forms.Label()
		Me.Label42 = New System.Windows.Forms.Label()
		Me.Label40 = New System.Windows.Forms.Label()
		Me.Label41 = New System.Windows.Forms.Label()
		Me.tbPCCPreviewUseCase2 = New System.Windows.Forms.TextBox()
		Me.Label38 = New System.Windows.Forms.Label()
		Me.Label39 = New System.Windows.Forms.Label()
		Me.tbPCCPreviewUseCase1 = New System.Windows.Forms.TextBox()
		Me.Label36 = New System.Windows.Forms.Label()
		Me.Label37 = New System.Windows.Forms.Label()
		Me.tbPCCMinSpeed = New System.Windows.Forms.TextBox()
		Me.Label34 = New System.Windows.Forms.Label()
		Me.Label35 = New System.Windows.Forms.Label()
		Me.tbPCCEnableSpeed = New System.Windows.Forms.TextBox()
		Me.Label31 = New System.Windows.Forms.Label()
		Me.Label33 = New System.Windows.Forms.Label()
		Me.tbPCCOverspeed = New System.Windows.Forms.TextBox()
		Me.Label20 = New System.Windows.Forms.Label()
		Me.Label22 = New System.Windows.Forms.Label()
		Me.tbPCCUnderspeed = New System.Windows.Forms.TextBox()
		Me.gbEcoRoll = New System.Windows.Forms.GroupBox()
		Me.Label44 = New System.Windows.Forms.Label()
		Me.Label45 = New System.Windows.Forms.Label()
		Me.tbEcoRollMaxAcc = New System.Windows.Forms.TextBox()
		Me.Label29 = New System.Windows.Forms.Label()
		Me.Label30 = New System.Windows.Forms.Label()
		Me.tbEcoRollUnderspeed = New System.Windows.Forms.TextBox()
		Me.Label27 = New System.Windows.Forms.Label()
		Me.Label28 = New System.Windows.Forms.Label()
		Me.tbEcoRollActivationDelay = New System.Windows.Forms.TextBox()
		Me.Label25 = New System.Windows.Forms.Label()
		Me.Label26 = New System.Windows.Forms.Label()
		Me.tbEcoRollMinSpeed = New System.Windows.Forms.TextBox()
		Me.gbEngineStopStart = New System.Windows.Forms.GroupBox()
		Me.lblESSUtilityFactorDrivingUnit = New System.Windows.Forms.Label()
		Me.Label47 = New System.Windows.Forms.Label()
		Me.tbESSUtilityFactorDriving = New System.Windows.Forms.TextBox()
		Me.lblESSUtilityFactorDriving = New System.Windows.Forms.Label()
		Me.tbEssUtility = New System.Windows.Forms.TextBox()
		Me.Label24 = New System.Windows.Forms.Label()
		Me.Label18 = New System.Windows.Forms.Label()
		Me.tbMaxEngineOffTimespan = New System.Windows.Forms.TextBox()
		Me.Label19 = New System.Windows.Forms.Label()
		Me.Label17 = New System.Windows.Forms.Label()
		Me.tbEngineStopStartActivationDelay = New System.Windows.Forms.TextBox()
		Me.Label16 = New System.Windows.Forms.Label()
		Me.ButAuxRem = New System.Windows.Forms.Button()
		Me.ButAuxAdd = New System.Windows.Forms.Button()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.GroupBox2 = New System.Windows.Forms.GroupBox()
		Me.TbUnderSpeed = New System.Windows.Forms.TextBox()
		Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
		Me.ToolStripStatusLabelGEN = New System.Windows.Forms.ToolStripStatusLabel()
		Me.ButOK = New System.Windows.Forms.Button()
		Me.ButCancel = New System.Windows.Forms.Button()
		Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
		Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
		Me.PictureBox1 = New System.Windows.Forms.PictureBox()
		Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.PicVehicle = New System.Windows.Forms.PictureBox()
		Me.PicBox = New System.Windows.Forms.PictureBox()
		Me.TbEngTxt = New System.Windows.Forms.TextBox()
		Me.TbVehCat = New System.Windows.Forms.TextBox()
		Me.TbAxleConf = New System.Windows.Forms.TextBox()
		Me.TbHVCclass = New System.Windows.Forms.TextBox()
		Me.TbGbxTxt = New System.Windows.Forms.TextBox()
		Me.TbMass = New System.Windows.Forms.TextBox()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
		Me.lblEngineCharacteristics = New System.Windows.Forms.Label()
		Me.lblTitle = New System.Windows.Forms.Label()
		Me.pnJobInfo = New System.Windows.Forms.Panel()
		Me.TabPgGen.SuspendLayout()
		Me.pnHybridStrategy.SuspendLayout()
		Me.pnVehicle.SuspendLayout()
		Me.pnEngine.SuspendLayout()
		Me.pnGearbox.SuspendLayout()
		Me.pnShiftParams.SuspendLayout()
		Me.GrCycles.SuspendLayout()
		Me.GrAuxMech.SuspendLayout()
		Me.pnAuxDeclarationMode.SuspendLayout()
		Me.pnAuxEngineering.SuspendLayout()
		CType(Me.picAuxInfo, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.tcJob.SuspendLayout()
		Me.tpAuxiliaries.SuspendLayout()
		Me.gbBusAux.SuspendLayout()
		Me.pnBusAux.SuspendLayout()
		Me.gbElectricAux.SuspendLayout()
		Me.tpCycles.SuspendLayout()
		Me.TabPgDriver.SuspendLayout()
		Me.GrVACC.SuspendLayout()
		Me.GrLAC.SuspendLayout()
		Me.pnLookAheadCoasting.SuspendLayout()
		Me.gbOverspeed.SuspendLayout()
		Me.PnEcoRoll.SuspendLayout()
		Me.TabPgADAS.SuspendLayout()
		Me.gbPCC.SuspendLayout()
		Me.gbEcoRoll.SuspendLayout()
		Me.gbEngineStopStart.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.PicBox, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.pnJobInfo.SuspendLayout()
		Me.SuspendLayout()
		'
		'TabPgGen
		'
		Me.TabPgGen.Controls.Add(Me.pnHybridStrategy)
		Me.TabPgGen.Controls.Add(Me.pnVehicle)
		Me.TabPgGen.Controls.Add(Me.pnEngine)
		Me.TabPgGen.Controls.Add(Me.pnGearbox)
		Me.TabPgGen.Controls.Add(Me.pnShiftParams)
		Me.TabPgGen.Location = New System.Drawing.Point(4, 24)
		Me.TabPgGen.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TabPgGen.Name = "TabPgGen"
		Me.TabPgGen.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TabPgGen.Size = New System.Drawing.Size(614, 572)
		Me.TabPgGen.TabIndex = 0
		Me.TabPgGen.Text = "General"
		Me.TabPgGen.UseVisualStyleBackColor = True
		'
		'pnHybridStrategy
		'
		Me.pnHybridStrategy.Controls.Add(Me.btnOpenHybridStrategyParameters)
		Me.pnHybridStrategy.Controls.Add(Me.tbHybridStrategyParams)
		Me.pnHybridStrategy.Controls.Add(Me.btnBrowseHybridStrategyParams)
		Me.pnHybridStrategy.Location = New System.Drawing.Point(5, 144)
		Me.pnHybridStrategy.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnHybridStrategy.Name = "pnHybridStrategy"
		Me.pnHybridStrategy.Size = New System.Drawing.Size(602, 30)
		Me.pnHybridStrategy.TabIndex = 4
		'
		'btnOpenHybridStrategyParameters
		'
		Me.btnOpenHybridStrategyParameters.Location = New System.Drawing.Point(4, 3)
		Me.btnOpenHybridStrategyParameters.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnOpenHybridStrategyParameters.Name = "btnOpenHybridStrategyParameters"
		Me.btnOpenHybridStrategyParameters.Size = New System.Drawing.Size(84, 24)
		Me.btnOpenHybridStrategyParameters.TabIndex = 0
		Me.btnOpenHybridStrategyParameters.Text = "Hyb. Str. P."
		Me.btnOpenHybridStrategyParameters.UseVisualStyleBackColor = True
		'
		'tbHybridStrategyParams
		'
		Me.tbHybridStrategyParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbHybridStrategyParams.Location = New System.Drawing.Point(94, 3)
		Me.tbHybridStrategyParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbHybridStrategyParams.Name = "tbHybridStrategyParams"
		Me.tbHybridStrategyParams.Size = New System.Drawing.Size(473, 23)
		Me.tbHybridStrategyParams.TabIndex = 1
		'
		'btnBrowseHybridStrategyParams
		'
		Me.btnBrowseHybridStrategyParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnBrowseHybridStrategyParams.Image = CType(resources.GetObject("btnBrowseHybridStrategyParams.Image"), System.Drawing.Image)
		Me.btnBrowseHybridStrategyParams.Location = New System.Drawing.Point(570, 1)
		Me.btnBrowseHybridStrategyParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnBrowseHybridStrategyParams.Name = "btnBrowseHybridStrategyParams"
		Me.btnBrowseHybridStrategyParams.Size = New System.Drawing.Size(28, 28)
		Me.btnBrowseHybridStrategyParams.TabIndex = 2
		Me.btnBrowseHybridStrategyParams.UseVisualStyleBackColor = True
		'
		'pnVehicle
		'
		Me.pnVehicle.Controls.Add(Me.ButOpenVEH)
		Me.pnVehicle.Controls.Add(Me.ButtonVEH)
		Me.pnVehicle.Controls.Add(Me.TbVEH)
		Me.pnVehicle.Location = New System.Drawing.Point(5, 8)
		Me.pnVehicle.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnVehicle.Name = "pnVehicle"
		Me.pnVehicle.Size = New System.Drawing.Size(604, 31)
		Me.pnVehicle.TabIndex = 0
		'
		'ButOpenVEH
		'
		Me.ButOpenVEH.Location = New System.Drawing.Point(5, 3)
		Me.ButOpenVEH.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButOpenVEH.Name = "ButOpenVEH"
		Me.ButOpenVEH.Size = New System.Drawing.Size(84, 24)
		Me.ButOpenVEH.TabIndex = 0
		Me.ButOpenVEH.Text = "Vehicle"
		Me.ButOpenVEH.UseVisualStyleBackColor = True
		'
		'ButtonVEH
		'
		Me.ButtonVEH.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"), System.Drawing.Image)
		Me.ButtonVEH.Location = New System.Drawing.Point(574, 2)
		Me.ButtonVEH.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButtonVEH.Name = "ButtonVEH"
		Me.ButtonVEH.Size = New System.Drawing.Size(28, 28)
		Me.ButtonVEH.TabIndex = 2
		Me.ButtonVEH.UseVisualStyleBackColor = True
		'
		'TbVEH
		'
		Me.TbVEH.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbVEH.Location = New System.Drawing.Point(96, 5)
		Me.TbVEH.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbVEH.Name = "TbVEH"
		Me.TbVEH.Size = New System.Drawing.Size(471, 23)
		Me.TbVEH.TabIndex = 1
		'
		'pnEngine
		'
		Me.pnEngine.Controls.Add(Me.ButOpenENG)
		Me.pnEngine.Controls.Add(Me.ButtonMAP)
		Me.pnEngine.Controls.Add(Me.TbENG)
		Me.pnEngine.Location = New System.Drawing.Point(5, 43)
		Me.pnEngine.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnEngine.Name = "pnEngine"
		Me.pnEngine.Size = New System.Drawing.Size(606, 31)
		Me.pnEngine.TabIndex = 1
		'
		'ButOpenENG
		'
		Me.ButOpenENG.Location = New System.Drawing.Point(5, 3)
		Me.ButOpenENG.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButOpenENG.Name = "ButOpenENG"
		Me.ButOpenENG.Size = New System.Drawing.Size(84, 24)
		Me.ButOpenENG.TabIndex = 0
		Me.ButOpenENG.Text = "Engine"
		Me.ButOpenENG.UseVisualStyleBackColor = True
		'
		'ButtonMAP
		'
		Me.ButtonMAP.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonMAP.Image = CType(resources.GetObject("ButtonMAP.Image"), System.Drawing.Image)
		Me.ButtonMAP.Location = New System.Drawing.Point(574, 1)
		Me.ButtonMAP.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButtonMAP.Name = "ButtonMAP"
		Me.ButtonMAP.Size = New System.Drawing.Size(28, 28)
		Me.ButtonMAP.TabIndex = 2
		Me.ButtonMAP.UseVisualStyleBackColor = True
		'
		'TbENG
		'
		Me.TbENG.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbENG.Location = New System.Drawing.Point(96, 3)
		Me.TbENG.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbENG.Name = "TbENG"
		Me.TbENG.Size = New System.Drawing.Size(471, 23)
		Me.TbENG.TabIndex = 1
		'
		'pnGearbox
		'
		Me.pnGearbox.Controls.Add(Me.ButOpenGBX)
		Me.pnGearbox.Controls.Add(Me.ButtonGBX)
		Me.pnGearbox.Controls.Add(Me.TbGBX)
		Me.pnGearbox.Location = New System.Drawing.Point(4, 76)
		Me.pnGearbox.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnGearbox.Name = "pnGearbox"
		Me.pnGearbox.Size = New System.Drawing.Size(603, 32)
		Me.pnGearbox.TabIndex = 2
		'
		'ButOpenGBX
		'
		Me.ButOpenGBX.Location = New System.Drawing.Point(5, 3)
		Me.ButOpenGBX.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButOpenGBX.Name = "ButOpenGBX"
		Me.ButOpenGBX.Size = New System.Drawing.Size(84, 24)
		Me.ButOpenGBX.TabIndex = 0
		Me.ButOpenGBX.Text = "Gearbox"
		Me.ButOpenGBX.UseVisualStyleBackColor = True
		'
		'ButtonGBX
		'
		Me.ButtonGBX.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonGBX.Image = CType(resources.GetObject("ButtonGBX.Image"), System.Drawing.Image)
		Me.ButtonGBX.Location = New System.Drawing.Point(574, 1)
		Me.ButtonGBX.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButtonGBX.Name = "ButtonGBX"
		Me.ButtonGBX.Size = New System.Drawing.Size(28, 28)
		Me.ButtonGBX.TabIndex = 2
		Me.ButtonGBX.UseVisualStyleBackColor = True
		'
		'TbGBX
		'
		Me.TbGBX.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbGBX.Location = New System.Drawing.Point(96, 3)
		Me.TbGBX.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbGBX.Name = "TbGBX"
		Me.TbGBX.Size = New System.Drawing.Size(471, 23)
		Me.TbGBX.TabIndex = 1
		'
		'pnShiftParams
		'
		Me.pnShiftParams.Controls.Add(Me.BtnShiftParamsForm)
		Me.pnShiftParams.Controls.Add(Me.TbShiftStrategyParams)
		Me.pnShiftParams.Controls.Add(Me.BtnShiftStrategyParams)
		Me.pnShiftParams.Location = New System.Drawing.Point(5, 111)
		Me.pnShiftParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnShiftParams.Name = "pnShiftParams"
		Me.pnShiftParams.Size = New System.Drawing.Size(602, 30)
		Me.pnShiftParams.TabIndex = 3
		'
		'BtnShiftParamsForm
		'
		Me.BtnShiftParamsForm.Location = New System.Drawing.Point(4, 3)
		Me.BtnShiftParamsForm.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtnShiftParamsForm.Name = "BtnShiftParamsForm"
		Me.BtnShiftParamsForm.Size = New System.Drawing.Size(84, 24)
		Me.BtnShiftParamsForm.TabIndex = 0
		Me.BtnShiftParamsForm.Text = "Shift Parameters"
		Me.BtnShiftParamsForm.UseVisualStyleBackColor = True
		'
		'TbShiftStrategyParams
		'
		Me.TbShiftStrategyParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbShiftStrategyParams.Location = New System.Drawing.Point(94, 3)
		Me.TbShiftStrategyParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbShiftStrategyParams.Name = "TbShiftStrategyParams"
		Me.TbShiftStrategyParams.Size = New System.Drawing.Size(473, 23)
		Me.TbShiftStrategyParams.TabIndex = 1
		'
		'BtnShiftStrategyParams
		'
		Me.BtnShiftStrategyParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtnShiftStrategyParams.Image = CType(resources.GetObject("BtnShiftStrategyParams.Image"), System.Drawing.Image)
		Me.BtnShiftStrategyParams.Location = New System.Drawing.Point(570, 1)
		Me.BtnShiftStrategyParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtnShiftStrategyParams.Name = "BtnShiftStrategyParams"
		Me.BtnShiftStrategyParams.Size = New System.Drawing.Size(28, 28)
		Me.BtnShiftStrategyParams.TabIndex = 2
		Me.BtnShiftStrategyParams.UseVisualStyleBackColor = True
		'
		'GrCycles
		'
		Me.GrCycles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GrCycles.Controls.Add(Me.Label2)
		Me.GrCycles.Controls.Add(Me.LvCycles)
		Me.GrCycles.Controls.Add(Me.BtDRIrem)
		Me.GrCycles.Controls.Add(Me.BtDRIadd)
		Me.GrCycles.Location = New System.Drawing.Point(7, 7)
		Me.GrCycles.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrCycles.Name = "GrCycles"
		Me.GrCycles.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrCycles.Size = New System.Drawing.Size(601, 159)
		Me.GrCycles.TabIndex = 0
		Me.GrCycles.TabStop = False
		Me.GrCycles.Text = "Cycles"
		'
		'Label2
		'
		Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(456, 126)
		Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(137, 15)
		Me.Label2.TabIndex = 3
		Me.Label2.Text = "(Right-Click for Options)"
		'
		'LvCycles
		'
		Me.LvCycles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.LvCycles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
		Me.LvCycles.FullRowSelect = True
		Me.LvCycles.GridLines = True
		Me.LvCycles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
		Me.LvCycles.LabelEdit = True
		Me.LvCycles.Location = New System.Drawing.Point(7, 22)
		Me.LvCycles.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.LvCycles.MultiSelect = False
		Me.LvCycles.Name = "LvCycles"
		Me.LvCycles.Size = New System.Drawing.Size(586, 102)
		Me.LvCycles.TabIndex = 0
		Me.LvCycles.TabStop = False
		Me.LvCycles.UseCompatibleStateImageBehavior = False
		Me.LvCycles.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Cycle path"
		Me.ColumnHeader1.Width = 470
		'
		'BtDRIrem
		'
		Me.BtDRIrem.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtDRIrem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
		Me.BtDRIrem.Location = New System.Drawing.Point(34, 126)
		Me.BtDRIrem.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtDRIrem.Name = "BtDRIrem"
		Me.BtDRIrem.Size = New System.Drawing.Size(28, 28)
		Me.BtDRIrem.TabIndex = 2
		Me.BtDRIrem.UseVisualStyleBackColor = True
		'
		'BtDRIadd
		'
		Me.BtDRIadd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtDRIadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
		Me.BtDRIadd.Location = New System.Drawing.Point(6, 126)
		Me.BtDRIadd.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtDRIadd.Name = "BtDRIadd"
		Me.BtDRIadd.Size = New System.Drawing.Size(28, 28)
		Me.BtDRIadd.TabIndex = 1
		Me.BtDRIadd.UseVisualStyleBackColor = True
		'
		'GrAuxMech
		'
		Me.GrAuxMech.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GrAuxMech.Controls.Add(Me.pnAuxDeclarationMode)
		Me.GrAuxMech.Controls.Add(Me.pnAuxEngineering)
		Me.GrAuxMech.Location = New System.Drawing.Point(7, 18)
		Me.GrAuxMech.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrAuxMech.Name = "GrAuxMech"
		Me.GrAuxMech.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrAuxMech.Size = New System.Drawing.Size(601, 323)
		Me.GrAuxMech.TabIndex = 0
		Me.GrAuxMech.TabStop = False
		Me.GrAuxMech.Text = "Mechanical Auxiliaries"
		'
		'pnAuxDeclarationMode
		'
		Me.pnAuxDeclarationMode.Controls.Add(Me.LvAux)
		Me.pnAuxDeclarationMode.Controls.Add(Me.Label32)
		Me.pnAuxDeclarationMode.Location = New System.Drawing.Point(7, 120)
		Me.pnAuxDeclarationMode.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnAuxDeclarationMode.Name = "pnAuxDeclarationMode"
		Me.pnAuxDeclarationMode.Size = New System.Drawing.Size(587, 196)
		Me.pnAuxDeclarationMode.TabIndex = 40
		'
		'LvAux
		'
		Me.LvAux.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.LvAux.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
		Me.LvAux.FullRowSelect = True
		Me.LvAux.GridLines = True
		Me.LvAux.Location = New System.Drawing.Point(4, 3)
		Me.LvAux.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.LvAux.MultiSelect = False
		Me.LvAux.Name = "LvAux"
		Me.LvAux.Size = New System.Drawing.Size(587, 167)
		Me.LvAux.TabIndex = 0
		Me.LvAux.TabStop = False
		Me.LvAux.UseCompatibleStateImageBehavior = False
		Me.LvAux.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader4
		'
		Me.ColumnHeader4.Text = "ID"
		Me.ColumnHeader4.Width = 45
		'
		'ColumnHeader5
		'
		Me.ColumnHeader5.Text = "Type"
		Me.ColumnHeader5.Width = 108
		'
		'ColumnHeader6
		'
		Me.ColumnHeader6.Text = "Input File"
		Me.ColumnHeader6.Width = 331
		'
		'Label32
		'
		Me.Label32.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label32.AutoSize = True
		Me.Label32.Location = New System.Drawing.Point(10, 174)
		Me.Label32.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label32.Name = "Label32"
		Me.Label32.Size = New System.Drawing.Size(121, 15)
		Me.Label32.TabIndex = 3
		Me.Label32.Text = "(Double-Click to Edit)"
		'
		'pnAuxEngineering
		'
		Me.pnAuxEngineering.Controls.Add(Me.tbPAuxStandstillICEOff)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxStandstillICEOffUnit)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxStandstillICEOff)
		Me.pnAuxEngineering.Controls.Add(Me.tbPAuxDrivingICEOff)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxDrivingICEOffUnit)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxDrivingICEOff)
		Me.pnAuxEngineering.Controls.Add(Me.TbAuxPAuxICEOn)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxICEOnUnit)
		Me.pnAuxEngineering.Controls.Add(Me.lblAuxICEOn)
		Me.pnAuxEngineering.Location = New System.Drawing.Point(7, 22)
		Me.pnAuxEngineering.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnAuxEngineering.Name = "pnAuxEngineering"
		Me.pnAuxEngineering.Size = New System.Drawing.Size(583, 92)
		Me.pnAuxEngineering.TabIndex = 0
		'
		'tbPAuxStandstillICEOff
		'
		Me.tbPAuxStandstillICEOff.Location = New System.Drawing.Point(198, 63)
		Me.tbPAuxStandstillICEOff.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPAuxStandstillICEOff.Name = "tbPAuxStandstillICEOff"
		Me.tbPAuxStandstillICEOff.Size = New System.Drawing.Size(88, 23)
		Me.tbPAuxStandstillICEOff.TabIndex = 2
		'
		'lblAuxStandstillICEOffUnit
		'
		Me.lblAuxStandstillICEOffUnit.AutoSize = True
		Me.lblAuxStandstillICEOffUnit.Location = New System.Drawing.Point(294, 67)
		Me.lblAuxStandstillICEOffUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxStandstillICEOffUnit.Name = "lblAuxStandstillICEOffUnit"
		Me.lblAuxStandstillICEOffUnit.Size = New System.Drawing.Size(26, 15)
		Me.lblAuxStandstillICEOffUnit.TabIndex = 50
		Me.lblAuxStandstillICEOffUnit.Text = "[W]"
		'
		'lblAuxStandstillICEOff
		'
		Me.lblAuxStandstillICEOff.AutoSize = True
		Me.lblAuxStandstillICEOff.Location = New System.Drawing.Point(4, 67)
		Me.lblAuxStandstillICEOff.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxStandstillICEOff.Name = "lblAuxStandstillICEOff"
		Me.lblAuxStandstillICEOff.Size = New System.Drawing.Size(159, 15)
		Me.lblAuxStandstillICEOff.TabIndex = 48
		Me.lblAuxStandstillICEOff.Text = "Aux Load (Standstill, ICE Off)"
		'
		'tbPAuxDrivingICEOff
		'
		Me.tbPAuxDrivingICEOff.Location = New System.Drawing.Point(198, 33)
		Me.tbPAuxDrivingICEOff.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPAuxDrivingICEOff.Name = "tbPAuxDrivingICEOff"
		Me.tbPAuxDrivingICEOff.Size = New System.Drawing.Size(88, 23)
		Me.tbPAuxDrivingICEOff.TabIndex = 1
		'
		'lblAuxDrivingICEOffUnit
		'
		Me.lblAuxDrivingICEOffUnit.AutoSize = True
		Me.lblAuxDrivingICEOffUnit.Location = New System.Drawing.Point(294, 37)
		Me.lblAuxDrivingICEOffUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxDrivingICEOffUnit.Name = "lblAuxDrivingICEOffUnit"
		Me.lblAuxDrivingICEOffUnit.Size = New System.Drawing.Size(26, 15)
		Me.lblAuxDrivingICEOffUnit.TabIndex = 47
		Me.lblAuxDrivingICEOffUnit.Text = "[W]"
		'
		'lblAuxDrivingICEOff
		'
		Me.lblAuxDrivingICEOff.AutoSize = True
		Me.lblAuxDrivingICEOff.Location = New System.Drawing.Point(4, 37)
		Me.lblAuxDrivingICEOff.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxDrivingICEOff.Name = "lblAuxDrivingICEOff"
		Me.lblAuxDrivingICEOff.Size = New System.Drawing.Size(149, 15)
		Me.lblAuxDrivingICEOff.TabIndex = 45
		Me.lblAuxDrivingICEOff.Text = "Aux Load (Driving, ICE Off)"
		'
		'TbAuxPAuxICEOn
		'
		Me.TbAuxPAuxICEOn.Location = New System.Drawing.Point(198, 3)
		Me.TbAuxPAuxICEOn.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbAuxPAuxICEOn.Name = "TbAuxPAuxICEOn"
		Me.TbAuxPAuxICEOn.Size = New System.Drawing.Size(88, 23)
		Me.TbAuxPAuxICEOn.TabIndex = 0
		'
		'lblAuxICEOnUnit
		'
		Me.lblAuxICEOnUnit.AutoSize = True
		Me.lblAuxICEOnUnit.Location = New System.Drawing.Point(294, 7)
		Me.lblAuxICEOnUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxICEOnUnit.Name = "lblAuxICEOnUnit"
		Me.lblAuxICEOnUnit.Size = New System.Drawing.Size(26, 15)
		Me.lblAuxICEOnUnit.TabIndex = 44
		Me.lblAuxICEOnUnit.Text = "[W]"
		'
		'lblAuxICEOn
		'
		Me.lblAuxICEOn.AutoSize = True
		Me.lblAuxICEOn.Location = New System.Drawing.Point(4, 7)
		Me.lblAuxICEOn.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblAuxICEOn.Name = "lblAuxICEOn"
		Me.lblAuxICEOn.Size = New System.Drawing.Size(104, 15)
		Me.lblAuxICEOn.TabIndex = 42
		Me.lblAuxICEOn.Text = "Aux Load (ICE On)"
		'
		'GrAux
		'
		Me.GrAux.Location = New System.Drawing.Point(0, 0)
		Me.GrAux.Name = "GrAux"
		Me.GrAux.Size = New System.Drawing.Size(200, 100)
		Me.GrAux.TabIndex = 0
		Me.GrAux.TabStop = False
		'
		'Label9
		'
		Me.Label9.Location = New System.Drawing.Point(0, 0)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(100, 23)
		Me.Label9.TabIndex = 0
		'
		'TbAuxPAdd
		'
		Me.TbAuxPAdd.Location = New System.Drawing.Point(0, 0)
		Me.TbAuxPAdd.Name = "TbAuxPAdd"
		Me.TbAuxPAdd.Size = New System.Drawing.Size(100, 23)
		Me.TbAuxPAdd.TabIndex = 0
		'
		'Label8
		'
		Me.Label8.Location = New System.Drawing.Point(0, 0)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(100, 23)
		Me.Label8.TabIndex = 0
		'
		'btnAAUXOpen
		'
		Me.btnAAUXOpen.Location = New System.Drawing.Point(0, 0)
		Me.btnAAUXOpen.Name = "btnAAUXOpen"
		Me.btnAAUXOpen.Size = New System.Drawing.Size(75, 23)
		Me.btnAAUXOpen.TabIndex = 0
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(0, 0)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(100, 23)
		Me.Label1.TabIndex = 0
		'
		'btnBrowseAAUXFile
		'
		Me.btnBrowseAAUXFile.Location = New System.Drawing.Point(0, 0)
		Me.btnBrowseAAUXFile.Name = "btnBrowseAAUXFile"
		Me.btnBrowseAAUXFile.Size = New System.Drawing.Size(75, 23)
		Me.btnBrowseAAUXFile.TabIndex = 0
		'
		'txtAdvancedAuxiliaryFile
		'
		Me.txtAdvancedAuxiliaryFile.Location = New System.Drawing.Point(0, 0)
		Me.txtAdvancedAuxiliaryFile.Name = "txtAdvancedAuxiliaryFile"
		Me.txtAdvancedAuxiliaryFile.Size = New System.Drawing.Size(100, 23)
		Me.txtAdvancedAuxiliaryFile.TabIndex = 0
		'
		'picAuxInfo
		'
		Me.picAuxInfo.Location = New System.Drawing.Point(0, 0)
		Me.picAuxInfo.Name = "picAuxInfo"
		Me.picAuxInfo.Size = New System.Drawing.Size(100, 50)
		Me.picAuxInfo.TabIndex = 0
		Me.picAuxInfo.TabStop = False
		'
		'cboAdvancedAuxiliaries
		'
		Me.cboAdvancedAuxiliaries.Location = New System.Drawing.Point(0, 0)
		Me.cboAdvancedAuxiliaries.Name = "cboAdvancedAuxiliaries"
		Me.cboAdvancedAuxiliaries.Size = New System.Drawing.Size(121, 23)
		Me.cboAdvancedAuxiliaries.TabIndex = 0
		'
		'lbAdvancedAuxiliaries
		'
		Me.lbAdvancedAuxiliaries.Location = New System.Drawing.Point(0, 0)
		Me.lbAdvancedAuxiliaries.Name = "lbAdvancedAuxiliaries"
		Me.lbAdvancedAuxiliaries.Size = New System.Drawing.Size(100, 23)
		Me.lbAdvancedAuxiliaries.TabIndex = 0
		'
		'tcJob
		'
		Me.tcJob.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tcJob.Controls.Add(Me.TabPgGen)
		Me.tcJob.Controls.Add(Me.tpAuxiliaries)
		Me.tcJob.Controls.Add(Me.tpCycles)
		Me.tcJob.Controls.Add(Me.TabPgDriver)
		Me.tcJob.Controls.Add(Me.TabPgADAS)
		Me.tcJob.Location = New System.Drawing.Point(1, 90)
		Me.tcJob.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tcJob.Name = "tcJob"
		Me.tcJob.SelectedIndex = 0
		Me.tcJob.Size = New System.Drawing.Size(622, 600)
		Me.tcJob.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
		Me.tcJob.TabIndex = 0
		'
		'tpAuxiliaries
		'
		Me.tpAuxiliaries.Controls.Add(Me.gbBusAux)
		Me.tpAuxiliaries.Controls.Add(Me.gbElectricAux)
		Me.tpAuxiliaries.Controls.Add(Me.GrAuxMech)
		Me.tpAuxiliaries.Location = New System.Drawing.Point(4, 24)
		Me.tpAuxiliaries.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tpAuxiliaries.Name = "tpAuxiliaries"
		Me.tpAuxiliaries.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tpAuxiliaries.Size = New System.Drawing.Size(614, 572)
		Me.tpAuxiliaries.TabIndex = 9
		Me.tpAuxiliaries.Text = "Auxiliaries"
		Me.tpAuxiliaries.UseVisualStyleBackColor = True
		'
		'gbBusAux
		'
		Me.gbBusAux.Controls.Add(Me.cbEnableBusAux)
		Me.gbBusAux.Controls.Add(Me.pnBusAux)
		Me.gbBusAux.Location = New System.Drawing.Point(7, 348)
		Me.gbBusAux.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbBusAux.Name = "gbBusAux"
		Me.gbBusAux.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbBusAux.Size = New System.Drawing.Size(601, 90)
		Me.gbBusAux.TabIndex = 1
		Me.gbBusAux.TabStop = False
		Me.gbBusAux.Text = "Bus Auxiliaries"
		'
		'cbEnableBusAux
		'
		Me.cbEnableBusAux.AutoSize = True
		Me.cbEnableBusAux.Location = New System.Drawing.Point(12, 20)
		Me.cbEnableBusAux.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.cbEnableBusAux.Name = "cbEnableBusAux"
		Me.cbEnableBusAux.Size = New System.Drawing.Size(124, 19)
		Me.cbEnableBusAux.TabIndex = 0
		Me.cbEnableBusAux.Text = "Use Bus Auxiliaries"
		Me.cbEnableBusAux.UseVisualStyleBackColor = True
		'
		'pnBusAux
		'
		Me.pnBusAux.Controls.Add(Me.btnBusAuxP)
		Me.pnBusAux.Controls.Add(Me.tbBusAuxParams)
		Me.pnBusAux.Controls.Add(Me.btnBrowsBusAuxParams)
		Me.pnBusAux.Enabled = False
		Me.pnBusAux.Location = New System.Drawing.Point(7, 46)
		Me.pnBusAux.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnBusAux.Name = "pnBusAux"
		Me.pnBusAux.Size = New System.Drawing.Size(587, 30)
		Me.pnBusAux.TabIndex = 1
		'
		'btnBusAuxP
		'
		Me.btnBusAuxP.Location = New System.Drawing.Point(4, 3)
		Me.btnBusAuxP.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnBusAuxP.Name = "btnBusAuxP"
		Me.btnBusAuxP.Size = New System.Drawing.Size(84, 24)
		Me.btnBusAuxP.TabIndex = 0
		Me.btnBusAuxP.TabStop = False
		Me.btnBusAuxP.Text = "BusAux P."
		Me.btnBusAuxP.UseVisualStyleBackColor = True
		'
		'tbBusAuxParams
		'
		Me.tbBusAuxParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbBusAuxParams.Location = New System.Drawing.Point(94, 3)
		Me.tbBusAuxParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbBusAuxParams.Name = "tbBusAuxParams"
		Me.tbBusAuxParams.Size = New System.Drawing.Size(458, 23)
		Me.tbBusAuxParams.TabIndex = 1
		'
		'btnBrowsBusAuxParams
		'
		Me.btnBrowsBusAuxParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnBrowsBusAuxParams.Image = CType(resources.GetObject("btnBrowsBusAuxParams.Image"), System.Drawing.Image)
		Me.btnBrowsBusAuxParams.Location = New System.Drawing.Point(555, 1)
		Me.btnBrowsBusAuxParams.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnBrowsBusAuxParams.Name = "btnBrowsBusAuxParams"
		Me.btnBrowsBusAuxParams.Size = New System.Drawing.Size(28, 28)
		Me.btnBrowsBusAuxParams.TabIndex = 2
		Me.btnBrowsBusAuxParams.UseVisualStyleBackColor = True
		'
		'gbElectricAux
		'
		Me.gbElectricAux.Controls.Add(Me.Label46)
		Me.gbElectricAux.Controls.Add(Me.lblElAuxConstUnit)
		Me.gbElectricAux.Controls.Add(Me.tbElectricAuxConstant)
		Me.gbElectricAux.Controls.Add(Me.lblElAuxConst)
		Me.gbElectricAux.Location = New System.Drawing.Point(7, 445)
		Me.gbElectricAux.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbElectricAux.Name = "gbElectricAux"
		Me.gbElectricAux.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbElectricAux.Size = New System.Drawing.Size(601, 63)
		Me.gbElectricAux.TabIndex = 2
		Me.gbElectricAux.TabStop = False
		Me.gbElectricAux.Text = "Electric Auxiliaries"
		Me.gbElectricAux.Visible = False
		'
		'Label46
		'
		Me.Label46.AutoSize = True
		Me.Label46.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point)
		Me.Label46.Location = New System.Drawing.Point(274, 25)
		Me.Label46.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label46.Name = "Label46"
		Me.Label46.Size = New System.Drawing.Size(106, 13)
		Me.Label46.TabIndex = 48
		Me.Label46.Text = "(high voltage system)"
		'
		'lblElAuxConstUnit
		'
		Me.lblElAuxConstUnit.AutoSize = True
		Me.lblElAuxConstUnit.Location = New System.Drawing.Point(223, 25)
		Me.lblElAuxConstUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblElAuxConstUnit.Name = "lblElAuxConstUnit"
		Me.lblElAuxConstUnit.Size = New System.Drawing.Size(26, 15)
		Me.lblElAuxConstUnit.TabIndex = 47
		Me.lblElAuxConstUnit.Text = "[W]"
		'
		'tbElectricAuxConstant
		'
		Me.tbElectricAuxConstant.Location = New System.Drawing.Point(127, 22)
		Me.tbElectricAuxConstant.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbElectricAuxConstant.Name = "tbElectricAuxConstant"
		Me.tbElectricAuxConstant.Size = New System.Drawing.Size(88, 23)
		Me.tbElectricAuxConstant.TabIndex = 0
		'
		'lblElAuxConst
		'
		Me.lblElAuxConst.AutoSize = True
		Me.lblElAuxConst.Location = New System.Drawing.Point(8, 25)
		Me.lblElAuxConst.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblElAuxConst.Name = "lblElAuxConst"
		Me.lblElAuxConst.Size = New System.Drawing.Size(108, 15)
		Me.lblElAuxConst.TabIndex = 45
		Me.lblElAuxConst.Text = "Constant Aux Load"
		'
		'tpCycles
		'
		Me.tpCycles.Controls.Add(Me.GrCycles)
		Me.tpCycles.Location = New System.Drawing.Point(4, 24)
		Me.tpCycles.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tpCycles.Name = "tpCycles"
		Me.tpCycles.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tpCycles.Size = New System.Drawing.Size(614, 572)
		Me.tpCycles.TabIndex = 10
		Me.tpCycles.Text = "Cycles"
		Me.tpCycles.UseVisualStyleBackColor = True
		'
		'TabPgDriver
		'
		Me.TabPgDriver.Controls.Add(Me.GrVACC)
		Me.TabPgDriver.Controls.Add(Me.GrLAC)
		Me.TabPgDriver.Controls.Add(Me.gbOverspeed)
		Me.TabPgDriver.Location = New System.Drawing.Point(4, 24)
		Me.TabPgDriver.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TabPgDriver.Name = "TabPgDriver"
		Me.TabPgDriver.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TabPgDriver.Size = New System.Drawing.Size(614, 572)
		Me.TabPgDriver.TabIndex = 7
		Me.TabPgDriver.Text = "Driver Model"
		Me.TabPgDriver.UseVisualStyleBackColor = True
		'
		'GrVACC
		'
		Me.GrVACC.Controls.Add(Me.Label15)
		Me.GrVACC.Controls.Add(Me.TbDesMaxFile)
		Me.GrVACC.Controls.Add(Me.BtDesMaxBr)
		Me.GrVACC.Controls.Add(Me.BtAccOpen)
		Me.GrVACC.Location = New System.Drawing.Point(10, 313)
		Me.GrVACC.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrVACC.Name = "GrVACC"
		Me.GrVACC.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrVACC.Size = New System.Drawing.Size(601, 80)
		Me.GrVACC.TabIndex = 2
		Me.GrVACC.TabStop = False
		Me.GrVACC.Text = "Max. acceleration and brake curves"
		'
		'Label15
		'
		Me.Label15.AutoSize = True
		Me.Label15.Location = New System.Drawing.Point(4, 23)
		Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(240, 15)
		Me.Label15.TabIndex = 4
		Me.Label15.Text = "Driver Acceleration/Deceleration File (.vacc):"
		'
		'TbDesMaxFile
		'
		Me.TbDesMaxFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbDesMaxFile.Location = New System.Drawing.Point(7, 42)
		Me.TbDesMaxFile.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbDesMaxFile.Name = "TbDesMaxFile"
		Me.TbDesMaxFile.Size = New System.Drawing.Size(524, 23)
		Me.TbDesMaxFile.TabIndex = 0
		'
		'BtDesMaxBr
		'
		Me.BtDesMaxBr.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtDesMaxBr.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
		Me.BtDesMaxBr.Location = New System.Drawing.Point(533, 39)
		Me.BtDesMaxBr.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtDesMaxBr.Name = "BtDesMaxBr"
		Me.BtDesMaxBr.Size = New System.Drawing.Size(28, 28)
		Me.BtDesMaxBr.TabIndex = 1
		Me.BtDesMaxBr.UseVisualStyleBackColor = True
		'
		'BtAccOpen
		'
		Me.BtAccOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtAccOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
		Me.BtAccOpen.Location = New System.Drawing.Point(561, 39)
		Me.BtAccOpen.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.BtAccOpen.Name = "BtAccOpen"
		Me.BtAccOpen.Size = New System.Drawing.Size(28, 28)
		Me.BtAccOpen.TabIndex = 2
		Me.BtAccOpen.UseVisualStyleBackColor = True
		'
		'GrLAC
		'
		Me.GrLAC.Controls.Add(Me.pnLookAheadCoasting)
		Me.GrLAC.Controls.Add(Me.CbLookAhead)
		Me.GrLAC.Location = New System.Drawing.Point(10, 106)
		Me.GrLAC.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrLAC.Name = "GrLAC"
		Me.GrLAC.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.GrLAC.Size = New System.Drawing.Size(600, 200)
		Me.GrLAC.TabIndex = 1
		Me.GrLAC.TabStop = False
		Me.GrLAC.Text = "Look-Ahead Coasting"
		'
		'pnLookAheadCoasting
		'
		Me.pnLookAheadCoasting.Controls.Add(Me.Label49)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label7)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label6)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacMinSpeed)
		Me.pnLookAheadCoasting.Controls.Add(Me.btnDfVelocityDrop)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label12)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbDfCoastingScale)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label11)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label3)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbDfCoastingOffset)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacDfTargetSpeedFile)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label10)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label4)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label5)
		Me.pnLookAheadCoasting.Controls.Add(Me.btnDfTargetSpeed)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacPreviewFactor)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacDfVelocityDropFile)
		Me.pnLookAheadCoasting.Location = New System.Drawing.Point(18, 22)
		Me.pnLookAheadCoasting.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnLookAheadCoasting.Name = "pnLookAheadCoasting"
		Me.pnLookAheadCoasting.Size = New System.Drawing.Size(575, 149)
		Me.pnLookAheadCoasting.TabIndex = 0
		'
		'Label49
		'
		Me.Label49.AutoSize = True
		Me.Label49.Location = New System.Drawing.Point(287, 36)
		Me.Label49.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label49.Name = "Label49"
		Me.Label49.Size = New System.Drawing.Size(20, 15)
		Me.Label49.TabIndex = 34
		Me.Label49.Text = "[-]"
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(286, 7)
		Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(44, 15)
		Me.Label7.TabIndex = 4
		Me.Label7.Text = "[km/h]"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(124, 6)
		Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(75, 15)
		Me.Label6.TabIndex = 33
		Me.Label6.Text = "Min. Velocity"
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'tbLacMinSpeed
		'
		Me.tbLacMinSpeed.Location = New System.Drawing.Point(209, 3)
		Me.tbLacMinSpeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbLacMinSpeed.Name = "tbLacMinSpeed"
		Me.tbLacMinSpeed.Size = New System.Drawing.Size(74, 23)
		Me.tbLacMinSpeed.TabIndex = 0
		'
		'btnDfVelocityDrop
		'
		Me.btnDfVelocityDrop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnDfVelocityDrop.Image = CType(resources.GetObject("btnDfVelocityDrop.Image"), System.Drawing.Image)
		Me.btnDfVelocityDrop.Location = New System.Drawing.Point(544, 90)
		Me.btnDfVelocityDrop.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnDfVelocityDrop.Name = "btnDfVelocityDrop"
		Me.btnDfVelocityDrop.Size = New System.Drawing.Size(28, 28)
		Me.btnDfVelocityDrop.TabIndex = 5
		Me.btnDfVelocityDrop.UseVisualStyleBackColor = True
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(331, 127)
		Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(134, 15)
		Me.Label12.TabIndex = 31
		Me.Label12.Text = "* DF_vTarget * DF_vDrop"
		'
		'tbDfCoastingScale
		'
		Me.tbDfCoastingScale.Location = New System.Drawing.Point(281, 123)
		Me.tbDfCoastingScale.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbDfCoastingScale.Name = "tbDfCoastingScale"
		Me.tbDfCoastingScale.Size = New System.Drawing.Size(42, 23)
		Me.tbDfCoastingScale.TabIndex = 7
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(261, 126)
		Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(15, 15)
		Me.Label11.TabIndex = 29
		Me.Label11.Text = "- "
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(64, 35)
		Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(129, 15)
		Me.Label3.TabIndex = 20
		Me.Label3.Text = "Preview distance factor"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'tbDfCoastingOffset
		'
		Me.tbDfCoastingOffset.Location = New System.Drawing.Point(210, 123)
		Me.tbDfCoastingOffset.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbDfCoastingOffset.Name = "tbDfCoastingOffset"
		Me.tbDfCoastingOffset.Size = New System.Drawing.Size(42, 23)
		Me.tbDfCoastingOffset.TabIndex = 6
		'
		'tbLacDfTargetSpeedFile
		'
		Me.tbLacDfTargetSpeedFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbLacDfTargetSpeedFile.Location = New System.Drawing.Point(209, 62)
		Me.tbLacDfTargetSpeedFile.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbLacDfTargetSpeedFile.Name = "tbLacDfTargetSpeedFile"
		Me.tbLacDfTargetSpeedFile.Size = New System.Drawing.Size(333, 23)
		Me.tbLacDfTargetSpeedFile.TabIndex = 2
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(110, 126)
		Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(85, 15)
		Me.Label10.TabIndex = 27
		Me.Label10.Text = "DF_coasting = "
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(21, 65)
		Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(166, 15)
		Me.Label4.TabIndex = 24
		Me.Label4.Text = "Decision Factor - Target Speed"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(23, 96)
		Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(169, 15)
		Me.Label5.TabIndex = 26
		Me.Label5.Text = "Decision Factor - Velocity Drop"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'btnDfTargetSpeed
		'
		Me.btnDfTargetSpeed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnDfTargetSpeed.Image = CType(resources.GetObject("btnDfTargetSpeed.Image"), System.Drawing.Image)
		Me.btnDfTargetSpeed.Location = New System.Drawing.Point(544, 60)
		Me.btnDfTargetSpeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.btnDfTargetSpeed.Name = "btnDfTargetSpeed"
		Me.btnDfTargetSpeed.Size = New System.Drawing.Size(28, 28)
		Me.btnDfTargetSpeed.TabIndex = 3
		Me.btnDfTargetSpeed.UseVisualStyleBackColor = True
		'
		'tbLacPreviewFactor
		'
		Me.tbLacPreviewFactor.Location = New System.Drawing.Point(209, 32)
		Me.tbLacPreviewFactor.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbLacPreviewFactor.Name = "tbLacPreviewFactor"
		Me.tbLacPreviewFactor.Size = New System.Drawing.Size(74, 23)
		Me.tbLacPreviewFactor.TabIndex = 1
		'
		'tbLacDfVelocityDropFile
		'
		Me.tbLacDfVelocityDropFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbLacDfVelocityDropFile.Location = New System.Drawing.Point(209, 93)
		Me.tbLacDfVelocityDropFile.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbLacDfVelocityDropFile.Name = "tbLacDfVelocityDropFile"
		Me.tbLacDfVelocityDropFile.Size = New System.Drawing.Size(333, 23)
		Me.tbLacDfVelocityDropFile.TabIndex = 4
		'
		'CbLookAhead
		'
		Me.CbLookAhead.AutoSize = True
		Me.CbLookAhead.Checked = True
		Me.CbLookAhead.CheckState = System.Windows.Forms.CheckState.Checked
		Me.CbLookAhead.Location = New System.Drawing.Point(19, 24)
		Me.CbLookAhead.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.CbLookAhead.Name = "CbLookAhead"
		Me.CbLookAhead.Size = New System.Drawing.Size(68, 19)
		Me.CbLookAhead.TabIndex = 0
		Me.CbLookAhead.Text = "Enabled"
		Me.CbLookAhead.UseVisualStyleBackColor = True
		'
		'gbOverspeed
		'
		Me.gbOverspeed.Controls.Add(Me.PnEcoRoll)
		Me.gbOverspeed.Controls.Add(Me.RdOverspeed)
		Me.gbOverspeed.Controls.Add(Me.RdOff)
		Me.gbOverspeed.Location = New System.Drawing.Point(10, 10)
		Me.gbOverspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbOverspeed.Name = "gbOverspeed"
		Me.gbOverspeed.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbOverspeed.Size = New System.Drawing.Size(601, 89)
		Me.gbOverspeed.TabIndex = 0
		Me.gbOverspeed.TabStop = False
		Me.gbOverspeed.Text = "Overspeed"
		'
		'PnEcoRoll
		'
		Me.PnEcoRoll.Controls.Add(Me.Label21)
		Me.PnEcoRoll.Controls.Add(Me.Label14)
		Me.PnEcoRoll.Controls.Add(Me.TbVmin)
		Me.PnEcoRoll.Controls.Add(Me.TbOverspeed)
		Me.PnEcoRoll.Controls.Add(Me.Label23)
		Me.PnEcoRoll.Controls.Add(Me.Label13)
		Me.PnEcoRoll.Location = New System.Drawing.Point(106, 20)
		Me.PnEcoRoll.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.PnEcoRoll.Name = "PnEcoRoll"
		Me.PnEcoRoll.Size = New System.Drawing.Size(271, 62)
		Me.PnEcoRoll.TabIndex = 2
		'
		'Label21
		'
		Me.Label21.AutoSize = True
		Me.Label21.Location = New System.Drawing.Point(198, 36)
		Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label21.Name = "Label21"
		Me.Label21.Size = New System.Drawing.Size(44, 15)
		Me.Label21.TabIndex = 3
		Me.Label21.Text = "[km/h]"
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New System.Drawing.Point(198, 6)
		Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(44, 15)
		Me.Label14.TabIndex = 3
		Me.Label14.Text = "[km/h]"
		'
		'TbVmin
		'
		Me.TbVmin.Location = New System.Drawing.Point(121, 32)
		Me.TbVmin.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbVmin.Name = "TbVmin"
		Me.TbVmin.Size = New System.Drawing.Size(74, 23)
		Me.TbVmin.TabIndex = 1
		'
		'TbOverspeed
		'
		Me.TbOverspeed.Location = New System.Drawing.Point(121, 2)
		Me.TbOverspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbOverspeed.Name = "TbOverspeed"
		Me.TbOverspeed.Size = New System.Drawing.Size(74, 23)
		Me.TbOverspeed.TabIndex = 0
		'
		'Label23
		'
		Me.Label23.AutoSize = True
		Me.Label23.Location = New System.Drawing.Point(21, 36)
		Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label23.Name = "Label23"
		Me.Label23.Size = New System.Drawing.Size(94, 15)
		Me.Label23.TabIndex = 1
		Me.Label23.Text = "Minimum speed"
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(15, 6)
		Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(92, 15)
		Me.Label13.TabIndex = 1
		Me.Label13.Text = "Max. Overspeed"
		'
		'RdOverspeed
		'
		Me.RdOverspeed.AutoSize = True
		Me.RdOverspeed.Checked = True
		Me.RdOverspeed.Location = New System.Drawing.Point(19, 51)
		Me.RdOverspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.RdOverspeed.Name = "RdOverspeed"
		Me.RdOverspeed.Size = New System.Drawing.Size(81, 19)
		Me.RdOverspeed.TabIndex = 1
		Me.RdOverspeed.TabStop = True
		Me.RdOverspeed.Text = "Overspeed"
		Me.RdOverspeed.UseVisualStyleBackColor = True
		'
		'RdOff
		'
		Me.RdOff.AutoSize = True
		Me.RdOff.Location = New System.Drawing.Point(19, 24)
		Me.RdOff.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.RdOff.Name = "RdOff"
		Me.RdOff.Size = New System.Drawing.Size(42, 19)
		Me.RdOff.TabIndex = 0
		Me.RdOff.Text = "Off"
		Me.RdOff.UseVisualStyleBackColor = True
		'
		'TabPgADAS
		'
		Me.TabPgADAS.Controls.Add(Me.gbPCC)
		Me.TabPgADAS.Controls.Add(Me.gbEcoRoll)
		Me.TabPgADAS.Controls.Add(Me.gbEngineStopStart)
		Me.TabPgADAS.Location = New System.Drawing.Point(4, 24)
		Me.TabPgADAS.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TabPgADAS.Name = "TabPgADAS"
		Me.TabPgADAS.Size = New System.Drawing.Size(614, 572)
		Me.TabPgADAS.TabIndex = 8
		Me.TabPgADAS.Text = "ADAS Parameters"
		Me.TabPgADAS.UseVisualStyleBackColor = True
		'
		'gbPCC
		'
		Me.gbPCC.Controls.Add(Me.Label43)
		Me.gbPCC.Controls.Add(Me.Label42)
		Me.gbPCC.Controls.Add(Me.Label40)
		Me.gbPCC.Controls.Add(Me.Label41)
		Me.gbPCC.Controls.Add(Me.tbPCCPreviewUseCase2)
		Me.gbPCC.Controls.Add(Me.Label38)
		Me.gbPCC.Controls.Add(Me.Label39)
		Me.gbPCC.Controls.Add(Me.tbPCCPreviewUseCase1)
		Me.gbPCC.Controls.Add(Me.Label36)
		Me.gbPCC.Controls.Add(Me.Label37)
		Me.gbPCC.Controls.Add(Me.tbPCCMinSpeed)
		Me.gbPCC.Controls.Add(Me.Label34)
		Me.gbPCC.Controls.Add(Me.Label35)
		Me.gbPCC.Controls.Add(Me.tbPCCEnableSpeed)
		Me.gbPCC.Controls.Add(Me.Label31)
		Me.gbPCC.Controls.Add(Me.Label33)
		Me.gbPCC.Controls.Add(Me.tbPCCOverspeed)
		Me.gbPCC.Controls.Add(Me.Label20)
		Me.gbPCC.Controls.Add(Me.Label22)
		Me.gbPCC.Controls.Add(Me.tbPCCUnderspeed)
		Me.gbPCC.Location = New System.Drawing.Point(8, 310)
		Me.gbPCC.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbPCC.Name = "gbPCC"
		Me.gbPCC.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbPCC.Size = New System.Drawing.Size(601, 250)
		Me.gbPCC.TabIndex = 2
		Me.gbPCC.TabStop = False
		Me.gbPCC.Text = "Predictive Cruise Control"
		'
		'Label43
		'
		Me.Label43.AutoSize = True
		Me.Label43.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point)
		Me.Label43.Location = New System.Drawing.Point(46, 226)
		Me.Label43.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label43.Name = "Label43"
		Me.Label43.Size = New System.Drawing.Size(157, 13)
		Me.Label43.TabIndex = 30
		Me.Label43.Text = "(cf. column HW in driving cycle)"
		'
		'Label42
		'
		Me.Label42.AutoSize = True
		Me.Label42.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point)
		Me.Label42.Location = New System.Drawing.Point(8, 209)
		Me.Label42.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label42.Name = "Label42"
		Me.Label42.Size = New System.Drawing.Size(403, 13)
		Me.Label42.TabIndex = 29
		Me.Label42.Text = "Note: Predictive cruise control is only activated on highway parts of the driving" &
	" cycle"
		'
		'Label40
		'
		Me.Label40.AutoSize = True
		Me.Label40.Location = New System.Drawing.Point(356, 175)
		Me.Label40.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label40.Name = "Label40"
		Me.Label40.Size = New System.Drawing.Size(26, 15)
		Me.Label40.TabIndex = 28
		Me.Label40.Text = "[m]"
		'
		'Label41
		'
		Me.Label41.AutoSize = True
		Me.Label41.Location = New System.Drawing.Point(8, 175)
		Me.Label41.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label41.Name = "Label41"
		Me.Label41.Size = New System.Drawing.Size(154, 15)
		Me.Label41.TabIndex = 26
		Me.Label41.Text = "Preview distance use case 2:"
		'
		'tbPCCPreviewUseCase2
		'
		Me.tbPCCPreviewUseCase2.Location = New System.Drawing.Point(288, 172)
		Me.tbPCCPreviewUseCase2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCPreviewUseCase2.Name = "tbPCCPreviewUseCase2"
		Me.tbPCCPreviewUseCase2.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCPreviewUseCase2.TabIndex = 5
		'
		'Label38
		'
		Me.Label38.AutoSize = True
		Me.Label38.Location = New System.Drawing.Point(356, 145)
		Me.Label38.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label38.Name = "Label38"
		Me.Label38.Size = New System.Drawing.Size(26, 15)
		Me.Label38.TabIndex = 25
		Me.Label38.Text = "[m]"
		'
		'Label39
		'
		Me.Label39.AutoSize = True
		Me.Label39.Location = New System.Drawing.Point(8, 145)
		Me.Label39.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label39.Name = "Label39"
		Me.Label39.Size = New System.Drawing.Size(154, 15)
		Me.Label39.TabIndex = 23
		Me.Label39.Text = "Preview distance use case 1:"
		'
		'tbPCCPreviewUseCase1
		'
		Me.tbPCCPreviewUseCase1.Location = New System.Drawing.Point(288, 142)
		Me.tbPCCPreviewUseCase1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCPreviewUseCase1.Name = "tbPCCPreviewUseCase1"
		Me.tbPCCPreviewUseCase1.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCPreviewUseCase1.TabIndex = 4
		'
		'Label36
		'
		Me.Label36.AutoSize = True
		Me.Label36.Location = New System.Drawing.Point(356, 115)
		Me.Label36.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label36.Name = "Label36"
		Me.Label36.Size = New System.Drawing.Size(44, 15)
		Me.Label36.TabIndex = 22
		Me.Label36.Text = "[km/h]"
		'
		'Label37
		'
		Me.Label37.AutoSize = True
		Me.Label37.Location = New System.Drawing.Point(8, 115)
		Me.Label37.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label37.Name = "Label37"
		Me.Label37.Size = New System.Drawing.Size(97, 15)
		Me.Label37.TabIndex = 20
		Me.Label37.Text = "Minimum speed:"
		'
		'tbPCCMinSpeed
		'
		Me.tbPCCMinSpeed.Location = New System.Drawing.Point(288, 112)
		Me.tbPCCMinSpeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCMinSpeed.Name = "tbPCCMinSpeed"
		Me.tbPCCMinSpeed.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCMinSpeed.TabIndex = 3
		'
		'Label34
		'
		Me.Label34.AutoSize = True
		Me.Label34.Location = New System.Drawing.Point(356, 85)
		Me.Label34.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label34.Name = "Label34"
		Me.Label34.Size = New System.Drawing.Size(44, 15)
		Me.Label34.TabIndex = 19
		Me.Label34.Text = "[km/h]"
		'
		'Label35
		'
		Me.Label35.AutoSize = True
		Me.Label35.Location = New System.Drawing.Point(8, 85)
		Me.Label35.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label35.Name = "Label35"
		Me.Label35.Size = New System.Drawing.Size(126, 15)
		Me.Label35.TabIndex = 17
		Me.Label35.Text = "PCC enabling velocity:"
		'
		'tbPCCEnableSpeed
		'
		Me.tbPCCEnableSpeed.Location = New System.Drawing.Point(288, 82)
		Me.tbPCCEnableSpeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCEnableSpeed.Name = "tbPCCEnableSpeed"
		Me.tbPCCEnableSpeed.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCEnableSpeed.TabIndex = 2
		'
		'Label31
		'
		Me.Label31.AutoSize = True
		Me.Label31.Location = New System.Drawing.Point(356, 55)
		Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label31.Name = "Label31"
		Me.Label31.Size = New System.Drawing.Size(44, 15)
		Me.Label31.TabIndex = 16
		Me.Label31.Text = "[km/h]"
		'
		'Label33
		'
		Me.Label33.AutoSize = True
		Me.Label33.Location = New System.Drawing.Point(8, 55)
		Me.Label33.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label33.Name = "Label33"
		Me.Label33.Size = New System.Drawing.Size(110, 15)
		Me.Label33.TabIndex = 14
		Me.Label33.Text = "Allowed overspeed:"
		'
		'tbPCCOverspeed
		'
		Me.tbPCCOverspeed.Location = New System.Drawing.Point(288, 52)
		Me.tbPCCOverspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCOverspeed.Name = "tbPCCOverspeed"
		Me.tbPCCOverspeed.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCOverspeed.TabIndex = 1
		'
		'Label20
		'
		Me.Label20.AutoSize = True
		Me.Label20.Location = New System.Drawing.Point(356, 25)
		Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New System.Drawing.Size(44, 15)
		Me.Label20.TabIndex = 13
		Me.Label20.Text = "[km/h]"
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New System.Drawing.Point(8, 25)
		Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New System.Drawing.Size(118, 15)
		Me.Label22.TabIndex = 11
		Me.Label22.Text = "Allowed underspeed:"
		'
		'tbPCCUnderspeed
		'
		Me.tbPCCUnderspeed.Location = New System.Drawing.Point(288, 22)
		Me.tbPCCUnderspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbPCCUnderspeed.Name = "tbPCCUnderspeed"
		Me.tbPCCUnderspeed.Size = New System.Drawing.Size(60, 23)
		Me.tbPCCUnderspeed.TabIndex = 0
		'
		'gbEcoRoll
		'
		Me.gbEcoRoll.Controls.Add(Me.Label44)
		Me.gbEcoRoll.Controls.Add(Me.Label45)
		Me.gbEcoRoll.Controls.Add(Me.tbEcoRollMaxAcc)
		Me.gbEcoRoll.Controls.Add(Me.Label29)
		Me.gbEcoRoll.Controls.Add(Me.Label30)
		Me.gbEcoRoll.Controls.Add(Me.tbEcoRollUnderspeed)
		Me.gbEcoRoll.Controls.Add(Me.Label27)
		Me.gbEcoRoll.Controls.Add(Me.Label28)
		Me.gbEcoRoll.Controls.Add(Me.tbEcoRollActivationDelay)
		Me.gbEcoRoll.Controls.Add(Me.Label25)
		Me.gbEcoRoll.Controls.Add(Me.Label26)
		Me.gbEcoRoll.Controls.Add(Me.tbEcoRollMinSpeed)
		Me.gbEcoRoll.Location = New System.Drawing.Point(8, 157)
		Me.gbEcoRoll.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbEcoRoll.Name = "gbEcoRoll"
		Me.gbEcoRoll.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbEcoRoll.Size = New System.Drawing.Size(601, 147)
		Me.gbEcoRoll.TabIndex = 1
		Me.gbEcoRoll.TabStop = False
		Me.gbEcoRoll.Text = "Eco-Roll"
		'
		'Label44
		'
		Me.Label44.AutoSize = True
		Me.Label44.Location = New System.Drawing.Point(356, 87)
		Me.Label44.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label44.Name = "Label44"
		Me.Label44.Size = New System.Drawing.Size(40, 15)
		Me.Label44.TabIndex = 19
		Me.Label44.Text = "[m/s²]"
		'
		'Label45
		'
		Me.Label45.AutoSize = True
		Me.Label45.Location = New System.Drawing.Point(8, 87)
		Me.Label45.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label45.Name = "Label45"
		Me.Label45.Size = New System.Drawing.Size(141, 15)
		Me.Label45.TabIndex = 17
		Me.Label45.Text = "Upper Acceleration Limit:"
		'
		'tbEcoRollMaxAcc
		'
		Me.tbEcoRollMaxAcc.Location = New System.Drawing.Point(288, 83)
		Me.tbEcoRollMaxAcc.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEcoRollMaxAcc.Name = "tbEcoRollMaxAcc"
		Me.tbEcoRollMaxAcc.Size = New System.Drawing.Size(60, 23)
		Me.tbEcoRollMaxAcc.TabIndex = 2
		'
		'Label29
		'
		Me.Label29.AutoSize = True
		Me.Label29.Location = New System.Drawing.Point(356, 115)
		Me.Label29.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label29.Name = "Label29"
		Me.Label29.Size = New System.Drawing.Size(44, 15)
		Me.Label29.TabIndex = 16
		Me.Label29.Text = "[km/h]"
		Me.Label29.Visible = False
		'
		'Label30
		'
		Me.Label30.AutoSize = True
		Me.Label30.Location = New System.Drawing.Point(8, 115)
		Me.Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label30.Name = "Label30"
		Me.Label30.Size = New System.Drawing.Size(126, 15)
		Me.Label30.TabIndex = 14
		Me.Label30.Text = "Underspeed threshold:"
		Me.Label30.Visible = False
		'
		'tbEcoRollUnderspeed
		'
		Me.tbEcoRollUnderspeed.Location = New System.Drawing.Point(288, 112)
		Me.tbEcoRollUnderspeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEcoRollUnderspeed.Name = "tbEcoRollUnderspeed"
		Me.tbEcoRollUnderspeed.Size = New System.Drawing.Size(60, 23)
		Me.tbEcoRollUnderspeed.TabIndex = 3
		Me.tbEcoRollUnderspeed.Visible = False
		'
		'Label27
		'
		Me.Label27.AutoSize = True
		Me.Label27.Location = New System.Drawing.Point(356, 55)
		Me.Label27.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label27.Name = "Label27"
		Me.Label27.Size = New System.Drawing.Size(20, 15)
		Me.Label27.TabIndex = 13
		Me.Label27.Text = "[s]"
		'
		'Label28
		'
		Me.Label28.AutoSize = True
		Me.Label28.Location = New System.Drawing.Point(8, 55)
		Me.Label28.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label28.Name = "Label28"
		Me.Label28.Size = New System.Drawing.Size(95, 15)
		Me.Label28.TabIndex = 11
		Me.Label28.Text = "Activation delay:"
		'
		'tbEcoRollActivationDelay
		'
		Me.tbEcoRollActivationDelay.Location = New System.Drawing.Point(288, 52)
		Me.tbEcoRollActivationDelay.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEcoRollActivationDelay.Name = "tbEcoRollActivationDelay"
		Me.tbEcoRollActivationDelay.Size = New System.Drawing.Size(60, 23)
		Me.tbEcoRollActivationDelay.TabIndex = 1
		'
		'Label25
		'
		Me.Label25.AutoSize = True
		Me.Label25.Location = New System.Drawing.Point(356, 25)
		Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label25.Name = "Label25"
		Me.Label25.Size = New System.Drawing.Size(44, 15)
		Me.Label25.TabIndex = 10
		Me.Label25.Text = "[km/h]"
		'
		'Label26
		'
		Me.Label26.AutoSize = True
		Me.Label26.Location = New System.Drawing.Point(8, 25)
		Me.Label26.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label26.Name = "Label26"
		Me.Label26.Size = New System.Drawing.Size(97, 15)
		Me.Label26.TabIndex = 8
		Me.Label26.Text = "Minimum speed:"
		'
		'tbEcoRollMinSpeed
		'
		Me.tbEcoRollMinSpeed.Location = New System.Drawing.Point(288, 22)
		Me.tbEcoRollMinSpeed.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEcoRollMinSpeed.Name = "tbEcoRollMinSpeed"
		Me.tbEcoRollMinSpeed.Size = New System.Drawing.Size(60, 23)
		Me.tbEcoRollMinSpeed.TabIndex = 0
		'
		'gbEngineStopStart
		'
		Me.gbEngineStopStart.Controls.Add(Me.lblESSUtilityFactorDrivingUnit)
		Me.gbEngineStopStart.Controls.Add(Me.Label47)
		Me.gbEngineStopStart.Controls.Add(Me.tbESSUtilityFactorDriving)
		Me.gbEngineStopStart.Controls.Add(Me.lblESSUtilityFactorDriving)
		Me.gbEngineStopStart.Controls.Add(Me.tbEssUtility)
		Me.gbEngineStopStart.Controls.Add(Me.Label24)
		Me.gbEngineStopStart.Controls.Add(Me.Label18)
		Me.gbEngineStopStart.Controls.Add(Me.tbMaxEngineOffTimespan)
		Me.gbEngineStopStart.Controls.Add(Me.Label19)
		Me.gbEngineStopStart.Controls.Add(Me.Label17)
		Me.gbEngineStopStart.Controls.Add(Me.tbEngineStopStartActivationDelay)
		Me.gbEngineStopStart.Controls.Add(Me.Label16)
		Me.gbEngineStopStart.Location = New System.Drawing.Point(8, 10)
		Me.gbEngineStopStart.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbEngineStopStart.Name = "gbEngineStopStart"
		Me.gbEngineStopStart.Padding = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.gbEngineStopStart.Size = New System.Drawing.Size(601, 140)
		Me.gbEngineStopStart.TabIndex = 0
		Me.gbEngineStopStart.TabStop = False
		Me.gbEngineStopStart.Text = "Engine Stop/Start"
		'
		'lblESSUtilityFactorDrivingUnit
		'
		Me.lblESSUtilityFactorDrivingUnit.AutoSize = True
		Me.lblESSUtilityFactorDrivingUnit.Location = New System.Drawing.Point(356, 113)
		Me.lblESSUtilityFactorDrivingUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblESSUtilityFactorDrivingUnit.Name = "lblESSUtilityFactorDrivingUnit"
		Me.lblESSUtilityFactorDrivingUnit.Size = New System.Drawing.Size(20, 15)
		Me.lblESSUtilityFactorDrivingUnit.TabIndex = 10
		Me.lblESSUtilityFactorDrivingUnit.Text = "[-]"
		'
		'Label47
		'
		Me.Label47.AutoSize = True
		Me.Label47.Location = New System.Drawing.Point(356, 83)
		Me.Label47.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label47.Name = "Label47"
		Me.Label47.Size = New System.Drawing.Size(20, 15)
		Me.Label47.TabIndex = 9
		Me.Label47.Text = "[-]"
		'
		'tbESSUtilityFactorDriving
		'
		Me.tbESSUtilityFactorDriving.Location = New System.Drawing.Point(288, 110)
		Me.tbESSUtilityFactorDriving.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbESSUtilityFactorDriving.Name = "tbESSUtilityFactorDriving"
		Me.tbESSUtilityFactorDriving.Size = New System.Drawing.Size(60, 23)
		Me.tbESSUtilityFactorDriving.TabIndex = 3
		'
		'lblESSUtilityFactorDriving
		'
		Me.lblESSUtilityFactorDriving.AutoSize = True
		Me.lblESSUtilityFactorDriving.Location = New System.Drawing.Point(8, 113)
		Me.lblESSUtilityFactorDriving.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblESSUtilityFactorDriving.Name = "lblESSUtilityFactorDriving"
		Me.lblESSUtilityFactorDriving.Size = New System.Drawing.Size(246, 15)
		Me.lblESSUtilityFactorDriving.TabIndex = 8
		Me.lblESSUtilityFactorDriving.Text = "Engine stop/start utility factor (while driving):"
		'
		'tbEssUtility
		'
		Me.tbEssUtility.Location = New System.Drawing.Point(288, 80)
		Me.tbEssUtility.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEssUtility.Name = "tbEssUtility"
		Me.tbEssUtility.Size = New System.Drawing.Size(60, 23)
		Me.tbEssUtility.TabIndex = 2
		'
		'Label24
		'
		Me.Label24.AutoSize = True
		Me.Label24.Location = New System.Drawing.Point(8, 83)
		Me.Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label24.Name = "Label24"
		Me.Label24.Size = New System.Drawing.Size(241, 15)
		Me.Label24.TabIndex = 6
		Me.Label24.Text = "Engine stop/start utility factor (vehicle stop):"
		'
		'Label18
		'
		Me.Label18.AutoSize = True
		Me.Label18.Location = New System.Drawing.Point(356, 53)
		Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label18.Name = "Label18"
		Me.Label18.Size = New System.Drawing.Size(20, 15)
		Me.Label18.TabIndex = 5
		Me.Label18.Text = "[s]"
		'
		'tbMaxEngineOffTimespan
		'
		Me.tbMaxEngineOffTimespan.Location = New System.Drawing.Point(288, 50)
		Me.tbMaxEngineOffTimespan.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbMaxEngineOffTimespan.Name = "tbMaxEngineOffTimespan"
		Me.tbMaxEngineOffTimespan.Size = New System.Drawing.Size(60, 23)
		Me.tbMaxEngineOffTimespan.TabIndex = 1
		'
		'Label19
		'
		Me.Label19.AutoSize = True
		Me.Label19.Location = New System.Drawing.Point(8, 53)
		Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label19.Name = "Label19"
		Me.Label19.Size = New System.Drawing.Size(122, 15)
		Me.Label19.TabIndex = 3
		Me.Label19.Text = "Max. engine-off time:"
		'
		'Label17
		'
		Me.Label17.AutoSize = True
		Me.Label17.Location = New System.Drawing.Point(356, 23)
		Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New System.Drawing.Size(20, 15)
		Me.Label17.TabIndex = 2
		Me.Label17.Text = "[s]"
		'
		'tbEngineStopStartActivationDelay
		'
		Me.tbEngineStopStartActivationDelay.Location = New System.Drawing.Point(288, 20)
		Me.tbEngineStopStartActivationDelay.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.tbEngineStopStartActivationDelay.Name = "tbEngineStopStartActivationDelay"
		Me.tbEngineStopStartActivationDelay.Size = New System.Drawing.Size(60, 23)
		Me.tbEngineStopStartActivationDelay.TabIndex = 0
		'
		'Label16
		'
		Me.Label16.AutoSize = True
		Me.Label16.Location = New System.Drawing.Point(8, 23)
		Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New System.Drawing.Size(98, 15)
		Me.Label16.TabIndex = 0
		Me.Label16.Text = "Delay engine-off:"
		'
		'ButAuxRem
		'
		Me.ButAuxRem.Location = New System.Drawing.Point(0, 0)
		Me.ButAuxRem.Name = "ButAuxRem"
		Me.ButAuxRem.Size = New System.Drawing.Size(75, 23)
		Me.ButAuxRem.TabIndex = 0
		'
		'ButAuxAdd
		'
		Me.ButAuxAdd.Location = New System.Drawing.Point(0, 0)
		Me.ButAuxAdd.Name = "ButAuxAdd"
		Me.ButAuxAdd.Size = New System.Drawing.Size(75, 23)
		Me.ButAuxAdd.TabIndex = 0
		'
		'GroupBox1
		'
		Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(200, 100)
		Me.GroupBox1.TabIndex = 0
		Me.GroupBox1.TabStop = False
		'
		'GroupBox2
		'
		Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(200, 100)
		Me.GroupBox2.TabIndex = 0
		Me.GroupBox2.TabStop = False
		'
		'TbUnderSpeed
		'
		Me.TbUnderSpeed.Location = New System.Drawing.Point(0, 0)
		Me.TbUnderSpeed.Name = "TbUnderSpeed"
		Me.TbUnderSpeed.Size = New System.Drawing.Size(100, 23)
		Me.TbUnderSpeed.TabIndex = 0
		'
		'StatusStrip1
		'
		Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
		Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelGEN})
		Me.StatusStrip1.Location = New System.Drawing.Point(0, 706)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 16, 0)
		Me.StatusStrip1.Size = New System.Drawing.Size(1101, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 6
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'ToolStripStatusLabelGEN
		'
		Me.ToolStripStatusLabelGEN.Name = "ToolStripStatusLabelGEN"
		Me.ToolStripStatusLabelGEN.Size = New System.Drawing.Size(119, 17)
		Me.ToolStripStatusLabelGEN.Text = "ToolStripStatusLabel1"
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButOK.Location = New System.Drawing.Point(905, 650)
		Me.ButOK.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New System.Drawing.Size(88, 27)
		Me.ButOK.TabIndex = 1
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.ButCancel.Location = New System.Drawing.Point(1000, 650)
		Me.ButCancel.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New System.Drawing.Size(88, 27)
		Me.ButCancel.TabIndex = 2
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
		Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
		Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator1, Me.ToolStripBtSendTo, Me.ToolStripSeparator2, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
		Me.ToolStrip1.Size = New System.Drawing.Size(1101, 31)
		Me.ToolStrip1.TabIndex = 20
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripBtNew.Text = "New"
		Me.ToolStripBtNew.ToolTipText = "New"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripBtOpen.Text = "Open"
		Me.ToolStripBtOpen.ToolTipText = "Open..."
		'
		'ToolStripBtSave
		'
		Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
		Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSave.Name = "ToolStripBtSave"
		Me.ToolStripBtSave.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripBtSave.Text = "Save"
		Me.ToolStripBtSave.ToolTipText = "Save"
		'
		'ToolStripBtSaveAs
		'
		Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
		Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
		Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripBtSaveAs.Text = "Save As"
		Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 31)
		'
		'ToolStripBtSendTo
		'
		Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
		Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
		Me.ToolStripBtSendTo.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripBtSendTo.Text = "Send to Job List"
		Me.ToolStripBtSendTo.ToolTipText = "Send to Job List"
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 31)
		'
		'ToolStripButton1
		'
		Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
		Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripButton1.Name = "ToolStripButton1"
		Me.ToolStripButton1.Size = New System.Drawing.Size(28, 28)
		Me.ToolStripButton1.Text = "Help"
		'
		'PictureBox1
		'
		Me.PictureBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.PictureBox1.BackColor = System.Drawing.Color.White
		Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_Mainform
		Me.PictureBox1.Location = New System.Drawing.Point(0, 32)
		Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New System.Drawing.Size(1101, 46)
		Me.PictureBox1.TabIndex = 21
		Me.PictureBox1.TabStop = False
		'
		'CmOpenFile
		'
		Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
		Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
		Me.CmOpenFile.Name = "CmOpenFile"
		Me.CmOpenFile.ShowImageMargin = False
		Me.CmOpenFile.Size = New System.Drawing.Size(128, 48)
		'
		'OpenWithToolStripMenuItem
		'
		Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
		Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
		Me.OpenWithToolStripMenuItem.Text = "Open with ..."
		'
		'ShowInFolderToolStripMenuItem
		'
		Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
		Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
		Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
		'
		'PicVehicle
		'
		Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
		Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.PicVehicle.Location = New System.Drawing.Point(5, 3)
		Me.PicVehicle.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.PicVehicle.Name = "PicVehicle"
		Me.PicVehicle.Size = New System.Drawing.Size(350, 101)
		Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.PicVehicle.TabIndex = 36
		Me.PicVehicle.TabStop = False
		'
		'PicBox
		'
		Me.PicBox.BackColor = System.Drawing.Color.LightGray
		Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.PicBox.Location = New System.Drawing.Point(5, 166)
		Me.PicBox.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.PicBox.Name = "PicBox"
		Me.PicBox.Size = New System.Drawing.Size(455, 341)
		Me.PicBox.TabIndex = 36
		Me.PicBox.TabStop = False
		'
		'TbEngTxt
		'
		Me.TbEngTxt.Location = New System.Drawing.Point(5, 107)
		Me.TbEngTxt.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbEngTxt.Name = "TbEngTxt"
		Me.TbEngTxt.ReadOnly = True
		Me.TbEngTxt.Size = New System.Drawing.Size(454, 23)
		Me.TbEngTxt.TabIndex = 6
		'
		'TbVehCat
		'
		Me.TbVehCat.Location = New System.Drawing.Point(358, 3)
		Me.TbVehCat.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbVehCat.Name = "TbVehCat"
		Me.TbVehCat.ReadOnly = True
		Me.TbVehCat.Size = New System.Drawing.Size(101, 23)
		Me.TbVehCat.TabIndex = 2
		'
		'TbAxleConf
		'
		Me.TbAxleConf.Location = New System.Drawing.Point(424, 33)
		Me.TbAxleConf.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbAxleConf.Name = "TbAxleConf"
		Me.TbAxleConf.ReadOnly = True
		Me.TbAxleConf.Size = New System.Drawing.Size(35, 23)
		Me.TbAxleConf.TabIndex = 4
		'
		'TbHVCclass
		'
		Me.TbHVCclass.Location = New System.Drawing.Point(358, 76)
		Me.TbHVCclass.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbHVCclass.Name = "TbHVCclass"
		Me.TbHVCclass.ReadOnly = True
		Me.TbHVCclass.Size = New System.Drawing.Size(101, 23)
		Me.TbHVCclass.TabIndex = 5
		'
		'TbGbxTxt
		'
		Me.TbGbxTxt.Location = New System.Drawing.Point(5, 136)
		Me.TbGbxTxt.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbGbxTxt.Name = "TbGbxTxt"
		Me.TbGbxTxt.ReadOnly = True
		Me.TbGbxTxt.Size = New System.Drawing.Size(454, 23)
		Me.TbGbxTxt.TabIndex = 7
		'
		'TbMass
		'
		Me.TbMass.Location = New System.Drawing.Point(358, 33)
		Me.TbMass.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.TbMass.Name = "TbMass"
		Me.TbMass.ReadOnly = True
		Me.TbMass.Size = New System.Drawing.Size(58, 23)
		Me.TbMass.TabIndex = 3
		'
		'lblEngineCharacteristics
		'
		Me.lblEngineCharacteristics.AutoSize = True
		Me.lblEngineCharacteristics.Location = New System.Drawing.Point(8, 513)
		Me.lblEngineCharacteristics.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblEngineCharacteristics.Name = "lblEngineCharacteristics"
		Me.lblEngineCharacteristics.Size = New System.Drawing.Size(0, 15)
		Me.lblEngineCharacteristics.TabIndex = 37
		'
		'lblTitle
		'
		Me.lblTitle.AutoSize = True
		Me.lblTitle.BackColor = System.Drawing.Color.White
		Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
		Me.lblTitle.Location = New System.Drawing.Point(133, 39)
		Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.lblTitle.Name = "lblTitle"
		Me.lblTitle.Size = New System.Drawing.Size(320, 29)
		Me.lblTitle.TabIndex = 38
		Me.lblTitle.Text = "Job Editor - VEHICLE_TYPE"
		'
		'pnJobInfo
		'
		Me.pnJobInfo.Controls.Add(Me.PicVehicle)
		Me.pnJobInfo.Controls.Add(Me.TbVehCat)
		Me.pnJobInfo.Controls.Add(Me.TbHVCclass)
		Me.pnJobInfo.Controls.Add(Me.TbAxleConf)
		Me.pnJobInfo.Controls.Add(Me.TbMass)
		Me.pnJobInfo.Controls.Add(Me.PicBox)
		Me.pnJobInfo.Controls.Add(Me.TbGbxTxt)
		Me.pnJobInfo.Controls.Add(Me.TbEngTxt)
		Me.pnJobInfo.Location = New System.Drawing.Point(621, 90)
		Me.pnJobInfo.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.pnJobInfo.Name = "pnJobInfo"
		Me.pnJobInfo.Size = New System.Drawing.Size(463, 543)
		Me.pnJobInfo.TabIndex = 39
		'
		'VectoJobForm
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New System.Drawing.Size(1101, 728)
		Me.Controls.Add(Me.pnJobInfo)
		Me.Controls.Add(Me.lblTitle)
		Me.Controls.Add(Me.tcJob)
		Me.Controls.Add(Me.lblEngineCharacteristics)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.ButCancel)
		Me.Controls.Add(Me.ButOK)
		Me.Controls.Add(Me.StatusStrip1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
		Me.MaximizeBox = False
		Me.Name = "VectoJobForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Job Editor"
		Me.TabPgGen.ResumeLayout(False)
		Me.pnHybridStrategy.ResumeLayout(False)
		Me.pnHybridStrategy.PerformLayout()
		Me.pnVehicle.ResumeLayout(False)
		Me.pnVehicle.PerformLayout()
		Me.pnEngine.ResumeLayout(False)
		Me.pnEngine.PerformLayout()
		Me.pnGearbox.ResumeLayout(False)
		Me.pnGearbox.PerformLayout()
		Me.pnShiftParams.ResumeLayout(False)
		Me.pnShiftParams.PerformLayout()
		Me.GrCycles.ResumeLayout(False)
		Me.GrCycles.PerformLayout()
		Me.GrAuxMech.ResumeLayout(False)
		Me.pnAuxDeclarationMode.ResumeLayout(False)
		Me.pnAuxDeclarationMode.PerformLayout()
		Me.pnAuxEngineering.ResumeLayout(False)
		Me.pnAuxEngineering.PerformLayout()
		CType(Me.picAuxInfo, System.ComponentModel.ISupportInitialize).EndInit()
		Me.tcJob.ResumeLayout(False)
		Me.tpAuxiliaries.ResumeLayout(False)
		Me.gbBusAux.ResumeLayout(False)
		Me.gbBusAux.PerformLayout()
		Me.pnBusAux.ResumeLayout(False)
		Me.pnBusAux.PerformLayout()
		Me.gbElectricAux.ResumeLayout(False)
		Me.gbElectricAux.PerformLayout()
		Me.tpCycles.ResumeLayout(False)
		Me.TabPgDriver.ResumeLayout(False)
		Me.GrVACC.ResumeLayout(False)
		Me.GrVACC.PerformLayout()
		Me.GrLAC.ResumeLayout(False)
		Me.GrLAC.PerformLayout()
		Me.pnLookAheadCoasting.ResumeLayout(False)
		Me.pnLookAheadCoasting.PerformLayout()
		Me.gbOverspeed.ResumeLayout(False)
		Me.gbOverspeed.PerformLayout()
		Me.PnEcoRoll.ResumeLayout(False)
		Me.PnEcoRoll.PerformLayout()
		Me.TabPgADAS.ResumeLayout(False)
		Me.gbPCC.ResumeLayout(False)
		Me.gbPCC.PerformLayout()
		Me.gbEcoRoll.ResumeLayout(False)
		Me.gbEcoRoll.PerformLayout()
		Me.gbEngineStopStart.ResumeLayout(False)
		Me.gbEngineStopStart.PerformLayout()
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.PicBox, System.ComponentModel.ISupportInitialize).EndInit()
		Me.pnJobInfo.ResumeLayout(False)
		Me.pnJobInfo.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents TabPgGen As TabPage
    Friend WithEvents tcJob As TabControl
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ButtonVEH As Button
    Friend WithEvents ButtonMAP As Button
    Friend WithEvents ButtonGBX As Button
    Friend WithEvents ButOpenVEH As Button
    Friend WithEvents ButOpenGBX As Button
    Friend WithEvents ButOpenENG As Button
    Friend WithEvents ToolStripStatusLabelGEN As ToolStripStatusLabel
    Friend WithEvents ButOK As Button
    Friend WithEvents TbGBX As TextBox
    Friend WithEvents TbENG As TextBox
    Friend WithEvents TbVEH As TextBox
    Friend WithEvents ButCancel As Button
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripBtNew As ToolStripButton
    Friend WithEvents ToolStripBtOpen As ToolStripButton
    Friend WithEvents ToolStripBtSave As ToolStripButton
    Friend WithEvents ToolStripBtSaveAs As ToolStripButton
    Friend WithEvents ToolStripBtSendTo As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents GrAuxMech As GroupBox
    Friend WithEvents LvAux As ListView
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents TabPgDriver As TabPage
    Friend WithEvents BtDesMaxBr As Button
    Friend WithEvents TbDesMaxFile As TextBox
    Friend WithEvents GrCycles As GroupBox
    Friend WithEvents LvCycles As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents BtDRIrem As Button
    Friend WithEvents BtDRIadd As Button
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    'Friend WithEvents CbEngOnly As CheckBox
    Friend WithEvents BtAccOpen As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GrLAC As GroupBox
    Friend WithEvents CbLookAhead As CheckBox
    Friend WithEvents gbOverspeed As GroupBox
    Friend WithEvents Label21 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents TbVmin As TextBox
    Friend WithEvents TbOverspeed As TextBox
    Friend WithEvents Label23 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents RdOverspeed As RadioButton
    Friend WithEvents RdOff As RadioButton
    Friend WithEvents Label32 As Label
    Friend WithEvents PnEcoRoll As Panel
    Friend WithEvents PicVehicle As PictureBox
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents TbEngTxt As TextBox
    Friend WithEvents TbVehCat As TextBox
    Friend WithEvents TbAxleConf As TextBox
    Friend WithEvents TbHVCclass As TextBox
    Friend WithEvents TbGbxTxt As TextBox
    Friend WithEvents TbMass As TextBox
    Friend WithEvents GrVACC As GroupBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents lblAuxICEOnUnit As System.Windows.Forms.Label
    Friend WithEvents TbAuxPAuxICEOn As System.Windows.Forms.TextBox
    Friend WithEvents lblAuxICEOn As System.Windows.Forms.Label
    Friend WithEvents TabPgADAS As TabPage
    Friend WithEvents gbEcoRoll As GroupBox
    Friend WithEvents Label29 As Label
    Friend WithEvents Label30 As Label
    Friend WithEvents tbEcoRollUnderspeed As TextBox
    Friend WithEvents Label27 As Label
    Friend WithEvents Label28 As Label
    Friend WithEvents tbEcoRollActivationDelay As TextBox
    Friend WithEvents Label25 As Label
    Friend WithEvents Label26 As Label
    Friend WithEvents tbEcoRollMinSpeed As TextBox
    Friend WithEvents gbEngineStopStart As GroupBox
    Friend WithEvents tbEssUtility As TextBox
    Friend WithEvents Label24 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents tbMaxEngineOffTimespan As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents tbEngineStopStartActivationDelay As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents gbPCC As GroupBox
    Friend WithEvents Label36 As Label
    Friend WithEvents Label37 As Label
    Friend WithEvents tbPCCMinSpeed As TextBox
    Friend WithEvents Label34 As Label
    Friend WithEvents Label35 As Label
    Friend WithEvents tbPCCEnableSpeed As TextBox
    Friend WithEvents Label31 As Label
    Friend WithEvents Label33 As Label
    Friend WithEvents tbPCCOverspeed As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents tbPCCUnderspeed As TextBox
    Friend WithEvents Label43 As Label
    Friend WithEvents Label42 As Label
    Friend WithEvents Label40 As Label
    Friend WithEvents Label41 As Label
    Friend WithEvents tbPCCPreviewUseCase2 As TextBox
    Friend WithEvents Label38 As Label
    Friend WithEvents Label39 As Label
    Friend WithEvents tbPCCPreviewUseCase1 As TextBox
    Friend WithEvents Label44 As Label
    Friend WithEvents Label45 As Label
    Friend WithEvents tbEcoRollMaxAcc As TextBox
    Friend WithEvents TbShiftStrategyParams As TextBox
    Friend WithEvents BtnShiftParamsForm As Button
    Friend WithEvents BtnShiftStrategyParams As Button
    Friend WithEvents tpAuxiliaries As TabPage
    Friend WithEvents tpCycles As TabPage
    Friend WithEvents gbElectricAux As GroupBox
    Friend WithEvents lblElAuxConstUnit As Label
    Friend WithEvents tbElectricAuxConstant As TextBox
    Friend WithEvents lblElAuxConst As Label
    Friend WithEvents lblTitle As Label
    Friend WithEvents pnJobInfo As Panel
    Friend WithEvents pnVehicle As Panel
    Friend WithEvents pnEngine As Panel
    Friend WithEvents pnGearbox As Panel
    Friend WithEvents pnShiftParams As Panel
    Friend WithEvents pnHybridStrategy As Panel
    Friend WithEvents btnOpenHybridStrategyParameters As Button
    Friend WithEvents tbHybridStrategyParams As TextBox
    Friend WithEvents btnBrowseHybridStrategyParams As Button
    Friend WithEvents Label46 As Label
    Friend WithEvents gbBusAux As GroupBox
    Friend WithEvents cbEnableBusAux As CheckBox
    Friend WithEvents pnBusAux As Panel
    Friend WithEvents btnBusAuxP As Button
    Friend WithEvents tbBusAuxParams As TextBox
    Friend WithEvents btnBrowsBusAuxParams As Button
    Friend WithEvents pnAuxEngineering As Panel
    Friend WithEvents tbPAuxStandstillICEOff As TextBox
    Friend WithEvents lblAuxStandstillICEOffUnit As Label
    Friend WithEvents lblAuxStandstillICEOff As Label
    Friend WithEvents tbPAuxDrivingICEOff As TextBox
    Friend WithEvents lblAuxDrivingICEOffUnit As Label
    Friend WithEvents lblAuxDrivingICEOff As Label
    Friend WithEvents pnAuxDeclarationMode As Panel
    Friend WithEvents tbESSUtilityFactorDriving As TextBox
    Friend WithEvents lblESSUtilityFactorDriving As Label
	Friend WithEvents GrAux As GroupBox
	Friend WithEvents ButAuxRem As Button
	Friend WithEvents ButAuxAdd As Button
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents TbUnderSpeed As TextBox
	Friend WithEvents cboAdvancedAuxiliaries As ComboBox
	Friend WithEvents picAuxInfo As PictureBox
	Friend WithEvents Label1 As Label
	Friend WithEvents btnBrowseAAUXFile As Button
	Friend WithEvents txtAdvancedAuxiliaryFile As TextBox
	Friend WithEvents lbAdvancedAuxiliaries As Label
	Friend WithEvents btnAAUXOpen As Button
	Friend WithEvents pnLookAheadCoasting As System.Windows.Forms.Panel
	Friend WithEvents btnDfVelocityDrop As System.Windows.Forms.Button
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents tbDfCoastingScale As System.Windows.Forms.TextBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents tbDfCoastingOffset As System.Windows.Forms.TextBox
	Friend WithEvents tbLacDfTargetSpeedFile As System.Windows.Forms.TextBox
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents btnDfTargetSpeed As System.Windows.Forms.Button
	Friend WithEvents tbLacPreviewFactor As System.Windows.Forms.TextBox
	Friend WithEvents tbLacDfVelocityDropFile As System.Windows.Forms.TextBox
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents tbLacMinSpeed As System.Windows.Forms.TextBox
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents TbAuxPAdd As System.Windows.Forms.TextBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents lblEngineCharacteristics As System.Windows.Forms.Label
	Friend WithEvents lblESSUtilityFactorDrivingUnit As Label
	Friend WithEvents Label47 As Label
    Friend WithEvents Label49 As Label
End Class
