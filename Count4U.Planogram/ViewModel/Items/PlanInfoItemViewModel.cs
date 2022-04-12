using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Planogram.ViewModel
{
    public class PlanInfoItemViewModel : NotificationObject
    {
        public string Color { get; set; }
        public int Number { get; set; }
        public string NumberOfProducts { get; set; }
    }
}