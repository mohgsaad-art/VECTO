Imports VectoAuxiliaries.Electrics

Namespace Electrics


Public Class ResultCard
Implements IResultCard


Private _results As Dictionary(Of Single, Single)


Public Sub New(results As Dictionary(Of Single, Single))

   If results Is Nothing Then Throw New ArgumentException("A dictionary of smart results must be supplied.")
   If results.Count < 2 Then Throw New ArgumentException("More than two entries are needed to interpolate results")
  _results = results

End Sub




Public Function GetSmartCurrentResult(key As Single) As Single Implements IResultCard.GetSmartCurrentResult


  Return GetOrInterpolate(key)


End Function


Private Function GetOrInterpolate(key As Single) As Single

Dim pre As Single
Dim post As Single
Dim dAmps As Single
Dim dSmartAmps As Single
Dim smartAmpsSlope As Single
Dim smartAmps As Single
Dim maxKey As Single
Dim minKey As Single

     maxKey = (From k In _results Select k).Last.Key
     minKey = (From k In _results Select k).First.Key


     'Is on boundary check
     If _results.ContainsKey(key) Then Return _results(key)

     'Is over map - Extrapolate
     If key > maxKey Then

            'get the entries before and after the supplied key
             pre = (From m In _results Order By m.Key Where m.Key < maxKey Select m).Last().Key
             post = maxKey

            'get the delta values 
             dAmps = post - pre
             dSmartAmps = _results(post) - _results(pre)

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((key - post) * smartAmpsSlope) + _results(post)

             Return smartAmps

     End If

     'Is under map - Extrapolate
     If key < minKey Then

            'get the entries before and after the supplied key
            'Post is the first entry and pre is the penultimate to first entry
             post = minKey
             pre = (From k In _results Order By k.Key Where k.Key > minKey Select k).First.Key

            'get the delta values 
             dAmps = post - pre
             dSmartAmps = _results(post) - _results(pre)

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((key - post) * smartAmpsSlope) + _results(post)

             Return smartAmps
     End If

     'Is Inside map - Interpolate

            'get the entries before and after the supplied rpm
             pre = (From m In _results Where m.Key < key Select m).Last().Key
             post = (From m In _results Where m.Key > key Select m).First().Key

            'get the delta values for rpm and the map values
             dAmps = post - pre
             dSmartAmps = _results(post) - _results(pre)

            'calculate the slopes
             smartAmpsSlope = dSmartAmps / dAmps

            'calculate the new values
             smartAmps = ((key - pre) * smartAmpsSlope) + _results(pre)


            Return smartAmps

End Function




End Class


End Namespace



