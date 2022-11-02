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
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData
{
	public interface IOutputDataWriter : IModalDataWriter, IReportWriter, ISummaryWriter
	{
		string JobFile { get; }
	}

	public interface IModalDataWriter
	{
		void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData);
	}

	public interface ISummaryWriter
	{
		void WriteSumData(DataTable sortedAndFilteredTable);
	}

	public interface IReportWriter
	{
		void WriteReport(ReportType type, XDocument data);

		void WriteReport(ReportType type, Stream data);


		IDictionary<ReportType, string> GetWrittenFiles();
		int NumberOfManufacturingStages { set; }

		XDocument MultistageXmlReport { get; }

	}



	public enum ReportType
	{
		DeclarationReportPdf,
		DeclarationReportManufacturerXML,
		DeclarationReportCustomerXML,
		DeclarationVTPReportXML,
		DeclarationReportMonitoringXML,
		DeclarationReportPrimaryVehicleXML,
		DeclarationReportMultistageVehicleXML
	}
}