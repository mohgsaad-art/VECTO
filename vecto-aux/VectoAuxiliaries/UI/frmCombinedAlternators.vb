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
    Private UserHitCancel As Boolean = False
    Private UserHitSave As Boolean = False
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

        gvAlternators.DataSource = New BindingList(Of IAlternator)(combinedAlt.Alternators.OrderBy(Function(o) o.AlternatorName).ToList())

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
    Public Sub UpdateButtonText()

        If txtIndex.Text = String.Empty Then
            btnUpdate.Text = "Add"
        Else
            btnUpdate.Text = "Update"
        End If

    End Sub
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

        Dim number As Integer

        If Not Integer.TryParse(test, number) Then Return False

        If number <= 0 Then Return False


        Return True

    End Function
    Private Function IsPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Double

        If Not Double.TryParse(test, number) Then Return False

        If number <= 0 Then Return False


        Return True

    End Function
    Private Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Double

        If Not Double.TryParse(test, number) Then Return False

        If number < 0 Then Return False


        Return True

    End Function
    Private Function IsNumberBetweenZeroandOne(test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Double

        If Not Double.TryParse(test, number) Then Return False

        If number < 0 OrElse number > 1 Then Return False

        Return True

    End Function
    Private Function IsNumberBetweenOverZeroAndLessThan100(txtBox As TextBox) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(txtBox.Text) Then
            ErrorProvider1.SetError(txtBox, "Please enter a number")
            Return False
        Else
            ErrorProvider1.SetError(txtBox, "")
        End If




        Dim number As Double = 0

        If Not Double.TryParse(txtBox.Text, number) Then
            ErrorProvider1.SetError(txtBox, "Please enter a number >0 and <100")
            Return False

        Else
            ErrorProvider1.SetError(txtBox, String.Empty)
        End If

        If number <= 0 OrElse number >= 100 Then

            ErrorProvider1.SetError(txtBox, "Please enter a number >0 and <100")
            Return False
        Else
            ErrorProvider1.SetError(txtBox, String.Empty)
            Return True

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
    Private Sub gvAlternators_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles gvAlternators.CellDoubleClick

        If gvAlternators.SelectedCells.Count < 1 Then Return

        Dim row As Integer = gvAlternators.SelectedCells(0).OwningRow.Index

        Dim alternatorName As String

        alternatorName = gvAlternators.Rows(row).Cells("AlternatorName").Value.ToString()

        Dim alt As IAlternator = combinedAlt.Alternators.First(Function(w) w.AlternatorName = alternatorName)


        FillEditPanel(row)

        UpdateButtonText()


    End Sub

    'Button Events
    Private Sub btnClearForm_Click(sender As Object, e As EventArgs) Handles btnClearForm.Click

        ClearEditPanel()
        UpdateButtonText()

    End Sub
    Public Function GetAlternatorFromPanel() As List(Of ICombinedAlternatorMapRow)

        Dim newAlt As New List(Of ICombinedAlternatorMapRow)

        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000, Convert.ToSingle(txt2K10Amps.Text), Convert.ToSingle(txt2K10Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000, Convert.ToSingle(txt2KMax2Amps.Text), Convert.ToSingle(txt2KMax2Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 2000, Convert.ToSingle(txt2KMaxAmps.Text), Convert.ToSingle(txt2KMaxEfficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))

        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000, Convert.ToSingle(txt4K10Amps.Text), Convert.ToSingle(txt4K10Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000, Convert.ToSingle(txt4KMax2Amps.Text), Convert.ToSingle(txt4KMax2Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 4000, Convert.ToSingle(txt4KMaxAmps.Text), Convert.ToSingle(txt4KMaxEfficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))

        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000, Convert.ToSingle(txt6K10Amps.Text), Convert.ToSingle(txt6K10Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000, Convert.ToSingle(txt6KMax2Amps.Text), Convert.ToSingle(txt6KMax2Efficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))
        newAlt.Add(New CombinedAlternatorMapRow(txtAlternatorName.Text, 6000, Convert.ToSingle(txt6KMaxAmps.Text), Convert.ToSingle(txt6KMaxEfficiency.Text), Convert.ToSingle(txtPulleyRatio.Text)))

        Return newAlt

    End Function
    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click


        Dim feedback As String = String.Empty

        If Not Validate_UpdatePanel() Then Return

        If txtIndex.Text.Trim.Length = 0 Then
            'This is an Add
            If Not combinedAlt.AddAlternator(GetAlternatorFromPanel(), feedback) Then
                MessageBox.Show(feedback)
            Else


                BindGrid()

                UpdateButtonText()

            End If

        Else

            'Get Existing row name.
            Dim altName As String = gvAlternators.Rows(Convert.ToInt32(txtIndex.Text)).Cells("AlternatorName").Value.ToString()

            'Does name used in update exist in other alternators excluding the original being edited ?, if so abort.
            If combinedAlt.Alternators.Where(Function(f) f.AlternatorName <> altName AndAlso f.AlternatorName = txtAlternatorName.Text).Count > 0 Then
                MessageBox.Show(String.Format("The lternator '{0}' name you are using to update the alternator '{1}' already exists, operation aborted", txtAlternatorName.Text, altName))
                Return
            End If

            'This is an update so delete the one being updated

            If combinedAlt.DeleteAlternator(altName, feedback) AndAlso combinedAlt.AddAlternator(GetAlternatorFromPanel(), feedback) Then

                BindGrid()
                ClearEditPanel()
                UpdateButtonText()

            Else
                MessageBox.Show(feedback)
            End If

        End If



    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        UserHitCancel = True
        Me.close()

    End Sub
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        '  If Not ValidateAll then Return 

        UserHitSave = True

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()



    End Sub


    'Form / Tab Events
    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged

        If TabControl1.SelectedIndex = 1 Then
            CreateDiagnostics()
        End If

    End Sub
    Private Sub frmCombinedAlternators_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing


        Dim result As DialogResult

        'If UserHitCancel then bail
        If UserHitCancel Then
            DialogResult = Windows.Forms.DialogResult.Cancel
            UserHitCancel = False
            Return
        End If

        'UserHitSave
        If UserHitSave Then
            DialogResult = Windows.Forms.DialogResult.Cancel
            If Not combinedAlt.Save(aaltPath) Then
                MessageBox.Show("Unable to save file, aborting.")
                e.Cancel = True
            End If
            UserHitSave = False
            DialogResult = Windows.Forms.DialogResult.OK
            Return
        End If


        ''This must be a close box event. If nothing changed, then bail, otherwise ask user if they wanna save
        If Not combinedAlt.IsEqualTo(originalAlt) Then

            result = (MessageBox.Show("Would you like to save changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))


            Select Case result

                Case DialogResult.Yes
                    'save 

                    If Not combinedAlt.Save(aaltPath) Then
                        e.Cancel = True
                    End If

                Case DialogResult.No
                    'just allow the form to close
                    'without saving
                    Me.DialogResult = Windows.Forms.DialogResult.Cancel

                Case DialogResult.Cancel
                    'cancel the close
                    e.Cancel = True
                    Me.DialogResult = Windows.Forms.DialogResult.Cancel

            End Select

        End If

        UserHitCancel = False
        UserHitSave = False



    End Sub


    'List Management
    Private Sub ClearEditPanel()

        txtIndex.Text = String.Empty
        txtAlternatorName.Text = String.Empty
        txt2K10Efficiency.Text = String.Empty
        txt2KMax2Efficiency.Text = String.Empty
        txt2KMaxEfficiency.Text = String.Empty

        txt4K10Efficiency.Text = String.Empty
        txt4KMax2Efficiency.Text = String.Empty
        txt4KMaxEfficiency.Text = String.Empty

        txt6K10Efficiency.Text = String.Empty
        txt6KMax2Efficiency.Text = String.Empty
        txt6KMaxEfficiency.Text = String.Empty

        txt2KMax2Amps.Text = String.Empty
        txt2KMaxAmps.Text = String.Empty

        txt4KMax2Amps.Text = String.Empty
        txt4KMaxAmps.Text = String.Empty

        txt6KMax2Amps.Text = String.Empty
        txt6KMaxAmps.Text = String.Empty

        txtPulleyRatio.Text = String.Empty

        ErrorProvider1.SetError(txtAlternatorName, String.empty)
        ErrorProvider1.SetError(txt2K10Efficiency, String.empty)
        ErrorProvider1.SetError(txt2KMax2Efficiency, String.empty)
        ErrorProvider1.SetError(txt2KMaxEfficiency, String.empty)
        ErrorProvider1.SetError(txt4K10Efficiency, String.empty)
        ErrorProvider1.SetError(txt4KMax2Efficiency, String.empty)
        ErrorProvider1.SetError(txt4KMaxEfficiency, String.empty)
        ErrorProvider1.SetError(txt6K10Efficiency, String.empty)
        ErrorProvider1.SetError(txt6KMax2Efficiency, String.empty)
        ErrorProvider1.SetError(txt6KMaxEfficiency, String.empty)

        ErrorProvider1.SetError(txtPulleyRatio, String.empty)


    End Sub
    Public Function Validate_UpdatePanel() As Boolean

        Dim returnResult As Boolean = True

        IsEmptyString(txtAlternatorName.Text, txtAlternatorName, "Please enter a name for the alternator, names must be unique", returnResult)

        If Not IsNumberBetweenOverZeroAndLessThan100(txt2K10Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt2KMax2Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt2KMaxEfficiency) Then returnResult = False

        If Not IsNumberBetweenOverZeroAndLessThan100(txt4K10Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt4KMax2Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt4KMaxEfficiency) Then returnResult = False

        If Not IsNumberBetweenOverZeroAndLessThan100(txt6K10Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt6KMax2Efficiency) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt6KMaxEfficiency) Then returnResult = False

        If Not IsNumberBetweenOverZeroAndLessThan100(txt2K10Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt2KMax2Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt2KMaxAmps) Then returnResult = False

        If Not IsNumberBetweenOverZeroAndLessThan100(txt4K10Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt4KMax2Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt4KMaxAmps) Then returnResult = False

        If Not IsNumberBetweenOverZeroAndLessThan100(txt6K10Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt6KMax2Amps) Then returnResult = False
        If Not IsNumberBetweenOverZeroAndLessThan100(txt6KMaxAmps) Then returnResult = False

        If Not IsPostiveNumber(txtPulleyRatio.text) Then
            ErrorProvider1.SetError(txtPulleyRatio, "Please enter a sensible positive number")
            returnResult = False
        Else
            ErrorProvider1.SetError(txtPulleyRatio, String.Empty)
        End If

        Return returnResult

    End Function
    Private Sub FillEditPanel(index As Integer)

        Dim alt As IAlternator
        Dim alternatorName As String = gvAlternators.Rows(index).Cells("AlternatorName").Value.ToString()

        alt = combinedAlt.Alternators.First(Function(f) f.AlternatorName = alternatorName)

        txtIndex.Text = index.ToString()
        txtAlternatorName.Text = alt.AlternatorName

        'Table1 - 2K Table
        txt2K10Amps.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(1).First().Amps.ToString()
        txt2K10Efficiency.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(1).First().Eff.ToString()

        txt2KMax2Amps.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(2).First().Amps.ToString()
        txt2KMax2Efficiency.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(2).First().Eff.ToString()

        txt2KMaxAmps.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(3).First().Amps.ToString()
        txt2KMaxEfficiency.Text = alt.InputTable2000.OrderBy(Function(x) x.Amps).Skip(3).First().Eff.ToString()

        'Table2 - 4K Table
        txt4K10Amps.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(1).First().Amps.ToString()
        txt4K10Efficiency.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(1).First().Eff.ToString()

        txt4KMax2Amps.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(2).First().Amps.ToString()
        txt4KMax2Efficiency.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(2).First().Eff.ToString()

        txt4KMaxAmps.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(3).First().Amps.ToString()
        txt4KMaxEfficiency.Text = alt.InputTable4000.OrderBy(Function(x) x.Amps).Skip(3).First().Eff.ToString()

        'Table3 - 6K Table
        txt6K10Amps.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(1).First().Amps.ToString()
        txt6K10Efficiency.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(1).First().Eff.ToString()

        txt6KMax2Amps.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(2).First().Amps.ToString()
        txt6KMax2Efficiency.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(2).First().Eff.ToString()

        txt6KMaxAmps.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(3).First().Amps.ToString()
        txt6KMaxEfficiency.Text = alt.InputTable6000.OrderBy(Function(x) x.Amps).Skip(3).First().Eff.ToString()
       
        txtPulleyRatio.Text = alt.PulleyRatio.ToString()


    End Sub



End Class


