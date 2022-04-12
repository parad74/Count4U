using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Extensions;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Common.ViewModel.AdapterTemplate
{
	public abstract class TemplateAdapterFileFolderViewModel : ImportModuleBaseViewModel, IDataErrorInfo
	{
		protected readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
		protected readonly InteractionRequest<OpenFolderDialogNotification> _folderChooseDilogRequest;
		protected string _path;
		protected string _sourcePath;
		protected readonly DelegateCommand _browseCommand;
		private readonly DelegateCommand _openCommand;
		protected readonly DelegateCommand _browseSourceCommand;
		private readonly DelegateCommand _openSourcePathCommand;
		protected bool _isDirectory;
		protected bool _isSingleFile;
		protected string _pathFilter;
		private int _processLisner;
		//private bool _listeningSourceFolder = false;
		private bool _isCopyFromSourceFolder = false;
		private bool _isCopyFromMainForm = false;

		private IObservable<long> observCountingFiles;
		//private IObservable<long> observCopyingFiles;
		private IDisposable disposeObservCountingFiles;
		//private IDisposable disposeObservCopyingFiles;


		protected AdapterMaskViewModel _maskViewModel;

		private AdapterFileWatcher _pathWatcher;

		protected TemplateAdapterFileFolderViewModel(IServiceLocator serviceLocator,
			IContextCBIRepository contextCBIRepository,
			IEventAggregator eventAggregator,
			IRegionManager regionManager,
			ILog logImport,
			IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
			IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
		{
			this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
			this._folderChooseDilogRequest = new InteractionRequest<OpenFolderDialogNotification>();
			this._browseCommand = new DelegateCommand(this.BrowseCommandExecuted, this.BrowseCommandCanExecute);
			this._openCommand = new DelegateCommand(this.OpenCommandExecuted, this.OpenCommandCanExecute);
			this._browseSourceCommand = new DelegateCommand(this.BrowseSourceCommandExecuted, this.BrowseSourceCommandCanExecute);
			this._openSourcePathCommand = new DelegateCommand(this.OpenCommandSourcePathExecuted, this.OpenCommandSourcePathCanExecute);
			this._isDirectory = false;
			this._isSingleFile = true;
			this._pathFilter = "All files (*.*)|*.*";
			//observCountingFiles = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4)).Select(x => x);
			//disposeObservCountingFiles = observCountingFiles.Subscribe(CountingFiles);
			//observCopyingFiles = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(4)).Select(x => x);
			//disposeObservCopyingFiles = observCopyingFiles.Subscribe(CopyingFiles);
		}

		#region UI properties
		[NotInludeAttribute]
		public string PathFilter
		{
			get { return this._pathFilter; }
			set
			{
				this._pathFilter = value;
			}
		}

		[NotInludeAttribute]
		public string Path
		{
			get { return this._path; }
			set
			{
				this._path = value;
				this.RaisePropertyChanged(() => this.Path);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();
				CountingFiles(0);
				this.RaisePropertyChanged(() => this.Tooltip);
				this.RaisePropertyChanged(() => this.ProcessLisner);
				this._openCommand.RaiseCanExecuteChanged();
			}
		}

		[NotInludeAttribute]
		public string SourcePath
		{
			get { return this._sourcePath; }
			set
			{
				this._sourcePath = value;
				this.RaisePropertyChanged(() => this.SourcePath);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();
				//CountingFiles(0);
				this.RaisePropertyChanged(() => this.TooltipSourcePath);
				//this.RaisePropertyChanged(() => this.ProcessLisner);
				this._openCommand.RaiseCanExecuteChanged();
			}
		}

		[NotInludeAttribute]
		public int ProcessLisner
		{
			get { return _processLisner; }
			set
			{
				this._processLisner = value;
				this.RaisePropertyChanged(() => this.ProcessLisner);
			}
		}


		//public bool ListeningSourceFolder
		//{
		//	get { return this._listeningSourceFolder; }
		//	set
		//	{
		//		if (this._isSingleFile == false)
		//		{
		//			this._listeningSourceFolder = value;
		//		}
		//		else
		//		{
		//			this._listeningSourceFolder = false;
		//			return;
		//		}

		//		RaisePropertyChanged(() => ListeningSourceFolder);
		//	}
		//}

		[NotInludeAttribute]
		public bool IsCopyFromSourceFolder  //checkbox on GUI for MIS не используется
		{
			get { return this._isCopyFromSourceFolder; }
			set
			{
				this._isCopyFromSourceFolder = value;

				RaisePropertyChanged(() => IsCopyFromSourceFolder);
				_userSettingsManager.CopyFromSourceSet(this._isCopyFromSourceFolder);
			}
		}

		[NotInludeAttribute]
		public bool IsCopyFromMainForm	
		{
			get { return this._isCopyFromMainForm; }
			set { this._isCopyFromMainForm = value; }
		}

		public bool IsDirectory
		{
			get { return this._isDirectory; }
			set
			{
				this._isDirectory = value;
				this.RaisePropertyChanged(() => this.IsDirectory);

				this._isSingleFile = !this._isDirectory;
				this.RaisePropertyChanged(() => this.IsSingleFile);

				if (_pathWatcher != null)
				{
					_pathWatcher.IsFile = _isSingleFile;
				}

				this.RaisePropertyChanged(() => this.Path);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();

				if (base.InputFileFolderChanged != null)
					base.InputFileFolderChanged(_isDirectory);

				this._openCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsSingleFile
		{
			get { return this._isSingleFile; }
			set
			{
				this._isSingleFile = value;
				this.RaisePropertyChanged(() => this.IsSingleFile);

				this._isDirectory = !this._isSingleFile;
				this.RaisePropertyChanged(() => this.IsDirectory);

				//if (value == true)
				//{
					//this.ListeningSourceFolder = false;
					//this.RaisePropertyChanged(() => this.ListeningSourceFolder);
					//this.IsCopyFromSourceFolder = false;
					//this.RaisePropertyChanged(() => this.IsCopyFromSourceFolder);
				//}

				if (_pathWatcher != null)
				{
					_pathWatcher.IsFile = _isSingleFile;
				}

				this.RaisePropertyChanged(() => this.Path);

				if (base.RaiseCanImport != null)
					base.RaiseCanImport();

				if (base.InputFileFolderChanged != null)
					base.InputFileFolderChanged(_isDirectory);

				this._openCommand.RaiseCanExecuteChanged();
			}
		}

		//[NotInludeAttribute]
		//public string BarcodeMask
		//{
		//	get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
		//}

		//[NotInludeAttribute]
		//public string MakatMask
		//{
		//	get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
		//}

		protected string _barcodeMask = "";				   //!! не убирать протектед
		public string BarcodeMask
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
			get
			{
				if (this._maskViewModel != null)
					return this._maskViewModel.BarcodeMask; // Из GUI
				else
					return this._barcodeMask;
			}
			set { _barcodeMask = value; }
		}

		protected string _makatMask = "";				// !! не убирать протектед
		public string MakatMask
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
			get
			{
				if (this._maskViewModel != null)
					return this._maskViewModel.MakatMask;	 // Из GUI
				else
					return this._makatMask;
			}
			set { _makatMask = value; }
		}

		#endregion

		#region other properties
	   [NotInludeAttribute]
		public string Tooltip
		{
			get { return BuildTooltip(this._path); }
		}

		[NotInludeAttribute]
		public string TooltipSourcePath
		{
			get { return BuildTooltip(this._sourcePath); }
		}

		public DelegateCommand BrowseCommand
		{
			get { return this._browseCommand; }
		}

		public DelegateCommand BrowseSourcePathCommand
		{
			get { return this._browseSourceCommand; }
		}

		[NotInludeAttribute]
		public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
		{
			get { return this._fileChooseDilogRequest; }
		}

		[NotInludeAttribute]
		public InteractionRequest<OpenFolderDialogNotification> FolderChooseDilogRequest
		{
			get { return this._folderChooseDilogRequest; }
		}

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

		_pathWatcher = new AdapterFileWatcher(this, TypedReflection<TemplateAdapterFileFolderViewModel>.GetPropertyInfo(r => r.Path), _isSingleFile);

			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AutoStartImportPda))
			{
				this.IsCopyFromMainForm = true;
			}

			if (base.InputFileFolderChanged != null)
				base.InputFileFolderChanged(_isDirectory);

			observCountingFiles = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4)).Select(x => x);
			disposeObservCountingFiles = observCountingFiles.Subscribe(CountingFiles);
		}

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);

			_pathWatcher.Clear();

			if( disposeObservCountingFiles != null) disposeObservCountingFiles.Dispose();
			//if( disposeObservCopyingFiles != null) disposeObservCopyingFiles.Dispose();
		}

		#endregion

		#region methods

		private bool BrowseCommandCanExecute()
		{
			return true;
		}

		protected void BrowseCommandExecuted()
		{
			if (this._isSingleFile == true)
			{
				OpenFileDialogNotification notification = new OpenFileDialogNotification();
				notification.Filter = this.PathFilter;

				this._fileChooseDilogRequest.Raise(notification, this.FileUploadProcess);
			}
			else
			{
				OpenFolderDialogNotification notification = new OpenFolderDialogNotification();
				this._folderChooseDilogRequest.Raise(notification, this.FolderUploadProcess);
			}
		}

		private bool BrowseSourceCommandCanExecute()
		{
			return true;
		}

		protected void BrowseSourceCommandExecuted()
		{
			OpenFolderDialogNotification notification = new OpenFolderDialogNotification();
			this._folderChooseDilogRequest.Raise(notification, this.FolderSourceProcess);
		}

		protected void FileUploadProcess(OpenFileDialogNotification notification)
		{
			if (notification.IsOK == false) return;

			this.Path = notification.FileName;
		}

		protected void FolderUploadProcess(OpenFolderDialogNotification notification)
		{
			if (notification.IsOk == false) return;

			this.Path = notification.FolderPath;
		}

		protected void FolderSourceProcess(OpenFolderDialogNotification notification)
		{
			if (notification.IsOk == false) return;

			this.SourcePath = notification.FolderPath;
		}

		private bool IsOkFile()
		{
			if (String.IsNullOrEmpty(this._path) == true) return true;

			bool yes = File.Exists(this._path);
			return yes;
		}


		private bool IsOkDirectory()
		{
			if (String.IsNullOrEmpty(this._path) == true) return true;

			return Directory.Exists(this._path);
		}

		public override bool CanImport()
		{
			return (String.IsNullOrEmpty(this._path) == false)
				   && (this._isSingleFile ? this.IsOkFile() : this.IsOkDirectory());
		}

		private bool OpenCommandCanExecute()
		{
			if (String.IsNullOrEmpty(_path))
				return false;

			if (_isDirectory)
			{
				return base.IsPathOkForOpenAsFolder(_path);
			}
			else
			{
				return base.IsPathOkForOpenAsFolder(_path);
			}
		}

		private void OpenCommandExecuted()
		{
			if (String.IsNullOrEmpty(_path))
				return;

			if (_isDirectory)
			{
				Utils.OpenFolderInExplorer(_path);
			}
			else
			{
				base.OpenPathAsFolder(_path);
			}
		}

		private bool OpenCommandSourcePathCanExecute()
		{
			if (String.IsNullOrEmpty(this._sourcePath))
				return false;

			if (_isDirectory)
			{
				return base.IsPathOkForOpenAsFolder(this._sourcePath);
			}
			else
			{
				return base.IsPathOkForOpenAsFolder(this._sourcePath);
			}
		}

		private void OpenCommandSourcePathExecuted()
		{
			if (String.IsNullOrEmpty(this._sourcePath))
				return;

			if (_isDirectory)
			{
				Utils.OpenFolderInExplorer(this._sourcePath);
			}
			else
			{
				base.OpenPathAsFolder(this._sourcePath);
			}
		}

		#endregion

		#region IDataErrorInfo

		public string this[string propertyName]
		{
			get
			{
				if (propertyName == "Path")
				{
					if (this._isSingleFile)
					{
						if (this.IsOkFile() == false)
						{
							return Localization.Resources.Validation_FileNotExist;
						}
					}
					else
					{
						if (this.IsOkDirectory() == false)
						{
							return Localization.Resources.Validation_DirectoryNotExist;
						}
					}
				}
				return String.Empty;
			}
		}

		[NotInludeAttribute]
		public string Error
		{
			get { return String.Empty; }
		}

		public DelegateCommand OpenCommand
		{
			get { return _openCommand; }
		}

		public DelegateCommand OpenSourcePathCommand
		{
			get { return _openSourcePathCommand; }
		}

		#endregion

		protected override void EncondingUpdated()
		{
			RaisePropertyChanged(() => Tooltip);
		}

		public void CountingFiles(long x)
		{
			bool copyFromSource = _userSettingsManager.CountingFromSourceGet(); //слушать или нет
			if (copyFromSource == false) return;
		//	if (this._listeningSourceFolder == false) return;
			if (string.IsNullOrWhiteSpace(this._path) == true) 
			{
				this.ProcessLisner = 0; 
				return;
			}

			if (Directory.Exists(this._path) == true) 
			{
				DirectoryInfo dir = new System.IO.DirectoryInfo(this._path);
				this.ProcessLisner = dir.GetFiles().Length;
			}
			else
			{
				this.ProcessLisner = 0;
			}
		}

		//public void CopyingFiles(long x)
		//{
		//	if (this._listeningSourceFolder == false) return;
		//	if (this._isCopyFromSourceFolder == false) return;
		//	if(string.IsNullOrWhiteSpace(this._sourcePath) == true) return;
		//	if (string.IsNullOrWhiteSpace(this._path) == true) return;

		//	string sourceDir = this._sourcePath;
		//	string unsureDir = System.IO.Path.Combine(this._sourcePath, "unsure");
		//	if (!Directory.Exists(unsureDir))
		//		Directory.CreateDirectory(unsureDir);

		//	string targetDir = this._path;
		//	string currentInventorCode = "";
		//	if(this.CurrentInventor != null) currentInventorCode = this.CurrentInventor.Code;
	
		//	try
		//	{
		//		if (IsPathOkForOpenAsFolder(sourceDir) == true)
		//		{
		//			if (IsPathOkForOpenAsFolder(targetDir) == true)
		//			{
		//				DirectoryInfo dir = new System.IO.DirectoryInfo(this._sourcePath);
		//				//List<string> sourceFiles = Directory.GetFiles(this._sourcePath);
		//				if (dir.GetFiles().Length == 0) return;

		//				FileInfo[] sourceFiles = dir.GetFiles();
		//				foreach (FileInfo fi in sourceFiles)
		//				{
		//					string sourcePath = System.IO.Path.Combine(sourceDir, fi.Name);
		//					string targetPath = System.IO.Path.Combine(targetDir, fi.Name);

		//					if (string.IsNullOrWhiteSpace(currentInventorCode) == false)
		//					{
		//						if (fi.Name.Contains(currentInventorCode) == false)
		//						{
		//							targetPath = System.IO.Path.Combine(unsureDir, fi.Name);
		//						}
		//					}

		//					try
		//					{
		//						if (File.Exists(targetPath) == true)
		//							File.Delete(targetPath);
		//					}
		//					catch (Exception exc)
		//					{
		//						string message = String.Format("CopyingFiles (File.Delete) {0}", targetPath);
		//						WriteErrorExceptionToAppLog(message, exc);
		//					}

		//					try
		//					{
		//						//_logger.Info("Moving to unsure {0} ...", file);
		//						if (File.Exists(sourcePath) == true)
		//						{
		//							File.Move(sourcePath, targetPath);
		//						}
		//					}
		//					catch (Exception exc)
		//					{
		//						string message = String.Format("CopyingFiles (File.Move) {0}", targetPath);
		//						WriteErrorExceptionToAppLog(message, exc);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		WriteErrorExceptionToAppLog("CopyingFiles", exc);
		//		throw;
		//	}
		//}
	}
}