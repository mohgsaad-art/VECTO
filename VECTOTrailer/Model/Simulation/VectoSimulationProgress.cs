using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Model.Simulation
{
	public class VectoSimulationProgress
	{
		public enum MsgType
		{
			StatusMessage,
			InfoMessage,
			Progress,
			LogError,
			LogWarning,

		}

		public string Message { get; set; }

		public MsgType Type { get; set; }

		public string Link { get; set; }
	}

	public static class MsgTypeExtensions
	{
		public static MessageType ToMessageType(this VectoSimulationProgress.MsgType mt)
		{
			switch (mt)
			{
				case VectoSimulationProgress.MsgType.StatusMessage: return MessageType.StatusMessage;
				case VectoSimulationProgress.MsgType.InfoMessage: return MessageType.InfoMessage;
				case VectoSimulationProgress.MsgType.Progress: return MessageType.StatusMessage;
				case VectoSimulationProgress.MsgType.LogError: return MessageType.ErrorMessage;
				case VectoSimulationProgress.MsgType.LogWarning: return MessageType.WarningMessage;
				default: throw new ArgumentOutOfRangeException(nameof(mt), mt, null);
			}
		}
	}
}
