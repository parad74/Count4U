using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.ExportErpTextAdapter.AS400_Leumit;
using Count4U.ExportErpTextAdapter.AvivMultiBarcodes;
using Count4U.ExportErpTextAdapter.AvivPOS;
using Count4U.ExportErpTextAdapter.Comax;
using Count4U.ExportErpTextAdapter.Gazit;
using Count4U.ExportErpTextAdapter.GeneralCSV;
using Count4U.ExportErpTextAdapter.MiniSoft;
using Count4U.ExportErpTextAdapter.MirkamSonol;
using Count4U.ExportErpTextAdapter.MirkamSonolSAP;
using Count4U.ExportErpTextAdapter.MirkamTen;
using Count4U.ExportErpTextAdapter.PosSuperPharm;
using Count4U.ExportErpTextAdapter.PriorityKeds;
using Count4U.ExportErpTextAdapter.PriorityKeds.Matrix;
using Count4U.ExportErpTextAdapter.PriorityKeds.Regular;
using Count4U.ExportErpTextAdapter.PriorityRenuar;
using Count4U.ExportErpTextAdapter.RetalixPOS_HO;
using Count4U.ExportErpTextAdapter.Unizag;
using Count4U.ExportErpTextAdapter.XtechMeuhedet;
using Count4U.ExportErpTextAdapter.Yarpa;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Count4U.Model;
using Count4U.ExportErpTextAdapter.RetalixNext;
using Count4U.ExportErpTextAdapter.AS400AmericanEagle;
using Count4U.ExportErpTextAdapter.MaccabiPharmSAP;
using Count4U.ExportErpTextAdapter.MikiKupot;
using Count4U.ExportErpTextAdapter.LadyComfort;
using Count4U.ExportErpTextAdapter.Made4Net;
using Count4U.ExportErpTextAdapter.AS400Jafora;
using Count4U.ExportErpTextAdapter.Nibit;
using Count4U.ExportErpTextAdapter.OrenOriginals;
using Count4U.ExportErpTextAdapter.NimrodAviv;
using Count4U.ExportErpTextAdapter.H_M;
using Count4U.ExportErpTextAdapter.AS400April;
using Count4U.ExportErpTextAdapter.BazanCsv;
using Count4U.ExportErpTextAdapter.PriorityKedsShowRoom;
using Count4U.ExportErpTextAdapter.Hash;
using Count4U.ExportErpTextAdapter.AS400Mango;
using Count4U.ExportErpTextAdapter.FRSVisionMirkam;
using Count4U.ExportErpTextAdapter.AS400Honigman;
using Count4U.ExportErpTextAdapter.MPL;
using Count4U.ExportErpTextAdapter.TafnitMatrix;
using Count4U.ExportErpTextAdapter.PriorityCastro;
using Count4U.ExportErpTextAdapter.Otech;
using Count4U.ExportErpTextAdapter.NikeInt;
using Count4U.ExportErpTextAdapter.WarehouseXslx;
using Count4U.ExportErpTextAdapter.Sapb1Xslx;
using Count4U.ExportErpTextAdapter.Sapb1Zometsfarim;
using Count4U.ExportErpTextAdapter.GazitVerifoneSteimaztzky;
using Count4U.ExportErpTemplateAdapter.Merkava;
using Count4U.ExportErpTemplateAdapter.Clalit;
using Count4U.ExportErpTextAdapter.AS400Mega;
using Count4U.ExportErpTemplateAdapter.Nativ;
using Count4U.ExportErpTextAdapter.PriorityAldo;
using Count4U.ExportErpTextAdapter.PrioritySweetGirlXlsx;
using Count4U.ExportErpTemplateAdapter.NativPlus;
using Count4U.ExportErpTextAdapter.GeneralXslx;
using Count4U.ExportErpTextAdapter.AS400Hamashbir;
using Count4U.ExportErpTextAdapter.AS400Ho;
using Count4U.ExportErpTextAdapter.XtechMeuhedetXlsx;
using Count4U.ExportErpTextAdapter.H_M_New;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.ExportErpTemplateAdapter.NativPlusYes;
using Count4U.ExportErpTemplateAdapter.NativPlusLadpc;
using Count4U.ExportErpTextAdapter.GazitLeeCooper;
using Count4U.ExportErpTemplateAdapter.NativPlusMateAshe;
using Count4U.ExportErpTemplateAdapter.MerkavaGov;
using Count4U.ExportErpTemplateAdapter.NativPlusMisradApnim;
using Count4U.ExportErpTemplateAdapter.StockSonigoXslx;
using NLog;
using System;
using Count4U.ExportErpTextAdapter.OrenMutagim;
using Count4U.ExportErpTextAdapter.YtungXslx;
using Count4U.ExportErpTextAdapter.NativExportErp;

