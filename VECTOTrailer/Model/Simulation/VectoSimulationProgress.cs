using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Model.Simulation
{
	public class VectoSimulationProgress
	{
		[Flags]
		public enum MsgTarget
		{
			None = 0,
			MessageWindow = 1,
			NLog = 2,
		}


		public enum MsgType
		{
			StatusMessage,
			InfoMessage,
			Progress,
			LogError,
			LogWarning,
		}
		public VectoSimulationProgress()
		{
			Target = MsgTarget.None;
		}

		public MsgTarget Target { get; set; }
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

		public static LogLevel ToLogLevel(this VectoSimulationProgress.MsgType mt)
		{
			switch (mt) {
				case VectoSimulationProgress.MsgType.StatusMessage:
					return LogLevel.Trace;
					break;
				case VectoSimulationProgress.MsgType.InfoMessage:
					return LogLevel.Info;
					break;
				case VectoSimulationProgress.MsgType.Progress:
					return LogLevel.Trace;
					break;
				case VectoSimulationProgress.MsgType.LogError:
					return LogLevel.Error;
					break;
				case VectoSimulationProgress.MsgType.LogWarning:
					return LogLevel.Warn;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(mt), mt, null);
			}
		}
	}
}
