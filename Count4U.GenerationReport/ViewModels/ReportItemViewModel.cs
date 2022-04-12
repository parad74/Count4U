using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Report.ViewModels
{
    public class ReportItemViewModel : NotificationObject
    {
        private GenerationReport.Report _report;

        private string _reportCode;
        private string _code;
        private string _path;
		public string _fileName {get; set;}
        private string _domainContext;
        private string _description;
        private bool _menu;
        private string _menuCaption;
        private string _tag;
		private bool _allowedContextSelectParm;

        private bool _isReportExistInFs;

        public ReportItemViewModel(GenerationReport.Report report)
        {        
            Update(report);
            
        }

        public Count4U.GenerationReport.Report Report
        {
            get { return _report; }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(()=>Code);
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }


        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => FileName);
            }
        }

        public string DomainContext
        {
            get { return _domainContext; }
            set
            {
                _domainContext = value;
                RaisePropertyChanged(() => DomainContext);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

		public bool AllowedContextSelectParm
		{
			get { return _allowedContextSelectParm; }
			set
			{
			    _allowedContextSelectParm = value;
                RaisePropertyChanged(() => AllowedContextSelectParm);
			}
		}

        public bool Menu
        {
            get { return _menu; }
            set
            {
                _menu = value;
                RaisePropertyChanged(() => Menu);
            }
        }

        public string MenuCaption
        {
            get { return _menuCaption; }
            set
            {
                _menuCaption = value;
                this._report.MenuCaption = this._menuCaption;
                RaisePropertyChanged(() => MenuCaption);
            }
        }

        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                this._report.Tag = this._tag;
                RaisePropertyChanged(() => Tag);
            }
        }

        public string ReportCode
        {
            get { return _reportCode; }
            set
            {
                _reportCode = value;
                RaisePropertyChanged(() => ReportCode);
            }
        }

        public bool IsReportExistInFs
        {
            get { return _isReportExistInFs; }
            set
            {
                _isReportExistInFs = value;
                RaisePropertyChanged(() => IsReportExistInFs);
            }
        }

        public int Index { get; set; }

      
        public void Update(GenerationReport.Report report)
        {
            this._report = report;

            this.ReportCode = report.CodeReport;
            this.Code = report.Code;
            this.Path = report.Path;
            this.FileName = report.FileName;
            this.DomainContext = report.DomainContext;
            this.Description = report.Description;
            this.Menu = report.Menu;
            this.MenuCaption = report.MenuCaption;
            this.Tag = report.Tag;
			this.AllowedContextSelectParm = report.AllowedContextSelectParm;
        }
    }
}