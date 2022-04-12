using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturStateChangeViewModel : CBIContextBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IIturRepository _iturRepository;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;

        private List<Itur> _iturs;

        private bool? _isDisabled;

        public IturStateChangeViewModel(
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IIturRepository iturRepository)
            : base(contextCbiRepository)
        {
            _iturRepository = iturRepository;
            _eventAggregator = eventAggregator;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }       

        public string ItursNumber { get; set; }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public bool? IsDisabled
        {
            get { return _isDisabled; }
            set
            {
                _isDisabled = value;
                RaisePropertyChanged(() => IsDisabled);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            using (new CursorWait())
            {
                string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
                string[] split = commaSeparatedNumbers.Split(new char[] {','}).Where(r => !string.IsNullOrEmpty(r)).ToArray();

                List<int> itursNumbers = new List<int>();

                this._iturs = new List<Itur>();
                foreach (string iturCode in split)
                {
                    Itur itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);
					if (itur != null)
					{
						itursNumbers.Add(itur.Number);
						this._iturs.Add(itur);
					}
                }

                this.ItursNumber = CommaDashStringParser.Reverse(itursNumbers);

                if (this._iturs.All(r => r.Disabled == true))
                    IsDisabled = true;
                else if (this._iturs.All(r => r.Disabled == false || r.Disabled == null))
                    IsDisabled = false;
                else
                    IsDisabled = null;
            }
        }

        private bool OkCommandCanExecute()
        {
            return this._isDisabled != null;
        }

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);

            using (new CursorWait())
            {
                this._eventAggregator.GetEvent<IturStateChangedEvent>().Publish(
                    new ItursStateChangedEventPayload() { Iturs = Iturs.FromEnumerable(this._iturs), Disabled = (bool)this._isDisabled });
            }
        }
    }
}