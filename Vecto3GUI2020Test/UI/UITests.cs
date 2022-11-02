using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using Vecto3GUI2020Test.UI;

namespace Vecto3GUI2020Test
{/// <summary>
/// The Application under Test has to run on a Windows 10 Machine on a Desktop called "VE
/// </summary>
/// 
    [Ignore("GUI tests are not working that good")]
	[TestFixture]
    public class UITests : VECTO3GUI2020Session
    {
		[SetUp]
		public void Setup()
		{
            Setup(TestContext.CurrentContext);
		}

		[Ignore("ignored UI test")]
		[Test]
		public void LoadFileSession()
		{
			session.FindElementByXPath(
				"//Button[@Name=\"New Multistage File\"][@AutomationId=\"JobListViewNewManufacturingStageFileButton\"]").Click(); //open new multistageWindow

			
			session.SwitchTo().Window(session.WindowHandles.Last());
			Assert.AreEqual(2, session.WindowHandles.Count);

			
			session.FindElementByXPath(
				".//Custom[@AutomationId=\"NewMultistageJobView\"]/Custom[@AutomationId=\"NewMultistageFilePicker\"]/Button[@AutomationId=\"button\"]").Click();

			

			Assert.AreEqual(3, session.WindowHandles);
			


			Thread.Sleep(100000);
		}  




		#region DesktopSessionTests

		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFile()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_multiple_stages.xml");
		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFileAirdrag()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_multiple_stages_airdrag.xml");

		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFileheatPump()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_multiple_stages_heatPump.xml");

		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFilehev()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_multiple_stages_hev.xml");

		//}

		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFileNGTankSystem()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_multiple_stages_NGTankSystem.xml");
		//}

		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFileConsolidatedOneStage()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_one_stage.xml");
		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFileConsolidatedTwoStages()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_two_stages.xml");
		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFilePrimaryVehicleOnly()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_primary_vehicle_only.xml");
		//}

		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFilePrimaryVehicleStage_2_3()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_consolidated_one_stage.xml");
		//}
		//[Ignore("disabled Desktop Session tests")]
		//[Test]
		//public void LoadFilePrimaryVehicleOnlyAndCheckFields()
		//{
		//	LoadFileThroughUIWithDesktopSession(fileName: "vecto_multistage_primary_vehicle_only.xml");
		//	//Check vehicle fields
		//	SelectVehicleTab();
		//	focusToActiveWindow();
		//	//var element = session.FindElementByXPath()
		//}

		//private void LoadFileThroughUIWithDesktopSession(string fileName)
		//{
		//	// LeftClick on Button "New Multistage File" at (65,28)
		//	// LeftClick on Button "New Multistage File" at (87,25)
		//	Console.WriteLine("LeftClick on Button \"New Multistage File\" at (87,25)");
		//	string xpath_LeftClickButtonNewMultist_87_25 = "/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][@Name=\"Vecto\"]/Custom[@ClassName=\"JobListView\"]/Button[@Name=\"New Multistage File\"][@AutomationId=\"JobListViewNewManufacturingStageFileButton\"]";
		//	var winElem_LeftClickButtonNewMultist_87_25 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonNewMultist_87_25);
			
		//	if (winElem_LeftClickButtonNewMultist_87_25 != null)
		//	{
		//		winElem_LeftClickButtonNewMultist_87_25.Click();
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonNewMultist_87_25}");
		//	}
		//	Assert.NotNull(winElem_LeftClickButtonNewMultist_87_25);

		//	// LeftDblClick on Button "" at (11,11)
		//	Console.WriteLine("LeftDblClick on Button \"\" at (11,11)");
		//	string xpath_LeftDblClickButton_11_11 = "/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][starts-with(@Name,\"VECTO3GUI2020.ViewModel.MultiStage.Implementation.NewMultiStageJ\")]/Custom[@AutomationId=\"NewMultistageJobView\"]/Custom[@AutomationId=\"NewMultistageFilePicker\"]/Button[@AutomationId=\"button\"]";
		//	var winElem_LeftDblClickButton_11_11 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickButton_11_11);
			
