Option Strict on

Imports System.Drawing
Imports System.Windows.Forms
Imports VectoAuxiliaries.Electrics
Imports System.ComponentModel
Imports VectoAuxiliaries.Hvac



Public Class frmCombinedAlternators

  Private combinedAlt As CombinedAlternator
  Private originalAlt As CombinedAlternator
  Private altSignals As ICombinedAlternatorSignals
  Protected gbColor As System.Drawing.Color = Color.LightGreen
  Private UserHitCancel As Boolean = false
  Private UserHitSave As Boolean = false
  Private aaltPath As String = ""

  'Constructor(s)
  Public Sub New(aaltPath As String, altSignals As ICombinedAlternatorSignals)

     ' This call is required by the designer.
       InitializeComponent()

     ' Add any initialization after the InitializeComponent() call.

       Me.aaltpath = aaltPath

       combinedAlt = New CombinedAlternator(aaltPath)
       originalAlt = New CombinedAlternator(aaltPath)

       SetupControls()
       BindGrid()


  End Sub


  'General Helpders
  Private Sub BindGrid()

     gvAlternators.DataSource = New BindingList(Of IAlternator)( combinedAlt.Alternators.OrderBy( Function(o) o.AlternatorName ).ToList())

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


     Dim deleteColumn As New DeleteAlternatorColumn()
     With deleteColumn     
       .HeaderText = ""
       .ToolTipText = "Delete this row"
       .Name = "Delete"
       .Width = 25
       .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
     End With

   '  deleteColumn.CellTemplate.ToolTipText="Delete this alternator"
     gvAlternators.Columns.Add(deleteColumn)



  End Sub
  public  Sub  UpdateButtonText()

    If txtIndex.Text=String.Empty then
          btnUpdate.Text = "Add"
      Else     
          btnUpdate.Text = "Update"
      End if

  End sub   
  Private Sub CreateDiagnostics()
  
     txtDiagnostics.Text = combinedAlt.ToString()
  
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

     Dim number As integer

     If Not Integer.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As double

     If Not Double.TryParse(test, number) Then Return False

     If number <= 0 Then Return False


     Return True

End Function
  Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As double

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 Then Return False


     Return True

End Function
  Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As double

     If Not Double.TryParse(test, number) Then Return False

     If number < 0 OrElse number > 1 Then Return False

     Return True

