using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCore.Utils.Ninject
{
	public interface IVehicleTypeAndArchitectureStringHelperReport
	{
		string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN, 
			bool exempted = false, bool iepc = false, bool ihpc = false);

		object[] ToParams(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
            bool exempted, bool iepc, bool ihpc);

		CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; }
	}

	public class VehicleTypeAndArchitectureStringHelperReport : IVehicleTypeAndArchitectureStringHelperReport
	{
		public  CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = (arguments => {

			//may be called with first argument of type string (when defining the bindings) or VehicleCategory when using the factory
			string vehicleType = arguments[0] as string;
			if (arguments[0] is VehicleCategory vehicleCategory) {
				vehicleType = vehicleCategory.GetVehicleType();
			}
			
			VectoSimulationJobType jobType = (VectoSimulationJobType)arguments[1];
			
			ArchitectureID archId = (ArchitectureID)arguments[2];
			if (jobType == VectoSimulationJobType.ParallelHybridVehicle){
				archId = ArchitectureID.P1; //same report for all p-hevs
			}

			bool exempted = (bool)arguments[3];
			bool iepc = (bool)arguments[4];
			bool ihpc = (bool)arguments[5];

			var classification =
				new VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification(jobType, archId, vehicleType,
					exempted, iepc, ihpc);

			return classification.ToString();
		});

		public virtual string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
			bool exempted = false, bool iepc = false, bool ihpc = false)
		{
			return CreateName(ToParams(vehicleType, jobType, archId, exempted, iepc, ihpc));
		}

		public virtual object[] ToParams(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
			bool exempted, bool iepc, bool ihpc)
		{
			if (!VehicleCategoryHelper.SuperCategories.Contains(vehicleType))
			{
				throw new Exception(
					$"String provided for {nameof(vehicleType)} must match the strings in {nameof(VehicleCategoryHelper.SuperCategories)}");
			}

			return new[] { (object)vehicleType, jobType, archId, exempted, iepc, ihpc };
		}
	}

	public class VehicleTypeAndArchitectureStringHelperRundata
	{
		public CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = arguments => 
		{
			if (arguments[0] is VehicleClassification classification) 
			{
                return GetName(classification);
            } 
			else {
				throw new ArgumentException($"{nameof(arguments)}[0] must be of type {typeof(VehicleClassification)}");
			}
		};

		#region Overrides of VehicleTypeAndArchitectureStringHelperReportBase

		private static string GetName(VehicleClassification classification)
		{
			return classification.GetHashCode().ToString();
		}

		public string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
            bool exempted = false, bool iepc = false, bool ihpc = false)
		{
			return GetName(new VehicleClassification(jobType, archId, vehicleType, exempted, iepc, ihpc));
		}

		public string GetName(string vehicleType, bool exempted)
		{
			return GetName(vehicleType, VectoSimulationJobType.ConventionalVehicle, exempted: exempted);
		}

		public string GetSingleBusName(VectoSimulationJobType jobType, ArchitectureID archId, bool exempted = false)
		{
			return GetName(new VehicleClassification(jobType, archId, null, exempted, false, false, true));
		}

		#endregion

		public struct VehicleClassification
		{
			private const string _singlebus = "SingleBus";

			private readonly VectoSimulationJobType _jobType;
			private readonly bool _exempted;
			private readonly ArchitectureID _archId;
            private readonly string _vehicleType;
			private readonly bool _iepc;
			private readonly bool _ihpc;
			private readonly bool _isSingleBus;

			private VectoSimulationJobType JobType => Exempted ? VectoSimulationJobType.ConventionalVehicle : _jobType; //ignored for exempted vehicles

			private ArchitectureID ArchId => Exempted ? ArchitectureID.UNKNOWN : _archId;//ignored for exempted vehicles

            private string VehicleType => _vehicleType;

			private bool Exempted => _exempted;

			private bool Iepc => _iepc;

			private bool Ihpc => _ihpc;

			private bool IsSingleBus => _isSingleBus;

			public VehicleClassification(VectoSimulationJobType jobType, ArchitectureID archId, string vehicleType, 
				bool exempted, bool iepc, bool ihpc, bool singleBus = false)
			{
				_iepc = iepc;
				_ihpc = ihpc;
				_exempted = exempted;
				_vehicleType = singleBus ? _singlebus : vehicleType;
				_archId = archId;
				_jobType = jobType;
				_isSingleBus = singleBus;
			}

			public VehicleClassification(IVehicleDeclarationInputData inputData) : this()
			{
				//Iepc = (inputData.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				_iepc = false;
				_ihpc = false;
				_exempted = inputData.ExemptedVehicle;
				_vehicleType = inputData.VehicleCategory.GetVehicleType();
				_archId = inputData.ArchitectureID;
				_jobType = inputData.VehicleType;
				_isSingleBus = false;
			}


			public VehicleClassification(IMultistageVIFInputData inputData) : this()
			{
				//Iepc = (inputData.MultistageJobInputData..Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				_iepc = false;
				_ihpc = false;
				_exempted = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle;
				_vehicleType = inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory.GetVehicleType();
				_archId = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;
				_jobType = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType;
				_isSingleBus = false;
				//inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.
			}

			public VehicleClassification(ISingleBusInputDataProvider singleBus) : this()
			{
				_isSingleBus = true;
				_vehicleType = _singlebus;
				_archId = singleBus.PrimaryVehicle.ArchitectureID;
				_jobType = singleBus.PrimaryVehicle.VehicleType;
				_exempted = singleBus.PrimaryVehicle.ExemptedVehicle;
			}

			public bool Equals(VehicleClassification other)
			{
				return JobType == other.JobType && ArchId == other.ArchId && VehicleType == other.VehicleType 
					&& Exempted == other.Exempted && Iepc == other.Iepc && Ihpc == other.Ihpc;
			}

			public override bool Equals(object obj)
			{
				return obj is VehicleClassification other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = (int)JobType;
					hashCode = (hashCode * 397) ^ (int)ArchId;
                    hashCode = (hashCode * 397) ^ (VehicleType != null ? VehicleType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ Exempted.GetHashCode();
					hashCode = (hashCode * 397) ^ Iepc.GetHashCode();
					hashCode = (hashCode * 397) ^ Ihpc.GetHashCode();
					return hashCode;
				}
			}

			#region Overrides of ValueType

			public override string ToString()
			{
				return string.Join("\n", VehicleType, JobType, ArchId, Exempted ? "Exempted" : "Non Exempted",
					IsSingleBus ? "Single Bus" : "");
			}

			#endregion
		}

	}

	public class VehicleTypeAndArchitectureStringHelperResults
	{
		public CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = arguments => 
		{
			if (arguments[0] is ResultsVehicleClassification classification) 
			{
				return GetName(classification);
			} 
			else 
			{
				throw new ArgumentException($"{nameof(arguments)}[0] must be of type {typeof(ResultsVehicleClassification)}");
			}
		};

		private static string GetName(ResultsVehicleClassification classification)
		{
			return classification.GetHashCode().ToString();
		}

		public string GetName(XmlDocumentType type, string vehicleCategory, string jobType, bool ovc,
			bool exempted = false)
		{
			return GetName(new ResultsVehicleClassification(type, vehicleCategory, jobType, ovc, exempted));
		}

		public string GetName(XmlDocumentType type, string vehicleType, bool exempted)
		{
			return GetName(type, vehicleType, VectoSimulationJobTypeHelper.Conventional, false, exempted: exempted);
		}

		public struct ResultsVehicleClassification
		{
			private readonly string _vehicleCategory;
			private readonly string _powertrainCategory;
			private readonly bool _ovc;
			private readonly bool _exempted;

			public ResultsVehicleClassification(XmlDocumentType type, string vehicleCategory, string powertrainCategory, bool ovc, bool exempted)
			{
				_vehicleCategory = vehicleCategory;
				_powertrainCategory = powertrainCategory;
				_ovc = ovc;
				_exempted = exempted;
				DocumentType = type;
			}

			public XmlDocumentType DocumentType { get; }

			public bool Exempted => _exempted;
			public string JobType => Exempted ? VectoSimulationJobTypeHelper.Conventional : _powertrainCategory;

			public string VehicleCategory => _vehicleCategory;
			public bool OVC => Exempted ? false : _ovc;

			#region Overrides of ValueType

			public override bool Equals(object obj)
			{
				return obj is ResultsVehicleClassification other && Equals(other);
			}

			public bool Equals(ResultsVehicleClassification other)
			{
				return VehicleCategory == other.VehicleCategory 
					&& JobType == other.JobType
					&& OVC == other.OVC
					&& Exempted == other.Exempted;
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = (VehicleCategory != null ? VehicleCategory.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (JobType != null ? JobType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ OVC.GetHashCode();
					hashCode = (hashCode * 397) ^ Exempted.GetHashCode();
					hashCode = (hashCode * 397) ^ DocumentType.GetHashCode();
					return hashCode;
				}
			}

			#endregion

			public override string ToString()
			{
				return string.Join("|", VehicleCategory, JobType, OVC ? "OVC" : "Non-OVC", Exempted ? "Exempted" : "Non Exempted");
			}
		}
	}

	public class UseFirstArgumentStringHelper
	{
		public CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = arguments => {
			if (arguments.Length > 0) {
				return arguments[0].ToString();
			}

			return null;
		};
    }
}