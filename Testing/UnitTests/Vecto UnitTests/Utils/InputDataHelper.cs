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

using System.Text.RegularExpressions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class InputDataHelper
	{
		public static MemoryStream InputDataAsStream(string header, string[] entries)
		{
			var cycleData = new MemoryStream();
			var writer = new StreamWriter(cycleData);
			writer.WriteLine(header);
			foreach (var entry in entries) {
				writer.WriteLine(entry);
			}
			writer.Flush();
			cycleData.Seek(0, SeekOrigin.Begin);
			return cycleData;
		}

		public static TableData InputDataAsTableData(string header, params string[] entries)
		{
			return VectoCSVFile.ReadStream(InputDataAsStream(header, entries));
		}
		public static string GetRandomFilename(string jobFile)
		{
			var path = Path.GetDirectoryName(Path.GetFullPath(jobFile));
			var filename = Path.GetFileNameWithoutExtension(jobFile);
			var extension = Path.GetExtension(jobFile);
			var random = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[^A-Za-z0-9_.]+", "").Substring(0, 5);
			return Path.Combine(path, $"{filename}-{random}.{extension}");
		}

		public static string CreateUniqueSubfolder(String inputFile)
        { 
			string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Guid.NewGuid().ToString(), Path.GetFileName(inputFile));

			if (!Directory.Exists(Path.GetDirectoryName(outputFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            }

			return outputFile;
		}
	}
}