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
using Count4U.Modules.ContextCBI.Events;
using Count4U.Common.Web;

namespace Count4U.ComplexDefaultAdapter
{																							
	public class ComplexDefaultAdapterViewModel : TemplateComplexAdapterViewModel, IImportPdaAdapter													  //TemplateAdapterFileFolderViewModel
    {
		//protected readonly IIturRepository _iturRepository;
		//protected readonly IReportIniRepository _reportIniRepository;
		//protected readonly IDBSettings _dbSettings;
		//protected readonly IUnityContainer _unityContainer;
		//protected readonly InteractionRequest<MessageBoxYesNoNotification> _yesNoRequest;
		//protected readonly List<string> _newSessionCodeList;
		//protected bool _isAutoPrint;
		//protected bool _isContinueGrabFiles;
		//protected string CurrentAdapterName { get; set; }
		//protected string _fileName { get; set; }

		////=====================

		//protected readonly IImportAdapterRepository _importAdapterRepository;

		//protected IImportModuleInfo _selectedCatalog;
		//protected ObservableCollection<IImportModuleInfo> _itemsCatalogs;
		//protected bool _configFileImportCatalogExists;


		//protected IImportModuleInfo _selectedUpdateCatalog;
		//protected ObservableCollection<IImportModuleInfo> _itemsUpdateCatalog;
		//protected bool _configFileImportUpdateCatalogExists;

		//protected IImportModuleInfo _selectedComplex;
		//protected ObservableCollection<IImportModuleInfo> _itemsComplex;

		//protected ResultModuleInfo _selectedSendToOffice;

		//protected IImportModuleInfo _selectedItur;
		//protected ObservableCollection<IImportModuleInfo> _itemsIturs;
		//protected bool _configFileImportIturExists;

		//protected IImportModuleInfo _selectedLocation;
		//protected ObservableCollection<IImportModuleInfo> _itemsLocations;
		//protected bool _configFileImportLocationExists;

		//protected IImportModuleInfo _selectedSection;
		//protected ObservableCollection<IImportModuleInfo> _itemsSections;
		//protected bool _configFileImportSectionExists;

		//protected IImportModuleInfo _selectedFamily;
		//protected ObservableCollection<IImportModuleInfo> _itemsFamilys;
		//protected bool _configFileImportFamilyExists;

		//protected IImportModuleInfo _selectedSupplier;
		//protected ObservableCollection<IImportModuleInfo> _itemsSuppliers;
		//protected bool _configFileImportSupplierExists;

		//protected IImportModuleInfo _selectedPDA;
		//protected ObservableCollection<IImportModuleInfo> _itemsPDA;
		//protected bool _configFileImportPDAExists;

		////============= Export PDA ===============
		//protected ObservableCollection<IExportPdaModuleInfo> _itemsExportPda;
		//protected IExportPdaModuleInfo _selectedExportPda;
		//protected bool _configFileExportPDAExists;

		////============= Export Erp ===============
		//protected ObservableCollection<IExportErpModuleInfo> _itemsExportErp;
		//protected IExportErpModuleInfo _selectedExportErp;
		//protected bool _configFileExportErpExists;


		//protected readonly UICommandRepository _commandRepository;
		//protected readonly UICommandRepository<IImportModuleInfo> _commandRepositoryImportModuleInfoObject;
		//protected readonly UICommandRepository<IExportPdaModuleInfo> _commandRepositoryExportPdaModuleInfoObject;
		//protected readonly UICommandRepository<IExportErpModuleInfo> _commandRepositoryExportErpModuleInfoObject;
		//protected readonly UICommandRepository<ResultModuleInfo> _commandRepositoryResultModuleInfoObject;


		//protected readonly DelegateCommand<IImportModuleInfo> _runImportByConfigCommand;
		//protected readonly DelegateCommand<IImportModuleInfo> _runImportClearByConfigCommand;
		//protected readonly DelegateCommand<IImportModuleInfo> _showImportLogCommand;
		//protected readonly DelegateCommand<IImportModuleInfo> _showImportConfigCommand;
		//protected readonly DelegateCommand<IImportModuleInfo> _navigateToGridImportCommand;


		//protected readonly DelegateCommand<IExportPdaModuleInfo> _runExportPdaByConfigCommand;
		//protected readonly DelegateCommand<IExportPdaModuleInfo> _runExportPdaClearByConfigCommand;
		//protected readonly DelegateCommand<IExportPdaModuleInfo> _showExportPdaLogCommand;
		//protected readonly DelegateCommand<IExportPdaModuleInfo> _showExportPdaConfigCommand;

		//protected readonly DelegateCommand<IExportErpModuleInfo> _runExportErpByConfigCommand;
		//protected readonly DelegateCommand<IExportErpModuleInfo> _runExportErpClearByConfigCommand;
		//protected readonly DelegateCommand<IExportErpModuleInfo> _showExportErpLogCommand;
		//protected readonly DelegateCommand<IExportErpModuleInfo> _showExportErpConfigCommand;

