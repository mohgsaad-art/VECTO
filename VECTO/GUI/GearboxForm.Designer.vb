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
Partial Class GearboxForm
	Inherits Form

	'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

	'Wird vom Windows Form-Designer benötigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GearboxForm))
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ButCancel = New System.Windows.Forms.Button()
        Me.ButOK = New System.Windows.Forms.Button()
        Me.TbTracInt = New System.Windows.Forms.TextBox()
        Me.LvGears = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.TBI_getr = New System.Windows.Forms.TextBox()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TbName = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.BtRemGear = New System.Windows.Forms.Button()
        Me.GrGearShift = New System.Windows.Forms.GroupBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.tbUpshiftMinAcceleration = New System.Windows.Forms.TextBox()
        Me.tbUpshiftAfterDownshift = New System.Windows.Forms.TextBox()
        Me.tbDownshiftAfterUpshift = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.PnTorqRes = New System.Windows.Forms.Panel()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TbTqResv = New System.Windows.Forms.TextBox()
        Me.TbMinTimeBetweenShifts = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TbStartAcc = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TbStartSpeed = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TbTqResvStart = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.gbTC = New System.Windows.Forms.GroupBox()
        Me.pnTcEngineering = New System.Windows.Forms.Panel()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.TBTCShiftPolygon = New System.Windows.Forms.TextBox()
        Me.tbTCmaxSpeed = New System.Windows.Forms.TextBox()
        Me.TbTCrefrpm = New System.Windows.Forms.TextBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.BtTCShiftFileBrowse = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TbTCinertia = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.LblTCShiftFile = New System.Windows.Forms.Label()
        Me.BtTCfileOpen = New System.Windows.Forms.Button()
        Me.TbTCfile = New System.Windows.Forms.TextBox()
        Me.BtTCfileBrowse = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.tbTCLUpshiftMinAcceleration = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.CbGStype = New System.Windows.Forms.ComboBox()
        Me.BtAddGear = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.btExportVGBS = New System.Windows.Forms.Button()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.PnInertiaTI = New System.Windows.Forms.Panel()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.gbPowershiftLosses = New System.Windows.Forms.GroupBox()
        Me.lbAtShiftTime = New System.Windows.Forms.Label()
        Me.tbATShiftTime = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.tbTCCUpshiftMinAcceleration = New System.Windows.Forms.TextBox()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.gbTCAccMin = New System.Windows.Forms.GroupBox()
        Me.lblGbxInfo = New System.Windows.Forms.Label()
        Me.btnExportXML = New System.Windows.Forms.Button()
        Me.btnExportAxlGearXML = New System.Windows.Forms.Button()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GrGearShift.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.PnTorqRes.SuspendLayout
        Me.GroupBox2.SuspendLayout
        Me.CmOpenFile.SuspendLayout
        Me.gbTC.SuspendLayout
        Me.pnTcEngineering.SuspendLayout
        Me.GroupBox4.SuspendLayout
        Me.PnInertiaTI.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.gbPowershiftLosses.SuspendLayout
        Me.gbTCAccMin.SuspendLayout
        Me.SuspendLayout
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(877, 31)
        Me.ToolStrip1.TabIndex = 30
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSave.Text = "ToolStripButton1"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSendTo.Text = "Send to Job Editor"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 31)
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
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 674)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(877, 22)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 37
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(39, 17)
        Me.LbStatus.Text = "Status"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(798, 648)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 8
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(717, 648)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 7
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'TbTracInt
        '
        Me.TbTracInt.Location = New System.Drawing.Point(299, 3)
        Me.TbTracInt.Name = "TbTracInt"
        Me.TbTracInt.Size = New System.Drawing.Size(57, 20)
        Me.TbTracInt.TabIndex = 1
        '
        'LvGears
        '
        Me.LvGears.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader7})
        Me.LvGears.FullRowSelect = true
        Me.LvGears.GridLines = true
        Me.LvGears.HideSelection = false
        Me.LvGears.Location = New System.Drawing.Point(6, 18)
        Me.LvGears.MultiSelect = false
        Me.LvGears.Name = "LvGears"
        Me.LvGears.Size = New System.Drawing.Size(429, 183)
        Me.LvGears.TabIndex = 0
        Me.LvGears.TabStop = false
        Me.LvGears.UseCompatibleStateImageBehavior = false
        Me.LvGears.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Gear"
        Me.ColumnHeader1.Width = 38
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Ratio"
        Me.ColumnHeader2.Width = 42
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Loss Map or Efficiency"
        Me.ColumnHeader3.Width = 111
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Shift Polygons"
        Me.ColumnHeader5.Width = 79
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Max Torque"
        Me.ColumnHeader6.Width = 78
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Max Speed"
        Me.ColumnHeader7.Width = 76
        '
        'TBI_getr
        '
        Me.TBI_getr.Location = New System.Drawing.Point(78, 3)
        Me.TBI_getr.Name = "TBI_getr"
        Me.TBI_getr.Size = New System.Drawing.Size(57, 20)
        Me.TBI_getr.TabIndex = 0
        '
        'Label49
        '
        Me.Label49.AutoSize = true
        Me.Label49.Location = New System.Drawing.Point(362, 6)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(18, 13)
        Me.Label49.TabIndex = 2
        Me.Label49.Text = "[s]"
        '
        'Label33
        '
        Me.Label33.AutoSize = true
        Me.Label33.Location = New System.Drawing.Point(141, 6)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(36, 13)
        Me.Label33.TabIndex = 2
        Me.Label33.Text = "[kgm²]"
        '
        'Label48
        '
        Me.Label48.AutoSize = true
        Me.Label48.Location = New System.Drawing.Point(191, 6)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(102, 13)
        Me.Label48.TabIndex = 2
        Me.Label48.Text = "Traction Interruption"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(6, 6)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(36, 13)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "Inertia"
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(26, 79)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 13)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "Make and Model"
        '
        'TbName
        '
        Me.TbName.Location = New System.Drawing.Point(119, 76)
        Me.TbName.Name = "TbName"
        Me.TbName.Size = New System.Drawing.Size(334, 20)
        Me.TbName.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_GBX
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(453, 40)
        Me.PictureBox1.TabIndex = 43
        Me.PictureBox1.TabStop = false
        '
        'BtRemGear
        '
        Me.BtRemGear.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.BtRemGear.Location = New System.Drawing.Point(30, 202)
        Me.BtRemGear.Name = "BtRemGear"
        Me.BtRemGear.Size = New System.Drawing.Size(24, 24)
        Me.BtRemGear.TabIndex = 2
        Me.BtRemGear.UseVisualStyleBackColor = true
        '
        'GrGearShift
        '
        Me.GrGearShift.Controls.Add(Me.GroupBox1)
        Me.GrGearShift.Controls.Add(Me.PnTorqRes)
        Me.GrGearShift.Controls.Add(Me.TbMinTimeBetweenShifts)
        Me.GrGearShift.Controls.Add(Me.Label12)
        Me.GrGearShift.Controls.Add(Me.Label13)
        Me.GrGearShift.Controls.Add(Me.GroupBox2)
        Me.GrGearShift.Location = New System.Drawing.Point(12, 377)
        Me.GrGearShift.Name = "GrGearShift"
        Me.GrGearShift.Size = New System.Drawing.Size(441, 291)
        Me.GrGearShift.TabIndex = 3
        Me.GrGearShift.TabStop = false
        Me.GrGearShift.Text = "Gear shift parameters"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.tbUpshiftMinAcceleration)
        Me.GroupBox1.Controls.Add(Me.tbUpshiftAfterDownshift)
        Me.GroupBox1.Controls.Add(Me.tbDownshiftAfterUpshift)
        Me.GroupBox1.Controls.Add(Me.Label24)
        Me.GroupBox1.Controls.Add(Me.Label23)
        Me.GroupBox1.Controls.Add(Me.Label22)
        Me.GroupBox1.Controls.Add(Me.Label21)
        Me.GroupBox1.Controls.Add(Me.Label20)
        Me.GroupBox1.Controls.Add(Me.Label19)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 82)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(429, 100)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Shift Strategy Parameters"
        '
        'tbUpshiftMinAcceleration
        '
        Me.tbUpshiftMinAcceleration.Location = New System.Drawing.Point(209, 71)
        Me.tbUpshiftMinAcceleration.Name = "tbUpshiftMinAcceleration"
        Me.tbUpshiftMinAcceleration.Size = New System.Drawing.Size(57, 20)
        Me.tbUpshiftMinAcceleration.TabIndex = 2
        '
        'tbUpshiftAfterDownshift
        '
        Me.tbUpshiftAfterDownshift.Location = New System.Drawing.Point(209, 45)
        Me.tbUpshiftAfterDownshift.Name = "tbUpshiftAfterDownshift"
        Me.tbUpshiftAfterDownshift.Size = New System.Drawing.Size(57, 20)
        Me.tbUpshiftAfterDownshift.TabIndex = 1
        '
        'tbDownshiftAfterUpshift
        '
        Me.tbDownshiftAfterUpshift.Location = New System.Drawing.Point(209, 20)
        Me.tbDownshiftAfterUpshift.Name = "tbDownshiftAfterUpshift"
        Me.tbDownshiftAfterUpshift.Size = New System.Drawing.Size(57, 20)
        Me.tbDownshiftAfterUpshift.TabIndex = 0
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(275, 74)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(34, 13)
        Me.Label24.TabIndex = 5
        Me.Label24.Text = "[m/s²]"
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Location = New System.Drawing.Point(275, 48)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(18, 13)
        Me.Label23.TabIndex = 4
        Me.Label23.Text = "[s]"
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(275, 25)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(18, 13)
        Me.Label22.TabIndex = 3
        Me.Label22.Text = "[s]"
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(20, 74)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(146, 13)
        Me.Label21.TabIndex = 2
        Me.Label21.Text = "Min. acceleration after upshift"
        '
        'Label20
        '
        Me.Label20.AutoSize = true
        Me.Label20.Location = New System.Drawing.Point(20, 48)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(140, 13)
        Me.Label20.TabIndex = 1
        Me.Label20.Text = "Upshift after downshift delay"
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(20, 25)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(140, 13)
        Me.Label19.TabIndex = 0
        Me.Label19.Text = "Downshift after upshift delay"
        '
        'PnTorqRes
        '
        Me.PnTorqRes.Controls.Add(Me.Label25)
        Me.PnTorqRes.Controls.Add(Me.Label2)
        Me.PnTorqRes.Controls.Add(Me.Label4)
        Me.PnTorqRes.Controls.Add(Me.TbTqResv)
        Me.PnTorqRes.Location = New System.Drawing.Point(9, 20)
        Me.PnTorqRes.Name = "PnTorqRes"
        Me.PnTorqRes.Size = New System.Drawing.Size(426, 32)
        Me.PnTorqRes.TabIndex = 0
        '
        'Label25
        '
        Me.Label25.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(294, 9)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(129, 13)
        Me.Label25.TabIndex = 1
        Me.Label25.Text = "(Early shift-up, Skip gears)"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(18, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Torque reserve"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(272, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(21, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "[%]"
        '
        'TbTqResv
        '
        Me.TbTqResv.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbTqResv.Location = New System.Drawing.Point(206, 6)
        Me.TbTqResv.Name = "TbTqResv"
        Me.TbTqResv.Size = New System.Drawing.Size(57, 20)
        Me.TbTqResv.TabIndex = 0
        '
        'TbMinTimeBetweenShifts
        '
        Me.TbMinTimeBetweenShifts.Location = New System.Drawing.Point(215, 52)
        Me.TbMinTimeBetweenShifts.Name = "TbMinTimeBetweenShifts"
        Me.TbMinTimeBetweenShifts.Size = New System.Drawing.Size(57, 20)
        Me.TbMinTimeBetweenShifts.TabIndex = 1
        '
        'Label12
        '
        Me.Label12.AutoSize = true
        Me.Label12.Location = New System.Drawing.Point(281, 55)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(18, 13)
        Me.Label12.TabIndex = 9
        Me.Label12.Text = "[s]"
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(27, 55)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(162, 13)
        Me.Label13.TabIndex = 8
        Me.Label13.Text = "Minimum time between gearshifts"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.TbStartAcc)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.TbStartSpeed)
        Me.GroupBox2.Controls.Add(Me.Label9)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.TbTqResvStart)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 186)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(429, 99)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Start Gear"
        '
        'TbStartAcc
        '
        Me.TbStartAcc.Location = New System.Drawing.Point(209, 71)
        Me.TbStartAcc.Name = "TbStartAcc"
        Me.TbStartAcc.Size = New System.Drawing.Size(57, 20)
        Me.TbStartAcc.TabIndex = 2
        Me.TbStartAcc.Visible = false
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(20, 74)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(173, 13)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "Reference acceleration at clutch-in"
        Me.Label11.Visible = false
        '
        'TbStartSpeed
        '
        Me.TbStartSpeed.Location = New System.Drawing.Point(209, 45)
        Me.TbStartSpeed.Name = "TbStartSpeed"
        Me.TbStartSpeed.Size = New System.Drawing.Size(57, 20)
        Me.TbStartSpeed.TabIndex = 1
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(20, 48)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(181, 13)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Reference vehicle speed at clutch-in"
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(272, 74)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(34, 13)
        Me.Label10.TabIndex = 0
        Me.Label10.Text = "[m/s²]"
        Me.Label10.Visible = false
        '
        'TbTqResvStart
        '
        Me.TbTqResvStart.Location = New System.Drawing.Point(209, 19)
        Me.TbTqResvStart.Name = "TbTqResvStart"
        Me.TbTqResvStart.Size = New System.Drawing.Size(57, 20)
        Me.TbTqResvStart.TabIndex = 0
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(272, 48)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(31, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "[m/s]"
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(20, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(79, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Torque reserve"
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(272, 22)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(21, 13)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "[%]"
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.Size = New System.Drawing.Size(153, 48)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'gbTC
        '
        Me.gbTC.Controls.Add(Me.pnTcEngineering)
        Me.gbTC.Controls.Add(Me.BtTCfileOpen)
        Me.gbTC.Controls.Add(Me.TbTCfile)
        Me.gbTC.Controls.Add(Me.BtTCfileBrowse)
        Me.gbTC.Controls.Add(Me.Label17)
        Me.gbTC.Location = New System.Drawing.Point(459, 377)
        Me.gbTC.Name = "gbTC"
        Me.gbTC.Size = New System.Drawing.Size(414, 162)
        Me.gbTC.TabIndex = 5
        Me.gbTC.TabStop = false
        Me.gbTC.Text = "Torque Converter"
        '
        'pnTcEngineering
        '
        Me.pnTcEngineering.Controls.Add(Me.Label26)
        Me.pnTcEngineering.Controls.Add(Me.TBTCShiftPolygon)
        Me.pnTcEngineering.Controls.Add(Me.tbTCmaxSpeed)
        Me.pnTcEngineering.Controls.Add(Me.TbTCrefrpm)
        Me.pnTcEngineering.Controls.Add(Me.Label27)
        Me.pnTcEngineering.Controls.Add(Me.Label14)
        Me.pnTcEngineering.Controls.Add(Me.BtTCShiftFileBrowse)
        Me.pnTcEngineering.Controls.Add(Me.Label1)
        Me.pnTcEngineering.Controls.Add(Me.TbTCinertia)
        Me.pnTcEngineering.Controls.Add(Me.Label15)
        Me.pnTcEngineering.Controls.Add(Me.Label18)
        Me.pnTcEngineering.Controls.Add(Me.LblTCShiftFile)
        Me.pnTcEngineering.Location = New System.Drawing.Point(7, 63)
        Me.pnTcEngineering.Name = "pnTcEngineering"
        Me.pnTcEngineering.Size = New System.Drawing.Size(399, 93)
        Me.pnTcEngineering.TabIndex = 3
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(1, 29)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(64, 13)
        Me.Label26.TabIndex = 50
        Me.Label26.Text = "Max. Speed"
        '
        'TBTCShiftPolygon
        '
        Me.TBTCShiftPolygon.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TBTCShiftPolygon.Location = New System.Drawing.Point(4, 70)
        Me.TBTCShiftPolygon.Name = "TBTCShiftPolygon"
        Me.TBTCShiftPolygon.Size = New System.Drawing.Size(349, 20)
        Me.TBTCShiftPolygon.TabIndex = 3
        '
        'tbTCmaxSpeed
        '
        Me.tbTCmaxSpeed.Location = New System.Drawing.Point(75, 26)
        Me.tbTCmaxSpeed.Name = "tbTCmaxSpeed"
        Me.tbTCmaxSpeed.Size = New System.Drawing.Size(57, 20)
        Me.tbTCmaxSpeed.TabIndex = 2
        '
        'TbTCrefrpm
        '
        Me.TbTCrefrpm.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbTCrefrpm.Location = New System.Drawing.Point(296, 0)
        Me.TbTCrefrpm.Name = "TbTCrefrpm"
        Me.TbTCrefrpm.Size = New System.Drawing.Size(57, 20)
        Me.TbTCrefrpm.TabIndex = 1
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(138, 29)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(30, 13)
        Me.Label27.TabIndex = 51
        Me.Label27.Text = "[rpm]"
        '
        'Label14
        '
        Me.Label14.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(357, 3)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(40, 13)
        Me.Label14.TabIndex = 41
        Me.Label14.Text = "[1/min]"
        '
        'BtTCShiftFileBrowse
        '
        Me.BtTCShiftFileBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtTCShiftFileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtTCShiftFileBrowse.Location = New System.Drawing.Point(354, 68)
        Me.BtTCShiftFileBrowse.Name = "BtTCShiftFileBrowse"
        Me.BtTCShiftFileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtTCShiftFileBrowse.TabIndex = 4
        Me.BtTCShiftFileBrowse.UseVisualStyleBackColor = true
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(138, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 13)
        Me.Label1.TabIndex = 43
        Me.Label1.Text = "[kgm²]"
        '
        'TbTCinertia
        '
        Me.TbTCinertia.Location = New System.Drawing.Point(75, 0)
        Me.TbTCinertia.Name = "TbTCinertia"
        Me.TbTCinertia.Size = New System.Drawing.Size(57, 20)
        Me.TbTCinertia.TabIndex = 0
        '
        'Label15
        '
        Me.Label15.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label15.AutoSize = true
        Me.Label15.Location = New System.Drawing.Point(213, 3)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(77, 13)
        Me.Label15.TabIndex = 42
        Me.Label15.Text = "Reference rpm"
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(1, 3)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(36, 13)
        Me.Label18.TabIndex = 44
        Me.Label18.Text = "Inertia"
        '
        'LblTCShiftFile
        '
        Me.LblTCShiftFile.AutoSize = true
        Me.LblTCShiftFile.Location = New System.Drawing.Point(1, 55)
        Me.LblTCShiftFile.Name = "LblTCShiftFile"
        Me.LblTCShiftFile.Size = New System.Drawing.Size(207, 13)
        Me.LblTCShiftFile.TabIndex = 48
        Me.LblTCShiftFile.Text = "Torque converter shift polygons file (.vgbs)"
        '
        'BtTCfileOpen
        '
        Me.BtTCfileOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtTCfileOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtTCfileOpen.Location = New System.Drawing.Point(383, 35)
        Me.BtTCfileOpen.Name = "BtTCfileOpen"
        Me.BtTCfileOpen.Size = New System.Drawing.Size(24, 24)
        Me.BtTCfileOpen.TabIndex = 2
        Me.BtTCfileOpen.UseVisualStyleBackColor = true
        '
        'TbTCfile
        '
        Me.TbTCfile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbTCfile.Location = New System.Drawing.Point(9, 37)
        Me.TbTCfile.Name = "TbTCfile"
        Me.TbTCfile.Size = New System.Drawing.Size(349, 20)
        Me.TbTCfile.TabIndex = 0
        '
        'BtTCfileBrowse
        '
        Me.BtTCfileBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtTCfileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtTCfileBrowse.Location = New System.Drawing.Point(359, 35)
        Me.BtTCfileBrowse.Name = "BtTCfileBrowse"
        Me.BtTCfileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtTCfileBrowse.TabIndex = 1
        Me.BtTCfileBrowse.UseVisualStyleBackColor = true
        '
        'Label17
        '
        Me.Label17.AutoSize = true
        Me.Label17.Location = New System.Drawing.Point(6, 21)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(209, 13)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "Torque converter characteristics file (.vtcc)"
        '
        'tbTCLUpshiftMinAcceleration
        '
        Me.tbTCLUpshiftMinAcceleration.Location = New System.Drawing.Point(78, 19)
        Me.tbTCLUpshiftMinAcceleration.Name = "tbTCLUpshiftMinAcceleration"
        Me.tbTCLUpshiftMinAcceleration.Size = New System.Drawing.Size(57, 20)
        Me.tbTCLUpshiftMinAcceleration.TabIndex = 0
        '
        'Label30
        '
        Me.Label30.AutoSize = true
        Me.Label30.Location = New System.Drawing.Point(141, 22)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(34, 13)
        Me.Label30.TabIndex = 42
        Me.Label30.Text = "[m/s²]"
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(6, 22)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(69, 13)
        Me.Label31.TabIndex = 41
        Me.Label31.Text = "Acc. for C->L"
        '
        'Label16
        '
        Me.Label16.AutoSize = true
        Me.Label16.Location = New System.Drawing.Point(18, 105)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(95, 13)
        Me.Label16.TabIndex = 45
        Me.Label16.Text = "Transmission Type"
        '
        'CbGStype
        '
        Me.CbGStype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbGStype.FormattingEnabled = true
        Me.CbGStype.Location = New System.Drawing.Point(119, 102)
        Me.CbGStype.Name = "CbGStype"
        Me.CbGStype.Size = New System.Drawing.Size(334, 21)
        Me.CbGStype.TabIndex = 1
        '
        'BtAddGear
        '
        Me.BtAddGear.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.BtAddGear.Location = New System.Drawing.Point(6, 202)
        Me.BtAddGear.Name = "BtAddGear"
        Me.BtAddGear.Size = New System.Drawing.Size(24, 24)
        Me.BtAddGear.TabIndex = 1
        Me.BtAddGear.UseVisualStyleBackColor = true
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.btExportVGBS)
        Me.GroupBox4.Controls.Add(Me.Label32)
        Me.GroupBox4.Controls.Add(Me.LvGears)
        Me.GroupBox4.Controls.Add(Me.BtRemGear)
        Me.GroupBox4.Controls.Add(Me.BtAddGear)
        Me.GroupBox4.Location = New System.Drawing.Point(12, 135)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(441, 232)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = false
        Me.GroupBox4.Text = "Gears"
        '
        'btExportVGBS
        '
        Me.btExportVGBS.Enabled = false
        Me.btExportVGBS.Location = New System.Drawing.Point(55, 202)
        Me.btExportVGBS.Name = "btExportVGBS"
        Me.btExportVGBS.Size = New System.Drawing.Size(153, 24)
        Me.btExportVGBS.TabIndex = 4
        Me.btExportVGBS.Text = "Export Shiftcurve as VGBS"
        Me.btExportVGBS.UseVisualStyleBackColor = true
        '
        'Label32
        '
        Me.Label32.AutoSize = true
        Me.Label32.Location = New System.Drawing.Point(331, 202)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(106, 13)
        Me.Label32.TabIndex = 3
        Me.Label32.Text = "(Double-Click to Edit)"
        '
        'PnInertiaTI
        '
        Me.PnInertiaTI.Controls.Add(Me.Label6)
        Me.PnInertiaTI.Controls.Add(Me.TBI_getr)
        Me.PnInertiaTI.Controls.Add(Me.Label33)
        Me.PnInertiaTI.Controls.Add(Me.Label48)
        Me.PnInertiaTI.Controls.Add(Me.Label49)
        Me.PnInertiaTI.Controls.Add(Me.TbTracInt)
        Me.PnInertiaTI.Location = New System.Drawing.Point(459, 344)
        Me.PnInertiaTI.Name = "PnInertiaTI"
        Me.PnInertiaTI.Size = New System.Drawing.Size(411, 30)
        Me.PnInertiaTI.TabIndex = 4
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(459, 28)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(414, 308)
        Me.PicBox.TabIndex = 48
        Me.PicBox.TabStop = false
        '
        'gbPowershiftLosses
        '
        Me.gbPowershiftLosses.Controls.Add(Me.lbAtShiftTime)
        Me.gbPowershiftLosses.Controls.Add(Me.tbATShiftTime)
        Me.gbPowershiftLosses.Controls.Add(Me.Label28)
        Me.gbPowershiftLosses.Location = New System.Drawing.Point(459, 598)
        Me.gbPowershiftLosses.Name = "gbPowershiftLosses"
        Me.gbPowershiftLosses.Size = New System.Drawing.Size(414, 47)
        Me.gbPowershiftLosses.TabIndex = 6
        Me.gbPowershiftLosses.TabStop = false
        Me.gbPowershiftLosses.Text = "Power shift losses"
        '
        'lbAtShiftTime
        '
        Me.lbAtShiftTime.AutoSize = true
        Me.lbAtShiftTime.Location = New System.Drawing.Point(6, 22)
        Me.lbAtShiftTime.Name = "lbAtShiftTime"
        Me.lbAtShiftTime.Size = New System.Drawing.Size(50, 13)
        Me.lbAtShiftTime.TabIndex = 0
        Me.lbAtShiftTime.Text = "Shift time"
        '
        'tbATShiftTime
        '
        Me.tbATShiftTime.Location = New System.Drawing.Point(78, 19)
        Me.tbATShiftTime.Name = "tbATShiftTime"
        Me.tbATShiftTime.Size = New System.Drawing.Size(57, 20)
        Me.tbATShiftTime.TabIndex = 0
        '
        'Label28
        '
        Me.Label28.AutoSize = true
        Me.Label28.Location = New System.Drawing.Point(141, 22)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(18, 13)
        Me.Label28.TabIndex = 41
        Me.Label28.Text = "[s]"
        '
        'tbTCCUpshiftMinAcceleration
        '
        Me.tbTCCUpshiftMinAcceleration.Location = New System.Drawing.Point(299, 19)
        Me.tbTCCUpshiftMinAcceleration.Name = "tbTCCUpshiftMinAcceleration"
        Me.tbTCCUpshiftMinAcceleration.Size = New System.Drawing.Size(57, 20)
        Me.tbTCCUpshiftMinAcceleration.TabIndex = 1
        '
        'Label34
        '
        Me.Label34.AutoSize = true
        Me.Label34.Location = New System.Drawing.Point(362, 22)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(34, 13)
        Me.Label34.TabIndex = 45
        Me.Label34.Text = "[m/s²]"
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(211, 22)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(70, 13)
        Me.Label35.TabIndex = 44
        Me.Label35.Text = "Acc. for C->C"
        '
        'gbTCAccMin
        '
        Me.gbTCAccMin.Controls.Add(Me.tbTCLUpshiftMinAcceleration)
        Me.gbTCAccMin.Controls.Add(Me.tbTCCUpshiftMinAcceleration)
        Me.gbTCAccMin.Controls.Add(Me.Label34)
        Me.gbTCAccMin.Controls.Add(Me.Label31)
        Me.gbTCAccMin.Controls.Add(Me.Label35)
        Me.gbTCAccMin.Controls.Add(Me.Label30)
        Me.gbTCAccMin.Location = New System.Drawing.Point(459, 545)
        Me.gbTCAccMin.Name = "gbTCAccMin"
        Me.gbTCAccMin.Size = New System.Drawing.Size(414, 47)
        Me.gbTCAccMin.TabIndex = 6
        Me.gbTCAccMin.TabStop = false
        Me.gbTCAccMin.Text = "Torque Converter: Min. acceleration after upshift"
        '
        'lblGbxInfo
        '
        Me.lblGbxInfo.AutoSize = true
        Me.lblGbxInfo.Location = New System.Drawing.Point(121, 126)
        Me.lblGbxInfo.Name = "lblGbxInfo"
        Me.lblGbxInfo.Size = New System.Drawing.Size(0, 13)
        Me.lblGbxInfo.TabIndex = 51
        '
        'btnExportXML
        '
        Me.btnExportXML.Location = New System.Drawing.Point(459, 648)
        Me.btnExportXML.Name = "btnExportXML"
        Me.btnExportXML.Size = New System.Drawing.Size(119, 23)
        Me.btnExportXML.TabIndex = 9
        Me.btnExportXML.Text = "Exp. Gearbox as XML"
        Me.btnExportXML.UseVisualStyleBackColor = true
        '
        'btnExportAxlGearXML
        '
        Me.btnExportAxlGearXML.Location = New System.Drawing.Point(584, 648)
        Me.btnExportAxlGearXML.Name = "btnExportAxlGearXML"
        Me.btnExportAxlGearXML.Size = New System.Drawing.Size(118, 23)
        Me.btnExportAxlGearXML.TabIndex = 10
        Me.btnExportAxlGearXML.Text = "Exp. AxlGear as XML"
        Me.btnExportAxlGearXML.UseVisualStyleBackColor = true
        '
        'GearboxForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(877, 696)
        Me.Controls.Add(Me.btnExportAxlGearXML)
        Me.Controls.Add(Me.btnExportXML)
        Me.Controls.Add(Me.lblGbxInfo)
        Me.Controls.Add(Me.gbTCAccMin)
        Me.Controls.Add(Me.gbPowershiftLosses)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.PnInertiaTI)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.CbGStype)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.GrGearShift)
        Me.Controls.Add(Me.gbTC)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TbName)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "GearboxForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F_GBX"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.GrGearShift.ResumeLayout(false)
        Me.GrGearShift.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.PnTorqRes.ResumeLayout(false)
        Me.PnTorqRes.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        Me.CmOpenFile.ResumeLayout(false)
        Me.gbTC.ResumeLayout(false)
        Me.gbTC.PerformLayout
        Me.pnTcEngineering.ResumeLayout(false)
        Me.pnTcEngineering.PerformLayout
        Me.GroupBox4.ResumeLayout(false)
        Me.GroupBox4.PerformLayout
        Me.PnInertiaTI.ResumeLayout(false)
        Me.PnInertiaTI.PerformLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.gbPowershiftLosses.ResumeLayout(false)
        Me.gbPowershiftLosses.PerformLayout
        Me.gbTCAccMin.ResumeLayout(false)
        Me.gbTCAccMin.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents LbStatus As ToolStripStatusLabel
	Friend WithEvents ButCancel As Button
	Friend WithEvents ButOK As Button
	Friend WithEvents BtRemGear As Button
	Friend WithEvents TbTracInt As TextBox
	Friend WithEvents LvGears As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents TBI_getr As TextBox
	Friend WithEvents Label49 As Label
	Friend WithEvents Label33 As Label
	Friend WithEvents Label48 As Label
	Friend WithEvents Label6 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents TbName As TextBox
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents GrGearShift As GroupBox
	Friend WithEvents Label2 As Label
	Friend WithEvents TbTqResvStart As TextBox
	Friend WithEvents TbTqResv As TextBox
	Friend WithEvents Label7 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents GroupBox2 As GroupBox
	Friend WithEvents TbStartAcc As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents TbStartSpeed As TextBox
	Friend WithEvents Label9 As Label
	Friend WithEvents Label10 As Label
	Friend WithEvents Label8 As Label
	Friend WithEvents TbMinTimeBetweenShifts As TextBox
	Friend WithEvents Label12 As Label
	Friend WithEvents Label13 As Label
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents gbTC As GroupBox
	Friend WithEvents TbTCfile As TextBox
	Friend WithEvents BtTCfileBrowse As Button
	Friend WithEvents BtTCfileOpen As Button
	Friend WithEvents Label16 As Label
	Friend WithEvents CbGStype As ComboBox
	Friend WithEvents Label17 As Label
	Friend WithEvents PnTorqRes As Panel
	Friend WithEvents BtAddGear As Button
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents GroupBox4 As GroupBox
	Friend WithEvents Label32 As Label
	Friend WithEvents PnInertiaTI As Panel
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents tbUpshiftMinAcceleration As TextBox
	Friend WithEvents tbUpshiftAfterDownshift As TextBox
	Friend WithEvents tbDownshiftAfterUpshift As TextBox
	Friend WithEvents Label24 As Label
	Friend WithEvents Label23 As Label
	Friend WithEvents Label22 As Label
	Friend WithEvents Label21 As Label
	Friend WithEvents Label20 As Label
	Friend WithEvents Label19 As Label
	Friend WithEvents Label25 As System.Windows.Forms.Label
	Friend WithEvents gbPowershiftLosses As System.Windows.Forms.GroupBox
	Friend WithEvents Label28 As System.Windows.Forms.Label
	Friend WithEvents tbATShiftTime As System.Windows.Forms.TextBox
	Friend WithEvents lbAtShiftTime As System.Windows.Forms.Label
	Friend WithEvents tbTCLUpshiftMinAcceleration As System.Windows.Forms.TextBox
	Friend WithEvents Label30 As System.Windows.Forms.Label
	Friend WithEvents Label31 As System.Windows.Forms.Label
	Friend WithEvents tbTCCUpshiftMinAcceleration As System.Windows.Forms.TextBox
	Friend WithEvents Label34 As System.Windows.Forms.Label
	Friend WithEvents Label35 As System.Windows.Forms.Label
	Friend WithEvents gbTCAccMin As System.Windows.Forms.GroupBox
	Friend WithEvents lblGbxInfo As System.Windows.Forms.Label
	Friend WithEvents btnExportXML As System.Windows.Forms.Button
	Friend WithEvents btnExportAxlGearXML As System.Windows.Forms.Button
	Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
	Friend WithEvents pnTcEngineering As System.Windows.Forms.Panel
	Friend WithEvents Label26 As System.Windows.Forms.Label
	Friend WithEvents TBTCShiftPolygon As System.Windows.Forms.TextBox
	Friend WithEvents tbTCmaxSpeed As System.Windows.Forms.TextBox
	Friend WithEvents TbTCrefrpm As System.Windows.Forms.TextBox
	Friend WithEvents Label27 As System.Windows.Forms.Label
	Friend WithEvents Label14 As System.Windows.Forms.Label
	Friend WithEvents BtTCShiftFileBrowse As System.Windows.Forms.Button
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents TbTCinertia As System.Windows.Forms.TextBox
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents Label18 As System.Windows.Forms.Label
	Friend WithEvents LblTCShiftFile As System.Windows.Forms.Label
	Friend WithEvents btExportVGBS As System.Windows.Forms.Button
End Class
