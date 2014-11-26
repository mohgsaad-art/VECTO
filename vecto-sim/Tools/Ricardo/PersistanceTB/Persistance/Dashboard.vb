Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports System.ComponentModel
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Runtime.Serialization
Imports Newtonsoft.Json

Public Class Dashboard

#Region "Fields"

Public auxEnvironment As New AuxillaryEnvironment("")
Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()
Private processing As Boolean = False
Private SecondsIntoCycle As Integer = 0

#End Region



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
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Energy included in the calculations of base vehicle"

     cIndex = gvElectricalConsumables.Columns.Add("NominalConsumptionAmps", "Nominal Amps")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NominalConsumptionAmps"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Nominal consumption in AMPS"

     cIndex = gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn", "PhaseIdle/ TractionOn")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "PhaseIdle_TractionOn"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Represents the amount of time (during engine fueling) as " & vbCrLf & "percentage that the consumer is active during the cycle."

     cIndex = gvElectricalConsumables.Columns.Add("NumberInActualVehicle", "Num in Vehicle")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NumberInActualVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1, 2, 1, 1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText = "Number of consumables of this" & vbCrLf & "type installed on the vehicle."

     'ResultCard Grids

     'Handler for deleting rows.


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

     'AuxEnvironment.Vecto Bindings
     txtPowernetVoltage.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "PowerNetVoltage")
     txtVehicleWeightKG.DataBindings.Add("Text", auxEnvironment.VectoInputs, "VehicleWeightKG")
     cboCycle.DataBindings.Add("Text", auxEnvironment.VectoInputs, "Cycle")
     txtFuelMap.DataBindings.Add("Text", auxEnvironment.VectoInputs, "FuelMap")

     'Electricals General
     txtAlternatorMapPath.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked", auxEnvironment.ElectricalUserInputsConfig, "SmartElectrical", False, DataSourceUpdateMode.OnPropertyChanged)

     'TODO:NOT NEEDED IN NORMAL OPS ONLY FOR TEST HARNESS
     chkSignalsSmartElectrics.DataBindings.Add("Checked", auxEnvironment.Signals, "SmartElectrics", False, DataSourceUpdateMode.OnPropertyChanged)

     'Electrical ConsumablesGrid
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items)
     gvElectricalConsumables.DataSource = electricalConsumerBinding



     'ResultCards

         'IDLE
         Dim idleBinding = New BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardIdle.Results)
         idleBinding.AllowNew = True
         idleBinding.AllowRemove = True
         gvResultsCardIdle.DataSource = idleBinding

         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardTraction.Results)
         tractionBinding.AllowNew = True
         tractionBinding.AllowRemove = True
         gvResultsCardTraction.DataSource = tractionBinding

         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardOverrun.Results)
         overrunBinding.AllowNew = True
         overrunBinding.AllowRemove = True
         gvResultsCardOverrun.DataSource = overrunBinding


        'Pneumatic Auxillaries Binding
        txtAdBlueNIperMinute.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "AdBlueNIperMinute")

        txtOverrunUtilisationForCompressionFraction.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "OverrunUtilisationForCompressionFraction")
        txtBrakingWithRetarderNIperKG.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "BrakingWithRetarderNIperKG")
        txtBrakingNoRetarderNIperKG.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "BrakingNoRetarderNIperKG")
        txtBreakingPerKneelingNIperKGinMM.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "BreakingPerKneelingNIperKGinMM", True, DataSourceUpdateMode.OnPropertyChanged, Nothing, "0.########")
        txtPerDoorOpeningNI.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "PerDoorOpeningNI")
        txtPerStopBrakeActuationNIperKG.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "PerStopBrakeActuationNIperKG")
        txtAirControlledSuspensionNIperMinute.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "AirControlledSuspensionNIperMinute")
        txtNonSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "NonSmartRegenFractionTotalAirDemand")
        txtSmartRegenFractionTotalAirDemand.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "SmartRegenFractionTotalAirDemand")
        txtDeadVolumeLitres.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "DeadVolumeLitres")
        txtDeadVolBlowOutsPerLitresperHour.DataBindings.Add("Text", auxEnvironment.PneumaticAuxillariesConfig, "DeadVolBlowOutsPerLitresperHour")

        'Pneumatic UserInputsConfig Binding    
        txtCompressorMap.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorMap")
        txtCompressorGearEfficiency.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorGearEfficiency")
        txtCompressorGearRatio.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorGearRatio")
        txtActuationsMap.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "ActuationsMap")
        chkSmartAirCompression.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "SmartAirCompression", False, DataSourceUpdateMode.OnPropertyChanged)

        'TODO:NOT NEEDED IN NORMAL OPS ONLY FOR TEST HARNESS     
        chkSignalsSmartAirCompression.DataBindings.Add("Checked", auxEnvironment.Signals, "SmartPneumatics", False, DataSourceUpdateMode.OnPropertyChanged)


        chkSmartRegeneration.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "SmartRegeneration", False, DataSourceUpdateMode.OnPropertyChanged)
        chkRetarderBrake.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "RetarderBrake")
        txtKneelingHeightMillimeters.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "KneelingHeightMillimeters")
        cboAirSuspensionControl.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "AirSuspensionControl", False)
        cboAdBlueDosing.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "AdBlueDosing")
        cboDoors.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "Doors")

        'HVAC Bindings     
        txtHVACElectricalLoadPowerWatts.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel, "HVACElectricalLoadPowerWatts")
        txtHVACFuellingLitresPerHour.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel, "HVACFuellingLitresPerHour")
        txtHVACMechanicalLoadPowerWatts.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel, "HVACMechanicalLoadPowerWatts")

        'Signals
        chkInNeutral.DataBindings.Add("Checked", auxEnvironment.Signals, "InNeutral", False, DataSourceUpdateMode.OnPropertyChanged)
        chkIdle.DataBindings.Add("Checked", auxEnvironment.Signals, "Idle", False, DataSourceUpdateMode.OnPropertyChanged)
        chkClutchEngaged.DataBindings.Add("Checked", auxEnvironment.Signals, "ClutchEngaged", False, DataSourceUpdateMode.OnPropertyChanged)
        txtPreExistingAuxPower.DataBindings.Add("Text", auxEnvironment.Signals, "PreExistingAuxPower")
        txtEngineDrivelinePower.DataBindings.Add("Text", auxEnvironment.Signals, "EngineDrivelinePower")
        txtEngineDrivelineTorque.DataBindings.Add("Text", auxEnvironment.Signals, "EngineDrivelineTorque")
        txtEngineMotoringPower.DataBindings.Add("Text", auxEnvironment.Signals, "EngineMotoringPower")
        txtEngineSpeed.DataBindings.Add("Text", auxEnvironment.Signals, "EngineSpeed")
        txtTotalCycleTimeSeconds.DataBindings.Add("Text", auxEnvironment.Signals, "TotalCycleTimeSeconds")


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
    End Sub


