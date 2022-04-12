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
    public class IturPrefixChangeViewModel : CBIContextBaseViewModel
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
        private List<Itur> _iturs;

		private string _prefixNew;
		//private string _itursNumber;
		private string _textValue;
		private bool _includeAllIturs;        

		public IturPrefixChangeViewModel(IContextCBIRepository repository,
            ILocationRepository locationRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._locationRepository = locationRepository;
			this._includeAllIturs = false;
            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

		//public string ItursNumber
		//{
		//	get { return this._itursNumber; }
		//	set
		//	{
		//		this._itursNumber = value;
		//		RaisePropertyChanged(() => ItursNumber);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}


		public string PrefixNew
		{
			get { return _prefixNew; }
			set
			{
				this._prefixNew = value;
				RaisePropertyChanged(() => PrefixNew);

				_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string TextValue
		{
			get { return this._textValue; }
			set
			{
				this._textValue = value;
				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.TotalIturs);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

		public string TotalIturs
		{
			get
			{
				List<int> res = CommaDashStringParser.Parse(this._textValue);
				return res == null ? "0" : res.Count.ToString();
			}
		}

		private bool IsTextValid()
		{
			return CommaDashStringParser.IsValid(this._textValue);
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
								return String.Format(Localization.Resources.ViewModel_IturAdd_Expression, Environment.NewLine, Environment.NewLine);
							}
						}
						break;
					case "PrefixNew":
						{
							int bit = this._prefixNew.PrefixValidate();
							if (bit != 0)
							{
								return IturValidate.Bit2PrefixErrorMessage(bit);
							}
						}
						//if (IsPrefixValid() == false)
						//{
						//    return "Prefix is not empty numeric string 4 characters maximum";
						//}

						break;
				}
				return null;
			}
		}

		public bool Disable
		{
			get { return !IncludeAllIturs; }
		}

		public bool IncludeAllIturs
		{
			get { return this._includeAllIturs; }
			set
			{
				this._includeAllIturs = value;
				if (_includeAllIturs == true)
				{
					this._textValue = "1-9999";
				}
				else
				{
					this._textValue = "1";
				}
				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.TotalIturs);

				this.RaisePropertyChanged(() => this.IncludeAllIturs);

				this._okCommand.RaiseCanExecuteChanged();
				
			}
		}

		public string Error
		{
			get { return string.Empty; }
		}

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
				if (itur != null)
				{
					itursNumbers.Add(itur.Number);
					this._iturs.Add(itur);
				}
            }

			this.TextValue = CommaDashStringParser.Reverse(itursNumbers);
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
			List<int> itursNumberList = CommaDashStringParser.Parse(this._textValue);
			List<int> itursNumberListDistinct = itursNumberList.Distinct().ToList();
			Iturs itursAll = new Iturs();
			
			using (new CursorWait())
			{
				if (this.IncludeAllIturs == false)
				{
					foreach (int iturNumber in itursNumberListDistinct)
					{
						var iturs = this._iturRepository.GetItursByNumber(iturNumber, base.GetDbPath);
						itursAll.AddEnumerableIturs(iturs);
					}
				}

              this._eventAggregator.GetEvent<IturPrefixChangedEvent>().Publish(
				new IturPrefixChangedEventPayload { Iturs = itursAll, PrefixNew = this.PrefixNew, AllChange = this.IncludeAllIturs}
              );
            }
        }

        private bool OkCommandCanExecuted()
        {
			bool can = (string.IsNullOrWhiteSpace(this._textValue) == false
				&& (this.IsTextValid() == true)
				&& (this._prefixNew.PrefixValidate() == 0));
			return can;
        }
    }
}