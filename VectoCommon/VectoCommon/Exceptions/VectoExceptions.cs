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
using System.Runtime.Serialization;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Exceptions
{
	/// <summary>
	/// Base Exception for all Exception in VECTO.
	/// </summary>
	[Serializable]
	public class VectoException : Exception
	{
		protected VectoException(SerializationInfo info, StreamingContext context) : base(info, context) {}

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
	}

	public class VectoXMLException : VectoException
	{
		protected VectoXMLException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public VectoXMLException(string message) : base(message) { }
		public VectoXMLException(string message, Exception innerException) : base(message, innerException) { }
		public VectoXMLException(string message, params object[] args) : base(message, args) { }
		public VectoXMLException(string message, Exception inner, params object[] args) : base(message, inner, args) { }
	}

	/// <summary>
	/// Exception when an Input/Output related error occured.
	/// </summary>
	[Serializable]
	public abstract class FileIOException : VectoException
	{
		protected FileIOException(string message) : base(message) {}
		protected FileIOException(string message, Exception inner) : base(message, inner) {}
	}

	/// <summary>
	/// Exception when the file format is invalid or a file was not found.
	/// </summary>
	[Serializable]
	public class InvalidFileFormatException : FileIOException
	{
		public InvalidFileFormatException(string message) : base(message) {}
		public InvalidFileFormatException(string message, params object[] args) : base(string.Format(message, args)) {}
	}

	/// <summary>
	/// Exception which gets thrown when the version of a file is not supported.
	/// </summary>
	[Serializable]
	public class UnsupportedFileVersionException : FileIOException
	{
		public UnsupportedFileVersionException(string message) : base(message) {}
		public UnsupportedFileVersionException(string message, Exception inner) : base(message, inner) {}
	}

	/// <summary>
	/// Exception which gets thrown when an error occurred during read of a vecto csv-file.
	/// </summary>
	[Serializable]
	public class CSVReadException : FileIOException
	{
		public CSVReadException(string message) : base(message) {}
		public CSVReadException(string message, Exception inner) : base(message, inner) {}
	}
}