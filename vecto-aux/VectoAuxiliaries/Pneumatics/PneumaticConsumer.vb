Namespace Pneumatics
    Public Class PneumaticConsumer
        Implements IPneumaticConsumer

        Private ReadOnly _name As String
        Private ReadOnly _volumePerCycle As Single

        ''' <summary>
        ''' Name of the consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Name() As String Implements IPneumaticConsumer.Name
            Get
                Return _name
            End Get
        End Property

        ''' <summary>
        ''' Volume of Air consumed per cycle of the consumer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property VolumePerCycle() As Single Implements IPneumaticConsumer.VolumePerCycle
            Get
                Return _volumePerCycle
            End Get
        End Property

        ''' <summary>
        ''' Get the total volume of air required for a number of cycles
        ''' </summary>
        ''' <param name="cycles">Number of cycles of consumer</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTotalVolume(ByVal cycles As Integer) As Single Implements IPneumaticConsumer.GetTotalVolume
            Return VolumePerCycle * cycles
        End Function

        ''' <summary>
        ''' Creates an instance of the PneumaticConsumer class
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="volumePerCycle"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal name As String, ByVal volumePerCycle As Single)
            If name = String.Empty Then
                Throw New ArgumentException("Name cannot be empty string")
            End If

            If Math.Abs(volumePerCycle - 0.0) < 0.001 Then
                Throw New ArgumentOutOfRangeException("volumePerCycle",
                                                      volumePerCycle,
                                                      "Supplied volume should be grater than zero")
            End If

            _name = name
            _volumePerCycle = volumePerCycle
        End Sub
    End Class
End Namespace