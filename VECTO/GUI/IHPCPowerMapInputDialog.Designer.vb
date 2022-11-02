<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IHPCPowerMapInputDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IHPCPowerMapInputDialog))
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.tbGear = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.btAddFilePath = New System.Windows.Forms.Button()
        Me.tbInputFile = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btAddInput = New System.Windows.Forms.Button()
        Me.btCancel = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout
        Me.TableLayoutPanel4.SuspendLayout
        Me.TableLayoutPanel5.SuspendLayout
        Me.TableLayoutPanel1.SuspendLayout
        Me.SuspendLayout
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.TableLayoutPanel4)
        Me.FlowLayoutPanel1.Controls.Add(Me.TableLayoutPanel5)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(762, 69)
        Me.FlowLayoutPanel1.TabIndex = 11
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TableLayoutPanel4.ColumnCount = 2
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.8!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.2!))
        Me.TableLayoutPanel4.Controls.Add(Me.tbGear, 1, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.Label3, 0, 0)
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(9, 13)
        Me.TableLayoutPanel4.Margin = New System.Windows.Forms.Padding(9, 5, 4, 5)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(192, 43)
        Me.TableLayoutPanel4.TabIndex = 10
        '
        'tbGear
        '
        Me.tbGear.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbGear.Location = New System.Drawing.Point(70, 8)
        Me.tbGear.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbGear.Name = "tbGear"
        Me.tbGear.ReadOnly = true
        Me.tbGear.Size = New System.Drawing.Size(114, 26)
        Me.tbGear.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(8, 11)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 20)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Gear"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 474!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60!))
        Me.TableLayoutPanel5.Controls.Add(Me.btAddFilePath, 1, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.tbInputFile, 0, 0)
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(209, 5)
        Me.TableLayoutPanel5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(534, 60)
        Me.TableLayoutPanel5.TabIndex = 11
        '
        'btAddFilePath
        '
        Me.btAddFilePath.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btAddFilePath.Image = CType(resources.GetObject("btAddFilePath.Image"),System.Drawing.Image)
        Me.btAddFilePath.Location = New System.Drawing.Point(486, 10)
        Me.btAddFilePath.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btAddFilePath.Name = "btAddFilePath"
        Me.btAddFilePath.Size = New System.Drawing.Size(36, 40)
        Me.btAddFilePath.TabIndex = 13
        Me.btAddFilePath.UseVisualStyleBackColor = true
        '
        'tbInputFile
        '
        Me.tbInputFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbInputFile.Location = New System.Drawing.Point(4, 17)
        Me.tbInputFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbInputFile.Name = "tbInputFile"
        Me.tbInputFile.Size = New System.Drawing.Size(466, 26)
        Me.tbInputFile.TabIndex = 12
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Controls.Add(Me.btAddInput, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btCancel, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(522, 78)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(222, 45)
        Me.TableLayoutPanel1.TabIndex = 12
        '
        'btAddInput
        '
        Me.btAddInput.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btAddInput.Location = New System.Drawing.Point(5, 5)
        Me.btAddInput.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btAddInput.Name = "btAddInput"
        Me.btAddInput.Size = New System.Drawing.Size(100, 35)
        Me.btAddInput.TabIndex = 14
        Me.btAddInput.Text = "OK"
        '
        'btCancel
        '
        Me.btCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btCancel.Location = New System.Drawing.Point(116, 5)
        Me.btCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btCancel.Name = "btCancel"
        Me.btCancel.Size = New System.Drawing.Size(100, 35)
        Me.btCancel.TabIndex = 15
        Me.btCancel.Text = "Cancel"
        '
        'IHPCPowerMapInputDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9!, 20!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(762, 131)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "IHPCPowerMapInputDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Power Map"
        Me.FlowLayoutPanel1.ResumeLayout(false)
        Me.TableLayoutPanel4.ResumeLayout(false)
        Me.TableLayoutPanel4.PerformLayout
        Me.TableLayoutPanel5.ResumeLayout(false)
        Me.TableLayoutPanel5.PerformLayout
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents TableLayoutPanel4 As TableLayoutPanel
    Friend WithEvents tbGear As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TableLayoutPanel5 As TableLayoutPanel
    Friend WithEvents btAddFilePath As Button
    Friend WithEvents tbInputFile As TextBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btAddInput As Button
    Friend WithEvents btCancel As Button
End Class
