using System;
using System.Collections.ObjectModel;
using System.Windows;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.SpeedLink
{
    public class SpeedLinkViewModel : CBIContextBaseViewModel
    {
        private readonly DelegateCommand _closeCommand;

        private readonly ObservableCollection<SpeedLinkItemViewModel> _items;

        public SpeedLinkViewModel(
            IContextCBIRepository contextCbiRepository)
            : base(contextCbiRepository)
        {
            _closeCommand = new DelegateCommand(CloseCommandExecuted);
            _items = new ObservableCollection<SpeedLinkItemViewModel>();
        }

        public FrameworkElement View { get; set; }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ObservableCollection<SpeedLinkItemViewModel> Items
        {
            get { return _items; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            BuildItems();
        }

        private void CloseCommandExecuted()
        {
            UtilsPopup.Close(View);
        }

        private void BuildItems()
        {
            SpeedLinkItemViewModel folder = new SpeedLinkItemViewModel();
            folder.Image = "/Count4U.Media;component/Icons/speed_folder.png";

            SpeedLinkItemViewModel arrow = new SpeedLinkItemViewModel();
            arrow.Image = "/Count4U.Media;component/Icons/speed_arrow.png";

            SpeedLinkItemViewModel target = new SpeedLinkItemViewModel();
            target.Image = "/Count4U.Media;component/Icons/speed_target.png";

            SpeedLinkItemViewModel printer = new SpeedLinkItemViewModel();
            printer.Image = "/Count4U.Media;component/Icons/speed_printer.png";

            SpeedLinkItemViewModel network = new SpeedLinkItemViewModel();
            network.Image = "/Count4U.Media;component/Icons/speed_network.png";

            _items.Add(folder);
            _items.Add(arrow);
            _items.Add(target);
            _items.Add(printer);
            _items.Add(network);
        }
    }
}