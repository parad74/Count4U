using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.Services.Ini;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Security.Cryptography;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NLog;
using Count4U.Common.Extensions;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.Constants;

namespace Count4U.Common.Helpers
{
    public static class Utils
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void MainWindowTitleSet(string title, IEventAggregator eventAggregator)
        {
            string finalTitle;
            if (String.IsNullOrEmpty(title))
            {
                finalTitle = "Count4U";
            }
            else
            {
                finalTitle = "Count4U: " + title;
            }

            //Application.Current.MainWindow.Title = finalTitle;
            eventAggregator.GetEvent<WindowTitleChangeEvent>().Publish(finalTitle);
        }

        public static string CodeNewGenerate()
        {
            return Guid.NewGuid().ToString();
        }

        public static string CodeNewGenerate(int passLenght)
        {
            string newCode = "";
            byte[] temp = RollDice(passLenght);
            for (int i = 0; i < temp.Length; i++)
            {
                newCode += (char)temp[i];
            }
            return newCode;
        }

        public static byte[] RollDice(int passLenght)
        {
            byte[] randomNumber = new byte[passLenght];
            byte[] password = new byte[passLenght];


            // Create a new instance of the RNGCryptoServiceProvider.
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            // Fill the array with a random value.
            Gen.GetBytes(randomNumber);

            for (int i = 0; i < randomNumber.Length; i++)
            {
                if ((int)randomNumber[i] > 48 && (int)randomNumber[i] < 57 ||						//0-9
                    //  (int)randomNumber[i] > 65 && (int)randomNumber[i] < 90 ||					   //A-Z
                    (int)randomNumber[i] > 97 && (int)randomNumber[i] < 122)						//a-z
                {
                    password[i] = randomNumber[i];
                }
                else
                {
                    Gen.GetBytes(randomNumber);
                    i--;
                }
            }
            return password;
        }



	
        private static string StringFromCBIContext(CBIContext cbiContext)
        {
            switch (cbiContext)
            {
                case CBIContext.CreateInventor:
                    return NavigationSettings.CBIContextInventor;
                case CBIContext.History:
                    return NavigationSettings.CBIContextHistory;
                case CBIContext.Main:
                    return NavigationSettings.CBIContextMain;
                default:
                    throw new ArgumentOutOfRangeException("cbiContext");
            }
        }

        public static void AddContextToDictionary(Dictionary<string, string> dic, CBIContext context)
        {
            if (dic == null) return;
            dic.Add(NavigationSettings.CBIContext, StringFromCBIContext(context));
        }

        public static void AddDbContextToDictionary(Dictionary<string, string> dic, string dbContext)
        {
            if (dic == null || String.IsNullOrEmpty(dbContext))
                return;

            dic.Add(NavigationSettings.CBIDbContext, dbContext);
        }

