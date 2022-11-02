using System;
using System.Windows.Forms;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Ninject
{
	public interface IMultistageDependencies
	{
		Lazy<IDialogHelper> DialogHelperLazy { get; }

		IDialogHelper DialogHelper { get; }
		IDeclarationInjectFactory InjectFactory { get; }
		IComponentViewModelFactory ComponentViewModelFactory { get; }

		IXMLWriterFactory XMLWriterFactory { get;  }
	}
}