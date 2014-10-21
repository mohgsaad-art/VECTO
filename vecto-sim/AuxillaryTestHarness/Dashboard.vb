Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports System.ComponentModel

Public Class Dashboard

Public auxEnvironment As New AuxillaryEnvironment("")

Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

cboCycle.SelectedIndex = 0

  SetupControls()

  CreateBindings()


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

     cIndex = gvElectricalConsumables.Columns.Add("ConsumerName", "Name")

     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "ConsumerName"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth = 300
     gvElectricalConsumables.Columns(cIndex).ReadOnly = True

     Dim baseVehicle As New DataGridViewCheckBoxColumn(False)
     baseVehicle.HeaderText = "BaseVehicle"
     cIndex = gvElectricalConsumables.Columns.Add(baseVehicle)
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "BaseVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 75

     cIndex = gvElectricalConsumables.Columns.Add("NominalConsumptionAmps", "Nominal Amps")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NominalConsumptionAmps"
     gvElectricalConsumables.Columns(cIndex).Width = 50

     cIndex = gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn", "PhaseIdle\n/TractionOn")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "PhaseIdle_TractionOn"
     gvElectricalConsumables.Columns(cIndex).Width = 50


     cIndex = gvElectricalConsumables.Columns.Add("NumberInActualVehicle", "Num in Vehicle")
     gvElectricalConsumables.Columns(cIndex).DataPropertyName = "NumberInActualVehicle"
     gvElectricalConsumables.Columns(cIndex).Width = 50

     'ResultCard Grids
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

     'ConsumablesGrid
     Dim electricalConsumerBinding As New BindingList(Of IElectricalConsumer)(auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items )
     gvElectricalConsumables.DataSource = electricalConsumerBinding

     'ResultCards

         'IDLE
         Dim idleBinding = new BindingList(Of SmartResult)
         idleBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardIdle)
         idleBinding.AllowNew=true   
         gvResultsCardIdle.DataSource=idleBinding 
                
         'TRACTION
         Dim tractionBinding As BindingList(Of SmartResult)
         tractionBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardTraction)
         tractionBinding.AllowNew=true   
         gvResultsCardTraction.DataSource = tractionBinding
        
         'OVERRUN
         Dim overrunBinding As BindingList(Of SmartResult)
         overrunBinding = New BindingList(Of SmartResult)(auxEnvironment.ElectricalUserInputsConfig.ResultCardOverrun)
         overrunBinding.AllowNew=true   
         gvResultsCardOverrun.DataSource=overrunBinding



End Sub





Private Sub gvElectricalConsumables_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles gvElectricalConsumables.CellEndEdit

End Sub




Private Sub gvElectricalConsumables_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating


   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
   Dim message As String = String.Empty

   If Not column.ReadOnly Then
       e.Cancel = Not IsValidElectricalConsumableEdit(column, e.FormattedValue, message)
   End If





End Sub


Private Function IsValidElectricalConsumableEdit(column As DataGridViewColumn, val As String, ByRef message As String) As Boolean

    Dim s As Single

    Dim tip = column.CellType



    Select Case column.Name

     Case "NominalConsumptionAmps"
           Return True

     Case "NumberInActualVehicle"
           If Not IsNumeric(val) Then
             MessageBox.Show("This value must be numeric")
             Return False
          Else
            s = Single.Parse(val)
           End If
           If s Mod 1 > 0 OrElse s < 0 Then
              MessageBox.Show("This value must be a positive whole number ( Integer ) ")
              Return False
           End If


     Case "PhaseIdle_TractionOn"
           If Not IsNumeric(val) Then
             MessageBox.Show("This value must be numeric")
             Return False
           Else
            s = Single.Parse(val)
           End If
           If s < 0 OrElse s > 1 Then
              MessageBox.Show("This must be a value between 0 and 1 ")
              Return False
           End If


    End Select



    Return True

End Function


Private Sub Panel1_Paint( sender As Object,  e As PaintEventArgs) Handles Panel1.Paint

End Sub
End Class