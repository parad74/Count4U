using System.Windows.Controls;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.Lib
{
    public abstract class PlanObject : UserControl
    {
        public abstract enPlanObjectType PlanType { get; }
        public string Code { get; set; }
        public virtual string PlanName { get; set; }
        public bool IsLocked { get; set; }
    }
}