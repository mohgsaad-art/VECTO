Namespace Electrics

    Public Class AverageElectricalDemand

        Public _powerNetVoltage As Single = 26.3

        ''' <summary>
        ''' Alternator Model
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        'Public ReadOnly Property Alternator() As IAlternator
        '    Get
        '        Return _alternator
        '    End Get
        'End Property

        Private _electricalConsumers As IElectricalConsumerList

        ''' <summary>
        ''' Creates a new instance of the AverageElectricalDemand class
        ''' </summary>
        ''' <param name="alternator">Alternator Models</param>
        ''' <param name="electricalConsumers">List of Electrical Consumers</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal electricalConsumers As IElectricalConsumerList, powerNetVoltage As Single)
            _powerNetVoltage = powerNetVoltage
            _electricalConsumers = electricalConsumers
        End Sub

        ''' <summary>
        ''' Initialised the Alternator Models
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean

        End Function

        ''' <summary>
        ''' Gets the total average power at the alternator for all electrical consumers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAveragePowerDemandAtAlternator() As Single

            ' Return _electricalConsumers.GetTotalAverageDemandAmps()

            Return 5000 ' TODO:FIX THIS
        End Function


        Public Function GetAveragePowerAtCrank(ByVal engineRpm As Integer) As Single
            Dim elecPower As Single = GetAveragePowerDemandAtAlternator()
            Dim alternatorEfficiency As Single = 0 '_alternator.GetEfficiency(engineRpm) TODO: Fix THis.
            Dim demandFromAlternator As Single = elecPower / alternatorEfficiency
            Dim powerAtCrank As Single = 0 ' TODO : FIX THIS demandFromAlternator / _alternator.PulleyGearEfficiency
            Return powerAtCrank
        End Function


    End Class
End Namespace