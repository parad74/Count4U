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
using Count4U.Model.Count4U.Validate;

namespace Count4U.Modules.Audit.ViewModels
{
	public class IturTagChangeViewModel : CBIContextBaseViewModel
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
		private Iturs _iturs;

       // private Location _selectedLocation;

		private string _textValue;
		//private bool _includeAllIturs;        

		public IturTagChangeViewModel(IContextCBIRepository repository,
            ILocationRepository locationRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._locationRepository = locationRepository;
			//this._includeAllIturs = false;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        //public string ItursNumber { get; set; }

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

		public string TextValue//*
		{
			get { return this._textValue; }
			set
			{
				this._textValue = value;
				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.TotalIturs);
				this.RaisePropertyChanged(() => this.IturCodes);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

	
		public string TotalIturs
		{
			get
			{
				if (_iturs == null) return "0";
				return _iturs.Count.ToString();
			}
		}

		public string IturCodes
		{
			get
			{
				if (_iturs == null) return "";

				string[] iturCodes = _iturs.Select(x => x.IturCode).ToArray();
				return iturCodes.JoinRecord(",");
			}
		}

		private bool IsTextValid() 
		{
			   string validation = CustomerValidate.CodeValidate(this._textValue);
			   if (String.IsNullOrEmpty(validation) == true) return true;
			   else return false;
		}

		public string this[string propertyName] 
		{
			get
			{
				switch (propertyName)
				{
					case "TextValue":
						{
							if (this.IsTextValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_LocationTag_Expression, Environment.NewLine, Environment.NewLine);
							}
						}
			
						break;
				}
				return null;
			}
		}

	
		public string Error
		{
			get { return string.Empty; }
		}

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

		   // string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
		   // string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

			List<string> iturcodes = new List<string>();

			this._iturs = new Iturs();
			foreach (string iturCode in split)
			{
				Itur itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);
				if (itur != null)
				{
				//	itursNumbers.Add(location.Number);
					this._iturs.Add(itur);
				}
			}

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
		
				this._eventAggregator.GetEvent<IturTagChangedEvent>().Publish(
					//new IturLocationChangedEventPayload { Iturs = Iturs.FromEnumerable(this._iturs), Location = SelectedLocation, AllChange = this.IncludeAllIturs }
					new IturTagChangedEventPayload { Iturs = this._iturs, Tag = this.TextValue }
                    );
            }
        }

        private bool OkCommandCanExecuted()
        {
			bool can = ((string.IsNullOrWhiteSpace(this._textValue) == false)//*
				&& (this.IsTextValid() == true)
				&&  (this._iturs != null)
				&& (this._iturs.Count > 0)); 
            return can; 
        }
    }
}