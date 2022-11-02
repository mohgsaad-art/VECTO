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

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Util
Imports TUGraz.VectoCore.OutputData.FileIO


Public Class frmAuxiliaryConfig

#Region "Fields"

    Public auxConfig As IAuxiliaryConfig
    Public originalConfig As IAuxiliaryConfig ' required to test if the form is dirty
    Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()
    Private processing As Boolean = False
    Private SecondsIntoCycle As Integer = 0
    Private vectoFile As String = ""
    Private aauxPath As String = ""
    Private auxFile As String
    Private cmFilesList As String()
    Private SaveClicked As Boolean
    'Private electricalConsumerBinding As New BindingList(Of IElectricalConsumer)


#End Region

    Private Function ValidateAuxFileName(filename As String) As Boolean

        Dim message As String = String.Empty

        If Not FilePathUtils.ValidateFilePath(filename, ".aaux", message) Then
            MessageBox.Show(message)
        End If

        Return True
    End Function

    'Constructor
    Public Sub New(ByVal fileName As String, ByVal vectoFileName As String)


        If Not ValidateAuxFileName(fileName) Then
            Me.DialogResult = Windows.Forms.DialogResult.Abort
            Me.Close()
        End If


        Me.vectoFile = vectoFileName
        Me.aauxPath = Path.GetDirectoryName(Path.Combine(FilePathUtils.filePathOnly(vectoFileName), fileName))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        auxFile = Path.Combine(FilePathUtils.filePathOnly(vectoFileName), fileName)

        Try

            auxConfig = BusAuxiliaryInputData.ReadBusAuxiliaries(auxFile, Nothing) ' New AuxiliaryConfig(auxFile)
            originalConfig = BusAuxiliaryInputData.ReadBusAuxiliaries(auxFile, Nothing) 

        Catch ex As Exception

            MessageBox.Show("The filename you supplied {0} was invalid or could not be found ", fileName)
            Me.DialogResult = Windows.Forms.DialogResult.Abort
            Me.Close()

        End Try
    End Sub

    'Validation

