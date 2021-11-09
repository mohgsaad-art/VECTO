using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ControlzEx.Standard;

namespace VECTOTrailer.Model.Simulation
{
	public interface IAutoSimulator
	{
		string InputDirectory { get; set; }
		string OutputDirectory { get; set; }


		bool WriteModData { get; set; }
		bool WriteModData1Hz { get; set; }
		bool ValidateData { get; set; }
		bool WriteActualModData { get; set; }


		bool StartAutoSimulator();
		bool StopOutputSimulator();
	}
    public class AutoSimulator : IAutoSimulator
	{

		private BackgroundWorker worker;
		public AutoSimulator()
		{
			worker = new BackgroundWorker() {
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			worker.DoWork += DoWork;
		}
		#region SimulationSettings

		public bool WriteModData { get; set; }
		public bool WriteModData1Hz { get; set; }
		public bool ValidateData { get; set; }
		public bool WriteActualModData { get; set; }

		#endregion

		private object dirLock = new object();

		#region Implementation of IAutoSimulator

		private string _inputDirectory;
		public string InputDirectory
		{
			get
			{
				lock (dirLock) {
					return _inputDirectory;
				}
			}
			set
			{
				lock (dirLock) {
					_inputDirectory = value;
				}
			}
		}

		private string _outputDirectory;
		public string OutputDirectory
		{
			get
			{
				lock (dirLock) {
					return _outputDirectory;
				}
				
			}
			set
			{
				lock (dirLock) {
					_outputDirectory = value;
				}
				
			}
		}


		public bool StartAutoSimulator()
		{
			if (!worker.IsBusy) {
				worker.RunWorkerAsync();
				return true;
			}

			return false;
		}

		public bool StopOutputSimulator()
		{
			if (worker.WorkerSupportsCancellation) {
				worker.CancelAsync();
				return true;
			}

			return false;
		}

		#endregion

		private void DoWork(object theSender, DoWorkEventArgs e)
		{
			var backgroundWorker = theSender as BackgroundWorker;
			if (backgroundWorker == null) {
				throw new ArgumentException();
			}

			
			while (!backgroundWorker.CancellationPending) {
				var filesToSimulate = CheckDirectoryAndCopyFiles(progressAction:(progress => backgroundWorker.ReportProgress(0, progress)));
				


			}
		}

		private IList<string> CheckDirectoryAndCopyFiles(Action<VectoSimulationProgress> progressAction)
		{
			var copiedFiles = new List<string>();
			lock (dirLock)
			{
				if (Directory.Exists(_inputDirectory))
				{
					foreach (var file in Directory.GetFiles(_inputDirectory))
					{

							if (_outputDirectory == _inputDirectory) {
								throw new Exception("Input and output directory must not be the same directory");
							}
							if (CopyTo(file, _outputDirectory, out var destFile, true, progressAction)){
								copiedFiles.Add(destFile);
							}
					}
				} else {
					progressAction.Invoke(new VectoSimulationProgress() {
						Message = $"Directory {_inputDirectory} does not exist",
						Type = VectoSimulationProgress.MsgType.LogError
					});
				}
			}

			return copiedFiles;
		}

		private bool CopyTo(string source, string destDirectory, out string destFileName, bool overwrite, Action<VectoSimulationProgress> errorAction)
		{
			destFileName = Path.Combine(destDirectory, Path.GetFileName(source));
	
			try {
				//TODO: Can be replaced in .NET5 with a single call to move;
				File.Copy(source, destFileName, overwrite);
				File.Delete(source);
			} catch (Exception ex) {
				errorAction.Invoke(new VectoSimulationProgress() {
					Type = VectoSimulationProgress.MsgType.LogError,
					Message = "Error while copying files: " + ex.Message
				});
				return false;
			}
			return true;
		}
	}


}
