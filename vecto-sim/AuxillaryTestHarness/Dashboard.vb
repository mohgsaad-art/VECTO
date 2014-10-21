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

     'ConsumablesGrid
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
        
            dgv.Rows(e.RowIndex).Selected = true
            Dim rowIndex As Integer  = e.RowIndex
            dgv.CurrentCell = dgv.Rows(e.RowIndex).Cells(1)
            resultCardContextMenu.Show(dgv, e.Location)
            resultCardContextMenu.Show(Cursor.Position)

        End if


    end sub

    private sub resultCardContextMenu_Click( sender As object,  e as  EventArgs) Handles resultCardContextMenu.Click
    
         Dim menu As ContextMenuStrip = CType( sender, ContextMenuStrip)

         Dim grid as DataGridView  = DirectCast( menu.SourceControl, DataGridView)

         'DirectCast(menu.SourceControl,System.Windows.Forms.DataGridView).SelectedRows(0).IsNewRow

        If Not grid.SelectedRows(0).IsNewRow then
        
            grid.Rows.RemoveAt(grid.SelectedRows(0).Index)

        End if

    end sub



End Class