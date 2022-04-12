using System;
using System.ComponentModel;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.Interfaces
{
    public interface ISearchGridViewModel : INavigationAware
    {
        void Search();
        Action<bool> IsBusy { get; set; }
        SelectParams BuildSelectParams();
        ISearchFieldViewModel SearchFieldViewModel { set; }
        void CanSearch(bool isCanSearch);
    }
}