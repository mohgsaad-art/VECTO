Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports System.IO
Imports VectoAuxiliaries.DownstreamModules
Imports System.Windows.Forms
Imports Newtonsoft.Json


<Serializable()>
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
  Public M6 As IM6
  Public M7 As IM7
  Public M8 As IM8
  Public M9 As IM9
  Public M10 As IM10
  Public M11 As IM11
  Public M12 As IM12
  Public M13 As IM13
  
  
Public Sub ClearDown()

     M0  = Nothing
     M05 = Nothing
     M1  = Nothing
     M2  = Nothing
     M3  = Nothing
     M4  = Nothing
     M5  = Nothing
     M6  = Nothing
     M7  = Nothing
     M8  = Nothing
     M9  = Nothing
     M10 = Nothing
     M11 = Nothing
     M12 = Nothing
     M13 = Nothing

End Sub

Public Sub Initialise()


Dim alternatoMap As IAlternatorMap = New AlternatorMap(ElectricalUserInputsConfig.AlternatorMap)
alternatoMap.Initialise()

Dim actuationsMap As  IPneumaticActuationsMAP = New PneumaticActuationsMAP( PneumaticUserInputsConfig.ActuationsMap)

Dim compressorMap As ICompressorMap = New CompressorMap( PneumaticUserInputsConfig.CompressorMap)
compressorMap.Initialise()

Dim fuelMap As IFUELMAP = New cMAP()
fuelMap.FilePath= VectoInputs.FuelMap
If Not fuelMap.ReadFile() then 
MessageBox.Show("Unable to read fuel map, aborting.")
return
End If
fuelMap.Triangulate()


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
                                            ElectricalUserInputsConfig.ResultCardOverrun,Signals)


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
         Signals)


M4 = New M4_AirCompressor(compressorMap,PneumaticUserInputsConfig.CompressorGearRatio,PneumaticUserInputsConfig.CompressorGearEfficiency,Signals)


M5 = New M5__SmartAlternatorSetGeneration( M05, VectoInputs.PowerNetVoltage,ElectricalUserInputsConfig.AlternatorGearEfficiency)


M6 = New M6(M1,M2,M3,M4,M5,Signals)


M7 = New M7(M5,M6,Signals)

M8 = New M8(M1,M6,M7,Signals)

M9 = New M9(M1,M4,M6,M8,fuelMap,PneumaticAuxillariesConfig,Signals)

M10 = New M10(M3,M9,Signals)

M11 = New M11(M1,M3,M6,M8,fuelMap,Signals)

M12 = New M12( M11, Signals )

M13 = New M13(M1,M10,M12,Signals)


End Sub
 
Public Sub new(auxConfigFile As String)


If auxConfigFile is Nothing orelse auxConfigFile.Trim().Length=0 orelse Not FILE.Exists(auxConfigFile) then

    setdefaults()

End If

End Sub

Private Sub setDefaults()

 'Here's where the magic happens.

 'TODO:
 'Assign Smarts to signals
 'Signals.SmartElectrics= ElectricalUserInputsConfig.SmartElectrical
 'Signals.SmartPneumatics= PneumaticUserInputsConfig.SmartAirCompression


 VectoInputs = New VectoInputs With {.Cycle="Urban", .VehicleWeightKG=16500, .PowerNetVoltage=26.3, .CycleDurationMinutes=51.9,.FuelMap="testFuelGoodMap.vmap"}
 
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


Private function CompareElectricaConfiguration( other as AuxillaryEnvironment) as boolean

'AlternatorGearEfficiency
If Me.ElectricalUserInputsConfig.AlternatorGearEfficiency<> other.ElectricalUserInputsConfig.AlternatorGearEfficiency  then return false

'AlternatorMap
If Me.ElectricalUserInputsConfig.AlternatorMap<> other.ElectricalUserInputsConfig.AlternatorMap  then return false

'DoorActuationTimeSecond
If Me.ElectricalUserInputsConfig.DoorActuationTimeSecond<> other.ElectricalUserInputsConfig.DoorActuationTimeSecond  then return false


'Consumer list
If Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count<> other.ElectricalUserInputsConfig.ElectricalConsumers.Items.count  then return false
For i=0 to Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count-1
       Dim thisConsumer, otherConsumer As IElectricalConsumer
       thisConsumer = Me.ElectricalUserInputsConfig.ElectricalConsumers.Items(i)
       otherConsumer = other.ElectricalUserInputsConfig.ElectricalConsumers.Items(i)

       If  thisConsumer.AvgConsumptionAmps         <> otherConsumer.AvgConsumptionAmps             OrElse _
           thisConsumer.BaseVehicle                <> otherConsumer.BaseVehicle                    OrElse _
           thisConsumer.Category                   <> otherConsumer.Category                       OrElse _
           thisConsumer.ConsumerName               <> otherConsumer.ConsumerName                   OrElse _
           thisConsumer.NominalConsumptionAmps     <> otherConsumer.NominalConsumptionAmps         OrElse _
           thisConsumer.NumberInActualVehicle      <> otherConsumer.NumberInActualVehicle          OrElse _ 
           thisConsumer.PhaseIdle_TractionOn       <> otherConsumer.PhaseIdle_TractionOn           OrElse _
           thisConsumer.TotalAvgConsumptionInWatts <> otherConsumer.TotalAvgConsumptionInWatts     OrElse _
           thisConsumer.TotalAvgConumptionAmps     <> otherConsumer.TotalAvgConumptionAmps         Then Return False  

Next

'PowerNetVoltage
If Me.ElectricalUserInputsConfig.PowerNetVoltage <> other.ElectricalUserInputsConfig.PowerNetVoltage then Return False

