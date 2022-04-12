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
    public class IturStatusChangeViewModel : CBIContextBaseViewModel
    {
        private readonly IStatusIturRepository _statusIturRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private List<Itur> _iturs;

        public IturStatusChangeViewModel(IContextCBIRepository repository,
            IStatusIturRepository statusIturRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._statusIturRepository = statusIturRepository;

            this._okCommand = new DelegateCommand(OkCommandExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public List<StatusItur> Statuses { get; set; }
        public StatusItur SelectedStatus { get; set; }
        public string ItursNumber { get; set; }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            List<int> numbers = new List<int>();

            string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
            string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

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

            Statuses = this._statusIturRepository.CodeStatusIturDictionary.Values.ToList();
            SelectedStatus = Statuses.FirstOrDefault();

        }

        public override void OnNavigatedFrom(Microsoft.Practices.Prism.Regions.NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

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
                this._eventAggregator.GetEvent<IturStatusChangedEvent>().Publish(
                    new IturStatusChangedEventPayload() {Iturs = Iturs.FromEnumerable(this._iturs), Status = this.SelectedStatus});
            }
        }
    }
}