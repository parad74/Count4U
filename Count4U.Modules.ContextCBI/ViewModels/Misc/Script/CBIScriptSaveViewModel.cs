using Count4U.Common;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel.Script;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using System;
using System.IO;
using Count4U.Model.Audit;
using System.Text;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Script
{
    public class CBIScriptSaveViewModel : ScriptSaveBaseViewModel
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly INavigationRepository _navigationRepository;

        private enCBIScriptMode _mode;
        private bool _isUseSelectParams;
        private SelectParams _selectParams;

        public CBIScriptSaveViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IServiceLocator serviceLocator,
            INavigationRepository navigationRepository)
            : base(contextCbiRepository, eventAggregator)
        {
            _navigationRepository = navigationRepository;
            this._serviceLocator = serviceLocator;
        }

        public bool IsUseSelectParams
        {
            get { return _isUseSelectParams; }
            set
            {
                _isUseSelectParams = value;
                RaisePropertyChanged(() => IsUseSelectParams);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            this._mode = UtilsConvert.ConvertToEnum<enCBIScriptMode>(navigationContext);
            this._selectParams = UtilsConvert.GetObjectFromNavigation(navigationContext, this._navigationRepository, NavigationObjects.SelectParams, true) as SelectParams;
        }

        protected override void RunScript()
        {
            this.Log = "";
            ILog log = this._serviceLocator.GetInstance<ILog>();
			log.Clear();
            IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			string retSql = "";
			string importTable = "";

			if (this._mode == enCBIScriptMode.Customer)
			{
				importTable = @"[Customer]";
				retSql = InsertIntoCustomer(importTable, this._selectParams);
			}
			else if (this._mode == enCBIScriptMode.Branch)
			{
				importTable = @"[Branch]";
				retSql = InsertIntoBranch(importTable, this._selectParams);
			}
			else if (this._mode == enCBIScriptMode.Inventor)
			{
				importTable = @"[Inventor]";
				retSql = InsertIntoInventor(importTable, this._selectParams);
			}
			else return;
			StreamWriter sw = GetStreamWriter(this._path, Encoding.GetEncoding(1255));
			try
			{
				//File.WriteAllText(this._path, retSql);
				sw.Write(retSql);
			}
			catch (Exception error)
			{
				_logger.ErrorException("RunScript" + error.Message, error);
			}
			finally
			{
					sw.Close();
			}
             this.Log = log.PrintLog();
        }


		protected StreamWriter GetStreamWriter(string toPathFile, Encoding encoding)
		{
			this.DeleteFile(toPathFile);
			string folder = System.IO.Path.GetDirectoryName(toPathFile);
			if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
			StreamWriter sw = new StreamWriter(toPathFile, false, encoding);
			return sw;
		}

		public void DeleteFile(string pathFile)
		{
			if (File.Exists(pathFile) == true)
			{
				try
				{
					File.Delete(pathFile);
				}
				catch (Exception error)
				{
					_logger.ErrorException("DeleteFile" + error.Message, error);
				}
			}
		}

		private string InsertIntoCustomer(string importTable, SelectParams selectParams)
		{
			string retSql = "";
			string sql = @"INSERT INTO " + importTable + " ";
			ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
			Customers customers = customerRepository.GetCustomers(selectParams);

			if (customers != null)
			{
				foreach (var customer in customers)
				{
					//([Code],[AdapterCode],[FileCode],[BarcodeMask],[MakadMask]) 
					//VALUES (N'94dbe765-e79f-46c9-858d-5a8e7623f780',N'ImportCatalogUnizagAdapter',N'1',N'7290000000000{F}',N'0000000000000{F}');
					string code = (String.IsNullOrWhiteSpace(customer.Code) == false) ? customer.Code : String.Empty;
					string name = (String.IsNullOrWhiteSpace(customer.Name) == false) ? customer.Name.Replace("'", "''") : String.Empty;
					string description = (String.IsNullOrWhiteSpace(customer.Description) == false) ? customer.Description.Replace("'", "''") : String.Empty;
					string phone = (String.IsNullOrWhiteSpace(customer.Phone) == false) ? customer.Phone : String.Empty;
					string address = (String.IsNullOrWhiteSpace(customer.Address) == false) ? customer.Address.Replace("'", "''") : String.Empty;
					string contactPerson = (String.IsNullOrWhiteSpace(customer.ContactPerson) == false) ? customer.ContactPerson.Replace("'", "''") : String.Empty;
					string mail = (String.IsNullOrWhiteSpace(customer.Mail) == false) ? customer.Mail.Replace("'", "''") : String.Empty;
					string fax = (String.IsNullOrWhiteSpace(customer.Fax) == false) ? customer.Fax : String.Empty;
					string logoPath = (String.IsNullOrWhiteSpace(customer.LogoPath) == false) ? customer.LogoPath : String.Empty;
					string importCatalogProviderCode = (String.IsNullOrWhiteSpace(customer.ImportCatalogProviderCode) == false) ? customer.ImportCatalogProviderCode : String.Empty;
					string DBPath = (String.IsNullOrWhiteSpace(customer.DBPath) == false) ? customer.DBPath : String.Empty;
					
					// VALUES (N'Customer2',N'CustomerDescription2',N'0879123125',N'Mana street 6/2 \r\n Tel-Aviv, 42444\r\n
					//',N'Contact2',N'B77@A898.tt',null,N'CustomerCode2',N'',N'12_388.jpg',null,N'ImportCatalogGazitVerifoneAdapter',N'ImportIturDefaultAdapter',N'ImportLocationDefaultAdapter',N'ImportPdaDefaultAdapter',N'CustomerCode2',null,null,null,null,null,null,N'ExportHT630Adapter',null);

					string sql1 = sql +
						//@"([Name],[Description],[Phone],[Address],[ContactPerson],[Mail],[Logo],[Code],[Fax],[LogoPath],[Tag],[ImportCatalogProviderCode],[ImportIturProviderCode],[ImportLocationProviderCode],[ImportPDAProviderCode],[DBPath],[ImportCatalogAdapterParms],[ImportIturAdapterParms],[ImportLocationAdapterParms],[ImportPDAAdapterParms],[ReportPath],[MaskCode],[ExportCatalogAdapterCode],[ExportIturAdapterCode])   " +
						"([Code],[Name],[Description],[Phone],[Address],[ContactPerson],[Mail]," +		 //0-6
						"[Fax],[LogoPath],[ImportCatalogProviderCode],[DBPath])   " +	  //7-10
						"VALUES " +																//logo	 code
						String.Format("(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}', N'{5}',N'{6}', N'{7}',N'{8}', N'{9}', N'{10}' );" + Environment.NewLine,
						code.Trim(), name.Trim(), description.Trim(), phone.Trim(), address.Trim(), contactPerson.Trim(), mail.Trim(),
						fax.Trim(), logoPath.Trim(), importCatalogProviderCode.Trim(), DBPath.Trim());

					retSql = retSql + sql1;
				}
			}
			return retSql;
		}

		private string InsertIntoBranch(string importTable, SelectParams selectParams)
		{
			string retSql = "";
			string sql = @"INSERT INTO " + importTable + " ";
			IBranchRepository branchRepository = this._serviceLocator.GetInstance<IBranchRepository>();
			Branches branches = branchRepository.GetBranches(selectParams);

			if (branches != null)
			{
				foreach (var branch in branches)
				{
					//([Code],[AdapterCode],[FileCode],[BarcodeMask],[MakadMask]) 
					//VALUES (N'94dbe765-e79f-46c9-858d-5a8e7623f780',N'ImportCatalogUnizagAdapter',N'1',N'7290000000000{F}',N'0000000000000{F}');
					string code = (String.IsNullOrWhiteSpace(branch.Code) == false) ? branch.Code : String.Empty;
					string name = (String.IsNullOrWhiteSpace(branch.Name) == false) ? branch.Name.Replace("'", "''") : String.Empty;
					string description = "RepairBranch";
					string phone = (String.IsNullOrWhiteSpace(branch.Phone) == false) ? branch.Phone : String.Empty;
					string address = (String.IsNullOrWhiteSpace(branch.Address) == false) ? branch.Address.Replace("'", "''") : String.Empty;
					string contactPerson = (String.IsNullOrWhiteSpace(branch.ContactPerson) == false) ? branch.ContactPerson.Replace("'", "''") : String.Empty;
					string mail = (String.IsNullOrWhiteSpace(branch.Mail) == false) ? branch.Mail.Replace("'", "''") : String.Empty;
					string fax = (String.IsNullOrWhiteSpace(branch.Fax) == false) ? branch.Fax : String.Empty;
					string importCatalogProviderCode = (String.IsNullOrWhiteSpace(branch.ImportCatalogProviderCode) == false) ? branch.ImportCatalogProviderCode : String.Empty;

	
					string DBPath = (String.IsNullOrWhiteSpace(branch.DBPath) == false) ? branch.DBPath : String.Empty;
					string customerCode = (String.IsNullOrWhiteSpace(branch.CustomerCode) == false) ? branch.CustomerCode : String.Empty;
					string branchCodeERP = (String.IsNullOrWhiteSpace(branch.BranchCodeERP) == false) ? branch.BranchCodeERP : String.Empty;
					string branchCodeLocal = (String.IsNullOrWhiteSpace(branch.BranchCodeLocal) == false) ? branch.BranchCodeLocal : String.Empty;  //12

					string importIturProviderCode = (String.IsNullOrWhiteSpace(branch.ImportIturProviderCode) == false) ? branch.ImportIturProviderCode : String.Empty;
					string importLocationProviderCode = (String.IsNullOrWhiteSpace(branch.ImportLocationProviderCode) == false) ? branch.ImportLocationProviderCode : String.Empty;
					string importPDAProviderCode = (String.IsNullOrWhiteSpace(branch.ImportPDAProviderCode) == false) ? branch.ImportPDAProviderCode : String.Empty;
					string importLocationAdapterParms = (String.IsNullOrWhiteSpace(branch.ImportLocationAdapterParms) == false) ? branch.ImportLocationAdapterParms : String.Empty;
					string importSectionAdapterCode = (String.IsNullOrWhiteSpace(branch.ImportSectionAdapterCode) == false) ? branch.ImportSectionAdapterCode : String.Empty;
					string updateCatalogAdapterCode = (String.IsNullOrWhiteSpace(branch.UpdateCatalogAdapterCode) == false) ? branch.UpdateCatalogAdapterCode : String.Empty;
					string exportERPAdapterCode = (String.IsNullOrWhiteSpace(branch.ExportERPAdapterCode) == false) ? branch.ExportERPAdapterCode : String.Empty;
					string importSupplierAdapterCode = (String.IsNullOrWhiteSpace(branch.ImportSupplierAdapterCode) == false) ? branch.ImportSupplierAdapterCode : String.Empty;
					string priceCode = (String.IsNullOrWhiteSpace(branch.PriceCode) == false) ? branch.PriceCode : String.Empty;  //21


//INSERT INTO [Branch] ([Name],[Description],[Address],[Phone],[Fax],[ContactPerson],[Mail],[Code],[CustomerCode],[ID],[ImportCatalogProviderCode],[ImportIturProviderCode],[ImportLocationProviderCode],[ImportPDAProviderCode],[DBPath],[ImportCatalogAdapterParms],[ImportIturAdapterParms],[ImportLocationAdapterParms],[ImportPDAAdapterParms],[BranchCodeLocal],[BranchCodeERP],[MaskCode],[ReportPath],[ExportCatalogAdapterCode],[ExportIturAdapterCode]) VALUES (N'Branch1.1',N'BranchDescription1.1',N'Mana street 5/1 \r\n Tel-Aviv, 42444\r\n
//',N'4441233331',N'3333444121',N'Person1',N'',N'BranchCode1_1',N'CustomerCode1',16,N'ImportCatalogDefaultAdapter',N'ImportIturDefaultAdapter',N'ImportLocationDefaultAdapter',null,N'BranchCode1_1',null,null,null,null,N'LocalBranch1',N'1111',null,null,null,null);
					string sql1 = sql +
						//@"([Name],[Description],[Phone],[Address],[ContactPerson],[Mail],[Logo],[Code],[Fax],[LogoPath],[Tag],[ImportCatalogProviderCode],[ImportIturProviderCode],[ImportLocationProviderCode],[ImportPDAProviderCode],[DBPath],[ImportCatalogAdapterParms],[ImportIturAdapterParms],[ImportLocationAdapterParms],[ImportPDAAdapterParms],[ReportPath],[MaskCode],[ExportCatalogAdapterCode],[ExportIturAdapterCode])   " +
						"([Code],[Name],[Description],[Phone],[Address],[ContactPerson],[Mail],[Fax],[ImportCatalogProviderCode],[DBPath], [BranchCodeLocal],[BranchCodeERP],[CustomerCode],				[ImportIturProviderCode], [ImportLocationProviderCode],	[ImportPDAProviderCode],	[ImportLocationAdapterParms], [ImportSectionAdapterCode], [UpdateCatalogAdapterCode], [ExportERPAdapterCode], [ImportSupplierAdapterCode],	[PriceCode])   " +
						"VALUES " +																//logo	 code
						String.Format("(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}', N'{5}',N'{6}', N'{7}',N'{8}', N'{9}', N'{10}',N'{11}', N'{12}', N'{13}', N'{14}', N'{15}', N'{16}', N'{17}', N'{18}', N'{19}', N'{20}', N'{21}');" + Environment.NewLine,
						code.Trim(), name.Trim(), description.Trim(), phone.Trim(), address.Trim(), contactPerson.Trim(), mail.Trim(), fax.Trim(), importCatalogProviderCode.Trim(), DBPath.Trim(), branchCodeLocal.Trim(), branchCodeERP.Trim(), customerCode.Trim(),
						importIturProviderCode.Trim(), importLocationProviderCode.Trim(),	importPDAProviderCode.Trim(),	importLocationAdapterParms.Trim(), importSectionAdapterCode.Trim(), updateCatalogAdapterCode.Trim(), exportERPAdapterCode.Trim(), importSupplierAdapterCode.Trim(),	priceCode.Trim());

					retSql = retSql + sql1;
				}
			}
			return retSql;
		}

		private string InsertIntoInventor(string importTable, SelectParams selectParams)
		{
			string retSql = "";
			string sql = @"INSERT INTO " + importTable + " ";
			IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
			Inventors inventors = inventorRepository.GetInventors(selectParams);

			if (inventors != null)
			{
				foreach (var inventor in inventors)
				{
					//([Code],[AdapterCode],[FileCode],[BarcodeMask],[MakadMask]) 
					//VALUES (N'94dbe765-e79f-46c9-858d-5a8e7623f780',N'ImportCatalogUnizagAdapter',N'1',N'7290000000000{F}',N'0000000000000{F}');
					string code = (String.IsNullOrWhiteSpace(inventor.Code) == false) ? inventor.Code : String.Empty;
					string name = (String.IsNullOrWhiteSpace(inventor.Name) == false) ? inventor.Name.Replace("'", "''") : String.Empty;
					string description = (String.IsNullOrWhiteSpace(inventor.Description) == false) ? inventor.Description.Replace("'", "''") : String.Empty;
					string customerCode = (String.IsNullOrWhiteSpace(inventor.CustomerCode) == false) ? inventor.CustomerCode : String.Empty;
					string branchCode = (String.IsNullOrWhiteSpace(inventor.BranchCode) == false) ? inventor.BranchCode : String.Empty;
					string importCatalogProviderCode = (String.IsNullOrWhiteSpace(inventor.ImportCatalogAdapterCode) == false) ? inventor.ImportCatalogAdapterCode : String.Empty;
					string DBPath = (String.IsNullOrWhiteSpace(inventor.DBPath) == false) ? inventor.DBPath : String.Empty;
		
					//INSERT INTO [Inventor] ([ID],[Code ],[CustomerCode ],[BranchCode ],[CreateDate ],[Description ],[InventorDate ],[Name],[StatusInventorCode ],[DBPath],[ImportCatalogAdapterCode],[ImportIturAdapterCode],[ImportLocationAdapterCode],[ImportCatalogParms],[ImportIturParms],[ImportLocationParms]) 
					//VALUES (97,N'94dbe765-e79f-46c9-858d-5a8e7623f780',N'CustomerCode1',N'BranchCode1_1',{ts '2011-07-21 21:57:14.323'},N'test11',{ts '2011-07-21 21:57:14.323'},N'21.07.2011 21:57:14',null,N'2011\7\21\94dbe765-e79f-46c9-858d-5a8e7623f780',N'ImportCatalogUnizagAdapter',N'ImportIturDefaultAdapter',N'ImportLocationDefaultAdapter',null,null,null);
					string sql1 = sql +
						//	@"([Name],[Description],[Phone],[Address],[ContactPerson],[Mail],[Code],[Fax],[ImportCatalogProviderCode],[DBPath], [BranchCodeLocal],[BranchCodeERP],[CustomerCode] )   " +
						@"([Code ],[CustomerCode ],[BranchCode ],[Description ],[Name],[DBPath],[ImportCatalogAdapterCode]) 	" +
						@"VALUES " +																//logo	 code
						String.Format("(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}', N'{5}',N'{6}' );" + Environment.NewLine,
						code.Trim(), customerCode.Trim(), branchCode.Trim(), description.Trim(), name.Trim(), DBPath.Trim(), importCatalogProviderCode.Trim());

					retSql = retSql + sql1;
				}
			}
			return retSql;
		}

    }
}