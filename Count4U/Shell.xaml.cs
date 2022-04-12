using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.GenerationReport.Settings;
using Count4U.Media;
using Count4U.Model;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;
using Count4U.GenerationReport;
using Count4U.Common.Extensions;
using System.Reactive.Linq;
using System.IO;
using Count4U.Model.Interface;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Microsoft.Practices.ServiceLocation;
using Count4U.Common.Helpers.Ftp;
using System.Net;

namespace Count4U
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int WndMinWidth = 965;
        private const int WndMinHeight = 750;

        private bool _appliationShouldCloseWithoutWarning;
		//public string CurrentInventorCode = "";
		//private IObservable<long> observCopyingHostFiles;
		//private IDisposable disposeObservCopyingHostFiles;
		private IObservable<long> observCopyingFiles;
		private IDisposable disposeObservCopyingFiles;

	

		//inData в папке каждого инвентора
		string _importPDAFolder;
		//"C:\MIS\"
		string _importSourcePDAFolder;
		//"C:\MIS\IDnextData\fromHT"
		string _sourceFolder;
		//"C:\MIS\IDnextData\fromHT\unsure"
		string _unsureFolder;
		//"C:\MIS\IDnextData\fromHT\backup"
		string _backupFolder;

		string _importFolder;

		private string _host = "";
		private string _hostToApp = "";
		private string _hostmINV = "";
		private string _hostFromApp = "";
		private string _hostUser = "";
		private string _hostPassword = "";
		//private bool _allowWindowsHost = true;
		

        public Shell()
        {
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;          // Add 26.12.2021
            InitializeComponent();
			//DwmDropShadow.DropShadowToWindow(this);
            this.Loaded += Shell_Loaded;
            this.Initialized += Shell_Initialized;
            this.SourceInitialized += win_SourceInitialized;
            this.Closing += MainWindow_Closing;
            this.PreviewKeyDown += Shell_PreviewKeyDown;
            this.PreviewMouseLeftButtonDown += Shell_MouseLeftButtonDown;
            this.PreviewMouseRightButtonDown += Shell_MouseRightButtonDown;

            //gridUpper.PreviewMouseLeftButtonDown += GridUpper_MouseLeftButtonDown;
            gridUpper.MouseLeftButtonDown += GridUpper_MouseLeftButtonDown;

            pathMin.MouseLeftButtonDown += PathMin_MouseLeftButtonDown;
            pathMax.MouseLeftButtonDown += PathMax_MouseLeftButtonDown;
            pathClose.MouseLeftButtonDown += PathClose_MouseLeftButtonDown;

            this.PreviewMouseUp += Shell_PreviewMouseUp;
            this.PreviewKeyUp += Shell_PreviewKeyUp;
            this.KeyUp += Shell_KeyUp;

            Application.Current.Deactivated += Application_Deactivated;
        }

        public IEventAggregator EventAggregator { get; set; }
        public IUnityContainer Container { get; set; }

        public void Init()
        {
            this.EventAggregator.GetEvent<WindowTitleChangeEvent>().Subscribe(WindowTitleChanged);
            this.EventAggregator.GetEvent<ApplicationShouldCloseWithoutWarningEvent>().Subscribe(AppliationShouldCloseWithoutWarning);
        }

        private void AppliationShouldCloseWithoutWarning(object o)
        {
            this._appliationShouldCloseWithoutWarning = true;
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
			if (Container != null)
            {
                IUserSettingsManager userSettingsManager = this.Container.Resolve<IUserSettingsManager>();
                MainWindowSettings settings = userSettingsManager.MainWindowSettingsGet();
				IDBSettings dbSettings = this.Container.Resolve<IDBSettings>();
                if (settings != null)
                {
                  //  if (settings.IsMaximized)
                        this.WindowState = WindowState.Maximized;
					//else
					//{
					//	this.Top = settings.Top;
					//	this.Left = settings.Left;
					//	this.Width = settings.Width;
					//	this.Height = settings.Height;
					//}
				//WindowStyler.DisableMinimize(this);
                }

                enLanguage language = userSettingsManager.LanguageGet();
                string langStr = "en";
                switch (language)
                {
                    case enLanguage.English:
                        langStr = "en";
                        break;
                    case enLanguage.Hebrew:
                        langStr = "he";
                        break;
                    case enLanguage.Italian:
                        langStr = "it";
                        break;
                    case enLanguage.Russian:
                        langStr = "ru";
                        break;
                }

                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langStr);

                if (language == enLanguage.Hebrew)
                {
                    this.FlowDirection = FlowDirection.RightToLeft;
                    FrameworkElement.FlowDirectionProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(FlowDirection.RightToLeft));
                }

                ISettingsRepository settingsRepository = Container.Resolve<ISettingsRepository>();
                if (settingsRepository != null)
                {
                    settingsRepository.CurrentLanguage = langStr;
                    settingsRepository.ReportRepositoryGet = userSettingsManager.ReportRepositoryGet();
                }

                SetVersionTextBlock();

				this._importPDAFolder = dbSettings.ImportFolderPath();//inData в папке каждого инвентора
				this._importSourcePDAFolder = userSettingsManager.ImportPDAPathGet();	//"C:\MIS\"
				this._sourceFolder = this._importSourcePDAFolder.Trim('\\') + @"\IDnextData\fromHT"; //"C:\MIS\IDnextData\fromHT"
				this._unsureFolder = System.IO.Path.Combine(this._sourceFolder, "unsure");	//"C:\MIS\IDnextData\fromHT\unsure"
				this._backupFolder = System.IO.Path.Combine(this._sourceFolder, "backup");		//"C:\MIS\IDnextData\fromHT\backup"

				try
				{
					//"C:\MIS\"
					if (Directory.Exists(this._importPDAFolder) == false) Directory.CreateDirectory(this._importPDAFolder);
					//"C:\MIS\IDnextData\fromHT"
					if (Directory.Exists(this._importSourcePDAFolder) == false) Directory.CreateDirectory(this._importSourcePDAFolder);
					//"C:\MIS\IDnextData\fromHT\unsure"
					if (Directory.Exists(this._unsureFolder) == false) Directory.CreateDirectory(this._unsureFolder);
					//"C:\MIS\IDnextData\fromHT\backup"
					if (Directory.Exists(this._backupFolder) == false) Directory.CreateDirectory(this._backupFolder);
				}
				catch { }

				//Copy From Source Folder
				//userSettingsManager.CopyFromSourceSet(false); // при открытии приложения всегда true //false
				//observCountingFiles = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)).Select(x => x);
				//disposeObservCountingFiles = observCountingFiles.Subscribe(CountingFiles);
				observCopyingFiles = Observable.Timer(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(8)).Select(x => x);
				disposeObservCopyingFiles = observCopyingFiles.Subscribe(CopyingFiles);

				//Host
				//this._host = userSettingsManager.HostGet().Trim('\\');
				//this._hostUser = userSettingsManager.UserGet();
				//this._hostPassword = userSettingsManager.PasswordGet();
				//this._hostmINV = @"mINV";
				//this._hostToApp = @"ToApp";
				//this._hostFromApp = @"FromApp";
		 

				//observCopyingHostFiles = Observable.Timer(TimeSpan.FromSeconds(23), TimeSpan.FromSeconds(8)).Select(x => x);
				//disposeObservCopyingHostFiles = observCopyingHostFiles.Subscribe(CopyingHostFiles);
				//Database.SetInitializer<PreviousContext>(new PreviousContextInitializer());
            }

			Task.Factory.StartNew(LogEnvironmentInfo).LogTaskFactoryExceptions("Shell_Loaded");
        }

        void Shell_Initialized(object sender, EventArgs e)
        {

        }

        void Shell_KeyUp(object sender, KeyEventArgs e)
        {
            if (EventAggregator == null) return;

            if (e.Key == Key.BrowserBack)
            {
                EventAggregator.GetEvent<SpecialKeyEvent>().Publish(SpecialKey.Back);
                return;
            }

            if (e.Key == Key.BrowserForward)
            {
                EventAggregator.GetEvent<SpecialKeyEvent>().Publish(SpecialKey.Forward);
                return;
            }
        }

        void Shell_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (EventAggregator == null) return;


            //            if (Keyboard.Modifiers == ModifierKeys.Control)
            //            {

            if (e.Key == Key.PageUp || e.Key == Key.PageDown)
            {
                EventAggregator.GetEvent<WindowPreviewKeyUpEvent>().Publish(e.Key);
            }
            //            }
        }

        void Shell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //close popup            
            this.EventAggregator.GetEvent<PopupWindowCloseEvent>().Publish(e.OriginalSource);
        }

        private void Shell_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //close popup            
            this.EventAggregator.GetEvent<PopupWindowCloseEvent>().Publish(e.OriginalSource);
        }

        void Application_Deactivated(object sender, EventArgs e)
        {
            //close popup            
            this.EventAggregator.GetEvent<ApplicationDeactivatedEvent>().Publish(this);
        }

        void Shell_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EventAggregator == null) return;

            if (e.ChangedButton == MouseButton.XButton1)
            {
                EventAggregator.GetEvent<SpecialKeyEvent>().Publish(SpecialKey.Back);
                return;
            }

            if (e.ChangedButton == MouseButton.XButton2)
            {
                EventAggregator.GetEvent<SpecialKeyEvent>().Publish(SpecialKey.Forward);
                return;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			//if (disposeObservCopyingHostFiles != null) disposeObservCopyingHostFiles.Dispose();
			//if (disposeObservCopyingFiles != null) disposeObservCopyingFiles.Dispose();

            if (this._appliationShouldCloseWithoutWarning)
            {
				if (disposeObservCopyingFiles != null)
				{
					disposeObservCopyingFiles.Dispose();
					_logger.Info("MainWindow_Closing1 >> disposeObservCopyingFiles.Dispose");
				}

				SaveMainWindowSettings();
            }
            else
            {
                IEventAggregator eventAggregator = this.Container.Resolve<IEventAggregator>();

                eventAggregator.GetEvent<ApplicationClosingEvent>().Publish(this);

                string messageText = Container.Resolve<ImportPdaPrintQueue>().IsPrinting ?
                    Localization.Resources.Msg_Close_Application_Printing :
                    Localization.Resources.Msg_Close_Application;

                MessageBoxResult result = UtilsMisc.ShowMessageBox(messageText, MessageBoxButton.OKCancel, MessageBoxImage.Question, this.Container.Resolve<IUserSettingsManager>());
                if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;

                if (result == MessageBoxResult.OK && this.Container != null)
                {
					if (disposeObservCopyingFiles != null)
					{
						disposeObservCopyingFiles.Dispose();
						_logger.Info("MainWindow_Closing2 >> disposeObservCopyingFiles.Dispose");
					}
                    SaveMainWindowSettings();
                }
            }

			if (this.Container != null)
			{
				try
				{
					IRegionManager regionManager = this.Container.Resolve<IRegionManager>();
					List<string> regionNames = regionManager.Regions.Select(x => x.Name).ToList();
					foreach (string name in regionNames)
					{
						try
						{
							regionManager.Regions.Remove(name);
						}
						catch { }
					}
					//regionNames = regionManager.Regions.Select(x => x.Name).ToList();

				}
				catch (Exception ex)
				{
					string messsage = ex.Message;
				}
			}
		}

		//CopyFromHostGet
		//public void CopyingHostFiles(long x)
		//{
		//	IUserSettingsManager userSettingsManager = this.Container.Resolve<IUserSettingsManager>();

		//	bool copyFromHost = userSettingsManager.CopyFromHostGet(); //копировать или нет
		//	if (copyFromHost == false) return;
		//	string customerCodeProcess = "";
		//	string inventorCodeProcess = "";
		//	//bool copyByCodeInventorOnly = userSettingsManager.CopyByCodeInventorGet();
		//	//if (copyByCodeInventorOnly == true)
		//	//{
		//	IServiceLocator serviceLocator = this.Container.Resolve<IServiceLocator>();
		//	IAuditConfigRepository audtiConfigRepository = serviceLocator.GetInstance<IAuditConfigRepository>();

		//	if (audtiConfigRepository != null)
		//	{
		//		AuditConfig ac = audtiConfigRepository.GetProcessConfig(CBIContext.History);
		//		customerCodeProcess = ac.CustomerCode;
		//		inventorCodeProcess = ac.InventorCode;
		//	}
		//	if (customerCodeProcess == "") return;
		//	if (inventorCodeProcess == "") return;
		//	string targetFolder = this._importPDAFolder.Trim('\\') + @"\Inventor\" + inventorCodeProcess + @"\inData";
		//	if (Directory.Exists(targetFolder) == false) return;

		//	//}

		//	//string _host = userSettingsManager.HostGet().Trim('\\');
		//	//string _hostUser = userSettingsManager.UserGet();
		//	//string _hostPassword = userSettingsManager.PasswordGet(); 
		//	//string _hostmINV = @"mINV";
		//	//string _hostFromApp = @"FromApp"; 

		//	FtpClient client = new FtpClient(this._host, this._hostUser, this._hostPassword);

		//	try
		//	{
		//		string sourceFolder = this._hostmINV + @"\" + this._hostFromApp + @"\" + customerCodeProcess;
		//		FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(sourceFolder);
		//		if (statusCode == FtpStatusCode.PathnameCreated)
		//		{
		//			List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(sourceFolder, this._host, this._hostUser, this._hostPassword);
		//			if (listInfoDirectory.Count < 1) return;

		//			bool haveBackupFolder = false;
		//			FileDirectoryInfo backup = listInfoDirectory.FirstOrDefault(k => k.IsDirectory == true && k.Name == "backup");
		//			if (backup != null) haveBackupFolder = true;

		//			foreach (FileDirectoryInfo fi in listInfoDirectory)
		//			{
		//				if (fi == null) continue;
		//				if (string.IsNullOrWhiteSpace(fi.Name) == true) continue;
		//				if (fi.IsDirectory == true) continue;
		//				string fileName = fi.Name;

		//				string inventorCode = inventorCodeProcess;
		//				string targetPath = "none";

		//				targetPath = targetFolder + @"\" + fileName; //Inventor
		//				string fromFtpPathProfile = sourceFolder;
		//				try
		//				{   //Inventor
		//					if (File.Exists(targetPath) == true) continue;//Вернуть //File.Delete(targetPath);				 //Count4U

		//					fromFtpPathProfile = sourceFolder + @"\" + fileName;		  //  на ftp	  (from)
		//					string result = client.GetFileFromFtp(fileName, targetPath);
		//					_logger.Info("Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
		//					_logger.Info("with Result [" + result + "]");

		//					if (haveBackupFolder == true)
		//					{
		//						string result1 = client.SaveFileToFtp(targetPath, @"backup\" + fileName);
		//					}

		//					string result2 = client.DeleteFile(fileName);
		//				}
		//				catch (Exception exc)
		//				{
		//					string message = String.Format("CopyingHostFiles (File.Move to unsurePath) {0} => {1}", fromFtpPathProfile, targetPath);
		//					_logger.ErrorException(message, exc);
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("CopyingHostFiles", exc);
		//		throw;
		//	}
		//}

		public void CopyingFiles(long x)
		{
			IUserSettingsManager userSettingsManager = this.Container.Resolve<IUserSettingsManager>();

			bool copyFromSource = userSettingsManager.CopyFromSourceGet(); //копировать или нет

			if (copyFromSource == false)
			{
				//_logger.Info("info: start CopyingFiles >>  CopyFromSourceGet == false");
				return;
			}
			//_logger.Info("info: start CopyingFiles >> CopyFromSourceGet == true");

			string inventorCodeProcess = "";
			bool copyByCodeInventorOnly = userSettingsManager.CopyByCodeInventorGet();
			bool forward = userSettingsManager.ForwardResendDataGet();
			if (copyByCodeInventorOnly == true)
			{
				IServiceLocator serviceLocator = this.Container.Resolve<IServiceLocator>();
				IAuditConfigRepository audtiConfigRepository = serviceLocator.GetInstance<IAuditConfigRepository>();

				if (audtiConfigRepository != null)
				{
					AuditConfig ac = audtiConfigRepository.GetProcessConfig(CBIContext.History);
					if (ac != null)
					{
						inventorCodeProcess = ac.InventorCode;
					}
					else
					{
						_logger.Info("info run CopyingFiles >> can't >> ac Is null ? Error");
						return;
					}
				}
				if (inventorCodeProcess == "")
				{
					_logger.Info("info run CopyingFiles >> can't >> inventorCodeProcess Is empty ? Error");
					return;
				}
			}
			

			//string _importPDAFolder; //inData в папке каждого инвентора
			//string _importSourcePDAFolder;//"C:\MIS\"
			//string _sourcePath;	//"C:\MIS\IDnextData\fromHT"
			//string _unsureFolder;	//"C:\MIS\IDnextData\fromHT\unsure"
			//string _backupFolder;//"C:\MIS\IDnextData\fromHT\backup"
			//string _importFolder;

			try
			{

				DirectoryInfo dir = new System.IO.DirectoryInfo(this._sourceFolder);
				if (dir == null) return;
				int filesInFolder = dir.GetFiles().Length;
				if (filesInFolder == 0) return;
				_logger.Info("info CopyingFiles >> sourceFiles count =" + filesInFolder.ToString());

				FileInfo[] sourceFiles = dir.GetFiles();
				foreach (FileInfo fi in sourceFiles)
				{
					if (fi == null) continue;
					string fileName = fi.Name;
					if (string.IsNullOrWhiteSpace(fileName) == true) continue;
					string sourcePath = System.IO.Path.Combine(this._sourceFolder, fileName);
					string backupPath = System.IO.Path.Combine(this._backupFolder, fileName);
					string unsurePath = System.IO.Path.Combine(this._unsureFolder, fileName);											//unsure

					string inventorCode = "none";
					string targetPath = "none";
					string targetForwardPath = "none";

					string[] part = fileName.Split('_');
					if (part.Length >= 2) inventorCode = part[1];

					bool copyToInventorCode = false;
					if (copyByCodeInventorOnly == true)		  //копировать только текущий Inventor остальное копировать в папку сомнительных
					{	 
						if (inventorCode.ToUpper().Trim() == inventorCodeProcess.ToUpper().Trim())	
						{
							copyToInventorCode = true;
						}
						else 
						{
							copyToInventorCode = false;
							_logger.Info("info CopyingFiles >> inventorCode [" + inventorCode + "] <> inventorCodeProcess [" + inventorCodeProcess + "] ");
						}
					}
					else // copyByCodeInventorOnly == false	  это как раньше все инвенторы по своим папкам
					{
						if (inventorCode != "none")
						{
							copyToInventorCode = true;
						}
					}

					if (copyToInventorCode == true) // если есть Inventor  который будем копировать в свою папку
					{
						string targetFolder = this._importPDAFolder.Trim('\\') + @"\Inventor\" + inventorCode + @"\inData";
						string targetFolderForward = this._importPDAFolder.Trim('\\') + @"\Inventor\" + inventorCode + @"\inData\Forward";
						if (Directory.Exists(targetFolder) == true) // есть Inventor
						{
							if (forward == true)
							{
								if (Directory.Exists(targetFolderForward) == false)
									try { Directory.CreateDirectory(targetFolderForward); }
									catch { }
								targetForwardPath = targetFolderForward + @"\" + fileName; 
							}

							targetPath = targetFolder + @"\" + fileName; //Inventor

							string backupFolder = this._backupFolder.Trim('\\') + @"\" + inventorCode; //backup
							if (Directory.Exists(backupFolder) == false) { Directory.CreateDirectory(backupFolder); }
							backupPath = backupFolder + @"\" + fileName;			//backup
							try
							{   //Inventor
								if (File.Exists(targetPath) == true) File.Delete(targetPath);
								if (File.Exists(sourcePath) == true)
								{
									File.Copy(sourcePath, targetPath);
									string message = String.Format("CopyingFile (File.Copy to inData) {0} => {1}", sourcePath, targetPath);
									_logger.Info(message);
								}
								//forward
								if (forward == true)
								{
									if (File.Exists(targetForwardPath) == true) File.Delete(targetForwardPath);
									if (File.Exists(sourcePath) == true) File.Copy(sourcePath, targetForwardPath);
								}
								//backup
								if (File.Exists(backupPath) == true) File.Delete(backupPath);
								if (File.Exists(sourcePath) == true) File.Move(sourcePath, backupPath);
							}
							catch (Exception exc)
							{
								string message = String.Format("CopyingFiles (File.Move to unsurePath) {0} => {1}", sourcePath, targetPath);
								_logger.ErrorException(message, exc);
							}
						}
						else // нет Inventor => to unsurePath
						{
							try
							{
								string unsureFolder = this._unsureFolder.Trim('\\') + @"\" + inventorCode;
								if (Directory.Exists(unsureFolder) == false) Directory.CreateDirectory(unsureFolder);
								unsurePath = unsureFolder + @"\" + fileName;			//unsure
								if (File.Exists(unsurePath) == true) File.Delete(unsurePath);
								if (File.Exists(sourcePath) == true) File.Move(sourcePath, unsurePath);

								string message = String.Format("info: CopyingFile (File.Move to unsurePath) {0} => {1}", sourcePath, unsurePath);
								_logger.Info(message);
							}
							catch (Exception exc)
							{
								string message = String.Format("CopyingFiles (File.Move to unsurePath) {0} => {1}", sourcePath, unsurePath);
								_logger.ErrorException(message, exc);
							}
						}
					}
						//(copyToInventorCode == false)
						// все что не выполняют условия попадают в 	   unsurePath
					else //inventorCode == "none"=> to unsurePath
					{
						try
						{
							if (File.Exists(unsurePath) == true) File.Delete(unsurePath);
							if (File.Exists(sourcePath) == true) File.Move(sourcePath, unsurePath);
						}
						catch (Exception exc)
						{
							string message = String.Format("CopyingFiles (File.Move to unsurePath) {0} => {1}", sourcePath, unsurePath);
							_logger.ErrorException(message, exc);
						}
					}

				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("CopyingFiles", exc);
				throw;
			}
		}

		public void CountingFiles(long x)
		{
			//if (this._listeningSourceFolder == false) return;
			//if (string.IsNullOrWhiteSpace(this._sourcePath) == true)
			//{
			//	this.ProcessLisner = 0;
			//	return;
			//}

			//if (Directory.Exists(this._sourcePath) == true)
			//{
			//	DirectoryInfo dir = new System.IO.DirectoryInfo(this._sourcePath);
			//	this.ProcessLisner = dir.GetFiles().Length;
			//}
			//else
			//{
			//	this.ProcessLisner = 0;
			//}
		}


        void GridUpper_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
				System.Windows.Shapes.Path path = e.OriginalSource as System.Windows.Shapes.Path;
                if (path != null/* && path != programNamePath*/) return;

                if (e.OriginalSource == pathMin || e.OriginalSource == pathMax || e.OriginalSource == pathClose)
                    return;

                if (e.ClickCount == 1)
                    this.DragMove();

                if (e.ClickCount == 2)
                {
                    e.Handled = true;
                    ToggleMaximizeNormal();
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("GridUpper_MouseLeftButtonDown", exc);
            }
        }

        void PathMax_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleMaximizeNormal();

            //            IRegionManager manager = Container.Resolve<IRegionManager>();
            //            IRegion region = manager.Regions[Common.RegionNames.ApplicationWindow];
            //            region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        void PathClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        void PathMin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void WindowTitleChanged(string s)
        {
            //tbTitle.Text = s;
        }

        void Shell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S &&
                (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                UtilsLogging.WriteNavigationInfoToLog(Container);
            }
        }

        #region magic with styless window

        private void win_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(WindowProc);
        }

        private static IntPtr WindowProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case 0x0046:
                    WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                    //                    if ((pos.flags & (int)WINDOWPOS.NOMOVE) != 0)
                    //                    {
                    //                        return IntPtr.Zero;
                    //                    }

                    Window wnd = (Window)HwndSource.FromHwnd(hwnd).RootVisual;
                    if (wnd == null)
                    {
                        return IntPtr.Zero;
                    }

                    bool changedPos = false;

                    // ***********************
                    // Here you check the values inside the pos structure
                    // if you want to override them just change the pos
                    // structure and set changedPos to true
                    // ***********************

                    // this is a simplified version that doesn't work in high-dpi settings
                    // pos.cx and pos.cy are in "device pixels" and MinWidth and WndMinHeight 
                    // are in "WPF pixels" (WPF pixels are always 1/96 of an inch - if your
                    // system is configured correctly).
						

						if (pos.cx < WndMinWidth)
						{
								 pos.cx = WndMinWidth;
							changedPos = true;
						}
						if (pos.cy < WndMinHeight)
						{
							pos.cy = WndMinHeight;
							changedPos = true;
						}
				
					//pos.flags = 0x0080;
                    // ***********************
                    // end of "logic"
                    // ***********************

                    if (!changedPos)
                    {
                        return IntPtr.Zero;
                    }

                    Marshal.StructureToPtr(pos, lParam, true);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            public RECT rcMonitor = new RECT();

            public RECT rcWork = new RECT();

            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static readonly RECT Empty = new RECT();

            public int Width
            {
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
            }

            public int Height
            {
                get { return bottom - top; }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

        private string GetProgramVersion()
        {
            Version v = this.Container.Resolve<IApplicationVersion>().Get();
            string versionText = String.Format("v. {0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision);
            return versionText;
        }

        private void SetVersionTextBlock()
        {
            string versionText = GetProgramVersion();
            tbVersion.Text = versionText;
        }

        private void LogEnvironmentInfo()
        {
            try
            {
                string programVersion = GetProgramVersion();
                int renderingTier = (RenderCapability.Tier >> 16);

                bool is64OperatingSystem = Environment.Is64BitOperatingSystem;
                OperatingSystem operatingSystem = Environment.OSVersion;
                int processorCount = Environment.ProcessorCount;
                Version clrVersion = Environment.Version;

                string processorInfo = String.Empty;
                string memoryInfo = String.Empty;
                using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor"),
                                                system = new ManagementObjectSearcher("select * from Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in win32Proc.Get())
                    {
                        string clockSpeed = obj["CurrentClockSpeed"].ToString();
                        string procName = obj["Name"].ToString();

                        processorInfo += String.Format("clock speed: {0}, name: {1}", clockSpeed, procName);
                    }

                    foreach (ManagementObject obj in system.Get())
                    {
                        var memory = Convert.ToInt64(obj["TotalPhysicalMemory"]);
                        memoryInfo += String.Format("Total MB: {0}", (int)(memory / 1024 / 1024));
                    }
                }

                string compilationMode = String.Empty;
#if DEBUG
                compilationMode = "DEBUG";
#else
            compilationMode = "RELEASE";
#endif

                string redaction = FileSystem.IsAppRedactionOffice() ? "OFFICE" : "LAPTOP";

                _logger.Info("Program version: " + programVersion);
                _logger.Info("Operating system: " + operatingSystem);
                _logger.Info("Is 64bit operating system: " + is64OperatingSystem);
                _logger.Info("Processor count: " + processorCount);
                _logger.Info("Processor info: " + processorInfo);
                _logger.Info("Memory info: " + memoryInfo);
                _logger.Info("RenderingTier: " + renderingTier);
                _logger.Info("CLR version: " + clrVersion);
                _logger.Info("Compilation mode: " + compilationMode);
                _logger.Info("App redaction: " + redaction);
            }
            catch (Exception exc)
            {
                _logger.Info("LogEnvironmentInfo " + exc);
            }
        }

        private void SaveMainWindowSettings()
        {
            IUserSettingsManager userSettingsManager = this.Container.Resolve<IUserSettingsManager>();
            MainWindowSettings settings = new MainWindowSettings();
            settings.Top = this.Top;
            settings.Left = this.Left;
            settings.Width = this.ActualWidth;
            settings.Height = this.ActualHeight;
            //settings.IsMaximized = this.WindowState == WindowState.Maximized;
			settings.IsMaximized = true;

            userSettingsManager.MainWindowSettingsSet(settings);
        }

        private void ToggleMaximizeNormal()
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
        }


    }
}
