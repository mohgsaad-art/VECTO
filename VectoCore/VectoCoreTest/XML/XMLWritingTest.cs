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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData.XML;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	//[Parallelizable(ParallelScope.All)]
	public class XMLWritingTests
	{
		const string EngineOnlyJob = @"TestData\XML\EngineOnlyJob\EngineOnly.vecto";
		const string EngineeringJob = @"TestData\XML\EngineeringJob\Class5_Tractor_4x2\Class5_Tractor_ENG.vecto";

		const string EngineeringJobFull =
			@"TestData\XML\XMLWriter\EngineeringJob\Class5_Tractor_4x2\Class5_Tractor_ENG-FULL.vecto";

		const string DeclarationJob = @"TestData\XML\XMLWriter\DeclarationJob\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";

		const string DeclarationJobFull =
			@"TestData\XML\XMLWriter\DeclarationJob\Class5_Tractor_4x2\Class5_Tractor_DECL-FULL.vecto";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestWriteEngineOnlySingleFile()
		{
			var outFile = "EngineOnlyJobSingleFile.xml";

			if (File.Exists(outFile)) {
				File.Delete(outFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineOnlyJob);
			var job = _kernel.Get<IXMLEngineeringWriter>().Write(inputData);
			job.Save(outFile);

			//var reader = XmlReader.Create(outFile);
			var xml = xmlInputReader.CreateEngineering(outFile);

			Assert.IsNotNull(xml);
			Assert.AreEqual("175kW 6.8l Engine", xml.JobInputData.JobName);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestWriteEngineeringSingleFile()
		{
			var outFile = "EngineeringJobSingleFile.xml";

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJob);
			var job = _kernel.Get<IXMLEngineeringWriter>().Write(inputData);
			job.Save(outFile);

			var xml = xmlInputReader.CreateEngineering(outFile);

			Assert.IsNotNull(xml);
			Assert.AreEqual("N/A N/A", xml.JobInputData.JobName);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestWriteEngineeringSingleFileFull()
		{
			var outFile = "EngineeringJobSingleFileFull.xml";

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFull);
			var job = _kernel.Get<IXMLEngineeringWriter>().Write(inputData);
			job.Save(outFile);

			var xml = xmlInputReader.CreateEngineering(outFile);

			Assert.IsNotNull(xml);
			Assert.AreEqual("N/A N/A", xml.JobInputData.JobName);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestWriteEngineeringMultipleFilesFull()
		{
			var outFile = "EngineeringJobMultipleFilesFull.xml";
			var outDir = "Engineering_MultipleFiles_Full";
			Directory.CreateDirectory(outDir);

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFull);
			var job = _kernel.Get<IXMLEngineeringWriter>().Write(inputData);
			job.Save(Path.Combine(outDir, outFile));

			var xml = xmlInputReader.CreateEngineering(Path.Combine(outDir, outFile));

			Assert.IsNotNull(xml);
			Assert.AreEqual("N/A N/A", xml.JobInputData.JobName);
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestWriteEngineeringMultipleFiles()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJob);
			Directory.CreateDirectory("Engineering_MultipleFiles");
			var job = _kernel.Get<IXMLEngineeringWriter>().Write(inputData);
			job.Save("Engineering_MultipleFiles/EngineeringJobMultipleFiles.xml");

			//var xml = new XMLEngineeringInputDataProvider(outFile, true);

			//Assert.IsNotNull(xml);
			//Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestCase]
		public void TestWriteDeclarationJob()
		{
			var outputFile = "DeclarationJobSingleFile.xml";

			if (File.Exists(outputFile)) {
				File.Delete(outputFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJob);

			var job = new XMLDeclarationWriter("TUG_IVT").GenerateVectoJob((IDeclarationInputDataProvider)inputData);

			job.Save(outputFile);

			var reader = XmlReader.Create(outputFile);
			var xml = xmlInputReader.CreateDeclaration(reader);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-NA", xml.JobInputData.JobName);
		}

		[TestCase]
		public void TestWriteDeclarationJobFull()
		{
			var outputFile = "DeclarationJobFullSingleFile.xml";

			if (File.Exists(outputFile)) {
				File.Delete(outputFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJobFull);

			var job = new XMLDeclarationWriter("TUG_IVT").GenerateVectoJob((IDeclarationInputDataProvider)inputData);

			job.Save(outputFile);

			var reader = XmlReader.Create(outputFile);
			var xml = xmlInputReader.CreateDeclaration(reader);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-NA", xml.JobInputData.JobName);
		}
	}
}
