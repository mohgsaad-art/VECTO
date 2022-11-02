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
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData
{
	public interface IInputDataProvider
	{
		DataSource DataSource { get; }
	}

	public interface IDeclarationInputDataProvider : IInputDataProvider
	{
		IDeclarationJobInputData JobInputData { get; }

		IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData { get; }

		XElement XMLHash { get; }
	}


	public interface IEngineeringInputDataProvider : IInputDataProvider
	{
		IEngineeringJobInputData JobInputData { get; }

		IDriverEngineeringInputData DriverInputData { get; }

	}

	public interface IPrimaryVehicleInformationInputDataProvider : IInputDataProvider
	{
		IVehicleDeclarationInputData Vehicle { get; }

		DigestData ManufacturerRecordHash { get; }

		DigestData PrimaryVehicleInputDataHash { get; }

		DigestData VehicleSignatureHash { get; }

		IResultsInputData ResultsInputData { get; }

		IApplicationInformation ApplicationInformation { get; }

		//DigestData ManufacturerHash { get; }

		IResult GetResult(VehicleClass vehicleClass, MissionType mission, string fuelMode, Kilogram payload);

		XElement XMLHash { get; }
	}

	public interface ISingleBusInputDataProvider : IDeclarationInputDataProvider
	{
		IVehicleDeclarationInputData PrimaryVehicle { get; }
		IVehicleDeclarationInputData CompletedVehicle { get; }
	}

	


	public interface IMultistageBusInputDataProvider : IDeclarationInputDataProvider
	{
		new IDeclarationMultistageJobInputData JobInputData { get; }
	}

	public interface IMultistagePrimaryAndStageInputDataProvider : IInputDataProvider
	{
		IDeclarationInputDataProvider PrimaryVehicle { get; }
		IVehicleDeclarationInputData StageInputData { get; }
	}


	public interface IDeclarationMultistageJobInputData
	{
		IPrimaryVehicleInformationInputDataProvider PrimaryVehicle { get; }

		IList<IManufacturingStageInputData> ManufacturingStages { get; }

		IManufacturingStageInputData ConsolidateManufacturingStage { get; }

		VectoSimulationJobType JobType { get; }

		bool InputComplete { get; }

		IList<string> InvalidEntries { get; }
	}


	public interface IMultiStageTypeInputData : IManufacturingStageInputData,  IInputDataProvider
	{
	}

	public interface IMultistageVIFInputData :  IInputDataProvider
	{
		IVehicleDeclarationInputData VehicleInputData { get; }
		IMultistageBusInputDataProvider MultistageJobInputData { get; }
	}
}


