using System;
using System.IO;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Utils
{
	public static class CompletedVIF
	{
        public static string CreateCompletedVifXML(
			JSONInputDataCompletedBusFactorMethodV7 completedJson,
			IXMLInputDataReader xmlInputReader)
		{
			var vifDataProvider = xmlInputReader.Create(completedJson.PrimaryInputDataFile);
			var completeDataProvider = xmlInputReader.CreateDeclaration(completedJson.CompletedInputDataFile);
			var inputDataAsm = new XMLDeclarationVIFInputData(
				vifDataProvider as IMultistepBusInputDataProvider, completeDataProvider.JobInputData.Vehicle);

			var filename = Guid.NewGuid().ToString().Substring(0, 20);
			var writerAsm = new FileOutputVIFWriter(filename, 0);

            var _kernel = new StandardKernel(new VectoNinjectModule());
            var factoryAsm = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputDataAsm, writerAsm, null, null, false);
            var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRuns(factoryAsm);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var completedVifXML = new XmlDocument();
			completedVifXML.Load(writerAsm.XMLMultistageReportFileName);
			var retVal = completedVifXML.OuterXml;
			File.Delete(writerAsm.XMLMultistageReportFileName);
			return retVal;
		}

		public static XMLDeclarationVIFInputData CreateCompletedVif(JSONInputDataCompletedBusFactorMethodV7 completedJson,
			IXMLInputDataReader xmlInputReader)
		{
			var completedVif =
				xmlInputReader.CreateDeclaration(XmlReader.Create(new StringReader(CreateCompletedVifXML(completedJson, xmlInputReader))));
			
			return new XMLDeclarationVIFInputData(completedVif as IMultistepBusInputDataProvider, null);
			}
	}
}