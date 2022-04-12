using System.Threading.Tasks;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.AdapterTemplate;
using Count4U.Common.ViewModel.Adapters;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common;
using System.Xml.Linq;
using System.IO;

namespace Count4U.ImportCatalogAvivRegularAdapter
{
    public class ImportAvivRegularAdapterViewModel : TemplateAdapterOneFileViewModel
    {
        public ImportAvivRegularAdapterViewModel(IServiceLocator serviceLocator,
         IContextCBIRepository contextCBIRepository,
         IEventAggregator eventAggregator,
         IRegionManager regionManager,
         ILog logImport,
         IIniFileParser iniFileParser,
			ITemporaryInventoryRepository temporaryInventoryRepository,
         IUserSettingsManager userSettingsManager) :
			base(serviceLocator, contextCBIRepository, eventAggregator, regionManager, logImport, iniFileParser, temporaryInventoryRepository, userSettingsManager)
        {
			base.ParmsDictionary.Clear();
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

			this._maskViewModel = this.BuildMaskControl("1", base.BuildMaskRegionName());
			if (this._maskViewModel != null)
			{
				if (string.IsNullOrWhiteSpace(this._makatMask) == false)
				{
					this._maskViewModel.MakatMask = this._makatMask;   //init Default
				}
				if (string.IsNullOrWhiteSpace(this._barcodeMask) == false)
				{
					this._maskViewModel.BarcodeMask = this._barcodeMask; //init Default
				}
			}
        }

		protected override void InitFromConfig(ImportCommandInfo info, CBIState state)
		{
			if (state == null) return;
			base.State = state;
			if (info.FromConfigXDoc != ConfigXDocFromEnum.InitWithoutConfig)
			{
				string configPath = base.GetXDocumentConfigPath(ref info);
				XDocument configXDoc = new XDocument();
				if (File.Exists(configPath) == true)	   //если есть сохраненный файла config.xml
				{
					try
					{
						configXDoc = XDocument.Load(configPath);
						XDocumentConfigRepository.InitXDocumentConfig(this, configXDoc);

						string importPath = XDocumentConfigRepository.GetImportPath(this, configXDoc);
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

        protected override void RunImport(ImportCommandInfo info)
        {
            this.Import();
        }      

        protected override void RunClear()
        {
            this.Clear();
        }

        #region Implementation of IImportAdapter

        public override void InitDefault(CBIState state = null)
        {
			if (state != null) base.State = state;
          

			//init GUI
            this.Path = System.IO.Path.GetFullPath(base.GetImportPath().Trim('\\') + @"\" + "items.csv");

			//init Provider Parms
        }

        public override void InitFromIni()
        {
				//init GUI
			//init Provider Parms
			if (System.IO.Path.GetExtension(base.Path) == ".xlsx") base.XlsxFormat = true; else base.XlsxFormat = false;
        }

        public override void Import()
        {
			DateTime updateDateTime = DateTime.Now;
			base.SetModifyDateTimeCurrentDomainObject(updateDateTime);

            //            IImportProvider provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider);
            //            provider.ToPathDB = base.GetDbPath;
            //            provider.FromPathFile = this.Path;
            //provider.Parms.Clear();
            //provider.Parms.AddCancellationUpdate(base.UpdateProgress, base.CancellationToken);
            //            provider.Import();            

            FileLogInfo fileLogInfo = new FileLogInfo();
            fileLogInfo.File = Path;
            base.SaveFileLog(fileLogInfo);
        }

        public override void Clear()
        {
            base.LogImport.Clear();
            //            LogImport provider = this.GetProviderInstance(ImportProviderEnum.ImportCatalogUnizagADOProvider);
            //            provider.ToPathDB = base.GetDbPath;
            //            provider.Clear();
            UpdateLogFromILog();
        }

        #endregion
    }
}