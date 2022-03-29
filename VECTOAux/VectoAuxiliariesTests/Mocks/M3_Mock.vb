
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules

Public Class M3_Mock
    Implements IM3_AveragePneumaticLoadDemand

    Public _GetAveragePowerDemandAtCrankFromPneumatics As Watt
    Public _TotalAirConsumedPerCycle As NormLiterPerSecond
    Private _totalAirDemand As NormLiter


    Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Watt _
        Implements IM3_AveragePneumaticLoadDemand.GetAveragePowerDemandAtCrankFromPneumatics
        Return _GetAveragePowerDemandAtCrankFromPneumatics
    End Function

    'Public ReadOnly Property TotalAirDemand As NormLiter Implements IM3_AveragePneumaticLoadDemand.TotalAirDemand
    '    Get
    '        Return _totalAirDemand
    '    End Get
    'End Property

    Public ReadOnly Property AverageAirConsumed() As NormLiterPerSecond _
        Implements IM3_AveragePneumaticLoadDemand.AverageAirConsumed
        get
            Return _TotalAirConsumedPerCycle
        end get
    End property


    Public Sub New()
    End Sub

    Public Sub New(GetAveragePowerDemandAtCrankFromPneumatics As Double, TotalAirConsumedPerCycle As Double)

        _GetAveragePowerDemandAtCrankFromPneumatics = GetAveragePowerDemandAtCrankFromPneumatics.SI (Of Watt)()
        _TotalAirConsumedPerCycle = TotalAirConsumedPerCycle.SI (Of NormLiterPerSecond)()
    End Sub

    Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
        Throw New NotImplementedException
    End Sub
End Class

