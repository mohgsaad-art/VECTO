Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports System.Windows.Forms

Public Class AdvancedAuxiliaries
 Implements IAdvancedAuxiliaries




    Private auxConfig As AuxiliaryConfig
     

    Public Property Signals As ISignals Implements IAdvancedAuxiliaries.Signals
    Public Property VectoInputs As IVectoInputs Implements IAdvancedAuxiliaries.VectoInputs

   
    Public Sub new( )

           VectoInputs = New VectoInputs() 
           Signals     = New Signals()

    End Sub


 


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

Public Sub Initialise( auxPath  As String )

auxConfig = New AuxiliaryConfig(auxPath)

'Pass some signals from config to Signals. ( These are stored in the configuration but shared in the signal distribution around modules )
Signals.SmartElectrics  = auxConfig.ElectricalUserInputsConfig.SmartElectrical
Signals.SmartPneumatics = auxConfig.PneumaticUserInputsConfig.SmartAirCompression

Dim alternatoMap As IAlternatorMap = New AlternatorMap(auxConfig.ElectricalUserInputsConfig.AlternatorMap)
alternatoMap.Initialise()

Dim actuationsMap As  IPneumaticActuationsMAP = New PneumaticActuationsMAP( auxConfig.PneumaticUserInputsConfig.ActuationsMap)

Dim compressorMap As ICompressorMap = New CompressorMap( auxConfig.PneumaticUserInputsConfig.CompressorMap)
compressorMap.Initialise()

Dim fuelMap As IFUELMAP = New cMAP()
fuelMap.FilePath= VectoInputs.FuelMap
If Not fuelMap.ReadFile() then 
MessageBox.Show("Unable to read fuel map, aborting.")
return
End If
fuelMap.Triangulate()


auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.DoorDutyCycleFraction = GetDoorActuationTimeFraction()


M0 = New M0_NonSmart_AlternatorsSetEfficiency( auxConfig.ElectricalUserInputsConfig.ElectricalConsumers,
                                               New HVACInputs,
                                               alternatoMap,
                                               auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,
                                               Signals,
                                               auxConfig.HvacUserInputsConfig.SteadyStateModel)


M05 = New M0_5_SmartAlternatorSetEfficiency(M0, 
                                            auxConfig.ElectricalUserInputsConfig.ElectricalConsumers, 
                                            alternatoMap,
                                            auxConfig.ElectricalUserInputsConfig.ResultCardIdle,
                                            auxConfig.ElectricalUserInputsConfig.ResultCardTraction,
                                            auxConfig.ElectricalUserInputsConfig.ResultCardOverrun,Signals)


M1 = New M1_AverageHVACLoadDemand(M0,
                                  New HVACMap(""),
                                  New HVACInputs(), 
                                  auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency, 
                                  auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency,
                                  auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,
                                  Signals,
                                  auxConfig.HvacUserInputsConfig.SteadyStateModel)


M2 = New M2_AverageElectricalLoadDemand(auxConfig.ElectricalUserInputsConfig.ElectricalConsumers,
                                        M0,
                                        auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency, 
                                        auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,Signals )



M3 = New M3_AveragePneumaticLoadDemand(auxConfig.PneumaticUserInputsConfig,
         auxConfig.PneumaticAuxillariesConfig,
         actuationsMap,
         compressorMap, 
         VectoInputs.VehicleWeightKG,
         VectoInputs.Cycle,
         Signals)


M4 = New M4_AirCompressor(compressorMap,auxConfig.PneumaticUserInputsConfig.CompressorGearRatio,auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency,Signals)
M5 = New M5__SmartAlternatorSetGeneration( M05, VectoInputs.PowerNetVoltage,auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency)
M6 = New M6(M1,M2,M3,M4,M5,Signals)
M7 = New M7(M5,M6,Signals)
M8 = New M8(M1,M6,M7,Signals)
M9 = New M9(M1,M4,M6,M8,fuelMap,auxConfig.PneumaticAuxillariesConfig,Signals)
M10 = New M10(M3,M9,Signals)
M11 = New M11(M1,M3,M6,M8,fuelMap,Signals)
M12 = New M12( M11, Signals )
M13 = New M13(M1,M10,M12,Signals)


End Sub



    Public Function Configure(filePath As String, vectoFilePath As String ) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.Configure
    
    try

             Dim frmAuxiliaryConfig As New frmAuxiliaryConfig( filePath, vectoFilePath)

             frmAuxiliaryConfig.Show()

    Catch ex As Exception

     Return False

    Return false

    End Try


    Return true

    End Function

    Public Function CycleStep(seconds As Integer, ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.CycleStep
         
       M9.CycleStep( seconds )      
       M11.CycleStep( seconds )
     
     
    End Function

    Public Event Message(Message As String, messageType As VectoAuxiliaries.AdvancedAuxiliaryMessageType) Implements VectoAuxiliaries.IAdvancedAuxiliaries.Message

    Public ReadOnly Property Running As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.Running
        Get
              throw new NotImplementedException
        End Get
    End Property

    Public Function RunStart( ByVal auxFilePath As String, ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.RunStart
          

       Initialise(auxFilePath)        
       'CycleStep( Signals.TotalCycleTimeSeconds, message)

    End Function

    Public Function RunStop(ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.RunStop
          throw new NotImplementedException
    End Function

'

    Public ReadOnly Property TotalFuelGRAMS As Single Implements VectoAuxiliaries.IAdvancedAuxiliaries.TotalFuelGRAMS
        Get
             If Not M13 is Nothing then

               Return M13.TotalCycleFuelConsumptionGrams

               Else
               'TODO:Issue a message            
               Return 0

             End If

             
        End Get
    End Property

    Public ReadOnly Property TotalFuelLITRES As Single Implements VectoAuxiliaries.IAdvancedAuxiliaries.TotalFuelLITRES
        Get
             If Not M13 is Nothing then

               Return M13.TotalCycleFuelConsumptionLitres

               Else
               'TODO:Issue a message
               Return 0

             End If
        End Get
    End Property

    Public ReadOnly Property AuxiliaryName As String Implements VectoAuxiliaries.IAdvancedAuxiliaries.AuxiliaryName
        Get
          Return "BusAuxiliaries"
        End Get
    End Property

    Public ReadOnly Property AuxiliaryVersion As String Implements VectoAuxiliaries.IAdvancedAuxiliaries.AuxiliaryVersion
        Get
           Return "Version 1.0 Beta"
        End Get
    End Property




    Private Function GetDoorActuationTimeFraction()As Single
   
    Dim actuationsMap as PneumaticActuationsMAP = New PneumaticActuationsMAP( auxConfig.PneumaticUserInputsConfig.ActuationsMap )
    Dim actuationsKey As ActuationsKey = New ActuationsKey( "Park brake + 2 doors",VectoInputs.Cycle)
   
    Dim numActuations       as single = actuationsMap.GetNumActuations( actuationsKey)
    Dim secondsPerActuation As single = auxConfig.ElectricalUserInputsConfig.DoorActuationTimeSecond
   
    Dim doorDutyCycleFraction as Single = (numActuations * secondsPerActuation)/Signals.TotalCycleTimeSeconds
   
    Return doorDutyCycleFraction
   
   End Function

End Class
