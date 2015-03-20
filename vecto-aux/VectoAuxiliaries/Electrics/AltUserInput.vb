

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



End Namespace


