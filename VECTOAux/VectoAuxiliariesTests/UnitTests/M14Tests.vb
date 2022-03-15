

Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests
	'This implements the ISSMTOOL and returns 50% of the EngineHeatWaste as a Fueling Value
	'for the purpose of this test.
	Public Class SSMToolMock
		Implements ISSMTOOL

        Public Property Calculate As ISSMCalculate Implements ISSMTOOL.Calculate
        Public Property HVACConstants As IHVACConstants Implements ISSMTOOL.HVACConstants

        'Public Sub Clone(from As ISSMTOOL) Implements ISSMTOOL.Clone
        'End Sub

        Public ReadOnly Property ElectricalWAdjusted As Watt Implements ISSMTOOL.ElectricalWAdjusted
            Get
                Throw New NotImplementedException
            End Get
        End Property

        'Public ReadOnly Property ElectricalWBase As Watt Implements ISSMTOOL.ElectricalWBase
        '    Get
        '        Throw New NotImplementedException
        '    End Get
        'End Property

        'Public ReadOnly Property FuelPerHBase As KilogramPerSecond Implements ISSMTOOL.FuelPerHBase
        '    Get
        '        Throw New NotImplementedException
        '    End Get
        'End Property

        'Public ReadOnly Property FuelPerHBaseAdjusted As KilogramPerSecond Implements ISSMTOOL.FuelPerHBaseAdjusted
        '    Get
        '        Throw New NotImplementedException
        '    End Get
        'End Property

        Public ReadOnly Property EngineWasteHeat As Watt Implements ISSMTOOL.EngineWasteHeat


        Public Function AverageAuxHeaterPower(averageUseableEngineWasteHeat As Watt) As Watt Implements ISSMTOOL.AverageAuxHeaterPower

            Return (0.5*(averageUseableEngineWasteHeat.Value()*0.835).SI(Unit.SI.Liter.Per.Hour).Value()).SI (of Watt)
        End Function

        Public Property SSMInputs As ISSMDeclarationInputs Implements ISSMTOOL.SSMInputs

            Get
                Return CType(Utils.GetAuxTestConfig().SSMInputs, ISSMDeclarationInputs)
            End Get
            Set(value As ISSMDeclarationInputs)
            End Set
        End Property

        'Public Function IsEqualTo(source As ISSMTOOL) As Boolean Implements ISSMTOOL.IsEqualTo
        '    Throw New NotImplementedException
        'End Function

        'Public Function Load(filePath As String) As Boolean Implements ISSMTOOL.Load
        '	Throw New NotImplementedException
        'End Function

        'Public ReadOnly Property MechanicalWBase As Watt Implements ISSMTOOL.MechanicalWBase
        '    Get
        '        Throw New NotImplementedException
        '    End Get
        'End Property

        Public ReadOnly Property MechanicalWBaseAdjusted As Watt Implements ISSMTOOL.MechanicalWBaseAdjusted
            Get
                Throw New NotImplementedException
            End Get
        End Property

        'Public Function Save(filePath As String) As Boolean Implements ISSMTOOL.Save
        '	Throw New NotImplementedException
        'End Function

        Public Property TechList As ISSMTechnologyBenefits Implements ISSMTOOL.TechList

	End Class


	'<TestFixture()>
	'Public Class M14Tests

 '       <OneTimeSetUp>
 '       Public Sub RunBeforeAnyTests()
 '           Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
 '       End Sub

	'	<Test()>
	'	Public Sub ValuesTest()

	'		'Arrange
	'		Dim ip1 As Double = 1000.0
	'		Dim ip5 As Double = 3114

	'		Dim expectedOut1 As Double = 1799.3334	' 780333.4 
	'		Dim expectedOut2 As Double = 2.13093

	'		Dim m13 As New Mock(Of IM13)
	'		Dim hvacSSM As New Mock(Of ISSMTOOL)
	'		Dim signals As New Mock(Of ISignals)
	'		Dim ssmMock As ISSMTOOL = New SSMToolMock()

	'		'Moq' Arrangements
	'		m13.Setup(Function(x) x.WHTCTotalCycleFuelConsumption).Returns((ip1 / 1000).SI(Of Kilogram))
	'		signals.Setup(Function(x) x.CurrentCycleTimeInSeconds).Returns(ip5)

 '           Dim fuel = New FuelData.Entry(FuelType.DieselCI, Nothing, 0.SI(of KilogramPerCubicMeter),1.0, 44800.SI(Unit.SI.Joule.Per.Gramm).Cast(Of JoulePerKilogramm), 44800.SI(Unit.SI.Joule.Per.Gramm).Cast(Of JoulePerKilogramm))
	'		'Act
	'		Dim m14 As New M14Impl(m13.Object, ssmMock, fuel, signals.Object)

	'		'Assert
 '           Assert.AreEqual(expectedOut1.SI(Unit.SI.Gramm).Value(), m14.TotalCycleFC.Value(), 0.1)
 '           'Assert.AreEqual(expectedOut2.SI(Of Liter).Value(), m14.TotalCycleFCLitres.Value(), 0.00001)
	'	End Sub
	'End Class
End Namespace


