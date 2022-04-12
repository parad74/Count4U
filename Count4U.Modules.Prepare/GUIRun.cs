using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.GenerationReport.Settings;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using NLog;

namespace Count4U.Modules.Prepare
{
    public class GUIRun
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDBSettings _dbSettings;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IRegionManager _regionManager;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly ILog _iLog;
		private readonly ISettingsRepository _settingsRepository;
		

        public GUIRun(IDBSettings dbSettings, 
            IContextCBIRepository contextCBIRepository, 
            IRegionManager regionManager, 
            IUserSettingsManager userSettingsManager,
			ISettingsRepository settingsRepository,
            ILog iLog)
        {
            this._iLog = iLog;
            this._userSettingsManager = userSettingsManager;
            this._regionManager = regionManager;
            this._contextCbiRepository = contextCBIRepository;
            this._dbSettings = dbSettings;
			this._settingsRepository = settingsRepository;
			
        }

        public void Run(Action<NavigationResult> navigationCallback = null)
        {
            //init logging
            this._iLog.SetIncludeMessageType(UserSettingsHelpers.LogTypeListGet(this._userSettingsManager).Where(r => r.Value).Select(r => r.Key).ToList());

            string checkDbResult = CheckDbConnection();
            if (String.IsNullOrEmpty(checkDbResult))
                this.ResolveViews(navigationCallback);
            else
            {
                Mouse.OverrideCursor = null;
                UtilsMisc.ShowMessageBox(checkDbResult, MessageBoxButton.OK, MessageBoxImage.Error, this._userSettingsManager, Application.Current.MainWindow);
                _logger.Error("CheckDbConnection returned error" + checkDbResult);
            }
        }

        private string CheckDbConnection()
        {
            _logger.Debug("Checking db...");
            return this._dbSettings.CheckDb();
        }

        private void ResolveViews(Action<NavigationResult> navigationCallback = null)
        {
            _logger.Debug("Resolving views...");
            try
            {
                if (Utils.IsThereProcessConfigInHistoryContext(this._contextCbiRepository))
                {
					AuditConfig auditConfig = new AuditConfig(this._contextCbiRepository.GetProcessCBIConfig(CBIContext.History));
					this._contextCbiRepository.SetCurrentCBIConfig(CBIContext.History, auditConfig);
					this._contextCbiRepository.SetCurrentCBIConfig(CBIContext.Main, auditConfig);

					UriQuery query = new UriQuery();

					Utils.AddContextToQuery(query, CBIContext.History);
					Utils.AddDbContextToQuery(query, Common.NavigationSettings.CBIDbContextInventor);
					Utils.AddAuditConfigToQuery(query, auditConfig);

					query.Add(Common.NavigationSettings.ApplicationStart, String.Empty);

					Dictionary<string, string> startupArgumentDictionary = this._settingsRepository.StartupArgumentDictionary;
					if (startupArgumentDictionary.Count > 0)
					{
						if (startupArgumentDictionary.ContainsKey(Common.ViewNames.HomeDashboardView) == true)
						{
							UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query, navigationCallback);
						}
						else if (startupArgumentDictionary.ContainsKey(Common.ViewNames.InventorDashboardFullView) == true)
						{
							UtilsNavigate.InventorDashboardOpen(CBIContext.History, this._regionManager, query);
						}
						else if (startupArgumentDictionary.ContainsKey(Common.ViewNames.CustomerDashboardFullView) == true)
						{
							UtilsNavigate.CustomerDashboardOpen(CBIContext.History, this._regionManager, query);
						}
						else if (startupArgumentDictionary.ContainsKey(Common.ViewNames.BranchDashboardFullView) == true)
						{
							UtilsNavigate.BranchDashboardOpen(CBIContext.History, this._regionManager, query);
						}
						else if (startupArgumentDictionary.ContainsKey(Common.ViewNames.ComplexOperationView) == true)
						{
							UriQuery uriQuery = new UriQuery();
							uriQuery.Add(Common.NavigationSettings.ImportMode, Common.NavigationSettings.ComplexModeObject);
							Utils.AddAuditConfigToQuery(uriQuery, auditConfig);

							bool branchIsProcess = true;
							if (branchIsProcess == false)
							{
								Utils.AddContextToQuery(uriQuery, CBIContext.History);
								Utils.AddDbContextToQuery(uriQuery, Common.NavigationSettings.CBIDbContextInventor);
								uriQuery = Utils.AddFromContextToQuery(uriQuery, FromContext.StartApplicationWithoutAction);
								UtilsNavigate.ComplexOperationViewOpen(this._regionManager, uriQuery);
							}
							else
							{
								UtilsNavigate.ComplexOperationBranchViewOpen(this._regionManager, uriQuery);
							}
						}
						else
						{
							UtilsNavigate.IturListDetailsOpen(this._regionManager, query, navigationCallback, false);
						}
					}
					else
					{
						UtilsNavigate.IturListDetailsOpen(this._regionManager, query, navigationCallback, false);
					}
					
				}
				else
				{
                    UriQuery query = new UriQuery();
                    
                    Utils.AddAuditConfigToQuery(query, this._contextCbiRepository.GetProcessCBIConfig(CBIContext.History));                    
                    UtilsNavigate.HomeDashboardOpen(CBIContext.Main, this._regionManager, query, navigationCallback);
                }              
            }
            catch (Exception exc)
            {
                _logger.DebugException("ResolveViews", exc);
            }
        }
    }
}