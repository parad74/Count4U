using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Modules.Prepare.Update;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using NLog;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model;
using Count4U.Common.Extensions;

namespace Count4U.Modules.Prepare.ViewModules
{
    public class UpdateViewModel : NotificationObject, INavigationAware
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IContextCBIRepository _contextCBIRepository;
        private readonly ILog _logImport;
        private readonly IDBSettings _dbSettings;
        private readonly InteractionRequest<MessageBoxNotification> _messageBoxRequest;
        private readonly IUserSettingsManager _userSettingsManager;
        protected readonly IAlterADOProvider _alterADOProvider;
        private readonly IRegionManager _regionManager;

        private readonly Updater _updater;
        private readonly GUIRun _guiRun;

        private bool _isBusy;
        private string _busyText;
        private string _progressText;
        private readonly IEventAggregator _eventAggregator;
		private IConnectionDB _connectionDB;

        public UpdateViewModel(
                Updater updater,
                GUIRun guiRun,
                IContextCBIRepository contextCBIRepository,
                IAlterADOProvider alterADOProvider,
                ILog logImport,
                IDBSettings dbSettings,
                IEventAggregator eventAggregator,
                IRegionManager regionManager,
                IUserSettingsManager userSettingsManager,
			 IConnectionDB connectionDB)
        {
            this._userSettingsManager = userSettingsManager;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._guiRun = guiRun;
            this._updater = updater;
            this._dbSettings = dbSettings;
            this._logImport = logImport;
            this._contextCBIRepository = contextCBIRepository;
            this._alterADOProvider = alterADOProvider;
			this._connectionDB = connectionDB;

            this._messageBoxRequest = new InteractionRequest<MessageBoxNotification>();
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                this.RaisePropertyChanged(() => this.IsBusy);
            }
        }

        public string BusyText
        {
            get { return this._busyText; }
            set
            {
                this._busyText = value;
                this.RaisePropertyChanged(() => this.BusyText);
            }
        }

        public string ProgressText
        {
            get { return this._progressText; }
            set
            {
                this._progressText = value;
                this.RaisePropertyChanged(() => this.ProgressText);
            }
        }

        public InteractionRequest<MessageBoxNotification> MessageBoxRequest
        {
            get { return this._messageBoxRequest; }
        }

        #region Implementation of INavigationAware

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Mouse.OverrideCursor = Cursors.Wait;
			Task.Factory.StartNew(StartUpdate).LogTaskFactoryExceptions("OnNavigatedTo");
         }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Mouse.OverrideCursor = null;
        }

        #endregion

        private void StartUpdate()
        {
			//bool tempCopyFromSource = this._userSettingsManager.CopyFromSourceGet();
			//this._userSettingsManager.CopyFromSourceSet(false); //копировать или нет   файлы 

#if DEBUG
            StartUpdateMigrateDb();
#else
            StartUpdateCopyAppData();            
            StartUpdateMigrateDb();
#endif

			//this._userSettingsManager.CopyFromSourceSet(tempCopyFromSource); //копировать или нет

        }

        private void StartUpdateCopyAppData()
        {
            _logger.Info("StartUpdateCopyAppData");
			
            try
            {
				if (this._updater.IsCleanRun() == true)					 //  не существует DBs return true
                {
					//от сюда
					//Utils.RunOnUI(() =>
					//{
					//	this.IsBusy = true;
					//	this.BusyText = Localization.Resources.Msg_PrepareDatabase;
					//});
					//до сюда вернуть если нужны будут сообщения о копировании БД -x
					// и восстановить 	 /*this.UpdateStatus*/ вниз по вызовам
                    this._updater.RunAppDataCopy(/*this.UpdateStatus*/);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("StartUpdate copy app data", exc);

                Utils.RunOnUI(() =>
                   {
                       string message = String.Format(Localization.Resources.Msg_Migrate_Db_Error, exc);
                       MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Error, Content = message, Settings = this._userSettingsManager };
                       this._messageBoxRequest.Raise(notification);

                       throw exc;
                   });
            }
        }

        private void StartUpdateMigrateDb()
        {
            _logger.Info("StartUpdateMigrateDb");
            try
            {

				this._connectionDB.CopyFromSetupEmptyProcessDB();
				
                if (this._alterADOProvider.IsIncreasedVerDB()) //invoke repository to check
                {

                    Utils.RunOnUI(() =>
                    {
                        this.IsBusy = true;
                    });

                    _logger.Info("Versions are different, db migration started...");

#if DEBUG
                    MigrateDb(/*dbVersion, */String.Empty);
#else
                    Utils.RunOnUI(() => BusyText = Localization.Resources.Msg_BackupDb);

                    string archivePath = this._updater.BackupAppDataBeforeSchemeMigration(UpdateProgressOnUI);

                    if (String.IsNullOrEmpty(archivePath))
                    {
                        ShowErrorMessage();
                        return;
                    }

                    MigrateDb(archivePath);
#endif
                }
                else
                {
                    Utils.RunOnUI(() =>
                    {
                        this.IsBusy = false;
                        RunNormalProgramFlow();
                    });

                }

            }
            catch (Exception exc)
            {
                _logger.ErrorException("StartUpdate, migrate db", exc);
                ShowErrorMessage();
            }
        }

        private void MigrateDb(string backupPath)
        {
            Utils.RunOnUI(() =>
            {
                IsBusy = true;
                BusyText = Localization.Resources.Msg_UpdateDbScheme;
                ProgressText = String.Empty;
            });

            try
            {
                _logger.Info("MigrateDb");

                this._alterADOProvider.UpdateDBViaScript();

                _logger.Info("--- MIGRATION LOG ---");
                string log = this._logImport.PrintLog();
                _logger.Info(log);
                _logger.Info("----------------------");

                _logger.Info("MigrateDb Done");
            }
            catch (Exception exc)
            {
                _logger.ErrorException("MigrateDb", exc);

#if DEBUG
              Utils.RunOnUI(() =>
               {
                   this.IsBusy = false;
                   string message = String.Format("Error while DB udpate");
                   MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Error, Content = message, Settings = this._userSettingsManager };
                   this._messageBoxRequest.Raise(notification);
               });
#else
                //rollback
                Utils.RunOnUI(() =>
                  {
                      this.IsBusy = false;

                      string message = String.Format(Localization.Resources.Msg_Migrate_Db_Error_Rollback, Environment.NewLine);
                      MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Error, Content = message, Settings = this._userSettingsManager };
                      this._messageBoxRequest.Raise(notification);

                      BusyText = Localization.Resources.Msg_Rollback;
                      this.IsBusy = true;
                  });

                _logger.Info("Restoring backup files");

                this._updater.RestoreAppDataOnFailedDbMigration(UpdateProgressOnUI, backupPath);
