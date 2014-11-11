Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq

Namespace UnitTests
<TestFixture()>
Public Class M9Tests

Private M1 As IM1_AverageHVACLoadDemand
Private M4 As IM4_AirCompressor
Private M6 As IM6
Private M8 As IM8


<Test()>
Public sub CreateNewInstanceTest()

Dim m1Mock As New Mock(Of IM1_AverageHVACLoadDemand)
Dim m4Mock As New Mock(Of IM4_AirCompressor)
Dim m6Mock As New Mock(Of IM6)
Dim m8Mock As New Mock(Of IM8)


End Sub

End Class

End Namespace