'ResultCardIdle
If  Me.ElectricalUserInputsConfig.ResultCardIdle.Results.count <> other.ElectricalUserInputsConfig.ResultCardIdle.Results.Count then Return False
For i = 0 to Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Count-1
       If Me.ElectricalUserInputsConfig.ResultCardIdle.Results(i).Amps      <> other .ElectricalUserInputsConfig.ResultCardIdle.Results(i).Amps       OrElse _
          Me.ElectricalUserInputsConfig.ResultCardIdle.Results(i).SmartAmps <> other .ElectricalUserInputsConfig.ResultCardIdle.Results(i).SmartAmps  Then Return False        
Next

'ResultCardOverrun
If  Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.count <> other.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count then Return False
For i = 0 to Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Count-1
       If Me.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).Amps      <> other .ElectricalUserInputsConfig.ResultCardOverrun.Results(i).Amps       OrElse _
          Me.ElectricalUserInputsConfig.ResultCardOverrun.Results(i).SmartAmps <> other .ElectricalUserInputsConfig.ResultCardOverrun.Results(i).SmartAmps  Then Return False        
Next


'ResultCardTraction
If  Me.ElectricalUserInputsConfig.ResultCardTraction.Results.count <> other.ElectricalUserInputsConfig.ResultCardTraction.Results.Count then Return False
For i = 0 to Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Count-1
       If Me.ElectricalUserInputsConfig.ResultCardTraction.Results(i).Amps      <> other .ElectricalUserInputsConfig.ResultCardTraction.Results(i).Amps       OrElse _
          Me.ElectricalUserInputsConfig.ResultCardTraction.Results(i).SmartAmps <> other .ElectricalUserInputsConfig.ResultCardTraction.Results(i).SmartAmps  Then Return False        
Next

'SmartElectrical
If Me.ElectricalUserInputsConfig.SmartElectrical <> other.ElectricalUserInputsConfig.SmartElectrical then Return False


Return true


End Function
Private Function ComparePneumaticAuxiliariesConfig( other As AuxillaryEnvironment ) As Boolean

 If Me.PneumaticAuxillariesConfig.AdBlueNIperMinute <> other.PneumaticAuxillariesConfig.AdBlueNIperMinute then Return False
 If Me.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute <> other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute then Return False 
 If Me.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG <> other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG then Return False
 If Me.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG <> other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG then Return False
 If Me.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM <> other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM then Return False
 If Me.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour <> other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour then Return False
 If Me.PneumaticAuxillariesConfig.DeadVolumeLitres <> other.PneumaticAuxillariesConfig.DeadVolumeLitres then Return False
 If Me.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand <> other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand then Return False
 If Me.PneumaticAuxillariesConfig.PerDoorOpeningNI <> other.PneumaticAuxillariesConfig.PerDoorOpeningNI then Return False
 If Me.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG <> other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG then Return False
 If Me.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand <> other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand then Return False
 If Me.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction <> other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction then Return False

 Return true

End Function
Private Function ComparePneumaticUserConfig( other As AuxillaryEnvironment ) As Boolean

 If Me.PneumaticUserInputsConfig.ActuationsMap <> other.PneumaticUserInputsConfig.ActuationsMap then Return False
 If Me.PneumaticUserInputsConfig.AdBlueDosing <> other.PneumaticUserInputsConfig.AdBlueDosing then Return False 
 If Me.PneumaticUserInputsConfig.AirSuspensionControl <> other.PneumaticUserInputsConfig.AirSuspensionControl then Return False
 If Me.PneumaticUserInputsConfig.CompressorGearEfficiency <> other.PneumaticUserInputsConfig.CompressorGearEfficiency then Return False
 If Me.PneumaticUserInputsConfig.CompressorGearRatio <> other.PneumaticUserInputsConfig.CompressorGearRatio then Return False
 If Me.PneumaticUserInputsConfig.CompressorMap <> other.PneumaticUserInputsConfig.CompressorMap then Return False
 If Me.PneumaticUserInputsConfig.Doors <> other.PneumaticUserInputsConfig.Doors then Return False
 If Me.PneumaticUserInputsConfig.KneelingHeightMillimeters <> other.PneumaticUserInputsConfig.KneelingHeightMillimeters then Return False
 If Me.PneumaticUserInputsConfig.RetarderBrake <> other.PneumaticUserInputsConfig.RetarderBrake then Return False
 If Me.PneumaticUserInputsConfig.SmartAirCompression <> other.PneumaticUserInputsConfig.SmartAirCompression then Return False
 If Me.PneumaticUserInputsConfig.SmartRegeneration <> other.PneumaticUserInputsConfig.SmartRegeneration then Return False


 Return true

End Function
Private Function CompareHVACConfig( other As AuxillaryEnvironment) As Boolean

  If Me.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts <> other.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts then Return false
  If Me.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour <> other.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour then Return false
  If Me.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts <> other.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts then Return false

  Return true

End Function


Public Function ShallowCopy() As AuxillaryEnvironment
    Return DirectCast(Me.MemberwiseClone(), AuxillaryEnvironment)
  End Function

Public Function ConfigValuesAreTheSameAs( other As AuxillaryEnvironment) As Boolean

   If Not CompareElectricaConfiguration     ( other ) then Return False
   If Not ComparePneumaticAuxiliariesConfig ( other ) then Return False
   If Not ComparePneumaticUserConfig        ( other ) then Return False
   If Not CompareHVACConfig                 ( other ) then Return False
      
   Return true

End Function

End Class

