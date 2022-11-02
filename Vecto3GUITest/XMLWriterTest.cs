using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;


namespace Vecto3GUITest
{
	[TestClass]
	public class WriterTests
	{
		private StandardKernel _kernel;
		private IXMLInputDataReader _xmlInputReader;


		public void Setup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}



	}

}