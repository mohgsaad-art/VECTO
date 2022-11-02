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
using System.Linq;
using TUGraz.VectoCore.Tests.Utils;

namespace GraphDrawer
{
    internal class Program
    {
        private const string HELP = @"
Tool for plotting graphs comparing Vecto 2.2 and Vecto 3

--split <leng> ... split input into parts of length <len> (in m), only distance output

";

        private static void Main(string[] args)
        {
			var graphWriter = new GraphWriter();
			if (args.Contains("--split")) {
				Console.Error.WriteLine("plotting graphs splitted by distance");
                var idx = Array.FindIndex(args, x => x == "--split");
                var lenght = int.Parse(args[idx + 1]);
                var success = true;
                var start = 0;
                do {
					Console.Error.WriteLine("plotting {0} - {1}", start / 1000, (start + lenght) / 1000);
                    success = graphWriter.WriteDistanceSlice(args[0], args[1], start, start + lenght);
                    start += lenght;
                } while (success);
				Console.Error.WriteLine("plotting full cycle");
                graphWriter.Write(args[0], args[1]);
				Console.Error.WriteLine("done");
                return;
            }
			Console.Error.WriteLine("plotting graphs...");
            graphWriter.Write(args[0], args[1]);
			Console.Error.WriteLine("done");
        }
    }
}