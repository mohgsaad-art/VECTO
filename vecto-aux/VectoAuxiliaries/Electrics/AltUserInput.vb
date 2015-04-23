Option Strict On


Namespace Electrics

'Used by the Combined Alternator Form/Classes to accept user input for the combined alternators efficiency
'At different Current Demands
  Public class AltUserInput
  
    Public Amps As Single
    Public Eff As Single 
  
    'Constructor
    Sub new( amps As Single , eff As single)

   Me.Amps=amps
   Me.Eff = eff

  End Sub
  
   'Equality
    Public Function IsEqual(  other As AltUserInput, Optional rounding As Integer=7) As Boolean

    Return Math.round(Me.Amps,rounding)= Math.Round(other.Amps,rounding) AndAlso _
        Math.Round(Me.Eff,rounding) = Math.Round(other.eff,rounding)

    End Function
  
  
  End class

End Namespace


