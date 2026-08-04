using System;
using System.Runtime.CompilerServices;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Ninject;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Factories;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test
{
	public class TestHelper
	{
		private IXMLInputDataReader _inputDataReader;

		public static IKernel GetKernel()
		{
			var kernel = new StandardKernel(
				new VectoNinjectModule(),
				new JobEditModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
				new FactoryModule(),
				new Vecto3GUI2020Module()
			);
			kernel.Rebind<IDialogHelper>().To<MockDialogHelper>().InSingletonScope();
			kernel.Rebind<IWindowHelper>().To<MockWindowHelper>().InSingletonScope();

			return kernel;
		}

		public static IKernel GetKernel(out MockDialogHelper mockDialogHelper, out MockWindowHelper mockWindowHelper)
		{
			var kernel = GetKernel();
			mockDialogHelper = kernel.Get<IDialogHelper>() as MockDialogHelper;
			mockWindowHelper = kernel.Get<IWindowHelper>() as MockWindowHelper;

			return kernel;
		}

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

	public static class IKernelHelperTest
	{
		public static MockDialogHelper GetMockDialogHelper(this IKernel k)
		{
			var mockDialogHelper = k.Get<IDialogHelper>() as MockDialogHelper;
            return mockDialogHelper;
		}

		public static MockWindowHelper GetMockWindowHelper(this IKernel k)
		{
			var mockWindowHelper = k.Get<IWindowHelper>() as MockWindowHelper;
			return mockWindowHelper;
			
		}

	}
}