Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Drawing

Public Class Dashboard

#Region "Fields"

Public auxEnvironment As New AuxillaryEnvironment("")
Private TabColors As Dictionary(Of TabPage, Color) = New Dictionary(Of TabPage, Color)()

#End Region


Private Sub SetupControls()


      Dim cIndex As Integer = 0

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
     txtPowernetVoltage.DataBindings.Add("Text", auxEnvironment.VectoInputs, "PowerNetVoltage")
     txtVehicleWeightKG.DataBindings.Add("Text", auxEnvironment.VectoInputs, "VehicleWeightKG")
     cboCycle.DataBindings.Add("Text", auxEnvironment.VectoInputs, "Cycle")

     'Electricals General
     txtAlternatorMapPath.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked", auxEnvironment.ElectricalUserInputsConfig, "SmartElectrical")

     'Electrical ConsumablesGrid
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items)
     gvElectricalConsumables.DataSource = electricalConsumerBinding

     'ResultCards

         'IDLE
         Dim idleBinding = New BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardIdle)
         idleBinding.AllowNew = True
         idleBinding.AllowRemove = True
         gvResultsCardIdle.DataSource = idleBinding

         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardTraction)
         tractionBinding.AllowNew = True
         tractionBinding.AllowRemove = True
         gvResultsCardTraction.DataSource = tractionBinding

         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardOverrun)
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
        cboCompressorType.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorType")
        txtCompressorMap.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorMap")
        txtCompressorGearEfficiency.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorGearEfficiency")
        txtCompressorGearRatio.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "CompressorGearRatio")
        txtActuationsMap.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "ActuationsMap")
        chkSmartAirCompression.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "SmartAirCompression")
        chkSmartRegeneration.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "SmartRegeneration")
        chkRetarderBrake.DataBindings.Add("Checked", auxEnvironment.PneumaticUserInputsConfig, "RetarderBrake")
        txtKneelingHeightMillimeters.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "KneelingHeightMillimeters")
        cboAirSuspensionControl.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "AirSuspensionControl")
        cboAdBlueDosing.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "AdBlueDosing")
        cboDoors.DataBindings.Add("Text", auxEnvironment.PneumaticUserInputsConfig, "Doors")

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
Public Sub Validating_PneumaticHandler(sender As Object, e As CancelEventArgs) Handles txtAdBlueNIperMinute.Validating, txtBrakingWithRetarderNIperKG.Validating, txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating, txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating, txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating, txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating, txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating, txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating, txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating, cboCompressorType.Validating, cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating

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
       If cboCompressorType.SelectedIndex < 1 Then
         ErrorProvider.SetError(cboCompressorType, "Please select a Compressor type from the Dropdown list.")
         result = False
       Else
         ErrorProvider.SetError(cboCompressorType, String.Empty)
       End If

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
         ErrorProvider.SetError(txtAlternatorMapPath, "Please enter the localtion of a valid compressor map.")
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
         ErrorProvider.SetError(txtAlternatorMapPath, "Error : map is invalid or cannot be found, please select a Cvalid compressor map")
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

'*****  HVAC VALIDATION


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




#End Region

#Region "Button Handlers"

Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        Validate_Pneumatics()
        Validate_Electrics()

End Sub


#End Region



End Class