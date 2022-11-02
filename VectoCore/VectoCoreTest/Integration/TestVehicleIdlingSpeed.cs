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
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class TestVehicleIdlingSpeed
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

		[TestCase, Category("LongRunning")]
		public void VehicleIdlingSpeedTest()
		{
			var VehicleEngineIdleSpeed = 900.RPMtoRad();

			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var idlingSpeed = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_IdlingSpeed), manager);
			idlingSpeed.SetValue(VehicleEngineIdleSpeed.AsRPM.ToXMLFormat(0));

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, new FileOutputWriter("Idle900"));
			//factory.WriteModalResults = true;

			var jobContainer = new JobContainer(null);
			jobContainer.AddRuns(factory);

			var runIdx = 0;
			var run = jobContainer.Runs[runIdx];
			var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
			var modData = modContainer.Data;

			run.Run.Run();

			modContainer.Data = modData;
			Assert.IsTrue(modContainer.Min<PerSecond>(ModalResultField.n_ice_avg).IsGreaterOrEqual(VehicleEngineIdleSpeed));


			//jobContainer.Execute();
			//jobContainer.WaitFinished();
		}
	}
}
