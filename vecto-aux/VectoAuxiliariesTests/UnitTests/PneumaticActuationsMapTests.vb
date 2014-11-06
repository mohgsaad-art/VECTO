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
<TestCase("Brakes","Heavy urban",191)> _
<TestCase("Brakes","Urban",153)> _
<TestCase("Brakes","Suburban",49)> _
<TestCase("Brakes","Interurban",190)> _
<TestCase("Brakes","Coach",27)> _
<TestCase("Park brake + 2 doors","Heavy urban",82)> _
<TestCase("Park brake + 2 doors","Urban",75)> _
<TestCase("Park brake + 2 doors","Suburban",25)> _
<TestCase("Park brake + 2 doors","Interurban",9)> _
<TestCase("Park brake + 2 doors","Coach",6)> _
<TestCase("Kneeling","Heavy urban",27)> _
<TestCase("Kneeling","Urban",25)> _
<TestCase("Kneeling","Suburban",6)> _
<TestCase("Kneeling","Interurban",0)> _
<TestCase("Kneeling","Coach",0)> _
Public Sub ValueLookupTest(key As String, cycle As String, expected As integer)

 Dim target As New PneumaticActuationsMAP(cstrPneumaticActuationsMapPath_GOODMAP)

    target.Initialise()
    Dim actual As Integer

    try
       actual = target.GetNumActuations(New ActuationsKey(key, cycle))
    Catch ex As Exception

    end try
    Assert.AreEqual(expected, actual)

End Sub


End Class




End Namespace


