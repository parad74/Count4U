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
using System.Reactive.Linq;
using Count4U.Common.Web;

namespace Count4U.Modules.Audit.ViewModels.Export
{
	public class DownloadBaseFromPdaViewModel : BasePdaViewModel
	{
		//  private readonly DelegateCommand _uploadCommand;

		//private string _uploadButtonText;
		//private bool _isUploading;
		

		public DownloadBaseFromPdaViewModel(
			IContextCBIRepository contextCbiRepository,
			IDBSettings dbSettings,
			IEventAggregator eventAggregator,
			IServiceLocator serviceLocator,
			IUserSettingsManager userSettingsManager)
			: base(contextCbiRepository, dbSettings, eventAggregator, serviceLocator, userSettingsManager)
		{
			base._execCommand = new DelegateCommand(this.DownloadCommandExecuted, base.DownloadCommandCanExecute);
			base._execButtonText = Localization.Resources.View_UploadToPda_btnStart;
		}


		public string DownloadButtonText
		{
			get { return base._execButtonText; }
		}

		//public string Path
		//{
		//	get { return base._path; }
		//	set
		//	{
		//		base._path = value;
		//		RaisePropertyChanged(() => Path);

		//		this._execCommand.RaiseCanExecuteChanged();
		//		base._openCommand.RaiseCanExecuteChanged();
		//	}
		//}


