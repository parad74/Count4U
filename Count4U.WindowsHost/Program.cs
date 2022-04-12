using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Main;
using Common.Main.Interface;
using Common.Main.Presenter;
using Common.Presenter.MainDB;
using Count4U.Common;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.GenerationReport;
using Count4U.GenerationReport.Settings;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.ServiceContract.Models;
using Count4U.Model.Transfer;
using Main.Service;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
//https://github.com/unitycontainer/examples/tree/v5.x/src/web/ASP.Net.Unity.Example
namespace Count4U.WindowsHost
{
    public static class Program
    {
		public static string[] ProgramArgs;
		//private Bootstrapper _bootstrapper;
		//private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		//private static readonly IUnityContainer _containerStatic = new UnityContainer();
   		private static readonly ModuleCatalog _moduleCatalog = new ModuleCatalog();
		private static IServiceLocator _serviceLocator;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
        {
			ProgramArgs = args;
			Database.SetInitializer<Process4ULogContext>(new Process4ULogContextInitializer());

			//IUnityContainer container = new UnityContainer();
			UnityContainer _container = Init.Common.UntContainer;
			//UnityDependencyResolver(UnityContainer);
			_container
			.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(_container))
			.RegisterInstance<IUnityContainer>(_container);

			InitUnity();

			_container
				//.RegisterType<Form1>()
				.RegisterType<IUserSettingsManager, UserSettingsManager>(new ContainerControlledLifetimeManager())
				.RegisterType<ISettingsRepository, SettingsRepository>(new ContainerControlledLifetimeManager())
				.RegisterType<IConnectionDB, ConnectionDB>(new ContainerControlledLifetimeManager())
				.RegisterType<IDBSettings, DBSettings>(new ContainerControlledLifetimeManager())
				.RegisterType<IZip, Zip>(new ContainerControlledLifetimeManager())
				.RegisterType<IAuditConfigRepository, AuditConfigEFRepository>(new ContainerControlledLifetimeManager());


		_container
				.RegisterType<IMainService, MainSerivce>()
				.RegisterType<IMainPresenter, MainPresenter>()
				.RegisterType<IMainView, Form1>()
			//	.RegisterType<ISip2TCPSocketServerPresenter, Sip2TCPSocketServerPresenter>()
				//.RegisterType<IRequest2ResponseServicePresenter, Request2ResponseServicePresenter>()
				.RegisterType<ILogMessageServicePresenter, LogMessageServicePresenter>()
				.RegisterType<IMainDBServicePresenter, MainDBServicePresenter>()
				;

		//		this.Container.Resolve<ISettingsRepository>().LogPath = App.LogPath;	  ??
		//	this.Container.Resolve<ISettingsRepository>().StartupArgumentDictionary = App.StartupArgumentDictionary;
 			
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			Application.Run((Form)_container.Resolve<IMainView>());
        }

		private static void InitUnity()
		{
			Init.Common.ConfigureModuleCatalog();
			Init.Common.InitializeModules();
			Init.Common.RegisterInterface();

		
			//TODO init from config ?
			Init.Common.ServerIP = "localhost";//Dns.GetHostName();	
			Init.Common.ServerTcpPort = "2744";
			Init.Common.ServerTcpPortMex = "2744";
		}

		public static string JoinRecord(this String[] records, string separator)
		{
			string ret = "";
			foreach (string record in records)
			{
				ret = ret + record + separator;
			}
			return ret;
		}
				//  ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
				//moduleCatalog.AddModule(typeof(ModelCommonModuleInit));
				//moduleCatalog.AddModule(typeof(DBModuleInit));

		//public static IServiceLocator ServiceLocatorResolve
		//{
		//	get { return _containerStatic.Resolve<IServiceLocator>(); }
		//}
		//public static IUnityContainer ContainerStatic
		//{
		//	get { return Program._containerStatic; }
		//}

		//public static void InitializeModules()
		//{

		//	Type iModule = typeof(IModule);
		//	//var types2 = AppDomain.CurrentDomain.GetAssemblies().ToList();
		//	List<Type> types1 = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

		//	System.Threading.Tasks.Task.Factory.StartNew(() =>
		//	{
		//		List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

		//		foreach (Type moduleType in types)
		//		{
		//			Assembly assembly = moduleType.Assembly;
		//			//_logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
		//		}
		//	});


		//	DBModuleInit.InitializeStatic(_containerStatic);
		//	//CommonViewModuleInit.InitializeStatic(_container);
		//	//CommonPresenterModuleInit.InitializeStatic(_container);
		//	//CommonUtilityModuleInit.InitializeStatic(_container);
		//	//CommonWCFModuleInit.InitializeStatic(_container);
		//	//ServiceClientModuleInit.InitializeStatic(_container);

		//	//ModelModuleInit.InitializeStatic(_container);
		//}

	

		
    }
}
