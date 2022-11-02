using System;
using System.Diagnostics;
using Newtonsoft.Json;
using TUGraz.VectoCore;
using VECTO3GUI2020.ViewModel.Implementation.Common;

namespace VECTO3GUI2020.Model.Multistage
{
	public class JSONJob : ObservableObject
	{
		private JSONJobHeader _header;

		public JSONJobHeader Header
		{
			get => _header;
			set => SetProperty(ref _header, value);
		}

		private JSONJobBody _jobBody;

		public JSONJobBody Body
		{
			get => _jobBody;
			set => SetProperty(ref _jobBody, value);
		}
	}



	public class JSONJobHeader : ObservableObject
	{
		public static int PrimaryAndInterimVersion = 10;
			//public const int CompletedBusFileVersion = 7;

		private string _createdBy;
		private DateTime _dateTime;
		private string _appVersion;
		private int _fileVersion;


		public string CreatedBy
		{
			get { return _createdBy; }
			set { SetProperty(ref _createdBy, value); }
		}

		public DateTime Date
		{
			get { return _dateTime; }
			set { SetProperty(ref _dateTime, value); }
		}

		public string AppVersion
		{
			get { return _appVersion; }
			set { SetProperty(ref _appVersion, value); }
		}

		public int FileVersion
		{
			get { return _fileVersion; }
			set
			{
				SetProperty(ref _fileVersion, value);
				//JobType = JobTypeHelper.GetJobTypeByFileVersion(_fileVersion);
			}
		}
	}


	public class JSONJobBody : ObservableObject
	{
		private string _interimStep;
		private string _primaryVehicle;
		private bool _completed;

		public string PrimaryVehicle
		{
			get { return _primaryVehicle; }
			set { SetProperty(ref _primaryVehicle, value); }
		}

		public string InterimStep
		{
			get { return _interimStep; }
			set { SetProperty(ref _interimStep, value); }
		}

		public bool Completed
		{
			get => _completed;
			set => SetProperty(ref _completed, value);
		}
	}
}