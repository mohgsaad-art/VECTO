' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports System.IO
Imports VectoAuxiliaries.DownstreamModules
Imports System.Windows.Forms
Imports Newtonsoft.Json
imports VectoAuxiliaries

<Serializable()>
Public Class AuxiliaryConfig
 Implements IAuxiliaryConfig

   'Vecto
   Public Property VectoInputs As IVectoInputs  implements IAuxiliaryConfig.VectoInputs
    
   'Electrical
   Public property ElectricalUserInputsConfig As IElectricsUserInputsConfig  Implements IAuxiliaryConfig.ElectricalUserInputsConfig
   
   
   'Pneumatics
   public Property PneumaticUserInputsConfig As IPneumaticUserInputsConfig Implements IAuxiliaryConfig.PneumaticUserInputsConfig
   public Property PneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig  Implements IAuxiliaryConfig.PneumaticAuxillariesConfig
   
   'Hvac
   Public Property  HvacUserInputsConfig As IHVACUserInputsConfig Implements IAuxiliaryConfig.HvacUserInputsConfig
   
   'Vecto Signals
   public Property Signals As ISignals
   
   
   'Constructors
   Sub new()

   Call Me.New("EMPTY")

 End Sub
   Public Sub new(auxConfigFile As String)

  'Special Condition
  If auxConfigFile="EMPTY" then 
      ElectricalUserInputsConfig = New  ElectricsUserInputsConfig() With { .PowerNetVoltage= 28.3}
      ElectricalUserInputsConfig.ElectricalConsumers= New ElectricalConsumerList(28.3,0.096,false)
      ElectricalUserInputsConfig.ResultCardIdle = new ResultCard( New List(Of SmartResult ))
      ElectricalUserInputsConfig.ResultCardOverrun= new ResultCard( New List(Of SmartResult ))
      ElectricalUserInputsConfig.ResultCardTraction= new ResultCard( New List(Of SmartResult ))
      PneumaticAuxillariesConfig= New PneumaticsAuxilliariesConfig(False)
      PneumaticUserInputsConfig= New PneumaticUserInputsConfig(False)
      HvacUserInputsConfig = New HVACUserInputsConfig(New HVACSteadyStateModel(), String.Empty, String.Empty)
  Exit sub
  
  End If
  
  If auxConfigFile is Nothing orelse auxConfigFile.Trim().Length=0 orelse Not FILE.Exists(auxConfigFile)  then
  
      setdefaults()
  
      Else
      
      setDefaults()
      'ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear
      If Not Load(auxConfigFile)
        MessageBox.Show(String.Format("Unable to load file  {0}", auxConfigFile))
      End If
  
  End If

End Sub
   
   'Set Default Values
   Private Sub setDefaults()

'.CycleDurationMinutes=51.9,

 VectoInputs = New VectoInputs With {.Cycle="Urban", .VehicleWeightKG=16500, .PowerNetVoltage=28.3, .FuelMap="testFuelGoodMap.vmap"}
 
 'Pneumatics
 PneumaticUserInputsConfig  = New PneumaticUserInputsConfig(true) 
 PneumaticAuxillariesConfig = New PneumaticsAuxilliariesConfig(true)



'testAlternatorMap.aalt
 ElectricalUserInputsConfig = New  ElectricsUserInputsConfig() With {.DoorActuationTimeSecond=4, 
                                                                     .AlternatorGearEfficiency=0.92,
                                                                     .PowerNetVoltage= VectoInputs.PowerNetVoltage,
                                                                     .ResultCardIdle= New  ResultCard( New List(Of SmartResult)),
                                                                     .ResultCardOverrun= New ResultCard(New List(Of SmartResult)),
                                                                     .ResultCardTraction=New  ResultCard(New List(Of SmartResult)),
                                                                     .SmartElectrical=True,
                                                                     .AlternatorMap=String.Empty
                                                                     }

 HvacUserInputsConfig = New HVACUserInputsConfig( New HVACSteadyStateModel(100,100,100), String.Empty, String.Empty)


 Signals = New Signals With { .EngineSpeed=2000, .TotalCycleTimeSeconds=3114, .ClutchEngaged=False}


 ElectricalUserInputsConfig.ElectricalConsumers= New ElectricalConsumerList(28.3,0.096,true)




End Sub
   
   Private Function GetDoorActuationTimeFraction()As Single

 Dim actuationsMap as PneumaticActuationsMAP = New PneumaticActuationsMAP( PneumaticUserInputsConfig.ActuationsMap )
 Dim actuationsKey As ActuationsKey = New ActuationsKey( "Park brake + 2 doors",VectoInputs.Cycle)

 Dim numActuations       as single = actuationsMap.GetNumActuations( actuationsKey)
 Dim secondsPerActuation As single = ElectricalUserInputsConfig.DoorActuationTimeSecond

 Dim doorDutyCycleFraction as Single = (numActuations * secondsPerActuation)/Signals.TotalCycleTimeSeconds

 Return doorDutyCycleFraction

End Function
   
   #Region "Comparison"

