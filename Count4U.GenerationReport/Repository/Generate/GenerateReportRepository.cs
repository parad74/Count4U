using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Count4U.Common.Helpers;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Modules.ContextCBI.Views.Report;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
//using Microsoft.Reporting.WinForms;
using NLog;
using System.Data;
using Count4U.Model.Interface.Main;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Count4U.Common.Constants;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model.Count4U.Translation;
using System.Reflection;
using Count4U.Model;
using Count4U.Common.ViewModel.Filter.Data;
using Zen.Barcode;
using Count4U.Common.UserSettings;
using Count4U.Common.Extensions;
using Microsoft.Reporting.WinForms;

namespace Count4U.GenerationReport
{
	public class GenerateReportRepository : IGenerateReportRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IUnityContainer _container;
		private readonly IServiceLocator _serviceLocator;
		private readonly IDBSettings _dbSettings;
		private readonly IReportRepository _reportRepository;
		private readonly IContextReportRepository _contextReportRepository;
		private readonly IIturAnalyzesSourceRepository _iturAnalyzesSourceRepository;
		private IUserSettingsManager _userSettingsManager;

		protected Microsoft.Reporting.WinForms.ReportDataSource _lastRreportDS;
		//protected string _lastDomainContextDataSet;
		private Stopwatch _stopwatch;
		private IList<Stream> m_streams;
		private Stream _stream;
		private int m_currentPageIndex;
		private static int indexFile = 1;
		private bool _landscape = false;

		public GenerateReportRepository(
		   IUnityContainer container,
		   IServiceLocator serviceLocator,
		   IDBSettings dbSettings,
		   IReportRepository reportRepository,
		   IContextReportRepository contextReportRepository,
			IIturAnalyzesSourceRepository iturAnalyzesSourceRepository,
			IUserSettingsManager userSettingsManager)
		{
			this._iturAnalyzesSourceRepository = iturAnalyzesSourceRepository;
			this._contextReportRepository = contextReportRepository;
			this._reportRepository = reportRepository;
			this._dbSettings = dbSettings;
			this._serviceLocator = serviceLocator;
			this._container = container;
			this._landscape = false;
			this._userSettingsManager = userSettingsManager;
		}

		#region GenerateReport
		public void GenerateReport(GenerateReportArgs args)
		{
			this._stopwatch = Stopwatch.StartNew();
			Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
			this._lastRreportDS = null;

			try
			{
				List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList = FillReportDSList(args);
				this.ShowReportView(args, reportDSList);
				Mouse.OverrideCursor = null;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("GenerateReport", exc);
				Mouse.OverrideCursor = null;
			}
		}

		private void ShowReportView(GenerateReportArgs args, List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList)
		{
			if (args == null)
			{
				_logger.Error("Error : ShowReportView - object args is Null");
				return;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : ShowReportView - object Report is Null");
				return;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				_logger.Error("Error : ShowReportView - args.Report.Path not  ContainsKey in GetDomainContextDataSetDictionary()");
				return;
			}

			// from DS => from Repository
			//string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];	
			string path = args.Report.Path;
			string reportFileName = args.Report.FileName;
			string codeReport = args.Report.CodeReport;
			ViewDomainContextEnum viewDomainContextType = args.ViewDomainContextType;
			Customer customer = args.Customer;
			Branch branch = args.Branch;
			Inventor inventor = args.Inventor;
			string dbPath = args.DbPath;
			SelectParams selectParams = args.SelectParams;
			// ================ Fill param for ReportContext ====================
			Itur itur = null;
			DocumentHeader documentHeader = null;

			if (selectParams != null)
			{
				if (selectParams.FilterParams.ContainsKey("IturCode") == true)		  //для заполнения reportContext
				{
					FilterParam filterParam = selectParams.FilterParams["IturCode"];
					IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
					itur = iturRepository.GetIturByCode(filterParam.Value.ToString(), dbPath);
				}

				if (selectParams.FilterParams.ContainsKey("DocumentCode") == true)
				{
					FilterParam filterParam = selectParams.FilterParams["DocumentCode"];
					IDocumentHeaderRepository docRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
					documentHeader = docRepository.GetDocumentHeaderByCode(filterParam.Value.ToString(), dbPath);
				}
			}

			// ======================= CreateReport ====================

			ReportsWinForm formReportView = this._container.Resolve<ReportsWinForm>();
			if (formReportView != null)
			{
				if ((reportDSList != null) && (reportDSList.Count > 0))
				// && (string.IsNullOrWhiteSpace(domainContextDataSet) == false)) //TODO: проверить зачем здесь из корня не запускаются отчеты
				{
					string reportTemplatePath = this.BuildReportFullPath(path, reportFileName);
					string reportFileName1 = Path.GetFileNameWithoutExtension(reportTemplatePath);
					formReportView.CreateReport(args, reportTemplatePath, reportFileName1, codeReport, reportDSList,
												customer, branch, inventor, itur, documentHeader); //, domainContextDataSet);
				}

				Mouse.OverrideCursor = null;

				_stopwatch.Stop();
				string timeSpent = String.Format("Report generation time: {0}", _stopwatch.ElapsedMilliseconds);
				System.Diagnostics.Debug.Print(timeSpent);
				_logger.Info(timeSpent);

				//formReportView.ShowDialog();
				formReportView.Show();

				if (IsInventProductAdvancedSearchView(viewDomainContextType) == false)
				{
					Task.Factory.StartNew(ClearIturAnalysis, dbPath).LogTaskFactoryExceptions("ShowReportView");
				}
			}
		}
		#endregion GenerateReport

		#region GetReportDS
		//Получить ReportDataSource в зависимости от V откуда вызывается 
		//и DS определенного в ReportTemplate
		private Microsoft.Reporting.WinForms.ReportDataSource GetReportDS(GenerateReportArgs args)
		{
			if (args == null)
			{
				_logger.Error("Error : GetReportDS - object args is Null");
				return null;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : GetReportDS - object Report is Null");
				return null;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				return null;
			}

			// from DS => from Repository
			string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];
			//from View, может быть несколько вариантов входа, то есть можно запускать их разных View
			ViewDomainContextEnum clickOnView = args.ViewDomainContextType;
			string dbPath = args.DbPath;
			SelectParams selectParams = args.SelectParams;

