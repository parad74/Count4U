using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.GenerationReport;
using System.Collections.Generic;
using Count4U.Model.Main;
using Count4U.Model.Interface.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters
{
    public class AdapterLinkScriptSaveViewModel : ScriptSaveBaseViewModel
    {
        private readonly IServiceLocator _serviceLocator;

        public AdapterLinkScriptSaveViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
            : base(contextCbiRepository, eventAggregator)
        {
            _serviceLocator = serviceLocator;
        }

        protected override void RunScript()
        {
            //INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) 
            //VALUES (N'Any',N'ImportCatalogUnizagAdapter',N'ImportCatalog',null);

            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
            IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
            IImportAdapterRepository importAdapterRepository = this._serviceLocator.GetInstance<IImportAdapterRepository>();
            //List<AllowedReportTemplate> allowedReportTemplates = new List<AllowedReportTemplate>();
            log.Clear();

            ImportAdapters importAdapters = null;
            string sql = @"INSERT INTO [ImportAdapter] ";
            string retSql = "";
            importAdapters = importAdapterRepository.GetImportAdapters();


            if (importAdapters != null)
            {
                foreach (var importAdapter in importAdapters)
                {
                    string code = (String.IsNullOrWhiteSpace(importAdapter.Code) == false) ? importAdapter.Code : String.Empty;
                    string adapterCode = (String.IsNullOrWhiteSpace(importAdapter.AdapterCode) == false) ? importAdapter.AdapterCode : String.Empty;
                    string domainType = (String.IsNullOrWhiteSpace(importAdapter.DomainType) == false) ? importAdapter.DomainType : String.Empty;
					string description = (String.IsNullOrWhiteSpace(importAdapter.Description) == false) ? importAdapter.Description.Replace("'", "''") : String.Empty;
                    string sql1 = sql +
                        @"([Code],[AdapterCode],[DomainType],[Description]) " +
                        @"VALUES " +
                        //	  {0}		{1}				                             	{2}					 {3}	
                        //(N'Any',N'ImportCatalogUnizagAdapter',N'ImportCatalog',null);
                        String.Format("(N'{0}',N'{1}',N'{2}',N'{3}');" + Environment.NewLine,
						code.Trim(), adapterCode.Trim(), domainType.Trim(), description.Trim());

                    retSql = retSql + sql1;
                }
                File.WriteAllText(this._path, retSql);
            }

            this.Log = log.PrintLog();
        }

    }
}