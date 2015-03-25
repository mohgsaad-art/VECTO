
Namespace Electrics


Public Class Alternator
 Implements IAlternator

 Public Property AlternatorName As String Implements IAlternator.AlternatorName
 Public Property  PulleyRatio As Single Implements IAlternator.PulleyRatio

 Public Property InputTable2000 As  New List(Of AltUserInput) Implements IAlternator.InputTable2000  
 Public Property InputTable4000 As  New List(Of AltUserInput) Implements IAlternator.InputTable4000
 Public Property InputTable6000 As  New List(Of AltUserInput) Implements IAlternator.InputTable6000
 Public Property RangeTable     As  New List(Of AltUserInput) Implements IAlternator.RangeTable

 Private signals As ICombinedAlternatorSignals 


 Public  Sub  Clone( other As IAlternator) Implements IAlternator.Clone

    Me.PulleyRatio = other.PulleyRatio
    Me.AlternatorName= other.AlternatorName

    InputTable2000.Clear() 
    InputTable4000.Clear() 
    InputTable6000.Clear() 
    RangeTable    .Clear() 
    
    For Each entry As AltUserInput In other.InputTable2000
      InputTable2000.Add(New AltUserInput( entry.Amps, entry.Eff))
    Next

    For Each entry As AltUserInput In other.InputTable4000
      InputTable4000.Add(New AltUserInput( entry.Amps, entry.Eff))
    Next

    For Each entry As AltUserInput In other.InputTable6000
      InputTable6000.Add(New AltUserInput( entry.Amps, entry.Eff))
    Next

    For Each entry As AltUserInput In other.RangeTable
      RangeTable.Add(New AltUserInput( entry.Amps, entry.Eff))
    Next

   
 End Sub
 Public ReadOnly Property Efficiency As Double Implements IAlternator.Efficiency
            Get

            End Get
        End Property
 Public ReadOnly Property SpindleSpeed As Double Implements IAlternator.SpindleSpeed
            Get

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



