using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Count4U.Common.Events;
using NLog;
using Count4U.Modules.ContextCBI.ViewModels;
using System.Collections.ObjectModel;
using Count4U.GenerationReport;
using Count4U.Model.Count4U;
using Count4U.Common.UserSettings;
using System.IO;
using Count4U.Model.Interface;
using Count4U.Common.Services.Ini;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Count4U.Common.Extensions;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;

namespace Count4U.Modules.Audit.ViewModels
{
    public class TagSelectViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ISectionRepository _sectionRepository;
		
		 private readonly IReportRepository _reportRepository;
		 private readonly IGenerateReportRepository _generateReportRepository;
		 private readonly IReportIniRepository _reportIniRepository;
		 private readonly IServiceLocator _serviceLocator;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
		

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _reloadConfigCommand;
		private readonly DelegateCommand _sendDataCommand;

		private readonly ObservableCollection<ReportPrintItemViewModel> _reports;
		private readonly IUserSettingsManager _userSettingsManager;

      	private string _countObjects;
		private string _codes;
		private string _reportIniFile;

        private string _tag;
		private bool _substring;

		private string _path;
		private bool _isBusy;
		private string _busyContent;

		private List<string> _objectTypes;
		private string _selectedObjectType;
		private  string _saveReportFolderPath;

		private readonly IDBSettings _dbSettings;
		private readonly IIniFileParser _iniFileParser;
		private readonly SendDataOffice _sendDataOffice;

