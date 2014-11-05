Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports System.ComponentModel

Public Class Dashboard

#Region "Fields"

Public auxEnvironment As New AuxillaryEnvironment("")
private TabColors As Dictionary( Of TabPage, Color)  = new   Dictionary( Of TabPage, Color) ()

#End Region


Private Sub SetupControls()


      Dim cIndex As Integer = 0

      gvElectricalConsumables.AutoGenerateColumns=false

     'ElectricalConsumerGrid 
     'Columns
     cIndex = gvElectricalConsumables.Columns.Add("Category", "Category")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "Category"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth = 150
     gvElectricalConsumables.Columns(cIndex).ReadOnly = True
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)

     cIndex = gvElectricalConsumables.Columns.Add("ConsumerName", "Name")

     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "ConsumerName"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth = 308
     gvElectricalConsumables.Columns(cIndex).ReadOnly = True
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)

     Dim baseVehicle As New DataGridViewCheckBoxColumn(False)
     baseVehicle.HeaderText = "Base Vehicle"
     cIndex = gvElectricalConsumables.Columns.Add(baseVehicle)
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "BaseVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 75
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText="Energy included in the calculations of base vehicle"

     cIndex = gvElectricalConsumables.Columns.Add("NominalConsumptionAmps", "Nominal Amps")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NominalConsumptionAmps"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText="Nominal consumption in AMPS"

     cIndex = gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn", "PhaseIdle/ TractionOn")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "PhaseIdle_TractionOn"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText="Represents the amount of time (during engine fueling) as " & vbCrLf & "percentage that the consumer is active during the cycle."

     cIndex = gvElectricalConsumables.Columns.Add("NumberInActualVehicle", "Num in Vehicle")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NumberInActualVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 70
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
     gvElectricalConsumables.Columns(cIndex).HeaderCell.Style.Padding = New Padding(1,2,1,1)
     gvElectricalConsumables.Columns(cIndex).HeaderCell.ToolTipText="Number of consumables of this" & vbCrLf & "type installed on the vehicle."

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

     'Electricals General
     txtAlternatorMapPath.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig, "DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked", auxEnvironment.ElectricalUserInputsConfig, "SmartElectrical")

     'Electrical ConsumablesGrid
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items )
     gvElectricalConsumables.DataSource = electricalConsumerBinding

     'ResultCards

         'IDLE
         Dim idleBinding = new BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardIdle.Results)
         idleBinding.AllowNew=true   
         idleBinding.AllowRemove=True
         gvResultsCardIdle.DataSource=idleBinding 
             
         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardTraction.Results)
         tractionBinding.AllowNew=true   
         tractionBinding.AllowRemove=true           
         gvResultsCardTraction.DataSource = tractionBinding
        
         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardOverrun.Results)
         overrunBinding.AllowNew=true   
         overrunBinding.AllowRemove=true   
         gvResultsCardOverrun.DataSource=overrunBinding


        'Pneumatic Auxillaries Binding
        txtAdBlueNIperMinute.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"AdBlueNIperMinute")     

        txtOverrunUtilisationForCompressionFraction.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"OverrunUtilisationForCompressionFraction")
        txtBrakingWithRetarderNIperKG.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BrakingWithRetarderNIperKG")
        txtBrakingNoRetarderNIperKG.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BrakingNoRetarderNIperKG")
        txtBreakingPerKneelingNIperKGinMM.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BreakingPerKneelingNIperKGinMM",true,DataSourceUpdateMode.OnPropertyChanged,nothing,"0.########")
        txtPerDoorOpeningNI.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"PerDoorOpeningNI")
        txtPerStopBrakeActuationNIperKG.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"PerStopBrakeActuationNIperKG")
        txtAirControlledSuspensionNIperMinute.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"AirControlledSuspensionNIperMinute")
        txtNonSmartRegenFractionTotalAirDemand.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"NonSmartRegenFractionTotalAirDemand")
        txtSmartRegenFractionTotalAirDemand.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"SmartRegenFractionTotalAirDemand")
        txtDeadVolumeLitres.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"DeadVolumeLitres")
        txtDeadVolBlowOutsPerLitresperHour.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"DeadVolBlowOutsPerLitresperHour")

        'Pneumatic UserInputsConfig Binding
        cboCompressorType.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"CompressorType")        
        txtCompressorMap.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"CompressorMap")             
        txtCompressorGearEfficiency.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"CompressorGearEfficiency") 
        txtCompressorGearRatio.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"CompressorGearRatio")      
        txtActuationsMap.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"ActuationsMap")        
        chkSmartAirCompression.DataBindings.Add("Checked",auxEnvironment.PneumaticUserInputsConfig,"SmartAirCompression")       
        chkSmartRegeneration.DataBindings.Add("Checked",auxEnvironment.PneumaticUserInputsConfig,"SmartRegeneration")         
        chkRetarderBrake.DataBindings.Add("Checked",auxEnvironment.PneumaticUserInputsConfig,"RetarderBrake")          
        txtKneelingHeightMillimeters.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"KneelingHeightMillimeters")  
        cboAirSuspensionControl.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"AirSuspensionControl")       
        cboAdBlueDosing.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"AdBlueDosing")             
        cboDoors.DataBindings.Add("Text",auxEnvironment.PneumaticUserInputsConfig,"Doors")    
        
        'HVAC Bindings     
        txtHVACElectricalLoadPowerWatts.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel,"HVACElectricalLoadPowerWatts")
        txtHVACFuellingLitresPerHour.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel,"HVACFuellingLitresPerHour")
        txtHVACMechanicalLoadPowerWatts.DataBindings.Add("Text", auxEnvironment.HvacUserInputsConfig.SteadyStateModel,"HVACMechanicalLoadPowerWatts")

        'Signals
        chkClutchEngaged.DataBindings.Add("Checked", auxEnvironment.Signals,"ClutchEngaged")
        txtEngineDrivelinePower.DataBindings.Add("Text", auxEnvironment.Signals,"EngineDrivelinePower")
        txtEngineDrivelineTorque.DataBindings.Add("Text", auxEnvironment.Signals,"EngineDrivelineTorque")
        txtEngineMotoringPower.DataBindings.Add("Text", auxEnvironment.Signals,"EngineMotoringPower")
        txtEngineSpeed.DataBindings.Add("Text", auxEnvironment.Signals,"EngineSpeed")
        txtTotalCycleTimeSeconds.DataBindings.Add("Text",auxEnvironment.Signals,"TotalCycleTimeSeconds")
                

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


