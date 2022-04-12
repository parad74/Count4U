using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.UpdateCatalogERPQuantityGeneralCSVAdapter
{
    public class UpdateCatalogERPQuantityGeneralCSVModuleInit : IModule
    {
         private readonly IUnityContainer _container;
		 private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

         public UpdateCatalogERPQuantityGeneralCSVModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			//ImportModuleInfo moduleInfo = new ImportModuleInfo();
			//moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityGeneralCSVAdapter;
			//moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityGeneralCSVAdapter;
			//moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityGeneralCSVView);
			//moduleInfo.ImportDomainEnum=ImportDomainEnum.UpdateCatalog;
			//moduleInfo.IsDefault = false;
			//moduleInfo.Description = "";
			//this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
        }
    }
}
