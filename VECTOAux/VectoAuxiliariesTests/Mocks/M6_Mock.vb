

Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Public Class M6_Mock
	Implements IM6

	Public Property _AveragePowerDemandAtCrankFromPneumatics As Watt
	Public Property _AvgPowerDemandAtCrankFromElectricsIncHVAC As Watt
	Public Property _OverrunFlag As Boolean
	Public Property _SmartElecAndPneumaticAirCompPowerGenAtCrank As Watt
	Public Property _SmartElecAndPneumaticAltPowerGenAtCrank As Watt
	Public Property _SmartElecAndPneumaticsCompressorFlag As Boolean
	Public Property _SmartElecOnlyAltPowerGenAtCrank As Watt
	Public Property _SmartPneumaticOnlyAirCompPowerGenAtCrank As Watt
	Public Property _SmartPneumaticsOnlyCompressorFlag As Boolean


	Public ReadOnly Property AveragePowerDemandAtCrankFromPneumatics As Watt _
		Implements IM6.AveragePowerDemandAtCrankFromPneumatics
		Get
			Return _AveragePowerDemandAtCrankFromPneumatics
		End Get
	End Property

	Public ReadOnly Property AvgPowerDemandAtCrankFromElectricsIncHVAC As Watt _
		Implements IM6.AvgPowerDemandAtCrankFromElectricsIncHVAC
		Get
			Return _AvgPowerDemandAtCrankFromElectricsIncHVAC
		End Get
	End Property

	Public ReadOnly Property OverrunFlag As Boolean Implements IM6.OverrunFlag
		Get
			Return _OverrunFlag
		End Get
	End Property

	Public ReadOnly Property SmartElecAndPneumaticAirCompPowerGenAtCrank As Watt _
		Implements IM6.SmartElecAndPneumaticAirCompPowerGenAtCrank
		Get
			Return _SmartElecAndPneumaticAirCompPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartElecAndPneumaticAltPowerGenAtCrank As Watt _
		Implements IM6.SmartElecAndPneumaticAltPowerGenAtCrank
		Get
			Return _SmartElecAndPneumaticAltPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartElecAndPneumaticsCompressorFlag As Boolean _
		Implements IM6.SmartElecAndPneumaticsCompressorFlag
		Get
			Return _SmartElecAndPneumaticsCompressorFlag
		End Get
	End Property

	Public ReadOnly Property SmartElecOnlyAltPowerGenAtCrank As Watt Implements IM6.SmartElecOnlyAltPowerGenAtCrank
		Get
			Return _SmartElecOnlyAltPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartPneumaticOnlyAirCompPowerGenAtCrank As Watt _
		Implements IM6.SmartPneumaticOnlyAirCompPowerGenAtCrank
		Get
			Return _SmartPneumaticOnlyAirCompPowerGenAtCrank
		End Get
	End Property

	Public ReadOnly Property SmartPneumaticsOnlyCompressorFlag As Boolean Implements IM6.SmartPneumaticsOnlyCompressorFlag
		Get
			Return _SmartPneumaticsOnlyCompressorFlag
		End Get
	End Property


	Public Sub New()
	End Sub

	Public Sub New(AveragePowerDemandAtCrankFromPneumatics As Double,
					AvgPowerDemandAtCrankFromElectricsIncHVAC As Double,
					OverrunFlag As Boolean,
					SmartElecAndPneumaticAirCompPowerGenAtCrank As Double,
					SmartElecAndPneumaticAltPowerGenAtCrank As Double,
					SmartElecAndPneumaticsCompressorFlag As Boolean,
					SmartElecOnlyAltPowerGenAtCrank As Double,
					SmartPneumaticOnlyAirCompPowerGenAtCrank As Double,
					SmartPneumaticsOnlyCompressorFlag As Boolean)


		_AveragePowerDemandAtCrankFromPneumatics = AveragePowerDemandAtCrankFromPneumatics.SI(Of Watt)()
		_AvgPowerDemandAtCrankFromElectricsIncHVAC = AvgPowerDemandAtCrankFromElectricsIncHVAC.SI(Of Watt)()
		_OverrunFlag = OverrunFlag
		_SmartElecAndPneumaticAirCompPowerGenAtCrank = SmartElecAndPneumaticAirCompPowerGenAtCrank.SI(Of Watt)()
		_SmartElecAndPneumaticAltPowerGenAtCrank = SmartElecAndPneumaticAltPowerGenAtCrank.SI(Of Watt)()
		_SmartElecAndPneumaticsCompressorFlag = SmartElecAndPneumaticsCompressorFlag
		_SmartElecOnlyAltPowerGenAtCrank = SmartElecOnlyAltPowerGenAtCrank.SI(Of Watt)()
		_SmartPneumaticOnlyAirCompPowerGenAtCrank = SmartPneumaticOnlyAirCompPowerGenAtCrank.SI(Of Watt)()
		_SmartPneumaticsOnlyCompressorFlag = SmartPneumaticsOnlyCompressorFlag
	End Sub

    Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
        Throw New NotImplementedException
    End Sub
End Class

