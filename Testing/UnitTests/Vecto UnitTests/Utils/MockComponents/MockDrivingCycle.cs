using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils;

public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
{
	private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _left;
	private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _right;

	public MockDrivingCycle(IVehicleContainer container, DrivingCycleData data) : base(container)
	{
		if (data != null) {
			_left = data.Entries.GetEnumerator();
			_right = data.Entries.GetEnumerator();
		} else {
			_left = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
			_right = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
		}
		_left.MoveNext();
		_right.MoveNext();
		_right.MoveNext();
	}


	public CycleData CycleData =>
		new CycleData {
			AbsTime = 0.SI<Second>(),
			AbsDistance = 0.SI<Meter>(),
			LeftSample = _left.Current,
			RightSample = _right.Current
		};

	public bool PTOActive { get; set; }

	public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
	{
		return new DrivingCycleData.DrivingCycleEntry() {
			RoadGradient = 0.SI<Radian>(),
			Altitude = 0.SI<Meter>()
		};
	}

	public Meter Altitude => 0.SI<Meter>();

	public Radian RoadGradient => 0.SI<Radian>();
	public MeterPerSecond TargetSpeed { get; set; }
	public Second StopTime { get; set; }

	protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
	{
		container[ModalResultField.dist] = 0.SI<Meter>();
		container[ModalResultField.v_targ] = 0.KMPHtoMeterPerSecond();
		container[ModalResultField.grad] = 0.SI<Scalar>();
		container[ModalResultField.altitude] = 0.SI<Meter>();
	}

	protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
	{
		_left.MoveNext();
		_right.MoveNext();
	}

	public Meter CycleStartDistance => 0.SI<Meter>();

	public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
	{
		return new List<DrivingCycleData.DrivingCycleEntry>();
	}

	public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
	{
		return new List<DrivingCycleData.DrivingCycleEntry>();
	}

	public SpeedChangeEntry LastTargetspeedChange { get; set; }

	public void FinishSimulation() { }

	protected override bool DoUpdateFrom(object other) => false;

}