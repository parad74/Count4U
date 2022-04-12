using Count4U.Model;
using Count4U.Model.ServiceContract;
using Count4U.WPF.Service;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;

namespace Count4U.Model.Service
{
    public class ServiceMainWpfImplementationInit : IModule
    {
         private readonly IUnityContainer _container;

		 public ServiceMainWpfImplementationInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			//ServiceWpfInfo moduleInfo = new ServiceWpfInfo();
			//moduleInfo.InterfaceName = Common.Constants.ServiceWcfInterfaceName.ICustomerReportWPFRepository;
			//moduleInfo.ClassName = Common.Constants.ServiceWcfName.CustomerReportRepositoryService;
			//moduleInfo.ServiceObjectType = typeof(CustomerReportRepositoryService);
			//moduleInfo.IServiceObjectType = typeof(ICustomerReportWcfRepository);
			//moduleInfo.ServiceDBEnum = ServiceDBEnum.MainDB;
			//this._container.RegisterInstance(typeof(ServiceWpfInfo), moduleInfo.InterfaceName, moduleInfo, new ContainerControlledLifetimeManager());
			//this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityYarpaViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityYarpaAdapter);

        }

	
		public static void InitializeStatic(IUnityContainer _containerStatic)
		{
			ServiceWpfInfo moduleInfo = new ServiceWpfInfo();
			moduleInfo.InterfaceName = Common.Constants.ServiceWcfInterfaceName.ICustomerReportWcfRepository;
			moduleInfo.ClassName = Common.Constants.ServiceWcfName.CustomerReportRepositoryService;
			moduleInfo.ServiceObjectType = typeof(CustomerReportRepositoryService);
			moduleInfo.IServiceObjectType = typeof(ICustomerReportWcfRepository);
			moduleInfo.ServiceDBEnum = ServiceDBEnum.MainDB;
			moduleInfo.TryHost = false;
			_containerStatic.RegisterInstance(typeof(ServiceWpfInfo), moduleInfo.InterfaceName, moduleInfo, new ContainerControlledLifetimeManager());

			ServiceWpfInfo customerInfo = new ServiceWpfInfo();
			customerInfo.InterfaceName = Common.Constants.ServiceWcfInterfaceName.ICustomerWcfRepository;
			customerInfo.ClassName = Common.Constants.ServiceWcfName.CustomerRepositoryService;
			customerInfo.ServiceObjectType = typeof(CustomerRepositoryService);
			customerInfo.IServiceObjectType = typeof(ICustomerWcfRepository);
			customerInfo.ServiceDBEnum = ServiceDBEnum.MainDB;
			customerInfo.TryHost = false;
			_containerStatic.RegisterInstance(typeof(ServiceWpfInfo), customerInfo.InterfaceName, customerInfo, new ContainerControlledLifetimeManager());

			ServiceWpfInfo iturInfo = new ServiceWpfInfo();
			iturInfo.InterfaceName = Common.Constants.ServiceWcfInterfaceName.IIturWcfRepository;
			iturInfo.ClassName = Common.Constants.ServiceWcfName.IturRepositoryService;
			iturInfo.ServiceObjectType = typeof(IturRepositoryService);
			iturInfo.IServiceObjectType = typeof(IIturWcfRepository);
			iturInfo.ServiceDBEnum = ServiceDBEnum.Count4UDB;
			iturInfo.TryHost = true;
			_containerStatic.RegisterInstance(typeof(ServiceWpfInfo), iturInfo.InterfaceName, iturInfo, new ContainerControlledLifetimeManager());

		}
	}
}
