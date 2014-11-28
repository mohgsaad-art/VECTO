<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AuxLauncher
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
        Me.txtAdvancedAuxiliaries = New System.Windows.Forms.TextBox()
        Me.btnLaunchAux = New System.Windows.Forms.Button()
        Me.btnRun = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInformation = New System.Windows.Forms.Button()
        Me.txtTotalFCGrams = New System.Windows.Forms.TextBox()
        Me.txtTotalFCLitres = New System.Windows.Forms.TextBox()
        Me.lblTotalFCGRAMS = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout
        '
        'txtAdvancedAuxiliaries
        '
        Me.txtAdvancedAuxiliaries.Location = New System.Drawing.Point(22, 30)
        Me.txtAdvancedAuxiliaries.Name = "txtAdvancedAuxiliaries"
        Me.txtAdvancedAuxiliaries.Size = New System.Drawing.Size(366, 20)
        Me.txtAdvancedAuxiliaries.TabIndex = 0
        Me.txtAdvancedAuxiliaries.Text = "TESTHARNESCONFIG.AAUX"
        '
        'btnLaunchAux
        '
        Me.btnLaunchAux.Location = New System.Drawing.Point(409, 30)
        Me.btnLaunchAux.Name = "btnLaunchAux"
        Me.btnLaunchAux.Size = New System.Drawing.Size(75, 23)
        Me.btnLaunchAux.TabIndex = 1
        Me.btnLaunchAux.Text = "Launch Aux"
        Me.btnLaunchAux.UseVisualStyleBackColor = true
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(409, 81)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(75, 23)
        Me.btnRun.TabIndex = 2
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = true
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(409, 141)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = true
        '
        'btnInformation
        '
        Me.btnInformation.Location = New System.Drawing.Point(409, 198)
        Me.btnInformation.Name = "btnInformation"
        Me.btnInformation.Size = New System.Drawing.Size(75, 23)
        Me.btnInformation.TabIndex = 4
        Me.btnInformation.Text = "Info"
        Me.btnInformation.UseVisualStyleBackColor = true
        '
        'txtTotalFCGrams
        '
        Me.txtTotalFCGrams.Location = New System.Drawing.Point(22, 81)
        Me.txtTotalFCGrams.Name = "txtTotalFCGrams"
        Me.txtTotalFCGrams.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalFCGrams.TabIndex = 5
        '
        'txtTotalFCLitres
        '
        Me.txtTotalFCLitres.Location = New System.Drawing.Point(159, 80)
        Me.txtTotalFCLitres.Name = "txtTotalFCLitres"
        Me.txtTotalFCLitres.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalFCLitres.TabIndex = 6
        '
        'lblTotalFCGRAMS
        '
        Me.lblTotalFCGRAMS.AutoSize = true
        Me.lblTotalFCGRAMS.Location = New System.Drawing.Point(22, 62)
        Me.lblTotalFCGRAMS.Name = "lblTotalFCGRAMS"
        Me.lblTotalFCGRAMS.Size = New System.Drawing.Size(77, 13)
        Me.lblTotalFCGRAMS.TabIndex = 7
        Me.lblTotalFCGRAMS.Text = "TotalFC Grams"
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(156, 62)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "TotalFC Grams"
        '
        'AuxLauncher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(527, 363)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTotalFCGRAMS)
        Me.Controls.Add(Me.txtTotalFCLitres)
        Me.Controls.Add(Me.txtTotalFCGrams)
        Me.Controls.Add(Me.btnInformation)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.btnLaunchAux)
        Me.Controls.Add(Me.txtAdvancedAuxiliaries)
        Me.Name = "AuxLauncher"
        Me.Text = "AuxLauncher"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents txtAdvancedAuxiliaries As System.Windows.Forms.TextBox
    Friend WithEvents btnLaunchAux As System.Windows.Forms.Button
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnInformation As System.Windows.Forms.Button
    Friend WithEvents txtTotalFCGrams As System.Windows.Forms.TextBox
    Friend WithEvents txtTotalFCLitres As System.Windows.Forms.TextBox
    Friend WithEvents lblTotalFCGRAMS As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
