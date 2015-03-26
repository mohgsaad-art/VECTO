
Namespace Electrics


Public Class Table4Row

  Public RPM As Single
  Public Efficiency As Single

  Public Sub new ( rpm As single, eff  As single)

   Me.rpm= rpm
   Me.Efficiency = eff


  End Sub


End Class


Public Class Alternator
 Implements IAlternator

 Public Property AlternatorName As String Implements IAlternator.AlternatorName
 Public Property  PulleyRatio As Single Implements IAlternator.PulleyRatio

 Public Property InputTable2000 As  New List(Of AltUserInput) Implements IAlternator.InputTable2000  
 Public Property InputTable4000 As  New List(Of AltUserInput) Implements IAlternator.InputTable4000
 Public Property InputTable6000 As  New List(Of AltUserInput) Implements IAlternator.InputTable6000
 Public Property RangeTable     As  New List(Of Table4Row) Implements IAlternator.RangeTable

 Private signals As ICombinedAlternatorSignals 


 Public  Sub  Clone( other As IAlternator) Implements IAlternator.Clone


   
 End Sub
 Public ReadOnly Property Efficiency As Double Implements IAlternator.Efficiency


            Get
               'First build RangeTable, table 4

               InitialiseRangeTable()
               CalculateRangeTable()

               'TODO: Calculate Efficiency
               'Calculate ( Interpolate ) Efficiency


            End Get



        End Property
 Public ReadOnly Property SpindleSpeed As Double Implements IAlternator.SpindleSpeed
            Get
               Return signals.CrankRPM * PulleyRatio
            End Get
        End Property

         
 'Constructors
 Sub new()
   
 End Sub
 Sub new( isignals As ICombinedAlternatorSignals, inputs as List(Of ICombinedAlternatorMapRow))


     If isignals is Nothing then Throw New ArgumentException("Alternator - ISignals supplied is nothing")
     signals = isignals

     Me.AlternatorName= inputs.First().AlternatorName
     Me.PulleyRatio = inputs.First().PulleyRatio

     Dim values2k As  Dictionary(Of single,single) =  inputs.where( function(x) x.RPM=2000).Select( function(x) new KeyValuePair(of single,single)(x.Amps,x.Efficiency)).ToDictionary( Function(x) x.Key, Function(x) x.Value)
     Dim values4k As  Dictionary(Of single,single) =  inputs.where( function(x) x.RPM=4000).Select( function(x) new KeyValuePair(of single,single)(x.Amps,x.Efficiency)).ToDictionary( Function(x) x.Key, Function(x) x.Value)
     Dim values6k As  Dictionary(Of single,single) =  inputs.where( function(x) x.RPM=6000).Select( function(x) new KeyValuePair(of single,single)(x.Amps,x.Efficiency)).ToDictionary( Function(x) x.Key, Function(x) x.Value)

    
     BuildInputTable( values2k, InputTable2000)
     BuildInputTable( values4k, InputTable4000)
     BuildInputTable( values6k, InputTable6000)


     CreateRangeTable()
     InitialiseRangeTable()


 End Sub

 Private Function Iterpolate( values As List(Of AltUserInput), x As single) As Single

    Dim lowestX As single = values.Min( Function(m) m.Amps)
    Dim highestX As Single = values.Max( Function(m) m.Amps)
    Dim lastX, nextX ,lastEff,NextEff As single
    Dim deltaX As single 
    Dim slope As single

    'Out of range, returns efficiency for lowest
    If x< lowestX then Return values.First( Function(f) f.Amps= lowestX).Eff

    'Out of range, efficiency for highest
    If x> highestX then Return values.First( Function(f) f.Amps= highestX).Eff

    'On Bounds check
    If  values.Where( Function(w) w.Amps=x).Count=1 then Return values.First( Function(w) w.Amps=x).Eff


    'OK, we need to interpolate.
    lastX   = values.Last(  Function(l)  l.Amps < x).Amps
    nextX   = values.First( Function(l)  l.Amps > x).Amps
    lastEff = values.First( Function(f)  f.Amps=lastX).Eff
    nextEff = values.First( Function(f)  f.Amps=nextX).Eff


    deltaX = nextX-lastX
    slope  = NextEff/lastEff

    Return lastEff + ( NextEff * slope)



 End Function


 Private Sub CalculateRangeTable()

 'TODO: CALCULATE RANGE TABLE


 End Sub


 Private sub InitialiseRangeTable()

  RangeTable(0).RPM=0:RangeTable(0).Efficiency=0
  RangeTable(1).RPM=0:RangeTable(0).Efficiency=0
  RangeTable(2).RPM=2000:RangeTable(0).Efficiency=0
  RangeTable(3).RPM=4000:RangeTable(0).Efficiency=0
  RangeTable(4).RPM=6000:RangeTable(0).Efficiency=0
  RangeTable(5).RPM=0:RangeTable(0).Efficiency=0
  RangeTable(6).RPM=0:RangeTable(0).Efficiency=0

 End Sub


 Private Sub CreateRangeTable()


     RangeTable.Clear()

     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))
     RangeTable.Add( New Table4Row(0,0))


 End Sub

 public Sub BuildInputTable(  inputs As Dictionary(of Single, single), targetTable As List (Of AltUserInput ))


       Dim C11,C12,C13,C14,D11,D12,D13,D14 As single
       Dim tmpAmp As single

       targetTable.Clear()

       'Row0
       targetTable.Add( New AltUserInput(0,D14))

       'Row1
       targetTable.Add( New AltUserInput( 10, inputs(10)))

       'Row2
       targetTable.Add( New AltUserInput( 40, inputs(40)))

       'Row3
       targetTable.Add( New AltUserInput( 60, inputs(60)))

       C11= targetTable(1).Amps : C12=targetTable(2).Amps : C13=targetTable(3).Amps
       D11= targetTable(1).Eff  : D12=targetTable(2).Eff  : D13=targetTable(3).Eff
       D14= IF(D12>D13,0, Math.Max(Math.MAX(D11,D12),D13))


       'Row4  - Eff
       targetTable.Add( new AltUserInput( 0 ,D14 ))


       'Row4  - Amps
       tmpAmp =IF((D13=0 OrElse D13=D12),C13+1,IF(D12>D13,((((C13-C12)/(D12-D13))*D13)+C13),((((C13-C12)/(D12-D13))*(D13-D14))+C13)))
       targetTable(4).Amps = tmpAmp

       'Row5 
       tmpAmp  =IF(C14>200,C14+1,200)
       targetTable.Add( New AltUserInput(tmpAmp,D14))

       'Row0
       targetTable(0).Eff=D11


  End Sub


   
End Class



End Namespace