Public Function IsPostiveNumber(byval test As string)As boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) then Return False

     Dim number As Single

     If Not Single.TryParse( test,  number)  then Return false

     If number<=0 then Return False
     

     Return true
  
End Function

Public Function IsZeroOrPostiveNumber(byval test As string)As boolean

     'Is this numeric sanity check.
     If Not IsNumeric(test) then Return False

     Dim number As Single

     If Not Single.TryParse( test,  number)  then Return false

     If number<0 then Return False
     

     Return true
  
End Function

Public Function IsNumberBetweenZeroandOne( test As String )

     'Is this numeric sanity check.
     If Not IsNumeric(test) then Return False

     Dim number As Single

     If Not Single.TryParse( test,  number)  then Return false

     If number<0 orelse number >1  then Return False

     Return True
     
End Function

Public Function IsIntegerZeroOrPositiveNumber( test As string)

     'Is this numeric sanity check.
     If Not IsNumeric(test) then Return False

     'if not integer then return false

     Dim number As integer

     If Not integer.TryParse( test,  number)  then Return false

     If number<0   then Return False

     Return True


End Function

#End Region

#REgion "Validation Control"


'****** PNEUMATIC VALIDATION
public sub Validating_PneumaticHandler( sender as Object, e As CancelEventArgs  )   Handles  txtAdBlueNIperMinute.Validating,  txtBrakingWithRetarderNIperKG.Validating, txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating, txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating, txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating, txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating, txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating, txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating, txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating, cboCompressorType.Validating, cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating                

    e.Cancel= Not Validate_Pneumatics()

