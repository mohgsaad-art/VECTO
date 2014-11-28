Imports System.Reflection
Imports VectoAuxiliaries

Public Class AuxLauncher

Public advancedAuxiliaries As IAdvancedAuxiliaries


'Configure
Private Sub btnLaunchAux_Click(sender As Object, e As EventArgs) Handles btnLaunchAux.Click



  If Not advancedAuxiliaries.Configure(txtAdvancedAuxiliaries.Text, "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto") then

  MessageBox.Show("Unable to configure Advanced Auxilliaries")

  End If





End Sub


'Run


'Stop


'Set Statics



'Invoke Messages


'Information



Private Sub txtAdvancedAuxiliaries_Validating( sender As Object,  e As System.ComponentModel.CancelEventArgs) Handles txtAdvancedAuxiliaries.Validating


'Check for correct extension
Dim message As String = String.Empty

If VectoAuxiliaries.FilePathUtils.ValidateFilePath( txtAdvancedAuxiliaries.Text,".aaux", message)=False

  messageBox.Show(message)
  e.Cancel=true
  

End If



End Sub




Private Sub btnRun_Click( sender As Object,  e As EventArgs) Handles btnRun.Click

Dim message As String = String.Empty



'Set Statics
advancedAuxiliaries.VectoInputs.Cycle="Urban"
advancedAuxiliaries.VectoInputs.VehicleWeightKG=16500
advancedAuxiliaries.VectoInputs.FuelMap= "testFuelGoodMap.vmap"
advancedAuxiliaries.VectoInputs.PowerNetVoltage=26.3


'set Signals
advancedAuxiliaries.Signals.EngineSpeed=1500
advancedAuxiliaries.Signals.TotalCycleTimeSeconds=3114

advancedAuxiliaries.RunStart(txtAdvancedAuxiliaries.Text,message)

'step whole cycle



End Sub



Private Sub AuxLauncher_Load( sender As Object,  e As EventArgs) Handles MyBase.Load



 Dim obj As System.Runtime.Remoting.ObjectHandle




Try
  obj = Activator.CreateInstance("VectoAuxiliaries", "VectoAuxiliaries.AdvancedAuxiliaries")


  advancedAuxiliaries = DirectCast(obj.Unwrap, IAdvancedAuxiliaries)

 ' If Not advancedAuxiliaries.Configure(txtAdvancedAuxiliaries.Text, "C:\Users\tb28\Source\Workspaces\VECTO\vecto-sim-ricardoaeaTB\AuxillaryTestHarness\bin\Debug\vectopath.vecto") then

    ' MessageBox.Show("Unable to configure Advanced Auxilliaries")

 ' End If

  Catch ex As Exception


    Dim a As String= "tdgfdgfdg"

  
  

  Finally


  End Try


End Sub



End Class