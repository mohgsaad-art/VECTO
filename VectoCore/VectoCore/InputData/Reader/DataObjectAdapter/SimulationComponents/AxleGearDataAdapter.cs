using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class AxleGearDataAdapter : IAxleGearDataAdapter
	{
		public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
		{
			if (double.IsNaN(gbxData.AxlegearRatio))
			{
				throw new VectoException(
					"Axlegear ratio required in gearbox data for gearboxes with included axlegear", ErrorCodes.AXLEGEAR_MISSING_RATIO);
			}

			return new AxleGearData()
			{
				AxleGear = new TransmissionData()
				{
					Ratio = gbxData.AxlegearRatio,
					LossMap = TransmissionLossMapReader.Create(1.0, gbxData.AxlegearRatio, "Axlegear")
				}
			};
		}
		internal TransmissionLossMap ReadAxleLossMap(IAxleGearInputData data, bool useEfficiencyFallback)
		{
			TransmissionLossMap axleLossMap;
			if (data.LossMap == null && useEfficiencyFallback)
			{
				axleLossMap = TransmissionLossMapReader.Create(data.Efficiency, data.Ratio, "Axlegear");
			}
			else
			{
				if (data.LossMap == null)
				{
					throw new VectoException("LossMap for Axlegear is missing.", ErrorCodes.FILE_INVALID_FORMAT);
				}
				axleLossMap = TransmissionLossMapReader.Create(data.LossMap, data.Ratio, "Axlegear", true);
			}
			if (axleLossMap == null)
			{
				throw new VectoException("LossMap for Axlegear is missing.", ErrorCodes.FILE_INVALID_FORMAT);
			}
			return axleLossMap;
		}
		internal AxleGearData SetCommonAxleGearData(IAxleGearInputData data)
		{
			return new AxleGearData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				LineType = data.LineType,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				AxleGear = new GearData { Ratio = data.Ratio }
			};
		}
		public virtual AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, false);
			return retVal;
		}
	}

	public class GenericCompletedBusAxleGearDataAdapter : AxleGearDataAdapter
	{
		#region Overrides of AxleGearDataAdapterBase
		private readonly GenericTransmissionComponentData _genericPowertrainData = new GenericTransmissionComponentData();
		public override AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			return _genericPowertrainData.CreateGenericBusAxlegearData(data);
		}

		#endregion
	}
}