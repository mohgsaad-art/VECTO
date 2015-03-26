

Namespace Electrics

Public Class InterpAltUserInputs

 Public shared  Function Iterpolate( values As List(Of AltUserInput), x As single) As Single

    Dim lowestX As single = values.Min( Function(m) m.Amps)
    Dim highestX As Single = values.Max( Function(m) m.Amps)
    Dim preKey, postKey ,preEff,postEff, xSlope, EffSlope As single
    Dim deltaX , deltaEff As single 
    Dim slope As single

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

End Class


End Namespace


