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
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Extensions;
using Count4U.Model.SelectionParams;

namespace Count4U.Modules.Audit.ViewModels
{
    public class LocationCodeSelectViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;
		 private readonly IReportRepository _reportRepository;
		 private readonly IGenerateReportRepository _generateReportRepository;
		 private readonly IReportIniRepository _reportIniRepository;
		 private readonly IServiceLocator _serviceLocator;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
		private readonly DelegateCommand _openConfigCommand;
		private readonly DelegateCommand _reloadConfigCommand;
		private readonly DelegateCommand _sendDataCommand;

		private readonly ObservableCollection<ReportPrintItemViewModel> _reports;
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;

        private string _textValue;
        private string _countItems;
		private string _totalIturs;
		private string _reportIniFile;

        private string _locationCode;

		private readonly IDBSettings _dbSettings;
		private readonly IIniFileParser _iniFileParser;
		private readonly SendDataOffice _sendDataOffice;

		private string _path;
		private bool _isBusy;
		private string _busyContent;


		public LocationCodeSelectViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IIturRepository iturRepository,
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
			this._reportRepository = reportRepository;
			this._generateReportRepository = generateReportRepository;
			this._reportIniRepository = reportIniRepository;
			this._eventAggregator = eventAggregator;
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._sendDataOffice = sendDataOffice;
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._reports = new ObservableCollection<ReportPrintItemViewModel>();
			this._openConfigCommand = new DelegateCommand(OpenConfigCommandExecuted, OpenConfigCommandCanExecute);
			this._reloadConfigCommand = new DelegateCommand(ReloadConfigCommandExecuted, ReloadConfigCommandCanExecute);
			this._sendDataCommand = new DelegateCommand(SendDataCommandExecuted, SendDataCommandCanExecute);

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

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				RaisePropertyChanged(() => IsBusy);
			}
		}

		public string ZipPath
		{
			get { return _path; }
			set
			{
				_path = value;
				RaisePropertyChanged(() => ZipPath);

				//_sendDataCommand.RaiseCanExecuteChanged();
			}
		}

		private bool IsOkZipPath()
		{
			if (String.IsNullOrWhiteSpace(_path))
				return false;

			try
			{
				FileInfo fi = new FileInfo(_path);
				if (!fi.Directory.Exists)
					return false;

				if (String.IsNullOrWhiteSpace(fi.Name))
					return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void SetDefaultPath()
		{
			string sendDataFolderPath = UtilsPath.ZipOfficeFolder(_dbSettings);

			string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
			string reportFolder = String.Format("{0}_{1}_{2}",
													   base.CurrentCustomer.Code,
													   base.CurrentBranch.BranchCodeERP,
													   date);

			string saveReportFolderPath = Path.Combine(sendDataFolderPath, reportFolder);

			this._path = saveReportFolderPath;
		}

        public string TotalIturs
        {
            get { return _totalIturs; }
            set
            {
                _totalIturs = value;
                RaisePropertyChanged(() => TotalIturs);
            }
        }

		public string CountIturs
        {
			get { return this._countItems; }
            set
            {
				this._countItems = value;
				RaisePropertyChanged(() => CountIturs);
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

        public string LocationCode
        {
            get { return _locationCode; }
            set
            {
                this._locationCode = value;
                RaisePropertyChanged(()=>LocationCode);

				List<string> res = new List<string>();

				if (string.IsNullOrEmpty(this._locationCode) == false)
				{
					string[] locationCodes = this._locationCode.Split(',');
					//foreach (var code in locationCodes)
					//{
						//res = this._iturRepository.GetIturCodesForLocationCode(this._locationCode, base.GetDbPath);
						res = this._iturRepository.GetIturCodesForLocationCodes(locationCodes, base.GetDbPath);
						
					//}
				}
				
				//List<int> res = CommaDashStringParser.Parse(this._locationCode);//TODO количество Itur
				CountIturs  = res == null ? "0" : res.Count.ToString();
				string ret	= " :  " ;
				if (res.Count > 999) ret = "!!! Too many Iturs. Make request less !!!  ";
				TotalIturs = ret + res.JoinRecord(",");

                _okCommand.RaiseCanExecuteChanged();
            }
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
			return IsOkZipPath();
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
					reportInfoList.Add(item);
				}
			}


			// ======== SeclectParam

			SelectParams selectParams = new SelectParams();
			List<string> searchLocationCode = new List<string>();

			if (string.IsNullOrEmpty(this._locationCode) == false)
			{
				string[] locationCodes = this._locationCode.Split(',');
				foreach (var code in locationCodes)
				{
					searchLocationCode.Add(code);
				}
			}
			else
			{
				searchLocationCode.Add("");
			}

			//searchLocationCode = this._iturRepository.GetIturCodesForLocationCode(this._locationCode, base.GetDbPath);
			
			//if (searchLocationCode.Count == 0)
			//{		 
			//	searchLocationCode.Add("");
			//}
 
			selectParams.FilterStringListParams.Add("LocationCode", new FilterStringListParam()
			//selectParams.FilterStringListParams.Add("IturCode", new FilterStringListParam()
			{
				Values = searchLocationCode
			});

			reportInfoList = reportInfoList.Where(x => x != null).ToList();
			foreach (ReportInfo reportInfo in reportInfoList)
			{
				reportInfo.BuildReportArgs(base.State);
				reportInfo.GenerateArgs.SelectParams = selectParams;
				reportInfo.GenerateArgs.Param1 = this._locationCode;
			}
			//build reports files
			List<string> reportFiles = _sendDataOffice.BuildAndSaveReports(
			cbiState: base.State,
			updateStatus: UpdateStatus,
			reportInfs: reportInfoList, 
			fromContext: "LocationCodeSelect");


			//public List<string> BuildAndSaveReports(CBIState cbiState, Action<string> updateStatus, IEnumerable<ReportInfo> reportInfs)

			if (Directory.Exists(this._path) == false)
			{
				Directory.CreateDirectory(this._path);
			}

			foreach (string reportFile in reportFiles)
			{
				try
				{
					string fileName = Path.GetFileName(reportFile);
					string distinctFilePath = Path.Combine(this._path, fileName);
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

			Utils.RunOnUI(() =>
			{
				IsBusy = false;

				if (Directory.Exists(this._path) == true)
				{
					Utils.OpenFileInExplorer(_path);
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
			//this.CopyReportTemplateIniFile();
			this._reportIniFile = this._reportIniRepository.CopyPrintReportTemplateIniFile(base.CurrentInventor.Code);
			this.SetDefaultPath();
			this.BuildReports();
			_logger.Info("LocationCodeSelect (print Report for Location) opened");
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
			_logger.Info("LocationCodeSelect (print Report for Location) closing");
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
					case "ZipPath":

						if (!IsOkZipPath())
							return Localization.Resources.ViewModel_InventorChangeStatus_InvalidPath;

						break;

					case "txtLocationCode":
                        {
                            if (String.IsNullOrWhiteSpace(_locationCode))
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
						item.Format = viewModel.FileFormat;
						if (item.Format == ReportFileFormat.Excel2007) item.Format = ReportFileFormat.EXCELOPENXML;
						item.IncludeInZip = viewModel.Include;
						item.param1 = this.LocationCode;
						item.param2 = viewModel.SelectReportBy;
						

						reportInfoList.Add(item);
					}
					//========
					ResultData = reportInfoList;
                    //ResultData = iturCodes;
                }

                this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

				Utils.RunOnUI(() =>
				{
					IsBusy = false;

					if (File.Exists(_path))
					{
						Utils.OpenFileInExplorer(_path);
					}
				});
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }
        }

        private bool OkCommandCanExecuted()
        {
            return	(String.IsNullOrEmpty(this._locationCode) == false);
             
                
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


			const string Context = ReportIniProperty.ContextPrintReportForLocation;
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

				bool isPrintReportForLocation = false;
				if (iniFileData.Data.ContainsKey(Context))
				{
					isPrintReportForLocation = iniFileData.Data[Context] == "1";
				}
				if (isPrintReportForLocation == false) continue;

				string selectReportBy = "";
				if (iniFileData.Data.ContainsKey(SelectReportByKey))
				{
					selectReportBy = iniFileData.Data[SelectReportByKey];
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