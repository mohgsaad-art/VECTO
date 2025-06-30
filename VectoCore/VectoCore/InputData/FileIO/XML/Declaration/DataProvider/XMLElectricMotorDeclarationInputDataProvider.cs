using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public abstract class XMLCommonElectricMotorDeclarationInputData : AbstractCommonComponentType, IComponentInputData,
		IElectricMotorVoltageLevel
	{

		protected XMLCommonElectricMotorDeclarationInputData(XmlNode node, string source) : base(node, source)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IPowerRatingInputData

		public virtual ElectricMachineType ElectricMachineType =>
			GetString(XMLNames.ElectricMachine_ElectricMachineType).ParseEnum<ElectricMachineType>();

		public virtual Watt R85RatedPower =>
			GetDouble(XMLNames.ElectricMachine_R85RatedPower).SI<Watt>();

		public virtual KilogramSquareMeter Inertia =>
			GetDouble(XMLNames.ElectricMachine_RotationalInertia).SI<KilogramSquareMeter>();

		public virtual NewtonMeter ContinuousTorque =>
			GetDouble(XMLNames.ElectricMachine_ContinuousTorque).SI<NewtonMeter>();

		public virtual PerSecond ContinuousTorqueSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedContinuousTorque).RPMtoRad();

		public virtual NewtonMeter OverloadTorque =>
			GetDouble(XMLNames.ElectricMachine_OverloadTorque).SI<NewtonMeter>();

		public virtual PerSecond OverloadTestSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedOverloadTorque).RPMtoRad();

		public virtual Second OverloadTime =>
			GetDouble(XMLNames.ElectricMachine_OverloadDuration).SI<Second>();

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace { get; }
		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IElectricMotorVoltageLevel

		public virtual Volt VoltageLevel => ElementExists(XMLNames.VoltageLevel_Voltage) ? 
			GetDouble(XMLNames.VoltageLevel_Voltage).SI<Volt>() : null ;

		public virtual IList<IElectricMotorLoadCurve> FullLoadCurve => new List<IElectricMotorLoadCurve>() { 
			new ElectricMotorLoadCurve() {
				Gear = 0,
				LoadCurve = ReadFullLoadCurve()
            }
		};

		public virtual IList<IElectricMotorPowerMap> PowerMap => GetPowerMaps();

		#endregion

		protected virtual TableData ReadFullLoadCurve()
		{
			return ReadTableData(XMLNames.MaxTorqueCurve, XMLNames.MaxTorqueCurve_Entry, new Dictionary<string, string> {
				{ XMLNames.MaxTorqueCurve_OutShaftSpeed, XMLNames.MaxTorqueCurve_OutShaftSpeed },
				{ XMLNames.MaxTorqueCurve_MaxTorque, XMLNames.MaxTorqueCurve_MaxTorque },
				{ XMLNames.MaxTorqueCurve_MinTorque, XMLNames.MaxTorqueCurve_MinTorque }
			});
		}
		
		protected virtual TableData ReadDragCurve()
		{
			return ReadTableData(XMLNames.DragCurve, XMLNames.DragCurve_Entry, new Dictionary<string, string>
			{
				{XMLNames.DragCurve_OutShaftSpeed, XMLNames.DragCurve_OutShaftSpeed},
				{XMLNames.DragCurve_DragTorque, XMLNames.DragCurve_DragTorque}
			});
		}

		protected virtual TableData ReadConditioning()
		{
			return ReadTableData(XMLNames.Conditioning, XMLNames.Conditioning_Entry, new Dictionary<string, string> {
				{ XMLNames.Conditioning_CoolantTempInlet, XMLNames.Conditioning_CoolantTempInlet },
				{ XMLNames.Conditioning_CoolingPower, XMLNames.Conditioning_CoolingPower }
			});
		}
		
		protected virtual IList<IElectricMotorVoltageLevel> GetVoltageLevels()
		{
			var voltageLevelNodes = GetNodes(XMLNames.ElectricMachine_VoltageLevel, BaseNode);
			if (voltageLevelNodes is null || voltageLevelNodes.Count == 0) {
				return null;
			}


			var voltageLevels = new List<IElectricMotorVoltageLevel>();

			foreach (XmlNode voltageLevelNode in voltageLevelNodes) {
				voltageLevels.Add(new XMLElectricMotorIEPCIInputDataProviderV23(null, voltageLevelNode, null));
			}

			if (voltageLevels.Count > 1) {
				voltageLevels = voltageLevels.OrderBy(x => x.VoltageLevel.Value()).ToList();
			}

			return voltageLevels;
		}

		
		private IList<IElectricMotorPowerMap> GetPowerMaps()
		{
			var powerMapNodes = GetNodes(XMLNames.PowerMap);
			if (powerMapNodes is null || powerMapNodes.Count == 0) {
				return null;
			}


			var powerMaps = new List<IElectricMotorPowerMap>();
			foreach (XmlNode powerMapNode in powerMapNodes)
			{
				powerMaps.Add(new ElectricMotorPowerMap(powerMapNode));
			}

			return powerMaps;
		}

		private class ElectricMotorPowerMap : AbstractXMLType, IElectricMotorPowerMap
		{
			private static readonly Dictionary<string, string> _powerMapMapping = AttributeMappings.EMPowerMap;
			private static readonly string _elPowerCol = _powerMapMapping.FirstOrDefault(x => x.Value == XMLNames.PowerMap_ElectricPower).Key;

			public ElectricMotorPowerMap(XmlNode xmlNode) : base(xmlNode) { }
			
			#region Implementation of IElectricMotorPowerMap

			public virtual int Gear => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.PowerMap_Gear));
			public virtual TableData PowerMap => ReadPowerMap();

			private TableData ReadPowerMap()
			{
				var powerMapEntryNodes = GetNodes(XMLNames.PowerMap_Entry);
				var powerMap = XMLHelper.ReadTableData(_powerMapMapping, powerMapEntryNodes);
				return powerMap;
			}

			#endregion
		}
	}


	// ---------------------------------------------------------------------------------------


	public class XMLElectricMotorDeclarationInputDataProviderV23 : XMLCommonElectricMotorDeclarationInputData,
		IXMLElectricMotorDeclarationInputData
	{
		public  static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "ElectricMachineSystemMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricMotorVoltageLevel> _voltageLevels;

		public XMLElectricMotorDeclarationInputDataProviderV23(
			XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}
		
		#region Implementation of IElectricMotorDeclarationInputData
		
		public virtual bool DcDcConverterIncluded => GetBool(XMLNames.ElectricMachine_DcDcConverterIncluded);

		public virtual string IHPCType => GetString(XMLNames.ElectricMachine_IHPCType);

		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public virtual TableData DragCurve => ReadDragCurve();


		public virtual TableData Conditioning => ElementExists(XMLNames.Conditioning) 
			? ReadConditioning() : null;
		

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorSystemStandardDeclarationInputDataProviderV23 :
			XMLElectricMotorDeclarationInputDataProviderV23
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public new const string XSD_TYPE = "ElectricMachineSystemStandardValuesDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public XMLElectricMotorSystemStandardDeclarationInputDataProviderV23(XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile) { }


		#region Overrides of XMLElectricMotorDeclarationInputDataProviderV23

		public override TableData Conditioning => null;

		#endregion
	}
	
	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIhpcDeclarationInputDataProviderV23 : XMLElectricMotorDeclarationInputDataProviderV23
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public new const string XSD_TYPE = "ElectricMachineSystemIHPCMeasuredDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLElectricMotorIhpcDeclarationInputDataProviderV23(XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile) { }
	}
	
	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIEPCIInputDataProviderV23 : XMLCommonElectricMotorDeclarationInputData, IXMLIEPCInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public const string XSD_TYPE = "IEPCMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IXMLDeclarationVehicleData _vehicle;
        private IList<IElectricMotorVoltageLevel> _voltageLevels;
		private IList<IGearEntry> _gears;
		private IList<IDragCurve> _dragCurves;

		protected Watt _ratedPowerCalculated;

		public XMLElectricMotorIEPCIInputDataProviderV23(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			//SourceType = DataSourceType.XMLEmbedded;
			ValidateGearCount();
		}

		#region Overrides of AbstractCommonComponentType

		public override CertificationMethod CertificationMethod
		{
			get
			{
				var certMethod = GetString(XMLNames.Component_Gearbox_CertificationMethod, required: false) ??
								GetString(XMLNames.Component_CertificationMethod, required: false);

				if (certMethod != null) {
					switch (certMethod) {
						case "Measured for complete component":
						case "Measured for EM and standard values for other components":
							return CertificationMethod.Measured;
						case "Standard values for all components":
							return CertificationMethod.StandardValues;
					}
				}

				return certMethod != null
					? EnumHelper.ParseEnum<CertificationMethod>(certMethod)
					: CertificationMethod.Measured;
			}
		}

		#endregion


		#region Implementation of IIEPCDeclarationInputData

		public Watt TotalRatedPowerCalculated => _ratedPowerCalculated ?? (_ratedPowerCalculated = CalculateRatedPower());

		protected virtual Watt CalculateRatedPower()
		{
			var gearRatioUsedForMeasurement = Gears
				.Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
				.OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();
			var count = DesignTypeWheelMotor && NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;
			var maxPwr = 0.SI<Watt>();
			foreach (var entry in VoltageLevels.OrderBy(x => x.VoltageLevel).AsEnumerable()) {
				var maxTq = IEPCFullLoadCurveReader.Create(entry.FullLoadCurve.First().LoadCurve, count,
					gearRatioUsedForMeasurement.Ratio);
				if (maxTq.MaxPower > maxPwr) {
					maxPwr = maxTq.MaxPower;
				}
			}

			return maxPwr;
		}

		public virtual bool DifferentialIncluded => GetBool(XMLNames.IEPC_DifferentialIncluded);

		public virtual bool DesignTypeWheelMotor => GetBool(XMLNames.IEPC_DesignTypeWheelMotor);

		public virtual int? NrOfDesignTypeWheelMotorMeasured => ElementExists(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured) ?
			(int?)Convert.ToInt32(GetString(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured)) : null;

		public virtual IList<IGearEntry> Gears =>
			_gears ?? (_gears = GetGearEntries());
		
		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public IList<IDragCurve> DragCurves => _dragCurves ?? (_dragCurves = GetDragCurves());

		public virtual TableData Conditioning => ElementExists(XMLNames.Conditioning)
			? ReadConditioning() : null;

		public virtual bool DisengagementClutch => false;

        #endregion

        protected virtual void ValidateGearCount()
		{
			if (Gears == null)
				return;
			
			var currentGears = new Dictionary<int, bool>();
			foreach (var gear in Gears) {
				currentGears.Add(gear.GearNumber, false);
			}

			foreach (var voltageLevel in VoltageLevels) {
				foreach (var powerMap in voltageLevel.PowerMap) {
					if (powerMap.Gear <= 0) {
						continue;
					}

					if (currentGears.ContainsKey(powerMap.Gear)) {
						currentGears[powerMap.Gear] = true;
					} else {
						throw new ArgumentException(
							"The PowerMaps contains a gear which was not specified under gears");
					}
				}

				if (AnyMissingGear(currentGears)) {
					throw new ArgumentException("The PowerMaps contains a gear which was not specified under gears");
				}
			}

			foreach (var dragCurve in DragCurves) {
				if (dragCurve.Gear == null) {
					continue;
				}

				if (currentGears.ContainsKey((int)dragCurve.Gear)) {
					currentGears[(int)dragCurve.Gear] = true;
				} else {
					throw new ArgumentException("The DragCurve contains a gear which was not specified under gears");
				}
			}

			if (DragCurves.Count > 1 && AnyMissingGear(currentGears)) {
				throw new ArgumentException("The DragCurve contains a gear which was not specified under gears");
			}
		}

		private bool AnyMissingGear(Dictionary<int, bool> foundGears)
		{
			var keys = foundGears.Keys.ToList();
			foreach (var key in keys) {
				if(!foundGears[key])
					return true;
				foundGears[key] = false;

			}

			return false;
		}

		private IList<IDragCurve> GetDragCurves()
		{
			var dragCurveNodes = GetNodes(XMLNames.DragCurve, BaseNode);
			if (dragCurveNodes is null || dragCurveNodes.Count == 0)
				return null;

			var dragCurves = new List<IDragCurve>();
			foreach (XmlNode dragCurve in dragCurveNodes) {
				dragCurves.Add(new DragCurveEntry(dragCurve));
			}
			return dragCurves;
		}

		
		private IList<IGearEntry> GetGearEntries()
		{
			var gearNodes = GetNodes(XMLNames.Gear_EntryName);
			if (gearNodes is null || gearNodes.Count == 0)
				return null;

			var gears = new List<IGearEntry>();
			foreach (XmlNode gearNode in gearNodes) {
				gears.Add(new GearEntry(gearNode));
			}
			return gears;
		}

		public class GearEntry : AbstractXMLType, IGearEntry
		{
			public GearEntry(XmlNode node) : base(node) { }

			#region Implementation of IGearEntry

			public virtual int GearNumber => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.Gear_GearNumber_Attr));
			
			public virtual double Ratio => GetDouble(XMLNames.Gear_Ratio);

			public virtual NewtonMeter MaxOutputShaftTorque => ElementExists(XMLNames.Gear_MaxOutputShaftTorque)
				? GetDouble(XMLNames.Gear_MaxOutputShaftTorque).SI<NewtonMeter>()
				: null;

			public virtual PerSecond MaxOutputShaftSpeed => ElementExists(XMLNames.Gear_MaxOutputShaftSpeed)
				? GetDouble(XMLNames.Gear_MaxOutputShaftSpeed).RPMtoRad()
				: null;

			#endregion
		}
		
		public class DragCurveEntry : AbstractXMLType, IDragCurve
		{
			private Dictionary<string, string> dargCurveMapping = new Dictionary<string, string> {
				{XMLNames.DragCurve_OutShaftSpeed, XMLNames.DragCurve_OutShaftSpeed },
				{XMLNames.DragCurve_DragTorque, XMLNames.DragCurve_DragTorque }
			};
			
			public DragCurveEntry(XmlNode node) : base(node) { }

			#region Implementation of IDragCurve
			
			public int? Gear
			{
				get
				{
					var gear = GetAttribute(BaseNode, XMLNames.DragCurve_Gear);
					return gear != null ? Convert.ToInt32(gear) : (int?)null;
				}
			}

			public TableData DragCurve
			{
				get
				{
					var dragCurveEntryNodes = GetNodes(XMLNames.DragCurve_Entry);
					return XMLHelper.ReadTableData(dargCurveMapping, dragCurveEntryNodes);
				}
			}

			#endregion
		}



		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

    // ---------------------------------------------------------------------------------------

    public class XMLElectricMotorIEPCStandardInputDataProviderV23 : XMLElectricMotorIEPCIInputDataProviderV23
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;
		public new const string XSD_TYPE = "IEPCStandardValuesDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLElectricMotorIEPCStandardInputDataProviderV23(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }

		protected override void ValidateGearCount()
		{
			return;
		}
		
		#region Overrides of XMLElectricMotorIEPCIInputDataProviderV23

		public override TableData Conditioning => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIEPCInputDataProviderV26 : XMLElectricMotorIEPCIInputDataProviderV23
	{
		public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
		public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricMotorLoadCurve> _fullLoadCurves;

		public XMLElectricMotorIEPCInputDataProviderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile)
		{
			ValidateCurveTypeUniformityAcrossVoltageLevels();
        }

		public override bool DisengagementClutch => ElementExists(XMLNames.IEPC_DisengagementClutch) ? GetBool(XMLNames.IEPC_DisengagementClutch) : false;

		public override IList<IElectricMotorLoadCurve> FullLoadCurve => _fullLoadCurves ?? (_fullLoadCurves = GetFullLoadCurves());

		private void ValidateCurveTypeUniformityAcrossVoltageLevels()
		{
            if ((VoltageLevels == null) || (Gears == null))
            {
                return;
            }

			var levelsWithCurvesPerGear = VoltageLevels.Where(x => (x.FullLoadCurve.Count > 1) || (x.FullLoadCurve[0].Gear > 0)).Count();
			
			if ((levelsWithCurvesPerGear < VoltageLevels.Count) && (levelsWithCurvesPerGear > 0))
			{
                throw new ArgumentException($"Not all voltage levels have MaxTorqueCurve per gear.");
            }
        }

		protected override void ValidateGearCount()
		{
			base.ValidateGearCount();

			if ((VoltageLevels == null) || (Gears == null))
			{
				return;
			}

			foreach (var voltageLevel in VoltageLevels.Where(x => (x.FullLoadCurve.Count > 1) || (x.FullLoadCurve[0].Gear > 0)))
			{
                var duplicates = voltageLevel.FullLoadCurve
					.GroupBy(i => i.Gear)
					.Where(g => g.Count() > 1)
					.Select(g => g.Key);

				if (duplicates.Any())
				{
                    throw new ArgumentException($"Voltage level: {voltageLevel.VoltageLevel} contains MaxTorqueCurves with duplicate gears: {duplicates.Join()}.");
                }

                var allGears = Gears.Select(x => x.GearNumber);
				
				var validCurveGears = voltageLevel.FullLoadCurve.Select(x => x.Gear).Where(x => allGears.Contains(x));
				if (validCurveGears.Count() < allGears.Count())
				{
                    throw new ArgumentException($"Voltage level: {voltageLevel.VoltageLevel} does not contain MaxTorqueCurve for gears: {allGears.Except(validCurveGears).Join()}.");
                }

                var invalidCurveGears = voltageLevel.FullLoadCurve.Select(x => x.Gear).Where(x => !allGears.Contains(x));
                if (invalidCurveGears.Count() > 0)
				{
                    throw new ArgumentException($"Voltage level: {voltageLevel.VoltageLevel} contains MaxTorqueCurve with invalid gear: {invalidCurveGears.Join()}.");
                }
			}
        }

        private IList<IElectricMotorLoadCurve> GetFullLoadCurves()
        {
            var flds = new List<IElectricMotorLoadCurve>();

            var torqueCurveNodes = GetNodes(XMLNames.MaxTorqueCurve);

            foreach (XmlNode powerMapNode in torqueCurveNodes)
            {
                flds.Add(new ElectricMotorLoadCurve(powerMapNode));
            }

            return flds;
        }

        protected override IList<IElectricMotorVoltageLevel> GetVoltageLevels()
        {
            var voltageLevelNodes = GetNodes(XMLNames.ElectricMachine_VoltageLevel, BaseNode);
            if (voltageLevelNodes is null || voltageLevelNodes.Count == 0)
            {
                return null;
            }

            var voltageLevels = new List<IElectricMotorVoltageLevel>();

            foreach (XmlNode voltageLevelNode in voltageLevelNodes)
            {
                voltageLevels.Add(new XMLElectricMotorIEPCInputDataProviderV26(null, voltageLevelNode, null));
            }

            if (voltageLevels.Count > 1)
            {
                voltageLevels = voltageLevels.OrderBy(x => x.VoltageLevel.Value()).ToList();
            }

            return voltageLevels;
        }

        protected override Watt CalculateRatedPower()
        {
            var gearRatioUsedForMeasurement = Gears
                .Select(x => new { x.GearNumber, x.Ratio, Diff = Math.Round(Math.Abs(x.Ratio - 1), 6) }).GroupBy(x => x.Diff)
                .OrderBy(x => x.Key).First().OrderBy(x => x.Ratio).Reverse().First();

            var count = DesignTypeWheelMotor && NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;
            var maxPwr = 0.SI<Watt>();

            foreach (var entry in VoltageLevels.OrderBy(x => x.VoltageLevel).AsEnumerable())
            {
                foreach (var curve in entry.FullLoadCurve)
                {
                    var ratio = (curve.Gear == 0) ? gearRatioUsedForMeasurement.Ratio : Gears.First(x => x.GearNumber == curve.Gear).Ratio;

                    var maxTq = IEPCFullLoadCurveReader.Create(curve.LoadCurve, count, ratio);

                    if (maxTq.MaxPower > maxPwr)
                    {
                        maxPwr = maxTq.MaxPower;
                    }
                }
            }

            return maxPwr;
        }

        protected class ElectricMotorLoadCurve : AbstractXMLType, IElectricMotorLoadCurve
        {
            private static readonly Dictionary<string, string> _loadCurveMapping = AttributeMappings.EMTorqueCurve;

            public ElectricMotorLoadCurve(XmlNode xmlNode) : base(xmlNode) { }

            public virtual int Gear => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.TorqueCurve_Gear) ?? "0");

            public virtual TableData LoadCurve => ReadLoadCurve();

            private TableData ReadLoadCurve()
            {
                var loadCurveEntryNodes = GetNodes(XMLNames.TorqueCurve_Entry);
                return XMLHelper.ReadTableData(_loadCurveMapping, loadCurveEntryNodes);
            }
		}
	}

	//________________________________________________________________________________________

	public class XMLElectricMotorIEPCStandardInputDataProviderV26 : XMLElectricMotorIEPCInputDataProviderV26
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;
		public new const string XSD_TYPE = "IEPCStandardValuesDataDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLElectricMotorIEPCStandardInputDataProviderV26(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override void ValidateGearCount()
		{
			return;
		}

		public override TableData Conditioning => null;
	}

    // ---------------------------------------------------------------------------------------

    public class XMLElectricMotorDeclarationInputDataProviderV01 : XMLCommonElectricMotorDeclarationInputData,
		IXMLElectricMotorDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "ElectricMachineSystemDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricMotorVoltageLevel> _voltageLevels;

		public XMLElectricMotorDeclarationInputDataProviderV01(
			XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IElectricMotorDeclarationInputData

		public bool DcDcConverterIncluded => GetBool(XMLNames.ElectricMachine_DcDcConverterIncluded);

		public string IHPCType => GetString(XMLNames.ElectricMachine_IHPCType);

		public IList<IElectricMotorVoltageLevel> VoltageLevels => 
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public TableData DragCurve => ReadDragCurve();
		public TableData Conditioning => ElementExists(XMLNames.Conditioning)
			? ReadConditioning() : null;

		#endregion

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLElectricMotorIEPCIInputDataProviderV01 : XMLElectricMotorIEPCIInputDataProviderV23
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;
		public const string XSD_TYPE = "IEPCDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLElectricMotorIEPCIInputDataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		#region Overrides of XMLElectricMotorIEPCIInputDataProviderV23

		protected override void ValidateGearCount()
		{
		}

		#endregion
	}

	public class XMLElectricMotorIEPCIInputDataProviderV10 : XMLElectricMotorIEPCInputDataProviderV26
	{
        public static new readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_V10;
        public new const string XSD_TYPE = "IEPCDataDeclarationType"; 
        public static new readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

        public XMLElectricMotorIEPCIInputDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : 
			base(vehicle, componentNode, sourceFile) { }
    
		protected override void ValidateGearCount() { }
	}

}
