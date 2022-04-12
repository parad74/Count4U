using System.Windows.Controls;
using Count4U.Planogram.View;
using Count4U.Planogram.View.PlanObjects;

namespace Count4U.Planogram.Lib
{
    public class ToolShelf : ToolRectangle
    {
        protected override PlanObject GetObject(DrawingCanvas drawingCanvas)
        {            
            return new PlanShelf(drawingCanvas);
        }
    }
}