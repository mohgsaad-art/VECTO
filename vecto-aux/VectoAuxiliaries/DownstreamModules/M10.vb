Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules

 Public Class M10
  Implements IM10


  'Aggregators
  Private _AverageAirConsumedPerSecondLitre As Single

  'Diagnostics
  Private   Dim x1,y1,x2,y2,x3,y3, xTA,interp1,interp2 As single

  Public ReadOnly Property P1X as single Implements IM10.P1X
      Get
       Return x1
      End Get
  End Property        
  Public ReadOnly Property P1Y as single Implements IM10.P1Y
      Get
      Return y1
      End Get
  End Property        
  Public ReadOnly Property P2X as single Implements IM10.P2X
      Get
      Return x2
      End Get
  End Property        
  Public ReadOnly Property P2Y  as single Implements IM10.P2Y
      Get
      Return y2
      End Get
  End Property       
  Public ReadOnly Property P3X as single Implements IM10.P3X
      Get
      Return x3
      End Get
  End Property        
  Public ReadOnly Property P3Y as single Implements IM10.P3Y
      Get
      Return y3
      End Get
  End Property        
  Public ReadOnly Property XTAIN  as single Implements IM10.XTAIN
      Get
      Return xTA
      End Get
  End Property     
  Public ReadOnly Property INTRP1  as single Implements IM10.INTRP1
      Get
      Return interp1
      End Get
  End Property    
  Public ReadOnly Property INTRP2   as single Implements IM10.INTRP2
      Get
      Return interp2
      End Get
  End Property   


 
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
 ' Dim x1,y1,x2,y2,x3,y3, xTA As Single
  
  x1=m9.LitresOfAirCompressorOnContinually
  y1=m9.TotalCycleFuelConsumptionCompressorOnContinuously
  x2=0
  y2=m9.TotalCycleFuelConsumptionCompressorOffContinuously
  x3=m9.LitresOfAirCompressorOnOnlyInOverrun
  y3=m9.TotalCycleFuelConsumptionCompressorOffContinuously


  xTA   = _AverageAirConsumedPerSecondLitre  'm3.AverageAirConsumedPerSecondLitre

  



  Select Case  interpType
  
      'Non-Smart Pneumatics ( OUT 1 )
      Case InterpolationType.NonSmartPneumtaics
      returnValue = y2 + ( ((y1-y2) * xTA) / x1)
      interp1=returnValue
         
      'Smart Pneumatics ( OUT 2 )
      Case InterpolationType.SmartPneumtaics 
      ReturnValue = y3 + (((y1-y3) /( x1-x3)) * ( xTA - x3))
      interp2= returnValue

      
  
  End Select
  
  
  Return returnValue

End Function
 
 'Public 
 #Region "Public Properties"

 Public ReadOnly Property AverageLoadsFuelConsumptionInterpolatedForPneumatics As Single Implements IM10.AverageLoadsFuelConsumptionInterpolatedForPneumatics
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
 
 Public Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0) Implements IM10.CycleStep

  _AverageAirConsumedPerSecondLitre+= If( Single.IsNaN(m3.AverageAirConsumedPerSecondLitre),0,m3.AverageAirConsumedPerSecondLitre )

 End Sub




 End Class


End Namespace






