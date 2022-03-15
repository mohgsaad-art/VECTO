using System;
using System.Xml;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{

	public abstract class JSONSubComponent
	{
		protected JSONVehicleDataV7 Base;

		protected JSONSubComponent(JSONVehicleDataV7 jsonFile)
		{
			Base = jsonFile;
		}

		protected JObject Body => Base.Body;

		protected TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			return Base.ReadTableData(filename, tableType, required);
		}

		protected string BasePath => Base.BasePath;

		//protected bool TolerateMissing => Base.TolerateMissing;

		public DataSource DataSource => Base.DataSource;

		public bool SavedInDeclarationMode => Base.SavedInDeclarationMode;

		public string Manufacturer => Base.Manufacturer;

		public string Model => Base.Model;

		public string Date => Base.Date;

		//public string AppVersion => Base.AppVersion;

		public CertificationMethod CertificationMethod => Base.CertificationMethod;

		public string CertificationNumber => Base.CertificationNumber;

		public DigestData DigestValue => Base.DigestValue;

		//public virtual XmlNode XMLSource => Base.XMLSource;
	}


	// ###################################################################
	// ###################################################################

	internal class JSONADASInputDataV7 : JSONSubComponent, IAdvancedDriverAssistantSystemsEngineering
	{
		public JSONADASInputDataV7(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		public virtual bool EngineStopStart => DeclarationData.Vehicle.ADAS.EngineStopStartDefault;

		public virtual EcoRollType EcoRoll => DeclarationData.Vehicle.ADAS.EcoRoll;

		public virtual PredictiveCruiseControlType PredictiveCruiseControl => DeclarationData.Vehicle.ADAS.PredictiveCruiseControlDefault;

		public virtual bool? ATEcoRollReleaseLockupClutch => null;

		#endregion

	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV8 : JSONADASInputDataV7
	{
		public JSONADASInputDataV8(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		#region Overrides of JSONADASInputDataV7

		public override bool EngineStopStart => Body.GetEx<bool>("EngineStopStart");

		public override EcoRollType EcoRoll => EcorollTypeHelper.Parse(Body.GetEx<string>("EcoRoll"));

		public override PredictiveCruiseControlType PredictiveCruiseControl => Body.GetEx<string>("PredictiveCruiseControl").ParseEnum<PredictiveCruiseControlType>();

		#endregion
	}

	// -------------------------------------------------------------------

	internal class JSONADASInputDataV9 : JSONADASInputDataV8
	{
		public JSONADASInputDataV9(JSONVehicleDataV7 vehicle) : base(vehicle) { }

		public override bool? ATEcoRollReleaseLockupClutch => Body["ATEcoRollReleaseLockupClutch"]?.Value<bool>();
	}
}