Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Hvac


Namespace UnitTests

<TestFixture()>
Public Class BusDatabaseTests


Private Const GOODMAP              = "TestFiles\testBusDatabase.csv"
Private Const DUPLICATES           = "TestFiles\testBusDatabaseDuplicates.csv"
Private Const INSUFFICIENTROWSMAP  = "TestFiles\testBusDatabaseInsufficientRows.csv"
Private Const INVALIDLENGTHMAP     = "TestFiles\testBusDatabaseInvalidLenght.csv"
Private Const INVALIDWIDTHMAP      = "TestFiles\testBusDatabaseInvalidWidth.csv"
Private Const INVALIDHEIGHTMAP     = "TestFiles\testBusDatabaseInvalidHeight.csv"
Private Const INVALIDPASSEMGERSMAP = "TestFiles\testBusDatabaseInvalidPassengers.csv"


<Test()>
Public Sub BusCreateTest()

            Dim target As IBus = New Bus(1, "IVECO - Arway Intercity 10.6m", "raised floor", "diesel", 10.655, 2.55, 2.275, 47, False)


            Assert.IsNotNull(target)

            Assert.AreEqual("IVECO - Arway Intercity 10.6m", target.Model)
            Assert.AreEqual("raised floor", target.FloorType)
            Assert.AreEqual("diesel", target.EngineType)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalFloorTypeTest()

            Dim target As IBus = New Bus(2, "", "raised floor", "diesel", 10.655, 2.55, 2.275, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalModelTest()

            Dim target As IBus = New Bus(3, "ABC", "raised", "diesel", 10.655, 2.55, 2.275, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalEngineTypeTest()

            Dim target As IBus = New Bus(4, "ABC", "raised floor", "vapour", 10.655, 2.55, 2.275, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalWidthTest()

            Dim target As IBus = New Bus(5, "IVECO - Arway Intercity 10.6m", "raised floor", "diesel", 10.655, 0, 2.275, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalHeightTest()

            Dim target As IBus = New Bus(6, "IVECO - Arway Intercity 10.6m", "raised floor", "diesel", 10.655, 2.55, 0, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalLengthTest()

            Dim target As IBus = New Bus(7, "IVECO - Arway Intercity 10.6m", "raised floor", "diesel", 0, 2.55, 2.275, 47, False)

        End Sub

        <Test()> _
        <ExpectedException("System.ArgumentException")>
        Public Sub IllegalPassengersTest()

            Dim target As IBus = New Bus(8, "IVECO - Arway Intercity 10.6m", "raised floor", "diesel", 10.655, 2.55, 2.275, 1, False)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVGoodDataTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(GOODMAP)

            Assert.IsTrue(result)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVNotEnoughRowsTest()


            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(INSUFFICIENTROWSMAP)

            Assert.IsFalse(result)


        End Sub

        <Test()>
        Public Sub InitialiseFromCSVInvalidLengthTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(INVALIDLENGTHMAP)

            Assert.IsFalse(result)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVInvalidWidthTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(INVALIDWIDTHMAP)

            Assert.IsFalse(result)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVInvalidHeightTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(INVALIDHEIGHTMAP)

            Assert.IsFalse(result)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVInvalidPassengersTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(INVALIDPASSEMGERSMAP)

            Assert.IsFalse(result)

        End Sub

        <Test()>
        Public Sub InitialiseFromCSVDuplicatesTest()

            Dim target As IBusDatabase = New BusDatabase()

            Dim result As Boolean = target.Initialise(DUPLICATES)

            Assert.IsFalse(result)

        End Sub

        <Test()>
        Public Sub FindBusTest()

            Dim target As IBusDatabase = New BusDatabase()
            Dim result As Boolean = target.Initialise(GOODMAP)
            Assert.IsTrue(result)

            Dim busList = target.GetBuses("IVECO - Crossway Intercity 10.6m")

            Assert.AreEqual(1, busList.Count())

        End Sub

        <Test()>
        Public Sub FindMultipleBusesTest()

            Dim target As IBusDatabase = New BusDatabase()
            Dim result As Boolean = target.Initialise(GOODMAP)
            Assert.IsTrue(result)

            Dim busList = target.GetBuses("IVECO")

            Assert.AreEqual(28, busList.Count())

        End Sub

        <Test()>
        Public Sub FindAllBusesTest()

            Dim target As IBusDatabase = New BusDatabase()
            Dim result As Boolean = target.Initialise(GOODMAP)
            Assert.IsTrue(result)

            Dim busList = target.GetBuses("")

            Assert.AreEqual(158, busList.Count())


        End Sub

        <Test()>
        Public Sub FindNonExistantBus()

            Dim target As IBusDatabase = New BusDatabase()
            Dim result As Boolean = target.Initialise(GOODMAP)
            Assert.IsTrue(result)

            Dim busList = target.GetBuses("ZQZQZQ111ZQZQZQ")

            Assert.AreEqual(0, busList.Count())

        End Sub



End Class





End Namespace




