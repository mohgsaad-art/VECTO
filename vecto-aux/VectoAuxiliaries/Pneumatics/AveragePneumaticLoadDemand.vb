Imports VectoAuxiliaries.Pneumatics

Namespace Pneumatics

    Public Class AveragePneumaticLoadDemand


        Private _map As ICompressorMap
        Private _pneumaticConsumers As List(Of IPneumaticConsumer)
        Private _pulleyGearEfficiency As Single

        Public Property PulleyGearEfficiancy As Single

            Private Set(value As Single)
                _pulleyGearEfficiency = value
            End Set
            Get
                Return _pulleyGearEfficiency
            End Get

        End Property

        Public Property TotalCycleTimeSeconds As Single


        Public ReadOnly Property PneumaticConsumers As List(Of IPneumaticConsumer)
            Get
                Return _pneumaticConsumers
            End Get
        End Property

        'Constructors
        Public Sub New(iMap As ICompressorMap, iTotalCycleTimeSeconds As Integer, iPulleyGearEfficiency As Single)

            _map = iMap
            _pulleyGearEfficiency = iPulleyGearEfficiency
            _TotalCycleTimeSeconds = iTotalCycleTimeSeconds

        End Sub


        'Get Average Power Demand @ Crank From Pneumatics
        Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Single



            'TODO
            Return -1

        End Function


        'Get Total Required Air Delivery Rate
        Public Function GetTotalRequiredAirPerCompressorUnitDeliveryRate() As Single

            Return PneumaticConsumers.Sum(Function(item) item.VolumePerCycle())

        End Function





    End Class

End Namespace


