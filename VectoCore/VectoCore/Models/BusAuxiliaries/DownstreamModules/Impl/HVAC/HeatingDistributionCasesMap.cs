using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
    public class HeatingDistributionCasesMap
    {
        private List<Entry> Map;

        public HeatingDistributionCasesMap(List<Entry> map)
        {
            Map = map;
        }

        public HeatingDistributionCase GetHeatingDistributionCase(HeatPumpType heatPump, HeaterType electricHeater,
            bool fuelHeater)
        {
            var matching = Map.Where(x => x.IsEqual(heatPump, electricHeater, fuelHeater)).ToList();
            if (matching.Count != 1) {
                throw new VectoException($"Zero or more than one matching entry! {matching.Count}", ErrorCodes.SYSTEM_NO_EXACT_MATCH);
            }

            return matching.First().HeatingDistributionCase;
        }

        public class Entry
        {
            public HeatingDistributionCase HeatingDistributionCase;
            public HeatPumpType HeatPumpType;
            public bool ElectricHeater;
            public bool FuelHeater;

            public bool IsEqual(HeatPumpType heatPump, HeaterType electricHeater, bool fuelHeater)
            {
                var hasElectricHeater = electricHeater != HeaterType.None;
                var otherHeater = ElectricHeater == hasElectricHeater;
                var heatPumpType = (heatPump == HeatPumpType.not_applicable) ? HeatPumpType.none : heatPump;

                return HeatPumpType == heatPumpType && otherHeater && FuelHeater == fuelHeater;
            }
        }
    }
}