#endif
                Utils.RunOnUI(() =>
                {
                    this.IsBusy = false;
                    //prevent from asking "Are you sure..." 
                    this._eventAggregator.GetEvent<ApplicationShouldCloseWithoutWarningEvent>().Publish(null);
                    //close application
                    Application.Current.Shutdown();
                });
                return;
            }

            //if all is ok 
            Action postUpdate = PostUpdateActions;
            Application.Current.Dispatcher.BeginInvoke(postUpdate);
        }

        private void RunNormalProgramFlow()
        {
            this._guiRun.Run(NavigationCallback);
        }

        private void NavigationCallback(NavigationResult result)
        {
            IRegion region = this._regionManager.Regions[Common.RegionNames.ApplicationWindow];

            Type type = typeof(RegionNavigationJournal);
            FieldInfo fi = type.GetField("backStack", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi == null)
                return;

            Stack<IRegionNavigationJournalEntry> backStack = fi.GetValue(region.NavigationService.Journal) as Stack<IRegionNavigationJournalEntry>;
            if (backStack != null)
            {
                backStack.Clear();
            }
        }

        private void ShowErrorMessage()
        {
            Utils.RunOnUI(() =>
            {
                this.IsBusy = false;

                string message = String.Format(Localization.Resources.Msg_Common_Error_Occured);
                MessageBoxNotification notification = new MessageBoxNotification { Image = MessageBoxImage.Error, Content = message, Settings = this._userSettingsManager };
                this._messageBoxRequest.Raise(notification);
            });
        }

        private void UpdateStatus(string text)
        {
            Utils.RunOnUI(() =>
            {
                this.ProgressText = text;
            });
        }

        private void UpdateProgressOnUI(double progress, string processedTotal, string fileName)
        {
          Utils.RunOnUI(() =>
          {
              this.ProgressText = fileName;
          });
        }

        private void PostUpdateActions()
        {
            this.IsBusy = false;
            MessageBoxNotification notification = new MessageBoxNotification
            {
                Image = MessageBoxImage.Information,
                Content = Localization.Resources.Msg_Migrate_Db_Done2,
                Settings = this._userSettingsManager
            };
            this._messageBoxRequest.Raise(notification);

            this.RunNormalProgramFlow();
        }
    }
}