End Sub
Public function Validate_Pneumatics(  ) As boolean

       Dim result As Boolean = true

       'PNEUMATIC AUXILLARIES PART
       '***************************


       'AdBlue NI per Minute : txtAdBlueNIperMinute
       If Not IsZeroOrPostiveNumber(txtAdBlueNIperMinute.Text) then 
         errorProvider.SetError(txtAdBlueNIperMinute ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtAdBlueNIperMinute ,String.Empty) 
       End if

       'Overrun Utilisation For Compression Fraction : txtOverrunUtilisationForCompressionFraction
       If Not IsNumberBetweenZeroandOne(txtOverrunUtilisationForCompressionFraction.Text) then 
         errorProvider.SetError(txtOverrunUtilisationForCompressionFraction ,"Please provide a non negative between 0 and 1.")    
         result = false
         Else
         errorProvider.SetError(txtOverrunUtilisationForCompressionFraction ,String.Empty)           
       End if

       'Braking With Retarder NI per KG : txtBrakingWithRetarderNIperKG
       If Not IsZeroOrPostiveNumber(txtBrakingWithRetarderNIperKG.Text) then 
         errorProvider.SetError(txtBrakingWithRetarderNIperKG ,"Please provide a non negative number.") 
         result = false
        Else
         errorProvider.SetError(txtBrakingWithRetarderNIperKG ,String.Empty)         
       End if

       'Braking No Retarder NI per KG : txtBrakingNoRetarderNIperKG
       If Not IsZeroOrPostiveNumber(txtBrakingNoRetarderNIperKG.Text) then 
         errorProvider.SetError(txtBrakingNoRetarderNIperKG ,"Please provide a non negative number.") 
         result=false
       Else
         errorProvider.SetError(txtBrakingNoRetarderNIperKG ,String.Empty)        
       End if

       'Breaking Per Kneeling NI per KG in MM : txtBreakingPerKneelingNIperKGinMM
       If Not IsZeroOrPostiveNumber(txtBreakingPerKneelingNIperKGinMM.Text) then 
         errorProvider.SetError(txtBreakingPerKneelingNIperKGinMM ,"Please provide a non negative number.")    
         result=false
       Else
         errorProvider.SetError(txtBreakingPerKneelingNIperKGinMM ,String.Empty)     
       End if

       'Per Door Opening NI : txtPerDoorOpeningNI
       If Not IsZeroOrPostiveNumber(txtPerDoorOpeningNI.Text) then 
         errorProvider.SetError(txtPerDoorOpeningNI ,"Please provide a non negative number.")    
         result=false
       Else
         errorProvider.SetError(txtPerDoorOpeningNI ,String.Empty)          
       End if

       'Per Stop Brake Actuation NI per KG : txtPerStopBrakeActuationNIperKG
       If Not IsZeroOrPostiveNumber(txtPerStopBrakeActuationNIperKG.Text) then 
         errorProvider.SetError(txtPerStopBrakeActuationNIperKG ,"Please provide a non negative number.")  
         result=false  
       Else
         errorProvider.SetError(txtPerStopBrakeActuationNIperKG ,String.Empty)         
       End if

       'Air Controlled Suspension NI per Minute : txtAirControlledSuspensionNIperMinute
       If Not IsZeroOrPostiveNumber(txtAirControlledSuspensionNIperMinute.Text) then 
         errorProvider.SetError(txtAirControlledSuspensionNIperMinute ,"Please provide a non negative number.")
         result=false    
       Else
         errorProvider.SetError(txtAirControlledSuspensionNIperMinute ,String.Empty)       
       End if

       'Non Smart Regen Fraction Total Air Demand : txtNonSmartRegenFractionTotalAirDemand
       If Not IsZeroOrPostiveNumber(txtNonSmartRegenFractionTotalAirDemand.Text) then 
         errorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand ,"Please provide a non negative number.")  
         result=false    
       Else
         errorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand ,String.Empty)         
       End if

       'Smart Regen Fraction Total Air Demand : txtSmartRegenFractionTotalAirDemand
       If Not IsNumberBetweenZeroandOne(txtSmartRegenFractionTotalAirDemand.Text) then 
         errorProvider.SetError(txtSmartRegenFractionTotalAirDemand ,"Please provide a non negative between 0 and 1.")    
         result=False
       Else
         errorProvider.SetError(txtSmartRegenFractionTotalAirDemand ,String.Empty)       
       End if


       'Dead Volume Litres : txtDeadVolumeLitres
       If Not IsZeroOrPostiveNumber(txtDeadVolumeLitres.Text) then 
         errorProvider.SetError(txtDeadVolumeLitres ,"Please provide a non negative between 0 and 1.")    
         result=False
       Else
         errorProvider.SetError(txtDeadVolumeLitres ,String.Empty)           
       End if


       'Dead Vol BlowOuts Per Litresper Hour : txtDeadVolBlowOutsPerLitresperHour
       If Not IsZeroOrPostiveNumber(txtDeadVolBlowOutsPerLitresperHour.Text) then 
         errorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour ,"Please provide a non negative between 0 and 1.")
         result=false    
        Else
         errorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour ,String.Empty)        
       End if


       'USER CONFIG PART 
       '*****************************************************************************************
       If cboCompressorType.SelectedIndex<1 then 
         errorProvider.SetError(cboCompressorType ,"Please select a Compressor type from the Dropdown list.")    
         result=false
       Else
         errorProvider.SetError(cboCompressorType ,String.Empty)        
       End if

        'Compressor Map path : txtCompressorMap
        'Test for empty after trim
        If txtCompressorMap.Text.Trim.Length=0 then
         errorProvider.SetError(txtCompressorMap ,"Please enter the localtion of a valid compressor map.") 
         result=false   
        else
         errorProvider.SetError(txtCompressorMap ,String.Empty) 
        End if
        'Test File is valid
        Dim comp As CompressorMap
        Try

        comp = New CompressorMap( txtCompressorMap.Text)
        comp.Initialise()
         errorProvider.SetError(txtCompressorMap ,String.Empty) 
        Catch ex As Exception
         errorProvider.SetError(txtCompressorMap ,"Error : map is invalid or cannot be found, please select a Cvalid compressor map")  
         result=false
        End Try

        'Compressor Gear Efficiency : txtCompressorGearEfficiency"
        If NOT  IsNumberBetweenZeroandOne(txtCompressorGearEfficiency.Text) then
          errorProvider.SetError(txtCompressorGearEfficiency ,"Please enter a number between 0 and 1") 
          result=false
        else
          errorProvider.SetError(txtCompressorGearEfficiency ,String.Empty) 
        End If

        'Compressor Gear Ratio : txtCompressorGearRatio
        If NOT  IsPostiveNumber(txtCompressorGearRatio.Text) then
          errorProvider.SetError(txtCompressorGearRatio ,"Please enter a number greater than 0.") 
          result=false
        else
          errorProvider.SetError(txtCompressorGearRatio ,String.Empty) 
        End If


        'Actuations Map : txtActuationsMap

        'NOT Required but included here so readers can see this is a positive ommission
        '******************************************************************************
        'Smart Air Compression : chkSmartAirCompression
        'Smart Regeneration : chkSmartRegeneration
        'Retarder Brake : chkRetarderBrake

        'txtKneelingHeightMillimeters : txtKneelingHeightMillimeters
        If NOT  IsPostiveNumber(txtKneelingHeightMillimeters.Text) then
          errorProvider.SetError(txtKneelingHeightMillimeters ,"Please enter a number greater than 0.") 
          result=false
        else
          errorProvider.SetError(txtKneelingHeightMillimeters ,String.Empty) 
        End If

        'cboAirSuspensionControl : cboAirSuspensionControl
        If cboAirSuspensionControl.SelectedIndex<1  then
          errorProvider.SetError(cboAirSuspensionControl ,"Please make a selection.") 
          result=false
        else
          errorProvider.SetError(cboAirSuspensionControl ,String.Empty) 
        End If

        'cboAdBlueDosing : cboAdBlueDosing
        If cboAdBlueDosing.SelectedIndex<1  then
          errorProvider.SetError(cboAdBlueDosing ,"Please make a selection.") 
          result=false
        else
          errorProvider.SetError(cboAdBlueDosing ,String.Empty) 
        End if

        'cboDoors : cboDoors
        If cboDoors.SelectedIndex<1  then
          errorProvider.SetError(cboDoors ,"Please make a selection.") 
          result=false
        else
          errorProvider.SetError(cboDoors ,String.Empty) 
        End if


        'Set Tab Color

        UpdateTabStatus("tabPneumaticConfig",result)




