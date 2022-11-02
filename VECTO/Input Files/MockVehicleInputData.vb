Imports System.Collections.Generic
Imports System.Xml
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

Public Class MockVehicleInputData
    Implements IVehicleDeclarationInputData
    Public Property DataSource As DataSource Implements IComponentInputData.DataSource
    Public Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
    Public Property Manufacturer As String Implements IComponentInputData.Manufacturer
    Public Property Model As String Implements IComponentInputData.Model
    Public Property [Date] As DateTime Implements IComponentInputData.[Date]
    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
    Public Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
    Public Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
    Public Property DigestValue As DigestData Implements IComponentInputData.DigestValue
    Public Property Identifier As String Implements IVehicleDeclarationInputData.Identifier
    Public Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
    Public Property VIN As String Implements IVehicleDeclarationInputData.VIN
    Public Property LegislativeClass As LegislativeClass? Implements IVehicleDeclarationInputData.LegislativeClass
    Public Property VehicleCategory As VehicleCategory Implements IVehicleDeclarationInputData.VehicleCategory
    Public Property AxleConfiguration As AxleConfiguration Implements IVehicleDeclarationInputData.AxleConfiguration
    Public Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
    Public Property GrossVehicleMassRating As Kilogram Implements IVehicleDeclarationInputData.GrossVehicleMassRating
    Public Property TorqueLimits As IList(Of ITorqueLimitInputData) Implements IVehicleDeclarationInputData.TorqueLimits
    Public Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
    Public Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
    Public Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
    Public Property SleeperCab As Boolean? Implements IVehicleDeclarationInputData.SleeperCab
    Public ReadOnly Property AirdragModifiedMultistep As Boolean? Implements IVehicleDeclarationInputData.AirdragModifiedMultistep
    Public Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem

    Public Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData _
        Implements IVehicleDeclarationInputData.ADAS

    Public Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
    Public Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
    Public Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
    Public Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
    Public Property MaxNetPower2 As Watt Implements IVehicleDeclarationInputData.MaxNetPower2
    Public ReadOnly Property ExemptedTechnology As String Implements IVehicleDeclarationInputData.ExemptedTechnology
    Public ReadOnly Property RegisteredClass As RegistrationClass? Implements IVehicleDeclarationInputData.RegisteredClass
    Public ReadOnly Property NumberPassengerSeatsUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsUpperDeck
    Public ReadOnly Property NumberPassengerSeatsLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengerSeatsLowerDeck
    Public ReadOnly Property NumberPassengersStandingLowerDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingLowerDeck
    Public ReadOnly Property NumberPassengersStandingUpperDeck As Integer? Implements IVehicleDeclarationInputData.NumberPassengersStandingUpperDeck
    Public ReadOnly Property CargoVolume As CubicMeter Implements IVehicleDeclarationInputData.CargoVolume
    Public ReadOnly Property VehicleCode As VehicleCode? Implements IVehicleDeclarationInputData.VehicleCode
    Public ReadOnly Property LowEntry As Boolean? Implements IVehicleDeclarationInputData.LowEntry
    Public ReadOnly Property Articulated As Boolean Implements IVehicleDeclarationInputData.Articulated
    Public ReadOnly Property Height As Meter Implements IVehicleDeclarationInputData.Height
    Public ReadOnly Property Length As Meter Implements IVehicleDeclarationInputData.Length
    Public ReadOnly Property Width As Meter Implements IVehicleDeclarationInputData.Width
    Public ReadOnly Property EntranceHeight As Meter Implements IVehicleDeclarationInputData.EntranceHeight
    Public ReadOnly Property DoorDriveTechnology As ConsumerTechnology? Implements IVehicleDeclarationInputData.DoorDriveTechnology
    Public ReadOnly Property VehicleDeclarationType As VehicleDeclarationType Implements IVehicleDeclarationInputData.VehicleDeclarationType
    Public ReadOnly Property ElectricMotorTorqueLimits As Dictionary(Of PowertrainPosition, List(Of Tuple(Of Volt, TableData))) Implements IVehicleDeclarationInputData.ElectricMotorTorqueLimits
    Public ReadOnly Property BoostingLimitations As TableData Implements IVehicleDeclarationInputData.BoostingLimitations
        Get
            Throw New NotImplementedException
        End Get
    End Property

    Public Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
    Public ReadOnly Property XMLSource As XmlNode Implements IVehicleDeclarationInputData.XMLSource
    Public ReadOnly Property VehicleTypeApprovalNumber As String Implements IVehicleDeclarationInputData.VehicleTypeApprovalNumber
    Public ReadOnly Property ArchitectureID As ArchitectureID Implements IVehicleDeclarationInputData.ArchitectureID
    Public ReadOnly Property OvcHev As Boolean Implements IVehicleDeclarationInputData.OvcHev
    Public ReadOnly Property MaxChargingPower As Watt Implements IVehicleDeclarationInputData.MaxChargingPower
End Class