        public static CBIContext? CBIContextFromNavigationParameters(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.CBIContext && r.Value == NavigationSettings.CBIContextInventor))
                return CBIContext.CreateInventor;

            if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.CBIContext && r.Value == NavigationSettings.CBIContextMain))
                return CBIContext.Main;

            if (navigationContext.Parameters.Any(r => r.Key == NavigationSettings.CBIContext && r.Value == NavigationSettings.CBIContextHistory))
                return CBIContext.History;

            return null;
        }

        public static string CBIDbContextFromNavigationParameters(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.CBIDbContext))
            {
                string value = navigationContext.Parameters.FirstOrDefault(r => r.Key == NavigationSettings.CBIDbContext).Value;
                return value;
            }

            return String.Empty;
        }

        public static void AddContextToQuery(UriQuery uriQuery, CBIContext context)
        {
            if (uriQuery == null) return;
            uriQuery.Add(NavigationSettings.CBIContext, StringFromCBIContext(context));
        }

		public static UriQuery AddFromContextToQuery(UriQuery uriQuery, FromContext context)
		{
			if (uriQuery == null) return null;
			UriQuery retUriQuery = new UriQuery();
			//foreach (var kvp in uriQuery)
			//	retUriQuery.Add(kvp.Key, kvp.Value);

			foreach (KeyValuePair<string, string> kvp in uriQuery)
			{
				if (kvp.Key == NavigationSettings.FromContext)
				{
					continue;
				}
				retUriQuery.Add(kvp.Key, kvp.Value);
			}
			retUriQuery.Add(NavigationSettings.FromContext, context.ToString());

			return retUriQuery;
		}

        public static void AddDbContextToQuery(UriQuery uriQuery, string dbContext)
        {
            if (uriQuery == null || String.IsNullOrEmpty(dbContext))
                return;
            uriQuery.Add(Common.NavigationSettings.CBIDbContext, dbContext);
        }

        public static UriQuery UriQueryFromNavigationContext(NavigationContext navigationContext)
        {
            UriQuery result = new UriQuery();
            if (navigationContext == null) return result;

            foreach (KeyValuePair<string, string> kvp in navigationContext.Parameters)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }

        public static void NavigateFromForInnerRegions(List<ContentControl> controls, NavigationContext navigationContext)
        {
            if (controls == null) return;
            foreach (ContentControl content in controls)
            {
                UserControl userControl = content.Content as UserControl;
                if (userControl == null) continue;

                INavigationAware view = userControl as INavigationAware;
                if (view != null)
                {
                    view.OnNavigatedFrom(navigationContext);
                }

                INavigationAware viewModel = userControl.DataContext as INavigationAware;
                if (viewModel != null)
                {
                    viewModel.OnNavigatedFrom(navigationContext);
                }
            }
        }

        public static bool IsThereProcessConfigInHistoryContext(IContextCBIRepository contextCBIRepository)
        {
            return contextCBIRepository.GetProcessCBIConfig(CBIContext.History) != null;
        }

        public static bool IsThereCurrentConfigInHistoryContext(IContextCBIRepository contextCBIRepository)
        {
            return contextCBIRepository.GetCurrentCBIConfig(CBIContext.History) != null;
        }

        public static List<IImportModuleInfo> GetImportAdapters(IUnityContainer container,
            IImportAdapterRepository adapterRepository,
            ImportDomainEnum importDomainEnum,
            string customer, string branch, string inventor)
        {
            ImportAdapters adapters = adapterRepository.GetAllowedImportAdapters(customer, branch, inventor, importDomainEnum);

            var containerAdapters = container.ResolveAll<IImportModuleInfo>().Where(r => r.ImportDomainEnum == importDomainEnum).ToList();

            List<IImportModuleInfo> result = containerAdapters.Where(r => adapters.Any(z => z.AdapterCode.ToLower() == r.Name.ToLower())).ToList();

            if (!result.Any(r => r.IsDefault) && containerAdapters.Any(r => r.IsDefault && r.ImportDomainEnum == importDomainEnum))
                result.Add(containerAdapters.First(r => r.IsDefault && r.ImportDomainEnum == importDomainEnum));

            result = result.OrderBy(r=>r.Title).ToList();            

            if (importDomainEnum == ImportDomainEnum.UpdateCatalog)
            {
                IImportModuleInfo empty = result.FirstOrDefault(r => r.Name == Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter);
                if (empty != null)
                {
                    result.Remove(empty);
                }

                empty = containerAdapters.FirstOrDefault(r => r.Name == Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter);
                if (empty != null)
                {
                    result.Insert(0, empty);
                }
            }



			if (importDomainEnum == ImportDomainEnum.ComplexOperation)
            {
				IImportModuleInfo empty = result.FirstOrDefault(r => r.Name == Common.Constants.ComplexAdapterName.ComplexOperationEmptyAdapter);
                if (empty != null)
                {
                    result.Remove(empty);
                }

				empty = containerAdapters.FirstOrDefault(r => r.Name == Common.Constants.ComplexAdapterName.ComplexOperationEmptyAdapter);
                if (empty != null)
                {
                    result.Insert(0, empty);
                }
            }
            return result;
        }

        public static List<IExportPdaModuleInfo> GetExportPdaAdapters(IUnityContainer container,
            IImportAdapterRepository adapterRepository,
            string customer, string branch, string inventor)
        {
            ImportAdapters adapters = adapterRepository.GetAllowedImportAdapters(customer, branch, inventor, ImportDomainEnum.ExportCatalogPDA);

            var containerAdapters = container.ResolveAll<IExportPdaModuleInfo>().ToList();

            List<IExportPdaModuleInfo> result = containerAdapters.Where(r => adapters.Any(z => z.AdapterCode == r.Name)).ToList();

            if (!result.Any(r => r.IsDefault) && containerAdapters.Any(r => r.IsDefault))
                result.Add(containerAdapters.First(r => r.IsDefault));

            return result.OrderBy(r => r.Title).ToList(); 
        }

        public static List<IExportErpModuleInfo> GetExportErpAdapters(IUnityContainer container,
           IImportAdapterRepository adapterRepository,
           string customer, string branch, string inventor)
        {
            ImportAdapters adapters = adapterRepository.GetAllowedImportAdapters(customer, branch, inventor, ImportDomainEnum.ExportCatalogERP);

            var containerAdapters = container.ResolveAll<IExportErpModuleInfo>().ToList();

            List<IExportErpModuleInfo> result = containerAdapters.Where(r => adapters.Any(z => z.AdapterCode.ToLower() == r.Name.ToLower())).ToList();

            if (!result.Any(r => r.IsDefault) && containerAdapters.Any(r => r.IsDefault))
                result.Add(containerAdapters.First(r => r.IsDefault));

            return result.OrderBy(r => r.Title).ToList(); 
        }

	

        public static void AddAuditConfigToQuery(UriQuery query, AuditConfig config)
        {
            if (config == null) return;

            if (!String.IsNullOrEmpty(config.Code))
                query.Add(Common.NavigationSettings.AuditConfigCode, config.Code);

            if (!String.IsNullOrEmpty(config.CustomerCode))
                query.Add(Common.NavigationSettings.AuditConfigCustomer, config.CustomerCode);

            if (!String.IsNullOrEmpty(config.BranchCode))
                query.Add(Common.NavigationSettings.AuditConfigBranch, config.BranchCode);

            if (!String.IsNullOrEmpty(config.InventorCode))
                query.Add(Common.NavigationSettings.AuditConfigInventor, config.InventorCode);
        }

        public static void SetCurrentAuditConfig(NavigationContext navigationContext, IContextCBIRepository repo)
        {
            CBIContext? context = Utils.CBIContextFromNavigationParameters(navigationContext);
            if (context == null)
            {
                throw new InvalidOperationException();
            }

            string auditConfigCode = String.Empty;
            string auditConfigCustomer = String.Empty;
            string auditConfigBranch = String.Empty;
            string auditConfigInventor = String.Empty;

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigCode))
                auditConfigCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigCode).Value;

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigCustomer))
                auditConfigCustomer = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigCustomer).Value;

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigBranch))
                auditConfigBranch = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigBranch).Value;

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AuditConfigInventor))
                auditConfigInventor = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.AuditConfigInventor).Value;

            if (context == CBIContext.History &&
                String.IsNullOrEmpty(auditConfigCustomer) &&
                String.IsNullOrEmpty(auditConfigBranch) &&
                String.IsNullOrEmpty(auditConfigInventor))
            {
                throw new InvalidOperationException(); //for testing purposes                
            }

            AuditConfig ac = new AuditConfig()
            {
                Code = auditConfigCode,
                CustomerCode = auditConfigCustomer,
                BranchCode = auditConfigBranch,
                InventorCode = auditConfigInventor,
            };

            repo.SetCurrentCBIConfig(context ?? CBIContext.History, ac);
        }

        public static T GetViewModelFromRegion<T>(string regionName, IRegionManager regionManager) where T : class
        {
            if (regionManager.Regions.Any(r => r.Name == regionName))
            {
                var region = regionManager.Regions[regionName];
                object view = region.ActiveViews.FirstOrDefault();
                if (view != null)
                {
                    UserControl uc = view as UserControl;
                    if (uc != null)
                    {
                        return uc.DataContext as T;
                    }
                }
            }

            return null;
        }

        public static void AddSelectParamsToQuery(UriQuery query, SelectParams sp)
        {
            if (sp != null)
            {
                sp.IsEnablePaging = false;
                string p = UtilsConvert.Serialize(sp);
                query.Add(Common.NavigationSettings.SelectParams, p);
            }
        }

        public static SelectParams GetSelectParamsFromNavigationContext(NavigationContext navigationContext)
        {
            try
            {
                if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.SelectParams))
                {
                    string p = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.SelectParams).Value;
                    if (!String.IsNullOrEmpty(p))
                    {
                        return UtilsConvert.Deserialize<SelectParams>(p);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("GetSelectParamsFromNavigationContext", exc);
            }

            return null;
        }

        public static void RunOnUI(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (action != null)
                Application.Current.Dispatcher.Invoke(action, priority);
        }

		public static void RunOnUIApplicationIdle(Action action, DispatcherPriority priority = DispatcherPriority.ApplicationIdle)
		{
			if (action != null)
				Application.Current.Dispatcher.Invoke(action, priority);
		}

        public static void RunOnUIAsync(Action action)
        {
            if (action != null)
                Application.Current.Dispatcher.BeginInvoke(action);
        }

        public static void SetCursor(bool isWait = true)
        {
            Mouse.OverrideCursor = isWait ? Cursors.Wait : null;
        }

        public static void InventorChangedGlobalAction(
            IUnityContainer container,
            CBIContext context,
            string dbPath
            )
        {
            _logger.Trace("InventorChangedGlobalAction");

            Task.Factory.StartNew(() =>
                                      {
                                          IContextCBIRepository repo = container.Resolve<IContextCBIRepository>();
                                          IIniFileInventor iniFileInventor = container.Resolve<IIniFileInventor>();
                                          IDBSettings dbSettings = container.Resolve<IDBSettings>();
                                          IServiceLocator serviceLocator = container.Resolve<IServiceLocator>();
                                          ICustomerConfigRepository customerConfigRepository = container.Resolve<ICustomerConfigRepository>();
										  IUserSettingsManager userSettingsManager = container.Resolve<IUserSettingsManager>();

                                          if (repo == null || iniFileInventor == null || dbSettings == null || serviceLocator == null || customerConfigRepository == null)
                                              return;

                                          InventorChangedGlobalActionParamsIni(repo, context, iniFileInventor, dbSettings, dbPath);
										  InventorChangedGlobalActionConfigIni(serviceLocator, repo, context, customerConfigRepository, userSettingsManager, iniFileInventor);
									  }).LogTaskFactoryExceptions("InventorChangedGlobalAction");
        }

        private static void InventorChangedGlobalActionParamsIni(IContextCBIRepository repo, CBIContext context, IIniFileInventor iniFileInventor, IDBSettings dbSettings, string dbPath)
        {
            try
            {
                AuditConfig ac = repo.GetCurrentCBIConfig(context, true);
				if (ac == null) return;

                Customer customer = repo.GetCustomerByCode(ac.CustomerCode);
                Branch branch = repo.GetBranchByCode(ac.BranchCode);
                Inventor inventor = repo.GetInventorByCode(ac.InventorCode);

                if (customer == null || branch == null || inventor == null) return;

                FileInfo fi = new FileInfo(dbSettings.BuildCount4UDBFilePath(dbPath));
                string sdfPath = fi.FullName;

                fi = new FileInfo(repo.GetExportToPDAFolderPath(inventor, true));
                string exprtToPdaPath = fi.FullName;

                string inDataPath = repo.GetImportFolderPath(inventor).Trim('\\') + @"\" + FileSystem.inData;

                if (!Directory.Exists(inDataPath))
                    Directory.CreateDirectory(inDataPath);

                IniFileInventorData iniData = new IniFileInventorData();
                iniData.CustomerCode = customer.Code;
                iniData.CustomerName = customer.Name;

                iniData.BranchCode = branch.Code;
                iniData.BranchName = branch.Name;
                iniData.BranchCodeLocal = branch.BranchCodeLocal;
                iniData.BranchCodeERP = branch.BranchCodeERP;

                iniData.InventorCode = inventor.Code;
                iniData.SDFPath = sdfPath;
                iniData.InDataFolderPath = inDataPath;
                iniData.ExportToPDAPath = exprtToPdaPath;
				iniData.ExportToERPPath = UtilsPath.ExportErpFolder(dbSettings, "Inventor", inventor.Code);
                iniData.ProgramType = @"OFFLINE";

                iniFileInventor.Save(iniData);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("InventorChangedGlobalAction", exc);
            }
        }

		
        private static void InventorChangedGlobalActionConfigIni(IServiceLocator serviceLocator,
            IContextCBIRepository repo,
            CBIContext context,
            ICustomerConfigRepository customerConfigRepository,
			IUserSettingsManager userSettingsManager,
            IIniFileInventor iniFileInventor)
        {
            try
			{
				AuditConfig ac = repo.GetCurrentCBIConfig(context, true);
				if (ac == null) return;

				Customer customer = repo.GetCustomerByCode(ac.CustomerCode);
				Branch branch = repo.GetBranchByCode(ac.BranchCode);
				Inventor inventor = repo.GetInventorByCode(ac.InventorCode);
  //630
//				[1:04:37] Eran Harel: in config.ini - store = branch code
//[1:04:53] Eran Harel: store number = branch code local
//[1:04:59] Eran Harel: store name = branch name
//[1:05:10] Marina Reva: ok If no branch?
//[1:05:17] Eran Harel: store number =000
//. store name = customer name (in case no branch)
				string customerCode = customer == null ? "000" : customer.Code;
				string customerName = customer == null ? "000" : customer.Name;

				string branchCodeLocal = "000";
				string branchName = "000";
				if (customer != null)
				{
					branchName = customerName;
				}
				if (branch != null)
				{
					branchCodeLocal = branch.BranchCodeLocal;
					branchName = branch.Name;
				}

				string inventorCode = inventor == null ? "" : inventor.Code;

				//	 ======================== for PdaHt630 =================================
				// for	ExportPdaAdapterName.ExportHT630Adapter	ganerate	  "config.ini"
				{
					IExportProvider provider1 = serviceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCustomerConfigToFileProvider.ToString());
					provider1.Parms.Clear();

					ExportCommandInfo infoHT630 = 
						UtilsExport.GetExportPdaCommandInfoDefaultData(ExportPdaAdapterName.ExportHT630Adapter, userSettingsManager);
		
					if (customer != null)
					{
					ExportCommandInfo infoHT630Customer =
						UtilsExport.GetExportPdaCustomerData(customerConfigRepository, customer, infoHT630, ExportPdaAdapterName.ExportHT630Adapter);
						infoHT630 = infoHT630Customer;
					}

					if (infoHT630 != null)
					{
						provider1.Parms[ImportProviderParmEnum.Hash] =  infoHT630.Hash;
						provider1.Parms[ImportProviderParmEnum.FileType] =  infoHT630.FileType;
						provider1.Parms[ImportProviderParmEnum.QType] =  infoHT630.QType;
						provider1.Parms[ImportProviderParmEnum.Password] =  infoHT630.Password;
						provider1.Parms[ImportProviderParmEnum.UseAlphaKey] =  infoHT630.UseAlphaKey;
						provider1.Parms[ImportProviderParmEnum.ClientId] =  infoHT630.ClientId;
						provider1.Parms[ImportProviderParmEnum.NewItem] =  infoHT630.NewItem;
						provider1.Parms[ImportProviderParmEnum.HTcalculateLookUp] =  infoHT630.HTcalculateLookUp;
						provider1.Parms[ImportProviderParmEnum.SearchDef] =  infoHT630.SearchDef;
						provider1.Parms[ImportProviderParmEnum.MaxLen] = infoHT630.MaxLen; //16
					}

					provider1.Parms[ImportProviderParmEnum.CustomerCode] = customerCode;
					provider1.Parms[ImportProviderParmEnum.CustomerName] =  customerName;
					provider1.Parms[ImportProviderParmEnum.StoreNumber] = branchCodeLocal;
					provider1.Parms[ImportProviderParmEnum.StoreName] = branchName;

					string path = iniFileInventor.BuildParamsFolderPath();
					provider1.ToPathFile = Path.Combine(path, "config.ini");

					CancellationTokenSource cts = new CancellationTokenSource();
					provider1.Parms.AddCancellationUpdate(r => { ; }, cts.Token);

					provider1.Export();
				}

	
				// ========================== for MIS ============================================
				// for	ExportPdaAdapterName.ExportPdaMISAdapter	ganerate	  "PreSettings.ini"
				//{
				//	IExportProvider provider2 = serviceLocator.GetInstance<IExportProvider>(ExportProviderEnum.ExportCustomerMISConfigToFileProvider.ToString());

				//	provider2.Parms.Clear();

				//	ExportCommandInfo infoMIS =
				//		UtilsExport.GetExportPdaCommandInfoDefaultData(ExportPdaAdapterName.ExportPdaMISAdapter, userSettingsManager);
					
				//	if (customer != null)
				//	{
				//	ExportCommandInfo infoMISCustomer =
				//		UtilsExport.GetExportPdaCustomerData(customerConfigRepository, customer, infoMIS, ExportPdaAdapterName.ExportPdaMISAdapter);
				//		infoMIS = infoMISCustomer;
				//	}
		
				//	if (infoMIS != null)
				//	{
				//		provider2.Parms[ImportProviderParmEnum.QType] = infoMIS.QType;
				//		provider2.Parms[ImportProviderParmEnum.Password] =  infoMIS.Password;
				//		provider2.Parms[ImportProviderParmEnum.NewItemBool] =  infoMIS.NewItemBool;
				//		provider2.Parms[ImportProviderParmEnum.MaxLen] =  infoMIS.MaxLen;
				//		provider2.Parms[ImportProviderParmEnum.LookUpEXE] =  infoMIS.LookUpEXE;
				//		provider2.Parms[ImportProviderParmEnum.HTcalculateLookUp] =  infoMIS.HTcalculateLookUp;
				//		provider2.Parms[ImportProviderParmEnum.AddNewLocation] =  infoMIS.AddNewLocation;
				//		provider2.Parms[ImportProviderParmEnum.AddExtraInputValueSelectFromBatchListForm] = infoMIS.AddExtraInputValueSelectFromBatchListForm;
				//		provider2.Parms[ImportProviderParmEnum.AllowNewValueFromBatchListForm] = infoMIS.AllowNewValueFromBatchListForm;
				//		provider2.Parms[ImportProviderParmEnum.SearchIfExistInBatchList] = infoMIS.SearchIfExistInBatchList;
				//		provider2.Parms[ImportProviderParmEnum.AllowMinusQuantity] = infoMIS.AllowMinusQuantity;
				//		provider2.Parms[ImportProviderParmEnum.FractionCalculate] = infoMIS.FractionCalculate;
				//		provider2.Parms[ImportProviderParmEnum.PartialQuantity] = infoMIS.PartialQuantity;
				//		provider2.Parms[ImportProviderParmEnum.Host1] = infoMIS.Host1;
				//		provider2.Parms[ImportProviderParmEnum.Host2] = infoMIS.Host2;
				//		provider2.Parms[ImportProviderParmEnum.Retry] = infoMIS.Retry;
				//		provider2.Parms[ImportProviderParmEnum.SameBarcodeInLocation] = infoMIS.SameBarcodeInLocation;
				//		provider2.Parms[ImportProviderParmEnum.Timeout] = infoMIS.Timeout;
				//		provider2.Parms[ImportProviderParmEnum.DefaultHost] = infoMIS.DefaultHost;
				//		provider2.Parms[ImportProviderParmEnum.AllowZeroQuantity] =  infoMIS.AllowZeroQuantity;
				//		provider2.Parms[ImportProviderParmEnum.MaxQuantity] =  infoMIS.MaxQuantity;
				//		provider2.Parms[ImportProviderParmEnum.LastSync] =  infoMIS.LastSync;
				//		provider2.Parms[ImportProviderParmEnum.InvertWords] =  infoMIS.IsInvertWords.BoolToStringParam();
				//		provider2.Parms[ImportProviderParmEnum.InvertLetters] =  infoMIS.IsInvertLetters.BoolToStringParam();
				//		provider2.Parms[ImportProviderParmEnum.InvertWordsConfig] =  infoMIS.IsInvertWordsConfig.BoolToStringParam();
				//		provider2.Parms[ImportProviderParmEnum.InvertLettersConfig] =  infoMIS.IsInvertLettersConfig;
				//		provider2.Parms[ImportProviderParmEnum.CutAfterInvert] =  infoMIS.IsCutAfterInvert;
				//		provider2.Parms[ImportProviderParmEnum.SearchDef] = infoMIS.SearchDef;
				//		provider2.Parms[ImportProviderParmEnum.ConfirmNewLocation] = infoMIS.ConfirmNewLocation;
				//		provider2.Parms[ImportProviderParmEnum.ConfirmNewItem] = infoMIS.ConfirmNewItem;
				//		provider2.Parms[ImportProviderParmEnum.AutoSendData] = infoMIS.AutoSendData;
				//		provider2.Parms[ImportProviderParmEnum.AllowQuantityFraction] = infoMIS.AllowQuantityFraction;
				//		provider2.Parms[ImportProviderParmEnum.AddExtraInputValue] = infoMIS.AddExtraInputValue;
				//		provider2.Parms[ImportProviderParmEnum.AddExtraInputValueHeaderName] = infoMIS.AddExtraInputValueHeaderName;
					
				//	}

				//		provider2.Parms[ImportProviderParmEnum.BranchCode] =  branchCodeLocal;
				//		provider2.Parms[ImportProviderParmEnum.BranchName] = branchName;
				//		provider2.Parms[ImportProviderParmEnum.CustomerCode] = customerCode;
				//		provider2.Parms[ImportProviderParmEnum.CustomerName] = customerName;
				//		provider2.Parms[ImportProviderParmEnum.InventorCode] = inventorCode;

				//	string path2 = iniFileInventor.BuildParamsFolderPath();
				//	provider2.ToPathFile = Path.Combine(path2, "PreSettings.ini");

				//	CancellationTokenSource cts2 = new CancellationTokenSource();
				//	provider2.Parms.AddCancellationUpdate(r => { ; }, cts2.Token);

				//	provider2.Export();

				//	//===== copy file ==========

				//	string processToHTFolder = infoMIS.LookUpEXE.Trim('\\') + @"\toHT";

				//	if (Directory.Exists(processToHTFolder) == false)
				//	{
				//		try { Directory.CreateDirectory(processToHTFolder); }
				//		catch { }
				//	}
				//	else
				//	{
				//		System.IO.DirectoryInfo dirInfo = new DirectoryInfo(processToHTFolder);
				//		foreach (FileInfo file in dirInfo.GetFiles())
				//		{
				//			file.Delete();
				//		}
				//	}

				//	if (Directory.Exists(processToHTFolder) == true)
				//	{
				//		string toHTPathFile2 = processToHTFolder + @"\" + @"PreSettings.ini";
				//		CopyFile(provider2.ToPathFile, toHTPathFile2);
				//	}
				//}

			}
            catch (Exception exc)
            {
                _logger.ErrorException("InventorChangedGlobalActionConfigIni", exc);
            }
        }

        public static void OpenFolderInExplorer(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

		//"ftp://ftp.boscom.com//mInv?username=myusername&password=mypassword"
		public static void OpenFtpFolderInExplorer(string host, string folder)
		{
			string url = host + @"/" + folder.Trim('/');
			System.Diagnostics.Process.Start("explorer.exe", url);
		}

        public static void OpenFileInExplorer(string path)
        {
            Process explorerWindowProcess = new Process();

            explorerWindowProcess.StartInfo.FileName = "explorer.exe";
            explorerWindowProcess.StartInfo.Arguments = "/select,\"" + path + "\"";

            explorerWindowProcess.Start();
        }

		public static void CopyFile(string fromFilePath, string toFilePath)
		{
			if (File.Exists(fromFilePath) == false) return;
			try
			{
				if (File.Exists(toFilePath) == true)
				{
					File.Delete(toFilePath);
				}
				string folder = Path.GetDirectoryName(toFilePath);
				if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
				File.Copy(fromFilePath, toFilePath);
				//GC.Collect();
			}
			catch { }
		}

		public static string BoolToStringParam(this bool param)
		{
			if (param == true) return "1";
			if (param == false) return "0";
			else return "0";
		}

		public static string CutLength1(this string stringValue, int MaxLength)
		{
			if (string.IsNullOrWhiteSpace(stringValue) == true) return "";
			stringValue = stringValue.Trim();
			if (stringValue.Length <= MaxLength) return stringValue;
			else return stringValue.Substring(0, MaxLength);
		}
    }
}