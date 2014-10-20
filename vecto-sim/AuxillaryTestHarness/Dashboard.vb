Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics

Public Class Dashboard

public auxEnvironment As New AuxillaryEnvironment("")

Private Sub Dashboard_Load( sender As Object,  e As EventArgs) Handles MyBase.Load

cboCycle.SelectedIndex=0

  SetupControls()

  CreateBindings()


End Sub

Private Sub cboCycle_SelectedIndexChanged( sender As Object,  e As EventArgs) Handles cboCycle.SelectedIndexChanged

End Sub

Private sub SetupControls()

     Dim cIndex As Integer=0

     'ElectricalConsumerGrid 
     'Columns
     cIndex=gvElectricalConsumables.Columns.Add("Category","Category") 
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="Category"
          gvElectricalConsumables.Columns(cIndex).MinimumWidth=150
     gvElectricalConsumables.Columns(cIndex).ReadOnly=True
              
     cIndex=gvElectricalConsumables.Columns.Add("ConsumerName","Name") 
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="ConsumerName"
     gvElectricalConsumables.Columns(cIndex).MinimumWidth=300
     gvElectricalConsumables.Columns(cIndex).ReadOnly=True

     Dim baseVehicle As New DataGridViewCheckBoxColumn(false)
     baseVehicle.HeaderText="BaseVehicle"
     cIndex=gvElectricalConsumables.Columns.Add(baseVehicle)
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="BaseVehicle"
     gvElectricalConsumables.Columns(cIndex).Width=60

     cIndex=gvElectricalConsumables.Columns.Add("NominalConsumptionAmps","Nominal Amps") 
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="NominalConsumptionAmps"
     gvElectricalConsumables.Columns(cIndex).Width=50

     cIndex=gvElectricalConsumables.Columns.Add("PhaseIdle_TractionOn","PhaseIdle\n/TractionOn") 
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="PhaseIdle_TractionOn"
     gvElectricalConsumables.Columns(cIndex).Width=50


     cIndex=gvElectricalConsumables.Columns.Add("NumberInActualVehicle","Num in Vehicle") 
     gvElectricalConsumables.Columns(cIndex).DataPropertyName="NumberInActualVehicle"
     gvElectricalConsumables.Columns(cIndex).Width=50

End Sub

Private Sub CreateBindings

     'AuxEnvironment.Vecto Bindings
     txtPowernetVoltage.DataBindings.Add("Text", auxEnvironment.VectoInputs, "PowerNetVoltage")
     txtVehicleWeightKG.DataBindings.Add("Text",auxEnvironment.VectoInputs,"VehicleWeightKG")
     cboCycle.DataBindings.Add("Text",auxEnvironment.VectoInputs,"Cycle")

     'Electricals General
     txtAlternatorMapPath.DataBindings.Add("Text",auxEnvironment.ElectricalUserInputsConfig,"AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig,"AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text",auxEnvironment.ElectricalUserInputsConfig,"DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked",auxEnvironment.ElectricalUserInputsConfig,"SmartElectrical")

     'ConsumablesGrid
     gvElectricalConsumables.DataSource = auxEnvironment.ElectricalUserInputsConfig.ElectricalConsumers.Items

     'ResultCards



End Sub


Private Sub gvElectricalConsumables_CellEndEdit( sender As Object,  e As DataGridViewCellEventArgs) Handles gvElectricalConsumables.CellEndEdit

End Sub




Private Sub gvElectricalConsumables_CellValidating( sender As Object,  e As DataGridViewCellValidatingEventArgs) Handles gvElectricalConsumables.CellValidating


   Dim column As DataGridViewColumn = gvElectricalConsumables.Columns(e.ColumnIndex)
   Dim message As String = String.Empty
   
   If Not column.ReadOnly then
       e.Cancel = Not IsValidElectricalConsumableEdit(column, e.FormattedValue, message)
   End If

      



End Sub


Private Function IsValidElectricalConsumableEdit( column As DataGridViewColumn, val As String , byref  message As string) As Boolean

    dim s As Single

    Dim tip =  column.CellType



    Select Case column.Name

     case "NominalConsumptionAmps"
           Return true

     case "NumberInActualVehicle"
           If not IsNumeric(val) 
             MessageBox.Show("This value must be numeric")
             Return false
          Else     
            s = Single.Parse(val)
           End If
           If s MOD 1 > 0 orelse s<0 then
              MessageBox.Show("This value must be a positive whole number ( Integer ) ")
              Return false
           End If


     case "PhaseIdle_TractionOn"
           If not IsNumeric(val) 
             MessageBox.Show("This value must be numeric")
             Return false
           Else     
            s = Single.Parse(val)
           End If
           If s < 0 orelse s>1 then
              MessageBox.Show("This must be a value between 0 and 1 ")
              Return false
           End If
           

    End Select



    Return true

End Function


End Class