using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class MultistageAuxiliariesViewModelTests : ViewModelTestBase
	{

		[Test]
        public void TestAllowedValuesHeatPumpTypeDriver()
        {
            var auxVm = new MultistageAuxiliariesViewModel(null);
			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration6;
			Assert.AreEqual(auxVm.HeatPumpTypeDriverAllowedValues.Count,1);
            Assert.Contains(HeatPumpType.not_applicable, auxVm.HeatPumpTypeDriverAllowedValues);

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration3;
            Assert.IsFalse(auxVm.HeatPumpTypeDriverAllowedValues.Contains(HeatPumpType.not_applicable));

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration10;
			Assert.AreEqual(auxVm.HeatPumpTypeDriverAllowedValues.Count, 1);
			Assert.Contains(HeatPumpType.not_applicable, auxVm.HeatPumpTypeDriverAllowedValues);

			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration2;
			Assert.IsFalse(auxVm.HeatPumpTypeDriverAllowedValues.Contains(HeatPumpType.not_applicable));
		}


        [Test]
        public void TestEnumParameters()
        {
            var auxVm = new MultistageAuxiliariesViewModel(null);
            auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.none;
            Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);
            auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.R_744;
            Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

            auxVm.HeatPumpGroupEditingEnabled = false;
            auxVm.ParameterViewModels[nameof(auxVm.HeatPumpTypeCoolingDriverCompartment)].CurrentContent =
                HeatPumpType.R_744;
            Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);

            auxVm.ParameterViewModels[nameof(auxVm.HeatPumpTypeCoolingDriverCompartment)].CurrentContent =
                HeatPumpType.none;


            Assert.IsTrue(auxVm.HeatPumpGroupEditingEnabled);
		}
	}
}