		public TagSelectViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IIturRepository iturRepository,
			ISectionRepository sectionRepository,
			ILocationRepository locationRepository ,
			SendDataOffice sendDataOffice,
			IDBSettings dbSettings,
			IIniFileParser iniFileParser,
			IUserSettingsManager userSettingsManager,
			IReportRepository reportRepository,
			 IGenerateReportRepository generateReportRepository,
			IReportIniRepository reportIniRepository,
			 IServiceLocator serviceLocator,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository)
            : base(contextCbiRepository)
        {
			this._iniFileParser = iniFileParser;
			this._dbSettings = dbSettings;
            this._iturRepository = iturRepository;
			 this._sectionRepository = sectionRepository;
			this._locationRepository = locationRepository;
			this._reportRepository = reportRepository;
			this._generateReportRepository = generateReportRepository;
			this._reportIniRepository = reportIniRepository;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._eventAggregator = eventAggregator;
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._sendDataOffice = sendDataOffice;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._reports = new ObservableCollection<ReportPrintItemViewModel>();
			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);
			this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);
			
        }

		public List<string> ObjectTypes 
		{  
			get { return _objectTypes; } 
		}

		public void SetDefaultSelectedObjectType()
		{
			this.Substring = this._userSettingsManager.TagSubstringGet();
			this.SelectedObjectType = this._userSettingsManager.DomainObjectSelectedItemGet();
			
		}


		public string SelectedObjectType
		{
			 get { return _selectedObjectType; }
            set
            {
                _selectedObjectType = value;
                RaisePropertyChanged(() => SelectedObjectType);
				this._tag = "";
				RaisePropertyChanged(() => Tag);
				RaisePropertyChanged(() => PathSave);

				this.CountAndFillCodes();
				this.BuildReports();
				_okCommand.RaiseCanExecuteChanged();
				
            }
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				RaisePropertyChanged(() => IsBusy);
			}
		}

		public string PathSave
		{
			get { return this._saveReportFolderPath + "[" + this.Tag + "]"; }
			set
			{
				PathSave = value;
				RaisePropertyChanged(() => PathSave);

			//	//_sendDataCommand.RaiseCanExecuteChanged();
			}
		}

		//private bool IsOkZipPath()
		//{
		//	if (String.IsNullOrWhiteSpace(_path))
		//		return false;

		//	try
		//	{
		//		FileInfo fi = new FileInfo(_path);
		//		if (!fi.Directory.Exists)
		//			return false;

		//		if (String.IsNullOrWhiteSpace(fi.Name))
		//			return false;
		//	}
		//	catch
		//	{
		//		return false;
		//	}

		//	return true;
		//}

		private void SetDefaultPath()
		{
			string sendDataFolderPath = UtilsPath.ZipOfficeFolder(_dbSettings);

			string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
			string reportFolder = String.Format("{0}_{1}_{2}",
													   base.CurrentCustomer.Code,
													   base.CurrentBranch.BranchCodeERP,
													   date);

			this._saveReportFolderPath = Path.Combine(sendDataFolderPath, reportFolder);

		//	RaisePropertyChanged(() => PathSave);
			//this.PathSave = this._saveReportFolderPath;
		}

		public bool Substring
		{
			get { return _substring; }
            set
            {
				_substring = value;
				RaisePropertyChanged(() => Substring);
				RaisePropertyChanged(() => PathSave);

				this.CountAndFillCodes();
            }
		}

		//public string TextValue
		//{
		//	get { return this._textValue; }
		//	set
		//	{
		//		this._textValue = value;
		//		RaisePropertyChanged(() => TextValue);

		//		List<int> res = CommaDashStringParser.Parse(this._textValue);//TODO количество Itur
		//		TotalIturs = res == null ? "0" : res.Count.ToString();

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

        public string Codes
        {
			get { return _codes; }
            set
            {
				_codes = value;
                RaisePropertyChanged(() => Codes);
            }
        }

		public string CountObjects
		{
			get { return this._countObjects; }
			set
			{
				this._countObjects = value;
				RaisePropertyChanged(() => CountObjects);
			}
		}
		

		public string PrinterName2
		{
			get 
			{
				if (String.IsNullOrWhiteSpace(this._userSettingsManager.PrinterGet()) == false)
					return " - " +this._userSettingsManager.PrinterGet();
				else return " - " + Localization.Resources.View_IturAdd_Printer2NotSet;
			}
			//set
			//{
			//	_totalIturs = value;
			//	RaisePropertyChanged(() => TotalIturs);
			//}
		}

        public string Tag
        {
            get { return _tag; }
            set
            {
                this._tag = value;
                RaisePropertyChanged(()=>Tag);
				RaisePropertyChanged(() => PathSave);

				this.CountAndFillCodes();

                _okCommand.RaiseCanExecuteChanged();
            }
        }

		private void CountAndFillCodes()
		{
			List<string> res = new List<string>();
			if (this.SelectedObjectType == "Location")
			{
				if (_substring == false)
				{
					res = this._locationRepository.GetLocationCodeListByTag(base.GetDbPath, this._tag);
				}
				else
				{
					res = this._locationRepository.GetLocationCodeListIncludedTag(base.GetDbPath, this._tag);
				}
			}
			else if (this.SelectedObjectType == "Itur")
			{
				if (_substring == false)
				{
					res = this._iturRepository.GetIturCodeListByTag(base.GetDbPath, this._tag);
				}
				else
				{
					res = this._iturRepository.GetIturCodeListIncludedTag(base.GetDbPath, this._tag);
				}
			}
			else if (this.SelectedObjectType == "Section")
			{
				if (_substring == false)
				{
					res = this._sectionRepository.GetSectionCodeListByTag(base.GetDbPath, this._tag);
				}
				else
				{
					res = this._sectionRepository.GetSectionCodeListIncludedTag(base.GetDbPath, this._tag);
				}
			}
			//List<int> res = CommaDashStringParser.Parse(this._locationCode);//TODO количество Itur
			CountObjects = res == null ? "0" : res.Count.ToString();
			Codes = " :  " + res.JoinRecord(",");
		}

		public ObservableCollection<ReportPrintItemViewModel> Reports
		{
			get { return _reports; }
		}

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

		public DelegateCommand SendDataCommand
		{
			get { return _sendDataCommand; }
		}

		private bool SendDataCommandCanExecute()
		{
			return true;//IsOkZipPath();
		}

		private void SendDataCommandExecuted()
		{
			if (this.IsBusy == false)
			{
				this.IsBusy = true;

				Task.Factory.StartNew(BuildAndSaveReport, Thread.CurrentThread.CurrentUICulture).LogTaskFactoryExceptions("SendDataCommandExecuted");
			}
		}

		private void BuildAndSaveReport(object state)
		{
			CultureInfo culture = state as CultureInfo;
			if (culture != null)
			{
				Thread.CurrentThread.CurrentUICulture = culture;
			}

			List<ReportInfo> reportInfoList = new List<ReportInfo>();
			foreach (ReportPrintItemViewModel viewModel in this._reports)
			{
				if (viewModel.Include == true)
				{
					ReportInfo item = new ReportInfo(this._reportRepository);
					item.ReportCode = viewModel.ReportCode;
					item.Format = viewModel.FileFormat;
					if (item.Format == ReportFileFormat.Excel2007) item.Format = ReportFileFormat.EXCELOPENXML;
					item.IncludeInZip = viewModel.Include;
					item.Print = viewModel.Print;
					item.Print2 = viewModel.Print2;
					item.param1 = this.Tag;
					item.param2 = viewModel.SelectReportBy;
					item.param3 = this.Substring;
					reportInfoList.Add(item);
				}
			}


			// ======== SeclectParam

			//==================== Location ============= 
			if (this.SelectedObjectType == "Location")
			{
				List<string> searchLocationCode = new List<string>();
				if (_substring == false)
				{
					searchLocationCode = this._locationRepository.GetLocationCodeListByTag(base.GetDbPath, this.Tag);
				}
				else
				{
					searchLocationCode = this._locationRepository.GetLocationCodeListIncludedTag(base.GetDbPath, this.Tag);
				}
				SelectParams selectParams = new SelectParams();
				if (searchLocationCode.Count == 0) searchLocationCode.Add("");

				// Problem - Location.Code веде LocationCode
				selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
				{
					Values = searchLocationCode
				});


				reportInfoList = reportInfoList.Where(x => x != null).ToList();
				foreach (ReportInfo reportInfo in reportInfoList)
				{
					reportInfo.BuildReportArgs(base.State);
					if (reportInfo.ReportCode.StartsWith("[Rep-L") == false)
					{
						reportInfo.GenerateArgs.SelectParams = selectParams;
					}
					else	//"Code"
					{
						SelectParams selectParams1 = new SelectParams();
						selectParams1.FilterStringListParams.Add("Code", new FilterStringListParam()
						{
							Values = searchLocationCode
						});
						reportInfo.GenerateArgs.SelectParams = selectParams1;
					}
				}
			}

				//==================== Itur ============= 
			else if (this.SelectedObjectType == "Itur")
			{
				List<string> searchIturCode = new List<string>();
				if (_substring == false)
				{
					searchIturCode = this._iturRepository.GetIturCodeListByTag(base.GetDbPath, this.Tag);
				}
				else
				{
					searchIturCode = this._iturRepository.GetIturCodeListIncludedTag(base.GetDbPath, this.Tag);
				}
				SelectParams selectParams = new SelectParams();
				if (searchIturCode.Count == 0) searchIturCode.Add("");

				// Problem - Location.Code веде LocationCode
				selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam()
				{
					Values = searchIturCode
				});


				reportInfoList = reportInfoList.Where(x => x != null).ToList();
				foreach (ReportInfo reportInfo in reportInfoList)
				{
					reportInfo.BuildReportArgs(base.State);
					reportInfo.GenerateArgs.SelectParams = selectParams;
				}
			}


			//==================== Section ============= 
			else if (this.SelectedObjectType == "Section")
			{
				List<string> searchSectionCode = new List<string>();
				if (_substring == false)
				{
					searchSectionCode = this._sectionRepository.GetSectionCodeListByTag(base.GetDbPath, this.Tag);
				}
				else
				{
					searchSectionCode = this._sectionRepository.GetSectionCodeListIncludedTag(base.GetDbPath, this.Tag);
				}
				SelectParams selectParams = new SelectParams();
				if (searchSectionCode.Count == 0) searchSectionCode.Add("");


				selectParams.FilterStringListParams.Add("SectionCode", new FilterStringListParam()
				{
					Values = searchSectionCode
				});


				reportInfoList = reportInfoList.Where(x => x != null).ToList();
				foreach (ReportInfo reportInfo in reportInfoList)
				{
					reportInfo.BuildReportArgs(base.State);
					reportInfo.GenerateArgs.SelectParams = selectParams;
				}
			}

			//build reports files
			List<string> reportFiles = new List<string>();

			{
				foreach (ReportInfo reportInfo in reportInfoList)
				{
					List<ReportInfo> tempReportInfoList = new List<ReportInfo>();
					tempReportInfoList.Add(reportInfo);

					List<string> retReportFiles = _sendDataOffice.BuildAndSaveReports(
					cbiState: base.State,
					updateStatus: UpdateStatus,
					reportInfs: tempReportInfoList,
					fromContext: "TagSelect");

					foreach (string retReportFile in retReportFiles)
					{
						reportFiles.Add(retReportFile);
					}
				}
			}

			if (Directory.Exists(this.PathSave) == false)
			{
				Directory.CreateDirectory(this.PathSave);
			}

			foreach (string reportFile in reportFiles)
			{
				try
				{
					string fileName = Path.GetFileName(reportFile);
					string distinctFilePath = Path.Combine(this.PathSave, fileName);
					if (File.Exists(distinctFilePath) == true)
					{
						File.Delete(distinctFilePath);
					}
					File.Copy(reportFile, distinctFilePath);
				}
				catch (Exception ep)
				{
					_logger.Error("BuildAndSaveReport", ep.Message);
				}
			}

			//public List<string> BuildAndSaveReports(CBIState cbiState, Action<string> updateStatus, IEnumerable<ReportInfo> reportInfs)


			Utils.RunOnUI(() =>
			{
				IsBusy = false;

				if (Directory.Exists(this.PathSave) == true)
				{
					Utils.OpenFileInExplorer(this.PathSave);
				}
			});
		}

		public string BusyContent
		{
			get { return _busyContent; }
			set
			{
				_busyContent = value;
				RaisePropertyChanged(() => BusyContent);
			}
		}

		private void UpdateStatus(string status)
		{
			Utils.RunOnUI(() => BusyContent = status);
		}

        public object ResultData { get; set; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			//Inventor inventor = base.CurrentInventor;
			//this._inventorDate = inventor.InventorDate;

			this.SetDefaultPath();

			this._objectTypes = new List<string> { "Location", "Itur", "Section" };
			this._selectedObjectType = this.ObjectTypes.FirstOrDefault();

			//this.CopyReportTemplateIniFile();
			this._reportIniFile = this._reportIniRepository.CopyPrintReportTemplateIniFile(base.CurrentInventor.Code);
			this.BuildReports();
			_logger.Info("TagSelect (print Report by Tag) opened");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
			_logger.Info("TagSelect (print Report by Tag) closing");
            base.OnNavigatedFrom(navigationContext);
			
	    }       

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
					//case "TextValue":
					//	{
					//		if (this.IsTextValid() == false)
					//		{
					//			return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
					//		}
					//	}
					//	break;
					case "PathSave":
						return String.Empty;
					//if (!IsOkZipPath())
					//	return Localization.Resources.ViewModel_InventorChangeStatus_InvalidPath;
					case "txtTag":
                        {
							if (String.IsNullOrWhiteSpace(_tag))
                                return String.Empty;

							//int bit = this._locationCode.PrefixValidate();
							//if (bit != 0)
							//{
							//	return IturValidate.Bit2PrefixErrorMessage(bit);
							//}
                        }
                        //if (IsPrefixValid() == false)
                        //{
                        //    return "Prefix is not empty numeric string 4 characters maximum";
                        //}

                        break;
                }

                return string.Empty;
            }
        }

        public string Error { get; private set; }       

		//private bool IsTextValid()
		//{
		//	return CommaDashStringParser.IsValid(this._textValue);
		//}

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            if (!OkCommand.CanExecute())
                return;

            try
            {
                using (new CursorWait())
                {
					//List<string> iturCodes = this._iturRepository.GetIturCodesForLocationCode(this._locationCode, base.GetDbPath);
					//List<int> viewCodes = CommaDashStringParser.Parse(this._textValue.Trim()).ToList();
					//List<string> dbCodes = _iturRepository.GetIturCodes(base.GetDbPath).ToList();

					//List<string> iturCodes = new List<string>();

					//foreach (int viewCode in viewCodes)
					//{
					//	string sufix = UtilsItur.SuffixFromNumber(viewCode);
					//	string prefix = UtilsItur.PrefixFromString(_locationCode);
					//	string code = UtilsItur.CodeFromPrefixAndSuffix(prefix, sufix);

					//	if (dbCodes.Contains(code))
					//	{
					//		iturCodes.Add(code);
					//	}
					//}


					//=======
					List<ReportInfo> reportInfoList = new List<ReportInfo>();

					foreach (ReportPrintItemViewModel viewModel in this._reports)
					{
						ReportInfo item = new ReportInfo(this._reportRepository);
						item.ReportCode = viewModel.ReportCode;
						item.Print = viewModel.Print;
						item.Print2 = viewModel.Print2;
						item.param1 = this.Tag;
						item.param2 = viewModel.SelectReportBy;
						item.param3 = this.Substring;

						item.Format = viewModel.FileFormat;
						if (item.Format == ReportFileFormat.Excel2007) item.Format = ReportFileFormat.EXCELOPENXML;
						item.IncludeInZip = viewModel.Include;

						reportInfoList.Add(item);
					}
					//========
					ResultData = reportInfoList;
                    //ResultData = iturCodes;
                }

                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

				//Utils.RunOnUI(() =>
				//{
				//	IsBusy = false;

				//	if (File.Exists(_path))
				//	{
				//		Utils.OpenFileInExplorer(_path);
				//	}
				//});
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }
        }

        private bool OkCommandCanExecuted()
        {
			return (String.IsNullOrEmpty(this._tag) == false);
             
                
        }


		//-------
		private void BuildReports()
		{
			this._reports.Clear();

			if (File.Exists(this._reportIniFile) == false)
			{
				//this.CopyReportTemplateIniFile();
				this._reportIniFile = this._reportIniRepository.CopyPrintReportTemplateIniFile(base.CurrentInventor.Code);
			}
			if (File.Exists(this._reportIniFile) == false) return;


			string Context = ReportIniProperty.ContextPrintReportByTagForLocation;
			string selectReportBy = SelectReportByType.LocationTag;
			if (SelectedObjectType == "Location")
			{
				Context = ReportIniProperty.ContextPrintReportByTagForLocation;
				selectReportBy = SelectReportByType.LocationTag;
			}
			else if (SelectedObjectType == "Itur")
			{
				Context = ReportIniProperty.ContextPrintReportByTagForItur;
				selectReportBy = SelectReportByType.IturTag;
			}
			else if (SelectedObjectType == "Section")
			{
				Context = ReportIniProperty.ContextPrintReportByTagForSection;
				selectReportBy = SelectReportByType.SectionTag;
			}

			//const string IncludeInReportByLocation = "IncludeInReportByLocation";
			const string PrintKey = ReportIniProperty.Print;
			const string SecondPrintKey = ReportIniProperty.SecondPrint;
			const string SelectReportByKey = ReportIniProperty.SelectReportBy;
			const string FileFormatKey = ReportIniProperty.FileType;
			const string IncludeInZipKey = ReportIniProperty.IncludeInZip;
			

			foreach (IniFileData iniFileData in this._iniFileParser.Get(this._reportIniFile))
			{
				string reportCode = iniFileData.SectionName;

				if (String.IsNullOrWhiteSpace(reportCode) == true) continue;

				bool isPrintReportForTabContext = false;
				if (iniFileData.Data.ContainsKey(Context))
				{
					isPrintReportForTabContext = iniFileData.Data[Context] == "1";
				}
				if (isPrintReportForTabContext == false) continue;

			
				//Здесь ориентируемся на контекст!!!
				//так как добавилась еще одна мерность, ее можно взять из контекста
				// не используем из ini файла, а заполняем от контекста!!!	  Context
				if (iniFileData.Data.ContainsKey(SelectReportByKey))
				{
					//selectReportBy = iniFileData.Data[SelectReportByKey];
				}
			

				bool isPrint = false;
				if (iniFileData.Data.ContainsKey(PrintKey))
				{
					isPrint = iniFileData.Data[PrintKey] == "1";
				}

				bool isSecondPrint = false;
				if (iniFileData.Data.ContainsKey(SecondPrintKey))
				{
					isSecondPrint = iniFileData.Data[SecondPrintKey] == "1";
				}

				bool isInclude = false;
				if (iniFileData.Data.ContainsKey(IncludeInZipKey))
				{
					isInclude = iniFileData.Data[IncludeInZipKey] == "1";
				}

				string fileTypeList = String.Empty;
				if (iniFileData.Data.ContainsKey(FileFormatKey))
				{
					fileTypeList = iniFileData.Data[FileFormatKey];
				}

				if (String.IsNullOrWhiteSpace(fileTypeList) == true) fileTypeList = "Excel";

				foreach (string fileType in fileTypeList.Split(','))
				{
					string reportCodeBracket = String.Format("[{0}]", reportCode);
					Count4U.GenerationReport.Reports reports = this._reportRepository.GetReportByCodeReport(reportCodeBracket);
					Count4U.GenerationReport.Report report = null;
					if (reports != null)
					{
						report = reports.FirstOrDefault();
					}

					if (report == null)
					{
						_logger.Warn("BuildReports: Report is missing{0}", reportCode);
						continue;
					}

					ReportPrintItemViewModel viewModel = new ReportPrintItemViewModel();
					viewModel.Print = isPrint;
					viewModel.Print2 = isSecondPrint;
					viewModel.ReportCode = reportCodeBracket;
					viewModel.ReportName = this._generateReportRepository.GetLocalizedReportName(report);
					viewModel.Landscape = report.Landscape;
					viewModel.SelectReportBy = selectReportBy;


					viewModel.FileFormat = FromStringToReportFileFormat(fileType);
					viewModel.Include = isInclude;


					this._reports.Add(viewModel);
				}

			}

			//----------- работает
					/*	List<string> reportCodes = new List<string>();
						reportCodes.Add("[Rep-IT2-03]");
						reportCodes.Add("[Rep-IS1-77L]");
						foreach (string reportCode in reportCodes)
						{
							ReportPrintItemViewModel viewModel = new ReportPrintItemViewModel();
							
							if (reportCode == "[Rep-IS1-77L]")
							{
								viewModel.Print2 = true;
								viewModel.Print = false;
							}
							else{
								viewModel.Print2 = false;
								viewModel.Print = true;
							}
							viewModel.ReportCode = reportCode;
							this._reports.Add(viewModel);
						}*/


		}

		//private void CopyReportTemplateIniFile()
		//{
		//	try
		//	{
		//		string source = UtilsPath.ExportReportTemplateIniFile(this._dbSettings);

		//		if (File.Exists(source) == false) return;

		//		string inventorCode = base.CurrentInventor.Code;
		//		if (String.IsNullOrWhiteSpace(inventorCode)) return;

		//		this._reportIniFile = Common.Helpers.UtilsPath.ExportReportIniFile(this._dbSettings, inventorCode);

		//		if (File.Exists(this._reportIniFile) == true) return;

		//		File.Copy(source, this._reportIniFile);
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.Info("CopyTemplateIniFile", exc);
		//	}
		//}

		public DelegateCommand OpenConfigCommand
		{
			get { return _openConfigCommand; }
		}

		public DelegateCommand ReloadConfigCommand
		{
			get { return _reloadConfigCommand; }
		}

		private bool OpenConfigCommandCanExecute()
		{
			//return File.Exists(this._reportIniFile);
			return true;
		}

		private void OpenConfigCommandExecuted()
		{
			Utils.OpenFileInExplorer(this._reportIniFile);
		}

		private bool ReloadConfigCommandCanExecute()
		{
			//return File.Exists(this._reportIniFile);
			return true;
		}

		private void ReloadConfigCommandExecuted()
		{
			//this.CopyReportTemplateIniFile();
			this._reportIniFile = this._reportIniRepository.CopyPrintReportTemplateIniFile(base.CurrentInventor.Code);
			this.BuildReports();
		}

		private string FromStringToReportFileFormat(string input)
		{
			input = input.Trim().ToLower();

			switch (input)
			{
				case "pdf":
					return ReportFileFormat.Pdf;
				case "excel":
					return ReportFileFormat.Excel;
				case "word":
					return ReportFileFormat.Word;
				case "excel2007":
					return ReportFileFormat.Excel2007;
			}

			return input;
		}

		public void ClearIturAnalysis(object param)
		{
			string dbPath = base.GetDbPath;
			if (String.IsNullOrWhiteSpace(dbPath) == false)
			{
				//this._iturAnalyzesSourceRepository.ClearIturAnalyzes(dbPath);
				this._iturAnalyzesSourceRepository.AlterTableIturAnalyzes(dbPath);
			}
		}

    }
}