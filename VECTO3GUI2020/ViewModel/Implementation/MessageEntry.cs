using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace VECTO3GUI2020.ViewModel.Implementation
{

	public enum MessageType
	{
		InfoMessage,
		StatusMessage,
		ErrorMessage,
		WarningMessage,
	}

	public class MessageEntry : ObservableObject
	{
		

		private string _message;
		private DateTime _time = DateTime.Now;
		private string _source;
		private MessageType _type;
		private string _link;

		public string Message
		{
			get { return _message; }
			set { SetProperty(ref _message, value); }
		}

		public string Link
		{
			get { return _link; }
			set { SetProperty(ref _link, value); }
		}

		public MessageType Type
		{
			get { return _type; }
			set { SetProperty(ref _type, value); }
		}

		public DateTime Time
		{
			get { return _time; }
			set { SetProperty(ref _time, value); }
		}
		public string Source
		{
			get { return _source; }
			set { SetProperty(ref _source, value); }
		}
	}
}
