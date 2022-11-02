using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
    public abstract class XMLEngineWriter : IXMLComponentWriter
	{
		protected XElement _xElement;
		protected XNamespace _defaultNameSpace;
		protected readonly IEngineDeclarationInputData _inputData;
		protected string id = "toDo-Add-Identifier";

		public XMLEngineWriter(IEngineDeclarationInputData inputData)
		{
			_inputData = inputData;
		}

		public virtual XElement GetElement()
		{
			if (_xElement == null) {
				Initialize();
				CreateDataElements();
				_xElement.Add(this.CreateSignatureElement(_defaultNameSpace, id, _inputData.DigestValue));
			}

			return _xElement;
		}

		protected abstract void Initialize();
		protected abstract void CreateDataElements();


	}

	public class XMLEngineWriter_v1_0 : XMLEngineWriter
	{

		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(EngineViewModel_v1_0).ToString()
		};


		public override XElement GetElement()
		{
			throw new NotImplementedException();
		}

		protected override void Initialize()
		{
			throw new NotImplementedException();
		}

		protected override void CreateDataElements()
		{
			throw new NotImplementedException();
		}

		public XMLEngineWriter_v1_0(IEngineDeclarationInputData inputData) : base(inputData) { }
	}

	public class XMLEngineWriter_v2_0 : XMLEngineWriter
	{
		private static XNamespace _v20 = XML.XMLNamespaces.V20;
		public static readonly string[] SUPPORTED_VERSIONS = {
			typeof(EngineViewModel_v2_0).ToString(),
		};

		protected override void Initialize()
		{
			_defaultNameSpace = _v20;
			_xElement = new XElement(_defaultNameSpace + XMLNames.Component_Engine);
		}

		protected override void CreateDataElements()
		{
			var _dataXElement = new XElement(_defaultNameSpace + XMLNames.ComponentDataWrapper);
			_xElement.Add(_dataXElement);
			_dataXElement.Add(new XAttribute(XMLNames.Component_ID_Attr, id));
			_dataXElement.Add(new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.Engine_Type_Attr));
			
			Debug.Assert(_xElement != null);
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Model, _inputData.Model));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_CertificationNumber,
				_inputData.CertificationNumber));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_Date, _inputData.Date));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_Displacement, _inputData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_IdlingSpeed, "100"));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_RatedSpeed, _inputData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_RatedPower, _inputData.RatedPowerDeclared.ToXMLFormat(0)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_MaxTorque, _inputData.MaxTorqueDeclared.ToXMLFormat(0)));

			Debug.Assert(_inputData.EngineModes.Count == 1, "Only 1 Engine Mode supported in XMLEngineWriter_v2_0");
			var mode = _inputData.EngineModes[0];

			Debug.Assert(mode.Fuels.Count == 1, "Only one fuel supported in XMLEngineWriter_v2_0");
			var fuel = mode.Fuels[0];

			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_WHTCUrban, fuel.WHTCUrban.ToXMLFormat(4)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_WHTCRural, fuel.WHTCRural.ToXMLFormat(4)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_WHTCMotorway, fuel.WHTCMotorway.ToXMLFormat(4)));

			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_ColdHotBalancingFactor, fuel.ColdHotBalancingFactor.ToXMLFormat(4)));
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_CorrectionFactor_RegPer, fuel.CorrectionFactorRegPer.ToXMLFormat(4)));

			
			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_CorrecionFactor_NCV, "1.0000"));

			_dataXElement.Add(new XElement(_defaultNameSpace + XMLNames.Engine_FuelType, fuel.FuelType.ToXMLFormat()));

			var fuelConsumptionMapElement = new XElement(_defaultNameSpace + XMLNames.Engine_FuelConsumptionMap);
			var tableData = fuel.FuelConsumptionMap;

			foreach (DataRow fuelEntry in tableData.Rows) {
				var entry = new XElement(_defaultNameSpace + XMLNames.Engine_FuelConsumptionMap_Entry);
				entry.Add(new XAttribute(XMLNames.Engine_FuelConsumptionMap_EngineSpeed_Attr, fuelEntry.ItemArray[0]));
				entry.Add(new XAttribute(XMLNames.Engine_FuelConsumptionMap_Torque_Attr, fuelEntry.ItemArray[1]));
				entry.Add(new XAttribute(XMLNames.Engine_FuelConsumptionMap_FuelConsumption_Attr, fuelEntry.ItemArray[2]));
				fuelConsumptionMapElement.Add(entry);
			}
			_dataXElement.Add(fuelConsumptionMapElement);

			var fullLoadAndDragCurveElement = new XElement(_defaultNameSpace + XMLNames.Engine_FullLoadAndDragCurve);
			tableData = mode.FullLoadCurve;
			foreach (DataRow loadAndDragEntry in tableData.Rows) {
				var entry = new XElement(_defaultNameSpace + XMLNames.Engine_FullLoadCurve_Entry);
				entry.Add(new XAttribute(XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr, loadAndDragEntry.ItemArray[0]));
				entry.Add(new XAttribute(XMLNames.Engine_FullLoadCurve_MaxTorque_Attr, loadAndDragEntry.ItemArray[1]));
				entry.Add(new XAttribute(XMLNames.Engine_FullLoadCurve_DragTorque_Attr, loadAndDragEntry.ItemArray[2]));
				fullLoadAndDragCurveElement.Add(entry);
			}
			_dataXElement.Add(fullLoadAndDragCurveElement);
		}



		public XMLEngineWriter_v2_0(IEngineDeclarationInputData inputData) : base(inputData)
		{
			
		}
	}
}
