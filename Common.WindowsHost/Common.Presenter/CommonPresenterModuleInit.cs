using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;

namespace Common.Presenter
{
    public class CommonPresenterModuleInit : IModule
    {
        private readonly IUnityContainer _container;

		public CommonPresenterModuleInit(IUnityContainer container)
        {
            this._container = container;
        }

        #region Implementation of IModule

        public void Initialize()
        {
            //services
			//this._container.RegisterType(typeof(ITask), typeof(Task), new ContainerControlledLifetimeManager());
         }

		public static void InitializeStatic(IUnityContainer _containerStatic)
		{
		}
        #endregion
    }
}