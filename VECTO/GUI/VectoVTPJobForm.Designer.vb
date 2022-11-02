Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VectoVTPJobForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VectoVTPJobForm))
        Me.GrCycles = New System.Windows.Forms.GroupBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LvCycles = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.BtDRIrem = New System.Windows.Forms.Button()
        Me.BtDRIadd = New System.Windows.Forms.Button()
        Me.GrAux = New System.Windows.Forms.GroupBox()
        Me.LvAux = New System.Windows.Forms.ListView()
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.TbVEH = New System.Windows.Forms.TextBox()
        Me.ButtonVEH = New System.Windows.Forms.Button()
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
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.tbFanDiameter = New System.Windows.Forms.TextBox()
        Me.tbC4 = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.pnFanParameters = New System.Windows.Forms.Panel()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbC1 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbC3 = New System.Windows.Forms.TextBox()
        Me.tbC2 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnManufacturerRecord = New System.Windows.Forms.Panel()
        Me.lblMileageUnit = New System.Windows.Forms.Label()
        Me.tbMileage = New System.Windows.Forms.TextBox()
        Me.lblMileage = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.tbManufacturerRecord = New System.Windows.Forms.TextBox()
        Me.ButtonManR = New System.Windows.Forms.Button()
        Me.GrCycles.SuspendLayout
        Me.GrAux.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.ToolStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PictureBox2,System.ComponentModel.ISupportInitialize).BeginInit
        Me.GroupBox1.SuspendLayout
        Me.pnFanParameters.SuspendLayout
        Me.pnManufacturerRecord.SuspendLayout
        Me.SuspendLayout
        '
        'GrCycles
        '
        Me.GrCycles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GrCycles.Controls.Add(Me.Label12)
        Me.GrCycles.Controls.Add(Me.Label2)
        Me.GrCycles.Controls.Add(Me.LvCycles)
        Me.GrCycles.Controls.Add(Me.BtDRIrem)
        Me.GrCycles.Controls.Add(Me.BtDRIadd)
        Me.GrCycles.Location = New System.Drawing.Point(11, 324)
        Me.GrCycles.Name = "GrCycles"
        Me.GrCycles.Size = New System.Drawing.Size(515, 166)
        Me.GrCycles.TabIndex = 3
        Me.GrCycles.TabStop = false
        Me.GrCycles.Text = "Cycles"
        '
        'Label12
        '
        Me.Label12.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label12.AutoSize = true
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label12.Location = New System.Drawing.Point(6, 20)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(426, 13)
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "The fuel cocnsumption in the cycle has to be corrected for standard NCV!"
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(391, 137)
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
        Me.LvCycles.Location = New System.Drawing.Point(6, 40)
        Me.LvCycles.MultiSelect = false
        Me.LvCycles.Name = "LvCycles"
        Me.LvCycles.Size = New System.Drawing.Size(503, 96)
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
        Me.BtDRIrem.Location = New System.Drawing.Point(29, 137)
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
        Me.BtDRIadd.Location = New System.Drawing.Point(5, 137)
        Me.BtDRIadd.Name = "BtDRIadd"
        Me.BtDRIadd.Size = New System.Drawing.Size(24, 24)
        Me.BtDRIadd.TabIndex = 1
        Me.BtDRIadd.UseVisualStyleBackColor = true
        '
        'GrAux
        '
        Me.GrAux.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.GrAux.Controls.Add(Me.LvAux)
        Me.GrAux.Location = New System.Drawing.Point(11, 178)
        Me.GrAux.Name = "GrAux"
        Me.GrAux.Size = New System.Drawing.Size(515, 140)
        Me.GrAux.TabIndex = 2
        Me.GrAux.TabStop = false
        Me.GrAux.Text = "Auxiliaries"
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
        Me.LvAux.Location = New System.Drawing.Point(4, 19)
        Me.LvAux.MultiSelect = false
        Me.LvAux.Name = "LvAux"
        Me.LvAux.Size = New System.Drawing.Size(503, 115)
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
        'TbVEH
        '
        Me.TbVEH.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbVEH.Location = New System.Drawing.Point(128, 86)
        Me.TbVEH.Name = "TbVEH"
        Me.TbVEH.Size = New System.Drawing.Size(373, 20)
        Me.TbVEH.TabIndex = 0
        '
        'ButtonVEH
        '
        Me.ButtonVEH.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"),System.Drawing.Image)
        Me.ButtonVEH.Location = New System.Drawing.Point(502, 84)
        Me.ButtonVEH.Name = "ButtonVEH"
        Me.ButtonVEH.Size = New System.Drawing.Size(24, 24)
        Me.ButtonVEH.TabIndex = 2
        Me.ButtonVEH.UseVisualStyleBackColor = true
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelGEN})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 660)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(944, 22)
        Me.StatusStrip1.SizingGrip = false
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
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(778, 633)
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
        Me.ButCancel.Location = New System.Drawing.Point(859, 633)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 6
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
        Me.ToolStrip1.Size = New System.Drawing.Size(944, 31)
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
        Me.PictureBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VECTO
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(944, 40)
        Me.PictureBox1.TabIndex = 21
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
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
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(542, 80)
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
        Me.PicBox.Location = New System.Drawing.Point(542, 219)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(390, 296)
        Me.PicBox.TabIndex = 36
        Me.PicBox.TabStop = false
        '
        'TbEngTxt
        '
        Me.TbEngTxt.Location = New System.Drawing.Point(542, 171)
        Me.TbEngTxt.Name = "TbEngTxt"
        Me.TbEngTxt.ReadOnly = true
        Me.TbEngTxt.Size = New System.Drawing.Size(390, 20)
        Me.TbEngTxt.TabIndex = 6
        '
        'TbVehCat
        '
        Me.TbVehCat.Location = New System.Drawing.Point(848, 84)
        Me.TbVehCat.Name = "TbVehCat"
        Me.TbVehCat.ReadOnly = true
        Me.TbVehCat.Size = New System.Drawing.Size(87, 20)
        Me.TbVehCat.TabIndex = 2
        '
        'TbAxleConf
        '
        Me.TbAxleConf.Location = New System.Drawing.Point(904, 113)
        Me.TbAxleConf.Name = "TbAxleConf"
        Me.TbAxleConf.ReadOnly = true
        Me.TbAxleConf.Size = New System.Drawing.Size(31, 20)
        Me.TbAxleConf.TabIndex = 4
        '
        'TbHVCclass
        '
        Me.TbHVCclass.Location = New System.Drawing.Point(848, 142)
        Me.TbHVCclass.Name = "TbHVCclass"
        Me.TbHVCclass.ReadOnly = true
        Me.TbHVCclass.Size = New System.Drawing.Size(87, 20)
        Me.TbHVCclass.TabIndex = 5
        '
        'TbGbxTxt
        '
        Me.TbGbxTxt.Location = New System.Drawing.Point(542, 194)
        Me.TbGbxTxt.Name = "TbGbxTxt"
        Me.TbGbxTxt.ReadOnly = true
        Me.TbGbxTxt.Size = New System.Drawing.Size(390, 20)
        Me.TbGbxTxt.TabIndex = 7
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(848, 113)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.ReadOnly = true
        Me.TbMass.Size = New System.Drawing.Size(50, 20)
        Me.TbMass.TabIndex = 3
        '
        'lblEngineCharacteristics
        '
        Me.lblEngineCharacteristics.AutoSize = true
        Me.lblEngineCharacteristics.Location = New System.Drawing.Point(542, 518)
        Me.lblEngineCharacteristics.Name = "lblEngineCharacteristics"
        Me.lblEngineCharacteristics.Size = New System.Drawing.Size(0, 13)
        Me.lblEngineCharacteristics.TabIndex = 37
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.TUGraz.VECTO.My.Resources.Resources.P_fan_eqn
        Me.PictureBox2.InitialImage = CType(resources.GetObject("PictureBox2.InitialImage"),System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(7, 19)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(277, 108)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 38
        Me.PictureBox2.TabStop = false
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.tbFanDiameter)
        Me.GroupBox1.Controls.Add(Me.tbC4)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.pnFanParameters)
        Me.GroupBox1.Controls.Add(Me.PictureBox2)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Location = New System.Drawing.Point(10, 496)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(517, 144)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Fan Power"
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(463, 120)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(16, 13)
        Me.Label13.TabIndex = 53
        Me.Label13.Text = "[-]"
        '
        'tbFanDiameter
        '
        Me.tbFanDiameter.Location = New System.Drawing.Point(371, 13)
        Me.tbFanDiameter.Name = "tbFanDiameter"
        Me.tbFanDiameter.Size = New System.Drawing.Size(86, 20)
        Me.tbFanDiameter.TabIndex = 0
        '
        'tbC4
        '
        Me.tbC4.Location = New System.Drawing.Point(371, 117)
        Me.tbC4.Name = "tbC4"
        Me.tbC4.Size = New System.Drawing.Size(86, 20)
        Me.tbC4.TabIndex = 2
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(342, 120)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(23, 13)
        Me.Label14.TabIndex = 51
        Me.Label14.Text = "C4:"
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Location = New System.Drawing.Point(463, 16)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(29, 13)
        Me.Label7.TabIndex = 47
        Me.Label7.Text = "[mm]"
        '
        'pnFanParameters
        '
        Me.pnFanParameters.Controls.Add(Me.Label11)
        Me.pnFanParameters.Controls.Add(Me.Label10)
        Me.pnFanParameters.Controls.Add(Me.Label9)
        Me.pnFanParameters.Controls.Add(Me.Label1)
        Me.pnFanParameters.Controls.Add(Me.tbC1)
        Me.pnFanParameters.Controls.Add(Me.Label3)
        Me.pnFanParameters.Controls.Add(Me.tbC3)
        Me.pnFanParameters.Controls.Add(Me.tbC2)
        Me.pnFanParameters.Controls.Add(Me.Label4)
        Me.pnFanParameters.Location = New System.Drawing.Point(290, 36)
        Me.pnFanParameters.Name = "pnFanParameters"
        Me.pnFanParameters.Size = New System.Drawing.Size(218, 78)
        Me.pnFanParameters.TabIndex = 1
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(173, 58)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(29, 13)
        Me.Label11.TabIndex = 50
        Me.Label11.Text = "[mm]"
        '
        'Label10
        '
        Me.Label10.AutoSize = true
        Me.Label10.Location = New System.Drawing.Point(173, 32)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(30, 13)
        Me.Label10.TabIndex = 49
        Me.Label10.Text = "[rpm]"
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(173, 6)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(30, 13)
        Me.Label9.TabIndex = 48
        Me.Label9.Text = "[kW]"
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(52, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(23, 13)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "C1:"
        '
        'tbC1
        '
        Me.tbC1.Location = New System.Drawing.Point(81, 3)
        Me.tbC1.Name = "tbC1"
        Me.tbC1.Size = New System.Drawing.Size(86, 20)
        Me.tbC1.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(52, 32)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(23, 13)
        Me.Label3.TabIndex = 41
        Me.Label3.Text = "C2:"
        '
        'tbC3
        '
        Me.tbC3.Location = New System.Drawing.Point(81, 55)
        Me.tbC3.Name = "tbC3"
        Me.tbC3.Size = New System.Drawing.Size(86, 20)
        Me.tbC3.TabIndex = 2
        '
        'tbC2
        '
        Me.tbC2.Location = New System.Drawing.Point(81, 29)
        Me.tbC2.Name = "tbC2"
        Me.tbC2.Size = New System.Drawing.Size(86, 20)
        Me.tbC2.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(52, 58)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(23, 13)
        Me.Label4.TabIndex = 43
        Me.Label4.Text = "C3:"
        '
        'Label6
        '
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(294, 16)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(71, 13)
        Me.Label6.TabIndex = 45
        Me.Label6.Text = "Fan diameter:"
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(15, 89)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(70, 13)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "Vehicle XML:"
        '
        'pnManufacturerRecord
        '
        Me.pnManufacturerRecord.Controls.Add(Me.lblMileageUnit)
        Me.pnManufacturerRecord.Controls.Add(Me.tbMileage)
        Me.pnManufacturerRecord.Controls.Add(Me.lblMileage)
        Me.pnManufacturerRecord.Controls.Add(Me.Label8)
        Me.pnManufacturerRecord.Controls.Add(Me.tbManufacturerRecord)
        Me.pnManufacturerRecord.Controls.Add(Me.ButtonManR)
        Me.pnManufacturerRecord.Location = New System.Drawing.Point(11, 112)
        Me.pnManufacturerRecord.Name = "pnManufacturerRecord"
        Me.pnManufacturerRecord.Size = New System.Drawing.Size(525, 66)
        Me.pnManufacturerRecord.TabIndex = 1
        '
        'lblMileageUnit
        '
        Me.lblMileageUnit.AutoSize = true
        Me.lblMileageUnit.Location = New System.Drawing.Point(199, 43)
        Me.lblMileageUnit.Name = "lblMileageUnit"
        Me.lblMileageUnit.Size = New System.Drawing.Size(27, 13)
        Me.lblMileageUnit.TabIndex = 49
        Me.lblMileageUnit.Text = "[km]"
        '
        'tbMileage
        '
        Me.tbMileage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbMileage.Location = New System.Drawing.Point(117, 40)
        Me.tbMileage.Name = "tbMileage"
        Me.tbMileage.Size = New System.Drawing.Size(76, 20)
        Me.tbMileage.TabIndex = 2
        '
        'lblMileage
        '
        Me.lblMileage.AutoSize = true
        Me.lblMileage.Location = New System.Drawing.Point(4, 43)
        Me.lblMileage.Name = "lblMileage"
        Me.lblMileage.Size = New System.Drawing.Size(47, 13)
        Me.lblMileage.TabIndex = 47
        Me.lblMileage.Text = "Mileage:"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(3, 11)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(111, 13)
        Me.Label8.TabIndex = 43
        Me.Label8.Text = "Manufacturer Record:"
        '
        'tbManufacturerRecord
        '
        Me.tbManufacturerRecord.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbManufacturerRecord.Location = New System.Drawing.Point(117, 8)
        Me.tbManufacturerRecord.Name = "tbManufacturerRecord"
        Me.tbManufacturerRecord.Size = New System.Drawing.Size(374, 20)
        Me.tbManufacturerRecord.TabIndex = 0
        '
        'ButtonManR
        '
        Me.ButtonManR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButtonManR.Image = CType(resources.GetObject("ButtonManR.Image"),System.Drawing.Image)
        Me.ButtonManR.Location = New System.Drawing.Point(492, 6)
        Me.ButtonManR.Name = "ButtonManR"
        Me.ButtonManR.Size = New System.Drawing.Size(24, 24)
        Me.ButtonManR.TabIndex = 1
        Me.ButtonManR.UseVisualStyleBackColor = true
        '
        'VectoVTPJobForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(944, 682)
        Me.Controls.Add(Me.pnManufacturerRecord)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GrAux)
        Me.Controls.Add(Me.GrCycles)
        Me.Controls.Add(Me.lblEngineCharacteristics)
        Me.Controls.Add(Me.TbHVCclass)
        Me.Controls.Add(Me.TbMass)
        Me.Controls.Add(Me.TbVEH)
        Me.Controls.Add(Me.TbAxleConf)
        Me.Controls.Add(Me.TbVehCat)
        Me.Controls.Add(Me.TbGbxTxt)
        Me.Controls.Add(Me.TbEngTxt)
        Me.Controls.Add(Me.ButtonVEH)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.PicVehicle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.StatusStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "VectoVTPJobForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Job Editor"
        Me.GrCycles.ResumeLayout(false)
        Me.GrCycles.PerformLayout
        Me.GrAux.ResumeLayout(false)
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PictureBox2,System.ComponentModel.ISupportInitialize).EndInit
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.pnFanParameters.ResumeLayout(false)
        Me.pnFanParameters.PerformLayout
        Me.pnManufacturerRecord.ResumeLayout(false)
        Me.pnManufacturerRecord.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents ButtonVEH As Button
	Friend WithEvents ToolStripStatusLabelGEN As ToolStripStatusLabel
	Friend WithEvents ButOK As Button
	Friend WithEvents TbVEH As TextBox
	Friend WithEvents ButCancel As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents GrAux As GroupBox
	Friend WithEvents LvAux As ListView
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents GrCycles As GroupBox
	Friend WithEvents LvCycles As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents BtDRIrem As Button
	Friend WithEvents BtDRIadd As Button
	Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents Label2 As Label
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents PicVehicle As PictureBox
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents TbEngTxt As TextBox
	Friend WithEvents TbVehCat As TextBox
	Friend WithEvents TbAxleConf As TextBox
	Friend WithEvents TbHVCclass As TextBox
	Friend WithEvents TbGbxTxt As TextBox
	Friend WithEvents TbMass As TextBox
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents lblEngineCharacteristics As System.Windows.Forms.Label
	Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents tbC3 As System.Windows.Forms.TextBox
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents tbC2 As System.Windows.Forms.TextBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents tbC1 As System.Windows.Forms.TextBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents tbFanDiameter As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents pnManufacturerRecord As Panel
    Friend WithEvents Label8 As Label
    Friend WithEvents tbManufacturerRecord As TextBox
    Friend WithEvents ButtonManR As Button
    Friend WithEvents lblMileageUnit As Label
    Friend WithEvents tbMileage As TextBox
    Friend WithEvents lblMileage As Label
    Friend WithEvents pnFanParameters As Panel
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents tbC4 As TextBox
    Friend WithEvents Label14 As Label
End Class
