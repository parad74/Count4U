using System.Windows.Controls;
using Count4U.Planogram.View;
using Count4U.Planogram.View.PlanObjects;

namespace Count4U.Planogram.Lib
{
    public class ToolWall : ToolRectangle
    {
        protected override PlanObject GetObject(DrawingCanvas drawingCanvas)
        {
            return new PlanWall();
        }
    }
}