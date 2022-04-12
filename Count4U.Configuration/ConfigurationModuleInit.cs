using System;
using Count4U.Common.Interfaces;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.FilterTemplate;
using Count4U.Configuration.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Configuration
{
	public class ConfigurationModuleInit : IModule
	{
		private readonly IUnityContainer _container;
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ConfigurationModuleInit(IUnityContainer container)
		{
			_container = container;
		}

		public void Initialize()
		{
			_logger.Info("ConfigurationModuleInit module initialization...");
			try
			{
				this._container.RegisterType<DynamicRepository, DynamicRepository>(new TransientLifetimeManager());
				this._container.RegisterType<EditorTemplateComboViewModel, EditorTemplateComboViewModel>(new TransientLifetimeManager());

				this._container.RegisterType(typeof(IPropertyLinkRepository), typeof(PropertyLinkRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IEditorTemplateRepository), typeof(EditorTemplateRepository), new ContainerControlledLifetimeManager());
				this._container.RegisterType(typeof(IAllowedAsPropertyLinkRepository), typeof(AllowedAsPropertyLinkRepository), new ContainerControlledLifetimeManager());

				this._container.RegisterType(typeof(IFilterTemplateRepository), typeof(FilterTemplateRepository), new ContainerControlledLifetimeManager());
			}
			catch (Exception exc)
			{
				_logger.Error("ConfigurationModuleInit " + exc.Message + " StackTrace : " + exc.StackTrace);
			}
		}
	}
}
	