using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Interfaces
{
	public interface IPrimaryVehicleBusViewModel : IComponentViewModel
	{
		#region Interfase IPrimaryVehicleInputDataProvider
		IVehicleDeclarationInputData Vehicle { get; set; }
		DigestData ResultDataHash { get; set; }
		IResultsInputData ResultsInputData { get; set; }
		IApplicationInformation ApplicationInformation { get; set; }
		DigestData ManufacturerHash { get; set; }

		#endregion

		#region VehiclePIFType

		string Manufacturer { get; set; }
		string ManufacturerAddress { get; set; }
		string Model { get; set; }
		string VIN { get; set; }
		DateTime Date { get; set; }
		VehicleCategory VehicleCategory { get; set; }
		AxleConfiguration AxleConfiguration { get; set; }
		bool Articulated { get; set; }
		Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		PerSecond IdlingSpeed { get; set; }
		RetarderType RetarderType { get; set; }
		double RetarderRatio { get; set; }
		AngledriveType AngledriveType { get; set; }
		bool ZeroEmissionVehicle { get; set; }

		//************** ADAS **************
		bool EngineStopStart { get; set; }
		EcoRollType EcoRoll { get; set; }
		PredictiveCruiseControlType PredictiveCruiseControl { get; set; }

		//**********************************


		IList<ITorqueLimitInputData> TorqueLimits { get; set; }

		#region Allowed Enties

		AllowedEntry<VehicleCategory>[] AllowedVehicleCategories { get; }
		AllowedEntry<AxleConfiguration> [] AllowedAxleConfigurations { get; }
		AllowedEntry<RetarderType>[] AllowedRetarderTypes { get; set; }
		AllowedEntry<AngledriveType>[] AllowedAngledriveTypes { get; set; }
		AllowedEntry<EcoRollType>[] AllowedEcoRollTypes { get; set; }
		AllowedEntry<PredictiveCruiseControlType>[] AllowedPredictiveCruiseControl { get; set; }

		#endregion
		
		#endregion


	}
}
