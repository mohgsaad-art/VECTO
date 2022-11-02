using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;

namespace VECTO3GUI2020.ViewModel
{

	public class OutputViewModel : ViewModelBase, IOutputViewModel
	{
		#region MembersAndProperties
		private object _messageLock = new Object();
		private ObservableCollection<MessageEntry> _messages = new ObservableCollection<MessageEntry>();
		private int _progress;
		private string _statusMessage;
		private ICommand _openFolderCommand;
		private ICommand _openFileCommand;

		public ObservableCollection<MessageEntry> Messages
		{
			get { return _messages; }
		}



		public int Progress
		{
			get => _progress;
			set => SetProperty(ref _progress, value);
		}

		public string StatusMessage
		{
			get { return _statusMessage; }
			set { SetProperty(ref _statusMessage, value); }
		}



		#endregion

		public void AddMessage(MessageEntry messageEntry)
		{
			lock (_messageLock)
			{
				Messages.Add(messageEntry);
			}
		}


		public OutputViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(Messages, _messageLock);
		}


		#region Commands

		// ReSharper disable once UnusedMember.Global
		public ICommand OpenFolderCommand =>
			_openFolderCommand ?? (_openFolderCommand = new RelayCommand<string>(
				OpenFolderExecute));

		// ReSharper disable once UnusedMember.Global
		public ICommand OpenFileCommand =>
			_openFileCommand ?? (_openFileCommand = new RelayCommand<string>(
				OpenFileExecute));

		private void OpenFolderExecute(string path)
		{
			ProcessHelper.OpenFolder(path);
		}

		private void OpenFileExecute(string path){
			if (path == null) {
				return;
			}

			ProcessHelper.OpenFile(path);
		}
		#endregion
	}




	public interface IOutputViewModel : IMainViewModel
	{
		//ObservableCollection<MessageEntry> Messages { get; }

		int Progress { get; set; }
		string StatusMessage { get; set; }
		void AddMessage(MessageEntry messageEntry);
	}
}