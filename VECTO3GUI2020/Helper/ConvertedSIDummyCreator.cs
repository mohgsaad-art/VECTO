using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;

namespace VECTO3GUI2020.Helper
{
    public static class ConvertedSIDummyCreator
    {
		public static ConvertedSI CreateMillimeterDummy()
		{
			return new ConvertedSI(0, "mm");
		} 

    }
}
