using System;
using System.CodeDom;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace Vecto3GUI2020Test
{
	public class TestHelper
	{
		private IXMLInputDataReader _inputDataReader;

		public TestHelper(IXMLInputDataReader inputDataReader)
		{
			_inputDataReader = inputDataReader;
		}




		public IInputDataProvider GetInputDataProvider(string fileName)
		{
			return _inputDataReader.Create(fileName);
		}

		public static string GetMethodName([CallerMemberName] string name = null)
		{
			if (name == null) {
				throw new ArgumentException();
			}
			return name;
		}
	}
}