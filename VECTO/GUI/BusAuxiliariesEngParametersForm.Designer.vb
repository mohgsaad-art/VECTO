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
Partial Class BusAuxiliariesEngParametersForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BusAuxiliariesEngParametersForm))
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.gbElectricSystem = New System.Windows.Forms.GroupBox()
        Me.pnDCDCEff = New System.Windows.Forms.Panel()
        Me.lblDCDCEff = New System.Windows.Forms.Label()
        Me.lblDCDCEffUnit = New System.Windows.Forms.Label()
        Me.tbDCDCEff = New System.Windows.Forms.TextBox()
        Me.pnES_HEVREESS = New System.Windows.Forms.Panel()
        Me.cbES_HEVREESS = New System.Windows.Forms.CheckBox()
        Me.pnAlternatorTechnology = New System.Windows.Forms.Panel()
        Me.cbAlternatorTechnology = New System.Windows.Forms.ComboBox()
        Me.lbAlternatorTechnology = New System.Windows.Forms.Label()
        Me.pnSmartElectricParams = New System.Windows.Forms.Panel()
        Me.pnBattEfficiency = New System.Windows.Forms.Panel()
        Me.lblBatEfficiency = New System.Windows.Forms.Label()
        Me.lblBatEfficiencyUnit = New System.Windows.Forms.Label()
        Me.tbBatEfficiency = New System.Windows.Forms.TextBox()
        Me.pnElectricStorageCapacity = New System.Windows.Forms.Panel()
        Me.lblElectricStorageCapacity = New System.Windows.Forms.Label()
        Me.lblElectricStorageCapacityUnit = New System.Windows.Forms.Label()
        Me.tbElectricStorageCapacity = New System.Windows.Forms.TextBox()
        Me.pnMaxAlternatorPower = New System.Windows.Forms.Panel()
        Me.lblMaxAlternatorPower = New System.Windows.Forms.Label()
        Me.lblMaxAlternatorPowerUnit = New System.Windows.Forms.Label()
        Me.tbMaxAlternatorPower = New System.Windows.Forms.TextBox()
        Me.pnAlternatorEfficiency = New System.Windows.Forms.Panel()
        Me.lblAlternatorEfficiency = New System.Windows.Forms.Label()
        Me.lblAlternatorEfficiencyUnit = New System.Windows.Forms.Label()
        Me.tbAlternatorEfficiency = New System.Windows.Forms.TextBox()
        Me.pnCurrentDemandEngineOffStandstill = New System.Windows.Forms.Panel()
        Me.lblCurrentDemandEngineIffStandstill = New System.Windows.Forms.Label()
        Me.lblCurrentDemandEngienOffStandstillUnit = New System.Windows.Forms.Label()
        Me.tbCurrentDemandEngineOffStandstill = New System.Windows.Forms.TextBox()
        Me.pnCurrentDemandEngineOffDriving = New System.Windows.Forms.Panel()
        Me.lblCurrentDemandEngineOffDriving = New System.Windows.Forms.Label()
        Me.lblCurrentDemandEngineOffDrivingUnit = New System.Windows.Forms.Label()
        Me.tbCurrentDemandEngineOffDriving = New System.Windows.Forms.TextBox()
        Me.pnCurrentDemand = New System.Windows.Forms.Panel()
        Me.lblCurrentDemand = New System.Windows.Forms.Label()
        Me.lblCurrentDemandUnit = New System.Windows.Forms.Label()
        Me.tbCurrentDemand = New System.Windows.Forms.TextBox()
        Me.bgPneumaticSystem = New System.Windows.Forms.GroupBox()
        Me.pnSmartCompressor = New System.Windows.Forms.Panel()
        Me.cbSmartCompressor = New System.Windows.Forms.CheckBox()
        Me.pnCompressorRatio = New System.Windows.Forms.Panel()
        Me.lblCompressorRatio = New System.Windows.Forms.Label()
        Me.lblCompressorRatioUnit = New System.Windows.Forms.Label()
        Me.tbCompressorRatio = New System.Windows.Forms.TextBox()
        Me.pnCompressorMap = New System.Windows.Forms.Panel()
        Me.lblCompressorMap = New System.Windows.Forms.Label()
        Me.btnBrowseCompressorMap = New System.Windows.Forms.Button()
        Me.tbCompressorMap = New System.Windows.Forms.TextBox()
        Me.pnAverageAirDemand = New System.Windows.Forms.Panel()
        Me.lblAverageAirDemand = New System.Windows.Forms.Label()
        Me.lblAverageAirDemandUnit = New System.Windows.Forms.Label()
        Me.tbAverageAirDemand = New System.Windows.Forms.TextBox()
        Me.gbHVAC = New System.Windows.Forms.GroupBox()
        Me.pnHvacHeatingDemand = New System.Windows.Forms.Panel()
        Me.lblHvacHeatingDemand = New System.Windows.Forms.Label()
        Me.lblHvacHeatingDemandUnit = New System.Windows.Forms.Label()
        Me.tbHvacHeatingDemand = New System.Windows.Forms.TextBox()
        Me.pnHvacAuxHeaterPwr = New System.Windows.Forms.Panel()
        Me.lblHvacAuxHEaterPwr = New System.Windows.Forms.Label()
        Me.lblHvacAuxHeaterPwrUnit = New System.Windows.Forms.Label()
        Me.tbHvacAuxHeaterPwr = New System.Windows.Forms.TextBox()
        Me.pnHvacElecPowerDemand = New System.Windows.Forms.Panel()
        Me.lblHvacElectricPowerDemand = New System.Windows.Forms.Label()
        Me.lblHvacElectricPowerDemandUnit = New System.Windows.Forms.Label()
        Me.tbHvacElectricPowerDemand = New System.Windows.Forms.TextBox()
        Me.pnHvacMechPowerDemand = New System.Windows.Forms.Panel()
        Me.lblHvacMechPowerDemand = New System.Windows.Forms.Label()
        Me.lblHvacMechPowerDemandUnit = New System.Windows.Forms.Label()
        Me.tbHvacMechPowerDemand = New System.Windows.Forms.TextBox()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.gbElectricSystem.SuspendLayout
        Me.pnDCDCEff.SuspendLayout
        Me.pnES_HEVREESS.SuspendLayout
        Me.pnAlternatorTechnology.SuspendLayout
        Me.pnSmartElectricParams.SuspendLayout
        Me.pnBattEfficiency.SuspendLayout
        Me.pnElectricStorageCapacity.SuspendLayout
        Me.pnMaxAlternatorPower.SuspendLayout
        Me.pnAlternatorEfficiency.SuspendLayout
        Me.pnCurrentDemandEngineOffStandstill.SuspendLayout
        Me.pnCurrentDemandEngineOffDriving.SuspendLayout
        Me.pnCurrentDemand.SuspendLayout
        Me.bgPneumaticSystem.SuspendLayout
        Me.pnSmartCompressor.SuspendLayout
        Me.pnCompressorRatio.SuspendLayout
        Me.pnCompressorMap.SuspendLayout
        Me.pnAverageAirDemand.SuspendLayout
        Me.gbHVAC.SuspendLayout
        Me.pnHvacHeatingDemand.SuspendLayout
        Me.pnHvacAuxHeaterPwr.SuspendLayout
        Me.pnHvacElecPowerDemand.SuspendLayout
        Me.pnHvacMechPowerDemand.SuspendLayout
        Me.SuspendLayout
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(936, 398)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 4
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(855, 398)
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
        Me.ToolStrip1.Size = New System.Drawing.Size(1023, 31)
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
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 424)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1023, 22)
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
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_ENG
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(1011, 40)
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
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(119, 35)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(307, 29)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Bus Auxiliaries Engineering"
        '
        'gbElectricSystem
        '
        Me.gbElectricSystem.Controls.Add(Me.pnDCDCEff)
        Me.gbElectricSystem.Controls.Add(Me.pnES_HEVREESS)
        Me.gbElectricSystem.Controls.Add(Me.pnAlternatorTechnology)
        Me.gbElectricSystem.Controls.Add(Me.pnSmartElectricParams)
        Me.gbElectricSystem.Controls.Add(Me.pnAlternatorEfficiency)
        Me.gbElectricSystem.Controls.Add(Me.pnCurrentDemandEngineOffStandstill)
        Me.gbElectricSystem.Controls.Add(Me.pnCurrentDemandEngineOffDriving)
        Me.gbElectricSystem.Controls.Add(Me.pnCurrentDemand)
        Me.gbElectricSystem.Location = New System.Drawing.Point(12, 74)
        Me.gbElectricSystem.Name = "gbElectricSystem"
        Me.gbElectricSystem.Size = New System.Drawing.Size(317, 339)
        Me.gbElectricSystem.TabIndex = 0
        Me.gbElectricSystem.TabStop = false
        Me.gbElectricSystem.Text = "Electric System"
        '
        'pnDCDCEff
        '
        Me.pnDCDCEff.Controls.Add(Me.lblDCDCEff)
        Me.pnDCDCEff.Controls.Add(Me.lblDCDCEffUnit)
        Me.pnDCDCEff.Controls.Add(Me.tbDCDCEff)
        Me.pnDCDCEff.Location = New System.Drawing.Point(10, 294)
        Me.pnDCDCEff.Name = "pnDCDCEff"
        Me.pnDCDCEff.Size = New System.Drawing.Size(288, 28)
        Me.pnDCDCEff.TabIndex = 9
        '
        'lblDCDCEff
        '
        Me.lblDCDCEff.AutoSize = true
        Me.lblDCDCEff.Location = New System.Drawing.Point(3, 7)
        Me.lblDCDCEff.Name = "lblDCDCEff"
        Me.lblDCDCEff.Size = New System.Drawing.Size(140, 13)
        Me.lblDCDCEff.TabIndex = 0
        Me.lblDCDCEff.Text = "DC/DC Converter Efficiency"
        '
        'lblDCDCEffUnit
        '
        Me.lblDCDCEffUnit.AutoSize = true
        Me.lblDCDCEffUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblDCDCEffUnit.Name = "lblDCDCEffUnit"
        Me.lblDCDCEffUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblDCDCEffUnit.TabIndex = 24
        Me.lblDCDCEffUnit.Text = "[-]"
        '
        'tbDCDCEff
        '
        Me.tbDCDCEff.Location = New System.Drawing.Point(197, 4)
        Me.tbDCDCEff.Name = "tbDCDCEff"
        Me.tbDCDCEff.Size = New System.Drawing.Size(57, 20)
        Me.tbDCDCEff.TabIndex = 3
        '
        'pnES_HEVREESS
        '
        Me.pnES_HEVREESS.Controls.Add(Me.cbES_HEVREESS)
        Me.pnES_HEVREESS.Location = New System.Drawing.Point(10, 265)
        Me.pnES_HEVREESS.Name = "pnES_HEVREESS"
        Me.pnES_HEVREESS.Size = New System.Drawing.Size(288, 28)
        Me.pnES_HEVREESS.TabIndex = 8
        '
        'cbES_HEVREESS
        '
        Me.cbES_HEVREESS.AutoSize = true
        Me.cbES_HEVREESS.Location = New System.Drawing.Point(6, 6)
        Me.cbES_HEVREESS.Name = "cbES_HEVREESS"
        Me.cbES_HEVREESS.Size = New System.Drawing.Size(160, 17)
        Me.cbES_HEVREESS.TabIndex = 25
        Me.cbES_HEVREESS.Text = "ES supply from HEV REESS"
        Me.cbES_HEVREESS.UseVisualStyleBackColor = true
        '
        'pnAlternatorTechnology
        '
        Me.pnAlternatorTechnology.Controls.Add(Me.cbAlternatorTechnology)
        Me.pnAlternatorTechnology.Controls.Add(Me.lbAlternatorTechnology)
        Me.pnAlternatorTechnology.Location = New System.Drawing.Point(10, 139)
        Me.pnAlternatorTechnology.Name = "pnAlternatorTechnology"
        Me.pnAlternatorTechnology.Size = New System.Drawing.Size(288, 28)
        Me.pnAlternatorTechnology.TabIndex = 4
        '
        'cbAlternatorTechnology
        '
        Me.cbAlternatorTechnology.FormattingEnabled = true
        Me.cbAlternatorTechnology.Location = New System.Drawing.Point(134, 4)
        Me.cbAlternatorTechnology.Name = "cbAlternatorTechnology"
        Me.cbAlternatorTechnology.Size = New System.Drawing.Size(121, 21)
        Me.cbAlternatorTechnology.TabIndex = 1
        '
        'lbAlternatorTechnology
        '
        Me.lbAlternatorTechnology.AutoSize = true
        Me.lbAlternatorTechnology.Location = New System.Drawing.Point(3, 7)
        Me.lbAlternatorTechnology.Name = "lbAlternatorTechnology"
        Me.lbAlternatorTechnology.Size = New System.Drawing.Size(111, 13)
        Me.lbAlternatorTechnology.TabIndex = 0
        Me.lbAlternatorTechnology.Text = "Alternator Technology"
        '
        'pnSmartElectricParams
        '
        Me.pnSmartElectricParams.Controls.Add(Me.pnBattEfficiency)
        Me.pnSmartElectricParams.Controls.Add(Me.pnElectricStorageCapacity)
        Me.pnSmartElectricParams.Controls.Add(Me.pnMaxAlternatorPower)
        Me.pnSmartElectricParams.Location = New System.Drawing.Point(10, 168)
        Me.pnSmartElectricParams.Name = "pnSmartElectricParams"
        Me.pnSmartElectricParams.Size = New System.Drawing.Size(288, 96)
        Me.pnSmartElectricParams.TabIndex = 5
        '
        'pnBattEfficiency
        '
        Me.pnBattEfficiency.Controls.Add(Me.lblBatEfficiency)
        Me.pnBattEfficiency.Controls.Add(Me.lblBatEfficiencyUnit)
        Me.pnBattEfficiency.Controls.Add(Me.tbBatEfficiency)
        Me.pnBattEfficiency.Location = New System.Drawing.Point(0, 63)
        Me.pnBattEfficiency.Name = "pnBattEfficiency"
        Me.pnBattEfficiency.Size = New System.Drawing.Size(300, 28)
        Me.pnBattEfficiency.TabIndex = 2
        '
        'lblBatEfficiency
        '
        Me.lblBatEfficiency.AutoSize = true
        Me.lblBatEfficiency.Location = New System.Drawing.Point(3, 7)
        Me.lblBatEfficiency.Name = "lblBatEfficiency"
        Me.lblBatEfficiency.Size = New System.Drawing.Size(131, 13)
        Me.lblBatEfficiency.TabIndex = 0
        Me.lblBatEfficiency.Text = "Electric Storage Efficiency"
        '
        'lblBatEfficiencyUnit
        '
        Me.lblBatEfficiencyUnit.AutoSize = true
        Me.lblBatEfficiencyUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblBatEfficiencyUnit.Name = "lblBatEfficiencyUnit"
        Me.lblBatEfficiencyUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblBatEfficiencyUnit.TabIndex = 24
        Me.lblBatEfficiencyUnit.Text = "[-]"
        '
        'tbBatEfficiency
        '
        Me.tbBatEfficiency.Location = New System.Drawing.Point(197, 4)
        Me.tbBatEfficiency.Name = "tbBatEfficiency"
        Me.tbBatEfficiency.Size = New System.Drawing.Size(57, 20)
        Me.tbBatEfficiency.TabIndex = 3
        '
        'pnElectricStorageCapacity
        '
        Me.pnElectricStorageCapacity.Controls.Add(Me.lblElectricStorageCapacity)
        Me.pnElectricStorageCapacity.Controls.Add(Me.lblElectricStorageCapacityUnit)
        Me.pnElectricStorageCapacity.Controls.Add(Me.tbElectricStorageCapacity)
        Me.pnElectricStorageCapacity.Location = New System.Drawing.Point(0, 33)
        Me.pnElectricStorageCapacity.Name = "pnElectricStorageCapacity"
        Me.pnElectricStorageCapacity.Size = New System.Drawing.Size(300, 28)
        Me.pnElectricStorageCapacity.TabIndex = 1
        '
        'lblElectricStorageCapacity
        '
        Me.lblElectricStorageCapacity.AutoSize = true
        Me.lblElectricStorageCapacity.Location = New System.Drawing.Point(3, 7)
        Me.lblElectricStorageCapacity.Name = "lblElectricStorageCapacity"
        Me.lblElectricStorageCapacity.Size = New System.Drawing.Size(168, 13)
        Me.lblElectricStorageCapacity.TabIndex = 0
        Me.lblElectricStorageCapacity.Text = "Useable Electric Storage Capacity"
        '
        'lblElectricStorageCapacityUnit
        '
        Me.lblElectricStorageCapacityUnit.AutoSize = true
        Me.lblElectricStorageCapacityUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblElectricStorageCapacityUnit.Name = "lblElectricStorageCapacityUnit"
        Me.lblElectricStorageCapacityUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblElectricStorageCapacityUnit.TabIndex = 24
        Me.lblElectricStorageCapacityUnit.Text = "[Wh]"
        '
        'tbElectricStorageCapacity
        '
        Me.tbElectricStorageCapacity.Location = New System.Drawing.Point(197, 4)
        Me.tbElectricStorageCapacity.Name = "tbElectricStorageCapacity"
        Me.tbElectricStorageCapacity.Size = New System.Drawing.Size(57, 20)
        Me.tbElectricStorageCapacity.TabIndex = 3
        '
        'pnMaxAlternatorPower
        '
        Me.pnMaxAlternatorPower.Controls.Add(Me.lblMaxAlternatorPower)
        Me.pnMaxAlternatorPower.Controls.Add(Me.lblMaxAlternatorPowerUnit)
        Me.pnMaxAlternatorPower.Controls.Add(Me.tbMaxAlternatorPower)
        Me.pnMaxAlternatorPower.Location = New System.Drawing.Point(0, 3)
        Me.pnMaxAlternatorPower.Name = "pnMaxAlternatorPower"
        Me.pnMaxAlternatorPower.Size = New System.Drawing.Size(288, 28)
        Me.pnMaxAlternatorPower.TabIndex = 0
        '
        'lblMaxAlternatorPower
        '
        Me.lblMaxAlternatorPower.AutoSize = true
        Me.lblMaxAlternatorPower.Location = New System.Drawing.Point(3, 7)
        Me.lblMaxAlternatorPower.Name = "lblMaxAlternatorPower"
        Me.lblMaxAlternatorPower.Size = New System.Drawing.Size(127, 13)
        Me.lblMaxAlternatorPower.TabIndex = 0
        Me.lblMaxAlternatorPower.Text = "Max Recuperation Power"
        '
        'lblMaxAlternatorPowerUnit
        '
        Me.lblMaxAlternatorPowerUnit.AutoSize = true
        Me.lblMaxAlternatorPowerUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblMaxAlternatorPowerUnit.Name = "lblMaxAlternatorPowerUnit"
        Me.lblMaxAlternatorPowerUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblMaxAlternatorPowerUnit.TabIndex = 24
        Me.lblMaxAlternatorPowerUnit.Text = "[W]"
        '
        'tbMaxAlternatorPower
        '
        Me.tbMaxAlternatorPower.Location = New System.Drawing.Point(197, 4)
        Me.tbMaxAlternatorPower.Name = "tbMaxAlternatorPower"
        Me.tbMaxAlternatorPower.Size = New System.Drawing.Size(57, 20)
        Me.tbMaxAlternatorPower.TabIndex = 3
        '
        'pnAlternatorEfficiency
        '
        Me.pnAlternatorEfficiency.Controls.Add(Me.lblAlternatorEfficiency)
        Me.pnAlternatorEfficiency.Controls.Add(Me.lblAlternatorEfficiencyUnit)
        Me.pnAlternatorEfficiency.Controls.Add(Me.tbAlternatorEfficiency)
        Me.pnAlternatorEfficiency.Location = New System.Drawing.Point(10, 109)
        Me.pnAlternatorEfficiency.Name = "pnAlternatorEfficiency"
        Me.pnAlternatorEfficiency.Size = New System.Drawing.Size(288, 28)
        Me.pnAlternatorEfficiency.TabIndex = 3
        '
        'lblAlternatorEfficiency
        '
        Me.lblAlternatorEfficiency.AutoSize = true
        Me.lblAlternatorEfficiency.Location = New System.Drawing.Point(3, 7)
        Me.lblAlternatorEfficiency.Name = "lblAlternatorEfficiency"
        Me.lblAlternatorEfficiency.Size = New System.Drawing.Size(101, 13)
        Me.lblAlternatorEfficiency.TabIndex = 0
        Me.lblAlternatorEfficiency.Text = "Alternator Efficiency"
        '
        'lblAlternatorEfficiencyUnit
        '
        Me.lblAlternatorEfficiencyUnit.AutoSize = true
        Me.lblAlternatorEfficiencyUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblAlternatorEfficiencyUnit.Name = "lblAlternatorEfficiencyUnit"
        Me.lblAlternatorEfficiencyUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblAlternatorEfficiencyUnit.TabIndex = 24
        Me.lblAlternatorEfficiencyUnit.Text = "[-]"
        '
        'tbAlternatorEfficiency
        '
        Me.tbAlternatorEfficiency.Location = New System.Drawing.Point(197, 4)
        Me.tbAlternatorEfficiency.Name = "tbAlternatorEfficiency"
        Me.tbAlternatorEfficiency.Size = New System.Drawing.Size(57, 20)
        Me.tbAlternatorEfficiency.TabIndex = 3
        '
        'pnCurrentDemandEngineOffStandstill
        '
        Me.pnCurrentDemandEngineOffStandstill.Controls.Add(Me.lblCurrentDemandEngineIffStandstill)
        Me.pnCurrentDemandEngineOffStandstill.Controls.Add(Me.lblCurrentDemandEngienOffStandstillUnit)
        Me.pnCurrentDemandEngineOffStandstill.Controls.Add(Me.tbCurrentDemandEngineOffStandstill)
        Me.pnCurrentDemandEngineOffStandstill.Location = New System.Drawing.Point(10, 79)
        Me.pnCurrentDemandEngineOffStandstill.Name = "pnCurrentDemandEngineOffStandstill"
        Me.pnCurrentDemandEngineOffStandstill.Size = New System.Drawing.Size(288, 28)
        Me.pnCurrentDemandEngineOffStandstill.TabIndex = 2
        '
        'lblCurrentDemandEngineIffStandstill
        '
        Me.lblCurrentDemandEngineIffStandstill.AutoSize = true
        Me.lblCurrentDemandEngineIffStandstill.Location = New System.Drawing.Point(3, 7)
        Me.lblCurrentDemandEngineIffStandstill.Name = "lblCurrentDemandEngineIffStandstill"
        Me.lblCurrentDemandEngineIffStandstill.Size = New System.Drawing.Size(182, 13)
        Me.lblCurrentDemandEngineIffStandstill.TabIndex = 0
        Me.lblCurrentDemandEngineIffStandstill.Text = "Current Demand Engine Off Standstill"
        '
        'lblCurrentDemandEngienOffStandstillUnit
        '
        Me.lblCurrentDemandEngienOffStandstillUnit.AutoSize = true
        Me.lblCurrentDemandEngienOffStandstillUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblCurrentDemandEngienOffStandstillUnit.Name = "lblCurrentDemandEngienOffStandstillUnit"
        Me.lblCurrentDemandEngienOffStandstillUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblCurrentDemandEngienOffStandstillUnit.TabIndex = 24
        Me.lblCurrentDemandEngienOffStandstillUnit.Text = "[A]"
        '
        'tbCurrentDemandEngineOffStandstill
        '
        Me.tbCurrentDemandEngineOffStandstill.Location = New System.Drawing.Point(197, 4)
        Me.tbCurrentDemandEngineOffStandstill.Name = "tbCurrentDemandEngineOffStandstill"
        Me.tbCurrentDemandEngineOffStandstill.Size = New System.Drawing.Size(57, 20)
        Me.tbCurrentDemandEngineOffStandstill.TabIndex = 3
        '
        'pnCurrentDemandEngineOffDriving
        '
        Me.pnCurrentDemandEngineOffDriving.Controls.Add(Me.lblCurrentDemandEngineOffDriving)
        Me.pnCurrentDemandEngineOffDriving.Controls.Add(Me.lblCurrentDemandEngineOffDrivingUnit)
        Me.pnCurrentDemandEngineOffDriving.Controls.Add(Me.tbCurrentDemandEngineOffDriving)
        Me.pnCurrentDemandEngineOffDriving.Location = New System.Drawing.Point(10, 49)
        Me.pnCurrentDemandEngineOffDriving.Name = "pnCurrentDemandEngineOffDriving"
        Me.pnCurrentDemandEngineOffDriving.Size = New System.Drawing.Size(288, 28)
        Me.pnCurrentDemandEngineOffDriving.TabIndex = 1
        '
        'lblCurrentDemandEngineOffDriving
        '
        Me.lblCurrentDemandEngineOffDriving.AutoSize = true
        Me.lblCurrentDemandEngineOffDriving.Location = New System.Drawing.Point(3, 7)
        Me.lblCurrentDemandEngineOffDriving.Name = "lblCurrentDemandEngineOffDriving"
        Me.lblCurrentDemandEngineOffDriving.Size = New System.Drawing.Size(173, 13)
        Me.lblCurrentDemandEngineOffDriving.TabIndex = 0
        Me.lblCurrentDemandEngineOffDriving.Text = "Current Demand Engine Off Driving"
        '
        'lblCurrentDemandEngineOffDrivingUnit
        '
        Me.lblCurrentDemandEngineOffDrivingUnit.AutoSize = true
        Me.lblCurrentDemandEngineOffDrivingUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblCurrentDemandEngineOffDrivingUnit.Name = "lblCurrentDemandEngineOffDrivingUnit"
        Me.lblCurrentDemandEngineOffDrivingUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblCurrentDemandEngineOffDrivingUnit.TabIndex = 24
        Me.lblCurrentDemandEngineOffDrivingUnit.Text = "[A]"
        '
        'tbCurrentDemandEngineOffDriving
        '
        Me.tbCurrentDemandEngineOffDriving.Location = New System.Drawing.Point(197, 4)
        Me.tbCurrentDemandEngineOffDriving.Name = "tbCurrentDemandEngineOffDriving"
        Me.tbCurrentDemandEngineOffDriving.Size = New System.Drawing.Size(57, 20)
        Me.tbCurrentDemandEngineOffDriving.TabIndex = 3
        '
        'pnCurrentDemand
        '
        Me.pnCurrentDemand.Controls.Add(Me.lblCurrentDemand)
        Me.pnCurrentDemand.Controls.Add(Me.lblCurrentDemandUnit)
        Me.pnCurrentDemand.Controls.Add(Me.tbCurrentDemand)
        Me.pnCurrentDemand.Location = New System.Drawing.Point(10, 19)
        Me.pnCurrentDemand.Name = "pnCurrentDemand"
        Me.pnCurrentDemand.Size = New System.Drawing.Size(288, 28)
        Me.pnCurrentDemand.TabIndex = 0
        '
        'lblCurrentDemand
        '
        Me.lblCurrentDemand.AutoSize = true
        Me.lblCurrentDemand.Location = New System.Drawing.Point(3, 7)
        Me.lblCurrentDemand.Name = "lblCurrentDemand"
        Me.lblCurrentDemand.Size = New System.Drawing.Size(137, 13)
        Me.lblCurrentDemand.TabIndex = 0
        Me.lblCurrentDemand.Text = "Current Demand Engine On"
        '
        'lblCurrentDemandUnit
        '
        Me.lblCurrentDemandUnit.AutoSize = true
        Me.lblCurrentDemandUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblCurrentDemandUnit.Name = "lblCurrentDemandUnit"
        Me.lblCurrentDemandUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblCurrentDemandUnit.TabIndex = 24
        Me.lblCurrentDemandUnit.Text = "[A]"
        '
        'tbCurrentDemand
        '
        Me.tbCurrentDemand.Location = New System.Drawing.Point(197, 4)
        Me.tbCurrentDemand.Name = "tbCurrentDemand"
        Me.tbCurrentDemand.Size = New System.Drawing.Size(57, 20)
        Me.tbCurrentDemand.TabIndex = 3
        '
        'bgPneumaticSystem
        '
        Me.bgPneumaticSystem.Controls.Add(Me.pnSmartCompressor)
        Me.bgPneumaticSystem.Controls.Add(Me.pnCompressorRatio)
        Me.bgPneumaticSystem.Controls.Add(Me.pnCompressorMap)
        Me.bgPneumaticSystem.Controls.Add(Me.pnAverageAirDemand)
        Me.bgPneumaticSystem.Location = New System.Drawing.Point(335, 74)
        Me.bgPneumaticSystem.Name = "bgPneumaticSystem"
        Me.bgPneumaticSystem.Size = New System.Drawing.Size(317, 181)
        Me.bgPneumaticSystem.TabIndex = 1
        Me.bgPneumaticSystem.TabStop = false
        Me.bgPneumaticSystem.Text = "Pneumatic System"
        '
        'pnSmartCompressor
        '
        Me.pnSmartCompressor.Controls.Add(Me.cbSmartCompressor)
        Me.pnSmartCompressor.Location = New System.Drawing.Point(10, 141)
        Me.pnSmartCompressor.Name = "pnSmartCompressor"
        Me.pnSmartCompressor.Size = New System.Drawing.Size(288, 28)
        Me.pnSmartCompressor.TabIndex = 3
        '
        'cbSmartCompressor
        '
        Me.cbSmartCompressor.AutoSize = true
        Me.cbSmartCompressor.Location = New System.Drawing.Point(6, 6)
        Me.cbSmartCompressor.Name = "cbSmartCompressor"
        Me.cbSmartCompressor.Size = New System.Drawing.Size(126, 17)
        Me.cbSmartCompressor.TabIndex = 25
        Me.cbSmartCompressor.Text = "Smart Air Compressor"
        Me.cbSmartCompressor.UseVisualStyleBackColor = true
        '
        'pnCompressorRatio
        '
        Me.pnCompressorRatio.Controls.Add(Me.lblCompressorRatio)
        Me.pnCompressorRatio.Controls.Add(Me.lblCompressorRatioUnit)
        Me.pnCompressorRatio.Controls.Add(Me.tbCompressorRatio)
        Me.pnCompressorRatio.Location = New System.Drawing.Point(10, 111)
        Me.pnCompressorRatio.Name = "pnCompressorRatio"
        Me.pnCompressorRatio.Size = New System.Drawing.Size(288, 28)
        Me.pnCompressorRatio.TabIndex = 2
        '
        'lblCompressorRatio
        '
        Me.lblCompressorRatio.AutoSize = true
        Me.lblCompressorRatio.Location = New System.Drawing.Point(3, 7)
        Me.lblCompressorRatio.Name = "lblCompressorRatio"
        Me.lblCompressorRatio.Size = New System.Drawing.Size(90, 13)
        Me.lblCompressorRatio.TabIndex = 0
        Me.lblCompressorRatio.Text = "Compressor Ratio"
        '
        'lblCompressorRatioUnit
        '
        Me.lblCompressorRatioUnit.AutoSize = true
        Me.lblCompressorRatioUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblCompressorRatioUnit.Name = "lblCompressorRatioUnit"
        Me.lblCompressorRatioUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblCompressorRatioUnit.TabIndex = 24
        Me.lblCompressorRatioUnit.Text = "[-]"
        '
        'tbCompressorRatio
        '
        Me.tbCompressorRatio.Location = New System.Drawing.Point(197, 4)
        Me.tbCompressorRatio.Name = "tbCompressorRatio"
        Me.tbCompressorRatio.Size = New System.Drawing.Size(57, 20)
        Me.tbCompressorRatio.TabIndex = 3
        '
        'pnCompressorMap
        '
        Me.pnCompressorMap.Controls.Add(Me.lblCompressorMap)
        Me.pnCompressorMap.Controls.Add(Me.btnBrowseCompressorMap)
        Me.pnCompressorMap.Controls.Add(Me.tbCompressorMap)
        Me.pnCompressorMap.Location = New System.Drawing.Point(10, 19)
        Me.pnCompressorMap.Name = "pnCompressorMap"
        Me.pnCompressorMap.Size = New System.Drawing.Size(288, 56)
        Me.pnCompressorMap.TabIndex = 0
        '
        'lblCompressorMap
        '
        Me.lblCompressorMap.AutoSize = true
        Me.lblCompressorMap.Location = New System.Drawing.Point(3, 8)
        Me.lblCompressorMap.Name = "lblCompressorMap"
        Me.lblCompressorMap.Size = New System.Drawing.Size(86, 13)
        Me.lblCompressorMap.TabIndex = 3
        Me.lblCompressorMap.Text = "Compressor Map"
        '
        'btnBrowseCompressorMap
        '
        Me.btnBrowseCompressorMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowseCompressorMap.Image = CType(resources.GetObject("btnBrowseCompressorMap.Image"),System.Drawing.Image)
        Me.btnBrowseCompressorMap.Location = New System.Drawing.Point(261, 27)
        Me.btnBrowseCompressorMap.Name = "btnBrowseCompressorMap"
        Me.btnBrowseCompressorMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseCompressorMap.TabIndex = 2
        Me.btnBrowseCompressorMap.UseVisualStyleBackColor = true
        '
        'tbCompressorMap
        '
        Me.tbCompressorMap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbCompressorMap.Location = New System.Drawing.Point(6, 30)
        Me.tbCompressorMap.Name = "tbCompressorMap"
        Me.tbCompressorMap.Size = New System.Drawing.Size(249, 20)
        Me.tbCompressorMap.TabIndex = 1
        '
        'pnAverageAirDemand
        '
        Me.pnAverageAirDemand.Controls.Add(Me.lblAverageAirDemand)
        Me.pnAverageAirDemand.Controls.Add(Me.lblAverageAirDemandUnit)
        Me.pnAverageAirDemand.Controls.Add(Me.tbAverageAirDemand)
        Me.pnAverageAirDemand.Location = New System.Drawing.Point(10, 81)
        Me.pnAverageAirDemand.Name = "pnAverageAirDemand"
        Me.pnAverageAirDemand.Size = New System.Drawing.Size(300, 28)
        Me.pnAverageAirDemand.TabIndex = 1
        '
        'lblAverageAirDemand
        '
        Me.lblAverageAirDemand.AutoSize = true
        Me.lblAverageAirDemand.Location = New System.Drawing.Point(3, 7)
        Me.lblAverageAirDemand.Name = "lblAverageAirDemand"
        Me.lblAverageAirDemand.Size = New System.Drawing.Size(105, 13)
        Me.lblAverageAirDemand.TabIndex = 0
        Me.lblAverageAirDemand.Text = "Average Air Demand"
        '
        'lblAverageAirDemandUnit
        '
        Me.lblAverageAirDemandUnit.AutoSize = true
        Me.lblAverageAirDemandUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblAverageAirDemandUnit.Name = "lblAverageAirDemandUnit"
        Me.lblAverageAirDemandUnit.Size = New System.Drawing.Size(33, 13)
        Me.lblAverageAirDemandUnit.TabIndex = 24
        Me.lblAverageAirDemandUnit.Text = "[Nl/s]"
        '
        'tbAverageAirDemand
        '
        Me.tbAverageAirDemand.Location = New System.Drawing.Point(197, 4)
        Me.tbAverageAirDemand.Name = "tbAverageAirDemand"
        Me.tbAverageAirDemand.Size = New System.Drawing.Size(57, 20)
        Me.tbAverageAirDemand.TabIndex = 3
        '
        'gbHVAC
        '
        Me.gbHVAC.Controls.Add(Me.pnHvacHeatingDemand)
        Me.gbHVAC.Controls.Add(Me.pnHvacAuxHeaterPwr)
        Me.gbHVAC.Controls.Add(Me.pnHvacElecPowerDemand)
        Me.gbHVAC.Controls.Add(Me.pnHvacMechPowerDemand)
        Me.gbHVAC.Location = New System.Drawing.Point(658, 74)
        Me.gbHVAC.Name = "gbHVAC"
        Me.gbHVAC.Size = New System.Drawing.Size(317, 160)
        Me.gbHVAC.TabIndex = 2
        Me.gbHVAC.TabStop = false
        Me.gbHVAC.Text = "HVAC System"
        '
        'pnHvacHeatingDemand
        '
        Me.pnHvacHeatingDemand.Controls.Add(Me.lblHvacHeatingDemand)
        Me.pnHvacHeatingDemand.Controls.Add(Me.lblHvacHeatingDemandUnit)
        Me.pnHvacHeatingDemand.Controls.Add(Me.tbHvacHeatingDemand)
        Me.pnHvacHeatingDemand.Location = New System.Drawing.Point(10, 109)
        Me.pnHvacHeatingDemand.Name = "pnHvacHeatingDemand"
        Me.pnHvacHeatingDemand.Size = New System.Drawing.Size(288, 28)
        Me.pnHvacHeatingDemand.TabIndex = 3
        '
        'lblHvacHeatingDemand
        '
        Me.lblHvacHeatingDemand.AutoSize = true
        Me.lblHvacHeatingDemand.Location = New System.Drawing.Point(3, 7)
        Me.lblHvacHeatingDemand.Name = "lblHvacHeatingDemand"
        Me.lblHvacHeatingDemand.Size = New System.Drawing.Size(130, 13)
        Me.lblHvacHeatingDemand.TabIndex = 0
        Me.lblHvacHeatingDemand.Text = "Average Heating Demand"
        '
        'lblHvacHeatingDemandUnit
        '
        Me.lblHvacHeatingDemandUnit.AutoSize = true
        Me.lblHvacHeatingDemandUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblHvacHeatingDemandUnit.Name = "lblHvacHeatingDemandUnit"
        Me.lblHvacHeatingDemandUnit.Size = New System.Drawing.Size(27, 13)
        Me.lblHvacHeatingDemandUnit.TabIndex = 24
        Me.lblHvacHeatingDemandUnit.Text = "[MJ]"
        '
        'tbHvacHeatingDemand
        '
        Me.tbHvacHeatingDemand.Location = New System.Drawing.Point(197, 4)
        Me.tbHvacHeatingDemand.Name = "tbHvacHeatingDemand"
        Me.tbHvacHeatingDemand.Size = New System.Drawing.Size(57, 20)
        Me.tbHvacHeatingDemand.TabIndex = 3
        '
        'pnHvacAuxHeaterPwr
        '
        Me.pnHvacAuxHeaterPwr.Controls.Add(Me.lblHvacAuxHEaterPwr)
        Me.pnHvacAuxHeaterPwr.Controls.Add(Me.lblHvacAuxHeaterPwrUnit)
        Me.pnHvacAuxHeaterPwr.Controls.Add(Me.tbHvacAuxHeaterPwr)
        Me.pnHvacAuxHeaterPwr.Location = New System.Drawing.Point(10, 79)
        Me.pnHvacAuxHeaterPwr.Name = "pnHvacAuxHeaterPwr"
        Me.pnHvacAuxHeaterPwr.Size = New System.Drawing.Size(288, 28)
        Me.pnHvacAuxHeaterPwr.TabIndex = 2
        '
        'lblHvacAuxHEaterPwr
        '
        Me.lblHvacAuxHEaterPwr.AutoSize = true
        Me.lblHvacAuxHEaterPwr.Location = New System.Drawing.Point(3, 7)
        Me.lblHvacAuxHEaterPwr.Name = "lblHvacAuxHEaterPwr"
        Me.lblHvacAuxHEaterPwr.Size = New System.Drawing.Size(93, 13)
        Me.lblHvacAuxHEaterPwr.TabIndex = 0
        Me.lblHvacAuxHEaterPwr.Text = "Aux Heater Power"
        '
        'lblHvacAuxHeaterPwrUnit
        '
        Me.lblHvacAuxHeaterPwrUnit.AutoSize = true
        Me.lblHvacAuxHeaterPwrUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblHvacAuxHeaterPwrUnit.Name = "lblHvacAuxHeaterPwrUnit"
        Me.lblHvacAuxHeaterPwrUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblHvacAuxHeaterPwrUnit.TabIndex = 24
        Me.lblHvacAuxHeaterPwrUnit.Text = "[W]"
        '
        'tbHvacAuxHeaterPwr
        '
        Me.tbHvacAuxHeaterPwr.Location = New System.Drawing.Point(197, 4)
        Me.tbHvacAuxHeaterPwr.Name = "tbHvacAuxHeaterPwr"
        Me.tbHvacAuxHeaterPwr.Size = New System.Drawing.Size(57, 20)
        Me.tbHvacAuxHeaterPwr.TabIndex = 3
        '
        'pnHvacElecPowerDemand
        '
        Me.pnHvacElecPowerDemand.Controls.Add(Me.lblHvacElectricPowerDemand)
        Me.pnHvacElecPowerDemand.Controls.Add(Me.lblHvacElectricPowerDemandUnit)
        Me.pnHvacElecPowerDemand.Controls.Add(Me.tbHvacElectricPowerDemand)
        Me.pnHvacElecPowerDemand.Location = New System.Drawing.Point(10, 49)
        Me.pnHvacElecPowerDemand.Name = "pnHvacElecPowerDemand"
        Me.pnHvacElecPowerDemand.Size = New System.Drawing.Size(288, 28)
        Me.pnHvacElecPowerDemand.TabIndex = 1
        '
        'lblHvacElectricPowerDemand
        '
        Me.lblHvacElectricPowerDemand.AutoSize = true
        Me.lblHvacElectricPowerDemand.Location = New System.Drawing.Point(3, 7)
        Me.lblHvacElectricPowerDemand.Name = "lblHvacElectricPowerDemand"
        Me.lblHvacElectricPowerDemand.Size = New System.Drawing.Size(126, 13)
        Me.lblHvacElectricPowerDemand.TabIndex = 0
        Me.lblHvacElectricPowerDemand.Text = "Electrical Power Demand"
        '
        'lblHvacElectricPowerDemandUnit
        '
        Me.lblHvacElectricPowerDemandUnit.AutoSize = true
        Me.lblHvacElectricPowerDemandUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblHvacElectricPowerDemandUnit.Name = "lblHvacElectricPowerDemandUnit"
        Me.lblHvacElectricPowerDemandUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblHvacElectricPowerDemandUnit.TabIndex = 24
        Me.lblHvacElectricPowerDemandUnit.Text = "[W]"
        '
        'tbHvacElectricPowerDemand
        '
        Me.tbHvacElectricPowerDemand.Location = New System.Drawing.Point(197, 4)
        Me.tbHvacElectricPowerDemand.Name = "tbHvacElectricPowerDemand"
        Me.tbHvacElectricPowerDemand.Size = New System.Drawing.Size(57, 20)
        Me.tbHvacElectricPowerDemand.TabIndex = 3
        '
        'pnHvacMechPowerDemand
        '
        Me.pnHvacMechPowerDemand.Controls.Add(Me.lblHvacMechPowerDemand)
        Me.pnHvacMechPowerDemand.Controls.Add(Me.lblHvacMechPowerDemandUnit)
        Me.pnHvacMechPowerDemand.Controls.Add(Me.tbHvacMechPowerDemand)
        Me.pnHvacMechPowerDemand.Location = New System.Drawing.Point(10, 19)
        Me.pnHvacMechPowerDemand.Name = "pnHvacMechPowerDemand"
        Me.pnHvacMechPowerDemand.Size = New System.Drawing.Size(288, 28)
        Me.pnHvacMechPowerDemand.TabIndex = 0
        '
        'lblHvacMechPowerDemand
        '
        Me.lblHvacMechPowerDemand.AutoSize = true
        Me.lblHvacMechPowerDemand.Location = New System.Drawing.Point(3, 7)
        Me.lblHvacMechPowerDemand.Name = "lblHvacMechPowerDemand"
        Me.lblHvacMechPowerDemand.Size = New System.Drawing.Size(138, 13)
        Me.lblHvacMechPowerDemand.TabIndex = 0
        Me.lblHvacMechPowerDemand.Text = "Mechanical Power Demand"
        '
        'lblHvacMechPowerDemandUnit
        '
        Me.lblHvacMechPowerDemandUnit.AutoSize = true
        Me.lblHvacMechPowerDemandUnit.Location = New System.Drawing.Point(258, 7)
        Me.lblHvacMechPowerDemandUnit.Name = "lblHvacMechPowerDemandUnit"
        Me.lblHvacMechPowerDemandUnit.Size = New System.Drawing.Size(24, 13)
        Me.lblHvacMechPowerDemandUnit.TabIndex = 24
        Me.lblHvacMechPowerDemandUnit.Text = "[W]"
        '
        'tbHvacMechPowerDemand
        '
        Me.tbHvacMechPowerDemand.Location = New System.Drawing.Point(197, 4)
        Me.tbHvacMechPowerDemand.Name = "tbHvacMechPowerDemand"
        Me.tbHvacMechPowerDemand.Size = New System.Drawing.Size(57, 20)
        Me.tbHvacMechPowerDemand.TabIndex = 3
        '
        'BusAuxiliariesEngParametersForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(1023, 446)
        Me.Controls.Add(Me.gbHVAC)
        Me.Controls.Add(Me.bgPneumaticSystem)
        Me.Controls.Add(Me.gbElectricSystem)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "BusAuxiliariesEngParametersForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Bus Auxiliaries Engineering"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.gbElectricSystem.ResumeLayout(false)
        Me.pnDCDCEff.ResumeLayout(false)
        Me.pnDCDCEff.PerformLayout
        Me.pnES_HEVREESS.ResumeLayout(false)
        Me.pnES_HEVREESS.PerformLayout
        Me.pnAlternatorTechnology.ResumeLayout(false)
        Me.pnAlternatorTechnology.PerformLayout
        Me.pnSmartElectricParams.ResumeLayout(false)
        Me.pnBattEfficiency.ResumeLayout(false)
        Me.pnBattEfficiency.PerformLayout
        Me.pnElectricStorageCapacity.ResumeLayout(false)
        Me.pnElectricStorageCapacity.PerformLayout
        Me.pnMaxAlternatorPower.ResumeLayout(false)
        Me.pnMaxAlternatorPower.PerformLayout
        Me.pnAlternatorEfficiency.ResumeLayout(false)
        Me.pnAlternatorEfficiency.PerformLayout
        Me.pnCurrentDemandEngineOffStandstill.ResumeLayout(false)
        Me.pnCurrentDemandEngineOffStandstill.PerformLayout
        Me.pnCurrentDemandEngineOffDriving.ResumeLayout(false)
        Me.pnCurrentDemandEngineOffDriving.PerformLayout
        Me.pnCurrentDemand.ResumeLayout(false)
        Me.pnCurrentDemand.PerformLayout
        Me.bgPneumaticSystem.ResumeLayout(false)
        Me.pnSmartCompressor.ResumeLayout(false)
        Me.pnSmartCompressor.PerformLayout
        Me.pnCompressorRatio.ResumeLayout(false)
        Me.pnCompressorRatio.PerformLayout
        Me.pnCompressorMap.ResumeLayout(false)
        Me.pnCompressorMap.PerformLayout
        Me.pnAverageAirDemand.ResumeLayout(false)
        Me.pnAverageAirDemand.PerformLayout
        Me.gbHVAC.ResumeLayout(false)
        Me.pnHvacHeatingDemand.ResumeLayout(false)
        Me.pnHvacHeatingDemand.PerformLayout
        Me.pnHvacAuxHeaterPwr.ResumeLayout(false)
        Me.pnHvacAuxHeaterPwr.PerformLayout
        Me.pnHvacElecPowerDemand.ResumeLayout(false)
        Me.pnHvacElecPowerDemand.PerformLayout
        Me.pnHvacMechPowerDemand.ResumeLayout(false)
        Me.pnHvacMechPowerDemand.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
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
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblTitle As Label
    Friend WithEvents gbElectricSystem As GroupBox
    Friend WithEvents pnCurrentDemandEngineOffStandstill As Panel
    Friend WithEvents lblCurrentDemandEngineIffStandstill As Label
    Friend WithEvents lblCurrentDemandEngienOffStandstillUnit As Label
    Friend WithEvents tbCurrentDemandEngineOffStandstill As TextBox
    Friend WithEvents pnCurrentDemandEngineOffDriving As Panel
    Friend WithEvents lblCurrentDemandEngineOffDriving As Label
    Friend WithEvents lblCurrentDemandEngineOffDrivingUnit As Label
    Friend WithEvents tbCurrentDemandEngineOffDriving As TextBox
    Friend WithEvents pnCurrentDemand As Panel
    Friend WithEvents lblCurrentDemand As Label
    Friend WithEvents lblCurrentDemandUnit As Label
    Friend WithEvents tbCurrentDemand As TextBox
    Friend WithEvents pnSmartElectricParams As Panel
    Friend WithEvents pnElectricStorageCapacity As Panel
    Friend WithEvents lblElectricStorageCapacity As Label
    Friend WithEvents lblElectricStorageCapacityUnit As Label
    Friend WithEvents tbElectricStorageCapacity As TextBox
    Friend WithEvents pnMaxAlternatorPower As Panel
    Friend WithEvents lblMaxAlternatorPower As Label
    Friend WithEvents lblMaxAlternatorPowerUnit As Label
    Friend WithEvents tbMaxAlternatorPower As TextBox
    Friend WithEvents pnAlternatorEfficiency As Panel
    Friend WithEvents lblAlternatorEfficiency As Label
    Friend WithEvents lblAlternatorEfficiencyUnit As Label
    Friend WithEvents tbAlternatorEfficiency As TextBox
    Friend WithEvents bgPneumaticSystem As GroupBox
    Friend WithEvents pnAverageAirDemand As Panel
    Friend WithEvents lblAverageAirDemand As Label
    Friend WithEvents lblAverageAirDemandUnit As Label
    Friend WithEvents tbAverageAirDemand As TextBox
    Friend WithEvents pnCompressorMap As Panel
    Friend WithEvents lblCompressorMap As Label
    Friend WithEvents btnBrowseCompressorMap As Button
    Friend WithEvents tbCompressorMap As TextBox
    Friend WithEvents pnSmartCompressor As Panel
    Friend WithEvents cbSmartCompressor As CheckBox
    Friend WithEvents pnCompressorRatio As Panel
    Friend WithEvents lblCompressorRatio As Label
    Friend WithEvents lblCompressorRatioUnit As Label
    Friend WithEvents tbCompressorRatio As TextBox
    Friend WithEvents gbHVAC As GroupBox
    Friend WithEvents pnHvacHeatingDemand As Panel
    Friend WithEvents lblHvacHeatingDemand As Label
    Friend WithEvents lblHvacHeatingDemandUnit As Label
    Friend WithEvents tbHvacHeatingDemand As TextBox
    Friend WithEvents pnHvacAuxHeaterPwr As Panel
    Friend WithEvents lblHvacAuxHEaterPwr As Label
    Friend WithEvents lblHvacAuxHeaterPwrUnit As Label
    Friend WithEvents tbHvacAuxHeaterPwr As TextBox
    Friend WithEvents pnHvacElecPowerDemand As Panel
    Friend WithEvents lblHvacElectricPowerDemand As Label
    Friend WithEvents lblHvacElectricPowerDemandUnit As Label
    Friend WithEvents tbHvacElectricPowerDemand As TextBox
    Friend WithEvents pnHvacMechPowerDemand As Panel
    Friend WithEvents lblHvacMechPowerDemand As Label
    Friend WithEvents lblHvacMechPowerDemandUnit As Label
    Friend WithEvents tbHvacMechPowerDemand As TextBox
    Friend WithEvents pnAlternatorTechnology As Panel
    Friend WithEvents cbAlternatorTechnology As ComboBox
    Friend WithEvents lbAlternatorTechnology As Label
    Friend WithEvents pnDCDCEff As Panel
    Friend WithEvents lblDCDCEff As Label
    Friend WithEvents lblDCDCEffUnit As Label
    Friend WithEvents tbDCDCEff As TextBox
    Friend WithEvents pnES_HEVREESS As Panel
    Friend WithEvents cbES_HEVREESS As CheckBox
    Friend WithEvents pnBattEfficiency As Panel
    Friend WithEvents lblBatEfficiency As Label
    Friend WithEvents lblBatEfficiencyUnit As Label
    Friend WithEvents tbBatEfficiency As TextBox
End Class
