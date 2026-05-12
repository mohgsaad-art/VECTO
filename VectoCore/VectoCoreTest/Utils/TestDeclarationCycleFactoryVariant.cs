using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class TestMissionFilter : IMissionFilter
	{
		private HashSet<(MissionType m, LoadingType l)> _missionsToRun = new HashSet<(MissionType m, LoadingType l)>();

		#region Implementation of IMissionFilter

		public bool Run(MissionType missionType, LoadingType loadingType)
		{
			var run = _missionsToRun.Contains((missionType, loadingType));
			if(!run){
				TestContext.Progress.WriteLine($"[{nameof(TestMissionFilter)}] skipping {missionType} - {loadingType}");
			}
			return run;
		}

		public void SetMissions(params (MissionType m, LoadingType loadingType)[] missionLoadingPairs)
		{
			foreach (var missionLoadingPair in missionLoadingPairs) {
				_missionsToRun.Add((missionLoadingPair.m, missionLoadingPair.loadingType));
			}
		}

		#endregion
	}


	public class TestDeclarationCycleFactoryVariant : IDeclarationCycleFactory
	{
		private const string BASE_PATH = "Resources/Missions/";
		public static readonly ConcurrentDictionary<MissionType, DrivingCycleData> CyclesCache =
			new ConcurrentDictionary<MissionType, DrivingCycleData>();


        public TestDeclarationCycleFactoryVariant()
		{

		}

		public string Variant { get; set; } = "Short_10";

		public virtual DrivingCycleData GetDeclarationCycle(Mission mission)
		{
			//return CyclesCache.GetOrAdd(mission.MissionType, ReadDeclarationCycle);
            
			
			
			return ReadDeclarationCycle(mission.MissionType);
		}

		protected virtual DrivingCycleData ReadDeclarationCycle(MissionType missionType)
		{
			var cycleFile = Path.Combine(BASE_PATH, Variant,
				missionType.ToString().Replace("EMS", "") + ".vdri");
			if (File.Exists(cycleFile)) {
				TestContext.Progress.WriteLine($"[{nameof(TestDeclarationCycleFactoryVariant)}] - Using {cycleFile}");
                var cycle = File.OpenRead(cycleFile);
				return DrivingCycleDataReader.ReadFromStream(cycle, CycleType.DistanceBased, "", false);
			}

			throw new Exception($"Cycle data for mission type {missionType} not found!");
		}
	}


	public class TestDeclarationCycleFactoryStartPoint : DeclarationCycleFactory
	{
		private bool _shortMissing = false;
		private readonly Dictionary<MissionType, (Meter start, Meter distance)> _startPointDict = new Dictionary<MissionType, (Meter start, Meter distance)>();
		#region Implementation of IDeclarationCycleFactory
		/// <summary>
		/// Sets start point for specific mission
		/// </summary>
		/// <param name="missionType"></param>
		/// <param name="startPoint"></param>
		/// <param name="shortMissing">if set to true, the last section of all cycle that are not specified is simulated</param>
		/// <param name="distance"></param>
		public void SetStartPoint(MissionType missionType, Meter startPoint, bool shortMissing, Meter distance = null)
		{
			_startPointDict[missionType] = (start: startPoint, distance: distance);
			_shortMissing = shortMissing;
		}
		/// <summary>
		/// Sets start point for all missions
		/// </summary>
		/// <param name="startPoint"></param>
		/// <param name="distance"></param>
		public void SetStartPoint(Meter startPoint, Meter distance = null)
		{
			foreach (var missionType in Enum.GetValues<MissionType>()) {
				_startPointDict[missionType] = (start: startPoint, distance: distance);
            }
		}

		public override DrivingCycleData GetDeclarationCycle(Mission mission)
		{
			var cycle = base.GetDeclarationCycle(mission);
			if (_startPointDict.TryGetValue(mission.MissionType, out var entry)) {
				RestrictCycle(cycle, entry);
			} else if(_shortMissing) {
				LastSection(cycle);
			}

			TestContext.Progress.WriteLine($"[{nameof(TestDeclarationCycleFactoryStartPoint)}] Cycle {mission.MissionType} restricted to \n \t" +
								$" start: [{cycle.Entries.First().Distance} - {cycle.Entries.First().VehicleTargetSpeed}] \n \t" +
								$" end: [{cycle.Entries.Last().Distance} - {cycle.Entries.Last().VehicleTargetSpeed}]");
	
			return cycle;
		}

		private void LastSection(DrivingCycleData cycle)
		{
			var entries = cycle.Entries;
			var stop = entries.Last();
			var start = entries.Last(e => e.VehicleTargetSpeed.IsEqual(0) 
										&& !e.Distance.IsEqual(stop.Distance));
			entries.RemoveAll(e =>
				e.Distance.IsSmaller(start.Distance) || e.Distance.IsGreater(stop.Distance));
        }

		private void RestrictCycle(DrivingCycleData cycle, (Meter start, Meter distance) startPoint)
		{
	
			var entries = cycle.Entries;
			DrivingCycleData.DrivingCycleEntry startEntry = entries.First();
			DrivingCycleData.DrivingCycleEntry endEntry = entries.Last();

			startEntry = entries
				.Where(e => e.Distance.IsSmallerOrEqual(startPoint.start) &&
							e.VehicleTargetSpeed.IsEqual(0)) //Select all points before startpoint
				.MinBy(e => startPoint.start - e.Distance) ?? startEntry; //select the nearest one;
			
			//remove entries before selected start point
			//var startIdx = entries.FindIndex(e => e.Distance.IsEqual(startEntry.Distance));
			//entries.RemoveRange(0, startIdx);

			if (startPoint.distance != null) {



				var endIdx = entries.FindIndex(e =>
					(e.Distance - startEntry.Distance).IsGreaterOrEqual(startPoint.distance) &&
					e.VehicleTargetSpeed.IsEqual(0));
				if (endIdx != -1) {
					endEntry = entries[endIdx];
				}
				
			}

			entries.RemoveAll(e =>
				e.Distance.IsSmaller(startEntry.Distance) || e.Distance.IsGreater(endEntry.Distance));
		}
		#endregion
	}
}