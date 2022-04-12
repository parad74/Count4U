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
using System.Windows.Threading;
using Count4U.Common.Web;

namespace Count4U.Modules.Audit.ViewModels.Export
{
    public class BasePdaViewModel : CBIContextBaseViewModel
    {
		protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private const string OkButtonIcon = "check16";
		private const string AbortButtonIcon = "abort16";
		private const string ErrorButtonIcon = "delete16";

		protected Object DeleteMultiItemLock = new Object();
		protected Object AddMultiItemLock = new Object();
		protected Object ChangeMultiItemLock = new Object();
		protected const string DefaultTerminalID = "HT630";

		protected readonly IDBSettings _dbSettings;
		protected readonly IEventAggregator _eventAggregator;
		protected readonly IWrapperMultiRepository _wrapperMultiRepository;
		protected readonly IUserSettingsManager _userSettingsManager;
		protected readonly IServiceLocator _serviceLocator;

		protected readonly InteractionRequest<OpenFolderDialogNotification> _openFolderRequest;

		protected readonly DelegateCommand _browseCommand;
		protected DelegateCommand _execCommand;

		protected readonly DelegateCommand _refreshCommand;
		protected readonly DelegateCommand _closeCommand;
		protected readonly DelegateCommand _openCommand;

		protected int _processValue;
		protected bool _isChecked;
		protected bool _isCheckedEnabled;
        protected string _path;

		protected string _execButtonText;

		protected bool _isUploading;
		protected bool _isDesposed;
		protected bool _isFinished;
		protected bool _canRefresh;
		protected bool _canBrowse;

		protected bool _isProgressBarVisible;
        protected bool _isExecButtonVisible;
        

		protected readonly ObservableCollection<UploadToPdaItemViewModel> _items;

		protected bool _isInitialized;
		protected bool _isInitializingTextBlockVisible;

		protected string _exportERPAdapterCode;
		private bool _checkBoundrate;

		private long _quantityChecked;
		private long _quantityStarted;
		protected bool _quantityCheckedTextBlockVisible;

	

		protected IObservable<long> observCountingChecked;
		protected IDisposable disposeObservCountingChecked;
		//protected IObservable<long> observCountingFiles;
	
		public BasePdaViewModel(
            IContextCBIRepository contextCbiRepository,
            IDBSettings dbSettings,
            IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
            IUserSettingsManager userSettingsManager)
            : base(contextCbiRepository)
        {
            this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._eventAggregator = eventAggregator;
			this._dbSettings = dbSettings;
			this._openFolderRequest = new InteractionRequest<OpenFolderDialogNotification>();
			this._browseCommand = new DelegateCommand(BrowseCommandExecuted, BrowseCommandCanExecuted);
	//		this._uploadCommand = new DelegateCommand(UploadCommandExecuted, UploadCommandCanExecute);
			this._openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);

			this._items = new ObservableCollection<UploadToPdaItemViewModel>();
			this._closeCommand = new DelegateCommand(CloseCommandExecute);
			this._refreshCommand = new DelegateCommand(RefreshCommandExecuted, RefreshCommandCanExecute);

			this._execButtonText = Localization.Resources.View_UploadToPda_btnStart;
			this._wrapperMultiRepository = this._serviceLocator.GetInstance<IWrapperMultiRepository>(WrapperMultiEnum.WrapperMultiRepository.ToString());//TODO @@

			this._isFinished = false;
			this._canRefresh = true;

			this._isCheckedEnabled = true;

			this._canBrowse = true;

			this._isProgressBarVisible = false;
            this._isExecButtonVisible = true;
			this._isInitialized = false;
			this._isInitializingTextBlockVisible = false;
			this._quantityCheckedTextBlockVisible = true;
			this._quantityChecked = 0;
			this._quantityStarted = 0;

			this._isDesposed = true;
			this.ProcessDone = false;
			
        }

		public bool ProcessDone { get; set; }

		protected bool CheckBoundrate
		{
			get { return _checkBoundrate; }
			set { _checkBoundrate = value; }
		}

		protected bool UploadCommandCanExecute()
		{
			if (this._isInitialized == false) return false;
            if (this.IsExecButtonVisible == false) return false;

			if (this._isFinished == true) return false;

			if (this._isUploading == true) return true; //UploadToPda_btnStop + 

			if (Directory.Exists(this._path) == false)
				return false;

			if (Directory.EnumerateFiles(this._path).Any() == false)
				return false;

			if (this._items.Any(r => r.IsChecked) == false)
				return false;

			//   return _items.Where(r => r.IsChecked).All(r => !String.IsNullOrWhiteSpace(r.Device));

			return true;
		}


