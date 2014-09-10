Namespace Electrics
    Public Interface IAlternator
        ''' <summary>
        ''' Ratio of gear/pulley to engine
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearRatio() As Single

        ''' <summary>
        ''' Efficiency of gear/pulley to engine
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearEfficiency() As Single

        ''' <summary>
        ''' Initialises the efficiency map
        ''' </summary>
        ''' <returns>Boolean - true is initialisation succeeds</returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns the alternator efficiency at a given engine speed
        ''' </summary>
        ''' <param name="engineRpm"></param>
        ''' <returns>Single</returns>
        ''' <remarks>rpm must result in alternator rpm values that fall within the alternator efficiency map</remarks>
        Function GetEfficiency(ByVal engineRpm As Single) As Single

        ''' <summary>
        ''' Gets the maximum Regenration Power of the alternator at the given engine speed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetMaximumRegenerationPower(ByVal engineRpm As Single) As Single

        ''' <summary>
        ''' Retuns the maximum regeneration power at the engine for the given engine speed
        ''' </summary>
        ''' <param name="engineRpm"></param>
        ''' <returns>Single</returns>
        ''' <remarks>rpm must result in alternator rpm values that fall within the alternator efficiency map</remarks>
        Function GetMaximumRegeneratinPowerAtCrank(ByVal engineRpm As Single) As Single
    End Interface
End Namespace