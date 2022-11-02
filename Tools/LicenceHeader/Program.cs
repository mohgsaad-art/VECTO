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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LicenceHeader
{
	/// <summary>
	/// Updates/Adds the EUPL license header to every .cs file in the solution directory
	/// </summary>
	internal class Program
	{
		private const string SolutionRootDirectory = @"..\..\..\..";

		private static void Main()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Error.WriteLine("Updating Licence-Headers in Sourcefiles.");
			Console.ResetColor();
			Console.Error.WriteLine();
			Console.Error.WriteLine("Search Directory: {0}", Path.GetFullPath(SolutionRootDirectory));
			Console.Error.WriteLine("Filter: *.cs");
			Console.Error.WriteLine("Excluded Dirs: \\obj, \\bin");
			Console.Error.WriteLine("Header-File: {0}", Path.GetFullPath("header.txt"));

			var licence = File.ReadAllText("header.txt", Encoding.UTF8).Replace("[currentyear]", DateTime.Today.Year.ToString());
			var re = new Regex("^.*?(?=using|namespace)", RegexOptions.Singleline);

			var updatedFiles = Directory.EnumerateFiles(SolutionRootDirectory, "*.cs", SearchOption.AllDirectories)
				.AsParallel()
				.Where(f => !(f.Contains("\\obj\\") || f.Contains("\\bin\\")))
				.Select(f => {
					var content = File.ReadAllText(f, Encoding.UTF8);
					return new { name = f, content, replacedContent = re.Replace(content, licence) };
				})
				.Where(f => f.content != f.replacedContent)
				.Select(f => {
					File.WriteAllText(f.name, f.replacedContent, Encoding.UTF8);
					return f.name;
				});

			var count = 0;
			foreach (var f in updatedFiles) {
				count++;
				Console.Error.WriteLine(f.Substring(SolutionRootDirectory.Length - 1));
			}

			Console.Error.WriteLine();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Finished. Updated {0} files.", count);
			Console.ResetColor();

			if (!Console.IsInputRedirected) {
				Console.ReadKey();
			}
		}
	}
}
