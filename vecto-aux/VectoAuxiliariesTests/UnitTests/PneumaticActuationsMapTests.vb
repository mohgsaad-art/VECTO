Imports NUnit
Imports VectoAuxiliaries.Pneumatics
Imports NUnit.Framework

Namespace Pneumatics


<TestFixture()>
Public Class PneumaticActuationsMapTests

Public Const cstrPneumaticActuationsMapPath_GOODMAP As String = "TestFiles\testPneumaticActuationsMap_GOODMAP.csv"
Public Const cstrPneumaticActuationsMapPath_INCORRECTCOLUMNS As String = "TestFiles\testPneumaticActuationsMap_INCORRECTCOLUMNS.csv"
Public Const cstrPneumaticActuationsMapPath_INVALIDINTEGERVALUE As String = "TestFiles\testPneumaticActuationsMap_INVALIDINTEGERVALUE.csv"
Public Const cstrPneumaticActuationsMapPath_DUPLICATEKEY As String = "TestFiles\testPneumaticActuationsMap_DUPLICATEKEY.csv"
Public Const cstrPneumaticActuationsMapPath_INVALIDCONSUMERNAME As String = "TestFiles\testPneumaticActuationsMap_INVALIDCONSUMERNAME.csv"
Public Const cstrPneumaticActuationsMapPath_INVALIDCYCLENAME As String = "TestFiles\testPneumaticActuationsMap_INVALIDCYCLENAME.csv"




<Test()>
Public Sub CreateNewTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_GOODMAP)
 Assert.IsNotNull(target)

End Sub


<Test()>
Public Sub InitialiseGoodMapTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_GOODMAP)
    target.Initialise()
End Sub


<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub InitialiseWrongNumberOfColumnTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_INCORRECTCOLUMNS)
    target.Initialise()

End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub InvalidIntegerTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_INVALIDINTEGERVALUE)
    target.Initialise()

End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub DuplicateKeyTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_DUPLICATEKEY)
    target.Initialise()

End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub EmptyConsumerNameTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_INVALIDCONSUMERNAME)
    target.Initialise()

End Sub

<Test()>
<ExpectedException("System.ArgumentException")>
Public Sub EmptyCycleNameTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_INVALIDCYCLENAME)
    target.Initialise()
End Sub

<Test()>
Public Sub ValueLookupTest()

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_GOODMAP)

    target.Initialise()

    'Brakes,Coach,27
    Dim actual As Integer = target.GetNumActuations(New ActuationsKey("Brakes", "Coach"))

    Dim expected As Integer = 27

    Assert.AreEqual(expected, actual)


End Sub


End Class




End Namespace


