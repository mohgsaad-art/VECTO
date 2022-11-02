using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace VECTO3GUI.Helper
{
	public static class  MetroDialogHelper
	{
		public static MessageDialogResult GetModalDialogBox(object viewModel, string title, string message,
			MetroDialogSettings settings)
		{
			return GetModalDialogBox(viewModel, title, message, MessageDialogStyle.AffirmativeAndNegative, settings);
		}

		public static MessageDialogResult GetModalDialogBox(object viewModel, string title, string message, 
			MessageDialogStyle style = MessageDialogStyle.AffirmativeAndNegative, MetroDialogSettings settings = null)
		{
			var coordinator = DialogCoordinator.Instance;
			return coordinator.ShowModalMessageExternal(viewModel, title, message, style, settings);
		}
	}
}
