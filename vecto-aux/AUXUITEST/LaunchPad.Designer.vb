<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LaunchPad
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
        Me.btnHVAC = New System.Windows.Forms.Button()
        Me.btnCompressor = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnHVAC
        '
        Me.btnHVAC.Location = New System.Drawing.Point(31, 26)
        Me.btnHVAC.Name = "btnHVAC"
        Me.btnHVAC.Size = New System.Drawing.Size(104, 46)
        Me.btnHVAC.TabIndex = 0
        Me.btnHVAC.Text = "HVAC"
        Me.btnHVAC.UseVisualStyleBackColor = True
        '
        'btnCompressor
        '
        Me.btnCompressor.Location = New System.Drawing.Point(164, 26)
        Me.btnCompressor.Name = "btnCompressor"
        Me.btnCompressor.Size = New System.Drawing.Size(106, 46)
        Me.btnCompressor.TabIndex = 1
        Me.btnCompressor.Text = "Compressor"
        Me.btnCompressor.UseVisualStyleBackColor = True
        '
        'LaunchPad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(426, 335)
        Me.Controls.Add(Me.btnCompressor)
        Me.Controls.Add(Me.btnHVAC)
        Me.Name = "LaunchPad"
        Me.Text = "LaunchPad"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnHVAC As System.Windows.Forms.Button
    Friend WithEvents btnCompressor As System.Windows.Forms.Button
End Class
