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

using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class EngineInputDataTests
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

		[TestCase(null, 1.0, 44.959295, TestName = "Engine CF - NONE"),
		TestCase("CFRegPer", 1.2, 44.959295 * 1.2, TestName = "Engine CF - CFRegPer"),
		TestCase("BFColdHot", 1.2, 44.959295 * 1.2, TestName = "Engine CF - BFColdHod"),
		TestCase("CFNCV", 1.2, 44.959295, TestName = "Engine CF - CFNCV")  // has no influence - only for documentation purpose
			]
		public void TestEngineCorrectionFactors(string correctionFactor, double value, double expectedFc)
		{
			var reader = XmlReader.Create(SampleVehicleDecl);
			var modified = reader;
			if (correctionFactor != null) {
				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var firstAxle = nav.SelectSingleNode("//"+
					helper.Query(
						//helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
						correctionFactor
					), manager);
				firstAxle.InnerXml = value.ToXMLFormat(4);

				 modified = XmlReader.Create(new StringReader(nav.OuterXml));

			}
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null, validate: false);
			var first = factory.SimulationRuns().ToArray().First();

			var modData = ((ModalDataContainer)first.GetContainer().ModalData).Data;
			first.Run();

			Assert.AreEqual(expectedFc, modData.Sum(r => ((SI)r[ModalResultField.FCFinal.GetName()]).Value()), 1e-3);
		}
	}
}