#End Region


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

Public Function IsNumberBetweenZeroandOne(test As String)

     'Is this numeric sanity check.
     If Not IsNumeric(test) Then Return False

     Dim number As Single

     If Not Single.TryParse(test, number) Then Return False

     If number < 0 OrElse number > 1 Then Return False

     Return True

End Function

Public Function IsIntegerZeroOrPositiveNumber(test As String)

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
Public Sub Validating_PneumaticHandler(sender As Object, e As CancelEventArgs) Handles txtAdBlueNIperMinute.Validating, txtBrakingWithRetarderNIperKG.Validating, txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating, txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating, txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating, txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating, txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating, txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating, txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating, cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating

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
         ErrorProvider.SetError(txtDeadVolumeLitres, "Please provide a non negative between 0 and 1.")
         result = False
       Else
         ErrorProvider.SetError(txtDeadVolumeLitres, String.Empty)
       End If


       'Dead Vol BlowOuts Per Litresper Hour : txtDeadVolBlowOutsPerLitresperHour
       If Not IsZeroOrPostiveNumber(txtDeadVolBlowOutsPerLitresperHour.Text) Then
         ErrorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour, "Please provide a non negative between 0 and 1.")
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
        Dim comp As CompressorMap
        Try

        comp = New CompressorMap(txtCompressorMap.Text)
        comp.Initialise()
         ErrorProvider.SetError(txtCompressorMap, String.Empty)
        Catch ex As Exception
         ErrorProvider.SetError(txtCompressorMap, "Error : map is invalid or cannot be found, please select a Cvalid compressor map")
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

        'NOT Required but included here so readers can see this is a positive ommission
        '******************************************************************************
        'Smart Air Compression : chkSmartAirCompression
        'Smart Regeneration : chkSmartRegeneration
        'Retarder Brake : chkRetarderBrake

        'txtKneelingHeightMillimeters : txtKneelingHeightMillimeters
        If Not IsPostiveNumber(txtKneelingHeightMillimeters.Text) Then
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
Public Sub Validating_ElectricsHandler(sender As Object, e As CancelEventArgs) Handles txtPowernetVoltage.Validating, txtAlternatorMapPath.Validating, txtAlternatorGearEfficiency.Validating, txtDoorActuationTimeSeconds.Validating

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
        Dim alt As AlternatorMap
        Try
        alt = New AlternatorMap(txtAlternatorMapPath.Text)
        alt.Initialise()
         ErrorProvider.SetError(txtAlternatorMapPath, String.Empty)
        Catch ex As Exception
         ErrorProvider.SetError(txtAlternatorMapPath, "Error : map is invalid or cannot be found, please select a valid alternator map")
         result = False
        End Try


       'Door Action Time : txtDoorActuationTimeSeconds
       If Not IsPostiveNumber(txtDoorActuationTimeSeconds.Text) Then
         ErrorProvider.SetError(txtDoorActuationTimeSeconds, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtDoorActuationTimeSeconds, String.Empty)
       End If


               UpdateTabStatus("tabElectricalConfig", result)


       Return result


