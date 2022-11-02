

Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Public Class M7_Mock
	Implements IM7

	Private _SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Watt
	Private _SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Watt
	Private _SmartElectricalOnlyAuxAltPowerGenAtCrank As Watt
	Private _SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Watt

	Public ReadOnly Property SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Watt _
		Implements IM7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
		Get
			Return _SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Watt _
		Implements IM7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
		Get
			Return _SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartElectricalOnlyAuxAltPowerGenAtCrank As Watt _
		Implements IM7.SmartElectricalOnlyAuxAltPowerGenAtCrank
		Get
			Return _SmartElectricalOnlyAuxAltPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Watt _
		Implements IM7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
		Get
			Return _SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
		End Get
	End Property


	'Constructors
	Public Sub New()
	End Sub

	Public Sub New(SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Watt,
					SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Watt,
					SmartElectricalOnlyAuxAltPowerGenAtCrank As Watt,
					SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Watt)

		_SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank = SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
		_SmartElectricalAndPneumaticAuxAltPowerGenAtCrank = SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
		_SmartElectricalOnlyAuxAltPowerGenAtCrank = SmartElectricalOnlyAuxAltPowerGenAtCrank
		_SmartPneumaticOnlyAuxAirCompPowerGenAtCrank = SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
	End Sub

	Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
		Throw New NotImplementedException
	End Sub
End Class