using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U.Translation;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Audit;
using Count4U.Model.Main;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.ServiceContract;
using Count4U.Model.AnalyticDB;
using Count4U.Model.Interface.ProcessC4U;
using Count4U.Model.ProcessC4U;
using NLog;

namespace Count4U.Model
{
	public class DBModuleInit : IModule
	{
		#region IModule Members
		private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public DBModuleInit(IUnityContainer container)
		{
			this._container = container;
			
		}

		public void Initialize()
		{
			_logger.Info("DBModuleInit module initialization...");
			try
			{
              this._container.RegisterType(typeof(IAccessParentChildRepository), typeof(AccessParentChildRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IBarcodeRepository), typeof(BarcodeRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderEFRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IInputTypeRepository), typeof(InputTypeRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IStatusAuditConfigRepository), typeof(StatusAuditConfigRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IIturRepository), typeof(IturRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IIturRepository), typeof(IturEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IBuildingConfigRepository), typeof(BuildingConfigEFRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(ILocationRepository), typeof(LocationRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ILocationRepository), typeof(LocationEFRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IProductRepository), typeof(ProductRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IProductRepository), typeof(ProductEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ISectionRepository), typeof(SectionEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ISessionRepository), typeof(SessionEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IStatusDocHeaderRepository), typeof(StatusDocHeaderRepository), new ContainerControlledLifetimeManager());
            // this._container.RegisterType(typeof(IStatusInventorConfigRepository), typeof(StatusInventorConfigRepository), new ContainerControlledLifetimeManager());
            //his._container.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IStatusInventProductRepository), typeof(StatusInventProductEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IStatusIturRepository), typeof(StatusIturRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IStatusIturGroupRepository), typeof(StatusIturGroupRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IStatusIturRepository), typeof(StatusIturEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ISupplierRepository), typeof(SupplierEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ITypeRepository), typeof(TypeRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IUnitTypeRepository), typeof(UnitTypeRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IInventProductRepository), typeof(InventProductEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IUnitPlanRepository), typeof(UnitPlanEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IUnitPlanValueRepository), typeof(UnitPlanValueEFRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof (ICatalogConfigRepository), typeof (CatalogConfigEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IFamilyRepository), typeof(FamilyEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IShelfRepository), typeof(ShelfEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IDeviceRepository), typeof(DeviceEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IPropertyStrRepository), typeof(PropertyStrEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IPropertyStrToObjectRepository), typeof(PropertyStrToObjectEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ICurrentInventoryAdvancedRepository), typeof(CurrentInventoryAdvancedEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IPreviousInventoryRepository), typeof(PreviousInventoryEFRepository), new ContainerControlledLifetimeManager()); 
			this._container.RegisterType(typeof(ITemporaryInventoryRepository), typeof(TemporaryInventoryEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ITemplateInventoryRepository), typeof(TemplateInventoryEFRepository), new ContainerControlledLifetimeManager());
												  

            this._container.RegisterType(typeof(IContextCBIRepository), typeof(ContextCBIRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "CreateInventorCBIConfig", new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "HistoryCBIConfig", new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "MainCBIConfig", new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IInventorConfigRepository), typeof(InventorConfigEFRepository), new ContainerControlledLifetimeManager());

           // this._container.RegisterType(typeof(IAuditConfig), typeof(AuditConfig), new ContainerControlledLifetimeManager());
            // this._container.RegisterType(typeof (IBranch), typeof (Branch));

			this._container.RegisterType(typeof(IMainProcessJobRepository), typeof(MainProcessJobEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ITemporaryMainProcessJobRepository), typeof(TemporaryMainProcessJobEFRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IProcessRepository), typeof(ProcessEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IBranchRepository), typeof(BranchEFRepository), new ContainerControlledLifetimeManager());
             this._container.RegisterType(typeof(ICustomerConfigRepository), typeof(CustomerConfigEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ICustomerReport), typeof(CustomerReport), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ICustomerReportRepository), typeof(CustomerReportEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ICustomerRepository), typeof(CustomerEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IInventorRepository), typeof(InventorEFRepository), new ContainerControlledLifetimeManager());
            //this._container.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IIturAnalyzesRepository), typeof(IturAnalyzesEFRepository), new ContainerControlledLifetimeManager());
			//this._container.RegisterType(typeof(IIturAnalyzesADORepository), typeof(IturAnalyzesADORepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IConnectionADO), typeof(ConnectionADO), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ILog), typeof(LogImport), new ContainerControlledLifetimeManager());
              this._container.RegisterType(typeof(IMaskRepository), typeof(CustomerMaskEFRepository), "CustomerMaskEFRepository", new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IMaskRepository), typeof(BranchMaskEFRepository), "BranchMaskEFRepository", new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(IMaskRepository), typeof(InventorMaskEFRepository), "InventorMaskEFRepository", new ContainerControlledLifetimeManager());

            this._container.RegisterType(typeof(IMakatRepository), typeof(MakatEFRepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ICatalogConfigRepository), typeof(CatalogConfigEFRepository), new ContainerControlledLifetimeManager());

            this._container.RegisterType(typeof(IAlterADOProvider), typeof(AlterTableADORepository), new ContainerControlledLifetimeManager());
            this._container.RegisterType(typeof(ISQLScriptRepository), typeof(SQLScriptRepository), new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(IConnectionDB), typeof(ConnectionDB), new ContainerControlledLifetimeManager());
	
            this._container.RegisterType(typeof(IImportAdapterRepository), typeof(ImportAdapterEFRepository), new ContainerControlledLifetimeManager());
	
            this._container.RegisterType(typeof(IPropertyTranslation), typeof(PropertyTranslation), new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(IFieldLinkRepository), typeof(FieldLinkEFRepository), new ContainerControlledLifetimeManager());

			}
			catch (Exception exc)
			{
				_logger.Error("DBModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}	

		//public static void InitializeStatic(IUnityContainer _containerStatic)
		//{

		//	_containerStatic.RegisterType(typeof(IAccessParentChildRepository), typeof(AccessParentChildRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IBarcodeRepository), typeof(BarcodeRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IDocumentHeaderRepository), typeof(DocumentHeaderEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IInputTypeRepository), typeof(InputTypeRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusAuditConfigRepository), typeof(StatusAuditConfigRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IIturRepository), typeof(IturRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IIturRepository), typeof(IturEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IBuildingConfigRepository), typeof(BuildingConfigEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(ILocationRepository), typeof(LocationRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ILocationRepository), typeof(LocationEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IProductRepository), typeof(ProductRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IProductRepository), typeof(ProductEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ISectionRepository), typeof(SectionEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ISessionRepository), typeof(SessionEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusDocHeaderRepository), typeof(StatusDocHeaderRepository), new ContainerControlledLifetimeManager());
		//	// _containerStatic.RegisterType(typeof(IStatusInventorConfigRepository), typeof(StatusInventorConfigRepository), new ContainerControlledLifetimeManager());
		//	//his._container.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusInventProductRepository), typeof(StatusInventProductEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusIturRepository), typeof(StatusIturRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IStatusIturGroupRepository), typeof(StatusIturGroupRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IStatusIturRepository), typeof(StatusIturEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ISupplierRepository), typeof(SupplierEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ITypeRepository), typeof(TypeRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IUnitTypeRepository), typeof(UnitTypeRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IInventProductRepository), typeof(InventProductEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IUnitPlanRepository), typeof(UnitPlanEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IUnitPlanValueRepository), typeof(UnitPlanValueEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof (ICatalogConfigRepository), typeof (CatalogConfigEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IFamilyRepository), typeof(FamilyEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IShelfRepository), typeof(ShelfEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IDeviceRepository), typeof(DeviceEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IPropertyStrRepository), typeof(PropertyStrEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IPropertyStrToObjectRepository), typeof(PropertyStrToObjectEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICurrentInventoryAdvancedRepository), typeof(CurrentInventoryAdvancedEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IPreviousInventoryRepository), typeof(PreviousInventoryEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ITemporaryInventoryRepository), typeof(TemporaryInventoryEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ITemplateInventoryRepository), typeof(TemplateInventoryEFRepository), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IConnectionDB), typeof(ConnectionDB), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IProcessRepository), typeof(ProcessEFRepository), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IContextCBIRepository), typeof(ContextCBIRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "CreateInventorCBIConfig", new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "HistoryCBIConfig", new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IAuditConfigRepository), typeof(AuditConfigEFRepository), "MainCBIConfig", new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IInventorConfigRepository), typeof(InventorConfigEFRepository), new ContainerControlledLifetimeManager());

		//	//_containerStatic.RegisterType(typeof(IAuditConfig), typeof(AuditConfig), new ContainerControlledLifetimeManager());
		//	// _containerStatic.RegisterType(typeof (IBranch), typeof (Branch));


		//	_containerStatic.RegisterType(typeof(IMainProcessJobRepository), typeof(MainProcessJobEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ITemporaryMainProcessJobRepository), typeof(TemporaryMainProcessJobEFRepository), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IBranchRepository), typeof(BranchEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICustomerConfigRepository), typeof(CustomerConfigEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICustomerReport), typeof(CustomerReport), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICustomerReportRepository), typeof(CustomerReportEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICustomerRepository), typeof(CustomerEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IInventorRepository), typeof(InventorEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IStatusInventorRepository), typeof(StatusInventorRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IIturAnalyzesRepository), typeof(IturAnalyzesEFRepository), new ContainerControlledLifetimeManager());
		//	//_containerStatic.RegisterType(typeof(IIturAnalyzesADORepository), typeof(IturAnalyzesADORepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IConnectionADO), typeof(ConnectionADO), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ILog), typeof(LogImport), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IMaskRepository), typeof(CustomerMaskEFRepository), "CustomerMaskEFRepository", new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IMaskRepository), typeof(BranchMaskEFRepository), "BranchMaskEFRepository", new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IMaskRepository), typeof(InventorMaskEFRepository), "InventorMaskEFRepository", new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IMakatRepository), typeof(MakatEFRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ICatalogConfigRepository), typeof(CatalogConfigEFRepository), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IAlterADOProvider), typeof(AlterTableADORepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(ISQLScriptRepository), typeof(SQLScriptRepository), new ContainerControlledLifetimeManager());
		//	_containerStatic.RegisterType(typeof(IImportAdapterRepository), typeof(ImportAdapterEFRepository), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IPropertyTranslation), typeof(PropertyTranslation), new ContainerControlledLifetimeManager());

		//	_containerStatic.RegisterType(typeof(IFieldLinkRepository), typeof(FieldLinkEFRepository), new ContainerControlledLifetimeManager());

		//}

		#endregion
	}

	public enum CBIContext
	{
		CreateInventor = 0,
		History = 1,
		Main = 2
	}

	public enum FromContext
	{
		StartApplicationWithoutAction = 0,
		MdiInventorLinkWithoutAction = 1,
		FromBranchWithoutAction = 2,
		FromBranchWithAction = 3,
		SelectAdapterWithoutAction = 4,
		FromInventorWithoutAction = 5
	}

	public enum ToContext
	{
		ToBranchWithoutAction = 0
	}


	public enum DomainTypeEnum
	{
		Customer = 0,
		Branch = 1,
		Inventor = 2,
		InventorConfig = 3
	}
}
