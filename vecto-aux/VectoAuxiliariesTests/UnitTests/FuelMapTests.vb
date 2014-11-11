Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests

<TestFixture()> _
Public Class FuelMapTests

Public Const GOODMAP As String = "TestFiles\TestFuelGoodMap.vmap"

<Test()> _
Public Sub createNewInstance()

Dim target = New cMAP()
Assert.IsNotNull(target)

End Sub

<Test()> _
Public Sub ReadMapTest()

Dim target = New cMAP()
target.FilePath=GOODMAP
Dim actual As Boolean = target.ReadFile()

Assert.AreEqual(True,actual)

End Sub

End Class

End Namespace



