<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHVACTool
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
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabBusParameters = New System.Windows.Forms.TabPage()
        Me.tabTechListInput = New System.Windows.Forms.TabPage()
        Me.cboBuses = New System.Windows.Forms.ComboBox()
        Me.TabControl1.SuspendLayout
        Me.tabBusParameters.SuspendLayout
        Me.SuspendLayout
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tabBusParameters)
        Me.TabControl1.Controls.Add(Me.tabTechListInput)
        Me.TabControl1.Location = New System.Drawing.Point(53, 37)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(648, 415)
        Me.TabControl1.TabIndex = 0
        '
        'tabBusParameters
        '
        Me.tabBusParameters.Controls.Add(Me.cboBuses)
        Me.tabBusParameters.Location = New System.Drawing.Point(4, 22)
        Me.tabBusParameters.Name = "tabBusParameters"
        Me.tabBusParameters.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBusParameters.Size = New System.Drawing.Size(640, 389)
        Me.tabBusParameters.TabIndex = 0
        Me.tabBusParameters.Text = "INP BusParameters"
        Me.tabBusParameters.UseVisualStyleBackColor = true
        '
        'tabTechListInput
        '
        Me.tabTechListInput.Location = New System.Drawing.Point(4, 22)
        Me.tabTechListInput.Name = "tabTechListInput"
        Me.tabTechListInput.Padding = New System.Windows.Forms.Padding(3)
        Me.tabTechListInput.Size = New System.Drawing.Size(640, 389)
        Me.tabTechListInput.TabIndex = 1
        Me.tabTechListInput.Text = "Tech List Input"
        Me.tabTechListInput.UseVisualStyleBackColor = true
        '
        'cboBuses
        '
        Me.cboBuses.FormattingEnabled = true
        Me.cboBuses.Location = New System.Drawing.Point(34, 30)
        Me.cboBuses.Name = "cboBuses"
        Me.cboBuses.Size = New System.Drawing.Size(558, 21)
        Me.cboBuses.TabIndex = 0
        '
        'frmHVACTool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(773, 510)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "frmHVACTool"
        Me.Text = "frmHVACTool"
        Me.TabControl1.ResumeLayout(false)
        Me.tabBusParameters.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tabBusParameters As System.Windows.Forms.TabPage
    Friend WithEvents tabTechListInput As System.Windows.Forms.TabPage
    Friend WithEvents cboBuses As System.Windows.Forms.ComboBox
End Class
