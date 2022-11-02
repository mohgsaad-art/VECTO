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
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringGearshiftDataV07 : AbstractXMLType, IXMLEngineeringGearshiftData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "ShiftStrategyParametersEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringGearshiftDataV07(XmlNode node) : base(node) { }

		#region Implementation of IGearshiftEngineeringInputData

		//public virtual Second TractionInterruption
		//{
		//	get { return GetNode(XMLNames.Gearbox_TractionInterruption)?.InnerText.ToDouble().SI<Second>(); }
		//}

		public virtual Second MinTimeBetweenGearshift =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift, required: false)
				?.InnerText.ToDouble().SI<Second>() ?? DeclarationData.Gearbox.MinTimeBetweenGearshifts;

		public virtual double TorqueReserve =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve, required: false)?.InnerText.ToDouble() ??
			DeclarationData.GearboxTCU.TorqueReserve;

		public virtual MeterPerSecond StartSpeed =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed, required: false)
				?.InnerText.ToDouble().SI<MeterPerSecond>() ?? DeclarationData.GearboxTCU.StartSpeed;

		public virtual MeterPerSquareSecond StartAcceleration =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration, required: false)
				?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ?? DeclarationData.GearboxTCU.StartAcceleration;

		public virtual double StartTorqueReserve =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve, required: false)
				?.InnerText.ToDouble() ??
			DeclarationData.GearboxTCU.TorqueReserveStart;

		public virtual Second DownshiftAfterUpshiftDelay =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_DownshiftAfterUpshiftDelay, required: false)
				?.InnerText.ToDouble().SI<Second>() ??
			DeclarationData.Gearbox.DownshiftAfterUpshiftDelay;

		public virtual Second UpshiftAfterDownshiftDelay =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_UpshiftAfterDownshiftDelay, required: false)
				?.InnerText.ToDouble().SI<Second>() ?? DeclarationData.Gearbox.UpshiftAfterDownshiftDelay;

		public virtual MeterPerSquareSecond UpshiftMinAcceleration =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_UpshiftMinAcceleration, required: false)
				?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ?? DeclarationData.Gearbox.UpshiftMinAcceleration;

		public virtual Second GearResidenceTime => null;
		public virtual double? DnT99LHMin1 => null;
		public virtual double? DnT99LHMin2 => null;
		public virtual int? AllowedGearRangeUp => null;
		public virtual int? AllowedGearRangeDown => null;
		public virtual Second LookBackInterval => null;
		public virtual Watt AvgCardanPowerThresholdPropulsion => null;
		public virtual Watt CurrCardanPowerThresholdPropulsion => null;
		public virtual double? TargetSpeedDeviationFactor => null;
		public virtual double? EngineSpeedHighDriveOffFactor => null;
		public virtual double? RatingFactorCurrentGear => null;
		public virtual TableData AccelerationReserveLookup => null;
		public virtual TableData ShareTorque99L => null;
		public virtual TableData PredictionDurationLookup => null;
		public virtual TableData ShareIdleLow => null;
		public virtual TableData ShareEngineHigh => null;
		public virtual string Source => null;
		public virtual Second DriverAccelerationLookBackInterval => null;
		public virtual MeterPerSquareSecond DriverAccelerationThresholdLow => null;
		public virtual double? RatioEarlyUpshiftFC => null;
		public virtual double? RatioEarlyDownshiftFC => null;
		public int? AllowedGearRangeFC => null;

		public double? VeloictyDropFactor => null;

		public PerSecond MinEngineSpeedPostUpshift => null;
		public Second ATLookAheadTime => null;
		public double[][] ShiftSpeedsTCToLocked => null;
		public double? PEV_TargetSpeedBrakeNorm => null;
		public double? PEV_DownshiftSpeedFactor => null;
		public double? PEV_DeRatingDownshiftSpeedFactor => null;
		public double? PEV_DownshiftMinSpeedFactor => null;

		public double? AccelerationFactor => null;

		public virtual TableData LoadStageShiftLines => null;
		public virtual IList<double> LoadStageThresholdsUp => null;
		public virtual IList<double> LoadStageThresholdsDown => null;

		public virtual Second PowershiftShiftTime =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_PowershiftShiftTime, required: false)
				?.InnerText.ToDouble().SI<Second>() ?? 0.8.SI<Second>();

		#endregion

		#region Implementation of ITorqueConverterEngineeringShiftParameterInputData

		public virtual MeterPerSquareSecond CLUpshiftMinAcceleration =>
			GetNode(XMLNames.TorqueConverter_CLUpshiftMinAcceleration, required: false)
				?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ??
			DeclarationData.Gearbox.UpshiftMinAcceleration;

		public virtual MeterPerSquareSecond CCUpshiftMinAcceleration =>
			GetNode(XMLNames.TorqueConverter_CCUpshiftMinAcceleration, required: false)
				?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ??
			DeclarationData.Gearbox.UpshiftMinAcceleration;

		#endregion
	}

	internal class XMLEngineeringGearshiftDataV10 : XMLEngineeringGearshiftDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "ShiftStrategyParametersType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringGearshiftDataV10(XmlNode node) : base(node) { }
	}
}
