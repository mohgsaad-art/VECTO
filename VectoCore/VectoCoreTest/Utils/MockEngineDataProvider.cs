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

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockEngineDataProvider : IEngineEngineeringInputData, IEngineModeEngineeringInputData, IEngineFuelEngineeringInputData
	{
		public DataSource DataSource { get; set; }
		public string Source { get; set; }
		public bool SavedInDeclarationMode { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public string Creator { get; set; }
		public DateTime Date { get; set; }
		public string AppVersion => "Mock-Class";
		public string TechnicalReportId { get; set; }
		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;
		public string CertificationNumber { get; set; }
		public DigestData DigestValue { get; set; }
		public CubicMeter Displacement { get; set; }
		public PerSecond IdleSpeed { get; set; }
		public double WHTCMotorway { get; set; }
		public double WHTCRural { get; set; }
		public double WHTCUrban { get; set; }
		public double ColdHotBalancingFactor { get; set; }
		public double CorrectionFactorRegPer { get; set; }
		public FuelType FuelType { get; set; }
		public TableData FuelConsumptionMap { get; set; }
		public TableData FullLoadCurve { get; set; }

		IList<IEngineFuelEngineeringInputData> IEngineModeEngineeringInputData.Fuels
		{
			get { return new[] { this }.Cast<IEngineFuelEngineeringInputData>().ToList(); }
		}

		public IList<IEngineFuelDeclarationInputData> Fuels
		{
			get { return new[] { this }.Cast<IEngineFuelDeclarationInputData>().ToList(); }
		}

		public IWHRData WasteHeatRecoveryDataElectrical { get; }

		public IWHRData WasteHeatRecoveryDataMechanical { get; }

		public Watt RatedPowerDeclared { get; set; }
		public PerSecond RatedSpeedDeclared { get; set; }
		public NewtonMeter MaxTorqueDeclared { get; set; }

		IList<IEngineModeEngineeringInputData> IEngineEngineeringInputData.EngineModes
		{
			get { return  new[] { this }.Cast<IEngineModeEngineeringInputData>().ToList(); }
		}

		public IList<IEngineModeDeclarationInputData> EngineModes { get { return new[] { this }.Cast<IEngineModeDeclarationInputData>().ToList(); } }
		public WHRType WHRType { get; }

		public KilogramSquareMeter Inertia { get; set; }
		public double WHTCEngineering { get; set; }

		public Second EngineStartTime => DeclarationData.Engine.DefaultEngineStartTime;
	}
}