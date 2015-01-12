Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules

 Public Class M10
  Implements IM10
 
 'Private
 #Region "Private Fields  = > Constructor Requirements"

Private m3 As IM3_AveragePneumaticLoadDemand
Private m9 As IM9
'Not Currently used but there for ease of refactoring in future.
Private signals As ISignals

#End Region
 Private Enum InterpolationType
 NonSmartPneumtaics
 SmartPneumtaics
End Enum
 Private Function Interpolate(  interpType As InterpolationType) As Single

  Dim returnValue As Single
  Dim x1,y1,x2,y2,x3,y3, xTA As Single
  
  x1=m9.LitresOfAirCompressorOnContinually
  y1=m9.TotalCycleFuelConsumptionCompressorOnContinuously
  x2=0
  y2=m9.TotalCycleFuelConsumptionCompressorOffContinuously
  x3=m9.LitresOfAirCompressorOnOnlyInOverrun
  y3=m9.TotalCycleFuelConsumptionCompressorOffContinuously

  xTA   = m3.TotalAirConsumedPerCycle


  
  
  Select Case  interpType
  
      'Non-Smart Pneumatics
      Case InterpolationType.NonSmartPneumtaics
      returnValue = y2 + ( ((y1-y2) * xTA) / x1)
         
      'Smart Pneumatics
      Case InterpolationType.SmartPneumtaics 
      ReturnValue = y3 + (((y1-y3) /( x1-x3)) * ( xTA - x3))

      
  
  End Select
  
  
  Return returnValue

End Function
 
 'Public 
 #Region "Public Properties"

 Public ReadOnly Property BaseFuelConsumptionWithAverageAuxiliaryLoads As Single Implements IM10.AverageLoadsFuelConsumptionInterpolatedForPneumatics
            Get
             Return  If( Single.IsNaN( Interpolate(InterpolationType.NonSmartPneumtaics)),0,Interpolate(InterpolationType.NonSmartPneumtaics))
            End Get
        End Property
 Public ReadOnly Property FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand As Single Implements IM10.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand
            Get
             Return If( Single.IsNaN(Interpolate( InterpolationType.SmartPneumtaics)),0,Interpolate(InterpolationType.SmartPneumtaics))
            End Get
        End Property
 
#End Region
 #Region "Contructors"
  
  Public sub new( m3 As IM3_AveragePneumaticLoadDemand, m9 As IM9, signals As ISignals)

   Me.m3=m3
   Me.m9=m9
   Me.signals= signals
   
End Sub
 
 #end region
 
 End Class


End Namespace






