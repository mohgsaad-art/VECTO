

Namespace Pneumatics


    Public Class AirCompressor
        Implements IAirCompressor

        Private Const MinRatio As Single = 1.25
        Private Const MaxRatio As Single = 5.5
        Private Const MinEff As Single = 0.25
        Private Const MaxEff As Single = 0.95

        Private _pulleyGearRatio As Single
        Private _pulleyGearEfficiency As Single
        Private _map As ICompressorMap

        ''' <summary>
        ''' Ratio of Gear or Pulley used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PulleyGearRatio() As Single Implements IAirCompressor.PulleyGearRatio
            Get
                Return _pulleyGearRatio
            End Get
            Set(value As Single)
                If (value < MinRatio OrElse value > MaxRatio) Then
                    Throw New ArgumentOutOfRangeException(String.Format("Invalid value, should be in the range {0} to {1}", MinRatio, MaxRatio), value)
                Else
                    _pulleyGearRatio = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Efficiency of the Pulley or Gear used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PulleyGearEfficiency() As Single Implements IAirCompressor.PulleyGearEfficiency
            Get
                Return _pulleyGearEfficiency
            End Get
            Set(value As Single)
                If (value < MinEff OrElse value > MaxEff) Then
                    Throw New ArgumentOutOfRangeException(String.Format("Invalid value, should be in the range {0} to {1}", MinEff, MaxEff), value)
                Else
                    _pulleyGearEfficiency = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Creates a new instance of the AirCompressor Class
        ''' </summary>
        ''' <param name="map">map of compressor values against compressor rpm</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal map As ICompressorMap)
            _map = map
        End Sub

        ''' <summary>
        ''' Creates a new instance of the AirCompressor Class
        ''' </summary>
        ''' <param name="map">map of compressor values against compressor rpm</param>
        ''' <param name="pulleyGearRatio">Ratio of Pulley/Gear</param>
        ''' <param name="pulleyGearEfficiency">Efficiency of Pulley/Gear</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal map As ICompressorMap, ByVal pulleyGearRatio As Single, ByVal pulleyGearEfficiency As Single)
            _map = map
            _pulleyGearRatio = pulleyGearRatio
            _pulleyGearEfficiency = pulleyGearEfficiency
        End Sub

        ''' <summary>
        ''' Initialises the AirCompressor Class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean Implements IAirCompressor.Initialise
            Return _map.Initialise()
        End Function

        'Queryable Compressor Methods
        '
        'Compressor ( Speed ) Flow Rate 
        'Power @ Crank From Pnumatics compressor off ( A )
        'Power @ Crank From Pnumatics compressor On  ( B )
        'Power   Delta ( A ) vs ( B )

        ''' <summary>
        ''' Returns the flow rate [litres/second] of compressor for the given engine rpm
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFlowRate(ByVal engineRpm As Integer) As Single Implements IAirCompressor.GetFlowRate
            Dim compressorRpm As Single = engineRpm * PulleyGearRatio
            Return _map.GetFlowRate(compressorRpm)
        End Function

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is off
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPowerCompressorOff(ByVal engineRpm As Integer) As Single Implements IAirCompressor.GetPowerCompressorOff
            Return GetCompressorPower(engineRpm, False)
        End Function

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is on
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPowerCompressorOn(ByVal engineRpm As Integer) As Single Implements IAirCompressor.GetPowerCompressorOn
            Return GetCompressorPower(engineRpm, True)
        End Function

        ''' <summary>
        ''' Returns the difference in power between compressonr on and compressor off operation at the given engine rpm
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPowerDifference(ByVal engineRpm As Integer) As Single Implements IAirCompressor.GetPowerDifference
            Dim powerOn As Single = GetPowerCompressorOn(engineRpm)
            Dim powerOff As Single = GetPowerCompressorOff(engineRpm)
            Return powerOn - powerOff
        End Function

        ''' <summary>
        ''' Looks up the compressor power from the map at given engine speed
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <param name="compressorOn">Is compressor on</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCompressorPower(ByVal engineRpm As Integer, ByVal compressorOn As Boolean) As Single
            Dim compressorRpm As Single = engineRpm * PulleyGearRatio
            If compressorOn Then
                Return _map.GetPowerCompressorOn(compressorRpm)
            Else
                Return _map.GetPowerCompressorOff(compressorRpm)
            End If
        End Function

    End Class

End Namespace