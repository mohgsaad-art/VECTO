Imports TUGraz.VectoCommon.InputData

Public Class MockComponents
    Implements IVehicleComponentsDeclaration

    Public Property AirdragInputData As IAirdragDeclarationInputData _
        Implements IVehicleComponentsDeclaration.AirdragInputData

    Public Property GearboxInputData As IGearboxDeclarationInputData _
        Implements IVehicleComponentsDeclaration.GearboxInputData

    Public Property TorqueConverterInputData As ITorqueConverterDeclarationInputData _
        Implements IVehicleComponentsDeclaration.TorqueConverterInputData

    Public Property AxleGearInputData As IAxleGearInputData Implements IVehicleComponentsDeclaration.AxleGearInputData

    Public Property AngledriveInputData As IAngledriveInputData _
        Implements IVehicleComponentsDeclaration.AngledriveInputData

    Public Property EngineInputData As IEngineDeclarationInputData _
        Implements IVehicleComponentsDeclaration.EngineInputData

    Public Property AuxiliaryInputData As IAuxiliariesDeclarationInputData _
        Implements IVehicleComponentsDeclaration.AuxiliaryInputData

    Public Property RetarderInputData As IRetarderInputData Implements IVehicleComponentsDeclaration.RetarderInputData

    Public Property PTOTransmissionInputData As IPTOTransmissionInputData _
        Implements IVehicleComponentsDeclaration.PTOTransmissionInputData

    Public Property AxleWheels As IAxlesDeclarationInputData Implements IVehicleComponentsDeclaration.AxleWheels
    Public ReadOnly Property BusAuxiliaries As IBusAuxiliariesDeclarationData Implements IVehicleComponentsDeclaration.BusAuxiliaries
    Public ReadOnly Property ElectricStorage As IElectricStorageSystemDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricStorage
    Public ReadOnly Property ElectricMachines As IElectricMachinesDeclarationInputData Implements IVehicleComponentsDeclaration.ElectricMachines
    Public ReadOnly Property IEPC As IIEPCDeclarationInputData Implements IVehicleComponentsDeclaration.IEPC
End Class