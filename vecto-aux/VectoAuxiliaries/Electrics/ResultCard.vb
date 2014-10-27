Imports VectoAuxiliaries.Electrics

Namespace Electrics


Public Class ResultCard
Implements IResultCard


Private _results As List(Of SmartResult)

Public  ReadOnly Property Results As List(Of SmartResult) Implements IResultCard.Results
    Get
    Return _results
    End Get
End Property


Public Sub New(results As List( of SmartResult))

   If results Is Nothing Then Throw New ArgumentException("A list of smart results must be supplied.")

  _results = results

End Sub




Public Function GetSmartCurrentResult(amps As Single) As Single Implements IResultCard.GetSmartCurrentResult


  If _results.Count<2 then Return 0.1

  Return GetOrInterpolate(amps)


End Function


Private Function GetOrInterpolate(amps As Single) As Single

Dim pre As Single
Dim post As Single
Dim dAmps As Single
Dim dSmartAmps As Single
Dim smartAmpsSlope As Single
Dim smartAmps As Single
Dim maxKey As Single
Dim minKey As Single

     maxKey = _results.Max.Amps
     minKey = _results.Min.Amps

     Dim compareKey As SmartResult = New SmartResult( amps,0)

     'Is on boundary check
     If _results.Contains(compareKey) Then Return _results.OrderBy( Function(x) x.Amps).First( Function( x ) x.Amps=compareKey.Amps ).SmartAmps

     'Is over map - Extrapolate
     If amps > maxKey Then

            'get the entries before and after the supplied key
             pre = (From a In _results Order By a.amps  Where a.amps < maxKey Select a ).Last().Amps
             post = maxKey

            'get the delta values 
             dAmps = post - pre
             dSmartAmps = ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps - ( From da In _results Order By da.Amps Where da.Amps=pre ).First().SmartAmps

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((amps - post) * smartAmpsSlope) + ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps

             Return smartAmps

     End If

     'Is under map - Extrapolate
     If amps < minKey Then

            'get the entries before and after the supplied key
            'Post is the first entry and pre is the penultimate to first entry
             post = minKey
             pre = (From k In _results Order By k.amps Where k.amps > minKey Select k).First().Amps

            'get the delta values 
             dAmps = post - pre
             dSmartAmps = ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps - ( From da In _results Order By da.Amps Where da.Amps=pre ).First().SmartAmps

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((amps - post) * smartAmpsSlope) + ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps

             Return smartAmps
     End If

     'Is Inside map - Interpolate

            'get the entries before and after the supplied rpm
             pre = (From m In _results Order By m.amps Where m.amps < amps Select m).Last().Amps
             post = (From m In _results Where m.amps > amps Select m).First().Amps

            'get the delta values for rpm and the map values
             dAmps = post - pre
             dSmartAmps =  ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps - ( From da In _results Order By da.Amps Where da.Amps=pre ).First().SmartAmps

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((amps - post) * smartAmpsSlope) + ( From da In _results Order By da.Amps Where da.Amps=post ).First().SmartAmps


            Return smartAmps

End Function




End Class


End Namespace