Return result


End function

'*****  ELECTRICAL VALIDATION
public sub Validating_ElectricsHandler( sender as Object, e As CancelEventArgs  ) Handles txtPowernetVoltage.Validating,  txtAlternatorMapPath.Validating, txtAlternatorGearEfficiency.Validating, txtDoorActuationTimeSeconds.Validating 

    e.Cancel= Not Validate_Electrics()

End Sub
Public function Validate_Electrics() As boolean

Dim result As Boolean = true

  
       'Powernet Voltage : txtPowernetVoltage
       If Not IsPostiveNumber(txtPowernetVoltage.Text) then 
         errorProvider.SetError(txtPowernetVoltage ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtPowernetVoltage ,String.Empty) 
       End if


        'Alternator Map  path : txtAlternatorMapPath
        'Test for empty after trim
        If txtAlternatorMapPath.Text.Trim.Length=0 then
         errorProvider.SetError(txtAlternatorMapPath ,"Please enter the localtion of a valid compressor map.") 
         result=false   
        else
         errorProvider.SetError(txtAlternatorMapPath ,String.Empty) 
        End if

        'Test File is valid
        Dim alt As AlternatorMap
        Try
        alt = New AlternatorMap( txtAlternatorMapPath.Text)
        alt.Initialise()
         errorProvider.SetError(txtAlternatorMapPath ,String.Empty) 
        Catch ex As Exception
         errorProvider.SetError(txtAlternatorMapPath ,"Error : map is invalid or cannot be found, please select a Cvalid compressor map")  
         result=false
        End Try


       'Door Action Time : txtDoorActuationTimeSeconds
       If Not IsPostiveNumber(txtDoorActuationTimeSeconds.Text) then 
         errorProvider.SetError(txtDoorActuationTimeSeconds ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtDoorActuationTimeSeconds ,String.Empty) 
       End if


               UpdateTabStatus("tabElectricalConfig",result)


       Return result


