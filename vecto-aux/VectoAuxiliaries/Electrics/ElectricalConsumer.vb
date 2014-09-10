Namespace Electrics
    ''' <summary>
    ''' Described a consumer of Alternator electrical power
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ElectricalConsumer
        Implements IElectricalConsumer
        Private ReadOnly _name As String
        Private ReadOnly _power As Single

        ''' <summary>
        ''' Name of the Consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Name() As String Implements IElectricalConsumer.Name
            Get
                Return _name
            End Get
        End Property

        ''' <summary>
        ''' Power Consumprion of Consumer (Watts)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Power() As Single Implements IElectricalConsumer.Power
            Get
                Return _power
            End Get
        End Property

        ''' <summary>
        ''' Creates a new nstance of the electrical consumer class
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="power"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal name As String, ByVal power As Single)
            If name = String.Empty Then Throw New ArgumentException("Name cannot be empty string")
            If Math.Abs(power - 0.0) < 0.001 Then Throw New ArgumentOutOfRangeException("power", power, "Supplied power must be greater than zero")
            _name = name
            _power = power
        End Sub
    End Class
End Namespace