using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Count4U.Model.Interface;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.GenerationReport;
using System.Collections.Generic;
using System.Text;

namespace Count4U.Report.ViewModels.Script
{
    public class ReportScriptSaveViewModel : ScriptSaveBaseViewModel
    {
        private readonly IServiceLocator _serviceLocator;

        protected bool _isMain;
        protected bool _isCurrentUser;
		protected bool _toSetupDB;

        public ReportScriptSaveViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
             IServiceLocator serviceLocator)
            : base(contextCbiRepository, eventAggregator)
        {
            this._serviceLocator = serviceLocator;
        }

        public bool IsMain
        {
            get { return _isMain; }
            set
            {
                _isMain = value;
                RaisePropertyChanged(() => IsMain);

                if (_isMain)
                {
                    _isCurrentUser = false;
                    RaisePropertyChanged(() => IsCurrentUser);
                }
            }
        }

        public bool IsCurrentUser
        {
            get { return _isCurrentUser; }
            set
            {
                _isCurrentUser = value;
                RaisePropertyChanged(() => IsMain);

                if (_isCurrentUser)
                {
                    _isMain = false;
                    RaisePropertyChanged(() => IsMain);
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
			IScriptReportRepository scriptReportRepository = this._serviceLocator.GetInstance<IScriptReportRepository>();
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
            log.Clear();

			bool isMain = this._isMain;
			//bool toSetupDB = this._toSetupDB;
			string path = this._path;
			Encoding encoding = Encoding.GetEncoding("windows-1255");

			scriptReportRepository.SaveReportScriptToFile(isMain, path, encoding);
           
            this.Log = log.PrintLog();
        }

		//private void SaveScriptReportLinkToFile(ILog log, bool isMain, string path, Encoding encoding)
		//{
		//    IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
		//    IReportRepository reportRepository = this._serviceLocator.GetInstance<IReportRepository>();
		//    List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
		//    Reports reports = null;
		//    string sql = @"INSERT INTO [Report] ";
		//    string retSql = "";
		//    if (isMain == true)
		//    {
		//        allowedReportTemplates.Add(AllowedReportTemplate.Main);
		//        reports = reportRepository.GetAllowedReportTemplate("", "", "",
		//        ViewDomainContextEnum.All, allowedReportTemplates);
		//    }
		//    else
		//    {
		//        allowedReportTemplates.Add(AllowedReportTemplate.Audit);
		//        reports = reportRepository.GetAllowedReportTemplate("", "", "",
		//        ViewDomainContextEnum.All, allowedReportTemplates);
		//        sql = @"INSERT INTO [AuditReport] ";
		//    }
		//    if (reports != null)
		//    {
		//        foreach (var report in reports)
		//        {
		//            if (string.IsNullOrWhiteSpace(report.FileName) == true) continue;
		//            string code = "Any";
		//            string reportMenu = report.Menu ? "1" : "0";
		//            string print = report.Print ? "1" : "0";
		//            string reportDomainContext = string.IsNullOrWhiteSpace(report.DomainContext) ? "NULL" : report.DomainContext.Replace("'", "''");
		//            string reportTypeDS = string.IsNullOrWhiteSpace(report.TypeDS) ? "NULL" : report.TypeDS.Replace("'", "''");
		//            string reportTag = string.IsNullOrWhiteSpace(report.Tag) ? "" : report.Tag.Replace("'", "''");
		//            string reportMenuCaption = string.IsNullOrWhiteSpace(report.MenuCaption) ? "" : report.MenuCaption.Replace("'", "''");
		//            string reportDescription = string.IsNullOrWhiteSpace(report.Description) ? "" : report.Description.Replace("'", "''");
		//            string reportPath = string.IsNullOrWhiteSpace(report.Path) ? "" : report.Path.Replace("'", "''");
		//            string reportDomainType = string.IsNullOrWhiteSpace(report.DomainType) ? "" : report.DomainType.Replace("'", "''");
		//            string nn = string.IsNullOrWhiteSpace(report.NN.ToString()) ? "" : report.NN.ToString();
		//            string menuCaptionLocalizationCode = string.IsNullOrWhiteSpace(report.MenuCaptionLocalizationCode) ? "" :
		//                report.MenuCaptionLocalizationCode.Replace("'", "''");
		//            try
		//            {
		//                string sql1 = sql +
		//                    @"([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[Print],[NN],[MenuCaptionLocalizationCode],[MenuCaption]) " +
		//                    @"VALUES " +
		//                    //	  {0}		{1}					{2}						 {3}		 {4}		 {5}			{6}				 {7}		{8}	  {9}
		//                    // ("(N'Any',N'Corporative Report',null,null,N'Iturs',N'ItursCorporativeReport.rdlc',N'Iturs',1,N'Corporative Report - Iturs', , , ,N'CorporativeReport')");
		//                    String.Format("(N'{0}',N'{1}',{2},{3},N'{4}',N'{5}','{6}','{7}',{8},{9},{10}, N'{11}',N'" + reportMenuCaption.Trim() + "');" + Environment.NewLine,
		//                    code.Trim(), reportDescription.Trim(), reportDomainContext.Trim(), reportTypeDS.Trim(), reportPath.Trim(), report.FileName.Trim(),
		//                    reportDomainType.Trim(), reportTag.Trim(), reportMenu.Trim(), print.Trim(), nn, menuCaptionLocalizationCode.Trim());
		//                retSql = retSql + sql1;
		//            }
		//            catch (Exception exp)
		//            {
		//                //this.Log = this.Log + exp.Message + " : " + report.FileName;
		//                log.Add(Model.MessageTypeEnum.Error, exp.Message + " : " + report.FileName);
		//            }
		//        }
		//        File.WriteAllText(path, retSql, encoding);
		//    }
		//}
    }
}