End Function 

'****** HVAC VALIDATION
public sub Validating_HVACHandler( sender as Object, e As CancelEventArgs  ) Handles txtHVACMechanicalLoadPowerWatts.Validating, txtHVACFuellingLitresPerHour.Validating, txtHVACElectricalLoadPowerWatts.Validating 

    e.Cancel= Not Validate_HVAC()

End Sub


Public function Validate_HVAC() As boolean

Dim result As Boolean = true

  
       'HVAC Electrical Load Power Watts : txtHVACElectricalLoadPowerWatts
       If Not IsZeroOrPostiveNumber(txtHVACElectricalLoadPowerWatts.Text) then 
         errorProvider.SetError(txtHVACElectricalLoadPowerWatts ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtHVACElectricalLoadPowerWatts ,String.Empty) 
       End if

       'HVAC Mechanical Load Power Watts : txtHVACMechanicalLoadPowerWatts
       If Not IsZeroOrPostiveNumber(txtHVACMechanicalLoadPowerWatts.Text) then 
         errorProvider.SetError(txtHVACMechanicalLoadPowerWatts ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtHVACMechanicalLoadPowerWatts ,String.Empty) 
       End if


       'HVAC Fuelling Litres Per Hour : txtHVACFuellingLitresPerHour
       If Not IsZeroOrPostiveNumber(txtHVACFuellingLitresPerHour.Text) then 
         errorProvider.SetError(txtHVACFuellingLitresPerHour ,"Please provide a non negative number.") 
         result= false
       Else
          errorProvider.SetError(txtHVACFuellingLitresPerHour ,String.Empty) 
       End if

       UpdateTabStatus("tabHVACConfig",result)


   Return result


