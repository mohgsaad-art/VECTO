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

		bool StartAutoSimulator();
		bool StopOutputSimulator();
	}
    public class AutoSimulator : IAutoSimulator
	{
		private BackgroundWorker worker;
		public AutoSimulator()
		{
			worker = new BackgroundWorker();
		}



		#region Implementation of IAutoSimulator

		private string _inputDirectory;
		public string InputDirectory
		{
			get
			{
				lock (_outputDirectory) {
					return _inputDirectory;
				}
			}
			set
			{
				lock (_outputDirectory) {
					_inputDirectory = value;
				}
			}
		}

		private string _outputDirectory;
		public string OutputDirectory
		{
			get
			{
				lock (_outputDirectory) {
					return _outputDirectory;
				}
				
			}
			set
			{
				lock (_outputDirectory) {
					_outputDirectory = value;
				}
				
			}
		}

		public bool StartAutoSimulator()
		{
			if (!worker.IsBusy) {
				worker.RunWorkerAsync();
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
			//backgroundWorker.ReportProgress();
			while (!backgroundWorker.CancellationPending) {
				var filesToSimulate = CheckDirectoryAndCopyFiles();
				


			}
		}

		private IList<string> CheckDirectoryAndCopyFiles()
		{
			var copiedFiles = new List<string>();
			lock (_inputDirectory)
			{
				if (Directory.Exists(_inputDirectory))
				{
					foreach (var file in Directory.GetFiles(_inputDirectory))
					{
						lock (_outputDirectory)
						{
							if (CopyTo(file, _outputDirectory, out var destFile, true, exception => Debug.WriteLine(exception.Message))){
								copiedFiles.Add(destFile);
							}
						}
					}
				}
			}

			return copiedFiles;
		}

		private bool CopyTo(string source, string destDirectory, out string destFileName, bool overwrite, Action<Exception> errorAction)
		{
			destFileName = Path.Combine(destDirectory, Path.GetFileName(source));
	
			try {
				//TODO: Can be replaced in .NET5 with a single call to move;
				File.Copy(source, destFileName, overwrite);
				File.Delete(source);
			} catch (Exception ex) {
				errorAction.Invoke(ex);
				return false;
			}
			return true;
		}
	}


}
