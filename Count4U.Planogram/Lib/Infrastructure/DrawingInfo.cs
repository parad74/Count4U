using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.Lib.Infrastructure
{
    public class DrawingInfo
    {
        public string SourceObjectCode { get; set; }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }
        public enPlanObjectType PlanType { get; set; }
        public string PlanName { get; set; }

        public string PlanText { get; set; }
        public int PlanFontSize { get; set; }
        public string PlanFontColor { get; set; }

        public string PlanPicture { get; set; }
    }
}