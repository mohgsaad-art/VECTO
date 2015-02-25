<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.gvTechBenefits = New System.Windows.Forms.DataGridView()
        CType(Me.gvTechBenefits,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'gvTechBenefits
        '
        Me.gvTechBenefits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gvTechBenefits.Location = New System.Drawing.Point(203, 139)
        Me.gvTechBenefits.Name = "gvTechBenefits"
        Me.gvTechBenefits.Size = New System.Drawing.Size(355, 137)
        Me.gvTechBenefits.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(747, 447)
        Me.Controls.Add(Me.gvTechBenefits)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.gvTechBenefits,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents gvTechBenefits As System.Windows.Forms.DataGridView

End Class