End Function


'*****  HVAC VALIDATION


'*****  IMPUTS VALIDATION


#End Region



'Form Controls & Events

Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load 

  'Required for OwnerDraw, this is required in order to color the tabs when a validation error occurs to draw
  'The attention of the user to the fact that attention is required on a particlar tab.
  TabColors.Add(tabGeneralConfig,Control.DefaultBackColor)
  TabColors.Add(tabElectricalConfig,Control.DefaultBackColor)
  TabColors.Add(tabPneumaticConfig,Control.DefaultBackColor)
  TabColors.Add(tabHVACConfig,Control.DefaultBackColor)
  TabColors.Add(tabPlayground,Control.DefaultBackColor)

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
  AddHandler tabMain.DrawItem , new System.Windows.Forms.DrawItemEventHandler(addressof tabMain_DrawItem)


  'Finally Initialise Environment.
  auxEnvironment.Initialise()
  SetProcessingStatus()

End Sub

#Region "Tab Header Color Change"

Private sub UpdateTabStatus( pageName as string, resultGood As Boolean)


       Dim page As TabPage = tabMain.TabPages(pageName)

           If Not resultGood

       SetTabHeader( page, Color.Red)

       Else
              SetTabHeader( page, Control.DefaultBackColor)

       End If




End Sub

private sub SetTabHeader( page as TabPage,  color As Color)

    TabColors(page) = color
    tabMain.Invalidate()

End Sub

private sub tabMain_DrawItem( sender As object,  e As DrawItemEventArgs) 

    dim br as Brush = New SolidBrush(TabColors(tabMain.TabPages(e.Index)))


    using ( br  ) 
    
        e.Graphics.FillRectangle(br, e.Bounds)
        dim sz as SizeF= e.Graphics.MeasureString(tabMain.TabPages(e.Index).Text, e.Font)
        e.Graphics.DrawString(tabMain.TabPages(e.Index).Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1)

        Dim  rect As Rectangle = e.Bounds
        rect.Offset(-1,-1)
        rect.Inflate(1, 1)
       ' e.Graphics.DrawRectangle(Pens.DarkGray, rect)
        'e.DrawFocusRectangle()

    End using




End Sub


#End Region 

#Region "GridHandlers"



Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating 

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
   Dim s As Single


   If e.ColumnIndex=-1 then 

   e.Cancel=true 
   Exit sub
   
   End If



   If  column.ReadOnly  Then return




    Select Case column.Name

     Case "NominalConsumptionAmps"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
             e.Cancel=true
          End if

     Case "NumberInActualVehicle"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
            e.Cancel=true
          Else
            s = Single.Parse(e.FormattedValue)
           End If
           If s Mod 1 > 0 OrElse s < 0 Then
              MessageBox.Show("This value must be a positive whole number ( Integer ) ")
             e.Cancel=true
           End If


     Case "PhaseIdle_TractionOn"
           If Not IsNumeric(e.FormattedValue) Then
             MessageBox.Show("This value must be numeric")
             e.Cancel=true
           Else
            s = Single.Parse(e.FormattedValue)
           End If
           If s < 0 OrElse s > 1 Then
              MessageBox.Show("This must be a value between 0 and 1 ")
              e.Cancel=true
           End If


    End Select

End Sub


Private Sub SmartResult_CellValidating( sender As Object,  e As DataGridViewCellValidatingEventArgs) Handles gvResultsCardIdle.CellValidating, gvResultsCardTraction.CellValidating, gvResultsCardOverrun.CellValidating 

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)

   If Not IsNumeric(e.FormattedValue) Then
       MessageBox.Show("This value must be numeric")
       e.Cancel=true      
   End If

End Sub


private sub resultCard_CellMouseUp( sender As Object,  e as DataGridViewCellMouseEventArgs)  
    
      Dim dgv As DataGridView = CType( sender, DataGridView)


        if e.Button = MouseButtons.Right then

            resultCardContextMenu.Show(dgv, e.Location)
            resultCardContextMenu.Show(Cursor.Position)

        End if


    end sub


