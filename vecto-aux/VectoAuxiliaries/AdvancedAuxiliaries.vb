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
    Public Sub Initialise( auxPath  As String )

      auxConfig = New AuxiliaryConfig(auxPath)
      
      'Pass some signals from config to Signals. ( These are stored in the configuration but shared in the signal distribution around modules )
      Signals.SmartElectrics  = auxConfig.ElectricalUserInputsConfig.SmartElectrical
      Signals.SmartPneumatics = auxConfig.PneumaticUserInputsConfig.SmartAirCompression
      
      alternatorMap  = New AlternatorMap(auxConfig.ElectricalUserInputsConfig.AlternatorMap)
      alternatorMap.Initialise()
      
      actuationsMap = New PneumaticActuationsMAP( auxConfig.PneumaticUserInputsConfig.ActuationsMap)
      
      compressorMap  = New CompressorMap( auxConfig.PneumaticUserInputsConfig.CompressorMap)
      compressorMap.Initialise()
      
      fuelMap  = New cMAP()
      fuelMap.FilePath= VectoInputs.FuelMap
      If Not fuelMap.ReadFile() then 
      MessageBox.Show("Unable to read fuel map, aborting.")
      return
      End If
      fuelMap.Triangulate()
      
      
      auxConfig.ElectricalUserInputsConfig.ElectricalConsumers.DoorDutyCycleFraction = GetDoorActuationTimeFraction()
      
      
      M0 = New M0_NonSmart_AlternatorsSetEfficiency( auxConfig.ElectricalUserInputsConfig.ElectricalConsumers,
                                                     New HVACInputs,
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
         
       M9.CycleStep( seconds )      
       M11.CycleStep( seconds )
     
     
    End Function

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
   
    Dim actuationsMap as PneumaticActuationsMAP = New PneumaticActuationsMAP( auxConfig.PneumaticUserInputsConfig.ActuationsMap )
    Dim actuationsKey As ActuationsKey = New ActuationsKey( "Park brake + 2 doors",VectoInputs.Cycle)
   
    Dim numActuations       as single = actuationsMap.GetNumActuations( actuationsKey)
    Dim secondsPerActuation As single = auxConfig.ElectricalUserInputsConfig.DoorActuationTimeSecond
   
    Dim doorDutyCycleFraction as Single = (numActuations * secondsPerActuation)/Signals.TotalCycleTimeSeconds
   
    Return doorDutyCycleFraction
   
   End Function

  
    Public Function ValidateAAUXFile(filePath As String, ByRef message As String) As Boolean Implements IAdvancedAuxiliaries.ValidateAAUXFile

     Try

       Dim AConfig As New AuxiliaryConfig( filePath )

       If Not AConfig is Nothing then 

         message="OK"
         Return true
       End If

       Return True
         
     Catch ex As Exception

       message= "AAUX File not found, or Invalid "
       Return false
         
     End Try


    End Function



End Class
