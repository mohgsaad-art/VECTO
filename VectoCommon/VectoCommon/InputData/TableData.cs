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
using System.Data;

namespace TUGraz.VectoCommon.InputData
{
	public class TableData : DataTable, IEquatable<TableData>
	{
		public TableData(string fileName, DataSourceType sourceType = DataSourceType.CSVFile)
		{
			SourceType = sourceType;
			Source = fileName;
		}

		public TableData()
		{
			SourceType = DataSourceType.Embedded;
			Source = "";
		}

		public DataSourceType SourceType { get; protected set; }

		public string Source { get; protected set; }

        public override bool Equals(object obj) => Equals(obj as TableData);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(TableData other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }
            
            return IsEqual(other);
        }

        private bool IsEqual(TableData tbl2)
        {
            if ((Rows.Count != tbl2.Rows.Count) || (Columns.Count != tbl2.Columns.Count))
            {
                return false;
            }

            for (int i = 0; i < Rows.Count; i++)
            {
                for (int c = 0; c < Columns.Count; c++)
                {
                    if (!Equals(Rows[i][c], tbl2.Rows[i][c]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator ==(TableData td1, TableData td2)
        {
            if (ReferenceEquals(td1, td2))
            {
                return true;
            }

            if (ReferenceEquals(td1, null) || ReferenceEquals(td2, null))
            {
                return false;
            }

            return td1.Equals(td2);
        }

        public static bool operator !=(TableData td1, TableData td2) => !(td1 == td2);
    }

}