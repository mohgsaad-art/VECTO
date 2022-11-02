using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public abstract class AbstractRESSResponse : IRESSResponse
	{
		protected AbstractRESSResponse(object source)
		{
			Source = source;
		}

		public Second AbsTime { get; set; }

		public Second SimulationInterval { get; set; }

		public Watt MaxChargePower { get; set; }

		public Watt MaxDischargePower { get; set; }

		public Watt PowerDemand { get; set; }

		public Watt LossPower { get; set; }

		public double StateOfCharge { get; set; }

		public object Source { get; }
	}


	public class RESSResponseSuccess : AbstractRESSResponse
	{
		public RESSResponseSuccess(object source) : base(source) { }

		
	}

	public class RESSOverloadResponse : AbstractRESSResponse
	{
		public RESSOverloadResponse(object source) : base(source) { }
	}

	public class RESSUnderloadResponse : AbstractRESSResponse
	{
		public RESSUnderloadResponse(object source) : base(source) { }
	}

	public class RESSDryRunResponse : AbstractRESSResponse
	{
		public RESSDryRunResponse(object source) : base(source) { }
	}



	public abstract class AbstractElectricSystemResponse : IElectricSystemResponse
	{
		protected AbstractElectricSystemResponse(object source)
		{
			Source = source;
		}

		public Second AbsTime { get; set; }

		public Second SimulationInterval { get; set; }

		public Watt AuxPower { get; set; }

		public Watt ConsumerPower { get; set; }

		public Watt ChargingPower { get; set; }

		public IRESSResponse RESSResponse { get; set; }

		public Watt MaxPowerDrive =>
			(RESSResponse != null && RESSResponse.MaxDischargePower != null ? RESSResponse.MaxDischargePower : 0.SI<Watt>()) -
			(ChargingPower != null ? ChargingPower : 0.SI<Watt>()) +
			(AuxPower != null ? AuxPower : 0.SI<Watt>());

		public Watt MaxPowerDrag =>
			(RESSResponse != null && RESSResponse.MaxChargePower != null ? RESSResponse.MaxChargePower : 0.SI<Watt>()) -
			(ChargingPower != null ? ChargingPower : 0.SI<Watt>()) +
			(AuxPower != null ? AuxPower : 0.SI<Watt>());

		public Watt RESSPowerDemand { get; set; }

		public object Source { get; }

		public override string ToString()
		{
			var t = GetType();
			return $"{t.Name}{{{t.GetProperties().Select(p => $"{p.Name}: {p.GetValue(this)}").Join()}}}";
		}
	}

	public class ElectricSystemResponseSuccess : AbstractElectricSystemResponse
	{
		public ElectricSystemResponseSuccess(object source) : base(source) { }
	}

	public class ElectricSystemOverloadResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemOverloadResponse(object source) : base(source) { }
	}

	public class ElectricSystemUnderloadResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemUnderloadResponse(object source) : base(source) { }
	}

	public class ElectricSystemDryRunResponse : AbstractElectricSystemResponse
	{
		public ElectricSystemDryRunResponse(object source) : base(source) { }

	}
}