		protected bool DownloadCommandCanExecute()
		{
			if (this._isInitialized == false) return false;

			if (this._isFinished) return false;

			if (this._isUploading == true) return true; //UploadToPda_btnStop  +

			if (Directory.Exists(this._path) == false)
				return false;

			//if (Directory.EnumerateFiles(base._path).Any() == false)
			//	return false;

			if (this._items.Any(r => r.IsChecked) == false)
				return false;

			//   return _items.Where(r => r.IsChecked).All(r => !String.IsNullOrWhiteSpace(r.Device));

			return true;
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

				this._execCommand.RaiseCanExecuteChanged();
				this._openCommand.RaiseCanExecuteChanged();
			}
		}

		public string QuantityCheckedString
		{
			get { return String.Format(Localization.Resources.View_BasePdaViewModel_tbQuantityPda, this._quantityStarted.ToString(), this._quantityChecked.ToString()); }
		}

		protected long QuantityChecked
		{
			get { return _quantityChecked; }
			set { _quantityChecked = value;
			this.RaisePropertyChanged(() => this.QuantityChecked);
			//this.RaisePropertyChanged(() => this.QuantityCheckedString);
			}
		}

		protected long QuantityStarted
		{
			get { return _quantityStarted; }
			set
			{
				_quantityStarted = value;
				this.RaisePropertyChanged(() => this.QuantityStarted);
				this.RaisePropertyChanged(() => this.QuantityCheckedString);
			}
		}

		public bool QuantityCheckedTextBlockVisible
		{
			get { return _quantityCheckedTextBlockVisible; }
			set { _quantityCheckedTextBlockVisible = value; }
		}

		public int ProcessValue
		{
			get { return this._processValue; }
			set
			{
				this._processValue = value;
				RaisePropertyChanged(() => ProcessValue);
			}
		}

		//public DelegateCommand UploadCommand
		//{
		//	get { return this._uploadCommand; }
		//}

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

		public string ExecButtonText
		{
			get { return this._execButtonText; }
            set
            {
                this._execButtonText = value;
                RaisePropertyChanged(() => ExecButtonText);
            }
		}

		public bool IsUploading
		{
			get { return this._isUploading; }
			set
			{
				this._isUploading = value;
		
				if (this._isUploading == true)
					this._execButtonText = Localization.Resources.View_UploadToPda_btnStop;
				else
					this._execButtonText = Localization.Resources.View_UploadToPda_btnStart;

				RaisePropertyChanged(() => IsUploading);
				RaisePropertyChanged(() => ExecButtonText);
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

        public bool IsExecButtonVisible
        {
            get { return this._isExecButtonVisible; }
            set
            {
                this._isExecButtonVisible = value;
                RaisePropertyChanged(() => IsExecButtonVisible);
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

		//public override void OnNavigatedTo(NavigationContext navigationContext)
		//{
		//	base.OnNavigatedTo(navigationContext);

		//	this._uploadValue = 0;

		//	if (base.State.CurrentInventor != null)
		//	{
		//		this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentInventor.Code);
		//		this._exportERPAdapterCode = base.State.CurrentInventor.ExportERPAdapterCode;
		//	}
		//	else if (base.State.CurrentBranch != null)
		//	{
		//		this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentBranch.Code);
		//		this._exportERPAdapterCode = base.State.CurrentBranch.ExportERPAdapterCode;
		//	}
		//	else if (base.State.CurrentCustomer != null)
		//	{
		//		this._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentCustomer.Code);
		//		this._exportERPAdapterCode = base.State.CurrentCustomer.ExportERPAdapterCode;
		//	}

		//	using (new CursorWait())
		//	{
				
		//		this.Build();  // build GUI
		//		//Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
		//		this.WakeUpAndGetTerminalID("OnNavigatedTo", true);
		//	}
		//}


		//public override void OnNavigatedFrom(NavigationContext navigationContext)
		//{
		//	base.OnNavigatedFrom(navigationContext);
		//	_logger.Info("OnNavigatedFrom");

		//	this.BeforeClose();
		//	this.ClearEventForm();
		//	_logger.Info("OnNavigatedFrom  + Clear");
		//}

        private bool BrowseCommandCanExecuted()
        {
            return this._canBrowse;
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
                this._path = notification.FolderPath;
            }
        }

		//private bool UploadCommandCanExecute()
		//{
		//	if (this._isInitialized == false) return false;

		//	if (this._isFinished) return false;

		//	if (this._isUploading) return true;

		//	if (Directory.Exists(this._path) == false)
		//		return false;

		//	if (Directory.EnumerateFiles(this._path).Any() == false)
		//		return false;

		//	if (this._items.Any(r => r.IsChecked) == false)
		//		return false;

		//	//   return _items.Where(r => r.IsChecked).All(r => !String.IsNullOrWhiteSpace(r.Device));

		//	return true;
		//}

		//private void UploadCommandExecuted()
		//{
		//	if (this._isUploading == true)  // Stop command
		//	{
		//		using (new CursorWait())
		//		{
		//			try
		//			{
		//				this.StopButtonPress();  // визуальный эффект и присваивание имени "abort" процессу

		//				//lock (ChangeMultiItemLock)
		//				//{
		//				//	foreach (UploadToPdaItemViewModel item in _items)
		//				//	{
		//				//		item.UploadUnit.StateChanged -= UploadPdaUnit_StateChanged;
		//				//		item.PropertyChanged -= MultiItem_PropertyChanged;
		//				//	}
		//				//}
		//				int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
		//				Thread.Sleep(uploadWakeupTimeOutFinish1);

		//				//this.Done("UploadCommandExecuted");
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.ErrorException("UploadCommandExecuted - Stop()", exc);
		//			}
		//		}
		//	}// end Stop command

		//	else // Upload command
		//	{
		//		try
		//		{
		//			lock (ChangeMultiItemLock)
		//			{
		//				foreach (UploadToPdaItemViewModel item in this._items)
		//				{
		//					item.wrapperMulti.StateChanged += UploadPdaUnit_StateChanged;
		//					item.PropertyChanged += MultiItem_PropertyChanged;
		//				}
		//			}
		//		}
		//		catch (Exception exc)
		//		{
		//			_logger.ErrorException("UploadCommandExecuted - Upload()", exc);
		//		}

		//		this.Upload();
		//	}// end Upload command
			
		//}

		// по разным причинам все сделано
		// можно отписываться от событий и Dispose for Multi
		// и закрывать порты
		private void Done(string from, UploadPdaMPStage stage, bool abort = false)
		{
            //if (stage == UploadPdaMPStage.Upload)
            //{
            //    this._execButtonText = "111";
            //    this.RaisePropertyChanged(() => this.ExecButtonText);
            //    this._isFinished = true;
            //    this._execCommand.RaiseCanExecuteChanged();
            //}
            //else
            //{
				

				this._execButtonText =Localization.Resources.View_UploadToPda_btnStop;
				this.RaisePropertyChanged(() => this.ExecButtonText);
				this._isFinished = true;
                //this.IsExecButtonVisible = false;
                //this._execCommand.RaiseCanExecuteChanged();
              
               
            //}
		
			
			int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
			Thread.Sleep(uploadWakeupTimeOutFinish1);
			_logger.Info("Thread.Sleep Done1" + from + " (" + uploadWakeupTimeOutFinish1 + ")");
			//this.AbortThreads("Done");
			_logger.Info("Done from : " + from);
		

			try
			{
				Thread.Sleep(uploadWakeupTimeOutFinish1);
				_logger.Info("Thread.Sleep Done2" + from + " (" + uploadWakeupTimeOutFinish1 + ")");
				//-----------postUpload actions
				if (stage == UploadPdaMPStage.Upload)
				{
					if (this._userSettingsManager.UploadOptionsHT630_AfterUploadPerformWarmStartGet() == true)
					{
						//List<Multi> multisStart = this._items.Where(x => x.Device != "not started").Select(x => x.UploadWrapperMulti.Multi).ToList();
						//List<IWrapperMulti> wrapperMultisStart = this._items.Select(x => x.wrapperMulti).ToList();
						List<IWrapperMulti> wrapperMultisChecked = this._items.Where(x => x.IsChecked == true).Select(x => x.wrapperMulti).ToList();
						this._wrapperMultiRepository.WarmStartAllPDA(wrapperMultisChecked, "Done :: UploadOptionsHT630_AfterUploadPerformWarmStartGet" + from);
						Thread.Sleep(3000);
						_logger.Info("Thread.Sleep Done3" + from + " (3000)");
					}

					if (this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileNeedDoGet() == true)
					{
						List<string> files = this._userSettingsManager.UploadOptionsHT630_AfterUploadRunExeFileListGet().Split(";".ToCharArray()).ToList<string>();
						//List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
					//	List<IWrapperMulti> wrapperMultisChecked = this._items.Where(x => x.IsChecked == true).Select(x => x.wrapperMulti).ToList();	   //Last***
						List<IWrapperMulti> wrapperMultisChecked = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
						//-------- add new
						//int baudrate = this.GetBaudrateFormConfig();
						//this._wrapperMultiRepository.WarmStartAllPDA(wrapperMultisChecked, "Done::before RunExeFile::WarmStartAllPDA " + from);
						//Thread.Sleep(3000);
						//---------
						MPMemory memory = GetRunMemoryFormConfig();
						this._wrapperMultiRepository.RunFilesAllPDA(wrapperMultisChecked, files, memory, "Done");
						int k = files.Count();
						int uploadWakeupTimeOut2 = this._userSettingsManager.UploadWakeupTimeGet() * 2000 * k;
						Thread.Sleep(uploadWakeupTimeOut2);
						_logger.Info("Thread.Sleep Done4" + from + " (" + uploadWakeupTimeOut2 + ")");
					}

				
				}

                lock (ChangeMultiItemLock)
                {
                    foreach (UploadToPdaItemViewModel item in this._items)
                    {
                        item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
                        //item.PropertyChanged -= MultiItem_PropertyChanged;
                    }
                }
             
				//--------------------------------

				if (abort == false)
				{
					//List<IUploadPdaUnit> units = this._items.Select(x => x.UploadUnit.Multi).ToList();
					//List<Multi> multis = this._items.Select(x => x.UploadWrapperMulti.Multi).ToList(); //@@
					
					//List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList();
					//List<IWrapperMulti> wrapperMultis = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();

					//List<UploadToPdaItemViewModel> itemsNotStarted = this._items.Where(x => x.Device != "not started").Select(x => x).ToList();
					List<UploadToPdaItemViewModel> itemsNotStarted = this._items.Select(x => x).ToList();//*****
					List<IWrapperMulti> wrapperMultis = itemsNotStarted.Select(x => x.wrapperMulti).ToList();
					this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, from + "- >Done");			   //*****
					//this.SetIsCheckedFalseAndNotStarted(itemsNotStarted);
					int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
					Thread.Sleep(2000);
					_logger.Info("Thread.Sleep" + from + " (2000)");
				}

			}
			catch (Exception exc)
			{
				_logger.ErrorException("UploadCommandExecuted - Done()", exc);
			}

			Thread.Sleep(uploadWakeupTimeOutFinish1);
			_logger.Info("Thread.Sleep Done5" + from + " (" + uploadWakeupTimeOutFinish1 + ")");
			_logger.Info("AbortThreadAllThreads finish " + "Done");

          

			this._canRefresh = true;
            
			this._refreshCommand.RaiseCanExecuteChanged();

           

			this.IsDesposed = true;

			//this._isFinished = true;
			//this._execCommand.RaiseCanExecuteChanged();

			this.IsUploading = false;//UploadToPda_btnStart

			this.IsProgressBarVisible = false;
			this.ProcessDone = false;

            //this.IsExecButtonVisible = true;
            //this._execCommand.RaiseCanExecuteChanged();


		}

        // 1 шаг
        //визуальный эффект и присваивание имени "abort" процессу
        protected void PreStopButtonPress() // есть старая закоментаренная версия
        {
            _logger.Info("PreStopButtonPress");
      
            List<UploadToPdaItemViewModel> itemsAll = this._items.Select(x => x).ToList();
            this.SetIsCheckedFalseAndNotStarted(itemsAll);

            //lock (ChangeMultiItemLock)
            //{
            //    foreach (UploadToPdaItemViewModel item in this._items)
            //    {
            //        //item.PropertyChanged -= MultiItem_PropertyChanged;

            //        _logger.Info("StopButtonPress " + item.Device + " => " + "not started");
            //        item.Device = "not started";

            //        item._buttonImage = ErrorButtonIcon;
            //        Utils.RunOnUI(() => RaisePropertyChanged(() => item.ButtonImage));
            //        item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
            //    }
            //}
        }

        // 2 шаг
        //визуальный эффект и присваивание имени "abort" процессу
		protected void StopButtonPress() // есть старая закоментаренная версия
		{
            _logger.Info("StopButtonPress");
            this.IsUploading = false;//UploadToPda_btnStart +

            this._canRefresh = true;
            this._refreshCommand.RaiseCanExecuteChanged();

            this._isFinished = true;
            this._execCommand.RaiseCanExecuteChanged();

            this.IsDesposed = true;

            this.IsProgressBarVisible = false;

            //lock (ChangeMultiItemLock)
            //{
            //    foreach (UploadToPdaItemViewModel item in this._items)
            //    {
            //        //item.PropertyChanged -= MultiItem_PropertyChanged;

            //        _logger.Info("StopButtonPress " + item.Device + " => " + "not started");
            //        item.Device = "not started";

            //        item._buttonImage = ErrorButtonIcon;
            //        Utils.RunOnUI(() => RaisePropertyChanged(() => item.ButtonImage));
            //        item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
            //    }
            //}

			int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1500;
			Thread.Sleep(uploadWakeupTimeOutFinish1);
			_logger.Info("Thread.Sleep StopButtonPress1 " + " (" + uploadWakeupTimeOutFinish1 + ")");

			List<UploadToPdaItemViewModel> itemsAll = this._items.Select(x => x).ToList();
			List<IWrapperMulti> wrapperMultis = itemsAll.Select(x => x.wrapperMulti).ToList();
			this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "::StopButtonPress");
			int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
			Thread.Sleep(2000);
			_logger.Info("Thread.Sleep StopButtonPress2 " + " (2000)");

			_logger.Info("StopButtonPress end");

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
				_logger.ErrorException("Finish", exc);
			}

		}

		//визуальный эффект и присваивание имени "abort" процессу
		//private void StopButtonPress()
		//{
		//	//lock (ChangeMultiItemLock)
		//	//{
		//	//    //foreach (UploadToPdaItemViewModel item in _items.Where(r => r.IsChecked))
		//		//int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 100;
		//		foreach (UploadToPdaItemViewModel item in this._items)
		//		{
		//			item.AbortItemUploadThread();  // визуальный эффект и присваивание имени "abort" процессу
		//		}
				

		//		//foreach (UploadToPdaItemViewModel item in this._items)
		//		//{
		//		//	item.UploadUnit.StateChanged -= UploadPdaUnit_StateChanged;
		//		//}

		//		//Thread.Sleep(uploadWakeupTimeOutFinish1);
		//	//}


		//	//List<IUploadPdaUnit> units = this._items.Select(x => x.UploadUnit).ToList();
		//	//this._uploadPdaRepository.AbortThreadAllThreads(units, "Stop");

		//	//Thread.Sleep(uploadWakeupTimeOutFinish1);
		//	//_logger.Info("AbortThreadAllThreads finish " + "Stop");

		//	//this.AbortThreads("Stop");

		//	this.IsUploading = false;

		//	this._canRefresh = true;
		//	this._refreshCommand.RaiseCanExecuteChanged();

		//	this._isFinished = true;
		//	this._uploadCommand.RaiseCanExecuteChanged();

		//	this.IsDesposed = true;

		//	this.IsProgressBarVisible = false;

		//	_logger.Info("StopButtonPress end");

		//}

		//protected void Upload()
		//{
		//	_logger.Info("Upload start");
		//	using (new CursorWait())
		//	{
		//		this.WakeUpAndGetTerminalID("Upload", false);
		//	}

		//	try
		//	{
		//		if (Directory.Exists(this._path) == false)
		//			return;
		//		List<string> filesToProcess = Directory.GetFiles(this._path).ToList();

		//		this.IsUploading = true;
		//		this.IsDesposed = false;

		//		if (this._userSettingsManager.UploadOptionsHT630_DeleteAllFilePDAGet() == true)
		//		{
		//			List<string> excludeSiles = this._userSettingsManager.UploadOptionsHT630_ExeptionFileNotDeleteGet().Split(";".ToCharArray()).ToList<string>();
		//			//List<Multi> multisStart = this._items.Where(x => x.Device != "not started").Select(x => x.UploadWrapperMulti.Multi).ToList(); //@@
		//			List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
		//			this._wrapperMultiRepository.DeleteFilesAllPDA(wrapperMultisStart, excludeSiles, "Upload");
		//			int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
		//			Thread.Sleep(uploadWakeupTimeOut1);
		//		}

		//		foreach (UploadToPdaItemViewModel item in this._items)
		//		{
		//			// --------- fill FileLists for Upload
		//			item.ClearUploadingFileLists();

		//			if (item.IsChecked == true)
		//			{
		//				item.AddFileInfoToUploadingFileList(filesToProcess);
		//				var localFilesToProcess = filesToProcess.ToList();

		//				if (String.IsNullOrEmpty(item.Device) == false && item.Device != "not started") //substitute jeng.exe
		//				{
		//					//файлы в папке для заливки (в зависимости от типа терминала)
		//					List<string> terminalFilePathList = BuildPathToJENGEXE(item.Device);
		//					//файлы на терминале
		//					List<string> filesOnPDA = item.wrapperMulti.GetDirectory(false).Select(x => x.name).ToList(); //@@

		//					foreach (var terminalFilePath in terminalFilePathList)
		//					{
		//						string fileName = System.IO.Path.GetFileName(terminalFilePath);
		//						//если на терминала нет файла с  именем fileName - то его добавить в список upload files - localFilesToProcess
		//						// иначе не добавлять и не заливать
		//						if (filesOnPDA.Contains(fileName) == false)
		//						{
		//							localFilesToProcess.Add(terminalFilePath);
		//						}
		//					}

		//					item.AddFileInfoToUploadingFileList(terminalFilePathList);
		//				}
		//				else
		//				{
		//					_logger.Info("Upload : item.Device - " + item.Device + "  Is Null Or Empty");
		//				}

		//				// ------------- 

		//				//if (item.UploadUnit.SerialIsOpen() == true)
		//				//{
		//				//	if (item.UploadUnit.UploadFiles(localFilesToProcess) == true)
		//				//	{
		//				//		item.StartGUIUpdate();
		//				//	}
		//				//}
		//			}
		//			else
		//			{
		//				item.ButtonEnabled = false;
		//				item.IsCheckedEnabled = false;
		//				item.ButtonVisible = false;
		//			}
		//		}

		//		//============  Upload process
		//		Task<bool>[] uploadFileTasks = new Task<bool>[this._items.Where(x => x.IsChecked == true).Count()];
		//		int countTask = uploadFileTasks.Count();
		//		int indexTask = 0;
		//		for (int i = 0; i < this._items.Count; i++)
		//		{
		//			if (this._items[i].IsChecked == true)
		//			{
		//				//if (this._items[i].UploadUnit.SerialIsOpen() == true)
		//				//{
		//				if (indexTask < countTask)
		//				{
		//					uploadFileTasks[indexTask] = new Task<bool>(() => { var ret = this._items[i].wrapperMulti.UploadFiles(this._items[i]._uploadingFullPathFiles); return ret; }); //@@
		//					uploadFileTasks[indexTask].Start();
		//					bool resultStartUpload = uploadFileTasks[indexTask].Result;
		//					indexTask++;

		//					if (resultStartUpload == true)
		//					{
		//						this._items[i].StartGUIUpdate();
		//					}
		//				}
		//				else
		//				{
		//					_logger.Info("Upload - it's strange: indexTask [" + indexTask.ToString() + "]  > " + " countTask [" + countTask.ToString() + "]");
		//				}
		//				//}
		//			}
		//		} //for
		//		Task.WaitAll(uploadFileTasks);



		//		_logger.Info("Upload + Task.WaitAll(uploadFileTasks) end");
		//		//----------------------

		//		this._canRefresh = false;
		//		this._refreshCommand.RaiseCanExecuteChanged();

		//		this._canBrowse = false;
		//		this._browseCommand.RaiseCanExecuteChanged();

		//		this.IsCheckedEnabled = false;

		//		this.IsProgressBarVisible = true;


		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("Upload", exc);
		//	}

		//}


		protected void ClearEventForm()
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
							_logger.ErrorException("ClearEventForm - Stop()", exc);
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

		protected void Build(bool reWakeup = false)//, int tryBaudrate = 57600)
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
					//  57600, 38400, 19200, 9600, 4800 
                    //if (tryBaudrate == 38400)
                    //{
                    //    int uploadWakeupTimeOut2 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
                    //    Thread.Sleep(uploadWakeupTimeOut2);

                    //    List<IWrapperMulti> wrapperMultisNotStart = uploadPdaWrapperMultiList.Where(x => x.Multi.TerminalID == "not started").Select(x => x).ToList();
                    //    this._wrapperMultiRepository.TryWakeUpAllNotStartPorts(wrapperMultisNotStart, 38400, "Build");
                    //}

					//int uploadWakeupTimeOut = 15000; //this._userSettingsManager.UploadWakeupTimeGet() * 1500;
					Thread.Sleep(15000);
					_logger.Info("Thread.Sleep Build1  (15000)");

                    this._wrapperMultiRepository.AbortThreadAllThreads(uploadPdaWrapperMultiList, "Build");
                    //int uploadWakeupTimeOut2 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
                    Thread.Sleep(2000);
					_logger.Info("Thread.Sleep Build2  (2000)");
       

                    //if (this._userSettingsManager.UploadOptionsHT630_CurrentDataPDAGet() == true)
                    //{
                    //    //List<Multi> multis = this._items.Select(x => x.UploadWrapperMulti.Multi).ToList(); // не известно что будет если не поднят @@
                    //    //List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList(); 
                    //    List<IWrapperMulti> wrapperMultisStart = uploadPdaWrapperMultiList.Where(x => x.Multi.TerminalID != "not started").Select(x => x).ToList();
                    //    this._wrapperMultiRepository.SetDateTimeAllPDA(wrapperMultisStart, DateTime.Now, "Build");
                    //    int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
                    //    Thread.Sleep(uploadWakeupTimeOut1);
                    //}
				}

				lock (AddMultiItemLock)
				{
					foreach (IWrapperMulti uploadPdaWrapperMulti in uploadPdaWrapperMultiList.OrderBy(r => Regex.Replace(r.Multi.Port, @"[^\d]", "")))
					{
						//if (uploadPdaUnit.SerialIsOpen() == false) continue;
						//!!** baundrate  uploadPdaUnit.StateChanged += UploadPdaUnit_StateChanged;
                        //uploadPdaWrapperMulti.StateChanged += UploadPdaUnit_StateChanged;

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


					_execCommand.RaiseCanExecuteChanged();
				}
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Build", exc);
            }

			//Task.Factory.StartNew(() =>
			//	{
			//		Thread.Sleep(_userSettingsManager.UploadWakeupTimeGet());
			//		WakeUpAndGetTerminalID("Build");
			//	});


			//WakeUpAndGetTerminalID("Build");
        }

		protected int GetBaudrateFormConfig()
		{
			int baudrate = 57600;
			bool baudrateGetFormConfig = this._userSettingsManager.UploadOptionsHT630_BaudratePDAGet();
			if (baudrateGetFormConfig == true)
			{
				Int32.TryParse(this._userSettingsManager.UploadOptionsHT630_BaudratePDAItemGet(), out baudrate);
			}
			return baudrate;
		}



		protected void WakeUpAndGetTerminalID(string from, bool abortThreadAfter = false, bool reWakeup = false, bool setDateTime = false)
        {
            try
            {
				this.IsDesposed = false;
				_logger.Info("WakeUpAndGetTerminalID start from " + from);

				//----------------- WakeUpAllPorts
				int baudrate = this.GetBaudrateFormConfig();
				List<IWrapperMulti> wrapperMultis = this._items.Select(x => x.wrapperMulti).ToList();
				this._wrapperMultiRepository.WakeUpAllPorts(wrapperMultis, baudrate, "WakeUpAndGetTerminalID::WakeUpAllPorts " + from);
				
				int uploadWakeupTimeOut = this._userSettingsManager.UploadWakeupTimeGet() * 1200;
				if (reWakeup == true)
				{
					uploadWakeupTimeOut = 12000;
				}
 				Thread.Sleep(uploadWakeupTimeOut);
				_logger.Info("Thread.Sleep WakeUpAndGetTerminalID1  (" + uploadWakeupTimeOut + ")");
				_logger.Info("WakeUpAndGetTerminalID after wakeUp " + from);

				//--------------------GetTerminalID and set in GUI
			
				this._wrapperMultiRepository.GetTerminalIDAllPDA(wrapperMultis, "WakeUpAndGetTerminalID::GetTerminalIDAllPDA " + from);

                int uploadWakeupTimeOutFinish = _userSettingsManager.UploadWakeupTimeGet() * 200;
                Thread.Sleep(uploadWakeupTimeOutFinish);
				_logger.Info("Thread.Sleep WakeUpAndGetTerminalID2  (" + uploadWakeupTimeOutFinish + ")");

				for (int i = 0; i < this._items.Count; i++)
				{
					string terminalId = "not started";
					terminalId = this._items[i].wrapperMulti.GetTerminalID(); //add
					Utils.RunOnUI(() => this._items[i].Device = terminalId);
				}
				_logger.Info("WakeUpAndGetTerminalID finish " + from);

				if (setDateTime == true)
				{
					SetDateTimeAllPDA();
				}

				// ------------------AbortThreadAllThreads
				if (abortThreadAfter == true)  // и все закрываем
				{
					this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "WakeUpAndGetTerminalID::abortThreadAfter ::" + from);
					//this.SetIsCheckedFalseAndNotStarted(itemsAll);
					Thread.Sleep(2000);
					_logger.Info("Thread.Sleep WakeUpAndGetTerminalID3  (2000)");
					this.IsDesposed = true;
					_logger.Info("AbortThreadAllThreads finish " + from);
				}
				else  // или готовим к uploade
				{
				
					var units1 = this._items.Where(x => x.Device == "not started").Select(x => x).ToList();
					foreach (var unit in units1)
					{
						unit.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
						unit.PropertyChanged -= MultiItem_PropertyChanged;
					}
					Thread.Sleep(500);
					_logger.Info("Thread.Sleep WakeUpAndGetTerminalID4  (" + 500 + ")");


					//------- SetDateTimeAllPDA
					//SetDateTimeAllPDA();	  было только здесь Eran попросил перенести в открытие формы
				}

                Utils.RunOnUI(() => IsInitializingTextBlockVisible = false);
            }
            catch (Exception exc)
            {
				_logger.ErrorException("WakeUpAndGetTerminalID", exc);
            }
            finally
            {
                _isInitialized = true;
                _isFinished = false;
                _execCommand.RaiseCanExecuteChanged();
            }
        }

		public void SetDateTimeAllPDA()
		{
			if (this._userSettingsManager.UploadOptionsHT630_CurrentDataPDAGet() == true)
			{
				List<IWrapperMulti> wrapperMultisStart = this._items.Where(x => x.Device != "not started").Select(x => x.wrapperMulti).ToList();
				this._wrapperMultiRepository.SetDateTimeAllPDA(wrapperMultisStart, DateTime.Now, "WakeUpAndGetTerminalID");
				int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
				Thread.Sleep(uploadWakeupTimeOut1);
				_logger.Info("Thread.Sleep WakeUpAndGetTerminalID  (" + uploadWakeupTimeOut1 + ")");
			}
		}

	
		//void WaitForTwoSecondsAsync(Action callback)		// Asynchronous NON-BLOCKING method
		//{
		//	new Timer(_ => callback()).Change(2000, -1);
		//}


		//void WaitForTenSecondsAsync(Action callback)		// Asynchronous NON-BLOCKING method
		//{
		//	new Timer(_ => callback()).Change(10000, -1);
		//}

		//void WaitForSecondsAsync(Action callback, int sec)		// Asynchronous NON-BLOCKING method
		//{
		//	sec = sec * 1000;
		//	new Timer(_ => callback()).Change(sec, -1);
		//}


		//Task Delay(int milliseconds)		// Asynchronous NON-BLOCKING method
		//{
		//	var tcs = new TaskCompletionSource<object>();
		//	new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
		//	return tcs.Task;
		//}

		// идет типа от EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size); в multi
		protected void UploadPdaUnit_StateChanged(IWrapperMulti uploadPdaWrapperMulti, UploadPdaMPStage stage, int state, string text, int current) 
		{
			//public enum MPStage { Bell = 0x07, Upload = 0x4C, Download = 0x55 };  //MPStage stage
			//public enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 }; //int state

			if (uploadPdaWrapperMulti.Multi.Address == 0x80) return;
			if (uploadPdaWrapperMulti.Multi.SerialIsOpen() == false) return;
            try
            {
                Utils.RunOnUI(() =>
                {

                    UploadToPdaItemViewModel item = this._items.FirstOrDefault(r => r.Port == uploadPdaWrapperMulti.Multi.Port);

                    if (item == null) return;
					//** baunrate
                    //if (stage == UploadPdaMPStage.Upload || stage == UploadPdaMPStage.Download)
                    //{
					if (item.Device == "not started") return;
					if (item.IsChecked == false) return;
                    //}


                    //if (string.IsNullOrWhiteSpace(item.Device) == true || item.Device == "not started")
                    //{
                    //	item.Device = uploadPDAUnit.GetTerminalID();
                    //}


                    if (state < 0)
                    {
                   
                        try
                        {
							//TODO need stop Multi
							//enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 }; //int state
							MPError errorFromUpload = (MPError)state;
							_logger.Info("Error in UploadPdaUnit_StateChanged  state : " + state + "enum :  " + errorFromUpload);

							List<UploadToPdaItemViewModel> itemsError = new List<UploadToPdaItemViewModel>();
							itemsError.Add(item);
							//item.IsChecked = false;
							//item.Device = "not started";
							//item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
							this.SetIsCheckedFalseAndNotStarted(itemsError);

							// Last***
							//List<IWrapperMulti> wrapperMultis = new List<IWrapperMulti>();
							//wrapperMultis.Add(item.wrapperMulti);
							//this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "UploadPdaUnit_StateChanged + Error : " + item.wrapperMulti.ComPortStatic);
							//Thread.Sleep(2000);
							//_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 01 (2000)");
							// Last***

							item.Error();
							Thread.Sleep(2000);
							_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 02 (2000)");
                         }
                        catch (Exception exp)
						{
							_logger.ErrorException("UploadPdaUnit_StateChanged 08 state < 0", exp);
						}

                    }
                    else
                    {
                        //int fileIndex = item._uploadingFilesNames.IndexOf(text.ToLower());
                        //if (fileIndex >= 0)
                        //{
                        //FileInfo file = item._uploadingFiles[fileIndex];

                        //long doneTotal = 0;

                        //for (int i = 0; i < fileIndex; i++)
                        //{
                        //	doneTotal += _uploadingFiles[i].Length;
                        //}

                        //doneTotal += current;

                        switch (stage)
                        {
							case UploadPdaMPStage.Bell:
								{
                                    //item.Value = "WakeUp " + uploadPdaWrapperMulti.Multi.Address;
                                    //Utils.RunOnUI(() => RaisePropertyChanged(() => ProcessValue));
                                    //Utils.RunOnUI(() => RaisePropertyChanged(() => ExecButtonText));
                                    //Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());
								}
								break;
								
                            case UploadPdaMPStage.Download:
                                {
                                    if (item._downloadFilesNames == null) return;
                                    if (item._downloadFiles == null) return;
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
                                    if (item._uploadingFilesNames == null) return;
                                    if (item._uploadingFiles == null) return;
                                    int fileIndexUpload = item._uploadingFilesNames.IndexOf(text.ToLower());
                                    if (fileIndexUpload >= 0)
                                    {
                                        item.Value = String.Format("{0}/{1}", state, current);//закочено/всего 
                                        item.File = text;
                                        item.Number = String.Format("{0}/{1}", fileIndexUpload + 1, item._uploadingFiles.Count);
                                        //  item.Progress = (int)((doneTotal * 100) / _uploadingFilesLength);
                                        double progress = 0;
                                        if (item._uploadingFiles.Count != 0)
                                        {
                                            progress = (double)(fileIndexUpload) / (double)item._uploadingFiles.Count;
                                        }
                                        item.Progress = 0;
                                        if (progress >= 0 && progress <= 1)
                                        {
                                            item.Progress = (int)(progress * 100);
                                        }
                                        if (fileIndexUpload + 1 == item._uploadingFiles.Count)
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
                                        if (state >= current && fileIndexUpload + 1 == item._uploadingFiles.Count)			//state/current  //закочено/всего //1075/1185
                                        {
                                            item.Finish();
                                           
                                     
                                        }

                                    }
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException("stage");
                        }

                    }
                });

                Utils.RunOnUI(() => RaisePropertyChanged(() => ProcessValue));
                Utils.RunOnUI(() => RaisePropertyChanged(() => ExecButtonText));
                Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());


                Utils.RunOnUI(() =>
                {

                    // STOP
                    if (this._items.Where(r => r.IsChecked && r.Device != "not started").All(r => r.Cancelled) == true) //все что чек + все отменены
                    {
                        using (new CursorWait())
                        {
                            int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
                            Thread.Sleep(uploadWakeupTimeOutFinish1);
							_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 2 (" + uploadWakeupTimeOutFinish1 + ")");
                            //DeleteAllMulti(multis);
                            //List<Multi> multis = this._items.Where(x => x.IsChecked == true).Select(x => x.UploadWrapperMulti.Multi).ToList(); //@@
                            //List<IWrapperMulti> wrapperMultis = this._items.Where(x => x.IsChecked == true).Select(x => x.wrapperMulti).ToList(); 

                            //List<UploadToPdaItemViewModel> itemsIsChecked = this._items.Where(x => x.Device != "not started").Select(x => x).ToList();
                            //List<UploadToPdaItemViewModel> itemsNotStarted = this._items.Where(x => x.IsChecked == true).Select(x => x).ToList();
                            List<UploadToPdaItemViewModel> itemsAll = this._items.Select(x => x).ToList();
                           // List<IWrapperMulti> wrapperMultis = itemsAll.Select(x => x.wrapperMulti).ToList();
                           // this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "UploadPdaUnit_StateChanged + Cancelled");

							if (itemsAll.Count > 0)
							{
								int uploadWakeupTimeOutFinish = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
								Thread.Sleep(uploadWakeupTimeOutFinish1);
								_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 3_1 (" + uploadWakeupTimeOutFinish1 + ")");
								this.SetIsCheckedFalseAndNotStarted(itemsAll);
   								Thread.Sleep(2000);
								_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 3_2 (2000)");
							}
                        }
                    }
                    else
                    {
                        // ERROR
                        using (new CursorWait())
                        {
							List<UploadToPdaItemViewModel> itemsIsError = this._items.Where(x => x.IsChecked == true && x.Device != "not started" && x.IsError == true).Select(x => x).ToList();
                            List<IWrapperMulti> wrapperMultisIsError = itemsIsError.Select(x => x.wrapperMulti).ToList();
                            if (wrapperMultisIsError.Count > 0)
                            {
                                int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
                                Thread.Sleep(uploadWakeupTimeOutFinish1);
								_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 4 (" + uploadWakeupTimeOutFinish1 + ")");
								//this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultisIsError, "UploadPdaUnit_StateChanged + ERROR : count " + itemsIsError.Count());
                                this.SetIsCheckedFalseAndNotStarted(itemsIsError);
                                Thread.Sleep(2000);
								_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 5  (2000)");
                            }
                        }
                    }


                });

                //Utils.RunOnUI(() => RaisePropertyChanged(() => ProcessValue));
                //Utils.RunOnUI(() => RaisePropertyChanged(() => ExecButtonText));
                //Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());
                if (this._items.Where(r => r.IsChecked).All(r => r.Done || r.IsError || r.Cancelled))
                {
					
                    //for (int i = 0; i < 10; i++)
                    //{
						UpdateTotalProgress();
                        this.IsExecButtonVisible = false;
                        Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());
                        this._execCommand.RaiseCanExecuteChanged();
                    ////}
                    int timeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 100;
                    Thread.Sleep(timeOutFinish1);
					_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 6 (" + timeOutFinish1 + ")");
                }
                //, DispatcherPriority.Send

                //  item.Device = uploadPDAUnit.GetTerminalID();
                // по разным причинам все сделано
                // можно отписываться от событий и Dispose for Multi
               // Utils.RunOnUI(() =>
               //{
                if (this._items.Where(r => r.IsChecked).All(r => r.Done || r.IsError || r.Cancelled))
                {
					if (this.ProcessDone == false)
					{
						this.ProcessDone = true;
						//this.ExecButtonText = "Wait";//Localization.Resources.View_UploadToPda_btnStop;
						//Utils.RunOnUI(() => RaisePropertyChanged(() => ExecButtonText), DispatcherPriority.Send);
						//Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged(), DispatcherPriority.Send);

						Utils.RunOnUI(() =>
						{
							//UpdateTotalProgress();
							//this.IsExecButtonVisible = false;
							//this._execCommand.RaiseCanExecuteChanged();

							using (new CursorWait())
							{
								this.Done("UploadPdaUnit_StateChanged", stage);
							}

							this.IsExecButtonVisible = true;
							this._execCommand.RaiseCanExecuteChanged();
						});
					}
                }

            }
            catch (Exception e)
			{
				this.ProcessDone = false;
				_logger.ErrorException("UploadPdaUnit_StateChanged", e);
            }
			//finally
			//{

			//}
			//});
		}

		protected void SetIsCheckedFalseAndNotStarted(List<UploadToPdaItemViewModel> itemsIsError)
		{
			_logger.Info("SetIsCheckedFalseAndNotStarted Count = " + itemsIsError.Count());
			for (int i = 0; i < itemsIsError.Count(); i++)
			{
				//itemsIsError[i].IsChecked = false;
				_logger.Info("SetIsCheckedFalseAndNotStarted " + itemsIsError[i].Device + " => " + "not started");
				itemsIsError[i].Device = "not started";

                itemsIsError[i]._buttonImage = ErrorButtonIcon;
                Utils.RunOnUI(() => RaisePropertyChanged(() => itemsIsError[i].ButtonImage));

                itemsIsError[i].State = UploadToPdaItemState.UploadError;
				itemsIsError[i].IsShowProgressBar = false;
				itemsIsError[i].ButtonEnabled = false;

                itemsIsError[i].wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
			}
		}



		protected void UpdateTotalProgress()
        {

			List<UploadToPdaItemViewModel> itemsInvolved = this._items.Where(r => r.IsChecked).Where(r => r.Cancelled == false && r.IsError == false).ToList();


             if (itemsInvolved.Count == 0)
             {
				 this.ProcessValue = 0;
                 return;
             }
	
			 long doneTotalProgress = 0;
			 if (itemsInvolved.Count == 0)
			 {
				 this.ProcessValue = 0;
				 return;
			 }

			
			 foreach (UploadToPdaItemViewModel item in itemsInvolved)
			 {
				 doneTotalProgress = doneTotalProgress + item.Progress;
			 }

			double progress = (double)doneTotalProgress / (double)(itemsInvolved.Count*100);
            this.ProcessValue = 0;
            if (progress >= 0 && progress <= 1)
            {
                this.ProcessValue = (int)(progress * 100);
            }
        }

		protected void MultiItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UploadToPdaItemViewModel item = sender as UploadToPdaItemViewModel;
            if (item == null) return;

			if (e.PropertyName == "IsChecked" || e.PropertyName == "Done")
            {
                this._execCommand.RaiseCanExecuteChanged();
            }

			if (e.PropertyName == "Cancelled" || e.PropertyName == "IsError" || e.PropertyName == "Done")
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

		protected void CloseCommandExecute()
        {
			//this.BeforeClose();

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

		protected void BeforeClose()
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
						_logger.Info("Thread.Sleep UploadPdaUnit_StateChanged 7  (" + uploadWakeupTimeOutFinish1 + ")");
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

		protected bool RefreshCommandCanExecute()
        {
            return _canRefresh;
        }

		protected void RefreshCommandExecuted()
        {
            this.ProcessValue = 0;

			this.IsUploading = false;//UploadToPda_btnStart

            this.IsChecked = false;
            this.IsCheckedEnabled = true;

            this._isFinished = false;

            this._canRefresh = true;
            this._refreshCommand.RaiseCanExecuteChanged();

            this._canBrowse = true;
            this._browseCommand.RaiseCanExecuteChanged();

            this._execCommand.RaiseCanExecuteChanged();
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
				this.WakeUpAndGetTerminalID("RefreshCommandExecuted", true);
			}
        }

		////файлы в папке для заливки (в зависимости от типа терминала)
		//private List<string> BuildPathToJENGEXE(string terminalId)
		//{
		//	List<string> result = new List<string>();
		//	_logger.Info("BuildPathToJENGEXE :" + terminalId);

		//	string terminalPath = _dbSettings.TerminalIDPath();
		//	terminalPath = terminalPath + @"\" + terminalId;

		//	if (Directory.Exists(terminalPath) == true)
		//	{
		//		result = Directory.GetFiles(terminalPath).ToList();
		//		//for (int i = 0; i < files.Length; i++)
		//		//{
		//		//	result.Add(files[i]);
		//		//}
		//		//string jengexePath = System.IO.Path.Combine(terminalPath, terminalId, "JENG.EXE");

		//		//if (File.Exists(jengexePath) == true)
		//		//{
		//		//	result = jengexePath;
		//		//}
		//		//else
		//		//{
		//		//	_logger.Info("BuildPathToJENGEXE: " + terminalId + " - File Path" + jengexePath + " not exists");
		//		//}
		//	}
		//	else
		//	{
		//		_logger.Info("BuildPathToJENGEXE: " + terminalId + " - File Path" + terminalPath + " not exists" );
		//	}

		//	return result;
		//}

		protected bool OpenCommandCanExecute()
        {
            return Directory.Exists(_path);
        }

		protected void OpenCommandExecuted()
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


		protected MPMemory GetRunMemoryFormConfig()
		{
			string memoryGetFormConfig = "RAM";
			MPMemory enMemory = MPMemory.RAM;
			try
			{
				memoryGetFormConfig = this._userSettingsManager.UploadOptionsRunMemoryItemGet();
				enMemory = (MPMemory)Enum.Parse(typeof(MPMemory), memoryGetFormConfig);
			}
			catch { }
			return enMemory;
		}
    }
}