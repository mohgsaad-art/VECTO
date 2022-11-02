/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DeclarationSimulationFactoryTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase("None", RetarderType.None),
		TestCase("Losses included in Gearbox", RetarderType.LossesIncludedInTransmission),
		TestCase("Engine Retarder", RetarderType.EngineRetarder),
		TestCase("Transmission Input Retarder", RetarderType.TransmissionInputRetarder),
		TestCase("Transmission Output Retarder", RetarderType.TransmissionOutputRetarder)]
		public void TestRetarderTypes(string xmlValue, RetarderType retarderType)
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderType), manager);
			xmlRetarderType.SetValue(xmlValue);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);
			factory.Validate = false;

			var runs = factory.SimulationRuns().ToArray();
			Assert.AreEqual(10, runs.Length);
		}

		[TestCase()]
		public void TestEngineCorrectionFactor()
		{
			var inputDataProvider = xmlInputReader.CreateDeclaration(SampleVehicleDecl);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);
			factory.Validate = false;

			var runs = factory.SimulationRuns().ToArray();

			var expected = new[] { 1.02, 1.02, 1.02, 1.02, 1.013299, 1.013299, 1.013299, 1.013299 };

			for (var i = 0; i < 8; i++)
				Assert.AreEqual(
					expected[i], runs[i].GetContainer().RunData.EngineData.Fuels.First().FuelConsumptionCorrectionFactor, 1e-6,
					"correction factor for cycle {0} payload {1} mismatch ({2})", runs[i].CycleName , runs[i].RunSuffix, i);
		}
	}
}
