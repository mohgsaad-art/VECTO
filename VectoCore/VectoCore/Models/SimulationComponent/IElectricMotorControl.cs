using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricMotorControl
	{
		/// <summary>
		/// the strategy decides if, and how much power the electric motor applies to the powertrain
		/// Convention: positive power retards energy and thus charges the battery, 
		///             negative power drives the powertrain and thus discharges the battery
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="prevOutAngularVelocity"></param>
		/// <param name="currOutAngularVelocity"></param>
		/// <param name="maxRecuperationTorque"></param>
		/// <param name="position"></param>
		/// <param name="dryRun"></param>
		/// <param name="maxDriveTorque"></param>
		/// <returns></returns>
		NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
			NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun);


		// TODO: MQ 2020-0618 - still needed?

		///// <summary>
		///// required for electric-only powertrain (i.e., serial hybrids) 
		///// returns the maximum power the engine may provide (i.e., full-load)
		///// </summary>
		///// <param name="avgSpeed"></param>
		///// <param name="dt"></param>
		///// <returns>power at full drive, has to be less than 0! </returns>
		//NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt);

		///// <summary>
		///// required for electric-only powertrain (i.e., serial hybrids)
		///// returns the maximum power the engine may apply during retardation (i.e., drag-load)
		///// </summary>
		///// <param name="avgSpeed"></param>
		///// <param name="dt"></param>
		///// <returns>power at full retardation (for current driving situation), has to be greater than 0!</returns>
		//NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt);
	}
}