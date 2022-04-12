using System;
using System.Threading;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Extensions;
using Count4U.Model.Main;

namespace Count4U.Common.ViewModel.Adapters.Import
{
    public class ImportCommandInfo
    {
        public bool IsWriteLogToFile { get; set; }
        public Action Callback { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public bool IsInvertLetters { get; set; }
        public bool IsInvertWords { get; set; }
        public bool TryFast { get; set; }
		public XDocument ConfigXDocument { get; set; }
		[NotInludeAttribute]
		public string ConfigXDocumentPath { get; set; }
		public ConfigXDocFromEnum FromConfigXDoc { get; set; }
		[NotInludeAttribute]
		public string AdapterName { get; set; }
		public ImportDomainEnum Mode { get; set; }
		//public Customer CurrentCustomer { get; set; }
		//public Branch CurrentBranch { get; set; }
		//public Inventor CurrentInventor { get; set; }

		public ImportCommandInfo()
		{
			this.AdapterName = "";
			this.Mode = ImportDomainEnum.None;
			this.ConfigXDocument = null;
			this.ConfigXDocumentPath = "";
			this.FromConfigXDoc = ConfigXDocFromEnum.InitWithoutConfig;
		}
    }

	public enum ConfigXDocFromEnum 
	{
		NotUse,
		InitWithoutConfig,
		FromConfigXDocument,
		FromDefaultAdapter,
		FromFullPath,
		FromRootPath,
		FromRootFolderAndCustomer,
		FromRootFolderAndBranch,
		FromRootFolderAndInventor,
		FromFtpCustomer,
		FromFtpBranch,
		FromFtpInventor,
		FromCustomerInData,
		FromBranchInData,
		FromInventorInData,
		//FromDomainObjectInData
	}
}