Imports System.Reflection
Imports VectoAuxiliaries

Public Class AuxLauncher

Public WithEvents advancedAuxiliaries As IAdvancedAuxiliaries


Public Sub AuxEventHandler( ByRef sender As Object, ByVal message As String, ByVal messageType as AdvancedAuxiliaryMessageType)  Handles advancedAuxiliaries.AuxiliaryEvent

   txtEvents.Text+= message

End Sub


'Configure
Private Sub btnLaunchAux_Click(sender As Object, e As EventArgs) Handles btnLaunchAux.Click

  If Not advancedAuxiliaries.Configure(txtAdvancedAuxiliaries.Text, "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto") then

       MessageBox.Show("Unable to configure Advanced Auxilliaries")



  End If

  
End Sub


Private Sub txtAdvancedAuxiliaries_Validating( sender As Object,  e As System.ComponentModel.CancelEventArgs) Handles txtAdvancedAuxiliaries.Validating


'Check for correct extension
Dim message As String = String.Empty

'Formatted correctely
If VectoAuxiliaries.FilePathUtils.ValidateFilePath( txtAdvancedAuxiliaries.Text,".aaux", message)=False
  messageBox.Show(message)
  e.Cancel=true
End If


'Can load the file.
If Not advancedAuxiliaries.ValidateAAUXFile(txtAdvancedAuxiliaries.Text, message)

  MessageBox.Show(message &  " : Please reenter and try again.")
  e.Cancel=true

End If


End Sub



Private sub setup()

Dim message As String = String.Empty

'Set Statics
advancedAuxiliaries.VectoInputs.Cycle="Urban"
advancedAuxiliaries.VectoInputs.VehicleWeightKG=16500
advancedAuxiliaries.VectoInputs.FuelMap= "testFuelGoodMap.vmap"
advancedAuxiliaries.VectoInputs.PowerNetVoltage=26.3


'set Signals
advancedAuxiliaries.Signals.EngineSpeed=1500
advancedAuxiliaries.Signals.TotalCycleTimeSeconds=3114

advancedAuxiliaries.RunStart(txtAdvancedAuxiliaries.Text,"",message)

End Sub

Private Sub btnRun_Click( sender As Object,  e As EventArgs) Handles btnRun.Click

  setup()

  timer1.Start

End Sub



Private Sub AuxLauncher_Load( sender As Object,  e As EventArgs) Handles MyBase.Load

cboWarningLevel.DataSource = System.Enum.GetValues(GetType(AdvancedAuxiliaryMessageType))
cboWarningLevel.SelectedIndex= AdvancedAuxiliaryMessageType.Critical

 Dim obj As System.Runtime.Remoting.ObjectHandle


Try
  obj = Activator.CreateInstance("BusAuxiliaries", "VectoAuxiliaries.AdvancedAuxiliaries")


      advancedAuxiliaries = DirectCast(obj.Unwrap, IAdvancedAuxiliaries)
      advancedAuxiliaries.Signals.AuxiliaryEventReportingLevel=CType(cboWarningLevel.SelectedValue, AdvancedAuxiliaryMessageType)
      lblAuxiliaryName.Text= advancedAuxiliaries.AuxiliaryName
      lblAuxiliaryVersion.Text = advancedAuxiliaries.AuxiliaryVersion

  Catch ex As Exception

  Finally


  End Try


End Sub



Private Sub Timer1_Tick( sender As Object,  e As EventArgs) Handles Timer1.Tick

If not advancedAuxiliaries is nothing

  Dim message As string = ""

   Timer1.Start

   advancedAuxiliaries.CycleStep(  Timer1.Interval/1000 , Message)
   txtTotalFCGrams.Text= advancedAuxiliaries.TotalFuelGRAMS
   txtTotalFCLitres.Text= advancedAuxiliaries.TotalFuelLITRES

End If


End Sub

Private Sub btnWholeCycle_Click( sender As Object,  e As EventArgs) Handles btnWholeCycle.Click
  Dim message As string = ""

    setup()

   advancedAuxiliaries.CycleStep(  3114 , Message)
   txtTotalFCGrams.Text= advancedAuxiliaries.TotalFuelGRAMS
   txtTotalFCLitres.Text= advancedAuxiliaries.TotalFuelLITRES

End Sub


Private Sub btnStop_Click( sender As Object,  e As EventArgs) Handles btnStop.Click

Timer1.Stop


End Sub



Private Sub cboWarningLevel_SelectionChangeCommitted( sender As Object,  e As EventArgs) Handles cboWarningLevel.SelectionChangeCommitted

advancedAuxiliaries.Signals.AuxiliaryEventReportingLevel=CType(cboWarningLevel.SelectedValue, AdvancedAuxiliaryMessageType)

End Sub


End Class