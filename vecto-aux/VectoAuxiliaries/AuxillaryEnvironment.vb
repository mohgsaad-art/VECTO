
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
public Property PneumaticUserInputsConfig As IPneumaticUserInputsConfig
public Property PneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig

 'Hvac
 Public Property  HvacUserInputsConfig As IHVACUserInputsConfig

 'Vecto Signals
 public Property Signals As ISignals





 'Test instantiations


  Public M0 As IM0_NonSmart_AlternatorsSetEfficiency
  Public M05 As IM0_5_SmartAlternatorSetEfficiency
  Public M1 As IM1_AverageHVACLoadDemand
  Public M2 As IM2_AverageElectricalLoadDemand
  Public M3 As IM3_AveragePneumaticLoadDemand
  Public M4 As IM4_AirCompressor
  Public M5 As IM5_SmartAlternatorSetGeneration


Public Sub Initialise()




Dim alternatoMap As IAlternatorMap = New AlternatorMap(ElectricalUserInputsConfig.AlternatorMap)
alternatoMap.Initialise()

Dim actuationsMap As  IPneumaticActuationsMAP = New PneumaticActuationsMAP( PneumaticUserInputsConfig.ActuationsMap)

Dim compressorMap As ICompressorMap = New CompressorMap( PneumaticUserInputsConfig.CompressorMap)
compressorMap.Initialise()

ElectricalUserInputsConfig.ElectricalConsumers.DoorDutyCycleFraction = GetDoorActuationTimeFraction()


M0 = New M0_NonSmart_AlternatorsSetEfficiency( ElectricalUserInputsConfig.ElectricalConsumers,
                                               New HVACInputs,
                                               alternatoMap,
                                               ElectricalUserInputsConfig.PowerNetVoltage,
                                               Signals,
                                               HvacUserInputsConfig.SteadyStateModel)


M05 = New M0_5_SmartAlternatorSetEfficiency(M0, 
                                            ElectricalUserInputsConfig.ElectricalConsumers, 
                                            alternatoMap,
                                            ElectricalUserInputsConfig.ResultCardIdle,
                                            ElectricalUserInputsConfig.ResultCardTraction,
                                            ElectricalUserInputsConfig.ResultCardTraction,Signals)


M1 = New M1_AverageHVACLoadDemand(M0,
                                  New HVACMap(""),
                                  New HVACInputs(), 
                                  ElectricalUserInputsConfig.AlternatorGearEfficiency, 
                                  PneumaticUserInputsConfig.CompressorGearEfficiency,
                                  ElectricalUserInputsConfig.PowerNetVoltage,
                                  Signals,
                                  HvacUserInputsConfig.SteadyStateModel)


M2 = New M2_AverageElectricalLoadDemand(ElectricalUserInputsConfig.ElectricalConsumers,
                                        M0,
                                        ElectricalUserInputsConfig.AlternatorGearEfficiency, 
                                        ElectricalUserInputsConfig.PowerNetVoltage,Signals )



M3 = New M3_AveragePneumaticLoadDemand(PneumaticUserInputsConfig,
         PneumaticAuxillariesConfig,
         actuationsMap,
         compressorMap, 
         VectoInputs.VehicleWeightKG,
         VectoInputs.Cycle,
         VectoInputs.CycleDurationMinutes)


M4 = New M4_AirCompressor(compressorMap,PneumaticUserInputsConfig.CompressorGearRatio,PneumaticUserInputsConfig.CompressorGearEfficiency,Signals)


M5 = New M5__SmartAlternatorSetGeneration( M05, VectoInputs.PowerNetVoltage,ElectricalUserInputsConfig.AlternatorGearEfficiency)



End Sub
 
Public Sub new(auxConfigFile As String)


If auxConfigFile.Trim().Length=0 orelse Not FILE.Exists(auxConfigFile) then

    setdefaults()

End If

End Sub

Private Sub setDefaults()

'Here's where the magic happens.

 VectoInputs = New VectoInputs With {.Cycle="Urban", .VehicleWeightKG=16500, .PowerNetVoltage=26.3, .CycleDurationMinutes=51.9}
 
 'Pneumatics
 PneumaticUserInputsConfig  = New PneumaticUserInputsConfig(true) 
 PneumaticAuxillariesConfig = New PneumaticsAuxilliariesConfig(true)






 ElectricalUserInputsConfig = New  ElectricsUserInputsConfig() With {.DoorActuationTimeSecond=4, 
                                                                     .AlternatorGearEfficiency=0.8,
                                                                     .PowerNetVoltage= VectoInputs.PowerNetVoltage,
                                                                     .ResultCardIdle= New  ResultCard( New List(Of SmartResult)),
                                                                     .ResultCardOverrun= New ResultCard(New List(Of SmartResult)),
                                                                     .ResultCardTraction=New  ResultCard(New List(Of SmartResult)),
                                                                     .SmartElectrical=True,
                                                                     .AlternatorMap="testAlternatorMap.csv"
                                                                     }

 HvacUserInputsConfig = New HVACUserInputsConfig( New HVACSteadyStateModel(100,100,100))


 Signals = New Signals With { .EngineSpeed=2000, .TotalCycleTimeSeconds=3114, .ClutchEngaged=False}


 'Set Electricals.


 Dim doorDutyCycleFraction as Single = GetDoorActuationTimeFraction

 ElectricalUserInputsConfig.ElectricalConsumers= New ElectricalConsumerList(VectoInputs.PowerNetVoltage,doorDutyCycleFraction,true)




End Sub

Private Function GetDoorActuationTimeFraction()As Single

 Dim actuationsMap as PneumaticActuationsMAP = New PneumaticActuationsMAP( PneumaticUserInputsConfig.ActuationsMap )
 Dim actuationsKey As ActuationsKey = New ActuationsKey( "Park brake + 2 doors",VectoInputs.Cycle)

 Dim numActuations       as single = actuationsMap.GetNumActuations( actuationsKey)
 Dim secondsPerActuation As single = ElectricalUserInputsConfig.DoorActuationTimeSecond

 Dim doorDutyCycleFraction as Single = (numActuations * secondsPerActuation)/Signals.TotalCycleTimeSeconds

 Return doorDutyCycleFraction

End Function


End Class

