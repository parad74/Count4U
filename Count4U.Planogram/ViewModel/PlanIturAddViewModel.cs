using System.ComponentModel;
using System.Linq;
using System.Windows;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;

namespace Count4U.Planogram.ViewModel
{
    public class PlanIturAddViewModel : NotificationObject, INavigationAware, IDataErrorInfo, IChildWindowViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private string _prefix;
        private string _numbers;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;


        public PlanIturAddViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public string Numbers
        {
            get { return _numbers; }
            set
            {
                _numbers = value;
                RaisePropertyChanged(() => Numbers);

                _okCommand.RaiseCanExecuteChanged();
            }
        }


        public string Prefix
        {
            get { return _prefix; }
            set
            {
                _prefix = value;
                RaisePropertyChanged(() => Prefix);

                _okCommand.RaiseCanExecuteChanged();
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
            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.IturPrefix))
                _prefix = navigationContext.Parameters[Common.NavigationSettings.IturPrefix];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Prefix":
                        if (!IsPrefixOk())
                        {
                            return Localization.Resources.ViewModel_ExportErpWithModules_invalidPrefix;
                        }
                        break;
                    case "Numbers":
                        if (IsNumbersOk() == false)
                        {
                            return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
                        }
                        break;
                }

                return String.Empty;
            }
        }

        public string Error { get; private set; }
        public object ResultData { get; set; }

        private bool IsNumbersOk()
        {
            return CommaDashStringParser.IsValid(_numbers);
        }

        private bool IsPrefixOk()
        {
            if (String.IsNullOrWhiteSpace(_prefix))
                return true;

            int dummy;
            return Int32.TryParse(_prefix, out dummy);
        }

        private void CancelCommandExecuted()
        {
            ResultData = null;
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return String.IsNullOrWhiteSpace(_numbers) == false && IsNumbersOk()
                && String.IsNullOrWhiteSpace(_prefix) == false && IsPrefixOk();
        }

        private void OkCommandExecuted()
        {
            int prefix = Int32.Parse(_prefix);
            List<int> numbers = CommaDashStringParser.Parse(_numbers);

            ResultData = new Tuple<int, List<int>>(prefix, numbers);

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}