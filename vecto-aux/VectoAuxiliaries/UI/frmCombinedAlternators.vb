Imports System.Drawing
Imports System.Windows.Forms
Imports VectoAuxiliaries.Electrics
Imports System.ComponentModel



Public Class frmCombinedAlternators

  Private combinedAlt As CombinedAlternator
  Private originalAlt As CombinedAlternator
  Private altSignals As ICombinedAlternatorSignals
  Protected gbColor As System.Drawing.Color = Color.LightGreen




  Public Sub New(aaltPath As String, altSignals As ICombinedAlternatorSignals)

     ' This call is required by the designer.
     InitializeComponent()

     ' Add any initialization after the InitializeComponent() call.
       combinedAlt = New CombinedAlternator(aaltPath, altSignals)
       originalAlt = New CombinedAlternator(aaltPath, altSignals)

       SetupControls()
       BindGrid()


  End Sub

  Private Sub BindGrid()

 ' New BindingList(Of ITechListBenefitLine)(ssmTOOL.TechList.TechLines.OrderBy( Function(o) o.Category).ThenBy( Function(t) t.BenefitName).ToList())
     gvAlternators.DataSource = New BindingList(Of combinedAlternator)( combinedAlt.Alternators)


    ' gvAlternators.Refresh()


  End Sub

  Private Sub SetupControls()

     'gvAlternators
     gvAlternators.AutoGenerateColumns = False

     Dim cIndex As Integer

     'Column - AlternatorName
     cIndex = gvAlternators.Columns.Add("AlternatorName", "AlternatorName")
     gvAlternators.Columns(cIndex).DataPropertyName = "AlternatorName"
     gvAlternators.Columns(cIndex).Width = 250
     gvAlternators.Columns(cIndex).ReadOnly = True
     gvAlternators.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvAlternators.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


    'Column - PulleyRatio
     cIndex = gvAlternators.Columns.Add("PulleyRatio", "PulleyRatio")
     gvAlternators.Columns(cIndex).DataPropertyName = "PulleyRatio"
     gvAlternators.Columns(cIndex).Width = 70
     gvAlternators.Columns(cIndex).ReadOnly = True
     gvAlternators.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvAlternators.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)


     Dim deleteColumn As New DeleteColumn
     With deleteColumn
       .HeaderText = ""
       .ToolTipText = "Delete this row"
       .Name = "Delete"
       .Width = 25
       .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
     End With
     gvAlternators.Columns.Add(deleteColumn)



  End Sub


  'Validation Helpers
  Private Sub IsTextBoxNumber(control As TextBox, errorProviderMessage As String, ByRef result As Boolean)

      If Not IsNumeric(control.Text) Then
         ErrorProvider1.SetError(control, errorProviderMessage)
         result = False
        Else
         ErrorProvider1.SetError(control, String.Empty)

        End If

  End Sub
  Private Sub IsEmptyString(text As String, control As Control, errorProviderMessage As String, ByRef result As Boolean)

      If String.IsNullOrEmpty(text) Then
         ErrorProvider1.SetError(control, errorProviderMessage)
         result = False
        Else
         ErrorProvider1.SetError(control, String.Empty)

        End If

  End Sub
  Private Function IsPostiveInteger(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Integer.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 Then Return False


     Return True

End Function
  Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 OrElse number > 1 Then Return False

     Return True

End Function
  Private Function IsIntegerZeroOrPositiveNumber(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     'if not integer then return false

     Dim number As Integer

     If Not Integer.TryParse(test, number) Then Return False

     If number < 0 Then Return False

     Return True


End Function
  Private Function ValidateAll() As Boolean




  End Function





  Private Sub groupBoxUserInput_Paint(sender As Object, e As Windows.Forms.PaintEventArgs) Handles grpTable2000PRM.Paint, grpTable6000PRM.Paint, grpTable4000PRM.Paint


            Dim p As Pen = Nothing

            Dim sdr As Control = DirectCast(sender, Control)

            Select Case sdr.Name

              Case "grpTable2000PRM"
                p = New Pen(Color.LightGreen, 3)
              Case "grpTable4000PRM"
                p = New Pen(Color.Yellow, 3)
              Case "grpTable6000PRM"
                p = New Pen(Color.LightPink, 3)

                Case Else
                p = New Pen(Color.Black, 3)

            End Select



            Dim gfx As Graphics = e.Graphics



            gfx.DrawLine(p, 0, 5, 0, e.ClipRectangle.Height - 2)
            gfx.DrawLine(p, 0, 5, 10, 5)
            gfx.DrawLine(p, 85, 5, e.ClipRectangle.Width - 2, 5)
            gfx.DrawLine(p, e.ClipRectangle.Width - 2, 5, e.ClipRectangle.Width - 2, e.ClipRectangle.Height - 2)
            gfx.DrawLine(p, e.ClipRectangle.Width - 2, e.ClipRectangle.Height - 2, 0, e.ClipRectangle.Height - 2)

  End Sub




  Private Sub gvAlternators_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles gvAlternators.CellClick

     If e.ColumnIndex < 0 OrElse e.RowIndex < 0 Then Return


     If gvAlternators.Columns(e.ColumnIndex).Name = "Delete" Then


        Dim feedback As String = String.Empty
        Dim alternatorName As String = gvAlternators.Rows(e.RowIndex).Cells(0).Value


        Select Case gvAlternators.Columns(e.ColumnIndex).Name


        Case "Delete"
           Dim dr As DialogResult = MessageBox.Show(String.Format("Do you want to delete  '{0}' ?", alternatorName), "", MessageBoxButtons.YesNo)
           If dr = Windows.Forms.DialogResult.Yes Then
            If combinedAlt.DeleteAlternator(alternatorName, feedback) Then
              BindGrid()
              Else
             MessageBox.Show(feedback)

            End If

           End If





        End Select



     End If

End Sub






End Class