End Function

'****** HVAC VALIDATION
Public Sub Validating_HVACHandler(sender As Object, e As CancelEventArgs) Handles txtHVACMechanicalLoadPowerWatts.Validating, txtHVACFuellingLitresPerHour.Validating, txtHVACElectricalLoadPowerWatts.Validating

    e.Cancel = Not Validate_HVAC()

End Sub
Public Function Validate_HVAC() As Boolean

Dim result As Boolean = True


       'HVAC Electrical Load Power Watts : txtHVACElectricalLoadPowerWatts
       If Not IsZeroOrPostiveNumber(txtHVACElectricalLoadPowerWatts.Text) Then
         ErrorProvider.SetError(txtHVACElectricalLoadPowerWatts, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtHVACElectricalLoadPowerWatts, String.Empty)
       End If

       'HVAC Mechanical Load Power Watts : txtHVACMechanicalLoadPowerWatts
       If Not IsZeroOrPostiveNumber(txtHVACMechanicalLoadPowerWatts.Text) Then
         ErrorProvider.SetError(txtHVACMechanicalLoadPowerWatts, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtHVACMechanicalLoadPowerWatts, String.Empty)
       End If


       'HVAC Fuelling Litres Per Hour : txtHVACFuellingLitresPerHour
       If Not IsZeroOrPostiveNumber(txtHVACFuellingLitresPerHour.Text) Then
         ErrorProvider.SetError(txtHVACFuellingLitresPerHour, "Please provide a non negative number.")
         result = False
       Else
          ErrorProvider.SetError(txtHVACFuellingLitresPerHour, String.Empty)
       End If

       UpdateTabStatus("tabHVACConfig", result)


   Return result


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
  TabColors.Add(tabPlayground, Control.DefaultBackColor)

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


  'Finally Initialise Environment.
  auxEnvironment.Initialise()
  SetProcessingStatus()

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
        e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1)

        Dim rect As Rectangle = e.Bounds
        rect.Offset(-1, -1)
        rect.Inflate(1, 1)
       ' e.Graphics.DrawRectangle(Pens.DarkGray, rect)
        'e.DrawFocusRectangle()

    End Using




End Sub


#End Region

#Region "GridHandlers"

Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating

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
            s = Single.Parse(e.FormattedValue)
           End If
           If s Mod 1 > 0 OrElse s < 0 Then
              MessageBox.Show("This value must be a positive whole number ( Integer ) ")
             e.Cancel = True
           End If


     Case "PhaseIdle_TractionOn"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
             e.Cancel = True
           Else
            s = Single.Parse(e.FormattedValue)
           End If
           If s < 0 OrElse s > 1 Then
              MessageBox.Show("This must be a value between 0 and 1 ")
              e.Cancel = True
           End If


    End Select

