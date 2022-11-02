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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;


namespace HashingCmd
{
	class Program
	{
		public delegate void HashingAction(string filename, VectoHash h);

		private const string Usage = @"
hashingcmd.exe (-h | [-v] [[-s] -x] [-c] [-r]) <file.xml> <file2.xml> <file3.xml>

";

		private const string Help = @"
hashingcmd.exe

-h:    print help
-v:    verify hashed file
-s:    create hashed file
-x:    validate XML file against VECTO XML schema
-c:    compute hash and write to stdout
-r:    read hash from file and write to stdout
";

		private static readonly Dictionary<string, HashingAction> Actions = new Dictionary<string, HashingAction> {
			{ "-v", VerifyHashAction },
			{ "-c", ComputeHashAction },
			{ "-r", ReadHashAction },
			{ "-s", CreateHashedFileAction }
		};

		static bool _validateXML;
		

		static int Main(string[] args)
		{
			try {
				if (args.Contains("-h")) {
					ShowVersionInformation();
					Console.Write(Help);
					return 0;
				}

				if (args.Contains("-x")) {
					_validateXML = true;
				}

				var fileList = args.Except(Actions.Keys.Concat(new[] { "-x" })).ToArray();
				if (fileList.Length == 0 || !args.Intersect(Actions.Keys.ToArray()).Any()) {
					ShowVersionInformation();
					Console.Write(Usage);
					return 0;
				}
				foreach (var file in fileList) {
					Console.Error.WriteLine("processing " + Path.GetFileName(file));
					if (!File.Exists(Path.GetFullPath(file))) {
						Console.Error.WriteLine("file " + Path.GetFullPath(file) + " not found!");
						continue;
					}
					foreach (var arg in args) {
						if (Actions.ContainsKey(arg)) {
							try {
								var h = VectoHash.Load(file);
								Actions[arg](Path.GetFullPath(file), h);
							} catch (Exception e) {
								Console.ForegroundColor = ConsoleColor.Red;
								Console.Error.WriteLine(e.Message);
								if (e.InnerException != null) {
									Console.Error.WriteLine(e.InnerException.Message);
								}
								Console.ResetColor();
							}
						}
					}
				}
			} catch (Exception e) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(e.Message);
				Console.ResetColor();

				//Console.Error.WriteLine("Please see log-file for further details (logs/log.txt)");
				Environment.ExitCode = Environment.ExitCode != 0 ? Environment.ExitCode : 1;
			}
#if DEBUG
			Console.Error.WriteLine("done.");

			if (!Console.IsInputRedirected)
				Console.ReadKey();
#endif
			return Environment.ExitCode;
		}

		private static void CreateHashedFileAction(string filename, VectoHash h)
		{
			var destination = Path.Combine(Path.GetDirectoryName(filename),
				Path.GetFileNameWithoutExtension(filename) + "_hashed.xml");
			if (File.Exists(destination)) {
				Console.Error.WriteLine("hashed file already exists. overwrite? (y/n) ");
				var key = Console.ReadKey(true);
				while (!(key.KeyChar == 'y' || key.KeyChar == 'n')) {
					Console.Error.WriteLine("overwrite? (y/n) ");
					key = Console.ReadKey(true);
				}
				if (key.KeyChar == 'n') {
					return;
				}
				Console.Error.WriteLine("overwriting file " + Path.GetFileName(destination));
			} else {
				Console.Error.WriteLine("creating file " + Path.GetFileName(destination));
			}
			var result = h.AddHash();
			var writer = new XmlTextWriter(destination, Encoding.UTF8) {
				Formatting = Formatting.Indented,
				Indentation = 4
			};
			result.WriteTo(writer);
			writer.Flush();
			writer.Close();

			if (_validateXML) {
				new XMLValidator(XmlReader.Create(destination), null, ValidationCallBack).ValidateXML(XmlDocumentType.DeclarationComponentData);
				WriteLine("Valid", ConsoleColor.Green);
				 
			}
		}

		private static void ValidationCallBack(XmlSeverityType severity, ValidationEvent evt)
		{
			var args = evt.ValidationEventArgs;
			if (severity == XmlSeverityType.Error) {
				throw new Exception($"Validation error: {args.Message}{Environment.NewLine}" +
									$"Line: {args.Exception.LineNumber}", evt.Exception);
			} else {
				Console.Error.WriteLine("Validation warning: {0}{2}Line: {1}", 
					args.Message, args.Exception.LineNumber, Environment.NewLine);
			}
		}

		private static void ValidateFile(string filename)
		{
			new XMLValidator(XmlReader.Create(filename),null, ValidationCallBack).ValidateXML(
				XmlDocumentType.DeclarationJobData | XmlDocumentType.CustomerReport |
				XmlDocumentType.ManufacturerReport | XmlDocumentType.DeclarationComponentData);
		}
		
		private static void ReadHashAction(string filename, VectoHash h)
		{
			WriteLine("reading hashes");
			ValidateFile(filename);
			var components = h.GetContainigComponents().GroupBy(s => s)
				.Select(g => new { Entry = g.Key, Count = g.Count() });

			foreach (var component in components) {
				if (component.Entry == VectoComponents.Vehicle) {
					continue;
				}
				for (var i = 0; i < component.Count; i++) {
					var readHash = h.ReadHash(component.Entry, i);
					WriteLine("  " + component.Entry.XMLElementName() + "\t ... " + readHash + "");
				}
			}
		}

		private static void ComputeHashAction(string filename, VectoHash h)
		{
			WriteLine("computing hashes");
			ValidateFile(filename);
			var components = h.GetContainigComponents();

			if (components.Count > 1) {
				var grouped = components.GroupBy(s => s)
					.Select(g => new { Entry = g.Key, Count = g.Count() });
				foreach (var component in grouped) {
					if (component.Entry == VectoComponents.Vehicle) {
						continue;
					}
					for (var i = 0; i < component.Count; i++) {
						var computedHash = h.ComputeHash(component.Entry, i);
						WriteLine("  " + component.Entry.XMLElementName() + "\t ... " + computedHash + "");
					}
				}
				var jobHash = h.ComputeHash();
				WriteLine("  job file\t ... " + jobHash + "");
			} else {
				var hash = h.ComputeHash();
				WriteLine("  computed hash:  " + hash + "");
			}
		}

		private static void VerifyHashAction(string filename, VectoHash h)
		{
			WriteLine("validating hashes");
			ValidateFile(filename);
			var components = h.GetContainigComponents().GroupBy(s => s)
				.Select(g => new { Entry = g.Key, Count = g.Count() });
			foreach (var component in components) {
				if (component.Entry == VectoComponents.Vehicle) {
					continue;
				}
				for (var i = 0; i < component.Count; i++) {
					var result = h.ValidateHash(component.Entry, i);
					WriteLine("  " + component.Entry.XMLElementName() + "\t ... " + (result ? "valid" : "invalid"),
						result ? ConsoleColor.Green : ConsoleColor.Red);
				}
			}
		}

		private static void WriteLine(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
		{
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void ShowVersionInformation()
		{
			var hashingLib = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VectoHashing.dll"))
				.GetName();
			WriteLine($@"HashingLibrary: {hashingLib.Version}");
		}
	}
}