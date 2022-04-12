using Common.Utility.Interface;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;

namespace Common.Utility
{
    public class CommonUtilityModuleInit : IModule
    {
        private readonly IUnityContainer _container;

		public CommonUtilityModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        #region Implementation of IModule

        public void Initialize()
        {
            //services
			//this._container.RegisterType(typeof(ITask), typeof(Task), new ContainerControlledLifetimeManager());
			_container.RegisterType(typeof(ILog), typeof(LogImport), new ContainerControlledLifetimeManager());

         }

		public static void InitializeStatic(IUnityContainer _containerStatic)
		{
			_containerStatic.RegisterType(typeof(ILog), typeof(LogImport), new ContainerControlledLifetimeManager());
		}
        #endregion
    }
}