using System.ComponentModel;
using Count4U.Common.Constants;
using Count4U.Common.ViewModel.Filter.Data;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.Interfaces
{
    public interface ISearchFieldViewModel : INavigationAware, INotifyPropertyChanged
    {
        IFilterData BuildFilterData();
		IFilterData BuildFilterSelectData(string selectedCode);
        void Reset();
        void ApplyFilterData(IFilterData data);
        DelegateCommand SearchCommand { get; set; }
        bool CanSearch();
        ViewDomainContextEnum GetReportContext();
    }
}