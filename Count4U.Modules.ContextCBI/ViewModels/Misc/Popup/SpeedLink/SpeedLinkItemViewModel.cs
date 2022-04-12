using Microsoft.Practices.Prism.Commands;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.SpeedLink
{
    public class SpeedLinkItemViewModel
    {
        public DelegateCommand Command { get; set; }
        public string Tooltip { get; set; }
        public string Image { get; set; }
    }
}