Namespace Pneumatics
    Public Interface ICompressorMap

        ''' <summary>
        ''' Initilaises the map from the supplied csv data
        ''' </summary>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns compressor flow rate at the given rotation speed
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Function GetFlowRate(ByVal rpm As Integer) As Single

        ''' <summary>
        ''' Returns mechanical power at rpm when compressor is on
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Function GetPowerCompressorOn(ByVal rpm As Integer) As Single

        ''' <summary>
        ''' Returns mechanical power at rpm when compressor is off
        ''' </summary>
        ''' <param name="rpm">compressor rotation speed</param>
        ''' <returns></returns>
        ''' <remarks>Single</remarks>
        Function GetPowerCompressorOff(ByVal rpm As Integer) As Single

    End Interface
End Namespace