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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration.Auxiliaries
{

	public class Fan : IDeclarationAuxiliaryTable
	{
		readonly FanMediumLorries fanMediumLorries = new FanMediumLorries();
		readonly FanHeavyLorries fanHeavyLorries = new FanHeavyLorries();

		public Watt LookupPowerDemand(VehicleClass vehicleClass, MissionType mission, string technology)
		{
			if (string.IsNullOrWhiteSpace(technology)) {
				technology = "Crankshaft mounted - Electronically controlled visco clutch";
			}
			if (!GetTechnologies().Contains(technology)) {
				throw new VectoException($"Auxiliary Lookup Error: Unknown Fan technology: '{technology}'", ErrorCodes.AUXILIARIES_UNKNOWN_TECHNOLOGY);
			}
			return vehicleClass.IsMediumLorry()
				? fanMediumLorries.Lookup(mission, technology, false).PowerDemand + fanMediumLorries.Lookup(mission, technology, true).PowerDemand
				: fanHeavyLorries.Lookup(mission, technology, false).PowerDemand + fanHeavyLorries.Lookup(mission, technology, true).PowerDemand;
		}

		public Watt LookupMechanicalPowerDemand(VehicleClass vehicleClass, MissionType mission, string technology)
		{
			if (!GetTechnologies().Contains(technology)) {
				throw new VectoException($"Auxiliary Lookup Error: Unknown Fan technology: '{technology}'", ErrorCodes.AUXILIARIES_UNKNOWN_TECHNOLOGY);
			}
			return (vehicleClass.IsMediumLorry()
				? fanMediumLorries.Lookup(mission, technology, false)
				: fanHeavyLorries.Lookup(mission, technology, false)).PowerDemand;
		}

		public Watt LookupElectricalPowerDemand(VehicleClass vehicleClass, MissionType mission, string technology)
		{
			if (!GetTechnologies().Contains(technology)) {
				throw new VectoException($"Auxiliary Lookup Error: Unknown Fan technology: '{technology}'", ErrorCodes.AUXILIARIES_UNKNOWN_TECHNOLOGY);
			}
			return (vehicleClass.IsMediumLorry()
				? fanMediumLorries.Lookup(mission, technology, true)
				: fanHeavyLorries.Lookup(mission, technology, true)).PowerDemand;
		}

		public bool IsApplicable(VehicleClass vehicleClass, VectoSimulationJobType jobType, string technology)
		{
			return vehicleClass.IsMediumLorry()
				? fanMediumLorries.IsApplicable(jobType, technology)
				: fanHeavyLorries.IsApplicable(jobType, technology);
		}

		public bool IsFullyElectric(VehicleClass vehicleClass, string technology)
		{
			var tech = vehicleClass.IsMediumLorry()
				? fanMediumLorries.FullyElectricTechnologies()
				: fanHeavyLorries.FullyElectricTechnologies();
			return tech.Contains(technology);
		}

		public string[] FullyElectricTechnologies()
		{
			return fanHeavyLorries.FullyElectricTechnologies();
		}

		public string[] GetTechnologies() => fanHeavyLorries.GetTechnologies();
	}

	public abstract class AbstractFan : LookupData<MissionType, string, bool, AuxDemandEntry>, IDeclarationAuxiliaryTable, IDeclarationAuxiliaryArchitectureTable, IDeclarationAuxiliaryFullyElectricTable
	{


		protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for Fan. Mission: '{0}', Technology: '{1}'";

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				var name = row.Field<string>("technology");
				var electric = row.ParseBoolean("fullyelectric");

				foreach (DataColumn col in table.Columns.Cast<DataColumn>().Skip(table.Columns.IndexOf("fullyelectric") + 1))
				{
					//if (col.Caption != "technology" && col.Caption != "fullyelectric") {
						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), name, electric)] = new AuxDemandEntry {
							PowerDemand = row.ParseDouble(col).SI<Watt>(),
						};
					//}
				}
			}
		}

		public override AuxDemandEntry Lookup(MissionType mission, string technology, bool electrical)
		{
			var lookup = Tuple.Create(mission, technology, electrical);
			return Data.GetVECTOValueOrDefault(lookup, new AuxDemandEntry { PowerDemand = 0.SI<Watt>() });
		}

		public bool IsFullyElectric(string technology)
		{
			return FullyElectricTechnologies().Contains(technology);
		}

		public string[] FullyElectricTechnologies() => Data.Keys.Where(x => x.Item3).Select(x => x.Item2).Distinct().ToArray();

		public string[] GetTechnologies() => Data.Keys.Select(x => x.Item2).Distinct().ToArray();

		protected abstract IDeclarationAuxiliaryArchitectureTable archMapping { get; }

		public  bool IsApplicable(VectoSimulationJobType simType, string technology)
		{
			return archMapping.IsApplicable(simType, technology);
		}

	}


	public sealed class FanMediumLorries : AbstractFan
	{
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.Fan-Tech-Medium.csv";

		#region Overrides of AbstractFan

		protected override IDeclarationAuxiliaryArchitectureTable archMapping => new
			FanMediumLorriesVehicleArchitecture();

		
		#endregion
	}

	public sealed class FanHeavyLorries : AbstractFan
	{
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.Fan-Tech.csv";

		protected override IDeclarationAuxiliaryArchitectureTable archMapping => new
			FanHeavyLorriesVehicleArchitecture();

	}

	public sealed class FanMediumLorriesVehicleArchitecture : AbstractAuxiliaryVehicleArchitectureLookup
	{
		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.Fan-Tech-Medium.csv";


		#endregion
	}

	public sealed class FanHeavyLorriesVehicleArchitecture : AbstractAuxiliaryVehicleArchitectureLookup
	{
		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.Fan-Tech.csv";


		#endregion
	}

	public sealed class FanBusCoolingCoefficient : LookupData<string, double>
	{
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUXBus.Fan-Tech-CoolingPowerCoef_Bus.csv";

		protected override string ErrorMessage => throw new NotImplementedException();

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows)
			{
				var name = row["technology"].ToString();
				foreach (DataColumn col in table.Columns.Cast<DataColumn>().Skip(1).Take(1))
				{
					Data[name] = row.ParseDouble(col.Caption);
				}
			}
		}

		public double GetTechnologyCoefficient(string technology)
		{
			Data.TryGetValue(technology, out var coefficient);
			
			return coefficient;
		}
	}

}