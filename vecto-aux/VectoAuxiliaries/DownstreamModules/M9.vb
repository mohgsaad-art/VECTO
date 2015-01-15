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

Namespace DownstreamModules

  Public Class M9
    Implements  IM9

  Private Const RPM_TO_RADS_PER_SECOND As Single = 9.55f
 
  #Region "Aggregates"

'AG1
Private _LitresOfAirCompressorOnContinuallyAggregate As Single
'AG2
Private _LitresOfAirCompressorOnOnlyInOverrunAggregate As Single
'AG3
Private _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate As Single
'AG4
Private _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate As Single

#End Region
  #Region "Constructor Requirements"

Private M1 As IM1_AverageHVACLoadDemand
Private M4 As IM4_AirCompressor
Private M6 As IM6
Private M8 As IM8
Private FMAP As IFUELMAP
Private PSAC As IPneumaticsAuxilliariesConfig
Private Signals As ISignals

#end region


  #Region "Class Outputs"
  'OUT 1
  Public ReadOnly Property LitresOfAirCompressorOnContinually As Single Implements IM9.LitresOfAirCompressorOnContinually
            Get
              Return _LitresOfAirCompressorOnContinuallyAggregate
            End Get
        End Property
  'OUT 2
  Public ReadOnly Property LitresOfAirCompressorOnOnlyInOverrun As Single Implements IM9.LitresOfAirCompressorOnOnlyInOverrun
            Get
             Return  _LitresOfAirCompressorOnOnlyInOverrunAggregate
            End Get
        End Property
  'OUT 3
  Public ReadOnly Property TotalCycleFuelConsumptionCompressorOffContinuously As Single Implements IM9.TotalCycleFuelConsumptionCompressorOffContinuously
            Get
             Return _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate
            End Get
        End Property
  'OUT 4
  Public ReadOnly Property TotalCycleFuelConsumptionCompressorOnContinuously As Single Implements IM9.TotalCycleFuelConsumptionCompressorOnContinuously
            Get
            Return _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate
            End Get
        End Property

#End Region

  'Staging Calculations
  Private Function S0( byval rpm As Single ) As Single

  If rpm<1 then rpm=1
  
  Return rpm / RPM_TO_RADS_PER_SECOND

End Function
  
  Private readonly Property S1 As single
    Get
     Return M6.AvgPowerDemandAtCrankFromElectricsIncHVAC + M1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
    End Get
End Property
  Private readonly Property S2 As single
    Get
      If S0( Signals.EngineSpeed)=0 then Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
      Return M4.GetPowerCompressorOn/S0( Signals.EngineSpeed)
    End Get
End Property
  Private readonly Property S3 As single
    Get
       If S0( Signals.EngineSpeed)=0 then Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
       Return M4.GetPowerCompressorOff/ S0( Signals.EngineSpeed)
    End Get
End Property
  Private readonly Property S4 As single
    Get
    If S0( Signals.EngineSpeed)=0 then Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
     Return S1/S0( Signals.EngineSpeed)
    End Get
End Property
  Private readonly Property S5 As single
    Get
      Return S2 + Signals.EngineDrivelineTorque
    End Get
End Property
  Private readonly Property S6 As single 
    Get
     Return Signals.EngineDrivelineTorque + s3
    End Get
End Property
  Private readonly Property S7 As single
    Get
      Return S4 + S5
    End Get
End Property
  Private readonly Property S8 As single
    Get
     Return S4 + S6
    End Get
End Property
  Private readonly Property S9 As single
    Get
     Return M4.GetFlowRate * M6.OverrunFlag * M8.CompressorFlag
    End Get
End Property
  Private ReadOnly Property S10 As Single
     Get
     Return S9 * PSAC.OverrunUtilisationForCompressionFraction
     End Get
 End Property
  private ReadOnly Property S11 As Single
     Get
      'SCHM 3_02
       Dim int1 As Single = FMAP.fFCdelaunay_Intp(Signals.EngineSpeed,s7)   
       int1 = If( int1 >0 Andalso Not Single.IsNaN(int1), int1,0)
       
      'TODO:REMOVE ONCE TESTED OK
      'Return FMAP.fFCdelaunay_Intp(Signals.EngineSpeed,s7) / 3600

       Return int1/3600 

     End Get
 End Property
  private ReadOnly Property S12 As Single
      Get
         'Divide by 3600 to get grams per second.
         'SCHM 3_02
         Dim int2 As Single = FMAP.fFCdelaunay_Intp(Signals.EngineSpeed,s8)
         int2 = If( int2 >0 Andalso Not Single.IsNaN(int2), int2,0)

         'TODO:REMOVE ONCE TESTED OK
         'return   FMAP.fFCdelaunay_Intp(Signals.EngineSpeed,s8) /3600

         Return  int2 /3600

      End Get
  End Property
  
  Private ReadOnly Property SW1 As Integer
     Get
       Return If( Signals.EngineStopped,0,1)
     End Get
 End Property

  'Utility Methods
  Public Sub ClearAggregates() Implements IM9.ClearAggregates

          _LitresOfAirCompressorOnContinuallyAggregate =0
          _LitresOfAirCompressorOnOnlyInOverrunAggregate =0
          _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate =0
          _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate=0

        End Sub
  Public Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0) Implements IM9.CycleStep

          If Signals.EngineStopped then return

          _LitresOfAirCompressorOnContinuallyAggregate +=stepTimeInSeconds* M4.GetFlowRate * sw1
          _LitresOfAirCompressorOnOnlyInOverrunAggregate +=stepTimeInSeconds * s10 * sw1
          _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate+= stepTimeInSeconds * s11 * sw1
          _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate +=stepTimeInSeconds * s12 * sw1

  End Sub

 'Constructor
  Public Sub new ( m1 As IM1_AverageHVACLoadDemand, m4 As IM4_AirCompressor, m6 As IM6, m8 As IM8, fmap As IFUELMAP, psac As IPneumaticsAuxilliariesConfig, signals As ISignals)
                Me.M1=m1
                Me.M4=m4
                Me.M6=m6
                Me.M8=m8
                Me.FMAP=fmap
                Me.PSAC= psac
                Me.Signals=signals  

        End Sub

 'Auxiliary Event
  Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) Implements IAuxiliaryEvent.AuxiliaryEvent


  End Class

End Namespace



