using Count4U.Common.Interfaces.Adapters;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportSectionKitAdapter
{
    public class ImportSectionKitModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportSectionKitModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
			//ImportModuleInfo moduleInfo = new ImportModuleInfo();
			//moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSectionDefaultAdapter;
			//moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSectionDefaultAdapter;
			//moduleInfo.UserControlType = typeof(ImportSectionDefaultAdapterView);
			//moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSection;
			//moduleInfo.Description = "Description";
			//moduleInfo.IsDefault = true;
			//this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());  
        }
    }
}