Private function CompareElectricalConfiguration( other as AuxiliaryConfig) as boolean

'AlternatorGearEfficiency
If Me.ElectricalUserInputsConfig.AlternatorGearEfficiency<> other.ElectricalUserInputsConfig.AlternatorGearEfficiency  then return false

'AlternatorMap
If Me.ElectricalUserInputsConfig.AlternatorMap<> other.ElectricalUserInputsConfig.AlternatorMap  then return false

'DoorActuationTimeSecond
If Me.ElectricalUserInputsConfig.DoorActuationTimeSecond<> other.ElectricalUserInputsConfig.DoorActuationTimeSecond  then return false


'Consumer list
If Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Count<> other.ElectricalUserInputsConfig.ElectricalConsumers.Items.count  then return false
Dim i As integer
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
Private Function ComparePneumaticAuxiliariesConfig( other As AuxiliaryConfig ) As Boolean

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
Private Function ComparePneumaticUserConfig( other As AuxiliaryConfig ) As Boolean

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
Private Function CompareHVACConfig( other As AuxiliaryConfig) As Boolean Implements IAuxiliaryConfig.ConfigValuesAreTheSameAs

  If Me.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts <> other.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts then Return false
  If Me.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour <> other.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour then Return false
  If Me.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts <> other.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts then Return false
  If Me.HvacUserInputsConfig.SSMFilePath <> other.HvacUserInputsConfig.SSMFilePath then Return false
  If Me.HvacUserInputsConfig.BusDatabasePath <> other.HvacUserInputsConfig.BusDatabasePath then Return false

  Return true

End Function

Public Function ConfigValuesAreTheSameAs( other As AuxiliaryConfig) As Boolean

   If Not CompareElectricalConfiguration     ( other ) then Return False
   If Not ComparePneumaticAuxiliariesConfig ( other ) then Return False
   If Not ComparePneumaticUserConfig        ( other ) then Return False
   If Not CompareHVACConfig                 ( other ) then Return False
      
   Return true

End Function


#End Region
   
   #Region "Persistance"


'Persistance Functions
Public Function Save(  auxFile As String ) As Boolean  Implements IAuxiliaryConfig.Save

  Dim returnValue As Boolean = true
  Dim settings As JsonSerializerSettings = new JsonSerializerSettings()
  settings.TypeNameHandling = TypeNameHandling.Objects

   'JSON METHOD
   try
   
      Dim  output As string = JsonConvert.SerializeObject(me, Formatting.Indented, settings)
     
      File.WriteAllText(auxFile , output)
     
      Catch ex as Exception
      
        'TODO:Do something meaningfull here perhaps logging
         returnValue= False
     
    End Try
  
  Return returnValue

End Function
Public Function Load(  auxFile As String  ) As Boolean  Implements IAuxiliaryConfig.Load

  Dim returnValue As Boolean = true
  Dim settings As JsonSerializerSettings = new JsonSerializerSettings()
  Dim tmpAux As AuxiliaryConfig

  settings.TypeNameHandling = TypeNameHandling.Objects

   'JSON METHOD
   try
      
     Dim output As String  = File.ReadAllText(auxFile)
   

     tmpAux =  JsonConvert.DeserializeObject( Of AuxiliaryConfig)(output,settings)
   
     'This is where we Assume values of loaded( Deserialized ) object.
      AssumeValuesOfOther( tmpAux ) 
   
    Catch ex as Exception
    
      'TODO:Do something meaningfull here perhaps logging
    
       returnValue= False
    End Try
  
  Return returnValue

End Function

'Persistance Helpers
Public sub AssumeValuesOfOther( other As AuxiliaryConfig )

   CloneElectricaConfiguration( other )
   ClonePneumaticsAuxiliariesConfig( other )
   ClonePneumaticsUserInputsConfig(other)
   CloneHVAC(other)

End sub
Private sub CloneElectricaConfiguration( other as AuxiliaryConfig) 

'AlternatorGearEfficiency
me.ElectricalUserInputsConfig.AlternatorGearEfficiency  =  other.ElectricalUserInputsConfig.AlternatorGearEfficiency
'AlternatorMap
me.ElectricalUserInputsConfig.AlternatorMap             = other.ElectricalUserInputsConfig.AlternatorMap
'DoorActuationTimeSecond
me.ElectricalUserInputsConfig.DoorActuationTimeSecond   = other.ElectricalUserInputsConfig.DoorActuationTimeSecond

'Electrical Consumer list
Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Clear
For  Each otherConsumer As IElectricalConsumer In other.ElectricalUserInputsConfig.ElectricalConsumers.Items    
  
      Dim  newConsumer  As ElectricalConsumer = New ElectricalConsumer( otherConsumer.BaseVehicle,            _
                                                                        otherConsumer.Category,               _
                                                                        otherConsumer.ConsumerName,           _
                                                                        otherConsumer.NominalConsumptionAmps, _
                                                                        otherConsumer.PhaseIdle_TractionOn,   _
                                                                        otherConsumer.PowerNetVoltage,        _
                                                                        otherConsumer.NumberInActualVehicle   )
       
      Me.ElectricalUserInputsConfig.ElectricalConsumers.Items.Add( newConsumer )

