Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports System.ComponentModel

Public Class Dashboard

Public auxEnvironment As New AuxillaryEnvironment("")

Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

cboCycle.SelectedIndex = 0

  SetupControls()

  CreateBindings()

  'Validate Pneumatics


  ' ValidateChildren(True)


End Sub

Private Sub cboCycle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCycle.SelectedIndexChanged

End Sub

Private Sub SetupControls()


      Dim cIndex As Integer = 0

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
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items )
     gvElectricalConsumables.DataSource = electricalConsumerBinding

     'ResultCards

         'IDLE
         Dim idleBinding = new BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardIdle)
         idleBinding.AllowNew=true   
         idleBinding.AllowRemove=True
         gvResultsCardIdle.DataSource=idleBinding 
             
         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardTraction)
         tractionBinding.AllowNew=true   
         tractionBinding.AllowRemove=true           
         gvResultsCardTraction.DataSource = tractionBinding
        
         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardOverrun)
         overrunBinding.AllowNew=true   
         overrunBinding.AllowRemove=true   
         gvResultsCardOverrun.DataSource=overrunBinding


        'Pneumatic Auxillaries Binding
        txtAdBlueNIperMinute.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"AdBlueNIperMinute")
        txtOverrunUtilisationForCompressionFraction.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"OverrunUtilisationForCompressionFraction")
        txtBrakingWithRetarderNIperKG.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BrakingWithRetarderNIperKG")
        txtBrakingNoRetarderNIperKG.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BrakingNoRetarderNIperKG")
        txtBreakingPerKneelingNIperKGinMM.DataBindings.Add("Text",auxEnvironment.PneumaticAuxillariesConfig,"BreakingPerKneelingNIperKGinMM")
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

End Sub


Private Sub gvElectricalConsumables_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles gvElectricalConsumables.CellEndEdit

End Sub


Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
   Dim s As Single

   If  column.ReadOnly Then return



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


Private Sub SmartResult_CellValidating( sender As Object,  e As DataGridViewCellValidatingEventArgs) Handles gvResultsCardIdle.CellValidating

   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)

   If Not IsNumeric(e.FormattedValue) Then
       MessageBox.Show("This value must be numeric")
       e.Cancel=true      
   End If

End Sub


private sub resultCard_CellMouseUp( sender As Object,  e as DataGridViewCellMouseEventArgs) Handles gvResultsCardIdle.CellMouseUp, gvResultsCardTraction.CellMouseUp, gvResultsCardOverrun.CellMouseUp
    
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


'****** PNEUMATIC VALIDATION

Public Sub Validating_PneumaticHandler( sender As Object, e As CancelEventArgs ) Handles  txtAdBlueNIperMinute.Validating,  txtBrakingWithRetarderNIperKG.Validating, txtBrakingNoRetarderNIperKG.Validating, txtAirControlledSuspensionNIperMinute.Validating, txtBreakingPerKneelingNIperKGinMM.Validating, txtSmartRegenFractionTotalAirDemand.Validating, txtPerStopBrakeActuationNIperKG.Validating, txtPerDoorOpeningNI.Validating, txtOverrunUtilisationForCompressionFraction.Validating, txtNonSmartRegenFractionTotalAirDemand.Validating, txtDeadVolumeLitres.Validating, txtDeadVolBlowOutsPerLitresperHour.Validating, txtKneelingHeightMillimeters.Validating, txtCompressorMap.Validating, txtCompressorGearRatio.Validating, txtCompressorGearEfficiency.Validating, txtActuationsMap.Validating, cboDoors.Validating, cboCompressorType.Validating, cboAirSuspensionControl.Validating, cboAdBlueDosing.Validating

Dim control As Control = CType( sender, Control)

