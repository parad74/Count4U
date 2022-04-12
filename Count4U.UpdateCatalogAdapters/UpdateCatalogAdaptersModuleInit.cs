using Count4U.Common.Interfaces.Adapters;
using Count4U.Model;
using Count4U.UpdateCatalogAdapters.Empty;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using System;
using Count4U.UpdateCatalogAdapters.ComplexOperationEmpty;
using Count4U.ComplexAutoDocumentAdapter;
using Count4U.ComplexDefaultAdapter;
using Count4U.ComplexAutoDocInvAdapter;
using NLog;


namespace Count4U.UpdateCatalogAdapters
{
    public class UpdateCatalogAdaptersModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UpdateCatalogAdaptersModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogAdaptersModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogEmptyAdapter;
			moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogEmptyAdapter;
			moduleInfo.UserControlType = typeof(UpdateCatalogEmptyView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			moduleInfo.IsDefault = true;
			moduleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());


			ImportModuleInfo importExportEmpty = new ImportModuleInfo();
			importExportEmpty.Name = Common.Constants.ComplexAdapterName.ComplexOperationEmptyAdapter;
			importExportEmpty.Title = Common.Constants.CompleхAdapterTitle.ComplexOperationEmptyAdapter;
			importExportEmpty.UserControlType = typeof(ComplexOperationEmptyAdapterView);
			importExportEmpty.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
			importExportEmpty.IsDefault = false;
			importExportEmpty.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), importExportEmpty.Name, importExportEmpty, new ContainerControlledLifetimeManager());



			ImportModuleInfo autoDocument = new ImportModuleInfo();
			autoDocument.Name = Common.Constants.ComplexAdapterName.ComplexAutoDocumentAdapter;
			autoDocument.Title = Common.Constants.CompleхAdapterTitle.ComplexAutoDocumentAdapter;
			autoDocument.UserControlType = typeof(ComplexAutoDocumentAdapterView);
			autoDocument.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
			autoDocument.IsDefault = false;
			autoDocument.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), autoDocument.Name, autoDocument, new ContainerControlledLifetimeManager());


			ImportModuleInfo autoDocInvDocument = new ImportModuleInfo();
			autoDocInvDocument.Name = Common.Constants.ComplexAdapterName.ComplexAutoDocInvAdapter;
			autoDocInvDocument.Title = Common.Constants.CompleхAdapterTitle.ComplexAutoDocInvAdapter;
			autoDocInvDocument.UserControlType = typeof(ComplexAutoDocInvAdapterView);
			autoDocInvDocument.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
			autoDocInvDocument.IsDefault = true;
			autoDocInvDocument.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), autoDocInvDocument.Name, autoDocInvDocument, new ContainerControlledLifetimeManager());


			ImportModuleInfo complexDefault = new ImportModuleInfo();
			complexDefault.Name = Common.Constants.ComplexAdapterName.ComplexDefaultAdapter;
			complexDefault.Title = Common.Constants.CompleхAdapterTitle.ComplexDefaultAdapter;
			complexDefault.UserControlType = typeof(ComplexDefaultAdapterView);
			complexDefault.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
			complexDefault.IsDefault = false;
			complexDefault.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), complexDefault.Name, complexDefault, new ContainerControlledLifetimeManager());

			//ImportModuleInfo complexOperationSetting = new ImportModuleInfo();
			//complexOperationSetting.Name = Common.Constants.ComplexAdapterName.ComplexOperationSettingAdapter;
			//complexOperationSetting.Title = Common.Constants.CompleхAdapterTitle.ComplexOperationSettingAdapter;
			//complexOperationSetting.UserControlType = typeof(ComplexOperationSettingAdapterView);
			//complexOperationSetting.ImportDomainEnum = ImportDomainEnum.ComplexOperation;
			//complexOperationSetting.IsDefault = false;
			//complexOperationSetting.Description = "";
			//this._container.RegisterInstance(typeof(IImportModuleInfo), complexOperationSetting.Name, complexOperationSetting, new ContainerControlledLifetimeManager());

			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogAdaptersModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}