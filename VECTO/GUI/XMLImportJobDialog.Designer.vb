<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XMLImportJobDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(XMLImportJobDialog))
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnBrowseOutput = New System.Windows.Forms.Button()
        Me.btnBrowseJob = New System.Windows.Forms.Button()
        Me.tbDestination = New System.Windows.Forms.TextBox()
        Me.tbJobFile = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(391, 92)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "Cancel"
        Me.btnClose.UseVisualStyleBackColor = true
        '
        'btnImport
        '
        Me.btnImport.Location = New System.Drawing.Point(310, 92)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(75, 23)
        Me.btnImport.TabIndex = 4
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = true
        '
        'btnBrowseOutput
        '
        Me.btnBrowseOutput.Location = New System.Drawing.Point(345, 41)
        Me.btnBrowseOutput.Name = "btnBrowseOutput"
        Me.btnBrowseOutput.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseOutput.TabIndex = 3
        Me.btnBrowseOutput.Text = "Browse"
        Me.btnBrowseOutput.UseVisualStyleBackColor = true
        '
        'btnBrowseJob
        '
        Me.btnBrowseJob.Location = New System.Drawing.Point(345, 12)
        Me.btnBrowseJob.Name = "btnBrowseJob"
        Me.btnBrowseJob.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseJob.TabIndex = 1
        Me.btnBrowseJob.Text = "Browse"
        Me.btnBrowseJob.UseVisualStyleBackColor = true
        '
        'tbDestination
        '
        Me.tbDestination.Location = New System.Drawing.Point(105, 43)
        Me.tbDestination.Name = "tbDestination"
        Me.tbDestination.Size = New System.Drawing.Size(224, 20)
        Me.tbDestination.TabIndex = 2
        '
        'tbJobFile
        '
        Me.tbJobFile.Location = New System.Drawing.Point(105, 14)
        Me.tbJobFile.Name = "tbJobFile"
        Me.tbJobFile.Size = New System.Drawing.Size(224, 20)
        Me.tbJobFile.TabIndex = 0
        '
        'label2
        '
        Me.label2.AutoSize = true
        Me.label2.Location = New System.Drawing.Point(11, 46)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(87, 13)
        Me.label2.TabIndex = 9
        Me.label2.Text = "Output Directory:"
        '
        'label1
        '
        Me.label1.AutoSize = true
        Me.label1.Location = New System.Drawing.Point(11, 17)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(46, 13)
        Me.label1.TabIndex = 8
        Me.label1.Text = "Job File:"
        '
        'XMLImportJobDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(480, 130)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.btnBrowseOutput)
        Me.Controls.Add(Me.btnBrowseJob)
        Me.Controls.Add(Me.tbDestination)
        Me.Controls.Add(Me.tbJobFile)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.Name = "XMLImportJobDialog"
        Me.Text = "VECTO Import Job"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Private WithEvents btnClose As System.Windows.Forms.Button
	Private WithEvents btnImport As System.Windows.Forms.Button
	Private WithEvents btnBrowseOutput As System.Windows.Forms.Button
	Private WithEvents btnBrowseJob As System.Windows.Forms.Button
	Private WithEvents tbDestination As System.Windows.Forms.TextBox
	Private WithEvents tbJobFile As System.Windows.Forms.TextBox
	Private WithEvents label2 As System.Windows.Forms.Label
	Private WithEvents label1 As System.Windows.Forms.Label
End Class
