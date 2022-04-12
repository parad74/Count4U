using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Export.Items;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using NLog;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Count4U.Model.Lib.MultiPoint;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Repository.Product.WrapperMulti;

namespace Count4U.Modules.Audit.ViewModels.Export
{
    public class DownloadFromPDAViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private Object DeleteMultiItemLock = new Object();
		private Object AddMultiItemLock = new Object();
		private Object ChangeMultiItemLock = new Object();
        private const string DefaultTerminalID = "HT630";

        private readonly IDBSettings _dbSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWrapperMultiRepository _wrapperMultiRepository;
        private readonly IUserSettingsManager _userSettingsManager;
		public readonly IServiceLocator _serviceLocator;

        private readonly InteractionRequest<OpenFolderDialogNotification> _openFolderRequest;

        private readonly DelegateCommand _browseCommand;
        private readonly DelegateCommand _downloadCommand;
		
        private readonly DelegateCommand _refreshCommand;
        private readonly DelegateCommand _closeCommand;
        private readonly DelegateCommand _openCommand;

        private int _uploadValue;
        private bool _isChecked;
        private bool _isCheckedEnabled;
        private string _path;

        private string _downloadButtonText;

        private bool _isDownload;
		private bool _isDesposed;
        private bool _isFinished;
        private bool _canRefresh;
        private bool _canBrowse;

        private bool _isProgressBarVisible;

        private readonly ObservableCollection<UploadToPdaItemViewModel> _items;
		//private List<FileInfo> _uploadingFiles;
		//private List<string> _uploadingFilesNames;
		//private long _uploadingFilesLength;

        private bool _isInitialized;
        private bool _isInitializingTextBlockVisible;

		private string _exportERPAdapterCode;

		public DownloadFromPDAViewModel(
            IContextCBIRepository contextCbiRepository,
            IDBSettings dbSettings,
            IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
         //   IWrapperMultiRepository wrapperMultiRepository,
            IUserSettingsManager userSettingsManager)
            : base(contextCbiRepository)
        {
            this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._eventAggregator = eventAggregator;
			this._dbSettings = dbSettings;
			this._openFolderRequest = new InteractionRequest<OpenFolderDialogNotification>();
			this._browseCommand = new DelegateCommand(BrowseCommandExecuted, BrowseCommandCanExecuted);
			this._downloadCommand = new DelegateCommand(DownloadCommandExecuted, DownloadCommandCanExecute);
		
			this._openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);

			this._items = new ObservableCollection<UploadToPdaItemViewModel>();
			this._closeCommand = new DelegateCommand(CloseCommandExecute);
			this._refreshCommand = new DelegateCommand(RefreshCommandExecuted, RefreshCommandCanExecute);

			this._downloadButtonText = Localization.Resources.View_DownloadFromPda_btnDownload;
			this._wrapperMultiRepository = this._serviceLocator.GetInstance<IWrapperMultiRepository>(WrapperMultiEnum.WrapperMultiRepository.ToString());//TODO @@

			this._isFinished = false;
			this._canRefresh = true;

			this._isCheckedEnabled = true;

			this._canBrowse = true;

			this._isProgressBarVisible = false;
			this._isInitialized = false;
			this._isInitializingTextBlockVisible = false;

