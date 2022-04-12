using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Bootstrapper _bootstrapper;
      
        public App()
        {
			
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            WpfSingleInstance.Make();

            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			System.Windows.Forms.Application.ThreadException += Application_ThreadException; 

            //configure logging
            BootstrapLogger logger = new BootstrapLogger();
            LogPath = logger.ConfigureLogging();
			StartupArgumentDictionary = new Dictionary<string, string>(); 

            this._bootstrapper = new Bootstrapper();
			foreach (string argument in e.Args)
			{
				StartupArgumentDictionary[argument] = argument;
			}
            this._bootstrapper.Run();
        }

        public static string LogPath { get; set; }
		public static Dictionary<string, string> StartupArgumentDictionary { get; set; }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			_logger.Error("CurrentDomain_UnhandledException  " + e.ExceptionObject.ToString());
            HandleException();
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.ErrorException("TaskScheduler_UnobservedTaskException ", e.Exception);
            HandleException();
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error("App_DispatcherUnhandledException  " + e.Exception);
			HandleException();
			e.Handled = true; 
        }

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			HandleError(e.Exception, false);
		}

		void HandleError(Exception ex, bool isTerminating)
		{
			_logger.Error( (isTerminating) ? "Fatal error" : "Error", ex);
			UtilsLogging.WriteNavigationInfoToLog(this._bootstrapper.Container);
		}

        void HandleException()
        {
            UtilsLogging.WriteNavigationInfoToLog(this._bootstrapper.Container);

#if DEBUG
#else
         //   IApplicationVersion applicationVersion = this._bootstrapper.Container.Resolve<IApplicationVersion>();
         //   Dumper dumper = new Dumper();
         //   dumper.CreateMiniDump(applicationVersion.Get());
#endif
        }
      
    }
}
