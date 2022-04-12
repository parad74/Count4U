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

namespace Count4U.Modules.Audit.ViewModels.Family
{
    public class FamilyAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
		private readonly IFamilyRepository _familyRepository;

		private Count4U.Model.Count4U.Family _family;

        private string _code;
        private string _name;
		private string _type;
		private string _size;
		private string _extra1;
		private string _extra2;
        private string _description;
        private bool _isNew;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private string[] _familyCodes;

		public FamilyAddEditViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
			IFamilyRepository familyRepository
            )
            : base(contextCbiRepository)
        {
			this._familyRepository = familyRepository;
            this._eventAggregator = eventAggregator;

			this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
			this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
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

		public string Type
		{
			get { return _type; }
			set
			{
				_type = value;
				RaisePropertyChanged(() => Type);

				//_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string Size
		{
			get { return _size; }
			set
			{
				_size = value;
				RaisePropertyChanged(() => Size);

				//_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string Extra1
		{
			get { return _extra1; }
			set
			{
				_extra1 = value;
				RaisePropertyChanged(() => Extra1);

				//_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string Extra2
		{
			get { return _extra2; }
			set
			{
				_extra2 = value;
				RaisePropertyChanged(() => Extra2);

				//_okCommand.RaiseCanExecuteChanged();
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

			if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.FamilyCode))
            {
				string familyCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.FamilyCode).Value;

				_family = _familyRepository.GetFamilyByCode(familyCode, base.GetDbPath);

				_code = _family.FamilyCode;
                _name = _family.Name;
				_type = _family.Type;
				_size = _family.Size;
				_extra1 = _family.Extra1;
				_extra2 = _family.Extra2;
                _description = _family.Description;

                _isNew = false;
            }
            else
            {
				_family = new Model.Count4U.Family();

                _isNew = true;
            }

			_familyCodes = _familyRepository.GetFamilys(base.GetDbPath).Select(r => r.FamilyCode).ToArray();
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
            if (_familyCodes.Any(r => r == _code))
				return Localization.Resources.View_FamilyAddEdit_CodeExist;

            if (!Regex.IsMatch(_code, "^[A-Za-z0-9]+$"))
				return Localization.Resources.ViewModel_FamilyAddEdit_CodeFormat;

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
                _family.Name = _name;
                _family.Description = _description;
				_family.Type = _type;
				_family.Size = _size;
				_family.Extra1 = _extra1;
				_family.Extra2 = _extra2;

                if (_isNew)
                {
                    _family.FamilyCode = _code;
                    _familyRepository.Insert(_family, base.GetDbPath);
                }
                else
                {
                    _familyRepository.Update(_family, base.GetDbPath);
                }

                ResultData = _family;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}