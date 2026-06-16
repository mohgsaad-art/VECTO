<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ErrorForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me._codeLbl = New System.Windows.Forms.Label()
        Me._cycleIdLbl = New System.Windows.Forms.Label()
        Me._clipboardBtn = New System.Windows.Forms.Button()
        Me._okBtn = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me._messageLbl = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me._fixLbl = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me._gearLbl = New System.Windows.Forms.Label()
        Me._fEquivLbl = New System.Windows.Forms.Label()
        Me._speedLbl = New System.Windows.Forms.Label()
        Me._distanceLbl = New System.Windows.Forms.Label()
        Me._dtLbl = New System.Windows.Forms.Label()
        Me._progressLbl = New System.Windows.Forms.Label()
        Me._absTimeLbl = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me._stackLbl = New System.Windows.Forms.TextBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 162)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 25)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Code:"
        '
        '_codeLbl
        '
        Me._codeLbl.AutoSize = True
        Me._codeLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._codeLbl.Location = New System.Drawing.Point(76, 162)
        Me._codeLbl.Name = "_codeLbl"
        Me._codeLbl.Size = New System.Drawing.Size(92, 25)
        Me._codeLbl.TabIndex = 1
        Me._codeLbl.Text = "_codeLbl"
        '
        '_cycleIdLbl
        '
        Me._cycleIdLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._cycleIdLbl.Location = New System.Drawing.Point(7, 9)
        Me._cycleIdLbl.Name = "_cycleIdLbl"
        Me._cycleIdLbl.Size = New System.Drawing.Size(915, 25)
        Me._cycleIdLbl.TabIndex = 4
        Me._cycleIdLbl.Text = "_cycleIdLbl"
        Me._cycleIdLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        '_clipboardBtn
        '
        Me._clipboardBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._clipboardBtn.Location = New System.Drawing.Point(145, 587)
        Me._clipboardBtn.Name = "_clipboardBtn"
        Me._clipboardBtn.Size = New System.Drawing.Size(280, 51)
        Me._clipboardBtn.TabIndex = 5
        Me._clipboardBtn.Text = "Copy To Clipboard"
        Me._clipboardBtn.UseVisualStyleBackColor = True
        '
        '_okBtn
        '
        Me._okBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._okBtn.Location = New System.Drawing.Point(505, 587)
        Me._okBtn.Name = "_okBtn"
        Me._okBtn.Size = New System.Drawing.Size(280, 51)
        Me._okBtn.TabIndex = 6
        Me._okBtn.Text = "OK"
        Me._okBtn.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox1.Controls.Add(Me._messageLbl)
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.ForeColor = System.Drawing.Color.Red
        Me.GroupBox1.Location = New System.Drawing.Point(12, 40)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(915, 119)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Error"
        '
        '_messageLbl
        '
        Me._messageLbl.BackColor = System.Drawing.SystemColors.ControlLight
        Me._messageLbl.Location = New System.Drawing.Point(6, 23)
        Me._messageLbl.Multiline = True
        Me._messageLbl.Name = "_messageLbl"
        Me._messageLbl.ReadOnly = True
        Me._messageLbl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me._messageLbl.Size = New System.Drawing.Size(902, 91)
        Me._messageLbl.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox2.Controls.Add(Me._fixLbl)
        Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.GroupBox2.Location = New System.Drawing.Point(12, 190)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(915, 119)
        Me.GroupBox2.TabIndex = 8
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Fix"
        '
        '_fixLbl
        '
        Me._fixLbl.BackColor = System.Drawing.SystemColors.ControlLight
        Me._fixLbl.Location = New System.Drawing.Point(9, 23)
        Me._fixLbl.Multiline = True
        Me._fixLbl.Name = "_fixLbl"
        Me._fixLbl.ReadOnly = True
        Me._fixLbl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me._fixLbl.Size = New System.Drawing.Size(902, 91)
        Me._fixLbl.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me._gearLbl)
        Me.Panel1.Controls.Add(Me._fEquivLbl)
        Me.Panel1.Controls.Add(Me._speedLbl)
        Me.Panel1.Controls.Add(Me._distanceLbl)
        Me.Panel1.Controls.Add(Me._dtLbl)
        Me.Panel1.Controls.Add(Me._progressLbl)
        Me.Panel1.Controls.Add(Me._absTimeLbl)
        Me.Panel1.Location = New System.Drawing.Point(12, 313)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(915, 86)
        Me.Panel1.TabIndex = 9
        '
        '_gearLbl
        '
        Me._gearLbl.AutoSize = True
        Me._gearLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._gearLbl.Location = New System.Drawing.Point(491, 44)
        Me._gearLbl.Name = "_gearLbl"
        Me._gearLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._gearLbl.Size = New System.Drawing.Size(92, 29)
        Me._gearLbl.TabIndex = 6
        Me._gearLbl.Text = "_gearLbl"
        '
        '_fEquivLbl
        '
        Me._fEquivLbl.AutoSize = True
        Me._fEquivLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._fEquivLbl.Location = New System.Drawing.Point(238, 44)
        Me._fEquivLbl.Name = "_fEquivLbl"
        Me._fEquivLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._fEquivLbl.Size = New System.Drawing.Size(107, 29)
        Me._fEquivLbl.TabIndex = 5
        Me._fEquivLbl.Text = "_fEquivLbl"
        '
        '_speedLbl
        '
        Me._speedLbl.AutoSize = True
        Me._speedLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._speedLbl.Location = New System.Drawing.Point(4, 44)
        Me._speedLbl.Name = "_speedLbl"
        Me._speedLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._speedLbl.Size = New System.Drawing.Size(107, 29)
        Me._speedLbl.TabIndex = 4
        Me._speedLbl.Text = "_speedLbl"
        '
        '_distanceLbl
        '
        Me._distanceLbl.AutoSize = True
        Me._distanceLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._distanceLbl.Location = New System.Drawing.Point(491, 4)
        Me._distanceLbl.Name = "_distanceLbl"
        Me._distanceLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._distanceLbl.Size = New System.Drawing.Size(126, 29)
        Me._distanceLbl.TabIndex = 3
        Me._distanceLbl.Text = "_distanceLbl"
        '
        '_dtLbl
        '
        Me._dtLbl.AutoSize = True
        Me._dtLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._dtLbl.Location = New System.Drawing.Point(772, 4)
        Me._dtLbl.Name = "_dtLbl"
        Me._dtLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._dtLbl.Size = New System.Drawing.Size(69, 29)
        Me._dtLbl.TabIndex = 2
        Me._dtLbl.Text = "_dtLbl"
        Me._dtLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        '_progressLbl
        '
        Me._progressLbl.AutoSize = True
        Me._progressLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._progressLbl.Location = New System.Drawing.Point(4, 4)
        Me._progressLbl.Name = "_progressLbl"
        Me._progressLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._progressLbl.Size = New System.Drawing.Size(129, 29)
        Me._progressLbl.TabIndex = 1
        Me._progressLbl.Text = "_progressLbl"
        '
        '_absTimeLbl
        '
        Me._absTimeLbl.AutoSize = True
        Me._absTimeLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me._absTimeLbl.Location = New System.Drawing.Point(233, 4)
        Me._absTimeLbl.Name = "_absTimeLbl"
        Me._absTimeLbl.Padding = New System.Windows.Forms.Padding(2)
        Me._absTimeLbl.Size = New System.Drawing.Size(129, 29)
        Me._absTimeLbl.TabIndex = 0
        Me._absTimeLbl.Text = "_absTimeLbl"
        '
        'GroupBox3
        '
        Me.GroupBox3.BackColor = System.Drawing.SystemColors.Control
        Me.GroupBox3.Controls.Add(Me._stackLbl)
        Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox3.Location = New System.Drawing.Point(12, 405)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(914, 176)
        Me.GroupBox3.TabIndex = 10
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "StackTrace"
        '
        '_stackLbl
        '
        Me._stackLbl.BackColor = System.Drawing.SystemColors.ControlLight
        Me._stackLbl.Location = New System.Drawing.Point(9, 23)
        Me._stackLbl.Multiline = True
        Me._stackLbl.Name = "_stackLbl"
        Me._stackLbl.ReadOnly = True
        Me._stackLbl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me._stackLbl.Size = New System.Drawing.Size(899, 146)
        Me._stackLbl.TabIndex = 0
        '
        'ErrorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(938, 650)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me._okBtn)
        Me.Controls.Add(Me._clipboardBtn)
        Me.Controls.Add(Me._cycleIdLbl)
        Me.Controls.Add(Me._codeLbl)
        Me.Controls.Add(Me.Label1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ErrorForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Error Details"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents _codeLbl As Label
    Friend WithEvents _cycleIdLbl As Label
    Friend WithEvents _clipboardBtn As Button
    Friend WithEvents _okBtn As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents _absTimeLbl As Label
    Friend WithEvents _progressLbl As Label
    Friend WithEvents _dtLbl As Label
    Friend WithEvents _distanceLbl As Label
    Friend WithEvents _speedLbl As Label
    Friend WithEvents _fEquivLbl As Label
    Friend WithEvents _gearLbl As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents _stackLbl As TextBox
    Friend WithEvents _messageLbl As TextBox
    Friend WithEvents _fixLbl As TextBox
End Class
