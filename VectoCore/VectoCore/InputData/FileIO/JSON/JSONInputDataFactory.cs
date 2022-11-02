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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	// ReSharper disable once InconsistentNaming
	public static class JSONInputDataFactory
	{
		internal static void WriteFile(JToken content, string path)
		{
			if (!content.Any()) {
				return;
			}

			try {
				var str = JsonConvert.SerializeObject(content, Formatting.Indented);
				File.WriteAllText(path, str, Encoding.UTF8);
			} catch (Exception) {
				return;
			}
		}

		internal static JObject ReadFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new FileNotFoundException("failed to load file: " + fileName, fileName);
			}

			using (var reader = File.OpenText(fileName)) {
				return (JObject)JToken.ReadFrom(new JsonTextReader(reader));
			}
		}

		public static IInputDataProvider ReadComponentData(string filename)
		{
			if (Constants.FileExtensions.VectoJobFile.Equals(Path.GetExtension(filename), StringComparison.OrdinalIgnoreCase)) {
				return ReadJsonJob(filename, true);
			}
			return new JSONComponentInputData(filename, null, true);
		}

		public static IInputDataProvider ReadJsonJob(string filename, bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);

			switch (version) {
				case 2:
					return new JSONInputDataV2(json, filename, tolerateMissing);
				case 3:
					return new JSONInputDataV3(json, filename, tolerateMissing);
				case 4:
					if (json["Body"]["DeclarationVehicle"] != null) {
						return new JSONVTPInputDataV4(json, filename, tolerateMissing);
					}
					return new JSONInputDataV4(json, filename, tolerateMissing);
				case 5:
					return new JSONInputDataV5(json, filename, tolerateMissing);
				case 6:
					return new JSONInputDataSingleBusV6(json, filename, tolerateMissing);
				case 7:
					return new JSONInputDataCompletedBusFactorMethodV7(json, filename, tolerateMissing);
				case 8:
					return new JSONInputDataV8_ParallelHybrid(json, filename, tolerateMissing);
				case 9:
					return new JSONInputDataV9_BEV(json, filename, tolerateMissing);
				case 10:
					return new JSONInputDataV10_PrimaryAndStageInputBus(json, filename, tolerateMissing);
				case 11:
					return new JSONInputDataV11_SerialHybrid(json, filename, tolerateMissing);
				case 12:
					return new JSONInputDataV12_IEPC(json, filename, tolerateMissing);
				case 13: 
					return new JSONInputDataV13_IHPC(json, filename, tolerateMissing);
				default:
					throw new VectoException("Job-File: Unsupported FileVersion. Got: {0} ", version);
			}
		}

		public static IVehicleEngineeringInputData ReadJsonVehicle(string filename, IJSONVehicleComponents job, bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 7:
					return new JSONVehicleDataV7(json, filename, job, tolerateMissing);
				case 8:
					return new JSONVehicleDataV8(json, filename, job, tolerateMissing);
				case 9:
					return new JSONVehicleDataV9(json, filename, job, tolerateMissing);
				case 10:
					return new JSONVehicleDataV10_HEV_BEV(json, filename, job, tolerateMissing);
				case 11:
					return new JSONVehicleDataV11_IEPC(json, filename, job, tolerateMissing);
				default:
					throw new VectoException("Vehicle-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IGearboxEngineeringInputData ReadGearbox(string filename, bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 5:
					return new JSONGearboxDataV5(json, filename, tolerateMissing);
				case 6:
					return new JSONGearboxDataV6(json, filename, tolerateMissing);
				default:
					throw new VectoException("Gearbox-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IEngineEngineeringInputData ReadEngine(string filename, bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 3:
					return new JSONEngineDataV3(json, filename, tolerateMissing);
				case 4:
					return new JSONEngineDataV4(json, filename, tolerateMissing);
				case 5:
					return new JSONEngineDataV5(json, filename, tolerateMissing);
				default:
					throw new VectoException("Engine-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		private static int ReadVersion(JObject json)
		{
			var value = json.GetEx(JsonKeys.JsonHeader).GetEx<string>(JsonKeys.JsonHeader_FileVersion);
			return (int)double.Parse(value.Trim('"'));
		}

		public static IGearshiftEngineeringInputData ReadShiftParameters(string filename, bool tolerateMissing)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONTCUDataV1(json, filename, tolerateMissing);
				case 6:
					return new JSONGearboxDataV6(json, filename, tolerateMissing);
				default:
					throw new VectoException("GearshiftParameter-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IHybridStrategyParameters ReadHybridStrategyParameters(string filename, bool tolerateMissing)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONHybridStrategyParametersParallelHybrid(json, filename, tolerateMissing);
				case 2:
					return new JSONHybridStrategyParametersSerialHybrid(json, filename, tolerateMissing);
				default:
					throw new VectoException("HybridStrategyParameter-File: Unsupported FileVersion. Got {0}", version);

			}
		}


		public static IREESSPackInputData ReadREESSData(string filename, bool tolerateMissing)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONBatteryV1(json, filename, tolerateMissing);
				default:
					throw new VectoException("Battery-File: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IElectricMotorEngineeringInputData ReadElectricMotorData(string filename, bool tolerateMissing)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONElectricMotorV1(json, filename, tolerateMissing);
				case 2:
					return new JSONElectricMotorV2(json, filename, tolerateMissing);
				case 3:
					return new JSONElectricMotorV3(json, filename, tolerateMissing);
				case 4:
					return new JSONElectricMotorV4(json, filename, tolerateMissing);
				case 5:
					return new JSONElectricMotorV5(json, filename, tolerateMissing);
				default:
					throw new VectoException("ElectricMotor-File: Unsupported FileVersion. Got {0}", version);
			}

		}

		public static IBusAuxiliariesEngineeringData ReadEngineeringBusAuxiliaries(string filename, bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONBusAuxiliariesEngineeringDataV1(json, filename, tolerateMissing);
				default:
					throw new VectoException("Engineering BusAuxiliaries: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IIEPCEngineeringInputData ReadIEPCEngineeringInputData(string filename,
			bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version) {
				case 1:
					return new JSONIEPCData(json, filename, tolerateMissing);
				default:
					throw new VectoException("Engineering IEPC: Unsupported FileVersion. Got {0}", version);
			}
		}

		public static IElectricMotorEngineeringInputData ReadIHPCEngineeringInputData(string filename,
			bool tolerateMissing = false)
		{
			var json = ReadFile(filename);
			var version = ReadVersion(json);
			switch (version)
			{
				case 5:
					return ReadElectricMotorData(filename, tolerateMissing);
				default:
					throw new VectoException("Engineering IHPC: Unsupported FileVersion. Got {0}", version);
			}
		}
	}
}
