
Namespace Electrics

Public class AltUserInput

  Public Amps As Single
  Public Eff As Single 

  Sub new( amps As Single , eff As single)

   Me.Amps=amps
   Me.Eff = eff

  End Sub


 Public Function IsEqual(  other As AltUserInput, Optional rounding As Integer=7) As Boolean

    Return Math.round(Me.Amps,rounding)= Math.Round(other.Amps,rounding) AndAlso _
        Math.Round(Me.Eff,rounding) = Math.Round(other.eff,rounding)

 End Function


End class


Public Class Alternator
 Implements IAlternator


 Public InputTable2000 As  New List(Of AltUserInput)
 Public InputTable4000 As  New List(Of AltUserInput)
 Public InputTable6000 As  New List(Of AltUserInput)


 Private signals As ICombinedAlternatorSignals
 Private pulleyRation As Single
 

 Public ReadOnly Property Efficiency As Double Implements IAlternator.Efficiency
            Get

            End Get
        End Property

 Public ReadOnly Property SpindleSpeed As Double Implements IAlternator.SpindleSpeed
            Get

            End Get
        End Property

 Sub new( isignals As ICombinedAlternatorSignals, pulleyRatio As single)


     If isignals is Nothing then Throw New ArgumentException("Alternator - ISignals supplied is nothing")
     signals = isignals

  


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



