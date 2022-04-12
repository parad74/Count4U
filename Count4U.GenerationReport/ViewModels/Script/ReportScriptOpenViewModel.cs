using System.Text;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Script;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Report.ViewModels.Script
{
	public class ReportScriptOpenViewModel : ScriptOpenBaseViewModel
    {
		//private readonly IServiceLocator _serviceLocator;

        protected bool _isMain;
        protected bool _isCurrentUser;
		protected bool _toSetupDB;
		

        public ReportScriptOpenViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
			: base(contextCbiRepository, eventAggregator, serviceLocator)
        {
			//this._serviceLocator = serviceLocator;
        }

        public bool IsMain
        {
			get { return this._isMain; }
            set
            {
				this._isMain = value;
				RaisePropertyChanged(() => this.IsMain);

                if (_isMain)
                {
					this._isCurrentUser = false;
					RaisePropertyChanged(() => this.IsCurrentUser);
                }
            }
        }


		public bool IsCurrentUser
        {
			get { return this._isCurrentUser; }
            set
            {
				this._isCurrentUser = value;
				RaisePropertyChanged(() => this.IsMain);

				if (this._isCurrentUser)
                {
					this._isMain = false;
					RaisePropertyChanged(() => this.IsMain);
                }
            }
        }

		public bool ToSetupDB
		{
			get { return this._toSetupDB; }
			set
			{
				this._toSetupDB = value;
				RaisePropertyChanged(() => this.ToSetupDB);
			}
		}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            AllowedReportTemplate mode = UtilsConvert.ConvertToEnum<AllowedReportTemplate>(navigationContext);

            switch (mode)
            {
                case AllowedReportTemplate.Main:
                    this._isMain = true;
                    break;
                case AllowedReportTemplate.Audit:
                    this._isCurrentUser = true;
                    break;
            }
			this._toSetupDB = false;
        }

        protected override void RunScript()
        {
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
			IScriptReportRepository scriptReportRepository = this._serviceLocator.GetInstance<IScriptReportRepository>();
            log.Clear();

			bool isMain = this._isMain;
			bool isClear = this._isClear;
			bool isClearTag = this._isClearTag;
			bool toSetupDB = this._toSetupDB;
			string path = this._path;
			Encoding encoding = Encoding.GetEncoding("windows-1255");

			scriptReportRepository.RunReportScriptFromFile(isMain, isClear, isClearTag, toSetupDB, path, encoding);

            this.Log = log.PrintLog();
        }

		//private void RunReportScriptFromFile(bool isMain, bool isClear, bool toSetupDB, string path, Encoding encoding)
		//{
		//    IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.CsvFileParser.ToString());
		//    string sql = "";
		//    IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
		//    foreach (String record in fileParser.GetRecords(path, encoding, 0))
		//    {
		//        if (record.ToUpper().Contains("DROP") == false
		//            && record.ToUpper().Contains("DELETE") == false
		//            && record.ToUpper().Contains("UPDATE") == false
		//            && record.ToUpper().Contains("ALTER") == false
		//            && record.ToUpper().Contains("CREATE") == false
		//            && record.ToUpper().Contains("SELECT") == false)
		//        {
		//            if (record.ToUpper().Contains("INSERT") == true)
		//            {
		//                string reportMainTable = @"[Report]";
		//                string reportAuditTable = @"[AuditReport]";
		//                if (isMain == true)
		//                {
		//                    if (record.Contains(reportMainTable) == true)
		//                    {
		//                        sql = sql + record + Environment.NewLine;
		//                    }
		//                    else if (record.Contains(reportAuditTable) == true)
		//                    {
		//                        string record1 = record.Replace(reportAuditTable, reportMainTable);
		//                        sql = sql + record1 + Environment.NewLine;
		//                    }
		//                }
		//                else	  //Audit
		//                {
		//                    if (record.Contains(reportAuditTable) == true)
		//                    {
		//                        sql = sql + record + Environment.NewLine;
		//                    }
		//                    else if (record.Contains(reportMainTable) == true)
		//                    {
		//                        string record1 = record.Replace(reportMainTable, reportAuditTable);
		//                        sql = sql + record1 + Environment.NewLine;
		//                    }
		//                }
		//            }
		//        }
		//    }  //foreach record

		//    if (isMain == true)
		//    {
		//        alterADOProvider.ImportMainReport(sql, isClear, toSetupDB);
		//    }
		//    else
		//    {
		//        alterADOProvider.ImportAuditReport(sql, isClear, toSetupDB);
		//    }
		//}


    }
}