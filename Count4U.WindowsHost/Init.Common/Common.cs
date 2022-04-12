using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NLog;
using Common.Presenter;
using Common.View;
using System.Threading.Tasks;
using Common.Utility;
using Common.WCF;
using Count4U.Common;
using Count4U.Model;
using Count4U.GenerationReport;
using Count4U.Model.ServiceClient;
using Common.Utility.Constant;
using Count4U.Model.Service;


namespace Init
{
	public static class Common
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private static readonly UnityContainer _container = new UnityContainer();
		private static readonly ModuleCatalog _moduleCatalog = new ModuleCatalog();


		public static void ConfigureModuleCatalog()
		{
			_logger.Info("ConfigureModuleCatalog");
			try
			{
				//Count4U
				_moduleCatalog.AddModule(typeof(CommonModuleInit));
				_moduleCatalog.AddModule(typeof(DBModuleInit));
				_moduleCatalog.AddModule(typeof(ModelCommonModuleInit));
				_moduleCatalog.AddModule(typeof(ExportImportModuleInit));


				_moduleCatalog.AddModule(typeof(CommonViewModuleInit));
				_moduleCatalog.AddModule(typeof(CommonPresenterModuleInit));
				_moduleCatalog.AddModule(typeof(CommonUtilityModuleInit));
				_moduleCatalog.AddModule(typeof(CommonWCFModuleInit));
				_moduleCatalog.AddModule(typeof(ServiceMainWpfImplementationInit));
				//_moduleCatalog.AddModule(typeof(ServiceProxyClientModuleInit));

				//_moduleCatalog.AddModule(typeof(ModelModuleInit));


			}
			catch (Exception exc)
			{
				_logger.DebugException("Module initialization error", exc);
			}

		}

		public static void InitializeModules()
		{

			Type iModule = typeof(IModule);
			//var types2 = AppDomain.CurrentDomain.GetAssemblies().ToList();
			List<Type> types1 = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

				foreach (Type moduleType in types)
				{
					Assembly assembly = moduleType.Assembly;
					_logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
				}
			});

			//====== Count4U
			DBModuleInit.InitializeStatic(_container);

			//=====
			CommonViewModuleInit.InitializeStatic(_container);
			CommonPresenterModuleInit.InitializeStatic(_container);
			CommonUtilityModuleInit.InitializeStatic(_container);
			CommonWCFModuleInit.InitializeStatic(_container);
			ServiceMainWpfImplementationInit.InitializeStatic(_container);
			//ServiceProxyClientModuleInit.InitializeStatic(_container);

			//ModelModuleInit.InitializeStatic(_container);
		}


		public static void RegisterInterface()
		{
			_container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(_container));
			GloobalConstantInit();
		}

		public static ModuleCatalog ModuleCatalog
		{
			get { return _moduleCatalog; }
		}


		public static UnityContainer UntContainer
		{
			get { return _container; }
		}

		public static IServiceLocator ServiceLocatorResolve
		{
			get { return _container.Resolve<IServiceLocator>(); }
		}

		public static void GloobalConstantInit()
		{
			GlogalConstantStatic.ServiceLocatorStatic = ServiceLocatorResolve;
			GlogalConstantStatic.UnityContainerStatic = UntContainer;

		}

		public static string ServerIP { get; set; }
		public static string ServerTcpPort { get; set; }
		public static string ServerTcpPortMex { get; set; }
		//public static string ServerHttpPort { get; set; }
		//public static string ServerHttpPortMex { get; set; }
	}
}
