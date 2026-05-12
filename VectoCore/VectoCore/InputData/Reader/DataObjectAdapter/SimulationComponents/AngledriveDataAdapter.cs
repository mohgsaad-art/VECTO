using System;
using NLog.Fluent;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class AngledriveDataAdapter : IAngledriveDataAdapter
	{
		public AngledriveData CreateAngledriveData(IAngledriveInputData data, bool useEfficiencyFallback)
		{
			try {
				var type = data?.Type ?? AngledriveType.None;

				switch (type) {
					case AngledriveType.LossesIncludedInGearbox:
					case AngledriveType.None:
						return null;
					case AngledriveType.SeparateAngledrive:
						var angledriveData = new AngledriveData {
							InputData = data,
							SavedInDeclarationMode = data.SavedInDeclarationMode,
							Manufacturer = data.Manufacturer,
							ModelName = data.Model,
							Date = data.Date,
							CertificationMethod = data.CertificationMethod,
							CertificationNumber = data.CertificationNumber,
							DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
							Type = type,
							Angledrive = new TransmissionData { Ratio = data.Ratio }
						};
						try {
							angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.LossMap,
								data.Ratio, "Angledrive", true);
						} catch (VectoException ex) {
							Log.Info("Angledrive Loss Map not found.");
							if (useEfficiencyFallback) {
								Log.Info("Angledrive Trying with Efficiency instead of Loss Map.");
								angledriveData.Angledrive.LossMap = TransmissionLossMapReader.Create(data.Efficiency,
									data.Ratio, "Angledrive");
							} else {
								throw new VectoException("Angledrive: LossMap not found.", ErrorCodes.ANGLEDRIVE_LOSS_MAP_NOT_FOUND, ex);
							}
						}

						return angledriveData;
					default:
						throw new ArgumentOutOfRangeException(nameof(data), "Unknown Angledrive Type.");
				}
			} catch (Exception e) {
				throw new VectoException($"Error while reading Angledrive data: {e.Message}", e.GetErrorCode(), e);
			}
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return CreateAngledriveData(data, false);
		}
	}

	public class GenericAngledriveDataAdapter : IAngledriveDataAdapter
	{
		private readonly GenericTransmissionComponentData _genericPowertrainData = new GenericTransmissionComponentData();
		#region Implementation of IAngledriveDataAdapter

		public AngledriveData CreateAngledriveData(IAngledriveInputData data, bool useEfficiencyFallback)
		{
			return _genericPowertrainData.CreateGenericBusAngledriveData(data);
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return CreateAngledriveData(data, false);
		}

		#endregion
	}
}