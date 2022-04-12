using Count4U.GenerationReport.Settings;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using System;
using NLog;

namespace Count4U.GenerationReport
{
    public class ModelCommonModuleInit : IModule
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnityContainer _container;
	
		public ModelCommonModuleInit(IUnityContainer container)
        {
            _container = container;

           
        }

        #region Implementation of IModule

        public void Initialize()
        {
			_logger.Info("Initialize ModelCommonModule");

          
			try
			{
				_logger.Info("Initialize ModelCommonModule - OK");
			}
            catch (Exception ex)
            {
				_logger.ErrorException("Initialize ModelCommonModule failed", ex);
            }

        }

        #endregion
    }
}