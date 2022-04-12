using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.ImportPdaMISAdapter;
using Count4U.ImportPdaWarehouseAdapter;
using Count4U.ImportPdaDB3Adapter;
using Count4U.ImportPdaMISAndDefaultAdapter;
using Count4U.ImportPdaMerkavaDB3Adapter;
using Count4U.ImportPdaClalitSqliteAdapter;
using Count4U.ImportPdaNativSqliteAdapter;
using Count4U.ImportPdaDefaultBackupAdapter;
using Count4U.ImportPdaCount4UdbSdfAdapter;
using Count4U.ImportPdaMISAndForwardAdapter;
using Count4U.ImportPdaMerkavaUpdateDbAdapter;
using Count4U.ImportPdaAddCount4UdbSdfAdapter;
using Count4U.ImportPdaCloneCount4UdbSdfAdapter;
using Count4U.ImportPdaMinusByMakatCount4UdbSdfAdapter;
using Count4U.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter;
using Count4U.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter;
using Count4U.ImportPdaNativPlusSqliteAdapter;
using Count4U.ImportPdaCompareCount4UdbSdfAdapter;
using Count4U.ImportPdaAddSumCount4UdbSdfAdapter;
using Count4U.ImportPdaUpdateBarcodeDbAdapter;
using Count4U.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter;
using Count4U.ImportPdaYesXlsxAdapter;
using Count4U.ImportPdaNativPlusMISSqliteAdapter;
using Count4U.ImportPdaContinueAfterCompareCount4UdbSdfAdapter;
using Count4U.ImportPdaMerkavaXlsxAdapter;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportPdaMergeCount4UdbSdfAdapter;
using NLog;
using System;
using Count4U.ImportPdaMultiCsvAdapter;
using Count4U.ImportPdaMISSqliteAdapter;

namespace Count4U.ImportPdaDefaultAdapter
{


    public class PdaDefaultModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PdaDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

		public void Initialize()
		{
			_logger.Info("PdaDefaultModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportPdaDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaDefaultAdapter;
			moduleInfo.UserControlType = typeof(ImportPdaDefaultAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			moduleInfo.Description = "";
			moduleInfo.IsDefault = true;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo moduleBackupInfo = new ImportModuleInfo();
			moduleBackupInfo.Name = Common.Constants.ImportAdapterName.ImportPdaDefaultBackupAdapter;
			moduleBackupInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaDefaultBackupAdapter;
			moduleBackupInfo.UserControlType = typeof(ImportPdaDefaultBackupAdapterView);
			moduleBackupInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			moduleBackupInfo.Description = "";
			moduleBackupInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleBackupInfo.Name, moduleBackupInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo misInfo = new ImportModuleInfo();
			misInfo.Name = Common.Constants.ImportAdapterName.ImportPdaMISAdapter;
			misInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaMISAdapter;
			misInfo.UserControlType = typeof(ImportPdaMISAdapterView);
			misInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			misInfo.Description = "";
			misInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), misInfo.Name, misInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo misAndDefaultInfo = new ImportModuleInfo();
			misAndDefaultInfo.Name = Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter;
			misAndDefaultInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaMISAndDefaultAdapter;
			misAndDefaultInfo.UserControlType = typeof(ImportPdaMISAndDefaultAdapterView);
			misAndDefaultInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			misAndDefaultInfo.Description = "";
			misAndDefaultInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), misAndDefaultInfo.Name, misAndDefaultInfo, new ContainerControlledLifetimeManager());



