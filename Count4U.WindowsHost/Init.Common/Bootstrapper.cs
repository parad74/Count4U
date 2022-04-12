using System;
using System.Windows;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using NLog;
using ExcelReportUnion;
using Count4U.Model;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

namespace Main.Init
{
    public class Bootstrapper : UnityBootstrapper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

      
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            _logger.Info("ConfigureModuleCatalog");

            try
            {
                ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
				moduleCatalog.AddModule(typeof(ExcelReportUnionModuleInit));
                //moduleCatalog.AddModule(typeof(DBModuleInit));
     
            }
            catch (Exception exc)
            {
                _logger.DebugException("Module initialization error", exc);
            }
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            _logger.Info("InitializeShell");

            try
            {
				//this.Container.RegisterType<IUserSettingsManager, UserSettingsManager>(new ContainerControlledLifetimeManager());
				//this.Container.RegisterType<ISettingsRepository, SettingsRepository>(new ContainerControlledLifetimeManager());                
                this.Container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(this.Container));
				//this.Container.RegisterType<IDBSettings, DBSettings>(new ContainerControlledLifetimeManager());
				//this.Container.RegisterType<IZip, Zip>(new ContainerControlledLifetimeManager());
				//this.Container.RegisterType<IApplicationVersion, ApplicationVersion>(new ContainerControlledLifetimeManager());

				//this.Container.Resolve<ISettingsRepository>().LogPath = App.LogPath;             

				//Shell shell = this.Container.Resolve<Shell>();
				//_logger.Info("Shell.Resolve");
				//shell.EventAggregator = this.Container.Resolve<IEventAggregator>();
				//_logger.Info("EventAggregator.Resolve");
				//shell.Container = this.Container;
				//shell.Init();
				//_logger.Info("Shell.Init");
				//shell.Show();
				//_logger.Info("Shell.Show");
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

            System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    Type iModule = typeof(IModule);
                    List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(r => r.GetTypes()).Where(r => iModule.IsAssignableFrom(r)).ToList();

                    foreach (Type moduleType in types)
                    {
                        Assembly assembly = moduleType.Assembly;
                        _logger.Info(String.Format("{0}: {1}", assembly.FullName, assembly.ImageRuntimeVersion));
                    }
                });
        }


		protected override System.Windows.DependencyObject CreateShell()
		{
			//Form1 shell = new Form1();
			//this.Container.RegisterInstance(shell);
			//Application.Current.MainWindow = shell;
			//return shell;
			return null; // error
		}
	}
}