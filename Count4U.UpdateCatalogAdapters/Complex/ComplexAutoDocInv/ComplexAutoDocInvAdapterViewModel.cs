using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.GenerationReport;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Common.Events;
using Count4U.Common.Constants;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Count4U.Common.Services.UICommandService;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism;
using System.Windows.Input;
using System.Windows.Threading;
using Count4U.Modules.Audit.ViewModels.Export;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Abstract;
using Count4U.Modules.ContextCBI.ViewModels;
using Count4U.Common.Extensions;
using Count4U.ComplexDefaultAdapter;
using Count4U.Modules.ContextCBI.Events;
using Count4U.Modules.Audit;
using Count4U.Common.Enums;
using Count4U.Common.Web;

namespace Count4U.ComplexAutoDocInvAdapter
{																							
	public class ComplexAutoDocInvAdapterViewModel : TemplateComplexAdapterViewModel, IImportPdaAdapter													  //TemplateAdapterFileFolderViewModel
    {

		protected DelegateCommand _runComplexImportByConfigCommand;
		protected DelegateCommand _runComplexExportToPdaByConfigCommand;
		protected DelegateCommand _runComplexImportFromPdaByConfigCommand;
		protected DelegateCommand _runComplexSendToOfficeByConfigCommand;
		protected DelegateCommand _runComplexUpdateByConfigCommand;

		protected DelegateCommand _clearComplexImportByConfigCommand;
		protected DelegateCommand _clearComplexExportToPdaByConfigCommand;
		protected DelegateCommand _clearComplexImportFromPdaByConfigCommand;
		protected DelegateCommand _clearComplexUpdateByConfigCommand;

		protected DelegateCommand _logComplexImportByConfigCommand;
		protected DelegateCommand _logComplexExportToPdaByConfigCommand;
		protected DelegateCommand _logComplexImportFromPdaByConfigCommand;
		
		protected DelegateCommand _logComplexUpdateByConfigCommand;

		protected string _selectedInfrastructuraImportTitle;
		protected bool PrepareInventor { get; set; }
		private bool _goBackAfterSendToOffice;
	

		public ComplexAutoDocInvAdapterViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager,
			UICommandRepository<IImportModuleInfo> commandRepositoryImportModuleInfoObject,
			UICommandRepository<IExportPdaModuleInfo> commandRepositoryExportPdaModuleInfoObject,
			UICommandRepository<IExportErpModuleInfo> commandRepositoryExportErpModuleInfoObject,
			UICommandRepository<ResultModuleInfo> commandRepositoryResultModuleInfoObject,
			 UICommandRepository commandRepository,
            IUnityContainer unityContainer,
			IImportAdapterRepository importAdapterRepository,
			IDBSettings dbSettings,
			IReportIniRepository reportIniRepository,
			FromFtpViewModel fromFtpViewModel,
			ToFtpViewModel toFtpViewModel) :
			base(  serviceLocator, contextCBIRepository, iturRepository, eventAggregator, regionManager,
             logImport,iniFileParser, temporaryInventoryRepository, userSettingsManager,  commandRepositoryImportModuleInfoObject,
			 commandRepositoryExportPdaModuleInfoObject,  commandRepositoryExportErpModuleInfoObject,
			 commandRepositoryResultModuleInfoObject,   commandRepository,	unityContainer,	importAdapterRepository,  dbSettings,
			reportIniRepository, fromFtpViewModel, toFtpViewModel)
        {
		   //!!! Имя адаптера
			base.CurrentAdapterName =
				Common.Constants.ComplexAdapterName.ComplexAutoDocInvAdapter;

			base.ParmsDictionary.Clear();

			base._openImportFixedPathCommand = new DelegateCommand(base.OpenImportFixedPathCommandExecute, base.OpenImportFixedPathCommandCanExecute);
			base._openExportErpFixedPathCommand = new DelegateCommand(base.OpenExportErpFixedPathCommandExecute, base.OpenExportErpFixedPathCommandCanExecute);
			base._openSendToOfficeFixedPathCommand = new DelegateCommand(base.OpenSendToOfficeFixedPathCommandExecute, base.OpenSendToOfficeFixedPathCommandCanExecute);
																																  
			base._openImportInDataPathCommand = new DelegateCommand(base.OpenImportInDataPathCommandExecute, base.OpenImportInDataPathCommandCanExecute);
			base._runImportByConfigCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.RunByConfig, base.RunImportByConfigCommandExecuted); //bool ret = importModuleBaseViewModel.CanImport();
			base._runImportClearByConfigCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.ClearByConfig, base.RunImportClearByConfigCommandExecuted);
			base._showImportLogCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.ShowLogByConfig, base.RunImportShowLogByConfigCommandExecuted);
			base._showImportConfigCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.ShowConfig, base.ShowImportConfigCommandExecuted);
			base._openImportLogPathCommand = new DelegateCommand(base.OpenImportLogPathCommandExecute, base.OpenImportLogPathCommandCanExecute);
			base._openImportConfigPathCommand = new DelegateCommand(base.OpenImportConfigPathCommandExecute, base.OpenImportConfigPathCommandCanExecute);
			base._navigateToGridImportCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.NavigateToGrid, base.NavigateToGridImportCommandExecuted);

			base._runExportPdaByConfigCommand = base._commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.RunByConfig, base.RunExportPdaByConfigCommandExecuted);
			base._runExportPdaClearByConfigCommand = base._commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ClearByConfig, base.RunExportPdaClearByConfigCommandExecuted);
			base._showExportPdaLogCommand = base._commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ShowLogByConfig, base.ShowExportPdaLogCommandExecuted);
			base._showExportPdaConfigCommand = base._commandRepositoryExportPdaModuleInfoObject.Build(enUICommand.ShowConfig, base.ShowExportPdaConfigCommandExecuted);
			base._openExportPdaLogPathCommand = new DelegateCommand(base.OpenExportPdaLogPathCommandExecute, base.OpenExportPdaLogPathCommandCanExecute);
			base._openExportPdaConfigPathCommand = new DelegateCommand(base.OpenExportPdaConfigPathCommandExecute, base.OpenExportPdaConfigPathCommandCanExecute);

			base._runExportErpByConfigCommand = base._commandRepositoryExportErpModuleInfoObject.Build(enUICommand.RunByConfig, base.RunExportErpByConfigCommandExecuted);
			base._runExportErpClearByConfigCommand = base._commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ClearByConfig, base.RunExportErpClearByConfigCommandExecuted);
			base._showExportErpLogCommand = base._commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ShowLogByConfig, base.ShowExportErpLogCommandExecuted);
			base._showExportErpConfigCommand = base._commandRepositoryExportErpModuleInfoObject.Build(enUICommand.ShowConfig, base.ShowExportErpConfigCommandExecuted);
			base._openExportErpLogPathCommand = new DelegateCommand(base.OpenExportErpLogPathCommandExecute, base.OpenExportErpLogPathCommandCanExecute);
			base._openExportErpConfigPathCommand = new DelegateCommand(base.OpenExportErpConfigPathCommandExecute, base.OpenExportErpConfigPathCommandCanExecute);


			base._runSendToOfficeCommand = base._commandRepositoryResultModuleInfoObject.Build(enUICommand.RunByConfig, this.RunSendToOfficeCommandExecuted, RunSendToOfficeCommandCanExecute);
			base._showSendToOfficeIniCommand = base._commandRepositoryResultModuleInfoObject.Build(enUICommand.ShowIni, base.ShowSendToOfficeIniCommandExecuted);
			base._openZipPathCommand = new DelegateCommand(base.OpenZipPathCommandExecute, base.OpenZipPathCommandCanExecute);
			base._openIniSendToOfficePathCommand = new DelegateCommand(base.OpenIniSendToOfficePathCommandExecute, base.OpenIniSendToOfficePathCommandCanExecute);
			
			// ===================== для текущего адаптера =========
			//Run
			this._runComplexImportByConfigCommand = _commandRepository.Build(enUICommand.RunByConfig, this.RunComplexImportByConfigCommandExecute, RunImportCatalogByConfigCommandCanExecute);//this.RunComplexImportByConfigCommandCanExecute);
			this._runComplexExportToPdaByConfigCommand = _commandRepository.Build(enUICommand.RunByConfig, this.RunComplexExportToPdaByConfigCommandExecute, this.RunComplexExportToPdaByConfigCommandCanExecute);
			this._runComplexImportFromPdaByConfigCommand = _commandRepository.Build(enUICommand.RunByConfig, this.RunComplexImportFromPdaByConfigCommandExecute, this.RunComplexImportFromPdaByConfigCommandCanExecute);
			this._runComplexSendToOfficeByConfigCommand = _commandRepository.Build(enUICommand.RunByConfig, this.RunComplexSendToOfficeByConfigCommandExecute, this.RunComplexSendToOfficeByConfigCommandCanExecute);
			  //TODO
			//	this._runComplexUpdateByConfigCommand = _commandRepository.Build(enUICommand.RunByConfig, this.RunComplexUpdateByConfigCommandExecute, this.RunComplexUpdateByConfigCommandCanExecute);

			//Clear																																												   
			this._clearComplexImportByConfigCommand = _commandRepository.Build(enUICommand.ClearByConfig, this.ClearComplexImportByConfigCommandExecute, this.ClearComplexImportByConfigCommandCanExecute);
			this._clearComplexExportToPdaByConfigCommand = _commandRepository.Build(enUICommand.ClearByConfig, this.ClearComplexExportToPdaByConfigCommandExecute, this.ClearComplexExportToPdaByConfigCommandCanExecute);
			this._clearComplexImportFromPdaByConfigCommand = _commandRepository.Build(enUICommand.ClearByConfig, this.ClearComplexImportFromPdaByConfigCommandExecute, this.ClearComplexImportFromPdaByConfigCommandCanExecute);
			this._clearComplexUpdateByConfigCommand = _commandRepository.Build(enUICommand.ClearByConfig, this.ClearComplexUpdateByConfigCommandExecute, this.ClearComplexUpdateByConfigCommandCanExecute);

			//Log 
			this._logComplexImportByConfigCommand = _commandRepository.Build(enUICommand.ShowLogByConfig, this.LogComplexImportByConfigCommandExecute, this.LogComplexImportByConfigCommandCanExecute);
			this._logComplexImportFromPdaByConfigCommand = _commandRepository.Build(enUICommand.ShowLogByConfig, this.LogComplexImportFromPdaByConfigCommandExecute, this.LogComplexImportFromPdaByConfigCommandCanExecute);
			this._logComplexExportToPdaByConfigCommand = _commandRepository.Build(enUICommand.ShowLogByConfig, this.LogComplexExportToPdaByConfigCommandExecute, this.LogComplexExportToPdaByConfigCommandCanExecute);
			this._logComplexUpdateByConfigCommand = _commandRepository.Build(enUICommand.ShowLogByConfig, this.LogComplexUpdateByConfigCommandExecute, this.LogComplexUpdateByConfigCommandCanExecute);

			this._isBusy = false;
			this._goBackAfterSendToOffice = true;

	    }

		private void UpdateStatus(string status)
		{
			//Utils.RunOnUI(() => BusyContent = status);
			BusyContent = status;
			RaisePropertyChanged(() => BusyContent);
		}

		private string _busyContent;
		public string BusyContent
		{
			get { return _busyContent; }
			set
			{
				_busyContent = value;
				RaisePropertyChanged(() => BusyContent);
			}
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				RaisePropertyChanged(() => IsBusy);
			}
		}


		public bool GoBackAfterSendToOffice
		{
			get { return _goBackAfterSendToOffice; }
			set 
			{
				_goBackAfterSendToOffice = value;
				RaisePropertyChanged(() => GoBackAfterSendToOffice);
			}
		}
		

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);
						//  <FROMPATH domainobject="inventor" adapteruse="import" howuse="relative" from="indata" value="" isdefault="1" />
						//<FROMPATH domainobject="inventor" adapteruse="import" howuse="asis" from="absolute" value="C:\ErpPath" isdefault="1" />
						//string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
						//this.ImportFixedPath = importPath;
						//this._openImportFixedPathCommand.RaiseCanExecuteChanged();
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

		//========== SendToOffice USE as override in RunComplexSendToOfficeByConfigCommandExecute (GUI run) =======
		public override void RunSendToOfficeCommandExecuted(ResultModuleInfo importModuleInfo)
		{
			SendToOffice(importModuleInfo);
		}

		//========== SendToOffice USE ==============
		public void SendToOffice(ResultModuleInfo importModuleInfo)
		{
			InventorChangeStatusViewModel inventorChangeStatusViewModel = null;
			try
			{
				inventorChangeStatusViewModel = base.ServiceLocator.GetInstance<InventorChangeStatusViewModel>(Common.ViewModelNames.InventorChangeStatusViewModel);
			}
			catch { }

			using (new CursorWait("RunSendToOfficeCommandExecuted"))
			{
				string targetFile = "";
				if (this.IsBusy == false)
				{
					this.IsBusy = true;

					UpdateStatus("EXPORT ERP");
					
					RunExportErpClearByConfig(this.SelectedExportErp);
					RunExportErpByConfig(this.SelectedExportErp);
					CopyExportErpFolderFormInventorToFixed();

					UpdateStatus("INIT");
					if (inventorChangeStatusViewModel == null) return;
					string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
					string configPathWithFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
					inventorChangeStatusViewModel.InitFromState(base.State, configPathWithFile);
					this._selectedSendToOffice.ZipPath = inventorChangeStatusViewModel.ZipPath;

					UpdateStatus("ZIP");
					//if (string.IsNullOrWhiteSpace(this.SendToOfficeFixedPath) == true)
					inventorChangeStatusViewModel.BuildZipWithoutGUI(Thread.CurrentThread.CurrentUICulture, UpdateStatus,
					this._selectedSendToOffice.ZipPath, base.State);
					

					////UpdateStatus("COPY ZIP");
					////targetFile = CopySendToOfficeZipToFixed(this._selectedSendToOffice.ZipPath);
					this.IsBusy = false;
				}
			}		// waitCursor

			Utils.RunOnUI(() => this.ZipPath = Path.GetDirectoryName(inventorChangeStatusViewModel.ZipPath));
			Utils.RunOnUI(() =>  this.OpenZipPathCommand.RaiseCanExecuteChanged());
		}
		//========================== RUN ===============================  

	 		 //========================= Import =======
		public void RunComplexImportByConfigCommandExecute()
		{
			this.ClearComplexImportByConfigCommandExecute();
			string adapterName = "";
			List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();
			foreach (IImportModuleInfo importInfo in list)
			{
				switch (importInfo.ImportDomainEnum)
				{
					case ImportDomainEnum.ImportCatalog:
						if (base.SelectedCatalog != null)
						{
							base.RunImportByConfigCommandExecuted(base.SelectedCatalog);
							adapterName = base.SelectedCatalog.Name;
						}
						break;
					//case ImportDomainEnum.ImportItur:
					//	if (base.SelectedItur != null)
					//	{
					//		base.RunImportByConfigCommandExecuted(base.SelectedItur);
					//		 adapterName = base.SelectedItur.Name;
					//	}
					//	break;
					//case ImportDomainEnum.ImportLocation:
					//	if (base.SelectedLocation != null)
					//	{
					//		base.RunImportByConfigCommandExecuted(base.SelectedLocation);
					//		adapterName = base.SelectedLocation.Name;
					//	}
					//	break;
					//case ImportDomainEnum.ImportSection:
					//	if (base.SelectedSection != null)
					//	{
					//		base.RunImportByConfigCommandExecuted(base.SelectedSection);
					//		adapterName = base.SelectedSection.Name;
					//	}
					//	break;
					//case ImportDomainEnum.ImportSupplier:
					//	if (base.SelectedSupplier != null)
					//	{
					//		base.RunImportByConfigCommandExecuted(base.SelectedSupplier);
					//		adapterName = base.SelectedSupplier.Name;
					//	}
					//	break;
					//case ImportDomainEnum.ImportFamily:
					//	if (base.SelectedFamily != null)
					//	{
					//		base.RunImportByConfigCommandExecuted(base.SelectedFamily);
					//		adapterName = base.SelectedFamily.Name;
					//	}
					//	break;
				}
	

				ComplexAdapterRecountProductEventPayload payload = new ComplexAdapterRecountProductEventPayload {  };
				this._eventAggregator.GetEvent<ComplexAdapterRecountProductEvent>().Publish(payload);
			}

			base.CopyImportFolderFormFixedToInventor();
			this.LogComplexImportByConfigCommandExecute();
		}

		public virtual bool RunImportCatalogByConfigCommandCanExecute()
		{
			if (this.SelectedCatalog == null) return false;
			base.ConfigFileImportCatalogExists = IsConfigFileImportExists(this.SelectedCatalog);
			return base.ConfigFileImportCatalogExists;
		}

		//public virtual bool RunComplexImportByConfigCommandCanExecute()
		//{
		//	List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();
		//	if (list.Count > 0) return true;
		//	else return false;
		//}

		public DelegateCommand RunComplexImportByConfigCommand
		{
			get { return this._runComplexImportByConfigCommand; }
		}

		//========== ExportToPda =======
		public void RunComplexExportToPdaByConfigCommandExecute()
		{
			base.RunExportPdaClearByConfig(base.SelectedExportPda);
			base.RunExportPdaByConfigCommandExecuted(base.SelectedExportPda);
			this.OpenExportPdaLogPathCommand.RaiseCanExecuteChanged();
		}

		public bool RunComplexExportToPdaByConfigCommandCanExecute()
		{
			if (this.SelectedExportPda == null) return false; 
			base.ConfigFileExportPDAExists = IsConfigFileExportPdaExists(this.SelectedExportPda);
			return base.ConfigFileExportPDAExists;
		}


		public DelegateCommand RunComplexExportToPdaByConfigCommand
		{
			get { return this._runComplexExportToPdaByConfigCommand; }
		}

		//========== ImportFromPda =======
		public void RunComplexImportFromPdaByConfigCommandExecute()
		{
			base.RunImportByConfigCommandExecuted(base.SelectedImportFromPDA);
			ComplexAdapterRecountInventProductEventPayload payloadIP = new ComplexAdapterRecountInventProductEventPayload { };
			this._eventAggregator.GetEvent<ComplexAdapterRecountInventProductEvent>().Publish(payloadIP);

	}

		public bool RunComplexImportFromPdaByConfigCommandCanExecute()
		{
			if (this.SelectedImportFromPDA == null) return false;
			base.ConfigFileImportPDAExists = IsConfigFileImportExists(this.SelectedImportFromPDA);
			return base.ConfigFileImportPDAExists;

		}


		public DelegateCommand RunComplexImportFromPdaByConfigCommand
		{
			get { return this._runComplexImportFromPdaByConfigCommand; }
		}

		//========== SendToOffice USE =======
		public void RunComplexSendToOfficeByConfigCommandExecute()
		{
				this.RunSendToOfficeCommandExecuted(base.SelectedSendToOffice);	   // override in this

			 //Это для перехода
				if (this.GoBackAfterSendToOffice == true)
				{
					ComplexAdapterEventPayload payload = new ComplexAdapterEventPayload { ToContext = ToContext.ToBranchWithoutAction };
					this._eventAggregator.GetEvent<ComplexAdapterEvent>().Publish(payload);
				}
		}

		public bool RunComplexSendToOfficeByConfigCommandCanExecute()
		{
			return true;
		}

		public DelegateCommand RunComplexSendToOfficeByConfigCommand
		{
			get { return this._runComplexSendToOfficeByConfigCommand; }
		}

		//========== _updateByConfigCommand =======
		public void RunComplexUpdateByConfigCommandExecute()
		{
			base.RunImportByConfigCommandExecuted(base.SelectedUpdateCatalog);
		}

		public bool RunComplexUpdateByConfigCommandCanExecute()
		{
			return ConfigFileImportUpdateCatalogExists;
		}

		public DelegateCommand RunComplexUpdateByConfigCommand
		{
			get { return this._runComplexUpdateByConfigCommand; }
		}

		//========================== CLEAR ===============================
		 //========== Import =======
		public void ClearComplexImportByConfigCommandExecute()
		{
			List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();
			string logTxt = "";
		
			foreach (IImportModuleInfo importInfo in list)
			{
				switch (importInfo.ImportDomainEnum)
				{
					case ImportDomainEnum.ImportCatalog:
						base.RunImportClearByConfig(base.SelectedCatalog);
						logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportCatalog.ToString() + Environment.NewLine + base.LogImport.PrintLog();
						break;
					//case ImportDomainEnum.ImportItur:
					//	base.RunImportClearByConfig(base.SelectedItur);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportItur.ToString() + Environment.NewLine + base.LogImport.PrintLog();
					//	break;
					//case ImportDomainEnum.ImportLocation:
					//	base.RunImportClearByConfig(base.SelectedLocation);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportLocation.ToString() + Environment.NewLine + base.LogImport.PrintLog();
					//	break;
					//case ImportDomainEnum.ImportSection:
					//	base.RunImportClearByConfig(base.SelectedSection);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportSection.ToString() + Environment.NewLine + base.LogImport.PrintLog();
					//	break;
					//case ImportDomainEnum.ImportSupplier:
					//	base.RunImportClearByConfig(base.SelectedSupplier);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportSupplier.ToString() + Environment.NewLine + base.LogImport.PrintLog();
					//	break;
					//case ImportDomainEnum.ImportFamily:
					//	base.RunImportClearByConfig(base.SelectedFamily);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportFamily.ToString() + Environment.NewLine + base.LogImport.PrintLog();
					//	break;
				}
			}
			base.LogText = logTxt;

		}

		public bool ClearComplexImportByConfigCommandCanExecute()
		{
			List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();
			if (list.Count > 0) return true;
			else return false;
		}

		public DelegateCommand ClearComplexImportByConfigCommand
		{
			get { return this._clearComplexImportByConfigCommand; }
		}

		//========== ExportToPda =======
		public void ClearComplexExportToPdaByConfigCommandExecute()
		{
			base.RunExportPdaClearByConfig(base.SelectedExportPda);
			base.LogText = base.LogImport.PrintLog();
		}

		public bool ClearComplexExportToPdaByConfigCommandCanExecute()
		{
			base.ConfigFileExportPDAExists = IsConfigFileExportPdaExists(this.SelectedExportPda);
			return base.ConfigFileExportPDAExists;
		}

		public DelegateCommand ClearComplexExportToPdaByConfigCommand
		{
			get { return this._clearComplexExportToPdaByConfigCommand; }
		}

		//========== ImportFromPda =======
		public void ClearComplexImportFromPdaByConfigCommandExecute()
		{
			//base.RunImportClearByConfigCommandExecuted(base.SelectedImportFromPDA);
			RunImportClearByConfig(base.SelectedImportFromPDA);
			base.LogText = base.LogImport.PrintLog();
		}

		public bool ClearComplexImportFromPdaByConfigCommandCanExecute()
		{
			return base.ConfigFileImportPDAExists;
		}

		public DelegateCommand ClearComplexImportFromPdaByConfigCommand
		{
			get { return this._clearComplexImportFromPdaByConfigCommand; }
		}
		//========== _updateByConfigCommand =======
		public void ClearComplexUpdateByConfigCommandExecute()
		{
			base.RunImportClearByConfig(base.SelectedUpdateCatalog);
			base.LogText = base.LogImport.PrintLog();
		}

		public bool ClearComplexUpdateByConfigCommandCanExecute()
		{
			return ConfigFileImportUpdateCatalogExists;
		}

		public DelegateCommand ClearComplexUpdateByConfigCommand
		{
			get { return this._clearComplexUpdateByConfigCommand; }
		}

		//========================== LOG ===============================
		//========== Import =======
		public void LogComplexImportByConfigCommandExecute()
		{
			List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();

			string importLogPath = "";
			string logTxt = "";
			importLogPath = base.RunImportShowLogByConfig(base.SelectedCatalog);
			string logTxtCatalog = this.GetLogTextFromPath(importLogPath);
			logTxt = logTxt + ImportDomainEnum.ImportCatalog.ToString() + Environment.NewLine + logTxtCatalog;

			//foreach (IImportModuleInfo importInfo in list)
			//{
			//	switch (importInfo.ImportDomainEnum)
			//	{
			//		case ImportDomainEnum.ImportCatalog:
			//			importLogPath = base.RunImportShowLogByConfig(base.SelectedCatalog);
			//			string logTxtCatalog = this.GetLogTextFromPath(importLogPath);
			//			logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportCatalog.ToString() + Environment.NewLine + logTxtCatalog;
			//			break;
					//case ImportDomainEnum.ImportItur:
					//	importLogPath = base.RunImportShowLogByConfig(base.SelectedItur);
					//	string logTxtItur = this.GetLogTextFromPath(importLogPath);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportItur.ToString() + Environment.NewLine + logTxtItur;
					//	break;
					//case ImportDomainEnum.ImportLocation:
					//	importLogPath = base.RunImportShowLogByConfig(base.SelectedLocation);
					//	string logTxtLocation = this.GetLogTextFromPath(importLogPath);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportLocation.ToString() + Environment.NewLine + logTxtLocation;
					//	break;
					//case ImportDomainEnum.ImportSection:
					//	importLogPath = base.RunImportShowLogByConfig(base.SelectedSection);
					//	string logTxtSection = this.GetLogTextFromPath(importLogPath);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportSection.ToString() + Environment.NewLine + logTxtSection;
					//	break;
					//case ImportDomainEnum.ImportSupplier:
					//	importLogPath = base.RunImportShowLogByConfig(base.SelectedSupplier);
					//	string logTxtSupplier = this.GetLogTextFromPath(importLogPath);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportSupplier.ToString() + Environment.NewLine + logTxtSupplier;
					//	break;
					//case ImportDomainEnum.ImportFamily:
					//	importLogPath = base.RunImportShowLogByConfig(base.SelectedFamily);
					//	string logTxtFamily = this.GetLogTextFromPath(importLogPath);
					//	logTxt = logTxt + Environment.NewLine + ImportDomainEnum.ImportFamily.ToString() + Environment.NewLine + logTxtFamily;
					//	break;
				//}
			//}
			base.LogText = logTxt;

		}

		public bool LogComplexImportByConfigCommandCanExecute()
		{
			List<IImportModuleInfo> list = GetAdapterInfoListOfImportAdaptersWithConfig();
			if (list.Count > 0) return true;
			else return false;
		}

		public DelegateCommand LogComplexImportByConfigCommand
		{
			get { return this._logComplexImportByConfigCommand; }
		}

		//========== ExportToPda =======
		public void LogComplexExportToPdaByConfigCommandExecute()
		{
			using (new CursorWait("ShowExportPdaLogCommand"))
			{
				string exportPdaLogPath = RunExportPdaShowLogByConfig(base.SelectedExportPda);
				if (string.IsNullOrWhiteSpace(exportPdaLogPath) == false)
				{
					this.RefreshLogText(exportPdaLogPath);
				}
				else
				{
					this.LogText = "";
				}
			}
		}

		public bool LogComplexExportToPdaByConfigCommandCanExecute()
		{
			return true;
		}

		public DelegateCommand LogComplexExportToPdaByConfigCommand
		{
			get { return this._logComplexExportToPdaByConfigCommand; }
		}

		//========== ImportFromPda =======
		public void LogComplexImportFromPdaByConfigCommandExecute()
		{
			string importLogPath = base.RunImportShowLogByConfig(base.SelectedImportFromPDA);
			if (string.IsNullOrWhiteSpace(importLogPath) == false)
			{
				this.RefreshLogText(importLogPath);
			}
			else
			{
				this.LogText = "";
			}
		}

		public bool LogComplexImportFromPdaByConfigCommandCanExecute()
		{
			return true;
		}

		public DelegateCommand LogComplexImportFromPdaByConfigCommand
		{
			get { return this._logComplexImportFromPdaByConfigCommand; }
		}

		//========== _updateByConfigCommand =======
		public void LogComplexUpdateByConfigCommandExecute()
		{
			//base.RunImportShowLogByConfigCommandExecuted(base.SelectedUpdateCatalog);

			string importLogPath = base.RunImportShowLogByConfig(base.SelectedUpdateCatalog);
			if (string.IsNullOrWhiteSpace(importLogPath) == false)
			{
				this.RefreshLogText(importLogPath);
			}
			else
			{
				this.LogText = "";
			}
		}

		public bool LogComplexUpdateByConfigCommandCanExecute()
		{
			return true;
		}

		public DelegateCommand LogComplexUpdateByConfigCommand
		{
			get { return this._logComplexUpdateByConfigCommand; }
		}

		public void RefreshLogText(string logFolderPath)
		{
			if (System.IO.Directory.Exists(logFolderPath) == true)
			{
				//this.LogText = System.IO.File.ReadAllText(this.LogPath);
				List<string> getFiles = Directory.GetFiles(logFolderPath).ToList();
				//base.LogImport.PrintLog();
				foreach (string filePath in getFiles)
				{
					FileInfo fi = new FileInfo(filePath);
					if (fi.Name.Contains(".full.log.txt") == true)
					{
						continue;
					}
					if (fi.Name.Contains(".log.txt") == true)
					{
						this.LogText = System.IO.File.ReadAllText(filePath);
						break;
					}
				}
			}
		}

		public string GetLogTextFromPath(string logFolderPath)
		{
			string logtext = "";
			if (System.IO.Directory.Exists(logFolderPath) == true)
			{
				//this.LogText = System.IO.File.ReadAllText(this.LogPath);
				List<string> getFiles = Directory.GetFiles(logFolderPath).ToList();
				foreach (string filePath in getFiles)
				{
					FileInfo fi = new FileInfo(filePath);
					if (fi.Name.Contains(".full.log.txt") == true)
					{
						continue;
					}
					if (fi.Name.Contains(".log.txt") == true)
					{
						logtext = System.IO.File.ReadAllText(filePath);
						break;
					}
				}
			}
			return logtext;
		}



		// ==================== END ===================

		//protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		// что-то есть  в TemplateCompexAdapter base	 =>  InitCatalogFromConfig
		protected void InitComplexFromConfig(ImportCommandInfo info)
		{
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);		//в нескольких местах - надо решить
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
	
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);   //Здесь инициализация из .Config
						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

		public void InitComplexDefault()
		{
			base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			//base.ProcessLisner = 0;
			//InitInventProductAdvancedField();
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			this.StepTotal = 1;
			this.Session = 0;
		}


	


		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);

			// если что так в base классе конкретном адаптере, можно перекрыть здесь
			// подумаеь 3 дня
			//ImportCommandInfo info = new ImportCommandInfo();
			//info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
			//info.AdapterName = this.SelectedCatalog.Name;
			//InitCatalogFromConfig(info, this.State);

			// здесь только каталог импортируется
			base.SelectedLocation = null;
			base.SelectedItur = null;
			base.SelectedSection = null;
			base.SelectedFamily = null;
			base.SelectedSupplier = null;


			ImportCommandInfo info = new ImportCommandInfo(); //все это сомнительно - потом рефакторить
			info.AdapterName = Common.Constants.ComplexAdapterName.ComplexAutoDocInvAdapter;
			info.FromConfigXDoc = ConfigXDocFromEnum.FromCustomerInData;
			info.ConfigXDocumentPath = "";
			info.Mode = ImportDomainEnum.ComplexOperation; 
			InitComplexFromConfig(info);

			this.InitComplexDefault();

			this.PrepareInventor = false;

			// FromBranchWithAction не срабатывает из-за SelectAdapter, где Withoutaction - !!! TODO пока Ирану не надо
			if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.FromContext && r.Value == FromContext.FromBranchWithAction.ToString()))
			{

				this.PrepareInventor = true;
				using (CursorWait wait = new CursorWait("CopyFile"))
				{
					base.CopyImportFolderFormCustomerToInventor();
					this.ClearComplexImportByConfigCommandExecute();
					this.RunComplexImportByConfigCommandExecute();
				}

			}
			//						public static void GenerationAndSaveExportErpConfig(this Customer customer, IServiceLocator serviceLocator, 
			//IExportErpModuleInfo exportErpModuleInfo, bool resave)
			Customer customer = base.CurrentCustomer;
			
	
			//HowUse howUse = HowUse.relative;
			//if (string.IsNullOrWhiteSpace(base.ExportErpFixedPath) == false) howUse = HowUse.asis;
			ExportImportWithoutGui.GenerationAndSaveExportErpConfig(customer, this.ServiceLocator, base.SelectedExportErp, DomainObjectType.inventor, HowUse.relative, this.State, false);
			
			ExportImportWithoutGui.GenerationAndSaveExportPdaConfig(customer, this.ServiceLocator, base.SelectedExportPda, DomainObjectType.inventor, HowUse.relative, this.State, false);
			ExportImportWithoutGui.GenerationAndSaveImportConfig(customer, this.ServiceLocator, base.SelectedImportFromPDA as ImportModuleInfo, DomainObjectType.inventor, HowUse.relative, this.State, false);

			HowUse howUse = HowUse.relative;
			if (string.IsNullOrWhiteSpace(base.ImportFixedPath) == false) howUse = HowUse.asis;
			ExportImportWithoutGui.GenerationAndSaveImportConfig(customer, this.ServiceLocator, base.SelectedCatalog as ImportModuleInfo, DomainObjectType.inventor, howUse, this.State, false);
		

			RaisePropertyChanged(() => SelectedCatalog);
			RaisePropertyChanged(() => SelectedCatalogName);

			//base.InitAdapterExportErpFromConfig(this.SelectedExportErp);

			//RaisePropertyChanged(() => SelectedCatalogTitle);
			
			//RaisePropertyChanged(() => ImportFixedPath);

			this.RunComplexImportByConfigCommand.RaiseCanExecuteChanged();
			this.RunComplexExportToPdaByConfigCommand.RaiseCanExecuteChanged();
			this.RunComplexImportFromPdaByConfigCommand.RaiseCanExecuteChanged();

			ComplexAdapterRecountProductEventPayload payload = new ComplexAdapterRecountProductEventPayload { };
			this._eventAggregator.GetEvent<ComplexAdapterRecountProductEvent>().Publish(payload);

			ComplexAdapterRecountInventProductEventPayload payloadIP = new ComplexAdapterRecountInventProductEventPayload { };
			this._eventAggregator.GetEvent<ComplexAdapterRecountInventProductEvent>().Publish(payloadIP);

		}


		public override void FillAdaptersNameFromCustomer(Customer customer)
		{
			//this._selectedComplex = this._itemsComplex.FirstOrDefault(r => r.Name == this.CurrentAdapterName);

			// ============== Import Adapters =========================
			// ============== Customer ================================
			if (customer != null)
			{
				string catalogCode = customer.ImportCatalogProviderCode;
				this._configFileImportCatalogExists = false;
				// =========== ImportCatalog
				if (!String.IsNullOrEmpty(catalogCode) && _itemsCatalogs.Any(r => r.Name == catalogCode))
					SelectedCatalog = _itemsCatalogs.FirstOrDefault(r => r.Name == catalogCode);
				else
					SelectedCatalog = _itemsCatalogs.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportCatalogExists = IsConfigFileImportExists(this.SelectedCatalog);

				//this._configFileImportUpdateCatalogExists = false;
				//if (!String.IsNullOrEmpty(customer.UpdateCatalogAdapterCode) && _itemsUpdateCatalog.Any(r => r.Name == customer.UpdateCatalogAdapterCode))
				//	SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.Name == customer.UpdateCatalogAdapterCode);
				//else
				//	SelectedUpdateCatalog = _itemsUpdateCatalog.FirstOrDefault(r => r.IsDefault);
				//ConfigFileImportUpdateCatalogExists = IsConfigFileImportExists(this.SelectedUpdateCatalog);

				//string iturCode = customer.ImportIturProviderCode;
				//if (!String.IsNullOrEmpty(iturCode) && _itemsIturs.Any(r => r.Name == iturCode))
				//	SelectedItur = _itemsIturs.FirstOrDefault(r => r.Name == iturCode);
				//else
				//	SelectedItur = _itemsIturs.FirstOrDefault(r => r.IsDefault);

				//ConfigFileImportIturExists = IsConfigFileImportExists(this.SelectedItur);

				//string locationCode = customer.ImportLocationProviderCode;
				//if (!String.IsNullOrEmpty(locationCode) && _itemsLocations.Any(r => r.Name == locationCode))
				//	SelectedLocation = _itemsLocations.FirstOrDefault(r => r.Name == locationCode);
				//else
				//	SelectedLocation = _itemsLocations.FirstOrDefault(r => r.IsDefault);

				//ConfigFileImportLocationExists = IsConfigFileImportExists(this.SelectedLocation);

				//string sectionCode = customer.ImportSectionAdapterCode;
				//if (!String.IsNullOrEmpty(sectionCode) && _itemsSections.Any(r => r.Name == sectionCode))
				//	SelectedSection = _itemsSections.FirstOrDefault(r => r.Name == sectionCode);
				//else
				//	SelectedSection = _itemsSections.FirstOrDefault(r => r.IsDefault);

				//ConfigFileImportSectionExists = IsConfigFileImportExists(this.SelectedSection);

				//string familyCode = customer.ContactPerson;
				//if (!String.IsNullOrEmpty(familyCode) && _itemsFamilys.Any(r => r.Name == familyCode))
				//	SelectedFamily = _itemsFamilys.FirstOrDefault(r => r.Name == familyCode);
				//else
				//	SelectedFamily = _itemsFamilys.FirstOrDefault(r => r.IsDefault);

				//ConfigFileImportFamilyExists = IsConfigFileImportExists(this.SelectedFamily);

				//string supplierCode = customer.ImportSupplierAdapterCode;
				//if (!String.IsNullOrEmpty(supplierCode) && _itemsSuppliers.Any(r => r.Name == supplierCode))
				//	SelectedSupplier = _itemsSuppliers.FirstOrDefault(r => r.Name == supplierCode);
				//else
				//	SelectedSupplier = _itemsSuppliers.FirstOrDefault(r => r.IsDefault);

				//ConfigFileImportSupplierExists = IsConfigFileImportExists(this.SelectedSupplier);
				// ==============  Import from PDA
				string importPdaCode = customer.ImportPDAProviderCode;
				if (!String.IsNullOrEmpty(importPdaCode) && _itemsPDA.Any(r => r.Name == importPdaCode))
					SelectedImportFromPDA = _itemsPDA.FirstOrDefault(r => r.Name == importPdaCode);
				else
					SelectedImportFromPDA = _itemsPDA.FirstOrDefault(r => r.IsDefault);

				ConfigFileImportPDAExists = IsConfigFileImportExists(this.SelectedImportFromPDA);

				// ==============  Export PDA
				string exportPdaCode = customer.ExportCatalogAdapterCode;
				IExportPdaModuleInfo defaultExportPda = _itemsExportPda.FirstOrDefault(r => r.IsDefault);
				if (String.IsNullOrWhiteSpace(exportPdaCode) == false)
				{
					if (this._itemsExportPda.Any(r => r.Name == exportPdaCode))
						defaultExportPda = _itemsExportPda.FirstOrDefault(r => r.Name == exportPdaCode);
				}
				this.SelectedExportPda = defaultExportPda;

				ConfigFileExportPDAExists = IsConfigFileExportPdaExists(this.SelectedExportPda);

				// ==============  Export Erp
				string exportErpCode = customer.ExportERPAdapterCode;
				IExportErpModuleInfo defaultExportErp = _itemsExportErp.FirstOrDefault(r => r.IsDefault);
				if (String.IsNullOrWhiteSpace(exportErpCode) == false)
				{
					if (this._itemsExportErp.Any(r => r.Name == exportErpCode))
						defaultExportErp = _itemsExportErp.FirstOrDefault(r => r.Name == exportErpCode);
				}
				this.SelectedExportErp = defaultExportErp;
				ConfigFileExportErpExists = IsConfigFileExportErpExists(this.SelectedExportErp);

				// ==============  Zip =====================
				ResultModuleInfo resultModuleInfo = new ResultModuleInfo();
				resultModuleInfo.ZipPath = ZipPath;
				string importPath = this._contextCBIRepository.GetConfigFolderPath(base.CurrentCustomer);
				resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFileToCustomer(importPath);
				//resultModuleInfo.ConfigPathWithFile = this._reportIniRepository.CopyReportTemplateIniFile(base.CurrentCustomer.Code, "Customer");
				this.IniSendToOfficePath = System.IO.Path.GetDirectoryName(resultModuleInfo.ConfigPathWithFile);
				resultModuleInfo.ConfigPath = "";
				if (string.IsNullOrWhiteSpace(resultModuleInfo.ConfigPathWithFile) == false)
				{
					resultModuleInfo.ConfigPath = System.IO.Path.GetDirectoryName(resultModuleInfo.ConfigPathWithFile);
				}

				this.SelectedSendToOffice = resultModuleInfo;


				this.SelectedInfrastructuraImportTitle = this.GetTitleImportAdapters();
				SelectedInfrastructuraImportTooltip = this.GetTooltipImportAdapters();
			}



		}

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

		//public void Refresh()
		//{
		//	IDocumentHeaderRepository documentHeaderRepository = base.ServiceLocator.GetInstance<IDocumentHeaderRepository>();
		//	List<string> docCodes = documentHeaderRepository.GetDocumentCodeList(base.GetDbPath);
		//}

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }


        #region Implementation of IImportAdapter
    
		#endregion

		#region	   not use
		public override void InitDefault(CBIState state = null)
		{
			if (state != null) base.State = state;
			base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
			//base.ProcessLisner = 0;
			//InitInventProductAdvancedField();
			base.IsInvertLetters = false;
			base.IsInvertWords = false;
			this.StepTotal = 1;
			this.Session = 0;
		}

		private void InitInventProductAdvancedField() {	}
        public override void InitFromIni()  { }
		public override void Import(){}
        public override void Clear()  {  }
		public void RunPrintReport(string documentCode) { }
		protected override void ProcessImportInfo(ImportCommandInfo info)
		{
			ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
			if (pdaInfo != null)
			{
				//this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
				//this._isAutoPrint = pdaInfo.IsAutoPrint;
				//this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
			}
		}

		protected override void RunImport(ImportCommandInfo info)
		{
			//this.Import();
		}

		protected override void RunClear()
		{
			// this.Clear();
		}

		//public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
		//{
		//	get { return base._yesNoRequest; }
		//}

        #endregion

       
		
	
    }
}