/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using NLog;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public static class LogManager
	{
		public static LoggingObject GetLogger(string fullName)
		{
			return new LoggingObject(fullName);
		}

		public static void Flush()
		{
			NLog.LogManager.Flush();
		}

		public static void DisableLogging()
		{
			LoggingObject.LogEnabled = false;
		}

		public static void EnableLogging()
		{
			LoggingObject.LogEnabled = true;
		}
	}

	/// <summary>
	/// Base Object for Logging
	/// </summary>
	public class LoggingObject
	{
		// ReSharper disable once InconsistentNaming
		private static readonly ThreadLocal<bool> _logEnabled = new ThreadLocal<bool>(() => true);

		public static bool LogEnabled
		{
			get => _logEnabled.Value;
			set => _logEnabled.Value = value;
		}

		public class LogEventArgs : EventArgs
		{
			public Exception Exception { get; private set; }

			public string Level { get; private set; }

			public string Message { get; private set; }

            public LogEventArgs(Exception e, string level, string message) 
			{
				Exception = e;
				Level = level;
				Message = message;
			}

			private LogEventArgs() { }
		}

        public static event EventHandler<LogEventArgs> OnLogEvent;

        private readonly Logger _log;

		public LoggingObject()
		{
			_log = NLog.LogManager.GetLogger(GetType().FullName);
		}

		public LoggingObject(string fullName)
		{
			_log = NLog.LogManager.GetLogger(fullName);
		}

		public static LoggingObject Logger<T>()
		{
			return new LoggingObject(typeof(T).ToString());
		}

		private static string FormatMessage(string message, params object[] args)
        {
            string formatted;
			var msg = message ?? string.Empty;

            try
            {
                formatted = (args == null || args.Length == 0) ? msg : string.Format(msg, args);
            }
            catch (FormatException)
            {
                var argText = string.Join(", ", args.Select(a => a?.ToString() ?? "<null>"));
                formatted = msg + " | args: " + argText;
            }

			return formatted;
        }

        /// <summary>
        /// Fatal > Error > Warn > Info > Debug > Trace
        /// </summary>
        public void Debug(string message, params object[] args)
		{
			if (LogEnabled) {
				_log.Debug(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(null, "Debug", FormatMessage(message, args)));
            }
        }



		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Debug(Exception e, string message = null, params object[] args)
		{
			if (LogEnabled) {
				_log.Debug(e, message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(e, "Debug", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Warn(string message, params object[] args)
		{
			if (LogEnabled) {
                _log.Warn(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(null, "Warn", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Warn(Exception e, string message = null, params object[] args)
		{
			if (LogEnabled) {
                _log.Warn(e, message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(e, "Warn", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Error(string message, params object[] args)
		{
			if (LogEnabled) {
                _log.Error(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(null, "Error", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Error(Exception e, string message = null, params object[] args)
		{
			if (LogEnabled) {
                _log.Error(e, message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(e, "Error", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Fatal(string message, params object[] args)
		{
			if (LogEnabled) {
                _log.Fatal(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(null, "Fatal", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Fatal(Exception e, string message = null, params object[] args)
		{
			if (LogEnabled) {
				_log.Fatal(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(e, "Fatal", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Info(string message, params object[] args)
		{
			if (LogEnabled) {
                _log.Info(message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(null, "Info", FormatMessage(message, args)));
            }
		}

		/// <summary>
		/// Fatal > Error > Warn > Info > Debug > Trace
		/// </summary>
		public void Info(Exception e, string message = null, params object[] args)
		{
			if (LogEnabled) {
                _log.Info(e, message, args);
                OnLogEvent?.Invoke(typeof(LoggingObject), new LogEventArgs(e, "Info", FormatMessage(message, args)));
            }
		}

		protected LoggingObject Log => this;
	}
}