Namespace Electrics
    Public Interface IAlternatorMap
        ''' <summary>
        ''' Initialise the map from supplied csv data
        ''' </summary>
        ''' <returns>Boolean - true if map is created successfully</returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns the alternator efficiency at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Function GetEfficiency(rpm As Integer) As Single

        ''' <summary>
        ''' Returns the alternator Maximum Regeneration Power at given rpm
        ''' </summary>
        ''' <param name="rpm">alternator rotation speed</param>
        ''' <returns>Single</returns>
        ''' <remarks></remarks>
        Function GetMaximumRegenerationPower(rpm As Integer) As Single
    End Interface
End Namespace