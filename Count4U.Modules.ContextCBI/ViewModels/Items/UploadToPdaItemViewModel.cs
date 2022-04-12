using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Count4U.Model.Count4U;
using Count4U.Model.Lib.MultiPoint;
using Count4U.Model.Repository.Product.WrapperMulti;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using NLog;

namespace Count4U.Common.Web
{
    public enum UploadToPdaItemState
    {
        UploadOK,
        UploadError,
        DownloadOK,
        DownloadError
    }

    public class UploadToPdaItemViewModel : NotificationObject
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string OkButtonIcon = "check16";
        private const string AbortButtonIcon = "abort16";
        private const string ErrorButtonIcon = "delete16";

        private string _port;
        private string _device;
        private string _file;
        private string _number;
        private string _value;
        private int _progress;
        private bool _isChecked;
        private bool _isCheckedEnabled;

	    private readonly IWrapperMulti _wrapperMulti;
		public readonly IServiceLocator _serviceLocator;
		private readonly IWrapperMultiRepository _wrapperMultiRepository;

        private UploadToPdaItemState _state;

        private string _image;
        public string _buttonImage;
        private bool _buttonVisible;
        private bool _buttonEnabled;

        private readonly DelegateCommand _okCancelCommand;
        private bool _cancelled;
        private bool _isError;

        private bool _isShowProgressBar;

		public List<FileInfo> _uploadingFiles;
		public List<string> _uploadingFilesNames;
		public List<string> _uploadingFullPathFiles;
		public long _uploadingFilesLength;

		public List<ItemDir> _downloadFiles;
		public List<string> _downloadFilesNames;
		public long _downloadFilesLength;

		private string _folderOnPDA;
	
	

        public UploadToPdaItemViewModel(IWrapperMulti uploadWrapperMulti,
			IServiceLocator serviceLocator)
        {
            this._wrapperMulti = uploadWrapperMulti;
			this._serviceLocator = serviceLocator;
			this._wrapperMultiRepository = this._serviceLocator.GetInstance<IWrapperMultiRepository>(WrapperMultiEnum.WrapperMultiRepository.ToString());//TODO @@

            State = UploadToPdaItemState.UploadOK;
			this._buttonImage = AbortButtonIcon;
			this._buttonVisible = false;
			this._buttonEnabled = false;
			this._okCancelCommand = new DelegateCommand(OkCancelCommandExecuted);
			this._isCheckedEnabled = true;
			this._isShowProgressBar = false;

			this._uploadingFilesNames = new List<string>();
			this._uploadingFiles = new List<FileInfo>();
			this._uploadingFullPathFiles = new List<string>();

			this._downloadFilesNames = new List<string>();
			this._downloadFiles = new List<ItemDir>();

			this._folderOnPDA = "";
			this._uploadingFilesLength = 0;
			this._downloadFilesLength = 0;
        }

        public long DoneDuringSession { get; set; }

        public bool Cancelled
        {
			get { return this._cancelled; }
            set
            {
				this._cancelled = value;
				this.RaisePropertyChanged(() => this.Cancelled);
            }
        }

        public bool IsError
        {
			get { return this._isError; }
            set
            {
				this._isError = value;
				this.RaisePropertyChanged(() => this.IsError);
            }
        }

        public bool Done { get; set; }

        public string Port
        {
			get { return this._port; }
            set
            {
				this._port = value;
				this.RaisePropertyChanged(() => this.Port);
            }
        }

        public string Device
        {
			get { return this._device; }
            set
            {
				this._device = value;
				this.RaisePropertyChanged(() => this.Device);
				this.RaisePropertyChanged(() => this.IsChecked);
            }
        }

        public string File
        {
			get { return this._file; }
            set
            {
				this._file = value;
				this.RaisePropertyChanged(() => this.File);
            }
        }

        public string Number
        {
			get { return this._number; }
            set
            {
				this._number = value;
				this.RaisePropertyChanged(() => this.Number);
            }
        }

        public string Value
        {
			get { return this._value; }
            set
            {
				this._value = value;
				this.RaisePropertyChanged(() => this.Value);
            }
        }

        public int Progress
        {
			get { return this._progress; }
            set
            {
				this._progress = value;
				this.RaisePropertyChanged(() => this.Progress);
            }
        }

        public bool IsChecked
        {
			get { return this._isChecked; }
            set
            {
				if (this.Device == "not started")
				{
					this._isChecked = false;
				}
				else
				{
					this._isChecked = value;
				}
				this.RaisePropertyChanged(() => this.IsChecked);
            }
        }

        public UploadToPdaItemState State
        {
			get { return this._state; }
            set
            {
				this._state = value;

                switch (value)
                {
                    case UploadToPdaItemState.UploadOK:
						this._image = "uploadblue";
                        break;
                    case UploadToPdaItemState.UploadError:
						this._image = "uploadred";
                        break;
                    case UploadToPdaItemState.DownloadOK:
						this._image = "downloadblue";
                        break;
                    case UploadToPdaItemState.DownloadError:
						this._image = "downloadred";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }

                RaisePropertyChanged(() => Image);
            }
        }

        public BitmapImage Image
        {
            get
            {
				return Build(this._image);
            }
        }

        public BitmapImage ButtonImage
        {
            get
            {
				return Build(this._buttonImage);
            }
  
        }

