Namespace Pneumatics
    Public Interface IPneumaticConsumer
        ''' <summary>
        ''' Name of the consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property ConsumerName() As String

        ''' <summary>
        ''' Volume of Air consumed per cycle of the consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property VolumePerActuation() As Single

        ''' <summary>
        ''' Get the total volume of air required for a number of cycles
        ''' </summary>
        ''' <param name="cycles">Number of cycles of consumer</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetTotalVolume(ByVal actuations As Integer) As Single
    End Interface
End NameSpace