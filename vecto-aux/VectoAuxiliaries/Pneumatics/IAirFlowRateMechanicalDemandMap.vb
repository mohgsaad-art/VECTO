
Namespace Pneumatics

    Public Interface IAirFlowRateMechanicalDemandMap


        ''' <summary>
        ''' Initialises the map with a privately stored field referring to filename and path.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Gets Power From Map for a given flow rate
        ''' </summary>
        ''' <param name="flowRate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPower(flowRate As Integer) As Integer


    End Interface

End Namespace