			//  @"IturDS"
			if (domainContextDataSet == DomainContextDataSet.Itur)
			{
				IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
				//var domainObjects = iturRepository.GetIturs(selectParams, dbPath);
				Iturs domainObjects = iturRepository.GetItursAndLocationName(selectParams, dbPath);
				if (args.Report.CodeReport == "[Rep-IT1-03]")
				{
					domainObjects = domainObjects.FillBarcode(this._userSettingsManager);
				}
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//  @"DocDS"
			else if (domainContextDataSet == DomainContextDataSet.Doc)
			{
				IDocumentHeaderRepository docRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
				var domainObjects = docRepository.GetDocumentHeaders(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//@"LocationDS"
			else if (domainContextDataSet == DomainContextDataSet.Location)
			{
				ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();
				var domainObjects = locationRepository.GetLocations(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//@"SectionDS"
			else if (domainContextDataSet == DomainContextDataSet.Section)
			{
				ISectionRepository sectionRepository = this._serviceLocator.GetInstance<ISectionRepository>();
				var domainObjects = sectionRepository.GetSections(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//@"DeviceDS"
			else if (domainContextDataSet == DomainContextDataSet.Device)
			{
				IDeviceRepository deviceRepository = this._serviceLocator.GetInstance<IDeviceRepository>();
				//var domainObjects = deviceRepository.GetDevices(selectParams, dbPath);
				DateTime startInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();
				DateTime endInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();

				Devices domainObjects = deviceRepository.RefillDeviceStatisticByDeviceCode(startInventorDateTime, endInventorDateTime, selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//@"DeviceWorkerDS"
			else if (domainContextDataSet == DomainContextDataSet.DeviceWorker)
			{
				IDeviceRepository deviceRepository = this._serviceLocator.GetInstance<IDeviceRepository>();
				//var domainObjects = deviceRepository.GetDevices(selectParams, dbPath);
				DateTime startInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();
				DateTime endInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();

				if (args.Report.CodeReport == "[Rep-DW1-11]"
					|| args.Report.CodeReport == "[Rep-DW1-11p]")
				{
					Devices domainObjects = deviceRepository.RefillDeviceStatisticByDeviceAndWorkerAndItur(startInventorDateTime, endInventorDateTime, selectParams, dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
				else if (args.Report.CodeReport == "[Rep-DW1-12sp]"	
					|| args.Report.CodeReport == "[Rep-DW1-13]"
					|| args.Report.CodeReport == "[Rep-DW1-13p]")
				{
					Devices domainObjects = deviceRepository.RefillDeviceStatisticByWorker(startInventorDateTime, endInventorDateTime, /* selectParams,*/ dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(DomainContextDataSet.Worker, domainObjects);
				}
				else
				{
					Devices domainObjects = deviceRepository.RefillDeviceStatisticByDeviceAndWorker(startInventorDateTime, endInventorDateTime, /*selectParams,*/ dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
			}
	

			//"FamilyDS"
			else if (domainContextDataSet == DomainContextDataSet.Family)
			{
				IFamilyRepository familyRepository = this._serviceLocator.GetInstance<IFamilyRepository>();
				var domainObjects = familyRepository.GetFamilys(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
	
			//@"SupplierDS"
			else if (domainContextDataSet == DomainContextDataSet.Supplier)
			{
				ISupplierRepository supplierRepository = this._serviceLocator.GetInstance<ISupplierRepository>();
				if (args.Report.CodeReport == "[Rep-SP1-01]")
				{
					supplierRepository.ReCountShilfSum(selectParams, dbPath);
					var domainObjects = supplierRepository.GetSuppliers(selectParams, dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
				else	if (args.Report.CodeReport == "[Rep-SP1-02]"
					|| args.Report.CodeReport == "[Rep-SP1-03]") // DOTO Shelf
					{
						IShelfRepository shelfRepository = this._serviceLocator.GetInstance<IShelfRepository>();
						//supplierRepository.ReCountShilfSum(selectParams, dbPath);
						var domainObjects = shelfRepository.GetShelves(selectParams, dbPath);
						this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(DomainContextDataSet.Shelf, domainObjects);
				}
					else
				{
					var domainObjects = supplierRepository.GetSuppliers(selectParams, dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
				
			}
			//@"IturDocDS"
			else if (domainContextDataSet == DomainContextDataSet.IturDoc)
			{
				IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
				var domainObjects = documentHeaderRepository.GetDocumentHeaders(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			else if (domainContextDataSet == DomainContextDataSet.PDA)
			{
				IInventProductRepository inventProductRepository =
					this._serviceLocator.GetInstance<IInventProductRepository>();
				if (args.Report.CodeReport == "[Rep-IP1-05]")
				{
					var domainObjects = inventProductRepository.GetInventProductsExtended(selectParams, args.Report.CodeReport, dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
				else
				{
					var domainObjects = inventProductRepository.GetInventProducts(selectParams, dbPath);
					this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
				}
			}
			else if (domainContextDataSet == DomainContextDataSet.Catalog)
			{
				IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
				var domainObjects = productRepository.GetProducts(selectParams, dbPath);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			//else if (domainContextDataSet == DomainContextDataSet.ContextReport)
			//{
			//    var domainObjects = this._contextReportRepository.GetContextReports();
			//    new ReportDataSource(domainContextDataSet, domainObjects);
			//}
			//  @"CustomerDS"
			else if (domainContextDataSet == DomainContextDataSet.Customer)
			{
				ICustomerRepository customerRepository = this._serviceLocator.GetInstance<ICustomerRepository>();
				var domainObjects = customerRepository.GetCustomers(selectParams);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			else if (domainContextDataSet == DomainContextDataSet.Branch)
			{
				IBranchRepository branchRepository = this._serviceLocator.GetInstance<IBranchRepository>();
				var domainObjects = branchRepository.GetBranches(selectParams);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			else if (domainContextDataSet == DomainContextDataSet.Inventor)
			{
				IInventorRepository inventorRepository = this._serviceLocator.GetInstance<IInventorRepository>();
				var domainObjects = inventorRepository.GetInventors(selectParams);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}
			else if (domainContextDataSet == DomainContextDataSet.AuditConfig)
			{
				IAuditConfigRepository auditConfigRepository = this._serviceLocator.GetInstance<IAuditConfigRepository>();
				var domainObjects = auditConfigRepository.GetAuditConfigs(Model.CBIContext.History);
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}

			else if (domainContextDataSet == DomainContextDataSet.IturAddInWorkerSession)
			{
				IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();
				//TODO !!!!
				//var domainObjects = iturAnalyzesRepository.GetIturAnalyzesCollection(selectParams);
				//this._lastRreportDS = new ReportDataSource(domainContextDataSet, domainObjects);
			}

			//==============================IturAnalyzes ================
			//@"IturDocAddInDS"
			//@"IturDocAddInSimpleDS"																
			else if (domainContextDataSet == DomainContextDataSet.IturDocPDA			//+!!!ReportAdvanced
					  || domainContextDataSet == DomainContextDataSet.IturDocAddInSimple
					  || domainContextDataSet == DomainContextDataSet.IturAddInSum
				)
			{
				IIturAnalyzesRepository iturAnalyzesRepository = this._serviceLocator.GetInstance<IIturAnalyzesRepository>();

				LevelInAnalyzesEnum levelInAnalyzes = LevelInAnalyzesEnum.None;
				levelInAnalyzes = GetLevelInAnalyzesByClickOnView(clickOnView);		//from view

				IturAnalyzeTypeEnum simpleListOrSum = IturAnalyzeTypeEnum.Full;
				simpleListOrSum = GetSimpleListOrSumTypeByDataSet(domainContextDataSet);  // need add familyDS 

				Stopwatch sw = Stopwatch.StartNew();
				bool refill = true;
	

				if (args.RefillReportDS == false && args.SaveReportToSendOffice)
				{
					refill = false;
				}

				if (IsInventProductAdvancedSearchView(clickOnView) == true)
				{
					refill = false;
				}

				

				Dictionary<object, object> dic = new Dictionary<object, object>();
				dic[ImportProviderParmEnum.PriceCode] = PriceCodeEnum.PriceBuy.ToString();
				dic[ImportProviderParmEnum.CodeReport] = args.Report.CodeReport;

				if (args.Inventor != null)
				{
					if (string.IsNullOrWhiteSpace(args.Inventor.PriceCode) == false)
					{
						dic[ImportProviderParmEnum.PriceCode] = args.Inventor.PriceCode;
					}
					if (args.Report.CodeReport == "[Rep-IS1-65p]"				 //Когда в отчете надо использовать DateModify from InentProduct
						|| args.Report.CodeReport == "[Rep-IS1-90]"
						|| args.Report.CodeReport == "[Rep-IS1-93]"
						|| args.Report.CodeReport == "[Rep-IS1-94]"
						|| args.Report.CodeReport == "[Rep-IS1-90c]"
						|| args.Report.CodeReport == "[Rep-IS1-90cd]")		   //?? 
					{
						dic[ImportProviderParmEnum.CreateDateTime] = "InventProductCreateDateTime";
					}
				}
				
				

				// if family - need domainObjects sorting and suming like need 
				// simpleListOrSum = IturAnalyzeTypeEnum.Full
				// utill if codeReport == [Rep-IS1-74p]
				// TODO DS and ....
				if (args.Report.CodeReport == "[Rep-IS1-74p]")
				{
					simpleListOrSum = IturAnalyzeTypeEnum.FullFamilySortLocationIturMakat;
				}
				bool refillCatalogDictionary = false; //?
				IturAnalyzesCollection domainObjects = iturAnalyzesRepository.GetIturAnalyzesCollection(
				levelInAnalyzes,
				selectParams, dbPath, refill,
				refillCatalogDictionary,
				simpleListOrSum,
				dic);
				System.Diagnostics.Debug.Print(String.Format("iturAnalyzesRepository.GetIturAnalyzesCollection: {0}", sw.ElapsedMilliseconds));

				System.Diagnostics.Debug.Print(String.Format("iturAnalyzesRepository.GetIturAnalyzesCollection: {0}", sw.ElapsedMilliseconds));
				this._lastRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(domainContextDataSet, domainObjects);
			}

			else
			{
				this._lastRreportDS = null;
			}

			return this._lastRreportDS;
		}

		private Microsoft.Reporting.WinForms.ReportDataSource GetFilterReportDS(GenerateReportArgs args)
		{
			if (args == null)
			{
				_logger.Error("Error : GetFilterReportDS - object args is Null");
				return null;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : GetFilterReportDS - object Report is Null");
				return null;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				return null;
			}

			string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];
			Microsoft.Reporting.WinForms.ReportDataSource filterReportDS = null;
			IFilterData filterData = args.FilterData;
			// Rep-IP1-01
			if (domainContextDataSet == DomainContextDataSet.PDA)
			{
				List<InventProductFilterData> filters = new List<InventProductFilterData>();
				InventProductFilterData filter = new InventProductFilterData();
				if (filterData != null && filterData is InventProductFilterData)
				{
					filter = filterData as InventProductFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.PDA.ToString(), filters);
			}

				// ??  Rep-IS1-50
			else if (domainContextDataSet == DomainContextDataSet.IturDocPDA)
			{
				List<InventProductSimpleFilterData> filters = new List<InventProductSimpleFilterData>();
				InventProductSimpleFilterData filter = new InventProductSimpleFilterData();
				if (filterData != null && filterData is InventProductSimpleFilterData)
				{
					filter = filterData as InventProductSimpleFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.IturDocPDA.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.IturAddInSum)
			{
				List<InventProductSumFilterData> filters = new List<InventProductSumFilterData>();
				InventProductSumFilterData filter = new InventProductSumFilterData();
				if (filterData != null && filterData is InventProductSumFilterData)
				{
					filter = filterData as InventProductSumFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.IturAddInSum.ToString(), filters);
			}

			//else if (domainContextDataSet == DomainContextDataSet.IturAddInWorkerSession)		 пока нет фильтра
			//{
			//    List<InventProductSumFilterData> filters = new List<InventProductSumFilterData>();
			//    InventProductSumFilterData filter = new InventProductSumFilterData();
			//    if (filterData != null && filterData is InventProductSumFilterData)
			//    {
			//        filter = filterData as InventProductSumFilterData;
			//    }
			//    filters.Add(filter);
			//    filterReportDS = new ReportDataSource(FilterDataSet..ToString(), filters);
			//}

			else if (domainContextDataSet == DomainContextDataSet.Branch)
			{
				List<BranchFilterData> filters = new List<BranchFilterData>();
				BranchFilterData filter = new BranchFilterData();
				if (filterData != null && filterData is BranchFilterData)
				{
					filter = filterData as BranchFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Branch.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Customer)
			{
				List<CustomerFilterData> filters = new List<CustomerFilterData>();
				CustomerFilterData filter = new CustomerFilterData();
				if (filterData != null && filterData is CustomerFilterData)
				{
					filter = filterData as CustomerFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Customer.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Inventor)
			{
				List<InventorFilterData> filters = new List<InventorFilterData>();
				InventorFilterData filter = new InventorFilterData();
				if (filterData != null && filterData is InventorFilterData)
				{
					filter = filterData as InventorFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Inventor.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Itur)
			{
				List<IturFilterData> filters = new List<IturFilterData>();
				IturFilterData filter = new IturFilterData();
				if (filterData != null && filterData is IturFilterData)
				{
					filter = filterData as IturFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Itur.ToString(), filters);

				if (filterData != null && filterData is IturAdvancedFilterData)
				{
					IturAdvancedFilterData filter1 = filterData as IturAdvancedFilterData;
					List<IturAdvancedFilterData> filters1 = new List<IturAdvancedFilterData>();
					filters1.Add(filter1);
					filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.IturAdvanced.ToString(), filters1);
				}
			}

			else if (domainContextDataSet == DomainContextDataSet.Location)
			{
				List<LocationFilterData> filters = new List<LocationFilterData>();
				LocationFilterData filter = new LocationFilterData();
				if (filterData != null && filterData is LocationFilterData)
				{
					filter = filterData as LocationFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Location.ToString(), filters);
			}


			else if (domainContextDataSet == DomainContextDataSet.Catalog)
			{
				List<ProductFilterData> filters = new List<ProductFilterData>();
				ProductFilterData filter = new ProductFilterData();
				if (filterData != null && filterData is ProductFilterData)
				{
					filter = filterData as ProductFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Catalog.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Section)
			{
				List<SectionFilterData> filters = new List<SectionFilterData>();
				SectionFilterData filter = new SectionFilterData();
				if (filterData != null && filterData is SectionFilterData)
				{
					filter = filterData as SectionFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Section.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Family)
			{
				List<FamilyFilterData> filters = new List<FamilyFilterData>();
				FamilyFilterData filter = new FamilyFilterData();
				if (filterData != null && filterData is FamilyFilterData)
				{
					filter = filterData as FamilyFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Family.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Supplier)
			{
				List<SupplierFilterData> filters = new List<SupplierFilterData>();
				SupplierFilterData filter = new SupplierFilterData();
				if (filterData != null && filterData is SupplierFilterData)
				{
					filter = filterData as SupplierFilterData;
				}
				filters.Add(filter);
				filterReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(FilterDataSet.Supplier.ToString(), filters);
			}

			else if (domainContextDataSet == DomainContextDataSet.Device)
			{
				//TO DO
				//List<DeviceFilterData> filters = new List<DeviceFilterData>();
				//DeviceFilterData filter = new DeviceFilterData();
				//if (filterData != null && filterData is DeviceFilterData)
				//{
				//	filter = filterData as DeviceFilterData;
				//}
				//filters.Add(filter);
				//filterReportDS = new ReportDataSource(FilterDataSet.Device.ToString(), filters);
			}
			else if (domainContextDataSet == DomainContextDataSet.DeviceWorker)
			{
				//TO DO
				//List<DeviceFilterData> filters = new List<DeviceFilterData>();
				//DeviceFilterData filter = new DeviceFilterData();
				//if (filterData != null && filterData is DeviceFilterData)
				//{
				//	filter = filterData as DeviceFilterData;
				//}
				//filters.Add(filter);
				//filterReportDS = new ReportDataSource(FilterDataSet.Device.ToString(), filters);
			}

			return filterReportDS;
		}


		private Microsoft.Reporting.WinForms.ReportDataSource GetContextReportDS(GenerateReportArgs args)
		//List<ReportDataSource> reportDSList//, string domainContextDataSet
		{
			if (args == null)
			{
				_logger.Error("Error : GetContextReportDS - object args is Null");
				return null;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : GetContextReportDS - object Report is Null");
				return null;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				return null;
			}

			string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];

			Microsoft.Reporting.WinForms.ReportDataSource contextReportDS = null;
			if (domainContextDataSet != DomainContextDataSet.Customer
				&& domainContextDataSet != DomainContextDataSet.Branch
				&& domainContextDataSet != DomainContextDataSet.Inventor
				&& domainContextDataSet != DomainContextDataSet.AuditConfig)
			{
				this._contextReportRepository.Clear();
				this._contextReportRepository.InitContextReport();
				this._contextReportRepository.InitContextReport(args.Customer);
				this._contextReportRepository.InitContextReport(args.Branch);
				this._contextReportRepository.InitContextReport(args.Inventor);
				this._contextReportRepository.InitContextReport(args.Location);
				this._contextReportRepository.InitContextReport(args.Itur);
				this._contextReportRepository.InitContextReport(args.Doc);
				this._contextReportRepository.InitContextReport(args.DbPath, args.Device);
				this._contextReportRepository.InitContextReport(args.DbPath, args.Param1, args.Param2, args.Param3, args.Report.CodeReport);

				if (args.Report.CodeReport == "[Rep-IS1-02]"
					|| args.Report.CodeReport == "[Rep-IS1-05]"
					|| args.Report.CodeReport == "[Rep-IS1-12]"
					|| args.Report.CodeReport == "[Rep-IS1-22]"
					|| args.Report.CodeReport == "[Rep-IS1-62]"
					|| args.Report.CodeReport == "[Rep-IS1-62n]"
					|| args.Report.CodeReport == "[Rep-IS1-62np]"
					|| args.Report.CodeReport == "[Rep-IS1-63q]"
					|| args.Report.CodeReport == "[Rep-IS1-63v]"
					|| args.Report.CodeReport == "[Rep-IS1-63s]"		
					|| args.Report.CodeReport == "[Rep-IS1-64]")
				{
					Dictionary<FilterAndSortEnum, string> dictionaryFilterAndSort = this.ReportDictionaryFromSelectParams(args.SelectParams);
					this._contextReportRepository.InitContextReport(dictionaryFilterAndSort);

				}

				ContextReports domainObjects = this._contextReportRepository.GetContextReports();
				contextReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(DomainContextDataSet.ContextReport.ToString(), domainObjects);
				//reportDSList.Add(contextReportDS);
			}
			return contextReportDS;
		}

		//IturList
		private Microsoft.Reporting.WinForms.ReportDataSource GetContextReportIturListDS(GenerateReportArgs args)
		{
			if (args == null)
			{
				_logger.Error("Error : GetContextReportListDS - object args is Null");
				return null;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : GetContextReportListDS - object Report is Null");
				return null;
			}

			if (this._reportRepository.GetDomainContextDataSetDictionary().ContainsKey(args.Report.Path) == false)
			{
				return null;
			}

			string domainContextDataSet = this._reportRepository.GetDomainContextDataSetDictionary()[args.Report.Path];

			Microsoft.Reporting.WinForms.ReportDataSource contextReportDS = null;
			if (domainContextDataSet != DomainContextDataSet.Customer
				&& domainContextDataSet != DomainContextDataSet.Branch
				&& domainContextDataSet != DomainContextDataSet.Inventor
				&& domainContextDataSet != DomainContextDataSet.AuditConfig)
			{
				this._contextReportRepository.Clear();
				this._contextReportRepository.InitContextReport();
				this._contextReportRepository.InitContextReport(args.Customer);
				this._contextReportRepository.InitContextReport(args.Branch);
				this._contextReportRepository.InitContextReport(args.Inventor);
				//this._contextReportRepository.InitContextReport(args.Location);
				//this._contextReportRepository.InitContextReport(args.Itur);
				//this._contextReportRepository.InitContextReport(args.Doc);

				//if (args.Report.CodeReport == "[Rep-IS1-02]"
				//	|| args.Report.CodeReport == "[Rep-IS1-12]"
				//	|| args.Report.CodeReport == "[Rep-IS1-22]"
				//	|| args.Report.CodeReport == "[Rep-IS1-62]")
				//{
				//	Dictionary<FilterAndSortEnum, string> dictionaryFilterAndSort = this.ReportDictionaryFromSelectParams(args.SelectParams);
				//	this._contextReportRepository.InitContextReport(dictionaryFilterAndSort);

				//}

				ContextReports domainObjects = this._contextReportRepository.GetIturListContextReports(args.DbPath);
				contextReportDS = new Microsoft.Reporting.WinForms.ReportDataSource(DomainContextDataSet.ContextReportIturList.ToString(), domainObjects);

			}
			return contextReportDS;
		}



		#endregion GetReportDS

		#region base
		public List<Microsoft.Reporting.WinForms.ReportDataSource> FillReportDSList(GenerateReportArgs args)
		{
			List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList = new List<Microsoft.Reporting.WinForms.ReportDataSource>();
			//==================  Fill and Add ReportDS        ===============================
			Microsoft.Reporting.WinForms.ReportDataSource reportDS = this.GetReportDS(args);
			if (reportDS != null) reportDSList.Add(reportDS);

			//==================  Worker Statistic  ===============================
			if (args.Report.CodeReport == "[Rep-DW1-12]"  )
			{
  				IDeviceRepository deviceRepository = this._serviceLocator.GetInstance<IDeviceRepository>();
  				DateTime startInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();
				DateTime endInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();
	  			Devices domainObjects = deviceRepository.RefillDeviceStatisticByWorker(startInventorDateTime, endInventorDateTime, args.DbPath);
				Microsoft.Reporting.WinForms.ReportDataSource workerRreportDS = new Microsoft.Reporting.WinForms.ReportDataSource(DomainContextDataSet.Worker, domainObjects);
				if (workerRreportDS != null) reportDSList.Add(workerRreportDS);
			}
			//==================  Fill and Add ContextReportDS ===============================
			Microsoft.Reporting.WinForms.ReportDataSource contextReportDS = this.GetContextReportDS(args);//, reportDSList);
			if (contextReportDS != null) reportDSList.Add(contextReportDS);

			//==================  Fill and Add contextReportListDS ===============================
			//if (args.Report.CodeReport == "[Rep-IS1-70]")
			//{
			//	ReportDataSource contextReportListDS = this.GetContextReportIturListDS(args);//, reportDSList);
			//	if (contextReportListDS != null) reportDSList.Add(contextReportListDS);
			//}
		
			//==================  Fill and Add FilterReportDS ===============================
			Microsoft.Reporting.WinForms.ReportDataSource filterReportDS = this.GetFilterReportDS(args);//, reportDSList);
			if (filterReportDS != null) reportDSList.Add(filterReportDS);
			return reportDSList;
		}


		private static bool IsInventProductAdvancedSearchView(ViewDomainContextEnum clickOnView)
		{
			bool ret = false;

			if (clickOnView == ViewDomainContextEnum.InventProductAdvancedSearch
				|| clickOnView == ViewDomainContextEnum.InventProductSumAdvancedSearch)
			//|| clickOnView == ViewDomainContextEnum.IturAdvancedSearch)
			//|| clickOnView == ViewDomainContextEnum.IturAdvancedSearch)
			{
				ret = true;
			}
			return ret;
		}

		private static IturAnalyzeTypeEnum GetSimpleListOrSumTypeByDataSet(string domainContextDataSet)
		{
			IturAnalyzeTypeEnum simpleListOrSum = IturAnalyzeTypeEnum.Full;
			if (domainContextDataSet == DomainContextDataSet.IturDocAddInSimple)
			{
				simpleListOrSum = IturAnalyzeTypeEnum.Simple;
			}
			else if (domainContextDataSet == DomainContextDataSet.IturAddInSum)
			{
				simpleListOrSum = IturAnalyzeTypeEnum.SimpleSum;
			}
			return simpleListOrSum;
		}

		private static LevelInAnalyzesEnum GetLevelInAnalyzesByClickOnView(
			ViewDomainContextEnum clickOnView)	  //from View, может быть несколько вариантов входа, то есть можно запускать их разных View
		{
			LevelInAnalyzesEnum context = LevelInAnalyzesEnum.None;
			if (clickOnView == ViewDomainContextEnum.Iturs
				 || clickOnView == ViewDomainContextEnum.IturAdvancedSearch
				)
				context = LevelInAnalyzesEnum.Iturs;
			else if (clickOnView == ViewDomainContextEnum.ItursItur
				//|| domainContextType == ViewDomainContextEnum.ItursIturAddIn
				)
				context = LevelInAnalyzesEnum.Itur;
			else if (clickOnView == ViewDomainContextEnum.ItursIturDoc
				//|| domainContextType == ViewDomainContextEnum.ItursIturDocAddIn
				//|| domainContextType == ViewDomainContextEnum.ItursIturDocAddInSimple
				)
				context = LevelInAnalyzesEnum.Doc;
			else if (clickOnView == ViewDomainContextEnum.ItursIturDocPDA
				//|| clickOnView == ViewDomainContextEnum.IturAdvancedSearch
				//	|| domainContextType == ViewDomainContextEnum.ItursIturDocPDAAddIn
				)
				context = LevelInAnalyzesEnum.InventProduct;
			//else if (domainContextType == DomainContextEnum.IturDoc
			//    || domainContextType == DomainContextEnum.IturDocAddIn) context = ReportDomainContextEnum.Doc;
			//??
			//else if (clickOnView == ViewDomainContextEnum.Inventor)
			//    context = LevelInAnalyzesEnum.Inventor;
			//else if (clickOnView == ViewDomainContextEnum.Customer)
			//    context = LevelInAnalyzesEnum.Customer;
			//else if (clickOnView == ViewDomainContextEnum.Branch)
			//    context = LevelInAnalyzesEnum.Branch;
			//else if (clickOnView == ViewDomainContextEnum.AuditConfig)
			//    context = LevelInAnalyzesEnum.AuditConfig;
			return context;
		}

		public string BuildReportFullPath(string path, string reportFileName)
		{
			string path1 = "";
			path = path.Trim(@" \".ToCharArray());
			if (string.IsNullOrWhiteSpace(path) == false)
			{
				path1 = path + @"\";
			}
			string result = this._dbSettings.ReportTemplatePath() + @"\" + path1 +
										reportFileName.Trim(@" \".ToCharArray());

			return result;
		}

		private void ClearIturAnalysis(object state)
		{
			string dbPath = state as String;
			if (String.IsNullOrWhiteSpace(dbPath) == false)
			{
				//this._iturAnalyzesSourceRepository.ClearIturAnalyzes(dbPath);
				this._iturAnalyzesSourceRepository.AlterTableIturAnalyzes(dbPath);
			}
		}
		#endregion base

		#region PrintReport
		// public print function
		public void RunPrintReport(GenerateReportArgs args, bool clearIturAnalysisAfterPrint = true)
		{
			if (args == null)
			{
				_logger.Error("Error : RunPrintReport - object args is Null");
				return;
			}

			if (args.Report == null)
			{
				_logger.Error("Error : RunPrintReport - object Report is Null");
				return;
			}


			_stopwatch = Stopwatch.StartNew();
			this._lastRreportDS = null; //test - always

			try
			{
				List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList = FillReportDSList(args);
				//==================  PrintReport	                   ================================
				this.PrintReport(args.Report.Path, args.Report.FileName, reportDSList, args.SelectParams,args.Report.CodeReport, args.DbPath, args.Report.Landscape, clearIturAnalysisAfterPrint, args.PrinterName);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunReportPrint", exc);
			}
		}

		private void PrintReport(string path, string reportFileName,
	   List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList, SelectParams argsSelectParams, string codeReport ,string dbPath, bool landscape, bool clearIturAnalysisAfterPrint = true,
			string printerName = "")
		{
			Microsoft.Reporting.WinForms.LocalReport report = new Microsoft.Reporting.WinForms.LocalReport();
			//report.pr
			try
			{

				string reportTemplatePath = BuildReportFullPath(path, reportFileName);
				report.ReportPath = reportTemplatePath;

				if ((reportDSList != null)
					&& (reportDSList.Count > 0))
				// && (string.IsNullOrWhiteSpace(domainContextDataSet) == false)) //TODO: проверить зачем здесь из корня не запускаются отчеты
				{
					if (codeReport == "[Rep-IS1-62]"
				|| codeReport == "[Rep-IS1-62n]"
				|| codeReport == "[Rep-IS1-62np]"
				|| codeReport == "[Rep-IS1-63s]"
				|| codeReport == "[Rep-IS1-63v]"
				|| codeReport == "[Rep-IS1-63q]"
				|| codeReport == "[Rep-IS1-62b]"
				|| codeReport == "[Rep-IS1-64]")
					{
						ReportParameter[] reportParams = new ReportParameter[2];
						reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", "0", true);
						reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", "0", true);
						SelectParams selectParams = argsSelectParams;
						if (selectParams != null)
						{
							if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportQuantityDifferenceERP.ToString()) == true)
							{
								string quantityDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportQuantityDifferenceERP.ToString()].ToString();
								if (string.IsNullOrWhiteSpace(quantityDifferenceERP) == false)
								{
									reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", quantityDifferenceERP, true);
								}
							}
							if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportValueDifferenceERP.ToString()) == true)
							{
								string valueDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportValueDifferenceERP.ToString()].ToString();
								if (string.IsNullOrWhiteSpace(valueDifferenceERP) == false)
								{
									reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", valueDifferenceERP, true);
								}
							}
						}
						report.SetParameters(reportParams);
					}
					////=======================   Show Report in Form
					//if (codeReport == "[Rep-IS1-62]"
					//	|| codeReport == "[Rep-IS1-62n]"
					//	|| codeReport == "[Rep-IS1-63s]"
					//	|| codeReport == "[Rep-IS1-63v]"
					//	|| codeReport == "[Rep-IS1-63q]"
					//	|| codeReport == "[Rep-IS1-62b]"
					//	|| codeReport == "[Rep-IS1-64]")
					//{

					//	ReportParameter[] reportParams = new ReportParameter[2];
					//	reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", "0", true);
					//	reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", "0", true);
					//	SelectParams selectParams = args.SelectParams;
					//	if (selectParams != null)
					//	{
					//		if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportQuantityDifferenceERP.ToString()) == true)
					//		{
					//			string quantityDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportQuantityDifferenceERP.ToString()].ToString();
					//			if (string.IsNullOrWhiteSpace(quantityDifferenceERP) == false)
					//			{
					//				reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", quantityDifferenceERP, true);
					//			}
					//		}
					//		if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportValueDifferenceERP.ToString()) == true)
					//		{
					//			string valueDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportValueDifferenceERP.ToString()].ToString();
					//			if (string.IsNullOrWhiteSpace(valueDifferenceERP) == false)
					//			{
					//				reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", valueDifferenceERP, true);
					//			}
					//		}
					//	}
					//	this.reportViewer1.LocalReport.SetParameters(reportParams);
					//}
					//===================
					foreach (Microsoft.Reporting.WinForms.ReportDataSource reportDS in reportDSList)
					{
						report.DataSources.Add(reportDS);
					}
					this._landscape = landscape;
					this.Export(report);
					m_currentPageIndex = 0;
					Print(printerName);
					Thread.Sleep(200);
					Dispose();
				}

				_stopwatch.Stop();
				string timeSpent = String.Format("RunReportPrint {0} time: {1}", reportTemplatePath, _stopwatch.ElapsedMilliseconds);
				System.Diagnostics.Debug.Print(timeSpent);
				_logger.Info(timeSpent);

				if (clearIturAnalysisAfterPrint == true)
				{
					//Task.Factory.StartNew(ClearIturAnalysis, dbPath).LogTaskFactoryExceptions("PrintReport");
					ClearIturAnalysis(dbPath);
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("PrintReport", exc);
			}
		}

		private void Export(Microsoft.Reporting.WinForms.LocalReport report)
		{
			//string pageSize = "  <PageWidth>8.5in</PageWidth>" +
			//  "  <PageHeight>11in</PageHeight>" +
			//    "  <MarginTop>0.25in</MarginTop>" +
			//  "  <MarginLeft>0.39in</MarginLeft>" +
			//  "  <MarginRight>0.39in</MarginRight>" +
			//  "  <MarginBottom>0.25in</MarginBottom>";
			//if (this._landscape == true)
			//{
			//    pageSize = "";
			////" <InteractiveHeight>8.5in</InteractiveHeight>"	+
			////"	<InteractiveWidth>11in</InteractiveWidth> "  +
			// //"  <PageWidth>11in</PageWidth>" +
			// // "  <PageHeight>8.5in</PageHeight>";// +
			//  //"  <MarginTop>0.05in</MarginTop>" +
			//  //"  <MarginLeft>0.05in</MarginLeft>" +
			//  //"  <MarginRight>0.05in</MarginRight>" +
			//  //"  <MarginBottom>0.05in</MarginBottom>";
			//}
			string deviceInfo =
			  "<DeviceInfo>" +
			  "  <OutputFormat>EMF</OutputFormat>" +
				// pageSize + 
				//"  <PageWidth>8.5in</PageWidth>" +
				//"  <PageHeight>11in</PageHeight>" +
				//"  <MarginTop>0.25in</MarginTop>" +
				//"  <MarginLeft>0.39in</MarginLeft>" +
				//"  <MarginRight>0.39in</MarginRight>" +
				//"  <MarginBottom>0.25in</MarginBottom>" +
			  "</DeviceInfo>";
			Microsoft.Reporting.WinForms.Warning[] warnings;
			m_streams = new List<Stream>();
			report.Render("Image", deviceInfo, CreateStream, out warnings);

			foreach (Stream stream in m_streams)
			{
				stream.Position = 0;
			}
		}

		private Stream CreateStream(string name, string fileNameExtension,
			Encoding encoding, string mimeType, bool willSeek)
		{
			//string fullPath = name;
			//if (string.IsNullOrEmpty(FileSystem.FileWithProgramDataPath()) == false
			//	&& Directory.Exists(FileSystem.FileWithProgramDataPath()) == true)
			//fullPath = System.IO.Path.Combine(FileSystem.FileWithProgramDataPath(), name);

			string fullPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), name);
			Stream stream = new FileStream(fullPath + "." + fileNameExtension, FileMode.Create);
			m_streams.Add(stream);
			return stream;
		}

		private void Print(string printerName = "")
		{
			//string printerName = _userSettingsManager.PrinterGet();
			try
			{
				if (m_streams == null || m_streams.Count == 0)
					return;
				try
				{
					PrintDocument printDoc = new PrintDocument();
					if (string.IsNullOrWhiteSpace(printerName) == false)
					{
						printDoc.PrinterSettings.PrinterName = printerName; //TODO 
						if (!printDoc.PrinterSettings.IsValid)
						{
							string msg = String.Format("Can't find printer \"{0}\".", printerName);
							_logger.Info("Print", msg);
							return;
						}
					}

					printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
					printDoc.DefaultPageSettings.Landscape = this._landscape;
					printDoc.Print();
				}
				catch (Exception exc)
				{
					_logger.ErrorException("Print", exc);
				}
			}
			finally
			{
				this.Dispose();
			}
		}

		private void PrintPage(object sender, PrintPageEventArgs ev)
		{
			Image pageImage = null;
			try
			{
				pageImage = new Metafile(m_streams[m_currentPageIndex]);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("PrintPage", exc);
			}

			ev.Graphics.DrawImage(pageImage, ev.PageBounds);
			m_currentPageIndex++;
			ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
		}

		private void Dispose()
		{
			_stream = null;
			if (m_streams != null)
			{
				foreach (Stream stream in m_streams)
					stream.Close();
				m_streams = null;
			}
		}
		#endregion

		#region	SaveReport
		public string RunSaveReport(GenerateReportArgs args, string outpuFilePath, string reportFileFormat, ReportInfo info = null)
		{
			if (args == null)
			{
				_logger.Error("Error : RunSaveReport - object args is Null");
				return "";
			}

			if (args.Report == null)
			{
				_logger.Error("Error : RunSaveReport - object Report is Null");
				return "";
			}

			string ret = "";

			string extFile = ReportFileFormat.Pdf.ToLower();
			if (reportFileFormat == ReportFileFormat.Word)
			{
				extFile = "doc";
			}
			else if (reportFileFormat == ReportFileFormat.Excel)
			{
				extFile = "xls";
			}
			else if (reportFileFormat == ReportFileFormat.EXCELOPENXML)
			{
				extFile = "xlsx";
			}
			else
			{
				reportFileFormat = ReportFileFormat.Pdf;
				extFile = "pdf";
			}


			string filePath = Path.GetDirectoryName(outpuFilePath);
			string fileName = Path.GetFileNameWithoutExtension(outpuFilePath);
			outpuFilePath = filePath.Trim('\\') + @"\" + fileName + "." + extFile;

			_stopwatch = Stopwatch.StartNew();
			this._lastRreportDS = null; //test - always
			try
			{
				List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList = FillReportDSList(args);
				//==================  SaveReport	                   ================================
				ret = this.SaveReport(args.Report.Path, args.Report.FileName, reportDSList, args.DbPath,
					 outpuFilePath, reportFileFormat,
					 args.Customer, args.Branch, args.Inventor, args.Itur, args.Doc, args.Device);
			}
			catch (Exception exc)
			{
				_logger.ErrorException("RunSaveReport", exc);
				//Mouse.OverrideCursor = null;
			}
			return ret;
		}


		private string SaveReport(string path, string reportFileName,
		   List<Microsoft.Reporting.WinForms.ReportDataSource> reportDSList, string dbPath,
			 string outpuFilePath, string outputReportFileFormat,
			 Customer customer, Branch branch, Inventor inventor,
			Itur itur = null, DocumentHeader documentHeader = null, Device device = null)
		{
			string outpuFilePathRet = "";
			Microsoft.Reporting.WinForms.Warning[] warnings;
			string[] streamids;
			string mimeType;
			string encoding;
			string extension;

			string iturCode = "";
			string docNum = "";
			string branchCodeERP = "";
			string customerCode = "";
			DateTime inventorDate = DateTime.Now;

			if (itur != null) { iturCode = itur.IturCode; }
			if (documentHeader != null) { docNum = documentHeader.DocNum.ToString(); }
			if (branch != null) { branchCodeERP = string.IsNullOrWhiteSpace(branch.BranchCodeERP) == false ? branch.BranchCodeERP : string.Empty; }
			if (customer != null) { customerCode = string.IsNullOrWhiteSpace(customer.Code) == false ? customer.Code : string.Empty; }
			if (inventor != null) { inventorDate = inventor.InventorDate; }

			Microsoft.Reporting.WinForms.LocalReport report = new Microsoft.Reporting.WinForms.LocalReport();
			string reportTemplatePath = BuildReportFullPath(path, reportFileName);
			report.ReportPath = reportTemplatePath;

			if ((reportDSList != null) && (reportDSList.Count > 0))
			{
				foreach (Microsoft.Reporting.WinForms.ReportDataSource reportDS in reportDSList)
				{
					report.DataSources.Add(reportDS);
				}

				byte[] bytes = report.Render(
				outputReportFileFormat, null, out mimeType, out encoding,
				 out extension,
				out streamids, out warnings);

				outpuFilePath = ReNameFile(outpuFilePath, branchCodeERP, iturCode, docNum, customerCode, inventorDate);

				FileStream fs = new FileStream(outpuFilePath, FileMode.Create);
				try
				{

					fs.Write(bytes, 0, bytes.Length);
					fs.Close();
					outpuFilePathRet = outpuFilePath;
				}
				catch (Exception exc)
				{
					_logger.ErrorException("SaveReport", exc);
				}
				finally
				{
					fs.Close();
				}
			}
			return outpuFilePathRet;
		}

		private static string ReNameFile(string outpuFilePath,
			string branchCodeERP, string iturCode, string docNum, string customerCode, DateTime inventorDate)
		{
			string filePath = Path.GetDirectoryName(outpuFilePath);
			string fileName = Path.GetFileName(outpuFilePath);

			if (outpuFilePath.Contains("XXX") == true)
			{
				fileName = fileName.Replace("XXX", branchCodeERP);
			}
			if (outpuFilePath.Contains("YYY") == true)
			{
				fileName = fileName.Replace("YYY", iturCode);
			}
			if (outpuFilePath.Contains("ZZZ") == true)
			{
				fileName = fileName.Replace("ZZZ", docNum);
			}
			if (outpuFilePath.Contains("VVV") == true)
			{
				fileName = fileName.Replace("VVV", customerCode);
			}
			if (outpuFilePath.Contains("DD") == true)
			{
				fileName = fileName.Replace("DD", inventorDate.ToString("dd"));
			}
			if (outpuFilePath.Contains("MM") == true)
			{
				fileName = fileName.Replace("MM", inventorDate.ToString("MM"));
			}
			if (outpuFilePath.Contains("YY") == true)
			{
				fileName = fileName.Replace("YY", inventorDate.ToString("yyyy"));
			}
			if (outpuFilePath.Contains("NOWDATE") == true)
			{
				DateTime dt = DateTime.Now;
				string dateNow = dt.ToString("dd") + "-" + dt.ToString("MM") + "-" + dt.ToString("yyyy");

				fileName = fileName.Replace("NOWDATE", dateNow);
			}

			string outpuFilePathRet = filePath.Trim('\\') + @"\" + fileName;
			return outpuFilePathRet;
		}
		#endregion

		#region LocalizedReportName
		public string GetLocalizedReportName(Count4U.GenerationReport.Report report)
		{
			string result = String.Empty;
			if (!String.IsNullOrWhiteSpace(report.MenuCaptionLocalizationCode))
			{
				result = UtilsMisc.LocalizationFromLocalizationKey(report.MenuCaptionLocalizationCode);
			}
			else
			{
				if (!String.IsNullOrWhiteSpace(report.MenuCaption))
				{
					result = report.MenuCaption;
				}
				else
				{
					if (!String.IsNullOrWhiteSpace(report.Description))
					{
						result = report.Description;
					}
					else
					{
						result = report.FileName;
					}
				}
			}

			return result;
		}

		#endregion LocalizedReportName

		#region SelectParams

		private Dictionary<FilterAndSortEnum, string> ReportDictionaryFromSelectParams(SelectParams selectParams)
		{
			Dictionary<FilterAndSortEnum, string> result = new Dictionary<FilterAndSortEnum, string>();

			if (selectParams == null) return result;

			try
			{
				if (!String.IsNullOrWhiteSpace(selectParams.SortParams))
				{
					Tuple<string, bool> sortTrimmed = TrimPropertyFromAsterisk(selectParams.SortParams);
					string propertyTranslation = PropertyTranslate(sortTrimmed.Item1);
					//result.Add(FilterAndSortEnum.SortByField, sortTrimmed.Item1);
					result.Add(FilterAndSortEnum.SortByField, propertyTranslation);
				}

				if (selectParams.FilterParams != null)
				{
					List<FilterAndSortEnum> properties = new List<FilterAndSortEnum>() { FilterAndSortEnum.QuantityDifferenceOriginalERP, FilterAndSortEnum.ValueDifferenceOriginalERP, FilterAndSortEnum.QuantityEdit, FilterAndSortEnum.ValueBuyEdit };
					foreach (FilterAndSortEnum filterAndSortParm in properties)
					{
						string property = filterAndSortParm.ToString().Trim();

						foreach (KeyValuePair<string, FilterParam> kvp in selectParams.FilterParams)
						{
							Tuple<string, bool> trimPropertyFromAsterisk = TrimPropertyFromAsterisk(kvp.Key.Trim());
							string propertySp = trimPropertyFromAsterisk.Item1;
							bool isAbsolute = trimPropertyFromAsterisk.Item2;

							if (propertySp == property)
							{
								FilterParam spValue = selectParams.FilterParams.First(r => r.Key.Contains(propertySp)).Value;

								if (spValue != null)
								{
									string valueFormatted = spValue.Value.ToString();
									if (isAbsolute)
									{
										double d;
										if (Double.TryParse(valueFormatted, out d))
										{
											valueFormatted = (Math.Sqrt(d)).ToString();
										}
									}
									string format = ConvertFilterOperatorToString(spValue.Operator);
									result.Add(filterAndSortParm, String.Format(format, valueFormatted));
								}
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("ReportDictionaryFromSelectParams", exc);
			}


			return result;
		}

		private Tuple<string, bool> TrimPropertyFromAsterisk(string property)
		{
			if (String.IsNullOrWhiteSpace(property))
			{
				return new Tuple<string, bool>(String.Empty, false);
			}

			string resultProperty = property;
			bool isTrimmed = false;
			int indexOfA = resultProperty.IndexOf('*');
			if (indexOfA >= 0)
			{
				if (resultProperty.Length >= indexOfA * 2 + 1)
				{
					resultProperty = resultProperty.Remove(indexOfA, indexOfA + 1).Trim();
					isTrimmed = true;
				}
			}
			return new Tuple<string, bool>(resultProperty, isTrimmed);
			//return new Tuple<string, bool>(resultPropertyLocalization, isTrimmed);
		}

		private string PropertyTranslate(string resultProperty)
		{
			string asc = "(" + enSortDirection.ASC.ToString() + ")";
			string desc = "(" + enSortDirection.DESC.ToString() + ")";
			int isSort = 0;
			if (resultProperty.Contains(enSortDirection.ASC.ToString()) == true)
			{
				isSort = 1;
				resultProperty = resultProperty.Replace(enSortDirection.ASC.ToString(), "").Trim();
			}
			else if (resultProperty.Contains(enSortDirection.DESC.ToString()) == true)
			{
				isSort = 2;
				resultProperty = resultProperty.Replace(enSortDirection.DESC.ToString(), "").Trim();
			}

			IPropertyTranslation propertyTranslation = ServiceLocator.Current.GetInstance<IPropertyTranslation>();
			System.Type t = typeof(IturAnalyzes);
			PropertyInfo pi = t.GetProperty(resultProperty);
			string resultPropertyLocalization = resultProperty;
			if (pi != null)
			{
				try
				{
					resultPropertyLocalization = propertyTranslation.GetTranslation(pi);
				}
				catch (Exception exp)
				{
					_logger.ErrorException("PropertyTranslate", exp);
				}
			}

			if (isSort == 1)
			{
				resultPropertyLocalization = String.Format(Localization.Resources.SortAsc, resultPropertyLocalization);
				//resultProperty = resultProperty.Replace(enSortDirection.ASC.ToString(), asc);
			}
			else if (isSort == 2)
			{
				resultPropertyLocalization = String.Format(Localization.Resources.SortDesc, resultPropertyLocalization);
				//resultProperty = resultProperty.Replace(enSortDirection.DESC.ToString(), desc);
			}

			return resultPropertyLocalization;
		}


		private string ConvertFilterOperatorToString(FilterOperator filterOperator)
		{
			switch (filterOperator)
			{
				case FilterOperator.Multiple:
					//return " = {0}";
					return Localization.Resources.Operator_Multiple;
				case FilterOperator.MultipleString:
					//return " = {0}";
					return Localization.Resources.Operator_MultipleString;
				case FilterOperator.Equal:
					// return " = {0}";
					return Localization.Resources.Operator_Equal;
				case FilterOperator.Less:
					// return " < {0}";
					return Localization.Resources.Operator_Less;
				case FilterOperator.LessOrEqual:
					// return " <= {0}";
					return Localization.Resources.Operator_LessOrEqual;
				case FilterOperator.Greater:
					// return " > {0}";
					return Localization.Resources.Operator_Greater;
				case FilterOperator.GreaterOrEqual:
					// return " >= {0}";
					return Localization.Resources.Operator_GreaterOrEqual;
				case FilterOperator.Contains:
					//return "Contains {0}";
					return Localization.Resources.Operator_Contains;
				case FilterOperator.StartsWith:
					// return "Starts with {0}";
					return Localization.Resources.Operator_StartsWith;
				case FilterOperator.EndsWith:
					//  return "Ends with {0}";
					return Localization.Resources.Operator_EndsWith;
				case FilterOperator.DateTimeGreaterOrEqual:
					// return " >= {0}";
					return Localization.Resources.Operator_DateTimeGreaterOrEqual;
				case FilterOperator.DateTimeLessOrEqual:
					//  return " <= {0}";
					return Localization.Resources.Operator_DateTimeLessOrEqual;
				default:
					throw new InvalidEnumArgumentException();
			}
		}

		#endregion


	}

	public static class DSUtil
	{
		public static byte[] CreateBarcode(IUserSettingsManager _userSettingsManager, String barcode)
		{
			//           Dim bdf As Code128BarcodeDraw = BarcodeDrawFactory.Code128WithChecksum
			//PictureBox1.Image = bdf.Draw("Hello world!", 20)
			//BarcodeMetrics tamccbb = new BarcodeMetrics(2, 90);
			System.Drawing.Image imagen;

			//string barcodeType = BarcodeDrawFactory.Code128WithChecksum.ToString();
			//var barcodeTypeEnum = BarcodeSymbology.Code128;
		//	imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code128).Draw(barcode, 30);
			//imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code39NC).Draw(barcode, 20);	   

			string barcodeType = _userSettingsManager.BarcodeTypeGet();
			var barcodeTypeEnum = (BarcodeSymbology)System.Enum.Parse(typeof(BarcodeSymbology), barcodeType.Trim());
			
			imagen = BarcodeDrawFactory.GetSymbology(barcodeTypeEnum).Draw(barcode, 45);
	
			ImageFormat format = ImageFormat.Bmp;

			MemoryStream mm = new MemoryStream();
			imagen.Save(mm, format);
		
			byte[] bytearray = mm.ToArray();

			imagen.Dispose();
			mm.Close();
			mm.Dispose();

			return bytearray;
		}

		//public static IturAnalyzesCollection FillLocationBarcode(this IturAnalyzesCollection iturAnalyzesCollection, IUserSettingsManager _userSettingsManager)
		//{
		//	string barcodePrefix = _userSettingsManager.BarcodePrefixGet(); 
		//	int k =0;
		//	foreach (IturAnalyzes item in iturAnalyzesCollection)
		//	{
		//		item.Number = 1 + (int)(k / 3);  //строки
		//		item.StatusIturBit = 1 + k % 3; // колонки
		//		item.Barcode = CreateBarcode(_userSettingsManager, barcodePrefix + item.LocationCode);
		//		k++;
		//	}
		//	return iturs;
		//}

		public static Iturs FillBarcode(this Iturs iturs, IUserSettingsManager _userSettingsManager)
		{
			string barcodePrefix = _userSettingsManager.BarcodePrefixGet();
			int k = 0;
			foreach (Itur item in iturs)
			{
				item.Number = 1 + (int)(k / 3);  //строки
				item.StatusIturBit = 1 + k % 3; // колонки
				item.BarcodeByteNotDB = CreateBarcode(_userSettingsManager, barcodePrefix + item.IturCode);
				k++;
			}
			return iturs;
		}
	}
}