End Function
  Private Function IsNumberBetweenOverZeroAndLessThan100(txtBox As TextBox ) As Boolean

     'Is this numeric sanity check.
     If Not IsNumeric(txtBox.Text) Then 
          ErrorProvider1.SetError(txtBox,"Please enter a number")
          return false
       else
        ErrorProvider1.SetError(txtBox,"")
     End If




     Dim number As double = 0

     If Not Double.TryParse(txtBox.Text, number) Then 
          ErrorProvider1.SetError(txtBox,"Please enter a number >0 and <100")
          Return False

        Else
          ErrorProvider1.SetError(txtBox,String.Empty)        
     End If

     If number <= 0 OrElse number >=100 Then 

        ErrorProvider1.SetError(txtBox,"Please enter a number >0 and <100")
        Return False
        Else 
         ErrorProvider1.SetError(txtBox,String.Empty)
        Return true

     End If


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


  'Other events
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


  'Grid Events
  Private Sub gvAlternators_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles gvAlternators.CellClick

     If e.ColumnIndex < 0 OrElse e.RowIndex < 0 Then Return


     If gvAlternators.Columns(e.ColumnIndex).Name = "Delete" Then


        Dim feedback As String = String.Empty
        Dim alternatorName As String = gvAlternators.Rows(e.RowIndex).Cells(0).Value.ToString()


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
  Private Sub gvAlternators_CellDoubleClick( sender As Object,  e As DataGridViewCellEventArgs) Handles gvAlternators.CellDoubleClick
 
       If  gvAlternators.SelectedCells.Count<1 then Return
       
   
        Dim row As Integer = gvAlternators.SelectedCells(0).OwningRow.Index
   
        Dim alternatorName  As String
 
        alternatorName = gvAlternators.Rows(row).Cells("AlternatorName").Value.ToString()
    
        Dim alt as IAlternator = combinedAlt.Alternators.First( Function(w) w.AlternatorName= alternatorName)
  
 
        FillEditPanel( row )
   
        UpdateButtonText()
 
 
 End Sub

 'Button Events
  Private Sub btnClearForm_Click( sender As Object,  e As EventArgs) Handles btnClearForm.Click
 
     ClearEditPanel()
     UpdateButtonText()
 
 End Sub
  public function GetAlternatorFromPanel() As List(Of ICombinedAlternatorMapRow )
  
  
  
    Dim newAlt As New List(Of ICombinedAlternatorMapRow)
  
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000,10,Convert.ToSingle(txt2K10Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000,40,Convert.ToSingle(txt2K40Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000,60,Convert.ToSingle(txt2K60Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
                                                                        
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000,10,Convert.ToSingle(txt4K10Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000,40,Convert.ToSingle(txt4K40Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000,60,Convert.ToSingle(txt4K60Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
                                                                       
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000,10,Convert.ToSingle(txt6K10Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000,40,Convert.ToSingle(txt6K40Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
    newAlt.Add( New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000,60,Convert.ToSingle(txt6K60Efficiency.Text),Convert.ToSingle(txtPulleyRatio.Text)))
  
  
  
    Return newAlt
  
  
  End Function
  Private Sub btnUpdate_Click( sender As Object,  e As EventArgs) Handles btnUpdate.Click
  
     
      Dim feedback As String = String.Empty
    
      If NOT Validate_UpdatePanel() then Return
      
      If txtIndex.Text.Trim.Length=0 then 
      'This is an Add
         If Not combinedAlt.AddAlternator( GetAlternatorFromPanel(), feedback) then
            MessageBox.Show( feedback )
         Else
       
  
          BindGrid()
    
          UpdateButtonText()
    
       End if
    
      Else

      'Get Existing row name.
       Dim altName As String = gvAlternators.Rows( Convert.ToInt32(txtIndex.Text)).Cells("AlternatorName").Value.ToString()

       'Does name used in update exist in other alternators excluding the original being edited ?, if so abort.
       If combinedAlt.Alternators.Where( Function(f) f.AlternatorName<> altName AndAlso f.AlternatorName=txtAlternatorName.Text).Count>0 then
         MessageBox.Show( String.Format("The lternator '{0}' name you are using to update the alternator '{1}' already exists, operation aborted",txtAlternatorName.Text,altName))
         return
       End If

      'This is an update so delete the one being updated

      If  combinedAlt.DeleteAlternator(altName, feedback) AndAlso combinedAlt.AddAlternator(GetAlternatorFromPanel(),feedback )  then

           BindGrid()
           ClearEditPanel()
           UpdateButtonText()
             
         Else
           MessageBox.Show( feedback )
        End If
    
      End If
  
  
  
  End Sub
  Private Sub btnCancel_Click( sender As Object,  e As EventArgs) Handles btnCancel.Click
  
     UserHitCancel=true
     Me.close
  
  End Sub
  Private Sub btnSave_Click( sender As Object,  e As EventArgs) Handles btnSave.Click

    '  If Not ValidateAll then Return 

        UserHitSave=true

        Me.DialogResult=Windows.Forms.DialogResult.OK
        Me.Close
       


  End Sub


  'Form / Tab Events
  Private Sub TabControl1_SelectedIndexChanged( sender As Object,  e As EventArgs) Handles TabControl1.SelectedIndexChanged
  
    If  TabControl1.SelectedIndex = 1
  
  
       CreateDiagnostics()
  
  
    End If
  End Sub
  Private Sub frmCombinedAlternators_FormClosing( sender As Object,  e As FormClosingEventArgs) Handles MyBase.FormClosing


       Dim result As DialogResult
      
       'If UserHitCancel then bail
       If UserHitCancel then 
          DialogResult= Windows.Forms.DialogResult.Cancel
          UserHitCancel=false
          return
       End If
      
       'UserHitSave
       If UserHitSave then 
          DialogResult= Windows.Forms.DialogResult.Cancel
          If NOT combinedAlt.Save(aaltPath )   then
                MessageBox.Show("Unable to save file, aborting.")
                e.Cancel=true
           End If
          UserHitSave=false
          return
       End If
      
      
      ''This must be a close box event. If nothing changed, then bail, otherwise ask user if they wanna save
      If  Not combinedAlt.IsEqualTo( originalAlt ) 
      
           result = (MessageBox.Show("Would you like to save changes before closing?","Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
      
      
                Select Case  result
                
                    case DialogResult.Yes:
                        'save 
      
                        If NOT combinedAlt.Save(aaltPath)   then
                          e.Cancel=true
                        End If
      
                    case DialogResult.No:
                        'just allow the form to close
                        'without saving
                        Me.DialogResult=Windows.Forms.DialogResult.Cancel
      
      
      
                    case DialogResult.Cancel:
                        'cancel the close
                        e.Cancel = true
                        Me.DialogResult=Windows.Forms.DialogResult.Cancel
      
      
                end select
      
      End If
      
      UserHitCancel=false
      UserHitSave=false



  End Sub


  'List Management
  Private Sub ClearEditPanel()
  
     txtIndex.Text          = String.Empty
     txtAlternatorName.Text = String.Empty
     txt2K10Efficiency.Text = string.Empty
     txt2K40Efficiency.Text = string.Empty
     txt2K60Efficiency.Text = string.Empty
                             
     txt4K10Efficiency.Text = string.Empty
     txt4K40Efficiency.Text = string.Empty
     txt4K60Efficiency.Text = string.Empty
                             
     txt6K10Efficiency.Text = string.Empty
     txt6K40Efficiency.Text = string.Empty
     txt6K60Efficiency.Text = string.Empty
                             
     txtPulleyRatio   .Text = string.Empty

     ErrorProvider1.SetError(txtAlternatorName    , String.empty)
     ErrorProvider1.SetError(txt2K10Efficiency    , String.empty)
     ErrorProvider1.SetError(txt2K40Efficiency    , String.empty)
     ErrorProvider1.SetError(txt2K60Efficiency    , String.empty)
     ErrorProvider1.SetError(txt4K10Efficiency    , String.empty)
     ErrorProvider1.SetError(txt4K40Efficiency    , String.empty)
     ErrorProvider1.SetError(txt4K60Efficiency    , String.empty)
     ErrorProvider1.SetError(txt6K10Efficiency    , String.empty)
     ErrorProvider1.SetError(txt6K40Efficiency    , String.empty)
     ErrorProvider1.SetError(txt6K60Efficiency    , String.empty)

     ErrorProvider1.SetError(txtPulleyRatio       , String.empty)

  
  End Sub
  public Function Validate_UpdatePanel() As Boolean
  
    Dim returnResult As Boolean = True
  
     IsEmptyString(txtAlternatorName.Text, txtAlternatorName,"Please enter a name for the alternator, names must be unique", returnResult)
  
     If Not IsNumberBetweenOverZeroAndLessThan100(txt2K10Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt2K40Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt2K60Efficiency) then   returnResult = False
  
     If Not IsNumberBetweenOverZeroAndLessThan100(txt4K10Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt4K40Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt4K60Efficiency) then   returnResult = False
  
     If Not IsNumberBetweenOverZeroAndLessThan100(txt6K10Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt6K40Efficiency) then   returnResult = False
     If Not IsNumberBetweenOverZeroAndLessThan100(txt6K60Efficiency) then   returnResult = False  
    
     If Not IsPostiveNumber(txtPulleyRatio.text) then
        ErrorProvider1.SetError(txtPulleyRatio,"Please enter a sensible positive number")
        returnResult=False
      Else
        ErrorProvider1.SetError(txtPulleyRatio,String.Empty)
     End If
  
    Return returnResult
  
  End Function
  Private sub FillEditPanel( index  as integer )

     Dim alt As IAlternator
     Dim alternatorName  As String = gvAlternators.Rows(index).Cells("AlternatorName").Value.ToString()

     alt = combinedAlt.Alternators.First( Function(f) f.AlternatorName=alternatorName )

     txtIndex.Text          = index.ToString()
     txtAlternatorName.Text = alt.AlternatorName
     txt2K10Efficiency.Text = alt.InputTable2000.First( Function(x) x.Amps=10).Eff.ToString()
     txt2K40Efficiency.Text = alt.InputTable2000.First( Function(x) x.Amps=40).Eff.ToString()
     txt2K60Efficiency.Text = alt.InputTable2000.First( Function(x) x.Amps=60).Eff.ToString()
                                                                                  
     txt4K10Efficiency.Text = alt.InputTable4000.First( Function(x) x.Amps=10).Eff.ToString()
     txt4K40Efficiency.Text = alt.InputTable4000.First( Function(x) x.Amps=40).Eff.ToString()
     txt4K60Efficiency.Text = alt.InputTable4000.First( Function(x) x.Amps=60).Eff.ToString()
                                                                                  
     txt6K10Efficiency.Text = alt.InputTable6000.First( Function(x) x.Amps=10).Eff.ToString()
     txt6K40Efficiency.Text = alt.InputTable6000.First( Function(x) x.Amps=40).Eff.ToString()
     txt6K60Efficiency.Text = alt.InputTable6000.First( Function(x) x.Amps=60).Eff.ToString()
          
     txtPulleyRatio   .Text = alt.PulleyRatio.ToString()


 End Sub



End Class


