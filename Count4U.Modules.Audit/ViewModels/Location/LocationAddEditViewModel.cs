using System;
using System.ComponentModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Windows.Media;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class LocationAddEditViewModel : CBIContextBaseViewModel, IDataErrorInfo
    {
        private static readonly Random _rnd = new Random();

        private readonly IEventAggregator _eventAggregator;
        private readonly ILocationRepository _locationRepository;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private Location _location;
        private Locations _locations;

        private string _code;
        private string _name;
        private string _description;
		private string _tag;
        private Color _color;

        private bool _isNewMode;
        private bool _isUnknown;

        public LocationAddEditViewModel(
            IContextCBIRepository contextCBIRepository,
            IEventAggregator eventAggregator,
            ILocationRepository locationRepository)
            : base(contextCBIRepository)
        {
            this._locationRepository = locationRepository;
            this._eventAggregator = eventAggregator;
            this._okCommand = new DelegateCommand(this.OkCommandExecute, this.OkCommandCanExecute);
            this._cancelCommand = new DelegateCommand(this.CancelCommandExecute, this.CancelCommandCanExecute);
        }

        public bool IsOk { get; set; }

     

        public string Description
        {
            get { return this._description; }
            set
            {
                this._description = value;
                RaisePropertyChanged(() => Description);
            }
        }

		public string Tag
		{
			get { return this._tag; }
			set
			{
				this._tag = value;
				RaisePropertyChanged(() => Tag);
			}
		}

        public Color Color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                RaisePropertyChanged(() => Color);
            }
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                RaisePropertyChanged(() => Name);
                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Code
        {
            get { return this._code; }
            set
            {
                this._code = value;
                RaisePropertyChanged(() => Code);

                this._okCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public bool IsNewMode
        {
            get { return this._isNewMode; }
        }

        public bool IsEditMode
        {
            get { return !this._isNewMode; }
        }

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "Code":
                        {
                            string validation = LocationValidate.CodeValidate(this._code);
                            if (!String.IsNullOrEmpty(validation))
                                return validation;

                            if (!this.IsLocationCodeUnique())
                                return Model.ValidateMessage.Location.CodeExist;

                            break;
                        }
                    case "Name":
                        {
                            string validation = LocationValidate.NameValidate(this._name);

                            if (!String.IsNullOrEmpty(validation))
                                return validation;

                            break;
                        }
                }
                return null;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }


        bool IsLocationCodeUnique()
        {
            if (this._isNewMode)
                return !this._locations.Any(r => r.Code.ToUpper() == this.Code.ToUpper());

            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            this._locations = this._locationRepository.GetLocations(base.GetDbPath);

            if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.LocationCode))
            {
                if (navigationContext.Parameters.Any(r => r.Key == Common.NavigationSettings.AddUnknownLocation))
                {
                    this._isUnknown = true;
                    string code = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.LocationCode).Value;
                    this._location = new Location();
                    this._location.Name = String.Empty;
                    this._location.Code = code;     
                    this._location.BackgroundColor = String.Format("{0}, {1}, {2}", _rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255));
                    this._location.Description = String.Empty;
                }
                else
                {
                    string code = navigationContext.Parameters.First(r => r.Key == Common.NavigationSettings.LocationCode).Value;
                    this._location = this._locationRepository.GetLocationByCode(code, base.GetDbPath);
                    this._isNewMode = false;
                }
            }
            else
            {
                Location location = new Location();
                location.Name = String.Empty;
                location.Code = GetNextLocationCode();//String.Empty;       //add next number codeLocation
                
                location.BackgroundColor = String.Format("{0}, {1}, {2}", _rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255));
                location.Description = String.Empty;

                this._location = location;

                this._isNewMode = true;
            }

            this._color = ColorParser.StringToColor(this._location.BackgroundColor);
            this._name = this._location.Name;
            this._description = this._location.Description;
			this._tag = this._location.Tag;
            this.Code = this._location.Code;
        }

        private void OkCommandExecute()
        {
            //this._location.Name = this._name;
            this._location.Name = this._name;

            this._location.Description = this._description;
			this._location.Tag = this._tag;
            this._location.BackgroundColor = ColorParser.ColorToString(this._color);

            if (this.IsNewMode || this._isUnknown)
            {
                this._location.Code = this.Code;

                this._locationRepository.Insert(this._location, base.GetDbPath);
                this._eventAggregator.GetEvent<LocationAddedEvent>().Publish(this._location);
            }
            else
            {
                this._locationRepository.Update(this._location, base.GetDbPath);
                this._eventAggregator.GetEvent<LocationEditedEvent>().Publish(this._location);
            }

            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return !String.IsNullOrWhiteSpace(_code) &&
                   this.IsLocationCodeUnique() &&
                   !String.IsNullOrWhiteSpace(_name);
        }

        private void CancelCommandExecute()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool CancelCommandCanExecute()
        {
            return true;
        }

        private string GetNextLocationCode()
        {
           if (_locations == null )   return "";
            for (int i=1; i < 999; i++ )
			{
                string newLocationCode = i.ToString().PadLeft(3, '0');
                Location location = _locations.FirstOrDefault(e => e.Code.CompareTo(newLocationCode) == 0);
                if(location == null) 
                   return  newLocationCode;
                else 
                  continue;  
			}

            return "";
        }

    }
}