			ImportModuleInfo misAndForwardInfo = new ImportModuleInfo();
			misAndForwardInfo.Name = Common.Constants.ImportAdapterName.ImportPdaMISAndForwardAdapter;
			misAndForwardInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaMISAndForwardAdapter;
			misAndForwardInfo.UserControlType = typeof(ImportPdaMISAndForwardAdapterView);
			misAndForwardInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			misAndForwardInfo.Description = "";
			misAndForwardInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), misAndForwardInfo.Name, misAndForwardInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo db3Info = new ImportModuleInfo();
			db3Info.Name = Common.Constants.ImportAdapterName.ImportPdaDB3Adapter;
			db3Info.Title = Common.Constants.ImportAdapterTitle.ImportPdaDB3Adapter;
			db3Info.UserControlType = typeof(ImportPdaDB3AdapterView);
			db3Info.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			db3Info.Description = "";
			db3Info.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), db3Info.Name, db3Info, new ContainerControlledLifetimeManager());

			ImportModuleInfo merkavaDb3Info = new ImportModuleInfo();
			merkavaDb3Info.Name = Common.Constants.ImportAdapterName.ImportPdaMerkavaDB3Adapter;
			merkavaDb3Info.Title = Common.Constants.ImportAdapterTitle.ImportPdaMerkavaDB3Adapter;
			merkavaDb3Info.UserControlType = typeof(ImportPdaMerkavaDB3AdapterView);
			merkavaDb3Info.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			merkavaDb3Info.Description = "";
			merkavaDb3Info.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), merkavaDb3Info.Name, merkavaDb3Info, new ContainerControlledLifetimeManager());

			ImportModuleInfo clalitDb3Info = new ImportModuleInfo();
			clalitDb3Info.Name = Common.Constants.ImportAdapterName.ImportPdaClalitSqliteAdapter;
			clalitDb3Info.Title = Common.Constants.ImportAdapterTitle.ImportPdaClalitSqliteAdapter;
			clalitDb3Info.UserControlType = typeof(ImportPdaClalitSqliteAdapterView);
			clalitDb3Info.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			clalitDb3Info.Description = "";
			clalitDb3Info.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), clalitDb3Info.Name, clalitDb3Info, new ContainerControlledLifetimeManager());

			ImportModuleInfo nativDb3Info = new ImportModuleInfo();
			nativDb3Info.Name = Common.Constants.ImportAdapterName.ImportPdaNativSqliteAdapter;
			nativDb3Info.Title = Common.Constants.ImportAdapterTitle.ImportPdaNativSqliteAdapter;
			nativDb3Info.UserControlType = typeof(ImportPdaNativSqliteAdapterView);
			nativDb3Info.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			nativDb3Info.Description = "";
			nativDb3Info.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), nativDb3Info.Name, nativDb3Info, new ContainerControlledLifetimeManager());

			ImportModuleInfo nativPlusDb3Info = new ImportModuleInfo();
			nativPlusDb3Info.Name = Common.Constants.ImportAdapterName.ImportPdaNativPlusSqliteAdapter;
			nativPlusDb3Info.Title = Common.Constants.ImportAdapterTitle.ImportPdaNativPlusSqliteAdapter;
			nativPlusDb3Info.UserControlType = typeof(ImportPdaNativPlusSqliteAdapterView);
			nativPlusDb3Info.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			nativPlusDb3Info.Description = "";
			nativPlusDb3Info.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), nativPlusDb3Info.Name, nativPlusDb3Info, new ContainerControlledLifetimeManager());


			ImportModuleInfo warehouseInfo = new ImportModuleInfo();
			warehouseInfo.Name = Common.Constants.ImportAdapterName.ImportPdaWarehouseAdapter;
			warehouseInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaWarehouseAdapter;
			warehouseInfo.UserControlType = typeof(ImportPdaWarehouseAdapterView);
			warehouseInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			warehouseInfo.Description = "";
			warehouseInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), warehouseInfo.Name, warehouseInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo count4UdbSdfInfo = new ImportModuleInfo();
			count4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaCount4UdbSdfAdapter;
			count4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaCount4UdbSdfAdapter;
			count4UdbSdfInfo.UserControlType = typeof(ImportPdaCount4UdbSdfAdapterView);
			count4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			count4UdbSdfInfo.Description = "";
			count4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), count4UdbSdfInfo.Name, count4UdbSdfInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo addCount4UdbSdfInfo = new ImportModuleInfo();
			addCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaAddCount4UdbSdfAdapter;
			addCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaAddCount4UdbSdfAdapter;
			addCount4UdbSdfInfo.UserControlType = typeof(ImportPdaAddCount4UdbSdfAdapterView);
			addCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			addCount4UdbSdfInfo.Description = "";
			addCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), addCount4UdbSdfInfo.Name,
				addCount4UdbSdfInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo mergeCount4UdbSdfInfo = new ImportModuleInfo();
			mergeCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaMergeCount4UdbSdfAdapter;
			mergeCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaMergeCount4UdbSdfAdapter;
			mergeCount4UdbSdfInfo.UserControlType = typeof(ImportPdaMergeCount4UdbSdfAdapterView);
			mergeCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			mergeCount4UdbSdfInfo.Description = "";
			mergeCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), mergeCount4UdbSdfInfo.Name,
				mergeCount4UdbSdfInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMergeCount4UdbSdfAdapterViewModel),
				Common.Constants.ImportAdapterName.ImportPdaMergeCount4UdbSdfAdapter);


			ImportModuleInfo cloneCount4UdbSdfInfo = new ImportModuleInfo();
			cloneCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaCloneCount4UdbSdfAdapter;
			cloneCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaCloneCount4UdbSdfAdapter;
			cloneCount4UdbSdfInfo.UserControlType = typeof(ImportPdaCloneCount4UdbSdfAdapterView);
			cloneCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			cloneCount4UdbSdfInfo.Description = "";
			cloneCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), cloneCount4UdbSdfInfo.Name, cloneCount4UdbSdfInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo minusByMakatCount4UdbSdfInfo = new ImportModuleInfo();
			minusByMakatCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaMinusByMakatCount4UdbSdfAdapter;
			minusByMakatCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaMinusByMakatCount4UdbSdfAdapter;
			minusByMakatCount4UdbSdfInfo.UserControlType = typeof(ImportPdaMinusByMakatCount4UdbSdfAdapterView);
			minusByMakatCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			minusByMakatCount4UdbSdfInfo.Description = "";
			minusByMakatCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), minusByMakatCount4UdbSdfInfo.Name, minusByMakatCount4UdbSdfInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo update2SumByIturMakatCount4UdbSdfInfo = new ImportModuleInfo();
			update2SumByIturMakatCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter;
			update2SumByIturMakatCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter;
			update2SumByIturMakatCount4UdbSdfInfo.UserControlType = typeof(ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapterView);
			update2SumByIturMakatCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			update2SumByIturMakatCount4UdbSdfInfo.Description = "";
			update2SumByIturMakatCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), update2SumByIturMakatCount4UdbSdfInfo.Name, update2SumByIturMakatCount4UdbSdfInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo update2SumByIturBarcodeCount4UdbSdfInfo = new ImportModuleInfo();
			update2SumByIturBarcodeCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter;
			update2SumByIturBarcodeCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter;
			update2SumByIturBarcodeCount4UdbSdfInfo.UserControlType = typeof(ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapterView);
			update2SumByIturBarcodeCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			update2SumByIturBarcodeCount4UdbSdfInfo.Description = "";
			update2SumByIturBarcodeCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), update2SumByIturBarcodeCount4UdbSdfInfo.Name, update2SumByIturBarcodeCount4UdbSdfInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo update2SumByIturDocMakatCount4UdbSdfInfo = new ImportModuleInfo();
			update2SumByIturDocMakatCount4UdbSdfInfo.Name = Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter;
			update2SumByIturDocMakatCount4UdbSdfInfo.Title = Common.Constants.ImportAdapterTitle.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter;
			update2SumByIturDocMakatCount4UdbSdfInfo.UserControlType = typeof(ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapterView);
			update2SumByIturDocMakatCount4UdbSdfInfo.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			update2SumByIturDocMakatCount4UdbSdfInfo.Description = "";
			update2SumByIturDocMakatCount4UdbSdfInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), update2SumByIturDocMakatCount4UdbSdfInfo.Name, update2SumByIturDocMakatCount4UdbSdfInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo merkavaUpdateDb = new ImportModuleInfo();
			merkavaUpdateDb.Name = Common.Constants.ImportAdapterName.ImportPdaMerkavaUpdateDbAdapter;
			merkavaUpdateDb.Title = Common.Constants.ImportAdapterTitle.ImportPdaMerkavaUpdateDbAdapter;
			merkavaUpdateDb.UserControlType = typeof(ImportPdaMerkavaUpdateDbAdapterView);
			merkavaUpdateDb.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			merkavaUpdateDb.Description = "";
			merkavaUpdateDb.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), merkavaUpdateDb.Name, merkavaUpdateDb, new ContainerControlledLifetimeManager());

			ImportModuleInfo compareCount4UdbSdf = new ImportModuleInfo();
			compareCount4UdbSdf.Name = Common.Constants.ImportAdapterName.ImportPdaCompareCount4UdbSdfAdapter;
			compareCount4UdbSdf.Title = Common.Constants.ImportAdapterTitle.ImportPdaCompareCount4UdbSdfAdapter;
			compareCount4UdbSdf.UserControlType = typeof(ImportPdaCompareCount4UdbSdfAdapterView);
			compareCount4UdbSdf.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			compareCount4UdbSdf.Description = "";
			compareCount4UdbSdf.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), compareCount4UdbSdf.Name, compareCount4UdbSdf, new ContainerControlledLifetimeManager());

			ImportModuleInfo addSumCount4UdbSdf = new ImportModuleInfo();
			addSumCount4UdbSdf.Name = Common.Constants.ImportAdapterName.ImportPdaAddSumCount4UdbSdfAdapter;
			addSumCount4UdbSdf.Title = Common.Constants.ImportAdapterTitle.ImportPdaAddSumCount4UdbSdfAdapter;
			addSumCount4UdbSdf.UserControlType = typeof(ImportPdaAddSumCount4UdbSdfAdapterView);
			addSumCount4UdbSdf.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			addSumCount4UdbSdf.Description = "";
			addSumCount4UdbSdf.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), addSumCount4UdbSdf.Name, addSumCount4UdbSdf, new ContainerControlledLifetimeManager());


			ImportModuleInfo importPdaUpdateBarcodeDbAdapter = new ImportModuleInfo();
			importPdaUpdateBarcodeDbAdapter.Name = Common.Constants.ImportAdapterName.ImportPdaUpdateBarcodeDbAdapter;
			importPdaUpdateBarcodeDbAdapter.Title = Common.Constants.ImportAdapterTitle.ImportPdaUpdateBarcodeDbAdapter;
			importPdaUpdateBarcodeDbAdapter.UserControlType = typeof(ImportPdaUpdateBarcodeDbAdapterView);
			importPdaUpdateBarcodeDbAdapter.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			importPdaUpdateBarcodeDbAdapter.Description = "";
			importPdaUpdateBarcodeDbAdapter.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), importPdaUpdateBarcodeDbAdapter.Name, importPdaUpdateBarcodeDbAdapter, new ContainerControlledLifetimeManager());


			ImportModuleInfo YesXlsx = new ImportModuleInfo();
			YesXlsx.Name = Common.Constants.ImportAdapterName.ImportPdaYesXlsxAdapter;
			YesXlsx.Title = Common.Constants.ImportAdapterTitle.ImportPdaYesXlsxAdapter;
			YesXlsx.UserControlType = typeof(ImportPdaYesXlsxAdapterView);
			YesXlsx.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			YesXlsx.Description = "";
			YesXlsx.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), YesXlsx.Name, YesXlsx, new ContainerControlledLifetimeManager());

			ImportModuleInfo MerkavaXlsx = new ImportModuleInfo();
			MerkavaXlsx.Name = Common.Constants.ImportAdapterName.ImportPdaMerkavaXlsxAdapter;
			MerkavaXlsx.Title = Common.Constants.ImportAdapterTitle.ImportPdaMerkavaXlsxAdapter;
			MerkavaXlsx.UserControlType = typeof(ImportPdaMerkavaXlsxAdapterView);
			MerkavaXlsx.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			MerkavaXlsx.Description = "";
			MerkavaXlsx.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), MerkavaXlsx.Name, MerkavaXlsx, new ContainerControlledLifetimeManager());


			ImportModuleInfo NativPlusMIS = new ImportModuleInfo();
			NativPlusMIS.Name = Common.Constants.ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter;
			NativPlusMIS.Title = Common.Constants.ImportAdapterTitle.ImportPdaNativPlusMISSqliteAdapter;
			NativPlusMIS.UserControlType = typeof(ImportPdaNativPlusMISSqliteAdapterView);
			NativPlusMIS.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			NativPlusMIS.Description = "";
			NativPlusMIS.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), NativPlusMIS.Name, NativPlusMIS, new ContainerControlledLifetimeManager());



			ImportModuleInfo ContinueAfterCompare = new ImportModuleInfo();
			ContinueAfterCompare.Name = Common.Constants.ImportAdapterName.ImportPdaContinueAfterCompareCount4UdbSdfAdapter;
			ContinueAfterCompare.Title = Common.Constants.ImportAdapterTitle.ImportPdaContinueAfterCompareCount4UdbSdfAdapter;
			ContinueAfterCompare.UserControlType = typeof(ImportPdaContinueAfterCompareCount4UdbSdfAdapterView);
			ContinueAfterCompare.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
			ContinueAfterCompare.Description = "";
			ContinueAfterCompare.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), ContinueAfterCompare.Name, ContinueAfterCompare, new ContainerControlledLifetimeManager());

				ImportModuleInfo multiCsv = new ImportModuleInfo();
				multiCsv.Name = Common.Constants.ImportAdapterName.ImportPdaMultiCsvAdapter;
				multiCsv.Title = Common.Constants.ImportAdapterTitle.ImportPdaMultiCsvAdapter;
				multiCsv.UserControlType = typeof(ImportPdaMultiCsvAdapterView);
				multiCsv.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
				multiCsv.Description = "";
				multiCsv.IsDefault = false;
				this._container.RegisterInstance(typeof(IImportModuleInfo), multiCsv.Name, multiCsv, new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMultiCsvAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMultiCsvAdapter);

				ImportModuleInfo MISSqlite = new ImportModuleInfo();
				MISSqlite.Name = Common.Constants.ImportAdapterName.ImportPdaMISSqliteAdapter;
				MISSqlite.Title = Common.Constants.ImportAdapterTitle.ImportPdaMISSqliteAdapter;
				MISSqlite.UserControlType = typeof(ImportPdaMISSqliteAdapterView);
				MISSqlite.ImportDomainEnum = ImportDomainEnum.ImportInventProduct;
				MISSqlite.Description = "";
				MISSqlite.IsDefault = false;
				this._container.RegisterInstance(typeof(IImportModuleInfo), MISSqlite.Name, MISSqlite, new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMISSqliteAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMISSqliteAdapter);


				
			//this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ExportPdaHt630AdapterViewModel), ExportPdaAdapterName.ExportHT630Adapter);
				this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAddCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaAddCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaAddSumCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaAddSumCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaClalitSqliteAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaClalitSqliteAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaCloneCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaCloneCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaCompareCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaCompareCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaContinueAfterCompareCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaContinueAfterCompareCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaDB3AdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaDB3Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaDefaultBackupAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaDefaultBackupAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMerkavaDB3AdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMerkavaDB3Adapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMerkavaXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMerkavaXlsxAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMinusByMakatCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMinusByMakatCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMISAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMISAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMISAndDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMISAndDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMISAndForwardAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMISAndForwardAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaNativPlusMISSqliteAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaNativPlusMISSqliteAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaNativPlusSqliteAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaNativPlusSqliteAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaNativSqliteAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaNativSqliteAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturBarcodeCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturDocMakatCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaUpdate2SumByIturMakatCount4UdbSdfAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaUpdateBarcodeDbAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaUpdateBarcodeDbAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaMerkavaUpdateDbAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaMerkavaUpdateDbAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaWarehouseAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaWarehouseAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportPdaYesXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportPdaYesXlsxAdapter);

			//ImportModuleBaseViewModel importModuleBaseViewModel = _serviceLocator.GetInstance<ImportModuleBaseViewModel>(Common.Constants.ImportAdapterName.ImportPdaYesXlsxAdapter);
			//bool ret = importModuleBaseViewModel.CanImport();

			}
			catch (Exception exc)
			{
				_logger.Error("PdaDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}

    }
}