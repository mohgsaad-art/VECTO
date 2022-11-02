<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XMLExportJobDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(XMLExportJobDialog))
        Me.lbVendor = New System.Windows.Forms.Label()
        Me.tbVendor = New System.Windows.Forms.TextBox()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.cbSingleFile = New System.Windows.Forms.CheckBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.tbJobfile = New System.Windows.Forms.TextBox()
        Me.tbMode = New System.Windows.Forms.TextBox()
        Me.lblJobfile = New System.Windows.Forms.Label()
        Me.lblDstDir = New System.Windows.Forms.Label()
        Me.tbDestination = New System.Windows.Forms.TextBox()
        Me.lblMode = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.BtTCfileBrowse = New System.Windows.Forms.Button()
        Me.SuspendLayout
        '
        'lbVendor
        '
        Me.lbVendor.AutoSize = true
        Me.lbVendor.Location = New System.Drawing.Point(10, 97)
        Me.lbVendor.Name = "lbVendor"
        Me.lbVendor.Size = New System.Drawing.Size(44, 13)
        Me.lbVendor.TabIndex = 25
        Me.lbVendor.Text = "Vendor:"
        '
        'tbVendor
        '
        Me.tbVendor.Location = New System.Drawing.Point(131, 94)
        Me.tbVendor.Name = "tbVendor"
        Me.tbVendor.Size = New System.Drawing.Size(231, 20)
        Me.tbVendor.TabIndex = 4
        '
        'btnExport
        '
        Me.btnExport.Location = New System.Drawing.Point(224, 150)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(75, 23)
        Me.btnExport.TabIndex = 6
        Me.btnExport.Text = "Export"
        Me.btnExport.UseVisualStyleBackColor = true
        '
        'cbSingleFile
        '
        Me.cbSingleFile.AutoSize = true
        Me.cbSingleFile.Location = New System.Drawing.Point(132, 121)
        Me.cbSingleFile.Name = "cbSingleFile"
        Me.cbSingleFile.Size = New System.Drawing.Size(15, 14)
        Me.cbSingleFile.TabIndex = 22
        Me.cbSingleFile.UseVisualStyleBackColor = true
        '
        'label1
        '
        Me.label1.AutoSize = true
        Me.label1.Location = New System.Drawing.Point(10, 121)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(116, 13)
        Me.label1.TabIndex = 5
        Me.label1.Text = "Output single XML File:"
        '
        'tbJobfile
        '
        Me.tbJobfile.Location = New System.Drawing.Point(131, 20)
        Me.tbJobfile.Name = "tbJobfile"
        Me.tbJobfile.ReadOnly = true
        Me.tbJobfile.Size = New System.Drawing.Size(231, 20)
        Me.tbJobfile.TabIndex = 0
        '
        'tbMode
        '
        Me.tbMode.Location = New System.Drawing.Point(131, 43)
        Me.tbMode.Name = "tbMode"
        Me.tbMode.ReadOnly = true
        Me.tbMode.Size = New System.Drawing.Size(231, 20)
        Me.tbMode.TabIndex = 1
        '
        'lblJobfile
        '
        Me.lblJobfile.AutoSize = true
        Me.lblJobfile.Location = New System.Drawing.Point(10, 23)
        Me.lblJobfile.Name = "lblJobfile"
        Me.lblJobfile.Size = New System.Drawing.Size(58, 13)
        Me.lblJobfile.TabIndex = 17
        Me.lblJobfile.Text = "Vecto Job:"
        '
        'lblDstDir
        '
        Me.lblDstDir.AutoSize = true
        Me.lblDstDir.Location = New System.Drawing.Point(10, 71)
        Me.lblDstDir.Name = "lblDstDir"
        Me.lblDstDir.Size = New System.Drawing.Size(87, 13)
        Me.lblDstDir.TabIndex = 16
        Me.lblDstDir.Text = "Output Directory:"
        '
        'tbDestination
        '
        Me.tbDestination.Location = New System.Drawing.Point(131, 68)
        Me.tbDestination.Name = "tbDestination"
        Me.tbDestination.Size = New System.Drawing.Size(231, 20)
        Me.tbDestination.TabIndex = 2
        '
        'lblMode
        '
        Me.lblMode.AutoSize = true
        Me.lblMode.Location = New System.Drawing.Point(10, 46)
        Me.lblMode.Name = "lblMode"
        Me.lblMode.Size = New System.Drawing.Size(68, 13)
        Me.lblMode.TabIndex = 14
        Me.lblMode.Text = "Vecto Mode:"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(305, 150)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'BtTCfileBrowse
        '
        Me.BtTCfileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtTCfileBrowse.Location = New System.Drawing.Point(362, 66)
        Me.BtTCfileBrowse.Name = "BtTCfileBrowse"
        Me.BtTCfileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtTCfileBrowse.TabIndex = 3
        Me.BtTCfileBrowse.UseVisualStyleBackColor = true
        '
        'XMLExportJobDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(392, 185)
        Me.Controls.Add(Me.BtTCfileBrowse)
        Me.Controls.Add(Me.lbVendor)
        Me.Controls.Add(Me.tbVendor)
        Me.Controls.Add(Me.btnExport)
        Me.Controls.Add(Me.cbSingleFile)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.tbJobfile)
        Me.Controls.Add(Me.tbMode)
        Me.Controls.Add(Me.lblJobfile)
        Me.Controls.Add(Me.lblDstDir)
        Me.Controls.Add(Me.tbDestination)
        Me.Controls.Add(Me.lblMode)
        Me.Controls.Add(Me.btnCancel)
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.Name = "XMLExportJobDialog"
        Me.Text = "VECTO XML Export"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Private WithEvents lbVendor As System.Windows.Forms.Label
	Private WithEvents tbVendor As System.Windows.Forms.TextBox
	Private WithEvents btnExport As System.Windows.Forms.Button
	Private WithEvents cbSingleFile As System.Windows.Forms.CheckBox
	Private WithEvents label1 As System.Windows.Forms.Label
	Private WithEvents tbJobfile As System.Windows.Forms.TextBox
	Private WithEvents tbMode As System.Windows.Forms.TextBox
	Private WithEvents lblJobfile As System.Windows.Forms.Label
	Private WithEvents lblDstDir As System.Windows.Forms.Label
	Private WithEvents tbDestination As System.Windows.Forms.TextBox
	Private WithEvents lblMode As System.Windows.Forms.Label
	Private WithEvents btnCancel As System.Windows.Forms.Button
	Friend WithEvents BtTCfileBrowse As System.Windows.Forms.Button
End Class
