Namespace Pneumatics
    Public Interface IM4_AirCompressor
        ''' <summary>
        ''' Ratio of Gear or Pulley used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearRatio() As Single

        ''' <summary>
        ''' Efficiency of the Pulley or Gear used to drive the compressor
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property PulleyGearEfficiency() As Single

        ''' <summary>
        ''' Initialises the AirCompressor Class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialise() As Boolean

        ''' <summary>
        ''' Returns the flow rate [litres/second] of compressor for the given engine rpm
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetFlowRate(ByVal engineRpm As Integer) As Single

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is off
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPowerCompressorOff(ByVal engineRpm As Integer) As Single

        ''' <summary>
        ''' Returns the power consumed for the given engine rpm when compressor is on
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPowerCompressorOn(ByVal engineRpm As Integer) As Single

        ''' <summary>
        ''' Returns the difference in power between compressonr on and compressor off operation at the given engine rpm
        ''' </summary>
        ''' <param name="engineRpm">Engine speed in rpm</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetPowerDifference(ByVal engineRpm As Integer) As Single

        ''' <summary>
        ''' Returns Average PoweDemand PeCompressor UnitFlowRate 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Single

    End Interface
End NameSpace