#Region "Validation Helpers"

    Public Function IsPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Single.TryParse(test, number) Then Return False

        If number <= 0 Then Return False


        Return True
    End Function

    Public Function IsZeroOrPostiveNumber(ByVal test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Single.TryParse(test, number) Then Return False

        If number < 0 Then Return False


        Return True
    End Function

    Public Function IsNumberBetweenZeroandOne(test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        Dim number As Single

        If Not Single.TryParse(test, number) Then Return False

        If number < 0 OrElse number > 1 Then Return False

        Return True
    End Function

    Public Function IsIntegerZeroOrPositiveNumber(test As String) As Boolean

        'Is this numeric sanity check.
        If Not IsNumeric(test) Then Return False

        'if not integer then return false

        Dim number As Integer

        If Not Integer.TryParse(test, number) Then Return False

        If number < 0 Then Return False

        Return True
    End Function

#End Region

#Region "Validation Control"


    '****** PNEUMATIC VALIDATION
    Public Sub Validating_PneumaticHandler(sender As Object, e As CancelEventArgs) _
        Handles txtAdBlueNIperMinute.Validating, txtBrakingWithRetarderNIperKG.Validating,
                txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating,
                txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating,
                txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating,
                txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating,
                txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating,
                txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating,
                txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating,
                cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating

        e.Cancel = Not Validate_Pneumatics()
    End Sub

    Public Function Validate_Pneumatics() As Boolean

        Dim result As Boolean = True

        'PNEUMATIC AUXILLARIES PART
        '***************************


        'AdBlue NI per Minute : txtAdBlueNIperMinute
        If Not IsZeroOrPostiveNumber(txtAdBlueNIperMinute.Text) Then
            ErrorProvider.SetError(txtAdBlueNIperMinute, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtAdBlueNIperMinute, String.Empty)
        End If

        'Overrun Utilisation For Compression Fraction : txtOverrunUtilisationForCompressionFraction
        If Not IsNumberBetweenZeroandOne(txtOverrunUtilisationForCompressionFraction.Text) Then
            ErrorProvider.SetError(txtOverrunUtilisationForCompressionFraction, "Please provide a non negative between 0 and 1.")
            result = False
        Else
            ErrorProvider.SetError(txtOverrunUtilisationForCompressionFraction, String.Empty)
        End If

        'Braking With Retarder NI per KG : txtBrakingWithRetarderNIperKG
        If Not IsZeroOrPostiveNumber(txtBrakingWithRetarderNIperKG.Text) Then
            ErrorProvider.SetError(txtBrakingWithRetarderNIperKG, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtBrakingWithRetarderNIperKG, String.Empty)
        End If

        'Braking No Retarder NI per KG : txtBrakingNoRetarderNIperKG
        If Not IsZeroOrPostiveNumber(txtBrakingNoRetarderNIperKG.Text) Then
            ErrorProvider.SetError(txtBrakingNoRetarderNIperKG, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtBrakingNoRetarderNIperKG, String.Empty)
        End If

        'Breaking Per Kneeling NI per KG in MM : txtBreakingPerKneelingNIperKGinMM
        If Not IsZeroOrPostiveNumber(txtBreakingPerKneelingNIperKGinMM.Text) Then
            ErrorProvider.SetError(txtBreakingPerKneelingNIperKGinMM, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtBreakingPerKneelingNIperKGinMM, String.Empty)
        End If

        'Per Door Opening NI : txtPerDoorOpeningNI
        If Not IsZeroOrPostiveNumber(txtPerDoorOpeningNI.Text) Then
            ErrorProvider.SetError(txtPerDoorOpeningNI, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtPerDoorOpeningNI, String.Empty)
        End If

        'Per Stop Brake Actuation NI per KG : txtPerStopBrakeActuationNIperKG
        If Not IsZeroOrPostiveNumber(txtPerStopBrakeActuationNIperKG.Text) Then
            ErrorProvider.SetError(txtPerStopBrakeActuationNIperKG, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtPerStopBrakeActuationNIperKG, String.Empty)
        End If

        'Air Controlled Suspension NI per Minute : txtAirControlledSuspensionNIperMinute
        If Not IsZeroOrPostiveNumber(txtAirControlledSuspensionNIperMinute.Text) Then
            ErrorProvider.SetError(txtAirControlledSuspensionNIperMinute, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtAirControlledSuspensionNIperMinute, String.Empty)
        End If

        'Non Smart Regen Fraction Total Air Demand : txtNonSmartRegenFractionTotalAirDemand
        If Not IsZeroOrPostiveNumber(txtNonSmartRegenFractionTotalAirDemand.Text) Then
            ErrorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand, String.Empty)
        End If

        'Smart Regen Fraction Total Air Demand : txtSmartRegenFractionTotalAirDemand
        If Not IsNumberBetweenZeroandOne(txtSmartRegenFractionTotalAirDemand.Text) Then
            ErrorProvider.SetError(txtSmartRegenFractionTotalAirDemand, "Please provide a non negative between 0 and 1.")
            result = False
        Else
            ErrorProvider.SetError(txtSmartRegenFractionTotalAirDemand, String.Empty)
        End If


        'Dead Volume Litres : txtDeadVolumeLitres
        If Not IsZeroOrPostiveNumber(txtDeadVolumeLitres.Text) Then
            ErrorProvider.SetError(txtDeadVolumeLitres, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtDeadVolumeLitres, String.Empty)
        End If


        'Dead Vol BlowOuts Per Litresper Hour : txtDeadVolBlowOutsPerLitresperHour
        If Not IsZeroOrPostiveNumber(txtDeadVolBlowOutsPerLitresperHour.Text) Then
            ErrorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour, String.Empty)
        End If


        'USER CONFIG PART 
        '*****************************************************************************************
        'Compressor Map path : txtCompressorMap
        'Test for empty after trim
        If txtCompressorMap.Text.Trim.Length = 0 Then
            ErrorProvider.SetError(txtCompressorMap, "Please enter the localtion of a valid compressor map.")
            result = False
        Else
            ErrorProvider.SetError(txtCompressorMap, String.Empty)
        End If
        'Test File is valid
        Dim comp As ICompressorMap
        Try

            comp = CompressorMapReader.ReadFile(FilePathUtils.ResolveFilePath(aauxPath, txtCompressorMap.Text), 1.0, "")
            'comp.Initialise()
            ErrorProvider.SetError(txtCompressorMap, String.Empty)
        Catch ex As Exception
            ErrorProvider.SetError(txtCompressorMap,
                                    "Error : map is invalid or cannot be found, please select a Cvalid compressor map")
            result = False
        End Try

        'Compressor Gear Efficiency : txtCompressorGearEfficiency"
        If Not IsNumberBetweenZeroandOne(txtCompressorGearEfficiency.Text) Then
            ErrorProvider.SetError(txtCompressorGearEfficiency, "Please enter a number between 0 and 1")
            result = False
        Else
            ErrorProvider.SetError(txtCompressorGearEfficiency, String.Empty)
        End If

        'Compressor Gear Ratio : txtCompressorGearRatio
        If Not IsPostiveNumber(txtCompressorGearRatio.Text) Then
            ErrorProvider.SetError(txtCompressorGearRatio, "Please enter a number greater than 0.")
            result = False
        Else
            ErrorProvider.SetError(txtCompressorGearRatio, String.Empty)
        End If


        'Actuations Map : txtActuationsMap
        If txtActuationsMap.Text.Trim.Length = 0 Then
            ErrorProvider.SetError(txtActuationsMap, "Please enter the localtion of a valid Pneumatic Actuations map.")
            result = False
        Else
            ErrorProvider.SetError(txtActuationsMap, String.Empty)
        End If
        'Test File is valid
        Dim actuations As IActuationsMap
        Try

            actuations = ActuationsMapReader.Read(FilePathUtils.ResolveFilePath(aauxPath, txtActuationsMap.Text))
            'actuations.Initialise()
            ErrorProvider.SetError(txtActuationsMap, String.Empty)
        Catch ex As Exception
            ErrorProvider.SetError(txtActuationsMap,
                                    "Error : Pneumatic Actuations map is invalid or cannot be found, please select a valid map")
            result = False
        End Try


        'NOT Required but included here so readers can see this is a positive ommission
        '******************************************************************************
        'Smart Air Compression : chkSmartAirCompression
        'Smart Regeneration : chkSmartRegeneration
        'Retarder Brake : chkRetarderBrake

        'txtKneelingHeightMillimeters : txtKneelingHeightMillimeters
        If Not IsZeroOrPostiveNumber(txtKneelingHeightMillimeters.Text) Then
            ErrorProvider.SetError(txtKneelingHeightMillimeters, "Please enter a number greater than 0.")
            result = False
        Else
            ErrorProvider.SetError(txtKneelingHeightMillimeters, String.Empty)
        End If

        'cboAirSuspensionControl : cboAirSuspensionControl
        If cboAirSuspensionControl.SelectedIndex < 1 Then
            ErrorProvider.SetError(cboAirSuspensionControl, "Please make a selection.")
            result = False
        Else
            ErrorProvider.SetError(cboAirSuspensionControl, String.Empty)
        End If

        'cboAdBlueDosing : cboAdBlueDosing
        If cboAdBlueDosing.SelectedIndex < 1 Then
            ErrorProvider.SetError(cboAdBlueDosing, "Please make a selection.")
            result = False
        Else
            ErrorProvider.SetError(cboAdBlueDosing, String.Empty)
        End If

        'cboDoors : cboDoors
        If cboDoors.SelectedIndex < 1 Then
            ErrorProvider.SetError(cboDoors, "Please make a selection.")
            result = False
        Else
            ErrorProvider.SetError(cboDoors, String.Empty)
        End If


        'Set Tab Color

        UpdateTabStatus("tabPneumaticConfig", result)


        Return result
    End Function

    '*****  ELECTRICAL VALIDATION
    Public Sub Validating_ElectricsHandler(sender As Object, e As CancelEventArgs) _
        Handles txtPowernetVoltage.Validating, txtAlternatorMapPath.Validating, txtAlternatorGearEfficiency.Validating,
                txtDoorActuationTimeSeconds.Validating, txtStoredEnergyEfficiency.Validating

        e.Cancel = Not Validate_Electrics()
    End Sub

    Public Function Validate_Electrics() As Boolean

        Dim result As Boolean = True


        'Powernet Voltage : txtPowernetVoltage
        If Not IsPostiveNumber(txtPowernetVoltage.Text) Then
            ErrorProvider.SetError(txtPowernetVoltage, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtPowernetVoltage, String.Empty)
        End If


        'Alternator Map  path : txtAlternatorMapPath
        'Test for empty after trim
        If txtAlternatorMapPath.Text.Trim.Length = 0 Then
            ErrorProvider.SetError(txtAlternatorMapPath, "Please enter the localtion of a valid alternator map.")
            result = False
        Else
            ErrorProvider.SetError(txtAlternatorMapPath, String.Empty)
        End If

        'Test File is valid
        Try
            Dim alt As IAlternatorMap = AlternatorReader.ReadMap(FilePathUtils.ResolveFilePath(aauxPath, txtAlternatorMapPath.Text))
            ErrorProvider.SetError(txtAlternatorMapPath, String.Empty)
        Catch ex As Exception
            ErrorProvider.SetError(txtAlternatorMapPath,
                                    "Error : map is invalid or cannot be found, please select a valid alternator map")
            result = False
        End Try


        'Alternator Gear Efficiency : txtAlternatorGearEfficiency
        If Not IsNumberBetweenZeroandOne(txtAlternatorGearEfficiency.Text) Then
            ErrorProvider.SetError(txtAlternatorGearEfficiency, "Please enter a number between 0 an 1")
            result = False
        Else
            ErrorProvider.SetError(txtAlternatorGearEfficiency, String.Empty)
        End If


        'Door Action Time : txtDoorActuationTimeSeconds
        If Not IsPostiveNumber(txtDoorActuationTimeSeconds.Text) Then
            ErrorProvider.SetError(txtDoorActuationTimeSeconds, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtDoorActuationTimeSeconds, String.Empty)
        End If

        'Stored Energy Efficiency : txtStoredEnergyEfficiency
        If Not IsPostiveNumber(txtStoredEnergyEfficiency.Text) Then
            ErrorProvider.SetError(txtStoredEnergyEfficiency, "Please provide a non negative number.")
            result = False
        Else
            ErrorProvider.SetError(txtStoredEnergyEfficiency, String.Empty)
        End If


        UpdateTabStatus("tabElectricalConfig", result)


        Return result
    End Function

    '****** HVAC VALIDATION
    Public Sub Validating_HVACHandler(sender As Object, e As CancelEventArgs) _
        Handles txtSSMFilePath.Validating, txtBusDatabaseFilePath.Validating

        e.Cancel = Not Validate_HVAC()
    End Sub

    Public Function Validate_HVAC() As Boolean

        Dim result As Boolean = True
        Dim message As String = ""

        'Validate abdb -  Bus Database 
        'Dim abdbFile As String = FilePathUtils.ResolveFilePath(aauxPath, txtBusDatabaseFilePath.Text)
        'Dim bdb As New BusDatabase()
        'If bdb.Initialise(abdbFile) Then
        '    ErrorProvider.SetError(txtBusDatabaseFilePath, String.Empty)
        'Else
        '    result = False
        '    ErrorProvider.SetError(Me.txtBusDatabaseFilePath, "Please choose a valid Steady State Model File (*.ABDB")
        'End If


        'Try ahsm - HVac Steady State Model
        Try

            Dim ahsmFile As String = FilePathUtils.ResolveFilePath(aauxPath, txtSSMFilePath.Text)
            Dim ssmTool As SSMTOOL = New SSMTOOL(SSMInputData.ReadFile(ahsmFile, Nothing, Nothing)) ', New HVACConstants, False)

            'If ssmTool.Load(ahsmFile) Then
            '    ErrorProvider.SetError(txtSSMFilePath, String.Empty)
            'Else
            '    result = False
            '    ErrorProvider.SetError(txtSSMFilePath, "Please choose a valid Steady State Model File (*.AHSM")
            'End If

        Catch ex As Exception
            'Just in case
            ErrorProvider.SetError(txtSSMFilePath, "Please choose a valid Steady State Model File (*.AHSM")
            result = False

        End Try


        UpdateTabStatus("tabHVACConfig", result)

        Return result
    End Function


    Public Function ValidateAll() As Boolean

        If Validate_Pneumatics() = False Or Validate_Electrics() = False Or Validate_HVAC() = False Then
            Return False
        End If

        Return True
    End Function

    '*****  IMPUTS VALIDATION

