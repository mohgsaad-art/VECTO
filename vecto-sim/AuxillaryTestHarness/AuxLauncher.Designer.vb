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
        Me.components = New System.ComponentModel.Container()
        Me.txtAdvancedAuxiliaries = New System.Windows.Forms.TextBox()
        Me.btnLaunchAux = New System.Windows.Forms.Button()
        Me.btnRun = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInformation = New System.Windows.Forms.Button()
        Me.txtTotalFCGrams = New System.Windows.Forms.TextBox()
        Me.txtTotalFCLitres = New System.Windows.Forms.TextBox()
        Me.lblTotalFCGRAMS = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblAuxiliaryName = New System.Windows.Forms.Label()
        Me.lblAuxiliaryVersion = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.btnWholeCycle = New System.Windows.Forms.Button()
        Me.txtEvents = New System.Windows.Forms.TextBox()
        Me.cboWarningLevel = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout
        '
        'txtAdvancedAuxiliaries
        '
        Me.txtAdvancedAuxiliaries.Location = New System.Drawing.Point(30, 101)
        Me.txtAdvancedAuxiliaries.Name = "txtAdvancedAuxiliaries"
        Me.txtAdvancedAuxiliaries.Size = New System.Drawing.Size(366, 20)
        Me.txtAdvancedAuxiliaries.TabIndex = 0
        Me.txtAdvancedAuxiliaries.Text = "TESTHARNESCONFIG.AAUX"
        '
        'btnLaunchAux
        '
        Me.btnLaunchAux.Location = New System.Drawing.Point(414, 101)
        Me.btnLaunchAux.Name = "btnLaunchAux"
        Me.btnLaunchAux.Size = New System.Drawing.Size(75, 23)
        Me.btnLaunchAux.TabIndex = 1
        Me.btnLaunchAux.Text = "Launch Aux"
        Me.btnLaunchAux.UseVisualStyleBackColor = true
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(414, 152)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(75, 23)
        Me.btnRun.TabIndex = 2
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = true
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(414, 212)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = true
        '
        'btnInformation
        '
        Me.btnInformation.Location = New System.Drawing.Point(414, 269)
        Me.btnInformation.Name = "btnInformation"
        Me.btnInformation.Size = New System.Drawing.Size(75, 23)
        Me.btnInformation.TabIndex = 4
        Me.btnInformation.Text = "Info"
        Me.btnInformation.UseVisualStyleBackColor = true
        '
        'txtTotalFCGrams
        '
        Me.txtTotalFCGrams.Location = New System.Drawing.Point(30, 152)
        Me.txtTotalFCGrams.Name = "txtTotalFCGrams"
        Me.txtTotalFCGrams.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalFCGrams.TabIndex = 5
        '
        'txtTotalFCLitres
        '
        Me.txtTotalFCLitres.Location = New System.Drawing.Point(164, 151)
        Me.txtTotalFCLitres.Name = "txtTotalFCLitres"
        Me.txtTotalFCLitres.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalFCLitres.TabIndex = 6
        '
        'lblTotalFCGRAMS
        '
        Me.lblTotalFCGRAMS.AutoSize = true
        Me.lblTotalFCGRAMS.Location = New System.Drawing.Point(30, 133)
        Me.lblTotalFCGRAMS.Name = "lblTotalFCGRAMS"
        Me.lblTotalFCGRAMS.Size = New System.Drawing.Size(77, 13)
        Me.lblTotalFCGRAMS.TabIndex = 7
        Me.lblTotalFCGRAMS.Text = "TotalFC Grams"
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(161, 133)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "TotalFC Litres"
        '
        'lblAuxiliaryName
        '
        Me.lblAuxiliaryName.AutoSize = true
        Me.lblAuxiliaryName.Location = New System.Drawing.Point(30, 9)
        Me.lblAuxiliaryName.Name = "lblAuxiliaryName"
        Me.lblAuxiliaryName.Size = New System.Drawing.Size(39, 13)
        Me.lblAuxiliaryName.TabIndex = 9
        Me.lblAuxiliaryName.Text = "Label2"
        '
        'lblAuxiliaryVersion
        '
        Me.lblAuxiliaryVersion.AutoSize = true
        Me.lblAuxiliaryVersion.Location = New System.Drawing.Point(30, 37)
        Me.lblAuxiliaryVersion.Name = "lblAuxiliaryVersion"
        Me.lblAuxiliaryVersion.Size = New System.Drawing.Size(39, 13)
        Me.lblAuxiliaryVersion.TabIndex = 10
        Me.lblAuxiliaryVersion.Text = "Label3"
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'btnWholeCycle
        '
        Me.btnWholeCycle.Location = New System.Drawing.Point(305, 151)
        Me.btnWholeCycle.Name = "btnWholeCycle"
        Me.btnWholeCycle.Size = New System.Drawing.Size(75, 23)
        Me.btnWholeCycle.TabIndex = 11
        Me.btnWholeCycle.Text = "Whole Cycle"
        Me.btnWholeCycle.UseVisualStyleBackColor = true
        '
        'txtEvents
        '
        Me.txtEvents.Location = New System.Drawing.Point(33, 214)
        Me.txtEvents.Multiline = true
        Me.txtEvents.Name = "txtEvents"
        Me.txtEvents.Size = New System.Drawing.Size(306, 99)
        Me.txtEvents.TabIndex = 12
        '
        'cboWarningLevel
        '
        Me.cboWarningLevel.FormattingEnabled = true
        Me.cboWarningLevel.Location = New System.Drawing.Point(30, 65)
        Me.cboWarningLevel.Name = "cboWarningLevel"
        Me.cboWarningLevel.Size = New System.Drawing.Size(121, 21)
        Me.cboWarningLevel.TabIndex = 13
        '
        'AuxLauncher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(527, 363)
        Me.Controls.Add(Me.cboWarningLevel)
        Me.Controls.Add(Me.txtEvents)
        Me.Controls.Add(Me.btnWholeCycle)
        Me.Controls.Add(Me.lblAuxiliaryVersion)
        Me.Controls.Add(Me.lblAuxiliaryName)
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
    Friend WithEvents lblAuxiliaryName As System.Windows.Forms.Label
    Friend WithEvents lblAuxiliaryVersion As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnWholeCycle As System.Windows.Forms.Button
    Friend WithEvents txtEvents As System.Windows.Forms.TextBox
    Friend WithEvents cboWarningLevel As System.Windows.Forms.ComboBox
End Class