namespace Count4U.ExportErpTextAdapter
{
    public class ExportErpTextModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		  private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ExportErpTextModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
			_logger.Info("ExportErpTextModuleInit module initialization...");
			try
			{
            IExportErpModuleInfo def = new ExportErpModuleInfo();
            def.Name = Common.Constants.ExportErpAdapterName.ExportErpDefaultAdapter;
            def.Title = Common.Constants.ExportErpAdapterTitle.Default;
            def.UserControlType = typeof(ExportErpComaxAdapterView);
            def.Description = "";
            def.IsDefault = true;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), def.Name, def, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpComaxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpDefaultAdapter);
	
            IExportErpModuleInfo comax = new ExportErpModuleInfo();
            comax.Name = Common.Constants.ExportErpAdapterName.ExportErpComaxAdapter;
            comax.Title = Common.Constants.ExportErpAdapterTitle.Comax;
            comax.UserControlType = typeof(ExportErpComaxAdapterView);
            comax.Description = "";
            comax.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), comax.Name, comax, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpComaxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpComaxAdapter);

            IExportErpModuleInfo gazit = new ExportErpModuleInfo();
            gazit.Name = Common.Constants.ExportErpAdapterName.ExportErpGazitAdapter;
            gazit.Title = Common.Constants.ExportErpAdapterTitle.Gazit;
            gazit.UserControlType = typeof(ExportErpGazitAdapterView);
            gazit.Description = "";
            gazit.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), gazit.Name, gazit, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpGazitAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpGazitAdapter);

            IExportErpModuleInfo unizag = new ExportErpModuleInfo();
            unizag.Name = Common.Constants.ExportErpAdapterName.ExportErpUnizagAdapter;
            unizag.Title = Common.Constants.ExportErpAdapterTitle.Unizag;
            unizag.UserControlType = typeof(ExportErpUnizagAdapterView);
            unizag.Description = "";
            unizag.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), unizag.Name, unizag, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpUnizagAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpUnizagAdapter);

            IExportErpModuleInfo renuar = new ExportErpModuleInfo();
            renuar.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityRenuarAdapter;
            renuar.Title = Common.Constants.ExportErpAdapterTitle.PriorityRenuar;
            renuar.UserControlType = typeof(ExportErpPriorityRenuarAdapterView);
            renuar.Description = "";
            renuar.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), renuar.Name, renuar, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityRenuarAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityRenuarAdapter);
			
			IExportErpModuleInfo pharm = new ExportErpModuleInfo();
            pharm.Name = Common.Constants.ExportErpAdapterName.ExportErpPosSuperPharmAdapter;
            pharm.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPosSuperPharmAdapter;
            pharm.UserControlType = typeof(ExportErpPosSuperPharmAdapterView);
            pharm.Description = "";
            pharm.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), pharm.Name, pharm, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPosSuperPharmAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPosSuperPharmAdapter);

			//IExportErpModuleInfo xTech = new ExportErpModuleInfo();
			//xTech.Name = Common.Constants.ExportErpAdapterName.ExportErpXtechMeuhedetAdapter;
			//xTech.Title = Common.Constants.ExportErpAdapterTitle.ExportErpXtechMeuhedetAdapter;
			//xTech.UserControlType = typeof(ExportErpXtechMeuhedetAdapterView);
			//xTech.Description = "";
			//xTech.IsDefault = false;
			//this._container.RegisterInstance(typeof(IExportErpModuleInfo), xTech.Name, xTech, new ContainerControlledLifetimeManager());

			IExportErpModuleInfo xTech = new ExportErpModuleInfo();
			xTech.Name = Common.Constants.ExportErpAdapterName.ExportErpXtechMeuhedetXlsxAdapter;
			xTech.Title = Common.Constants.ExportErpAdapterTitle.ExportErpXtechMeuhedetXlsxAdapter;
			xTech.UserControlType = typeof(ExportErpXtechMeuhedetXlsxAdapterView);
			xTech.Description = "";
			xTech.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), xTech.Name, xTech, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpXtechMeuhedetXlsxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpXtechMeuhedetXlsxAdapter);

            IExportErpModuleInfo yarpa = new ExportErpModuleInfo();
            yarpa.Name = Common.Constants.ExportErpAdapterName.ExportErpYarpaAdapter;
            yarpa.Title = Common.Constants.ExportErpAdapterTitle.ExportErpYarpaAdapter;
            yarpa.UserControlType = typeof(ExportERPYarpaAdapterView);
            yarpa.Description = "";
            yarpa.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), yarpa.Name, yarpa, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpYarpaAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpYarpaAdapter);

            IExportErpModuleInfo generalCSV = new ExportErpModuleInfo();
            generalCSV.Name = Common.Constants.ExportErpAdapterName.ExportErpGeneralCSVAdapter;
            generalCSV.Title = Common.Constants.ExportErpAdapterTitle.ExportErpGeneralCSVAdapter;
			generalCSV.UserControlType = typeof(ExportErpGeneralCSVAdapterView);
            generalCSV.Description = "";
            generalCSV.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), generalCSV.Name, generalCSV, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpGeneralCSVAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpGeneralCSVAdapter);
			
			IExportErpModuleInfo avivPOS = new ExportErpModuleInfo();
            avivPOS.Name = Common.Constants.ExportErpAdapterName.ExportErpAvivPOSAdapter;
            avivPOS.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAvivPOSAdapter;
            avivPOS.UserControlType = typeof(ExportErpAvivPOSAdapterView);
            avivPOS.Description = "";
            avivPOS.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), avivPOS.Name, avivPOS, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAvivPOSAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAvivPOSAdapter);

            IExportErpModuleInfo avivMulti = new ExportErpModuleInfo();
            avivMulti.Name = Common.Constants.ExportErpAdapterName.ExportErpAvivMulitBarcodes;
            avivMulti.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAvivMulitBarcodes;
            avivMulti.UserControlType = typeof(ExportErpAvivMulitBarcodesView);
            avivMulti.Description = "";
            avivMulti.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), avivMulti.Name, avivMulti, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAvivMulitBarcodesViewModel), Common.Constants.ExportErpAdapterName.ExportErpAvivMulitBarcodes);
			
			IExportErpModuleInfo mirkamSonol = new ExportErpModuleInfo();
            mirkamSonol.Name = Common.Constants.ExportErpAdapterName.ExportErpMirkamSonolAdapter;
            mirkamSonol.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMirkamSonol;
            mirkamSonol.UserControlType = typeof(ExportErpMirkamSonolAdapterView);
            mirkamSonol.Description = "";
            mirkamSonol.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), mirkamSonol.Name, mirkamSonol, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMirkamSonolAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMirkamSonolAdapter);

            IExportErpModuleInfo mirkamSonolSAP = new ExportErpModuleInfo();
            mirkamSonolSAP.Name = Common.Constants.ExportErpAdapterName.ExportErpMirkamSonolSAPAdapter;
            mirkamSonolSAP.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMirkamSonolSAP;
            mirkamSonolSAP.UserControlType = typeof(ExportErpMirkamSonolSAPAdapterView);
            mirkamSonolSAP.Description = "";
            mirkamSonolSAP.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), mirkamSonolSAP.Name, mirkamSonolSAP, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMirkamSonolSAPAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMirkamSonolSAPAdapter);

            IExportErpModuleInfo mirkamTen = new ExportErpModuleInfo();
            mirkamTen.Name = Common.Constants.ExportErpAdapterName.ExportErpMirkamTenAdapter;
            mirkamTen.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMirkamTen;
            mirkamTen.UserControlType = typeof(ExportErpMirkamTenAdapterView);
            mirkamTen.Description = "";
            mirkamTen.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), mirkamTen.Name, mirkamTen, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMirkamTenAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMirkamTenAdapter);

            IExportErpModuleInfo priorityKedsRegular = new ExportErpModuleInfo();
            priorityKedsRegular.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsRegularAdapter;
            priorityKedsRegular.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPriorityKedsRegularAdapter;
            priorityKedsRegular.UserControlType = typeof(ExportErpPriorityKedsRegularAdapterView);
            priorityKedsRegular.Description = "";
            priorityKedsRegular.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), priorityKedsRegular.Name, priorityKedsRegular, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityKedsRegularAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsRegularAdapter);

            IExportErpModuleInfo priorityKedsMatrix = new ExportErpModuleInfo();
            priorityKedsMatrix.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsMatrixAdapter;
            priorityKedsMatrix.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPriorityKedsMatrixAdapter;
            priorityKedsMatrix.UserControlType = typeof(ExportErpPriorityKedsMatrixAdapterView);
            priorityKedsMatrix.Description = "";
            priorityKedsMatrix.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), priorityKedsMatrix.Name, priorityKedsMatrix, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityKedsMatrixAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsMatrixAdapter);

			IExportErpModuleInfo retalixnext = new ExportErpModuleInfo();
			retalixnext.Name = Common.Constants.ExportErpAdapterName.ExportErpRetalixNextAdapter;
			retalixnext.Title = Common.Constants.ExportErpAdapterTitle.ExportErpRetalixNext;
			retalixnext.UserControlType = typeof(ExportErpRetalixNextAdapterView);
			retalixnext.Description = "";
			renuar.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), retalixnext.Name, retalixnext, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpRetalixNextAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpRetalixNextAdapter);

            IExportErpModuleInfo retalixpos_ho = new ExportErpModuleInfo();
            retalixpos_ho.Name = Common.Constants.ExportErpAdapterName.ExportErpRetalixPOS_HOAdapter;
            retalixpos_ho.Title = Common.Constants.ExportErpAdapterTitle.ExportErpRetalixPOS_HO;
            retalixpos_ho.UserControlType = typeof(ExportErpRetalixPOS_HOAdapterView);
            retalixpos_ho.Description = "";
            renuar.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), retalixpos_ho.Name, retalixpos_ho, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpRetalixPOS_HOAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpRetalixPOS_HOAdapter);

            IExportErpModuleInfo as400_leumit = new ExportErpModuleInfo();
            as400_leumit.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400_LeumitAdapter;
            as400_leumit.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400_Leumit;
            as400_leumit.UserControlType = typeof(ExportERPAS400_LeumitAdapterView);
            as400_leumit.Description = "";
            as400_leumit.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), as400_leumit.Name, as400_leumit, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400_LeumitAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400_LeumitAdapter);

            IExportErpModuleInfo miniSoft = new ExportErpModuleInfo();
            miniSoft.Name = Common.Constants.ExportErpAdapterName.ExportErpMiniSoftAdapter;
            miniSoft.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMiniSoftAdapter;
            miniSoft.UserControlType = typeof(ExportErpMiniSoftAdapterView);
            miniSoft.Description = "";
            miniSoft.IsDefault = false;
            this._container.RegisterInstance(typeof(IExportErpModuleInfo), miniSoft.Name, miniSoft, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMiniSoftAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMiniSoftAdapter);

			IExportErpModuleInfo AS400AmericanEagle = new ExportErpModuleInfo();
			AS400AmericanEagle.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400AmericanEagleAdapter;
			AS400AmericanEagle.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400AmericanEagleAdapter;
			AS400AmericanEagle.UserControlType = typeof(ExportErpAS400AmericanEagleAdapterView);
			AS400AmericanEagle.Description = "";
			AS400AmericanEagle.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), AS400AmericanEagle.Name, AS400AmericanEagle, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400AmericanEagleAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400AmericanEagleAdapter);

			IExportErpModuleInfo maccabiPharmSAP = new ExportErpModuleInfo();
			maccabiPharmSAP.Name = Common.Constants.ExportErpAdapterName.ExportErpMaccabiPharmSAPAdapter;
			maccabiPharmSAP.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMaccabiPharmSAPAdapter;
			maccabiPharmSAP.UserControlType = typeof(ExportErpMaccabiPharmSAPView);
			maccabiPharmSAP.Description = "";
			maccabiPharmSAP.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), maccabiPharmSAP.Name, maccabiPharmSAP, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMaccabiPharmSAPAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMaccabiPharmSAPAdapter);

			IExportErpModuleInfo one1 = new ExportErpModuleInfo();
			one1.Name = Common.Constants.ExportErpAdapterName.ExportErpOne1Adapter;
			one1.Title = Common.Constants.ExportErpAdapterTitle.ExportErpOne1Adapter;
			one1.UserControlType = typeof(ExportErpOne1AdapterView);
			one1.Description = "";
			one1.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), one1.Name, one1, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpOne1AdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpOne1Adapter);

			IExportErpModuleInfo orenMutagim = new ExportErpModuleInfo();
			orenMutagim.Name = Common.Constants.ExportErpAdapterName.ExportErpOrenMutagimAdapter;
			orenMutagim.Title = Common.Constants.ExportErpAdapterTitle.ExportErpOrenMutagimAdapter;
			orenMutagim.UserControlType = typeof(ExportErpOrenMutagimAdapterView);
			orenMutagim.Description = "";
			orenMutagim.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), orenMutagim.Name, orenMutagim, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpOrenMutagimAdapterViewModel), 
				Common.Constants.ExportErpAdapterName.ExportErpOrenMutagimAdapter);

			IExportErpModuleInfo otech = new ExportErpModuleInfo();
			otech.Name = Common.Constants.ExportErpAdapterName.ExportErpOtechAdapter;
			otech.Title = Common.Constants.ExportErpAdapterTitle.ExportErpOtechAdapter;
			otech.UserControlType = typeof(ExportErpOtechAdapterView);
			otech.Description = "";
			otech.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), otech.Name, otech, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpOtechAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpOtechAdapter);


				IExportErpModuleInfo bazanCsv = new ExportErpModuleInfo();
				bazanCsv.Name = Common.Constants.ExportErpAdapterName.ExportErpBazanCsvAdapter;
				bazanCsv.Title = Common.Constants.ExportErpAdapterTitle.ExportErpBazanCsvAdapter;
				bazanCsv.UserControlType = typeof(ExportErpBazanCsvAdapterView);
				bazanCsv.Description = "";
				bazanCsv.IsDefault = false;
				this._container.RegisterInstance(typeof(IExportErpModuleInfo), bazanCsv.Name, bazanCsv, new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpBazanCsvAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpBazanCsvAdapter);


				IExportErpModuleInfo nativExportErp = new ExportErpModuleInfo();
				nativExportErp.Name = Common.Constants.ExportErpAdapterName.ExportErpNativExportErpAdapter;
				nativExportErp.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativExportErpAdapter;
				nativExportErp.UserControlType = typeof(ExportErpNativExportErpAdapterView);
				nativExportErp.Description = "";
				nativExportErp.IsDefault = false;
				this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativExportErp.Name, nativExportErp, new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativExportErpAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNativExportErpAdapter);


				IExportErpModuleInfo as400April = new ExportErpModuleInfo();
			as400April.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400AprilAdapter;
			as400April.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400AprilAdapter;
			as400April.UserControlType = typeof(ExportErpAS400AprilAdapterView);
			as400April.Description = "";
			as400April.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), as400April.Name, as400April, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400AprilAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400AprilAdapter);

			IExportErpModuleInfo H_M = new ExportErpModuleInfo();
			H_M.Name = Common.Constants.ExportErpAdapterName.ExportErpH_MAdapter;
			H_M.Title = Common.Constants.ExportErpAdapterTitle.ExportErpH_MAdapter;
			H_M.UserControlType = typeof(ExportErpH_MAdapterView);
			H_M.Description = "";
			H_M.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), H_M.Name, H_M, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpH_MAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpH_MAdapter);

			IExportErpModuleInfo H_M_New = new ExportErpModuleInfo();
			H_M_New.Name = Common.Constants.ExportErpAdapterName.ExportErpH_M_NewAdapter;
			H_M_New.Title = Common.Constants.ExportErpAdapterTitle.ExportErpH_M_NewAdapter;
			H_M_New.UserControlType = typeof(ExportErpH_M_NewAdapterView);
			H_M_New.Description = "";
			H_M_New.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), H_M_New.Name, H_M_New, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpH_M_NewAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpH_M_NewAdapter);


			IExportErpModuleInfo nimrodAviv = new ExportErpModuleInfo();
			nimrodAviv.Name = Common.Constants.ExportErpAdapterName.ExportErpNimrodAvivAdapter;
			nimrodAviv.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNimrodAvivAdapter;
			nimrodAviv.UserControlType = typeof(ExportErpNimrodAvivAdapterView);
			nimrodAviv.Description = "";
			nimrodAviv.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nimrodAviv.Name, nimrodAviv, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNimrodAvivAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNimrodAvivAdapter);

			IExportErpModuleInfo nibit = new ExportErpModuleInfo();
			nibit.Name = Common.Constants.ExportErpAdapterName.ExportErpNibitAdapter;
			nibit.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNibitAdapter;
			nibit.UserControlType = typeof(ExportErpNibitAdapterView);
			nibit.Description = "";
			nibit.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nibit.Name, nibit, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNibitAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNibitAdapter);

			IExportErpModuleInfo made4Net = new ExportErpModuleInfo();
			made4Net.Name = Common.Constants.ExportErpAdapterName.ExportErpMade4NetAdapter;
			made4Net.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMade4NetAdapter;
			made4Net.UserControlType = typeof(ExportErpMade4NetAdapterView);
			made4Net.Description = "";
			made4Net.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), made4Net.Name, made4Net, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMade4NetAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMade4NetAdapter);

			IExportErpModuleInfo tafnitMatrix = new ExportErpModuleInfo();
			tafnitMatrix.Name = Common.Constants.ExportErpAdapterName.ExportErpTafnitMatrixAdapter;
			tafnitMatrix.Title = Common.Constants.ExportErpAdapterTitle.ExportErpTafnitMatrixAdapter;
			tafnitMatrix.UserControlType = typeof(ExportErpTafnitMatrixAdapterView);
			tafnitMatrix.Description = "";
			tafnitMatrix.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), tafnitMatrix.Name, tafnitMatrix, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpTafnitMatrixAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpTafnitMatrixAdapter);

			IExportErpModuleInfo nikeInt = new ExportErpModuleInfo();
			nikeInt.Name = Common.Constants.ExportErpAdapterName.ExportErpNikeIntAdapter;
			nikeInt.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNikeIntAdapter;
			nikeInt.UserControlType = typeof(ExportErpNikeIntAdapterView);
			nikeInt.Description = "";
			nikeInt.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nikeInt.Name, nikeInt, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNikeIntAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNikeIntAdapter);


			IExportErpModuleInfo mpl = new ExportErpModuleInfo();
			mpl.Name = Common.Constants.ExportErpAdapterName.ExportErpMPLAdapter;
			mpl.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMPLAdapter;
			mpl.UserControlType = typeof(ExportErpMPLAdapterView);
			mpl.Description = "";
			mpl.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), mpl.Name, mpl, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMPLAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMPLAdapter);

			IExportErpModuleInfo as400Honigman = new ExportErpModuleInfo();
			as400Honigman.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400HonigmanAdapter;
			as400Honigman.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400HonigmanAdapter;
			as400Honigman.UserControlType = typeof(ExportErpAS400HonigmanAdapterView);
			as400Honigman.Description = "";
			as400Honigman.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), as400Honigman.Name, as400Honigman, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400HonigmanAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400HonigmanAdapter);
			
			IExportErpModuleInfo AS400Ho = new ExportErpModuleInfo();
			AS400Ho.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400HoAdapter;
			AS400Ho.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400HoAdapter;
			AS400Ho.UserControlType = typeof(ExportErpAS400HoAdapterView);
			AS400Ho.Description = "";
			AS400Ho.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), AS400Ho.Name, AS400Ho, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400HoAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400HoAdapter);


			IExportErpModuleInfo frsVisionMirkam = new ExportErpModuleInfo();
			frsVisionMirkam.Name = Common.Constants.ExportErpAdapterName.ExportErpFRSVisionMirkamAdapter;
			frsVisionMirkam.Title = Common.Constants.ExportErpAdapterTitle.ExportErpFRSVisionMirkamAdapter;
			frsVisionMirkam.UserControlType = typeof(ExportErpFRSVisionMirkamAdapterView);
			frsVisionMirkam.Description = "";
			frsVisionMirkam.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), frsVisionMirkam.Name, frsVisionMirkam, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpFRSVisionMirkamAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpFRSVisionMirkamAdapter);

			
			IExportErpModuleInfo as400Mango = new ExportErpModuleInfo();
			as400Mango.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400MangoAdapter;
			as400Mango.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400MangoAdapter;
			as400Mango.UserControlType = typeof(ExportErpAS400MangoAdapterView);
			as400Mango.Description = "";
			as400Mango.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), as400Mango.Name, as400Mango, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400MangoAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400MangoAdapter);

			IExportErpModuleInfo hash = new ExportErpModuleInfo();
			hash.Name = Common.Constants.ExportErpAdapterName.ExportErpHashAdapter;
			hash.Title = Common.Constants.ExportErpAdapterTitle.ExportErpHashAdapter;
			hash.UserControlType = typeof(ExportErpHashAdapterView);
			hash.Description = "";
			hash.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), hash.Name, hash, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpHashAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpHashAdapter);

			
			IExportErpModuleInfo warehouseXslx = new ExportErpModuleInfo();
			warehouseXslx.Name = Common.Constants.ExportErpAdapterName.ExportErpWarehouseXslxAdapter;
			warehouseXslx.Title = Common.Constants.ExportErpAdapterTitle.ExportErpWarehouseXslxAdapter;
			warehouseXslx.UserControlType = typeof(ExportErpWarehouseXslxAdapterView);
			warehouseXslx.Description = "";
			warehouseXslx.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), warehouseXslx.Name, warehouseXslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpWarehouseXslxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpWarehouseXslxAdapter);


			IExportErpModuleInfo priorityKedsShowRoom = new ExportErpModuleInfo();
			priorityKedsShowRoom.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.UserControlType = typeof(ExportErpPriorityKedsShowRoomAdapterView);
			priorityKedsShowRoom.Description = "";
			priorityKedsShowRoom.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), priorityKedsShowRoom.Name, priorityKedsShowRoom, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityKedsShowRoomAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityKedsShowRoomAdapter);

			IExportErpModuleInfo OrenOriginals = new ExportErpModuleInfo();
			OrenOriginals.Name = Common.Constants.ExportErpAdapterName.ExportErpOrenOriginalsAdapter;
			OrenOriginals.Title = Common.Constants.ExportErpAdapterTitle.ExportErpOrenOriginalsAdapter;
			OrenOriginals.UserControlType = typeof(ExportErpOrenOriginalsAdapterView);
			OrenOriginals.Description = "";
			OrenOriginals.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), OrenOriginals.Name, OrenOriginals, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpOrenOriginalsAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpOrenOriginalsAdapter);
			

			IExportErpModuleInfo mikiKupot = new ExportErpModuleInfo();
			mikiKupot.Name = Common.Constants.ExportErpAdapterName.ExportErpMikiKupotAdapter;
			mikiKupot.Title = Common.Constants.ExportErpAdapterTitle.ExportErpMikiKupotAdapter;
			mikiKupot.UserControlType = typeof(ExportErpMikiKupotAdapterView);
			mikiKupot.Description = "";
			mikiKupot.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), mikiKupot.Name, mikiKupot, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpMikiKupotAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpMikiKupotAdapter);

			IExportErpModuleInfo ladyComfort = new ExportErpModuleInfo();
			ladyComfort.Name = Common.Constants.ExportErpAdapterName.ExportErpLadyComfortAdapter;
			ladyComfort.Title = Common.Constants.ExportErpAdapterTitle.ExportErpLadyComfortAdapter;
			ladyComfort.UserControlType = typeof(ExportErpLadyComfortAdapterView);
			ladyComfort.Description = "";
			ladyComfort.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), ladyComfort.Name, ladyComfort, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpLadyComfortAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpLadyComfortAdapter);

			IExportErpModuleInfo as400Jafora = new ExportErpModuleInfo();
			as400Jafora.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400JaforaAdapter;
			as400Jafora.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400JaforaAdapter;
			as400Jafora.UserControlType = typeof(ExportErpAS400JaforaView);
			as400Jafora.Description = "";
			as400Jafora.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), as400Jafora.Name, as400Jafora, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400JaforaViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400JaforaAdapter);

			IExportErpModuleInfo priorityCastro = new ExportErpModuleInfo();
			priorityCastro.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityCastroAdapter;
			priorityCastro.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPriorityCastroAdapter;
			priorityCastro.UserControlType = typeof(ExportErpPriorityCastroAdapterView);
			priorityCastro.Description = "";
			priorityCastro.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), priorityCastro.Name, priorityCastro, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityCastroAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityCastroAdapter);

			IExportErpModuleInfo sapb1Xslx = new ExportErpModuleInfo();
			sapb1Xslx.Name = Common.Constants.ExportErpAdapterName.ExportErpSapb1XslxAdapter;
			sapb1Xslx.Title = Common.Constants.ExportErpAdapterTitle.ExportErpSapb1XslxAdapter;
			sapb1Xslx.UserControlType = typeof(ExportErpSapb1XslxAdapterView);
			sapb1Xslx.Description = "";
			sapb1Xslx.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), sapb1Xslx.Name, sapb1Xslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpSapb1XslxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpSapb1XslxAdapter);

			IExportErpModuleInfo sapb1Zometsfarim = new ExportErpModuleInfo();
			sapb1Zometsfarim.Name = Common.Constants.ExportErpAdapterName.ExportErpSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.Title = Common.Constants.ExportErpAdapterTitle.ExportErpSapb1ZometsfarimAdapter;
			sapb1Zometsfarim.UserControlType = typeof(ExportErpSapb1ZometsfarimView);
			sapb1Zometsfarim.Description = "";
			sapb1Zometsfarim.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), sapb1Zometsfarim.Name, sapb1Zometsfarim, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpSapb1ZometsfarimAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpSapb1ZometsfarimAdapter);

			IExportErpModuleInfo gazitVerifoneSteimaztzky = new ExportErpModuleInfo();
			gazitVerifoneSteimaztzky.Name = Common.Constants.ExportErpAdapterName.ExportErpGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.Title = Common.Constants.ExportErpAdapterTitle.ExportErpGazitVerifoneSteimaztzkyAdapter;
			gazitVerifoneSteimaztzky.UserControlType = typeof(ExportErpGazitVerifoneSteimaztzkyView);
			gazitVerifoneSteimaztzky.Description = "";
			gazitVerifoneSteimaztzky.IsDefault = true;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), gazitVerifoneSteimaztzky.Name, gazitVerifoneSteimaztzky, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpGazitVerifoneSteimaztzkyAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpGazitVerifoneSteimaztzkyAdapter);

			IExportErpModuleInfo merkava = new ExportErpModuleInfo();
			merkava.Name = Common.Constants.ExportErpAdapterName.ExportErpTemplateMerkavaAdapter;
			merkava.Title = Common.Constants.ExportErpAdapterTitle.ExportErpTemplateMerkavaAdapter;
			merkava.UserControlType = typeof(ExportErpTemplateMerkavaAdapterView);
			merkava.Description = "";
			merkava.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), merkava.Name, merkava, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpTemplateMerkavaAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpTemplateMerkavaAdapter);

			IExportErpModuleInfo merkavaGov = new ExportErpModuleInfo();
			merkavaGov.Name = Common.Constants.ExportErpAdapterName.ExportErpTemplateMerkavaGovAdapter;
			merkavaGov.Title = Common.Constants.ExportErpAdapterTitle.ExportErpTemplateMerkavaGovAdapter;
			merkavaGov.UserControlType = typeof(ExportErpTemplateMerkavaGovAdapterView);
			merkavaGov.Description = "";
			merkavaGov.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), merkavaGov.Name, merkavaGov, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpTemplateMerkavaGovAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpTemplateMerkavaGovAdapter);


			IExportErpModuleInfo clalit = new ExportErpModuleInfo();
			clalit.Name = Common.Constants.ExportErpAdapterName.ExportErpTemplateClalitAdapter;
			clalit.Title = Common.Constants.ExportErpAdapterTitle.ExportErpTemplateClalitAdapter;
			clalit.UserControlType = typeof(ExportErpTemplateClalitAdapterView);
			clalit.Description = "";
			clalit.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), clalit.Name, clalit, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpTemplateClalitAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpTemplateClalitAdapter);


			IExportErpModuleInfo AS400Mega = new ExportErpModuleInfo();
			AS400Mega.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400MegaAdapter;
			AS400Mega.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400MegaAdapter;
			AS400Mega.UserControlType = typeof(ExportErpAS400MegaAdapterView);
			AS400Mega.Description = "";
			AS400Mega.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), AS400Mega.Name, AS400Mega, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400MegaAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400MegaAdapter);


			IExportErpModuleInfo gazitLeeCooper = new ExportErpModuleInfo();
			gazitLeeCooper.Name = Common.Constants.ExportErpAdapterName.ExportErpGazitLeeCooperAdapter;
			gazitLeeCooper.Title = Common.Constants.ExportErpAdapterTitle.ExportErpGazitLeeCooperAdapter;
			gazitLeeCooper.UserControlType = typeof(ExportErpGazitLeeCooperAdapterView);
			gazitLeeCooper.Description = "";
			gazitLeeCooper.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), gazitLeeCooper.Name, gazitLeeCooper, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpGazitLeeCooperAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpGazitLeeCooperAdapter);

			

			IExportErpModuleInfo nativ = new ExportErpModuleInfo();
			nativ.Name = Common.Constants.ExportErpAdapterName.ExportErpNativAdapter;
			nativ.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativAdapter;
			nativ.UserControlType = typeof(ExportErpNativAdapterView);
			nativ.Description = "";
			nativ.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativ.Name, nativ, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNativAdapter);

			IExportErpModuleInfo nativPlusLadpc = new ExportErpModuleInfo();
			nativPlusLadpc.Name = Common.Constants.ExportErpAdapterName.ExportErpNativPlusLadpcAdapter;
			nativPlusLadpc.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativPlusLadpcAdapter;
			nativPlusLadpc.UserControlType = typeof(ExportErpNativPlusLadpcAdapterView);
			nativPlusLadpc.Description = "";
			nativPlusLadpc.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativPlusLadpc.Name, nativPlusLadpc, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel),
				typeof(ExportErpNativPlusLadpcAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNativPlusLadpcAdapter);


			IExportErpModuleInfo priorityAldo = new ExportErpModuleInfo();
			priorityAldo.Name = Common.Constants.ExportErpAdapterName.ExportErpPriorityAldoAdapter;
			priorityAldo.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPriorityAldoAdapter;
			priorityAldo.UserControlType = typeof(ExportErpPriorityAldoView);
			priorityAldo.Description = "";
			priorityAldo.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), priorityAldo.Name, priorityAldo, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPriorityAldoAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPriorityAldoAdapter);


			IExportErpModuleInfo prioritySweetGirl = new ExportErpModuleInfo();
			prioritySweetGirl.Name = Common.Constants.ExportErpAdapterName.ExportErpPrioritySweetGirlXlsxAdapter;
			prioritySweetGirl.Title = Common.Constants.ExportErpAdapterTitle.ExportErpPrioritySweetGirlXlsxAdapter;
			prioritySweetGirl.UserControlType = typeof(ExportErpPrioritySweetGirlXlsxAdapterView);
			prioritySweetGirl.Description = "";
			prioritySweetGirl.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), prioritySweetGirl.Name, prioritySweetGirl, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpPrioritySweetGirlXlsxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpPrioritySweetGirlXlsxAdapter);



			IExportErpModuleInfo nativPlus = new ExportErpModuleInfo();
			nativPlus.Name = Common.Constants.ExportErpAdapterName.ExportErpNativPlusAdapter;
			nativPlus.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativPlusAdapter;
			nativPlus.UserControlType = typeof(ExportErpNativPlusAdapterView);
			nativPlus.Description = "";
			nativPlus.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativPlus.Name, nativPlus, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativPlusAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpNativPlusAdapter);


			

			IExportErpModuleInfo nativPlusMateAshe = new ExportErpModuleInfo();
			nativPlusMateAshe.Name = Common.Constants.ExportErpAdapterName.ExportErpNativPlusMateAsheAdapter;
			nativPlusMateAshe.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativPlusMateAsheAdapter;
			nativPlusMateAshe.UserControlType = typeof(ExportErpNativPlusMateAsheAdapterView);
			nativPlusMateAshe.Description = "";
			nativPlusMateAshe.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativPlusMateAshe.Name, nativPlusMateAshe, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativPlusMateAsheAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpNativPlusMateAsheAdapter);


			IExportErpModuleInfo nativPlusYes = new ExportErpModuleInfo();
			nativPlusYes.Name = Common.Constants.ExportErpAdapterName.ExportErpNativPlusYesAdapter;
			nativPlusYes.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativPlusYesAdapter;
			nativPlusYes.UserControlType = typeof(ExportErpNativPlusYesAdapterView);
			nativPlusYes.Description = "";
			nativPlusYes.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), nativPlusYes.Name, nativPlusYes, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativPlusYesAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpNativPlusYesAdapter);

			IExportErpModuleInfo GeneralXslx = new ExportErpModuleInfo();
			GeneralXslx.Name = Common.Constants.ExportErpAdapterName.ExportErpGeneralXslxAdapter;
			GeneralXslx.Title = Common.Constants.ExportErpAdapterTitle.ExportErpGeneralXslxAdapter;
			GeneralXslx.UserControlType = typeof(ExportErpGeneralXslxAdapterView);
			GeneralXslx.Description = "";
			GeneralXslx.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), GeneralXslx.Name, GeneralXslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpGeneralXslxAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpGeneralXslxAdapter);


			IExportErpModuleInfo AS400Hamashbir = new ExportErpModuleInfo();
			AS400Hamashbir.Name = Common.Constants.ExportErpAdapterName.ExportErpAS400HamashbirAdapter;
			AS400Hamashbir.Title = Common.Constants.ExportErpAdapterTitle.ExportErpAS400HamashbirAdapter;
			AS400Hamashbir.UserControlType = typeof(ExportErpAS400HamashbirAdapterView);
			AS400Hamashbir.Description = "";
			AS400Hamashbir.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), AS400Hamashbir.Name, AS400Hamashbir, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpAS400HamashbirAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpAS400HamashbirAdapter);

			IExportErpModuleInfo NativPlusMisradApnim = new ExportErpModuleInfo();
			NativPlusMisradApnim.Name = Common.Constants.ExportErpAdapterName.ExportErpNativPlusMisradApnimAdapter;
			NativPlusMisradApnim.Title = Common.Constants.ExportErpAdapterTitle.ExportErpNativPlusMisradApnimAdapter;
			NativPlusMisradApnim.UserControlType = typeof(ExportErpNativPlusMisradApnimAdapterView);
			NativPlusMisradApnim.Description = "";
			NativPlusMisradApnim.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), NativPlusMisradApnim.Name, NativPlusMisradApnim, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpNativPlusMisradApnimAdapterViewModel), Common.Constants.ExportErpAdapterName.ExportErpNativPlusMisradApnimAdapter);


			IExportErpModuleInfo stockSonigoXslx = new ExportErpModuleInfo();
			stockSonigoXslx.Name = Common.Constants.ExportErpAdapterName.ExportErpStockSonigoXslxAdapter;
			stockSonigoXslx.Title = Common.Constants.ExportErpAdapterTitle.ExportErpStockSonigoXslxAdapter;
			stockSonigoXslx.UserControlType = typeof(ExportErpStockSonigoXslxAdapterView);
			stockSonigoXslx.Description = "";
			stockSonigoXslx.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), stockSonigoXslx.Name, stockSonigoXslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpStockSonigoXslxAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpStockSonigoXslxAdapter);

			IExportErpModuleInfo YtungXslx = new ExportErpModuleInfo();
			YtungXslx.Name = Common.Constants.ExportErpAdapterName.ExportErpYtungXslxAdapter;
			YtungXslx.Title = Common.Constants.ExportErpAdapterTitle.ExportErpYtungXslxAdapter;
			YtungXslx.UserControlType = typeof(ExportErpYtungXslxAdapterView);
			YtungXslx.Description = "";
			YtungXslx.IsDefault = false;
			this._container.RegisterInstance(typeof(IExportErpModuleInfo), YtungXslx.Name, YtungXslx, new ContainerControlledLifetimeManager());
			this._container.RegisterType(typeof(ExportErpModuleBaseViewModel), typeof(ExportErpYtungXslxAdapterViewModel),
				Common.Constants.ExportErpAdapterName.ExportErpYtungXslxAdapter);

				

			}
			catch (Exception exc)
			{
				_logger.Error("ExportErpTextModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }

    }
}