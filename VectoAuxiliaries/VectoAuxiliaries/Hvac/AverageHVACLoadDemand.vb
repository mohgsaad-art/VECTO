Imports VectoAuxiliaries.Electrics

Namespace Hvac
    Public Class AverageHVACLoadDemand
        Dim map As IHVACMap
        Dim alternator As IAlternator

        Private _region As Integer
        Private _season As Integer
        Private _gerPulleyEffifiency As Single

        Public Property Region() As Integer
            Get
                Return _region
            End Get
            Set(value As Integer)
                _region = value
            End Set
        End Property

        Public Property Season() As Integer
            Get
                Return _season
            End Get
            Set(value As Integer)
                _season = value
            End Set
        End Property

        Public Property GerPulleyEffifiency() As Single
            Get
                Return _gerPulleyEffifiency
            End Get
            Set(value As Single)
                _gerPulleyEffifiency = value
            End Set
        End Property

        Public Sub New(ByVal map As IHVACMap, ByVal alternator As IAlternator)
            Me.map = map
            Me.alternator = alternator
        End Sub

        Public Function Initialise() As Boolean
            Return alternator.Initialise() AndAlso map.Initialise()
        End Function

        Public Function AverageMechanicalPowerAtCrank() As Single
            Return map.GetMechanicalDemand(_region, _season)
        End Function

        Function AverageElectricalPowerAtAlternator() As Single
            Return map.GetElectricalDemand(_region, _season)
        End Function

        Function AverageElectricalPowerAtCrank(ByVal rpm As Integer) As Single
            Throw New NotImplementedException
        End Function
    End Class
End Namespace