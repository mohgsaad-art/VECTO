Option Strict On

Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports System.IO
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics

Namespace UnitTests

    <TestFixture()>
    Public Class CombinedAlternatorTests

        Private Alt1ExpectedTable2000 As New List(Of AltUserInput(of Ampere))
        Private Alt1ExpectedTable4000 As New List(Of AltUserInput(of Ampere))
        Private Alt1ExpectedTable6000 As New List(Of AltUserInput(of Ampere))
        Private Alt2ExpectedTable2000 As New List(Of AltUserInput(of Ampere))
        Private Alt2ExpectedTable4000 As New List(Of AltUserInput(of Ampere))
        Private Alt2ExpectedTable6000 As New List(Of AltUserInput(of Ampere))
        Private Alt3ExpectedTable2000 As New List(Of AltUserInput(of Ampere))
        Private Alt3ExpectedTable4000 As New List(Of AltUserInput(of Ampere))
        Private Alt3ExpectedTable6000 As New List(Of AltUserInput(of Ampere))
        Private Alt4ExpectedTable2000 As New List(Of AltUserInput(of Ampere))
        Private Alt4ExpectedTable4000 As New List(Of AltUserInput(of Ampere))
        Private Alt4ExpectedTable6000 As New List(Of AltUserInput(of Ampere))
        Private RangeTableExpected As New List(Of AltUserInput(of PerSecond))



        Private Const COMBINEDALT_GOODMAP = "testfiles\testCombinedAlternatorMap.aalt"


        Sub New()

            Alt1ExpectedTable2000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(Of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 50)}

            Alt1ExpectedTable4000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 70),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 70),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 70),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 70),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 70),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 70)}


            Alt1ExpectedTable6000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 60)}

            'ALT 2
            Alt2ExpectedTable2000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 80),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 80),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 80),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 80),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 80),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 80)}

            Alt2ExpectedTable4000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 40),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 40),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 40),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 40),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 40),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 40)}


            Alt2ExpectedTable6000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 60),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 60)}


            'ALT 3
            Alt3ExpectedTable2000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 95),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 95),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 50),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 90),
                                                                         New AltUserInput(of Ampere)(62.5.SI(of Ampere), 95),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 95)}

            Alt3ExpectedTable4000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 99),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 99),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 1),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 55),
                                                                         New AltUserInput(of Ampere)(76.2962963.SI(of Ampere), 99),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 99)}


            Alt3ExpectedTable6000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 94),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 94),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 86),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 13),
                                                                         New AltUserInput(of Ampere)(63.5616438.SI(of Ampere), 0),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 0)}


            'ALT 4
            Alt4ExpectedTable2000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 55),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 55),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 45),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 67),
                                                                         New AltUserInput(of Ampere)(61.SI(of Ampere), 67),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 67)}

            Alt4ExpectedTable4000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 77),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 77),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 39),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 23),
                                                                         New AltUserInput(of Ampere)(88.75.SI(of Ampere), 0),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 0)}


            Alt4ExpectedTable6000 = New List(Of AltUserInput(of Ampere))() From {New AltUserInput(of Ampere)(0.SI(of Ampere), 34),
                                                                         New AltUserInput(of Ampere)(10.SI(of Ampere), 34),
                                                                         New AltUserInput(of Ampere)(40.SI(of Ampere), 67),
                                                                         New AltUserInput(of Ampere)(60.SI(of Ampere), 35),
                                                                         New AltUserInput(of Ampere)(81.875.SI(of Ampere), 0),
                                                                         New AltUserInput(of Ampere)(200.SI(of Ampere), 0)}



            'RangeTable
            RangeTableExpected = New List(Of AltUserInput(of PerSecond))() From {New AltUserInput(of PerSecond)(-3001.RPMtoRad(), 0),
                                                                        New AltUserInput(of PerSecond)(-3000.RPMtoRad(), 0),
                                                                        New AltUserInput(of PerSecond)(2000.RPMtoRad(), 50),
                                                                        New AltUserInput(of PerSecond)(4000.RPMtoRad(), 70),
                                                                        New AltUserInput(of PerSecond)(6000.RPMtoRad(), 60),
                                                                        New AltUserInput(of PerSecond)(18000.RPMtoRad(), 0),
                                                                        New AltUserInput(of PerSecond)(18001.RPMtoRad(), 0)}









        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        <TestCase(1, 2, 50.0F)>
        <TestCase(1, 4, 70)>
        <TestCase(1, 6, 60)>
        <TestCase(2, 2, 80)>
        <TestCase(2, 4, 40)>
        <TestCase(2, 6, 60)>
        <TestCase(3, 2, 55)>
        <TestCase(3, 4, 7.75F)>
        <TestCase(3, 6, 76.875F)>
        <TestCase(4, 2, 47.75F)>
        <TestCase(4, 4, 37)>
        <TestCase(4, 6, 63)>
        Public Sub Interpolate4Table4(alt As Integer, rpmK As Integer, expected As Single)

            Dim interpValue As Double

            Select Case alt

                Case 1

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable2000, 42.5.SI(Of Ampere))
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable4000, 42.5.SI(Of Ampere))
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable6000, 42.5.SI(Of Ampere))

                    End Select


                Case 2

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable2000, 42.5.SI(Of Ampere))
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable4000, 42.5.SI(Of Ampere))
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable6000, 42.5.SI(Of Ampere))

                    End Select

                Case 3

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable2000, 42.5.SI(Of Ampere))
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable4000, 42.5.SI(Of Ampere))
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable6000, 42.5.SI(Of Ampere))

                    End Select

                Case 4

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable2000, 42.5.SI(Of Ampere))
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable4000, 42.5.SI(Of Ampere))
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable6000, 42.5.SI(Of Ampere))

                    End Select

            End Select


            Assert.AreEqual(interpValue, expected, 0.001)


        End Sub


        <Test()>
        Public Sub Alt1TableConstructTest()


            'Arrange
           
            'Act
            Dim alt = CType(AlternatorReader.ReadMap(COMBINEDALT_GOODMAP), CombinedAlternator)



            Dim idx As Integer

            For idx = 0 To alt.Alternators(0).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable2000(idx).IsEqual(Alt1ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(0).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable4000(idx).IsEqual(Alt1ExpectedTable4000(idx)))
            Next

            For idx = 0 To alt.Alternators(0).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable6000(idx).IsEqual(Alt1ExpectedTable6000(idx)))
            Next


        End Sub

        <Test()>
        Public Sub Alt2TableConstructTest()

            
            'Act
            Dim alt = CType(AlternatorReader.ReadMap(COMBINEDALT_GOODMAP), CombinedAlternator)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(1).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable2000(idx).IsEqual(Alt2ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(1).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable4000(idx).IsEqual(Alt2ExpectedTable4000(idx)))
            Next

            For idx = 0 To alt.Alternators(1).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable6000(idx).IsEqual(Alt2ExpectedTable6000(idx)))
            Next


        End Sub
        <Test()>
        Public Sub Alt3TableConstructTest()

            
            'Act
            Dim alt = CType(AlternatorReader.ReadMap(COMBINEDALT_GOODMAP), CombinedAlternator)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(2).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable2000(idx).IsEqual(Alt3ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(2).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable4000(idx).IsEqual(Alt3ExpectedTable4000(idx), 3))
            Next

            For idx = 0 To alt.Alternators(2).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable6000(idx).IsEqual(Alt3ExpectedTable6000(idx), 3))
            Next


        End Sub
        <Test()>
        Public Sub Alt4TableConstructTest()

           'Act
            Dim alt = CType(AlternatorReader.ReadMap(COMBINEDALT_GOODMAP), CombinedAlternator)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(3).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable2000(idx).IsEqual(Alt4ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(3).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable4000(idx).IsEqual(Alt4ExpectedTable4000(idx), 3))
            Next

            For idx = 0 To alt.Alternators(3).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable6000(idx).IsEqual(Alt4ExpectedTable6000(idx), 3))
            Next


        End Sub


        'testCombinedAlternatorMap
        <Test()>
        Public Sub InitialiseCombinedAlternatorMapFromFile()

            'Act
            Dim target = CType(AlternatorReader.ReadMap(COMBINEDALT_GOODMAP), CombinedAlternator)

            
            'Assert

            Assert.AreEqual(target.Alternators.Count, 4)



        End Sub

        <Test()>
        Public Sub InitialiseCombinedAlternatorMapFromDefault()

            'Act
            Dim target = CType( AlternatorReader.ReadMap("TestFiles/CombinedAlternatorDefaultsTest.aalt"), CombinedAlternator)

            'Assert
            Assert.AreEqual(target.Alternators.Count, 2)

        End Sub


        <Test()>
        Public Sub AveragedEfficiency()


            '  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals() With {.CrankRPM=1750, .CurrentDemandAmps=170}

            Dim ca  = CType( AlternatorReader.ReadMap("TestFiles/CombinedAlternatorDefaultsTest.aalt"), CombinedAlternator)

            Dim actual As double = ca.GetEfficiency(1750.RPMtoRad(), 170.SI(Of Ampere))

            Assert.AreEqual(0.684354842, actual, 1e-6)


        End Sub

        '<Test()>
        Public Sub Performance()


            Dim ca  = CType( AlternatorReader.ReadMap("TestFiles/CombinedAlternatorDefaultsTest.aalt"), CombinedAlternator)

            Dim startDT As DateTime = DateTime.Now
            Dim endDateDT As DateTime
            Dim crank As Single
            Dim demand As Single
            Dim rand As New Random(50)

            Dim min As Double = 0.1
            Dim max As Double = 0.1

            For x = 1 To 500000

                'crank = rand.Next(500,3000)
                'demand = rand.Next(1,200)

                crank = rand.Next(0, 0)
                demand = rand.Next(0, 0)

                Dim actual As double = ca.GetEfficiency(crank.RPMtoRad(), demand.SI(Of Ampere))

                If actual < min Then min = actual

                If actual > max Then max = actual

            Next

            endDateDT = DateTime.Now

            Dim secs As Single = (endDateDT - startDT).Seconds


        End Sub


        <Test()>
        Public Sub AlternatorsAreEqual()


            Dim ca As ICombinedAlternator = CType( AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)
            Dim original As ICombinedAlternator = CType(AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)

            Assert.IsTrue(ca.IsEqualTo(original))

        End Sub

        <Test()>
        Public Sub AlternatorsUnequalName()


            Dim ca As ICombinedAlternator = CType( AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)
            Dim original As ICombinedAlternator = CType(AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)

            ca.Alternators(0).AlternatorName = "ZCZZCZCZCZCXXXYYY"

            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub

        <Test()>
        Public Sub AlternatorsUnequalPulley()


            Dim ca As ICombinedAlternator = CType( AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)
            Dim original As ICombinedAlternator = CType(AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)

            ca.Alternators(0).PulleyRatio = 9

            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub

        '<Test()>
        ' Public Sub AlternatorsUnequalEfficiency()


        '  Dim ca As new CombinedAlternator("abc.aalt")
        '  Dim original As new CombinedAlternator("abc.aalt")

        '  ca.Alternators(0).InputTable2000(1).Eff=0.99999

        '  'Only tests efficiency values table row 1-3
        '  Assert.IsFalse(ca.IsEqualTo( original))

        ' End Sub


        <Test()>
        Public Sub AlternatorsUnequalEfficiency()


            Dim ca As ICombinedAlternator = CType( AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)
            Dim original As ICombinedAlternator = CType(AlternatorReader.ReadMap("TestFiles/testCombinedAlternatorMap.aalt"), ICombinedAlternator)

            ca.Alternators(0).InputTable2000(1).Eff = 0.99999

            'Only tests efficiency values table row 1-3
            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub






    End Class


End Namespace





