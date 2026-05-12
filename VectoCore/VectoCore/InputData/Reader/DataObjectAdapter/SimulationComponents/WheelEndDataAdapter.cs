using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	internal class WheelEndDataAdapter : IWheelEndDataAdapter
	{
		private const int WHEELENDS_PER_AXLE = 2;

		public WheelEndData CreateWheelEndData(VehicleClass vehicleClass, IList<IAxleDeclarationInputData> axles)
		{
			var stdFriction = DeclarationData.WwheelEndStdFrictions.Find(vehicleClass);

			var axlesWithFriction = axles.Where(x => x.WheelEndFriction != null);

			return new WheelEndData() { 
				DeltaFrictionTorque = CalculateDeltaFrictionTorque(axlesWithFriction.AsEnumerable(), stdFriction),
				DisallowedVehicleClassHasMeasuredData = Validate(axlesWithFriction.AsEnumerable(), stdFriction)
			};
		}

		private static NewtonMeter CalculateDeltaFrictionTorque(IEnumerable<IAxleDeclarationInputData> axlesWithFriction, 
			NewtonMeter stdFriction)
		{
			return (stdFriction != null)
				? axlesWithFriction
					.Select(x => x.WheelEndFriction.Value())
					.DefaultIfEmpty(stdFriction.Value())
					.Sum(x => (x - stdFriction.Value()) * WHEELENDS_PER_AXLE)
					.SI<NewtonMeter>()
				: 0.SI<NewtonMeter>();
		}

		private static bool Validate(IEnumerable<IAxleDeclarationInputData> axlesWithFriction, NewtonMeter stdFriction)
		{
			foreach (var axle in axlesWithFriction) {

				if ((stdFriction != null) && (axle.WheelEndFriction > stdFriction)) {
					throw new VectoException(
						$"WheelEndFriction ({axle.WheelEndFriction}) is greater than the vehicle's standard friction ({stdFriction}).", ErrorCodes.WHEELEND_FRICTION_EXCEEDS_STANDARD);
				}

				if (axle.WheelEndFriction < 0) {
					throw new VectoException($"WheelEndFriction ({axle.WheelEndFriction}) is negative.", ErrorCodes.WHEELEND_FRICTION_NEGATIVE);
				}

				if ((axle.WheelEndFriction != null) && (axle.AxleType == AxleType.VehicleDriven)) {
					throw new VectoException("VehicleDriven axle cannot have WheelEndFriction", ErrorCodes.WHEELEND_INVALID_AXLE_TYPE);
				}
			}

			return (stdFriction == null) && (axlesWithFriction.Count() > 0);
		}
		
	}
}
