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
Partial Class EngineForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EngineForm))
        Me.TbIdleSpeed = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TbInertia = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ButCancel = New System.Windows.Forms.Button()
        Me.ButOK = New System.Windows.Forms.Button()
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TbDispl = New System.Windows.Forms.TextBox()
        Me.TbName = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TbMAP = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BtMAP = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtMAPopen = New System.Windows.Forms.Button()
        Me.PnInertia = New System.Windows.Forms.Panel()
        Me.GrWHTC = New System.Windows.Forms.GroupBox()
        Me.PnWhtcDeclaration = New System.Windows.Forms.Panel()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.tbRegPerCorrFactor = New System.Windows.Forms.TextBox()
        Me.lblColdHotFactor = New System.Windows.Forms.Label()
        Me.TbColdHotFactor = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TbWHTCmw = New System.Windows.Forms.TextBox()
        Me.TbWHTCurban = New System.Windows.Forms.TextBox()
        Me.TbWHTCrural = New System.Windows.Forms.TextBox()
        Me.PnWhtcEngineering = New System.Windows.Forms.Panel()
        Me.TbWHTCEngineering = New System.Windows.Forms.TextBox()
        Me.lblWhtcEngineering = New System.Windows.Forms.Label()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.TbFLD = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.BtFLD = New System.Windows.Forms.Button()
        Me.BtFLDopen = New System.Windows.Forms.Button()
        Me.tbRatedSpeed = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.tbRatedPower = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.tbMaxTorque = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.cbFuelType = New System.Windows.Forms.ComboBox()
        Me.lblEngineCharacteristics = New System.Windows.Forms.Label()
        Me.tbDualFuel = New System.Windows.Forms.TabControl()
        Me.tpPrimaryfuel = New System.Windows.Forms.TabPage()
        Me.tpSecondaryFuel = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.pnWhtcFuel2 = New System.Windows.Forms.Panel()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.tbRegPerFuel2 = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.tbColdHotFuel2 = New System.Windows.Forms.TextBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.tbWhtcMotorwayFuel2 = New System.Windows.Forms.TextBox()
        Me.tbWhtcUrbanFuel2 = New System.Windows.Forms.TextBox()
        Me.tbWhtcRuralFuel2 = New System.Windows.Forms.TextBox()
        Me.pnEngCFFuel2 = New System.Windows.Forms.Panel()
        Me.tbEngineeringCFFuel2 = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.tbMapFuel2 = New System.Windows.Forms.TextBox()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.btMapFuel2 = New System.Windows.Forms.Button()
        Me.btMapOpenFuel2 = New System.Windows.Forms.Button()
        Me.cbFuelType2 = New System.Windows.Forms.ComboBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.cbDualFuel = New System.Windows.Forms.CheckBox()
        Me.tbWHR = New System.Windows.Forms.TabControl()
        Me.tbElectricalWHR = New System.Windows.Forms.TabPage()
        Me.tbMechanicalWHR = New System.Windows.Forms.TabPage()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.pnElWHRDeclaration = New System.Windows.Forms.Panel()
        Me.lblWHRRegPer = New System.Windows.Forms.Label()
        Me.tbElWHRRegPer = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.tbElWHRColdHot = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.tbElWHRMotorway = New System.Windows.Forms.TextBox()
        Me.tbElWHRUrban = New System.Windows.Forms.TextBox()
        Me.tbElWHRRural = New System.Windows.Forms.TextBox()
        Me.pnElWhrEngineering = New System.Windows.Forms.Panel()
        Me.tbElWHREngineering = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.pnMechWhrDeclaration = New System.Windows.Forms.Panel()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.tbMechWHRRegPerCF = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.tbMechWHRBFColdHot = New System.Windows.Forms.TextBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.tbMechWHRMotorway = New System.Windows.Forms.TextBox()
        Me.tbMechWHRUrban = New System.Windows.Forms.TextBox()
        Me.tbMechWHRRural = New System.Windows.Forms.TextBox()
        Me.pnMechWhrEngineering = New System.Windows.Forms.Panel()
        Me.tbMechWHREngineering = New System.Windows.Forms.TextBox()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.cbMechWHRInMap = New System.Windows.Forms.CheckBox()
        Me.cbMechWHRNotConnectedCrankshaft = New System.Windows.Forms.CheckBox()
        Me.cbElWHR = New System.Windows.Forms.CheckBox()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.PnInertia.SuspendLayout
        Me.GrWHTC.SuspendLayout
        Me.PnWhtcDeclaration.SuspendLayout
        Me.PnWhtcEngineering.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.tbDualFuel.SuspendLayout
        Me.tpPrimaryfuel.SuspendLayout
        Me.tpSecondaryFuel.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.pnWhtcFuel2.SuspendLayout
        Me.pnEngCFFuel2.SuspendLayout
        Me.tbWHR.SuspendLayout
        Me.tbElectricalWHR.SuspendLayout
        Me.tbMechanicalWHR.SuspendLayout
        Me.pnElWHRDeclaration.SuspendLayout
        Me.pnElWhrEngineering.SuspendLayout
        Me.pnMechWhrDeclaration.SuspendLayout
        Me.pnMechWhrEngineering.SuspendLayout
        Me.TabPage1.SuspendLayout
        Me.SuspendLayout
        '
        'TbIdleSpeed
        '
        Me.TbIdleSpeed.Location = New System.Drawing.Point(123, 108)
        Me.TbIdleSpeed.Name = "TbIdleSpeed"
        Me.TbIdleSpeed.Size = New System.Drawing.Size(57, 20)
        Me.TbIdleSpeed.TabIndex = 1
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(15, 111)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(102, 13)
        Me.Label11.TabIndex = 15
        Me.Label11.Text = "Idling Engine Speed"
        '
        'TbInertia
        '
        Me.TbInertia.Location = New System.Drawing.Point(111, 4)
        Me.TbInertia.Name = "TbInertia"
        Me.TbInertia.Size = New System.Drawing.Size(57, 20)
        Me.TbInertia.TabIndex = 3
        '
        'Label41
        '
        Me.Label41.AutoSize = true
        Me.Label41.Location = New System.Drawing.Point(174, 7)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(36, 13)
        Me.Label41.TabIndex = 24
        Me.Label41.Text = "[kgm²]"
        '
        'Label40
        '
        Me.Label40.AutoSize = true
        Me.Label40.Location = New System.Drawing.Point(186, 111)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(30, 13)
        Me.Label40.TabIndex = 24
        Me.Label40.Text = "[rpm]"
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(3, 7)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Inertia incl. Flywheel"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(916, 552)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 13
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(835, 552)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 12
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1003, 25)
        Me.ToolStrip1.TabIndex = 30
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
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 578)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1003, 22)
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
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(186, 137)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "[ccm]"
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(46, 137)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(71, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Displacement"
        '
        'TbDispl
        '
        Me.TbDispl.Location = New System.Drawing.Point(123, 134)
        Me.TbDispl.Name = "TbDispl"
        Me.TbDispl.Size = New System.Drawing.Size(57, 20)
        Me.TbDispl.TabIndex = 2
        '
        'TbName
        '
        Me.TbName.Location = New System.Drawing.Point(123, 82)
        Me.TbName.Name = "TbName"
        Me.TbName.Size = New System.Drawing.Size(370, 20)
        Me.TbName.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(30, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Make and Model"
        '
        'TbMAP
        '
        Me.TbMAP.Location = New System.Drawing.Point(6, 57)
        Me.TbMAP.Name = "TbMAP"
        Me.TbMAP.Size = New System.Drawing.Size(406, 20)
        Me.TbMAP.TabIndex = 5
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(6, 41)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(115, 13)
        Me.Label6.TabIndex = 38
        Me.Label6.Text = "Fuel Consumption Map"
        '
        'BtMAP
        '
        Me.BtMAP.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtMAP.Location = New System.Drawing.Point(418, 55)
        Me.BtMAP.Name = "BtMAP"
        Me.BtMAP.Size = New System.Drawing.Size(24, 24)
        Me.BtMAP.TabIndex = 6
        Me.BtMAP.TabStop = false
        Me.BtMAP.UseVisualStyleBackColor = true
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_ENG
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(502, 40)
        Me.PictureBox1.TabIndex = 39
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
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
        'BtMAPopen
        '
        Me.BtMAPopen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtMAPopen.Location = New System.Drawing.Point(441, 55)
        Me.BtMAPopen.Name = "BtMAPopen"
        Me.BtMAPopen.Size = New System.Drawing.Size(24, 24)
        Me.BtMAPopen.TabIndex = 7
        Me.BtMAPopen.TabStop = false
        Me.BtMAPopen.UseVisualStyleBackColor = true
        '
        'PnInertia
        '
        Me.PnInertia.Controls.Add(Me.Label5)
        Me.PnInertia.Controls.Add(Me.Label41)
        Me.PnInertia.Controls.Add(Me.TbInertia)
        Me.PnInertia.Location = New System.Drawing.Point(272, 188)
        Me.PnInertia.Name = "PnInertia"
        Me.PnInertia.Size = New System.Drawing.Size(212, 30)
        Me.PnInertia.TabIndex = 3
        '
        'GrWHTC
        '
        Me.GrWHTC.Controls.Add(Me.PnWhtcDeclaration)
        Me.GrWHTC.Controls.Add(Me.PnWhtcEngineering)
        Me.GrWHTC.Location = New System.Drawing.Point(3, 94)
        Me.GrWHTC.Name = "GrWHTC"
        Me.GrWHTC.Size = New System.Drawing.Size(462, 135)
        Me.GrWHTC.TabIndex = 9
        Me.GrWHTC.TabStop = false
        Me.GrWHTC.Text = "Fuel Consumption Correction Factors"
        '
        'PnWhtcDeclaration
        '
        Me.PnWhtcDeclaration.Controls.Add(Me.Label20)
        Me.PnWhtcDeclaration.Controls.Add(Me.tbRegPerCorrFactor)
        Me.PnWhtcDeclaration.Controls.Add(Me.lblColdHotFactor)
        Me.PnWhtcDeclaration.Controls.Add(Me.TbColdHotFactor)
        Me.PnWhtcDeclaration.Controls.Add(Me.Label4)
        Me.PnWhtcDeclaration.Controls.Add(Me.Label7)
        Me.PnWhtcDeclaration.Controls.Add(Me.Label13)
        Me.PnWhtcDeclaration.Controls.Add(Me.Label8)
        Me.PnWhtcDeclaration.Controls.Add(Me.TbWHTCmw)
        Me.PnWhtcDeclaration.Controls.Add(Me.TbWHTCurban)
        Me.PnWhtcDeclaration.Controls.Add(Me.TbWHTCrural)
        Me.PnWhtcDeclaration.Location = New System.Drawing.Point(3, 19)
        Me.PnWhtcDeclaration.Name = "PnWhtcDeclaration"
        Me.PnWhtcDeclaration.Size = New System.Drawing.Size(455, 74)
        Me.PnWhtcDeclaration.TabIndex = 8
        '
        'Label20
        '
        Me.Label20.AutoSize = true
        Me.Label20.Location = New System.Drawing.Point(296, 49)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(89, 13)
        Me.Label20.TabIndex = 9
        Me.Label20.Text = "Reg. Per. Corr. F."
        '
        'tbRegPerCorrFactor
        '
        Me.tbRegPerCorrFactor.Location = New System.Drawing.Point(391, 46)
        Me.tbRegPerCorrFactor.Name = "tbRegPerCorrFactor"
        Me.tbRegPerCorrFactor.Size = New System.Drawing.Size(57, 20)
        Me.tbRegPerCorrFactor.TabIndex = 10
        '
        'lblColdHotFactor
        '
        Me.lblColdHotFactor.AutoSize = true
        Me.lblColdHotFactor.Location = New System.Drawing.Point(42, 49)
        Me.lblColdHotFactor.Name = "lblColdHotFactor"
        Me.lblColdHotFactor.Size = New System.Drawing.Size(177, 13)
        Me.lblColdHotFactor.TabIndex = 5
        Me.lblColdHotFactor.Text = "Cold/Hot Emission Balancing Factor"
        '
        'TbColdHotFactor
        '
        Me.TbColdHotFactor.Location = New System.Drawing.Point(225, 46)
        Me.TbColdHotFactor.Name = "TbColdHotFactor"
        Me.TbColdHotFactor.Size = New System.Drawing.Size(57, 20)
        Me.TbColdHotFactor.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(3, 23)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 13)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "WHTC Urban"
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(151, 23)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(68, 13)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "WHTC Rural"
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(1, 1)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(242, 13)
        Me.Label13.TabIndex = 3
        Me.Label13.Text = "Correction Factors calculated with VECTO-Engine"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(296, 23)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(89, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "WHTC Motorway"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TbWHTCmw
        '
        Me.TbWHTCmw.Location = New System.Drawing.Point(391, 20)
        Me.TbWHTCmw.Name = "TbWHTCmw"
        Me.TbWHTCmw.Size = New System.Drawing.Size(57, 20)
        Me.TbWHTCmw.TabIndex = 2
        '
        'TbWHTCurban
        '
        Me.TbWHTCurban.Location = New System.Drawing.Point(81, 20)
        Me.TbWHTCurban.Name = "TbWHTCurban"
        Me.TbWHTCurban.Size = New System.Drawing.Size(57, 20)
        Me.TbWHTCurban.TabIndex = 0
        '
        'TbWHTCrural
        '
        Me.TbWHTCrural.Location = New System.Drawing.Point(225, 20)
        Me.TbWHTCrural.Name = "TbWHTCrural"
        Me.TbWHTCrural.Size = New System.Drawing.Size(57, 20)
        Me.TbWHTCrural.TabIndex = 1
        '
        'PnWhtcEngineering
        '
        Me.PnWhtcEngineering.Controls.Add(Me.TbWHTCEngineering)
        Me.PnWhtcEngineering.Controls.Add(Me.lblWhtcEngineering)
        Me.PnWhtcEngineering.Location = New System.Drawing.Point(3, 99)
        Me.PnWhtcEngineering.Name = "PnWhtcEngineering"
        Me.PnWhtcEngineering.Size = New System.Drawing.Size(455, 30)
        Me.PnWhtcEngineering.TabIndex = 7
        '
        'TbWHTCEngineering
        '
        Me.TbWHTCEngineering.Location = New System.Drawing.Point(225, 3)
        Me.TbWHTCEngineering.Name = "TbWHTCEngineering"
        Me.TbWHTCEngineering.Size = New System.Drawing.Size(57, 20)
        Me.TbWHTCEngineering.TabIndex = 5
        '
        'lblWhtcEngineering
        '
        Me.lblWhtcEngineering.AutoSize = true
        Me.lblWhtcEngineering.Location = New System.Drawing.Point(156, 6)
        Me.lblWhtcEngineering.Name = "lblWhtcEngineering"
        Me.lblWhtcEngineering.Size = New System.Drawing.Size(63, 13)
        Me.lblWhtcEngineering.TabIndex = 6
        Me.lblWhtcEngineering.Text = "Engineering"
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(503, 28)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(482, 323)
        Me.PicBox.TabIndex = 40
        Me.PicBox.TabStop = false
        '
        'TbFLD
        '
        Me.TbFLD.Location = New System.Drawing.Point(12, 241)
        Me.TbFLD.Name = "TbFLD"
        Me.TbFLD.Size = New System.Drawing.Size(434, 20)
        Me.TbFLD.TabIndex = 5
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(12, 225)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(128, 13)
        Me.Label14.TabIndex = 38
        Me.Label14.Text = "Full Load and Drag Curve"
        '
        'BtFLD
        '
        Me.BtFLD.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtFLD.Location = New System.Drawing.Point(446, 239)
        Me.BtFLD.Name = "BtFLD"
        Me.BtFLD.Size = New System.Drawing.Size(24, 24)
        Me.BtFLD.TabIndex = 6
        Me.BtFLD.TabStop = false
        Me.BtFLD.UseVisualStyleBackColor = true
        '
        'BtFLDopen
        '
        Me.BtFLDopen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtFLDopen.Location = New System.Drawing.Point(469, 239)
        Me.BtFLDopen.Name = "BtFLDopen"
        Me.BtFLDopen.Size = New System.Drawing.Size(24, 24)
        Me.BtFLDopen.TabIndex = 7
        Me.BtFLDopen.TabStop = false
        Me.BtFLDopen.UseVisualStyleBackColor = true
        '
        'tbRatedSpeed
        '
        Me.tbRatedSpeed.Location = New System.Drawing.Point(381, 108)
        Me.tbRatedSpeed.Name = "tbRatedSpeed"
        Me.tbRatedSpeed.Size = New System.Drawing.Size(57, 20)
        Me.tbRatedSpeed.TabIndex = 41
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(304, 111)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(70, 13)
        Me.Label9.TabIndex = 42
        Me.Label9.Text = "Rated Speed"
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(444, 111)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(30, 13)
        Me.Label10.TabIndex = 43
        Me.Label10.Text = "[rpm]"
        '
        'tbRatedPower
        '
        Me.tbRatedPower.Location = New System.Drawing.Point(381, 134)
        Me.tbRatedPower.Name = "tbRatedPower"
        Me.tbRatedPower.Size = New System.Drawing.Size(57, 20)
        Me.tbRatedPower.TabIndex = 44
        '
        'Label12
        '
        Me.Label12.AutoSize = true
        Me.Label12.Location = New System.Drawing.Point(304, 137)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(69, 13)
        Me.Label12.TabIndex = 45
        Me.Label12.Text = "Rated Power"
        '
        'Label15
        '
        Me.Label15.AutoSize = true
        Me.Label15.Location = New System.Drawing.Point(444, 137)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(30, 13)
        Me.Label15.TabIndex = 46
        Me.Label15.Text = "[kW]"
        '
        'tbMaxTorque
        '
        Me.tbMaxTorque.Location = New System.Drawing.Point(381, 158)
        Me.tbMaxTorque.Name = "tbMaxTorque"
        Me.tbMaxTorque.Size = New System.Drawing.Size(57, 20)
        Me.tbMaxTorque.TabIndex = 47
        '
        'Label16
        '
        Me.Label16.AutoSize = true
        Me.Label16.Location = New System.Drawing.Point(309, 161)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(64, 13)
        Me.Label16.TabIndex = 48
        Me.Label16.Text = "Max Torque"
        '
        'Label17
        '
        Me.Label17.AutoSize = true
        Me.Label17.Location = New System.Drawing.Point(444, 161)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(29, 13)
        Me.Label17.TabIndex = 49
        Me.Label17.Text = "[Nm]"
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(11, 13)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(54, 13)
        Me.Label18.TabIndex = 50
        Me.Label18.Text = "Fuel Type"
        '
        'cbFuelType
        '
        Me.cbFuelType.FormattingEnabled = true
        Me.cbFuelType.Location = New System.Drawing.Point(71, 10)
        Me.cbFuelType.Name = "cbFuelType"
        Me.cbFuelType.Size = New System.Drawing.Size(143, 21)
        Me.cbFuelType.TabIndex = 51
        '
        'lblEngineCharacteristics
        '
        Me.lblEngineCharacteristics.AutoSize = true
        Me.lblEngineCharacteristics.Location = New System.Drawing.Point(506, 354)
        Me.lblEngineCharacteristics.Name = "lblEngineCharacteristics"
        Me.lblEngineCharacteristics.Size = New System.Drawing.Size(0, 13)
        Me.lblEngineCharacteristics.TabIndex = 52
        '
        'tbDualFuel
        '
        Me.tbDualFuel.Controls.Add(Me.tpPrimaryfuel)
        Me.tbDualFuel.Controls.Add(Me.tpSecondaryFuel)
        Me.tbDualFuel.Location = New System.Drawing.Point(12, 268)
        Me.tbDualFuel.Name = "tbDualFuel"
        Me.tbDualFuel.SelectedIndex = 0
        Me.tbDualFuel.Size = New System.Drawing.Size(481, 279)
        Me.tbDualFuel.TabIndex = 56
        '
        'tpPrimaryfuel
        '
        Me.tpPrimaryfuel.Controls.Add(Me.GrWHTC)
        Me.tpPrimaryfuel.Controls.Add(Me.TbMAP)
        Me.tpPrimaryfuel.Controls.Add(Me.Label6)
        Me.tpPrimaryfuel.Controls.Add(Me.BtMAP)
        Me.tpPrimaryfuel.Controls.Add(Me.BtMAPopen)
        Me.tpPrimaryfuel.Controls.Add(Me.cbFuelType)
        Me.tpPrimaryfuel.Controls.Add(Me.Label18)
        Me.tpPrimaryfuel.Location = New System.Drawing.Point(4, 22)
        Me.tpPrimaryfuel.Name = "tpPrimaryfuel"
        Me.tpPrimaryfuel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpPrimaryfuel.Size = New System.Drawing.Size(473, 253)
        Me.tpPrimaryfuel.TabIndex = 0
        Me.tpPrimaryfuel.Text = "Primary Fuel"
        Me.tpPrimaryfuel.UseVisualStyleBackColor = true
        '
        'tpSecondaryFuel
        '
        Me.tpSecondaryFuel.Controls.Add(Me.GroupBox1)
        Me.tpSecondaryFuel.Controls.Add(Me.tbMapFuel2)
        Me.tpSecondaryFuel.Controls.Add(Me.Label34)
        Me.tpSecondaryFuel.Controls.Add(Me.btMapFuel2)
        Me.tpSecondaryFuel.Controls.Add(Me.btMapOpenFuel2)
        Me.tpSecondaryFuel.Controls.Add(Me.cbFuelType2)
        Me.tpSecondaryFuel.Controls.Add(Me.Label35)
        Me.tpSecondaryFuel.Location = New System.Drawing.Point(4, 22)
        Me.tpSecondaryFuel.Name = "tpSecondaryFuel"
        Me.tpSecondaryFuel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSecondaryFuel.Size = New System.Drawing.Size(473, 253)
        Me.tpSecondaryFuel.TabIndex = 1
        Me.tpSecondaryFuel.Text = "Secondary Fuel"
        Me.tpSecondaryFuel.UseVisualStyleBackColor = true
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.pnWhtcFuel2)
        Me.GroupBox1.Controls.Add(Me.pnEngCFFuel2)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 85)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(462, 135)
        Me.GroupBox1.TabIndex = 55
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Fuel Consumption Correction Factors"
        '
        'pnWhtcFuel2
        '
        Me.pnWhtcFuel2.Controls.Add(Me.Label27)
        Me.pnWhtcFuel2.Controls.Add(Me.tbRegPerFuel2)
        Me.pnWhtcFuel2.Controls.Add(Me.Label28)
        Me.pnWhtcFuel2.Controls.Add(Me.tbColdHotFuel2)
        Me.pnWhtcFuel2.Controls.Add(Me.Label29)
        Me.pnWhtcFuel2.Controls.Add(Me.Label30)
        Me.pnWhtcFuel2.Controls.Add(Me.Label31)
        Me.pnWhtcFuel2.Controls.Add(Me.Label32)
        Me.pnWhtcFuel2.Controls.Add(Me.tbWhtcMotorwayFuel2)
        Me.pnWhtcFuel2.Controls.Add(Me.tbWhtcUrbanFuel2)
        Me.pnWhtcFuel2.Controls.Add(Me.tbWhtcRuralFuel2)
        Me.pnWhtcFuel2.Location = New System.Drawing.Point(3, 19)
        Me.pnWhtcFuel2.Name = "pnWhtcFuel2"
        Me.pnWhtcFuel2.Size = New System.Drawing.Size(455, 74)
        Me.pnWhtcFuel2.TabIndex = 8
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(296, 49)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(89, 13)
        Me.Label27.TabIndex = 9
        Me.Label27.Text = "Reg. Per. Corr. F."
        '
        'tbRegPerFuel2
        '
        Me.tbRegPerFuel2.Location = New System.Drawing.Point(391, 46)
        Me.tbRegPerFuel2.Name = "tbRegPerFuel2"
        Me.tbRegPerFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbRegPerFuel2.TabIndex = 10
        '
        'Label28
        '
        Me.Label28.AutoSize = true
        Me.Label28.Location = New System.Drawing.Point(42, 49)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(177, 13)
        Me.Label28.TabIndex = 5
        Me.Label28.Text = "Cold/Hot Emission Balancing Factor"
        '
        'tbColdHotFuel2
        '
        Me.tbColdHotFuel2.Location = New System.Drawing.Point(225, 46)
        Me.tbColdHotFuel2.Name = "tbColdHotFuel2"
        Me.tbColdHotFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbColdHotFuel2.TabIndex = 6
        '
        'Label29
        '
        Me.Label29.AutoSize = true
        Me.Label29.Location = New System.Drawing.Point(3, 23)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(72, 13)
        Me.Label29.TabIndex = 0
        Me.Label29.Text = "WHTC Urban"
        '
        'Label30
        '
        Me.Label30.AutoSize = true
        Me.Label30.Location = New System.Drawing.Point(151, 23)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(68, 13)
        Me.Label30.TabIndex = 0
        Me.Label30.Text = "WHTC Rural"
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(1, 1)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(242, 13)
        Me.Label31.TabIndex = 3
        Me.Label31.Text = "Correction Factors calculated with VECTO-Engine"
        '
        'Label32
        '
        Me.Label32.AutoSize = true
        Me.Label32.Location = New System.Drawing.Point(296, 23)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(89, 13)
        Me.Label32.TabIndex = 0
        Me.Label32.Text = "WHTC Motorway"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbWhtcMotorwayFuel2
        '
        Me.tbWhtcMotorwayFuel2.Location = New System.Drawing.Point(391, 20)
        Me.tbWhtcMotorwayFuel2.Name = "tbWhtcMotorwayFuel2"
        Me.tbWhtcMotorwayFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbWhtcMotorwayFuel2.TabIndex = 2
        '
        'tbWhtcUrbanFuel2
        '
        Me.tbWhtcUrbanFuel2.Location = New System.Drawing.Point(81, 20)
        Me.tbWhtcUrbanFuel2.Name = "tbWhtcUrbanFuel2"
        Me.tbWhtcUrbanFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbWhtcUrbanFuel2.TabIndex = 0
        '
        'tbWhtcRuralFuel2
        '
        Me.tbWhtcRuralFuel2.Location = New System.Drawing.Point(225, 20)
        Me.tbWhtcRuralFuel2.Name = "tbWhtcRuralFuel2"
        Me.tbWhtcRuralFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbWhtcRuralFuel2.TabIndex = 1
        '
        'pnEngCFFuel2
        '
        Me.pnEngCFFuel2.Controls.Add(Me.tbEngineeringCFFuel2)
        Me.pnEngCFFuel2.Controls.Add(Me.Label33)
        Me.pnEngCFFuel2.Location = New System.Drawing.Point(3, 99)
        Me.pnEngCFFuel2.Name = "pnEngCFFuel2"
        Me.pnEngCFFuel2.Size = New System.Drawing.Size(455, 30)
        Me.pnEngCFFuel2.TabIndex = 7
        '
        'tbEngineeringCFFuel2
        '
        Me.tbEngineeringCFFuel2.Location = New System.Drawing.Point(225, 3)
        Me.tbEngineeringCFFuel2.Name = "tbEngineeringCFFuel2"
        Me.tbEngineeringCFFuel2.Size = New System.Drawing.Size(57, 20)
        Me.tbEngineeringCFFuel2.TabIndex = 5
        '
        'Label33
        '
        Me.Label33.AutoSize = true
        Me.Label33.Location = New System.Drawing.Point(156, 6)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(63, 13)
        Me.Label33.TabIndex = 6
        Me.Label33.Text = "Engineering"
        '
        'tbMapFuel2
        '
        Me.tbMapFuel2.Location = New System.Drawing.Point(6, 54)
        Me.tbMapFuel2.Name = "tbMapFuel2"
        Me.tbMapFuel2.Size = New System.Drawing.Size(406, 20)
        Me.tbMapFuel2.TabIndex = 52
        '
        'Label34
        '
        Me.Label34.AutoSize = true
        Me.Label34.Location = New System.Drawing.Point(6, 38)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(115, 13)
        Me.Label34.TabIndex = 56
        Me.Label34.Text = "Fuel Consumption Map"
        '
        'btMapFuel2
        '
        Me.btMapFuel2.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btMapFuel2.Location = New System.Drawing.Point(418, 52)
        Me.btMapFuel2.Name = "btMapFuel2"
        Me.btMapFuel2.Size = New System.Drawing.Size(24, 24)
        Me.btMapFuel2.TabIndex = 53
        Me.btMapFuel2.TabStop = false
        Me.btMapFuel2.UseVisualStyleBackColor = true
        '
        'btMapOpenFuel2
        '
        Me.btMapOpenFuel2.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btMapOpenFuel2.Location = New System.Drawing.Point(441, 52)
        Me.btMapOpenFuel2.Name = "btMapOpenFuel2"
        Me.btMapOpenFuel2.Size = New System.Drawing.Size(24, 24)
        Me.btMapOpenFuel2.TabIndex = 54
        Me.btMapOpenFuel2.TabStop = false
        Me.btMapOpenFuel2.UseVisualStyleBackColor = true
        '
        'cbFuelType2
        '
        Me.cbFuelType2.FormattingEnabled = true
        Me.cbFuelType2.Location = New System.Drawing.Point(71, 6)
        Me.cbFuelType2.Name = "cbFuelType2"
        Me.cbFuelType2.Size = New System.Drawing.Size(143, 21)
        Me.cbFuelType2.TabIndex = 58
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(11, 9)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(54, 13)
        Me.Label35.TabIndex = 57
        Me.Label35.Text = "Fuel Type"
        '
        'cbDualFuel
        '
        Me.cbDualFuel.AutoSize = true
        Me.cbDualFuel.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cbDualFuel.Location = New System.Drawing.Point(30, 160)
        Me.cbDualFuel.Name = "cbDualFuel"
        Me.cbDualFuel.Size = New System.Drawing.Size(107, 17)
        Me.cbDualFuel.TabIndex = 58
        Me.cbDualFuel.Text = "Dual Fuel Engine"
        Me.cbDualFuel.UseVisualStyleBackColor = true
        '
        'tbWHR
        '
        Me.tbWHR.Controls.Add(Me.TabPage1)
        Me.tbWHR.Controls.Add(Me.tbElectricalWHR)
        Me.tbWHR.Controls.Add(Me.tbMechanicalWHR)
        Me.tbWHR.Location = New System.Drawing.Point(503, 376)
        Me.tbWHR.Name = "tbWHR"
        Me.tbWHR.SelectedIndex = 0
        Me.tbWHR.Size = New System.Drawing.Size(482, 171)
        Me.tbWHR.TabIndex = 59
        '
        'tbElectricalWHR
        '
        Me.tbElectricalWHR.Controls.Add(Me.Label36)
        Me.tbElectricalWHR.Controls.Add(Me.pnElWHRDeclaration)
        Me.tbElectricalWHR.Controls.Add(Me.pnElWhrEngineering)
        Me.tbElectricalWHR.Location = New System.Drawing.Point(4, 22)
        Me.tbElectricalWHR.Name = "tbElectricalWHR"
        Me.tbElectricalWHR.Padding = New System.Windows.Forms.Padding(3)
        Me.tbElectricalWHR.Size = New System.Drawing.Size(474, 145)
        Me.tbElectricalWHR.TabIndex = 0
        Me.tbElectricalWHR.Text = "Correction Factors Electrical WHR"
        Me.tbElectricalWHR.UseVisualStyleBackColor = true
        '
        'tbMechanicalWHR
        '
        Me.tbMechanicalWHR.Controls.Add(Me.Label37)
        Me.tbMechanicalWHR.Controls.Add(Me.pnMechWhrDeclaration)
        Me.tbMechanicalWHR.Controls.Add(Me.pnMechWhrEngineering)
        Me.tbMechanicalWHR.Location = New System.Drawing.Point(4, 22)
        Me.tbMechanicalWHR.Name = "tbMechanicalWHR"
        Me.tbMechanicalWHR.Padding = New System.Windows.Forms.Padding(3)
        Me.tbMechanicalWHR.Size = New System.Drawing.Size(474, 145)
        Me.tbMechanicalWHR.TabIndex = 1
        Me.tbMechanicalWHR.Text = "Correction Factors Mechanical WHR"
        Me.tbMechanicalWHR.UseVisualStyleBackColor = true
        '
        'Label36
        '
        Me.Label36.AutoSize = true
        Me.Label36.Location = New System.Drawing.Point(7, 119)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(408, 13)
        Me.Label36.TabIndex = 14
        Me.Label36.Text = "Note: Electric power generated by WHR has to be provided in FC-Map of primary fue"& _ 
    "l"
        '
        'pnElWHRDeclaration
        '
        Me.pnElWHRDeclaration.Controls.Add(Me.lblWHRRegPer)
        Me.pnElWHRDeclaration.Controls.Add(Me.tbElWHRRegPer)
        Me.pnElWHRDeclaration.Controls.Add(Me.Label21)
        Me.pnElWHRDeclaration.Controls.Add(Me.tbElWHRColdHot)
        Me.pnElWHRDeclaration.Controls.Add(Me.Label22)
        Me.pnElWHRDeclaration.Controls.Add(Me.Label23)
        Me.pnElWHRDeclaration.Controls.Add(Me.Label24)
        Me.pnElWHRDeclaration.Controls.Add(Me.Label25)
        Me.pnElWHRDeclaration.Controls.Add(Me.tbElWHRMotorway)
        Me.pnElWHRDeclaration.Controls.Add(Me.tbElWHRUrban)
        Me.pnElWHRDeclaration.Controls.Add(Me.tbElWHRRural)
        Me.pnElWHRDeclaration.Location = New System.Drawing.Point(6, 6)
        Me.pnElWHRDeclaration.Name = "pnElWHRDeclaration"
        Me.pnElWHRDeclaration.Size = New System.Drawing.Size(458, 74)
        Me.pnElWHRDeclaration.TabIndex = 13
        '
        'lblWHRRegPer
        '
        Me.lblWHRRegPer.AutoSize = true
        Me.lblWHRRegPer.Location = New System.Drawing.Point(280, 49)
        Me.lblWHRRegPer.Name = "lblWHRRegPer"
        Me.lblWHRRegPer.Size = New System.Drawing.Size(89, 13)
        Me.lblWHRRegPer.TabIndex = 11
        Me.lblWHRRegPer.Text = "Reg. Per. Corr. F."
        '
        'tbElWHRRegPer
        '
        Me.tbElWHRRegPer.Location = New System.Drawing.Point(376, 46)
        Me.tbElWHRRegPer.Name = "tbElWHRRegPer"
        Me.tbElWHRRegPer.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHRRegPer.TabIndex = 10
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(11, 49)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(177, 13)
        Me.Label21.TabIndex = 5
        Me.Label21.Text = "Cold/Hot Emission Balancing Factor"
        '
        'tbElWHRColdHot
        '
        Me.tbElWHRColdHot.Location = New System.Drawing.Point(194, 46)
        Me.tbElWHRColdHot.Name = "tbElWHRColdHot"
        Me.tbElWHRColdHot.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHRColdHot.TabIndex = 6
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(21, 23)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(36, 13)
        Me.Label22.TabIndex = 0
        Me.Label22.Text = "Urban"
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Location = New System.Drawing.Point(155, 27)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(32, 13)
        Me.Label23.TabIndex = 0
        Me.Label23.Text = "Rural"
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(1, 1)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(242, 13)
        Me.Label24.TabIndex = 3
        Me.Label24.Text = "Correction Factors calculated with VECTO-Engine"
        '
        'Label25
        '
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(316, 23)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(53, 13)
        Me.Label25.TabIndex = 0
        Me.Label25.Text = "Motorway"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbElWHRMotorway
        '
        Me.tbElWHRMotorway.Location = New System.Drawing.Point(376, 20)
        Me.tbElWHRMotorway.Name = "tbElWHRMotorway"
        Me.tbElWHRMotorway.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHRMotorway.TabIndex = 2
        '
        'tbElWHRUrban
        '
        Me.tbElWHRUrban.Location = New System.Drawing.Point(64, 20)
        Me.tbElWHRUrban.Name = "tbElWHRUrban"
        Me.tbElWHRUrban.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHRUrban.TabIndex = 0
        '
        'tbElWHRRural
        '
        Me.tbElWHRRural.Location = New System.Drawing.Point(194, 20)
        Me.tbElWHRRural.Name = "tbElWHRRural"
        Me.tbElWHRRural.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHRRural.TabIndex = 1
        '
        'pnElWhrEngineering
        '
        Me.pnElWhrEngineering.Controls.Add(Me.tbElWHREngineering)
        Me.pnElWhrEngineering.Controls.Add(Me.Label26)
        Me.pnElWhrEngineering.Location = New System.Drawing.Point(6, 86)
        Me.pnElWhrEngineering.Name = "pnElWhrEngineering"
        Me.pnElWhrEngineering.Size = New System.Drawing.Size(458, 30)
        Me.pnElWhrEngineering.TabIndex = 12
        '
        'tbElWHREngineering
        '
        Me.tbElWHREngineering.Location = New System.Drawing.Point(194, 3)
        Me.tbElWHREngineering.Name = "tbElWHREngineering"
        Me.tbElWHREngineering.Size = New System.Drawing.Size(57, 20)
        Me.tbElWHREngineering.TabIndex = 5
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(125, 6)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(63, 13)
        Me.Label26.TabIndex = 6
        Me.Label26.Text = "Engineering"
        '
        'Label37
        '
        Me.Label37.AutoSize = true
        Me.Label37.Location = New System.Drawing.Point(7, 119)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(426, 13)
        Me.Label37.TabIndex = 14
        Me.Label37.Text = "Note: Mechanica power generated by WHR has to be provided in FC-Map of primary fu"& _ 
    "el"
        '
        'pnMechWhrDeclaration
        '
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label38)
        Me.pnMechWhrDeclaration.Controls.Add(Me.tbMechWHRRegPerCF)
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label39)
        Me.pnMechWhrDeclaration.Controls.Add(Me.tbMechWHRBFColdHot)
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label42)
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label43)
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label44)
        Me.pnMechWhrDeclaration.Controls.Add(Me.Label45)
        Me.pnMechWhrDeclaration.Controls.Add(Me.tbMechWHRMotorway)
        Me.pnMechWhrDeclaration.Controls.Add(Me.tbMechWHRUrban)
        Me.pnMechWhrDeclaration.Controls.Add(Me.tbMechWHRRural)
        Me.pnMechWhrDeclaration.Location = New System.Drawing.Point(6, 6)
        Me.pnMechWhrDeclaration.Name = "pnMechWhrDeclaration"
        Me.pnMechWhrDeclaration.Size = New System.Drawing.Size(458, 74)
        Me.pnMechWhrDeclaration.TabIndex = 13
        '
        'Label38
        '
        Me.Label38.AutoSize = true
        Me.Label38.Location = New System.Drawing.Point(280, 49)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(89, 13)
        Me.Label38.TabIndex = 11
        Me.Label38.Text = "Reg. Per. Corr. F."
        '
        'tbMechWHRRegPerCF
        '
        Me.tbMechWHRRegPerCF.Location = New System.Drawing.Point(376, 46)
        Me.tbMechWHRRegPerCF.Name = "tbMechWHRRegPerCF"
        Me.tbMechWHRRegPerCF.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHRRegPerCF.TabIndex = 10
        '
        'Label39
        '
        Me.Label39.AutoSize = true
        Me.Label39.Location = New System.Drawing.Point(11, 49)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(177, 13)
        Me.Label39.TabIndex = 5
        Me.Label39.Text = "Cold/Hot Emission Balancing Factor"
        '
        'tbMechWHRBFColdHot
        '
        Me.tbMechWHRBFColdHot.Location = New System.Drawing.Point(194, 46)
        Me.tbMechWHRBFColdHot.Name = "tbMechWHRBFColdHot"
        Me.tbMechWHRBFColdHot.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHRBFColdHot.TabIndex = 6
        '
        'Label42
        '
        Me.Label42.AutoSize = true
        Me.Label42.Location = New System.Drawing.Point(21, 23)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(36, 13)
        Me.Label42.TabIndex = 0
        Me.Label42.Text = "Urban"
        '
        'Label43
        '
        Me.Label43.AutoSize = true
        Me.Label43.Location = New System.Drawing.Point(155, 27)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(32, 13)
        Me.Label43.TabIndex = 0
        Me.Label43.Text = "Rural"
        '
        'Label44
        '
        Me.Label44.AutoSize = true
        Me.Label44.Location = New System.Drawing.Point(1, 1)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(242, 13)
        Me.Label44.TabIndex = 3
        Me.Label44.Text = "Correction Factors calculated with VECTO-Engine"
        '
        'Label45
        '
        Me.Label45.AutoSize = true
        Me.Label45.Location = New System.Drawing.Point(316, 23)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(53, 13)
        Me.Label45.TabIndex = 0
        Me.Label45.Text = "Motorway"
        Me.Label45.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbMechWHRMotorway
        '
        Me.tbMechWHRMotorway.Location = New System.Drawing.Point(376, 20)
        Me.tbMechWHRMotorway.Name = "tbMechWHRMotorway"
        Me.tbMechWHRMotorway.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHRMotorway.TabIndex = 2
        '
        'tbMechWHRUrban
        '
        Me.tbMechWHRUrban.Location = New System.Drawing.Point(64, 20)
        Me.tbMechWHRUrban.Name = "tbMechWHRUrban"
        Me.tbMechWHRUrban.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHRUrban.TabIndex = 0
        '
        'tbMechWHRRural
        '
        Me.tbMechWHRRural.Location = New System.Drawing.Point(194, 20)
        Me.tbMechWHRRural.Name = "tbMechWHRRural"
        Me.tbMechWHRRural.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHRRural.TabIndex = 1
        '
        'pnMechWhrEngineering
        '
        Me.pnMechWhrEngineering.Controls.Add(Me.tbMechWHREngineering)
        Me.pnMechWhrEngineering.Controls.Add(Me.Label46)
        Me.pnMechWhrEngineering.Location = New System.Drawing.Point(6, 86)
        Me.pnMechWhrEngineering.Name = "pnMechWhrEngineering"
        Me.pnMechWhrEngineering.Size = New System.Drawing.Size(458, 30)
        Me.pnMechWhrEngineering.TabIndex = 12
        '
        'tbMechWHREngineering
        '
        Me.tbMechWHREngineering.Location = New System.Drawing.Point(194, 3)
        Me.tbMechWHREngineering.Name = "tbMechWHREngineering"
        Me.tbMechWHREngineering.Size = New System.Drawing.Size(57, 20)
        Me.tbMechWHREngineering.TabIndex = 5
        '
        'Label46
        '
        Me.Label46.AutoSize = true
        Me.Label46.Location = New System.Drawing.Point(125, 6)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(63, 13)
        Me.Label46.TabIndex = 6
        Me.Label46.Text = "Engineering"
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.cbElWHR)
        Me.TabPage1.Controls.Add(Me.cbMechWHRNotConnectedCrankshaft)
        Me.TabPage1.Controls.Add(Me.cbMechWHRInMap)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(474, 145)
        Me.TabPage1.TabIndex = 2
        Me.TabPage1.Text = "WHR Type"
        Me.TabPage1.UseVisualStyleBackColor = true
        '
        'cbMechWHRInMap
        '
        Me.cbMechWHRInMap.AutoSize = true
        Me.cbMechWHRInMap.Location = New System.Drawing.Point(17, 12)
        Me.cbMechWHRInMap.Name = "cbMechWHRInMap"
        Me.cbMechWHRInMap.Size = New System.Drawing.Size(287, 17)
        Me.cbMechWHRInMap.TabIndex = 0
        Me.cbMechWHRInMap.Text = "Mechanical WHR System incuded in FC measurements"
        Me.cbMechWHRInMap.UseVisualStyleBackColor = true
        '
        'cbMechWHRNotConnectedCrankshaft
        '
        Me.cbMechWHRNotConnectedCrankshaft.AutoSize = true
        Me.cbMechWHRNotConnectedCrankshaft.Location = New System.Drawing.Point(17, 35)
        Me.cbMechWHRNotConnectedCrankshaft.Name = "cbMechWHRNotConnectedCrankshaft"
        Me.cbMechWHRNotConnectedCrankshaft.Size = New System.Drawing.Size(303, 17)
        Me.cbMechWHRNotConnectedCrankshaft.TabIndex = 1
        Me.cbMechWHRNotConnectedCrankshaft.Text = "Mechanical WHR System not connected to the crankshaft"
        Me.cbMechWHRNotConnectedCrankshaft.UseVisualStyleBackColor = true
        '
        'cbElWHR
        '
        Me.cbElWHR.AutoSize = true
        Me.cbElWHR.Location = New System.Drawing.Point(17, 58)
        Me.cbElWHR.Name = "cbElWHR"
        Me.cbElWHR.Size = New System.Drawing.Size(136, 17)
        Me.cbElWHR.TabIndex = 2
        Me.cbElWHR.Text = "Electrical WHR System"
        Me.cbElWHR.UseVisualStyleBackColor = true
        '
        'EngineForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(1003, 600)
        Me.Controls.Add(Me.tbWHR)
        Me.Controls.Add(Me.cbDualFuel)
        Me.Controls.Add(Me.tbDualFuel)
        Me.Controls.Add(Me.lblEngineCharacteristics)
        Me.Controls.Add(Me.tbMaxTorque)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.tbRatedPower)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.tbRatedSpeed)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.PnInertia)
        Me.Controls.Add(Me.BtFLDopen)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.BtFLD)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.TbIdleSpeed)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.TbDispl)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.TbFLD)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label40)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TbName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "EngineForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F_ENG"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.PnInertia.ResumeLayout(false)
        Me.PnInertia.PerformLayout
        Me.GrWHTC.ResumeLayout(false)
        Me.PnWhtcDeclaration.ResumeLayout(false)
        Me.PnWhtcDeclaration.PerformLayout
        Me.PnWhtcEngineering.ResumeLayout(false)
        Me.PnWhtcEngineering.PerformLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.tbDualFuel.ResumeLayout(false)
        Me.tpPrimaryfuel.ResumeLayout(false)
        Me.tpPrimaryfuel.PerformLayout
        Me.tpSecondaryFuel.ResumeLayout(false)
        Me.tpSecondaryFuel.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.pnWhtcFuel2.ResumeLayout(false)
        Me.pnWhtcFuel2.PerformLayout
        Me.pnEngCFFuel2.ResumeLayout(false)
        Me.pnEngCFFuel2.PerformLayout
        Me.tbWHR.ResumeLayout(false)
        Me.tbElectricalWHR.ResumeLayout(false)
        Me.tbElectricalWHR.PerformLayout
        Me.tbMechanicalWHR.ResumeLayout(false)
        Me.tbMechanicalWHR.PerformLayout
        Me.pnElWHRDeclaration.ResumeLayout(false)
        Me.pnElWHRDeclaration.PerformLayout
        Me.pnElWhrEngineering.ResumeLayout(false)
        Me.pnElWhrEngineering.PerformLayout
        Me.pnMechWhrDeclaration.ResumeLayout(false)
        Me.pnMechWhrDeclaration.PerformLayout
        Me.pnMechWhrEngineering.ResumeLayout(false)
        Me.pnMechWhrEngineering.PerformLayout
        Me.TabPage1.ResumeLayout(false)
        Me.TabPage1.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents TbIdleSpeed As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents TbInertia As TextBox
	Friend WithEvents Label41 As Label
	Friend WithEvents Label40 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents ButCancel As Button
	Friend WithEvents ButOK As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents LbStatus As ToolStripStatusLabel
	Friend WithEvents Label1 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents TbDispl As TextBox
	Friend WithEvents TbName As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents TbMAP As TextBox
	Friend WithEvents Label6 As Label
	Friend WithEvents BtMAP As Button
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BtMAPopen As Button
	Friend WithEvents PnInertia As Panel
	Friend WithEvents GrWHTC As GroupBox
	Friend WithEvents TbWHTCmw As TextBox
	Friend WithEvents TbWHTCrural As TextBox
	Friend WithEvents TbWHTCurban As TextBox
	Friend WithEvents Label8 As Label
	Friend WithEvents Label7 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents Label13 As Label
	Friend WithEvents TbFLD As TextBox
	Friend WithEvents Label14 As Label
	Friend WithEvents BtFLD As Button
	Friend WithEvents BtFLDopen As Button
	Friend WithEvents TbWHTCEngineering As System.Windows.Forms.TextBox
	Friend WithEvents lblWhtcEngineering As System.Windows.Forms.Label
	Friend WithEvents PnWhtcDeclaration As System.Windows.Forms.Panel
	Friend WithEvents PnWhtcEngineering As System.Windows.Forms.Panel
	Friend WithEvents lblColdHotFactor As System.Windows.Forms.Label
	Friend WithEvents TbColdHotFactor As System.Windows.Forms.TextBox
	Friend WithEvents Label20 As System.Windows.Forms.Label
	Friend WithEvents tbRegPerCorrFactor As System.Windows.Forms.TextBox
	Friend WithEvents tbRatedSpeed As System.Windows.Forms.TextBox
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents tbRatedPower As System.Windows.Forms.TextBox
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents tbMaxTorque As System.Windows.Forms.TextBox
	Friend WithEvents Label16 As System.Windows.Forms.Label
	Friend WithEvents Label17 As System.Windows.Forms.Label
	Friend WithEvents Label18 As System.Windows.Forms.Label
	Friend WithEvents cbFuelType As System.Windows.Forms.ComboBox
	Friend WithEvents lblEngineCharacteristics As System.Windows.Forms.Label
    Friend WithEvents tbDualFuel As TabControl
    Friend WithEvents tpPrimaryfuel As TabPage
    Friend WithEvents tpSecondaryFuel As TabPage
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents pnWhtcFuel2 As Panel
    Friend WithEvents Label27 As Label
    Friend WithEvents tbRegPerFuel2 As TextBox
    Friend WithEvents Label28 As Label
    Friend WithEvents tbColdHotFuel2 As TextBox
    Friend WithEvents Label29 As Label
    Friend WithEvents Label30 As Label
    Friend WithEvents Label31 As Label
    Friend WithEvents Label32 As Label
    Friend WithEvents tbWhtcMotorwayFuel2 As TextBox
    Friend WithEvents tbWhtcUrbanFuel2 As TextBox
    Friend WithEvents tbWhtcRuralFuel2 As TextBox
    Friend WithEvents pnEngCFFuel2 As Panel
    Friend WithEvents tbEngineeringCFFuel2 As TextBox
    Friend WithEvents Label33 As Label
    Friend WithEvents tbMapFuel2 As TextBox
    Friend WithEvents Label34 As Label
    Friend WithEvents btMapFuel2 As Button
    Friend WithEvents btMapOpenFuel2 As Button
    Friend WithEvents cbFuelType2 As ComboBox
    Friend WithEvents Label35 As Label
    Friend WithEvents cbDualFuel As CheckBox
    Friend WithEvents tbWHR As TabControl
    Friend WithEvents tbElectricalWHR As TabPage
    Friend WithEvents tbMechanicalWHR As TabPage
    Friend WithEvents Label36 As Label
    Friend WithEvents pnElWHRDeclaration As Panel
    Friend WithEvents lblWHRRegPer As Label
    Friend WithEvents tbElWHRRegPer As TextBox
    Friend WithEvents Label21 As Label
    Friend WithEvents tbElWHRColdHot As TextBox
    Friend WithEvents Label22 As Label
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents Label25 As Label
    Friend WithEvents tbElWHRMotorway As TextBox
    Friend WithEvents tbElWHRUrban As TextBox
    Friend WithEvents tbElWHRRural As TextBox
    Friend WithEvents pnElWhrEngineering As Panel
    Friend WithEvents tbElWHREngineering As TextBox
    Friend WithEvents Label26 As Label
    Friend WithEvents Label37 As Label
    Friend WithEvents pnMechWhrDeclaration As Panel
    Friend WithEvents Label38 As Label
    Friend WithEvents tbMechWHRRegPerCF As TextBox
    Friend WithEvents Label39 As Label
    Friend WithEvents tbMechWHRBFColdHot As TextBox
    Friend WithEvents Label42 As Label
    Friend WithEvents Label43 As Label
    Friend WithEvents Label44 As Label
    Friend WithEvents Label45 As Label
    Friend WithEvents tbMechWHRMotorway As TextBox
    Friend WithEvents tbMechWHRUrban As TextBox
    Friend WithEvents tbMechWHRRural As TextBox
    Friend WithEvents pnMechWhrEngineering As Panel
    Friend WithEvents tbMechWHREngineering As TextBox
    Friend WithEvents Label46 As Label
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents cbElWHR As CheckBox
    Friend WithEvents cbMechWHRNotConnectedCrankshaft As CheckBox
    Friend WithEvents cbMechWHRInMap As CheckBox
End Class
