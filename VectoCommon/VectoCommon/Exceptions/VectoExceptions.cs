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
	/// <summary>
	/// Base Exception for all Exception in VECTO.
	/// </summary>
	[Serializable]
	public class VectoException : Exception
	{
		public VectoException(string message, ErrorCode errorCode) : base(message)
        {
			Data[RunInfo.ErrorCode] = errorCode;

            LogManager.Flush();
        }

        public VectoException(string message, ErrorCode errorCode, Exception innerException) : base(message, innerException)
        {
            Data[RunInfo.ErrorCode] = errorCode;

            LogManager.Flush();
        }

		public ErrorCode ErrorCode => (ErrorCode)Data[RunInfo.ErrorCode];
    }

    public enum RunInfo
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
        Progress
    }

    public static class ExceptionExtensions
    {
        public static ErrorCode GetErrorCode(this Exception ex)
        {
            return (ex is VectoException vectoException) ? vectoException.ErrorCode : ErrorCodes.EXCEPTION_CANNOT_INFER_ERROR_TYPE;
        }

        public static string GetMessageWithCycleRunInfo(this Exception ex)
        {
            if (!ex.Data.Contains(RunInfo.HasFullRunInfo) || !(bool)ex.Data[RunInfo.HasFullRunInfo])
            {
                return ex.Message;
            }

            return $"{ex.Data[RunInfo.RunIdentifier]} ({ex.Data[RunInfo.RunName]} {ex.Data[RunInfo.CycleName]} {ex.Data[RunInfo.RunSuffix]}) " +
                $"Progress: {ex.Data[RunInfo.Progress]:P1} - absTime: {ex.Data[RunInfo.AbsTime]:F1}, " +
                $"distance: {ex.Data[RunInfo.Distance]:F1}, dt: {ex.Data[RunInfo.Dt]:F1}, v: {ex.Data[RunInfo.Speed]}, " +
                $"{(ex.Data[RunInfo.GearInfo] as Dictionary<AxleNumber, GearshiftPosition>)?.Select(kvp => $"Gear{kvp.Key.FormatAxleNumber()}: {kvp.Value}").Join()} " +
                $", f_equiv:{ex.Data[RunInfo.F_equiv] ?? "-"} | [{ex.GetErrorCode()}]: {ex.Message}";
        }
    }
}