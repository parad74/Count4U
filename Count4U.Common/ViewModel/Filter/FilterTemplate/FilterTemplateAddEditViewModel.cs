using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.Filter.FilterTemplate
{
    public class FilterTemplateAddEditViewModel : NotificationObject, IChildWindowViewModel, INavigationAware, IDataErrorInfo
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IFilterTemplateRepository _filterTemplateRepository;
        private readonly INavigationRepository _navigationRepository;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private string _displayName;
        private string _fileName;

        private List<string> _templateList;

        private bool _isNew;
        private string _originalName;

        public FilterTemplateAddEditViewModel(
            IEventAggregator eventAggregator,
            IUserSettingsManager userSettingsManager,
            IFilterTemplateRepository filterTemplateRepository,
            INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
            _filterTemplateRepository = filterTemplateRepository;
            this._userSettingsManager = userSettingsManager;
            this._eventAggregator = eventAggregator;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public object ResultData { get; set; }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;

                RaisePropertyChanged(() => DisplayName);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;

                RaisePropertyChanged(() => FileName);

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
            object objectFromNavigation = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, isRemove: true);

            FilterTemplateAddEditDTO dto = objectFromNavigation as FilterTemplateAddEditDTO;
            if (dto == null) return;

            string context = dto.Context;
            string displayName = dto.DisplayName;
            string fileName = dto.FileName;

            if (String.IsNullOrEmpty(context) == false)
            {
                _templateList = _filterTemplateRepository.GetFiles(context).Select(r => Path.GetFileNameWithoutExtension(r.FullName)).ToList();
            }
            else
            {
                _templateList = new List<string>();
            }

            if (String.IsNullOrEmpty(fileName) == false)
            {
                _fileName = fileName;
                _originalName = fileName;
                _displayName = displayName;
                _isNew = false;
            }
            else
            {
                _isNew = true;
            }
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
            this.ResultData = new FilterTemplateAddEditDTO()
            {
                Context = String.Empty,
                DisplayName = _displayName,
                FileName = _fileName,
            };
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            if (String.IsNullOrWhiteSpace(_displayName))
                return false;

            if (String.IsNullOrWhiteSpace(_fileName))
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
                    case "FileName":
                        if (String.IsNullOrWhiteSpace(_fileName))
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
            if (this._fileName.Any(r => invalidChars.Contains(r)))
                return Localization.Resources.Msg_NotValidFileName;

            if (_isNew)
            {
                if (this._templateList.Any(r => r.ToLower() == this._fileName.ToLower()))
                    return Localization.Resources.Msg_FilterTemplateExists;
            }
            else
            {
                if (_fileName != _originalName && this._templateList.Any(r => r.ToLower() == this._fileName.ToLower()))
                    return Localization.Resources.Msg_FilterTemplateExists;
            }

            return String.Empty;
        }
    }
}