Private Sub resultCardContextMenu_ItemClicked( sender As Object,  e As ToolStripItemClickedEventArgs) Handles resultCardContextMenu.ItemClicked 

      Dim menu As ContextMenuStrip = CType( sender, ContextMenuStrip)

      Dim grid as DataGridView  = DirectCast( menu.SourceControl, DataGridView)

      Select Case e.ClickedItem.Text


      Case "Delete"

         For Each selectedRow As datagridviewrow In grid.SelectedRows

            If Not selectedRow.IsNewRow then
            
                grid.Rows.RemoveAt(selectedRow.Index)
           
            End if
           
         Next

      case "Insert"


      End Select
    








End Sub




#End Region

#Region "Button Handlers"

Private Sub btnStart_Click( sender As Object,  e As EventArgs) Handles btnStart.Click 
    auxEnvironment.Initialise()
    processing=true
    SetProcessingStatus

Timer1.Start

End Sub


#End Region


Private sub RefreshDisplays()

    'M0 Refresh Output Displays
    txtM0_Out_AlternatorsEfficiency.Text= auxEnvironment.M0.AlternatorsEfficiency
    txtM0_Out_HVacElectricalCurrentDemand.Text=auxEnvironment.M0.GetHVACElectricalPowerDemandAmps

    'M05
     txtM05_OutSmartIdleCurrent.Text= auxEnvironment.M05.SmartIdleCurrent
     txtM05_Out_AlternatorsEfficiencyIdle.Text=auxEnvironment.M05.AlternatorsEfficiencyIdleResultCard
     txtM05_out_SmartTractionCurrent.Text = auxEnvironment.M05.SmartTractionCurrent
     txtM05_out_AlternatorsEfficiencyTraction.Text=auxEnvironment.M05.AlternatorsEfficiencyTractionOnResultCard
     txtM05_out_SmartOverrunCurrent.Text= auxEnvironment.M05.SmartOverrunCurrent
     txtM05_out_AlternatorsEfficiencyOverrun.Text = auxEnvironment.M05.AlternatorsEfficiencyOverrunResultCard
     'M1
     txtM1_out_AvgPowerDemandAtAlternatorHvacElectrics.Text= auxEnvironment.M1.AveragePowerDemandAtAlternatorFromHVACElectricsWatts
     txtM1_out_AvgPowerDemandAtCrankMech.Text= auxEnvironment.M1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
     txtM1_out_AvgPwrAtCrankFromHVACElec.Text=auxEnvironment.M1.AveragePowerDemandAtCrankFromHVACElectricsWatts
     txtM1_out_HVACFuelling.Text=auxEnvironment.M1.HVACFuelingLitresPerHour

     'M2
     txtM2_out_AvgPowerAtAltFromElectrics.Text=auxEnvironment.M2.GetAveragePowerDemandAtAlternator
     txtM2_out_AvgPowerAtCrankFromElectrics.Text=auxEnvironment.M2.GetAveragePowerAtCrankFromElectrics()

     'M3
     txtM3_out_AveragePowerAtCrankFromPneumatics.Text=auxEnvironment.M3.GetAveragePowerDemandAtCrankFromPneumatics
     txtM3_out_TotalAirConsumedPerCycleInLitres.Text=auxEnvironment.M3.TotalAirConsumedPerCycle

     'M4
      txtM4_out_CompressorFlowRate.Text = auxEnvironment.M4.GetFlowRate
      txtM4_out_CompresssorPwrOnMinusPwrOff.Text=auxEnvironment.M4.GetPowerDifference
      txtM4_out_PowerAtCrankFromPneumaticsCompressorOFF.Text= auxEnvironment.M4.GetPowerCompressorOff
      txtM4_out_PowerAtCrankFromPneumaticsCompressorON.Text= auxEnvironment.M4.GetPowerCompressorOn

      'M5
      txtM5_out_AltRegenPowerAtCrankIdleWatts.text= auxEnvironment.m5.AlternatorsGenerationPowerAtCrankIdleWatts
      txtM5_out_AltRegenPowerAtCrankOverrunWatts.text= auxEnvironment.m5.AlternatorsGenerationPowerAtCrankOverrunWatts
      txtM5_out_AltRegenPowerAtCrankTractionWatts.text= auxEnvironment.m5.AlternatorsGenerationPowerAtCrankTractionOnWatts

 

