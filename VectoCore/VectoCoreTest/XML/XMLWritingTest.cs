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

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	//[Parallelizable(ParallelScope.All)]
	public class XMLWritingTests
	{
		const string EngineOnlyJob = @"TestData/XML/EngineOnlyJob/EngineOnly.vecto";
		const string EngineeringJob = @"TestData/XML/EngineeringJob/Class5_Tractor_4x2/Class5_Tractor_ENG.vecto";

		const string EngineeringJobFull =
			@"TestData/XML/XMLWriter/EngineeringJob/Class5_Tractor_4x2/Class5_Tractor_ENG-FULL.vecto";

		const string DeclarationJob = @"TestData/XML/XMLWriter/DeclarationJob/Class5_Tractor_4x2/Class5_Tractor_DECL.vecto";

		const string DeclarationJobFull =
			@"TestData/XML/XMLWriter/DeclarationJob/Class5_Tractor_4x2/Class5_Tractor_DECL-FULL.vecto";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

	}
}
