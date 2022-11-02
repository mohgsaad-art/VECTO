Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.OutputData.FileIO


Namespace UnitTests
    <TestFixture()>
    Public Class _SSMTOOLTests
        'TechBenefitsList - FilePath Constants
        Private Const GOODTechList As String = "TestFiles\testSSMTechBenefits.csv"
        Private Const GOODTechListALLON As String = "TestFiles\testSSMTechBenefitsALLON.csv"
        Private Const GOODTechListALLOFF As String = "TestFiles\testSSMTechBenefitsALLOFF.csv"
        Private Const GOODTechListEMPTYLIST As String = "TestFiles\testSSMTechBenefitsEMPTYLIST.csv"

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        'Helpers
        Private Sub AddDefaultTechLine(source As ISSMTOOL)

            Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

            Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = src.SSMInputs.BusParameters.BusFloorType


            'newItem.Units = "fraction"
            newItem.Category = "Insulation"
            newItem.BenefitName = "Benefit1"

            newItem.LowFloorH = 0.1
            newItem.LowFloorV = 0.1
            newItem.LowFloorC = 0.1

            newItem.SemiLowFloorH = 0.1
            newItem.SemiLowFloorV = 0.1
            newItem.SemiLowFloorC = 0.1

            newItem.RaisedFloorH = 0.1
            newItem.RaisedFloorV = 0.1
            newItem.RaisedFloorC = 0.1

            'newItem.OnVehicle = True
            newItem.ActiveVH = True
            newItem.ActiveVV = True
            newItem.ActiveVC = True
            'newItem.LineType = TechLineType.Normal

            Dim feedback As String = String.Empty

            'CType(src.TechList, SSMTechList).TechLines = New List(Of SSMTechnology)({newItem})
        End Sub


        'SSMGenInputTests
        <Test()> _
        <TestCase("BusParameterisation")> _
        <TestCase("BoundaryConditions")> _
        <TestCase("EnvironmentalConditions")> _
        <TestCase("AC-System")> _
        <TestCase("Ventilation")> _
        <TestCase("AuxHeater")>
        Public Sub InstantiateDefaultSSMGenInputsTest(section As String)

            Dim mission As New Mission With {
                .BusParameter = New BusParameters() With {
                .HVACCompressorType = HeatPumpType.non_R_744_2_stage,
                .HVACAuxHeaterPower = 30000.0.SI(Of Watt),
                .HVACConfiguration = BusHVACSystemConfiguration.Configuration6,
                .DoubleDecker = False,
                .VehicleWidth = 2.55.SI(Of Meter),
                .VehicleLength = 10.655.SI(Of Meter),
                .BodyHeight = 2.275.SI(Of Meter),
                .PassengerDensityRef = 3.SI(Of PerSquareMeter),
                .PassengerDensityLow = 3.SI(Of PerSquareMeter)
            },
            .MissionType = MissionType.Urban
            }

            Dim auxInput as IBusAuxiliariesDeclarationData = nothing

            Dim dao = New DeclarationDataAdapterCompletedBusGeneric()
            Dim target As ISSMDeclarationInputs = dao.CreateSSMModelParameters(auxInput, mission, FuelData.Diesel, LoadingType.ReferenceLoad)

            If section = "BusParameterisation" Then
                'BUS Parameterisation
                '********************
                Assert.AreEqual(73.33075, target.BusParameters.NumberOfPassengers, 1e-3)
                Assert.AreEqual(FloorType.HighFloor, target.BusParameters.BusFloorType)
                'Assert.AreEqual(24.1102486R, target.BusParameters.BusFloorSurfaceArea.Value(), 2)
                Assert.AreEqual(114.42325R, target.BusParameters.BusSurfaceArea.Value())
                Assert.AreEqual(20.98R, Math.Round(target.BusParameters.BusWindowSurface.Value(), 2))
                'Assert.AreEqual(61.81231875D, Math.Round(target.BusParameters.BusVolume.Value(), 8))
                'Assert.AreEqual(10.655R, target.BusParameters.BusLength.Value())
                'Assert.AreEqual(2.55R, target.BusParameters.BusWidth.Value())
            End If

            If section = "BoundaryConditions" Then
                'BOUNDRY CONDITIONS
                '******************
                Assert.AreEqual(0.95R, target.BoundaryConditions.GFactor)
                Assert.AreEqual(0.8R, target.BoundaryConditions.SolarClouding(20.0.DegCelsiusToKelvin()))
                Assert.AreEqual(80,
                                target.BoundaryConditions.HeatPerPassengerIntoCabin(20.0.DegCelsiusToKelvin()).Value())
                'Assert.AreEqual(12, target.BoundaryConditions.PassengerBoundaryTemperature.AsDegCelsius)
                'Assert.AreEqual(3.0R, target.BusParameters.PassengerDensityLowFloor.Value())
                'Assert.AreEqual(2.2R, target.BusParameters.PassengerDensitySemiLowFloor.Value())
                'Assert.AreEqual(1.4R, target.BusParameters.PassengerDensityRaisedFloor.Value())
                'Assert.AreEqual(34.0R, Math.Round(target.BusParameters.CalculatedPassengerNumber, 4))
                Assert.AreEqual(3.0R, target.BoundaryConditions.UValue.Value())
                Assert.AreEqual(18, target.BoundaryConditions.HeatingBoundaryTemperature.AsDegCelsius)
                Assert.AreEqual(23, target.BoundaryConditions.CoolingBoundaryTemperature.AsDegCelsius)
                Assert.AreEqual(20, target.BoundaryConditions.VentilationRate.Value()*3600)
                'Assert.AreEqual(7, target.BoundaryConditions.LowVentilation.Value()*3600)
                'Assert.AreEqual(1236.25, Math.Round(target.BoundaryConditions.VolumeExchange.Value()*3600, 2))
                'Assert.AreEqual(432.69, Math.Round(target.BoundaryConditions.LowVolumeExchange.Value()*3600, 2))
                Assert.AreEqual(540.14, Math.Round(target.BoundaryConditions.VentPower(False).Value(), 2))
                'Assert.AreEqual(242.3, Math.Round(target.BoundaryConditions.LowVentPower.Value(), 2))
                Assert.AreEqual(0.56R, target.BoundaryConditions.SpecificVentilationPower.Value()/3600)
                Assert.AreEqual(0.84, target.BoundaryConditions.AuxHeaterEfficiency)
                Assert.AreEqual(42700.0/3600.0, target.BoundaryConditions.GCVDieselOrHeatingOil.Value()/3600.0/1000.0)
                'Assert.AreEqual(11.8, target.BoundaryConditions.GCVDieselOrHeatingOil.Value()/3600.0/1000.0)
                'Assert.AreEqual(1.5R, target.BoundaryConditions.WindowAreaPerUnitBusLength.Value())
                'Assert.AreEqual(5, target.BoundaryConditions.FrontRearWindowArea.Value())
                Assert.AreEqual(3, target.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses.Value())
                Assert.AreEqual(0.5R, target.BoundaryConditions.MaxPossibleBenefitFromTechnologyList)
            End If


            If section = "EnvironmentalConditions" Then
                'Environmental Conditions
                '************************
                Assert.AreEqual(25.0, target.EnvironmentalConditions.DefaultConditions.Temperature.AsDegCelsius)
                Assert.AreEqual(400.0, target.EnvironmentalConditions.DefaultConditions.Solar.Value())

            End If

            If section = "AC-System" Then
                'AC-SYSTEM
                '*********
                Assert.AreEqual(HeatPumpType.non_R_744_2_stage, target.ACSystem.HVACCompressorType)
                Assert.AreEqual(15.5567, target.ACSystem.HVACMaxCoolingPower.Value()/1000.0, 1e-3)
                Assert.AreEqual(3.5, target.ACSystem.COP)
            End If

            If section = "Ventilation" Then
                'VENTILATION
                '***********                                                                            
                Assert.Areequal(True, target.Ventilation.VentilationOnDuringHeating)
                Assert.Areequal(True, target.Ventilation.VentilationWhenBothHeatingAndACInactive)
                Assert.Areequal(True, target.Ventilation.VentilationDuringAC)
                'Assert.Areequal(VentilationLevel.High, target.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive)
                'Assert.Areequal(VentilationLevel.High, target.Ventilation.VentilationDuringHeating)
                'Assert.AreEqual(VentilationLevel.High, target.Ventilation.VentilationDuringCooling)

            End If

            If section = "AuxHeater" Then
                'AUX HEATER
                '**********
                'Assert.AreEqual(0, target.AuxHeater.EngineWasteHeatkW.ConvertToKiloWatt().Value())
                Assert.AreEqual(30, target.AuxHeater.FuelFiredHeaterPower.ConvertToKiloWatt().Value())
            End If
        End Sub

        'Basic TechListTests
        <Test()>
        Public Sub Instantiate_TechListTest()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechList).Items


            'Assert.IsTrue(target.TechLines.Count > 0)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestALLON()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListALLON).Items

            'For Each entry As ISSMTechnology In target.TechLines
                'entry.OnVehicle = True
            'Next

            'Assert.IsTrue(target.TechLines.Count > 0)
            'Assert.AreEqual(0.142, Math.Round(target.HValueVariation, 3))
            'Assert.AreEqual(0.006, Math.Round(target.VHValueVariation, 3))
            'Assert.AreEqual(0.006, Math.Round(target.VVValueVariation, 3))
            'Assert.AreEqual(0.006, Math.Round(target.VCValueVariation, 3))
            'Assert.AreEqual(0.259, Math.Round(target.CValueVariation, 3))

            'Assert.AreEqual(0.0, Math.Round(target.VHValueVariationKW, 3))
            'Assert.AreEqual(0.0, Math.Round(target.VVValueVariationKW, 3))
            'Assert.AreEqual(0.0, Math.Round(target.VCValueVariationKW, 3))
            'Assert.AreEqual(0.0, Math.Round(target.VCValueVariationKW, 3))
            'Assert.AreEqual(-0.2, Math.Round(target.CValueVariationKW, 3))
        End Sub

        'List Management Methods
        <Test()>
        Public Sub Instantiate_TechListTestEMPTYList()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            ''Assert.IsTrue(target.Initialise())

            'Assert.IsTrue(target.TechLines.Count = 0)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListADD1()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            'Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            ''newItem.Units = "fraction"
            'newItem.Category = "Insulation"
            'newItem.BenefitName = "Benefit1"

            'newItem.LowFloorH = 0.1
            'newItem.LowFloorV = 0.1
            'newItem.LowFloorC = 0.1

            'newItem.SemiLowFloorH = 0.1
            'newItem.SemiLowFloorV = 0.1
            'newItem.SemiLowFloorC = 0.1

            'newItem.RaisedFloorH = 0.1
            'newItem.RaisedFloorV = 0.1
            'newItem.RaisedFloorC = 0.1

            ''newItem.OnVehicle = True
            'newItem.ActiveVH = True
            'newItem.ActiveVV = True
            'newItem.ActiveVC = True
            ''newItem.LineType = TechLineType.Normal


            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)({newItem})


            'Assert.IsTrue(target.TechLines.Count = 1)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListADD1Duplicate()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            'Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            ''newItem.Units = "fraction"
            'newItem.Category = "Insulation"
            'newItem.BenefitName = "Benefit1"

            'newItem.LowFloorH = 0.1
            'newItem.LowFloorV = 0.1
            'newItem.LowFloorC = 0.1

            'newItem.SemiLowFloorH = 0.1
            'newItem.SemiLowFloorV = 0.1
            'newItem.SemiLowFloorC = 0.1

            'newItem.RaisedFloorH = 0.1
            'newItem.RaisedFloorV = 0.1
            'newItem.RaisedFloorC = 0.1

            ''newItem.OnVehicle = True
            'newItem.ActiveVH = True
            'newItem.ActiveVV = True
            'newItem.ActiveVC = True
            ''newItem.LineType = TechLineType.Normal

            'Dim feedback As String = String.Empty

            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)({newItem})
            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)({newItem})

            'Assert.IsTrue(target.TechLines.Count = 1)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListADD1AndClear()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            'Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            ''newItem.Units = "fraction"
            'newItem.Category = "Insulation"
            'newItem.BenefitName = "Benefit1"

            'newItem.LowFloorH = 0.1
            'newItem.LowFloorV = 0.1
            'newItem.LowFloorC = 0.1

            'newItem.SemiLowFloorH = 0.1
            'newItem.SemiLowFloorV = 0.1
            'newItem.SemiLowFloorC = 0.1

            'newItem.RaisedFloorH = 0.1
            'newItem.RaisedFloorV = 0.1
            'newItem.RaisedFloorC = 0.1

            ''newItem.OnVehicle = True
            'newItem.ActiveVH = True
            'newItem.ActiveVV = True
            'newItem.ActiveVC = True
            ''newItem.LineType = TechLineType.Normal

            'Dim feedback As String = String.Empty

            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)({newItem})
            'Assert.IsTrue(target.TechLines.Count = 1)
            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)()
            'Assert.IsTrue(target.TechLines.Count = 0)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListADD1AndModify()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As ISSMTechList = New SSMTechList(gen.BusParameters.BusFloorType)
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            'Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            ''newItem.Units = "fraction"
            'newItem.Category = "Insulation"
            'newItem.BenefitName = "Benefit1"

            'newItem.LowFloorH = 0.1
            'newItem.LowFloorV = 0.1
            'newItem.LowFloorC = 0.1

            'newItem.SemiLowFloorH = 0.1
            'newItem.SemiLowFloorV = 0.1
            'newItem.SemiLowFloorC = 0.1

            'newItem.RaisedFloorH = 0.1
            'newItem.RaisedFloorV = 0.1
            'newItem.RaisedFloorC = 0.1

            ''newItem.OnVehicle = True
            'newItem.ActiveVH = True
            'newItem.ActiveVV = True
            'newItem.ActiveVC = True
            ''newItem.LineType = TechLineType.Normal

            ''Add
            'CType(target, SSMTechList).TechLines = New List(Of ISSMTechnology)({newItem})

            ''Modify
            'newItem.LowFloorC = 0.99
            'Assert.IsTrue(target.TechLines(0).IsEqualTo(newItem))
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListADD1andDeleteIt()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As SSMTechList = New SSMTechList()
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            'newItem.Units = "fraction"
            newItem.Category = "Insulation"
            newItem.BenefitName = "Benefit1"

            newItem.LowFloorH = 0.1
            newItem.LowFloorV = 0.1
            newItem.LowFloorC = 0.1

            newItem.SemiLowFloorH = 0.1
            newItem.SemiLowFloorV = 0.1
            newItem.SemiLowFloorC = 0.1

            newItem.RaisedFloorH = 0.1
            newItem.RaisedFloorV = 0.1
            newItem.RaisedFloorC = 0.1

            'newItem.OnVehicle = True
            newItem.ActiveVH = True
            newItem.ActiveVV = True
            newItem.ActiveVC = True
            'newItem.LineType = TechLineType.Normal

            'CType(target, SSMTechList).TechLines = New List(Of SSMTechnology)({newItem})
            'Assert.IsTrue(target.TechLines.Count = 1)
            'CType(target, SSMTechList).TechLines = New List(Of SSMTechnology)()
            'Assert.IsTrue(target.TechLines.Count = 0)
        End Sub

        <Test()>
        Public Sub Instantiate_TechListTestEMPTYListandDeleteNonExistantItem()


            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            'Dim target As SSMTechList = New SSMTechList()
            'target.TechLines = SSMTechnologiesReader.ReadFromFile(GOODTechListEMPTYLIST).Items

            Dim newItem As SSMTechnology = New SSMTechnology()
            'newItem.BusFloorType = gen.BusParameters.BusFloorType

            'newItem.Units = "fraction"
            newItem.Category = "Insulation"
            newItem.BenefitName = "Benefit1"

            newItem.LowFloorH = 0.1
            newItem.LowFloorV = 0.1
            newItem.LowFloorC = 0.1

            newItem.SemiLowFloorH = 0.1
            newItem.SemiLowFloorV = 0.1
            newItem.SemiLowFloorC = 0.1

            newItem.RaisedFloorH = 0.1
            newItem.RaisedFloorV = 0.1
            newItem.RaisedFloorC = 0.1

            'newItem.OnVehicle = True
            newItem.ActiveVH = True
            newItem.ActiveVV = True
            newItem.ActiveVC = True
            'newItem.LineType = TechLineType.Normal

            'CType(target, SSMTechList).TechLines = New List(Of SSMTechnology)()
        End Sub

        'TechListLineTests
        <Test()>
        Public Sub Instantiate_NewTechListLine()

            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            Dim ttl As SSMTechnology = New SSMTechnology()
            'ttl.BusFloorType = gen.BusParameters.BusFloorType

            Assert.IsNotNull(ttl)
        End Sub

        <Test()>
        Public Sub TechBenefitLineCompareAsEqual()

            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            Dim ttl1 As SSMTechnology = New SSMTechnology()
            'ttl1.BusFloorType = gen.BusParameters.BusFloorType

            Dim ttl2 As SSMTechnology = New SSMTechnology()
            'ttl2.BusFloorType = gen.BusParameters.BusFloorType

            'Assert.IsTrue(ttl1.IsEqualTo(ttl2))
        End Sub

        '<TestCase("Units")> _
        '<TestCase("LineType")> _
        <Test()> _
        <TestCase("Category")> _
        <TestCase("BenefitName")> _
        <TestCase("ActiveVC")> _
        <TestCase("ActiveVH")> _
        <TestCase("ActiveVV")> _
        <TestCase("LowFloorC")> _
        <TestCase("LowFloorV")> _
        <TestCase("LowFloorH")> _
        <TestCase("SemiLowFloorC")> _
        <TestCase("SemiLowFloorH")> _
        <TestCase("SemiLowFloorV")> _
        <TestCase("RaisedFloorC")> _
        <TestCase("RaisedFloorH")> _
        <TestCase("RaisedFloorV")> _
        <TestCase("OnVehicle")>
        Public Sub TechBenefitLineCompareAsUnequal(prop As String)

            Dim gen As ISSMDeclarationInputs = New SSMInputs(Nothing)

            Dim ttl1 As SSMTechnology = New SSMTechnology()
            'ttl1.BusFloorType = gen.BusParameters.BusFloorType

            Dim ttl2 As SSMTechnology = New SSMTechnology()
            'ttl2.BusFloorType = gen.BusParameters.BusFloorType

            Select Case prop


                Case "Category"
                    ttl2.Category = "NOT"
                Case "BenefitName"
                    ttl2.BenefitName = "NOT"
                Case "ActiveVC"
                    ttl2.ActiveVC = True
                Case "ActiveVH"
                    ttl2.ActiveVH = True
                Case "ActiveVV"
                    ttl2.ActiveVV = True
                    'Case "LineType"
                    '	ttl2.LineType = TechLineType.HVCActiveSelection
                Case "LowFloorC"
                    ttl2.LowFloorC = 1
                Case "LowFloorV"
                    ttl2.LowFloorV = 1
                Case "LowFloorH"
                    ttl2.LowFloorH = 1
                Case "SemiLowFloorC"
                    ttl2.SemiLowFloorC = 1
                Case "SemiLowFloorH"
                    ttl2.SemiLowFloorH = 1
                Case "SemiLowFloorV"
                    ttl2.SemiLowFloorH = 1
                Case "RaisedFloorC"
                    ttl2.RaisedFloorC = 1
                Case "RaisedFloorH"
                    ttl2.RaisedFloorH = 1
                Case "RaisedFloorV"
                    ttl2.RaisedFloorV = 1
                    'Case "Units"
                    '	ttl2.Units = "NONE"
                Case "OnVehicle"
                    'ttl2.OnVehicle = True

            End Select


            'Assert.IsFalse(ttl1.IsEqualTo(ttl2))
        End Sub

        'SSMTOOL Persistance
        <Test()>
        Public Sub SaveAndRetreiveTest()

            Const filePath As String = "SSMTOOLTestSaveRetreive.json"
            Dim success As Boolean


            Dim mission As New Mission With {
                .MissionType = MissionType.HeavyUrban,
                .BusParameter = New BusParameters() With {
                    .HVACCompressorType = HeatPumpType.non_R_744_2_stage,
                    .HVACAuxHeaterPower = 18000.0.SI(Of Watt),
                    .HVACConfiguration = BusHVACSystemConfiguration.Configuration6,
                    .DoubleDecker = False,
                    .BodyHeight = 2.7.SI(Of Meter),
                    .VehicleWidth = 2.55.SI(Of Meter),
                    .VehicleLength = 12.SI(Of Meter),
                    .PassengerDensityLow = 3.SI(Of PerSquareMeter),
                    .PassengerDensityRef = 3.SI(Of PerSquareMeter)
                    }
                    }

            Dim auxInput as IBusAuxiliariesDeclarationData = nothing

            Dim dao = New DeclarationDataAdapterCompletedBusGeneric()
            Dim target As SSMTOOL = New SSMTOOL(dao.CreateSSMModelParameters(auxInput, mission,
                                                                             FuelData.Diesel, LoadingType.ReferenceLoad))

            success = BusAuxWriter.SaveSSMConfig(target.SSMInputs, filePath)
            'success = target.Save(filePath)
            Assert.IsTrue(success)

            'change something
            CType(target.SSMInputs.BoundaryConditions, SSMInputs).VentilationRate = 202.202.SI (Of PerSecond)

            Assert.AreEqual(202.202, target.SSMInputs.BoundaryConditions.VentilationRate.Value(), 1e-3)

            'Retreive
            'success = target.Load(filePath)
            Try
                target = New SSMTOOL(SSMInputData.ReadFile(filePath, utils.GetDefaultVehicleData(),
                                                           DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions))
            Catch
                success = false
            end try
            Assert.IsTrue(success)

            Assert.AreEqual(20.SI(Unit.SI.Per.Hour).Value(), target.SSMInputs.BoundaryConditions.VentilationRate.Value(),
                            1e-3)
        End Sub

        'SSMInputs Comparison -- MQ: 2020-01-28: testcases no longer needed - ssminputdata and ssmtool is not used to 'edit' ssm data
        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_GENINPUTS_EQUAL()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    '    New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList)) _
        '    '', New HVACConstants())
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                        DeclarationData.BusAuxiliaries.
        '    '                                                           DefaultEnvironmentalConditions,
        '    '                                                        DeclarationData.BusAuxiliaries.SSMTechnologyList)) _
        '    ', New HVACConstants())


        '    'Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_GENINPUTS_UNEQUAL()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                        DeclarationData.BusAuxiliaries.
        '    '                                                           DefaultEnvironmentalConditions,
        '    '                                                        DeclarationData.BusAuxiliaries.SSMTechnologyList)) _
        '    ' New HVACConstants())

        '    'Alter somthing
        '    'CType(ssmTool1.genInputs, IssmInputs)._vehicle.Length = 11.SI(Of Meter)
        '    CType(ssmTool1.SSMInputs, SSMInputs).CoolingBoundaryTemperature =
        '        99.0.DegCelsiusToKelvin()

        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                        DeclarationData.BusAuxiliaries.
        '    '                                                           DefaultEnvironmentalConditions,
        '    '                                                        DeclarationData.BusAuxiliaries.SSMTechnologyList)) _
        '    ', New HVACConstants())


        '    'Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        ''TechListBenefitLine Comparison
        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_TECHLIST_EQUAL()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))


        '    'Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_TECHLIST_EMPTYLISTS_UNEQUALCOUNT()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))

        '    'Change something on techlist
        '    AddDefaultTechLine(ssmTool1)

        '    'Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_EQUAL()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))

        '    'Change something on techlist
        '    AddDefaultTechLine(ssmTool1)
        '    AddDefaultTechLine(ssmTool2)

        '    'Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_SINGLEKeyValueDifference()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                        DeclarationData.BusAuxiliaries.
        '    '                                                           DefaultEnvironmentalConditions,
        '    '                                                        DeclarationData.BusAuxiliaries.SSMTechnologyList))
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))

        '    'Change something on techlist
        '    AddDefaultTechLine(ssmTool1)
        '    AddDefaultTechLine(ssmTool2)

        '    'Make Unequal
        '    'CType(ssmTool2.TechList.TechLines(0), SSMTechnology).BenefitName = "Doobie"

        '    'Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub

        '<Test()>
        'Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_SINGLEValueDifference()

        '    Const filePath As String = "SSMTOOLTestSaveRetreive.json"


        '    Dim ssmTool1 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))
        '    Dim ssmTool2 As SSMTOOL = New SSMTOOL(Utils.GetAuxTestConfig().SSMInputs)
        '    'New SSMTOOL(SSMInputData.ReadFile(filePath,utils.GetDefaultVehicleData(),
        '    '                                                            DeclarationData.BusAuxiliaries.
        '    '                                                               DefaultEnvironmentalConditions,
        '    '                                                            DeclarationData.BusAuxiliaries.SSMTechnologyList))

        '    'Change something on techlist
        '    AddDefaultTechLine(ssmTool1)
        '    AddDefaultTechLine(ssmTool2)

        '    'Make Unequal
        '    'CType(ssmTool2.TechList.TechLines(0), SSMTechnology).ActiveVV = False

        '    'Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
        'End Sub
    End Class
End Namespace


