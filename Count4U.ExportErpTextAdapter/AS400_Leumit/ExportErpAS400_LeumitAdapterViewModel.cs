using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Count4U.Common;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.ExportErpTextAdapter.AS400_Leumit
{
    public class ExportErpAS400_LeumitAdapterViewModel : ExportErpMakatViewModel
    {
		
        public ExportErpAS400_LeumitAdapterViewModel(IContextCBIRepository contextCbiRepository,
                                               ILog logImport,
                                               IServiceLocator serviceLocator,
                                               IUserSettingsManager userSettingsManager,
                                               IDBSettings dbSettings)
            : base(contextCbiRepository, logImport, serviceLocator, userSettingsManager, dbSettings)
        {
            this._makatOriginal = true;
        }        

        protected override string GetPathToIniFile()
        {
            return Path.Combine(base.GetModulesFolderPath(), "ExportErpAS400_LeumitAdapter.ini");
        }

        protected override void InitDefault()
        {
			base.ParmsDictionary.Clear();
			if (base.CurrentCustomer != null)
			{
				base.AddParamsInDictionary(base.CurrentCustomer.ImportCatalogAdapterParms);
			}

			if (base.CurrentBranch != null)
			{
				base.AddParamsInDictionary(base.CurrentBranch.ImportCatalogAdapterParms);
			}

            //init GUI
            base.Encoding = System.Text.Encoding.GetEncoding("windows-1255");
            base.IsInvertLetters = false;
            base.IsInvertWords = false;

            //init Provider Parms
        }

        protected override void InitFromIniFile()
        {
            Dictionary<ImportProviderParmEnum, string> iniData = base.GetIniData();
            //init GUI
            //init Provider Parms
        }

		protected override void InitFromConfig(ExportErpCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = this.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						//string exportPath = XDocumentConfigRepository.GetExportPath(this, configXDoc);
					}
					catch (Exception exp)
					{
						base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
					}
				}
				else
				{
					base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
				}
			}
		}

		//protected override void InitFromConfig(ExportErpCommandInfo info)
		//{
		//	if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
		//	{
		//		string configPath = base.GetXDocumentConfigPath(ref info);
		//		XDocument configXDoc = new XDocument();
		//		if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
		//		{
		//			try
		//			{
		//				configXDoc = XDocument.Load(configPath);
		//				XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

		//				string exportPath = XDocumentConfigRepository.GetExportPath(this, configXDoc);

		 //base.IsWithoutCatalog
		//this._makatOriginal
		//RunExportInner(info);
		//info
		//			}
		//			catch (Exception exp)
		//			{
		//				base.LogImport.Add(MessageTypeEnum.Error, String.Format("Error load file[ {0} ] : {1}", configPath, exp.Message));
		//			}
		//		}
		//		else
		//		{
		//			base.LogImport.Add(MessageTypeEnum.Warning, String.Format("Warning load file[ {0} ]  not find", configPath));
		//		}
		//	}
		//}

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
           
        }

        protected override void RunExportInner(ExportErpCommandInfo info)
        {
            string branchCodeErp = string.Empty;
			string branchCodeErp1 = string.Empty;
			if (base.CurrentBranch != null)
			{
				branchCodeErp1 = base.CurrentBranch.BranchCodeERP;
				branchCodeErp = base.CurrentBranch.BranchCodeERP.PadLeft(8,'0');
			}
            string inventorDate = string.Empty;
			string inventorDate1 = string.Empty;
            if (base.CurrentInventor != null)
            {
                DateTime dt = base.CurrentInventor.InventorDate;
				inventorDate = dt.ToString("yyyyMMdd"); //the FORMAT need to be : YYYYMMDD
				DateTime dt1 = base.CurrentInventor.InventorDate;
				inventorDate1 = dt.ToString("ddMMyyyy");
				 
            }

            //FILE1
            string fileName1 = String.Format("FR{0}", branchCodeErp1); //result file only name without extension            
            string fileNameWithExtension1 = String.Format("{0}.csv", fileName1);
            string fullPath1 = Path.Combine(base.PathToExportErp, fileNameWithExtension1);

			IExportERPProvider provider = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400LeumitERPFileProvider.ToString());
            provider.Parms.Clear();
            provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider.FromPathDB = base.GetDbPath;
            base.IsFromCatalog = true;
            base.IsWithoutCatalog = true;
            provider.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
            provider.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
            provider.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
            provider.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
			provider.Parms[ImportProviderParmEnum.ExcludeNotExistingInCatalog] = info.IsExcludeNotExistingInCatalog ? "1" : String.Empty;
			
            provider.ToPathFile = fullPath1;
			provider.Export(info.IsFull,  info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            //FILE2
			string fileName2 = String.Format("NF_INV_COUNT{0}_{1}", branchCodeErp1, inventorDate1); //result file only name with extension            
            string fileNameWithExtension2 = String.Format("{0}.dat", fileName2);
            string fullPath2 = Path.Combine(base.PathToExportErp, fileNameWithExtension2);

			IExportERPProvider provider1 = base.ServiceLocator.GetInstance<IExportERPProvider>(ExportProviderEnum.ExportInventProductAS400LeumitERPFileProvider1.ToString());
            provider1.Parms.Clear();
            provider1.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            provider1.FromPathDB = base.GetDbPath;
            base.IsFromCatalog = false;
            base.IsWithoutCatalog = true;
            provider1.Parms[ImportProviderParmEnum.ERPNum] = branchCodeErp;
            provider1.Parms[ImportProviderParmEnum.InventorDate] = inventorDate;
            provider1.Parms[ImportProviderParmEnum.MakatWithoutMask] = this._makatOriginal ? "1" : "";
            provider1.Parms[ImportProviderParmEnum.Encoding] = base.Encoding;
            provider1.Parms[ImportProviderParmEnum.InvertLetters] = base.IsInvertLetters ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.InvertWords] = base.IsInvertWords ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.FromCatalog] = base.IsFromCatalog ? "1" : String.Empty;
            provider1.Parms[ImportProviderParmEnum.WithoutCatalog] = base.IsWithoutCatalog ? "1" : String.Empty;
            provider1.ToPathFile = fullPath2;
			provider1.Export(info.IsFull, info.IsFilterByLocations, info.LocationCodeList, info.IsFilterByIturs, info.IturCodeList);


            SaveFileLog(fileName1 + "_Log");
        }
    }
}