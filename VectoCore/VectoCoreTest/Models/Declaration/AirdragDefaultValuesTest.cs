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
using TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class AirdragDefaultValuesTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestClass2()
		{
			var file = @"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto";
			var fileNoAirdrag =
				@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_NoAirdrag_DECL.vecto";

			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(fileNoAirdrag);
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(4.83, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());

			Assert.AreEqual(7.2, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}

		[TestCase]
		public void TestClass5()
		{
			var file = @"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";
			var fileNoAirdrag = @"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_NoAirdrag_DECL.vecto";

			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(fileNoAirdrag);
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(5.3, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());
			Assert.AreEqual(8.7, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}

		[TestCase]
		public void TestClass9()
		{
			var file = @"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";
			var fileNoAirdrag =
				@"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_NoAirdrag_DECL.vecto";

			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(fileNoAirdrag);
			var runDataOrig = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(5.2, runDataOrig[0].AirdragData.DeclaredAirdragArea.Value());
			Assert.AreEqual(8.5, runData[0].AirdragData.DeclaredAirdragArea.Value());
		}
	}
}