#End Region

    'Form Controls & Events
    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Required for OwnerDraw, this is required in order to color the tabs when a validation error occurs to draw
        'The attention of the user to the fact that attention is required on a particlar tab.
        TabColors.Add(tabGeneralConfig, Control.DefaultBackColor)
        TabColors.Add(tabElectricalConfig, Control.DefaultBackColor)
        TabColors.Add(tabPneumaticConfig, Control.DefaultBackColor)
        TabColors.Add(tabHVACConfig, Control.DefaultBackColor)


        'This is here only for testing purposes, the actual cycle will be a result of Vecto input.
        cboCycle.SelectedIndex = 0

        'General Setup of all controls 
        SetupControls()

        'Binding Values in Aux environment to the input controls on relevent tabs in the form.
        CreateBindings()

        'This function is neccesary because binding does not occur when the page is invisible, so a track across all of them
        'Is required in order to set the binding. This only needs to be done once at at setup time. after values are set in the
        'Aux environment either by setting defaults of reading and setting from the Auxillaries persistance file.
        EnsureBinding()


        'Additional atatched events
        'For Tab Coloring, this is the place where the background will get filled on the tab when attention is required.
        AddHandler tabMain.DrawItem, New System.Windows.Forms.DrawItemEventHandler(AddressOf tabMain_DrawItem)

        'Select Electric Tab
        tabMain.SelectTab(tabMain.TabPages("tabElectricalConfig"))


        'Enabled  / Disables Smart Cards based on chkSmartElectrical
        SetSmartCardEmabledStatus()

        'Merge Info data from ElectricalConsumer in a Default set into live set
        'This is required because the info is stored in the AAUX file and we do not want to use a persistance stored version.
        'auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.MergeInfoData()
    End Sub

    Private Sub frmAuxiliaryConfig_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing


        If Me.DialogResult = Windows.Forms.DialogResult.Cancel Then Return

        Dim result As DialogResult

        If Not File.Exists(FilePathUtils.ResolveFilePath(aauxPath, auxFile)) OrElse
            Not auxConfig.ConfigValuesAreTheSameAs(originalConfig) Then

            result =
                (MessageBox.Show("Would you like to save changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question))

            Select Case result

                Case DialogResult.Yes
                    'save 
                    If Not SaveFile() Then
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
        Else
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            e.Cancel = False
        End If
    End Sub

#Region "GridHandlers"

    Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) _
        Handles gvElectricalConsumables.CellValidating

        Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
        Dim s As Single


        If e.ColumnIndex = -1 Then

            e.Cancel = True
            Exit Sub

        End If

        If column.ReadOnly Then Return

        Select Case column.Name

            Case "NominalConsumptionAmps"
                If Not IsNumeric(e.FormattedValue) Then
                    MessageBox.Show("This value must be numeric")
                    e.Cancel = True
                End If

            Case "NumberInActualVehicle"
                If Not IsNumeric(e.FormattedValue) Then
                    MessageBox.Show("This value must be numeric")
                    e.Cancel = True
                Else
                    s = Single.Parse(e.FormattedValue.ToString())
                End If
                If s Mod 1 > 0 OrElse s < 0 Then
                    MessageBox.Show("This value must be a positive whole number ( Integer ) ")
                    e.Cancel = True
                End If

                'Veh Electronics &Engine
                If e.RowIndex = 1 Then
                    If Not IsNumeric(e.FormattedValue) Then
                        MessageBox.Show("This value must be numeric")
                        e.Cancel = True
                    End If

                    If chkSmartElectricals.Checked AndAlso s <> 0 Then
                        MessageBox.Show("This must be set to 0 in smart mode")
                        e.Cancel = True
                    ElseIf Not chkSmartElectricals.Checked AndAlso s <> 1 Then
                        MessageBox.Show("This must be set to 1 in classic mode")
                        e.Cancel = True
                    End If
                End If

                'Exterior Bulb
                If e.RowIndex = 14 Then
                    If Not IsNumeric(e.FormattedValue) Then
                        MessageBox.Show("This value must be numeric")
                        e.Cancel = True
                    End If
                    If s <> 1 Then
                        MessageBox.Show("This must be set 1")
                        e.Cancel = True
                    End If
                End If

                'Bonus Bulbs
                If e.RowIndex >= 15 AndAlso e.RowIndex <= 19 Then

                    If Not IsNumeric(e.FormattedValue) Then
                        MessageBox.Show("This value must be numeric")
                        e.Cancel = True
                    End If
                    If s <> 0 AndAlso s <> 1 Then
                        MessageBox.Show("This must be set to 0 or 1")
                        e.Cancel = True
                    End If

                End If


            Case "PhaseIdle_TractionOn"
                If Not IsNumeric(e.FormattedValue) Then
                    MessageBox.Show("This value must be numeric")
                    e.Cancel = True
                Else
                    s = Single.Parse(e.FormattedValue.ToString())
                End If
                If s < 0 OrElse s > 1 Then
                    MessageBox.Show("This must be a value between 0 and 1 ")
                    e.Cancel = True
                End If


        End Select
    End Sub

    Private Sub SmartResult_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) _
        Handles gvResultsCardIdle.CellValidating, gvResultsCardTraction.CellValidating, gvResultsCardOverrun.CellValidating

        Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)

        If Not IsNumeric(e.FormattedValue) Then
            MessageBox.Show("This value must be numeric")
            e.Cancel = True
        End If
    End Sub

    Private Sub resultCard_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) _
        Handles gvResultsCardIdle.CellMouseUp, gvResultsCardTraction.CellMouseUp, gvResultsCardOverrun.CellMouseUp

        Dim dgv As DataGridView = CType(sender, DataGridView)


        If e.Button = MouseButtons.Right Then

            resultCardContextMenu.Show(dgv, e.Location)
            resultCardContextMenu.Show(Cursor.Position)

        End If
    End Sub

    Private Sub resultCardContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) _
        Handles resultCardContextMenu.ItemClicked

        Dim menu As ContextMenuStrip = CType(sender, ContextMenuStrip)

        Dim grid As DataGridView = DirectCast(menu.SourceControl, DataGridView)

        Select Case e.ClickedItem.Text


            Case "Delete"

                For Each selectedRow As DataGridViewRow In grid.SelectedRows

                    If Not selectedRow.IsNewRow Then

                        grid.Rows.RemoveAt(selectedRow.Index)

                    End If

                Next

            Case "Insert"


        End Select
    End Sub

    Private Sub gvElectricalConsumables_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
        Handles gvElectricalConsumables.CellFormatting
    End Sub

    Private Sub gvElectricalConsumables_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) _
        Handles gvElectricalConsumables.CellBeginEdit

        If e.ColumnIndex = 4 AndAlso e.RowIndex = 0 Then

            MessageBox.Show("This cell is calculated and cannot be edited")
            e.Cancel = True

        End If
    End Sub

