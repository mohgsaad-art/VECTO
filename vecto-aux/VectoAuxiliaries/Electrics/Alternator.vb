
Namespace Electrics

    Public Class Alternator
        Implements IAlternator

        Private Const MinRatio As Single = 1.25
        Private Const MaxRatio As Single = 5.5
        Private Const MinEff As Single = 0.25
        Private Const MaxEff As Single = 0.95

        Private _pulleyGearRatio As Single
        Private _pulleyGearEfficiency As Single
        Private _map As IAlternatorMap

        ''' <summary>
        ''' Ratio of gear/pulley to engine
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PulleyGearRatio() As Single Implements IAlternator.PulleyGearRatio
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
        ''' Efficiency of gear/pulley to engine
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PulleyGearEfficiency() As Single Implements IAlternator.PulleyGearEfficiency
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
        ''' Creates a new instance of the Alternator class
        ''' </summary>
        ''' <param name="map">instance of and object implementing IAlternatorMap</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef map As IAlternatorMap)
            _map = map
        End Sub

        ''' <summary>
        ''' Creates a new instance of the Alternator class
        ''' </summary>
        ''' <param name="ratio">pulley / gear ratio to engine</param>
        ''' <param name="efficiency">pulley / gear efficiency to engine</param>
        ''' <param name="map">Instance of an object that implements IAlternatorMap</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef map As IAlternatorMap, ByVal ratio As Single, ByVal efficiency As Single)
            PulleyGearRatio = ratio
            PulleyGearEfficiency = efficiency
            _map = map
        End Sub

        ''' <summary>
        ''' Initialises the efficiency map
        ''' </summary>
        ''' <returns>Boolean - true is initialisation succeeds</returns>
        ''' <remarks></remarks>
        Public Function Initialise() As Boolean Implements IAlternator.Initialise
            Return _map.Initialise()
        End Function

        ''' <summary>
        ''' Returns the alternator efficiency at a given engine speed
        ''' </summary>
        ''' <param name="engineRpm"></param>
        ''' <returns>Single</returns>
        ''' <remarks>rpm must result in alternator rpm values that fall within the alternator efficiency map</remarks>
        Public Function GetEfficiency(ByVal engineRpm As Single) As Single Implements IAlternator.GetEfficiency
            Dim alternatorspeed As Single = engineRpm * PulleyGearRatio
            Dim value As Single = _map.GetEfficiency(alternatorspeed)
            Return value
        End Function

        ''' <summary>
        ''' Gets the maximum Regenration Power of the alternator at the given engine speed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaximumRegenerationPower(ByVal engineRpm As Single) As Single Implements IAlternator.GetMaximumRegenerationPower
            Dim alternatorspeed As Single = engineRpm * PulleyGearRatio
            Dim value As Single = _map.GetMaximumRegenerationPower(alternatorspeed)
            Return value
        End Function

        ''' <summary>
        ''' Retuns the maximum regeneration power at the engine for the given engine speed
        ''' </summary>
        ''' <param name="engineRpm"></param>
        ''' <returns>Single</returns>
        ''' <remarks>rpm must result in alternator rpm values that fall within the alternator efficiency map</remarks>
        Public Function GetMaximumRegeneratinPowerAtCrank(ByVal engineRpm As Single) As Single Implements IAlternator.GetMaximumRegeneratinPowerAtCrank
            Dim maxRegenPower As Single = GetMaximumRegenerationPower(engineRpm)    '100 from mock
            Dim efficiency As Single = GetEfficiency(engineRpm)                     '0.5 from mock
            Dim value As Single = maxRegenPower / (efficiency / PulleyGearEfficiency)
            Return value
        End Function

    End Class
End Namespace