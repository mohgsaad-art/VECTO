Imports VectoAuxiliaries.Pneumatics

Namespace Pneumatics

    Public Class AveragePneumaticLoadDemand


        Private _compressor As IAirCompressor
        Private _pneumaticConsumers As List(Of IPneumaticConsumer)
        Private _pulleyGearEfficiency As Single

        Public Property PulleyGearEfficiency As Single

            Private Set(value As Single)
                _pulleyGearEfficiency = value
            End Set
            Get
                Return _pulleyGearEfficiency
            End Get

        End Property

        Public Property TotalCycleTimeSeconds As Integer

        Public ReadOnly Property PneumaticConsumers As List(Of IPneumaticConsumer)
            Get

                If _pneumaticConsumers Is Nothing Then
                    _pneumaticConsumers = New List(Of IPneumaticConsumer)
                End If
                Return _pneumaticConsumers

            End Get
        End Property

        'Constructors
        Public Sub New(iCompressor As IAirCompressor, iTotalCycleTimeSeconds As Integer, iPulleyGearEfficiency As Single, Optional consumers As List(Of IPneumaticConsumer) = Nothing)

            _compressor = iCompressor
            _pulleyGearEfficiency = iPulleyGearEfficiency
            _TotalCycleTimeSeconds = iTotalCycleTimeSeconds

            If iCompressor.Initialise() <> True Then
                Throw New ArgumentException("This Module requires a valid compressor")
            End If


            If Not consumers Is Nothing AndAlso consumers.Count > 0 Then
                _pneumaticConsumers = consumers
            Else
                _pneumaticConsumers = New List(Of IPneumaticConsumer)
            End If

        End Sub


        'Get Average Power Demand @ Crank From Pneumatics
        Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Single

            Dim averagePowerDemandPerCompressorUnitFlowRate As Single
            Dim effectiveTotalAirRequired As Single

            averagePowerDemandPerCompressorUnitFlowRate = _compressor.GetAveragePowerDemandPerCompressorUnitFlowRate

            effectiveTotalAirRequired = GetTotalRequiredAirPerCompressorUnitDeliveryRate() / TotalCycleTimeSeconds

            Return (averagePowerDemandPerCompressorUnitFlowRate * effectiveTotalAirRequired) / PulleyGearEfficiency


        End Function


        'Get Total Required Air Delivery Rate
        Public Function GetTotalRequiredAirPerCompressorUnitDeliveryRate() As Single

            Return PneumaticConsumers.Sum(Function(item) item.VolumePerActuation())

        End Function





    End Class

End Namespace


