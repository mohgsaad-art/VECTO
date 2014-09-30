Namespace UI
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class F_HVAC
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
            Me.dgMapResults = New System.Windows.Forms.DataGridView()
            Me.btnSave = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnBrowseMap = New System.Windows.Forms.Button()
            Me.txtMapFile = New System.Windows.Forms.TextBox()
            Me.lblMapFile = New System.Windows.Forms.Label()
            Me.pnlSearchBar = New System.Windows.Forms.Panel()
            Me.lblMechanicalDemand = New System.Windows.Forms.Label()
            Me.txtMechanicalDemand = New System.Windows.Forms.TextBox()
            Me.lblElectricalDemand = New System.Windows.Forms.Label()
            Me.txtElectricalDemand = New System.Windows.Forms.TextBox()
            CType(Me.dgMapResults, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgMapResults
            '
            Me.dgMapResults.AllowUserToAddRows = False
            Me.dgMapResults.AllowUserToDeleteRows = False
            Me.dgMapResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgMapResults.Location = New System.Drawing.Point(14, 164)
            Me.dgMapResults.MultiSelect = False
            Me.dgMapResults.Name = "dgMapResults"
            Me.dgMapResults.ReadOnly = True
            Me.dgMapResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgMapResults.Size = New System.Drawing.Size(650, 245)
            Me.dgMapResults.TabIndex = 0
            '
            'btnSave
            '
            Me.btnSave.Location = New System.Drawing.Point(497, 426)
            Me.btnSave.Name = "btnSave"
            Me.btnSave.Size = New System.Drawing.Size(75, 23)
            Me.btnSave.TabIndex = 1
            Me.btnSave.Text = "Save"
            Me.btnSave.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(584, 426)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(75, 23)
            Me.btnCancel.TabIndex = 2
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnBrowseMap
            '
            Me.btnBrowseMap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.btnBrowseMap.Location = New System.Drawing.Point(584, 12)
            Me.btnBrowseMap.Name = "btnBrowseMap"
            Me.btnBrowseMap.Size = New System.Drawing.Size(75, 23)
            Me.btnBrowseMap.TabIndex = 3
            Me.btnBrowseMap.Text = "Browse"
            Me.btnBrowseMap.UseVisualStyleBackColor = True
            '
            'txtMapFile
            '
            Me.txtMapFile.Location = New System.Drawing.Point(48, 12)
            Me.txtMapFile.Name = "txtMapFile"
            Me.txtMapFile.ReadOnly = True
            Me.txtMapFile.Size = New System.Drawing.Size(530, 20)
            Me.txtMapFile.TabIndex = 4
            '
            'lblMapFile
            '
            Me.lblMapFile.AutoSize = True
            Me.lblMapFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblMapFile.Location = New System.Drawing.Point(9, 15)
            Me.lblMapFile.Name = "lblMapFile"
            Me.lblMapFile.Size = New System.Drawing.Size(34, 13)
            Me.lblMapFile.TabIndex = 5
            Me.lblMapFile.Text = "Map :"
            '
            'pnlSearchBar
            '
            Me.pnlSearchBar.Location = New System.Drawing.Point(16, 95)
            Me.pnlSearchBar.Name = "pnlSearchBar"
            Me.pnlSearchBar.Size = New System.Drawing.Size(643, 59)
            Me.pnlSearchBar.TabIndex = 6
            '
            'lblMechanicalDemand
            '
            Me.lblMechanicalDemand.AutoSize = True
            Me.lblMechanicalDemand.Location = New System.Drawing.Point(291, 58)
            Me.lblMechanicalDemand.Name = "lblMechanicalDemand"
            Me.lblMechanicalDemand.Size = New System.Drawing.Size(132, 13)
            Me.lblMechanicalDemand.TabIndex = 9
            Me.lblMechanicalDemand.Text = "Mechanical Demand (KW)"
            '
            'txtMechanicalDemand
            '
            Me.txtMechanicalDemand.Location = New System.Drawing.Point(431, 54)
            Me.txtMechanicalDemand.Name = "txtMechanicalDemand"
            Me.txtMechanicalDemand.ReadOnly = True
            Me.txtMechanicalDemand.Size = New System.Drawing.Size(56, 20)
            Me.txtMechanicalDemand.TabIndex = 10
            Me.txtMechanicalDemand.Text = "0"
            '
            'lblElectricalDemand
            '
            Me.lblElectricalDemand.AutoSize = True
            Me.lblElectricalDemand.Location = New System.Drawing.Point(493, 58)
            Me.lblElectricalDemand.Name = "lblElectricalDemand"
            Me.lblElectricalDemand.Size = New System.Drawing.Size(120, 13)
            Me.lblElectricalDemand.TabIndex = 11
            Me.lblElectricalDemand.Text = "Electrical Demand (KW)"
            '
            'txtElectricalDemand
            '
            Me.txtElectricalDemand.Location = New System.Drawing.Point(619, 54)
            Me.txtElectricalDemand.Name = "txtElectricalDemand"
            Me.txtElectricalDemand.ReadOnly = True
            Me.txtElectricalDemand.Size = New System.Drawing.Size(26, 20)
            Me.txtElectricalDemand.TabIndex = 12
            Me.txtElectricalDemand.Text = "0"
            '
            'F_HVAC
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(674, 458)
            Me.Controls.Add(Me.txtElectricalDemand)
            Me.Controls.Add(Me.lblElectricalDemand)
            Me.Controls.Add(Me.txtMechanicalDemand)
            Me.Controls.Add(Me.lblMechanicalDemand)
            Me.Controls.Add(Me.pnlSearchBar)
            Me.Controls.Add(Me.lblMapFile)
            Me.Controls.Add(Me.txtMapFile)
            Me.Controls.Add(Me.btnBrowseMap)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSave)
            Me.Controls.Add(Me.dgMapResults)
            Me.Name = "F_HVAC"
            Me.Text = "HVAC Properties"
            CType(Me.dgMapResults, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents dgMapResults As System.Windows.Forms.DataGridView
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnBrowseMap As System.Windows.Forms.Button
        Friend WithEvents txtMapFile As System.Windows.Forms.TextBox
        Friend WithEvents lblMapFile As System.Windows.Forms.Label
        Friend WithEvents pnlSearchBar As System.Windows.Forms.Panel
        Friend WithEvents lblMechanicalDemand As System.Windows.Forms.Label
        Friend WithEvents txtMechanicalDemand As System.Windows.Forms.TextBox
        Friend WithEvents lblElectricalDemand As System.Windows.Forms.Label
        Friend WithEvents txtElectricalDemand As System.Windows.Forms.TextBox

    End Class

End Namespace