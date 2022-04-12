using System;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using Count4U.Model;
using System.IO;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask.Script
{
    public class MaskScriptSaveViewModel : ScriptSaveBaseViewModel
    {
        private readonly IServiceLocator _serviceLocator;

        public MaskScriptSaveViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator)
            : base(contextCbiRepository, eventAggregator)
        {
            _serviceLocator = serviceLocator;
        }

        protected override void RunScript()
        {
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
            IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
            log.Clear();
		
			string retSql = "";	 
			object domainObject = base.GetCurrentDomainObject();
			string importMaskTable = "";
			IMaskRepository maskRepository = null;
			if (domainObject is Customer)
			{
				importMaskTable = @"[CustomerMask]";
				maskRepository = this._serviceLocator.GetInstance<IMaskRepository>("CustomerMaskEFRepository");
			}
			else if (domainObject is Branch)
			{
				importMaskTable = @"[BranchMask]";
				maskRepository = this._serviceLocator.GetInstance<IMaskRepository>("BranchMaskEFRepository");
			}
			else if (domainObject is Inventor)
			{
				importMaskTable = @"[InventorMask]";
				maskRepository = this._serviceLocator.GetInstance<IMaskRepository>("InventorMaskEFRepository");
			}
			else return;

			string sql = @"INSERT INTO " + importMaskTable + " ";

			Masks masks = maskRepository.GetMasks();

			if (masks != null)
			{
				foreach (var mask in masks)
				{
					//([Code],[AdapterCode],[FileCode],[BarcodeMask],[MakadMask]) 
					//VALUES (N'94dbe765-e79f-46c9-858d-5a8e7623f780',N'ImportCatalogUnizagAdapter',N'1',N'7290000000000{F}',N'0000000000000{F}');
					string code = (String.IsNullOrWhiteSpace(mask.Code) == false) ? mask.Code : String.Empty;
					string adapterCode = (String.IsNullOrWhiteSpace(mask.AdapterCode) == false) ? mask.AdapterCode : String.Empty;
					string fileCode = (String.IsNullOrWhiteSpace(mask.FileCode) == false) ? mask.FileCode : String.Empty;
					string barcodeMask = (String.IsNullOrWhiteSpace(mask.BarcodeMask) == false) ? mask.BarcodeMask : String.Empty;
					string makatMask = (String.IsNullOrWhiteSpace(mask.MakatMask) == false) ? mask.MakatMask : String.Empty;

					string sql1 = sql +
						@"([Code],[AdapterCode],[FileCode],[BarcodeMask],[MakadMask])  " +
						@"VALUES " +
						//	  {0}		{1}				                             	{2}					 {3}	
						//(N'Any',N'ImportCatalogUnizagAdapter',N'ImportCatalog',null);
						String.Format("(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}');" + Environment.NewLine,
						code.Trim(), adapterCode.Trim(), fileCode.Trim(), barcodeMask.Trim(), makatMask.Trim());

					retSql = retSql + sql1;
				}
				File.WriteAllText(this._path, retSql);
			}

			this.Log = log.PrintLog();
        }

    }
}