		public DelegateCommand DownloadCommand
		{
			get { return this._execCommand; }
		}


		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			//--------
			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.CheckBoundrate))
			{
				string val= navigationContext.Parameters.FirstOrDefault(r => r.Key == Common.NavigationSettings.CheckBoundrate).Value;
				bool checkBoundrate = false;
				bool ret = bool.TryParse(val, out checkBoundrate);
				base.CheckBoundrate = checkBoundrate;
			}

			//-------
			base._processValue = 0;

			if (base.State.CurrentInventor != null)
			{
				base._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentInventor.Code);
				base._exportERPAdapterCode = base.State.CurrentInventor.ExportERPAdapterCode;
			}
			else if (base.State.CurrentBranch != null)
			{
				base._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentBranch.Code);
				base._exportERPAdapterCode = base.State.CurrentBranch.ExportERPAdapterCode;
			}
			else if (base.State.CurrentCustomer != null)
			{
				base._path = System.IO.Path.Combine(_dbSettings.ExportToPdaFolderPath(), base.State.CurrentCustomer.Code);
				base._exportERPAdapterCode = base.State.CurrentCustomer.ExportERPAdapterCode;
			}

			if (CheckBoundrate == true)
			{
				using (new CursorWait())
				{
					_logger.Info("OnNavigatedTo.Build1  uploadWakeupTimeOut + [---------------------" + this._userSettingsManager.UploadWakeupTimeGet() + "------------------------]");
					base.Build(true);  // build GUI
					//Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
					base.WakeUpAndGetTerminalID("OnNavigatedTo", true, true);
				}
				Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);
				using (new CursorWait())
				{
					base.Build();
					base.WakeUpAndGetTerminalID("OnNavigatedTo1", true, true);
				}

			}
			else //CheckBoundrate == false
			{
				{
					_logger.Info("OnNavigatedTo.Build (CheckBoundrate == false)  uploadWakeupTimeOut + [---------------------" + this._userSettingsManager.UploadWakeupTimeGet() + "------------------------]");
					base.Build();  // build GUI
					base.WakeUpAndGetTerminalID("OnNavigatedTo", true);
				}
				Utils.RunOnUI(() => IsInitializingTextBlockVisible = true);

			}

            observCountingChecked = Observable.Timer(TimeSpan.FromSeconds(25), TimeSpan.FromSeconds(2)).Select(x => x);
            disposeObservCountingChecked = observCountingChecked.Subscribe(CountingCheckedPDA);

		}

		public void CountingCheckedPDA(long x)
		{
			bool conting = true; //слушать или нет

			if (conting == false)
			{
				base.QuantityChecked = 0;
				this.QuantityStarted = 0;
				return;
			}

			if (base._items == null) return;
			if (base._items.Count() < 1)
			{
				base.QuantityChecked = 0;
				this.QuantityStarted = 0;
                return;
			}

			var itemsCheck = base._items.Where(k => k.IsChecked == true && k.Device != "not started").Select(k => k).ToList();

			if (itemsCheck.Count() < 1)
			{
				base.QuantityChecked = 0;
			}
			else
			{
				base.QuantityChecked = itemsCheck.LongCount();
			}

			var itemsNotStarted = base._items.Where(k => k.Device != "not started").Select(k => k).ToList();
            if (itemsNotStarted.Count() < 1)
			{
				this.QuantityStarted = 0;
			}
			else
			{
				this.QuantityStarted = itemsNotStarted.LongCount();
			}
		}


		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
			_logger.Info("OnNavigatedFrom");

			base.BeforeClose();
			base.ClearEventForm();
			if (disposeObservCountingChecked != null) disposeObservCountingChecked.Dispose();
			_logger.Info("OnNavigatedFrom  + Clear");
		}


		//private bool DownloadCommandCanExecute()
		//{
		//	if (base._isInitialized == false) return false;

		//	if (base._isFinished) return false;

		//	if (base._isUploading == true) return true; //UploadToPda_btnStop  +

		//	if (Directory.Exists(base._path) == false)
		//		return false;

		//	//if (Directory.EnumerateFiles(base._path).Any() == false)
		//	//	return false;

		//	if (base._items.Any(r => r.IsChecked) == false)
		//		return false;

		//	//   return _items.Where(r => r.IsChecked).All(r => !String.IsNullOrWhiteSpace(r.Device));

		//	return true;
		//}

		private void DownloadCommandExecuted()
		{
           
			//STOP COMMAND
			if (base._isUploading == true)  // Stop command //UploadToPda_btnStop +
			{
                base.PreStopButtonPress();
				using (new CursorWait())
				{
					try
					{
						base.StopButtonPress();  // визуальный эффект и присваивание имени "abort" процессу

						int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
						Thread.Sleep(uploadWakeupTimeOutFinish1);
						_logger.Info("Thread.Sleep DownloadCommandExecuted1  (" + uploadWakeupTimeOutFinish1 + ")");
						//this.Done("UploadCommandExecuted");
					}
					catch (Exception exc)
					{
						_logger.ErrorException("DownloadCommandExecuted - Stop()", exc);
					}
				}
			}// end Stop command

			//DOWNLOAD COMMAND
			else // Upload command //UploadToPda_btnStart //_isUploading == false  +
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



		// перенесена в Base
		//визуальный эффект и присваивание имени "abort" процессу
		//private void StopButtonPress() // need todo
		//{
		//	//lock (ChangeMultiItemLock)
		//	//{
		//	//    //foreach (UploadToPdaItemViewModel item in _items.Where(r => r.IsChecked))
		//		//int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 100;
		//	//foreach (UploadToPdaItemViewModel item in base._items) //!!!TODO
		//	//	{
		//	//		item.AbortItemUploadThread();  // визуальный эффект и присваивание имени "abort" процессу
		//	//	}


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
		//	//--------------------------------------------------------------
		//	base.IsUploading = false;//UploadToPda_btnStart +

		//	base._canRefresh = true;
		//	base._refreshCommand.RaiseCanExecuteChanged();

		//	base._isFinished = true;
		//	this._execCommand.RaiseCanExecuteChanged();

		//	base.IsDesposed = true;

		//	base.IsProgressBarVisible = false;

		//	lock (ChangeMultiItemLock)
		//	{
		//		foreach (UploadToPdaItemViewModel item in this._items)
		//		{
		//			item.wrapperMulti.StateChanged -= UploadPdaUnit_StateChanged;
		//			//item.PropertyChanged -= MultiItem_PropertyChanged;
		//		}
		//	}

		//	int uploadWakeupTimeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 200;
		//	Thread.Sleep(uploadWakeupTimeOutFinish1);

		//	List<UploadToPdaItemViewModel> itemsAll = this._items.Select(x => x).ToList();
		//	List<IWrapperMulti> wrapperMultis = itemsAll.Select(x => x.wrapperMulti).ToList();
		//	this._wrapperMultiRepository.AbortThreadAllThreads(wrapperMultis, "::StopButtonPress", this._userSettingsManager.UploadWakeupTimeGet());
		//	int uploadWakeupTimeOut1 = this._userSettingsManager.UploadWakeupTimeGet() * 1000;
		//	Thread.Sleep(uploadWakeupTimeOut1);

		//	_logger.Info("StopButtonPress end");

		//}

		public void Download()
		{
			_logger.Info("Download start");

			if (Directory.Exists(this._path) == false) return;

			base.IsUploading = true;//UploadToPda_btnStop +
			base.IsDesposed = false;
			//--------------------
			base._canRefresh = false;
			base._refreshCommand.RaiseCanExecuteChanged();

			base._canBrowse = false;
			base._browseCommand.RaiseCanExecuteChanged();

			base.IsCheckedEnabled = false;

			base.IsProgressBarVisible = true;
			this.ProcessDone = false;

			this.IsExecButtonVisible = false;
			Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());
			this._execCommand.RaiseCanExecuteChanged();
			int timeOutFinish1 = this._userSettingsManager.UploadWakeupTimeGet() * 10;
			Thread.Sleep(timeOutFinish1);
			_logger.Info("Thread.Sleep Download  (" + timeOutFinish1 + ")");

			using (new CursorWait())
			{
				try
				{
					//--------------------
					base.WakeUpAndGetTerminalID("Download", false);

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
								string pathIn = this._path;
								string portN = this._items[i].Port.Replace("COM", "");

								string pathPort = this._path + @"\" + portN;
								if (Directory.Exists(pathIn) == true)
								{
									_logger.Info("Fils Download to Path :" + pathPort);
									if (Directory.Exists(pathPort) == false)
									{
										try
										{
											Directory.CreateDirectory(pathPort);
											pathIn = pathPort;
										}
										catch (Exception ex) { _logger.ErrorException("Download : CreateDirectory : " + pathPort, ex); }
									}
									else
									{
										pathIn = pathPort;
									}
								}
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




					_logger.Info("Download + Task.WaitAll(uploadFileTasks) end");
					//----------------------

					//base._canRefresh = false;
					//base._refreshCommand.RaiseCanExecuteChanged();

					//base._canBrowse = false;
					//base._browseCommand.RaiseCanExecuteChanged();

					//base.IsCheckedEnabled = false;

					//base.IsProgressBarVisible = true;


				}
				catch (Exception exc)
				{
					_logger.ErrorException("Download", exc);
				}

			}


			this.IsExecButtonVisible = true;
			Utils.RunOnUI(() => this._execCommand.RaiseCanExecuteChanged());
			this._execCommand.RaiseCanExecuteChanged();
			Thread.Sleep(timeOutFinish1);
			_logger.Info("Thread.Sleep Download  (" + timeOutFinish1 + ")");
		}




	}
}