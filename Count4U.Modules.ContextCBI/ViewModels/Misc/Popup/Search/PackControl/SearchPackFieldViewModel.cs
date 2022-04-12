using System;
using Count4U.Common.Constants;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Common.ViewModel.Filter.Data;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Commands;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.PackControl
{
    public class SearchPackFieldViewModel : CBIContextBaseViewModel, ISearchFieldViewModel
    {
        private string _code;
        private string _name;
        private DateTime? _from;
        private DateTime? _to;

        private DelegateCommand _searchCommand;

        public SearchPackFieldViewModel(IContextCBIRepository contextCbiRepository)
            : base(contextCbiRepository)
        {
			_code = String.Empty;
			_name = String.Empty;

        }

        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged(() => Code);
            }
        }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public DelegateCommand SearchCommand
        {
            get { return _searchCommand; }
            set { _searchCommand = value; }
        }

        public DateTime? From
        {
            get { return _from; }
            set
            {
                _from = value;
                RaisePropertyChanged(()=>From);
            }
        }

        public DateTime? To
        {
            get { return _to; }
            set
            {
                _to = value;
                RaisePropertyChanged(() => To);
            }
        }

        public bool CanSearch()
        {
            return true;
        }

        public ViewDomainContextEnum GetReportContext()
        {
            throw new InvalidOperationException();
        }
     
        public IFilterData BuildFilterData()
        {
            PackFilterData result = new PackFilterData();

            result.Code = _code;
            result.Name = _name;
            result.From = _from;
            result.To = _to;

            return result;
        }


		public IFilterData BuildFilterSelectData(string selectedCode) //TODO
		{
			PackFilterData result = new PackFilterData();

			result.Code = _code;
			result.Name = _name;
			result.From = _from;
			result.To = _to;

			return result;
		}

        public void ApplyFilterData(IFilterData data)
        {
            PackFilterData packData = data as PackFilterData;

            if (packData == null) return;

            Code = packData.Code;
            Name = packData.Name;
            From = packData.From;
            To = packData.To;
        }

        public void Reset()
        {
            Code = String.Empty;
            Name = String.Empty;

            From = null;
            To = null;
        }
    }
}