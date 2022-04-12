using System;
using System.ComponentModel;
using System.IO;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Extensions;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4Mobile;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Common.ViewModel.AdapterTemplate
{
    public abstract class TemplateAdapterThreeFilesViewModel : ImportModuleBaseViewModel, IDataErrorInfo
    {
        private readonly InteractionRequest<OpenFileDialogNotification> _fileChooseDilogRequest;
        private string _path1;
        private string _path2;
        private string _path3;
        private readonly DelegateCommand _browseCommand1;
        private readonly DelegateCommand _browseCommand2;
        private readonly DelegateCommand _browseCommand3;
        private readonly DelegateCommand _openCommand1;
        private readonly DelegateCommand _openCommand2;
        private readonly DelegateCommand _openCommand3;
        private string _pathFilter1;
        private string _pathFilter2;
        private string _pathFilter3;
        protected AdapterMaskViewModel _maskViewModel1;
        protected AdapterMaskViewModel _maskViewModel2;
        protected AdapterMaskViewModel _maskViewModel3;

        protected TemplateAdapterThreeFilesViewModel(
            IServiceLocator serviceLocator,
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager,
            ILog logImport,
            IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
            IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
            this._browseCommand1 = new DelegateCommand(this.BrowseCommand1Executed);
            this._browseCommand2 = new DelegateCommand(this.BrowseCommand2Executed);
            this._browseCommand3 = new DelegateCommand(this.BrowseCommand3Executed);
            this._fileChooseDilogRequest = new InteractionRequest<OpenFileDialogNotification>();
            this._openCommand1 = new DelegateCommand(OpenCommand1Executed, OpenCommand1CanExecute);
            this._openCommand2 = new DelegateCommand(OpenCommand2Executed, OpenCommand2CanExecute);
            this._openCommand3 = new DelegateCommand(OpenCommand3Executed, OpenCommand3CanExecute);
        }

        #region UI properties

		[NotInludeAttribute]
        public string Path1
        {
            get { return this._path1; }
            set
            {
                this._path1 = value;
                this.RaisePropertyChanged(() => this.Path1);

                if (base.RaiseCanImport != null)
                    base.RaiseCanImport();

                this.RaisePropertyChanged(() => this.Tooltip1);

                this._openCommand1.RaiseCanExecuteChanged();
            }
        }

		[NotInludeAttribute]
        public string Path2
        {
            get { return this._path2; }
            set
            {
                this._path2 = value;
                this.RaisePropertyChanged(() => this.Path2);

                if (base.RaiseCanImport != null)
                    base.RaiseCanImport();

                this.RaisePropertyChanged(() => this.Tooltip2);

                this._openCommand2.RaiseCanExecuteChanged();
            }
        }

		[NotInludeAttribute]
        public string Path3
        {
            get { return this._path3; }
            set
            {
                this._path3 = value;
                this.RaisePropertyChanged(() => this.Path3);

                if (base.RaiseCanImport != null)
                    base.RaiseCanImport();

                this.RaisePropertyChanged(() => this.Tooltip3);

                this._openCommand3.RaiseCanExecuteChanged();
            }
        }

		[NotInludeAttribute]
        public string PathFilter1
        {
            get { return this._pathFilter1; }
            set { this._pathFilter1 = value; }
        }

		[NotInludeAttribute]
        public string PathFilter2
        {
            get { return this._pathFilter2; }
            set { this._pathFilter2 = value; }
        }

		[NotInludeAttribute]
        public string PathFilter3
        {
            get { return this._pathFilter3; }
            set { this._pathFilter3 = value; }
        }

		////[NotInludeAttribute]
		////public string BarcodeMask1
		////{
		////	get { return this._maskViewModel1 == null ? String.Empty : this._maskViewModel1.BarcodeMask; }
		////}

		////[NotInludeAttribute]
		////public string MakatMask1
		////{
		////	get { return this._maskViewModel1 == null ? String.Empty : this._maskViewModel1.MakatMask; }
		////}

		////[NotInludeAttribute]
		////public string BarcodeMask2
		////{
		////	get { return this._maskViewModel2 == null ? String.Empty : this._maskViewModel2.BarcodeMask; }
		////}

		////[NotInludeAttribute]
		////public string MakatMask2
		////{
		////	get { return this._maskViewModel2 == null ? String.Empty : this._maskViewModel2.MakatMask; }
		////}

		////[NotInludeAttribute]
		////public string BarcodeMask3
		////{
		////	get { return this._maskViewModel3 == null ? String.Empty : this._maskViewModel3.BarcodeMask; }
		////}

		////[NotInludeAttribute]
		////public string MakatMask3
		////{
		////	get { return this._maskViewModel3 == null ? String.Empty : this._maskViewModel3.MakatMask; }
		////}

		//================ 1
		protected string _barcodeMask1 = "";				   //!! не убирать протектед
		public string BarcodeMask1
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
			get
			{
				if (this._maskViewModel1 != null)
					return this._maskViewModel1.BarcodeMask; // Из GUI
				else
					return this._barcodeMask1;
			}
			set { _barcodeMask1 = value; }
		}

		protected string _makatMask1 = "";				// !! не убирать протектед
		public string MakatMask1
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
			get
			{
				if (this._maskViewModel1 != null)
					return this._maskViewModel1.MakatMask;	 // Из GUI
				else
					return this._makatMask1;
			}
			set { _makatMask1 = value; }
		}


		//================ 2
		protected string _barcodeMask2 = "";				   //!! не убирать протектед
		public string BarcodeMask2
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
			get
			{
				if (this._maskViewModel2 != null)
					return this._maskViewModel2.BarcodeMask; // Из GUI
				else
					return this._barcodeMask2;
			}
			set { _barcodeMask2 = value; }
		}

		protected string _makatMask2 = "";				// !! не убирать протектед
		public string MakatMask2
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
			get
			{
				if (this._maskViewModel2 != null)
					return this._maskViewModel2.MakatMask;	 // Из GUI
				else
					return this._makatMask2;
			}
			set { _makatMask2 = value; }
		}

		//================ 3
		protected string _barcodeMask3 = "";				   //!! не убирать протектед
		public string BarcodeMask3
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.BarcodeMask; }
			get
			{
				if (this._maskViewModel3 != null)
					return this._maskViewModel3.BarcodeMask; // Из GUI
				else
					return this._barcodeMask3;
			}
			set { _barcodeMask3 = value; }
		}

		protected string _makatMask3 = "";				// !! не убирать протектед
		public string MakatMask3
		{
			// get { return this._maskViewModel == null ? String.Empty : this._maskViewModel.MakatMask; }
			get
			{
				if (this._maskViewModel3 != null)
					return this._maskViewModel3.MakatMask;	 // Из GUI
				else
					return this._makatMask3;
			}
			set { _makatMask3 = value; }
		}

        #endregion

        #region other properties
		[NotInludeAttribute]
        public string Tooltip1
        {
            get { return BuildTooltip(this._path1); }
        }

		[NotInludeAttribute]
        public string Tooltip2
        {
            get { return BuildTooltip(this._path2); }
        }

		[NotInludeAttribute]
        public string Tooltip3
        {
            get { return BuildTooltip(this._path3); }
        }

        public DelegateCommand BrowseCommand1
        {
            get { return this._browseCommand1; }
        }

        public DelegateCommand BrowseCommand2
        {
            get { return this._browseCommand2; }
        }

        public DelegateCommand BrowseCommand3
        {
            get { return this._browseCommand3; }
        }

		[NotInludeAttribute]
        public InteractionRequest<OpenFileDialogNotification> FileChooseDilogRequest
        {
            get { return this._fileChooseDilogRequest; }
        }


        public DelegateCommand OpenCommand1
        {
            get { return _openCommand1; }
        }

        public DelegateCommand OpenCommand2
        {
            get { return _openCommand2; }
        }

        public DelegateCommand OpenCommand3
        {
            get { return _openCommand3; }
        }

        #endregion

        #region methods

        public override bool CanImport()
        {
            return (String.IsNullOrWhiteSpace(this._path1) == false)
                   && (String.IsNullOrWhiteSpace(this._path2) == false)
                   && (String.IsNullOrWhiteSpace(this._path3) == false)
                   && this.IsOkPath(this._path1)
                   && this.IsOkPath(this._path2)
                   && this.IsOkPath(this._path3);
        }


        private bool IsOkPath(string path)
        {
            if (String.IsNullOrEmpty(path) == true) return true;
            else return File.Exists(path);
        }

        private void BrowseCommand1Executed()
        {
            this.Browse(this.FileUploadProcess1);
        }

        private void BrowseCommand2Executed()
        {
            this.Browse(this.FileUploadProcess2);
        }

        private void BrowseCommand3Executed()
        {
            this.Browse(this.FileUploadProcess3);
        }

        private void Browse(Action<OpenFileDialogNotification> action)
        {
            OpenFileDialogNotification notification = new OpenFileDialogNotification();

            this.FileChooseDilogRequest.Raise(notification, action);
        }

        private void FileUploadProcess1(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.Path1 = notification.FileName;
        }

        private void FileUploadProcess2(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.Path2 = notification.FileName;
        }

        private void FileUploadProcess3(OpenFileDialogNotification notification)
        {
            if (notification.IsOK == false) return;

            this.Path3 = notification.FileName;
        }

        private bool OpenCommand1CanExecute()
        {
            return base.IsPathOkForOpenAsFolder(_path1);
        }

        private void OpenCommand1Executed()
        {
            Open(this._path1);
        }

        private bool OpenCommand2CanExecute()
        {
            return base.IsPathOkForOpenAsFolder(_path2);
        }

        private void OpenCommand2Executed()
        {
            Open(this._path2);
        }

        private bool OpenCommand3CanExecute()
        {
            return base.IsPathOkForOpenAsFolder(_path3);
        }

        private void OpenCommand3Executed()
        {
            Open(this._path3);
        }

        private void Open(string path)
        {
            base.OpenPathAsFolder(path);
        }

        #endregion

        #region IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Path1":
                        {
                            if (this.IsOkPath(this._path1) == false) return Localization.Resources.Validation_FileNotExist;
                            break;
                        }
                    case "Path2":
                        {
                            if (this.IsOkPath(this._path2) == false) return Localization.Resources.Validation_FileNotExist;
                            break;
                        }

                    case "Path3":
                        {
                            if (this.IsOkPath(this._path3) == false) return Localization.Resources.Validation_FileNotExist;
                            break;
                        }
                }

                if ((String.IsNullOrWhiteSpace(this._path1) == false)
                    && (String.IsNullOrEmpty(this._path2) == false)                    
                    && (this._path1 == this._path2))
                    return Localization.Resources.Validation_Files_Cant_Equal;

                if ((String.IsNullOrWhiteSpace(this._path1) == false)
                    && (String.IsNullOrEmpty(this._path3) == false)
                    && (this._path1 == this._path3))
                    return Localization.Resources.Validation_Files_Cant_Equal;

                if ((String.IsNullOrWhiteSpace(this._path2) == false)
                    && (String.IsNullOrEmpty(this._path3) == false)
                    && (this._path2 == this._path3))
                    return Localization.Resources.Validation_Files_Cant_Equal;

                return String.Empty;
            }
        }

		[NotInludeAttribute]
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
        #endregion


        protected override void EncondingUpdated()
        {
            RaisePropertyChanged(() => Tooltip1);
            RaisePropertyChanged(() => Tooltip2);
            RaisePropertyChanged(() => Tooltip3);
        }
    }
}