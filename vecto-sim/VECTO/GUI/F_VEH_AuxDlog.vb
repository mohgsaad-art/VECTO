' Copyright 2014 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports VectoAuxiliaries.Hvac

''' <summary>
''' Aux Config Editor (Job Editor sub-dialog)
''' </summary>
''' <remarks></remarks>
Public Class F_VEH_AuxDlog

    Public VehPath As String = ""

    Public Property ListItems As New Dictionary(Of String, Single)


    'New instance
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Me.CbType.Items.Add("Fan")
        Me.CbType.Items.Add("Steering pump")
        Me.CbType.Items.Add("HVAC")
        Me.CbType.Items.Add("Electric System")

        Me.PnFile.Enabled = Not Cfg.DeclMode
        Me.PnTech.Enabled = Cfg.DeclMode


    End Sub




    'Initialise form
    Private Sub F_VEH_AuxDlog_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Text = CbType.Text
    End Sub


    'Set generic values for Declaration mode
    Private Sub DeclInit()
        Dim txt As String
        Dim kv As KeyValuePair(Of String, Dictionary(Of tMission, Single))

        Me.CbTech.Items.Clear()

        'Added this section to enable or disable the new controls for Post file version 2 - TB 25/9/14
        Tabs.TabPages.Clear()
        Tabs.TabPages.Add(tabMain)

        Select Case TbID.Text

            Case sKey.AUX.ElecSys.ToString()
                tabListItems.Text = "Electrical Consumers"
                Tabs.TabPages.Add(tabListItems)

            Case sKey.AUX.PneumSys.ToString()
                tabListItems.Text = "Pneumatic Consumers"
                Tabs.TabPages.Add(tabListItems)

            Case sKey.AUX.HVAC.ToString()
                tabListItems.Text = "Map Inputs"
                Tabs.TabPages.Add(tabListItems)

            Case sKey.AUX.Fan
                Tabs.TabPages.Add(tabTechnologies)

            Case sKey.AUX.SteerPump
                Tabs.TabPages.Add(tabTechnologies)

        End Select

        Select Case TbID.Text
            Case sKey.AUX.Fan
                For Each txt In Declaration.AuxTechs(tAux.Fan)
                    Me.CbTech.Items.Add(txt)
                Next

            Case sKey.AUX.SteerPump
                For Each txt In Declaration.AuxTechs(tAux.SteerPump)
                    Me.CbTech.Items.Add(txt)
                Next

            Case sKey.AUX.HVAC
                For Each txt In Declaration.AuxTechs(tAux.HVAC)
                    Me.CbTech.Items.Add(txt)
                Next
                Me.CbTech.SelectedIndex = 0

            Case sKey.AUX.ElecSys
                For Each txt In Declaration.AuxTechs(tAux.ElectricSys)
                    Me.CbTech.Items.Add(txt)
                Next
                Me.CbTech.SelectedIndex = 0


            Case sKey.AUX.PneumSys
                For Each txt In Declaration.AuxTechs(tAux.PneumSys)
                    Me.CbTech.Items.Add(txt)
                Next
                Me.CbTech.SelectedIndex = 0

        End Select


        If TbID.Text = sKey.AUX.ElecSys Then

            Me.LVTech.Items.Clear()
            For Each kv In Declaration.AuxESpower
                Me.LVTech.Items.Add(kv.Key)
            Next
            Me.LVTech.Visible = True

            'TB Removed for newer design of existing form 25/9/14
            '  Me.Height = 457

        Else

            Me.LVTech.Visible = False
            'TB Removed for newer design of existing form 25/9/14
            ' Me.Height = 220

        End If

    End Sub

    'Save and close
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    'Cancel
    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    'Close form. Check if form is complete and valid
    Private Sub F_VEH_AuxDlog_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.WindowsShutDown And Me.DialogResult <> Windows.Forms.DialogResult.Cancel Then

            If Trim(Me.TbID.Text) = "" Or Trim(Me.CbType.Text) = "" Then
                MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
                e.Cancel = True
            End If

            If Me.TbID.Text.Contains(",") Or Me.CbType.Text.Contains(",") Or Me.TbPath.Text.Contains(",") Then
                MsgBox("',' is no valid character!", MsgBoxStyle.Critical)
                e.Cancel = True
            End If

            If Cfg.DeclMode Then

                'Old Tech only seems to apply to electricals 
                If Me.CbTech.Text = "" AndAlso sKey.AUX.ElecSys.ToString() = TbID.Text Then
                    MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
                    e.Cancel = True
                End If

            Else

                'Engineering Mode
                If Trim(Me.TbPath.Text) = "" Then
                    MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
                    e.Cancel = True
                End If

                'Determin specific Validation based on type
                Select Case TbID.Text

                    Case sKey.AUX.HVAC
                        e.Cancel = Not ValidateHVAC()



                End Select



            End If

        End If
    End Sub

    ''' <summary>
    ''' HVAC VALIDATION
    ''' </summary>
    ''' <param name="message">Returns a string with any errors</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateHVAC() As Boolean

        Dim message As String = String.Empty

        'Validate Pulley
        If Not ValidatePulley(message) Then
            MessageBox.Show(message)
            Return False
        End If

        'Validate Inputs
        If Not ValidateHVACInputs(message) Then
            MessageBox.Show(message)
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' HVAC and Alternators use pulleys, this routine checks them
    ''' </summary>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidatePulley(ByRef message As String) As Boolean

        Dim pulleyEfficiency As String = txtPulleyGearEfficiency.Text.Trim
        Dim pulleyGearRatio As String = txtPulleyGearRatio.Text.Trim

        'Values present
        If (pulleyEfficiency.Length = 0) OrElse (pulleyGearRatio.Length = 0) Then
            message = "Please fill in the pulley values in the main tab."
            Return False
        End If

        'Values numeric
        If Not IsNumeric(pulleyEfficiency) OrElse Not IsNumeric(pulleyGearRatio) Then
            message = "One of the pulley values on the main tab is not a numeric value."
            Return False
        End If

        'Value Ranges
        Dim efficiencyValue As Single = CType(pulleyEfficiency, Single)
        Dim gearRatio As Single = CType(pulleyGearRatio, Single)

        Const TooLowRatio As Single = 0.0
        Const TooHighRatio As Single = 6.0
        Const TooLowEfficiency As Single = 0
        Const TooHighEfficiency As Single = 1

        'Efficiency check
        If (efficiencyValue <= TooLowEfficiency) OrElse (efficiencyValue >= TooHighEfficiency) Then
            message = "Efficiency value must be greater than 0 and less than 1"
            Return False
        End If

        'Ratio Check
        If (gearRatio <= TooLowRatio) OrElse (gearRatio >= TooHighRatio) Then
            message = "Pulley gear ratio value must be greater than 0 and less than 6"
            Return False
        End If

        message = String.Empty
        Return True

    End Function

    ''' <summary>
    ''' HVAC Require the correct number of inputs, for this we need to instantiate the HVACLoad Demand
    ''' Using the HVACMap lookup Map File 
    ''' </summary>
    ''' <param name="message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateHVACInputs(ByRef message As String) As Boolean

        'Validate Number of inputs
        If dgvInputs.Rows.Count < 2 Then
            message = "No Inputs are available please select the lookup map on the Main tab"
            Return False
        End If

        Return True

    End Function

    'Browse for .vaux files
    Private Sub BtBrowse_Click(sender As System.Object, e As System.EventArgs) Handles BtBrowse.Click


        If fbAUX.OpenDialog(fFileRepl(Me.TbPath.Text, VehPath)) Then Me.TbPath.Text = fFileWoDir(fbAUX.Files(0), VehPath)

        If (TbID.Text = sKey.AUX.HVAC AndAlso Me.TbPath.Text.Length <> 0) Then
            Dim frmHVAC As New VectoAuxiliaries.UI.F_HVAC(Me.TbPath.Text)

            'If we have results then populate the inputs tab
            If (frmHVAC.ShowDialog() = Windows.Forms.DialogResult.OK) Then

                dgvInputs.Rows.Clear()

                ListItems.Clear()

                For Each item As KeyValuePair(Of String, String) In frmHVAC.Inputs

                    Dim row As DataGridViewRow = dgvInputs.Rows(dgvInputs.Rows.Add())
                    row.Cells(0).Value = item.Key
                    row.Cells(1).Value = item.Value


                    ListItems.Add(item.Key, item.Value)

                Next

 
            End If


        End If


    End Sub

    'Update ID when Aux Type was changed
    Private Sub CbType_TextChanged(sender As Object, e As System.EventArgs) Handles CbType.TextChanged

        If Me.CbType.Text = "" Then
            Me.TbID.Text = ""
        Else
            If Cfg.DeclMode Then
                Select Case Me.CbType.SelectedIndex
                    Case 0
                        Me.TbID.Text = sKey.AUX.Fan
                    Case 1
                        Me.TbID.Text = sKey.AUX.SteerPump

                    Case Else '2
                        Me.TbID.Text = sKey.AUX.HVAC
                End Select
            Else
                Me.TbID.Text = Trim(UCase(Me.CbType.Text.Substring(0, CInt(Math.Min(Me.CbType.Text.Length, 3)))))
            End If
        End If




    End Sub

    'Update help label if ID was changed
    Private Sub TbID_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbID.TextChanged

        DeclInit()

        If Trim(Me.TbID.Text) = "" Or Cfg.DeclMode Then
            Me.LbIDhelp.Text = ""
        Else
            Me.LbIDhelp.Text = "Header in Driving cycle: <AUX_" & Trim(Me.TbID.Text) & ">"
        End If

    End Sub


    Private Sub btnConsumerAdd_Click(sender As Object, e As EventArgs) Handles btnConsumerAdd.Click

    End Sub


    Public Sub ClearAllValues(Optional clearTypes As Boolean = False)

        If (clearTypes) Then
            Me.CbType.SelectedIndex = -1
            Me.CbType.Text = ""
            Me.TbID.Text = ""
        End If

        Me.TbPath.Text = ""
        Me.txtPulleyGearEfficiency.Text = String.Empty
        Me.txtPulleyGearRatio.Text = String.Empty
        Me.dgvInputs.ClearSelection()
        Me.LVTech.Clear()

    End Sub

End Class
