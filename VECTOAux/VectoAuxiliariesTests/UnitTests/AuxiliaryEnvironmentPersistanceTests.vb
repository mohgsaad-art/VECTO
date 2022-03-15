
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.OutputData.FileIO


Namespace UnitTests
    <TestFixture()>
    Public Class AuxiliaryPersistanceTests
        'Simply, with this test we create one Aux of default and save the config.
        'We create an Empty but initialised Aux 
        'We load the previously saved config into the Emptu Aux
        'We then compare the two Aux's, if they are the same persistance has worked and they are the same.

    
        Public Sub SaveDefaultFile()

            dim auxDefault = AuxiliaryComparisonTests.GetDefaultAuxiliaryConfig() ' New AuxiliaryConfig("")
            BusAuxWriter.SaveAuxConfig(auxDefault, "TestFiles\auxiliaryConfigKEEP.json")
        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub Persistance_A_BasicLoad()

            'Arrange
            Dim auxEmpty = AuxiliaryComparisonTests.GetDefaultAuxiliaryConfig()
            Dim auxDefault = AuxiliaryComparisonTests.GetDefaultAuxiliaryConfig()

            Dim actual As Boolean = false
            Dim expected As Boolean = true

            'Act
            SaveDefaultFile()
            BusAuxiliaryInputData.ReadBusAuxiliaries("TestFiles\auxiliaryConfigKEEP.json", utils.GetDefaultVehicleData())
            'actual=auxEmpty.Load("TestFiles\auxiliaryConfigKEEP.json")

            Assert.AreEqual(auxDefault, auxEmpty)
        End Sub

        <Test()>
        Public Sub Persistance_Load_NameNotExist_Test()

            'Arrange
            Dim auxDefault = AuxiliaryComparisonTests.GetDefaultAuxiliaryConfig()
            Dim expected As boolean = false
            Dim actual As Boolean = False

            'Act

            Assert.Throws (Of FileNotFoundException)(
                sub() _
                                                        auxDefault =
                                                        BusAuxiliaryInputData.ReadBusAuxiliaries(
                                                            "ThisFileDoesNotExist.NoExtEverKnown",
                                                            Utils.GetDefaultVehicleData()))


            Assert.AreEqual(expected, actual)
        End Sub


        <Test()>
        Public Sub Persistance_LoadThroughInstantiationPlusConfigFile_Test()

            'Arrange
            
            Dim auxDefault As IAuxiliaryConfig
            Dim auxTest As IAuxiliaryConfig = AuxiliaryComparisonTests.GetDefaultAuxiliaryConfig()

            'Act
            SaveDefaultFile()
            auxDefault = BusAuxiliaryInputData.ReadBusAuxiliaries("TestFiles\auxiliaryConfigKEEP.json",
                                                                  Utils.GetDefaultVehicleData())
            Dim areEqual = auxTest.ConfigValuesAreTheSameAs(auxDefault)
            'Assert
            Assert.IsTrue(areEqual)
        End Sub
    End Class
End Namespace