Select Case control.Name

     case "txtAdBlueNIperMinute"
       If Not IsZeroOrPostiveNumber(txtAdBlueNIperMinute.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtAdBlueNIperMinute ,"Please provide a non negative number.")    
       End if

      case "txtOverrunUtilisationForCompressionFraction"
         If Not IsNumberBetweenZeroandOne(txtOverrunUtilisationForCompressionFraction.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtOverrunUtilisationForCompressionFraction ,"Please provide a non negative between 0 and 1.")    
       End if

      case "txtBrakingWithRetarderNIperKG"
        If Not IsZeroOrPostiveNumber(txtBrakingWithRetarderNIperKG.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtBrakingWithRetarderNIperKG ,"Please provide a non negative number.")    
       End if

      case "txtBrakingNoRetarderNIperKG"
       If Not IsZeroOrPostiveNumber(txtBrakingNoRetarderNIperKG.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtBrakingNoRetarderNIperKG ,"Please provide a non negative number.")    
       End if

      case "txtBreakingPerKneelingNIperKGinMM"
       If Not IsZeroOrPostiveNumber(txtBreakingPerKneelingNIperKGinMM.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtBreakingPerKneelingNIperKGinMM ,"Please provide a non negative number.")    
       End if

      case "txtPerDoorOpeningNI"
       If Not IsZeroOrPostiveNumber(txtPerDoorOpeningNI.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtPerDoorOpeningNI ,"Please provide a non negative number.")    
       End if

      case "txtPerStopBrakeActuationNIperKG"
       If Not IsZeroOrPostiveNumber(txtPerStopBrakeActuationNIperKG.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtPerStopBrakeActuationNIperKG ,"Please provide a non negative number.")    
       End if

      case "txtAirControlledSuspensionNIperMinute"
       If Not IsZeroOrPostiveNumber(txtAirControlledSuspensionNIperMinute.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtAirControlledSuspensionNIperMinute ,"Please provide a non negative number.")    
       End if

      case "txtNonSmartRegenFractionTotalAirDemand"
       If Not IsZeroOrPostiveNumber(txtNonSmartRegenFractionTotalAirDemand.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtNonSmartRegenFractionTotalAirDemand ,"Please provide a non negative number.")    
       End if


      case "txtSmartRegenFractionTotalAirDemand"
         If Not IsNumberBetweenZeroandOne(txtSmartRegenFractionTotalAirDemand.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtSmartRegenFractionTotalAirDemand ,"Please provide a non negative between 0 and 1.")    
       End if


      case "txtDeadVolumeLitres"
         If Not IsZeroOrPostiveNumber(txtDeadVolumeLitres.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtDeadVolumeLitres ,"Please provide a non negative between 0 and 1.")    
       End if


      case "txtDeadVolBlowOutsPerLitresperHour"
         If Not IsZeroOrPostiveNumber(txtDeadVolBlowOutsPerLitresperHour.Text) then 
         e.Cancel=true
         errorProvider.SetError(txtDeadVolBlowOutsPerLitresperHour ,"Please provide a non negative between 0 and 1.")    
       End if


       'USER CONFIG

        case "cboCompressorType"
         If cboCompressorType.SelectedIndex<1 then 
         e.Cancel=true
         errorProvider.SetError(cboCompressorType ,"Please select a Compressor type from the Dropdown list.")    
       End if

        case "txtCompressorMap"
        'Test for empty after trim
        If txtCompressorMap.Text.Trim.Length=0 then
         e.Cancel=true
         errorProvider.SetError(txtCompressorMap ,"Please enter the localtion of a valid compressor map.")    
        End if
        'Test File is valid
        Dim comp As CompressorMap
        Try

        comp = New CompressorMap( txtCompressorMap.Text)
        comp.Initialise()

        Catch ex As Exception

         e.Cancel=true
         errorProvider.SetError(txtCompressorMap ,"Error : map is invalid or cannot be found, please select a Cvalid compressor map")  

        End Try


        case "txtCompressorGearEfficiency"

        case "txtCompressorGearRatio"

        case "txtActuationsMap"

        case "chkSmartAirCompression"

        case "chkSmartRegeneration"

        case "chkRetarderBrake"

        case "txtKneelingHeightMillimeters"

        case "cboAirSuspensionControl" 

        case "cboAdBlueDosing"

        case "cboDoors.DataBindings"


         
End Select


End Sub


Public Sub Validated_PneumaticHandler( sender As Object, e As EventArgs ) Handles tabMain.Validating, txtAdBlueNIperMinute.Validating, txtAdBlueNIperMinute.Validated, txtSmartRegenFractionTotalAirDemand.Validated, txtPerStopBrakeActuationNIperKG.Validated, txtPerDoorOpeningNI.Validated, txtOverrunUtilisationForCompressionFraction.Validated, txtNonSmartRegenFractionTotalAirDemand.Validated, txtDeadVolumeLitres.Validated, txtDeadVolBlowOutsPerLitresperHour.Validated, txtBreakingPerKneelingNIperKGinMM.Validated, txtBrakingWithRetarderNIperKG.Validated, txtBrakingNoRetarderNIperKG.Validated, txtAirControlledSuspensionNIperMinute.Validated, txtKneelingHeightMillimeters.Validated, txtCompressorMap.Validated, txtCompressorGearRatio.Validated, txtCompressorGearEfficiency.Validated, txtActuationsMap.Validated, cboDoors.Validated, cboCompressorType.Validated, cboAirSuspensionControl.Validated, cboAdBlueDosing.Validated

Dim control As Control = CType( sender, Control)

       errorProvider.SetError(control ,string.Empty)

End Sub



'*****  ELECTRICAL VALIDATION

'Validation helpers

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


Private Sub tabMain_Validating( sender As Object,  e As CancelEventArgs) Handles tabMain.Validating

End Sub




End Class