End Sub
Private Sub SmartResult_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvResultsCardIdle.CellValidating, gvResultsCardTraction.CellValidating, gvResultsCardOverrun.CellValidating

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)

   If Not IsNumeric(e.FormattedValue) Then
       MessageBox.Show("This value must be numeric")
       e.Cancel = True
   End If

End Sub
Private Sub resultCard_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs)

      Dim dgv As DataGridView = CType(sender, DataGridView)


        If e.Button = MouseButtons.Right Then

            resultCardContextMenu.Show(dgv, e.Location)
            resultCardContextMenu.Show(Cursor.Position)

        End If


    End Sub
Private Sub resultCardContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles resultCardContextMenu.ItemClicked

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
Private Sub gvElectricalConsumables_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles gvElectricalConsumables.CellFormatting


    If e.ColumnIndex = 4 AndAlso e.RowIndex = 0 Then

       e.CellStyle.BackColor = Color.LightGray
       e.CellStyle.ForeColor = Color.LightGray


    End If



End Sub
Private Sub gvElectricalConsumables_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles gvElectricalConsumables.CellBeginEdit

    If e.ColumnIndex = 4 AndAlso e.RowIndex = 0 Then

     MessageBox.Show("This cell is calculated and cannot be edited")
     e.Cancel = True

    End If




End Sub

#End Region

#Region "Button Handlers"

Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

SecondsIntoCycle = 0
    auxEnvironment.Initialise()
    processing = True
    SetProcessingStatus()

Timer1.Start()

End Sub
Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click

    processing = False
    SetProcessingStatus()

 Timer1.Stop()

End Sub

#End Region

