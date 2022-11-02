using System.Diagnostics;
using System.Windows.Input;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020.ViewModel.Implementation
{
	public class AboutViewModel : ViewModelBase, IMainViewModel
	{
		#region Members

		private ICommand _euplLinkClickedCommand;
		private ICommand _mailClickedCommand;
		private ICommand _jrcPicClickedCommand;

		#endregion

		#region Properties

		public string EUPLLink { get; set; }
		public string JRCMail { get; set; }
		public string JRCPic { get; set; }

		#endregion

		public AboutViewModel()
		{
			EUPLLink = "https://joinup.ec.europa.eu/community/eupl/og_page/eupl";
			JRCMail = "mailto:jrc-vecto@ec.europa.eu";
			JRCPic = "http://ec.europa.eu/dgs/jrc/index.cfm";
		}
		
		#region Commands

		public ICommand EUPLLinkClickedCommand
		{
			get
			{
				return _euplLinkClickedCommand ?? (_euplLinkClickedCommand = new RelayCommand(DoLinkClickedCommand));
			}
		}

		private void DoLinkClickedCommand()
		{
			Process.Start(new ProcessStartInfo(EUPLLink) { UseShellExecute = true});
		}

		public ICommand MailClickedCommand
		{
			get { return _mailClickedCommand ?? (_mailClickedCommand = new RelayCommand(DoMailClickedCommand)); }
		}

		private void DoMailClickedCommand()
		{
			Process.Start(new ProcessStartInfo(JRCMail) { UseShellExecute = true});
		}

		public ICommand JrcPicClickedCommand
		{
			get { return _jrcPicClickedCommand ?? (_jrcPicClickedCommand = new RelayCommand(DoJrcPicClickedCommand)); }
		}

		private void DoJrcPicClickedCommand()
		{
			Process.Start(new ProcessStartInfo(JRCPic) { UseShellExecute = true});
		}
		#endregion


	}
}