#End Region

#Region "Button Handlers"

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click


        If SaveFile() Then

            'originalConfig.AssumeValuesOfOther(auxConfig)
            Me.Close()


        End If
    End Sub

    Private Function SaveFile() As Boolean

        Dim result As Boolean

        If Not ValidateAll() Then
            If _
                (MessageBox.Show("There are invalid values. Do you want so save anyway?", "Save Changes", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) = DialogResult.No) Then
                Return False
            End If
        End If

        result = BusAuxWriter.SaveAuxConfig(auxConfig, FilePathUtils.ResolveFilePath(aauxPath, auxFile)) ' auxConfig.Save(FilePathUtils.ResolveFilePath(aauxPath, auxFile))

        If Not result Then MessageBox.Show($"Unable to Save the file '{auxFile}'")

        Return result
    End Function

    Private Function LoadFile() As Boolean

        'JSON METHOD
        Dim result As Boolean

        'Release existing databindings
        UnbindAllControls(Me)

        auxConfig = BusAuxiliaryInputData.ReadBusAuxiliaries(FilePathUtils.ResolveFilePath(aauxPath, auxFile), Nothing)

        'If Not result Then
        '    MessageBox.Show(String.Format("Unable to load the file '{0}'", auxFile))
        'Else
            CreateBindings()
        'End If

        Return result
    End Function

    Private Sub btnFuelMap_Click(sender As Object, e As EventArgs) Handles btnFuelMap.Click

        Dim fbAux As New FileBrowser("AAUXFuelMap", False, False)


        ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
        Dim fname As String = Path.GetFileName(vectoFile) ' fFILE(vectoFile, True)

        fbAux.Extensions = New String() {"vmap"}
        '  If fbAux.OpenDialog(fFileRepl(fname, fPATH(VECTOfile))) Then 
        If fbAux.OpenDialog(Path.GetDirectoryName(vectoFile)) Then

            txtFuelMap.Text = GetRelativePath(fbAux.Files(0), path.GetDirectoryName(vectoFile))

        End If
    End Sub

    Private Sub btnAlternatorMapPath_Click(sender As Object, e As EventArgs) Handles btnAlternatorMapPath.Click

        Dim fbAux As New FileBrowser("AAUXALT", False, False)
        fbAux.Extensions = New String() {"AALT"}

        Dim suppliedAALTPath As String = txtAlternatorMapPath.Text
        Dim absoluteAALTPath As String = FilePathUtils.ResolveFilePath(aauxPath, suppliedAALTPath)
        Dim message As String = String.Empty
        Dim newFile As Boolean = False

        Dim validAALTFile As Boolean = FilePathUtils.ValidateFilePath(absoluteAALTPath, ".aalt", message)
        Dim fileExists As Boolean = File.Exists(absoluteAALTPath)

        If suppliedAALTPath.Length > 0 AndAlso Not validAALTFile Then
            MessageBox.Show(message)
            Return
        End If

        'If file Exists, Check validity, else fire up a default SSM Config.
        If fileExists Then
            Try
                Dim aaltFile As String = FilePathUtils.ResolveFilePath(aauxPath, absoluteAALTPath)
                Dim combinedAlt As IAlternatorMap =  AlternatorReader.ReadMap(aaltFile)
            Catch ex As Exception
                MessageBox.Show("The supplied .AALT File was invalid, aborting.")
                Return
            End Try
        End If

        If Not fileExists Then

            Dim needToFindOrCreateFile As Boolean = True
            While needToFindOrCreateFile

                'Find / Create  file and configure.
                If fbAux.CustomDialog(absoluteAALTPath, False, False, FileBrowserFileExtensionMode.ForceExt, False, String.Empty) Then
                    txtAlternatorMapPath.Text = GetRelativePath(fbAux.Files(0), Path.GetDirectoryName(vectoFile))
                    suppliedAALTPath = txtAlternatorMapPath.Text
                    absoluteAALTPath = FilePathUtils.ResolveFilePath(path.GetDirectoryName(vectoFile), suppliedAALTPath)

                    If _
                        IO.File.Exists(absoluteAALTPath) OrElse
                        MsgBox("Do you want to create a new .AALT file?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        needToFindOrCreateFile = False
                        newFile = True
                    End If
                Else
                    needToFindOrCreateFile = False
                End If

            End While

        End If

        If fileExists OrElse newFile Then

            Using frm As New frmCombinedAlternators(absoluteAALTPath)
                'If Dialog result is OK, then take action else bail
                If frm.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    If suppliedAALTPath.Contains(":\") AndAlso Not String.IsNullOrEmpty(aauxPath) Then
                        txtAlternatorMapPath.Text =
                            If(suppliedAALTPath.Contains(aauxPath), suppliedAALTPath.Replace(aauxPath, ""), suppliedAALTPath)
                    Else
                        txtAlternatorMapPath.Text = path.GetFileName(suppliedAALTPath)
                    End If
                Else
                    Return
                End If
            End Using
            BindingContext(auxConfig.ElectricalUserInputsConfig).EndCurrentEdit()
            Validate_Electrics()

        End If
    End Sub

    Private Sub btnCompressorMap_Click(sender As Object, e As EventArgs) Handles btnCompressorMap.Click


        Dim fbAux As New FileBrowser("AAUXComp", False, False)


        ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
        Dim fname As String = path.GetFileName(vectoFile)

        fbAux.Extensions = New String() {"ACMP"}
        If fbAux.OpenDialog(Path.Combine(aauxPath, txtCompressorMap.Text)) Then

            txtCompressorMap.Text = GetRelativePath(fbAux.Files(0), aauxPath)

        End If

        Validate_Pneumatics()

        'Causes binding to fire
        txtCompressorMap.Focus()
    End Sub

    Private Sub btnActuationsMap_BindingContextChanged(sender As Object, e As EventArgs) _
        Handles btnActuationsMap.BindingContextChanged
    End Sub

    Private Sub btnActuationsMap_Click(sender As Object, e As EventArgs) Handles btnActuationsMap.Click

        Dim fbAux As New FileBrowser("AAUXPneuAct", False, False)

        ' Dim vectoFile As String = "C:\Users\tb28\Source\Workspaces\VECTO\AuxillaryTestHarness\bin\Debug\vectopath.vecto"
        Dim fname As String = path.GetFileName(vectoFile)

        fbAux.Extensions = New String() {"APAC"}
        If fbAux.OpenDialog(Path.Combine(aauxPath, txtActuationsMap.Text)) Then

            txtActuationsMap.Text = GetRelativePath(fbAux.Files(0), aauxPath)

        End If

        Validate_Pneumatics()

        'Causes Binding to fire.
        txtActuationsMap.Focus()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click


        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnBusDatabaseSource_Click(sender As Object, e As EventArgs) Handles btnBusDatabaseSource.Click

        Dim fbAux As New FileBrowser("AAUXBusDB", False, False)
        Dim message As String = String.Empty


        fbAux.Extensions = New String() {"abdb"}

        If fbAux.OpenDialog(Path.Combine(Path.GetDirectoryName(auxFile), txtBusDatabaseFilePath.Text)) Then

            txtBusDatabaseFilePath.Focus()
            txtBusDatabaseFilePath.Text = GetRelativePath(fbAux.Files(0), Path.GetDirectoryName(auxFile))

            'Dim busDB As New BusDatabase()

            'If Not busDB.Initialise(FilePathUtils.ResolveFilePath(aauxPath, txtBusDatabaseFilePath.Text)) Then

            '    MessageBox.Show("Unable to load")

            'End If

            Validate_HVAC()
        End If
    End Sub

    Private Sub btnSSMBSource_BindingContextChanged(sender As Object, e As EventArgs) _
        Handles btnSSMBSource.BindingContextChanged
    End Sub

    Private Sub btnSSMBSource_Click(sender As Object, e As EventArgs) Handles btnSSMBSource.Click

        Dim fbAux As New FileBrowser("AAUXSSM", False, False)
        fbAux.Extensions = New String() {"AHSM"}

        Dim suppliedSSMPath As String = txtSSMFilePath.Text.Trim()
        Dim absoluteSSMPath As String = FilePathUtils.ResolveFilePath(aauxPath, suppliedSSMPath)
        Dim absoluteBusDatabasePath As String = FilePathUtils.ResolveFilePath(aauxPath,
                                                                            Me.txtBusDatabaseFilePath.Text.Trim())
        Dim message As String = String.Empty
        Dim newFile As Boolean = False

        Dim validSSMTFile As Boolean = FilePathUtils.ValidateFilePath(absoluteSSMPath, ".ahsm", message)
        Dim fileExists As Boolean = File.Exists(absoluteSSMPath)

        If suppliedSSMPath.Length > 0 AndAlso Not validSSMTFile Then
            MessageBox.Show(message)
            Return
        End If

        'If file Exists, Check validity, else fire up a default SSM Config.
        If File.Exists(absoluteSSMPath) Then
            'is file valid Try ahsm - HVac Steady State Model
            Try
                Dim ahsmFile As String = FilePathUtils.ResolveFilePath(aauxPath, absoluteSSMPath)
                Dim ssmTool As SSMTOOL = new SSMTOOL(SSMInputData.ReadFile(ahsmFile, Nothing, Nothing)) ' New SSMTOOL(ahsmFile, New HVACConstants, False)
                'ssmTool.Load(ahsmFile)
            Catch ex As Exception
                MessageBox.Show("The supplied AHSM File was invalid, aborting.")
                Return
            End Try
        End If

        If Not fileExists Then

            Dim needToFindOrCreateFile As Boolean = True
            While needToFindOrCreateFile

                'Find / Create  file and configure.
                If fbAux.CustomDialog(absoluteSSMPath, False, False, FileBrowserFileExtensionMode.ForceExt, False, String.Empty) Then
                    txtSSMFilePath.Text = GetRelativePath(fbAux.Files(0), aauxPath)
                    suppliedSSMPath = txtSSMFilePath.Text
                    absoluteSSMPath = FilePathUtils.ResolveFilePath(aauxPath, suppliedSSMPath)
                    If _
                        IO.File.Exists(absoluteSSMPath) OrElse
                        MsgBox("Do you want to create a new .AHSM file?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                        needToFindOrCreateFile = False
                        newFile = True
                    End If
                Else
                    needToFindOrCreateFile = False
                End If

            End While

        End If

        If fileExists OrElse newFile Then

            Using frm As New frmHVACTool(absoluteBusDatabasePath, absoluteSSMPath, vectoFile, Not fileExists)
                If frm.ShowDialog() = Windows.Forms.DialogResult.OK Then
                    If suppliedSSMPath.Contains(":\") AndAlso Not String.IsNullOrEmpty(aauxPath) Then
                        txtSSMFilePath.Text =
                            If(suppliedSSMPath.Contains(aauxPath), suppliedSSMPath.Replace(aauxPath, ""), suppliedSSMPath)
                    Else
                        txtSSMFilePath.Text = path.GetFileName(suppliedSSMPath)
                    End If
                Else
                    Return
                End If
            End Using
            'BindingContext(auxConfig.HvacUserInputsConfig).EndCurrentEdit()
            Validate_HVAC()

        End If
    End Sub

#End Region

    Private Sub chkSmartElectricals_CheckedChanged(sender As Object, e As EventArgs) _
        Handles chkSmartElectricals.CheckedChanged

        SetSmartCardEmabledStatus()
    End Sub

#Region "File Viewer Button Events"

    Private Sub btnAALTOpen_Click(sender As Object, e As EventArgs) Handles btnAALTOpen.Click

        OpenFiles(GetRelativePath(Me.txtAlternatorMapPath.Text, Path.GetDirectoryName(vectoFile)))
    End Sub

    Private Sub btnOpenACMP_Click(sender As Object, e As EventArgs) Handles btnOpenACMP.Click


        OpenFiles(GetRelativePath(Me.txtCompressorMap.Text, Path.GetDirectoryName(vectoFile)))
    End Sub

    Private Sub btnOpenAPAC_Click(sender As Object, e As EventArgs) Handles btnOpenAPAC.Click

        OpenFiles(GetRelativePath(Me.txtActuationsMap.Text, Path.GetDirectoryName(vectoFile)))
    End Sub

    Private Sub btnOpenAHSM_Click(sender As Object, e As EventArgs) Handles btnOpenAHSM.Click

        OpenFiles(GetRelativePath(Me.txtSSMFilePath.Text, Path.GetDirectoryName(vectoFile)))
    End Sub

    Private Sub btnOpenABDB_Click(sender As Object, e As EventArgs) Handles btnOpenABDB.Click


        OpenFiles(GetRelativePath(Me.txtBusDatabaseFilePath.Text, Path.GetDirectoryName(vectoFile)))
    End Sub


#End Region

    'Overrides
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        If keyData = Keys.Enter AndAlso Me.AcceptButton Is Nothing Then

            If TypeOf (Me.ActiveControl) Is TextBoxBase Then
                Dim box As TextBoxBase = CType(Me.ActiveControl, TextBoxBase)

                If box Is Nothing OrElse Not box.Multiline Then

                    Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
                    Return True

                End If
            End If

        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    'Helpers
    Private Sub OpenFiles(ParamArray files() As String)

        If files.Length = 0 Then Exit Sub

        cmFilesList = files

        OpenWithToolStripMenuItem.Text = "Open with notepad"

        CmFiles.Show(Cursor.Position)
    End Sub

    Private Sub OpenWithToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
        Handles OpenWithToolStripMenuItem.Click
        If Not FileOpenAlt(cmFilesList(0)) Then MsgBox("Failed to open file!")
    End Sub

    Private Sub ShowInFolderToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
        Handles ShowInFolderToolStripMenuItem.Click
        If IO.File.Exists(cmFilesList(0)) Then
            Try
                System.Diagnostics.Process.Start("explorer", "/select,""" & cmFilesList(0) & "")
            Catch ex As Exception
                MsgBox("Failed to open file!")
            End Try
        Else
            MsgBox("File not found!")
        End If
    End Sub

#Region "Tab Header Color Change"

    Private Sub UpdateTabStatus(pageName As String, resultGood As Boolean)


        Dim page As TabPage = tabMain.TabPages(pageName)

        If Not resultGood Then

            SetTabHeader(page, Color.Red)

        Else
            SetTabHeader(page, Control.DefaultBackColor)

        End If
    End Sub

    Private Sub SetTabHeader(page As TabPage, color As Color)

        TabColors(page) = color
        tabMain.Invalidate()
    End Sub

    Private Sub tabMain_DrawItem(sender As Object, e As DrawItemEventArgs)

        Dim br As Brush = New SolidBrush(TabColors(tabMain.TabPages(e.Index)))


        Using (br)

            e.Graphics.FillRectangle(br, e.Bounds)
            Dim sz As SizeF = e.Graphics.MeasureString(tabMain.TabPages(e.Index).Text, e.Font)
            e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black,
                                e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1)

            Dim rect As Rectangle = e.Bounds
            rect.Offset(-1, -1)
            rect.Inflate(1, 1)
            ' e.Graphics.DrawRectangle(Pens.DarkGray, rect)
            'e.DrawFocusRectangle()

        End Using
    End Sub


#End Region

    Public Sub UnbindAllControls(ByRef container As Control)
        'Clear all of the controls within the container object
        'If "Recurse" is true, then also clear controls within any sub-containers
        Dim ctrl As Control = Nothing

        For Each ctrl In container.Controls

            ctrl.DataBindings.Clear()

            If ctrl.HasChildren Then
                UnbindAllControls(ctrl)
            End If

        Next
    End Sub

    Private Sub SetupControls()


        Dim cIndex As Integer = 0

        gvElectricalConsumables.AutoGenerateColumns = False

        'ElectricalConsumerGrid 
        'Columns
        cIndex = gvElectricalConsumables.Columns.Add("Category", "Category")
        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "Category"
        gvElectricalConsumables.Columns(cIndex).MinimumWidth = 150
        gvElectricalConsumables.Columns(cIndex).ReadOnly = True
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

        cIndex = gvElectricalConsumables.Columns.Add("ConsumerName", "Name")

        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "ConsumerName"
        gvElectricalConsumables.Columns(cIndex).MinimumWidth = 308
        gvElectricalConsumables.Columns(cIndex).ReadOnly = True
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)

        Dim baseVehicle As New DataGridViewCheckBoxColumn(False)
        baseVehicle.HeaderText = "Base Vehicle"
        cIndex = gvElectricalConsumables.Columns.Add(baseVehicle)
        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "BaseVehicle"
        gvElectricalConsumables.Columns(cIndex).Width = 75
        gvElectricalConsumables.Columns(cIndex).Visible = False
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
        gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Energy included in the calculations of base vehicle"

        cIndex = gvElectricalConsumables.Columns.Add("NominalConsumptionAmps", "Nominal Amps")
        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NominalConsumptionAmps"
        gvElectricalConsumables.Columns(cIndex).Width = 60
        gvElectricalConsumables.Columns(cIndex).ReadOnly = True
        gvElectricalConsumables.Columns(cIndex).DefaultCellStyle = New DataGridViewCellStyle() _
            With {.BackColor = Color.LightGray}
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
        gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Nominal consumption in AMPS"

        cIndex = gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn", "PhaseIdle/ TractionOn")
        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "PhaseIdle_TractionOn"
        gvElectricalConsumables.Columns(cIndex).Width = 60
        gvElectricalConsumables.Columns(cIndex).ReadOnly = True
        gvElectricalConsumables.Columns(cIndex).DefaultCellStyle = New DataGridViewCellStyle() _
            With {.BackColor = Color.LightGray}
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
        gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText =
            "Represents the amount of time (during engine fueling) as " & vbCrLf &
            "percentage that the consumer is active during the cycle."

        cIndex = gvElectricalConsumables.Columns.Add("NumberInActualVehicle", "Num in Vehicle")
        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NumberInActualVehicle"
        gvElectricalConsumables.Columns(cIndex).Width = 55
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
        gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Number of consumables of this" & vbCrLf &
                                                                        "type installed on the vehicle."


        'INFO COLUMN
        cIndex = gvElectricalConsumables.Columns.Add("info", "Info")
        '  cIndex = gvElectricalConsumables.Columns.Add( New ImageColumn())

        gvElectricalConsumables.Columns(cIndex).DataPropertyName = "Info"
        gvElectricalConsumables.Columns(cIndex).Width = 120
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
        gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
        gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Further Information"

        ' ResultCard Grids

        'IDLE

        cIndex = gvResultsCardIdle.Columns.Add("Amps", "Amps")
        gvResultsCardIdle.Columns(cIndex).DataPropertyName = "Amps"
        gvResultsCardIdle.Columns(cIndex).Width = 65

        cIndex = gvResultsCardIdle.Columns.Add("SmartAmps", "SmartAmps")
        gvResultsCardIdle.Columns(cIndex).DataPropertyName = "SmartAmps"
        gvResultsCardIdle.Columns(cIndex).Width = 65

        'TRACTION
        cIndex = gvResultsCardTraction.Columns.Add("Amps", "Amps")
        gvResultsCardTraction.Columns(cIndex).DataPropertyName = "Amps"
        gvResultsCardTraction.Columns(cIndex).Width = 65

        cIndex = gvResultsCardTraction.Columns.Add("SmartAmps", "SmartAmps")
        gvResultsCardTraction.Columns(cIndex).DataPropertyName = "SmartAmps"
        gvResultsCardTraction.Columns(cIndex).Width = 65

        'OVERRUN
        cIndex = gvResultsCardOverrun.Columns.Add("Amps", "Amps")
        gvResultsCardOverrun.Columns(cIndex).DataPropertyName = "Amps"
        gvResultsCardOverrun.Columns(cIndex).Width = 65

        cIndex = gvResultsCardOverrun.Columns.Add("SmartAmps", "SmartAmps")
        gvResultsCardOverrun.Columns(cIndex).DataPropertyName = "SmartAmps"
        gvResultsCardOverrun.Columns(cIndex).Width = 65
    End Sub

#Region "Binding Control"

    Private Sub CreateBindings()

        'auxConfig.Vecto Bindings
        dim bVoltage As Binding = new Binding("Text", auxConfig.ElectricalUserInputsConfig, "PowerNetVoltage")
        AddHandler bVoltage.Parse, new ConvertEventHandler(AddressOf TextToSI(Of Volt))
        AddHandler bVoltage.Format, new ConvertEventHandler(AddressOf SIToText)
        txtPowernetVoltage.DataBindings.Add(bVoltage)
        'txtVehicleWeightKG.DataBindings.Add("Text", auxConfig.VectoInputs, "VehicleWeightKG")
        'cboCycle.DataBindings.Add("Text", auxConfig.VectoInputs, "Cycle")
        'txtFuelMap.DataBindings.Add("Text", auxConfig.VectoInputs, "FuelMap")

        'Electricals General
        txtAlternatorMapPath.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "AlternatorMap")
        txtAlternatorGearEfficiency.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "AlternatorGearEfficiency")
        txtDoorActuationTimeSeconds.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "DoorActuationTimeSecond")
        txtStoredEnergyEfficiency.DataBindings.Add("Text", auxConfig.ElectricalUserInputsConfig, "StoredEnergyEfficiency")
        chkSmartElectricals.DataBindings.Add("Checked", auxConfig.ElectricalUserInputsConfig, "SmartElectrical", False,
                                            DataSourceUpdateMode.OnPropertyChanged)


        'Electrical ConsumablesGrid
        'electricalConsumerBinding =
        '    New BindingList(Of IElectricalConsumer)(CType (auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.Items, List(Of IElectricalConsumer)))
        'gvElectricalConsumables.DataSource = electricalConsumerBinding
        AddHandler gvElectricalConsumables.CellFormatting, New DataGridViewCellFormattingEventHandler(AddressOf SIToText)


        'ResultCards

        'IDLE
        Dim idleBinding As BindingList(Of SmartResult)
        'idleBinding = New BindingList(Of SmartResult)(CType( auxConfig.ElectricalUserInputsConfig.ResultCardIdle.Results, List(Of SmartResult)))
        idleBinding.AllowNew = True
        idleBinding.AllowRemove = True
        gvResultsCardIdle.DataSource = idleBinding
        AddHandler gvResultsCardIdle.CellParsing, new DataGridViewCellParsingEventHandler (AddressOf TextToSI(of Ampere))
        AddHandler gvResultsCardIdle.CellFormatting, new DataGridViewCellFormattingEventHandler(AddressOf SIToText)

        'TRACTION
        Dim tractionBinding As BindingList(Of SmartResult)
        'tractionBinding = New BindingList(Of SmartResult)(CType(auxConfig.ElectricalUserInputsConfig.ResultCardTraction.Results, List(Of SmartResult)))
        tractionBinding.AllowNew = True
        tractionBinding.AllowRemove = True
        gvResultsCardTraction.DataSource = tractionBinding
        AddHandler gvResultsCardTraction.CellParsing, new DataGridViewCellParsingEventHandler (AddressOf TextToSI(of Ampere))
        AddHandler gvResultsCardTraction.CellFormatting, new DataGridViewCellFormattingEventHandler(AddressOf SIToText)

        'OVERRUN
        Dim overrunBinding As BindingList(Of SmartResult)
        'overrunBinding = New BindingList(Of SmartResult)(CType(auxConfig.ElectricalUserInputsConfig.ResultCardOverrun.Results, List(Of SmartResult)))
        overrunBinding.AllowNew = True
        overrunBinding.AllowRemove = True
        gvResultsCardOverrun.DataSource = overrunBinding
        AddHandler gvResultsCardOverrun.CellParsing, new DataGridViewCellParsingEventHandler (AddressOf TextToSI(of Ampere))
        AddHandler gvResultsCardOverrun.CellFormatting, new DataGridViewCellFormattingEventHandler(AddressOf SIToText)


        'Pneumatic Auxillaries Binding
        txtAdBlueNIperMinute.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "AdBlueNIperMinute")

        txtOverrunUtilisationForCompressionFraction.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                                    "OverrunUtilisationForCompressionFraction")
        txtBrakingWithRetarderNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                        "BrakingWithRetarderNIperKG")
        txtBrakingNoRetarderNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "BrakingNoRetarderNIperKG")
        txtBreakingPerKneelingNIperKGinMM.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                            "BreakingPerKneelingNIperKGinMM", True, DataSourceUpdateMode.OnPropertyChanged, Nothing, "0.########")
        txtPerDoorOpeningNI.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "PerDoorOpeningNI")
        txtPerStopBrakeActuationNIperKG.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                        "PerStopBrakeActuationNIperKG")
        txtAirControlledSuspensionNIperMinute.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                                "AirControlledSuspensionNIperMinute")
        txtNonSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                                "NonSmartRegenFractionTotalAirDemand")
        txtSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                            "SmartRegenFractionTotalAirDemand")
        txtDeadVolumeLitres.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig, "DeadVolumeLitres")
        txtDeadVolBlowOutsPerLitresperHour.DataBindings.Add("Text", auxConfig.PneumaticAuxillariesConfig,
                                                            "DeadVolBlowOutsPerLitresperHour")

        'Pneumatic UserInputsConfig Binding    
        txtCompressorMap.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorMap")
        txtCompressorGearEfficiency.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorGearEfficiency")
        txtCompressorGearRatio.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "CompressorGearRatio")
        txtActuationsMap.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "ActuationsMap")
        chkSmartAirCompression.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "SmartAirCompression", False,
                                                DataSourceUpdateMode.OnPropertyChanged)

        chkSmartRegeneration.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "SmartRegeneration", False,
                                            DataSourceUpdateMode.OnPropertyChanged)
        chkRetarderBrake.DataBindings.Add("Checked", auxConfig.PneumaticUserInputsConfig, "RetarderBrake")
        txtKneelingHeightMillimeters.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "KneelingHeightMillimeters")
        cboAirSuspensionControl.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "AirSuspensionControl", False)
        cboAdBlueDosing.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "AdBlueDosing")
        cboDoors.DataBindings.Add("Text", auxConfig.PneumaticUserInputsConfig, "Doors")

        'txtSSMFilePath.DataBindings.Add("Text", auxConfig.HvacUserInputsConfig, "SSMFilePath")
        'txtBusDatabaseFilePath.DataBindings.Add("Text", auxConfig.HvacUserInputsConfig, "BusDatabasePath")
        'chkDisableHVAC.DataBindings.Add("Checked", auxConfig.HvacUserInputsConfig, "SSMDisabled", False,
                                        'DataSourceUpdateMode.OnPropertyChanged)

        SetSmartCardEmabledStatus()
    End Sub

    Private Sub EnsureBinding()
        With tabMain
            Dim lastSelectedTabIndex As Integer = .SelectedIndex
            If lastSelectedTabIndex < 0 OrElse lastSelectedTabIndex > .TabCount Then lastSelectedTabIndex = 0
            For currentTab As Integer = 0 To .TabCount - 1
                .SelectedIndex = currentTab
            Next
            .SelectedIndex = 0
        End With

        SetSmartCardEmabledStatus()
    End Sub

