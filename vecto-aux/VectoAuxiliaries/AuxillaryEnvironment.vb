
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports System.IO


Public Class AuxillaryEnvironment

 'Vecto
 Public Property VectoInputs As IVectoInputs
  
 'Electrical
 Public property ElectricalUserInputsConfig As IElectricsUserInputsConfig


 'Pneumatics
 Private Property PneumaticUserInputsConfig As IPneumaticUserInputsConfig
 Private Property PneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig

 'Hvac
 Public Property  HvacUserInputsConfig As IHVACUserInputsConfig

 'Vecto Signals
 public Property Signals As ISignals

 
Public Sub new(auxConfigFile As String)


If auxConfigFile.Trim().Length=0 orelse Not FILE.Exists(auxConfigFile) then

    setdefaults()

End If

End Sub

Private Sub setDefaults()

'Here's where the magic happens.

 VectoInputs = New VectoInputs With {.Cycle="Urban", .VehicleWeightKG=16500, .PowerNetVoltage=26.3}
 
 'Pneumatics
 PneumaticUserInputsConfig  = New PneumaticUserInputsConfig(true) 
 PneumaticAuxillariesConfig = New PneumaticsAuxilliariesConfig(true)

 ElectricalUserInputsConfig = New  ElectricsUserInputsConfig() With {.DoorActuationTimeSecond=4, 
                                                                     .ElectricalConsumers= New ElectricalConsumerList(VectoInputs.PowerNetVoltage,0.1,true),
                                                                     .PowerNetVoltage= VectoInputs.PowerNetVoltage,
                                                                     .ResultCardIdleAmps= New Dictionary(Of Single, Single),
                                                                     .ResultCardOverrunAmps= New Dictionary(Of Single, Single),
                                                                     .ResultCardTractionAmps=New Dictionary(Of Single, Single),
                                                                     .SmartElectrical=True}

 HvacUserInputsConfig = New HVACUserInputsConfig(1,1,New HVACInputs(),"HVACMAPPATHGOESHERE.CSV")

End Sub


End Class

