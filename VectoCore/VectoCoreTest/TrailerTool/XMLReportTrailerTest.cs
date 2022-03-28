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
*   Vasileios kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Ninject;
using NLog.LayoutRenderers;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML
{
	public class XMLReportTrailerTest
	{
		private const string MinimalTrailer = @"TestData\Trailer\2_Axle_DB_drybox.xml";
		private const string TrailerWithCertifiedAeroDevice = @"TestData\Trailer\3_axle_DA_drybox.xml";
		private const string OldTyreXml = @"TestData\Trailer\old_tyre.xml";
		private const string TrailerWithStandardAeroDevice =
			@"TestData\Trailer\2_Axle_DB_drybox_standard_aeroDevice.xml";

		protected IXMLInputDataReader XMLInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			XMLInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(MinimalTrailer, TestName = "Minimal")]
		[TestCase(TrailerWithCertifiedAeroDevice, TestName="CertifiedAeroDevice")]
		[TestCase(TrailerWithStandardAeroDevice, TestName = "StandardAeroDevice")]
		public void RunTrailerDeclarationJob(string fileName)
		{
			var fileWriter = new FileOutputWriter(fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = XMLInputReader.CreateDeclarationTrailer(XmlReader.Create(fileName));
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = false,
				ActualModalData = false,
				Validate = false,
			};
			try {
				jobContainer.AddRuns(runsFactory);
				jobContainer.Execute();
				jobContainer.WaitFinished();
			} catch (Exception ex) {
				Assert.IsTrue(false, ex.Message);
			}


			var customerRecord = fileWriter.XMLCustomerReportName;
			var manufacturerRecord = fileWriter.XMLFullReportName;

			var validationMsg1 = new List<string> {customerRecord} ;

			//XML Schema validation
			var validator1 = new XMLValidator(XmlReader.Create(customerRecord), validationErrorAction: (s,e) => {
				validationMsg1.Add(e?.ValidationEventArgs?.Message ?? "no schema found?");
			});
			Assert.IsTrue(validator1.ValidateXML(XmlDocumentType.CustomerReport), string.Join("\n", validationMsg1));

			var validationMsg2 = new List<string> {manufacturerRecord};
			var validator2 = new XMLValidator(XmlReader.Create(manufacturerRecord), validationErrorAction: (s,e) => {
				validationMsg2.Add(e?.ValidationEventArgs?.Message ?? "no schema found");
			});
			Assert.IsTrue(validator2.ValidateXML(XmlDocumentType.ManufacturerReport), string.Join("\n", validationMsg2));
		}
	}
}