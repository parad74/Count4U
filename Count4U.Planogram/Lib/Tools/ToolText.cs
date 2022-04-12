using Count4U.Planogram.View.PlanObjects;

namespace Count4U.Planogram.Lib
{
    public class ToolText : ToolRectangle
    {
        protected override PlanObject GetObject(DrawingCanvas drawingCanvas)
        {
            return new PlanText();
        }
    }
}