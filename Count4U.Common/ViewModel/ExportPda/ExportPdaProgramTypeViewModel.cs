using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Ini;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Model;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface;
using Microsoft.Practices.Unity;
using System.Linq;
using NLog;
using Count4U.Model.Extensions;

namespace Count4U.Common.ViewModel.ExportPda
{
    public class ExportPdaProgramTypeViewModel : CBIContextBaseViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string SectionPDAType = "PDAType";
        private const string SectionMaintenanceType = "MaintenanceType";
        private const string SectionProgramType = "ProgramType";

        private readonly IDBSettings _dbSettings;
        private readonly IUnityContainer _unityContainer;
        private readonly IImportAdapterRepository _importAdapterRepository;
        private readonly IIniFileParser _iniFileParser;

        private readonly ObservableCollection<ExportPdaProgramTypeItemViewModel> _pdaTypeItems;
        private ExportPdaProgramTypeItemViewModel _pdaType;

        private readonly ObservableCollection<ExportPdaProgramTypeItemViewModel> _maintenanceTypeItems;
        private ExportPdaProgramTypeItemViewModel _maintenanceType;

        private readonly ObservableCollection<ExportPdaProgramTypeItemViewModel> _programTypeItems;
        private ExportPdaProgramTypeItemViewModel _programType;

        private bool _isEditable;

        private readonly DelegateCommand _pdaTypeOpenCommand;
        private readonly DelegateCommand _maintenanceTypeOpenCommand;
        private readonly DelegateCommand _programTypeOpenCommand;
        private string _adapterExportDir;

		private bool _isMISVisible;
		private bool _isHT360Visible;

        public ExportPdaProgramTypeViewModel(
            IContextCBIRepository contextCbiRepository,
            IDBSettings dbSettings,
            IUnityContainer unityContainer,
            IImportAdapterRepository importAdapterRepository,
            IIniFileParser iniFileParser)
            : base(contextCbiRepository)
        {
            _iniFileParser = iniFileParser;
            _importAdapterRepository = importAdapterRepository;
            _unityContainer = unityContainer;
            _dbSettings = dbSettings;
            _pdaTypeItems = new ObservableCollection<ExportPdaProgramTypeItemViewModel>();
            _maintenanceTypeItems = new ObservableCollection<ExportPdaProgramTypeItemViewModel>();
            _programTypeItems = new ObservableCollection<ExportPdaProgramTypeItemViewModel>();

            _pdaTypeOpenCommand = new DelegateCommand(PdaTypeOpenCommandExecuted, PdaTypeOpenCommandCanExecute);
            _maintenanceTypeOpenCommand = new DelegateCommand(MaintenanceTypeOpenCommandExecuted, MaintenanceTypeOpenCommandCanExecute);
            _programTypeOpenCommand = new DelegateCommand(ProgramTypeOpenCommandExecuted, ProgramTypeOpenCommandCanExecute);
			_adapterExportDir = "";
			this.IsMISVisible = false;
			this.IsHT360Visible = false;
        }

		[NotInludeAttribute]
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

		[NotInludeAttribute]
		public bool IsMISVisible
		{
			get { return _isMISVisible; }
			set
			{
				_isMISVisible = value;
				RaisePropertyChanged(() => IsMISVisible);
			}
		}

		[NotInludeAttribute]
		public bool IsHT360Visible
		{
			get { return _isHT360Visible; }
			set
			{
				_isHT360Visible = value;
				RaisePropertyChanged(() => IsHT360Visible);
			}
		}

		[NotInludeAttribute]
        public ObservableCollection<ExportPdaProgramTypeItemViewModel> PdaTypeItems
        {
            get { return _pdaTypeItems; }
        }

		[NotInludeAttribute]
        public ExportPdaProgramTypeItemViewModel PdaType
        {
            get { return _pdaType; }
            set { _pdaType = value; }
        }

		[NotInludeAttribute]
        public ObservableCollection<ExportPdaProgramTypeItemViewModel> MaintenanceTypeItems
        {
            get { return _maintenanceTypeItems; }
        }

		[NotInludeAttribute]
        public ExportPdaProgramTypeItemViewModel MaintenanceType
        {
            get { return _maintenanceType; }
            set { _maintenanceType = value; }
        }

		[NotInludeAttribute]
        public ObservableCollection<ExportPdaProgramTypeItemViewModel> ProgramTypeItems
        {
            get { return _programTypeItems; }
        }

		[NotInludeAttribute]
        public ExportPdaProgramTypeItemViewModel ProgramType
        {
            get { return _programType; }
            set { _programType = value; }
        }

		[NotInludeAttribute]
        public DelegateCommand PdaTypeOpenCommand
        {
            get { return _pdaTypeOpenCommand; }
        }

