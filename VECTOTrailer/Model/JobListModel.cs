using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using Castle.Core.Internal;
using VECTOTrailer.Helper;
using VECTOTrailer.ViewModel.Impl;

namespace VECTOTrailer.Model
{
	public class JobListEntry
	{
		public string JobTypeName { get; set; }
		public bool IsSelected { get; set; }
		public string JobFilePath { get; set; }
	}

    [System.Configuration.SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class JobCollection: IList<JobListEntry>
	{
		private IList<JobListEntry> _listImplementation = new List<JobListEntry>();



		#region Implementation of IEnumerable

		public IEnumerator<JobListEntry> GetEnumerator()
		{
			return _listImplementation.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_listImplementation).GetEnumerator();
		}

		#endregion

		#region Implementation of ICollection<JobListEntry>

		public void Add(JobListEntry item)
		{
			_listImplementation.Add(item);
		}

		public void Clear()
		{
			_listImplementation.Clear();
		}

		public bool Contains(JobListEntry item)
		{
			return _listImplementation.Contains(item);
		}

		public void CopyTo(JobListEntry[] array, int arrayIndex)
		{
			_listImplementation.CopyTo(array, arrayIndex);
		}

		public bool Remove(JobListEntry item)
		{
			return _listImplementation.Remove(item);
		}

		public int Count => _listImplementation.Count;

		public bool IsReadOnly => _listImplementation.IsReadOnly;

		#endregion

		#region Implementation of IList<JobListEntry>

		public int IndexOf(JobListEntry item)
		{
			return _listImplementation.IndexOf(item);
		}

		public void Insert(int index, JobListEntry item)
		{
			_listImplementation.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_listImplementation.RemoveAt(index);
		}

		public JobListEntry this[int index]
		{
			get => _listImplementation[index];
			set => _listImplementation[index] = value;
		}

		#endregion
	}


	public class JobListModel
	{
		private JobCollection _jobCollection;
		private readonly ITrailerToolSettings _settings;

		public JobListModel(ITrailerToolSettings settings)
		{
			_settings = settings;
			LoadJobList();
		}


		private void LoadJobList()
		{
			_jobCollection = _settings.GeneralSettings.JobList ?? new JobCollection();
		}

		public void SaveJobList(IList<JobEntry> jobEntries)
		{
			_jobCollection.Clear();
			foreach (var jobEntry in jobEntries) {
				_jobCollection.Add(
					jobEntry.Missing 
						? new JobListEntry()
							{
								JobFilePath = jobEntry.JobEntryFilePath,
								IsSelected = false,
								JobTypeName = JobType.Unknown.GetLabel(),
							} 
						: new JobListEntry
							{
								JobTypeName = jobEntry.Header.JobType.GetLabel(),
								IsSelected = jobEntry.Selected,
								JobFilePath = jobEntry.JobEntryFilePath
							}
						);
			}

			var trailerToolSettings = new TrailerToolSettings();
			trailerToolSettings.GeneralSettings.JobList = _jobCollection;
		}

		public IList<JobEntry> GetJobEntries()
		{
			var jobEntries = new List<JobEntry>();

			if (_jobCollection.IsNullOrEmpty()) {
				return jobEntries;
			}

			foreach (var jobListEntry in _jobCollection)
			{
				if (!File.Exists(jobListEntry.JobFilePath)) {
					jobEntries.Add(new JobEntry() {
						JobEntryFilePath = jobListEntry.JobFilePath,
						Missing = true
					});
					continue;
				}
				var jobType = JobTypeHelper.Parse(jobListEntry.JobTypeName);
				JobEntry jobEntry;

				if (jobType == JobType.CompletedTrailerJob)
				{
					jobEntry = SerializeHelper.DeserializeToObject<JobEntry>(jobListEntry.JobFilePath);
				}
				else
				{
					jobEntry = new JobEntry
					{
						Header = new JobHeader { JobType = jobType},
						Body = new JobBody {
							CompletedVehicle = JobType.CompletedXml == jobType ? jobListEntry.JobFilePath : null
						}
					};
				}
				
				if (jobEntry != null) {
					jobEntry.JobEntryFilePath = jobListEntry.JobFilePath;
					jobEntry.Selected = jobListEntry.IsSelected;
					jobEntries.Add(jobEntry);
				}
			}
			return jobEntries;
		}
	}
}
