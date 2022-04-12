using Count4U.Common.Interfaces.Adapters;
using Count4U.Model;
using Count4U.UpdateCatalogKitAdapter.RetalixNext;
using Count4U.UpdateCatalogKitAdapter.One1;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.UpdateCatalogKitAdapter.MaccabiPharmSAP;
using Count4U.UpdateCatalogKitAdapter.Nibit;
using Count4U.UpdateCatalogKitAdapter.AS400April;
using Count4U.UpdateCatalogERPQuantityMPLAdapter;
using Count4U.UpdateCatalogERPQuantityTafnitMatrixAdapter;
using Count4U.UpdateCatalogERPQuantityOrenOriginalsAdapter;
using Count4U.UpdateCatalogKitAdapter.Otech;
using Count4U.UpdateCatalogERPQuantityNikeIntAdapter;
using Count4U.UpdateCatalogERPQuantitySapb1XslxAdapter;
using Count4U.UpdateCatalogERPQuantitySapb1ZometsfarimAdapter;
using Count4U.UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyAdapter;
using Count4U.UpdateCatalogKitAdapter.AS400Mega;
using Count4U.UpdateCatalogKitAdapter.GeneralCSV;
using Count4U.UpdateCatalogKitAdapter.AS400Hamashbir;
using Count4U.UpdateCatalogNativPlusXslxAdapter;
using Count4U.ImportCatalogKitAdapter.ProfileXml;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.UpdateCatalogERPQuantityPrioritytEsteeLouderAdapter;
using NLog;
using System;

