using Count4U.Model.Main;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Adapters
{
    public class AdapterLinkItemViewModel
    {
        private readonly ImportAdapter _importAdapter;

        public AdapterLinkItemViewModel(ImportAdapter importAdapter)
        {
            _importAdapter = importAdapter;
        }

        public string Name { get; set; }
        public string AdapterCode { get; set; }
        public string DomainType { get; set;  }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsAdapterExistInFs { get; set; }

        public ImportAdapter ImportAdapter
        {
            get { return _importAdapter; }
        }
    }
}