		[NotInludeAttribute]
        public DelegateCommand MaintenanceTypeOpenCommand
        {
            get { return _maintenanceTypeOpenCommand; }
        }

		[NotInludeAttribute]
        public DelegateCommand ProgramTypeOpenCommand
        {
            get { return _programTypeOpenCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Build();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

		public void Build()
        {
            try
            {
                List<IExportPdaModuleInfo> exportPdaModuleInfo = Utils.GetExportPdaAdapters(_unityContainer, _importAdapterRepository, String.Empty, String.Empty, String.Empty);

                IExportPdaModuleInfo ht630 = exportPdaModuleInfo.FirstOrDefault(r => r.Name == ExportPdaAdapterName.ExportHT630Adapter);

                if (ht630 == null)
                {
                    _logger.Warn("ht630 module is missing");
                    return;
                }

                string name = ht630.UserControlType.Assembly.GetName().Name;
                string iniName = string.Format("{0}{1}", name, ".ini");

                string iniPath = Path.Combine(FileSystem.ExportModulesFolderPath(), iniName);

                if (!File.Exists(iniPath))
                {
                    _logger.Warn("INI file is missing: {0}", iniPath);
                    return;
                }

                List<IniFileData> iniData = _iniFileParser.Get(iniPath);

                foreach (IniFileData section in iniData)
                {
                    if (section.SectionName == SectionPDAType)
                    {
                        foreach (KeyValuePair<string, string> kvp in section.Data)
                        {
                            _pdaTypeItems.Add(new ExportPdaProgramTypeItemViewModel() { Key = kvp.Key, Value = kvp.Value });
                        }
                    }

                    if (section.SectionName == SectionMaintenanceType)
                    {
                        foreach (KeyValuePair<string, string> kvp in section.Data)
                        {
                            _maintenanceTypeItems.Add(new ExportPdaProgramTypeItemViewModel() { Key = kvp.Key, Value = kvp.Value });
                        }
                    }

                    if (section.SectionName == SectionProgramType)
                    {
                        foreach (KeyValuePair<string, string> kvp in section.Data)
                        {
                            _programTypeItems.Add(new ExportPdaProgramTypeItemViewModel() { Key = kvp.Key, Value = kvp.Value });
                        }
                    }

				
                }               
            }
            catch (Exception e)
            {
                _logger.ErrorException("Build", e);
            }
        }

        public void FillAdapterCustomerData(Customer customer)
        {
			ExportPdaProgramTypeItemViewModel pdaType = null;
			if (customer != null)
			{
				pdaType = this._pdaTypeItems.FirstOrDefault(r => r.Key == customer.PDAType);
			}
			PdaType = pdaType ?? this._pdaTypeItems.FirstOrDefault();	   

			ExportPdaProgramTypeItemViewModel maintenanceType = null;
			if (customer != null)
			{
				maintenanceType = this._maintenanceTypeItems.FirstOrDefault(r => r.Key == customer.MaintenanceType);
			}
			MaintenanceType = maintenanceType ?? this._maintenanceTypeItems.FirstOrDefault();

            ExportPdaProgramTypeItemViewModel programType = null;
			if (customer != null)
			{
				programType = this._programTypeItems.FirstOrDefault(r => r.Key == customer.ProgramType);
			}
			ProgramType = programType ?? this._programTypeItems.FirstOrDefault();
        }

		/// <summary>
		/// заполнение данных по умолчанию для каждого адаптера своих
		/// потом - на следущем шаге заполняется из кастомра
		/// </summary>
		/// <param name="adapter"></param>
        public void FillGUIAdapterData(IExportPdaModuleInfo adapter,  ExportCommandInfo info )
        {
	        if (adapter == null)
            {
                _adapterExportDir = String.Empty;
            }
            else
            {
                string sourceDir = _dbSettings.ExportToPdaFolderPath();
                _adapterExportDir = sourceDir = Path.Combine(sourceDir, adapter.Name);
                if (!Directory.Exists(sourceDir))
                {
                    Directory.CreateDirectory(sourceDir);
                }
            }
            _pdaTypeOpenCommand.RaiseCanExecuteChanged();
            _maintenanceTypeOpenCommand.RaiseCanExecuteChanged();
            _programTypeOpenCommand.RaiseCanExecuteChanged();
        }

        public void Save(Customer customer)
        {
            string pdaType = String.Empty;
            string maintenanceType = String.Empty;
            string programType = String.Empty;

            if (_pdaType != null)
            {
                pdaType = _pdaType.Key;
            }

            if (_maintenanceType != null)
            {
                maintenanceType = _maintenanceType.Key;
            }

            if (_programType != null)
            {
                programType = _programType.Key;
            }

            customer.PDAType = pdaType;
            customer.MaintenanceType = maintenanceType;
            customer.ProgramType = programType;
        }


		public string PdaTypeKey
        {
            get
			{ 
				if 	(_pdaType == null) return "";
				return _pdaType.Key; 
			}
			set
			{
				ExportPdaProgramTypeItemViewModel pdaType = null;
				if (value != null)
				{
					pdaType = this._pdaTypeItems.FirstOrDefault(r => r.Key == value);
				}
				PdaType = pdaType ?? this._pdaTypeItems.FirstOrDefault();
			}
        }
	
			//	ExportPdaProgramTypeItemViewModel pdaType = null;
			//if (customer != null)
			//{
			//	pdaType = this._pdaTypeItems.FirstOrDefault(r => r.Key == customer.PDAType);
			//}
			//PdaType = pdaType ?? this._pdaTypeItems.FirstOrDefault();	   

			//ExportPdaProgramTypeItemViewModel maintenanceType = null;
			//if (customer != null)
			//{
			//	maintenanceType = this._maintenanceTypeItems.FirstOrDefault(r => r.Key == customer.MaintenanceType);
			//}
			//MaintenanceType = maintenanceType ?? this._maintenanceTypeItems.FirstOrDefault();

			//ExportPdaProgramTypeItemViewModel programType = null;
			//if (customer != null)
			//{
			//	programType = this._programTypeItems.FirstOrDefault(r => r.Key == customer.ProgramType);
			//}
			//ProgramType = programType ?? this._programTypeItems.FirstOrDefault();

		public string MaintenanceTypeKey
        {
            get
			{ 
				if 	(_maintenanceType == null) return "";
				return _maintenanceType.Key; 
			}
			set
			{
				ExportPdaProgramTypeItemViewModel maintenanceType = null;
				if (value != null)
				{
					maintenanceType = this._maintenanceTypeItems.FirstOrDefault(r => r.Key == value);
				}
				MaintenanceType = maintenanceType ?? this._maintenanceTypeItems.FirstOrDefault();
			}
        }

        public string ProgramTypeKey
        {
            get
			{ 
				if 	(_programType == null) return "";
				return _programType.Key; 
			}
			set
			{
  				ExportPdaProgramTypeItemViewModel programType = null;
				if (value != null)
				{
					programType = this._programTypeItems.FirstOrDefault(r => r.Key == value);
				}
				ProgramType = programType ?? this._programTypeItems.FirstOrDefault();
			}
        }


		/// <summary>
		/// В  ExportCommandInfo упаковываем данные из контрола.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
        public ExportCommandInfo  FillExportProgramTypeInfo(ExportCommandInfo info)
        {
            string pdaType = String.Empty;
            string maintenanceType = String.Empty;
            string programType = String.Empty;

            if (_pdaType != null)
            {
                pdaType = _pdaType.Value;
            }

            if (_maintenanceType != null)
            {
                maintenanceType = _maintenanceType.Value;
            }

            if (_programType != null)
            {
                programType = _programType.Value;
            }

            info.PDAType = pdaType;
            info.MaintenanceType = maintenanceType;
            info.ProgramType = programType;
			return info;
        }

        private bool PdaTypeOpenCommandCanExecute()
        {
            if (_pdaType == null)
                return false;

            return CanOpenSubfolder(_pdaType.Value);
        }

        private void PdaTypeOpenCommandExecuted()
        {
            if (_pdaType == null)
                return;

            OpenSubfolder(_pdaType.Value);
        }

        private bool MaintenanceTypeOpenCommandCanExecute()
        {
            if (_maintenanceType == null)
                return false;

            return CanOpenSubfolder(_maintenanceType.Value);
        }

        private void MaintenanceTypeOpenCommandExecuted()
        {
            if (_maintenanceType == null)
                return;

            OpenSubfolder(_maintenanceType.Value);
        }

        private bool ProgramTypeOpenCommandCanExecute()
        {
            if (_programType == null)
            return false;

            return CanOpenSubfolder(_programType.Value);
        }

        private void ProgramTypeOpenCommandExecuted()
        {
            if (_programType == null)
                return;

            OpenSubfolder(_programType.Value);
        }

        private bool CanOpenSubfolder(string subfolder)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(subfolder)) return false;

                if (String.IsNullOrWhiteSpace(_adapterExportDir)) return false;

                string finalDir = Path.Combine(_adapterExportDir, subfolder.Trim('"'));

                if (!Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("CanOpenSubfolder", exc);

                return false;
            }

            return true;
        }

        private void OpenSubfolder(string subfolder)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(subfolder)) return;

                if (String.IsNullOrWhiteSpace(_adapterExportDir)) return;

                string finalDir = Path.Combine(_adapterExportDir, subfolder.Trim('"'));

                if (!Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }

                Utils.OpenFolderInExplorer(finalDir);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OpenSubfolder", exc);
            }
        }
    }
}