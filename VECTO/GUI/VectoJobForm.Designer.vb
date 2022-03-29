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
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.BtDRIrem = New System.Windows.Forms.Button()
        Me.BtDRIadd = New System.Windows.Forms.Button()
        Me.GrAuxMech = New System.Windows.Forms.GroupBox()
        Me.pnAuxDeclarationMode = New System.Windows.Forms.Panel()
        Me.LvAux = New System.Windows.Forms.ListView()
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
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
        Me.gbShiftStrategy = New System.Windows.Forms.GroupBox()
        Me.cbGearshiftStrategy = New System.Windows.Forms.ComboBox()
        Me.GrVACC = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.TbDesMaxFile = New System.Windows.Forms.TextBox()
        Me.BtDesMaxBr = New System.Windows.Forms.Button()
        Me.BtAccOpen = New System.Windows.Forms.Button()
        Me.GrLAC = New System.Windows.Forms.GroupBox()
        Me.pnLookAheadCoasting = New System.Windows.Forms.Panel()
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
        Me.TabPgGen.SuspendLayout
        Me.pnHybridStrategy.SuspendLayout
        Me.pnVehicle.SuspendLayout
        Me.pnEngine.SuspendLayout
        Me.pnGearbox.SuspendLayout
        Me.pnShiftParams.SuspendLayout
        Me.GrCycles.SuspendLayout
        Me.GrAuxMech.SuspendLayout
        Me.pnAuxDeclarationMode.SuspendLayout
        Me.pnAuxEngineering.SuspendLayout
        CType(Me.picAuxInfo,System.ComponentModel.ISupportInitialize).BeginInit
        Me.tcJob.SuspendLayout
        Me.tpAuxiliaries.SuspendLayout
        Me.gbBusAux.SuspendLayout
        Me.pnBusAux.SuspendLayout
        Me.gbElectricAux.SuspendLayout
        Me.tpCycles.SuspendLayout
        Me.TabPgDriver.SuspendLayout
        Me.gbShiftStrategy.SuspendLayout
        Me.GrVACC.SuspendLayout
        Me.GrLAC.SuspendLayout
        Me.pnLookAheadCoasting.SuspendLayout
        Me.gbOverspeed.SuspendLayout
        Me.PnEcoRoll.SuspendLayout
        Me.TabPgADAS.SuspendLayout
        Me.gbPCC.SuspendLayout
        Me.gbEcoRoll.SuspendLayout
        Me.gbEngineStopStart.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.ToolStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.pnJobInfo.SuspendLayout
        Me.SuspendLayout
        '
        'TabPgGen
        '
        Me.TabPgGen.Controls.Add(Me.pnHybridStrategy)
        Me.TabPgGen.Controls.Add(Me.pnVehicle)
        Me.TabPgGen.Controls.Add(Me.pnEngine)
        Me.TabPgGen.Controls.Add(Me.pnGearbox)
        Me.TabPgGen.Controls.Add(Me.pnShiftParams)
        Me.TabPgGen.Location = New System.Drawing.Point(4, 22)
        Me.TabPgGen.Name = "TabPgGen"
        Me.TabPgGen.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPgGen.Size = New System.Drawing.Size(525, 494)
        Me.TabPgGen.TabIndex = 0
        Me.TabPgGen.Text = "General"
        Me.TabPgGen.UseVisualStyleBackColor = true
        '
        'pnHybridStrategy
        '
        Me.pnHybridStrategy.Controls.Add(Me.btnOpenHybridStrategyParameters)
        Me.pnHybridStrategy.Controls.Add(Me.tbHybridStrategyParams)
        Me.pnHybridStrategy.Controls.Add(Me.btnBrowseHybridStrategyParams)
        Me.pnHybridStrategy.Location = New System.Drawing.Point(4, 125)
        Me.pnHybridStrategy.Name = "pnHybridStrategy"
        Me.pnHybridStrategy.Size = New System.Drawing.Size(516, 26)
        Me.pnHybridStrategy.TabIndex = 15
        Me.pnHybridStrategy.Visible = false
        '
        'btnOpenHybridStrategyParameters
        '
        Me.btnOpenHybridStrategyParameters.Location = New System.Drawing.Point(3, 3)
        Me.btnOpenHybridStrategyParameters.Name = "btnOpenHybridStrategyParameters"
        Me.btnOpenHybridStrategyParameters.Size = New System.Drawing.Size(72, 21)
        Me.btnOpenHybridStrategyParameters.TabIndex = 11
        Me.btnOpenHybridStrategyParameters.TabStop = false
        Me.btnOpenHybridStrategyParameters.Text = "Hyb. Str. P."
        Me.btnOpenHybridStrategyParameters.UseVisualStyleBackColor = true
        '
        'tbHybridStrategyParams
        '
        Me.tbHybridStrategyParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbHybridStrategyParams.Location = New System.Drawing.Point(81, 3)
        Me.tbHybridStrategyParams.Name = "tbHybridStrategyParams"
        Me.tbHybridStrategyParams.Size = New System.Drawing.Size(406, 20)
        Me.tbHybridStrategyParams.TabIndex = 12
        '
        'btnBrowseHybridStrategyParams
        '
        Me.btnBrowseHybridStrategyParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowseHybridStrategyParams.Image = CType(resources.GetObject("btnBrowseHybridStrategyParams.Image"),System.Drawing.Image)
        Me.btnBrowseHybridStrategyParams.Location = New System.Drawing.Point(489, 1)
        Me.btnBrowseHybridStrategyParams.Name = "btnBrowseHybridStrategyParams"
        Me.btnBrowseHybridStrategyParams.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseHybridStrategyParams.TabIndex = 13
        Me.btnBrowseHybridStrategyParams.TabStop = false
        Me.btnBrowseHybridStrategyParams.UseVisualStyleBackColor = true
        '
        'pnVehicle
        '
        Me.pnVehicle.Controls.Add(Me.ButOpenVEH)
        Me.pnVehicle.Controls.Add(Me.ButtonVEH)
        Me.pnVehicle.Controls.Add(Me.TbVEH)
        Me.pnVehicle.Location = New System.Drawing.Point(4, 7)
        Me.pnVehicle.Name = "pnVehicle"
        Me.pnVehicle.Size = New System.Drawing.Size(518, 27)
        Me.pnVehicle.TabIndex = 17
        '
        'ButOpenVEH
        '
        Me.ButOpenVEH.Location = New System.Drawing.Point(4, 3)
        Me.ButOpenVEH.Name = "ButOpenVEH"
        Me.ButOpenVEH.Size = New System.Drawing.Size(72, 21)
        Me.ButOpenVEH.TabIndex = 0
        Me.ButOpenVEH.TabStop = false
        Me.ButOpenVEH.Text = "Vehicle"
        Me.ButOpenVEH.UseVisualStyleBackColor = true
        '
        'ButtonVEH
        '
        Me.ButtonVEH.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"),System.Drawing.Image)
        Me.ButtonVEH.Location = New System.Drawing.Point(492, 2)
        Me.ButtonVEH.Name = "ButtonVEH"
        Me.ButtonVEH.Size = New System.Drawing.Size(24, 24)
        Me.ButtonVEH.TabIndex = 2
        Me.ButtonVEH.TabStop = false
        Me.ButtonVEH.UseVisualStyleBackColor = true
        '
        'TbVEH
        '
        Me.TbVEH.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbVEH.Location = New System.Drawing.Point(82, 4)
        Me.TbVEH.Name = "TbVEH"
        Me.TbVEH.Size = New System.Drawing.Size(404, 20)
        Me.TbVEH.TabIndex = 1
        '
        'pnEngine
        '
        Me.pnEngine.Controls.Add(Me.ButOpenENG)
        Me.pnEngine.Controls.Add(Me.ButtonMAP)
        Me.pnEngine.Controls.Add(Me.TbENG)
        Me.pnEngine.Location = New System.Drawing.Point(4, 37)
        Me.pnEngine.Name = "pnEngine"
        Me.pnEngine.Size = New System.Drawing.Size(519, 27)
        Me.pnEngine.TabIndex = 16
        '
        'ButOpenENG
        '
        Me.ButOpenENG.Location = New System.Drawing.Point(4, 3)
        Me.ButOpenENG.Name = "ButOpenENG"
        Me.ButOpenENG.Size = New System.Drawing.Size(72, 21)
        Me.ButOpenENG.TabIndex = 3
        Me.ButOpenENG.TabStop = false
        Me.ButOpenENG.Text = "Engine"
        Me.ButOpenENG.UseVisualStyleBackColor = true
        '
        'ButtonMAP
        '
        Me.ButtonMAP.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButtonMAP.Image = CType(resources.GetObject("ButtonMAP.Image"),System.Drawing.Image)
        Me.ButtonMAP.Location = New System.Drawing.Point(492, 1)
        Me.ButtonMAP.Name = "ButtonMAP"
        Me.ButtonMAP.Size = New System.Drawing.Size(24, 24)
        Me.ButtonMAP.TabIndex = 5
        Me.ButtonMAP.TabStop = false
        Me.ButtonMAP.UseVisualStyleBackColor = true
        '
        'TbENG
        '
        Me.TbENG.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbENG.Location = New System.Drawing.Point(82, 3)
        Me.TbENG.Name = "TbENG"
        Me.TbENG.Size = New System.Drawing.Size(404, 20)
        Me.TbENG.TabIndex = 4
        '
        'pnGearbox
        '
        Me.pnGearbox.Controls.Add(Me.ButOpenGBX)
        Me.pnGearbox.Controls.Add(Me.ButtonGBX)
        Me.pnGearbox.Controls.Add(Me.TbGBX)
        Me.pnGearbox.Location = New System.Drawing.Point(3, 66)
        Me.pnGearbox.Name = "pnGearbox"
        Me.pnGearbox.Size = New System.Drawing.Size(517, 28)
        Me.pnGearbox.TabIndex = 15
        '
        'ButOpenGBX
        '
        Me.ButOpenGBX.Location = New System.Drawing.Point(4, 3)
        Me.ButOpenGBX.Name = "ButOpenGBX"
        Me.ButOpenGBX.Size = New System.Drawing.Size(72, 21)
        Me.ButOpenGBX.TabIndex = 6
        Me.ButOpenGBX.TabStop = false
        Me.ButOpenGBX.Text = "Gearbox"
        Me.ButOpenGBX.UseVisualStyleBackColor = true
        '
        'ButtonGBX
        '
        Me.ButtonGBX.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButtonGBX.Image = CType(resources.GetObject("ButtonGBX.Image"),System.Drawing.Image)
        Me.ButtonGBX.Location = New System.Drawing.Point(492, 1)
        Me.ButtonGBX.Name = "ButtonGBX"
        Me.ButtonGBX.Size = New System.Drawing.Size(24, 24)
        Me.ButtonGBX.TabIndex = 8
        Me.ButtonGBX.TabStop = false
        Me.ButtonGBX.UseVisualStyleBackColor = true
        '
        'TbGBX
        '
        Me.TbGBX.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbGBX.Location = New System.Drawing.Point(82, 3)
        Me.TbGBX.Name = "TbGBX"
        Me.TbGBX.Size = New System.Drawing.Size(404, 20)
        Me.TbGBX.TabIndex = 7
        '
        'pnShiftParams
        '
        Me.pnShiftParams.Controls.Add(Me.BtnShiftParamsForm)
        Me.pnShiftParams.Controls.Add(Me.TbShiftStrategyParams)
        Me.pnShiftParams.Controls.Add(Me.BtnShiftStrategyParams)
        Me.pnShiftParams.Location = New System.Drawing.Point(4, 96)
        Me.pnShiftParams.Name = "pnShiftParams"
        Me.pnShiftParams.Size = New System.Drawing.Size(516, 26)
        Me.pnShiftParams.TabIndex = 14
        '
        'BtnShiftParamsForm
        '
        Me.BtnShiftParamsForm.Location = New System.Drawing.Point(3, 3)
        Me.BtnShiftParamsForm.Name = "BtnShiftParamsForm"
        Me.BtnShiftParamsForm.Size = New System.Drawing.Size(72, 21)
        Me.BtnShiftParamsForm.TabIndex = 11
        Me.BtnShiftParamsForm.TabStop = false
        Me.BtnShiftParamsForm.Text = "Shift Parameters"
        Me.BtnShiftParamsForm.UseVisualStyleBackColor = true
        '
        'TbShiftStrategyParams
        '
        Me.TbShiftStrategyParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbShiftStrategyParams.Location = New System.Drawing.Point(81, 3)
        Me.TbShiftStrategyParams.Name = "TbShiftStrategyParams"
        Me.TbShiftStrategyParams.Size = New System.Drawing.Size(406, 20)
        Me.TbShiftStrategyParams.TabIndex = 12
        '
        'BtnShiftStrategyParams
        '
        Me.BtnShiftStrategyParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtnShiftStrategyParams.Image = CType(resources.GetObject("BtnShiftStrategyParams.Image"),System.Drawing.Image)
        Me.BtnShiftStrategyParams.Location = New System.Drawing.Point(489, 1)
        Me.BtnShiftStrategyParams.Name = "BtnShiftStrategyParams"
        Me.BtnShiftStrategyParams.Size = New System.Drawing.Size(24, 24)
        Me.BtnShiftStrategyParams.TabIndex = 13
        Me.BtnShiftStrategyParams.TabStop = false
        Me.BtnShiftStrategyParams.UseVisualStyleBackColor = true
        '
        'GrCycles
        '
        Me.GrCycles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GrCycles.Controls.Add(Me.Label2)
        Me.GrCycles.Controls.Add(Me.LvCycles)
        Me.GrCycles.Controls.Add(Me.BtDRIrem)
        Me.GrCycles.Controls.Add(Me.BtDRIadd)
        Me.GrCycles.Location = New System.Drawing.Point(6, 6)
        Me.GrCycles.Name = "GrCycles"
        Me.GrCycles.Size = New System.Drawing.Size(515, 138)
        Me.GrCycles.TabIndex = 10
        Me.GrCycles.TabStop = false
        Me.GrCycles.Text = "Cycles"
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(391, 109)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(118, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "(Right-Click for Options)"
        '
        'LvCycles
        '
        Me.LvCycles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvCycles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.LvCycles.FullRowSelect = true
        Me.LvCycles.GridLines = true
        Me.LvCycles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.LvCycles.HideSelection = false
        Me.LvCycles.LabelEdit = true
        Me.LvCycles.Location = New System.Drawing.Point(6, 19)
        Me.LvCycles.MultiSelect = false
        Me.LvCycles.Name = "LvCycles"
        Me.LvCycles.Size = New System.Drawing.Size(503, 89)
        Me.LvCycles.TabIndex = 0
        Me.LvCycles.TabStop = false
        Me.LvCycles.UseCompatibleStateImageBehavior = false
        Me.LvCycles.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Cycle path"
        Me.ColumnHeader1.Width = 470
        '
        'BtDRIrem
        '
        Me.BtDRIrem.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtDRIrem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.BtDRIrem.Location = New System.Drawing.Point(29, 109)
        Me.BtDRIrem.Name = "BtDRIrem"
        Me.BtDRIrem.Size = New System.Drawing.Size(24, 24)
        Me.BtDRIrem.TabIndex = 2
        Me.BtDRIrem.UseVisualStyleBackColor = true
        '
        'BtDRIadd
        '
        Me.BtDRIadd.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtDRIadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.BtDRIadd.Location = New System.Drawing.Point(5, 109)
        Me.BtDRIadd.Name = "BtDRIadd"
        Me.BtDRIadd.Size = New System.Drawing.Size(24, 24)
        Me.BtDRIadd.TabIndex = 1
        Me.BtDRIadd.UseVisualStyleBackColor = true
        '
        'GrAuxMech
        '
        Me.GrAuxMech.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GrAuxMech.Controls.Add(Me.pnAuxDeclarationMode)
        Me.GrAuxMech.Controls.Add(Me.pnAuxEngineering)
        Me.GrAuxMech.Location = New System.Drawing.Point(6, 16)
        Me.GrAuxMech.Name = "GrAuxMech"
        Me.GrAuxMech.Size = New System.Drawing.Size(515, 280)
        Me.GrAuxMech.TabIndex = 9
        Me.GrAuxMech.TabStop = false
        Me.GrAuxMech.Text = "Mechanical Auxiliaries"
        '
        'pnAuxDeclarationMode
        '
        Me.pnAuxDeclarationMode.Controls.Add(Me.LvAux)
        Me.pnAuxDeclarationMode.Controls.Add(Me.Label32)
        Me.pnAuxDeclarationMode.Location = New System.Drawing.Point(6, 104)
        Me.pnAuxDeclarationMode.Name = "pnAuxDeclarationMode"
        Me.pnAuxDeclarationMode.Size = New System.Drawing.Size(503, 170)
        Me.pnAuxDeclarationMode.TabIndex = 40
        '
        'LvAux
        '
        Me.LvAux.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvAux.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
        Me.LvAux.FullRowSelect = true
        Me.LvAux.GridLines = true
        Me.LvAux.HideSelection = false
        Me.LvAux.Location = New System.Drawing.Point(3, 3)
        Me.LvAux.MultiSelect = false
        Me.LvAux.Name = "LvAux"
        Me.LvAux.Size = New System.Drawing.Size(504, 145)
        Me.LvAux.TabIndex = 0
        Me.LvAux.TabStop = false
        Me.LvAux.UseCompatibleStateImageBehavior = false
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
        Me.Label32.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label32.AutoSize = true
        Me.Label32.Location = New System.Drawing.Point(9, 151)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(106, 13)
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
        Me.pnAuxEngineering.Location = New System.Drawing.Point(6, 19)
        Me.pnAuxEngineering.Name = "pnAuxEngineering"
        Me.pnAuxEngineering.Size = New System.Drawing.Size(500, 80)
        Me.pnAuxEngineering.TabIndex = 45
        '
        'tbPAuxStandstillICEOff
        '
        Me.tbPAuxStandstillICEOff.Location = New System.Drawing.Point(170, 55)
        Me.tbPAuxStandstillICEOff.Name = "tbPAuxStandstillICEOff"
        Me.tbPAuxStandstillICEOff.Size = New System.Drawing.Size(76, 20)
        Me.tbPAuxStandstillICEOff.TabIndex = 49
        '
        'lblAuxStandstillICEOffUnit
        '
        Me.lblAuxStandstillICEOffUnit.AutoSize = true
        Me.lblAuxStandstillICEOffUnit.Location = New System.Drawing.Point(252, 58)
        Me.lblAuxStandstillICEOffUnit.Name = "lblAuxStandstillICEOffUnit"
        Me.lblAuxStandstillICEOffUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblAuxStandstillICEOffUnit.TabIndex = 50
        Me.lblAuxStandstillICEOffUnit.Text = "[W]"
        '
        'lblAuxStandstillICEOff
        '
        Me.lblAuxStandstillICEOff.AutoSize = true
        Me.lblAuxStandstillICEOff.Location = New System.Drawing.Point(3, 58)
        Me.lblAuxStandstillICEOff.Name = "lblAuxStandstillICEOff"
        Me.lblAuxStandstillICEOff.Size = New System.Drawing.Size(143, 13)
        Me.lblAuxStandstillICEOff.TabIndex = 48
        Me.lblAuxStandstillICEOff.Text = "Aux Load (Standstill, ICE Off)"
        '
        'tbPAuxDrivingICEOff
        '
        Me.tbPAuxDrivingICEOff.Location = New System.Drawing.Point(170, 29)
        Me.tbPAuxDrivingICEOff.Name = "tbPAuxDrivingICEOff"
        Me.tbPAuxDrivingICEOff.Size = New System.Drawing.Size(76, 20)
        Me.tbPAuxDrivingICEOff.TabIndex = 46
        '
        'lblAuxDrivingICEOffUnit
        '
        Me.lblAuxDrivingICEOffUnit.AutoSize = true
        Me.lblAuxDrivingICEOffUnit.Location = New System.Drawing.Point(252, 32)
        Me.lblAuxDrivingICEOffUnit.Name = "lblAuxDrivingICEOffUnit"
        Me.lblAuxDrivingICEOffUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblAuxDrivingICEOffUnit.TabIndex = 47
        Me.lblAuxDrivingICEOffUnit.Text = "[W]"
        '
        'lblAuxDrivingICEOff
        '
        Me.lblAuxDrivingICEOff.AutoSize = true
        Me.lblAuxDrivingICEOff.Location = New System.Drawing.Point(3, 32)
        Me.lblAuxDrivingICEOff.Name = "lblAuxDrivingICEOff"
        Me.lblAuxDrivingICEOff.Size = New System.Drawing.Size(134, 13)
        Me.lblAuxDrivingICEOff.TabIndex = 45
        Me.lblAuxDrivingICEOff.Text = "Aux Load (Driving, ICE Off)"
        '
        'TbAuxPAuxICEOn
        '
        Me.TbAuxPAuxICEOn.Location = New System.Drawing.Point(170, 3)
        Me.TbAuxPAuxICEOn.Name = "TbAuxPAuxICEOn"
        Me.TbAuxPAuxICEOn.Size = New System.Drawing.Size(76, 20)
        Me.TbAuxPAuxICEOn.TabIndex = 43
        '
        'lblAuxICEOnUnit
        '
        Me.lblAuxICEOnUnit.AutoSize = true
        Me.lblAuxICEOnUnit.Location = New System.Drawing.Point(252, 6)
        Me.lblAuxICEOnUnit.Name = "lblAuxICEOnUnit"
        Me.lblAuxICEOnUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblAuxICEOnUnit.TabIndex = 44
        Me.lblAuxICEOnUnit.Text = "[W]"
        '
        'lblAuxICEOn
        '
        Me.lblAuxICEOn.AutoSize = true
        Me.lblAuxICEOn.Location = New System.Drawing.Point(3, 6)
        Me.lblAuxICEOn.Name = "lblAuxICEOn"
        Me.lblAuxICEOn.Size = New System.Drawing.Size(95, 13)
        Me.lblAuxICEOn.TabIndex = 42
        Me.lblAuxICEOn.Text = "Aux Load (ICE On)"
        '
        'GrAux
        '
        Me.GrAux.Location = New System.Drawing.Point(0, 0)
        Me.GrAux.Name = "GrAux"
        Me.GrAux.Size = New System.Drawing.Size(200, 100)
        Me.GrAux.TabIndex = 0
        Me.GrAux.TabStop = false
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
        Me.TbAuxPAdd.Size = New System.Drawing.Size(100, 20)
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
        Me.txtAdvancedAuxiliaryFile.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvancedAuxiliaryFile.TabIndex = 0
        '
        'picAuxInfo
        '
        Me.picAuxInfo.Location = New System.Drawing.Point(0, 0)
        Me.picAuxInfo.Name = "picAuxInfo"
        Me.picAuxInfo.Size = New System.Drawing.Size(100, 50)
        Me.picAuxInfo.TabIndex = 0
        Me.picAuxInfo.TabStop = false
        '
        'cboAdvancedAuxiliaries
        '
        Me.cboAdvancedAuxiliaries.Location = New System.Drawing.Point(0, 0)
        Me.cboAdvancedAuxiliaries.Name = "cboAdvancedAuxiliaries"
        Me.cboAdvancedAuxiliaries.Size = New System.Drawing.Size(121, 21)
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
        Me.tcJob.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tcJob.Controls.Add(Me.TabPgGen)
        Me.tcJob.Controls.Add(Me.tpAuxiliaries)
        Me.tcJob.Controls.Add(Me.tpCycles)
        Me.tcJob.Controls.Add(Me.TabPgDriver)
        Me.tcJob.Controls.Add(Me.TabPgADAS)
        Me.tcJob.Location = New System.Drawing.Point(1, 78)
        Me.tcJob.Name = "tcJob"
        Me.tcJob.SelectedIndex = 0
        Me.tcJob.Size = New System.Drawing.Size(533, 520)
        Me.tcJob.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tcJob.TabIndex = 0
        '
        'tpAuxiliaries
        '
        Me.tpAuxiliaries.Controls.Add(Me.gbBusAux)
        Me.tpAuxiliaries.Controls.Add(Me.gbElectricAux)
        Me.tpAuxiliaries.Controls.Add(Me.GrAuxMech)
        Me.tpAuxiliaries.Location = New System.Drawing.Point(4, 22)
        Me.tpAuxiliaries.Name = "tpAuxiliaries"
        Me.tpAuxiliaries.Padding = New System.Windows.Forms.Padding(3)
        Me.tpAuxiliaries.Size = New System.Drawing.Size(525, 494)
        Me.tpAuxiliaries.TabIndex = 9
        Me.tpAuxiliaries.Text = "Auxiliaries"
        Me.tpAuxiliaries.UseVisualStyleBackColor = true
        '
        'gbBusAux
        '
        Me.gbBusAux.Controls.Add(Me.cbEnableBusAux)
        Me.gbBusAux.Controls.Add(Me.pnBusAux)
        Me.gbBusAux.Location = New System.Drawing.Point(6, 302)
        Me.gbBusAux.Name = "gbBusAux"
        Me.gbBusAux.Size = New System.Drawing.Size(515, 78)
        Me.gbBusAux.TabIndex = 11
        Me.gbBusAux.TabStop = false
        Me.gbBusAux.Text = "Bus Auxiliaries"
        '
        'cbEnableBusAux
        '
        Me.cbEnableBusAux.AutoSize = true
        Me.cbEnableBusAux.Location = New System.Drawing.Point(10, 17)
        Me.cbEnableBusAux.Name = "cbEnableBusAux"
        Me.cbEnableBusAux.Size = New System.Drawing.Size(122, 21)
        Me.cbEnableBusAux.TabIndex = 17
        Me.cbEnableBusAux.Text = "Use Bus Auxiliaries"
        Me.cbEnableBusAux.UseVisualStyleBackColor = true
        '
        'pnBusAux
        '
        Me.pnBusAux.Controls.Add(Me.btnBusAuxP)
        Me.pnBusAux.Controls.Add(Me.tbBusAuxParams)
        Me.pnBusAux.Controls.Add(Me.btnBrowsBusAuxParams)
        Me.pnBusAux.Enabled = false
        Me.pnBusAux.Location = New System.Drawing.Point(6, 40)
        Me.pnBusAux.Name = "pnBusAux"
        Me.pnBusAux.Size = New System.Drawing.Size(503, 26)
        Me.pnBusAux.TabIndex = 16
        '
        'btnBusAuxP
        '
        Me.btnBusAuxP.Location = New System.Drawing.Point(3, 3)
        Me.btnBusAuxP.Name = "btnBusAuxP"
        Me.btnBusAuxP.Size = New System.Drawing.Size(72, 21)
        Me.btnBusAuxP.TabIndex = 11
        Me.btnBusAuxP.TabStop = false
        Me.btnBusAuxP.Text = "BusAux P."
        Me.btnBusAuxP.UseVisualStyleBackColor = true
        '
        'tbBusAuxParams
        '
        Me.tbBusAuxParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbBusAuxParams.Location = New System.Drawing.Point(81, 3)
        Me.tbBusAuxParams.Name = "tbBusAuxParams"
        Me.tbBusAuxParams.Size = New System.Drawing.Size(393, 20)
        Me.tbBusAuxParams.TabIndex = 12
        '
        'btnBrowsBusAuxParams
        '
        Me.btnBrowsBusAuxParams.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowsBusAuxParams.Image = CType(resources.GetObject("btnBrowsBusAuxParams.Image"),System.Drawing.Image)
        Me.btnBrowsBusAuxParams.Location = New System.Drawing.Point(476, 1)
        Me.btnBrowsBusAuxParams.Name = "btnBrowsBusAuxParams"
        Me.btnBrowsBusAuxParams.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowsBusAuxParams.TabIndex = 13
        Me.btnBrowsBusAuxParams.TabStop = false
        Me.btnBrowsBusAuxParams.UseVisualStyleBackColor = true
        '
        'gbElectricAux
        '
        Me.gbElectricAux.Controls.Add(Me.Label46)
        Me.gbElectricAux.Controls.Add(Me.lblElAuxConstUnit)
        Me.gbElectricAux.Controls.Add(Me.tbElectricAuxConstant)
        Me.gbElectricAux.Controls.Add(Me.lblElAuxConst)
        Me.gbElectricAux.Location = New System.Drawing.Point(6, 386)
        Me.gbElectricAux.Name = "gbElectricAux"
        Me.gbElectricAux.Size = New System.Drawing.Size(515, 55)
        Me.gbElectricAux.TabIndex = 10
        Me.gbElectricAux.TabStop = false
        Me.gbElectricAux.Text = "Electric Auxiliaries"
        Me.gbElectricAux.Visible = false
        '
        'Label46
        '
        Me.Label46.AutoSize = true
        Me.Label46.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label46.Location = New System.Drawing.Point(235, 22)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(169, 20)
        Me.Label46.TabIndex = 48
        Me.Label46.Text = "(high voltage system)"
        '
        'lblElAuxConstUnit
        '
        Me.lblElAuxConstUnit.AutoSize = true
        Me.lblElAuxConstUnit.Location = New System.Drawing.Point(191, 22)
        Me.lblElAuxConstUnit.Name = "lblElAuxConstUnit"
        Me.lblElAuxConstUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblElAuxConstUnit.TabIndex = 47
        Me.lblElAuxConstUnit.Text = "[W]"
        '
        'tbElectricAuxConstant
        '
        Me.tbElectricAuxConstant.Location = New System.Drawing.Point(109, 19)
        Me.tbElectricAuxConstant.Name = "tbElectricAuxConstant"
        Me.tbElectricAuxConstant.Size = New System.Drawing.Size(76, 20)
        Me.tbElectricAuxConstant.TabIndex = 46
        '
        'lblElAuxConst
        '
        Me.lblElAuxConst.AutoSize = true
        Me.lblElAuxConst.Location = New System.Drawing.Point(7, 22)
        Me.lblElAuxConst.Name = "lblElAuxConst"
        Me.lblElAuxConst.Size = New System.Drawing.Size(97, 13)
        Me.lblElAuxConst.TabIndex = 45
        Me.lblElAuxConst.Text = "Constant Aux Load"
        '
        'tpCycles
        '
        Me.tpCycles.Controls.Add(Me.GrCycles)
        Me.tpCycles.Location = New System.Drawing.Point(4, 22)
        Me.tpCycles.Name = "tpCycles"
        Me.tpCycles.Padding = New System.Windows.Forms.Padding(3)
        Me.tpCycles.Size = New System.Drawing.Size(525, 494)
        Me.tpCycles.TabIndex = 10
        Me.tpCycles.Text = "Cycles"
        Me.tpCycles.UseVisualStyleBackColor = true
        '
        'TabPgDriver
        '
        Me.TabPgDriver.Controls.Add(Me.gbShiftStrategy)
        Me.TabPgDriver.Controls.Add(Me.GrVACC)
        Me.TabPgDriver.Controls.Add(Me.GrLAC)
        Me.TabPgDriver.Controls.Add(Me.gbOverspeed)
        Me.TabPgDriver.Location = New System.Drawing.Point(4, 22)
        Me.TabPgDriver.Name = "TabPgDriver"
        Me.TabPgDriver.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPgDriver.Size = New System.Drawing.Size(525, 494)
        Me.TabPgDriver.TabIndex = 7
        Me.TabPgDriver.Text = "Driver Model"
        Me.TabPgDriver.UseVisualStyleBackColor = true
        '
        'gbShiftStrategy
        '
        Me.gbShiftStrategy.Controls.Add(Me.cbGearshiftStrategy)
        Me.gbShiftStrategy.Location = New System.Drawing.Point(9, 347)
        Me.gbShiftStrategy.Name = "gbShiftStrategy"
        Me.gbShiftStrategy.Size = New System.Drawing.Size(514, 50)
        Me.gbShiftStrategy.TabIndex = 4
        Me.gbShiftStrategy.TabStop = false
        Me.gbShiftStrategy.Text = "Gearshift Strategy"
        '
        'cbGearshiftStrategy
        '
        Me.cbGearshiftStrategy.FormattingEnabled = true
        Me.cbGearshiftStrategy.Location = New System.Drawing.Point(6, 19)
        Me.cbGearshiftStrategy.Name = "cbGearshiftStrategy"
        Me.cbGearshiftStrategy.Size = New System.Drawing.Size(270, 21)
        Me.cbGearshiftStrategy.TabIndex = 0
        '
        'GrVACC
        '
        Me.GrVACC.Controls.Add(Me.Label15)
        Me.GrVACC.Controls.Add(Me.TbDesMaxFile)
        Me.GrVACC.Controls.Add(Me.BtDesMaxBr)
        Me.GrVACC.Controls.Add(Me.BtAccOpen)
        Me.GrVACC.Location = New System.Drawing.Point(9, 271)
        Me.GrVACC.Name = "GrVACC"
        Me.GrVACC.Size = New System.Drawing.Size(515, 69)
        Me.GrVACC.TabIndex = 3
        Me.GrVACC.TabStop = false
        Me.GrVACC.Text = "Max. acceleration and brake curves"
        '
        'Label15
        '
        Me.Label15.AutoSize = true
        Me.Label15.Location = New System.Drawing.Point(3, 20)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(220, 13)
        Me.Label15.TabIndex = 4
        Me.Label15.Text = "Driver Acceleration/Deceleration File (.vacc):"
        '
        'TbDesMaxFile
        '
        Me.TbDesMaxFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbDesMaxFile.Location = New System.Drawing.Point(6, 36)
        Me.TbDesMaxFile.Name = "TbDesMaxFile"
        Me.TbDesMaxFile.Size = New System.Drawing.Size(450, 20)
        Me.TbDesMaxFile.TabIndex = 0
        '
        'BtDesMaxBr
        '
        Me.BtDesMaxBr.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtDesMaxBr.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtDesMaxBr.Location = New System.Drawing.Point(457, 34)
        Me.BtDesMaxBr.Name = "BtDesMaxBr"
        Me.BtDesMaxBr.Size = New System.Drawing.Size(24, 24)
        Me.BtDesMaxBr.TabIndex = 1
        Me.BtDesMaxBr.UseVisualStyleBackColor = true
        '
        'BtAccOpen
        '
        Me.BtAccOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtAccOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtAccOpen.Location = New System.Drawing.Point(481, 34)
        Me.BtAccOpen.Name = "BtAccOpen"
        Me.BtAccOpen.Size = New System.Drawing.Size(24, 24)
        Me.BtAccOpen.TabIndex = 2
        Me.BtAccOpen.TabStop = false
        Me.BtAccOpen.UseVisualStyleBackColor = true
        '
        'GrLAC
        '
        Me.GrLAC.Controls.Add(Me.pnLookAheadCoasting)
        Me.GrLAC.Controls.Add(Me.CbLookAhead)
        Me.GrLAC.Location = New System.Drawing.Point(9, 92)
        Me.GrLAC.Name = "GrLAC"
        Me.GrLAC.Size = New System.Drawing.Size(514, 173)
        Me.GrLAC.TabIndex = 2
        Me.GrLAC.TabStop = false
        Me.GrLAC.Text = "Look-Ahead Coasting"
        '
        'pnLookAheadCoasting
        '
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
        Me.pnLookAheadCoasting.Location = New System.Drawing.Point(15, 19)
        Me.pnLookAheadCoasting.Name = "pnLookAheadCoasting"
        Me.pnLookAheadCoasting.Size = New System.Drawing.Size(493, 129)
        Me.pnLookAheadCoasting.TabIndex = 20
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(245, 6)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 13)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "[km/h]"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(106, 5)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(67, 13)
        Me.Label6.TabIndex = 33
        Me.Label6.Text = "Min. Velocity"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tbLacMinSpeed
        '
        Me.tbLacMinSpeed.Location = New System.Drawing.Point(179, 3)
        Me.tbLacMinSpeed.Name = "tbLacMinSpeed"
        Me.tbLacMinSpeed.Size = New System.Drawing.Size(64, 20)
        Me.tbLacMinSpeed.TabIndex = 34
        '
        'btnDfVelocityDrop
        '
        Me.btnDfVelocityDrop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnDfVelocityDrop.Image = CType(resources.GetObject("btnDfVelocityDrop.Image"),System.Drawing.Image)
        Me.btnDfVelocityDrop.Location = New System.Drawing.Point(466, 78)
        Me.btnDfVelocityDrop.Name = "btnDfVelocityDrop"
        Me.btnDfVelocityDrop.Size = New System.Drawing.Size(24, 24)
        Me.btnDfVelocityDrop.TabIndex = 32
        Me.btnDfVelocityDrop.TabStop = false
        Me.btnDfVelocityDrop.UseVisualStyleBackColor = true
        '
        'Label12
        '
        Me.Label12.AutoSize = true
        Me.Label12.Location = New System.Drawing.Point(284, 110)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(130, 13)
        Me.Label12.TabIndex = 31
        Me.Label12.Text = "* DF_vTarget * DF_vDrop"
        '
        'tbDfCoastingScale
        '
        Me.tbDfCoastingScale.Location = New System.Drawing.Point(241, 107)
        Me.tbDfCoastingScale.Name = "tbDfCoastingScale"
        Me.tbDfCoastingScale.Size = New System.Drawing.Size(37, 20)
        Me.tbDfCoastingScale.TabIndex = 30
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(224, 109)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(13, 13)
        Me.Label11.TabIndex = 29
        Me.Label11.Text = "- "
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(55, 30)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(118, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Preview distance factor"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tbDfCoastingOffset
        '
        Me.tbDfCoastingOffset.Location = New System.Drawing.Point(180, 107)
        Me.tbDfCoastingOffset.Name = "tbDfCoastingOffset"
        Me.tbDfCoastingOffset.Size = New System.Drawing.Size(37, 20)
        Me.tbDfCoastingOffset.TabIndex = 28
        '
        'tbLacDfTargetSpeedFile
        '
        Me.tbLacDfTargetSpeedFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbLacDfTargetSpeedFile.Location = New System.Drawing.Point(179, 54)
        Me.tbLacDfTargetSpeedFile.Name = "tbLacDfTargetSpeedFile"
        Me.tbLacDfTargetSpeedFile.Size = New System.Drawing.Size(286, 20)
        Me.tbLacDfTargetSpeedFile.TabIndex = 22
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(94, 109)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(79, 13)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "DF_coasting = "
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(18, 56)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(155, 13)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "Decision Factor - Target Speed"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(20, 83)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(153, 13)
        Me.Label5.TabIndex = 26
        Me.Label5.Text = "Decision Factor - Velocity Drop"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnDfTargetSpeed
        '
        Me.btnDfTargetSpeed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnDfTargetSpeed.Image = CType(resources.GetObject("btnDfTargetSpeed.Image"),System.Drawing.Image)
        Me.btnDfTargetSpeed.Location = New System.Drawing.Point(466, 52)
        Me.btnDfTargetSpeed.Name = "btnDfTargetSpeed"
        Me.btnDfTargetSpeed.Size = New System.Drawing.Size(24, 24)
        Me.btnDfTargetSpeed.TabIndex = 23
        Me.btnDfTargetSpeed.TabStop = false
        Me.btnDfTargetSpeed.UseVisualStyleBackColor = true
        '
        'tbLacPreviewFactor
        '
        Me.tbLacPreviewFactor.Location = New System.Drawing.Point(179, 28)
        Me.tbLacPreviewFactor.Name = "tbLacPreviewFactor"
        Me.tbLacPreviewFactor.Size = New System.Drawing.Size(64, 20)
        Me.tbLacPreviewFactor.TabIndex = 21
        '
        'tbLacDfVelocityDropFile
        '
        Me.tbLacDfVelocityDropFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbLacDfVelocityDropFile.Location = New System.Drawing.Point(179, 81)
        Me.tbLacDfVelocityDropFile.Name = "tbLacDfVelocityDropFile"
        Me.tbLacDfVelocityDropFile.Size = New System.Drawing.Size(286, 20)
        Me.tbLacDfVelocityDropFile.TabIndex = 25
        '
        'CbLookAhead
        '
        Me.CbLookAhead.AutoSize = true
        Me.CbLookAhead.Checked = true
        Me.CbLookAhead.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CbLookAhead.Location = New System.Drawing.Point(16, 21)
        Me.CbLookAhead.Name = "CbLookAhead"
        Me.CbLookAhead.Size = New System.Drawing.Size(72, 21)
        Me.CbLookAhead.TabIndex = 0
        Me.CbLookAhead.Text = "Enabled"
        Me.CbLookAhead.UseVisualStyleBackColor = true
        '
        'gbOverspeed
        '
        Me.gbOverspeed.Controls.Add(Me.PnEcoRoll)
        Me.gbOverspeed.Controls.Add(Me.RdOverspeed)
        Me.gbOverspeed.Controls.Add(Me.RdOff)
        Me.gbOverspeed.Location = New System.Drawing.Point(9, 9)
        Me.gbOverspeed.Name = "gbOverspeed"
        Me.gbOverspeed.Size = New System.Drawing.Size(515, 77)
        Me.gbOverspeed.TabIndex = 1
        Me.gbOverspeed.TabStop = false
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
        Me.PnEcoRoll.Location = New System.Drawing.Point(91, 17)
        Me.PnEcoRoll.Name = "PnEcoRoll"
        Me.PnEcoRoll.Size = New System.Drawing.Size(232, 54)
        Me.PnEcoRoll.TabIndex = 3
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(170, 31)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(38, 13)
        Me.Label21.TabIndex = 3
        Me.Label21.Text = "[km/h]"
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(170, 5)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(38, 13)
        Me.Label14.TabIndex = 3
        Me.Label14.Text = "[km/h]"
        '
        'TbVmin
        '
        Me.TbVmin.Location = New System.Drawing.Point(104, 28)
        Me.TbVmin.Name = "TbVmin"
        Me.TbVmin.Size = New System.Drawing.Size(64, 20)
        Me.TbVmin.TabIndex = 2
        '
        'TbOverspeed
        '
        Me.TbOverspeed.Location = New System.Drawing.Point(104, 2)
        Me.TbOverspeed.Name = "TbOverspeed"
        Me.TbOverspeed.Size = New System.Drawing.Size(64, 20)
        Me.TbOverspeed.TabIndex = 0
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Location = New System.Drawing.Point(18, 31)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(80, 13)
        Me.Label23.TabIndex = 1
        Me.Label23.Text = "Minimum speed"
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(13, 5)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(85, 13)
        Me.Label13.TabIndex = 1
        Me.Label13.Text = "Max. Overspeed"
        '
        'RdOverspeed
        '
        Me.RdOverspeed.AutoSize = true
        Me.RdOverspeed.Checked = true
        Me.RdOverspeed.Location = New System.Drawing.Point(16, 44)
        Me.RdOverspeed.Name = "RdOverspeed"
        Me.RdOverspeed.Size = New System.Drawing.Size(84, 20)
        Me.RdOverspeed.TabIndex = 1
        Me.RdOverspeed.TabStop = true
        Me.RdOverspeed.Text = "Overspeed"
        Me.RdOverspeed.UseVisualStyleBackColor = true
        '
        'RdOff
        '
        Me.RdOff.AutoSize = true
        Me.RdOff.Location = New System.Drawing.Point(16, 21)
        Me.RdOff.Name = "RdOff"
        Me.RdOff.Size = New System.Drawing.Size(46, 20)
        Me.RdOff.TabIndex = 0
        Me.RdOff.Text = "Off"
        Me.RdOff.UseVisualStyleBackColor = true
        '
        'TabPgADAS
        '
        Me.TabPgADAS.Controls.Add(Me.gbPCC)
        Me.TabPgADAS.Controls.Add(Me.gbEcoRoll)
        Me.TabPgADAS.Controls.Add(Me.gbEngineStopStart)
        Me.TabPgADAS.Location = New System.Drawing.Point(4, 22)
        Me.TabPgADAS.Name = "TabPgADAS"
        Me.TabPgADAS.Size = New System.Drawing.Size(525, 494)
        Me.TabPgADAS.TabIndex = 8
        Me.TabPgADAS.Text = "ADAS Parameters"
        Me.TabPgADAS.UseVisualStyleBackColor = true
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
        Me.gbPCC.Location = New System.Drawing.Point(7, 269)
        Me.gbPCC.Name = "gbPCC"
        Me.gbPCC.Size = New System.Drawing.Size(515, 217)
        Me.gbPCC.TabIndex = 7
        Me.gbPCC.TabStop = false
        Me.gbPCC.Text = "Predictive Cruise Control"
        '
        'Label43
        '
        Me.Label43.AutoSize = true
        Me.Label43.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label43.Location = New System.Drawing.Point(39, 196)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(248, 20)
        Me.Label43.TabIndex = 30
        Me.Label43.Text = "(cf. column HW in driving cycle)"
        '
        'Label42
        '
        Me.Label42.AutoSize = true
        Me.Label42.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label42.Location = New System.Drawing.Point(7, 181)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(631, 20)
        Me.Label42.TabIndex = 29
        Me.Label42.Text = "Note: Predictive cruise conrol is only activated on highway parts of the driving "& _ 
    "cycle"
        '
        'Label40
        '
        Me.Label40.AutoSize = true
        Me.Label40.Location = New System.Drawing.Point(305, 152)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(21, 13)
        Me.Label40.TabIndex = 28
        Me.Label40.Text = "[m]"
        '
        'Label41
        '
        Me.Label41.AutoSize = true
        Me.Label41.Location = New System.Drawing.Point(7, 152)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(146, 13)
        Me.Label41.TabIndex = 26
        Me.Label41.Text = "Preview distance use case 2:"
        '
        'tbPCCPreviewUseCase2
        '
        Me.tbPCCPreviewUseCase2.Location = New System.Drawing.Point(247, 149)
        Me.tbPCCPreviewUseCase2.Name = "tbPCCPreviewUseCase2"
        Me.tbPCCPreviewUseCase2.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCPreviewUseCase2.TabIndex = 27
        '
        'Label38
        '
        Me.Label38.AutoSize = true
        Me.Label38.Location = New System.Drawing.Point(305, 126)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(21, 13)
        Me.Label38.TabIndex = 25
        Me.Label38.Text = "[m]"
        '
        'Label39
        '
        Me.Label39.AutoSize = true
        Me.Label39.Location = New System.Drawing.Point(7, 126)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(146, 13)
        Me.Label39.TabIndex = 23
        Me.Label39.Text = "Preview distance use case 1:"
        '
        'tbPCCPreviewUseCase1
        '
        Me.tbPCCPreviewUseCase1.Location = New System.Drawing.Point(247, 123)
        Me.tbPCCPreviewUseCase1.Name = "tbPCCPreviewUseCase1"
        Me.tbPCCPreviewUseCase1.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCPreviewUseCase1.TabIndex = 24
        '
        'Label36
        '
        Me.Label36.AutoSize = true
        Me.Label36.Location = New System.Drawing.Point(305, 100)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(38, 13)
        Me.Label36.TabIndex = 22
        Me.Label36.Text = "[km/h]"
        '
        'Label37
        '
        Me.Label37.AutoSize = true
        Me.Label37.Location = New System.Drawing.Point(7, 100)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(83, 13)
        Me.Label37.TabIndex = 20
        Me.Label37.Text = "Minimum speed:"
        '
        'tbPCCMinSpeed
        '
        Me.tbPCCMinSpeed.Location = New System.Drawing.Point(247, 97)
        Me.tbPCCMinSpeed.Name = "tbPCCMinSpeed"
        Me.tbPCCMinSpeed.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCMinSpeed.TabIndex = 21
        '
        'Label34
        '
        Me.Label34.AutoSize = true
        Me.Label34.Location = New System.Drawing.Point(305, 74)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(38, 13)
        Me.Label34.TabIndex = 19
        Me.Label34.Text = "[km/h]"
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(7, 74)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(113, 13)
        Me.Label35.TabIndex = 17
        Me.Label35.Text = "PCC enabling velocity:"
        '
        'tbPCCEnableSpeed
        '
        Me.tbPCCEnableSpeed.Location = New System.Drawing.Point(247, 71)
        Me.tbPCCEnableSpeed.Name = "tbPCCEnableSpeed"
        Me.tbPCCEnableSpeed.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCEnableSpeed.TabIndex = 18
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(305, 48)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(38, 13)
        Me.Label31.TabIndex = 16
        Me.Label31.Text = "[km/h]"
        '
        'Label33
        '
        Me.Label33.AutoSize = true
        Me.Label33.Location = New System.Drawing.Point(7, 48)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(100, 13)
        Me.Label33.TabIndex = 14
        Me.Label33.Text = "Allowed overspeed:"
        '
        'tbPCCOverspeed
        '
        Me.tbPCCOverspeed.Location = New System.Drawing.Point(247, 45)
        Me.tbPCCOverspeed.Name = "tbPCCOverspeed"
        Me.tbPCCOverspeed.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCOverspeed.TabIndex = 15
        '
        'Label20
        '
        Me.Label20.AutoSize = true
        Me.Label20.Location = New System.Drawing.Point(305, 22)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(38, 13)
        Me.Label20.TabIndex = 13
        Me.Label20.Text = "[km/h]"
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(7, 22)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(106, 13)
        Me.Label22.TabIndex = 11
        Me.Label22.Text = "Allowed underspeed:"
        '
        'tbPCCUnderspeed
        '
        Me.tbPCCUnderspeed.Location = New System.Drawing.Point(247, 19)
        Me.tbPCCUnderspeed.Name = "tbPCCUnderspeed"
        Me.tbPCCUnderspeed.Size = New System.Drawing.Size(52, 20)
        Me.tbPCCUnderspeed.TabIndex = 12
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
        Me.gbEcoRoll.Location = New System.Drawing.Point(7, 136)
        Me.gbEcoRoll.Name = "gbEcoRoll"
        Me.gbEcoRoll.Size = New System.Drawing.Size(515, 127)
        Me.gbEcoRoll.TabIndex = 6
        Me.gbEcoRoll.TabStop = false
        Me.gbEcoRoll.Text = "Eco-Roll"
        '
        'Label44
        '
        Me.Label44.AutoSize = true
        Me.Label44.Location = New System.Drawing.Point(305, 75)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(34, 13)
        Me.Label44.TabIndex = 19
        Me.Label44.Text = "[m/s²]"
        '
        'Label45
        '
        Me.Label45.AutoSize = true
        Me.Label45.Location = New System.Drawing.Point(7, 75)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(125, 13)
        Me.Label45.TabIndex = 17
        Me.Label45.Text = "Upper Acceleration Limit:"
        '
        'tbEcoRollMaxAcc
        '
        Me.tbEcoRollMaxAcc.Location = New System.Drawing.Point(247, 72)
        Me.tbEcoRollMaxAcc.Name = "tbEcoRollMaxAcc"
        Me.tbEcoRollMaxAcc.Size = New System.Drawing.Size(52, 20)
        Me.tbEcoRollMaxAcc.TabIndex = 18
        '
        'Label29
        '
        Me.Label29.AutoSize = true
        Me.Label29.Location = New System.Drawing.Point(305, 100)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(38, 13)
        Me.Label29.TabIndex = 16
        Me.Label29.Text = "[km/h]"
        Me.Label29.Visible = false
        '
        'Label30
        '
        Me.Label30.AutoSize = true
        Me.Label30.Location = New System.Drawing.Point(7, 100)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(114, 13)
        Me.Label30.TabIndex = 14
        Me.Label30.Text = "Underspeed threshold:"
        Me.Label30.Visible = false
        '
        'tbEcoRollUnderspeed
        '
        Me.tbEcoRollUnderspeed.Location = New System.Drawing.Point(247, 97)
        Me.tbEcoRollUnderspeed.Name = "tbEcoRollUnderspeed"
        Me.tbEcoRollUnderspeed.Size = New System.Drawing.Size(52, 20)
        Me.tbEcoRollUnderspeed.TabIndex = 15
        Me.tbEcoRollUnderspeed.Visible = false
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(305, 48)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(18, 13)
        Me.Label27.TabIndex = 13
        Me.Label27.Text = "[s]"
        '
        'Label28
        '
        Me.Label28.AutoSize = true
        Me.Label28.Location = New System.Drawing.Point(7, 48)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(85, 13)
        Me.Label28.TabIndex = 11
        Me.Label28.Text = "Activation delay:"
        '
        'tbEcoRollActivationDelay
        '
        Me.tbEcoRollActivationDelay.Location = New System.Drawing.Point(247, 45)
        Me.tbEcoRollActivationDelay.Name = "tbEcoRollActivationDelay"
        Me.tbEcoRollActivationDelay.Size = New System.Drawing.Size(52, 20)
        Me.tbEcoRollActivationDelay.TabIndex = 12
        '
        'Label25
        '
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(305, 22)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(38, 13)
        Me.Label25.TabIndex = 10
        Me.Label25.Text = "[km/h]"
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(7, 22)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(83, 13)
        Me.Label26.TabIndex = 8
        Me.Label26.Text = "Minimum speed:"
        '
        'tbEcoRollMinSpeed
        '
        Me.tbEcoRollMinSpeed.Location = New System.Drawing.Point(247, 19)
        Me.tbEcoRollMinSpeed.Name = "tbEcoRollMinSpeed"
        Me.tbEcoRollMinSpeed.Size = New System.Drawing.Size(52, 20)
        Me.tbEcoRollMinSpeed.TabIndex = 9
        '
        'gbEngineStopStart
        '
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
        Me.gbEngineStopStart.Location = New System.Drawing.Point(7, 9)
        Me.gbEngineStopStart.Name = "gbEngineStopStart"
        Me.gbEngineStopStart.Size = New System.Drawing.Size(515, 121)
        Me.gbEngineStopStart.TabIndex = 5
        Me.gbEngineStopStart.TabStop = false
        Me.gbEngineStopStart.Text = "Engine Stop/Start"
        '
        'tbESSUtilityFactorDriving
        '
        Me.tbESSUtilityFactorDriving.Location = New System.Drawing.Point(247, 95)
        Me.tbESSUtilityFactorDriving.Name = "tbESSUtilityFactorDriving"
        Me.tbESSUtilityFactorDriving.Size = New System.Drawing.Size(52, 20)
        Me.tbESSUtilityFactorDriving.TabIndex = 9
        '
        'lblESSUtilityFactorDriving
        '
        Me.lblESSUtilityFactorDriving.AutoSize = true
        Me.lblESSUtilityFactorDriving.Location = New System.Drawing.Point(7, 98)
        Me.lblESSUtilityFactorDriving.Name = "lblESSUtilityFactorDriving"
        Me.lblESSUtilityFactorDriving.Size = New System.Drawing.Size(208, 13)
        Me.lblESSUtilityFactorDriving.TabIndex = 8
        Me.lblESSUtilityFactorDriving.Text = "Engie stop/start utility factor (while driving):"
        '
        'tbEssUtility
        '
        Me.tbEssUtility.Location = New System.Drawing.Point(247, 69)
        Me.tbEssUtility.Name = "tbEssUtility"
        Me.tbEssUtility.Size = New System.Drawing.Size(52, 20)
        Me.tbEssUtility.TabIndex = 7
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(7, 72)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(207, 13)
        Me.Label24.TabIndex = 6
        Me.Label24.Text = "Engie stop/start utility factor (vehicle stop):"
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(305, 46)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(18, 13)
        Me.Label18.TabIndex = 5
        Me.Label18.Text = "[s]"
        '
        'tbMaxEngineOffTimespan
        '
        Me.tbMaxEngineOffTimespan.Location = New System.Drawing.Point(247, 43)
        Me.tbMaxEngineOffTimespan.Name = "tbMaxEngineOffTimespan"
        Me.tbMaxEngineOffTimespan.Size = New System.Drawing.Size(52, 20)
        Me.tbMaxEngineOffTimespan.TabIndex = 4
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(7, 46)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(105, 13)
        Me.Label19.TabIndex = 3
        Me.Label19.Text = "Max. engine-off time:"
        '
        'Label17
        '
        Me.Label17.AutoSize = true
        Me.Label17.Location = New System.Drawing.Point(305, 20)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(18, 13)
        Me.Label17.TabIndex = 2
        Me.Label17.Text = "[s]"
        '
        'tbEngineStopStartActivationDelay
        '
        Me.tbEngineStopStartActivationDelay.Location = New System.Drawing.Point(247, 17)
        Me.tbEngineStopStartActivationDelay.Name = "tbEngineStopStartActivationDelay"
        Me.tbEngineStopStartActivationDelay.Size = New System.Drawing.Size(52, 20)
        Me.tbEngineStopStartActivationDelay.TabIndex = 1
        '
        'Label16
        '
        Me.Label16.AutoSize = true
        Me.Label16.Location = New System.Drawing.Point(7, 20)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(87, 13)
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
        Me.GroupBox1.TabStop = false
        '
        'GroupBox2
        '
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(200, 100)
        Me.GroupBox2.TabIndex = 0
        Me.GroupBox2.TabStop = false
        '
        'TbUnderSpeed
        '
        Me.TbUnderSpeed.Location = New System.Drawing.Point(0, 0)
        Me.TbUnderSpeed.Name = "TbUnderSpeed"
        Me.TbUnderSpeed.Size = New System.Drawing.Size(100, 20)
        Me.TbUnderSpeed.TabIndex = 0
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelGEN})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 599)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(944, 32)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelGEN
        '
        Me.ToolStripStatusLabelGEN.Name = "ToolStripStatusLabelGEN"
        Me.ToolStripStatusLabelGEN.Size = New System.Drawing.Size(180, 25)
        Me.ToolStripStatusLabelGEN.Text = "ToolStripStatusLabel1"
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(776, 563)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 0
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(857, 563)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 1
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator1, Me.ToolStripBtSendTo, Me.ToolStripSeparator2, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(944, 33)
        Me.ToolStrip1.TabIndex = 20
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtNew.Text = "New"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtOpen.Text = "Open"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSave.Text = "Save"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSaveAs.Text = "Save As"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 33)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSendTo.Text = "Send to Job List"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job List"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 33)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripButton1.Text = "Help"
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VECTO
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(942, 40)
        Me.PictureBox1.TabIndex = 21
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = false
        Me.CmOpenFile.Size = New System.Drawing.Size(178, 68)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(177, 32)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(177, 32)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(4, 3)
        Me.PicVehicle.Name = "PicVehicle"
        Me.PicVehicle.Size = New System.Drawing.Size(300, 88)
        Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicVehicle.TabIndex = 36
        Me.PicVehicle.TabStop = false
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(4, 144)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(390, 296)
        Me.PicBox.TabIndex = 36
        Me.PicBox.TabStop = false
        '
        'TbEngTxt
        '
        Me.TbEngTxt.Location = New System.Drawing.Point(4, 93)
        Me.TbEngTxt.Name = "TbEngTxt"
        Me.TbEngTxt.ReadOnly = true
        Me.TbEngTxt.Size = New System.Drawing.Size(390, 20)
        Me.TbEngTxt.TabIndex = 6
        '
        'TbVehCat
        '
        Me.TbVehCat.Location = New System.Drawing.Point(307, 3)
        Me.TbVehCat.Name = "TbVehCat"
        Me.TbVehCat.ReadOnly = true
        Me.TbVehCat.Size = New System.Drawing.Size(87, 20)
        Me.TbVehCat.TabIndex = 2
        '
        'TbAxleConf
        '
        Me.TbAxleConf.Location = New System.Drawing.Point(363, 29)
        Me.TbAxleConf.Name = "TbAxleConf"
        Me.TbAxleConf.ReadOnly = true
        Me.TbAxleConf.Size = New System.Drawing.Size(31, 20)
        Me.TbAxleConf.TabIndex = 4
        '
        'TbHVCclass
        '
        Me.TbHVCclass.Location = New System.Drawing.Point(307, 66)
        Me.TbHVCclass.Name = "TbHVCclass"
        Me.TbHVCclass.ReadOnly = true
        Me.TbHVCclass.Size = New System.Drawing.Size(87, 20)
        Me.TbHVCclass.TabIndex = 5
        '
        'TbGbxTxt
        '
        Me.TbGbxTxt.Location = New System.Drawing.Point(4, 118)
        Me.TbGbxTxt.Name = "TbGbxTxt"
        Me.TbGbxTxt.ReadOnly = true
        Me.TbGbxTxt.Size = New System.Drawing.Size(390, 20)
        Me.TbGbxTxt.TabIndex = 7
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(307, 29)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.ReadOnly = true
        Me.TbMass.Size = New System.Drawing.Size(50, 20)
        Me.TbMass.TabIndex = 3
        '
        'lblEngineCharacteristics
        '
        Me.lblEngineCharacteristics.AutoSize = true
        Me.lblEngineCharacteristics.Location = New System.Drawing.Point(7, 445)
        Me.lblEngineCharacteristics.Name = "lblEngineCharacteristics"
        Me.lblEngineCharacteristics.Size = New System.Drawing.Size(0, 13)
        Me.lblEngineCharacteristics.TabIndex = 37
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(114, 32)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(477, 40)
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
        Me.pnJobInfo.Location = New System.Drawing.Point(532, 78)
        Me.pnJobInfo.Name = "pnJobInfo"
        Me.pnJobInfo.Size = New System.Drawing.Size(397, 471)
        Me.pnJobInfo.TabIndex = 39
        '
        'VectoJobForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(944, 631)
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
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "VectoJobForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Job Editor"
        Me.TabPgGen.ResumeLayout(false)
        Me.pnHybridStrategy.ResumeLayout(false)
        Me.pnHybridStrategy.PerformLayout
        Me.pnVehicle.ResumeLayout(false)
        Me.pnVehicle.PerformLayout
        Me.pnEngine.ResumeLayout(false)
        Me.pnEngine.PerformLayout
        Me.pnGearbox.ResumeLayout(false)
        Me.pnGearbox.PerformLayout
        Me.pnShiftParams.ResumeLayout(false)
        Me.pnShiftParams.PerformLayout
        Me.GrCycles.ResumeLayout(false)
        Me.GrCycles.PerformLayout
        Me.GrAuxMech.ResumeLayout(false)
        Me.pnAuxDeclarationMode.ResumeLayout(false)
        Me.pnAuxDeclarationMode.PerformLayout
        Me.pnAuxEngineering.ResumeLayout(false)
        Me.pnAuxEngineering.PerformLayout
        CType(Me.picAuxInfo,System.ComponentModel.ISupportInitialize).EndInit
        Me.tcJob.ResumeLayout(false)
        Me.tpAuxiliaries.ResumeLayout(false)
        Me.gbBusAux.ResumeLayout(false)
        Me.gbBusAux.PerformLayout
        Me.pnBusAux.ResumeLayout(false)
        Me.pnBusAux.PerformLayout
        Me.gbElectricAux.ResumeLayout(false)
        Me.gbElectricAux.PerformLayout
        Me.tpCycles.ResumeLayout(false)
        Me.TabPgDriver.ResumeLayout(false)
        Me.gbShiftStrategy.ResumeLayout(false)
        Me.GrVACC.ResumeLayout(false)
        Me.GrVACC.PerformLayout
        Me.GrLAC.ResumeLayout(false)
        Me.GrLAC.PerformLayout
        Me.pnLookAheadCoasting.ResumeLayout(false)
        Me.pnLookAheadCoasting.PerformLayout
        Me.gbOverspeed.ResumeLayout(false)
        Me.gbOverspeed.PerformLayout
        Me.PnEcoRoll.ResumeLayout(false)
        Me.PnEcoRoll.PerformLayout
        Me.TabPgADAS.ResumeLayout(false)
        Me.gbPCC.ResumeLayout(false)
        Me.gbPCC.PerformLayout
        Me.gbEcoRoll.ResumeLayout(false)
        Me.gbEcoRoll.PerformLayout
        Me.gbEngineStopStart.ResumeLayout(false)
        Me.gbEngineStopStart.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.pnJobInfo.ResumeLayout(false)
        Me.pnJobInfo.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

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
    Friend WithEvents gbShiftStrategy As GroupBox
    Friend WithEvents cbGearshiftStrategy As ComboBox
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

End Class
