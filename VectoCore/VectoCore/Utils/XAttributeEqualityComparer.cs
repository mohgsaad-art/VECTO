using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TUGraz.VectoCore.Utils
{
    public class XAttributeEqualityComparer : IEqualityComparer<XAttribute>
    {
		public bool Equals(XAttribute x, XAttribute y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (ReferenceEquals(x, null))
				return false;
			if (ReferenceEquals(y, null))
				return false;
			if (x.GetType() != y.GetType())
				return false;
			return x.IsNamespaceDeclaration == y.IsNamespaceDeclaration && x.Name.Equals(y.Name) && x.NodeType == y.NodeType && x.Value == y.Value;
		}

		public int GetHashCode(XAttribute obj)
		{
			unchecked
			{
				var hashCode = obj.IsNamespaceDeclaration.GetHashCode();
				hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)obj.NodeType;
				hashCode = (hashCode * 397) ^ obj.Value.GetHashCode();
				return hashCode;
			}
		}
	}
}
