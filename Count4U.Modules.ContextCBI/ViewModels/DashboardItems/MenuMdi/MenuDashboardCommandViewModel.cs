using System;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.DashboardItems.Menu
{
    public class MenuDashboardCommandViewModel : NotificationObject
    {
        private string _image;
        private bool _isVisible;
        private Color _backgroundColor;

        public MenuDashboardCommandViewModel()
        {
            IsVisible = true;
            _backgroundColor = Colors.Transparent;
        }

        public string Name { get; set; }
        public string PartName { get; set; }
        public string DashboardName { get; set; }
        public DelegateCommand Command { get; set; }

        public int SortIndexOriginal { get; set; }
        public int SortIndex { get; set; }

        public string Key
        {
            get { return string.Format("{0}{1}{2}", DashboardName, PartName, Name); }
        }

        public string Image
        {
            get { return String.Format(@"/Count4U.Media;component/Icons/{0}.png", _image); ; }
            set { _image = value; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                RaisePropertyChanged(() => BackgroundColor);
            }
        }
    }
}