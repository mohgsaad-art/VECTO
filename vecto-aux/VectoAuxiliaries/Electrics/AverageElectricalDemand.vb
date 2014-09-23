Namespace Electrics

    Public Class AverageElectricalDemand

        Private _alternator As IAlternator
        ''' <summary>
        ''' Alternator Model
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        Public ReadOnly Property Alternator() As IAlternator
            Get
                Return _alternator
            End Get
        End Property

        Private _electricalConsumers As List(Of IElectricalConsumer)
        ''' <summary>
        ''' List of Electrical Consumers
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ElectricalConsumers() As List(Of IElectricalConsumer)
            Get
                Return _electricalConsumers
            End Get
        End Property

        ''' <summary>
        ''' Creates a new instance of the AverageElectricalDemand class
        ''' </summary>
        ''' <param name="alternator">Alternator Models</param>
        ''' <param name="electricalConsumers">List of Electrical Consumers</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal alternator As IAlternator, ByVal electricalConsumers As List(Of IElectricalConsumer))
            _alternator = alternator
            _electricalConsumers = electricalConsumers
        End Sub

        ''' <summary>
        ''' Initialised the Alternator Models
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean
            Return _alternator.Initialise()
        End Function

        ''' <summary>
        ''' Gets the total average power at the alternator for all electrical consumers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAveragePowerDemandAtAlternator() As Single
            Dim total As Single = (From ctx In ElectricalConsumers
                    Select ctx.Power).Sum()
            Return total
        End Function

        ''' <summary>
        ''' GEts the total average power at the crank for all electrical consumers as a given engine rpm
        ''' </summary>
        ''' <param name="engineRpm"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAveragePowerAtCrank(ByVal engineRpm As Integer) As Single
            Dim elecPower As Single = GetAveragePowerDemandAtAlternator()
            Dim alternatorEfficiency As Single = _alternator.GetEfficiency(engineRpm)
            Dim demandFromAlternator As Single = elecPower / alternatorEfficiency
            Dim powerAtCrank As Single = demandFromAlternator / _alternator.PulleyGearEfficiency
            Return powerAtCrank
        End Function
    End Class
End Namespace