Private Sub RefreshDisplays()

    'M0 Refresh Output Displays
    txtM0_Out_AlternatorsEfficiency.Text = auxEnvironment.M0.AlternatorsEfficiency
    txtM0_Out_HVacElectricalCurrentDemand.Text = auxEnvironment.M0.GetHVACElectricalPowerDemandAmps

    'M05
     txtM05_OutSmartIdleCurrent.Text = auxEnvironment.M05.SmartIdleCurrent
     txtM05_Out_AlternatorsEfficiencyIdle.Text = auxEnvironment.M05.AlternatorsEfficiencyIdleResultCard
     txtM05_out_SmartTractionCurrent.Text = auxEnvironment.M05.SmartTractionCurrent
     txtM05_out_AlternatorsEfficiencyTraction.Text = auxEnvironment.M05.AlternatorsEfficiencyTractionOnResultCard
     txtM05_out_SmartOverrunCurrent.Text = auxEnvironment.M05.SmartOverrunCurrent
     txtM05_out_AlternatorsEfficiencyOverrun.Text = auxEnvironment.M05.AlternatorsEfficiencyOverrunResultCard

     'M1
     txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.Text = auxEnvironment.M1.AveragePowerDemandAtAlternatorFromHVACElectricsWatts
     txtM1_out_AvgPowerDemandAtCrankMech.Text = auxEnvironment.M1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
     txtM1_out_AvgPwrAtCrankFromHVACElec.Text = auxEnvironment.M1.AveragePowerDemandAtCrankFromHVACElectricsWatts
     txtM1_out_HVACFuelling.Text = auxEnvironment.M1.HVACFuelingLitresPerHour

     'M2
     txtM2_out_AvgPowerAtAltFromElectrics.Text = auxEnvironment.M2.GetAveragePowerDemandAtAlternator
     txtM2_out_AvgPowerAtCrankFromElectrics.Text = auxEnvironment.M2.GetAveragePowerAtCrankFromElectrics()

     'M3
     txtM3_out_AveragePowerAtCrankFromPneumatics.Text = auxEnvironment.M3.GetAveragePowerDemandAtCrankFromPneumatics
     txtM3_out_TotalAirConsumedPerCycleInLitres.Text = auxEnvironment.M3.TotalAirConsumedPerCycle

     'M4
      txtM4_out_CompressorFlowRate.Text = auxEnvironment.M4.GetFlowRate
      txtM4_out_CompresssorPwrOnMinusPwrOff.Text = auxEnvironment.M4.GetPowerDifference
      txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.Text = auxEnvironment.M4.GetPowerCompressorOff
      txtM4_out_PowerAtCrankFromPneumaticsCompressorON.Text = auxEnvironment.M4.GetPowerCompressorOn

      'M5
      txtM5_out_AltRegenPowerAtCrankIdleWatts.Text = auxEnvironment.M5.AlternatorsGenerationPowerAtCrankIdleWatts
      txtM5_out_AltRegenPowerAtCrankOverrunWatts.Text = auxEnvironment.M5.AlternatorsGenerationPowerAtCrankOverrunWatts
      txtM5_out_AltRegenPowerAtCrankTractionWatts.Text = auxEnvironment.M5.AlternatorsGenerationPowerAtCrankTractionOnWatts

      'M6
      txtM6_out_OverrunFlag.Text = auxEnvironment.M6.OverrunFlag
      txtM6_out_SmartElectricalAndPneumaticsCompressorFlag.Text = auxEnvironment.M6.SmartElecAndPneumaticsCompressorFlag
      txtM6_out_SmartElectriclAndPneumaticsAltPowerGenAtCrank.Text = auxEnvironment.M6.SmartElecAndPneumaticAltPowerGenAtCrank
      txtM6_out_SmartElectricalAndPneumaticAirCompPowerGenAtCrank.Text = auxEnvironment.M6.SmartElecAndPneumaticAirCompPowerGenAtCrank
      txtM6_out_SmarElectricalOnlyAltPowerGenAtCrank.Text = auxEnvironment.M6.SmartElecOnlyAltPowerGenAtCrank
      txtM6_out_AveragePowerDemandAtCrankFromPneumatics.Text = auxEnvironment.M6.AveragePowerDemandAtCrankFromPneumatics
      txtM6_out_SmartPneumaticOnlyAirCompPowerGenAtCrank.Text = auxEnvironment.M6.SmartPneumaticOnlyAirCompPowerGenAtCrank
      txtM6_out_AveragePowerDemandAtCrankFromElectricsIncHVAC.Text = auxEnvironment.M6.AvgPowerDemandAtCrankFromElectricsIncHVAC
      txtM6_out_SmartPneumaticsOnlyCompressorFlag.Text = auxEnvironment.M6.SmartPneumaticsOnlyCompressorFlag

      'M7
      txtM7_out_SmartElectricalAndPneumaticsAux_AltPowerGenAtCrank.Text = auxEnvironment.M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
      txtM7_out_SmartElectricalAndPneumaticAux_AirCompPowerGenAtCrank.Text = auxEnvironment.M7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
      txtM7_out_SmartElecOnlyAux_AltPwrGenAtCrank.Text = auxEnvironment.M7.SmartElectricalOnlyAuxAltPowerGenAtCrank
      txtM7_out_SmartPneumaticsOnlyAux_AirCompPwrRegenAtCrank.Text = auxEnvironment.M7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank


      'M8
      txtM8_out_AuxPowerAtCrankFromAllAncillaries.Text = auxEnvironment.M8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries
      txtM8_out_SmartElectricalAltPwrGenAtCrank.Text = auxEnvironment.M8.SmartElectricalAlternatorPowerGenAtCrank
      txtM8_out_CompressorFlag.Text = auxEnvironment.M8.CompressorFlag

      'M9
      txtM9_out_LitresOfAirConsumptionCompressorONContinuously.Text = auxEnvironment.M9.LitresOfAirCompressorOnContinually
      txtM9_out_LitresOfAirCompressorOnlyOnInOverrun.Text = auxEnvironment.M9.LitresOfAirCompressorOnOnlyInOverrun
      txtM9_out_TotalCycleFuelConsumptionCompressorOnContinuously.Text = auxEnvironment.M9.TotalCycleFuelConsumptionCompressorOnContinuously
      txtM9_out_TotalCycleFuelConsumptionCompressorOFFContinuously.Text = auxEnvironment.M9.TotalCycleFuelConsumptionCompressorOffContinuously


      'M10
      txtM10_out_BaseFCWithAvgAuxLoads.Text = auxEnvironment.M10.BaseFuelConsumptionWithAverageAuxiliaryLoads
      txtM10_out_FCWithSmartPSAndAvgElecPowerDemand.Text = auxEnvironment.M10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand


      'M11
      txtM11_out_TotalCycleElectricalEnergyGenOverrunOnly.Text = auxEnvironment.M11.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly
      txtM11_out_SmartElectricalTotalCycleElectricalEnergyGenerated.Text = auxEnvironment.M11.SmartElectricalTotalCycleEletricalEnergyGenerated
      txtM11_out_TotalCycleElectricalDemand.Text = auxEnvironment.M11.TotalCycleElectricalDemand
      txtM11_out_TotalCycleFuelConsumptionSmartElectricalLoad.Text = auxEnvironment.M11.TotalCycleFuelConsumptionSmartElectricalLoad
      txtM11_out_TotalCycleFuelConsumptionZeroElectricalLoad.Text = auxEnvironment.M11.TotalCycleFuelConsumptionZeroElectricalLoad

      'M12
      txtM12_out_FuelConsumptionWithSmartElectricsAndAveragePneumaticPowerDemand.Text = auxEnvironment.M12.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand


      'M13
      txtM13_out_TotalCycleFuelCalculation.Text = auxEnvironment.M13.TotalCycleFuelConsumption

