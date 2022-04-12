using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.ImportSectionSapb1ZometsfarimAdapter;
using Count4U.ImportSectionGazitVerifoneSteimaztzkyAdapter;
using Count4U.ImportSectionAS400MegaAdapter;
using Count4U.ImportSectionAS400HamashbirAdapter;
using Count4U.Common.ViewModel.Adapters.Import;
using NLog;
using System;

namespace Count4U.ImportSectionDefaultAdapter
{
    public class SectionDefaultModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SectionDefaultModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("SectionDefaultModuleInit module initialization...");
			try
			{
			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.ImportAdapterName.ImportSectionDefaultAdapter;
			moduleInfo.Title = Common.Constants.ImportAdapterTitle.ImportSectionDefaultAdapter;
			moduleInfo.UserControlType = typeof(ImportSectionDefaultAdapterView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.ImportSection;
			moduleInfo.Description = "Description";
			moduleInfo.IsDefault = true;
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo as400April = new ImportModuleInfo();
			as400April.Name = Common.Constants.ImportAdapterName.ImportSectionAS400AprilAdapter;
			as400April.Title = Common.Constants.ImportAdapterTitle.ImportSectionAS400AprilAdapter;
			as400April.UserControlType = typeof(ImportSectionDefaultAdapterView);
			as400April.ImportDomainEnum = ImportDomainEnum.ImportSection;
			as400April.Description = "";
			as400April.IsDefault = true;
			this._container.RegisterInstance(typeof(IImportModuleInfo), as400April.Name, as400April, new ContainerControlledLifetimeManager());

			ImportModuleInfo sapb1Zometsfarim = new ImportModuleInfo();
			sapb1Zometsfarim.Name = Common.Constants.ImportAdapterName.ImportSectionSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.Title = Common.Constants.ImportAdapterTitle.ImportSectionSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.UserControlType = typeof(ImportSectionSapb1ZometsfarimAdapterView);
			sapb1Zometsfarim.ImportDomainEnum = ImportDomainEnum.ImportSection;
			sapb1Zometsfarim.Description = "";
			sapb1Zometsfarim.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), sapb1Zometsfarim.Name, sapb1Zometsfarim, new ContainerControlledLifetimeManager());


			ImportModuleInfo gazitVerifoneSteimaztzky = new ImportModuleInfo();
			gazitVerifoneSteimaztzky.Name = Common.Constants.ImportAdapterName.ImportSectionGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.Title = Common.Constants.ImportAdapterTitle.ImportSectionGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.UserControlType = typeof(ImportSectionGazitVerifoneSteimaztzkyAdapterView);
			gazitVerifoneSteimaztzky.ImportDomainEnum = ImportDomainEnum.ImportSection;
			gazitVerifoneSteimaztzky.Description = "";
			gazitVerifoneSteimaztzky.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), gazitVerifoneSteimaztzky.Name, gazitVerifoneSteimaztzky, new ContainerControlledLifetimeManager());


			ImportModuleInfo as400Mega = new ImportModuleInfo();
			as400Mega.Name = Common.Constants.ImportAdapterName.ImportSectionAS400MegaAdapter;
			as400Mega.Title = Common.Constants.ImportAdapterTitle.ImportSectionAS400MegaAdapter;
			as400Mega.UserControlType = typeof(ImportSectionAS400MegaAdapterView);
			as400Mega.ImportDomainEnum = ImportDomainEnum.ImportSection;
			as400Mega.Description = "";
			as400Mega.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), as400Mega.Name, as400Mega, new ContainerControlledLifetimeManager());

			ImportModuleInfo as400Hamashbir = new ImportModuleInfo();
			as400Hamashbir.Name = Common.Constants.ImportAdapterName.ImportSectionAS400HamashbirAdapter;
			as400Hamashbir.Title = Common.Constants.ImportAdapterTitle.ImportSectionAS400HamashbirAdapter;
			as400Hamashbir.UserControlType = typeof(ImportSectionAS400HamashbirAdapterView);
			as400Hamashbir.ImportDomainEnum = ImportDomainEnum.ImportSection;
			as400Hamashbir.Description = "";
			as400Hamashbir.IsDefault = false;
			this._container.RegisterInstance(typeof(IImportModuleInfo), as400Hamashbir.Name, as400Hamashbir, new ContainerControlledLifetimeManager());

			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionAS400AprilAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionSapb1ZometsfarimAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionSapb1ZometsfarimAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionGazitVerifoneSteimaztzkyAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionGazitVerifoneSteimaztzkyAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionAS400MegaAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionAS400MegaAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportSectionAS400HamashbirAdapterViewModel), Common.Constants.ImportAdapterName.ImportSectionAS400HamashbirAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("SectionDefaultModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

    }
}
