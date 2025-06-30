using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class TestGenericBusPowertrainModelData
	{

        /*  expected values copied from excel demo calculation (T_max=1120, n_rated=2200, i=1
                   , mue   , TP_1000
                   , [-]   , [-]   , [Nm]
Stall point        , 0.000 , 1.800 , 377.804
Intermediate point , 0.600 , 1.233 , 302.243
Coupling point     , 0.900 , 0.950 , 188.902
Overrun            , 1.000 , 0.950 , 0.000
Drag               , 5.000 , 0.900 , -1511.216

		 */
		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericATSerialTCData()
		{
			var maxEngineTorque = 1120.SI<NewtonMeter>();
			var engineRatedSpeed = 2200.RPMtoRad();
			var ratioFirstGear = 1;

			var genericTCData =
				GenericTorqueConverterData.CreateGenericTorqueConverterCharacteristics(ratioFirstGear,
					engineRatedSpeed, maxEngineTorque);

			var tcData = TorqueConverterDataReader.Create(
				genericTCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratioFirstGear,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);

			// stall point
			var idx = 0;
			Assert.AreEqual(0.0, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.8, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(377.80, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			// intermediate point
			idx = 1;
			Assert.AreEqual(0.6, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.23, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(302.24, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			// coupling point
			idx = 2;
			Assert.AreEqual(0.9, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(0.95, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(188.90, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			// overrun point
			idx = 3;
			Assert.AreEqual(1, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(0.95, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(0, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);
        }

        /*  expected values copied from excel demo calculation (T_max=1120, n_rated=2200, i=1.4492754
					, nue   , mue   , TP_1000
					, [-]   , [-]   , [Nm]
Stall point         , 0.000 , 5.507 , 377.80
					, 0.069 , 4.039 , 346.32
					, 0.138 , 3.430 , 314.84
					, 0.207 , 2.964 , 283.35
					, 0.276 , 2.570 , 251.87
					, 0.345 , 2.223 , 220.39
Intermediate point  , 0.414 , 1.910 , 188.90
					, 0.483 , 1.622 , 144.82
					, 0.552 , 1.353 , 100.75
Coupling point      , 0.621 , 1.101 , 56.67
Overrun             , 0.690 , 0.942 , 0.00
Drag                , 3.450 , 1.304 , -1511.22
		*/
        [TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestGenericATPowerSplitTCData()
		{
			var maxEngineTorque = 1120.SI<NewtonMeter>();
			var engineRatedSpeed = 2200.RPMtoRad();
			var ratioFirstGear = 1.4492754;

			var genericTCData =
				GenericTorqueConverterData.CreateGenericTorqueConverterCharacteristicsPowersplit(ratioFirstGear,
					engineRatedSpeed, maxEngineTorque);

			var tcData = TorqueConverterDataReader.Create(
				genericTCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratioFirstGear,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);

			// stall point
			var idx = 0;
            Assert.AreEqual(0.0, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(5.507, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(377.80, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			idx = 1;
			Assert.AreEqual(0.069, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(4.039, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(346.32, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);


			idx = 4;
			Assert.AreEqual(0.276, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(2.570, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(251.87, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);
			
			// intermediate point
			idx = 6;
			Assert.AreEqual(0.414, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.910, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(188.90, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			idx = 7;
			Assert.AreEqual(0.483, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.622, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(144.82, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			idx = 8;
			Assert.AreEqual(0.552, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.353, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(100.75, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

            // coupling point
            idx = 9;
			Assert.AreEqual(0.621, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(1.101, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(56.67, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);

			// overrun point
			idx = 10;
			Assert.AreEqual(0.690, tcData.TorqueConverterEntries[idx].SpeedRatio, 1e-3);
			Assert.AreEqual(0.942, tcData.TorqueConverterEntries[idx].TorqueRatio, 1e-2);
			Assert.AreEqual(0.0, tcData.TorqueConverterEntries[idx].Torque.Value(), 1e-3);
        }
    }
}