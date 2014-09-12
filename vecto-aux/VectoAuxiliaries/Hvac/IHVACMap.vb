Namespace Hvac
    Public Interface IHVACMap
        ''' <summary>
        ''' Initialise the map data
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Get the average mechanical demand for the given imput parameters
        ''' </summary>
        ''' <param name="region"></param>
        ''' <param name="season"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetMechanicalDemand(ByVal region As Integer, ByVal season As Integer) As Integer

        ''' <summary>
        ''' Get the average electrical demand for the given imput parameters
        ''' </summary>
        ''' <param name="region"></param>
        ''' <param name="season"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetElectricalDemand(ByVal region As Integer, ByVal season As Integer) As Integer
    End Interface
End NameSpace