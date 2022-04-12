using System;
using System.Collections.Generic;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Interface.Count4U;
using System.Linq;
using Count4U.Model.Count4U;

namespace Count4U.Modules.Audit.ViewModels
{
    public class IturNameChangeViewModel : CBIContextBaseViewModel
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private List<Itur> _iturs;

        private Location _selectedLocation;
		private string _iturName;
		private string _iturERPCode;

		public IturNameChangeViewModel(IContextCBIRepository repository,
            ILocationRepository locationRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._locationRepository = locationRepository;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

		public string IturCode { get; set; }

		public string IturName
		{
			get { return this._iturName;  }
			set
			{
				this._iturName = value;
				RaisePropertyChanged(() => IturName);

				_okCommand.RaiseCanExecuteChanged();
			}
		}


		public string IturERPCode
		{
			get { return this._iturERPCode; }
			set
			{
				this._iturERPCode = value;
				RaisePropertyChanged(() => IturERPCode);

				_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string ItursNumber { get; set; }

		//public Locations Locations { get; set; }

		//public Location SelectedLocation
		//{
		//	get { return _selectedLocation; }
		//	set
		//	{
		//		_selectedLocation = value;
		//		RaisePropertyChanged(() => SelectedLocation);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

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

            string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
            string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

            List<int> itursNumbers = new List<int>();

            this._iturs = new List<Itur>();
            foreach (string iturCode in split)
            {
                Itur itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);
				this.IturCode = iturCode;
				this.IturName = itur.Name;
				this.IturERPCode = itur.ERPIturCode;
				if (itur != null)
				{
					itursNumbers.Add(itur.Number);
					this._iturs.Add(itur);
				}
            }

            this.ItursNumber = CommaDashStringParser.Reverse(itursNumbers);
			
			//this.Locations = this._locationRepository.GetLocations(base.GetDbPath);

			//string LocationCode = this._locationRepository.GetFistLocationCodeWithoutIturs(base.GetDbPath);
			//if (string.IsNullOrWhiteSpace(LocationCode) == false)
			//{
			//	this.SelectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			//}
			//else
			//{
			//	this.SelectedLocation = this.Locations.FirstOrDefault();
			//}
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
                this._eventAggregator.GetEvent<IturNameChangedEvent>().Publish(
                   // new IturLocationChangedEventPayload { Iturs = Iturs.FromEnumerable(this._iturs), Location = SelectedLocation }
					new IturNameChangedEventPayload { Iturs = Iturs.FromEnumerable(this._iturs), Name = IturName, ERPCode = IturERPCode }
                    );
            }
        }

        private bool OkCommandCanExecuted()
        {
            //return string.IsNullOrWhiteSpace(IturName) != true;
			return true;
        }
    }
}