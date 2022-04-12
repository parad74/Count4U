using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using System;
using Count4U.Model.Main;
using NLog;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Repository.Product.WrapperMulti;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Interface.Count4Mobile;
//using Count4U.Model.Repository.Profile;

namespace Count4U.Model
{
    public class ExportImportModuleInit : IModule
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnityContainer _container;

		public ExportImportModuleInit(IUnityContainer container)
        {
            this._container = container;

           
        }

        #region Implementation of IModule

        public void Initialize()
        {
			_logger.Info("Initialize ExportImportModule");
			try
			{
				//this._container.RegisterType(typeof(IProfileRepository), typeof(ProfileRESTRepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportLocationRepository), typeof(ImportLocationADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportLocationSQLiteADORepository), typeof(ImportLocationSQLiteADORepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportPropertyStrListSQLiteADORepository), typeof(ImportPropertyStrListSQLiteADORepository), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportPreviousInventorysSQLiteADORepository), typeof(ImportPreviousInventorySQLiteADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportTemplateInventorySQLiteADORepository), typeof(ImportTemplateInventorySQLiteADORepository), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IImportPreviousInventorysRepository), typeof(ImportPreviousInventorysBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportTemporaryInventoryRepository), typeof(ImportTemporaryInventoryBulkRepository), new ContainerControlledLifetimeManager());
				// OLd this._container.RegisterType(typeof(IImportTemplateInventoryRepository), typeof(ImportTemplateInventoryDbSetRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportTemplateInventoryRepository), typeof(ImportTemplateInventoryBulkRepository), new ContainerControlledLifetimeManager());			
												
	  																		  
				this._container.RegisterType(typeof(IImportIturRepository), typeof(ImportIturADORepository), new ContainerControlledLifetimeManager());
			//	this._container.RegisterType(typeof(IIturAnalyzesCaseSourceRepository), typeof(IturAnalyzesSourceADORepository), IturAnalyzesRepositoryEnum.IturAnalyzesADORepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesCaseSourceRepository), typeof(IturAnalyzesSourceReaderADORepository), IturAnalyzesRepositoryEnum.IturAnalyzesReaderADORepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesCaseSourceRepository), typeof(IturAnalyzesSourceReaderBulkRepository), IturAnalyzesRepositoryEnum.IturAnalyzesBulkRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesSourceRepository), typeof(IturAnalyzesSourceRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICurrentInventoryAdvancedSourceRepository), typeof(CurrentInventoryAdvancedSourceReaderBulkRepository), new ContainerControlledLifetimeManager());
		
				


				this._container.RegisterType(typeof(IImportPropertyStrRepository), typeof(ImportPropertyStrADORepository), new ContainerControlledLifetimeManager());


				//this._container.RegisterType(typeof(IIturAnalyzesADORepository), typeof(IturAnalyzesReaderADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportCatalogADORepository), typeof(ImportCatalogADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportCatalogSQLiteADORepository), typeof(ImportCatalogSQLiteADORepository), new ContainerControlledLifetimeManager());
																
				this._container.RegisterType(typeof(IImportBuildingConfigSQLiteADORepository), typeof(ImportBuildingConfigSQLiteADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportBuildingConfigADORepository), typeof(ImportBuildingConfigADORepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportCurrentInventorSQLiteADORepository), typeof(ImportCurrentInventorSQLiteADORepository), new ContainerControlledLifetimeManager());
		

				this._container.RegisterType(typeof(IImportCatalogSqlBulkRepository), typeof(ImportCatalogSqlBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportInventProductRepository), typeof(ImportInventProductSimpleADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportInventProductBlukRepository), typeof(ImportInventProductBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportSectionRepository), typeof(ImportSectionADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportSupplierRepository), typeof(ImportSupplierADORepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportBranchRepository), typeof(ImportBranchEFRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportCatalogBlukRepository), typeof(ImportCatalogBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportIturAnalyzesBlukRepository), typeof(IturAnalyzesReaderBulkRepository), IturAnalyzesTableRepositoryTypeEnum.IturAnalyzesBulk_IturTypeRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportShelfBlukRepository), typeof(ImportShelfBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportIturBlukRepository), typeof(ImportIturBulkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportCurrentInventoryAdvancedBlukRepository), typeof(CurrentInventoryAdvancedReaderBlukRepository), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportStatusInventProductBlukRepository), typeof(ImportStatusInventProductBulkRepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportDocumentHeaderBlukRepository), typeof(ImportDocumentHeaderBulkRepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportUnitPlanRepository), typeof(ImportUnitPlanEFRepository), new ContainerControlledLifetimeManager());
			//	this._container.RegisterType(typeof(IImportUnitPlanRepository), typeof(ImportUnitPartDBEFRepository), ImportProviderEnum.ImportUnitPartDBEFRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportFamilyRepository), typeof(ImportFamilyADORepository), new ContainerControlledLifetimeManager());
		
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationADOProvider), ImportProviderEnum.ImportLocationADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationMultiCsvProvider), ImportProviderEnum.ImportLocationMultiCsvProvider.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationXlsxProvider), ImportProviderEnum.ImportLocationXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationYesXlsxProviderQ), ImportProviderEnum.ImportLocationYesXlsxProviderQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationYesXlsxProviderSN), ImportProviderEnum.ImportLocationYesXlsxProviderSN.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationNativPlusLadpcProvider), ImportProviderEnum.ImportLocationNativPlusLadpcProvider.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateTagProvider), ImportProviderEnum.ImportLocationUpdateTagProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportEmulationLocationADOProvider), ImportProviderEnum.ImportEmulationLocationADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationMerkavaXslx2SqliteProvider), ImportProviderEnum.ImportLocationMerkavaXslx2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportLocationMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationClalitSdf2SqliteProvider), ImportProviderEnum.ImportLocationClalitSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationNativSdf2SqliteProvider), ImportProviderEnum.ImportLocationNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationNativPlusMISSdf2SqliteProvider), ImportProviderEnum.ImportLocationNativPlusMISSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateMerkavaSqlite2SdfProvider), ImportProviderEnum.ImportLocationUpdateMerkavaSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateClalitSqlite2SdfProvider), ImportProviderEnum.ImportLocationUpdateClalitSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateNativSqlite2SdfProvider), ImportProviderEnum.ImportLocationUpdateNativSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateNativSimpleSqlite2SdfProvider), ImportProviderEnum.ImportLocationUpdateNativSimpleSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
			   
			
				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationMerkavaXslx2SdfProvider), ImportProviderEnum.ImportLocationMerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationClalitXslx2SdfProvider), ImportProviderEnum.ImportLocationClalitXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationNativXslx2SdfProvider), ImportProviderEnum.ImportLocationNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationUpdateNativXslx2SdfProvider), ImportProviderEnum.ImportLocationUpdateNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemporaryInventoryMerkavaSqlite2SdfProvider), ImportProviderEnum.ImportTemporaryInventoryMerkavaSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemporaryInventoryClalitSqlite2SdfProvider), ImportProviderEnum.ImportTemporaryInventoryClalitSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemporaryInventoryNativSqlite2SdfProvider), ImportProviderEnum.ImportTemporaryInventoryNativSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemporaryInventoryNativPlusMISSqlite2SdfProvider), ImportProviderEnum.ImportTemporaryInventoryNativPlusMISSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCurrentInventorMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportCurrentInventorMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCurrentInventorClalitSdf2SqliteProvider), ImportProviderEnum.ImportCurrentInventorClalitSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCurrentInventorNativSdf2SqliteProvider), ImportProviderEnum.ImportCurrentInventorNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCurrentInventorNativPlusSdf2SqliteProvider), ImportProviderEnum.ImportCurrentInventorNativPlusSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
		
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr6ListMerkavaSqliteXslxProvider), ImportProviderEnum.ImportPropertyStr6ListMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7ListMerkavaSqliteXslxProvider), ImportProviderEnum.ImportPropertyStr7ListMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr6ListMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr6ListMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7ListMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr7ListMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr18ListMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr18ListMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrListClalitSdf2SqliteProvider1), ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrListClalitSdf2SqliteProvider2), ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrListClalitSdf2SqliteProvider3), ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrListClalitSdf2SqliteProvider4), ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrListClalitSdf2SqliteProvider5), ImportProviderEnum.ImportPropertyStrListClalitSdf2SqliteProvider5.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr1ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr1ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr2ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr2ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr3ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr3ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr4ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr4ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr5ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr5ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr6ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr6ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr7ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr8ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr8ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr9ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr9ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr10ListNativSdf2SqliteProvider), ImportProviderEnum.ImportPropertyStr10ListNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr6MerkavaXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr6MerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7MerkavaXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr7MerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7MerkavaXslx2SdfProvider1), ImportProviderEnum.ImportPropertyStr7MerkavaXslx2SdfProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr18MerkavaXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr18MerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr1NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr1NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr2NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr2NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr3NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr3NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr4NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr4NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr5NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr5NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr6NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr6NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr7NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr7NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr8NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr8NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr9NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr9NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStr10NativXslx2SdfProvider), ImportProviderEnum.ImportPropertyStr10NativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				
	

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrClalitXslx2SdfProvider1), ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrClalitXslx2SdfProvider2), ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrClalitXslx2SdfProvider3), ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrClalitXslx2SdfProvider4), ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrClalitXslx2SdfProvider5), ImportProviderEnum.ImportPropertyStrClalitXslx2SdfProvider5.ToString(), new ContainerControlledLifetimeManager());

				
			 	this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturMerkavaSqliteXslxProvider), ImportProviderEnum.ImportIturMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturMerkavaXslx2SdfProvider), ImportProviderEnum.ImportIturMerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturNativXslx2SdfProvider), ImportProviderEnum.ImportIturNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
			//	this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturUpdateNativXslx2SdfProvider), ImportProviderEnum.ImportIturUpdateNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturClalitXslx2SdfProvider), ImportProviderEnum.ImportIturClalitXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturADOProvider), ImportProviderEnum.ImportIturADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturERPADOProvider), ImportProviderEnum.ImportIturERPADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturFacingProvider), ImportProviderEnum.ImportIturFacingProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturMultiCsvProvider), ImportProviderEnum.ImportIturMultiCsvProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturMultiCsvProvider1), ImportProviderEnum.ImportIturMultiCsvProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturMultiCsvDelete9999999Provider), ImportProviderEnum.ImportIturMultiCsvDelete9999999Provider.ToString(), new ContainerControlledLifetimeManager());

				


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportDocumentHeaderFromDBBlukProvider), ImportProviderEnum.ImportDocumentHeaderFromDBBlukProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportDocumentHeaderAddFristDocToIturBlukProvider), ImportProviderEnum.ImportDocumentHeaderAddFristDocToIturBlukProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportDocumentHeaderAddSpetialDocToIturBlukProvider), ImportProviderEnum.ImportDocumentHeaderAddSpetialDocToIturBlukProvider.ToString(), new ContainerControlledLifetimeManager());
				 

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogUnizagADOProvider), ImportProviderEnum.ImportCatalogUnizagADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogUnizagADOProvider1), ImportProviderEnum.ImportCatalogUnizagADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogUnizagADOProvider2), ImportProviderEnum.ImportCatalogUnizagADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogUnizagADOProvider3), ImportProviderEnum.ImportCatalogUnizagADOProvider3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryMerkavaDbSetProvider), ImportProviderEnum.ImportPreviousInventoryMerkavaDbSetProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryClalitDbSetProvider), ImportProviderEnum.ImportPreviousInventoryClalitDbSetProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativDbSetProvider), ImportProviderEnum.ImportPreviousInventoryNativDbSetProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativPlusDbSetProvider), ImportProviderEnum.ImportPreviousInventoryNativPlusDbSetProvider.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativYesDbSetProviderQ), ImportProviderEnum.ImportPreviousInventoryNativYesDbSetProviderQ.ToString(), new ContainerControlledLifetimeManager());
			   	this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativYesDbSetProviderSN), ImportProviderEnum.ImportPreviousInventoryNativYesDbSetProviderSN.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativPlusLadpcDbSetProvider), ImportProviderEnum.ImportPreviousInventoryNativPlusLadpcDbSetProvider.ToString(), new ContainerControlledLifetimeManager());

																													  
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemplateInventoryNativPlusDbSetProvider), ImportProviderEnum.ImportTemplateInventoryNativPlusDbSetProvider.ToString(), new ContainerControlledLifetimeManager());



				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductYesXlsxProviderSN), ImportProviderEnum.ImportInventProductYesXlsxProviderSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductYesXlsxProviderQ), ImportProviderEnum.ImportInventProductYesXlsxProviderQ.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMerkavaXlsxProviderSN), ImportProviderEnum.ImportInventProductMerkavaXlsxProviderSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMerkavaXlsxProviderQ), ImportProviderEnum.ImportInventProductMerkavaXlsxProviderQ.ToString(), new ContainerControlledLifetimeManager());



				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMultiCsvProvider), ImportProviderEnum.ImportInventProductMultiCsvProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductSimpleADOProvider), ImportProviderEnum.ImportInventProductSimpleADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductSimpleYarpaADOProvider), ImportProviderEnum.ImportInventProductSimpleYarpaADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductAdvancedADOProvider), ImportProviderEnum.ImportInventProductAdvancedADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductAdvancedADOProvider), ImportProviderEnum.ImportInventProductAdvancedYarpaADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBulkProvider1), ImportProviderEnum.ImportInventProductBulkProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductFromDbBulkProvider), ImportProviderEnum.ImportInventProductFromDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductFromNativDbBulkProvider), ImportProviderEnum.ImportInventProductFromNativDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMisADOProvider), ImportProviderEnum.ImportInventProductMisADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductWarehouseADOProvider), ImportProviderEnum.ImportInventProductWarehouseADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBD3Provider), ImportProviderEnum.ImportInventProductDB3Provider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMerkavaSqlite2SdfProvider), ImportProviderEnum.ImportInventProductMerkavaSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMinusByMakatFromDbBulkProvider), ImportProviderEnum.ImportInventProductMinusByMakatFromDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturMakatDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturMakatSNumberDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatSNumberDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturMakatSNumberProp10DbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturMakatSNumberProp10DbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturBarcodeSNumberDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeSNumberDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturBarcodeSNumberProp10DbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeSNumberProp10DbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturBarcodeDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturBarcodeDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
 				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductAfterCompareFromDbBulkProvider), ImportProviderEnum.ImportInventProductAfterCompareFromDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdateMakat2BarcodeDbBulkProvider), ImportProviderEnum.ImportInventProductUpdateMakat2BarcodeDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2MakatAndSNDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2MakatAndSNDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2BarcodeAndSNDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2BarcodeAndSNDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());


			   
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdate2SumByIturDocMakatDbBulkProvider), ImportProviderEnum.ImportInventProductUpdate2SumByIturDocMakatDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider1), ImportProviderEnum.ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider2), ImportProviderEnum.ImportInventProductUpdateCompare2SumByIturMakatDbBulkProvider2.ToString(), new ContainerControlledLifetimeManager());
	

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportStatusInventProductNativPlusSqlite2SdfProvider), ImportProviderEnum.ImportStatusInventProductNativPlusSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportStatusInventProductUpdateSumNativPlusSqlite2SdfProvider), ImportProviderEnum.ImportStatusInventProductUpdateSumNativPlusSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				
				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductClalitSqlite2SdfProvider), ImportProviderEnum.ImportInventProductClalitSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductNativSqlite2SdfProvider), ImportProviderEnum.ImportInventProductNativSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductNativPlusSqlite2SdfProvider), ImportProviderEnum.ImportInventProductNativPlusSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductNativPlusMISSqlite2SdfProvider), ImportProviderEnum.ImportInventProductNativPlusMISSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMISSqlite2SdfProvider), ImportProviderEnum.ImportInventProductMISSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductMisAndDefaultADOProvider), ImportProviderEnum.ImportInventProductMisAndDefaultADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductDefaultBackupProvider), ImportProviderEnum.ImportInventProductDefaultBackupProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBulkUpdateProvider), ImportProviderEnum.ImportInventProductBulkUpdateProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBulkUpdateProvider2), ImportProviderEnum.ImportInventProductBulkUpdateProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBulkUpdateProvider3), ImportProviderEnum.ImportInventProductBulkUpdateProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductBulkUpdateProvider4), ImportProviderEnum.ImportInventProductBulkUpdateProvider4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductUpdateBarcodeFromDbBulkProvider), ImportProviderEnum.ImportInventProductUpdateBarcodeFromDbBulkProvider.ToString(), new ContainerControlledLifetimeManager());
		
				
																																				  
												 
			//?? не используется
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportInventProductADOProvider1), ImportProviderEnum.ImportInventProductADOProvider1.ToString(), new ContainerControlledLifetimeManager());
	

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogForComaxASPADOProvider), ImportProviderEnum.ImportCatalogForComaxASPADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogForComaxASPADOProvider1), ImportProviderEnum.ImportCatalogForComaxASPADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogComaxASPMultiBarcodeADOProvider), ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogComaxASPMultiBarcodeADOProvider1), ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogComaxASPMultiBarcodeADOProvider2), ImportProviderEnum.ImportCatalogComaxASPMultiBarcodeADOProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitVerifoneADOProvider), ImportProviderEnum.ImportCatalogGazitVerifoneADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportHamarotGazitVerifoneADOProvider1), ImportProviderEnum.ImportHamarotGazitVerifoneADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportHamarotGazitVerifoneADOProvider2), ImportProviderEnum.ImportHamarotGazitVerifoneADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogFromDBADOProvider), ImportProviderEnum.ImportCatalogFromDBADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogFromDBBulkProvider), ImportProviderEnum.ImportCatalogFromDBBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturFromDBADOProvider), ImportProviderEnum.ImportIturFromDBADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturFromDBBlukProvider), ImportProviderEnum.ImportIturFromDBBlukProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturYesXlsxProviderQ), ImportProviderEnum.ImportIturYesXlsxProviderQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturYesXlsxProviderSN), ImportProviderEnum.ImportIturYesXlsxProviderSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturNativPlusLadpcProvider1), ImportProviderEnum.ImportIturNativPlusLadpcProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportIturNativPlusLadpcProvider9999), ImportProviderEnum.ImportIturNativPlusLadpcProvider9999.ToString(), new ContainerControlledLifetimeManager());
				
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportLocationFromDBADOProvider), ImportProviderEnum.ImportLocationFromDBADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityRenuarADOProvider), ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityRenuarADOProvider1), ImportProviderEnum.ImportCatalogPriorityRenuarADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNetPOSSuperPharmADOProvider), ImportProviderEnum.ImportCatalogNetPOSSuperPharmADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNetPOSSuperPharmADOProvider1), ImportProviderEnum.ImportCatalogNetPOSSuperPharmADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNetPOSSuperPharmUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogNetPOSSuperPharmUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNetPOSSuperPharmBulkProvider), ImportProviderEnum.ImportCatalogNetPOSSuperPharmBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				
 				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetADOProvider1), ImportProviderEnum.ImportCatalogXtechMeuhedetADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetADOProvider2), ImportProviderEnum.ImportCatalogXtechMeuhedetADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogXtechMeuhedetUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixNextADOProvider), ImportProviderEnum.ImportCatalogRetalixNextADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixNextADOProvider1), ImportProviderEnum.ImportCatalogRetalixNextADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixNextBulkProvider), ImportProviderEnum.ImportCatalogRetalixNextBulkProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixNextUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogRetalixNextUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogHashProvider), ImportProviderEnum.ImportCatalogHashProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogHashProvider1), ImportProviderEnum.ImportCatalogHashProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400MangoProvider), ImportProviderEnum.ImportCatalogAS400MangoProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400MangoProvider1), ImportProviderEnum.ImportCatalogAS400MangoProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitLeeCooperProvider), ImportProviderEnum.ImportCatalogGazitLeeCooperProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitLeeCooperProvider1), ImportProviderEnum.ImportCatalogGazitLeeCooperProvider1.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralCSVADOProvider1), ImportProviderEnum.ImportCatalogGeneralCSVADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralCSVADOProvider), ImportProviderEnum.ImportCatalogGeneralCSVADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralCSVUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogGeneralCSVUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralXLSXProvider), ImportProviderEnum.ImportCatalogGeneralXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralXLSXProvider1), ImportProviderEnum.ImportCatalogGeneralXLSXProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralXLSXUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogGeneralXLSXUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGeneralXLSXUpdateERPQuentetyADOProvider2), ImportProviderEnum.ImportCatalogGeneralXLSXUpdateERPQuentetyADOProvider2.ToString(), new ContainerControlledLifetimeManager());

			   

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetXLSXProvider), ImportProviderEnum.ImportCatalogXtechMeuhedetXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetXLSXProvider1), ImportProviderEnum.ImportCatalogXtechMeuhedetXLSXProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetXLSXProvider2), ImportProviderEnum.ImportCatalogXtechMeuhedetXLSXProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogXtechMeuhedetXLSXUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogXtechMeuhedetXLSXUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogLadyComfortADOProvider), ImportProviderEnum.ImportCatalogLadyComfortADOProvider.ToString(), new ContainerControlledLifetimeManager());
			
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMade4NetADOProvider), ImportProviderEnum.ImportCatalogMade4NetADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMade4NetADOProvider1), ImportProviderEnum.ImportCatalogMade4NetADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400JaforaProvider), ImportProviderEnum.ImportCatalogAS400JaforaProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNimrodAvivProvider), ImportProviderEnum.ImportCatalogNimrodAvivProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNimrodAvivProvider1), ImportProviderEnum.ImportCatalogNimrodAvivProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNimrodAvivProvider2), ImportProviderEnum.ImportCatalogNimrodAvivProvider2.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AprilProvider), ImportProviderEnum.ImportCatalogAS400AprilProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AprilProvider1), ImportProviderEnum.ImportCatalogAS400AprilProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AprilUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogAS400AprilUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogBazanCsvProvider), ImportProviderEnum.ImportCatalogBazanCsvProvider.ToString(), new ContainerControlledLifetimeManager());
			   	this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNesherProvider), ImportProviderEnum.ImportCatalogNesherProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityKedsShowRoomProvider), ImportProviderEnum.ImportCatalogPriorityKedsShowRoomProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogFRSVisionMirkamADOProvider), ImportProviderEnum.ImportCatalogFRSVisionMirkamADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogFRSVisionMirkamADOProvider1), ImportProviderEnum.ImportCatalogFRSVisionMirkamADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HonigmanProvider), ImportProviderEnum.ImportCatalogAS400HonigmanProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HonigmanProvider1), ImportProviderEnum.ImportCatalogAS400HonigmanProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HonigmanProvider2), ImportProviderEnum.ImportCatalogAS400HonigmanProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityCastroProvider), ImportProviderEnum.ImportCatalogPriorityCastroProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityCastroProvider1), ImportProviderEnum.ImportCatalogPriorityCastroProvider1.ToString(), new ContainerControlledLifetimeManager());

			   	this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAutosoftProvider), ImportProviderEnum.ImportCatalogAutosoftProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAutosoftProvider1), ImportProviderEnum.ImportCatalogAutosoftProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAutosoftProvider2), ImportProviderEnum.ImportCatalogAutosoftProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1XslxProvider), ImportProviderEnum.ImportCatalogSapb1XslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1XslxProvider1), ImportProviderEnum.ImportCatalogSapb1XslxProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1XslxUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogSapb1XslxUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1ZometsfarimProvider), ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1ZometsfarimProvider1), ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1ZometsfarimProvider2), ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1ZometsfarimProvider3), ImportProviderEnum.ImportCatalogSapb1ZometsfarimProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogSapb1ZometsfarimUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogSapb1ZometsfarimUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HoProvider), ImportProviderEnum.ImportCatalogAS400HoProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HoProvider1), ImportProviderEnum.ImportCatalogAS400HoProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HoProvider2_2), ImportProviderEnum.ImportCatalogAS400HoProvider2_2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HoProvider2_1), ImportProviderEnum.ImportCatalogAS400HoProvider2_1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitVerifoneSteimaztzkyProvider), ImportProviderEnum.ImportCatalogGazitVerifoneSteimaztzkyProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitVerifoneSteimaztzkyProvider1), ImportProviderEnum.ImportCatalogGazitVerifoneSteimaztzkyProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HamashbirProvider), ImportProviderEnum.ImportCatalogAS400HamashbirProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HamashbirProvider1), ImportProviderEnum.ImportCatalogAS400HamashbirProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HamashbirUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogAS400HamashbirUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HamashbirUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogAS400HamashbirUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400HamashbirUpdateERPQuentetyProvider2), ImportProviderEnum.ImportCatalogAS400HamashbirUpdateERPQuentetyProvider2.ToString(), new ContainerControlledLifetimeManager());

				
	
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMerkavaXslxProvider), ImportProviderEnum.ImportCatalogMerkavaXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMerkavaXslxProvider1), ImportProviderEnum.ImportCatalogMerkavaXslxProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMerkavaXslxUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogMerkavaXslxUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogClalitXslxProvider), ImportProviderEnum.ImportCatalogClalitXslxProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativXslx2SdfProvider), ImportProviderEnum.ImportCatalogNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativSqlite2SdfProvider), ImportProviderEnum.ImportCatalogNativSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativMISSqlite2SdfProvider), ImportProviderEnum.ImportCatalogNativMISSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativXslxUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogNativXslxUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyStrUpdateNativSqlite2SdfProvider), ImportProviderEnum.ImportPropertyStrUpdateNativSqlite2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
																										
				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMerkavaSqliteXslxProvider), ImportProviderEnum.ImportCatalogMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportCatalogMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativSdf2SqliteProvider), ImportProviderEnum.ImportCatalogNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
   				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativPlusMISSdf2SqliteProvider), ImportProviderEnum.ImportCatalogNativPlusMISSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
  				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogClalitSdf2SqliteProvider), ImportProviderEnum.ImportCatalogClalitSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());

				//this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativXslxProvider), ImportProviderEnum.ImportCatalogNativXslxProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPrioritySweetGirlXLSXProvider), ImportProviderEnum.ImportCatalogPrioritySweetGirlXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitGlobalXlsxProvider), ImportProviderEnum.ImportCatalogGazitGlobalXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogGazitAlufHaSportXlsxProvider), ImportProviderEnum.ImportCatalogGazitAlufHaSportXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPrioritytEsteeLouderXslxProvider), ImportProviderEnum.ImportCatalogPrioritytEsteeLouderXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYtungXlsxProvider), ImportProviderEnum.ImportCatalogYtungXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
	
								
																								
					
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400MegaProvider), ImportProviderEnum.ImportCatalogAS400MegaProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400MegaProvider1), ImportProviderEnum.ImportCatalogAS400MegaProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400MegaUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogAS400MegaUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigMerkavaSqliteXslxProvider), ImportProviderEnum.ImportBuildingConfigMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportBuildingConfigMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigNativSdf2SqliteProvider), ImportProviderEnum.ImportBuildingConfigNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigNativPlusMISSdf2SqliteProvider), ImportProviderEnum.ImportBuildingConfigNativPlusMISSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigClalitSdf2SqliteProvider), ImportProviderEnum.ImportBuildingConfigClalitSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigMerkavaXslx2SdfProvider), ImportProviderEnum.ImportBuildingConfigMerkavaXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigClalitXslx2SdfProvider), ImportProviderEnum.ImportBuildingConfigClalitXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBuildingConfigNativXslx2SdfProvider), ImportProviderEnum.ImportBuildingConfigNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportProfileNativXslx2SdfProvider), ImportProviderEnum.ImportProfileNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportProfileXml2SdfProvider), ImportProviderEnum.ImportProfileXml2SdfProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyDecoratorNativXslx2SdfProvider), ImportProviderEnum.ImportPropertyDecoratorNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyDecoratorNativExportErpProvider1), ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyDecoratorNativExportErpProvider2), ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyDecoratorNativExportErpProvider3), ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPropertyDecoratorNativExportErpProvider4), ImportProviderEnum.ImportPropertyDecoratorNativExportErpProvider4.ToString(), new ContainerControlledLifetimeManager());



				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryMerkavaSqliteXslxProvider), ImportProviderEnum.ImportPreviousInventoryMerkavaSqliteXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryMerkavaSdf2SqliteProvider), ImportProviderEnum.ImportPreviousInventoryMerkavaSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryClalitSdf2SqliteProvider), ImportProviderEnum.ImportPreviousInventoryClalitSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPreviousInventoryNativSdf2SqliteProvider), ImportProviderEnum.ImportPreviousInventoryNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportTemplateInventoryNativSdf2SqliteProvider), ImportProviderEnum.ImportTemplateInventoryNativSdf2SqliteProvider.ToString(), new ContainerControlledLifetimeManager());

				
						  																																							 
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionNetPOSSuperPharmADOProvider), ImportProviderEnum.ImportSectionDefaultADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionFromDBADOProvider), ImportProviderEnum.ImportSectionFromDBADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionGeneralCSVADOProvider), ImportProviderEnum.ImportSectionGeneralCSVADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionGeneralXLSXProvider), ImportProviderEnum.ImportSectionGeneralXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionNativXslx2SdfProvider), ImportProviderEnum.ImportSectionNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionNativXslx2SdfProvider1), ImportProviderEnum.ImportSectionNativXslx2SdfProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionMikramSonolADOProvider), ImportProviderEnum.ImportSectionMikramSonolADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionNimrodAvivProvider), ImportProviderEnum.ImportSectionNimrodAvivProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionH_MProvider), ImportProviderEnum.ImportSectionH_MProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionH_M_NewProvider), ImportProviderEnum.ImportSectionH_M_NewProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionGazitGlobalXlsxProvider), ImportProviderEnum.ImportSectionGazitGlobalXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionFRSVisionMirkamProvider), ImportProviderEnum.ImportSectionFRSVisionMirkamProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionMPLProvider), ImportProviderEnum.ImportSectionMPLProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionTafnitMatrixProvider), ImportProviderEnum.ImportSectionTafnitMatrixProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionSapb1ZometsfarimProvider), ImportProviderEnum.ImportSectionSapb1ZometsfarimProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionSapb1ZometsfarimProvider1), ImportProviderEnum.ImportSectionSapb1ZometsfarimProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionGazitVerifoneSteimaztzkyProvider), ImportProviderEnum.ImportSectionGazitVerifoneSteimaztzkyProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionGazitVerifoneSteimaztzkyProvider1), ImportProviderEnum.ImportSectionGazitVerifoneSteimaztzkyProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionAS400MegaProvider), ImportProviderEnum.ImportSectionAS400MegaProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSectionAS400HamashbirProvider), ImportProviderEnum.ImportSectionAS400HamashbirProvider.ToString(), new ContainerControlledLifetimeManager());

					
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierFromDBADOProvider), ImportProviderEnum.ImportSupplierFromDBADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierAvivPOSADOProvider), ImportProviderEnum.ImportSupplierAvivPOSADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierComaxASPADOProvider), ImportProviderEnum.ImportSupplierComaxASPADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierDefaultADOProvider), ImportProviderEnum.ImportSupplierDefaultADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierMade4NetADOProvider), ImportProviderEnum.ImportSupplierMade4NetADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierNibitADOProvider), ImportProviderEnum.ImportSupplierNibitADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierAS400AprilProvider), ImportProviderEnum.ImportSupplierAS400AprilProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierFRSVisionMirkamADOProvider), ImportProviderEnum.ImportSupplierFRSVisionMirkamADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierGeneralXLSXProvider), ImportProviderEnum.ImportSupplierGeneralXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierNativXslx2SdfProvider), ImportProviderEnum.ImportSupplierNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierSapb1ZometsfarimProvider), ImportProviderEnum.ImportSupplierSapb1ZometsfarimProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportSupplierAS400MegaProvider), ImportProviderEnum.ImportSupplierAS400MegaProvider.ToString(), new ContainerControlledLifetimeManager());
																						
																										  



				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyDefaultProvider), ImportProviderEnum.ImportFamilyDefaultProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyPriorityRenuarADOProvider), ImportProviderEnum.ImportFamilyPriorityRenuarADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyLadyComfortADOProvider), ImportProviderEnum.ImportFamilyLadyComfortADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyH_MProvider), ImportProviderEnum.ImportFamilyH_MProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyH_M_NewProvider), ImportProviderEnum.ImportFamilyH_M_NewProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyPriorityKedsShowRoomProvider), ImportProviderEnum.ImportFamilyPriorityKedsShowRoomProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyTafnitMatrixProvider), ImportProviderEnum.ImportFamilyTafnitMatrixProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyAS400HamashbirProvider), ImportProviderEnum.ImportFamilyAS400HamashbirProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyGeneralXLSXProvider), ImportProviderEnum.ImportFamilyGeneralXLSXProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportFamilyNativXslx2SdfProvider), ImportProviderEnum.ImportFamilyNativXslx2SdfProvider.ToString(), new ContainerControlledLifetimeManager());
	
				  



				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchXtechMeuhedetXlsxProvider), ImportProviderEnum.ImportBranchXtechMeuhedetXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchXtechMeuhedetProvider), ImportProviderEnum.ImportBranchXtechMeuhedetProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchDefautProvider), ImportProviderEnum.ImportBranchDefaultProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchDefautXlsxProvider), ImportProviderEnum.ImportBranchDefautXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchGazitVerifoneProvider), ImportProviderEnum.ImportBranchGazitVerifoneProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchMikramSonolProvider), ImportProviderEnum.ImportBranchMikramSonolProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchAS400LeumitProvider), ImportProviderEnum.ImportBranchAS400LeumitProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchAS400HonigmanProvider), ImportProviderEnum.ImportBranchAS400HonigmanProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportBranchPriorityCastroProvider), ImportProviderEnum.ImportBranchPriorityCastroProvider.ToString(), new ContainerControlledLifetimeManager());
			
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportPartUnitEFProvider), ImportProviderEnum.ImportPartUnitEFProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportUnitPartDBEFProvider), ImportProviderEnum.ImportUnitPartDBEFProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaADOProvider), ImportProviderEnum.ImportCatalogYarpaADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaADOProvider1), ImportProviderEnum.ImportCatalogYarpaADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaUpdateERPQuentetyADOProvider2), ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaWindowsADOProvider), ImportProviderEnum.ImportCatalogYarpaWindowsADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaWindowsADOProvider1), ImportProviderEnum.ImportCatalogYarpaWindowsADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaUpdateERPQuentetyWindowsADOProvider1), ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyWindowsADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYarpaUpdateERPQuentetyWindowsADOProvider2), ImportProviderEnum.ImportCatalogYarpaUpdateERPQuentetyWindowsADOProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivPOSADOProvider), ImportProviderEnum.ImportCatalogAvivPOSADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivPOSADOProvider1), ImportProviderEnum.ImportCatalogAvivPOSADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivPOSADOProvider2), ImportProviderEnum.ImportCatalogAvivPOSADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivPOSUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogAvivPOSUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMikramSonolADOProvider), ImportProviderEnum.ImportCatalogMikramSonolADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMikramSonolADOProvider1), ImportProviderEnum.ImportCatalogMikramSonolADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMikramSonolUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogMikramSonolUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivMultiADOProvider), ImportProviderEnum.ImportCatalogAvivMultiADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivMultiADOProvider1), ImportProviderEnum.ImportCatalogAvivMultiADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAvivMultiUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogAvivMultiUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityKedsADOProvider), ImportProviderEnum.ImportCatalogPriorityKedsADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityKedsADOProvider1), ImportProviderEnum.ImportCatalogPriorityKedsADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityKedsUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogPriorityKedsUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixPosHOADOProvider), ImportProviderEnum.ImportCatalogRetalixPosHOADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogRetalixPosHOUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogRetalixPosHOUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400LeumitADOProvider), ImportProviderEnum.ImportCatalogAS400LeumitADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400LeumitADOProvider1), ImportProviderEnum.ImportCatalogAS400LeumitADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400LeumitUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogAS400LeumitUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMiniSoftADOProvider), ImportProviderEnum.ImportCatalogMiniSoftADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMiniSoftADOProvider1), ImportProviderEnum.ImportCatalogMiniSoftADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOne1ADOProvider), ImportProviderEnum.ImportCatalogOne1ADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOne1ADOProvider1), ImportProviderEnum.ImportCatalogOne1ADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOne1UpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogOne1UpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOtechProvider), ImportProviderEnum.ImportCatalogOtechProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOtechProvider1), ImportProviderEnum.ImportCatalogOtechProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOtechProvider2), ImportProviderEnum.ImportCatalogOtechProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOtechUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogOtechUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNibitADOProvider), ImportProviderEnum.ImportCatalogNibitADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNibitADOProvider0), ImportProviderEnum.ImportCatalogNibitADOProvider0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNibitADOProvider1), ImportProviderEnum.ImportCatalogNibitADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNibitUpdateERPQuentetyADOProvider), ImportProviderEnum.ImportCatalogNibitUpdateERPQuentetyADOProvider.ToString(), new ContainerControlledLifetimeManager());


                this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AmericanEagleADOProvider), ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider.ToString(), new ContainerControlledLifetimeManager());
                this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AmericanEagleADOProvider1), ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AmericanEagleADOProvider2), ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogAS400AmericanEagleADOProvider3), ImportProviderEnum.ImportCatalogAS400AmericanEagleADOProvider3.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMaccabiPharmSAPADOProvider), ImportProviderEnum.ImportCatalogMaccabiPharmSAPADOProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMaccabiPharmSAPADOProvider1), ImportProviderEnum.ImportCatalogMaccabiPharmSAPADOProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMaccabiPharmSAPADOProvider2), ImportProviderEnum.ImportCatalogMaccabiPharmSAPADOProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMaccabiPharmSAPUpdateERPQuentetyADOProvider1), ImportProviderEnum.ImportCatalogMaccabiPharmSAPUpdateERPQuentetyADOProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMikiKupotProvider), ImportProviderEnum.ImportCatalogMikiKupotProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenOriginalsProvider), ImportProviderEnum.ImportCatalogOrenOriginalsProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenOriginalsProvider1), ImportProviderEnum.ImportCatalogOrenOriginalsProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenOriginalsProvider2), ImportProviderEnum.ImportCatalogOrenOriginalsProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenOriginalsUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogOrenOriginalsUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenMutagimProvider), ImportProviderEnum.ImportCatalogOrenMutagimProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogOrenMutagimProvider1), ImportProviderEnum.ImportCatalogOrenMutagimProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_MProvider), ImportProviderEnum.ImportCatalogH_MProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_MProvider1), ImportProviderEnum.ImportCatalogH_MProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_MProvider2), ImportProviderEnum.ImportCatalogH_MProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_MProvider3), ImportProviderEnum.ImportCatalogH_MProvider3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_M_NewProvider), ImportProviderEnum.ImportCatalogH_M_NewProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_M_NewProvider2), ImportProviderEnum.ImportCatalogH_M_NewProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogH_M_NewProvider3), ImportProviderEnum.ImportCatalogH_M_NewProvider3.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportShelfFacingProvider), ImportProviderEnum.ImportShelfFacingProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMPLProvider), ImportProviderEnum.ImportCatalogMPLProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMPLProvider1), ImportProviderEnum.ImportCatalogMPLProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMPLProvider2), ImportProviderEnum.ImportCatalogMPLProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogMPLUpdateERPQuentetyProvider1), ImportProviderEnum.ImportCatalogMPLUpdateERPQuentetyProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogTafnitMatrixProvider), ImportProviderEnum.ImportCatalogTafnitMatrixProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogTafnitMatrixProvider1), ImportProviderEnum.ImportCatalogTafnitMatrixProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogTafnitMatrixProvider2), ImportProviderEnum.ImportCatalogTafnitMatrixProvider2.ToString(), new ContainerControlledLifetimeManager());
                this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogTafnitMatrixUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogTafnitMatrixUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNikeIntProvider), ImportProviderEnum.ImportCatalogNikeIntProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNikeIntProvider1), ImportProviderEnum.ImportCatalogNikeIntProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNikeIntProvider2), ImportProviderEnum.ImportCatalogNikeIntProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNikeIntUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogNikeIntUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPrioritytEsteeLouderUpdateERPQuentetyProvider), ImportProviderEnum.ImportCatalogPrioritytEsteeLouderUpdateERPQuentetyProvider.ToString(), new ContainerControlledLifetimeManager());
	
				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogShalevetCsvProvider), ImportProviderEnum.ImportCatalogShalevetCsvProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityAldoProvider), ImportProviderEnum.ImportCatalogPriorityAldoProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogPriorityAldoProvider1), ImportProviderEnum.ImportCatalogPriorityAldoProvider1.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogWarehouseXslxProvider), ImportProviderEnum.ImportCatalogWarehouseXslxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogWarehouseXslxProvider1), ImportProviderEnum.ImportCatalogWarehouseXslxProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogWarehouseXslxProvider2), ImportProviderEnum.ImportCatalogWarehouseXslxProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogWarehouseXslxProvider3), ImportProviderEnum.ImportCatalogWarehouseXslxProvider3.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYesXlsxProviderSN), ImportProviderEnum.ImportCatalogYesXlsxProviderSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogYesXlsxProviderQ), ImportProviderEnum.ImportCatalogYesXlsxProviderQ.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IImportProvider), typeof(ImportCatalogNativPlusLadpcProvider), ImportProviderEnum.ImportCatalogNativPlusLadpcProvider.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportProvider), typeof(ForwardInventProductMalamFileProvider), ImportProviderEnum.ForwardInventProductMalamFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ForwardInventProductMalamDataSetProvider), ImportProviderEnum.ForwardInventProductMalamDataSetProvider.ToString(), new ContainerControlledLifetimeManager());

																				   
  						 
				//this._container.RegisterType(typeof(IExportProvider), typeof(ExportCatalogToFileProvider), ExportProviderEnum.ExportCatalogToFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportCatalogPdaHt630FileProvider), ExportProviderEnum.ExportCatalogPdaHt630FileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportCatalogPdaMISFileProvider), ExportProviderEnum.ExportCatalogPdaMISFileProvider.ToString(), new ContainerControlledLifetimeManager());
				
				
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportIturToFileProvider), ExportProviderEnum.ExportIturToFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportIturPdaHt630FileProvider), ExportProviderEnum.ExportIturPdaHt630FileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportIturPdaMISFileProvider), ExportProviderEnum.ExportIturPdaMISFileProvider.ToString(), new ContainerControlledLifetimeManager());
														   
									 
				
				//this._container.RegisterType(typeof(IExportProvider), typeof(ExportCatalogPdaMade4NetFileProvider), ExportProviderEnum.ExportCatalogPdaMade4NetFileProvider.ToString(), new ContainerControlledLifetimeManager());

				
				//? не используется
				//this._container.RegisterType(typeof(IExportProvider), typeof(ExportCatalogPdaHt630UploadProvider), ExportProviderEnum.ExportCatalogPdaHt630UploadProvider.ToString(), new ContainerControlledLifetimeManager());
	

				this._container.RegisterType(typeof(IExportProvider), typeof(ExportUserIniToFileProvider), ExportProviderEnum.ExportUserIniToFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportCustomerConfigToFileProvider), ExportProviderEnum.ExportCustomerConfigToFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportProvider), typeof(ExportCustomerMISConfigToFileProvider), ExportProviderEnum.ExportCustomerMISConfigToFileProvider.ToString(), new ContainerControlledLifetimeManager());
	
				
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductComaxERPFileProvider), ExportProviderEnum.ExportInventProductComaxERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductUnizagERPFileProvider), ExportProviderEnum.ExportInventProductUnizagERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductUnizagERPFileProvider1), ExportProviderEnum.ExportInventProductUnizagERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGazitERPFileProvider), ExportProviderEnum.ExportInventProductGazitERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
                
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider_M), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider_M.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider_T), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider_T.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider_TM), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider_TM.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider_TW), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider_TW.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider_W), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider_W.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider1), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityRenuarERPFileProvider2), ExportProviderEnum.ExportInventProductPriorityRenuarERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider), ExportProviderEnum.ExportInventProductOtechERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider_M), ExportProviderEnum.ExportInventProductOtechERPFileProvider_M.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider_T), ExportProviderEnum.ExportInventProductOtechERPFileProvider_T.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider_W), ExportProviderEnum.ExportInventProductOtechERPFileProvider_W.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider1), ExportProviderEnum.ExportInventProductOtechERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOtechERPFileProvider2), ExportProviderEnum.ExportInventProductOtechERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNetPOSSuperPharmERPFileProvider), ExportProviderEnum.ExportInventProductNetPOSSuperPharmERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNetPOSSuperPharmERPFileProvider1), ExportProviderEnum.ExportInventProductNetPOSSuperPharmERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductXtechMeuhedetERPFileProvider), ExportProviderEnum.ExportInventProductXtechMeuhedetERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductXtechMeuhedetERPFileProvider1), ExportProviderEnum.ExportInventProductXtechMeuhedetERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductYarpaERPFileProvider), ExportProviderEnum.ExportInventProductYarpaERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductYarpaERPFileProvider1), ExportProviderEnum.ExportInventProductYarpaERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGeneralCSVERPFileProvider), ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGeneralCSVERPFileProvider1), ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGeneralCSVERPFileProvider2), ExportProviderEnum.ExportInventProductGeneralCSVERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductYellowPazERPFileProvider), ExportProviderEnum.ExportInventProductYellowPazERPFileProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HoProvider), ExportProviderEnum.ExportInventProductAS400HoProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HoProvider1), ExportProviderEnum.ExportInventProductAS400HoProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HoProvider2), ExportProviderEnum.ExportInventProductAS400HoProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductXtechMeuhedetXlsxProvider), ExportProviderEnum.ExportInventProductXtechMeuhedetXlsxProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductXtechMeuhedetXlsxProvider2), ExportProviderEnum.ExportInventProductXtechMeuhedetXlsxProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductXtechMeuhedetXlsxProvider1), ExportProviderEnum.ExportInventProductXtechMeuhedetXlsxProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAvivPOSERPFileProvider), ExportProviderEnum.ExportInventProductAvivPOSERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAvivMultiERPFileProvider), ExportProviderEnum.ExportInventProductAvivMultiERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAvivMultiERPFileProvider1), ExportProviderEnum.ExportInventProductAvivMultiERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramSonolERPFileProvider), ExportProviderEnum.ExportInventProductMikramSonolERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramSonolERPFileProvider1), ExportProviderEnum.ExportInventProductMikramSonolERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramSonolSAPERPFileProvider), ExportProviderEnum.ExportInventProductMikramSonolSAPERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramSonolSAPERPFileProvider1), ExportProviderEnum.ExportInventProductMikramSonolSAPERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramTenERPFileProvider), ExportProviderEnum.ExportInventProductMikramTenERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikramTenERPFileProvider1), ExportProviderEnum.ExportInventProductMikramTenERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityKedsMatrixERPFileProvider), ExportProviderEnum.ExportInventProductPriorityKedsMatrixERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityKedsRegularERPFileProvider), ExportProviderEnum.ExportInventProductPriorityKedsRegularERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityKedsERPFileProvider1), ExportProviderEnum.ExportInventProductPriorityKedsERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixPosHOERPFileProvider), ExportProviderEnum.ExportInventProductRetalixPosHOERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixPosHOERPFileProvider0), ExportProviderEnum.ExportInventProductRetalixPosHOERPFileProvider0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixPosHOERPFileProvider1), ExportProviderEnum.ExportInventProductRetalixPosHOERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400LeumitERPFileProvider), ExportProviderEnum.ExportInventProductAS400LeumitERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400LeumitERPFileProvider1), ExportProviderEnum.ExportInventProductAS400LeumitERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMiniSoftERPFileProvider), ExportProviderEnum.ExportInventProductMiniSoftERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMiniSoftERPFileProvider1), ExportProviderEnum.ExportInventProductMiniSoftERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMiniSoftERPFileProvider2), ExportProviderEnum.ExportInventProductMiniSoftERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixNextERPFileProvider), ExportProviderEnum.ExportInventProductRetalixNextERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixNextERPFileProvider0), ExportProviderEnum.ExportInventProductRetalixNextERPFileProvider0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductRetalixNextERPFileProvider1), ExportProviderEnum.ExportInventProductRetalixNextERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOne1ERPFileProvider), ExportProviderEnum.ExportInventProductOne1ERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400AmericanEagleERPFileProvider), ExportProviderEnum.ExportInventProductAS400AmericanEagleERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400AmericanEagleERPFileProvider1), ExportProviderEnum.ExportInventProductAS400AmericanEagleERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMaccabiPharmSAPERPFileProvider), ExportProviderEnum.ExportInventProductMaccabiPharmSAPERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMaccabiPharmSAPERPFileProvider1), ExportProviderEnum.ExportInventProductMaccabiPharmSAPERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMikiKupotERPFileProvider), ExportProviderEnum.ExportInventProductMikiKupotERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductLadyComfortERPFileProvider), ExportProviderEnum.ExportInventProductLadyComfortERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductLadyComfortERPFileProvider1), ExportProviderEnum.ExportInventProductLadyComfortERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMade4NetERPFileProvider), ExportProviderEnum.ExportInventProductMade4NetERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMade4NetERPFileProvider1), ExportProviderEnum.ExportInventProductMade4NetERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400JaforaERPFileProvider), ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400JaforaERPFileProvider1), ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400JaforaERPFileProvider2), ExportProviderEnum.ExportInventProductAS400JaforaERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNibitERPFileProvider), ExportProviderEnum.ExportInventProductNibitERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNibitERPFileProvider1), ExportProviderEnum.ExportInventProductNibitERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOrenOriginalsERPFileProvider), ExportProviderEnum.ExportInventProductOrenOriginalsERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOrenOriginalsERPFileProvider1), ExportProviderEnum.ExportInventProductOrenOriginalsERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOrenOriginalsERPFileProvider2), ExportProviderEnum.ExportInventProductOrenOriginalsERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductOrenMutagimERPFileProvider), ExportProviderEnum.ExportInventProductOrenMutagimERPFileProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNimrodAvivERPFileProvider), ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNimrodAvivERPFileProvider1), ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNimrodAvivERPFileProvider2), ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNimrodAvivERPFileProvider3), ExportProviderEnum.ExportInventProductNimrodAvivERPFileProvider3.ToString(), new ContainerControlledLifetimeManager());
																																								   
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductH_MERPFileProvider), ExportProviderEnum.ExportInventProductH_MERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductH_M_NewERPFileProvider), ExportProviderEnum.ExportInventProductH_M_NewERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNesherERPFileProvider), ExportProviderEnum.ExportInventProductNesherERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
									

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400AprilERPFileProvider), ExportProviderEnum.ExportInventProductAS400AprilERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400AprilERPFileProvider1), ExportProviderEnum.ExportInventProductAS400AprilERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductBazanCsvERPFileProvider1), ExportProviderEnum.ExportInventProductBazanCsvERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativExportErpFileProvider), ExportProviderEnum.ExportInventProductNativExportErpFileProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityKedsShowRoomERPFileProvider), ExportProviderEnum.ExportInventProductPriorityKedsShowRoomERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityKedsShowRoomERPFileProvider1), ExportProviderEnum.ExportInventProductPriorityKedsShowRoomERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductHashERPFileProvider), ExportProviderEnum.ExportInventProductHashERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductHashERPFileProvider1), ExportProviderEnum.ExportInventProductHashERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400MangoERPFileProvider), ExportProviderEnum.ExportInventProductAS400MangoERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400MangoERPFileProvider1), ExportProviderEnum.ExportInventProductAS400MangoERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductFRSVisionMirkamERPFileProvider), ExportProviderEnum.ExportInventProductFRSVisionMirkamERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductFRSVisionMirkamERPFileProvider1), ExportProviderEnum.ExportInventProductFRSVisionMirkamERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HonigmanERPFileProvider), ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HonigmanERPFileProvider1), ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HonigmanERPFileProvider2), ExportProviderEnum.ExportInventProductAS400HonigmanERPFileProvider2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMPLFileProvider), ExportProviderEnum.ExportInventProductMPLFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMPLFileProvider1), ExportProviderEnum.ExportInventProductMPLFileProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductTafnitMatrixERPFileProvider), ExportProviderEnum.ExportInventProductTafnitMatrixERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductTafnitMatrixERPFileProvider1), ExportProviderEnum.ExportInventProductTafnitMatrixERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityCastroERPFileProvider), ExportProviderEnum.ExportInventProductPriorityCastroERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityCastroERPFileProvider1), ExportProviderEnum.ExportInventProductPriorityCastroERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNikeIntERPFileProvider), ExportProviderEnum.ExportInventProductNikeIntERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNikeIntERPFileProvider1), ExportProviderEnum.ExportInventProductNikeIntERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductWarehouseXslxFileProvider), ExportProviderEnum.ExportInventProductWarehouseXslxFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductWarehouseXslxFileProvider1), ExportProviderEnum.ExportInventProductWarehouseXslxFileProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductWarehouseXslxFileFormulaProvider), ExportProviderEnum.ExportInventProductWarehouseXslxFileFormulaProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGeneralXslxFileProvider), ExportProviderEnum.ExportInventProductGeneralXslxFileProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductYtungXslxFileProvider), ExportProviderEnum.ExportInventProductYtungXslxFileProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductClalitXlsxTemplateProvider1), ExportProviderEnum.ExportInventProductClalitXlsxTemplateProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider1), ExportProviderEnum.ExportInventProductMerkavaProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider2_1), ExportProviderEnum.ExportInventProductMerkavaProvider2_1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaGovProvider2_1), ExportProviderEnum.ExportInventProductMerkavaGovProvider2_1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider3), ExportProviderEnum.ExportInventProductMerkavaProvider3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider4), ExportProviderEnum.ExportInventProductMerkavaProvider4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider5), ExportProviderEnum.ExportInventProductMerkavaProvider5.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativProvider1), ExportProviderEnum.ExportInventProductNativProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusProvider1), ExportProviderEnum.ExportInventProductNativPlusProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusProvider1_Q), ExportProviderEnum.ExportInventProductNativPlusProvider1_Q.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusProvider1_SN), ExportProviderEnum.ExportInventProductNativPlusProvider1_SN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusProvider2), ExportProviderEnum.ExportInventProductNativPlusProvider2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusMisradApnimProvider), ExportProviderEnum.ExportInventProductNativPlusMisradApnim.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusLadpcCsvProvider1), ExportProviderEnum.ExportInventProductNativPlusLadpcCsvProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusLadpcXslxProvider2), ExportProviderEnum.ExportInventProductNativPlusLadpcXslxProvider2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusMateAsherProvider1), ExportProviderEnum.ExportInventProductNativPlusMateAsherProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductStockSonigoXslxProvider1), ExportProviderEnum.ExportInventProductStockSonigoXslxProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1), ExportProviderEnum.ExportInventProductNativPlusYesProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1_Q), ExportProviderEnum.ExportInventProductNativPlusYesProvider1_Q.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1_SN), ExportProviderEnum.ExportInventProductNativPlusYesProvider1_SN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider2), ExportProviderEnum.ExportInventProductNativPlusYesProvider2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1), ExportProviderEnum.ExportInventProductNativPlusYesProvider1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1_Q), ExportProviderEnum.ExportInventProductNativPlusYesProvider1_Q.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider1_SN), ExportProviderEnum.ExportInventProductNativPlusYesProvider1_SN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductNativPlusYesProvider2), ExportProviderEnum.ExportInventProductNativPlusYesProvider2.ToString(), new ContainerControlledLifetimeManager());

				
				//this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductMerkavaProvider2_2), ExportProviderEnum.ExportInventProductMerkavaProvider2_2.ToString(), new ContainerControlledLifetimeManager());
																																								  

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductSapb1XslxERPFileProvider), ExportProviderEnum.ExportInventProductSapb1XslxERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductSapb1XslxERPFileProvider1), ExportProviderEnum.ExportInventProductSapb1XslxERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductSapb1ZometsfarimERPFileProvider), ExportProviderEnum.ExportInventProductSapb1ZometsfarimERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductSapb1ZometsfarimERPFileProvider1), ExportProviderEnum.ExportInventProductSapb1ZometsfarimERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPriorityAldoERPFileProvider), ExportProviderEnum.ExportInventProductPriorityAldoERPFileProvider.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGazitVerifoneSteimaztzkyERPFileProvider), ExportProviderEnum.ExportInventProductGazitVerifoneSteimaztzkyERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGazitVerifoneSteimaztzkyERPFileProvider1), ExportProviderEnum.ExportInventProductGazitVerifoneSteimaztzkyERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400MegaERPFileProvider), ExportProviderEnum.ExportInventProductAS400MegaERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400MegaERPFileProvider1), ExportProviderEnum.ExportInventProductAS400MegaERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductGazitLeeCooperERPFileProvider), ExportProviderEnum.ExportInventProductGazitLeeCooperERPFileProvider.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HamashbirERPFileProvider), ExportProviderEnum.ExportInventProductAS400HamashbirERPFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductAS400HamashbirERPFileProvider1), ExportProviderEnum.ExportInventProductAS400HamashbirERPFileProvider1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPrioritySweetGirlXlsxErpFileProvider), ExportProviderEnum.ExportInventProductPrioritySweetGirlXlsxErpFileProvider.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportERPProvider), typeof(ExportInventProductPrioritySweetGirlXlsxErpFileProvider1), ExportProviderEnum.ExportInventProductPrioritySweetGirlXlsxErpFileProvider1.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IExportCatalogSimpleRepository), typeof(ExportCatalogSimpleFileRepository), ExportRepositoryEnum.ExportCatalogSimpleFileRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportIturRepository), typeof(ExportIturFileRepository), ExportRepositoryEnum.ExportIturFileRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductSimpleRepository), typeof(ExportInventProductSimpleERPFileRepository), ExportRepositoryEnum.ExportInventProductSimpleERPFileRepository.ToString(), new ContainerControlledLifetimeManager());
				//this._container.RegisterType(typeof(IUploadCatalogFileRepository), typeof(UploadCatalogFileRepository), ExportRepositoryEnum.UploadCatalogFileRepository.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportCurrentInventorAdvancedRepository), typeof(ExportCurrentInventorAdvancedERPRepository), ExportRepositoryEnum.ExportCurrentInventorAdvancedERPRepository.ToString(), new ContainerControlledLifetimeManager());
	
				
				this._container.RegisterType(typeof(IExportConfigIniRepository), typeof(ExportUserIniFileRepository), ExportRepositoryEnum.ExportUserIniFileRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportConfigIniRepository), typeof(ExportCustomerConfigFileRepository), ExportRepositoryEnum.ExportCustomerConfigFileRepository.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportConfigIniRepository), typeof(ExportCustomerMISConfigFileRepository), ExportRepositoryEnum.ExportCustomerMISConfigFileRepository.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IFileParser), typeof(CsvFileParser), FileParserEnum.CsvFileParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFileParser), typeof(ExcelFileParser), FileParserEnum.ExcelFileParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFileParser), typeof(SqliteFileParser), FileParserEnum.SqliteFileParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFileParser), typeof(ExcelMacrosFileParser), FileParserEnum.ExcelMacrosFileParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IIturParser), typeof(IturParser), IturParserEnum.IturParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturYesXlsxParserQ), IturParserEnum.IturYesXlsxParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturYesXlsxParserSN), IturParserEnum.IturYesXlsxParserSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturNativPlusLadpcParser1), IturParserEnum.IturNativPlusLadpcParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturNativPlusLadpcParser9999), IturParserEnum.IturNativPlusLadpcParser9999.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturFacingParser), IturParserEnum.IturFacingParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturERPParser), IturParserEnum.IturERPParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturDBParser), IturParserEnum.IturDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturFromDBParser1), IturParserEnum.IturFromDBParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturMultiCsv2SdfParser), IturParserEnum.IturMultiCsv2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturMultiCsv2SdfParser1), IturParserEnum.IturMultiCsv2SdfParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IIturParser), typeof(IturMerkavaXslx2SdfParser), IturParserEnum.IturMerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturClalitXslx2SdfParser), IturParserEnum.IturClalitXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturNativXslx2SdfParser), IturParserEnum.IturNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturNativXslx2SdfUpdateUpLevelParser), IturParserEnum.IturNativXslx2SdfUpdateUpLevelParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturMerkavaXslx2SdfUpdateUpLevelParser), IturParserEnum.IturMerkavaXslx2SdfUpdateUpLevelParser.ToString(), new ContainerControlledLifetimeManager());


				
				this._container.RegisterType(typeof(IIturParser), typeof(IturUpdateMerkavaSqlite2SdfParser), IturParserEnum.IturUpdateMerkavaSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturUpdateClalitSqlite2SdfParser), IturParserEnum.IturUpdateClalitSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturUpdateNativSqlite2SdfParser), IturParserEnum.IturUpdateNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
																										
				this._container.RegisterType(typeof(IIturParser), typeof(IturUpdateNativSimpleSqlite2SdfParser), IturParserEnum.IturUpdateNativSimpleSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturParser), typeof(IturUpdateNativXslx2SdfParser), IturParserEnum.IturUpdateNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
	
				
				

				this._container.RegisterType(typeof(ITemporaryInventorySQLiteParser), typeof(TemporaryInventoryFromDeletedInventorySqlite2SdfParser), TemporaryInventorySQLiteParserEnum.TemporaryInventoryFromDeletedInventorySqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ITemporaryInventorySQLiteParser), typeof(TemporaryInventoryFromAddedInventorySqlite2SdfParser), TemporaryInventorySQLiteParserEnum.TemporaryInventoryFromAddedInventorySqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				//this._container.RegisterType(typeof(ITemporaryInventorySQLiteParser), typeof(TemporaryInventoryNativSqlite2SdfParser), TemporaryInventorySQLiteParserEnum.TemporaryInventoryNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IShelfParser), typeof(ShelfFacingParser), ImportShelfParserEnum.ShelfFacingParser.ToString(), new ContainerControlledLifetimeManager());
	
				
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationParser), LocationParserEnum.LocationParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationXlsxParser), LocationParserEnum.LocationXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationYesXlsxParserQ), LocationParserEnum.LocationYesXlsxParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationYesXlsxParserSN), LocationParserEnum.LocationYesXlsxParserSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationNativPlusLadpcParser), LocationParserEnum.LocationNativPlusLadpcParser.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationEmulationParser), LocationParserEnum.LocationEmulationParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationFromDBParser), LocationParserEnum.LocationFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationMultyCsvParser), LocationParserEnum.LocationMultyCsvParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationUpdateTagParser), LocationParserEnum.LocationUpdateTagParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationMerkavaXslx2SdfParser), LocationParserEnum.LocationMerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationClalitXslx2SdfParser), LocationParserEnum.LocationClalitXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationNativXslx2SdfParser), LocationParserEnum.LocationNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationMerkavaXslx2SqliteParser), LocationSQLiteParserEnum.LocationMerkavaXslx2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationMerkavaSdf2SqliteParser), LocationSQLiteParserEnum.LocationMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationClalitSdf2SqliteParser), LocationSQLiteParserEnum.LocationClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationNativSdf2SqliteParser), LocationSQLiteParserEnum.LocationNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationNativPlusMISSdf2SqliteParser), LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationNativPlusMISSdf2SqliteParserIturCode), LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParserIturCode.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(LocationNativPlusMISSdf2SqliteParserERP), LocationSQLiteParserEnum.LocationNativPlusMISSdf2SqliteParserERP.ToString(), new ContainerControlledLifetimeManager());
			
				
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationUpdateMerkavaSqlite2SdfParser), LocationParserEnum.LocationUpdateMerkavaSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationUpdateClalitSqlite2SdfParser), LocationParserEnum.LocationUpdateClalitSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ILocationParser), typeof(LocationUpdateNativSqlite2SdfParser), LocationParserEnum.LocationUpdateNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
																																									   
				this._container.RegisterType(typeof(ILocationSQLiteParser), typeof(IturMerkavaSdf2SqliteParser), LocationSQLiteParserEnum.IturMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IDocumentHeaderParser), typeof(DocumentHeaderFromDBParser), DocumentHeaderParseEnum.DocumentHeaderFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IDocumentHeaderParser), typeof(DocumentHeaderAddFristDocToIturParser), DocumentHeaderParseEnum.DocumentHeaderAddFristDocToIturParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IDocumentHeaderParser), typeof(DocumentHeaderAddSpetialDocToIturParser), DocumentHeaderParseEnum.DocumentHeaderAddSpetialDocToIturParser.ToString(), new ContainerControlledLifetimeManager());
													   

				this._container.RegisterType(typeof(ICurrentInventorSQLiteParser), typeof(CurrentInventoryMerkavaSdf2SqliteParser), CurrentInventorSQLiteParserEnum.CurrentInventoryMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICurrentInventorSQLiteParser), typeof(CurrentInventoryClalitSdf2SqliteParser), CurrentInventorSQLiteParserEnum.CurrentInventoryClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICurrentInventorSQLiteParser), typeof(CurrentInventoryNativSdf2SqliteParser), CurrentInventorSQLiteParserEnum.CurrentInventoryNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICurrentInventorSQLiteParser), typeof(CurrentInventoryNativPlusSdf2SqliteParser), CurrentInventorSQLiteParserEnum.CurrentInventoryNativPlusSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
															   																																																				 
			   

				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr6MerkavaXslx2SdfParser), PropertyStrParserEnum.PropertyStr6MerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr7MerkavaXslx2SdfParser), PropertyStrParserEnum.PropertyStr7MerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr6MerkavaXslx2SdfParser1), PropertyStrParserEnum.PropertyStr6MerkavaXslx2SdfParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr18MerkavaXslx2SdfParser), PropertyStrParserEnum.PropertyStr18MerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr1NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr1NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr2NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr2NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr3NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr3NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr4NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr4NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr5NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr5NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr6NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr6NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr7NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr7NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr8NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr8NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr9NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr9NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStr10NativXslx2SdfParser), PropertyStrParserEnum.PropertyStr10NativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrNativSqlite2SdfParser), PropertyStrParserEnum.PropertyStrNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				 

				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrClalitXslx2SdfParser1), PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrClalitXslx2SdfParser2), PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrClalitXslx2SdfParser3), PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrClalitXslx2SdfParser4), PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrClalitXslx2SdfParser5), PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser5.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrBuildingConfigMerkavaXslx2SdfParser), PropertyStrParserEnum.PropertyStrBuildingConfigMerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrBuildingConfigClalitXslx2SdfParser), PropertyStrParserEnum.PropertyStrBuildingConfigClalitXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrBuildingConfigNativXslx2SdfParser), PropertyStrParserEnum.PropertyStrBuildingConfigNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrProfileNativXslx2SdfParser), PropertyStrParserEnum.PropertyStrProfileNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrProfileXml2SdfParser), PropertyStrParserEnum.PropertyStrProfileXml2SdfParser.ToString(), new ContainerControlledLifetimeManager());

																									  
		
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrPropertyDecoratorNativXslx2SdfParser), PropertyStrParserEnum.PropertyStrPropertyDecoratorNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrPropertyDecoratorNativExportErpParser1), PropertyStrParserEnum.PropertyStrPropertyDecoratorNativExportErpParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrPropertyDecoratorNativExportErpParser2), PropertyStrParserEnum.PropertyStrPropertyDecoratorNativExportErpParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrPropertyDecoratorNativExportErpParser3), PropertyStrParserEnum.PropertyStrPropertyDecoratorNativExportErpParser3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrParser), typeof(PropertyStrPropertyDecoratorNativExportErpParser4), PropertyStrParserEnum.PropertyStrPropertyDecoratorNativExportErpParser4.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IPropertyStrListSQLiteParser), typeof(PropertyStrListDefaultXslx2SqliteParser), PropertyStrListSQLiteParserEnum.PropertyStrListDefaultXslx2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrListSQLiteParser), typeof(PropertyStrListMerkavaSdf2SqliteParser), PropertyStrListSQLiteParserEnum.PropertyStrListMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrListSQLiteParser), typeof(PropertyStrListClalitSdf2SqliteParser), PropertyStrListSQLiteParserEnum.PropertyStrListClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPropertyStrListSQLiteParser), typeof(PropertyStrListNativSdf2SqliteParser), PropertyStrListSQLiteParserEnum.PropertyStrListNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
																																  

				this._container.RegisterType(typeof(ISectionParser), typeof(SectionNetPOSSuperPharmParser), SectionParserEnum.SectionNetPOSSuperPharmParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionDBParser), SectionParserEnum.SectionDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionGeneralCSVParser), SectionParserEnum.SectionGeneralCSVParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionGeneralXLSXParser), SectionParserEnum.SectionGeneralXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionNativXslx2SdfParser), SectionParserEnum.SectionNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionNativXslx2SdfParser1), SectionParserEnum.SectionNativXslx2SdfParser1.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionMikramSonolParser), SectionParserEnum.SectionMikramSonolParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionNimrodAvivParser), SectionParserEnum.SectionNimrodAvivParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionH_MParser), SectionParserEnum.SectionH_MParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionH_M_NewParser), SectionParserEnum.SectionH_M_NewParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionGazitGlobalXlsxParser), SectionParserEnum.SectionGazitGlobalXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionFRSVisionMirkamParser), SectionParserEnum.SectionFRSVisionMirkamParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionMPLParser), SectionParserEnum.SectionMPLParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionTafnitMatrixParser), SectionParserEnum.SectionTafnitMatrixParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionSapb1ZometsfarimParser), SectionParserEnum.SectionSapb1ZometsfarimParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionSapb1ZometsfarimParser1), SectionParserEnum.SectionSapb1ZometsfarimParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionGazitVerifoneSteimaztzkyParser), SectionParserEnum.SectionGazitVerifoneSteimaztzkyParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionGazitVerifoneSteimaztzkyParser1), SectionParserEnum.SectionGazitVerifoneSteimaztzkyParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionAS400MegaParser), SectionParserEnum.SectionAS400MegaParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISectionParser), typeof(SectionAS400HamashbirParser), SectionParserEnum.SectionAS400HamashbirParser.ToString(), new ContainerControlledLifetimeManager());
				
				
	
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierDBParser), SupplierParserEnum.SupplierDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierAvivPOSParser), SupplierParserEnum.SupplierAvivPOSParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierComaxASPParser), SupplierParserEnum.SupplierComaxASPParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierDefaultParser), SupplierParserEnum.SupplierDefaultParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierMade4NetParser), SupplierParserEnum.SupplierMade4NetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierNibitParser), SupplierParserEnum.SupplierNibitParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierAS400AprilParser), SupplierParserEnum.SupplierAS400AprilParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierFRSVisionMirkamParser), SupplierParserEnum.SupplierFRSVisionMirkamParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierGeneralXLSXParser), SupplierParserEnum.SupplierGeneralXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierNativXslx2SdfParser), SupplierParserEnum.SupplierNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
	
				
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierSapb1ZometsfarimParser), SupplierParserEnum.SupplierSapb1ZometsfarimParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ISupplierParser), typeof(SupplierAS400MegaParser), SupplierParserEnum.SupplierAS400MegaParser.ToString(), new ContainerControlledLifetimeManager());

				
				

				this._container.RegisterType(typeof(IUnitPlanParser), typeof(UnitPlanParser), UnitPlanParserEnum.UnitPlanParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IUnitPlanParser), typeof(UnitPlanFromDBParser), UnitPlanParserEnum.UnitPlanFromDBParser.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyPriorityRenuarParser), FamilyParserEnum.FamilyPriorityRenuarParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyDefaultParser), FamilyParserEnum.FamilyDefaultParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyLadyComfortParser), FamilyParserEnum.FamilyLadyComfortParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyH_MParser), FamilyParserEnum.FamilyH_MParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyH_M_NewParser), FamilyParserEnum.FamilyH_M_NewParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyPriorityKedsShowRoomParser), FamilyParserEnum.FamilyPriorityKedsShowRoomParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyTafnitMatrixParser), FamilyParserEnum.FamilyTafnitMatrixParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyAS400HamashbirParser), FamilyParserEnum.FamilyAS400HamashbirParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyGeneralXLSXParser), FamilyParserEnum.FamilyGeneralXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IFamilyParser), typeof(FamilyNativXslx2SdfParser), FamilyParserEnum.FamilyNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());

																							 
											 
				
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForUnizagParser), ProductSimpleParserEnum.ProductCatalogForUnizagParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForUnizagParser1), ProductSimpleParserEnum.ProductCatalogForUnizagParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForUnizagParser2), ProductSimpleParserEnum.ProductCatalogForUnizagParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForUnizagParser3), ProductSimpleParserEnum.ProductCatalogForUnizagParser3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForComaxASPParser), ProductSimpleParserEnum.ProductCatalogForComaxASPParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForComaxASPParser1), ProductSimpleParserEnum.ProductCatalogForComaxASPParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogComaxASPMultiBarcodeParser), ProductSimpleParserEnum.ProductCatalogComaxASPMultiBarcodeParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogComaxASPMultiBarcodeParser1), ProductSimpleParserEnum.ProductCatalogComaxASPMultiBarcodeParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogComaxASPMultiBarcodeParser2), ProductSimpleParserEnum.ProductCatalogComaxASPMultiBarcodeParser2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGazitVerifoneParser), ProductSimpleParserEnum.ProductCatalogForGazitVerifoneParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductHamarotForGazitVerifoneParser2), ProductSimpleParserEnum.ProductHamarotForGazitVerifoneParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductHamarotForGazitVerifoneParser1), ProductSimpleParserEnum.ProductHamarotForGazitVerifoneParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForPriorityRenuarParser), ProductSimpleParserEnum.ProductCatalogForPriorityRenuarParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForPriorityRenuarParser1), ProductSimpleParserEnum.ProductCatalogForPriorityRenuarParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForNetPOSSuperPharmParser), ProductSimpleParserEnum.ProductCatalogForNetPOSSuperPharmParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForNetPOSSuperPharmParser1), ProductSimpleParserEnum.ProductCatalogForNetPOSSuperPharmParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductFromDBParser), ProductSimpleParserEnum.ProductFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductNetPOSSuperPharmParserUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductNetPOSSuperPharmParserUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductFromDBUpdateERPQuentetyParser), ProductSimpleParserEnum.ProductFromDBUpdateERPQuentetyParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForXtechMeuhedetParser1), ProductSimpleParserEnum.ProductCatalogForXtechMeuhedetParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForXtechMeuhedetParser2), ProductSimpleParserEnum.ProductCatalogForXtechMeuhedetParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForXtechMeuhedetUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogForXtechMeuhedetUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaParser), ProductSimpleParserEnum.ProductCatalogForYarpaParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaParser1), ProductSimpleParserEnum.ProductCatalogForYarpaParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogForYarpaUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaUpdateERPQuentetyDBParser2), ProductSimpleParserEnum.ProductCatalogForYarpaUpdateERPQuentetyDBParser2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaWindowsParser), ProductSimpleParserEnum.ProductCatalogForYarpaWindowsParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaWindowsParser1), ProductSimpleParserEnum.ProductCatalogForYarpaWindowsParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForYarpaUpdateERPQuentetyWindowsDBParser1), ProductSimpleParserEnum.ProductCatalogForYarpaUpdateERPQuentetyWindowsDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralCSVParser), ProductSimpleParserEnum.ProductCatalogForGeneralCSVParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralCSVParser1), ProductSimpleParserEnum.ProductCatalogForGeneralCSVParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralCSVUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogForGeneralCSVUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralXLSXParser), ProductSimpleParserEnum.ProductCatalogForGeneralXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralXLSXParser1), ProductSimpleParserEnum.ProductCatalogForGeneralXLSXParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser2), ProductSimpleParserEnum.ProductCatalogForGeneralXLSXUpdateERPQuentetyDBParser2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogXtechMeuhedetXLSXParser), ProductSimpleParserEnum.ProductCatalogXtechMeuhedetXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogXtechMeuhedetXLSXParser1), ProductSimpleParserEnum.ProductCatalogXtechMeuhedetXLSXParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogXtechMeuhedetXLSXParser2), ProductSimpleParserEnum.ProductCatalogXtechMeuhedetXLSXParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogXtechMeuhedetXLSXUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogXtechMeuhedetXLSXUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivPOSParser), ProductSimpleParserEnum.ProductCatalogAvivPOSParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivPOSParser1), ProductSimpleParserEnum.ProductCatalogAvivPOSParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivPOSParser2), ProductSimpleParserEnum.ProductCatalogAvivPOSParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivPOSParser3), ProductSimpleParserEnum.ProductCatalogAvivPOSParser3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivPOSUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogAvivPOSUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivMultiParser), ProductSimpleParserEnum.ProductCatalogAvivMultiParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivMultiParser1), ProductSimpleParserEnum.ProductCatalogAvivMultiParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAvivMultiUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogAvivMultiUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMikramSonolParser), ProductSimpleParserEnum.ProductCatalogMikramSonolParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMikramSonolParser1), ProductSimpleParserEnum.ProductCatalogMikramSonolParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMikramSonolUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogMikramSonolUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityKedsParser), ProductSimpleParserEnum.ProductCatalogPriorityKedsParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityKedsParser1), ProductSimpleParserEnum.ProductCatalogPriorityKedsParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityKedsUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogPriorityKedsUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductParser), typeof(ProductNetPOSSuperPharmParser), ProductSimpleParserEnum.ProductNetPOSSuperPharmParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductRetalixPosHOParser), ProductSimpleParserEnum.ProductRetalixPosHOParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductRetalixPosHOParserUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductRetalixPosHOParserUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400LeumitParser), ProductSimpleParserEnum.ProductCatalogAS400LeumitParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400LeumitParser1), ProductSimpleParserEnum.ProductCatalogAS400LeumitParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400LeumitUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogAS400LeumitUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogRetalixNextParser), ProductSimpleParserEnum.ProductCatalogRetalixNextParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogRetalixNextParser1), ProductSimpleParserEnum.ProductCatalogRetalixNextParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductRetalixNextUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductRetalixNextUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMiniSoftParser), ProductSimpleParserEnum.ProductCatalogMiniSoftParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMiniSoftParser1), ProductSimpleParserEnum.ProductCatalogMiniSoftParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOne1Parser), ProductSimpleParserEnum.ProductCatalogOne1Parser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOne1Parser1), ProductSimpleParserEnum.ProductCatalogOne1Parser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOne1UpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogOne1UpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOtechParser), ProductSimpleParserEnum.ProductCatalogOtechParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOtechParser1), ProductSimpleParserEnum.ProductCatalogOtechParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOtechParser2), ProductSimpleParserEnum.ProductCatalogOtechParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOtechUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogOtechUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());


                this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AmericanEagleParser), ProductSimpleParserEnum.ProductCatalogAS400AmericanEagleParser.ToString(), new ContainerControlledLifetimeManager());
                this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AmericanEagleParser1), ProductSimpleParserEnum.ProductCatalogAS400AmericanEagleParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AmericanEagleParser2), ProductSimpleParserEnum.ProductCatalogAS400AmericanEagleParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AmericanEagleParser3), ProductSimpleParserEnum.ProductCatalogAS400AmericanEagleParser3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMaccabiPharmSAPParser), ProductSimpleParserEnum.ProductCatalogMaccabiPharmSAPParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMaccabiPharmSAPParser1), ProductSimpleParserEnum.ProductCatalogMaccabiPharmSAPParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMaccabiPharmSAPParser2), ProductSimpleParserEnum.ProductCatalogMaccabiPharmSAPParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMaccabiPharmSAPUpdateERPQuentetyParser), ProductSimpleParserEnum.ProductCatalogMaccabiPharmSAPUpdateERPQuentetyParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMikiKupotParser), ProductSimpleParserEnum.ProductCatalogMikiKupotParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogLadyComfortParser), ProductSimpleParserEnum.ProductCatalogLadyComfortParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMade4NetParser), ProductSimpleParserEnum.ProductCatalogMade4NetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMade4NetParser1), ProductSimpleParserEnum.ProductCatalogMade4NetParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400JaforaParser), ProductSimpleParserEnum.ProductCatalogAS400JaforaParser.ToString(), new ContainerControlledLifetimeManager());
	

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNibitParser), ProductSimpleParserEnum.ProductCatalogNibitParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNibitParser0), ProductSimpleParserEnum.ProductCatalogNibitParser0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNibitParser1), ProductSimpleParserEnum.ProductCatalogNibitParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNibitUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogNibitUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenOriginalsParser), ProductSimpleParserEnum.ProductCatalogOrenOriginalsParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenOriginalsParser1), ProductSimpleParserEnum.ProductCatalogOrenOriginalsParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenOriginalsParser2), ProductSimpleParserEnum.ProductCatalogOrenOriginalsParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenOriginalsUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogOrenOriginalsUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNimrodAvivParser), ProductSimpleParserEnum.ProductCatalogNimrodAvivParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNimrodAvivParser1), ProductSimpleParserEnum.ProductCatalogNimrodAvivParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNimrodAvivParser2), ProductSimpleParserEnum.ProductCatalogNimrodAvivParser2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_MParser), ProductSimpleParserEnum.ProductCatalogH_MParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_MParser1), ProductSimpleParserEnum.ProductCatalogH_MParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_MParser2), ProductSimpleParserEnum.ProductCatalogH_MParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_MParser3), ProductSimpleParserEnum.ProductCatalogH_MParser3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_M_NewParser), ProductSimpleParserEnum.ProductCatalogH_M_NewParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_M_NewParser2), ProductSimpleParserEnum.ProductCatalogH_M_NewParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogH_M_NewParser3), ProductSimpleParserEnum.ProductCatalogH_M_NewParser3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenMutagimParser), ProductSimpleParserEnum.ProductCatalogOrenMutagimParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogOrenMutagimParser1), ProductSimpleParserEnum.ProductCatalogOrenMutagimParser1.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AprilParser), ProductSimpleParserEnum.ProductCatalogAS400AprilParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AprilParser1), ProductSimpleParserEnum.ProductCatalogAS400AprilParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400AprilUpdateERPQuentetyParser), ProductSimpleParserEnum.ProductCatalogAS400AprilUpdateERPQuentetyParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HamashbirParser), ProductSimpleParserEnum.ProductCatalogAS400HamashbirParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HamashbirParser1), ProductSimpleParserEnum.ProductCatalogAS400HamashbirParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HamashbirUpdateERPQuentetyParser), ProductSimpleParserEnum.ProductCatalogAS400HamashbirUpdateERPQuentetyParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HamashbirUpdateERPQuentetyParser2), ProductSimpleParserEnum.ProductCatalogAS400HamashbirUpdateERPQuentetyParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HamashbirUpdateERPQuentetyParser1), ProductSimpleParserEnum.ProductCatalogAS400HamashbirUpdateERPQuentetyParser1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogBazanCsvParser), ProductSimpleParserEnum.ProductCatalogNesherParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNesherParser), ProductSimpleParserEnum.ProductCatalogNesherParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityKedsShowRoomParser), ProductSimpleParserEnum.ProductCatalogPriorityKedsShowRoomParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogHashParser), ProductSimpleParserEnum.ProductCatalogHashParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogHashParser1), ProductSimpleParserEnum.ProductCatalogHashParser1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400MangoParser), ProductSimpleParserEnum.ProductCatalogAS400MangoParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400MangoParser1), ProductSimpleParserEnum.ProductCatalogAS400MangoParser1.ToString(), new ContainerControlledLifetimeManager());



				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitLeeCooperParser), ProductSimpleParserEnum.ProductCatalogGazitLeeCooperParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitLeeCooperParser1), ProductSimpleParserEnum.ProductCatalogGazitLeeCooperParser1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogFRSVisionMirkamParser), ProductSimpleParserEnum.ProductCatalogFRSVisionMirkamParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogFRSVisionMirkamParser1), ProductSimpleParserEnum.ProductCatalogFRSVisionMirkamParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HonigmanParser), ProductSimpleParserEnum.ProductCatalogAS400HonigmanParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HonigmanParser1), ProductSimpleParserEnum.ProductCatalogAS400HonigmanParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HonigmanParser2), ProductSimpleParserEnum.ProductCatalogAS400HonigmanParser2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMPLParser), ProductSimpleParserEnum.ProductCatalogMPLParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMPLParser1), ProductSimpleParserEnum.ProductCatalogMPLParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMPLParser2), ProductSimpleParserEnum.ProductCatalogMPLParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMPLUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogMPLUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogTafnitMatrixParser), ProductSimpleParserEnum.ProductCatalogTafnitMatrixParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogTafnitMatrixParser1), ProductSimpleParserEnum.ProductCatalogTafnitMatrixParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogTafnitMatrixParser2), ProductSimpleParserEnum.ProductCatalogTafnitMatrixParser2.ToString(), new ContainerControlledLifetimeManager());
                this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogTafnitMatrixUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogTafnitMatrixUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityCastroParser), ProductSimpleParserEnum.ProductCatalogPriorityCastroParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityCastroParser1), ProductSimpleParserEnum.ProductCatalogPriorityCastroParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAutosoftParser), ProductSimpleParserEnum.ProductCatalogAutosoftParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAutosoftParser1), ProductSimpleParserEnum.ProductCatalogAutosoftParser1.ToString(), new ContainerControlledLifetimeManager());
			    this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAutosoftParser2), ProductSimpleParserEnum.ProductCatalogAutosoftParser2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNikeIntParser), ProductSimpleParserEnum.ProductCatalogNikeIntParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNikeIntParser1), ProductSimpleParserEnum.ProductCatalogNikeIntParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNikeIntParser2), ProductSimpleParserEnum.ProductCatalogNikeIntParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNikeIntUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogNikeIntUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPrioritytEsteeLouderUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogPrioritytEsteeLouderUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());
				

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityAldoParser), ProductSimpleParserEnum.ProductCatalogPriorityAldoParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPriorityAldoParser1), ProductSimpleParserEnum.ProductCatalogPriorityAldoParser1.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1ZometsfarimParser), ProductSimpleParserEnum.ProductCatalogSapb1ZometsfarimParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1ZometsfarimParser1), ProductSimpleParserEnum.ProductCatalogSapb1ZometsfarimParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1ZometsfarimParser2), ProductSimpleParserEnum.ProductCatalogSapb1ZometsfarimParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1ZometsfarimParser3), ProductSimpleParserEnum.ProductCatalogSapb1ZometsfarimParser3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1ZometsfarimUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogSapb1ZometsfarimUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HoParser), ProductSimpleParserEnum.ProductCatalogAS400HoParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HoParser1), ProductSimpleParserEnum.ProductCatalogAS400HoParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HoParser2_2), ProductSimpleParserEnum.ProductCatalogAS400HoParser2_2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400HoParser2_1), ProductSimpleParserEnum.ProductCatalogAS400HoParser2_1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogWarehouseXslxParser), ProductSimpleParserEnum.ProductCatalogWarehouseXslxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogWarehouseXslxParser1), ProductSimpleParserEnum.ProductCatalogWarehouseXslxParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogWarehouseXslxParser2), ProductSimpleParserEnum.ProductCatalogWarehouseXslxParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogWarehouseXslxParser3), ProductSimpleParserEnum.ProductCatalogWarehouseXslxParser3.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogYesXslxParserSN), ProductSimpleParserEnum.ProductCatalogYesXslxParserSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogYesXslxParserQ), ProductSimpleParserEnum.ProductCatalogYesXslxParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNativPlusLadpcParser), ProductSimpleParserEnum.ProductCatalogNativPlusLadpcParser.ToString(), new ContainerControlledLifetimeManager());
				

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1XslxParser), ProductSimpleParserEnum.ProductCatalogSapb1XslxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1XslxParser1), ProductSimpleParserEnum.ProductCatalogSapb1XslxParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogSapb1XslxUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogSapb1XslxUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMerkavaXslx2SdfParser), ProductSimpleParserEnum.ProductCatalogMerkavaXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMerkavaXslx2SdfParser1), ProductSimpleParserEnum.ProductCatalogMerkavaXslx2SdfParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogMerkavaXslx2SdfUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogMerkavaXslx2SdfUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogClalitXslx2SdfParser), ProductSimpleParserEnum.ProductCatalogClalitXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNativXslx2SdfParser), ProductSimpleParserEnum.ProductCatalogNativXslx2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNativSqlite2SdfParser), ProductSimpleParserEnum.ProductCatalogNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNativMISSqlite2SdfParser), ProductSimpleParserEnum.ProductCatalogNativMISSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogNativXslx2SdfUpdateERPQuentetyDBParser1), ProductSimpleParserEnum.ProductCatalogNativXslx2SdfUpdateERPQuentetyDBParser1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitVerifoneSteimaztzkyParser), ProductSimpleParserEnum.ProductCatalogGazitVerifoneSteimaztzkyParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitVerifoneSteimaztzkyParser1), ProductSimpleParserEnum.ProductCatalogGazitVerifoneSteimaztzkyParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyDBParser), ProductSimpleParserEnum.ProductCatalogGazitVerifoneSteimaztzkyUpdateERPQuentetyDBParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400MegaParser), ProductSimpleParserEnum.ProductCatalogAS400MegaParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400MegaParser1), ProductSimpleParserEnum.ProductCatalogAS400MegaParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogAS400MegaUpdateERPQuentetyParser), ProductSimpleParserEnum.ProductCatalogAS400MegaUpdateERPQuentetyParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPrioritySweetGirlXLSXParser), ProductSimpleParserEnum.ProductCatalogPrioritySweetGirlXLSXParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitGlobalXlsxXParser), ProductSimpleParserEnum.ProductCatalogGazitGlobalXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogYtungXlsxParser), ProductSimpleParserEnum.ProductCatalogYtungXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogPrioritytEsteeLouderXslxParser), ProductSimpleParserEnum.ProductCatalogPrioritytEsteeLouderXslxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IProductSimpleParser), typeof(ProductCatalogGazitAlufHaSportXlsxParser), ProductSimpleParserEnum.ProductCatalogGazitAlufHaSportXlsxParser.ToString(), new ContainerControlledLifetimeManager());
																																	 
				  
				
				

				this._container.RegisterType(typeof(ICatalogSQLiteParser), typeof(CatalogMerkavaXslx2SqliteParser), CatalogSQLiteParserEnum.CatalogMerkavaXslx2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICatalogSQLiteParser), typeof(CatalogMerkavaSdf2SqliteParser), CatalogSQLiteParserEnum.CatalogMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICatalogSQLiteParser), typeof(CatalogClalitSdf2SqliteParser), CatalogSQLiteParserEnum.CatalogClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICatalogSQLiteParser), typeof(CatalogNativSdf2SqliteParser), CatalogSQLiteParserEnum.CatalogNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ICatalogSQLiteParser), typeof(CatalogNativPlusMISSdf2SqliteParser), CatalogSQLiteParserEnum.CatalogNativPlusMISSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IBuildingConfigParser), typeof(BuildingConfigMerkavaXslxParser), BuildingConfigParserEnum.BuildingConfigMerkavaXslxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBuildingConfigParser), typeof(BuildingConfigMerkavaSdf2SqliteParser), BuildingConfigParserEnum.BuildingConfigMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBuildingConfigParser), typeof(BuildingConfigNativSdf2SqliteParser), BuildingConfigParserEnum.BuildingConfigNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBuildingConfigParser), typeof(BuildingConfigNativPlusMISSdf2SqliteParser), BuildingConfigParserEnum.BuildingConfigNativPlusMISSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBuildingConfigParser), typeof(BuildingConfigClalitSdf2SqliteParser), BuildingConfigParserEnum.BuildingConfigClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryMerkavaXslx2SqliteParser), PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaXslx2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryMerkavaSdf2SqliteParser), PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativSdf2SqliteParser), PreviousInventorySQLiteParserEnum.PreviousInventoryNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryClalitSdf2SqliteParser), PreviousInventorySQLiteParserEnum.PreviousInventoryClalitSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryMerkavaXslx2DbSetParser), PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaXslx2DbSetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryMerkavaXslx2DictiontyParser), PreviousInventorySQLiteParserEnum.PreviousInventoryMerkavaXslx2DictiontyParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(ITemplateInventorySQLiteParser), typeof(TemplateInventoryNativSdf2SqliteParser), TemplateInventorySQLiteParserEnum.TemplateInventoryNativSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryClalitXslx2DbSetParser), PreviousInventorySQLiteParserEnum.PreviousInventoryClalitXslx2DbSetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativXslx2DbSetParser), PreviousInventorySQLiteParserEnum.PreviousInventoryNativXslx2DbSetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativPlusXslx2DbSetParser), PreviousInventorySQLiteParserEnum.PreviousInventoryNativPlusXslx2DbSetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativPlusYesXlsxDbSetParserQ), PreviousInventorySQLiteParserEnum.PreviousInventoryNativPlusYesXlsxDbSetParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativPlusYesXlsxDbSetParserSN), PreviousInventorySQLiteParserEnum.PreviousInventoryNativPlusYesXlsxDbSetParserSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IPreviousInventorySQLiteParser), typeof(PreviousInventoryNativPlusLadpc2DbSetParser), PreviousInventorySQLiteParserEnum.PreviousInventoryNativPlusLadpc2DbSetParser.ToString(), new ContainerControlledLifetimeManager());

				
				
				this._container.RegisterType(typeof(ITemplateInventorySQLiteParser), typeof(TemplateInventoryNativPlusXslx2DbSetParser), TemplateInventorySQLiteParserEnum.TemplateInventoryNativPlusXslx2DbSetParser.ToString(), new ContainerControlledLifetimeManager());

					 
														
			//	this._container.RegisterType(typeof(ITemporaryInventorySQLiteParser), typeof(TemporaryInventoryMerkavaSdf2SqliteParser), TemporaryInventorySQLiteParserEnum.TemporaryInventoryMerkavaSdf2SqliteParser.ToString(), new ContainerControlledLifetimeManager());
				
				
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchXtechMeuhedetParser), BranchParserEnum.BranchXtechMeuhedetParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchDefaultParser), BranchParserEnum.BranchDefaultParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchDefaultXlsxParser), BranchParserEnum.BranchDefaultXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchXtechMeuhedetXlsxParser), BranchParserEnum.BranchXtechMeuhedetXlsxParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchGazitVerifoneParser), BranchParserEnum.BranchGazitVerifoneParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchMikramSonolParser), BranchParserEnum.BranchMikramSonolParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchAS400LeumitParser), BranchParserEnum.BranchAS400LeumitParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchAS400HonigmanParser), BranchParserEnum.BranchAS400HonigmanParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IBranchParser), typeof(BranchPriorityCastroParser), BranchParserEnum.BranchPriorityCastroParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMultiCsvParser), InventProductSimpleParserEnum.InventProductMultiCsvParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductYesXlsxParserQ), InventProductSimpleParserEnum.InventProductYesXlsxParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductYesXlsxParserSN), InventProductSimpleParserEnum.InventProductYesXlsxParserSN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductSimpleParser), InventProductSimpleParserEnum.InventProductSimpleParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductSimpleYarpaParser), InventProductSimpleParserEnum.InventProductSimpleYarpaParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductParser), InventProductSimpleParserEnum.InventProductParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMisParser), InventProductSimpleParserEnum.InventProductMisParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductNativPlusMISSqlite2SdfParser), InventProductSimpleParserEnum.InventProductNativPlusMISSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMISSqlite2SdfParser), InventProductSimpleParserEnum.InventProductMISSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMerkavaXlsxParserQ), InventProductSimpleParserEnum.InventProductMerkavaXlsxParserQ.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMerkavaXlsxParserSN), InventProductSimpleParserEnum.InventProductMerkavaXlsxParserSN.ToString(), new ContainerControlledLifetimeManager());
				

				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductDB3Parser), InventProductSimpleParserEnum.InventProductDB3Parser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMerkavaSqlite2SdfParser), InventProductSimpleParserEnum.InventProductMerkavaSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductClalitSqlite2SdfParser), InventProductSimpleParserEnum.InventProductClalitSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductNativSqlite2SdfParser), InventProductSimpleParserEnum.InventProductNativSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductNativPlusSqlite2SdfParser), InventProductSimpleParserEnum.InventProductNativPlusSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				//this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductNativPlusMISSqlite2SdfParser), InventProductSimpleParserEnum.InventProductNativPlusMISSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductWarehouseParser), InventProductSimpleParserEnum.InventProductWarehouseParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductDefaultBackupParser), InventProductSimpleParserEnum.InventProductDefaultBackupParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductFromDBParser), InventProductSimpleParserEnum.InventProductFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductFromNativDBParser), InventProductSimpleParserEnum.InventProductFromNativDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturMakatFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturMakatFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturMakatSNumberFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturMakatSNumberFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturMakatSNumberProp10FromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturMakatSNumberProp10FromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturBarcodeSNumberFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturBarcodeSNumberFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturBarcodeSNumberProp10FromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturBarcodeSNumberProp10FromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturBarcodeFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturBarcodeFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductFromDBAfterCompareParser), InventProductSimpleParserEnum.InventProductFromDBAfterCompareParser.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateCompare2SumByIturMakatFromDBParser1), InventProductSimpleParserEnum.InventProductUpdateCompare2SumByIturMakatFromDBParser1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateCompare2SumByIturMakatFromDBParser2), InventProductSimpleParserEnum.InventProductUpdateCompare2SumByIturMakatFromDBParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2SumByIturDocMakatFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2SumByIturDocMakatFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateMakat2BarcodeDBParser), InventProductSimpleParserEnum.InventProductUpdateMakat2BarcodeDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2MakatAndSNFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2MakatAndSNFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdate2BarcodeAndSNFromDBParser), InventProductSimpleParserEnum.InventProductUpdate2BarcodeAndSNFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				
																																															   
				
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductMinusByMakatFromDBParser), InventProductSimpleParserEnum.InventProductMinusByMakatFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateDBParser), InventProductSimpleParserEnum.InventProductUpdateDBParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateDBParser2), InventProductSimpleParserEnum.InventProductUpdateDBParser2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateDBParser4), InventProductSimpleParserEnum.InventProductUpdateDBParser4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductUpdateBarcodeFromDBParser), InventProductSimpleParserEnum.InventProductUpdateBarcodeFromDBParser.ToString(), new ContainerControlledLifetimeManager());
				
																												  
																						 
				this._container.RegisterType(typeof(IInventProductToObjectParser), typeof(InventProductToMalamXMLParser), InventProductSimpleParserEnum.InventProductToMalamXMLParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IInventProductToObjectParser), typeof(InventProductToMalamDataSetParser), InventProductSimpleParserEnum.InventProductToMalamDataSetParser.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IStatusInventProductSimpleParser), typeof(StatusInventProductNativPlusSqlite2SdfParser), StatusInventProductSimpleParserEnum.StatusInventProductNativPlusSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IStatusInventProductSimpleParser), typeof(StatusInventProductUpdateSumNativPlusSqlite2SdfParser), StatusInventProductSimpleParserEnum.StatusInventProductUpdateSumNativPlusSqlite2SdfParser.ToString(), new ContainerControlledLifetimeManager());
	
				
																	 
				
				//this._container.RegisterType(typeof(IInventProductSimpleParser), typeof(InventProductSimpleYarpaParser), InventProductSimpleParserEnum.InventProductSimpleYarpaParser.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IIturAnalyzesReader), typeof(IturAnalyzesReader), IturAnalyzesReaderEnum.IturAnalyzesReader.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesReader), typeof(IturAnalyzesSimpleReader), IturAnalyzesReaderEnum.IturAnalyzesSimpleReader.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesReader), typeof(IturAnalyzesSimpleSumReader), IturAnalyzesReaderEnum.IturAnalyzesSimpleSumReader.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IIturAnalyzesReader), typeof(IturAnalyzesFamilyReader), IturAnalyzesReaderEnum.IturAnalyzesFamilyReader.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(ICurrentInventoryAdvancedReader), typeof(CurrentInventoryAdvancedReader), CurrentInventoryAdvancedReaderEnum.CurrentInventoryAdvancedReader.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductComaxERPFileWriter), WriterEnum.ExportInventProductComaxERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGazitERPFileWriter), WriterEnum.ExportInventProductGazitERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAvivPOSERPFileWriter), WriterEnum.ExportInventProductAvivPOSERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductUnizagERPFileWriter), WriterEnum.ExportInventProductUnizagERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
 				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNetPOSSuperPharmERPFileWriter), WriterEnum.ExportInventProductNetPOSSuperPharmERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNetPOSSuperPharmERPFileWriter1), WriterEnum.ExportInventProductNetPOSSuperPharmERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter_M), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter_M.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter_T), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter_T.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter_TM), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter_TM.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter_TW), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter_TW.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter_W), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter_W.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter1), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityRenuarERPFileWriter2), WriterEnum.ExportInventProductPriorityRenuarERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter), WriterEnum.ExportInventProductOtechERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter_M), WriterEnum.ExportInventProductOtechERPFileWriter_M.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter_T), WriterEnum.ExportInventProductOtechERPFileWriter_T.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter_W), WriterEnum.ExportInventProductOtechERPFileWriter_W.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter1), WriterEnum.ExportInventProductOtechERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOtechERPFileWriter2), WriterEnum.ExportInventProductOtechERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400MegaERPFileWriter), WriterEnum.ExportInventProductAS400MegaERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400MegaERPFileWriter1), WriterEnum.ExportInventProductAS400MegaERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());



				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGazitLeeCooperERPFileWriter), WriterEnum.ExportInventProductGazitLeeCooperERPFileWriter.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HamashbirERPFileWriter), WriterEnum.ExportInventProductAS400HamashbirERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HamashbirERPFileWriter1), WriterEnum.ExportInventProductAS400HamashbirERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

												   
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductXtechMeuhedetERPFileWriter), WriterEnum.ExportInventProductXtechMeuhedetERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductXtechMeuhedetERPFileWriter1), WriterEnum.ExportInventProductXtechMeuhedetERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductYarpaERPFileWriter), WriterEnum.ExportInventProductYarpaERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductYarpaERPFileWriter1), WriterEnum.ExportInventProductYarpaERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAvivMultiERPFileWriter), WriterEnum.ExportInventProductAvivMultiERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAvivMultiERPFileWriter1), WriterEnum.ExportInventProductAvivMultiERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramSonolERPFileWriter), WriterEnum.ExportInventProductMikramSonolERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramSonolERPFileWriter1), WriterEnum.ExportInventProductMikramSonolERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramSonolSAPERPFileWriter), WriterEnum.ExportInventProductMikramSonolSAPERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramSonolSAPERPFileWriter1), WriterEnum.ExportInventProductMikramSonolSAPERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
 				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramTenERPFileWriter), WriterEnum.ExportInventProductMikramTenERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikramTenERPFileWriter1), WriterEnum.ExportInventProductMikramTenERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGeneralCSVERPFileWriter), WriterEnum.ExportInventProductGeneralCSVERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductYellowPazERPFileWriter), WriterEnum.ExportInventProductYellowPazERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGeneralCSVERPFileWriter1), WriterEnum.ExportInventProductGeneralCSVERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGeneralCSVERPFileWriter2), WriterEnum.ExportInventProductGeneralCSVERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMikiKupotERPFileWriter), WriterEnum.ExportInventProductMikiKupotERPFileWriter.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HoERPFileWriter), WriterEnum.ExportInventProductAS400HoERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HoERPFileWriter1), WriterEnum.ExportInventProductAS400HoERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HoERPFileWriter2), WriterEnum.ExportInventProductAS400HoERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductXtechMeuhedetXlsxERPFileWriter), WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductXtechMeuhedetXlsxERPFileWriter2), WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductXtechMeuhedetXlsxERPFileWriter1), WriterEnum.ExportInventProductXtechMeuhedetXlsxERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityKedsMatrixERPFileWriter), WriterEnum.ExportInventProductPriorityKedsMatrixERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityKedsRegularERPFileWriter), WriterEnum.ExportInventProductPriorityKedsRegularERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityKedsERPFileWriter1), WriterEnum.ExportInventProductPriorityKedsERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixPosHOERPFileWriter), WriterEnum.ExportInventProductRetalixPosHOERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixPosHOERPFileWriter0), WriterEnum.ExportInventProductRetalixPosHOERPFileWriter0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixPosHOERPFileWriter1), WriterEnum.ExportInventProductRetalixPosHOERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400LeumitERPFileWriter), WriterEnum.ExportInventProductAS400LeumitERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400LeumitERPFileWriter1), WriterEnum.ExportInventProductAS400LeumitERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMiniSoftERPFileWriter), WriterEnum.ExportInventProductMiniSoftERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMiniSoftERPFileWriter1), WriterEnum.ExportInventProductMiniSoftERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMiniSoftERPFileWriter2), WriterEnum.ExportInventProductMiniSoftERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixNextERPFileWriter), WriterEnum.ExportInventProductRetalixNextERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixNextERPFileWriter0), WriterEnum.ExportInventProductRetalixNextERPFileWriter0.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductRetalixNextERPFileWriter1), WriterEnum.ExportInventProductRetalixNextERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOne1ERPFileWriter), WriterEnum.ExportInventProductOne1ERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400AmericanEagleERPFileWriter), WriterEnum.ExportInventProductAS400AmericanEagleERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400AmericanEagleERPFileWriter1), WriterEnum.ExportInventProductAS400AmericanEagleERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMaccabiPharmSAPERPFileWriter), WriterEnum.ExportInventProductMaccabiPharmSAPERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMaccabiPharmSAPERPFileWriter1), WriterEnum.ExportInventProductMaccabiPharmSAPERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductLadyComfortERPFileWriter), WriterEnum.ExportInventProductLadyComfortERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductLadyComfortERPFileWriter1), WriterEnum.ExportInventProductLadyComfortERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMade4NetERPFileWriter), WriterEnum.ExportInventProductMade4NetERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMade4NetERPFileWriter1), WriterEnum.ExportInventProductMade4NetERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400JaforaERPFileWriter), WriterEnum.ExportInventProductAS400JaforaERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400JaforaERPFileWriter1), WriterEnum.ExportInventProductAS400JaforaERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400JaforaERPFileWriter2), WriterEnum.ExportInventProductAS400JaforaERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNibitERPFileWriter), WriterEnum.ExportInventProductNibitERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNibitERPFileWriter1), WriterEnum.ExportInventProductNibitERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOrenOriginalsERPFileWriter), WriterEnum.ExportInventProductOrenOriginalsERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOrenOriginalsERPFileWriter1), WriterEnum.ExportInventProductOrenOriginalsERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOrenOriginalsERPFileWriter2), WriterEnum.ExportInventProductOrenOriginalsERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNimrodAvivERPFileWriter), WriterEnum.ExportInventProductNimrodAvivERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNimrodAvivERPFileWriter1), WriterEnum.ExportInventProductNimrodAvivERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNimrodAvivERPFileWriter2), WriterEnum.ExportInventProductNimrodAvivERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNimrodAvivERPFileWriter3), WriterEnum.ExportInventProductNimrodAvivERPFileWriter3.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductH_MERPFileWriter), WriterEnum.ExportInventProductH_MERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductH_M_NewERPFileWriter), WriterEnum.ExportInventProductH_M_NewERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNesherERPFileWriter), WriterEnum.ExportInventProductNesherERPFileWriter.ToString(), new ContainerControlledLifetimeManager());

				
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductOrenMutagimERPFileWriter), WriterEnum.ExportInventProductOrenMutagimERPFileWriter.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400AprilERPFileWriter), WriterEnum.ExportInventProductAS400AprilERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400AprilERPFileWriter1), WriterEnum.ExportInventProductAS400AprilERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductBazanCsvERPFileWriter1), WriterEnum.ExportInventProductBazanCsvERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNativExportErpERPFileWriter), WriterEnum.ExportInventProductNativExportErpERPFileWriter.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityKedsShowRoomERPFileWriter), WriterEnum.ExportInventProductPriorityKedsShowRoomERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityKedsShowRoomERPFileWriter1), WriterEnum.ExportInventProductPriorityKedsShowRoomERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductHashERPFileWriter), WriterEnum.ExportInventProductHashERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductHashERPFileWriter1), WriterEnum.ExportInventProductHashERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400MangoERPFileWriter), WriterEnum.ExportInventProductAS400MangoERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400MangoERPFileWriter1), WriterEnum.ExportInventProductAS400MangoERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductFRSVisionMirkamERPFileWriter), WriterEnum.ExportInventProductFRSVisionMirkamERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductFRSVisionMirkamERPFileWriter1), WriterEnum.ExportInventProductFRSVisionMirkamERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HonigmanERPFileWriter), WriterEnum.ExportInventProductAS400HonigmanERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HonigmanERPFileWriter1), WriterEnum.ExportInventProductAS400HonigmanERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductAS400HonigmanERPFileWriter2), WriterEnum.ExportInventProductAS400HonigmanERPFileWriter2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMPLFileWriter1), WriterEnum.ExportInventProductMPLFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductMPLFileWriter2), WriterEnum.ExportInventProductMPLFileWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductTafnitMatrixERPFileWriter), WriterEnum.ExportInventProductTafnitMatrixERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductTafnitMatrixERPFileWriter1), WriterEnum.ExportInventProductTafnitMatrixERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityCastroERPFileWriter), WriterEnum.ExportInventProductPriorityCastroERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityCastroERPFileWriter1), WriterEnum.ExportInventProductPriorityCastroERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNikeIntERPFileWriter), WriterEnum.ExportInventProductNikeIntERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductNikeIntERPFileWriter1), WriterEnum.ExportInventProductNikeIntERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductYtungXslxFileWriter), WriterEnum.ExportInventProductYtungXslxFileWriter.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductWarehouseXslxFileWriter), WriterEnum.ExportInventProductWarehouseXslxFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductWarehouseXslxFileWriter1), WriterEnum.ExportInventProductWarehouseXslxFileWriter1.ToString(), new ContainerControlledLifetimeManager());
			   	this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductWarehouseXslxFileFormulaWriter), WriterEnum.ExportInventProductWarehouseXslxFileFormulaWriter.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGeneralXslxFileWriter), WriterEnum.ExportInventProductGeneralXslxFileWriter.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductSapb1XslxERPFileWriter), WriterEnum.ExportInventProductSapb1XslxERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductSapb1XslxERPFileWriter1), WriterEnum.ExportInventProductSapb1XslxERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductSapb1ZometsfarimERPFileWriter), WriterEnum.ExportInventProductSapb1ZometsfarimERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductSapb1ZometsfarimERPFileWriter1), WriterEnum.ExportInventProductSapb1ZometsfarimERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPriorityAldoERPFileWriter), WriterEnum.ExportInventProductPriorityAldoERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
	
				

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGazitVerifoneSteimaztzkyERPFileWriter), WriterEnum.ExportInventProductGazitVerifoneSteimaztzkyERPFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductGazitVerifoneSteimaztzkyERPFileWriter1), WriterEnum.ExportInventProductGazitVerifoneSteimaztzkyERPFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPrioritySweetGirlXlsxErpFileWriter), WriterEnum.ExportInventProductPrioritySweetGirlXlsxErpFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportInventProductFileWriter), typeof(ExportInventProductPrioritySweetGirlXlsxErpFileWriter1), WriterEnum.ExportInventProductPrioritySweetGirlXlsxErpFileWriter1.ToString(), new ContainerControlledLifetimeManager());

				

				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryClaitXslxWriter1), WriterEnum.ExportCurrentInventoryClaitXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter1), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter3), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter3.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter4), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter4.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter5), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter5.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter2_1), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter2_1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaXslxWriter2_2), WriterEnum.ExportCurrentInventoryMerkavaXslxWriter2_2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaGovXslxWriter2_1), WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryMerkavaGovXslxWriter2_2), WriterEnum.ExportCurrentInventoryMerkavaGovXslxWriter2_2.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativXslxWriter1), WriterEnum.ExportCurrentInventoryNativXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusXslxWriter1), WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusXslxWriter1_Q), WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_Q.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusXslxWriter1_SN), WriterEnum.ExportCurrentInventoryNativPlusXslxWriter1_SN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusXslxWriter2), WriterEnum.ExportCurrentInventoryNativPlusXslxWriter2.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusMisradApnimXslxWriter), WriterEnum.ExportCurrentInventoryNativPlusMisradApnimXslxWriter.ToString(), new ContainerControlledLifetimeManager());


				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusYesXslxWriter1), WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusYesXslxWriter1_Q), WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_Q.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusYesXslxWriter1_SN), WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter1_SN.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusYesXslxWriter2), WriterEnum.ExportCurrentInventoryNativPlusYesXslxWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusLadpcCsvWriter1), WriterEnum.ExportCurrentInventoryNativPlusLadpcCsvWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusLadpcXslxWriter2), WriterEnum.ExportCurrentInventoryNativPlusLadpcXslxWriter2.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryNativPlusMateAsherXslxWriter1), WriterEnum.ExportCurrentInventoryNativPlusMateAsherXslxWriter1.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportCurrentInventoryWriter), typeof(ExportCurrentInventoryStockSonigoXslxWriter1), WriterEnum.ExportCurrentInventoryStockSonigoXslxWriter1.ToString(), new ContainerControlledLifetimeManager());


				//this._container.RegisterType(typeof(IExportCustomerConfigFileWriter), typeof(ExportCustomerConfigFileWriter), WriterEnum.ExportCustomerConfigFileWriter.ToString(), new ContainerControlledLifetimeManager());
				//this._container.RegisterType(typeof(IExportCustomerConfigFileWriter), typeof(ExportUserIniFileWriter), WriterEnum.ExportUserIniFileWriter.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportProductStreamWriter), typeof(ExportCatalogPdaHt630FileWriter), WriterEnum.ExportCatalogPdaHt630FileWriter.ToString(), new ContainerControlledLifetimeManager());
				//this._container.RegisterType(typeof(IExportProductStreamWriter), typeof(ExportCatalogPdaMade4NetFileWriter), WriterEnum.ExportCatalogPdaMade4NetFileWriter.ToString(), new ContainerControlledLifetimeManager());

				
				//?? не используется
				//this._container.RegisterType(typeof(IExportProductStreamWriter), typeof(ExportCatalogFileWriter), WriterEnum.ExportCatalogFileWriter.ToString(), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IExportIturStreamWriter), typeof(ExportIturFileWriter), WriterEnum.ExportIturFileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportIturStreamWriter), typeof(ExportIturPdaHt630FileWriter), WriterEnum.ExportIturPdaHt630FileWriter.ToString(), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IExportIturStreamWriter), typeof(ExportIturPdaMISFileWriter), WriterEnum.ExportIturPdaMISFileWriter.ToString(), new ContainerControlledLifetimeManager());

				
				//this._container.RegisterType(typeof(IScriptFieldLinkRepository), typeof(ScriptFieldLinktRepository), new ContainerControlledLifetimeManager());
                

				//this._container.RegisterType<IUploadPdaRepository, FakeUploadPdaRepository>(new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IWrapperMultiRepository), typeof(WrapperMultiRepository), WrapperMultiEnum.WrapperMultiRepository.ToString(), new ContainerControlledLifetimeManager());
	//              this._container.RegisterType<IWrapperMultiRepository, WrapperMultiRepository>(new ContainerControlledLifetimeManager());
  
				_logger.Info("Initialize ExportImportModule - OK");
			}
            catch (Exception ex)
            {
				_logger.ErrorException("Initialize ReportModule failed", ex);
            }

        }

        #endregion
    }
}