Imports System.Text
Imports NUnit.Framework
Imports NUnit
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace UnitTests

<TestFixture()>
Public Class AuxiliaryPersistanceTests

    'Simply, with this test we create one Aux of default and save the config.
    'We create an Empty but initialised Aux 
    'We load the previously saved config into the Emptu Aux
    'We then compare the two Aux's, if they are the same persistance has worked and they are the same.
    

     Public Sub SaveDefaultFile()

      dim auxDefault  = New AuxillaryEnvironment("")
      auxDefault.Save("TestFiles\auxiliaryConfigKEEP.json")

     End Sub

    <Test()>
    Public Sub Persistance_A_BasicLoad()

    'Arrange
    Dim auxEmpty = New AuxillaryEnvironment("EMPTY")
    Dim auxDefault  = New AuxillaryEnvironment("")
    
    Dim actual        As Boolean =false
    Dim expected      As Boolean = true

    'Act
    SaveDefaultFile()
    actual=auxEmpty.Load("TestFiles\auxiliaryConfigKEEP.json")
 
    Assert.AreEqual( expected,actual )

    End Sub

    <Test()>
    Public Sub Persistance_Load_NameNotExist_Test()

    'Arrange
    Dim auxDefault  = New AuxillaryEnvironment("")
    Dim expected As boolean = false
    Dim actual   As Boolean = False

    'Act
    actual = auxDefault.Load("ThisFileDoesNotExist.NoExtEverKnown")


    Assert.AreEqual( expected,actual )


    End Sub


    <Test()>
    Public Sub Persistance_LoadThroughInstantiationPlusConfigFile_Test()

    'Arrange
    Dim expected As boolean = true
    Dim actual   As Boolean = False
    Dim auxDefault As AuxillaryEnvironment
    Dim auxTest As AuxillaryEnvironment =  New AuxillaryEnvironment("")

    'Act
    SaveDefaultFile()
    auxDefault  = New AuxillaryEnvironment("TestFiles\auxiliaryConfigKEEP.json")    
    actual = auxTest.ConfigValuesAreTheSameAs( auxDefault)
    'Assert
    Assert.AreEqual( expected,actual )


    End Sub


End Class

End Namespace

