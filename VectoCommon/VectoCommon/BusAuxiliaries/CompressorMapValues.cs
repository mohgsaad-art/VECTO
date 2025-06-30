using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	/// <summary>
	/// 		''' Encapsulates compressor map values
	/// 		''' Flow Rate
	/// 		''' Power - Compressor On
	/// 		''' Power - Compressor Off
	/// 		''' </summary>
	/// 		''' <remarks></remarks>
	/// 		'''
	public struct CompressorMapValues
	{
		public PerSecond CompressorSpeed { get; }

		/// <summary>
		/// 			''' Compressor flowrate
		/// 			''' </summary>
		/// 			''' <remarks></remarks>
		public NormLiterPerSecond FlowRate { get; }

		/// <summary>
		/// 			''' Power, compressor on
		/// 			''' </summary>
		/// 			''' <remarks></remarks>
		public Watt PowerCompressorOn { get; }

		/// <summary>
		/// 			''' Power compressor off
		/// 			''' </summary>
		/// 			''' <remarks></remarks>
		public Watt PowerCompressorOff { get; }

		/// <summary>
		/// 			''' Creates a new instance of CompressorMapValues
		/// 			''' </summary>
		/// 			'''
		/// <param name="compressorSpeed"></param>
		/// <param name="flowRate">flow rate</param>
		/// 			''' <param name="powerCompressorOn">power - compressor on</param>
		/// 			''' <param name="powerCompressorOff">power - compressor off</param>
		/// 			''' <remarks></remarks>
		public CompressorMapValues(
			PerSecond compressorSpeed, NormLiterPerSecond flowRate, Watt powerCompressorOn, Watt powerCompressorOff)
		{
			CompressorSpeed = compressorSpeed;
			FlowRate = flowRate;
			PowerCompressorOn = powerCompressorOn;
			PowerCompressorOff = powerCompressorOff;
		}
	}
}