End Sub



Private processing As Boolean = False


Private sub SetProcessingStatus()

Dim thisExe As System.Reflection.Assembly 
Dim file as System.IO.Stream 
thisExe = System.Reflection.Assembly.GetExecutingAssembly()

If processing then

file  = thisExe.GetManifestResourceStream("AuxillaryTestHarness.greenLight.jpg")

Else

file  = thisExe.GetManifestResourceStream("AuxillaryTestHarness.amberLight.jpg")

End If

me.pictureBox1.Image = Image.FromStream(file)


End Sub



Private Sub RefreshDisplayValues_Timed( sender As Object,  e As EventArgs) Handles Timer1.Tick

  
  SetProcessingStatus()

  RefreshDisplays()


End Sub


   'Form Overrides
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        if keyData = Keys.Enter andalso me.AcceptButton is nothing   then
        dim box As TextBoxBase = CType( me.ActiveControl ,TextBoxBase)

        if box is nothing orelse not box.Multiline then
          
          me.SelectNextControl(me.ActiveControl, true, true, true, true)
          return true
        
       End If
      
       End If
       

        Return MyBase.ProcessCmdKey(msg, keyData)



    End Function


Private Sub btnFinish_Click( sender As Object,  e As EventArgs) Handles btnFinish.Click


    processing=False
    SetProcessingStatus


 Timer1.Stop

End Sub


Private Sub Button2_Click( sender As Object,  e As EventArgs) Handles Button2.Click

    Dim altMap As IAlternatorMap = New AlternatorMap("testAlternatorMap.csv")
    Dim efficiency , rpm, amp As Single
    
    altMap.Initialise()

    Dim CrankSpeed As New System.Collections.Generic.List(Of single)
    CrankSpeed.AddRange(New single() {1500,2000,4000,6000,7000})
    Dim amps As New List(Of single)
    amps.AddRange(New single() {10,27,53,63,68,125,136})

    'On Boundaries
    Console.WriteLine("On BOUNDARY TESTS")
    Console.WriteLine("_________________")
    Console.WriteLine("")
    For Each  rpm  in CrankSpeed 
     For Each amp  In amps
         efficiency  = altMap.GetEfficiency( rpm, amp).Efficiency
        Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
     Next
    Next

    Console.WriteLine("")
    Console.WriteLine("Four Corner Points With Interpolated other")
    Console.WriteLine("_________________")
    Console.WriteLine("")

    rpm=1500 : amp=18.5
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=7000 : amp=96.5
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=1750 : amp=10
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=6500 : amp=10
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))

    Console.WriteLine("")
    Console.WriteLine("Interpolated Both - four data points")
    Console.WriteLine("_________________")
    Console.WriteLine("")

    rpm=1750 : amp=18.5
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=6500 : amp=18.5
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))

    rpm=1750 : amp=96.5
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=6500 : amp=96.4
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))

    Console.WriteLine("")
    Console.WriteLine("Range Limiting")
    Console.WriteLine("_________________")
    Console.WriteLine("")

    rpm=0 : amp=0
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=0 : amp=10
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=0 : amp=200
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=1500 : amp=200
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=7000 : amp=200
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))
    rpm=8000 : amp=200
    efficiency = altMap.GetEfficiency( rpm, amp).Efficiency
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))


    Console.WriteLine("")
    Console.WriteLine("MIKES 40*1000")
    Console.WriteLine("_________________")
    Console.WriteLine("")
    rpm=1000 : amp=40
    Console.WriteLine(String.Format("RPM:{0} , AMP:{1}, EFF:{2})",rpm.ToString(),amp.ToString(),efficiency.ToString()))



End Sub


End Class