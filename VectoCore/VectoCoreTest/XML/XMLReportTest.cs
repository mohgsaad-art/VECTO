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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLReportTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";
		const string SampleVehicleDeclTqLimits = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_torqueLimits.xml";
		const string SampleVehicleDeclLNG = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_LNG.xml";

		const string SampleVehicleDeclAT = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_AT.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(SampleVehicleDecl),
		TestCase(SampleVehicleDeclTqLimits),
		TestCase(SampleVehicleDeclAT),
		TestCase(SampleVehicleDeclLNG)]
		public void RunDeclarationJob(string filename)
		{
			var fileWriter = new FileOutputWriter(filename);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = false;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var customerRecord = fileWriter.XMLCustomerReportName;
			var manufacturerRecord = fileWriter.XMLFullReportName;

			var validationMsg1 = new List<string> {customerRecord} ;

			var validator1 = new XMLValidator(XmlReader.Create(customerRecord), validationErrorAction: (s,e) => {
				validationMsg1.Add(e?.ValidationEventArgs?.Message ?? "no schema found?");
			});
			Assert.IsTrue(validator1.ValidateXML(XmlDocumentType.CustomerReport), validationMsg1.Join("\n"));

			var validationMsg2 = new List<string> {manufacturerRecord};
			var validator2 = new XMLValidator(XmlReader.Create(manufacturerRecord), validationErrorAction: (s,e) => {
				validationMsg2.Add(e?.ValidationEventArgs?.Message ?? "no schema found");
			});
			Assert.IsTrue(validator2.ValidateXML(XmlDocumentType.ManufacturerReport), validationMsg2.Join("\n"));
		}
	}
}