        public DelegateCommand OkCancelCommand
        {
			get { return this._okCancelCommand; }
        }

        public IWrapperMulti wrapperMulti
        {
			get { return this._wrapperMulti; }
        }

        public bool ButtonVisible
        {
			get { return this._buttonVisible; }
            set
            {
				this._buttonVisible = value;
				this.RaisePropertyChanged(() => this.ButtonVisible);
            }
        }

        public bool ButtonEnabled
        {
			get { return this._buttonEnabled; }
            set
            {
				this._buttonEnabled = value;
				this.RaisePropertyChanged(() => this.ButtonEnabled);
            }
        }

        public bool IsCheckedEnabled
        {
			get { return this._isCheckedEnabled; }
            set
            {
				this._isCheckedEnabled = value;
				this.RaisePropertyChanged(() => this.IsCheckedEnabled);
            }
        }

        public bool IsShowProgressBar
        {
			get { return this._isShowProgressBar; }
            set
            {
				this._isShowProgressBar = value;
				this.RaisePropertyChanged(() => this.IsShowProgressBar);
            }
        }

        private static BitmapImage Build(string name)
        {
            return new BitmapImage(new Uri(String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", name)));
        }

		// icon-button in Grid
		private void OkCancelCommandExecuted()  
        {
			this.AbortItemUploadThread();
        }

		// визуальный эффект и присваивание имени "abort" процессу
		public void AbortItemUploadThread()
        {
            try
            {
				//this._uploadUnit.AbortUpDownLoading();  было

				//II бокер
				//if (this._uploadWrapperMulti.Multi.Address != 0x80)
				//{
				//	this._uploadWrapperMulti.Multi.AbortUpDownLoading();
				//}

				//присваивание имени "abort" процессу
				this._wrapperMultiRepository.AbortUpDownLoading(this._wrapperMulti);
		
				this._buttonImage = AbortButtonIcon;
				this.RaisePropertyChanged(() => this.ButtonImage);

				this.ButtonEnabled = false;
				this.Cancelled = true;
				//this.IsChecked = false;

				this.IsShowProgressBar = false;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Cancel", exc);
            }
        }

		public void ClearUploadingFileLists()
		{
			this._uploadingFiles.Clear();
			this._uploadingFullPathFiles.Clear();
			this._uploadingFilesNames.Clear();
			this._uploadingFilesLength = 0;

			this._downloadFiles.Clear();
			this._downloadFilesNames.Clear();
			this._downloadFilesLength = 0;
			
		}

		public void AddFileInfoToUploadingFileList(List<string> filePathList)
		{
			foreach (string file in filePathList)
			{
				FileInfo fi = new FileInfo(file);
				this._uploadingFiles.Add(fi);
				this._uploadingFullPathFiles.Add(file);
				this._uploadingFilesNames.Add(fi.Name.ToLower());
				this._uploadingFilesLength += fi.Length;
				_logger.Info("File for Upload : " + fi.Name);
			}
		}

		public void AddFileInfoToDownloadFileList(List<ItemDir> filesOnPDA)
		{
			foreach (var file in filesOnPDA)
			{
				if (file.size > 0)
				{
					this._downloadFiles.Add(file);
					this._downloadFilesNames.Add(file.name.ToLower());
					this._downloadFilesLength += file.size;
					_logger.Info("File for Download : " + file.name);
				}
				else
				{
					_logger.Info("File " + file.name + " will not be Download - because size == 0 ");
				}
			}
		}

        public void StartGUIUpdate()
        {
			this._buttonImage = AbortButtonIcon;
			this.RaisePropertyChanged(() => this.ButtonImage);

			this.ButtonVisible = true;
			this.ButtonEnabled = true;
			this.IsCheckedEnabled = false;

			this.IsShowProgressBar = true;
        }

		//public void StopViaButtonStop()
		//{
		//	this.AbortItemUploadThread();
		//}

        public void Finish()
        {
			this._buttonImage = OkButtonIcon;
			this.RaisePropertyChanged(() => ButtonImage);

			this.ButtonEnabled = false;
			this.ButtonVisible = true;  //?

			this.DoneDuringSession = 0;

			this.Done = true;

			this.IsShowProgressBar = false;

			this.File = String.Empty;
			this.Number = String.Empty;
			this.Value = String.Empty;
        }

        public void Error()
        {
            try
            {
				_logger.Info("Error() start" + this._wrapperMulti.ComPortStatic);
				this._buttonImage = ErrorButtonIcon;
				this.RaisePropertyChanged(() => this.ButtonImage);

				this.ButtonEnabled = false;
				this.ButtonVisible = true;
				this.IsShowProgressBar = false;
				//this.IsChecked = false;
				this.IsError = true;

				//this.UploadUnit.AbortUpDownLoading(); было
	
				//II бокер
				//if (this._uploadWrapperMulti.Multi.Address != 0x80)
				//{
				//	this._uploadWrapperMulti.Multi.AbortUpDownLoading();
				//}

				//присваивание имени "abort" процессу
				// Last***
				//this._wrapperMultiRepository.AbortUpDownLoading(this._wrapperMulti);
				// Last***

                this.State = UploadToPdaItemState.UploadError;

				_logger.Info("Error() finish " + this._wrapperMulti.ComPortStatic); 
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Error()", exc);
            }
        }
    }
}