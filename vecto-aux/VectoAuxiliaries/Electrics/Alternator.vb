Option Strict On


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
 Public Property RangeTable     As  New List(Of Table4Row)    Implements IAlternator.RangeTable

 Private signals As ICombinedAlternatorSignals 


 Public  Sub  Clone( other As IAlternator) Implements IAlternator.Clone


   
 End Sub
 Public ReadOnly Property Efficiency As Double Implements IAlternator.Efficiency


            Get
               'First build RangeTable, table 4

               InitialiseRangeTable()
               CalculateRangeTable()

               'Calculate ( Interpolate ) Efficiency

               Dim range as List(Of AltUserInput) = RangeTable.Select( Function(s) New AltUserInput(s.RPM,s.Efficiency)).ToList()

               Dim v As Single =  Alternator.Iterpolate( range,Convert.ToSingle( SpindleSpeed))

               Return v

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
     'InitialiseRangeTable()


 End Sub


 Public  shared Function Iterpolate( values As List(Of AltUserInput), x As single) As Single

    Dim lowestX As single = values.Min( Function(m) m.Amps)
    Dim highestX As Single = values.Max( Function(m) m.Amps)
    Dim preKey, postKey ,preEff,postEff, EffSlope As single
    Dim deltaX , deltaEff As single 

    'Out of range, returns efficiency for lowest
    If x< lowestX then Return values.First( Function(f) f.Amps= lowestX).Eff

    'Out of range, efficiency for highest
    If x> highestX then Return values.First( Function(f) f.Amps= highestX).Eff

    'On Bounds check
    If  values.Where( Function(w) w.Amps=x).Count=1 then Return values.First( Function(w) w.Amps=x).Eff


    'OK, we need to interpolate.
    preKey   = values.Last(  Function(l)  l.Amps < x).Amps
    postKey   = values.First( Function(l)  l.Amps > x).Amps
    preEff = values.First( Function(f)  f.Amps=preKey).Eff
    postEff = values.First( Function(f)  f.Amps=postKey).Eff


    deltaX = postKey-preKey
    deltaEff  = postEff-preEff

    'slopes
    effSlope = deltaEff/deltaX


    Dim retVal As Single =   ((x - preKey) * effSlope) + preEff

    Return retVal


 End Function


 Private Sub CalculateRangeTable()

    'M10=Row0-Rpm - N10=Row0-Eff
    'M11=Row1-Rpm - N11=Row1-Eff
    'M12=Row2-Rpm - N12=Row2-Eff - 2000
    'M13=Row3-Rpm - N13=Row3-Eff - 4000
    'M14=Row4-Rpm - N14=Row4-Eff - 6000
    'M15=Row5-Rpm - N15=Row5-Eff
    'M16=Row6-Rpm - N16=Row6-Eff

     Dim N10,N11,N12,N13,N14,N15,N16 As single
     Dim M10,M11,M12,M13,M14,M15,M16 As single

     'EFFICIENCY
  
    '2000
     N12= Alternator.Iterpolate(InputTable2000,signals.CurrentDemandAmps)
     RangeTable(2).Efficiency= N12
    '4000
     N13 = Alternator.Iterpolate(InputTable4000,signals.CurrentDemandAmps)
     RangeTable(3).Efficiency= N13
    '6000
     N14 =Alternator.Iterpolate(InputTable6000,signals.CurrentDemandAmps)
     RangeTable(4).Efficiency= N14

    'Row0 & Row1 Efficiency  =IF(N13>N12,0,MAX(N12:N14)) - Example Alt 1 N13=
     N11=IF(N13>N12,0,Math.Max(Math.MAX(N12,N13),N14))
     RangeTable(1).Efficiency = N11
     N10=N11
     RangeTable(0).Efficiency = N10


    'Row 5 Efficiency
     N15 =IF(N13>N14,0,Math.Max(Math.MAX(N12,N13),N14))
     RangeTable(5).Efficiency = N15
    'Row 6 - Efficiency
     N16 = N15
     RangeTable(6).Efficiency = N16

     'RPM

     '2000 Row 2 - RPM
      M12 =  2000
      RangeTable(2).RPM = M12

     '4000 Row 3 - RPM
      M13 = 4000
      RangeTable(3).RPM =  M13

     '6000 Row 4 - RPM
      M14 = 6000
      RangeTable(4).RPM =  M14

            'Row 1 - RPM
            'NOTE: Update to reflect CombineALternatorSchematicV02 20150429
            'IF(M12=IF(N12>N13,M12-((M12-M13)/(N12-N13))*(N12-N11),M12-((M12-M13)/(N12-N13))*(N12-N11)), M12-0.01, IF(N12>N13,M12-((M12-M13)/(N12-N13))*(N12-N11),M12-((M12-M13)/(N12-N13))*(N12-N11)))
            M11 = Convert.ToSingle(If(N12 - N13 = 0, 0, If(M12 = If(N12 > N13, M12 - ((M12 - M13) / (N12 - N13)) * (N12 - N11), M12 - ((M12 - M13) / (N12 - N13)) * (N12 - N11)), M12 - 0.01, If(N12 > N13, M12 - ((M12 - M13) / (N12 - N13)) * (N12 - N11), M12 - ((M12 - M13) / (N12 - N13)) * (N12 - N11)))))
      RangeTable(1).RPM =M11

      'Row 0 - RPM
      M10 = IF(M11<1500,M11-1,1500)
      RangeTable(0).RPM  = M10

      'Row 5 - RPM
      M15 =  Convert.ToSingle(IF(M14=IF((N14=0 OrElse N14=N13),M14+1,IF(N13>N14,((((M14-M13)/(N13-N14))*N14)+M14),((((M14-M13)/(N13-N14))*(N14-N15))+M14))),M14+0.01,IF((N14=0 OrElse N14=N13),M14+1,IF(N13>N14,((((M14-M13)/(N13-N14))*N14)+M14),((((M14-M13)/(N13-N14))*(N14-N15))+M14)))))
      RangeTable(5).RPM = M15


      'Row 6 - RPM
      M16 =  IF(M15>10000,M15+1,10000)
      RangeTable(6).RPM =  M16

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



 Public Function IsEqualTo(other As IAlternator) As Boolean Implements IAlternator.IsEqualTo

     If Me.AlternatorName <> other.AlternatorName then Return False
     If Me.PulleyRatio <> other.PulleyRatio then Return false
     
     Dim i As Integer =1

     For i= 1 to 3

       If Me.InputTable2000(i).Eff <> other.InputTable2000(i).Eff then Return False
       If Me.InputTable4000(i).Eff <> other.InputTable4000(i).Eff then Return False
       If Me.InputTable6000(i).Eff <> other.InputTable6000(i).Eff then Return False


     Next

    Return True
    
  End Function



End Class



End Namespace



