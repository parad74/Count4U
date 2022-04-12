using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Linq;

namespace Count4U.Modules.ContextCBI.ViewModels.Settings
{
    public class ConfigurationSetAddViewModel : NotificationObject, IChildWindowViewModel, INavigationAware, IDataErrorInfo
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private string _name;

        private List<string> _configNames;

        public ConfigurationSetAddViewModel(IEventAggregator eventAggregator, IUserSettingsManager userSettingsManager)
        {
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator = eventAggregator;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public object ResultData { get; set; }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                RaisePropertyChanged(() => Name);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _configNames = this._userSettingsManager.AdminListConfiguration().Select(r => r.Replace(FileSystem.ConfigSetFileExtension, String.Empty)).ToList();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private void OkCommandExecuted()
        {
            this.ResultData = this._name;
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            if (String.IsNullOrWhiteSpace(_name))
                return false;

            return String.IsNullOrEmpty(this.IsFileNameValid());
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Name":
                        if (String.IsNullOrWhiteSpace(_name))
                            return String.Empty;

                        return IsFileNameValid();

                }
                return String.Empty;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        private string IsFileNameValid()
        {           
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (this._name.Any(r => invalidChars.Contains(r)))
                return Localization.Resources.Msg_NotValidFileName;

            if (this._configNames.Any(r => r.ToLower() == this._name.ToLower()))
                return Localization.Resources.Msg_ConfigurationSetExist;

            return String.Empty;
        }
    }
}