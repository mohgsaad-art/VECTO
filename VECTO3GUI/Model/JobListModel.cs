using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.Model
{
	public class JobListEntry
	{
		public string JobTypeName { get; set; }
		public bool IsSelected { get; set; }
		public string JobFilePath { get; set; }
	}

	public class JobListModel
	{
		private const string ConfigFolderName = "Config";
		private const string JobListFileName = "JobList.json";

		private string _jobListFilePath = Path.Combine(".", ConfigFolderName, JobListFileName);

		public List<JobListEntry> JobList { get; set; }

		public JobListModel()
		{
			SetConfigFolder();
			LoadJobList();
		}

		private void SetConfigFolder()
		{
			if (!Directory.Exists($"./{ConfigFolderName}"))
			{
				Directory.CreateDirectory($"{ConfigFolderName}");
			}
		}

		private void LoadJobList()
		{
			var jobList = new List<JobListEntry>();
			if (File.Exists(_jobListFilePath))
				jobList = SerializeHelper.DeserializeToObject<List<JobListEntry>>(_jobListFilePath);
			JobList = jobList;
		}

		public void SaveJobList(IList<JobEntry> jobEntries)
		{
			SetJobList(jobEntries);
			SerializeHelper.SerializeToFile(_jobListFilePath, JobList);
		}

		private void SetJobList(IList<JobEntry> jobEntries)
		{
			if (jobEntries == null)
				return;

			JobList = new List<JobListEntry>();

			for (int i = 0; i < jobEntries.Count; i++)
			{
				JobList.Add
				(	jobEntries[i].Missing ? new JobListEntry() {
							JobFilePath = jobEntries[i].JobEntryFilePath,
							IsSelected = false,
							JobTypeName = JobType.Unknown.GetLabel(),
						}: 
					new JobListEntry
					{
						JobTypeName = jobEntries[i].Header.JobType.GetLabel(),
						IsSelected = jobEntries[i].Selected,
						JobFilePath = jobEntries[i].JobEntryFilePath
					}
				);
			}
		}

		public IList<JobEntry> GetJobEntries()
		{
			var jobEntries = new List<JobEntry>();

			if (JobList.IsNullOrEmpty())
				return jobEntries;

			for (int i = 0; i < JobList.Count; i++)
			{
				if (!File.Exists(JobList[i].JobFilePath)) {
					jobEntries.Add(new JobEntry() {
						JobEntryFilePath = JobList[i].JobFilePath,
						Missing = true
					});
					continue;
				}
				var jobType = JobTypeHelper.Parse(JobList[i].JobTypeName);
				JobEntry jobEntry;

				if (jobType == JobType.CompletedBusJob || jobType == JobType.SingleBusJob)
				{
					jobEntry = SerializeHelper.DeserializeToObject<JobEntry>(JobList[i].JobFilePath);
				}
				else
				{
					jobEntry = new JobEntry
					{
						Header = new JobHeader { JobType = jobType},
						Body = new JobBody {
							CompletedVehicle = JobType.CompletedXml == jobType ? JobList[i].JobFilePath : null
						}
					};
				}
				
				if (jobEntry != null) {
					jobEntry.JobEntryFilePath = JobList[i].JobFilePath;
					jobEntry.Selected = JobList[i].IsSelected;
					jobEntries.Add(jobEntry);
				}
			}
			return jobEntries;
		}
	}
}
