using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;

namespace Count4U.Modules.ContextCBI.ViewModels.Strip
{
    public class StripBaseViewModel : CBIContextBaseViewModel
    {
        public StripBaseViewModel(IContextCBIRepository contextCBIRepository)
            : base(contextCBIRepository)
        {
            Column = 0;
            ColumnSpan = 1;
        }

        public int Column { get; set; }
        public int ColumnSpan { get; set; }
    }
}