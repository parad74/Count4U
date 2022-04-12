using System;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Planogram.ViewModel
{
    public class PlanLocationAssignViewModel : CBIContextBaseViewModel, IChildWindowViewModel
    {
        private const string UnassignedLocationCode = "Unassigned";

        private readonly INavigationRepository _navigationRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly ObservableCollection<PlanLocationItemViewModel> _items;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        public PlanLocationAssignViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            ILocationRepository locationRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _locationRepository = locationRepository;
            _navigationRepository = navigationRepository;

            _items = new ObservableCollection<PlanLocationItemViewModel>();

            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }        

        public ObservableCollection<PlanLocationItemViewModel> Items
        {
            get { return _items; }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            PlanAssignedLocations assignedLocations = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, null, true) as PlanAssignedLocations;

            if (assignedLocations != null)
            {
                if (String.IsNullOrWhiteSpace(assignedLocations.CurrentLocationCode))
                {
                    assignedLocations.CurrentLocationCode = UnassignedLocationCode;
                }
                this.Build(assignedLocations);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void Build(PlanAssignedLocations assignedLocations)
        {
            _items.Clear();
            _items.Add(new PlanLocationItemViewModel(
                new Location()
                    {
                        Code = UnassignedLocationCode,
                        Name = Localization.Resources.ViewModel_PlanLocation_Assign_EmptyLocation,
                    })                    
                    {
                        IsSelected = assignedLocations.CurrentLocationCode == UnassignedLocationCode
                    }
            );

            Locations locations = _locationRepository.GetLocations(base.GetDbPath);
            foreach (Location location in locations)
            {               
                PlanLocationItemViewModel item = new PlanLocationItemViewModel(location);
                if (assignedLocations.CurrentLocationCode == location.Code)
                {
                    item.IsSelected = true;
                    _items.Add(item);
                }
                else
                {
//                    if (assignedLocations.AssignedLocationCodes.Contains(location.Code))
//                    {
//                        continue;
//                    }

                    _items.Add(item);
                }
            }

            foreach (PlanLocationItemViewModel item in _items)
            {
                item.IsSelectedChanged += Item_IsSelectedChanged;
            }
        }

        void Item_IsSelectedChanged(object sender, System.EventArgs e)
        {
            _okCommand.RaiseCanExecuteChanged();
        }

        private void CancelCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return _items.Any(r => r.IsSelected);
        }

        private void OkCommandExecuted()
        {
            string locationCode = String.Empty;

            PlanLocationItemViewModel item = _items.FirstOrDefault(r => r.IsSelected == true);

            if (item != null)
            {
                locationCode = item.Code;
            }

            if (locationCode == UnassignedLocationCode)
            {
                locationCode = String.Empty;
            }

            this.ResultData = locationCode;
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        public object ResultData { get; set; }
    }
}