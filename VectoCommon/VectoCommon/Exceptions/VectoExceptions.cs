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
using System.Linq;
using System.Runtime.Serialization;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Exceptions
{
	[Serializable]
	public class VectoException : Exception
	{
		public VectoException(string message, ErrorCode errorCode, string fix = null) : base(message)
        {
			Data[ExceptionData.ErrorCode] = errorCode;
            Data[ExceptionData.Fix] = fix ?? NO_FIX_AVAILABLE;

            LogManager.Flush();
        }

        public VectoException(string message) : base(message)
        {
            LogManager.Flush();
        }

        public VectoException(string message, Exception innerException) : base(message, innerException)
        {
            LogManager.Flush();
        }

        public VectoException(string message, params object[] args)
            : base(string.Format(message, args))
        {
            LogManager.Flush();
        }

        public VectoException(string message, Exception inner, params object[] args)
            : base(string.Format(message, args), inner)
        {
            LogManager.Flush();
        }

        public const string NO_FIX_AVAILABLE = "Cannot suggest a fix. You may want to report this issue.";
    }

    public enum ExceptionData
    {
        HasFullRunInfo,
        ErrorCode,
        RunIdentifier,
        RunName,
        CycleName,
        RunSuffix,
        AbsTime,
        Distance,
        Speed,
        F_equiv,
        GearInfo,
        Dt,
        Progress,
        Fix
    }

    public static class ExceptionExtensions
    {
        public static ErrorCode GetErrorCode(this Exception ex)
        {
            return (ex is VectoException vectoException) ? (ErrorCode)vectoException.Data[ExceptionData.ErrorCode] : ErrorCodes.EXCEPTION_CANNOT_INFER_ERROR_TYPE;
        }

        public static string GetFix(this Exception ex)
        {
            return (ex is VectoException vectoException) ? (string)vectoException.Data[ExceptionData.Fix] : VectoException.NO_FIX_AVAILABLE;
        }

        public static bool HasFullRunInfo(this Exception ex)
        {
            return (ex != null) && ex.Data.Contains(ExceptionData.HasFullRunInfo) && (bool)ex.Data[ExceptionData.HasFullRunInfo];
        }

        public static string GetHeader(this Exception ex)
        {
            var cycleIdComponents = (ex != null)
                ? new List<Object>() { ex.Data[ExceptionData.RunName], ex.Data[ExceptionData.CycleName], ex.Data[ExceptionData.RunSuffix] }
                : new List<Object>();

            return (cycleIdComponents.Count(c => c != null) > 0) ? $"({cycleIdComponents.Where(c => c != null).Join(" ")})" : string.Empty;
        } 
                
        public static string GetMessageWithHeaderAndCode(this Exception ex)
        {
            return (ex != null) 
                ? $"{ex.GetHeader()} [{ex.GetErrorCode()}]: {ex.Message}" 
                : (ex?.Message ?? string.Empty);
        }

        public static string GetMessageWithCycleRunInfo(this Exception ex)
        {
            return ex.HasFullRunInfo()
                ? $"{ex.Data[ExceptionData.RunIdentifier]} {ex.GetMessageWithHeaderAndCode()} | " +
                  $"Progress: {ex.Data[ExceptionData.Progress]:P1} - absTime: {ex.Data[ExceptionData.AbsTime]}, " +
                  $"distance: {ex.Data[ExceptionData.Distance]}, dt: {ex.Data[ExceptionData.Dt]}, v: {ex.Data[ExceptionData.Speed]}, " +
                  $"{(ex.Data[ExceptionData.GearInfo] as Dictionary<AxleNumber, GearshiftPosition>)?.Select(kvp => $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}").Join()} " +
                  $", f_equiv:{ex.Data[ExceptionData.F_equiv] ?? "-"}"
                : (ex?.Message ?? string.Empty);
        }
    }
}