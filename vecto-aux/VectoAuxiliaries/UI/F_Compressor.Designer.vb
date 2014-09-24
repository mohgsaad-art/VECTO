
Namespace UI
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class F_Compressor
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
            Me.lblPullyEfficiency = New System.Windows.Forms.Label()
            Me.txtPullyEfficiency = New System.Windows.Forms.TextBox()
            Me.lblMapFile = New System.Windows.Forms.Label()
            Me.txtMapFile = New System.Windows.Forms.TextBox()
            Me.btnBrowseMap = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.lblMapLabel = New System.Windows.Forms.Label()
            Me.txtMapPath = New System.Windows.Forms.TextBox()
            Me.btnBrowsePowerMap = New System.Windows.Forms.Button()
            Me.lblPulleyGearRatio = New System.Windows.Forms.Label()
            Me.txtPulleyEfficiency = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'lblPullyEfficiency
            '
            Me.lblPullyEfficiency.AutoSize = True
            Me.lblPullyEfficiency.Location = New System.Drawing.Point(6, 63)
            Me.lblPullyEfficiency.Name = "lblPullyEfficiency"
            Me.lblPullyEfficiency.Size = New System.Drawing.Size(87, 13)
            Me.lblPullyEfficiency.TabIndex = 21
            Me.lblPullyEfficiency.Text = " Pully Efficiency :"
            '
            'txtPullyEfficiency
            '
            Me.txtPullyEfficiency.Location = New System.Drawing.Point(105, 60)
            Me.txtPullyEfficiency.Name = "txtPullyEfficiency"
            Me.txtPullyEfficiency.Size = New System.Drawing.Size(46, 20)
            Me.txtPullyEfficiency.TabIndex = 20
            '
            'lblMapFile
            '
            Me.lblMapFile.AutoSize = True
            Me.lblMapFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblMapFile.Location = New System.Drawing.Point(-52, -32)
            Me.lblMapFile.Name = "lblMapFile"
            Me.lblMapFile.Size = New System.Drawing.Size(34, 13)
            Me.lblMapFile.TabIndex = 18
            Me.lblMapFile.Text = "Map :"
            '
            'txtMapFile
            '
            Me.txtMapFile.Location = New System.Drawing.Point(-13, -35)
            Me.txtMapFile.Name = "txtMapFile"
            Me.txtMapFile.ReadOnly = True
            Me.txtMapFile.Size = New System.Drawing.Size(530, 20)
            Me.txtMapFile.TabIndex = 17
            '
            'btnBrowseMap
            '
            Me.btnBrowseMap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnBrowseMap.Location = New System.Drawing.Point(523, -35)
            Me.btnBrowseMap.Name = "btnBrowseMap"
            Me.btnBrowseMap.Size = New System.Drawing.Size(75, 23)
            Me.btnBrowseMap.TabIndex = 16
            Me.btnBrowseMap.Text = "Browse"
            Me.btnBrowseMap.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(518, 113)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 15
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(431, 113)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 14
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'lblMapLabel
            '
            Me.lblMapLabel.AutoSize = True
            Me.lblMapLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblMapLabel.Location = New System.Drawing.Point(59, 25)
            Me.lblMapLabel.Name = "lblMapLabel"
            Me.lblMapLabel.Size = New System.Drawing.Size(34, 13)
            Me.lblMapLabel.TabIndex = 28
            Me.lblMapLabel.Text = "Map :"
            '
            'txtMapPath
            '
            Me.txtMapPath.Location = New System.Drawing.Point(105, 22)
            Me.txtMapPath.Name = "txtMapPath"
            Me.txtMapPath.ReadOnly = True
            Me.txtMapPath.Size = New System.Drawing.Size(397, 20)
            Me.txtMapPath.TabIndex = 27
            '
            'btnBrowsePowerMap
            '
            Me.btnBrowsePowerMap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnBrowsePowerMap.Location = New System.Drawing.Point(512, 22)
            Me.btnBrowsePowerMap.Name = "btnBrowsePowerMap"
            Me.btnBrowsePowerMap.Size = New System.Drawing.Size(75, 23)
            Me.btnBrowsePowerMap.TabIndex = 26
            Me.btnBrowsePowerMap.Text = "Browse"
            Me.btnBrowsePowerMap.UseVisualStyleBackColor = True
            '
            'lblPulleyGearRatio
            '
            Me.lblPulleyGearRatio.AutoSize = True
            Me.lblPulleyGearRatio.Location = New System.Drawing.Point(163, 64)
            Me.lblPulleyGearRatio.Name = "lblPulleyGearRatio"
            Me.lblPulleyGearRatio.Size = New System.Drawing.Size(81, 13)
            Me.lblPulleyGearRatio.TabIndex = 30
            Me.lblPulleyGearRatio.Text = " Pully Efficiency"
            '
            'txtPulleyEfficiency
            '
            Me.txtPulleyEfficiency.Location = New System.Drawing.Point(250, 61)
            Me.txtPulleyEfficiency.Name = "txtPulleyEfficiency"
            Me.txtPulleyEfficiency.Size = New System.Drawing.Size(41, 20)
            Me.txtPulleyEfficiency.TabIndex = 29
            '
            'F_Compressor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(602, 147)
            Me.Controls.Add(Me.lblPulleyGearRatio)
            Me.Controls.Add(Me.txtPulleyEfficiency)
            Me.Controls.Add(Me.lblMapLabel)
            Me.Controls.Add(Me.txtMapPath)
            Me.Controls.Add(Me.btnBrowsePowerMap)
            Me.Controls.Add(Me.lblPullyEfficiency)
            Me.Controls.Add(Me.txtPullyEfficiency)
            Me.Controls.Add(Me.lblMapFile)
            Me.Controls.Add(Me.txtMapFile)
            Me.Controls.Add(Me.btnBrowseMap)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSave)
            Me.Name = "F_Compressor"
            Me.Text = "Pneumatic Compressor"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents lblPullyEfficiency As System.Windows.Forms.Label
        Friend WithEvents txtPullyEfficiency As System.Windows.Forms.TextBox
        Friend WithEvents lblMapFile As System.Windows.Forms.Label
        Friend WithEvents txtMapFile As System.Windows.Forms.TextBox
        Friend WithEvents btnBrowseMap As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents lblMapLabel As System.Windows.Forms.Label
        Friend WithEvents txtMapPath As System.Windows.Forms.TextBox
        Friend WithEvents btnBrowsePowerMap As System.Windows.Forms.Button
        Friend WithEvents lblPulleyGearRatio As System.Windows.Forms.Label
        Friend WithEvents txtPulleyEfficiency As System.Windows.Forms.TextBox
    End Class
End Namespace

