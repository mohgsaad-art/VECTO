Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ResultCardTests

Private results As New List(Of SmartResult)
Private unorderedResults As New List(of SmartResult)

Private resultCard As ResultCard
Private unorderedResultCard As ResultCard


Public Sub New()

results.Add(new SmartResult(20, 18))
results.Add(new SmartResult(30, 27))
results.Add(new SmartResult(40, 36))
results.Add(new SmartResult(50, 45))

unorderedResults.Add(new SmartResult(40, 36))
unorderedResults.Add(new SmartResult(30, 27))
unorderedResults.Add(new SmartResult(50, 45))
unorderedResults.Add(new SmartResult(20, 18))

'results.Add(60, 54)

resultCard = New ResultCard(results)

unorderedResultCard = New ResultCard( unorderedResults)



End Sub


<Test()>
Public Sub CreateNewResultsOKTest()

 Dim target As New ResultCard(results)

 Assert.IsNotNull(target)

End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub CreateNewBanResultsNullTest()

 Dim target As New ResultCard(Nothing)

 Assert.IsNotNull(target)

End Sub




<Test()>
Public Sub GetBotomBoundryValueTest()

Dim expected As Single = 18
Dim actual As Single = resultCard.GetSmartCurrentResult(20)

Assert.AreEqual(expected, actual)

End Sub


<Test()>
Public Sub UnorderedGetBotomBoundryValueTest()

Dim expected As Single = 18
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(20)

Assert.AreEqual(expected, actual)

End Sub


<Test()>
Public Sub GetCentreBoundayValueTest()

Dim expected As Single = 36
Dim actual As Single = resultCard.GetSmartCurrentResult(40)

Assert.AreEqual(expected, actual)


End Sub


<Test()>
Public Sub UnorderedGetCentreBoundayValueTest()

Dim expected As Single = 36
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(40)

Assert.AreEqual(expected, actual)


End Sub


<Test()>
Public Sub GetTopBoundaryValueTest()

Dim expected As Single = 45
Dim actual As Single = resultCard.GetSmartCurrentResult(50)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub UnorderedGetTopBoundaryValueTest()

Dim expected As Single = 45
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(50)

Assert.AreEqual(expected, actual)

End Sub


<Test()>
Public Sub GetInterpolatedValue35AmpsTest()

Dim expected As Single = 31.5
Dim actual As Single = resultCard.GetSmartCurrentResult(35)

Assert.AreEqual(expected, actual)

End Sub


<Test()>
Public Sub UnorderedGetInterpolatedValue35AmpsTest()

Dim expected As Single = 31.5
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(35)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetExtrapolatedValue60AmpsTest()

Dim expected As Single = 54
Dim actual As Single = resultCard.GetSmartCurrentResult(60)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub UnorderedGetExtrapolatedValue60AmpsTest()

Dim expected As Single = 54
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(60)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetExtrapolatedValue10AmpsTest()

Dim expected As Single = 9
Dim actual As Single = resultCard.GetSmartCurrentResult(10)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub UnorderedGetExtrapolatedValue10AmpsTest()

Dim expected As Single = 9
Dim actual As Single = unorderedResultCard.GetSmartCurrentResult(10)

Assert.AreEqual(expected, actual)

End Sub


<Test()>
Public Sub EmptyOrInsufficientResultsTest()

Dim resultSet As new List(Of SmartResult)

Dim expected As Single = 10
Dim actual As Single = (New ResultCard(resultSet)).GetSmartCurrentResult(10)

Assert.AreEqual(expected, actual)

End Sub


End Class

