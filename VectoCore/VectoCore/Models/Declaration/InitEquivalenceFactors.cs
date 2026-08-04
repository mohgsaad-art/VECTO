using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public class HEVStrategyParameters
	{
		public double PHEVChargeDepletingEquivalenceFactor => 0.01;

        protected AbstractHEVStrategyParameters Lorry = new HEVStrategyParametersLorry();
		protected AbstractHEVStrategyParameters Bus = new HEVStrategyParametersBus();

        public virtual double LookupEquivalenceFactor(MissionType mission, VehicleClass hdvClass,
			LoadingType loading,
			double socRange)
		{
			return hdvClass.IsHeavyLorry() || hdvClass.IsMediumLorry()
				? Lorry.LookupEquivalenceFactor(mission, hdvClass, loading, socRange)
				: Bus.LookupEquivalenceFactor(mission, hdvClass, loading, socRange);
		}

		public virtual double LookupSlope(MissionType mission, VehicleClass hdvClass, LoadingType loading)
		{
			return hdvClass.IsHeavyLorry() || hdvClass.IsMediumLorry()
				? Lorry.LookupSlope(mission, hdvClass, loading)
				: Bus.LookupSlope(mission, hdvClass, loading);
		}
    }


    public class HEVStrategyParametersLorry : AbstractHEVStrategyParameters
    {
		public HEVStrategyParametersLorry() : base("Lorry") { }
	}

	public class HEVStrategyParametersBus : AbstractHEVStrategyParameters
    {
		public HEVStrategyParametersBus() : base("Bus") { }
	}

	public abstract class AbstractHEVStrategyParameters
	{
		private Dictionary<int, InitEquivalenceFactors> _initEquivalenceFactors =
			new Dictionary<int, InitEquivalenceFactors>(3);

		private readonly IList<int> _socRanges = new List<int>{ 10, 20, 40 };
        private readonly Slope _slope;

        protected AbstractHEVStrategyParameters(string vehicle)
		{
			foreach(var range in _socRanges)
			{
                _initEquivalenceFactors.Add(range, new InitEquivalenceFactors(range, vehicle));
			}

			_slope = new Slope(vehicle);
		}



		/// <summary>
        /// Looks up the initial equivalence factor.
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="hdvClass"></param>
        /// <param name="loading"></param>
        /// <param name="socRange">If the SOC range of the vehicle to be simulated differs from the given SOC ranges, use the values from the given SCOC ranges that are closest to the SOC range of the vehicle </param>
        /// <returns></returns>
		public virtual double LookupEquivalenceFactor(MissionType mission, VehicleClass hdvClass, LoadingType loading,
			double socRange)
		{
			if (socRange > 1 || socRange < 0) {
				throw new ArgumentException($"{nameof(socRange)} must be between 0 and 1 ");
			}

			socRange *= 100; //Percent from here on
			var a = _socRanges.MinBy((i => Math.Abs(socRange - i))); //closest

			if (socRange <= _socRanges.Min() || socRange >= _socRanges.Max()) {
				return _initEquivalenceFactors[a].LookupEquivalenceFactor(mission, hdvClass, loading);
            }

			var b = _socRanges.Where(x => x != a).MinBy((i => Math.Abs(socRange - i))); //next close

			return VectoMath.Interpolate(a, b, _initEquivalenceFactors[a].LookupEquivalenceFactor(mission, hdvClass, loading),
				_initEquivalenceFactors[b].LookupEquivalenceFactor(mission, hdvClass, loading), socRange);
		}

		public virtual double LookupSlope(MissionType mission, VehicleClass hdvClass, LoadingType loading)
		{
			return _slope.Lookup( hdvClass.GetClassNumberWithoutSubSuffix(), mission.GetNonEMSMissionType().GetName().ToLowerInvariant(),
                loading);
		}

		private sealed class InitEquivalenceFactors : LookupData<string, InitEquivalenceFactors.Entry>
        {
            private string ResourceIdFormatString = DeclarationData.DeclarationDataResourcePrefix + ".HEVParameters.{1}.HEV_Strategy_Parameters_fequiv_{0}soc.csv";
            private string _resourceId;
            protected override string ResourceId => _resourceId; // todo rename csv file...


            /// <summary>
            /// 
            /// </summary>
            /// <param name="socRange">"20, 40, 10"</param>
            /// <param name="vehicleType">"Lorry" or "Bus"</param>
            public InitEquivalenceFactors(int socRange, string vehicleType) : base()
            {
                _resourceId = string.Format(ResourceIdFormatString, socRange, vehicleType);
                ReadData(); //call again here because the base constructor doesn't consider the updated ResourceId
            }
            public double LookupEquivalenceFactor(MissionType mission, VehicleClass hdvClass, LoadingType loading)
            {
                var entry = Lookup(hdvClass.GetClassNumberWithoutSubSuffix()).cycleDict[mission.GetNonEMSMissionType()];

                switch (loading)
                {
                    case LoadingType.LowLoading:
                        return entry.Item1;
                    case LoadingType.ReferenceLoad:
                        return entry.Item2;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(loading), loading, null);
                }
            }

            public override Entry Lookup(string key)
            {
                return base.Lookup(key.RemoveWhitespace());
            }

            protected override void ParseData(DataTable table)
            {
                //var vehicleClasses = table.Rows.Cast<DataRow>().Select(row => row.Field<string>("vehiclegroup"));

				var cycles = EnumHelper.GetValues<MissionType>().Where(x => table.Columns.Contains(x.ToString())).ToArray();

				//var cycleEntries = table.Rows.Cast<DataRow>().Select(row => new CycleEntry
    //            {
    //                VehicleGroup = row.Field<string>("vehiclegroup").RemoveWhitespace(),
    //                LongHaul = SplitStringToDoubleTuple(row.Field<string>("longhaul")),
    //                RegionalDelivery = SplitStringToDoubleTuple(row.Field<string>("regionaldelivery")),
    //                UrbanDelivery = SplitStringToDoubleTuple(row.Field<string>("urbandelivery")),
    //                MunicipalUtility = SplitStringToDoubleTuple(row.Field<string>("municipalutility")),
    //                Construction = SplitStringToDoubleTuple(row.Field<string>("construction"))
    //            });

				//foreach (var cycleEntry in cycleEntries) {
				foreach (DataRow row in table.Rows) {
					var vehicleGroups = row.Field<string>("vehiclegroup");
					foreach (string vehClass in vehicleGroups.Split('/')) {
                        var newEntry = new Entry
                        {
                            VehicleGroup = vehClass.Trim(),
                            cycleDict = new Dictionary<MissionType, Tuple<double, double>>()
                        };

						foreach (var cycle in cycles) {
							newEntry.cycleDict.Add(cycle,SplitStringToDoubleTuple(row.Field<string>(cycle.ToString())));
						}

                        //newEntry.cycleDict.Add(MissionType.LongHaul, cycleEntry.LongHaul);
                        //newEntry.cycleDict.Add(MissionType.RegionalDelivery, cycleEntry.RegionalDelivery);
                        //newEntry.cycleDict.Add(MissionType.UrbanDelivery, cycleEntry.UrbanDelivery);
                        //newEntry.cycleDict.Add(MissionType.MunicipalUtility, cycleEntry.MunicipalUtility);
                        //newEntry.cycleDict.Add(MissionType.Construction, cycleEntry.Construction);

                        Data.Add(newEntry.VehicleGroup, newEntry);
                    }
                }
            }

            //commented-out to avoid compile-time warning
            //public struct CycleEntry
            //{
            //    public string VehicleGroup;
            //    public Tuple<double, double> LongHaul;
            //    public Tuple<double, double> RegionalDelivery;
            //    public Tuple<double, double> UrbanDelivery;
            //    public Tuple<double, double> MunicipalUtility;
            //    public Tuple<double, double> Construction;

            //}

            public struct Entry
            {
                public string VehicleGroup;
                public Dictionary<MissionType, Tuple<double, double>> cycleDict;
            }

			private Tuple<double, double> SplitStringToDoubleTuple(string input)
            {
                var arr = input.Split('/');
                return input.IsNullOrEmpty() ? Tuple.Create(0.0, 0.0) : Tuple.Create(arr[0].ToDouble(), arr[1].ToDouble());
            }
        }


		private sealed class Slope : LookupData<string, string, LoadingType, double> //Class, MissionNonEMS, LoadingType
		{
            #region Overrides of LookupData

			private string ResourceIdFormatString = DeclarationData.DeclarationDataResourcePrefix + ".HEVParameters.{0}.Gradient_40.csv";
            private string _resourceId;

			protected override string ResourceId => _resourceId;
			protected override string ErrorMessage { get; }

			public Slope(string vehicleType)
			{
				_resourceId = string.Format(ResourceIdFormatString, vehicleType);
				ReadData(); //call again here because the base constructor doesn't consider the updated ResourceId
            }
			protected override void ParseData(DataTable table)
            {
                foreach (DataRow row in table.Rows)
                {
                    var vehicleGroups = row.Field<string>("vehiclegroup");

                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.Caption == "vehiclegroup")
                        {
                            continue;
                        }

						foreach (var vehicleGroup in vehicleGroups.Split('/')) {
							var values = SplitStringToDoubleTuple(row.Field<string>(col));
							Data[Tuple.Create(vehicleGroup.Trim(), col.Caption, LoadingType.LowLoading)] = values.Item1;
							Data[Tuple.Create(vehicleGroup.Trim(), col.Caption, LoadingType.ReferenceLoad)] = values.Item2;
						}
					}
                }
            }

			private Tuple<double, double> SplitStringToDoubleTuple(string input)
			{
				var arr = input.Split('/');
				return input.IsNullOrEmpty() ? Tuple.Create(0.0, 0.0) : Tuple.Create(arr[0].ToDouble(), arr[1].ToDouble());
			}
			#endregion
        }
	}

}





	