		//	if (winElem_LeftDblClickButton_11_11 != null) {
		//		desktopSession.DesktopSessionElement.Mouse.MouseMove(winElem_LeftDblClickButton_11_11.Coordinates);
		//		desktopSession.DesktopSessionElement.Mouse.DoubleClick(null);
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickButton_11_11}");

		//	}
		//	Assert.NotNull(winElem_LeftDblClickButton_11_11);

		//	// LeftDblClick on Edit "Name" at (284,8)
		//	Console.WriteLine("LeftDblClick on Edit \"Name\" at (284,8)");
		//	string xpath_LeftDblClickEditName_284_8 = $"/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][starts-with(@Name,\"VECTO3GUI2020.ViewModel.MultiStage.Implementation.NewMultiStageJ\")]/Window[@ClassName=\"#32770\"][@Name=\"Öffnen\"]/Pane[@ClassName=\"DUIViewWndClassName\"]/Pane[@Name=\"Shellordneransicht\"][@AutomationId=\"listview\"]/List[@ClassName=\"UIItemsView\"][@Name=\"Elementansicht\"]/ListItem[@ClassName=\"UIItem\"][@Name={fileName}]/Edit[@Name=\"Name\"][@AutomationId=\"System.ItemNameDisplay\"]";
		//	var winElem_LeftDblClickEditName_284_8 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftDblClickEditName_284_8);
		//	if (winElem_LeftDblClickEditName_284_8 != null)
		//	{
		//		desktopSession.DesktopSessionElement.Mouse.MouseMove(winElem_LeftDblClickEditName_284_8.Coordinates);
		//		desktopSession.DesktopSessionElement.Mouse.DoubleClick(null);
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftDblClickEditName_284_8}");
		//	}
		//	Assert.NotNull(winElem_LeftDblClickEditName_284_8);



		//	SelectAirdragTab();
		//	SelectAuxiliariesTab();
		//	SelectVehicleTab();
		//	SelectAuxiliariesTab();
		//	SelectAirdragTab();

			



		//}


		//private void SelectVehicleTab()
		//{
		//	// LeftClick on Text "Vehicle" at (13,14)
		//	Console.WriteLine("LeftClick on Text \"Vehicle\" at (13,14)");
		//	string xpath_LeftClickTextVehicle_13_14 = "/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][starts-with(@Name,\"VECTO3GUI2020.ViewModel.MultiStage.Implementation.NewMultiStageJ\")]/Custom[@AutomationId=\"NewMultistageJobView\"]/Custom[@ClassName=\"MultiStageView\"]/Custom[@ClassName=\"ManufacturingStageView\"]/Button[@ClassName=\"Button\"][@Name=\"Vehicle\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Vehicle\"]";
		//	var winElem_LeftClickTextVehicle_13_14 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickTextVehicle_13_14);
		//	if (winElem_LeftClickTextVehicle_13_14 != null)
		//	{
		//		winElem_LeftClickTextVehicle_13_14.Click();
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickTextVehicle_13_14}");
		//	}
		//	Assert.NotNull(winElem_LeftClickTextVehicle_13_14);
		//}

		//private void SelectAirdragTab()
		//{
		//	// LeftClick on Text "Airdrag" at (21,13)
		//	Console.WriteLine("LeftClick on Text \"Airdrag\" at (21,13)");
		//	string xpath_LeftClickTextAirdrag_21_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][starts-with(@Name,\"VECTO3GUI2020.ViewModel.MultiStage.Implementation.NewMultiStageJ\")]/Custom[@AutomationId=\"NewMultistageJobView\"]/Custom[@ClassName=\"MultiStageView\"]/Custom[@ClassName=\"ManufacturingStageView\"]/Button[@ClassName=\"Button\"][@Name=\"Airdrag\"]/Text[@ClassName=\"TextBlock\"][@Name=\"Airdrag\"]";
		//	var winElem_LeftClickTextAirdrag_21_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickTextAirdrag_21_13);
		//	if (winElem_LeftClickTextAirdrag_21_13 != null)
		//	{
		//		winElem_LeftClickTextAirdrag_21_13.Click();
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickTextAirdrag_21_13}");
		//		return;
		//	}
		//	Assert.NotNull(winElem_LeftClickTextAirdrag_21_13);
		//}

		//private void SelectAuxiliariesTab()
		//{
		//	// LeftClick on Button "Auxiliaries" at (9,13)
		//	Console.WriteLine("LeftClick on Button \"Auxiliaries\" at (9,13)");
		//	string xpath_LeftClickButtonAuxiliarie_9_13 = "/Pane[@ClassName=\"#32769\"][@Name=\"Vecto\"]/Window[@ClassName=\"Window\"][starts-with(@Name,\"VECTO3GUI2020.ViewModel.MultiStage.Implementation.NewMultiStageJ\")]/Custom[@AutomationId=\"NewMultistageJobView\"]/Custom[@ClassName=\"MultiStageView\"]/Custom[@ClassName=\"ManufacturingStageView\"]/Button[@ClassName=\"Button\"][@Name=\"Auxiliaries\"]";
		//	var winElem_LeftClickButtonAuxiliarie_9_13 = desktopSession.FindElementByAbsoluteXPath(xpath_LeftClickButtonAuxiliarie_9_13);
		//	if (winElem_LeftClickButtonAuxiliarie_9_13 != null)
		//	{
		//		winElem_LeftClickButtonAuxiliarie_9_13.Click();
		//	}
		//	else
		//	{
		//		Console.WriteLine($"Failed to find element using xpath: {xpath_LeftClickButtonAuxiliarie_9_13}");
		//	}
		//	Assert.NotNull(winElem_LeftClickButtonAuxiliarie_9_13);


		//}


		#endregion


		[TearDown]
		public void OneTimeTeardown()
		{
			TearDown();
			Thread.Sleep(0);
		}



		private void focusToActiveWindow()
		{
			session.SwitchTo().ActiveElement();

		}



    }
}
