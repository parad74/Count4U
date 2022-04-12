using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Count4U.Common;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Configuration;
using Count4U.GenerationReport.Settings;
using Count4U.Model.Interface;
using Count4U.Model.Transfer;
using Count4U.Modules.Audit;
using Count4U.Modules.ContextCBI;
using Count4U.Modules.Prepare;
using Count4U.GenerationReport;
using Count4U.Planogram;
using Count4U.Services;
using Count4U.UpdateCatalogAdapters;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Count4U.Model;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Count4U.Common.Extensions;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using System.Threading.Tasks;
using Count4U.Model.ProcessC4U;
using Count4U.Model.Interface.ProcessC4U;

namespace Count4U
{
    public class Bootstrapper : UnityBootstrapper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected override DependencyObject CreateShell()
        {
            Shell shell = new Shell();
            this.Container.RegisterInstance(shell);
            Application.Current.MainWindow = shell;
		
            return shell;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
			
            _logger.Info("ConfigureModuleCatalog");

            try
            {
                ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
                moduleCatalog.AddModule(typeof(ModelCommonModuleInit));
                moduleCatalog.AddModule(typeof(DBModuleInit));

                moduleCatalog.AddModule(typeof(ConfigurationModuleInit));
                moduleCatalog.AddModule(typeof(CommonModuleInit));

                moduleCatalog.AddModule(typeof(AuditModuleInit));
                moduleCatalog.AddModule(typeof(ContextCBIModuleInit));

                //this one is referenced directly
                moduleCatalog.AddModule(typeof(UpdateCatalogAdaptersModuleInit));

                moduleCatalog.AddModule(typeof(ReportModuleInit));
                moduleCatalog.AddModule(typeof(ExportImportModuleInit));                
                moduleCatalog.AddModule(typeof(PlanogramModeInit));

                moduleCatalog.AddModule(typeof(PrepareModuleInit)); //must be last
            }
            catch (Exception exc)
            {
                _logger.DebugException("Module initialization error", exc);
            }
        }

        protected override void InitializeShell()
        {
            base.InitializeShell(); //ничего не делает, рекомендованно не вызывать

            _logger.Info("InitializeShell");

            try
            {
                this.Container.RegisterType<IUserSettingsManager, UserSettingsManager>(new ContainerControlledLifetimeManager());
                this.Container.RegisterType<ISettingsRepository, SettingsRepository>(new ContainerControlledLifetimeManager());                
                this.Container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(this.Container));
                this.Container.RegisterType<IDBSettings, DBSettings>(new ContainerControlledLifetimeManager());
				this.Container.RegisterType<IConnectionDB, ConnectionDB>(new ContainerControlledLifetimeManager());
                this.Container.RegisterType<IZip, Zip>(new ContainerControlledLifetimeManager());
                this.Container.RegisterType<IApplicationVersion, ApplicationVersion>(new ContainerControlledLifetimeManager());
				this.Container.RegisterType<IAuditConfigRepository, AuditConfigEFRepository>(new ContainerControlledLifetimeManager());
				this.Container.RegisterType<IProcessRepository, ProcessEFRepository>(new ContainerControlledLifetimeManager());				 
			

                this.Container.Resolve<ISettingsRepository>().LogPath = App.LogPath;
				this.Container.Resolve<ISettingsRepository>().StartupArgumentDictionary = App.StartupArgumentDictionary;
				string ProcessCode = "";
				
			
				try
				{
					IServiceLocator serviceLocator = this.Container.Resolve<IServiceLocator>();
					IProcessRepository processRepository = serviceLocator.GetInstance<IProcessRepository>();
					ProcessCode = processRepository.GetProcessCode_InProcess();
				}
				catch(Exception exp)
				{
					_logger.Error("InitializeShell : GetProcessCode_InProcess()" + exp.Message);
				}
				this.Container.Resolve<ISettingsRepository>().ProcessCode = ProcessCode;

                Shell shell = this.Container.Resolve<Shell>();
                _logger.Info("Shell.Resolve");
                shell.EventAggregator = this.Container.Resolve<IEventAggregator>();
                _logger.Info("EventAggregator.Resolve");
                shell.Container = this.Container;
                shell.Init();
                _logger.Info("Shell.Init");
				 shell.Show();
			    _logger.Info("Shell.Show");
            }
            catch (Exception exc)
            {
                _logger.DebugException("InitializeShell error", exc);
            }
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            List<String> originalPathes = new List<string>();

            string baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            originalPathes.Add(FileSystem.ImportModulesFolderPath());
            originalPathes.Add(FileSystem.ExportModulesFolderPath());

            string temporaryPath = PrepareAdapters(originalPathes);
            if (String.IsNullOrEmpty(temporaryPath) || !Directory.Exists(temporaryPath))
                return null;            