Next

'PowerNetVoltage
Me.ElectricalUserInputsConfig.PowerNetVoltage = other.ElectricalUserInputsConfig.PowerNetVoltage 
'ResultCardIdle
Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Clear
For each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardIdle.Results
       Me.ElectricalUserInputsConfig.ResultCardIdle.Results.Add( New SmartResult(result.Amps,result.SmartAmps))    
Next
'ResultCardOverrun
Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Clear
For each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardOverrun.Results
        Me.ElectricalUserInputsConfig.ResultCardOverrun.Results.Add( New SmartResult(result.Amps,result.SmartAmps))       
Next
'ResultCardTraction
Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Clear
For each result As SmartResult In other.ElectricalUserInputsConfig.ResultCardTraction.Results
        Me.ElectricalUserInputsConfig.ResultCardTraction.Results.Add( New SmartResult(result.Amps,result.SmartAmps))          
Next
'SmartElectrical
Me.ElectricalUserInputsConfig.SmartElectrical = other.ElectricalUserInputsConfig.SmartElectrical 

End Sub
Private sub ClonePneumaticsAuxiliariesConfig( other as AuxiliaryConfig) 

 Me.PneumaticAuxillariesConfig.AdBlueNIperMinute                        =other.PneumaticAuxillariesConfig.AdBlueNIperMinute 
 Me.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute       =other.PneumaticAuxillariesConfig.AirControlledSuspensionNIperMinute 
 Me.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG                 =other.PneumaticAuxillariesConfig.BrakingNoRetarderNIperKG 
 Me.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG               =other.PneumaticAuxillariesConfig.BrakingWithRetarderNIperKG 
 Me.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM           =other.PneumaticAuxillariesConfig.BreakingPerKneelingNIperKGinMM 
 Me.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour          =other.PneumaticAuxillariesConfig.DeadVolBlowOutsPerLitresperHour 
 Me.PneumaticAuxillariesConfig.DeadVolumeLitres                         =other.PneumaticAuxillariesConfig.DeadVolumeLitres 
 Me.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand      =other.PneumaticAuxillariesConfig.NonSmartRegenFractionTotalAirDemand 
 Me.PneumaticAuxillariesConfig.PerDoorOpeningNI                         =other.PneumaticAuxillariesConfig.PerDoorOpeningNI 
 Me.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG             =other.PneumaticAuxillariesConfig.PerStopBrakeActuationNIperKG 
 Me.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand         =other.PneumaticAuxillariesConfig.SmartRegenFractionTotalAirDemand 
 Me.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction =other.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction 

End Sub
Private Sub ClonePneumaticsUserInputsConfig ( other As AuxiliaryConfig )

  Me.PneumaticUserInputsConfig.ActuationsMap = other.PneumaticUserInputsConfig.ActuationsMap 
  Me.PneumaticUserInputsConfig.AdBlueDosing = other.PneumaticUserInputsConfig.AdBlueDosing 
  Me.PneumaticUserInputsConfig.AirSuspensionControl = other.PneumaticUserInputsConfig.AirSuspensionControl 
  Me.PneumaticUserInputsConfig.CompressorGearEfficiency = other.PneumaticUserInputsConfig.CompressorGearEfficiency
  Me.PneumaticUserInputsConfig.CompressorGearRatio = other.PneumaticUserInputsConfig.CompressorGearRatio 
  Me.PneumaticUserInputsConfig.CompressorMap = other.PneumaticUserInputsConfig.CompressorMap 
  Me.PneumaticUserInputsConfig.Doors = other.PneumaticUserInputsConfig.Doors 
  Me.PneumaticUserInputsConfig.KneelingHeightMillimeters = other.PneumaticUserInputsConfig.KneelingHeightMillimeters 
  Me.PneumaticUserInputsConfig.RetarderBrake = other.PneumaticUserInputsConfig.RetarderBrake 
  Me.PneumaticUserInputsConfig.SmartAirCompression = other.PneumaticUserInputsConfig.SmartAirCompression 
  Me.PneumaticUserInputsConfig.SmartRegeneration = other.PneumaticUserInputsConfig.SmartRegeneration 


End Sub
Private Sub CloneHVAC( other As AuxiliaryConfig)

  Me.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts = other.HvacUserInputsConfig.SteadyStateModel.HVACElectricalLoadPowerWatts 
  Me.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour    = other.HvacUserInputsConfig.SteadyStateModel.HVACFuellingLitresPerHour 
  Me.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts = other.HvacUserInputsConfig.SteadyStateModel.HVACMechanicalLoadPowerWatts 

  Me.HvacUserInputsConfig.SSMFilePath = other.HvacUserInputsConfig.SSMFilePath
  Me.HvacUserInputsConfig.BusDatabasePath = other.HvacUserInputsConfig.BusDatabasePath

End Sub




#End Region


End Class

