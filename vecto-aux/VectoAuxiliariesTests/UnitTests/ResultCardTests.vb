Imports NUnit.Framework
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ResultCardTests

Private results As New Dictionary(Of Single, Single)
Private resultCard As ResultCard

Public Sub New()

results.Add(20, 18)
results.Add(30, 27)
results.Add(40, 36)
results.Add(50, 45)
'results.Add(60, 54)

resultCard = New ResultCard(results)

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
<ExpectedException("System.ArgumentException")>
Public Sub CreateNewBanResultsInsufficientEntriesTest()


 Dim target As New ResultCard(New Dictionary(Of Single, Single))


End Sub

<Test()>
Public Sub GetBotomBoundryValueTest()

Dim expected As Single = 18
Dim actual As Single = resultCard.GetSmartCurrentResult(20)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetCentreBoundayValueTest()

Dim expected As Single = 36
Dim actual As Single = resultCard.GetSmartCurrentResult(40)

Assert.AreEqual(expected, actual)


End Sub


<Test()>
Public Sub GetTopBoundaryValueTest()

Dim expected As Single = 45
Dim actual As Single = resultCard.GetSmartCurrentResult(50)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetInterpolatedValue35AmpsTest()

Dim expected As Single = 31.5
Dim actual As Single = resultCard.GetSmartCurrentResult(35)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetExtrapolatedValue60AmpsTest()

Dim expected As Single = 54
Dim actual As Single = resultCard.GetSmartCurrentResult(60)

Assert.AreEqual(expected, actual)

End Sub

<Test()>
Public Sub GetExtrapolatedValue10AmpsTest()

Dim expected As Single = 9
Dim actual As Single = resultCard.GetSmartCurrentResult(10)

Assert.AreEqual(expected, actual)

End Sub


End Class

