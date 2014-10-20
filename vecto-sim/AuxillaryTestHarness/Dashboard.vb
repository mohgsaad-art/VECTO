Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics

Public Class Dashboard

public auxEnvironment As New AuxillaryEnvironment("")

Private Sub Dashboard_Load( sender As Object,  e As EventArgs) Handles MyBase.Load

cboCycle.SelectedIndex=0

  CreateBindings()

End Sub

Private Sub cboCycle_SelectedIndexChanged( sender As Object,  e As EventArgs) Handles cboCycle.SelectedIndexChanged

End Sub

Private Sub CreateBindings

     'auxEnvironment.Vecto Bindings
     txtPowernetVoltage.DataBindings.Add("Text", auxEnvironment.VectoInputs, "PowerNetVoltage")
     txtVehicleWeightKG.DataBindings.Add("Text",auxEnvironment.VectoInputs,"VehicleWeightKG")
     cboCycle.DataBindings.Add("Text",auxEnvironment.VectoInputs,"Cycle")

     'Electricals

     'UserInput
     txtAlternatorMapPath.DataBindings.Add("Text",auxEnvironment.ElectricalUserInputsConfig,"AlternatorMap")
     txtAlternatorGearEfficiency.DataBindings.Add("Text", auxEnvironment.ElectricalUserInputsConfig,"AlternatorGearEfficiency")
     txtDoorActuationTimeSeconds.DataBindings.Add("Text",auxEnvironment.ElectricalUserInputsConfig,"DoorActuationTimeSecond")
     chkSmartElectricals.DataBindings.Add("Checked",auxEnvironment.ElectricalUserInputsConfig,"SmartElectrical")


     'Dim cIndex As Integer = gvElectricalConsumables.Columns.Add("ConsumerName","ConsumerName")
     'gvElectricalConsumables.Columns(cIndex).DataPropertyName="Value.ConsumerName"

     'Dim list As New List(of IElectricalConsumer)

     'gvElectricalConsumables.DataSource = list


End Sub


End Class