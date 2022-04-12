using System;
using Count4U.Common.Interfaces;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.ImportFamilyKitAdapter.AS400Hamashbir;
using Count4U.ImportFamilyKitAdapter.Default;
using Count4U.ImportFamilyKitAdapter.H_M;
using Count4U.ImportFamilyKitAdapter.LadyComfort;
using Count4U.ImportFamilyKitAdapter.PriorityKedsShowRoom;
using Count4U.ImportFamilyKitAdapter.PriorityRenuar;
using Count4U.Model;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.ImportFamilyKitAdapter
{
    public class ImportFamilyKitModuleInit : IModule
    {
        private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ImportFamilyKitModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        public void Initialize()
        {
			_logger.Info("ImportFamilyKitModuleInit module initialization...");
			try
			{
			ImportModuleInfo defaultModuleInfo = new ImportModuleInfo();
			defaultModuleInfo.Name = Common.Constants.ImportAdapterName.ImportFamilyDefaultAdapter;
			defaultModuleInfo.Title = Common.Constants.ImportAdapterTitle.ImportFamilyDefaultAdapter;
			defaultModuleInfo.UserControlType = typeof(ImportFamilyDefaultAdapterView);
			defaultModuleInfo.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			defaultModuleInfo.IsDefault = true;
			defaultModuleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), defaultModuleInfo.Name, defaultModuleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo hmModuleInfo = new ImportModuleInfo();
			hmModuleInfo.Name = Common.Constants.ImportAdapterName.ImportFamilyH_MAdapter;
			hmModuleInfo.Title = Common.Constants.ImportAdapterTitle.ImportFamilyH_MAdapter;
			hmModuleInfo.UserControlType = typeof(ImportFamilyH_MAdapterView);
			hmModuleInfo.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			hmModuleInfo.IsDefault = true;
			hmModuleInfo.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), hmModuleInfo.Name, hmModuleInfo, new ContainerControlledLifetimeManager());

			ImportModuleInfo ladyComfort = new ImportModuleInfo();
			ladyComfort.Name = Common.Constants.ImportAdapterName.ImportFamilyLadyComfortAdapter;
			ladyComfort.Title = Common.Constants.ImportAdapterTitle.ImportFamilyLadyComfortAdapter;
			ladyComfort.UserControlType = typeof(ImportFamilyLadyComfortAdapterView);
			ladyComfort.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			ladyComfort.IsDefault = true;
			ladyComfort.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), ladyComfort.Name, ladyComfort, new ContainerControlledLifetimeManager());

			ImportModuleInfo priorityKedsShowRoom = new ImportModuleInfo();
			priorityKedsShowRoom.Name = Common.Constants.ImportAdapterName.ImportFamilyPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.Title = Common.Constants.ImportAdapterTitle.ImportFamilyPriorityKedsShowRoomAdapter;
			priorityKedsShowRoom.UserControlType = typeof(ImportFamilyPriorityKedsShowRoomAdapterView);
			priorityKedsShowRoom.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			priorityKedsShowRoom.IsDefault = true;
			priorityKedsShowRoom.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), priorityKedsShowRoom.Name, priorityKedsShowRoom, new ContainerControlledLifetimeManager());

			ImportModuleInfo priorityRenuar = new ImportModuleInfo();
			priorityRenuar.Name = Common.Constants.ImportAdapterName.ImportFamilyPriorityRenuarAdapter;
			priorityRenuar.Title = Common.Constants.ImportAdapterTitle.ImportFamilyPriorityRenuarAdapter;
			priorityRenuar.UserControlType = typeof(ImportFamilyPriorityRenuarAdapterView);
			priorityRenuar.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			priorityRenuar.IsDefault = true;
			priorityRenuar.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), priorityRenuar.Name, priorityRenuar, new ContainerControlledLifetimeManager());


			ImportModuleInfo AS400Hamashbir = new ImportModuleInfo();
			AS400Hamashbir.Name = Common.Constants.ImportAdapterName.ImportFamilyAS400HamashbirAdapter;
			AS400Hamashbir.Title = Common.Constants.ImportAdapterTitle.ImportFamilyAS400HamashbirAdapter;
			AS400Hamashbir.UserControlType = typeof(ImportFamilyAS400HamashbirAdapterView);
			AS400Hamashbir.ImportDomainEnum = ImportDomainEnum.ImportFamily;
			AS400Hamashbir.IsDefault = true;
			AS400Hamashbir.Description = "";
			this._container.RegisterInstance(typeof(IImportModuleInfo), AS400Hamashbir.Name, AS400Hamashbir, new ContainerControlledLifetimeManager());


			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyDefaultAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyDefaultAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyH_MAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyH_MAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyLadyComfortAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyLadyComfortAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyPriorityKedsShowRoomAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyPriorityKedsShowRoomAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyPriorityRenuarAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyPriorityRenuarAdapter);
			this._container.RegisterType(typeof(ImportModuleBaseViewModel), typeof(ImportFamilyAS400HamashbirAdapterViewModel), Common.Constants.ImportAdapterName.ImportFamilyAS400HamashbirAdapter);
			}
			catch (Exception exc)
			{
				_logger.Error("ImportFamilyKitModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
        }
    }
}
