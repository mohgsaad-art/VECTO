Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq


Namespace UnitTests


'This implements the ISSMTOOL and returns 50% of the EngineHeatWaste as a Fueling Value
'for the purpose of this test.
public class SSMToolMock 
    implements ISSMTOOL



        Public Property Calculate As ISSMCalculate Implements ISSMTOOL.Calculate

        Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone

        End Sub

        Public ReadOnly Property ElectricalWAdjusted As Single Implements ISSMTOOL.ElectricalWAdjusted
            Get

            End Get
        End Property

        Public ReadOnly Property ElectricalWBase As Single Implements ISSMTOOL.ElectricalWBase
            Get

            End Get
        End Property

        Public ReadOnly Property FuelPerHBase As Single Implements ISSMTOOL.FuelPerHBase
            Get

            End Get
        End Property

        Public ReadOnly Property FuelPerHBaseAdjusted As Single Implements ISSMTOOL.FuelPerHBaseAdjusted
            Get

            End Get
        End Property

        Public Function FuelPerHBaseAsjusted(AverageUseableEngineWasteHeatKW As Single) As Single Implements ISSMTOOL.FuelPerHBaseAsjusted

          Return 0.5 * AverageUseableEngineWasteHeatKW

        End Function

        Public Property GenInputs As ISSMGenInputs Implements ISSMTOOL.GenInputs

        Public Function IsEqualTo(source As ISSMTOOL) As Boolean Implements ISSMTOOL.IsEqualTo

        End Function

        Public Function Load(filePath As String) As Boolean Implements ISSMTOOL.Load

        End Function

        Public ReadOnly Property MechanicalWBase As Single Implements ISSMTOOL.MechanicalWBase
            Get

            End Get
        End Property

        Public ReadOnly Property MechanicalWBaseAdjusted As Single Implements ISSMTOOL.MechanicalWBaseAdjusted
            Get

            End Get
        End Property

        Public Function Save(filePath As String) As Boolean Implements ISSMTOOL.Save

        End Function

        Public Property TechList As ISSMTechList Implements ISSMTOOL.TechList
End Class


<TestFixture()> _
Public Class M14Tests

    <Test()> _
    Public sub ValuesTest()

        'Arrange
        Dim ip1 As Single = 1000f
        Dim ip5 As Single = 3114
        Dim ip6 As Single = 1500

        Dim  expectedOut1 As Single=1375.40137
        Dim  expectedOut2 As Single=1.64718723

        Dim m13       As New Mock(Of IM13)
        Dim hvacSSM   As New Mock(Of ISSMTOOL)
        Dim signals   As new Mock( Of ISignals)
        Dim ssmMock   As ISSMTOOL = New SSMToolMock()
        Dim constants As IHVACConstants = New HVACConstants()

        'Moq' Arrangements
        m13.Setup    ( Function(x) x.WHTCTotalCycleFuelConsumptionGrams).Returns(ip1)
        signals.Setup( Function(x) x.TotalCycleTimeSeconds)             .Returns(ip5)
        signals.Setup( Function(x) x.CurrentCycleTimeInSeconds)         .Returns(1500)

        'Act
        Dim m14 As New M14(m13.Object,ssmMock,constants,signals.Object)

       'Assert
        Assert.AreEqual( expectedOut1, m14.TotalCycleFCGrams)
        Assert.AreEqual( expectedOut2, m14.TotalCycleFCLitres)

    End sub


End Class

End Namespace