		//protected readonly DelegateCommand<ResultModuleInfo> _runSendToOfficeCommand;
		//protected readonly DelegateCommand<ResultModuleInfo> _showSendToOfficeIniCommand;

		//protected DelegateCommand _openImportLogPathCommand;
		//protected DelegateCommand _openImportConfigPathCommand;
		//protected DelegateCommand _openExportPdaLogPathCommand;
		//protected DelegateCommand _openExportPdaConfigPathCommand;
		//protected DelegateCommand _openExportErpLogPathCommand;
		//protected DelegateCommand _openExportErpConfigPathCommand;

		//DelegateCommand _openZipPathCommand;
		//DelegateCommand _openIniSendToOfficePathCommand;

		//protected string _zipPath;
		//protected string _iniSendToOfficePath;
		//protected string _importLogPath;
		//protected string _exportPdaLogPath;
		//protected string _exportErpLogPath;
		//protected string _logText;
		//protected string _importConfigPath;
		//protected string _exportPdaConfigPath;
		//protected string _exportErpConfigPath;


		public ComplexDefaultAdapterViewModel(
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
			this.CurrentAdapterName = 
				Common.Constants.ComplexAdapterName.ComplexDefaultAdapter;

			base.ParmsDictionary.Clear();

			base._openImportFixedPathCommand = new DelegateCommand(base.OpenImportFixedPathCommandExecute, base.OpenImportFixedPathCommandCanExecute);
			base._openExportErpFixedPathCommand = new DelegateCommand(base.OpenExportErpFixedPathCommandExecute, base.OpenExportErpFixedPathCommandCanExecute);
			base._openSendToOfficeFixedPathCommand = new DelegateCommand(base.OpenSendToOfficeFixedPathCommandExecute, base.OpenSendToOfficeFixedPathCommandCanExecute);
					 
			base._runImportByConfigCommand = base._commandRepositoryImportModuleInfoObject.Build(enUICommand.RunByConfig, base.RunImportByConfigCommandExecuted);
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
			base._copyFilesImportFromCustomerToInventorCommand = new DelegateCommand(base.CopyFilesImportFromCustomerToInventorCommandExecute);

			base._runSendToOfficeCommand = base._commandRepositoryResultModuleInfoObject.Build(enUICommand.RunByConfig, base.RunSendToOfficeCommandExecuted);
			base._showSendToOfficeIniCommand = base._commandRepositoryResultModuleInfoObject.Build(enUICommand.ShowIni, base.ShowSendToOfficeIniCommandExecuted);
			base._openZipPathCommand = new DelegateCommand(base.OpenZipPathCommandExecute, base.OpenZipPathCommandCanExecute);
			base._openIniSendToOfficePathCommand = new DelegateCommand(base.OpenIniSendToOfficePathCommandExecute, base.OpenIniSendToOfficePathCommandCanExecute);

	    }

		
	



		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			//if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			//{
			//	string configPath = this.GetXDocumentConfigPath(ref info);		//в нескольких местах - надо решить
			//	XDocument configXDoc = new XDocument();
			//	if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
			//	{
			//		try
			//		{
			//			configXDoc = XDocument.Load(configPath);
			//			XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

			//			string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
			//			base.Path = System.IO.Path.GetFullPath(importPath.Trim('\\') + @"\" + this._fileName);
						
			//		}
			//		catch (Exception exp)
			//		{
			//			base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
			//		}
			//	}
			//	else
			//	{
			//		base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
			//	}
			//}
		}

	


        protected override void RunImport(ImportCommandInfo info)
        {
            this.Import();
        }

        protected override void RunClear()
        {
            this.Clear();
        }

		//public InteractionRequest<MessageBoxYesNoNotification> YesNoRequest
		//{
		//	get { return base._yesNoRequest; }
		//}


     

		

		public override void OnNavigatedTo(NavigationContext navigationContext)
		{
			base.OnNavigatedTo(navigationContext);
			ComplexAdapterRecountProductEventPayload payload = new ComplexAdapterRecountProductEventPayload { };
			this._eventAggregator.GetEvent<ComplexAdapterRecountProductEvent>().Publish(payload);

			ComplexAdapterRecountInventProductEventPayload payloadIP = new ComplexAdapterRecountInventProductEventPayload { };
			this._eventAggregator.GetEvent<ComplexAdapterRecountInventProductEvent>().Publish(payloadIP);
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

        protected override void ProcessImportInfo(ImportCommandInfo info)
        {
            ImportFromPdaCommandInfo pdaInfo = info as ImportFromPdaCommandInfo;
            if (pdaInfo != null)
            {
				//this._report = pdaInfo.Report as Count4U.GenerationReport.Report;
                this._isAutoPrint = pdaInfo.IsAutoPrint;
                this._isContinueGrabFiles = pdaInfo.IsContinueGrabFiles;
            }
        }

        #region Implementation of IImportAdapter

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

		private void InitInventProductAdvancedField()
		{
		
		}

        public override void InitFromIni()
        {
		
        }


		public override void Import()
		{
		}

        public override void Clear()
        {
		
        }

        #endregion

        public void RunPrintReport(string documentCode)
        {
		
        }
		
	
    }
}