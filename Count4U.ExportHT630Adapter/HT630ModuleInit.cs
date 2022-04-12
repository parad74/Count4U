using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.ExportPdaMISAdapter;
using Count4U.ExportPdaMerkavaSQLiteAdapter;
using Count4U.ExportPdaClalitSQLiteAdapter;
using Count4U.ExportPdaNativSQLiteAdapter;
using Count4U.ExportPdaNativPlusSQLiteAdapter;
using Count4U.ExportPdaNativPlusMISSQLiteAdapter;
using Count4U.Common.Constants;
using Count4U.Common.ViewModel.Adapters.Export;
using Microsoft.Practices.ServiceLocation;
using NLog;
using System;

namespace Count4U.ExportHT630Adapter
{
	public class HT630ModuleInit : IModule
	{
		private readonly IUnityContainer _container;
		private readonly IServiceLocator _serviceLocator;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public HT630ModuleInit(IUnityContainer container,  IServiceLocator serviceLocator)
		{
			_container = container;
			_serviceLocator = serviceLocator;
		}

		public void Initialize()
		{
			_logger.Info("HT630ModuleInit module initialization...");
			try
			{
			IExportPdaModuleInfo moduleInfo = new ExportPdaModuleInfo();
			moduleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportHT630Adapter;
			moduleInfo.Title = Common.Constants.ExportAdapterTitle.ExportHT630Adapter;
			moduleInfo.UserControlType = typeof(ExportHT630AdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			moduleInfo.Description = "";
			moduleInfo.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			IExportPdaModuleInfo misModuleInfo = new ExportPdaModuleInfo();
			misModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaMISAdapter;
			misModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaMISAdapter;
			misModuleInfo.UserControlType = typeof(ExportPdaMISAdapterView);
			misModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			misModuleInfo.Description = "";
			misModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), misModuleInfo.Name, misModuleInfo, new ContainerControlledLifetimeManager());

			IExportPdaModuleInfo sqliteModuleInfo = new ExportPdaModuleInfo();
			sqliteModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter;
			sqliteModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaMerkavaSQLiteAdapter;
			sqliteModuleInfo.UserControlType = typeof(ExportPdaMerkavaSQLiteAdapterView);
			sqliteModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			sqliteModuleInfo.Description = "";
			sqliteModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), sqliteModuleInfo.Name, sqliteModuleInfo, new ContainerControlledLifetimeManager());

			IExportPdaModuleInfo clalitModuleInfo = new ExportPdaModuleInfo();
			clalitModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter;
			clalitModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaClalitSQLiteAdapter;
			clalitModuleInfo.UserControlType = typeof(ExportPdaClalitSQLiteAdapterView);
			clalitModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			clalitModuleInfo.Description = "";
			clalitModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), clalitModuleInfo.Name, clalitModuleInfo, new ContainerControlledLifetimeManager());


			IExportPdaModuleInfo nativModuleInfo = new ExportPdaModuleInfo();
			nativModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaNativSqliteAdapter;
			nativModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaNativSqliteAdapter;
			nativModuleInfo.UserControlType = typeof(ExportPdaNativSQLiteAdapterView);
			nativModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			nativModuleInfo.Description = "";
			nativModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), nativModuleInfo.Name, nativModuleInfo, new ContainerControlledLifetimeManager());

			IExportPdaModuleInfo nativPlusModuleInfo = new ExportPdaModuleInfo();
			nativPlusModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter;
			nativPlusModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaNativPlusSQLiteAdapter;
			nativPlusModuleInfo.UserControlType = typeof(ExportPdaNativPlusSQLiteAdapterView);
			nativPlusModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			nativPlusModuleInfo.Description = "";
			nativPlusModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), nativPlusModuleInfo.Name, nativPlusModuleInfo, new ContainerControlledLifetimeManager());


			IExportPdaModuleInfo nativPlusMISModuleInfo = new ExportPdaModuleInfo();
			nativPlusMISModuleInfo.Name = Common.Constants.ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter;
			nativPlusMISModuleInfo.Title = Common.Constants.ExportAdapterTitle.ExportPdaNativPlusMISSQLiteAdapter;
			nativPlusMISModuleInfo.UserControlType = typeof(ExportPdaNativPlusMISSQLiteAdapterView);
			nativPlusMISModuleInfo.ImportDomainEnum = ImportDomainEnum.ExportCatalog;
			nativPlusMISModuleInfo.Description = "";
			nativPlusMISModuleInfo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportPdaModuleInfo), nativPlusMISModuleInfo.Name, nativPlusMISModuleInfo, new ContainerControlledLifetimeManager());


			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaHt630AdapterViewModel), ExportPdaAdapterName.ExportHT630Adapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaMISAdapterViewModel), ExportPdaAdapterName.ExportPdaMISAdapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaMerkavaSQLiteAdapterViewModel), ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaClalitSQLiteAdapterViewModel), ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaNativSQLiteAdapterViewModel), ExportPdaAdapterName.ExportPdaNativSqliteAdapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaNativPlusSQLiteAdapterViewModel), ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter);
			this._container.RegisterType(typeof(ExportPdaModuleBaseViewModel), typeof(ExportPdaNativPlusMISSQLiteAdapterViewModel), ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter);

			//var exportPdaModuleBaseViewModel = _serviceLocator.GetAllInstances<ExportPdaModuleBaseViewModel>();

			//ExportPdaModuleBaseViewModel exportPdaModuleBaseViewModel1 = _serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter);
			//bool ret = exportPdaModuleBaseViewModel1.CanExport();

			}
			catch (Exception exc)
			{
				_logger.Error("HT630ModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
	}
}