using Count4U.Common.Interfaces.Adapters;
using Count4U.ImportCatalogKitAdapter.One1;
using Count4U.ImportCatalogKitAdapter.RetalixNEXT;
using Count4U.ImportCatalogKitAdapter.AS400AmericanEagle;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.ImportCatalogKitAdapter.MaccabiPharmSAP;
using Count4U.ImportCatalogKitAdapter.MikiKupot;
using Count4U.ImportCatalogKitAdapter.LadyComfort;
using Count4U.ImportCatalogKitAdapter.Made4Net;
using Count4U.ImportCatalogKitAdapter.AS400Jafora;
using Count4U.ImportCatalogKitAdapter.Nibit;
using Count4U.ImportCatalogKitAdapter.OrenOriginals;
using Count4U.ImportCatalogKitAdapter.NimrodAviv;
using Count4U.ImportCatalogKitAdapter.H_M;
using Count4U.ImportCatalogKitAdapter.AS400April;
using Count4U.ImportCatalogKitAdapter.BazanCsv;
using Count4U.ImportCatalogKitAdapter.PriorityKedsShowRoom;
using Count4U.ImportCatalogKitAdapter.Hash;
using Count4U.ImportCatalogKitAdapter.AS400Mango;
using Count4U.ImportCatalogKitAdapter.FRSVisionMirkam;
using Count4U.ImportCatalogAS400HonigmanAdapter;
using Count4U.ImportCatalogMPLAdapter;
using Count4U.ImportCatalogKitAdapter.TafnitMatrix;
using Count4U.ImportCatalogKitAdapter.PriorityCastro;
using Count4U.ImportCatalogKitAdapter.Otech;
using Count4U.ImportCatalogKitAdapter.NikeInt;
using Count4U.ImportCatalogKitAdapter.WarehouseXslx;
using Count4U.ImportCatalogSapb1XslxAdapter;
using Count4U.ImportCatalogKitAdapter.MerkavaSqliteXslx;
using Count4U.ImportCatalogKitAdapter.MerkavaXslx;
using Count4U.ImportCatalogSapb1ZometsfarimAdapter;
using Count4U.ImportCatalogGazitVerifoneSteimaztzkyAdapter;
using Count4U.ImportCatalogKitAdapter.ClalitXslx;
using Count4U.ImportCatalogKitAdapter.AS400Mega;
using Count4U.ImportCatalogKitAdapter.NativXslx;
using Count4U.ImportCatalogKitAdapter.PriorityAldo;
using Count4U.ImportCatalogPrioritySweetGirlXlsxAdapter;
using Count4U.ImportCatalogKitAdapter.NativPlusXslx;
using Count4U.ImportCatalogKitAdapter.AS400Hamashbir;
using Count4U.ImportCatalogGazitAlufHaSportXlsxAdapter;
using Count4U.ImportCatalogAS400HoAdapter;
using Count4U.ImportCatalogYesXlsxAdapter;
using Count4U.ImportCatalogKitAdapter.H_M_New;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportCatalogKitAdapter.PrioritytEsteeLouderXslx;
using Count4U.ImportCatalogKitAdapter.GazitLeeCooper;
using Count4U.ImportCatalogNativPlusLadpcAdapter;
using Count4U.ImportCatalogGazitGlobalXlsxAdapter;
using NLog;
using System;
using Count4U.ImportCatalogKitAdapter.OrenMutagim;
using Count4U.ImportCatalogYtungXlsxAdapter;
using Count4U.ImportCatalogKitAdapter.Autosoft;
using Count4U.ImportCatalogKitAdapter.NativExportErp;

namespace Count4U.ImportCatalogKitAdapter
{
    public class ImportCatalogKitModuleInit : IModule
    {
        private readonly IUnityContainer _unityContainer;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImportCatalogKitModuleInit(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            //retalix next

			_logger.Info("ImportCatalogKitModuleInit module initialization...");
			try
			{
			ImportModuleInfo AS400AmericanEagle = new ImportModuleInfo();
			AS400AmericanEagle.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400AmericanEagleAdapter;
			AS400AmericanEagle.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400AmericanEagleAdapter;
			AS400AmericanEagle.UserControlType = typeof(ImportCatalogAS400AmericanEagleView);
			AS400AmericanEagle.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			AS400AmericanEagle.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), AS400AmericanEagle.Name, AS400AmericanEagle, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400AmericanEagleViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400AmericanEagleAdapter);
		
