using System;
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

namespace Count4U.Modules.Audit.ViewModels.Section
{
    public class SectionAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo, IChildWindowViewModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _eventAggregator;
        private readonly ISectionRepository _sectionRepository;

        private Count4U.Model.Count4U.Section _section;

        private string _code;
        private string _name;
		private string _tag;
        private string _description;
        private bool _isNew;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;
        private string[] _sectionCodes;

        public SectionAddEditViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            ISectionRepository sectionRepository
            )
            : base(contextCbiRepository)
        {
            _sectionRepository = sectionRepository;
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

		public string Tag
		{
			get { return _tag; }
			set
			{
				_tag = value;
				RaisePropertyChanged(() => Tag);

				_okCommand.RaiseCanExecuteChanged();
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

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.SectionCode))
            {
                string sectionCode = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.SectionCode).Value;

                _section = _sectionRepository.GetSectionByCode(sectionCode, base.GetDbPath);

                _code = _section.SectionCode;
                _name = _section.Name;
				_tag = _section.Tag;
                _description = _section.Description;

                _isNew = false;
            }
            else
            {
                _section = new Model.Count4U.Section();

                _isNew = true;
            }

            _sectionCodes = _sectionRepository.GetSectionCodes(base.GetDbPath);
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
            if (_sectionCodes.Any(r => r == _code))
                return Localization.Resources.View_SectionAddEdit_CodeExist;

            if (!Regex.IsMatch(_code, "^[A-Za-z0-9]+$"))
                return Localization.Resources.ViewModel_SectionAddEdit_CodeFormat;

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
                _section.Name = _name;
                _section.Description = _description;
				_section.Tag = _tag;

                if (_isNew)
                {
                    _section.SectionCode = _code;
                    _sectionRepository.Insert(_section, base.GetDbPath);
                }
                else
                {
                    _sectionRepository.Update(_section, base.GetDbPath);
                }

                ResultData = _section;
            }
            catch (Exception exc)
            {
                _logger.ErrorException("OkCommandExecuted", exc);
            }

            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }
    }
}