using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;

namespace Count4U.Modules.Audit.ViewModels
{
    public class LocationListViewModelBuilder
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;

        public LocationListViewModelBuilder(
            IContextCBIRepository contextCbiRepository,
            ILocationRepository locationRepository,
            IIturRepository iturRepository)
        {
            _iturRepository = iturRepository;
            _locationRepository = locationRepository;
        }

		//Func<T, TResult>
		//public delegate TResult Func<in T, out TResult>(T arg)
		public void Build(ObservableCollection<LocationItemViewModel> items, string dbPath, SelectParams selectParams,
			Func<Location, bool> getIsChecked, Func<Location, bool> getIsAddVisible)
        {
			Locations locations = _locationRepository.GetLocations(selectParams, dbPath);
			//Locations locationsByIturs = _iturRepository.GetLocationList(dbPath);
			
			//foreach (Location location in locationsByIturs)
			//{
			//	Location realLocation = locations.FirstOrDefault(r => r.Code == location.Code);
			//	if (realLocation == null)
			//	{
			//		location.BackgroundColor = DefaultColors.EmptyLocationColor();
			//		location.Name = String.IsNullOrWhiteSpace(location.Name) ? location.Code : location.Name;

			//		locations.Add(location);
			//	}
			//}

            foreach (Location location in locations)
            {
                LocationItemViewModel viewModel = new LocationItemViewModel(location);
                viewModel.IsChecked = getIsChecked(location);
                viewModel.IsAddVisible = getIsAddVisible(location);
                items.Add(viewModel);
            }
        }
    }
}