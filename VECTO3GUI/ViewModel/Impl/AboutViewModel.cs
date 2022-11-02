
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VECTO3GUI.Util;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AboutViewModel : ObservableObject
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
			Process.Start(EUPLLink);
		}

		public ICommand MailClickedCommand
		{
			get { return _mailClickedCommand ?? (_mailClickedCommand = new RelayCommand(DoMailClickedCommand)); }
		}

		private void DoMailClickedCommand()
		{
			Process.Start(JRCMail);
		}

		public ICommand JrcPicClickedCommand
		{
			get { return _jrcPicClickedCommand ?? (_jrcPicClickedCommand = new RelayCommand(DoJrcPicClickedCommand)); }
		}

		private void DoJrcPicClickedCommand()
		{
			Process.Start(JRCPic);
		}

		public ICommand CloseWindowCommand
		{
			get { return null; }
		}

		#endregion


	}
}
