using System;
using System.IO;
using Newtonsoft.Json;


namespace VECTO3GUI.ViewModel.Impl
{
	public enum JobType
	{
		SingleBusJob,
		CompletedBusJob,
		CompletedXml,
		Unknown
	}

	public static class JobTypeHelper
	{
		private const string SingleBusJobLabel = "Single Bus Job";
		private const string CompletedBusJobLabel = "Completed Bus Job";
		private const string CompletedXmlLabel = "Completed Bus XML";

		public static string GetLabel(this JobType jobType)
		{
			switch (jobType)
			{
				case JobType.SingleBusJob:
					return SingleBusJobLabel;
				case JobType.CompletedBusJob:
					return CompletedBusJobLabel;
				case JobType.CompletedXml:
					return CompletedXmlLabel;
				default:
					return nameof(JobType.Unknown);
			}
		}

		public static JobType Parse(this string jobTypeName)
		{
			if (SingleBusJobLabel == jobTypeName)
				return JobType.SingleBusJob;
			if (CompletedBusJobLabel == jobTypeName)
				return JobType.CompletedBusJob;
			if (CompletedXmlLabel == jobTypeName)
				return JobType.CompletedXml;

			return JobType.Unknown;
		}

		public static JobType GetJobTypeByFileVersion(int fileVersion)
		{
			if (fileVersion == JobHeader.SingleBusFileVersion)
				return JobType.SingleBusJob;
			if (fileVersion == JobHeader.CompletedBusFileVersion)
				return JobType.CompletedBusJob;
			return JobType.Unknown;
		}

		public static int GetJobTypeNumberByJobType(this JobType jobType)
		{
			if (jobType == JobType.CompletedBusJob)
				return JobHeader.CompletedBusFileVersion;
			if (jobType == JobType.SingleBusJob)
				return JobHeader.SingleBusFileVersion;
			return 0;
		}
	}

	public class JobEntry : ObservableObject
	{
		public const string APP_VERSION = "VECTO3GUI";

		private JobHeader _header;
		private JobBody _body;
		private bool _selected;
		private string _jobEntryFilePath;
		private bool _missing;

		[JsonIgnore]
		public bool Selected
		{
			get { return !Missing && _selected; }
			set { SetProperty(ref _selected, value); }
		}

		[JsonIgnore]
		public string JobEntryFilePath
		{
			get { return _jobEntryFilePath; }
			set { SetProperty(ref _jobEntryFilePath, value); }
		}
		
		public JobHeader Header
		{
			get { return _header; }
			set { SetProperty(ref _header, value); }
		}
		public JobBody Body
		{
			get { return _body; }
			set { SetProperty(ref _body, value); }
		}

		public bool Missing
		{
			get { return _missing; }
			set { SetProperty(ref _missing, value); }
		}

		public string GetAbsoluteFilePath(string propertyFilePath)
		{
			if (IsFileName(propertyFilePath)) {
				var folderPath = Path.GetDirectoryName(JobEntryFilePath);
				return Path.Combine(folderPath, propertyFilePath);
			}
			return propertyFilePath;
		}

		private bool IsFileName(string filePath)
		{
			return !Directory.Exists(filePath) && !File.Exists(filePath);
		}
	}


	public class JobHeader : ObservableObject
	{
		public const int SingleBusFileVersion = 6;
		public const int CompletedBusFileVersion = 7;

		private JobType _jobType;
		private string _createdBy;
		private DateTime _dateTime;
		private string _appVersion;
		private int _fileVersion;
	
		[JsonIgnore]
		public JobType JobType
		{
			get { return _jobType; }
			set { SetProperty(ref _jobType, value); }
		}
		
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
				JobType = JobTypeHelper.GetJobTypeByFileVersion(_fileVersion);
			}
		}
	}


	public class JobBody : ObservableObject
	{
		private string _completedVehicle;
		private string _primaryVehicle;
		private string _primaryVehicleResults;

		public string CompletedVehicle
		{
			get { return _completedVehicle; }
			set { SetProperty(ref _completedVehicle, value); }
		}

		public string PrimaryVehicle
		{
			get { return _primaryVehicle; }
			set { SetProperty(ref _primaryVehicle, value); }
		}

		public string PrimaryVehicleResults
		{
			get { return _primaryVehicleResults; }
			set { SetProperty(ref _primaryVehicleResults, value); }
		}
	}
}