using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings.PathSettings
{
    public class PathSettingsItemViewModel : NotificationObject
    {
        private string _name;
        private string _path;
        private DelegateCommand _openCommand;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public DelegateCommand OpenCommand
        {
            get { return _openCommand; }
            set { _openCommand = value; }
        }
    }
}