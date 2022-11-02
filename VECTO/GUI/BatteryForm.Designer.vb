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

<DesignerGenerated()>
Partial Class BatteryForm
    Inherits Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <DebuggerNonUserCode()>
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
    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BatteryForm))
        Me.tbCapacity = New System.Windows.Forms.TextBox()
        Me.lblCapacityUnit = New System.Windows.Forms.Label()
        Me.lblCapacity = New System.Windows.Forms.Label()
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
        Me.tbMakeModel = New System.Windows.Forms.TextBox()
        Me.lblMakeModel = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnInertia = New System.Windows.Forms.Panel()
        Me.tbSoCCurve = New System.Windows.Forms.TextBox()
        Me.lblSoCCurve = New System.Windows.Forms.Label()
        Me.btnBrowseSoCCurve = New System.Windows.Forms.Button()
        Me.btnSoCCurveOpen = New System.Windows.Forms.Button()
        Me.btnRiMapOpen = New System.Windows.Forms.Button()
        Me.btnBrowseRiMap = New System.Windows.Forms.Button()
        Me.lblRiMap = New System.Windows.Forms.Label()
        Me.tbRiCurve = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblMinSoc = New System.Windows.Forms.Label()
        Me.lblSoCMinUnit = New System.Windows.Forms.Label()
        Me.tbSoCMin = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblSoCMax = New System.Windows.Forms.Label()
        Me.lblSoCMaxUnit = New System.Windows.Forms.Label()
        Me.tbSoCMax = New System.Windows.Forms.TextBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.pnBattery = New System.Windows.Forms.Panel()
        Me.tbMaxCurrentMap = New System.Windows.Forms.TextBox()
        Me.lblMaxCurrentMap = New System.Windows.Forms.Label()
        Me.btnBrowseMaxCurrentMap = New System.Windows.Forms.Button()
        Me.btnMaxCurrentMapOpen = New System.Windows.Forms.Button()
        Me.pnSuperCap = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lblMaxCurrentDischarge = New System.Windows.Forms.Label()
        Me.lblMaxCurrentDischargeUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapMaxCurrentDischarge = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblMaxCurrentChg = New System.Windows.Forms.Label()
        Me.lblMaxCurrentChargeUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapMaxCurrentCharge = New System.Windows.Forms.TextBox()
        Me.pnSuperCapMaxV = New System.Windows.Forms.Panel()
        Me.lblSuperCapMaxV = New System.Windows.Forms.Label()
        Me.lblSuperCapMaxVUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapMaxV = New System.Windows.Forms.TextBox()
        Me.pnSuperCapMinV = New System.Windows.Forms.Panel()
        Me.lblSuperCapMinV = New System.Windows.Forms.Label()
        Me.lblSuperCapMinVUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapMinV = New System.Windows.Forms.TextBox()
        Me.pnSuperCapResistance = New System.Windows.Forms.Panel()
        Me.lblSuperCapRi = New System.Windows.Forms.Label()
        Me.lblSuperCapRiUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapRi = New System.Windows.Forms.TextBox()
        Me.pnSuperCapCapacity = New System.Windows.Forms.Panel()
        Me.lblSuperCapCapacity = New System.Windows.Forms.Label()
        Me.lblSuperCapCapacityUnit = New System.Windows.Forms.Label()
        Me.tbSuperCapCapacity = New System.Windows.Forms.TextBox()
        Me.cbRESSType = New System.Windows.Forms.ComboBox()
        Me.lblRessType = New System.Windows.Forms.Label()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.pnInertia.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
        Me.Panel2.SuspendLayout
        Me.FlowLayoutPanel1.SuspendLayout
        Me.pnBattery.SuspendLayout
        Me.pnSuperCap.SuspendLayout
        Me.Panel4.SuspendLayout
        Me.Panel3.SuspendLayout
        Me.pnSuperCapMaxV.SuspendLayout
        Me.pnSuperCapMinV.SuspendLayout
        Me.pnSuperCapResistance.SuspendLayout
        Me.pnSuperCapCapacity.SuspendLayout
        Me.SuspendLayout
        '
        'tbCapacity
        '
        Me.tbCapacity.Location = New System.Drawing.Point(90, 4)
        Me.tbCapacity.Name = "tbCapacity"
        Me.tbCapacity.Size = New System.Drawing.Size(57, 20)
        Me.tbCapacity.TabIndex = 0
        '
        'lblCapacityUnit
        '
        Me.lblCapacityUnit.AutoSize = true
        Me.lblCapacityUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblCapacityUnit.Name = "lblCapacityUnit"
        Me.lblCapacityUnit.Size = New System.Drawing.Size(26, 13)
        Me.lblCapacityUnit.TabIndex = 24
        Me.lblCapacityUnit.Text = "[Ah]"
        '
        'lblCapacity
        '
        Me.lblCapacity.AutoSize = true
        Me.lblCapacity.Location = New System.Drawing.Point(3, 7)
        Me.lblCapacity.Name = "lblCapacity"
        Me.lblCapacity.Size = New System.Drawing.Size(48, 13)
        Me.lblCapacity.TabIndex = 0
        Me.lblCapacity.Text = "Capacity"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(811, 321)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 4
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(730, 321)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 3
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(898, 31)
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
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 347)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(898, 22)
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
        'tbMakeModel
        '
        Me.tbMakeModel.Location = New System.Drawing.Point(109, 82)
        Me.tbMakeModel.Name = "tbMakeModel"
        Me.tbMakeModel.Size = New System.Drawing.Size(370, 20)
        Me.tbMakeModel.TabIndex = 0
        '
        'lblMakeModel
        '
        Me.lblMakeModel.AutoSize = true
        Me.lblMakeModel.Location = New System.Drawing.Point(16, 85)
        Me.lblMakeModel.Name = "lblMakeModel"
        Me.lblMakeModel.Size = New System.Drawing.Size(87, 13)
        Me.lblMakeModel.TabIndex = 11
        Me.lblMakeModel.Text = "Make and Model"
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
        'pnInertia
        '
        Me.pnInertia.Controls.Add(Me.lblCapacity)
        Me.pnInertia.Controls.Add(Me.lblCapacityUnit)
        Me.pnInertia.Controls.Add(Me.tbCapacity)
        Me.pnInertia.Location = New System.Drawing.Point(3, 3)
        Me.pnInertia.Name = "pnInertia"
        Me.pnInertia.Size = New System.Drawing.Size(212, 30)
        Me.pnInertia.TabIndex = 0
        '
        'tbSoCCurve
        '
        Me.tbSoCCurve.Location = New System.Drawing.Point(3, 123)
        Me.tbSoCCurve.Name = "tbSoCCurve"
        Me.tbSoCCurve.Size = New System.Drawing.Size(434, 20)
        Me.tbSoCCurve.TabIndex = 6
        '
        'lblSoCCurve
        '
        Me.lblSoCCurve.AutoSize = true
        Me.lblSoCCurve.Location = New System.Drawing.Point(3, 107)
        Me.lblSoCCurve.Name = "lblSoCCurve"
        Me.lblSoCCurve.Size = New System.Drawing.Size(60, 13)
        Me.lblSoCCurve.TabIndex = 38
        Me.lblSoCCurve.Text = "OCV Curve"
        '
        'btnBrowseSoCCurve
        '
        Me.btnBrowseSoCCurve.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseSoCCurve.Location = New System.Drawing.Point(437, 122)
        Me.btnBrowseSoCCurve.Name = "btnBrowseSoCCurve"
        Me.btnBrowseSoCCurve.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseSoCCurve.TabIndex = 7
        Me.btnBrowseSoCCurve.UseVisualStyleBackColor = true
        '
        'btnSoCCurveOpen
        '
        Me.btnSoCCurveOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnSoCCurveOpen.Location = New System.Drawing.Point(460, 122)
        Me.btnSoCCurveOpen.Name = "btnSoCCurveOpen"
        Me.btnSoCCurveOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnSoCCurveOpen.TabIndex = 8
        Me.btnSoCCurveOpen.UseVisualStyleBackColor = true
        '
        'btnRiMapOpen
        '
        Me.btnRiMapOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnRiMapOpen.Location = New System.Drawing.Point(460, 159)
        Me.btnRiMapOpen.Name = "btnRiMapOpen"
        Me.btnRiMapOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnRiMapOpen.TabIndex = 11
        Me.btnRiMapOpen.UseVisualStyleBackColor = true
        '
        'btnBrowseRiMap
        '
        Me.btnBrowseRiMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseRiMap.Location = New System.Drawing.Point(437, 159)
        Me.btnBrowseRiMap.Name = "btnBrowseRiMap"
        Me.btnBrowseRiMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseRiMap.TabIndex = 10
        Me.btnBrowseRiMap.UseVisualStyleBackColor = true
        '
        'lblRiMap
        '
        Me.lblRiMap.AutoSize = true
        Me.lblRiMap.Location = New System.Drawing.Point(3, 144)
        Me.lblRiMap.Name = "lblRiMap"
        Me.lblRiMap.Size = New System.Drawing.Size(129, 13)
        Me.lblRiMap.TabIndex = 43
        Me.lblRiMap.Text = "Internal Resistance Curve"
        '
        'tbRiCurve
        '
        Me.tbRiCurve.Location = New System.Drawing.Point(3, 161)
        Me.tbRiCurve.Name = "tbRiCurve"
        Me.tbRiCurve.Size = New System.Drawing.Size(434, 20)
        Me.tbRiCurve.TabIndex = 9
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(115, 34)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(266, 29)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Electric Energy Storage"
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(508, 28)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(382, 266)
        Me.PicBox.TabIndex = 49
        Me.PicBox.TabStop = false
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblMinSoc)
        Me.Panel1.Controls.Add(Me.lblSoCMinUnit)
        Me.Panel1.Controls.Add(Me.tbSoCMin)
        Me.Panel1.Location = New System.Drawing.Point(3, 36)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(212, 30)
        Me.Panel1.TabIndex = 1
        '
        'lblMinSoc
        '
        Me.lblMinSoc.AutoSize = true
        Me.lblMinSoc.Location = New System.Drawing.Point(3, 7)
        Me.lblMinSoc.Name = "lblMinSoc"
        Me.lblMinSoc.Size = New System.Drawing.Size(46, 13)
        Me.lblMinSoc.TabIndex = 0
        Me.lblMinSoc.Text = "SoC min"
        '
        'lblSoCMinUnit
        '
        Me.lblSoCMinUnit.AutoSize = true
        Me.lblSoCMinUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSoCMinUnit.Name = "lblSoCMinUnit"
        Me.lblSoCMinUnit.Size = New System.Drawing.Size(21, 13)
        Me.lblSoCMinUnit.TabIndex = 24
        Me.lblSoCMinUnit.Text = "[%]"
        '
        'tbSoCMin
        '
        Me.tbSoCMin.Location = New System.Drawing.Point(90, 4)
        Me.tbSoCMin.Name = "tbSoCMin"
        Me.tbSoCMin.Size = New System.Drawing.Size(57, 20)
        Me.tbSoCMin.TabIndex = 0
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblSoCMax)
        Me.Panel2.Controls.Add(Me.lblSoCMaxUnit)
        Me.Panel2.Controls.Add(Me.tbSoCMax)
        Me.Panel2.Location = New System.Drawing.Point(225, 36)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(212, 30)
        Me.Panel2.TabIndex = 2
        '
        'lblSoCMax
        '
        Me.lblSoCMax.AutoSize = true
        Me.lblSoCMax.Location = New System.Drawing.Point(3, 7)
        Me.lblSoCMax.Name = "lblSoCMax"
        Me.lblSoCMax.Size = New System.Drawing.Size(49, 13)
        Me.lblSoCMax.TabIndex = 0
        Me.lblSoCMax.Text = "SoC max"
        '
        'lblSoCMaxUnit
        '
        Me.lblSoCMaxUnit.AutoSize = true
        Me.lblSoCMaxUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSoCMaxUnit.Name = "lblSoCMaxUnit"
        Me.lblSoCMaxUnit.Size = New System.Drawing.Size(21, 13)
        Me.lblSoCMaxUnit.TabIndex = 24
        Me.lblSoCMaxUnit.Text = "[%]"
        '
        'tbSoCMax
        '
        Me.tbSoCMax.Location = New System.Drawing.Point(90, 4)
        Me.tbSoCMax.Name = "tbSoCMax"
        Me.tbSoCMax.Size = New System.Drawing.Size(57, 20)
        Me.tbSoCMax.TabIndex = 0
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.pnBattery)
        Me.FlowLayoutPanel1.Controls.Add(Me.pnSuperCap)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(8, 136)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(2)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(495, 200)
        Me.FlowLayoutPanel1.TabIndex = 2
        '
        'pnBattery
        '
        Me.pnBattery.Controls.Add(Me.tbMaxCurrentMap)
        Me.pnBattery.Controls.Add(Me.lblMaxCurrentMap)
        Me.pnBattery.Controls.Add(Me.btnBrowseMaxCurrentMap)
        Me.pnBattery.Controls.Add(Me.btnMaxCurrentMapOpen)
        Me.pnBattery.Controls.Add(Me.pnInertia)
        Me.pnBattery.Controls.Add(Me.tbSoCCurve)
        Me.pnBattery.Controls.Add(Me.lblSoCCurve)
        Me.pnBattery.Controls.Add(Me.Panel2)
        Me.pnBattery.Controls.Add(Me.btnBrowseSoCCurve)
        Me.pnBattery.Controls.Add(Me.Panel1)
        Me.pnBattery.Controls.Add(Me.btnSoCCurveOpen)
        Me.pnBattery.Controls.Add(Me.tbRiCurve)
        Me.pnBattery.Controls.Add(Me.lblRiMap)
        Me.pnBattery.Controls.Add(Me.btnRiMapOpen)
        Me.pnBattery.Controls.Add(Me.btnBrowseRiMap)
        Me.pnBattery.Location = New System.Drawing.Point(2, 2)
        Me.pnBattery.Margin = New System.Windows.Forms.Padding(2)
        Me.pnBattery.Name = "pnBattery"
        Me.pnBattery.Size = New System.Drawing.Size(489, 188)
        Me.pnBattery.TabIndex = 0
        '
        'tbMaxCurrentMap
        '
        Me.tbMaxCurrentMap.Location = New System.Drawing.Point(4, 87)
        Me.tbMaxCurrentMap.Name = "tbMaxCurrentMap"
        Me.tbMaxCurrentMap.Size = New System.Drawing.Size(434, 20)
        Me.tbMaxCurrentMap.TabIndex = 3
        '
        'lblMaxCurrentMap
        '
        Me.lblMaxCurrentMap.AutoSize = true
        Me.lblMaxCurrentMap.Location = New System.Drawing.Point(4, 71)
        Me.lblMaxCurrentMap.Name = "lblMaxCurrentMap"
        Me.lblMaxCurrentMap.Size = New System.Drawing.Size(88, 13)
        Me.lblMaxCurrentMap.TabIndex = 47
        Me.lblMaxCurrentMap.Text = "Max Current Map"
        '
        'btnBrowseMaxCurrentMap
        '
        Me.btnBrowseMaxCurrentMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseMaxCurrentMap.Location = New System.Drawing.Point(438, 85)
        Me.btnBrowseMaxCurrentMap.Name = "btnBrowseMaxCurrentMap"
        Me.btnBrowseMaxCurrentMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseMaxCurrentMap.TabIndex = 4
        Me.btnBrowseMaxCurrentMap.UseVisualStyleBackColor = true
        '
        'btnMaxCurrentMapOpen
        '
        Me.btnMaxCurrentMapOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnMaxCurrentMapOpen.Location = New System.Drawing.Point(461, 85)
        Me.btnMaxCurrentMapOpen.Name = "btnMaxCurrentMapOpen"
        Me.btnMaxCurrentMapOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnMaxCurrentMapOpen.TabIndex = 5
        Me.btnMaxCurrentMapOpen.UseVisualStyleBackColor = true
        '
        'pnSuperCap
        '
        Me.pnSuperCap.Controls.Add(Me.Panel4)
        Me.pnSuperCap.Controls.Add(Me.Panel3)
        Me.pnSuperCap.Controls.Add(Me.pnSuperCapMaxV)
        Me.pnSuperCap.Controls.Add(Me.pnSuperCapMinV)
        Me.pnSuperCap.Controls.Add(Me.pnSuperCapResistance)
        Me.pnSuperCap.Controls.Add(Me.pnSuperCapCapacity)
        Me.pnSuperCap.Location = New System.Drawing.Point(2, 194)
        Me.pnSuperCap.Margin = New System.Windows.Forms.Padding(2)
        Me.pnSuperCap.Name = "pnSuperCap"
        Me.pnSuperCap.Size = New System.Drawing.Size(489, 110)
        Me.pnSuperCap.TabIndex = 1
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lblMaxCurrentDischarge)
        Me.Panel4.Controls.Add(Me.lblMaxCurrentDischargeUnit)
        Me.Panel4.Controls.Add(Me.tbSuperCapMaxCurrentDischarge)
        Me.Panel4.Location = New System.Drawing.Point(225, 76)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(212, 30)
        Me.Panel4.TabIndex = 5
        '
        'lblMaxCurrentDischarge
        '
        Me.lblMaxCurrentDischarge.AutoSize = true
        Me.lblMaxCurrentDischarge.Location = New System.Drawing.Point(3, 7)
        Me.lblMaxCurrentDischarge.Name = "lblMaxCurrentDischarge"
        Me.lblMaxCurrentDischarge.Size = New System.Drawing.Size(100, 13)
        Me.lblMaxCurrentDischarge.TabIndex = 0
        Me.lblMaxCurrentDischarge.Text = "Max Current Dischg"
        '
        'lblMaxCurrentDischargeUnit
        '
        Me.lblMaxCurrentDischargeUnit.AutoSize = true
        Me.lblMaxCurrentDischargeUnit.Location = New System.Drawing.Point(171, 8)
        Me.lblMaxCurrentDischargeUnit.Name = "lblMaxCurrentDischargeUnit"
        Me.lblMaxCurrentDischargeUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblMaxCurrentDischargeUnit.TabIndex = 24
        Me.lblMaxCurrentDischargeUnit.Text = "[A]"
        '
        'tbSuperCapMaxCurrentDischarge
        '
        Me.tbSuperCapMaxCurrentDischarge.Location = New System.Drawing.Point(108, 5)
        Me.tbSuperCapMaxCurrentDischarge.Name = "tbSuperCapMaxCurrentDischarge"
        Me.tbSuperCapMaxCurrentDischarge.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapMaxCurrentDischarge.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.lblMaxCurrentChg)
        Me.Panel3.Controls.Add(Me.lblMaxCurrentChargeUnit)
        Me.Panel3.Controls.Add(Me.tbSuperCapMaxCurrentCharge)
        Me.Panel3.Location = New System.Drawing.Point(3, 76)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(212, 30)
        Me.Panel3.TabIndex = 2
        '
        'lblMaxCurrentChg
        '
        Me.lblMaxCurrentChg.AutoSize = true
        Me.lblMaxCurrentChg.Location = New System.Drawing.Point(3, 7)
        Me.lblMaxCurrentChg.Name = "lblMaxCurrentChg"
        Me.lblMaxCurrentChg.Size = New System.Drawing.Size(86, 13)
        Me.lblMaxCurrentChg.TabIndex = 0
        Me.lblMaxCurrentChg.Text = "Max Current Chg"
        '
        'lblMaxCurrentChargeUnit
        '
        Me.lblMaxCurrentChargeUnit.AutoSize = true
        Me.lblMaxCurrentChargeUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblMaxCurrentChargeUnit.Name = "lblMaxCurrentChargeUnit"
        Me.lblMaxCurrentChargeUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblMaxCurrentChargeUnit.TabIndex = 24
        Me.lblMaxCurrentChargeUnit.Text = "[A]"
        '
        'tbSuperCapMaxCurrentCharge
        '
        Me.tbSuperCapMaxCurrentCharge.Location = New System.Drawing.Point(90, 4)
        Me.tbSuperCapMaxCurrentCharge.Name = "tbSuperCapMaxCurrentCharge"
        Me.tbSuperCapMaxCurrentCharge.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapMaxCurrentCharge.TabIndex = 0
        '
        'pnSuperCapMaxV
        '
        Me.pnSuperCapMaxV.Controls.Add(Me.lblSuperCapMaxV)
        Me.pnSuperCapMaxV.Controls.Add(Me.lblSuperCapMaxVUnit)
        Me.pnSuperCapMaxV.Controls.Add(Me.tbSuperCapMaxV)
        Me.pnSuperCapMaxV.Location = New System.Drawing.Point(225, 40)
        Me.pnSuperCapMaxV.Name = "pnSuperCapMaxV"
        Me.pnSuperCapMaxV.Size = New System.Drawing.Size(212, 30)
        Me.pnSuperCapMaxV.TabIndex = 4
        '
        'lblSuperCapMaxV
        '
        Me.lblSuperCapMaxV.AutoSize = true
        Me.lblSuperCapMaxV.Location = New System.Drawing.Point(3, 7)
        Me.lblSuperCapMaxV.Name = "lblSuperCapMaxV"
        Me.lblSuperCapMaxV.Size = New System.Drawing.Size(66, 13)
        Me.lblSuperCapMaxV.TabIndex = 0
        Me.lblSuperCapMaxV.Text = "Max Voltage"
        '
        'lblSuperCapMaxVUnit
        '
        Me.lblSuperCapMaxVUnit.AutoSize = true
        Me.lblSuperCapMaxVUnit.Location = New System.Drawing.Point(171, 8)
        Me.lblSuperCapMaxVUnit.Name = "lblSuperCapMaxVUnit"
        Me.lblSuperCapMaxVUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblSuperCapMaxVUnit.TabIndex = 24
        Me.lblSuperCapMaxVUnit.Text = "[V]"
        '
        'tbSuperCapMaxV
        '
        Me.tbSuperCapMaxV.Location = New System.Drawing.Point(108, 5)
        Me.tbSuperCapMaxV.Name = "tbSuperCapMaxV"
        Me.tbSuperCapMaxV.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapMaxV.TabIndex = 0
        '
        'pnSuperCapMinV
        '
        Me.pnSuperCapMinV.Controls.Add(Me.lblSuperCapMinV)
        Me.pnSuperCapMinV.Controls.Add(Me.lblSuperCapMinVUnit)
        Me.pnSuperCapMinV.Controls.Add(Me.tbSuperCapMinV)
        Me.pnSuperCapMinV.Location = New System.Drawing.Point(3, 40)
        Me.pnSuperCapMinV.Name = "pnSuperCapMinV"
        Me.pnSuperCapMinV.Size = New System.Drawing.Size(212, 30)
        Me.pnSuperCapMinV.TabIndex = 1
        '
        'lblSuperCapMinV
        '
        Me.lblSuperCapMinV.AutoSize = true
        Me.lblSuperCapMinV.Location = New System.Drawing.Point(3, 7)
        Me.lblSuperCapMinV.Name = "lblSuperCapMinV"
        Me.lblSuperCapMinV.Size = New System.Drawing.Size(63, 13)
        Me.lblSuperCapMinV.TabIndex = 0
        Me.lblSuperCapMinV.Text = "Min Voltage"
        '
        'lblSuperCapMinVUnit
        '
        Me.lblSuperCapMinVUnit.AutoSize = true
        Me.lblSuperCapMinVUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSuperCapMinVUnit.Name = "lblSuperCapMinVUnit"
        Me.lblSuperCapMinVUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblSuperCapMinVUnit.TabIndex = 24
        Me.lblSuperCapMinVUnit.Text = "[V]"
        '
        'tbSuperCapMinV
        '
        Me.tbSuperCapMinV.Location = New System.Drawing.Point(90, 4)
        Me.tbSuperCapMinV.Name = "tbSuperCapMinV"
        Me.tbSuperCapMinV.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapMinV.TabIndex = 0
        '
        'pnSuperCapResistance
        '
        Me.pnSuperCapResistance.Controls.Add(Me.lblSuperCapRi)
        Me.pnSuperCapResistance.Controls.Add(Me.lblSuperCapRiUnit)
        Me.pnSuperCapResistance.Controls.Add(Me.tbSuperCapRi)
        Me.pnSuperCapResistance.Location = New System.Drawing.Point(225, 3)
        Me.pnSuperCapResistance.Name = "pnSuperCapResistance"
        Me.pnSuperCapResistance.Size = New System.Drawing.Size(212, 30)
        Me.pnSuperCapResistance.TabIndex = 3
        '
        'lblSuperCapRi
        '
        Me.lblSuperCapRi.AutoSize = true
        Me.lblSuperCapRi.Location = New System.Drawing.Point(3, 7)
        Me.lblSuperCapRi.Name = "lblSuperCapRi"
        Me.lblSuperCapRi.Size = New System.Drawing.Size(98, 13)
        Me.lblSuperCapRi.TabIndex = 0
        Me.lblSuperCapRi.Text = "Internal Resistance"
        '
        'lblSuperCapRiUnit
        '
        Me.lblSuperCapRiUnit.AutoSize = true
        Me.lblSuperCapRiUnit.Location = New System.Drawing.Point(171, 8)
        Me.lblSuperCapRiUnit.Name = "lblSuperCapRiUnit"
        Me.lblSuperCapRiUnit.Size = New System.Drawing.Size(22, 13)
        Me.lblSuperCapRiUnit.TabIndex = 24
        Me.lblSuperCapRiUnit.Text = "[Ω]"
        '
        'tbSuperCapRi
        '
        Me.tbSuperCapRi.Location = New System.Drawing.Point(108, 5)
        Me.tbSuperCapRi.Name = "tbSuperCapRi"
        Me.tbSuperCapRi.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapRi.TabIndex = 0
        '
        'pnSuperCapCapacity
        '
        Me.pnSuperCapCapacity.Controls.Add(Me.lblSuperCapCapacity)
        Me.pnSuperCapCapacity.Controls.Add(Me.lblSuperCapCapacityUnit)
        Me.pnSuperCapCapacity.Controls.Add(Me.tbSuperCapCapacity)
        Me.pnSuperCapCapacity.Location = New System.Drawing.Point(3, 3)
        Me.pnSuperCapCapacity.Name = "pnSuperCapCapacity"
        Me.pnSuperCapCapacity.Size = New System.Drawing.Size(212, 30)
        Me.pnSuperCapCapacity.TabIndex = 0
        '
        'lblSuperCapCapacity
        '
        Me.lblSuperCapCapacity.AutoSize = true
        Me.lblSuperCapCapacity.Location = New System.Drawing.Point(3, 7)
        Me.lblSuperCapCapacity.Name = "lblSuperCapCapacity"
        Me.lblSuperCapCapacity.Size = New System.Drawing.Size(67, 13)
        Me.lblSuperCapCapacity.TabIndex = 0
        Me.lblSuperCapCapacity.Text = "Capacitance"
        '
        'lblSuperCapCapacityUnit
        '
        Me.lblSuperCapCapacityUnit.AutoSize = true
        Me.lblSuperCapCapacityUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSuperCapCapacityUnit.Name = "lblSuperCapCapacityUnit"
        Me.lblSuperCapCapacityUnit.Size = New System.Drawing.Size(19, 13)
        Me.lblSuperCapCapacityUnit.TabIndex = 24
        Me.lblSuperCapCapacityUnit.Text = "[F]"
        '
        'tbSuperCapCapacity
        '
        Me.tbSuperCapCapacity.Location = New System.Drawing.Point(90, 4)
        Me.tbSuperCapCapacity.Name = "tbSuperCapCapacity"
        Me.tbSuperCapCapacity.Size = New System.Drawing.Size(57, 20)
        Me.tbSuperCapCapacity.TabIndex = 0
        '
        'cbRESSType
        '
        Me.cbRESSType.FormattingEnabled = true
        Me.cbRESSType.Location = New System.Drawing.Point(111, 107)
        Me.cbRESSType.Margin = New System.Windows.Forms.Padding(2)
        Me.cbRESSType.Name = "cbRESSType"
        Me.cbRESSType.Size = New System.Drawing.Size(142, 21)
        Me.cbRESSType.TabIndex = 1
        '
        'lblRessType
        '
        Me.lblRessType.AutoSize = true
        Me.lblRessType.Location = New System.Drawing.Point(16, 109)
        Me.lblRessType.Name = "lblRessType"
        Me.lblRessType.Size = New System.Drawing.Size(70, 13)
        Me.lblRessType.TabIndex = 52
        Me.lblRessType.Text = "REESS Type"
        '
        'BatteryForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(898, 369)
        Me.Controls.Add(Me.lblRessType)
        Me.Controls.Add(Me.cbRESSType)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.lblMakeModel)
        Me.Controls.Add(Me.tbMakeModel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "BatteryForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Electric Energy Storage"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.pnInertia.ResumeLayout(false)
        Me.pnInertia.PerformLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.Panel2.ResumeLayout(false)
        Me.Panel2.PerformLayout
        Me.FlowLayoutPanel1.ResumeLayout(false)
        Me.pnBattery.ResumeLayout(false)
        Me.pnBattery.PerformLayout
        Me.pnSuperCap.ResumeLayout(false)
        Me.Panel4.ResumeLayout(false)
        Me.Panel4.PerformLayout
        Me.Panel3.ResumeLayout(false)
        Me.Panel3.PerformLayout
        Me.pnSuperCapMaxV.ResumeLayout(false)
        Me.pnSuperCapMaxV.PerformLayout
        Me.pnSuperCapMinV.ResumeLayout(false)
        Me.pnSuperCapMinV.PerformLayout
        Me.pnSuperCapResistance.ResumeLayout(false)
        Me.pnSuperCapResistance.PerformLayout
        Me.pnSuperCapCapacity.ResumeLayout(false)
        Me.pnSuperCapCapacity.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents tbCapacity As TextBox
    Friend WithEvents lblCapacityUnit As Label
    Friend WithEvents lblCapacity As Label
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
    Friend WithEvents tbMakeModel As TextBox
    Friend WithEvents lblMakeModel As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents pnInertia As Panel
    Friend WithEvents tbSoCCurve As TextBox
    Friend WithEvents lblSoCCurve As Label
    Friend WithEvents btnBrowseSoCCurve As Button
    Friend WithEvents btnSoCCurveOpen As Button
    Friend WithEvents btnRiMapOpen As Button
    Friend WithEvents btnBrowseRiMap As Button
    Friend WithEvents lblRiMap As Label
    Friend WithEvents tbRiCurve As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblMinSoc As Label
    Friend WithEvents lblSoCMinUnit As Label
    Friend WithEvents tbSoCMin As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblSoCMax As Label
    Friend WithEvents lblSoCMaxUnit As Label
    Friend WithEvents tbSoCMax As TextBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents pnBattery As Panel
    Friend WithEvents pnSuperCap As Panel
    Friend WithEvents cbRESSType As ComboBox
    Friend WithEvents lblRessType As Label
    Friend WithEvents pnSuperCapMaxV As Panel
    Friend WithEvents lblSuperCapMaxV As Label
    Friend WithEvents lblSuperCapMaxVUnit As Label
    Friend WithEvents tbSuperCapMaxV As TextBox
    Friend WithEvents pnSuperCapMinV As Panel
    Friend WithEvents lblSuperCapMinV As Label
    Friend WithEvents lblSuperCapMinVUnit As Label
    Friend WithEvents tbSuperCapMinV As TextBox
    Friend WithEvents pnSuperCapResistance As Panel
    Friend WithEvents lblSuperCapRi As Label
    Friend WithEvents lblSuperCapRiUnit As Label
    Friend WithEvents tbSuperCapRi As TextBox
    Friend WithEvents pnSuperCapCapacity As Panel
    Friend WithEvents lblSuperCapCapacity As Label
    Friend WithEvents lblSuperCapCapacityUnit As Label
    Friend WithEvents tbSuperCapCapacity As TextBox
    Friend WithEvents tbMaxCurrentMap As TextBox
    Friend WithEvents lblMaxCurrentMap As Label
    Friend WithEvents btnBrowseMaxCurrentMap As Button
    Friend WithEvents btnMaxCurrentMapOpen As Button
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lblMaxCurrentDischarge As Label
    Friend WithEvents lblMaxCurrentDischargeUnit As Label
    Friend WithEvents tbSuperCapMaxCurrentDischarge As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblMaxCurrentChg As Label
    Friend WithEvents lblMaxCurrentChargeUnit As Label
    Friend WithEvents tbSuperCapMaxCurrentCharge As TextBox
End Class
