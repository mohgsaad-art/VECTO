Namespace Electrics
    Public Interface IElectricalConsumer
        ''' <summary>
        ''' Name of the Consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Name() As String

        ''' <summary>
        ''' Power Consumprion of Consumer (Watts)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Power() As Single
    End Interface
End Namespace