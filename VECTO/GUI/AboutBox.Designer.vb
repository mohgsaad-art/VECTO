' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class AboutBox
	Inherits Form

	'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
	<DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Wird vom Windows Form-Designer benÃ¶tigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AboutBox))
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.LinkLabel2 = New System.Windows.Forms.LinkLabel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PictureBoxJRC = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PictureBoxJRC,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 241)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(162, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Developed on behalf of the"
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label8.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.Label8.Location = New System.Drawing.Point(32, 264)
        Me.Label8.Margin = New System.Windows.Forms.Padding(0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(198, 13)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "European Commission DG CLIMA"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_About
        Me.PictureBox1.Location = New System.Drawing.Point(23, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(447, 182)
        Me.PictureBox1.TabIndex = 14
        Me.PictureBox1.TabStop = false
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 331)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(105, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Support Contact:"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = true
        Me.LinkLabel1.Location = New System.Drawing.Point(123, 331)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(122, 13)
        Me.LinkLabel1.TabIndex = 17
        Me.LinkLabel1.TabStop = true
        Me.LinkLabel1.Text = "jrc-vecto@ec.europa.eu"
        '
        'LinkLabel2
        '
        Me.LinkLabel2.AutoSize = true
        Me.LinkLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LinkLabel2.Location = New System.Drawing.Point(20, 197)
        Me.LinkLabel2.Name = "LinkLabel2"
        Me.LinkLabel2.Size = New System.Drawing.Size(210, 13)
        Me.LinkLabel2.TabIndex = 18
        Me.LinkLabel2.TabStop = true
        Me.LinkLabel2.Text = "VECTO is licensed under EUPL 1.1+"
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.Label3.Location = New System.Drawing.Point(20, 213)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(237, 13)
        Me.Label3.TabIndex = 19
        Me.Label3.Text = "Copyright © 2012-2019 European Union"
        '
        'PictureBoxJRC
        '
        Me.PictureBoxJRC.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBoxJRC.Image = CType(resources.GetObject("PictureBoxJRC.Image"),System.Drawing.Image)
        Me.PictureBoxJRC.Location = New System.Drawing.Point(240, 248)
        Me.PictureBoxJRC.Name = "PictureBoxJRC"
        Me.PictureBoxJRC.Size = New System.Drawing.Size(230, 54)
        Me.PictureBoxJRC.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxJRC.TabIndex = 15
        Me.PictureBoxJRC.TabStop = false
        '
        'AboutBox
        '
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(491, 356)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.LinkLabel2)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.PictureBoxJRC)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label8)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "AboutBox"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "About VECTO"
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PictureBoxJRC,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents Label2 As Label
	Friend WithEvents Label8 As Label
	Friend WithEvents Label1 As Label
	Friend WithEvents LinkLabel1 As LinkLabel
	Friend WithEvents LinkLabel2 As LinkLabel
	Friend WithEvents Label3 As Label
	Friend WithEvents PictureBoxJRC As PictureBox
End Class
