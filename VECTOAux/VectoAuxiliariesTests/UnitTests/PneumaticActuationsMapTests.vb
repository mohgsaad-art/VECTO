
Imports NUnit.Framework
Imports System.IO
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCore.InputData.Reader.ComponentData

Namespace Pneumatics
    <TestFixture()>
    Public Class PneumaticActuationsMapTests
        Public _
            Const cstrPneumaticActuationsMapPath_GOODMAP As String = "TestFiles\testPneumaticActuationsMap_GOODMAP.apac"

        Public _
            Const cstrPneumaticActuationsMapPath_INCORRECTCOLUMNS As String =
            "TestFiles\testPneumaticActuationsMap_INCORRECTCOLUMNS.apac"

        Public _
            Const cstrPneumaticActuationsMapPath_INVALIDINTEGERVALUE As String =
            "TestFiles\testPneumaticActuationsMap_INVALIDINTEGERVALUE.apac"

        Public _
            Const cstrPneumaticActuationsMapPath_DUPLICATEKEY As String =
            "TestFiles\testPneumaticActuationsMap_DUPLICATEKEY.apac"

        Public _
            Const cstrPneumaticActuationsMapPath_INVALIDCONSUMERNAME As String =
            "TestFiles\testPneumaticActuationsMap_INVALIDCONSUMERNAME.apac"

        Public _
            Const cstrPneumaticActuationsMapPath_INVALIDCYCLENAME As String =
            "TestFiles\testPneumaticActuationsMap_INVALIDCYCLENAME.apac"


        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub CreateNewTest()

            Dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_GOODMAP)
            Assert.IsNotNull(target)
        End Sub


        <Test()>
        Public Sub InitialiseGoodMapTest()

            Dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_GOODMAP)
            'target.Initialise()
        End Sub


        <TestCase()>
        Public Sub InitialiseWrongNumberOfColumnTest()

            Assert.That(Sub()
                dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_INCORRECTCOLUMNS)
            End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub InvalidIntegerTest()

            Assert.That(Sub()
                dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_INVALIDINTEGERVALUE)
            End Sub, Throws.InstanceOf (Of FormatException))
        End Sub

        <Test()>
        Public Sub DuplicateKeyTest()

            Assert.That(Sub()
                dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_DUPLICATEKEY)
            End Sub, Throws.InstanceOf (Of VectoException),
                        "Duplicate entries in pneumatic actuations map! Brakes / Urban")
        End Sub

        <Test()>
        Public Sub EmptyConsumerNameTest()

            Assert.That(Sub()
                dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_INVALIDCONSUMERNAME)
            End Sub, Throws.InstanceOf (Of VectoException))
        End Sub

        <Test()>
        Public Sub EmptyCycleNameTest()

            Assert.That(Sub()
                Dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_INVALIDCYCLENAME)
            End Sub, Throws.InstanceOf (Of ArgumentException))
        End Sub

        '<Test()>
        '<TestCase("Brakes", "Heavy urban", 191)>
        '<TestCase("Brakes", "Urban", 153)>
        '<TestCase("Brakes", "Suburban", 49)>
        '<TestCase("Brakes", "Interurban", 190)>
        '<TestCase("Brakes", "Coach", 27)>
        '<TestCase("Park brake + 2 doors", "Heavy urban", 82)>
        '<TestCase("Park brake + 2 doors", "Urban", 75)>
        '<TestCase("Park brake + 2 doors", "Suburban", 25)>
        '<TestCase("Park brake + 2 doors", "Interurban", 9)>
        '<TestCase("Park brake + 2 doors", "Coach", 6)>
        '<TestCase("Kneeling", "Heavy urban", 27)>
        '<TestCase("Kneeling", "Urban", 25)>
        '<TestCase("Kneeling", "Suburban", 6)>
        '<TestCase("Kneeling", "Interurban", 0)>
        '<TestCase("Kneeling", "Coach", 0)>
        'Public Sub ValueLookupTest(key As String, cycle As String, expected As Integer)

        '    Dim target = ActuationsMapReader.Read(cstrPneumaticActuationsMapPath_GOODMAP)

        '    'target.Initialise()
        '    Dim actual As Integer

        '    Try
        '        actual = target.GetNumActuations(New ActuationsKey(key, cycle))
        '    Catch ex As Exception

        '    End Try
        '    Assert.AreEqual(expected, actual)
        'End Sub
    End Class
End Namespace


