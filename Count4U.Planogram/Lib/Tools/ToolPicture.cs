using Count4U.Model.Interface;
using Count4U.Planogram.View.PlanObjects;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Unity;

namespace Count4U.Planogram.Lib
{
    public class ToolPicture : ToolRectangle
    {
        private readonly IUnityContainer _container;

        public ToolPicture(IUnityContainer container)
        {
            _container = container;
        }

        protected override PlanObject GetObject(DrawingCanvas drawingCanvas)
        {
            return new PlanPicture(_container.Resolve<IDBSettings>());
        }
    }
}