using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;
using System;

namespace Count4U.Modules.Audit.ViewModels.Supplier
{
    public class SupplierAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly ISupplierRepository _supplierRepository;

        private Count4U.Model.Count4U.Supplier _supplier;

        private string _code;
        private string _name;
        private string _description;
        private bool _isNew;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private string[] _supplierCodes;

        public SupplierAddEditViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            ISupplierRepository supplierRepository
            )
            : base(contextCbiRepository)
        {
            _supplierRepository = supplierRepository;
            this._eventAggregator = eventAggregator;

            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public object ResultData { get; set; }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public bool IsNew
        {
            get { return _isNew; }
        }

        public bool IsEdit
        {
            get { return !_isNew; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.SupplierCode))
            {
                string supplierCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.SupplierCode).Value;

                _supplier = _supplierRepository.GetSupplierByCode(supplierCode, base.GetDbPath);

                _code = _supplier.SupplierCode;
                _name = _supplier.Name;
                _description = _supplier.Description;

                _isNew = false;
            }
            else
            {
                _supplier = new Model.Count4U.Supplier();

                _isNew = true;
            }

            _supplierCodes = _supplierRepository.GetSuppliers(base.GetDbPath).Select(r=>r.SupplierCode).ToArray();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Code":
                        if (this._isNew)
                        {
                            if (String.IsNullOrWhiteSpace(_code))
                                return String.Empty;

                            return IsCodeOk();
                        }
                        break;
                }
                return string.Empty;
            }
        }

        private string IsCodeOk()
        {
            if (_supplierCodes.Any(r => r == _code))
                return Localization.Resources.View_SupplierAddEdit_CodeExist;

            if (!Regex.IsMatch(_code, "^[A-Za-z0-9]+$"))
                return Localization.Resources.ViewModel_SupplierAddEdit_CodeFormat;

            return String.Empty;
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private void CancelCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            if (_isNew)
                return !String.IsNullOrWhiteSpace(_code) && String.IsNullOrEmpty(IsCodeOk()) && !String.IsNullOrWhiteSpace(_name);

            return true;
        }

        private void OkCommandExecuted()
        {
            try
            {
                _supplier.Name = _name;
                _supplier.Description = _description;

                if (_isNew)
                {
                    _supplier.SupplierCode = _code;
                    _supplierRepository.Insert(_supplier, base.GetDbPath);
                }
                else
                {
                    _supplierRepository.Update(_supplier, base.GetDbPath);
                }

                ResultData = _supplier;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}