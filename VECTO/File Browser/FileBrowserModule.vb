' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

''' <summary>
''' Global File Brower properties and cFilebrowser instances.
''' </summary>
''' <remarks></remarks>
Public Module FileBrowserModule
	Public ReadOnly FileBrowserFolderHistory(19) As String
	Public Drives() As String
	Public FileBrowserFolderHistoryIninialized As Boolean
	Public FileHistoryPath As String
	'-----------------------------
	Public FolderFileBrowser As FileBrowser
	Public JobfileFileBrowser As FileBrowser
	Public TextFileBrowser As FileBrowser
	Public VehicleFileBrowser As FileBrowser
	Public VehicleXMLFileBrowser As FileBrowser
    public ManRXMLFileBrowser As FileBrowser
	Public DrivingCycleFileBrowser As FileBrowser
	Public PTODrivingCycleStandstillFileBrowser As FileBrowser
    Public PTODrivingCycleDrivingFileBrowser As FileBrowser
	Public FuelConsumptionMapFileBrowser As FileBrowser
    Public FullLoadCurveFileBrowser As FileBrowser

    Public ElectricMachineMaxTorqueFileBrowser As FileBrowser
    Public ElectricMachineDragTorqueFileBrowser As FileBrowser
    Public ElectricMachineEfficiencyMapFileBrowser As FileBrowser

	public PropulsionTorqueLimitFileBrowser as FileBrowser

    Public BatteryMaxCurrentCurveFileBrowser As FileBrowser
    Public BatteryInternalResistanceCurveFileBrowser As FileBrowser
    Public BatterySoCCurveFileBrowser As FileBrowser

    Public HCUFileBrowser As FileBrowser

	public BusAuxFileBrowser As FileBrowser
	public BusAuxCompressorMapFileBrowser As FileBrowser

    Public EngineFileBrowser As FileBrowser
	Public GearboxFileBrowser As FileBrowser
    Public TCUFileBrowser As FileBrowser
	Public DriverAccelerationFileBrowser As FileBrowser
	Public DriverDecisionFactorTargetSpeedFileBrowser As FileBrowser
	Public DriverDecisionFactorVelocityDropFileBrowser As FileBrowser
    'public PTOSideloadCycleBrowser As FileBrowser
	Public AuxFileBrowser As FileBrowser

	Public GearboxShiftPolygonFileBrowser As FileBrowser
	Public TransmissionLossMapFileBrowser As FileBrowser
	Public PtoLossMapFileBrowser As FileBrowser
	Public RetarderLossMapFileBrowser As FileBrowser
	Public TorqueConverterFileBrowser As FileBrowser
	Public TorqueConverterShiftPolygonFileBrowser As FileBrowser
	Public CrossWindCorrectionFileBrowser As FileBrowser

    Public ModalResultsFileBrowser As FileBrowser

    Public ElectricMotorFileBrowser As FileBrowser
    Public REESSFileBrowser As FileBrowser

	public EmADCLossMapFileBrowser As FileBrowser
	Public IEPCFileBrowser As FileBrowser
	Public IEPCFLCFileBrowser As FileBrowser
	Public IEPCDragFileBrowser As FileBrowser
	Public IEPCPowerMapFileBrowser As FileBrowser

	Public IHPCFileBrowser As FileBrowser
	Public IHPCPowerMapFileBrowser As FileBrowser
	Public IHPCFullLoadCurveFileBrowser As FileBrowser
	public IHPCDragCurveFileBrowser As FileBrowser

End Module
