Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports System.Windows.Forms

Public Class AdvancedAuxiliaries
 Implements IAdvancedAuxiliaries






    Private  auxConfig As AuxiliaryConfig


    'Supporting classes which may generate event messages
    Private WithEvents compressorMap As ICompressorMap
    Private Withevents alternatorMap  As IAlternatorMap 
    Private WithEvents actuationsMap As IPneumaticActuationsMAP
    Private WithEvents fuelMap       As IFUELMAP 

    'Classes which compose the model.
    private WithEvents M0  As IM0_NonSmart_AlternatorsSetEfficiency
    private WithEvents M05 As IM0_5_SmartAlternatorSetEfficiency
    private WithEvents M1  As IM1_AverageHVACLoadDemand
    private WithEvents M2  As IM2_AverageElectricalLoadDemand
    private WithEvents M3  As IM3_AveragePneumaticLoadDemand
    private WithEvents M4  As IM4_AirCompressor
    private WithEvents M5  As IM5_SmartAlternatorSetGeneration
    private WithEvents M6  As IM6
    private WithEvents M7  As IM7
    private WithEvents M8  As IM8
    private WithEvents M9  As IM9
    private WithEvents M10 As IM10
    private WithEvents M11 As IM11
    private WithEvents M12 As IM12
    private WithEvents M13 As IM13
    private vectoDirectory As String 

    'Event Handler top level bubble.
    Public Sub VectoEventHandler( byref sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) handles compressorMap.AuxiliaryEvent, alternatorMap.AuxiliaryEvent


    If Signals.AuxiliaryEventReportingLevel <= messageType then

        RaiseEvent AuxiliaryEvent( sender, message,messageType)

    End If
        


  End Sub   

    'Constructor
    Public Sub new( )

           VectoInputs = New VectoInputs() 
           Signals     = New Signals()

    End Sub

    'Initialise Model
    Public Sub Initialise( IAuxPath  As String , vectoFilePath As string)

      Dim auxPath As string
      vectoDirectory  =fPATH(vectoFilePath)

      auxPath = FilePathUtils.ResolveFilePath(vectoDirectory,IAuxPath)


      Signals.CurrentCycleTimeInSeconds=0
      auxConfig = New AuxiliaryConfig(auxPath)
      
      'Pass some signals from config to Signals. ( These are stored in the configuration but shared in the signal distribution around modules )
      Signals.SmartElectrics  = auxConfig.ElectricalUserInputsConfig.SmartElectrical
      Signals.SmartPneumatics = auxConfig.PneumaticUserInputsConfig.SmartAirCompression
      
      alternatorMap  = New AlternatorMap(FilePathUtils.ResolveFilePath(vectoDirectory,auxConfig.ElectricalUserInputsConfig.AlternatorMap))
      alternatorMap.Initialise()
      
      actuationsMap = New PneumaticActuationsMAP( FilePathUtils.ResolveFilePath(vectoDirectory,auxConfig.PneumaticUserInputsConfig.ActuationsMap))
      
      compressorMap  = New CompressorMap(FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.PneumaticUserInputsConfig.CompressorMap))
      compressorMap.Initialise()
      
      fuelMap  = New cMAP()
      fuelMap.FilePath= FilePathUtils.ResolveFilePath(vectoDirectory,VectoInputs.FuelMap)
      If Not fuelMap.ReadFile() then 
      MessageBox.Show("Unable to read fuel map, aborting.")
      return
      End If
      fuelMap.Triangulate()
      
      
      auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.DoorDutyCycleFraction = GetDoorActuationTimeFraction( )
      
      
      M0 = New M0_NonSmart_AlternatorsSetEfficiency( auxConfig.ElectricalUserInputsConfig.ElectricalConsumers,
                                                     alternatorMap,
                                                     auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,
                                                     Signals,
                                                     auxConfig.HvacUserInputsConfig.SteadyStateModel)
      
      
      M05 = New M0_5_SmartAlternatorSetEfficiency(M0, 
                                                  auxConfig.ElectricalUserInputsConfig.ElectricalConsumers, 
                                                  alternatorMap,
                                                  auxConfig.ElectricalUserInputsConfig.ResultCardIdle,
                                                  auxConfig.ElectricalUserInputsConfig.ResultCardTraction,
                                                  auxConfig.ElectricalUserInputsConfig.ResultCardOverrun,Signals)
      
      
      M1 = New M1_AverageHVACLoadDemand(M0,
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
      M5 = New M5__SmartAlternatorSetGeneration( M05, auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency)
      M6 = New M6(M1,M2,M3,M4,M5,Signals)
      M7 = New M7(M5,M6,Signals)
      M8 = New M8(M1,M6,M7,Signals)
      M9 = New M9(M1,M4,M6,M8,fuelMap,auxConfig.PneumaticAuxillariesConfig,Signals)
      M10 = New M10(M3,M9,Signals)
      M11 = New M11(M1,M3,M6,M8,fuelMap,Signals)
      M12 = New M12(M10, M11, Signals )
      M13 = New M13(M1,M10,M12,Signals)
    
    
End Sub
  
    #Region "Interface implementation"

    Public Property Signals As ISignals Implements IAdvancedAuxiliaries.Signals
    Public Property VectoInputs As IVectoInputs Implements IAdvancedAuxiliaries.VectoInputs

    Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements IAuxiliaryEvent.AuxiliaryEvent

    Public Function Configure(filePath As String, vectoFilePath As String ) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.Configure
    
    try

             Dim frmAuxiliaryConfig As New frmAuxiliaryConfig( filePath, vectoFilePath)

             frmAuxiliaryConfig.Show()

             If frmAuxiliaryConfig.DialogResult<>DialogResult.OK then

               Return true

               Else
               
               Return False

             End If


    Catch ex As Exception

     Return False

    Return false

    End Try


    Return true

    End Function

    Public Function CycleStep(seconds As Integer, ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.CycleStep
         
      
       try

       M9.CycleStep( seconds ) 
       M10.CycleStep( seconds )     
       M11.CycleStep( seconds )

       Signals.CurrentCycleTimeInSeconds+=1


       Catch ex As Exception
          'TODO: Should this raise an event ?
          MessageBox.Show("Im an exception")
          Return false

       end try


     
       Return true
     
    End Function

    Public ReadOnly Property Running As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.Running
        Get
              throw new NotImplementedException
        End Get
    End Property

    Public Function RunStart( ByVal auxFilePath As String,byval vectoFilePath as string , ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.RunStart
          
          Try
          Initialise(auxFilePath, vectoFilePath)  

          Catch ex As Exception

          Return false

          End Try
 
       
       'TODO:Modify Initialise to return a Bool.
       Return true     

    End Function

    Public Function RunStop(ByRef message As String) As Boolean Implements VectoAuxiliaries.IAdvancedAuxiliaries.RunStop
          throw new NotImplementedException
    End Function

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


    #End Region

    'Helpers
    Private Function GetDoorActuationTimeFraction()As Single
   
    Dim actuationsMap as PneumaticActuationsMAP = New PneumaticActuationsMAP( FilePathUtils.ResolveFilePath(  vectoDirectory, auxConfig.PneumaticUserInputsConfig.ActuationsMap ))
    Dim actuationsKey As ActuationsKey = New ActuationsKey( "Park brake + 2 doors",VectoInputs.Cycle)
   
    Dim numActuations       as single = actuationsMap.GetNumActuations( actuationsKey)
    Dim secondsPerActuation As single = auxConfig.ElectricalUserInputsConfig.DoorActuationTimeSecond
   
    Dim doorDutyCycleFraction as Single = (numActuations * secondsPerActuation)/Signals.TotalCycleTimeSeconds
   
    Return doorDutyCycleFraction
   
   End Function
 
    Public Function ValidateAAUXFile(filePath As String, ByRef message As String) As Boolean Implements IAdvancedAuxiliaries.ValidateAAUXFile


     Dim validResult As Boolean =  FilePathUtils.ValidateFilePath( filePath, ".aaux", message)

     If Not validResult then Return False


     Return true

    End Function

    'MOD
    Public ReadOnly Property AA_NonSmartAlternatorsEfficiency As Single? Implements IAdvancedAuxiliaries.AA_NonSmartAlternatorsEfficiency
        Get
          Return M0.AlternatorsEfficiency
        End Get
    End Property

    Public ReadOnly Property AA_SmartIdleCurrent_Amps As Single? Implements IAdvancedAuxiliaries.AA_SmartIdleCurrent_Amps
        Get
         Return M05.SmartIdleCurrent
        End Get
    End Property

    Public ReadOnly Property AA_SmartIdleAlternatorsEfficiency As Single? Implements IAdvancedAuxiliaries.AA_SmartIdleAlternatorsEfficiency
        Get
         Return M05.AlternatorsEfficiencyIdleResultCard
        End Get
    End Property

    Public ReadOnly Property AA_SmartTractionCurrent_Amps As Single? Implements IAdvancedAuxiliaries.AA_SmartTractionCurrent_Amps
        Get
         Return M05.SmartTractionCurrent
        End Get
    End Property

    Public ReadOnly Property AA_SmartTractionAlternatorEfficiency As Single? Implements IAdvancedAuxiliaries.AA_SmartTractionAlternatorEfficiency
        Get
         Return M05.AlternatorsEfficiencyTractionOnResultCard
        End Get
    End Property

    Public ReadOnly Property AA_SmartOverrunCurrent_Amps As Single? Implements IAdvancedAuxiliaries.AA_SmartOverrunCurrent_Amps
        Get
         Return M05.SmartOverrunCurrent
        End Get
    End Property

    Public ReadOnly Property AA_SmartOverrunAlternatorEfficiency As Single? Implements IAdvancedAuxiliaries.AA_SmartOverrunAlternatorEfficiency
        Get
         Return M05.AlternatorsEfficiencyOverrunResultCard
        End Get
    End Property

    Public ReadOnly Property AA_CompressorFlowRate_LitrePerSec As Single? Implements IAdvancedAuxiliaries.AA_CompressorFlowRate_LitrePerSec
        Get
         Return  M4.GetAveragePowerDemandPerCompressorUnitFlowRate
        End Get
    End Property

    Public ReadOnly Property AA_OverrunFlag As Integer? Implements IAdvancedAuxiliaries.AA_OverrunFlag
        Get
         Return M6.OverrunFlag
        End Get
    End Property

    Public ReadOnly Property AA_EngineIdleFlag As Integer? Implements IAdvancedAuxiliaries.AA_EngineIdleFlag
        Get
         Return  Signals.Idle
        End Get
    End Property

    Public ReadOnly Property AA_CompressorFlag As Integer? Implements IAdvancedAuxiliaries.AA_CompressorFlag
        Get
         Return M8.CompressorFlag
        End Get
    End Property

    Public ReadOnly Property AA_TotalCycleFC_Grams As Single? Implements IAdvancedAuxiliaries.AA_TotalCycleFC_Grams
        Get
         Return M13.TotalCycleFuelConsumptionGrams
        End Get
    End Property

    Public ReadOnly Property AA_TotalCycleFC_Litres As Single? Implements IAdvancedAuxiliaries.AA_TotalCycleFC_Litres
        Get
         Return M13.TotalCycleFuelConsumptionLitres
        End Get
    End Property



    Public ReadOnly Property AuxiliaryPowerAtCrankWatts As Single Implements IAdvancedAuxiliaries.AuxiliaryPowerAtCrankWatts
        Get
          Return M8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries
        End Get
    End Property



    'TODO:REMOVE WHEN TESTING IS COMPLETE
    'PURE DIAGNOSTICS SHOULD ONLY BE USED IN  MOD FOR ENGINEERING TESTS

    Public ReadOnly Property AA_D_M10_INTERP1 As Single Implements IAdvancedAuxiliaries.AA_D_M12_INTERP1
        Get
         Return M12.INTRP1
        End Get
    End Property
    Public ReadOnly Property AA_D_M10_INTERP2 As Single Implements IAdvancedAuxiliaries.AA_D_M12_INTERP2
        Get
         Return M12.INTRP2
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P1X As Single Implements IAdvancedAuxiliaries.AA_D_M12_P1X
        Get
          Return M12.P1X
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P1Y As Single Implements IAdvancedAuxiliaries.AA_D_M12_P1Y
        Get
         Return M12.P1Y
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P2X As Single Implements IAdvancedAuxiliaries.AA_D_M12_P2X
        Get
         Return M12.P2X
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P2Y As Single Implements IAdvancedAuxiliaries.AA_D_M12_P2Y
        Get
          Return M12.P2Y
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P3X As Single Implements IAdvancedAuxiliaries.AA_D_M12_P3X
        Get
          Return M12.P3X
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_P3Y As Single Implements IAdvancedAuxiliaries.AA_D_M12_P3Y
        Get
         Return M12.P3Y
        End Get
    End Property
    Public ReadOnly Property AA_D_M12_XTAIN As Single Implements IAdvancedAuxiliaries.AA_D_M12_XTAIN
        Get
          Return M12.XTAIN
        End Get
    End Property



End Class
