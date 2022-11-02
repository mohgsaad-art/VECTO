using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
    public abstract class ComponentWriter
    {
		protected readonly XNamespace _writerNamespace;

		public ComponentWriter(XNamespace writerNamespace)
		{
			_writerNamespace = writerNamespace;
		}
    }
}