#End Region

    Protected Sub SetSmartCardEmabledStatus()

        If chkSmartElectricals.Checked Then

            gvResultsCardIdle.Enabled = True
            gvResultsCardTraction.Enabled = True
            gvResultsCardOverrun.Enabled = True

            gvResultsCardIdle.BackgroundColor = Color.Gray
            gvResultsCardTraction.BackgroundColor = Color.Gray
            gvResultsCardOverrun.BackgroundColor = Color.Gray

            'electricalConsumerBinding.Single(Function(c) c.Category = "Veh Electronics &Engine").NumberInActualVehicle = 0
        Else

            gvResultsCardIdle.Enabled = False
            gvResultsCardTraction.Enabled = False
            gvResultsCardOverrun.Enabled = False


            gvResultsCardIdle.BackgroundColor = Color.White
            gvResultsCardTraction.BackgroundColor = Color.White
            gvResultsCardOverrun.BackgroundColor = Color.White

            'electricalConsumerBinding.Single(Function(c) c.Category = "Veh Electronics &Engine").NumberInActualVehicle = 1
        End If
    End Sub

    Public Function FileOpenAlt(ByVal file As String) As Boolean
        Dim PSI As New ProcessStartInfo

        If Not IO.File.Exists(file) Then Return False

        PSI.FileName = "notepad.exe"
        PSI.Arguments = ChrW(34) & file & ChrW(34)
        Try
            Process.Start(PSI)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub chkDisableHVAC_CheckedChanged(sender As Object, e As EventArgs) Handles chkDisableHVAC.CheckedChanged

        If chkDisableHVAC.Checked Then

            txtSSMFilePath.ReadOnly = True
            txtBusDatabaseFilePath.ReadOnly = True
            btnSSMBSource.Enabled = False
            btnBusDatabaseSource.Enabled = False
            btnOpenAHSM.Enabled = False
            btnOpenABDB.Enabled = False

        Else

            txtSSMFilePath.ReadOnly = False
            txtBusDatabaseFilePath.ReadOnly = False
            btnSSMBSource.Enabled = True
            btnBusDatabaseSource.Enabled = True
            btnOpenAHSM.Enabled = True
            btnOpenABDB.Enabled = True

        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class