End Sub

Private Sub SetProcessingStatus()

Dim thisExe As System.Reflection.Assembly
Dim file As System.IO.Stream
thisExe = System.Reflection.Assembly.GetExecutingAssembly()

If processing Then

file = thisExe.GetManifestResourceStream("VectoAuxiliaries.greenLight.jpg")

Else

file = thisExe.GetManifestResourceStream("VectoAuxiliaries.amberLight.jpg")

End If

Me.PictureBox1.Image = Image.FromStream(file)


End Sub

Private Sub RefreshDisplayValues_Timed(sender As Object, e As EventArgs) Handles Timer1.Tick


  If SecondsIntoCycle = 0 Then

  auxEnvironment.M9.CycleStep(auxEnvironment.Signals.TotalCycleTimeSeconds)

  auxEnvironment.M11.CycleStep(auxEnvironment.Signals.TotalCycleTimeSeconds)
  End If

  SecondsIntoCycle += 1

  SetProcessingStatus()

  RefreshDisplays()




End Sub

'Form Overrides
Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        If keyData = Keys.Enter AndAlso Me.AcceptButton Is Nothing Then
        Dim box As TextBoxBase = CType(Me.ActiveControl, TextBoxBase)

        If box Is Nothing OrElse Not box.Multiline Then

          Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
          Return True

       End If

       End If


        Return MyBase.ProcessCmdKey(msg, keyData)



    End Function


Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


 'JSON METHOD

  auxEnvironment.ClearDown()

  auxEnvironment.Save()


  'BINARY METHOD
 'Dim formatter = New BinaryFormatter
 'Dim _stream As System.IO.Stream = New FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None)
 'auxEnvironment.ClearDown()
 'formatter.Serialize(_stream, auxEnvironment)
 '_stream.Close()


End Sub




Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click



  'JSON METHOD
  'Release existing databindings
  UnbindAllControls( Me)


  auxEnvironment.Load()

  ''Only required for Harness environment
  auxEnvironment.Initialise()

  CreateBindings()

 'BINARY METHOD
  'Dim formatter As IFormatter = New BinaryFormatter()
  'Dim stream As Stream 


  'stream =  new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read)
  'Dim BAux As AuxillaryEnvironment  =  DirectCast(formatter.Deserialize(stream),AuxillaryEnvironment)

  ''Release existing databindings
  'UnbindAllControls( Me)

  ''Replace ReferenceConverter to auxEnvironment with retreived stream.
  'auxEnvironment=Baux
  
  ''Only required for Harness environment.

  'auxEnvironment.Initialise()

  'CreateBindings()

  'stream.Close()



End Sub

public sub UnbindAllControls(ByRef container As Control)   
  'Clear all of the controls within the container object
  'If "Recurse" is true, then also clear controls within any sub-containers
  Dim ctrl As Control = Nothing
 
  For Each ctrl In container.Controls

           ctrl.DataBindings.Clear()

           If ctrl.HasChildren then
              UnbindAllControls(ctrl)
           End If

  next

End Sub


End Class