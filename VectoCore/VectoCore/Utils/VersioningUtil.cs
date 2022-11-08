

namespace TUGraz.VectoCore.Utils
{
    public static class VersioningUtil
    {
		public enum VersionPart {
			Major = 0,
			Minor = 1,
			Patch = 2,
			Build = 3
		};

		public static int CompareVersions(string a, string b, VersionPart bound = VersionPart.Build)
		{ 
			var va = a.Split('-')[0].Split('.');
			var vb = b.Split('-')[0].Split('.');
			
			for (int i = 0; (i < va.Length) && (i <= (int)bound); i++) {
				string vbPartStr = (i < vb.Length) ? vb[i] : "0";
				int vbPart = 0;
				int.TryParse(vbPartStr, out vbPart);

				int vaPart = 0;
				int.TryParse(va[i], out vaPart);

				if (vaPart != vbPart) {
					return (vaPart > vbPart) ? 1 : -1;
				}
			}

			return 0;
		}
    }
}