			this._isDesposed = true;

        }

        public DelegateCommand BrowseCommand
        {
			get { return this._browseCommand; }
        }

        public InteractionRequest<OpenFolderDialogNotification> OpenFolderRequest
        {
			get { return this._openFolderRequest; }
        }

        public string Path
        {
			get { return this._path; }
            set
            {
				this._path = value;
                RaisePropertyChanged(() => Path);

				this._downloadCommand.RaiseCanExecuteChanged();
				this._openCommand.RaiseCanExecuteChanged();
            }
        }

        public int UploadValue
        {
			get { return this._uploadValue; }
            set
            {
				this._uploadValue = value;
                RaisePropertyChanged(() => UploadValue);
            }
        }

      
		public DelegateCommand DownloadCommand
        {
			get { return this._downloadCommand; }
        }
		

        public bool IsChecked
        {
			get { return this._isChecked; }
            set
            {
				this._isChecked = value;
                RaisePropertyChanged(() => IsChecked);
				lock (ChangeMultiItemLock)
				{
					foreach (UploadToPdaItemViewModel item in this._items)
					{
						item.IsChecked = this._isChecked;
					}
				}
            }
        }

        public ObservableCollection<UploadToPdaItemViewModel> Items
        {
			get { return this._items; }
        }

        public DelegateCommand CloseCommand
        {
			get { return this._closeCommand; }
        }

        public DelegateCommand RefreshCommand
        {
			get { return this._refreshCommand; }
        }

        public string DownloadButtonText
        {
			get { return this._downloadButtonText; }
        }

        public bool IsDownload
        {
			get { return this._isDownload; }
            set
            {
				this._isDownload = value;
                RaisePropertyChanged(() => IsDownload);

				if (this._isDownload == true)
					this._downloadButtonText = Localization.Resources.View_DownloadFromPda_btnStop;
                else
					this._downloadButtonText = Localization.Resources.View_DownloadFromPda_btnDownload;

                RaisePropertyChanged(() => DownloadButtonText);
            }
        }

        public bool IsCheckedEnabled
        {
			get { return this._isCheckedEnabled; }
            set
            {
				this._isCheckedEnabled = value;
                RaisePropertyChanged(() => IsCheckedEnabled);
            }
        }

		public bool IsDesposed
        {
			get { return this._isDesposed; }
            set
            {
				this._isDesposed = value;
				RaisePropertyChanged(() => IsDesposed);
            }
        }

		

        public bool IsProgressBarVisible
        {
			get { return this._isProgressBarVisible; }
            set
            {
				this._isProgressBarVisible = value;
                RaisePropertyChanged(() => IsProgressBarVisible);
            }
        }

        public bool IsInitializingTextBlockVisible
        {
			get { return this._isInitializingTextBlockVisible; }
            set
            {
				this._isInitializingTextBlockVisible = value;
                RaisePropertyChanged(() => IsInitializingTextBlockVisible);
            }
        }

        public DelegateCommand OpenCommand
        {
			get { return this._openCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._uploadValue = 0;

			if (base.State.CurrentInventor != null)
			{
				this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentInventor.Code);
				this._exportERPAdapterCode = base.State.CurrentInventor.ExportERPAdapterCode;
			}
			else if (base.State.CurrentBranch != null)
			{
				this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentBranch.Code);
				this._exportERPAdapterCode = base.State.CurrentBranch.ExportERPAdapterCode;
			}
			else if (base.State.CurrentCustomer != null)
			{
				this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentCustomer.Code);
				this._exportERPAdapterCode = base.State.CurrentCustomer.ExportERPAdapterCode;
			}

            using (new CursorWait())
            {
				
				this.Build();  // build GUI
				//Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
				this.AssignTerminalID("OnNavigatedTo", true);
            }
        }


        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
			_logger.Info("OnNavigatedFrom");

			this.BeforeClose();
			this.ClearEventForm();
			_logger.Info("OnNavigatedFrom  + Clear");
        }

        private bool BrowseCommandCanExecuted()
        {
            return _canBrowse;
        }

        private void BrowseCommandExecuted()
        {
            OpenFolderDialogNotification notification = new OpenFolderDialogNotification();
			if (Directory.Exists(this._path) == true)
            {
				notification.FolderPath = this._path;
            }
            this._openFolderRequest.Raise(notification, FolderOpen);
        }

        private void FolderOpen(OpenFolderDialogNotification notification)
        {
            if (notification.IsOk == true)
            {
                this.Path = notification.FolderPath;
            }
        }

        private bool DownloadCommandCanExecute()
        {
			if (this._isInitialized == false) return false;

			if (this._isFinished) return false;

			if (this._isDownload) return true;

			if (Directory.Exists(this._path) == false)
                return false;

			//if (Directory.EnumerateFiles(this._path).Any() == false)
			//	return false;

			if (this._items.Any(r => r.IsChecked) == false)
                return false;

            //   return _items.Where(r => r.IsChecked).All(r => !String.IsNullOrWhiteSpace(r.Device));

            return true;
        }

		// cм     private void UploadCommandExecuted()
        private void DownloadCommandExecuted()
        {
			if (this._isDownload == true)  // Stop command
            {
				using (new CursorWait())
				{
					try
					{
						this.StopButtonPress();  // визуальный эффект и присваивание имени "abort" процессу

						//lock (ChangeMultiItemLock)
						//{
						//	foreach (UploadToPdaItemViewModel item in _items)
						//	{
						//		item.UploadUnit.StateChanged -= UploadPdaUnit_StateChanged;
						//		item.PropertyChanged -= MultiItem_PropertyChanged;
						//	}
						//}
                        int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
                        Thread.Sleep(uploadWakeupTimeOutFinish1);

						//this.Done("UploadCommandExecuted");
					}
					catch (Exception exc)
					{
						_logger.ErrorException("DownloadCommandExecuted - Stop()", exc);
					}
				}
		    }// end Stop command

			else // Download command
            {
				try
				{
					lock (ChangeMultiItemLock)
					{
						foreach (UploadToPdaItemViewModel item in this._items)
						{
							item.wrapperMulti.StateChanged += UploadPdaUnit_StateChanged;
							item.PropertyChanged += MultiItem_PropertyChanged;
						}
					}
				}
				catch (Exception exc)
				{
					_logger.ErrorException("DownloadCommandExecuted - Download()", exc);
				}

				this.Download();
			}// end Download command
			
        }

		// по разным причинам все сделано
		// можно отписываться от событий и Dispose for Multi
		// и закрывать порты
        private void Done(string from, bool abort = false)
        {
			this._downloadButtonText = Localization.Resources.View_DownloadFromPda_btnDownload;
			this.RaisePropertyChanged(() => this.DownloadButtonText);

			int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
            Thread.Sleep(uploadWakeupTimeOutFinish1);
			//this.AbortThreads("Done");
			_logger.Info("Done from : " + from);

			try
			{
				lock (ChangeMultiItemLock)
				{
					foreach (UploadToPdaItemViewModel item in this._items)
					{
						item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
						//item.PropertyChanged -= MultiItem_PropertyChanged;
					}
				}

				Thread.Sleep(uploadWakeupTimeOutFinish1);
				//-----------postUpload actions

				if (this._userSettingsManager.UploadOptionsHT630_AfterUploadPerformWarmStartGet() == true)
				{
					//List<Multi> multisStart = this._items.Where(x => x.Device != "not started").Select(x => x.UploadWrapperMulti.Multi).ToList();
					List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
					this._wrapperMultiRepository.WarmStartAllPDA(wrapperMultisStart, "Done");
					int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
					Thread.Sleep(uploadWakeupTimeOut1);
				}

				if (this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileNeedDoGet() == true)
				{
					List<string> files = this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileListGet().Split(";".ToCharArray()).ToList<string>();
					List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
					this._wrapperMultiRepository.RunFilesAllPDA(wrapperMultisStart, files, "Done");
					int uploadWakeupTimeOut2 = this._userSettingsManager.UploadWakeupTimeGet() * 2000;
					Thread.Sleep(uploadWakeupTimeOut2);
				}

				//--------------------------------

				if (abort == false)
				{
					//List<IUploadPdaUnit> units = this._items.Select(x => x.UploadUnit.Multi).ToList();
					//List<Multi> multis = this._items.Select(x => x.UploadWrapperMulti.Multi).ToList(); //@@

					//List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList();
					List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
					this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultisStart, from + "- >Done", this._userSettingsManager.UploadWakeupTimeGet());
				}
				
			}
			catch (Exception exc)
			{
				_logger.ErrorException("UploadCommandExecuted - Done()", exc);
			}

			Thread.Sleep(uploadWakeupTimeOutFinish1);
			_logger.Info("AbortThreadAllThreads finish " + "Done");

			this._canRefresh = true;
			this._refreshCommand.RaiseCanExecuteChanged();

			this.IsDesposed = true;

			this._isFinished = true;
			this._downloadCommand.RaiseCanExecuteChanged();

			this.IsDownload = false; 

			this.IsProgressBarVisible = false;
			
		
        }

		private void Finish()
		{
			try
			{
				lock (ChangeMultiItemLock)
				{
					foreach (UploadToPdaItemViewModel item in this._items)
					{
						item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
						item.PropertyChanged -= MultiItem_PropertyChanged;
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("Done", exc);
			}
		
		}

		//визуальный эффект и присваивание имени "abort" процессу
        private void StopButtonPress()
        {
            //lock (ChangeMultiItemLock)
            //{
            //    //foreach (UploadToPdaItemViewModel item in _items.Where(r => r.IsChecked))
				//int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 100;
				foreach (UploadToPdaItemViewModel item in this._items)
				{
					item.AbortItemUploadThread();  // визуальный эффект и присваивание имени "abort" процессу
				}
				

				//foreach (UploadToPdaItemViewModel item in this._items)
				//{
				//	item.UploadUnit.StateChanged -= UploadPdaUnit_StateChanged;
				//}

				//Thread.Sleep(uploadWakeupTimeOutFinish1);
            //}


			//List<IUploadPdaUnit> units = this._items.Select(x => x.UploadUnit).ToList();
			//this._uploadPdaRepository.AbortThreadAllThreads(units, "Stop");

			//Thread.Sleep(uploadWakeupTimeOutFinish1);
			//_logger.Info("AbortThreadAllThreads finish " + "Stop");

			//this.AbortThreads("Stop");

            this.IsDownload = false;

			this._canRefresh = true;
			this._refreshCommand.RaiseCanExecuteChanged();

			this._isFinished = true;
			this._downloadCommand.RaiseCanExecuteChanged();

			this.IsDesposed = true;

			this.IsProgressBarVisible = false;

			_logger.Info("StopButtonPress end");

        }

		//private void Upload() см 

		private void Download()
		{
			_logger.Info("Download start");
			using (new CursorWait())
			{
				this.AssignTerminalID("Download", false);
			}

			try
			{
				if (Directory.Exists(this._path) == false) return;
				//List<string> filesToProcess = Directory.GetFiles(this._path).ToList();

				this.IsDownload = true;
				this.IsDesposed = false;

				//if (this._userSettingsManager.UploadOptionsHT630_DeleteAllFilePDAGet() == true)
				//{
				//	List<string> excludeSiles = this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteGet().Split(";".ToCharArray()).ToList<string>();
				//	List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList(); 
				//	this._wrapperMultiRepository.DeleteFilesAllPDA(wrapperMultisStart, excludeSiles, "Upload");
				//	int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
				//	Thread.Sleep(uploadWakeupTimeOut1);
				//}

				//foreach (UploadToPdaItemViewModel item in this._items)
				//{
					// --------- fill FileLists for Upload
					//	item.ClearUploadingFileLists();

					//	if (item.IsChecked == true)
					//	{
					//		item.AddFileInfoFromFilePathList(filesToProcess);
					//		var localFilesToProcess = filesToProcess.ToList();

					//		if (String.IsNullOrEmpty(item.Device) == false && item.Device != "not started") //substitute jeng.exe
					//		{
					//			//файлы в папке для заливки (в зависимости от типа терминала)
					//			List<string> terminalFilePathList = BuildPathToJENGEXE(item.Device);
					//			//файлы на терминале
					//			List<string> filesOnPDA = item.wrapperMulti.GetDirectory(false).Select(x=>x.name).ToList(); //@@

					//			foreach (var terminalFilePath in terminalFilePathList)
					//			{
					//				string fileName = System.IO.Path.GetFileName(terminalFilePath);
					//				//если на терминала нет файла с  именем fileName - то его добавить в список upload files - localFilesToProcess
					//				// иначе не добавлять и не заливать
					//				if (filesOnPDA.Contains(fileName) == false)
					//				{
					//					localFilesToProcess.Add(terminalFilePath);
					//				}
					//			}

					//			item.AddFileInfoFromFilePathList(terminalFilePathList);
					//		}
					//		else
					//		{
					//			_logger.Info("Download : item.Device - " + item.Device + "  Is Null Or Empty");
					//		}

					//	}
					//	else
					//	{
					//		item.ButtonEnabled = false;
					//		item.IsCheckedEnabled = false;
					//		item.ButtonVisible = false;
					//	}
				//}

					
				foreach (UploadToPdaItemViewModel item in this._items)
				{
					item.ClearUploadingFileLists();

					if (item.IsChecked == true)
					{
						List<ItemDir> filesOnPDA = item.wrapperMulti.GetDirectory(true);
						item.AddFileInfoToDownloadFileList(filesOnPDA);
					}
				}
				
				//============  Download process
					Task<bool>[] downloadFileTasks = new Task<bool>[this._items.Where(x => x.IsChecked == true).Count()];
					int countTask = downloadFileTasks.Count();
					int indexTask = 0;
					for (int i = 0; i < this._items.Count; i++)
					{
						if (this._items[i].IsChecked == true)
						{
							if (indexTask < countTask)
							{
								string  pathIn = this._path;
								string pathPort = this._path + @"\" + this._items[i].Port;
								if (Directory.Exists(pathPort) == false)
									try
									{
										Directory.CreateDirectory(pathPort);
										pathIn = pathPort;
									}
									catch (Exception ex) { _logger.ErrorException("Download : CreateDirectory : " + pathPort, ex); }
								//List<string> filesOnPDA = this._items[i].wrapperMulti.GetDirectory(false).Select(x => x.name).ToList();
								downloadFileTasks[indexTask] = new Task<bool>(() => { var ret = this._items[i].wrapperMulti.DownloadFiles(this._items[i]._downloadFilesNames, pathIn); return ret; });
								downloadFileTasks[indexTask].Start();
								bool resultStartDownload = downloadFileTasks[indexTask].Result;
								indexTask++;

								if (resultStartDownload == true)
								{
									this._items[i].StartGUIUpdate();
								}
							}
							else
							{
								_logger.Info("Download - it's strange: indexTask [" + indexTask.ToString() + "]  > " + " countTask [" + countTask.ToString() + "]");
							}

						}
					} //for
					Task.WaitAll(downloadFileTasks);



					_logger.Info("Download + Task.WaitAll(downloadFileTasks) end");
					//----------------------

					this._canRefresh = false;
					this._refreshCommand.RaiseCanExecuteChanged();

					this._canBrowse = false;
					this._browseCommand.RaiseCanExecuteChanged();

					this.IsCheckedEnabled = false;

					this.IsProgressBarVisible = true;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("Download", exc);
			}
		}
		

        private void ClearEventForm()
        {
            using (new CursorWait())
            {
                try
                {
					lock (DeleteMultiItemLock)
					{
						foreach (UploadToPdaItemViewModel item in this._items)
						{
							//!! item.UploadUnit.StateChanged -= UploadPdaUnit_StateChanged;
							item.PropertyChanged -= MultiItem_PropertyChanged;
							//if (item.UploadUnit.SerialIsOpen() == true)
							//{
							//	//item.UploadUnit.Multi.Stop();
							//	item.UploadUnit.Close(); //??          //f (serial != null && serial.IsOpen)	serial.Close();
							//}
						}


						try
						{
							lock (ChangeMultiItemLock)
							{
								foreach (UploadToPdaItemViewModel item in this._items)
								{
									item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
								}
							}
						}
						catch (Exception exc)
						{
							_logger.ErrorException("UploadCommandExecuted - Stop()", exc);
						}
					}//lock

					//int uploadWakeupTimeOut = _userSettingsManager.UploadWakeupTimeGet() * 1000;
					//Thread.Sleep(uploadWakeupTimeOut);
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("Clear", exc);
                }
            }
        }

		private void Build(bool reWakeup = false)
        {

            try
            {
				
				Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
                _isInitialized = false;

				this._items.Clear();
				int baudrate = this.GetBaudrateFormConfig();

				List<IWrapperMulti> uploadPdaWrapperMultiList = this._wrapperMultiRepository.GetPortsAndWakeUP(baudrate, "Build", reWakeup).ToList();

				if (reWakeup == true)
				{
					int uploadWakeupTimeOut = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
					Thread.Sleep(uploadWakeupTimeOut);

					if (this._userSettingsManager.UploadOptionsHT630_CurrentDataPDAGet() == true)
					{
						//List<Multi> multis = this._items.Select(x => x.UploadWrapperMulti.Multi).ToList(); // не известно что будет если не поднят @@
						List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList(); 
						
						this._wrapperMultiRepository.SetDateTimeAllPDA(wrapperMultis, DateTime.Now, "Build");
						int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 100;
						Thread.Sleep(uploadWakeupTimeOut1);
					}
				}

				lock (AddMultiItemLock)
				{
					foreach (IWrapperMulti uploadPdaWrapperMulti in uploadPdaWrapperMultiList.OrderBy(r => Regex.Replace(r.Multi.Port, @"[^\d]", "")))
					{
						//if (uploadPdaUnit.SerialIsOpen() == false) continue;
						//!!uploadPdaUnit.StateChanged += UploadPdaUnit_StateChanged;

						string terminalId = "not started";
						UploadToPdaItemViewModel item = new UploadToPdaItemViewModel(uploadPdaWrapperMulti, this._serviceLocator);
						item.Port = uploadPdaWrapperMulti.ComPortStatic;
						item.Device = terminalId;
						item.File = String.Empty;
						item.Number = String.Empty;
						item.Value = String.Empty;
						item.Progress = 0;

						item.PropertyChanged += MultiItem_PropertyChanged;

						this._items.Add(item);
					}


					_downloadCommand.RaiseCanExecuteChanged();
				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
            }

			//Task.Factory.StartNew(() =>
			//	{
			//		Thread.Sleep(_userSettingsManager.UploadWakeupTimeGet());
			//		AssignTerminalID("Build");
			//	});

			
			//AssignTerminalID("Build");
        }

		private int GetBaudrateFormConfig()
		{
			int baudrate = 57600;
			bool baudrateGetFormConfig = this._userSettingsManager.UploadOptionsHT630_BaudratePDAGet();
			if (baudrateGetFormConfig == true)
			{
				Int32.TryParse(this._userSettingsManager.UploadOptionsHT630_BaudratePDAItemGet(), out baudrate);
			}
			return baudrate;
		}

        private void AssignTerminalID( string from, bool abortThreadAfter = false)
        {
            try
            {
				this.IsDesposed = false;
				_logger.Info("AssignTerminalID start from " + from);

				//----------------- WakeUpAllPorts
				int baudrate = this.GetBaudrateFormConfig();

				List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList();
				this._wrapperMultiRepository.WakeUpAllPorts(wrapperMultis, baudrate, "AssignTerminalID::WakeUpAllPorts " + from);

				int uploadWakeupTimeOut = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
				Thread.Sleep(uploadWakeupTimeOut);

				_logger.Info("AssignTerminalID after wakeUp " + from);

				//--------------------GetTerminalID and set in GUI
			
					//Task<string>[] getIDTasks = new Task<string>[_items.Count()];
				
					this._wrapperMultiRepository.GetTerminalIDAllPDA(wrapperMultis, "AssignTerminalID::GetTerminalIDAllPDA " + from);
					for (int i = 0; i < this._items.Count; i++)
					{
						string terminalId = "not started";
						////----------
						//lock (AddMultiItemLock)
						//{
						//getIDTasks[i] = new Task<string>(() => { var id = this._items[i].UploadUnit.GetTerminalID(); return id; });
						//getIDTasks[i].Start();

						//terminalId = getIDTasks[i].Result;
						//} //lock
						//------------
						terminalId = this._items[i].wrapperMulti.GetTerminalID(); //add
						Utils.RunOnUI(() => this._items[i].Device = terminalId);
					}
				
					//Task.WaitAll(getIDTasks);

	
				int uploadWakeupTimeOutFinish = _userSettingsManager.UploadWakeupTimeGet() * 1000;
				Thread.Sleep(uploadWakeupTimeOutFinish);
				_logger.Info("AssignTerminalID finish " + from);

			

				// ------------------AbortThreadAllThreads
				if (abortThreadAfter == true)  // и все закрываем
				{
					this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "AssignTerminalID::" + from, this._userSettingsManager.UploadWakeupTimeGet());
					Thread.Sleep(uploadWakeupTimeOutFinish);
					this.IsDesposed = true;
					_logger.Info("AbortThreadAllThreads finish " + from);
				}
				else  // или готовим к uploade
				{
					Thread.Sleep(uploadWakeupTimeOutFinish);
					var units1 = this._items.Where(x => x.Device == "not started").Select(x => x).ToList();
					foreach (var unit in units1)
					{
						unit.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
						unit.PropertyChanged -= MultiItem_PropertyChanged;
					}

					List<IWrapperMulti> wrapperMultisNotStart = this._items.Where(x => x.Device == "not started").Select(x => x.wrapperMulti).ToList();
					this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultisNotStart, "AssignTerminalID::units1: not started " + from, this._userSettingsManager.UploadWakeupTimeGet());
					Thread.Sleep(uploadWakeupTimeOutFinish);


					//------- SetDateTimeAllPDA
					if (this._userSettingsManager.UploadOptionsHT630_CurrentDataPDAGet() == true)
					{
						List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
						this._wrapperMultiRepository.SetDateTimeAllPDA(wrapperMultisStart, DateTime.Now, "AssignTerminalID");
						int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
						Thread.Sleep(uploadWakeupTimeOut1);
					}
					_logger.Info("AbortThreadAllThreads finish " + from);


				}

                Utils.RunOnUI(() => IsInitializingTextBlockVisible = false);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("AssignTerminalID", exc);
            }
            finally
            {
                _isInitialized = true;
                _isFinished = false;
                _downloadCommand.RaiseCanExecuteChanged();
            }
        }


		void WaitForTwoSecondsAsync(Action callback)		// Asynchronous NON-BLOCKING method
		{
			new Timer(_ => callback()).Change(2000, -1);
		}


		void WaitForTenSecondsAsync(Action callback)		// Asynchronous NON-BLOCKING method
		{
			new Timer(_ => callback()).Change(10000, -1);
		}

		void WaitForSecondsAsync(Action callback, int sec)		// Asynchronous NON-BLOCKING method
		{
			sec = sec * 1000;
			new Timer(_ => callback()).Change(sec, -1);
		}

		//void UploadPdaUnit_StateChanged(IWrapperMulti wrapperMulti, UploadPdaMPStage stage, int state, string text, int current) // идет типа от EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size); в multi
		void UploadPdaUnit_StateChanged(IWrapperMulti wrapperMulti, UploadPdaMPStage stage, int state, string text, int current) // идет типа от EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size); в multi
        {
			//MPStage - оставлю пока UploadPdaUnit_StateChanged - может объединю
			//public enum MPStage { Bell = 0x07, Upload = 0x4C, Download = 0x55 };  //MPStage stage
			//public enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 }; //int state

			if (wrapperMulti.Multi.Address == 0x80) return;
			if (wrapperMulti.Multi.SerialIsOpen() == false) return;
	
            Utils.RunOnUI(() =>
            {

				UploadToPdaItemViewModel item = this._items.FirstOrDefault(r => r.Port == wrapperMulti.Multi.Port);

                if (item == null) return;
				if (item.Device == "not started") return;
				if (item.IsChecked == false) return;
				if (item._uploadingFilesNames == null) return;
				if (item._uploadingFiles == null) return;

                try
                {
                    if (state < 0)
                    {
						try
						{
							_logger.Info("Error in UploadPdaUnit_StateChanged  state : " + state);
							MPError errorFromUpload = (MPError)state;
							_logger.Info("Error in UploadPdaUnit_StateChanged enum :  " + errorFromUpload);//enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 }; //int state
						}
						catch { }
                        item.Error();
                    }
                    else
                    {
						//int fileIndex = item._uploadingFilesNames.IndexOf(text.ToLower());
						//if (fileIndex >= 0)
						//{
						//	long doneTotal = 0;

	                        switch (stage)
                            {
                                case UploadPdaMPStage.Bell:
									break;
								case UploadPdaMPStage.Download:
									{
										int fileIndexDownload = item._downloadFilesNames.IndexOf(text.ToLower());
										if (fileIndexDownload >= 0)
										{
											item.Value = String.Format("{0}/{1}", state, current);//закочено/всего 
											item.File = text;
											item.Number = String.Format("{0}/{1}", fileIndexDownload + 1, item._downloadFilesNames.Count);
											double progress = 0;
											if (item._downloadFilesNames.Count != 0)
											{
												progress = (double)(fileIndexDownload) / (double)item._downloadFilesNames.Count;
											}
											item.Progress = 0;
											if (progress >= 0 && progress <= 1)
											{
												item.Progress = (int)(progress * 100);
											}
											if (fileIndexDownload + 1 == item._downloadFilesNames.Count)
											{
												if (state > 0)
												{
													if (state == current)
													{
														item.Progress = 100;
													}
												}
											}
											UpdateTotalProgress();

											//закочено >= всего && последний файл из списка файлов
											if (state >= current && fileIndexDownload + 1 == item._downloadFilesNames.Count)			//state/current  //закочено/всего //1075/1185
											{
												item.Finish();
											}
										}
									}
									break;

								case UploadPdaMPStage.Upload:
									{
										int fileIndex = item._uploadingFilesNames.IndexOf(text.ToLower());
										if (fileIndex >= 0)
										{
											//long doneTotal = 0;
											//item.DoneDuringSession = doneTotal;
											item.Value = String.Format("{0}/{1}", state, current);//закочено/всего 
											item.File = text;
											item.Number = String.Format("{0}/{1}", fileIndex + 1, item._uploadingFiles.Count);
											double progress = 0;
											if (item._uploadingFiles.Count != 0)
											{
												progress = (double)(fileIndex) / (double)item._uploadingFiles.Count;
											}
											item.Progress = 0;
											if (progress >= 0 && progress <= 1)
											{
												item.Progress = (int)(progress * 100);
											}
											if (fileIndex + 1 == item._uploadingFiles.Count)
											{
												if (state > 0)
												{
													if (state == current)
													{
														item.Progress = 100;
													}
												}
											}

											UpdateTotalProgress();

											//закочено >= всего && последний файл из списка файлов
											if (state >= current && fileIndex + 1 == item._uploadingFiles.Count)			//state/current  //закочено/всего //1075/1185
											{
												item.Finish();
											}
										}
										break;
									}
                                default:
                                    throw new ArgumentOutOfRangeException("stage");
                            }
                    }

					// STOP
					if (this._items.Where(r => r.IsChecked).All(r => r.Cancelled))
					{
						using (new CursorWait())
						{
							int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
							Thread.Sleep(uploadWakeupTimeOutFinish1);
							List<IWrapperMulti> wrapperMultis = this._items.Where(x => x.IsChecked == true).Select(x => x.wrapperMulti).ToList();
							//List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
							this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "UploadPdaUnit_StateChanged + Cancelled", this._userSettingsManager.UploadWakeupTimeGet());
							Thread.Sleep(uploadWakeupTimeOutFinish1);
						}
					}
						   
                    //  item.Device = uploadPDAUnit.GetTerminalID();
					// по разным причинам все сделано
					// можно отписываться от событий и Dispose for Multi
					if (this._items.Where(r => r.IsChecked).All(r => r.Done || r.IsError || r.Cancelled))
                    {
						using (new CursorWait())
						{
							this.Done("UploadPdaUnit_StateChanged");
						}
	                }
                }
                catch (Exception e)
                {
                    _logger.ErrorException("UploadPdaUnit_StateChanged", e);
                }
		    });
        }

	

        private void UpdateTotalProgress()
        {

			List<UploadToPdaItemViewModel> itemsInvolved = this._items.Where(r => r.IsChecked).Where(r => r.Cancelled == false && r.IsError == false).ToList();


             if (itemsInvolved.Count == 0)
             {
                 UploadValue = 0;
                 return;
             }
	
			 long doneTotalProgress = 0;
			 if (itemsInvolved.Count == 0)
			 {
				 this.UploadValue = 0;
				 return;
			 }

			
			 foreach (UploadToPdaItemViewModel item in itemsInvolved)
			 {
				 doneTotalProgress = doneTotalProgress + item.Progress;
			 }

			double progress = (double)doneTotalProgress / (double)(itemsInvolved.Count*100);
            this.UploadValue = 0;
            if (progress >= 0 && progress <= 1)
            {
                this.UploadValue = (int)(progress * 100);
            }
        }

        void MultiItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UploadToPdaItemViewModel item = sender as UploadToPdaItemViewModel;
            if (item == null) return;

            if (e.PropertyName == "IsChecked")
            {
                _downloadCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "Cancelled" || e.PropertyName == "IsError")
            {
                UpdateTotalProgress();
				//все отенены
                //if (this._items.Where(r => r.IsChecked).All(r=>r.Cancelled))
                //{
                //    this.Done("MultiItem_PropertyChanged + All Cancelled", true);
                //}
                //else
                //{
                //    // по разным причинам все сделано
                //    // можно отписываться от событий + Dispose for Multi
                //    if (this._items.Where(r => r.IsChecked).All(r => r.Done || r.IsError || r.Cancelled))
                //    {
                //        using (new CursorWait())
                //        {
                //            this.Done("MultiItem_PropertyChanged");
                //        }
                //    }
                //}
            }
        }

        private void CloseCommandExecute()
        {
			//this.BeforeClose();

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

		private void BeforeClose()
		{
			using (new CursorWait())
			{
				try
				{
					_logger.Info("CloseCommandExecute start");
					if (this.IsDesposed == false)
					{
						//визуальный эффект и присваивание имени "abort" процессу
						//this.StopButtonPress();
						foreach (UploadToPdaItemViewModel item in this._items)
						{
								item.AbortItemUploadThread();  // визуальный эффект и присваивание имени "abort" процессу
						}

						_logger.Info("CloseCommandExecute:: StopButtonPress() finish");
						int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
						Thread.Sleep(uploadWakeupTimeOutFinish1);

						//this.Done("CloseCommandExecute");
					}
					_logger.Info("CloseCommandExecute:: Done() finish");
				}
				catch (Exception exc)
				{
					_logger.ErrorException("CloseCommandExecute :: StopButtonPress() + Done()", exc);
				}
			}
		}

        private bool RefreshCommandCanExecute()
        {
            return _canRefresh;
        }

        private void RefreshCommandExecuted()
        {
            this.UploadValue = 0;

            this.IsDownload = false;

            this.IsChecked = false;
            this.IsCheckedEnabled = true;

            this._isFinished = false;

            this._canRefresh = true;
            this._refreshCommand.RaiseCanExecuteChanged();

            this._canBrowse = true;
            this._browseCommand.RaiseCanExecuteChanged();

            this._downloadCommand.RaiseCanExecuteChanged();
			this.IsInitializingTextBlockVisible = true;

          	try
			{
				lock (ChangeMultiItemLock)
				{
					foreach (UploadToPdaItemViewModel item in this._items)
					{
						item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
						item.PropertyChanged -= MultiItem_PropertyChanged;
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("UploadCommandExecuted - Stop()", exc);
			}

			using (new CursorWait())
			{
				//this.AbortThreads("RefreshCommandExecuted - 1");

				//lock (DeleteMultiItemLock)
				//{
					//foreach (UploadToPdaItemViewModel item in _items)
					//{
					//	item.UploadUnit.MultiAbortThread();
					//	//item.Clear(); 
					//}
					//int uploadWakeupTimeOut = _userSettingsManager.UploadWakeupTimeGet() * 1000;
					//Thread.Sleep(uploadWakeupTimeOut);
					//this._items.Clear();
				//}
				
				this.Build();  // build GUI
				Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
				this.AssignTerminalID("RefreshCommandExecuted", true);
			}
        }

		//файлы в папке для заливки (в зависимости от типа терминала)
        private List<string> BuildPathToJENGEXE(string terminalId)
        {
			List<string> result = new List<string>();
			_logger.Info("BuildPathToJENGEXE :" + terminalId);

            string terminalPath = _dbSettings.TerminalIDPath();
            terminalPath = terminalPath + @"\" + terminalId;

			if (Directory.Exists(terminalPath) == true)
			{
				result = Directory.GetFiles(terminalPath).ToList();
				//for (int i = 0; i < files.Length; i++)
				//{
				//	result.Add(files[i]);
				//}
				//string jengexePath = System.IO.Path.Combine(terminalPath, terminalId, "JENG.EXE");

				//if (File.Exists(jengexePath) == true)
				//{
				//	result = jengexePath;
				//}
				//else
				//{
				//	_logger.Info("BuildPathToJENGEXE: " + terminalId + " - File Path" + jengexePath + " not exists");
				//}
			}
			else
			{
				_logger.Info("BuildPathToJENGEXE: " + terminalId + " - File Path" + terminalPath + " not exists" );
			}

            return result;
        }

        private bool OpenCommandCanExecute()
        {
            return Directory.Exists(_path);
        }

        private void OpenCommandExecuted()
        {
            Utils.OpenFolderInExplorer(_path);
        }

        //Sample
        //private void MPStateReceiver(Multi obj, MultiPoint.MPStage stage, int state, string text, int current)
        //{
        //	UploadToPdaItemViewModel item = null;
        //	foreach (UploadToPdaItemViewModel it in this._items)
        //	{
        //		if (it.Port ==  obj.GetCOMPort())
        //			item = it;
        //	}

        //	switch (stage)
        //	{
        //		case MPStage.Bell:
        //			item.SubItems[0].Text = (Convert.ToChar(obj.GetAddress())).ToString();
        //			item.SubItems[2].Text = "WakeUp " + item.SubItems[0].Text;
        //			break;
        //		case MPStage.Download:
        //			item.SubItems[2].Text = "Down: " + text + " " + state + " / " + current;
        //			break;
        //		case MPStage.Upload:
        //			item.SubItems[2].Text = "Up: " + text + " " + state + " / " + current;
        //			break;
        //	}

        //	item.Port = это ключ он заполняется при инциализации словаря
        //	item.Device = obj.GetTerminalID() или obj.GetTerminalVertion() 
        //	item.File = текущий файл ? 
        //	item.Number = "5/10"; номер файла / всего файлов
        //	item.Value =  state + " / " + current;
        //	item.Process = (state  /  current) * 100;
        //	если state < 0 то это ошибка из enum MPError
        //		Если Error то картинка меняется на красную -тултип иконки тип ошибки.
        //}
    }
}