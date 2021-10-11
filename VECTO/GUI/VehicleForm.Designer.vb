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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VehicleForm
	Inherits System.Windows.Forms.Form

	'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
	<System.Diagnostics.DebuggerNonUserCode()> _
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
	Private components As System.ComponentModel.IContainer

	'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VehicleForm))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TbMass = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbLoad = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBcdA = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TBrdyn = New System.Windows.Forms.TextBox()
        Me.ButOK = New System.Windows.Forms.Button()
        Me.ButCancel = New System.Windows.Forms.Button()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.CbCdMode = New System.Windows.Forms.ComboBox()
        Me.TbCdFile = New System.Windows.Forms.TextBox()
        Me.BtCdFileBrowse = New System.Windows.Forms.Button()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.BtCdFileOpen = New System.Windows.Forms.Button()
        Me.LbCdMode = New System.Windows.Forms.Label()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.PnRt = New System.Windows.Forms.Panel()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.BtRtBrowse = New System.Windows.Forms.Button()
        Me.TbRtPath = New System.Windows.Forms.TextBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.LbRtRatio = New System.Windows.Forms.Label()
        Me.TbRtRatio = New System.Windows.Forms.TextBox()
        Me.CbRtType = New System.Windows.Forms.ComboBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.TbMassExtra = New System.Windows.Forms.TextBox()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ButAxlRem = New System.Windows.Forms.Button()
        Me.LvRRC = New System.Windows.Forms.ListView()
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ButAxlAdd = New System.Windows.Forms.Button()
        Me.PnWheelDiam = New System.Windows.Forms.Panel()
        Me.CbAxleConfig = New System.Windows.Forms.ComboBox()
        Me.CbCat = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TbMassMass = New System.Windows.Forms.TextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TbHDVclass = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.PnLoad = New System.Windows.Forms.Panel()
        Me.GrAirRes = New System.Windows.Forms.GroupBox()
        Me.PnCdATrTr = New System.Windows.Forms.Panel()
        Me.tbVehicleHeight = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.pnAngledriveFields = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.tbAngledriveRatio = New System.Windows.Forms.TextBox()
        Me.btAngledriveLossMapBrowse = New System.Windows.Forms.Button()
        Me.tbAngledriveLossMapPath = New System.Windows.Forms.TextBox()
        Me.cbAngledriveType = New System.Windows.Forms.ComboBox()
        Me.PicVehicle = New System.Windows.Forms.PictureBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cbPTOType = New System.Windows.Forms.ComboBox()
        Me.tbPTOCycle = New System.Windows.Forms.TextBox()
        Me.tbPTOLossMap = New System.Windows.Forms.TextBox()
        Me.tbPTODrive = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.tbVehIdlingSpeed = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.cbTankSystem = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.lvTorqueLimits = New System.Windows.Forms.ListView()
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.Label17 = New System.Windows.Forms.Label()
        Me.btDelMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.btAddMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.tpADAS = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.cbPcc = New System.Windows.Forms.ComboBox()
        Me.cbEcoRoll = New System.Windows.Forms.ComboBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.cbEngineStopStart = New System.Windows.Forms.CheckBox()
        Me.lblPCC = New System.Windows.Forms.Label()
        Me.tpRoadSweeper = New System.Windows.Forms.TabPage()
        Me.gbPTODrive = New System.Windows.Forms.GroupBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.tbPtoGear = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.tbPtoEngineSpeed = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.pnPTO = New System.Windows.Forms.Panel()
        Me.btPTOCycleDrive = New System.Windows.Forms.Button()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.btPTOCycle = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btPTOLossMapBrowse = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.gbPTO = New System.Windows.Forms.GroupBox()
        Me.cbLegislativeClass = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.cbAtEcoRollReleaseLockupClutch = New System.Windows.Forms.CheckBox()
        Me.GroupBox6.SuspendLayout
        Me.ToolStrip1.SuspendLayout
        Me.GroupBox7.SuspendLayout
        Me.PnRt.SuspendLayout
        Me.GroupBox8.SuspendLayout
        Me.PnWheelDiam.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.PnLoad.SuspendLayout
        Me.GrAirRes.SuspendLayout
        Me.PnCdATrTr.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.GroupBox2.SuspendLayout
        Me.pnAngledriveFields.SuspendLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).BeginInit
        Me.TabControl1.SuspendLayout
        Me.TabPage1.SuspendLayout
        Me.GroupBox4.SuspendLayout
        Me.Panel1.SuspendLayout
        Me.TabPage2.SuspendLayout
        Me.GroupBox9.SuspendLayout
        Me.TabPage3.SuspendLayout
        Me.tpADAS.SuspendLayout
        Me.GroupBox5.SuspendLayout
        Me.tpRoadSweeper.SuspendLayout
        Me.gbPTODrive.SuspendLayout
        Me.pnPTO.SuspendLayout
        Me.gbPTO.SuspendLayout
        Me.SuspendLayout
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(177, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Corrected Actual Curb Mass Vehicle"
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(188, 19)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.Size = New System.Drawing.Size(57, 20)
        Me.TbMass.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(128, 31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Loading"
        '
        'TbLoad
        '
        Me.TbLoad.Location = New System.Drawing.Point(182, 28)
        Me.TbLoad.Name = "TbLoad"
        Me.TbLoad.Size = New System.Drawing.Size(57, 20)
        Me.TbLoad.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(3, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Cd x A"
        '
        'TBcdA
        '
        Me.TBcdA.Location = New System.Drawing.Point(46, 3)
        Me.TBcdA.Name = "TBcdA"
        Me.TBcdA.Size = New System.Drawing.Size(57, 20)
        Me.TBcdA.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(3, 7)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(40, 13)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Radius"
        '
        'TBrdyn
        '
        Me.TBrdyn.Location = New System.Drawing.Point(46, 4)
        Me.TBrdyn.Name = "TBrdyn"
        Me.TBrdyn.Size = New System.Drawing.Size(57, 20)
        Me.TBrdyn.TabIndex = 0
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(431, 560)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 5
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(512, 560)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 6
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(247, 22)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(25, 13)
        Me.Label14.TabIndex = 24
        Me.Label14.Text = "[kg]"
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(241, 31)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(25, 13)
        Me.Label31.TabIndex = 24
        Me.Label31.Text = "[kg]"
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(105, 7)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(29, 13)
        Me.Label35.TabIndex = 24
        Me.Label35.Text = "[mm]"
        '
        'CbCdMode
        '
        Me.CbCdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCdMode.FormattingEnabled = true
        Me.CbCdMode.Items.AddRange(New Object() {"No Correction", "Speed dependent (User-defined)", "Speed dependent (Declaration Mode)", "Vair & Beta Input"})
        Me.CbCdMode.Location = New System.Drawing.Point(6, 19)
        Me.CbCdMode.Name = "CbCdMode"
        Me.CbCdMode.Size = New System.Drawing.Size(267, 21)
        Me.CbCdMode.TabIndex = 0
        '
        'TbCdFile
        '
        Me.TbCdFile.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TbCdFile.Enabled = false
        Me.TbCdFile.Location = New System.Drawing.Point(9, 65)
        Me.TbCdFile.Name = "TbCdFile"
        Me.TbCdFile.Size = New System.Drawing.Size(210, 20)
        Me.TbCdFile.TabIndex = 1
        '
        'BtCdFileBrowse
        '
        Me.BtCdFileBrowse.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.BtCdFileBrowse.Enabled = false
        Me.BtCdFileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtCdFileBrowse.Location = New System.Drawing.Point(225, 62)
        Me.BtCdFileBrowse.Name = "BtCdFileBrowse"
        Me.BtCdFileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtCdFileBrowse.TabIndex = 2
        Me.BtCdFileBrowse.UseVisualStyleBackColor = true
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.BtCdFileOpen)
        Me.GroupBox6.Controls.Add(Me.LbCdMode)
        Me.GroupBox6.Controls.Add(Me.CbCdMode)
        Me.GroupBox6.Controls.Add(Me.BtCdFileBrowse)
        Me.GroupBox6.Controls.Add(Me.TbCdFile)
        Me.GroupBox6.Location = New System.Drawing.Point(290, 84)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(281, 96)
        Me.GroupBox6.TabIndex = 5
        Me.GroupBox6.TabStop = false
        Me.GroupBox6.Text = "Cross Wind Correction"
        '
        'BtCdFileOpen
        '
        Me.BtCdFileOpen.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.BtCdFileOpen.Enabled = false
        Me.BtCdFileOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtCdFileOpen.Location = New System.Drawing.Point(249, 62)
        Me.BtCdFileOpen.Name = "BtCdFileOpen"
        Me.BtCdFileOpen.Size = New System.Drawing.Size(24, 24)
        Me.BtCdFileOpen.TabIndex = 3
        Me.BtCdFileOpen.TabStop = false
        Me.BtCdFileOpen.UseVisualStyleBackColor = true
        '
        'LbCdMode
        '
        Me.LbCdMode.AutoSize = true
        Me.LbCdMode.Location = New System.Drawing.Point(6, 47)
        Me.LbCdMode.Name = "LbCdMode"
        Me.LbCdMode.Size = New System.Drawing.Size(59, 13)
        Me.LbCdMode.TabIndex = 28
        Me.LbCdMode.Text = "LbCdMode"
        Me.LbCdMode.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(599, 25)
        Me.ToolStrip1.TabIndex = 29
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtSave.Text = "ToolStripButton1"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtSendTo.Text = "Send to Job Editor"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "Help"
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.PnRt)
        Me.GroupBox7.Controls.Add(Me.CbRtType)
        Me.GroupBox7.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(293, 111)
        Me.GroupBox7.TabIndex = 3
        Me.GroupBox7.TabStop = false
        Me.GroupBox7.Text = "Retarder Losses"
        '
        'PnRt
        '
        Me.PnRt.Controls.Add(Me.Label15)
        Me.PnRt.Controls.Add(Me.BtRtBrowse)
        Me.PnRt.Controls.Add(Me.TbRtPath)
        Me.PnRt.Controls.Add(Me.Label45)
        Me.PnRt.Controls.Add(Me.LbRtRatio)
        Me.PnRt.Controls.Add(Me.TbRtRatio)
        Me.PnRt.Location = New System.Drawing.Point(3, 42)
        Me.PnRt.Name = "PnRt"
        Me.PnRt.Size = New System.Drawing.Size(272, 63)
        Me.PnRt.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.Location = New System.Drawing.Point(6, 23)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(201, 16)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Retarder Loss Map"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'BtRtBrowse
        '
        Me.BtRtBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.BtRtBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtRtBrowse.Location = New System.Drawing.Point(245, 39)
        Me.BtRtBrowse.Name = "BtRtBrowse"
        Me.BtRtBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtRtBrowse.TabIndex = 14
        Me.BtRtBrowse.UseVisualStyleBackColor = true
        '
        'TbRtPath
        '
        Me.TbRtPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.TbRtPath.Location = New System.Drawing.Point(6, 41)
        Me.TbRtPath.Name = "TbRtPath"
        Me.TbRtPath.Size = New System.Drawing.Size(239, 20)
        Me.TbRtPath.TabIndex = 13
        '
        'Label45
        '
        Me.Label45.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label45.AutoSize = true
        Me.Label45.Location = New System.Drawing.Point(251, 4)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(16, 13)
        Me.Label45.TabIndex = 10
        Me.Label45.Text = "[-]"
        '
        'LbRtRatio
        '
        Me.LbRtRatio.Location = New System.Drawing.Point(19, 4)
        Me.LbRtRatio.Name = "LbRtRatio"
        Me.LbRtRatio.Size = New System.Drawing.Size(167, 17)
        Me.LbRtRatio.TabIndex = 5
        Me.LbRtRatio.Text = "Ratio"
        Me.LbRtRatio.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TbRtRatio
        '
        Me.TbRtRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbRtRatio.Location = New System.Drawing.Point(193, 2)
        Me.TbRtRatio.Name = "TbRtRatio"
        Me.TbRtRatio.Size = New System.Drawing.Size(56, 20)
        Me.TbRtRatio.TabIndex = 0
        '
        'CbRtType
        '
        Me.CbRtType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbRtType.FormattingEnabled = true
        Me.CbRtType.Items.AddRange(New Object() {"Included in Transmission Loss Maps", "Primary Retarder", "Secondary Retarder"})
        Me.CbRtType.Location = New System.Drawing.Point(6, 19)
        Me.CbRtType.Name = "CbRtType"
        Me.CbRtType.Size = New System.Drawing.Size(266, 21)
        Me.CbRtType.TabIndex = 0
        '
        'Label46
        '
        Me.Label46.AutoSize = true
        Me.Label46.Location = New System.Drawing.Point(32, 5)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(145, 13)
        Me.Label46.TabIndex = 31
        Me.Label46.Text = "Curb Mass Extra Trailer/Body"
        '
        'Label50
        '
        Me.Label50.AutoSize = true
        Me.Label50.Location = New System.Drawing.Point(241, 5)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(25, 13)
        Me.Label50.TabIndex = 24
        Me.Label50.Text = "[kg]"
        '
        'TbMassExtra
        '
        Me.TbMassExtra.Location = New System.Drawing.Point(182, 2)
        Me.TbMassExtra.Name = "TbMassExtra"
        Me.TbMassExtra.Size = New System.Drawing.Size(57, 20)
        Me.TbMassExtra.TabIndex = 0
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label6)
        Me.GroupBox8.Controls.Add(Me.ButAxlRem)
        Me.GroupBox8.Controls.Add(Me.LvRRC)
        Me.GroupBox8.Controls.Add(Me.ButAxlAdd)
        Me.GroupBox8.Location = New System.Drawing.Point(6, 186)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(564, 151)
        Me.GroupBox8.TabIndex = 2
        Me.GroupBox8.TabStop = false
        Me.GroupBox8.Text = "Axles / Wheels"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(450, 121)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(106, 13)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "(Double-Click to Edit)"
        '
        'ButAxlRem
        '
        Me.ButAxlRem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.ButAxlRem.Location = New System.Drawing.Point(29, 122)
        Me.ButAxlRem.Name = "ButAxlRem"
        Me.ButAxlRem.Size = New System.Drawing.Size(24, 24)
        Me.ButAxlRem.TabIndex = 2
        Me.ButAxlRem.UseVisualStyleBackColor = true
        '
        'LvRRC
        '
        Me.LvRRC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvRRC.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader2, Me.ColumnHeader9, Me.ColumnHeader1, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader10})
        Me.LvRRC.FullRowSelect = true
        Me.LvRRC.GridLines = true
        Me.LvRRC.HideSelection = false
        Me.LvRRC.Location = New System.Drawing.Point(6, 19)
        Me.LvRRC.MultiSelect = false
        Me.LvRRC.Name = "LvRRC"
        Me.LvRRC.Size = New System.Drawing.Size(552, 102)
        Me.LvRRC.TabIndex = 0
        Me.LvRRC.TabStop = false
        Me.LvRRC.UseCompatibleStateImageBehavior = false
        Me.LvRRC.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "#"
        Me.ColumnHeader7.Width = 22
        '
        'ColumnHeader8
        '
        Me.ColumnHeader8.Text = "Rel. load"
        Me.ColumnHeader8.Width = 62
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Twin T."
        Me.ColumnHeader2.Width = 51
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "RRC"
        Me.ColumnHeader9.Width = 59
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Fz ISO"
        Me.ColumnHeader1.Width = 55
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Wheels"
        Me.ColumnHeader3.Width = 100
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Inertia"
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Axle Type"
        Me.ColumnHeader10.Width = 130
        '
        'ButAxlAdd
        '
        Me.ButAxlAdd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.ButAxlAdd.Location = New System.Drawing.Point(5, 122)
        Me.ButAxlAdd.Name = "ButAxlAdd"
        Me.ButAxlAdd.Size = New System.Drawing.Size(24, 24)
        Me.ButAxlAdd.TabIndex = 1
        Me.ButAxlAdd.UseVisualStyleBackColor = true
        '
        'PnWheelDiam
        '
        Me.PnWheelDiam.Controls.Add(Me.Label13)
        Me.PnWheelDiam.Controls.Add(Me.TBrdyn)
        Me.PnWheelDiam.Controls.Add(Me.Label35)
        Me.PnWheelDiam.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnWheelDiam.Location = New System.Drawing.Point(3, 16)
        Me.PnWheelDiam.Name = "PnWheelDiam"
        Me.PnWheelDiam.Size = New System.Drawing.Size(132, 53)
        Me.PnWheelDiam.TabIndex = 5
        '
        'CbAxleConfig
        '
        Me.CbAxleConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbAxleConfig.FormattingEnabled = true
        Me.CbAxleConfig.Items.AddRange(New Object() {"-", "4x2", "4x4", "6x2", "6x4", "6x6", "8x2", "8x4", "8x6", "8x8"})
        Me.CbAxleConfig.Location = New System.Drawing.Point(153, 80)
        Me.CbAxleConfig.Name = "CbAxleConfig"
        Me.CbAxleConfig.Size = New System.Drawing.Size(60, 21)
        Me.CbAxleConfig.TabIndex = 1
        '
        'CbCat
        '
        Me.CbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCat.FormattingEnabled = true
        Me.CbCat.Items.AddRange(New Object() {"-", "Rigid Truck", "Tractor", "City Bus", "Interurban Bus", "Coach"})
        Me.CbCat.Location = New System.Drawing.Point(12, 80)
        Me.CbCat.Name = "CbCat"
        Me.CbCat.Size = New System.Drawing.Size(135, 21)
        Me.CbCat.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(31, 108)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(116, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Technically Permissible"
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(197, 114)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(16, 13)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "[t]"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TbMassMass
        '
        Me.TbMassMass.Location = New System.Drawing.Point(153, 111)
        Me.TbMassMass.Name = "TbMassMass"
        Me.TbMassMass.Size = New System.Drawing.Size(42, 20)
        Me.TbMassMass.TabIndex = 2
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 586)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(599, 22)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 36
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(39, 17)
        Me.LbStatus.Text = "Status"
        '
        'TbHDVclass
        '
        Me.TbHDVclass.Location = New System.Drawing.Point(153, 141)
        Me.TbHDVclass.Name = "TbHDVclass"
        Me.TbHDVclass.ReadOnly = true
        Me.TbHDVclass.Size = New System.Drawing.Size(42, 20)
        Me.TbHDVclass.TabIndex = 3
        Me.TbHDVclass.TabStop = false
        Me.TbHDVclass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PnLoad)
        Me.GroupBox1.Controls.Add(Me.TbMass)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(278, 118)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Masses / Loading"
        '
        'PnLoad
        '
        Me.PnLoad.Controls.Add(Me.Label2)
        Me.PnLoad.Controls.Add(Me.Label31)
        Me.PnLoad.Controls.Add(Me.TbLoad)
        Me.PnLoad.Controls.Add(Me.TbMassExtra)
        Me.PnLoad.Controls.Add(Me.Label50)
        Me.PnLoad.Controls.Add(Me.Label46)
        Me.PnLoad.Location = New System.Drawing.Point(6, 43)
        Me.PnLoad.Name = "PnLoad"
        Me.PnLoad.Size = New System.Drawing.Size(269, 58)
        Me.PnLoad.TabIndex = 1
        '
        'GrAirRes
        '
        Me.GrAirRes.Controls.Add(Me.PnCdATrTr)
        Me.GrAirRes.Location = New System.Drawing.Point(290, 6)
        Me.GrAirRes.Name = "GrAirRes"
        Me.GrAirRes.Size = New System.Drawing.Size(137, 72)
        Me.GrAirRes.TabIndex = 1
        Me.GrAirRes.TabStop = false
        Me.GrAirRes.Text = "Air Resistance"
        '
        'PnCdATrTr
        '
        Me.PnCdATrTr.Controls.Add(Me.tbVehicleHeight)
        Me.PnCdATrTr.Controls.Add(Me.Label11)
        Me.PnCdATrTr.Controls.Add(Me.Label20)
        Me.PnCdATrTr.Controls.Add(Me.TBcdA)
        Me.PnCdATrTr.Controls.Add(Me.Label38)
        Me.PnCdATrTr.Controls.Add(Me.Label3)
        Me.PnCdATrTr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnCdATrTr.Location = New System.Drawing.Point(3, 16)
        Me.PnCdATrTr.Name = "PnCdATrTr"
        Me.PnCdATrTr.Size = New System.Drawing.Size(131, 53)
        Me.PnCdATrTr.TabIndex = 0
        '
        'tbVehicleHeight
        '
        Me.tbVehicleHeight.Location = New System.Drawing.Point(46, 29)
        Me.tbVehicleHeight.Name = "tbVehicleHeight"
        Me.tbVehicleHeight.Size = New System.Drawing.Size(57, 20)
        Me.tbVehicleHeight.TabIndex = 25
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(105, 32)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(21, 13)
        Me.Label11.TabIndex = 27
        Me.Label11.Text = "[m]"
        '
        'Label20
        '
        Me.Label20.AutoSize = true
        Me.Label20.Location = New System.Drawing.Point(3, 32)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(38, 13)
        Me.Label20.TabIndex = 26
        Me.Label20.Text = "Height"
        '
        'Label38
        '
        Me.Label38.AutoSize = true
        Me.Label38.Location = New System.Drawing.Point(105, 6)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(24, 13)
        Me.Label38.TabIndex = 24
        Me.Label38.Text = "[m²]"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VEH
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(604, 40)
        Me.PictureBox1.TabIndex = 37
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = false
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
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.PnWheelDiam)
        Me.GroupBox3.Location = New System.Drawing.Point(433, 6)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(138, 72)
        Me.GroupBox3.TabIndex = 6
        Me.GroupBox3.TabStop = false
        Me.GroupBox3.Text = "Dynamic Tyre Radius"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.pnAngledriveFields)
        Me.GroupBox2.Controls.Add(Me.cbAngledriveType)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 123)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(293, 111)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Angledrive"
        '
        'pnAngledriveFields
        '
        Me.pnAngledriveFields.Controls.Add(Me.Label4)
        Me.pnAngledriveFields.Controls.Add(Me.Label12)
        Me.pnAngledriveFields.Controls.Add(Me.Label10)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveRatio)
        Me.pnAngledriveFields.Controls.Add(Me.btAngledriveLossMapBrowse)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveLossMapPath)
        Me.pnAngledriveFields.Location = New System.Drawing.Point(3, 42)
        Me.pnAngledriveFields.Name = "pnAngledriveFields"
        Me.pnAngledriveFields.Size = New System.Drawing.Size(272, 63)
        Me.pnAngledriveFields.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(251, 4)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(16, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "[-]"
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(6, 23)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(263, 16)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = "Transmission Loss Map or Efficiency Value [0..1]"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(144, 4)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(44, 18)
        Me.Label10.TabIndex = 15
        Me.Label10.Text = "Ratio"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbAngledriveRatio
        '
        Me.tbAngledriveRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveRatio.Location = New System.Drawing.Point(193, 2)
        Me.tbAngledriveRatio.Name = "tbAngledriveRatio"
        Me.tbAngledriveRatio.Size = New System.Drawing.Size(56, 20)
        Me.tbAngledriveRatio.TabIndex = 12
        '
        'btAngledriveLossMapBrowse
        '
        Me.btAngledriveLossMapBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btAngledriveLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btAngledriveLossMapBrowse.Location = New System.Drawing.Point(245, 39)
        Me.btAngledriveLossMapBrowse.Name = "btAngledriveLossMapBrowse"
        Me.btAngledriveLossMapBrowse.Size = New System.Drawing.Size(24, 24)
        Me.btAngledriveLossMapBrowse.TabIndex = 14
        Me.btAngledriveLossMapBrowse.UseVisualStyleBackColor = true
        '
        'tbAngledriveLossMapPath
        '
        Me.tbAngledriveLossMapPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveLossMapPath.Location = New System.Drawing.Point(6, 41)
        Me.tbAngledriveLossMapPath.Name = "tbAngledriveLossMapPath"
        Me.tbAngledriveLossMapPath.Size = New System.Drawing.Size(239, 20)
        Me.tbAngledriveLossMapPath.TabIndex = 13
        '
        'cbAngledriveType
        '
        Me.cbAngledriveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAngledriveType.FormattingEnabled = true
        Me.cbAngledriveType.Location = New System.Drawing.Point(6, 19)
        Me.cbAngledriveType.Name = "cbAngledriveType"
        Me.cbAngledriveType.Size = New System.Drawing.Size(266, 21)
        Me.cbAngledriveType.TabIndex = 0
        '
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(281, 70)
        Me.PicVehicle.Name = "PicVehicle"
        Me.PicVehicle.Size = New System.Drawing.Size(300, 88)
        Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicVehicle.TabIndex = 39
        Me.PicVehicle.TabStop = false
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(85, 144)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(62, 13)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "HDV Group"
        '
        'cbPTOType
        '
        Me.cbPTOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPTOType.Location = New System.Drawing.Point(6, 17)
        Me.cbPTOType.Name = "cbPTOType"
        Me.cbPTOType.Size = New System.Drawing.Size(550, 21)
        Me.cbPTOType.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.cbPTOType, "Transmission type to the PTO consumer")
        '
        'tbPTOCycle
        '
        Me.tbPTOCycle.Location = New System.Drawing.Point(6, 71)
        Me.tbPTOCycle.Name = "tbPTOCycle"
        Me.tbPTOCycle.Size = New System.Drawing.Size(239, 20)
        Me.tbPTOCycle.TabIndex = 16
        Me.ToolTip1.SetToolTip(Me.tbPTOCycle, "PTO Consumer Loss Map")
        '
        'tbPTOLossMap
        '
        Me.tbPTOLossMap.Location = New System.Drawing.Point(6, 24)
        Me.tbPTOLossMap.Name = "tbPTOLossMap"
        Me.tbPTOLossMap.Size = New System.Drawing.Size(239, 20)
        Me.tbPTOLossMap.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.tbPTOLossMap, "PTO Consumer Loss Map")
        '
        'tbPTODrive
        '
        Me.tbPTODrive.Location = New System.Drawing.Point(6, 183)
        Me.tbPTODrive.Name = "tbPTODrive"
        Me.tbPTODrive.Size = New System.Drawing.Size(239, 20)
        Me.tbPTODrive.TabIndex = 19
        Me.ToolTip1.SetToolTip(Me.tbPTODrive, "PTO Consumer Loss Map")
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.tpADAS)
        Me.TabControl1.Controls.Add(Me.tpRoadSweeper)
        Me.TabControl1.Location = New System.Drawing.Point(6, 173)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(587, 381)
        Me.TabControl1.TabIndex = 40
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Controls.Add(Me.GroupBox3)
        Me.TabPage1.Controls.Add(Me.GroupBox6)
        Me.TabPage1.Controls.Add(Me.GroupBox8)
        Me.TabPage1.Controls.Add(Me.GrAirRes)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(579, 355)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        Me.TabPage1.UseVisualStyleBackColor = true
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Panel1)
        Me.GroupBox4.Location = New System.Drawing.Point(6, 130)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(278, 50)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = false
        Me.GroupBox4.Text = "Vehicle Idling Speed"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tbVehIdlingSpeed)
        Me.Panel1.Controls.Add(Me.Label18)
        Me.Panel1.Controls.Add(Me.Label19)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 16)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(272, 31)
        Me.Panel1.TabIndex = 0
        '
        'tbVehIdlingSpeed
        '
        Me.tbVehIdlingSpeed.Location = New System.Drawing.Point(185, 4)
        Me.tbVehIdlingSpeed.Name = "tbVehIdlingSpeed"
        Me.tbVehIdlingSpeed.Size = New System.Drawing.Size(57, 20)
        Me.tbVehIdlingSpeed.TabIndex = 0
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(241, 7)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(30, 13)
        Me.Label18.TabIndex = 24
        Me.Label18.Text = "[rpm]"
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(82, 6)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(94, 13)
        Me.Label19.TabIndex = 8
        Me.Label19.Text = "Engine Idle Speed"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.GroupBox9)
        Me.TabPage2.Controls.Add(Me.GroupBox7)
        Me.TabPage2.Controls.Add(Me.GroupBox2)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(579, 355)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Powertrain"
        Me.TabPage2.UseVisualStyleBackColor = true
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.cbTankSystem)
        Me.GroupBox9.Controls.Add(Me.Label23)
        Me.GroupBox9.Location = New System.Drawing.Point(306, 7)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(264, 63)
        Me.GroupBox9.TabIndex = 5
        Me.GroupBox9.TabStop = false
        Me.GroupBox9.Text = "Tank System"
        '
        'cbTankSystem
        '
        Me.cbTankSystem.FormattingEnabled = true
        Me.cbTankSystem.Location = New System.Drawing.Point(9, 33)
        Me.cbTankSystem.Name = "cbTankSystem"
        Me.cbTankSystem.Size = New System.Drawing.Size(247, 21)
        Me.cbTankSystem.TabIndex = 1
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label23.Location = New System.Drawing.Point(6, 16)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(187, 13)
        Me.Label23.TabIndex = 0
        Me.Label23.Text = "Only applicable for NG engines!"
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.lvTorqueLimits)
        Me.TabPage3.Controls.Add(Me.Label17)
        Me.TabPage3.Controls.Add(Me.btDelMaxTorqueEntry)
        Me.TabPage3.Controls.Add(Me.btAddMaxTorqueEntry)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(579, 355)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Torque Limits"
        Me.TabPage3.UseVisualStyleBackColor = true
        '
        'lvTorqueLimits
        '
        Me.lvTorqueLimits.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lvTorqueLimits.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5, Me.ColumnHeader6})
        Me.lvTorqueLimits.FullRowSelect = true
        Me.lvTorqueLimits.GridLines = true
        Me.lvTorqueLimits.HideSelection = false
        Me.lvTorqueLimits.Location = New System.Drawing.Point(7, 8)
        Me.lvTorqueLimits.MultiSelect = false
        Me.lvTorqueLimits.Name = "lvTorqueLimits"
        Me.lvTorqueLimits.Size = New System.Drawing.Size(282, 102)
        Me.lvTorqueLimits.TabIndex = 7
        Me.lvTorqueLimits.TabStop = false
        Me.lvTorqueLimits.UseCompatibleStateImageBehavior = false
        Me.lvTorqueLimits.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Gear #"
        Me.ColumnHeader5.Width = 67
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Max. Torque"
        Me.ColumnHeader6.Width = 146
        '
        'Label17
        '
        Me.Label17.AutoSize = true
        Me.Label17.Location = New System.Drawing.Point(183, 113)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(106, 13)
        Me.Label17.TabIndex = 6
        Me.Label17.Text = "(Double-Click to Edit)"
        '
        'btDelMaxTorqueEntry
        '
        Me.btDelMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btDelMaxTorqueEntry.Location = New System.Drawing.Point(31, 116)
        Me.btDelMaxTorqueEntry.Name = "btDelMaxTorqueEntry"
        Me.btDelMaxTorqueEntry.Size = New System.Drawing.Size(24, 24)
        Me.btDelMaxTorqueEntry.TabIndex = 5
        Me.btDelMaxTorqueEntry.UseVisualStyleBackColor = true
        '
        'btAddMaxTorqueEntry
        '
        Me.btAddMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btAddMaxTorqueEntry.Location = New System.Drawing.Point(7, 116)
        Me.btAddMaxTorqueEntry.Name = "btAddMaxTorqueEntry"
        Me.btAddMaxTorqueEntry.Size = New System.Drawing.Size(24, 24)
        Me.btAddMaxTorqueEntry.TabIndex = 4
        Me.btAddMaxTorqueEntry.UseVisualStyleBackColor = true
        '
        'tpADAS
        '
        Me.tpADAS.Controls.Add(Me.GroupBox5)
        Me.tpADAS.Location = New System.Drawing.Point(4, 22)
        Me.tpADAS.Name = "tpADAS"
        Me.tpADAS.Padding = New System.Windows.Forms.Padding(3)
        Me.tpADAS.Size = New System.Drawing.Size(579, 355)
        Me.tpADAS.TabIndex = 3
        Me.tpADAS.Text = "ADAS"
        Me.tpADAS.UseVisualStyleBackColor = true
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.cbAtEcoRollReleaseLockupClutch)
        Me.GroupBox5.Controls.Add(Me.cbPcc)
        Me.GroupBox5.Controls.Add(Me.cbEcoRoll)
        Me.GroupBox5.Controls.Add(Me.Label22)
        Me.GroupBox5.Controls.Add(Me.cbEngineStopStart)
        Me.GroupBox5.Controls.Add(Me.lblPCC)
        Me.GroupBox5.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(565, 136)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = false
        Me.GroupBox5.Text = "ADAS Options"
        '
        'cbPcc
        '
        Me.cbPcc.FormattingEnabled = true
        Me.cbPcc.Location = New System.Drawing.Point(18, 104)
        Me.cbPcc.Name = "cbPcc"
        Me.cbPcc.Size = New System.Drawing.Size(179, 21)
        Me.cbPcc.TabIndex = 8
        '
        'cbEcoRoll
        '
        Me.cbEcoRoll.FormattingEnabled = true
        Me.cbEcoRoll.Location = New System.Drawing.Point(18, 58)
        Me.cbEcoRoll.Name = "cbEcoRoll"
        Me.cbEcoRoll.Size = New System.Drawing.Size(179, 21)
        Me.cbEcoRoll.TabIndex = 7
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(6, 42)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(47, 13)
        Me.Label22.TabIndex = 6
        Me.Label22.Text = "Eco-Roll"
        '
        'cbEngineStopStart
        '
        Me.cbEngineStopStart.AutoSize = true
        Me.cbEngineStopStart.Location = New System.Drawing.Point(6, 19)
        Me.cbEngineStopStart.Name = "cbEngineStopStart"
        Me.cbEngineStopStart.Size = New System.Drawing.Size(203, 17)
        Me.cbEngineStopStart.TabIndex = 4
        Me.cbEngineStopStart.Text = "Engine Stop/Start during vehicle stop"
        Me.cbEngineStopStart.UseVisualStyleBackColor = true
        '
        'lblPCC
        '
        Me.lblPCC.AutoSize = true
        Me.lblPCC.Location = New System.Drawing.Point(6, 88)
        Me.lblPCC.Name = "lblPCC"
        Me.lblPCC.Size = New System.Drawing.Size(122, 13)
        Me.lblPCC.TabIndex = 3
        Me.lblPCC.Text = "Predictive Cruise Control"
        '
        'tpRoadSweeper
        '
        Me.tpRoadSweeper.Controls.Add(Me.gbPTODrive)
        Me.tpRoadSweeper.Controls.Add(Me.pnPTO)
        Me.tpRoadSweeper.Controls.Add(Me.gbPTO)
        Me.tpRoadSweeper.Location = New System.Drawing.Point(4, 22)
        Me.tpRoadSweeper.Name = "tpRoadSweeper"
        Me.tpRoadSweeper.Size = New System.Drawing.Size(579, 355)
        Me.tpRoadSweeper.TabIndex = 4
        Me.tpRoadSweeper.Text = "PTO"
        Me.tpRoadSweeper.UseVisualStyleBackColor = true
        '
        'gbPTODrive
        '
        Me.gbPTODrive.Controls.Add(Me.Label27)
        Me.gbPTODrive.Controls.Add(Me.tbPtoGear)
        Me.gbPTODrive.Controls.Add(Me.Label26)
        Me.gbPTODrive.Controls.Add(Me.tbPtoEngineSpeed)
        Me.gbPTODrive.Controls.Add(Me.Label25)
        Me.gbPTODrive.Controls.Add(Me.Label24)
        Me.gbPTODrive.Location = New System.Drawing.Point(5, 172)
        Me.gbPTODrive.Name = "gbPTODrive"
        Me.gbPTODrive.Size = New System.Drawing.Size(566, 55)
        Me.gbPTODrive.TabIndex = 0
        Me.gbPTODrive.TabStop = false
        Me.gbPTODrive.Text = "Working operation settings (PTO mode 2)"
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(446, 24)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(20, 13)
        Me.Label27.TabIndex = 6
        Me.Label27.Text = "[#]"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbPtoGear
        '
        Me.tbPtoGear.Location = New System.Drawing.Point(371, 21)
        Me.tbPtoGear.Name = "tbPtoGear"
        Me.tbPtoGear.Size = New System.Drawing.Size(70, 20)
        Me.tbPtoGear.TabIndex = 5
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(196, 24)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(30, 13)
        Me.Label26.TabIndex = 4
        Me.Label26.Text = "[rpm]"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbPtoEngineSpeed
        '
        Me.tbPtoEngineSpeed.Location = New System.Drawing.Point(120, 21)
        Me.tbPtoEngineSpeed.Name = "tbPtoEngineSpeed"
        Me.tbPtoEngineSpeed.Size = New System.Drawing.Size(70, 20)
        Me.tbPtoEngineSpeed.TabIndex = 2
        '
        'Label25
        '
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(297, 24)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(68, 13)
        Me.Label25.TabIndex = 1
        Me.Label25.Text = "Gear number"
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(7, 24)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(97, 13)
        Me.Label24.TabIndex = 0
        Me.Label24.Text = "Min. Engine Speed"
        '
        'pnPTO
        '
        Me.pnPTO.Controls.Add(Me.btPTOCycleDrive)
        Me.pnPTO.Controls.Add(Me.Label28)
        Me.pnPTO.Controls.Add(Me.tbPTODrive)
        Me.pnPTO.Controls.Add(Me.btPTOCycle)
        Me.pnPTO.Controls.Add(Me.Label16)
        Me.pnPTO.Controls.Add(Me.tbPTOCycle)
        Me.pnPTO.Controls.Add(Me.btPTOLossMapBrowse)
        Me.pnPTO.Controls.Add(Me.Label7)
        Me.pnPTO.Controls.Add(Me.tbPTOLossMap)
        Me.pnPTO.Location = New System.Drawing.Point(5, 73)
        Me.pnPTO.Name = "pnPTO"
        Me.pnPTO.Size = New System.Drawing.Size(566, 211)
        Me.pnPTO.TabIndex = 6
        '
        'btPTOCycleDrive
        '
        Me.btPTOCycleDrive.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOCycleDrive.Location = New System.Drawing.Point(245, 181)
        Me.btPTOCycleDrive.Name = "btPTOCycleDrive"
        Me.btPTOCycleDrive.Size = New System.Drawing.Size(24, 24)
        Me.btPTOCycleDrive.TabIndex = 20
        Me.btPTOCycleDrive.UseVisualStyleBackColor = true
        '
        'Label28
        '
        Me.Label28.Location = New System.Drawing.Point(3, 164)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(266, 16)
        Me.Label28.TabIndex = 21
        Me.Label28.Text = "PTO Cycle during driving (PTO mode 3) (.vptor)"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'btPTOCycle
        '
        Me.btPTOCycle.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOCycle.Location = New System.Drawing.Point(245, 69)
        Me.btPTOCycle.Name = "btPTOCycle"
        Me.btPTOCycle.Size = New System.Drawing.Size(24, 24)
        Me.btPTOCycle.TabIndex = 17
        Me.btPTOCycle.UseVisualStyleBackColor = true
        '
        'Label16
        '
        Me.Label16.Location = New System.Drawing.Point(3, 52)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(266, 16)
        Me.Label16.TabIndex = 18
        Me.Label16.Text = "PTO Cycle during standstill (PTO mode 1) (.vptoc)"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'btPTOLossMapBrowse
        '
        Me.btPTOLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOLossMapBrowse.Location = New System.Drawing.Point(245, 22)
        Me.btPTOLossMapBrowse.Name = "btPTOLossMapBrowse"
        Me.btPTOLossMapBrowse.Size = New System.Drawing.Size(24, 24)
        Me.btPTOLossMapBrowse.TabIndex = 14
        Me.btPTOLossMapBrowse.UseVisualStyleBackColor = true
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(3, 5)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(201, 16)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "PTO Consumer Loss Map (.vptoi)"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'gbPTO
        '
        Me.gbPTO.Controls.Add(Me.cbPTOType)
        Me.gbPTO.Location = New System.Drawing.Point(5, 10)
        Me.gbPTO.Name = "gbPTO"
        Me.gbPTO.Size = New System.Drawing.Size(564, 57)
        Me.gbPTO.TabIndex = 5
        Me.gbPTO.TabStop = false
        Me.gbPTO.Text = "PTO Design Variant"
        '
        'cbLegislativeClass
        '
        Me.cbLegislativeClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLegislativeClass.FormattingEnabled = true
        Me.cbLegislativeClass.Location = New System.Drawing.Point(220, 140)
        Me.cbLegislativeClass.Name = "cbLegislativeClass"
        Me.cbLegislativeClass.Size = New System.Drawing.Size(52, 21)
        Me.cbLegislativeClass.TabIndex = 41
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(32, 123)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(112, 13)
        Me.Label21.TabIndex = 42
        Me.Label21.Text = "Maximum Laden Mass"
        '
        'cbAtEcoRollReleaseLockupClutch
        '
        Me.cbAtEcoRollReleaseLockupClutch.AutoSize = true
        Me.cbAtEcoRollReleaseLockupClutch.Location = New System.Drawing.Point(265, 19)
        Me.cbAtEcoRollReleaseLockupClutch.Name = "cbAtEcoRollReleaseLockupClutch"
        Me.cbAtEcoRollReleaseLockupClutch.Size = New System.Drawing.Size(243, 17)
        Me.cbAtEcoRollReleaseLockupClutch.TabIndex = 10
        Me.cbAtEcoRollReleaseLockupClutch.Text = "AT Gearbox: Eco-Roll Release Lockup Clutch"
        Me.cbAtEcoRollReleaseLockupClutch.UseVisualStyleBackColor = true
        '
        'VehicleForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(599, 608)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.cbLegislativeClass)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.TbHDVclass)
        Me.Controls.Add(Me.PicVehicle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.CbAxleConfig)
        Me.Controls.Add(Me.TbMassMass)
        Me.Controls.Add(Me.CbCat)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.ToolStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "VehicleForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F05_VEH"
        Me.GroupBox6.ResumeLayout(false)
        Me.GroupBox6.PerformLayout
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.GroupBox7.ResumeLayout(false)
        Me.PnRt.ResumeLayout(false)
        Me.PnRt.PerformLayout
        Me.GroupBox8.ResumeLayout(false)
        Me.GroupBox8.PerformLayout
        Me.PnWheelDiam.ResumeLayout(false)
        Me.PnWheelDiam.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.PnLoad.ResumeLayout(false)
        Me.PnLoad.PerformLayout
        Me.GrAirRes.ResumeLayout(false)
        Me.PnCdATrTr.ResumeLayout(false)
        Me.PnCdATrTr.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox2.ResumeLayout(false)
        Me.pnAngledriveFields.ResumeLayout(false)
        Me.pnAngledriveFields.PerformLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).EndInit
        Me.TabControl1.ResumeLayout(false)
        Me.TabPage1.ResumeLayout(false)
        Me.GroupBox4.ResumeLayout(false)
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.TabPage2.ResumeLayout(false)
        Me.GroupBox9.ResumeLayout(false)
        Me.GroupBox9.PerformLayout
        Me.TabPage3.ResumeLayout(false)
        Me.TabPage3.PerformLayout
        Me.tpADAS.ResumeLayout(false)
        Me.GroupBox5.ResumeLayout(false)
        Me.GroupBox5.PerformLayout
        Me.tpRoadSweeper.ResumeLayout(false)
        Me.gbPTODrive.ResumeLayout(false)
        Me.gbPTODrive.PerformLayout
        Me.pnPTO.ResumeLayout(false)
        Me.pnPTO.PerformLayout
        Me.gbPTO.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents TbMass As System.Windows.Forms.TextBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents TbLoad As System.Windows.Forms.TextBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents TBcdA As System.Windows.Forms.TextBox
	Friend WithEvents Label13 As System.Windows.Forms.Label
	Friend WithEvents TBrdyn As System.Windows.Forms.TextBox
	Friend WithEvents ButOK As System.Windows.Forms.Button
	Friend WithEvents ButCancel As System.Windows.Forms.Button
	Friend WithEvents Label14 As System.Windows.Forms.Label
	Friend WithEvents Label31 As System.Windows.Forms.Label
	Friend WithEvents Label35 As System.Windows.Forms.Label
	Friend WithEvents CbCdMode As System.Windows.Forms.ComboBox
	Friend WithEvents TbCdFile As System.Windows.Forms.TextBox
	Friend WithEvents BtCdFileBrowse As System.Windows.Forms.Button
	Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
	Friend WithEvents LbCdMode As System.Windows.Forms.Label
	Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
	Friend WithEvents ToolStripBtNew As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtOpen As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtSave As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As System.Windows.Forms.ToolStripButton
	Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
	Friend WithEvents LbRtRatio As System.Windows.Forms.Label
	Friend WithEvents TbRtRatio As System.Windows.Forms.TextBox
	Friend WithEvents CbRtType As System.Windows.Forms.ComboBox
	Friend WithEvents Label45 As System.Windows.Forms.Label
	Friend WithEvents PnRt As System.Windows.Forms.Panel
	Friend WithEvents Label46 As System.Windows.Forms.Label
	Friend WithEvents Label50 As System.Windows.Forms.Label
	Friend WithEvents TbMassExtra As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
	Friend WithEvents ButAxlRem As System.Windows.Forms.Button
	Friend WithEvents LvRRC As System.Windows.Forms.ListView
	Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader8 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ButAxlAdd As System.Windows.Forms.Button
	Friend WithEvents CbCat As System.Windows.Forms.ComboBox
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents TbMassMass As System.Windows.Forms.TextBox
	Friend WithEvents ColumnHeader9 As System.Windows.Forms.ColumnHeader
	Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
	Friend WithEvents LbStatus As System.Windows.Forms.ToolStripStatusLabel
	Friend WithEvents CbAxleConfig As System.Windows.Forms.ComboBox
	Friend WithEvents TbHDVclass As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents GrAirRes As System.Windows.Forms.GroupBox
	Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
	Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
	Friend WithEvents CmOpenFile As System.Windows.Forms.ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents BtCdFileOpen As System.Windows.Forms.Button
	Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
	Friend WithEvents PnLoad As System.Windows.Forms.Panel
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
	Friend WithEvents PnWheelDiam As System.Windows.Forms.Panel
	Friend WithEvents PicVehicle As System.Windows.Forms.PictureBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents PnCdATrTr As System.Windows.Forms.Panel
	Friend WithEvents Label38 As System.Windows.Forms.Label
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents cbAngledriveType As System.Windows.Forms.ComboBox
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents BtRtBrowse As System.Windows.Forms.Button
	Friend WithEvents TbRtPath As System.Windows.Forms.TextBox
	Friend WithEvents pnAngledriveFields As System.Windows.Forms.Panel
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents tbAngledriveRatio As System.Windows.Forms.TextBox
	Friend WithEvents btAngledriveLossMapBrowse As System.Windows.Forms.Button
	Friend WithEvents tbAngledriveLossMapPath As System.Windows.Forms.TextBox
	Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
	Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
	Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
	Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
	Friend WithEvents Label17 As System.Windows.Forms.Label
	Friend WithEvents btDelMaxTorqueEntry As System.Windows.Forms.Button
	Friend WithEvents btAddMaxTorqueEntry As System.Windows.Forms.Button
	Friend WithEvents lvTorqueLimits As System.Windows.Forms.ListView
	Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader10 As System.Windows.Forms.ColumnHeader
	Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents tbVehIdlingSpeed As System.Windows.Forms.TextBox
	Friend WithEvents Label18 As System.Windows.Forms.Label
	Friend WithEvents Label19 As System.Windows.Forms.Label
	Friend WithEvents cbLegislativeClass As System.Windows.Forms.ComboBox
	Friend WithEvents tbVehicleHeight As System.Windows.Forms.TextBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As Label
    Friend WithEvents tpADAS As TabPage
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents cbEngineStopStart As CheckBox
    Friend WithEvents lblPCC As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents cbPcc As ComboBox
    Friend WithEvents cbEcoRoll As ComboBox
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents cbTankSystem As ComboBox
    Friend WithEvents Label23 As Label
    Friend WithEvents tpRoadSweeper As TabPage
    Friend WithEvents gbPTODrive As GroupBox
    Friend WithEvents tbPtoGear As TextBox
    Friend WithEvents Label26 As Label
    Friend WithEvents tbPtoEngineSpeed As TextBox
    Friend WithEvents Label25 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents gbPTO As GroupBox
    Friend WithEvents cbPTOType As ComboBox
    Friend WithEvents pnPTO As Panel
    Friend WithEvents btPTOCycle As Button
    Friend WithEvents Label16 As Label
    Friend WithEvents tbPTOCycle As TextBox
    Friend WithEvents btPTOLossMapBrowse As Button
    Friend WithEvents Label7 As Label
    Friend WithEvents tbPTOLossMap As TextBox
    Friend WithEvents Label27 As Label
    Friend WithEvents btPTOCycleDrive As Button
    Friend WithEvents Label28 As Label
    Friend WithEvents tbPTODrive As TextBox
    Friend WithEvents cbAtEcoRollReleaseLockupClutch As CheckBox
End Class