            return new DirectoryModuleCatalog() { ModulePath = temporaryPath };
        }

        private string PrepareAdapters(IEnumerable<string> originalPathes)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string temporaryAdaptersFolders = String.Empty;
            try
            {
                const string adaptersFolderName = "ImportModules";
                string userCount4U = FileSystem.UserCount4UFolder();

                temporaryAdaptersFolders = Path.Combine(userCount4U, adaptersFolderName);

                if (Directory.Exists(temporaryAdaptersFolders))
                {
                    foreach (string file in Directory.EnumerateFiles(temporaryAdaptersFolders))
                    {
                        _logger.Trace(String.Format("File delete: {0}", file));
                        File.Delete(file);
                    }

                    _logger.Trace(String.Format("Directory delete: {0}", temporaryAdaptersFolders));
                    Directory.Delete(temporaryAdaptersFolders);
                }

                Directory.CreateDirectory(temporaryAdaptersFolders);

                foreach (string originalPath in originalPathes)
                {
                    DirectoryInfo di = new DirectoryInfo(originalPath);
                    foreach (FileInfo fi in di.EnumerateFiles())
                    {
                        fi.CopyTo(Path.Combine(temporaryAdaptersFolders, fi.Name));
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("PrepareAdapters", exc);
                sw.Stop();
                // return String.Empty;
            }

            sw.Stop();
            System.Diagnostics.Debug.Print(String.Format("Adapters prepare = {0}", sw.ElapsedMilliseconds.ToString()));

            return temporaryAdaptersFolders;
        }

		protected override void InitializeModules()
        {
            base.InitializeModules();
	 
			try
			{
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                    Type iModule = typeof(IModule);
                    List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

                    foreach (Type moduleType in types)
                    {
                        Assembly assembly = moduleType.Assembly;
                        _logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
                    }
				}).LogTaskFactoryExceptions("InitializeModules");

			}
			catch (ReflectionTypeLoadException ex)
			{
				_logger.Info("InitializeModules problem ReflectionTypeLoadException >> ");
				foreach (var item in ex.LoaderExceptions)
				{
					_logger.Error(item.Message);
					//FileNotFoundException exFileNotFound = item as FileNotFoundException;
					//if (exFileNotFound != null)
					//{
					//	if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
					//	{
					//		_logger.Error(exFileNotFound.FusionLog);
					//	}
					//}
				}
			}
			catch (Exception exc)
			{
				_logger.Error("InitializeModules" + exc.Message + " StackTrace >>" + exc.StackTrace);
			}
	   	 	
		}
	
	

		//protected override void InitializeModules()
		//{
		//	_logger.Info("Bootstrapper.InitializeModules start");
		//	base.InitializeModules();
		//	_logger.Info("Bootstrapper.InitializeModules end");
		//	Type iModule = typeof(IModule);
		//	_logger.Info("Bootstrapper IModule");
		//	List<Type> types = new List<Type>();
		//	try
		//	{
		//		//types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();
		//		types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).ToList();
		//	}
		//	catch (ReflectionTypeLoadException ex)
		//	{
		//		_logger.Info("InitializeModules problem ReflectionTypeLoadException >> ");
		//		foreach (var item in ex.LoaderExceptions)
		//		{
		//			_logger.Error(item.Message);
		//			//FileNotFoundException exFileNotFound = item as FileNotFoundException;
		//			//if (exFileNotFound != null)
		//			//{
		//			//	if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
		//			//	{
		//			//		_logger.Error(exFileNotFound.FusionLog);
		//			//	}
		//			//}
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.Error("InitializeModules" + exc.Message + " StackTrace >>" + exc.StackTrace);
		//	}


		//	foreach (Type moduleType in types)
		//	{
		//		if (moduleType == null) continue;
		//		if (iModule.IsAssignableFrom(moduleType) == true)
		//		{
		//			try
		//			{
		//				Assembly assembly = moduleType.Assembly;
		//				if (assembly == null) continue;
		//				_logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.Error("InitializeModules1" + moduleType.FullName + exc.Message + " StackTrace1 >>" + exc.StackTrace);
		//			}

		//		}
		//	}
		//}


		//protected override void InitializeModules()		//Работает на SETup
		//{
		//	_logger.Info("Bootstrapper.InitializeModules start");
		//	base.InitializeModules();
		//	_logger.Info("Bootstrapper.InitializeModules end");
		//	Type iModule = typeof(IModule);
		//	_logger.Info("Bootstrapper IModule");
		//	List<Type> types = new List<Type>();
		//	try
		//	{
		//		//types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();
		//		var types1 = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes());//.ToList();

		//		_logger.Info("InitializeModules Select Assemblies [" + types1.Count() + "]");
		//		foreach (Type type1 in types1)
		//		{
		//			if (type1 == null) continue;
		//			try
		//			{
		//				types.Add(type1);
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.Error("InitializeModules problem" + type1.FullName + exc.Message + " StackTrace1 >>" + exc.StackTrace);
		//			}
		//		}
		//	}
		//	catch (ReflectionTypeLoadException ex)
		//	{
		//		_logger.Info("InitializeModules problem ReflectionTypeLoadException >> ");
		//		foreach (var item in ex.LoaderExceptions)
		//		{
		//			//MessageBox.Show(item.Message);
		//			_logger.Error(item.Message);
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.Error("InitializeModules" + exc.Message + " StackTrace >>" + exc.StackTrace);
		//	}


		//	foreach (Type moduleType in types)
		//	{
		//		if (moduleType == null) continue;
		//		if (iModule.IsAssignableFrom(moduleType) == true)
		//		{
		//			try
		//			{
		//				Assembly assembly = moduleType.Assembly;
		//				if (assembly == null) continue;
		//				_logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.Error("InitializeModules1" + moduleType.FullName + exc.Message + " StackTrace1 >>" + exc.StackTrace);
		//			}

		//		}
		//	}
		//}

	
		//protected override void InitializeModules()
		//{
		//	base.InitializeModules();

		//	Type iModule = typeof(IModule);
		//	List<Type> types = new List<Type>();
		//	try
		//	{
		//		 types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.Error("InitializeModules" + exc.Message + " StackTrace >>" + exc.StackTrace);
		//	}
		//	foreach (Type moduleType in types)
		//	{
		//		if (moduleType == null) continue;
		//		if (iModule.IsAssignableFrom(moduleType) == true)
		//		{
		//			try
		//			{
		//				Assembly assembly = moduleType.Assembly;
		//				if (assembly == null) continue;
		//				_logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.Error("InitializeModules1" + exc.Message + " StackTrace1 >>" + exc.StackTrace);
		//			}

		//		}
		//	}
		//}

    }
}