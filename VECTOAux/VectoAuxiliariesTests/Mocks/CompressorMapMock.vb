
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils

Namespace Mocks
    Public Class CompressorMapMock
        Implements ICompressorMap

        Dim failing As Boolean

        Public Sub New(ByVal isFailing As Boolean)
            failing = isFailing
        End Sub

        public Function interpolate(rpm As PerSecond) As CompressorResult Implements ICompressorMap.Interpolate
            Return New CompressorResult() With {
                .PowerOff =5.0.SI (Of Watt)(),
                .PowerOn = 8.0.SI (Of Watt)(),
                .FlowRate = 2.0.SI(Unit.SI.Liter.Per.Minute).cast (Of NormLiterPerSecond)(),
                .RPM = rpm}
        End Function


        Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As JoulePerNormLiter _
            Implements ICompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate

            Return 0.01.SI(Unit.SI.Watt.Per.Liter.Per.Hour).Cast (Of JoulePerNormLiter)
        End Function

        Public ReadOnly Property Technology As String Implements ICompressorMap.Technology
        get
                Return ""
        End Get
        End Property

        Public readonly property Source As String Implements ICompressorMap.Source
            get
                return ""
            End Get
        End property


        'Public Event AuxiliaryEvent As AuxiliaryEventEventHandler Implements IAuxiliaryEvent.AuxiliaryEvent
    End Class
End Namespace