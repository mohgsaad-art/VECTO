using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Integration.GenericVehicles
{
	[Parallelizable(ParallelScope.All), TestFixture]
	public static class GenericVehiclesInputFilesAdapter
	{
		private static string BasePath;

		enum VectoFileType
		{
			Vecto,
			XML
		}

		enum VectoTestVehicleType
		{
			HEV,
			PEV,
			ICE,
			Other
		}

		public static void CopyDeclarationInputFiles(string basePath)
		{
			CopyInputFiles(basePath, "Declaration Mode");
		}

		public static void CopyEngineeringInputFiles(string basePath)
		{
			CopyInputFiles(basePath, "Engineering Mode");
		}

		private static void CopyInputFiles(string basePath, string executionMode)
		{
			if(string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath))
			{
				throw new ArgumentNullException($"{nameof(basePath)} cannot be null.");
			}

			BasePath = basePath;

			var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string genericVehiclesDirectory = @$"{executingDirectory}\..\..\..\..\..\Generic Vehicles\{executionMode}";

			var vehicleFiles = GetVehicleFiles(genericVehiclesDirectory);

			var vectoOriginDestinationPaths = GetInputFilesDestinationPaths(vehicleFiles[VectoFileType.Vecto], GetVectoFileDestinationPath);
			var xmlOriginDestinationPaths = GetInputFilesDestinationPaths(vehicleFiles[VectoFileType.XML], GetXmlFileDestinationPath);

			var originDestinationPaths = vectoOriginDestinationPaths
				.ToList()
				.Concat(xmlOriginDestinationPaths);

			foreach (var originDestination in originDestinationPaths)
			{
				CopyDirectory(originDestination.Key, originDestination.Value);
			}
		}

		private static void CopyDirectory(string origin, string destination)
		{
			var originDirectory = Path.GetDirectoryName(origin);
			var destinationDirectory = Path.GetDirectoryName(destination);

			Directory.CreateDirectory(destinationDirectory);
			var directoryFiles = Directory.GetFiles(originDirectory);
			foreach (var file in directoryFiles)
			{
				string destinationFile = Path.Combine(destinationDirectory, Path.GetFileName(file));
				File.Copy(file, destinationFile, true);
			}
		}

		private static Dictionary<string, string> GetInputFilesDestinationPaths(List<string> inputFiles, Func<string, string> getInputFilesDestinationPathFunc)
		{
			Dictionary<string, string> originDestinationPath = new Dictionary<string, string>();
			foreach (string inputFile in inputFiles)
			{
				string destinationPath = getInputFilesDestinationPathFunc(inputFile);
				originDestinationPath.Add(inputFile, destinationPath);

				var commonFolder = GetCommonFoldersPaths(inputFile, destinationPath);
				if (!string.IsNullOrEmpty(commonFolder.Key) && !originDestinationPath.ContainsKey(commonFolder.Key))
				{
					originDestinationPath.Add(commonFolder.Key, commonFolder.Value);
				}
			}

			return originDestinationPath;
		}

		private static KeyValuePair<string, string> GetCommonFoldersPaths(string vectoFile, string vectoFileDestination)
		{
			if (Path.GetExtension(vectoFileDestination) != ".vecto")
			{
				return new KeyValuePair<string, string>(string.Empty, string.Empty); ;
			}

			var fileParentDirectory = Path.GetDirectoryName(vectoFile);
			var fileParentDestinationDirectory = Path.GetDirectoryName(vectoFileDestination);

			const string commonDirectoryName = "../Common";
			var commonDirectory = Path.Combine(fileParentDirectory, commonDirectoryName);
			var commonFileDestinationDirectory = Path.Combine(fileParentDestinationDirectory, commonDirectoryName);

			if (Directory.Exists(commonDirectory))
			{
				var commonFile = Directory.GetFiles(commonDirectory)[0];
				
				return new KeyValuePair<string, string>(
					commonFile,
					Path.Combine(commonFileDestinationDirectory, Path.GetFileName(commonFile)));
			}
			
			return new KeyValuePair<string, string>(string.Empty, string.Empty);
		}

		private static string GetVectoFileDestinationPath(string vectoFile)
		{
			var vectoFileContent = JObject.Parse(File.ReadAllText(vectoFile));
			var vehicleFile = vectoFileContent["Body"]["VehicleFile"]?.Value<string>();

			JObject vehicleFileContent = null;
			var isPevIEPCVehicle = false;
			if (!string.IsNullOrEmpty(vehicleFile))
			{
				var fileParentDirectory = Path.GetDirectoryName(vectoFile);
				var vehicleFilePath = Path.Join(fileParentDirectory, vehicleFile);
				vehicleFileContent = JObject.Parse(File.ReadAllText(vehicleFilePath));
				var powertrainConfiguration = vehicleFileContent["Body"]["PowertrainConfiguration"]?.Value<string>();
				var numberOfEMs = vehicleFileContent["Body"]["ElectricMotors"];

				isPevIEPCVehicle = !string.IsNullOrEmpty(powertrainConfiguration) ?
					powertrainConfiguration == "IEPC_E" || powertrainConfiguration == "IEPC" || numberOfEMs != null :
					false;
			}

			bool hasICE  = !string.IsNullOrEmpty(vectoFileContent["Body"]["EngineFile"]?.Value<string>());
			bool hasEM   = !string.IsNullOrEmpty(vectoFileContent["Body"]["MotorFile"]?.Value<string>());
			bool hasIEPC = !string.IsNullOrEmpty(vectoFileContent["Body"]["IEPC"]?.Value<string>());
			bool hasHybridParams = ! string.IsNullOrEmpty(vectoFileContent["Body"]["HybridStrategyParams"]?.Value<string>());

			if (!hasIEPC && !string.IsNullOrEmpty(vehicleFile))
			{
				var iepcVehFile = vehicleFileContent["Body"]["IEPC"]?.Value<string>();
				hasIEPC = !iepcVehFile.IsNullOrEmpty();
			}

			bool isEM = hasEM || hasIEPC;
			bool isHEV = (hasICE && isEM) || hasHybridParams;
			bool isICE = hasICE && !isHEV;
			bool isPEV = (isEM && !isHEV) || isPevIEPCVehicle;

			VectoTestVehicleType vehicleType = VectoTestVehicleType.Other;
			if (isHEV)
			{
				vehicleType = VectoTestVehicleType.HEV;
			}
			else if (isICE)
			{
				vehicleType = VectoTestVehicleType.ICE;

			}
			else if (isPEV)
			{
				vehicleType = VectoTestVehicleType.PEV;
			}

			return GetVehicleDestinationPath(vectoFile, vehicleType);
		}

		private static string GetXmlFileDestinationPath(string xmlFile)
		{
			XmlDocument xDocument = new XmlDocument();
			xDocument.Load(xmlFile);

			string xmlVehicleType = xDocument.DocumentElement.FirstChild.Attributes["xsi:type"].InnerXml;

			VectoTestVehicleType vehicleType = VectoTestVehicleType.ICE;
			if (xmlVehicleType.Contains("PEV"))
			{
				vehicleType = VectoTestVehicleType.PEV;
			}
			else if (xmlVehicleType.Contains("HEV"))
			{
				vehicleType = VectoTestVehicleType.HEV;
			}

			return GetVehicleDestinationPath(xmlFile, vehicleType);
		}

		private static string GetVehicleDestinationPath(string filePath, VectoTestVehicleType vehicleType)
		{
			var fileParentDirectory = new DirectoryInfo(Path.GetDirectoryName(filePath)).Name;
			var fileName = Path.GetFileName(filePath);
			switch (vehicleType)
			{
				case VectoTestVehicleType.PEV:
					return Path.Join(BasePath, @$"PEV/{fileParentDirectory}/{fileName}");
				case VectoTestVehicleType.HEV:
					return Path.Join(BasePath, @$"HEV/{fileParentDirectory}/{fileName}");
				case VectoTestVehicleType.ICE:
				default:
					return Path.Join(BasePath, @$"ICE/{fileParentDirectory}/{fileName}");
			}
		}

		private static Dictionary<VectoFileType, List<string>> GetVehicleFiles(string declarationModeDirectory)
		{
			Dictionary<VectoFileType, List<string>> vehicleFiles = new Dictionary<VectoFileType, List<string>>
			{
				{ VectoFileType.Vecto, new List<string>() },
				{ VectoFileType.XML, new List<string>() }
			};

			foreach (var directory in Directory.GetDirectories(declarationModeDirectory))
			{
				var files = GetFiles(directory);

				List<string> vectoFiles = files
					.Where(f => Path.GetExtension(f) == ".vecto")
					.ToList();
				List<string> xmlFiles = files
					.Where(f => Path.GetExtension(f) == ".xml" && !IsInvalidFile(f))
					.ToList();

				vehicleFiles[VectoFileType.Vecto].AddRange(vectoFiles);

				if (xmlFiles.Count > 0)
				{
					vehicleFiles[VectoFileType.XML].Add(GetMainXmlFile(xmlFiles));
				}
			}

			return vehicleFiles;
		}

		private static List<string> GetFiles(string directory)
		{
			var files = Directory.GetFiles(directory).ToList();
			var directories = Directory.GetDirectories(directory);
			if (files.Count == 0 && directories.Length != 0)
			{
				foreach (var dir in directories)
				{
					files.AddRange(GetFiles(dir));
				}
			}

			return files;
		}

		private static string GetMainXmlFile(List<string> busVehiclePaths)
		{
			if (busVehiclePaths.Count == 0)
			{
				return string.Empty;
			}

			string[] primaryBusXMLs = busVehiclePaths.Where(f => f.ToUpper().Contains("PRIMARY")).ToArray();

			return primaryBusXMLs.Length != 0 ? primaryBusXMLs[0] : busVehiclePaths[0];
		}

		private static bool IsInvalidFile(string filePath)
		{
			string[] invalidFileConditions = { "VIF", "CUSTOMER", "MANUFACTURER", "REPORT", "COMPLETED", "MONITORING" };

			return invalidFileConditions.Any(condition => filePath.ToUpper().Contains(condition));
		}
	}
}