			ImportModuleInfo maccabiPharmSAP = new ImportModuleInfo();
			maccabiPharmSAP.Name = Common.Constants.ImportAdapterName.ImportCatalogMaccabiPharmSAPAdapter;
			maccabiPharmSAP.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMaccabiPharmSAPAdapter;
			maccabiPharmSAP.UserControlType = typeof(ImportCatalogMaccabiPharmSAPView);
			maccabiPharmSAP.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			maccabiPharmSAP.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), maccabiPharmSAP.Name, maccabiPharmSAP, new ContainerControlledLifetimeManager());
			IImportModuleInfo maccabiPharmSAP1 = this._unityContainer.Resolve<IImportModuleInfo>(Common.Constants.ImportAdapterName.ImportCatalogMaccabiPharmSAPAdapter);
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMaccabiPharmSAPViewModel), Common.Constants.ImportAdapterName.ImportCatalogMaccabiPharmSAPAdapter);


			ImportModuleInfo one1 = new ImportModuleInfo();
			one1.Name = Common.Constants.ImportAdapterName.ImportCatalogOne1Adapter;
			one1.Title = Common.Constants.ImportAdapterTitle.ImportCatalogOne1Adapter;
			one1.UserControlType = typeof(ImportCatalogOne1View);
			one1.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			one1.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), one1.Name, one1, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogOne1ViewModel), Common.Constants.ImportAdapterName.ImportCatalogOne1Adapter);

			ImportModuleInfo orenMutagim = new ImportModuleInfo();				
			orenMutagim.Name = Common.Constants.ImportAdapterName.ImportCatalogOrenMutagimAdapter;
			orenMutagim.Title = Common.Constants.ImportAdapterTitle.ImportCatalogOrenMutagimAdapter;
			orenMutagim.UserControlType = typeof(ImportCatalogOrenMutagimView);
			orenMutagim.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			orenMutagim.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), orenMutagim.Name, orenMutagim, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogOrenMutagimViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogOrenMutagimAdapter);

			ImportModuleInfo otech = new ImportModuleInfo();
			otech.Name = Common.Constants.ImportAdapterName.ImportCatalogOtechAdapter;
			otech.Title = Common.Constants.ImportAdapterTitle.ImportCatalogOtechAdapter;
			otech.UserControlType = typeof(ImportCatalogOtechView);
			otech.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			otech.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), otech.Name, otech, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogOtechViewModel), Common.Constants.ImportAdapterName.ImportCatalogOtechAdapter);


			ImportModuleInfo bazanCsv = new ImportModuleInfo();
			bazanCsv.Name = Common.Constants.ImportAdapterName.ImportCatalogBazanCsvAdapter;
			bazanCsv.Title = Common.Constants.ImportAdapterTitle.ImportCatalogBazanCsvAdapter;
			bazanCsv.UserControlType = typeof(ImportCatalogBazanCsvView);
			bazanCsv.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			bazanCsv.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), bazanCsv.Name, bazanCsv, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogBazanCsvViewModel), Common.Constants.ImportAdapterName.ImportCatalogBazanCsvAdapter);


			ImportModuleInfo as400April = new ImportModuleInfo();
			as400April.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400AprilAdapter;
			as400April.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400AprilAdapter;
			as400April.UserControlType = typeof(ImportCatalogAS400AprilView);
			as400April.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			as400April.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), as400April.Name, as400April, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400AprilViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400AprilAdapter);

			ImportModuleInfo H_M = new ImportModuleInfo();
			H_M.Name = Common.Constants.ImportAdapterName.ImportCatalogH_MAdapter;
			H_M.Title = Common.Constants.ImportAdapterTitle.ImportCatalogH_MAdapter;
			H_M.UserControlType = typeof(ImportCatalogH_MView);
			H_M.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			H_M.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), H_M.Name, H_M, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogH_MViewModel), Common.Constants.ImportAdapterName.ImportCatalogH_MAdapter);

			ImportModuleInfo H_M_New = new ImportModuleInfo();
			H_M_New.Name = Common.Constants.ImportAdapterName.ImportCatalogH_M_NewAdapter;
			H_M_New.Title = Common.Constants.ImportAdapterTitle.ImportCatalogH_M_NewAdapter;
			H_M_New.UserControlType = typeof(ImportCatalogH_M_NewView);
			H_M_New.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			H_M_New.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), H_M_New.Name, H_M_New, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogH_M_NewViewModel), Common.Constants.ImportAdapterName.ImportCatalogH_M_NewAdapter);


			ImportModuleInfo nimrodAviv = new ImportModuleInfo();
			nimrodAviv.Name = Common.Constants.ImportAdapterName.ImportCatalogNimrodAvivAdapter;
			nimrodAviv.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNimrodAvivAdapter;
			nimrodAviv.UserControlType = typeof(ImportCatalogNimrodAvivView);
			nimrodAviv.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			nimrodAviv.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), nimrodAviv.Name, nimrodAviv, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNimrodAvivViewModel), Common.Constants.ImportAdapterName.ImportCatalogNimrodAvivAdapter);


			ImportModuleInfo nibit = new ImportModuleInfo();
			nibit.Name = Common.Constants.ImportAdapterName.ImportCatalogNibitAdapter;
			nibit.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNibitAdapter;
			nibit.UserControlType = typeof(ImportCatalogNibitView);
			nibit.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			nibit.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), nibit.Name, nibit, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNibitViewModel), Common.Constants.ImportAdapterName.ImportCatalogNibitAdapter);

			ImportModuleInfo made4Net = new ImportModuleInfo();
			made4Net.Name = Common.Constants.ImportAdapterName.ImportCatalogMade4NetAdapter;
			made4Net.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMade4NetAdapter;
			made4Net.UserControlType = typeof(ImportCatalogMade4NetView);
			made4Net.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			made4Net.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), made4Net.Name, made4Net, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMade4NetViewModel), Common.Constants.ImportAdapterName.ImportCatalogMade4NetAdapter);

			ImportModuleInfo tafnitMatrix = new ImportModuleInfo();
			tafnitMatrix.Name = Common.Constants.ImportAdapterName.ImportCatalogTafnitMatrixAdapter;
			tafnitMatrix.Title = Common.Constants.ImportAdapterTitle.ImportCatalogTafnitMatrixAdapter;
			tafnitMatrix.UserControlType = typeof(ImportCatalogTafnitMatrixView);
			tafnitMatrix.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			tafnitMatrix.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), tafnitMatrix.Name, tafnitMatrix, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogTafnitMatrixViewModel), Common.Constants.ImportAdapterName.ImportCatalogTafnitMatrixAdapter);


			ImportModuleInfo nikeInt = new ImportModuleInfo();
			nikeInt.Name = Common.Constants.ImportAdapterName.ImportCatalogNikeIntAdapter;
			nikeInt.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNikeIntAdapter;
			nikeInt.UserControlType = typeof(ImportCatalogNikeIntView);
			nikeInt.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			nikeInt.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), nikeInt.Name, nikeInt, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNikeIntViewModel), Common.Constants.ImportAdapterName.ImportCatalogNikeIntAdapter);
			
			ImportModuleInfo shalevetCsv = new ImportModuleInfo();
			shalevetCsv.Name = Common.Constants.ImportAdapterName.ImportCatalogShalevetCsvAdapter;
			shalevetCsv.Title = Common.Constants.ImportAdapterTitle.ImportCatalogShalevetCsvAdapter;
			shalevetCsv.UserControlType = typeof(ImportCatalogShalevetCsvView);
			shalevetCsv.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			shalevetCsv.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), shalevetCsv.Name, shalevetCsv, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogShalevetCsvViewModel), Common.Constants.ImportAdapterName.ImportCatalogShalevetCsvAdapter);


			ImportModuleInfo mpl = new ImportModuleInfo();
			mpl.Name = Common.Constants.ImportAdapterName.ImportCatalogMPLAdapter;
			mpl.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMPLAdapter;
			mpl.UserControlType = typeof(ImportCatalogMPLAdapterView);
			mpl.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			mpl.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), mpl.Name, mpl, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMPLAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogMPLAdapter);


			ImportModuleInfo as400Honigman = new ImportModuleInfo();
			as400Honigman.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400HonigmanAdapter;
			as400Honigman.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400HonigmanAdapter;
			as400Honigman.UserControlType = typeof(ImportCatalogAS400HonigmanAdapterView);
			as400Honigman.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			as400Honigman.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), as400Honigman.Name, as400Honigman, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400HonigmanAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400HonigmanAdapter);


			ImportModuleInfo frsVisionMirkam = new ImportModuleInfo();
			frsVisionMirkam.Name = Common.Constants.ImportAdapterName.ImportCatalogFRSVisionMirkamAdapter;
			frsVisionMirkam.Title = Common.Constants.ImportAdapterTitle.ImportCatalogFRSVisionMirkamAdapter;
			frsVisionMirkam.UserControlType = typeof(ImportCatalogFRSVisionMirkamView);
			frsVisionMirkam.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			frsVisionMirkam.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), frsVisionMirkam.Name, frsVisionMirkam, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogFRSVisionMirkamViewModel), Common.Constants.ImportAdapterName.ImportCatalogFRSVisionMirkamAdapter);

			ImportModuleInfo as400Mango = new ImportModuleInfo();
			as400Mango.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400MangoAdapter;
			as400Mango.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400MangoAdapter;
			as400Mango.UserControlType = typeof(ImportCatalogAS400MangoView);
			as400Mango.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			as400Mango.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), as400Mango.Name, as400Mango, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400MangoViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400MangoAdapter);


			ImportModuleInfo hash = new ImportModuleInfo();
			hash.Name = Common.Constants.ImportAdapterName.ImportCatalogHashAdapter;
			hash.Title = Common.Constants.ImportAdapterTitle.ImportCatalogHashAdapter;
			hash.UserControlType = typeof(ImportCatalogHashView);
			hash.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			hash.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), hash.Name, hash, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogHashViewModel), Common.Constants.ImportAdapterName.ImportCatalogHashAdapter);

			ImportModuleInfo warehouseXslx = new ImportModuleInfo();
			warehouseXslx.Name = Common.Constants.ImportAdapterName.ImportCatalogWarehouseXslxAdapter;
			warehouseXslx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogWarehouseXslxAdapter;
			warehouseXslx.UserControlType = typeof(ImportCatalogWarehouseXslxView);
			warehouseXslx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			warehouseXslx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), warehouseXslx.Name, warehouseXslx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogWarehouseXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogWarehouseXslxAdapter);

			ImportModuleInfo priorityKedsShowRoom = new ImportModuleInfo();
			priorityKedsShowRoom.Name = Common.Constants.ImportAdapterName.ImportCatalogPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.UserControlType = typeof(ImportCatalogPriorityKedsShowRoomView);
			priorityKedsShowRoom.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			priorityKedsShowRoom.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), priorityKedsShowRoom.Name, priorityKedsShowRoom, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPriorityKedsShowRoomViewModel), Common.Constants.ImportAdapterName.ImportCatalogPriorityKedsShowRoomAdapter);

			ImportModuleInfo OrenOriginals = new ImportModuleInfo();
			OrenOriginals.Name = Common.Constants.ImportAdapterName.ImportCatalogOrenOriginalsAdapter;
			OrenOriginals.Title = Common.Constants.ImportAdapterTitle.ImportCatalogOrenOriginalsAdapter;
			OrenOriginals.UserControlType = typeof(ImportCatalogOrenOriginalsView);
			OrenOriginals.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			OrenOriginals.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), OrenOriginals.Name, OrenOriginals, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogOrenOriginalsViewModel), Common.Constants.ImportAdapterName.ImportCatalogOrenOriginalsAdapter);
			
			ImportModuleInfo retalixNext = new ImportModuleInfo();
			retalixNext.Name = Common.Constants.ImportAdapterName.ImportCatalogRetalixNextAdapter;
			retalixNext.Title = Common.Constants.ImportAdapterTitle.ImportCatalogRetalixNextAdapter;
			retalixNext.UserControlType = typeof(ImportCatalogRetalixNextView);
			retalixNext.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			retalixNext.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), retalixNext.Name, retalixNext, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogRetalixNextViewModel), Common.Constants.ImportAdapterName.ImportCatalogRetalixNextAdapter);

			ImportModuleInfo mikiKupot = new ImportModuleInfo();
			mikiKupot.Name = Common.Constants.ImportAdapterName.ImportCatalogMikiKupotAdapter;
			mikiKupot.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMikiKupotAdapter;
			mikiKupot.UserControlType = typeof(ImportCatalogMikiKupotView);
			mikiKupot.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			mikiKupot.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), mikiKupot.Name, mikiKupot, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMikiKupotViewModel), Common.Constants.ImportAdapterName.ImportCatalogMikiKupotAdapter);

			ImportModuleInfo ladyComfort = new ImportModuleInfo();
			ladyComfort.Name = Common.Constants.ImportAdapterName.ImportCatalogLadyComfortAdapter;
			ladyComfort.Title = Common.Constants.ImportAdapterTitle.ImportCatalogLadyComfortAdapter;
			ladyComfort.UserControlType = typeof(ImportCatalogLadyComfortView);
			ladyComfort.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			ladyComfort.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), ladyComfort.Name, ladyComfort, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogLadyComfortViewModel), Common.Constants.ImportAdapterName.ImportCatalogLadyComfortAdapter);

			ImportModuleInfo AS400Jafora = new ImportModuleInfo();
			AS400Jafora.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400JaforaAdapter;
			AS400Jafora.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400JaforaAdapter;
			AS400Jafora.UserControlType = typeof(ImportCatalogAS400JaforaView);
			AS400Jafora.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			AS400Jafora.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), AS400Jafora.Name, AS400Jafora, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400JaforaViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400JaforaAdapter);

			ImportModuleInfo priorityCastro = new ImportModuleInfo();
			priorityCastro.Name = Common.Constants.ImportAdapterName.ImportCatalogPriorityCastroAdapter;
			priorityCastro.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPriorityCastroAdapter;
			priorityCastro.UserControlType = typeof(ImportCatalogPriorityCastroView);
			priorityCastro.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			priorityCastro.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), priorityCastro.Name, priorityCastro, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPriorityCastroViewModel), Common.Constants.ImportAdapterName.ImportCatalogPriorityCastroAdapter);

			ImportModuleInfo sapb1Xslx = new ImportModuleInfo();
			sapb1Xslx.Name = Common.Constants.ImportAdapterName.ImportCatalogSapb1XslxAdapter;
			sapb1Xslx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogSapb1XslxAdapter;
			sapb1Xslx.UserControlType = typeof(ImportCatalogSapb1XslxAdapterView);
			sapb1Xslx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			sapb1Xslx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), sapb1Xslx.Name, sapb1Xslx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogSapb1XslxAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogSapb1XslxAdapter);

			ImportModuleInfo merkavaSqliteXslx = new ImportModuleInfo();
			merkavaSqliteXslx.Name = Common.Constants.ImportAdapterName.ImportCatalogMerkavaSqliteXslxAdapter;
			merkavaSqliteXslx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMerkavaSqliteXslxAdapter;
			merkavaSqliteXslx.UserControlType = typeof(ImportCatalogMerkavaSqliteXslxView);
			merkavaSqliteXslx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			merkavaSqliteXslx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), merkavaSqliteXslx.Name, merkavaSqliteXslx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMerkavaSqliteXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogMerkavaSqliteXslxAdapter);

			ImportModuleInfo merkavaXslx = new ImportModuleInfo();
			merkavaXslx.Name = Common.Constants.ImportAdapterName.ImportCatalogMerkavaXslxAdapter;
			merkavaXslx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogMerkavaXslxAdapter;
			merkavaXslx.UserControlType = typeof(ImportCatalogMerkavaXslxView);
			merkavaXslx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			merkavaXslx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), merkavaXslx.Name, merkavaXslx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogMerkavaXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogMerkavaXslxAdapter);

			ImportModuleInfo clalitXslx = new ImportModuleInfo();
			clalitXslx.Name = Common.Constants.ImportAdapterName.ImportCatalogClalitXslxAdapter;
			clalitXslx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogClalitXslxAdapter;
			clalitXslx.UserControlType = typeof(ImportCatalogClalitXslxView);
			clalitXslx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			clalitXslx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), clalitXslx.Name, clalitXslx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogClalitXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogClalitXslxAdapter);

			ImportModuleInfo sapb1Zometsfarim = new ImportModuleInfo();
			sapb1Zometsfarim.Name = Common.Constants.ImportAdapterName.ImportCatalogSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.Title = Common.Constants.ImportAdapterTitle.ImportCatalogSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.UserControlType = typeof(ImportCatalogSapb1ZometsfarimAdapterView);
			sapb1Zometsfarim.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			sapb1Zometsfarim.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), sapb1Zometsfarim.Name, sapb1Zometsfarim, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogSapb1ZometsfarimAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogSapb1ZometsfarimAdapter);

			ImportModuleInfo gazitVerifoneSteimaztzky = new ImportModuleInfo();
			gazitVerifoneSteimaztzky.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.UserControlType = typeof(ImportCatalogGazitVerifoneSteimaztzkyAdapterView);
			gazitVerifoneSteimaztzky.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			gazitVerifoneSteimaztzky.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), gazitVerifoneSteimaztzky.Name, gazitVerifoneSteimaztzky, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGazitVerifoneSteimaztzkyAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogGazitVerifoneSteimaztzkyAdapter);


			ImportModuleInfo AS400Mega = new ImportModuleInfo();
			AS400Mega.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400MegaAdapter;
			AS400Mega.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400MegaAdapter;
			AS400Mega.UserControlType = typeof(ImportCatalogAS400MegaView);
			AS400Mega.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			AS400Mega.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), AS400Mega.Name, AS400Mega, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400MegaViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400MegaAdapter);


			ImportModuleInfo gazitLeeCooper = new ImportModuleInfo();
			gazitLeeCooper.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitLeeCooperAdapter;
			gazitLeeCooper.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitLeeCooperAdapter;
			gazitLeeCooper.UserControlType = typeof(ImportCatalogGazitLeeCooperView);
			gazitLeeCooper.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			gazitLeeCooper.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), gazitLeeCooper.Name, gazitLeeCooper, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGazitLeeCooperViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogGazitLeeCooperAdapter);

			

			ImportModuleInfo Nativ = new ImportModuleInfo();
			Nativ.Name = Common.Constants.ImportAdapterName.ImportCatalogNativXslxAdapter;
			Nativ.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNativXslxAdapter;
			Nativ.UserControlType = typeof(ImportCatalogNativXslxView);
			Nativ.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			Nativ.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), Nativ.Name, Nativ, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNativXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogNativXslxAdapter);

			ImportModuleInfo NativPlus = new ImportModuleInfo();
			NativPlus.Name = Common.Constants.ImportAdapterName.ImportCatalogNativPlusXslxAdapter;
			NativPlus.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNativPlusXslxAdapter;
			NativPlus.UserControlType = typeof(ImportCatalogNativPlusXslxView);
			NativPlus.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			NativPlus.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), NativPlus.Name, NativPlus, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNativPlusXslxViewModel), Common.Constants.ImportAdapterName.ImportCatalogNativPlusXslxAdapter);

			ImportModuleInfo priorityAldo = new ImportModuleInfo();
			priorityAldo.Name = Common.Constants.ImportAdapterName.ImportCatalogPriorityAldoAdapter;
			priorityAldo.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPriorityAldoAdapter;
			priorityAldo.UserControlType = typeof(ImportCatalogPriorityAldoView);
			priorityAldo.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			priorityAldo.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), priorityAldo.Name, priorityAldo, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPriorityAldoViewModel), Common.Constants.ImportAdapterName.ImportCatalogPriorityAldoAdapter);

								 //ImportCatalogPrioritySweetGirlXlsxAdapter
			ImportModuleInfo prioritySweetGirl = new ImportModuleInfo();
			prioritySweetGirl.Name = Common.Constants.ImportAdapterName.ImportCatalogPrioritySweetGirlXlsxAdapter;
			prioritySweetGirl.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPrioritySweetGirlXlsxAdapter;
			prioritySweetGirl.UserControlType = typeof(ImportCatalogPrioritySweetGirlXlsxAdapterView);
			prioritySweetGirl.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			prioritySweetGirl.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), prioritySweetGirl.Name, prioritySweetGirl, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPrioritySweetGirlXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogPrioritySweetGirlXlsxAdapter);


			//ImportCatalogAS400HamashbirAdapter
			ImportModuleInfo as400Hamashbir = new ImportModuleInfo();
			as400Hamashbir.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400HamashbirAdapter;
			as400Hamashbir.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400HamashbirAdapter;
			as400Hamashbir.UserControlType = typeof(ImportCatalogAS400HamashbirAdapterView);
			as400Hamashbir.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			as400Hamashbir.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), as400Hamashbir.Name, as400Hamashbir, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400HamashbirAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400HamashbirAdapter);

			//ImportCatalogGazitAlufHaSportXlsxAdapter
			ImportModuleInfo GazitAlufHaSport = new ImportModuleInfo();
			GazitAlufHaSport.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitAlufHaSportXlsxAdapter;
			GazitAlufHaSport.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitAlufHaSportXlsxAdapter;
			GazitAlufHaSport.UserControlType = typeof(ImportCatalogGazitAlufHaSportXlsxAdapterView);
			GazitAlufHaSport.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			GazitAlufHaSport.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), GazitAlufHaSport.Name, GazitAlufHaSport, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGazitAlufHaSportXlsxAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogGazitAlufHaSportXlsxAdapter);


			ImportModuleInfo AS400Ho = new ImportModuleInfo();
			AS400Ho.Name = Common.Constants.ImportAdapterName.ImportCatalogAS400HoAdapter;
			AS400Ho.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAS400HoAdapter;
			AS400Ho.UserControlType = typeof(ImportCatalogAS400HoAdapterView);
			AS400Ho.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			AS400Ho.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), AS400Ho.Name, AS400Ho, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAS400HoAdapterViewModel), Common.Constants.ImportAdapterName.ImportCatalogAS400HoAdapter);


			ImportModuleInfo YesXlsx = new ImportModuleInfo();
			YesXlsx.Name = Common.Constants.ImportAdapterName.ImportCatalogYesXlsxAdapter;
			YesXlsx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogYesXlsxAdapter;
			YesXlsx.UserControlType = typeof(ImportCatalogYesXlsxAdapterView);
			YesXlsx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			YesXlsx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), YesXlsx.Name, YesXlsx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogYesXlsxAdapterViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogYesXlsxAdapter);
					  
			ImportModuleInfo YtungXlsx = new ImportModuleInfo();
			YtungXlsx.Name = Common.Constants.ImportAdapterName.ImportCatalogYtungXlsxAdapter;
			YtungXlsx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogYtungXlsxAdapter;
			YtungXlsx.UserControlType = typeof(ImportCatalogYtungXlsxAdapterView);
			YtungXlsx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			YtungXlsx.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), YtungXlsx.Name, YtungXlsx, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogYtungXlsxAdapterViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogYtungXlsxAdapter);


				ImportModuleInfo NativPlusLadpc = new ImportModuleInfo();
				NativPlusLadpc.Name = Common.Constants.ImportAdapterName.ImportCatalogNativPlusLadpcAdapter;
				NativPlusLadpc.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNativPlusLadpcAdapter;
				NativPlusLadpc.UserControlType = typeof(ImportCatalogNativPlusLadpcAdapterView);
				NativPlusLadpc.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
				NativPlusLadpc.Description = "";
				this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), NativPlusLadpc.Name, NativPlusLadpc, new ContainerControlledLifetimeManager());
				this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNativPlusLadpcAdapterViewModel),
					Common.Constants.ImportAdapterName.ImportCatalogNativPlusLadpcAdapter);





				ImportModuleInfo NativExportErp = new ImportModuleInfo();
				NativExportErp.Name = Common.Constants.ImportAdapterName.ImportCatalogNativExportErpAdapter;
				NativExportErp.Title = Common.Constants.ImportAdapterTitle.ImportCatalogNativExportErpAdapter;
				NativExportErp.UserControlType = typeof(ImportCatalogNativExportErpAdapterView);
				NativExportErp.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
				NativExportErp.Description = "";
				this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), NativExportErp.Name, NativExportErp, new ContainerControlledLifetimeManager());
				this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogNativExportErpAdapterViewModel),
					Common.Constants.ImportAdapterName.ImportCatalogNativExportErpAdapter);

				ImportModuleInfo prioritytEsteeLouder = new ImportModuleInfo();
			prioritytEsteeLouder.Name = Common.Constants.ImportAdapterName.ImportCatalogPrioritytEsteeLouderXslxAdapter;
			prioritytEsteeLouder.Title = Common.Constants.ImportAdapterTitle.ImportCatalogPrioritytEsteeLouderXslxAdapter;
			prioritytEsteeLouder.UserControlType = typeof(ImportCatalogPrioritytEsteeLouderXslxView);
			prioritytEsteeLouder.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			prioritytEsteeLouder.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), prioritytEsteeLouder.Name, prioritytEsteeLouder, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogPrioritytEsteeLouderXslxViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogPrioritytEsteeLouderXslxAdapter);


				//ImportCatalogGazitGlobalXlsxAdapter
				ImportModuleInfo gazitGlobalXlsx = new ImportModuleInfo();
				gazitGlobalXlsx.Name = Common.Constants.ImportAdapterName.ImportCatalogGazitGlobalXlsxAdapter;
				gazitGlobalXlsx.Title = Common.Constants.ImportAdapterTitle.ImportCatalogGazitGlobalXlsxAdapter;
				gazitGlobalXlsx.UserControlType = typeof(ImportCatalogGazitGlobalXlsxAdapterView);
				gazitGlobalXlsx.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
				gazitGlobalXlsx.Description = "";
				this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), gazitGlobalXlsx.Name, gazitGlobalXlsx, new ContainerControlledLifetimeManager());
				this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogGazitGlobalXlsxAdapterViewModel),
					Common.Constants.ImportAdapterName.ImportCatalogGazitGlobalXlsxAdapter);

			ImportModuleInfo autosoft = new ImportModuleInfo();
			autosoft.Name = Common.Constants.ImportAdapterName.ImportCatalogAutosoftAdapter;
			autosoft.Title = Common.Constants.ImportAdapterTitle.ImportCatalogAutosoftAdapter;
			autosoft.UserControlType = typeof(ImportCatalogAutosoftView);
			autosoft.ImportDomainEnum = ImportDomainEnum.ImportCatalog;
			autosoft.Description = "";
			this._unityContainer.RegisterInstance(typeof(IImportModuleInfo), autosoft.Name, autosoft, new ContainerControlledLifetimeManager());
			this._unityContainer.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportCatalogAutosoftViewModel),
				Common.Constants.ImportAdapterName.ImportCatalogAutosoftAdapter);

				

			}
			catch (Exception exc)
			{
				_logger.Error("ImportCatalogKitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}