namespace Count4U.UpdateCatalogKitAdapter
{
    public class UpdateCatalogKitModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UpdateCatalogKitModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
			_logger.Info("UpdateCatalogKitModuleInit module initialization...");
			try
			{
            ImportModuleInfo retalixNext = new ImportModuleInfo();
            retalixNext.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPRetalixNextAdapter;
            retalixNext.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPRetalixNextAdapter;
            retalixNext.UserControlType = typeof(UpdateCatalogRetalixNextView);
            retalixNext.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
            retalixNext.IsDefault = false;
            retalixNext.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), retalixNext.Name, retalixNext, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogRetalixNextViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPRetalixNextAdapter);


			ImportModuleInfo maccabiPharmSAP = new ImportModuleInfo();
			maccabiPharmSAP.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityMaccabiPharmSAPAdapter;
			maccabiPharmSAP.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityMaccabiPharmSAPAdapter;
			maccabiPharmSAP.UserControlType = typeof(UpdateCatalogERPQuantityMaccabiPharmSAPView);
			maccabiPharmSAP.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			maccabiPharmSAP.IsDefault = false;
			maccabiPharmSAP.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), maccabiPharmSAP.Name, maccabiPharmSAP, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityMaccabiPharmSAPViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityMaccabiPharmSAPAdapter);

			ImportModuleInfo one1 = new ImportModuleInfo();
			one1.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOne1Adapter;
			one1.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityOne1Adapter;
			one1.UserControlType = typeof(UpdateCatalogERPQuantityOne1View);
			one1.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			one1.IsDefault = false;
			one1.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), one1.Name, one1, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityOne1ViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOne1Adapter);

			ImportModuleInfo otech = new ImportModuleInfo();
			otech.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOtechAdapter;
			otech.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityOtechAdapter;
			otech.UserControlType = typeof(UpdateCatalogERPQuantityOtechView);
			otech.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			otech.IsDefault = false;
			otech.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), otech.Name, otech, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityOtechViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOtechAdapter);


			ImportModuleInfo mpl = new ImportModuleInfo();
			mpl.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityMPLAdapter;
			mpl.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityMPLAdapter;
			mpl.UserControlType = typeof(UpdateCatalogERPQuantityMPLView);
			mpl.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			mpl.IsDefault = false;
			mpl.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), mpl.Name, mpl, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityMPLViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityMPLAdapter);

			ImportModuleInfo orenOriginals = new ImportModuleInfo();
			orenOriginals.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOrenOriginalsAdapter;
			orenOriginals.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityOrenOriginalsAdapter;
			orenOriginals.UserControlType = typeof(UpdateCatalogERPQuantityOrenOriginalsView);
			orenOriginals.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			orenOriginals.IsDefault = false;
			orenOriginals.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), orenOriginals.Name, orenOriginals, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityOrenOriginalsViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityOrenOriginalsAdapter);
			

            ImportModuleInfo as400April = new ImportModuleInfo();
            as400April.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPAS400AprilAdapter;
            as400April.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPAS400AprilAdapter;
            as400April.UserControlType = typeof(UpdateCatalogAS400AprilView);
            as400April.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
            as400April.IsDefault = false;
            as400April.Description = "";
            this._container.RegisterInstance(typeof(IImportModuleInfo), as400April.Name, as400April, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogAS400AprilViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPAS400AprilAdapter);

			ImportModuleInfo tafnitMatrix = new ImportModuleInfo();
			tafnitMatrix.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityTafnitMatrixAdapter;
			tafnitMatrix.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityTafnitMatrixAdapter;
			tafnitMatrix.UserControlType = typeof(UpdateCatalogERPQuantityTafnitMatrixView);
			tafnitMatrix.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			tafnitMatrix.IsDefault = false;
			tafnitMatrix.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), tafnitMatrix.Name, tafnitMatrix, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityTafnitMatrixViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityTafnitMatrixAdapter);

			ImportModuleInfo nikeInk = new ImportModuleInfo();
			nikeInk.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityNikeIntAdapter;
			nikeInk.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityNikeIntAdapter;
			nikeInk.UserControlType = typeof(UpdateCatalogERPQuantityNikeIntView);
			nikeInk.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			nikeInk.IsDefault = false;
			nikeInk.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), nikeInk.Name, nikeInk, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityNikeIntViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityNikeIntAdapter);


			ImportModuleInfo nibit = new ImportModuleInfo();
			nibit.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityNibitAdapter;
			nibit.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityNibitAdapter;
			nibit.UserControlType = typeof(UpdateCatalogNibitView);
			nibit.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			nibit.IsDefault = false;
			nibit.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), nibit.Name, nibit, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogNibitViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityNibitAdapter);

			ImportModuleInfo sapb1Xslx = new ImportModuleInfo();
			sapb1Xslx.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1XslxAdapter;
			sapb1Xslx.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantitySapb1XslxAdapter;
			sapb1Xslx.UserControlType = typeof(UpdateCatalogERPQuantitySapb1XslxAdapterView);
			sapb1Xslx.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			sapb1Xslx.IsDefault = false;
			sapb1Xslx.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), sapb1Xslx.Name, sapb1Xslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantitySapb1XslxAdapterViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1XslxAdapter);

			ImportModuleInfo sapb1Zometsfarim = new ImportModuleInfo();
			sapb1Zometsfarim.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1ZometsfarimAdapter;
			sapb1Zometsfarim.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantitySapb1ZometsfarimAdapter;
			sapb1Zometsfarim.UserControlType = typeof(UpdateCatalogERPQuantitySapb1ZometsfarimView);
			sapb1Zometsfarim.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			sapb1Zometsfarim.IsDefault = false;
			sapb1Zometsfarim.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), sapb1Zometsfarim.Name, sapb1Zometsfarim, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantitySapb1ZometsfarimViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1ZometsfarimAdapter);


			ImportModuleInfo gazitVerifoneSteimaztzky = new ImportModuleInfo();
			gazitVerifoneSteimaztzky.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.UserControlType = typeof(UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyView);
			gazitVerifoneSteimaztzky.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			gazitVerifoneSteimaztzky.IsDefault = false;
			gazitVerifoneSteimaztzky.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), gazitVerifoneSteimaztzky.Name, gazitVerifoneSteimaztzky, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyAdapter);

			ImportModuleInfo AS400Mega = new ImportModuleInfo();
			AS400Mega.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPAS400MegaAdapter;
			AS400Mega.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPAS400MegaAdapter;
			AS400Mega.UserControlType = typeof(UpdateCatalogAS400MegaView);
			AS400Mega.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			AS400Mega.IsDefault = false;
			AS400Mega.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), AS400Mega.Name, AS400Mega, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogAS400MegaViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPAS400MegaAdapter);


			ImportModuleInfo moduleInfo = new ImportModuleInfo();
			moduleInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityGeneralCSVAdapter;
			moduleInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityGeneralCSVAdapter;
			moduleInfo.UserControlType = typeof(UpdateCatalogERPQuantityGeneralCSVView);
			moduleInfo.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			moduleInfo.IsDefault = false;
			moduleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), moduleInfo.Name, moduleInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityGeneralCSVViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityGeneralCSVAdapter);

			ImportModuleInfo AS400HamashbirInfo = new ImportModuleInfo();
			AS400HamashbirInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogAS400HamashbirAdapter;
			AS400HamashbirInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogAS400HamashbirAdapter;
			AS400HamashbirInfo.UserControlType = typeof(UpdateCatalogAS400HamashbirView);
			AS400HamashbirInfo.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			AS400HamashbirInfo.IsDefault = false;
			AS400HamashbirInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), AS400HamashbirInfo.Name, AS400HamashbirInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogAS400HamashbirViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogAS400HamashbirAdapter);

			
			ImportModuleInfo nativPlusInfo = new ImportModuleInfo();
			nativPlusInfo.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogNativPlusXslxAdapter;
			nativPlusInfo.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogNativPlusXslxAdapter;
			nativPlusInfo.UserControlType = typeof(UpdateCatalogNativPlusXslxAdapterView);
			nativPlusInfo.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			nativPlusInfo.IsDefault = false;
			nativPlusInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), nativPlusInfo.Name, nativPlusInfo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogNativPlusXslxAdapterViewModel), Common.Constants.UpdateCatalogAdapterName.UpdateCatalogNativPlusXslxAdapter);

			ImportModuleInfo ProfileXml = new ImportModuleInfo();
			ProfileXml.Name = Common.Constants.ImportAdapterName.ImportCatalogProfileXmlAdapter;
			ProfileXml.Title = Common.Constants.ImportAdapterTitle.ImportCatalogProfileXmlAdapter;
			ProfileXml.UserControlType = typeof(ImportCatalogProfileXmlAdapterView);
			ProfileXml.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			ProfileXml.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), ProfileXml.Name, ProfileXml, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogProfileXmlAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogProfileXmlAdapter);


			//PrioritytEsteeLouder
			ImportModuleInfo prioritytEsteeLouder = new ImportModuleInfo();
			prioritytEsteeLouder.Name = Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityPrioritytEsteeLouderAdapter;
			prioritytEsteeLouder.Title = Common.Constants.UpdateCatalogAdapterTitle.UpdateCatalogERPQuantityPrioritytEsteeLouderAdapter;
			prioritytEsteeLouder.UserControlType = typeof(UpdateCatalogERPQuantityPrioritytEsteeLouderView);
			prioritytEsteeLouder.ImportDomainEnum = ImportDomainEnum.UpdateCatalog;
			prioritytEsteeLouder.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), prioritytEsteeLouder.Name, prioritytEsteeLouder, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(UpdateCatalogERPQuantityPrioritytEsteeLouderViewModel),
				Common.Constants.UpdateCatalogAdapterName.UpdateCatalogERPQuantityPrioritytEsteeLouderAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("UpdateCatalogKitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
			
        }
    }
}