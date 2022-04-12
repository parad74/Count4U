using System;
using System.IO;
using System.Text;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel;
using System.Linq;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Extensions;

namespace Count4U.Common.ViewModel.AdapterTemplate
{
    public abstract class TemplateAdapterOneFileViewModel : ImportModuleBaseViewModel, IDataErrorInfo
    {
        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private string _path;
        private string _pathFilter;
        private readonly DelegateCommand _browseCommand;
        private readonly DelegateCommand _openCommand;
        protected AdapterMaskViewModel _maskViewModel;
        private AdapterFileWatcher _pathWatcher;
		private bool _xlsxFormat = false;
		//private bool _xlsmFormat = false;
		

//        private string _pathWarning;

        protected TemplateAdapterOneFileViewModel(IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager
            ) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository,userSettingsManager)
        {
            this._browseCommand = new DelegateCommand(this.BrowseCommandExecuted);
            this._openCommand = new DelegateCommand(OpenCommandExecuted, OpenCommandCanExecute);
            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
            this.PathFilter = "*.csv|*.csv|All files (*.*)|*.*";
			this.XlsxFormat = false;
        }

        #region UI properties
		[NotInludeAttribute]
        public string Path
        {
            get { return this._path; }
            set
            {
				if (string.IsNullOrWhiteSpace(value) == false)
				{
					try
					{
						FileInfo fi = new FileInfo(value);
						if (fi.Extension == ".exe")
						{
							value = fi.Name.Replace(fi.Extension, String.Empty);
						}
					}
					catch { }
				}

				this._path = value;
                this.RaisePropertyChanged(() => this.Path);

                if (base.RaiseCanImport != null)
                    base.RaiseCanImport();

                this._openCommand.RaiseCanExecuteChanged();

                this.RaisePropertyChanged(() => this.Tooltip);
                this.OnPathChanged();
            }
        }

		[NotInludeAttribute]
        public string PathFilter
        {
            get { return this._pathFilter; }
            set { this._pathFilter = value; }
        }

		protected string _barcodeMask = "";				   //!! не убирать протектед
        public string BarcodeMask
        {
           // get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
			get 
			{   
			if ( this._maskViewModel != null)
				return this._maskViewModel.BarcodeMask; // Из GUI
			else
				return this._barcodeMask; 
			}
			set { _barcodeMask = value; }
        }

		protected string _makatMask = "";				// !! не убирать протектед
		public string MakatMask
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
			get
			{
				if (this._maskViewModel != null)
					return this._maskViewModel.MakatMask;	 // Из GUI
				else
					return this._makatMask;
			}
			set { _makatMask = value; }
		}

		[NotInludeAttribute]
		public bool XlsxFormat
		{
			get { return this._xlsxFormat; }
			set { this._xlsxFormat = value; }
		}
			 



        #endregion

        #region other properties
		[NotInludeAttribute]
        public string Tooltip
        {
            get { return BuildTooltip(this._path); }
        }

        public DelegateCommand BrowseCommand
        {
            get { return this._browseCommand; }
        }

		[NotInludeAttribute]
        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return this._fileChooseDilogRequest; }
        }

        public DelegateCommand OpenCommand
        {
            get { return _openCommand; }
        }

        #endregion

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _pathWatcher = new AdapterFileWatcher(this, TypedReflection<TemplateAdapterOneFileViewModel>.GetPropertyInfo(r=>r.Path));

            if (base.InputFileFolderChanged != null)
                base.InputFileFolderChanged(false);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _pathWatcher.Clear();
        }


        #region methods

        private void BrowseCommandExecuted()
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();
            //notification.Filter = "*.csv|*.csv|*.dat|*.dat|All files (*.*)|*.*";
            notification.Filter = this.PathFilter;

            this._fileChooseDilogRequest.Raise(notification, FileUploadProcess);
        }

        private void FileUploadProcess(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.Path = notification.FileName;
        }

		bool IsOkPath()
        {
            if (String.IsNullOrEmpty(this._path) == true) return true;
            else return File.Exists(this._path);
        }

        public override bool CanImport()
        {
            return (String.IsNullOrWhiteSpace(this._path) == false)
                && this.IsOkPath() == true;
        }

        #endregion

        #region IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Path":
                        {
                            if (this.IsOkPath() == false)
                                return Localization.Resources.Validation_FileNotExist;
                            break;
                        }
                }
                return String.Empty;
            }
        }

		[NotInludeAttribute]
        public string Error
        {
            get { throw new System.NotImplementedException(); }
        }

//        public string PathWarning
//        {
//            get { return _pathWarning; }
//            set
//            {
//                _pathWarning = value;
//                RaisePropertyChanged(() => PathWarning);
//            }
//        }

        #endregion

        private bool OpenCommandCanExecute()
        {
            return base.IsPathOkForOpenAsFolder(_path);
        }

        private void OpenCommandExecuted()
        {
            base.OpenPathAsFolder(_path);
        }

        protected override void EncondingUpdated()
        {
            RaisePropertyChanged(() => Tooltip);
        }

        protected virtual void OnPathChanged()
        {

        }
    }
}